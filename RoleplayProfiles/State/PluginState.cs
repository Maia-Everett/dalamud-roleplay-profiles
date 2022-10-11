using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Logging;
using RestSharp;
using RoleplayProfiles.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RoleplayProfiles.State
{
    public class PluginState: IDisposable
    {
        private static readonly TimeSpan ProfileCacheDuration = TimeSpan.FromMinutes(5);

        private readonly Dictionary<Player, ProfileCacheEntry> profileCache = new();
        private readonly ConditionalWeakTable<PlayerCharacter, Player> playerCache = new();

        public ApiClient ApiClient { get; init; } = new();
        public Configuration Configuration { get; init; }

        public Player? TargetPlayer { get; set; } = null;
        public Player? ProfilePlayer { get; set; } = null;
        public bool TargetPlayerSelected { get; set; } = false;

        private readonly ClientState clientState;

        public PluginState(Configuration configuration, ClientState clientState)
        {
            this.Configuration = configuration;
            this.clientState = clientState;

            ApiClient.OnCharacterUpdated += player =>
            {
                if (profileCache.TryGetValue(player, out var cacheEntry) && cacheEntry.State == CacheEntryState.Retrieved)
                {
                    // Invalidate cache for a single character
                    cacheEntry.State = CacheEntryState.Invalidated;
                }
            };

            ApiClient.OnDisconnected += () =>
            {
                // Disconnection from the WebSocket server invalidates the entire cache
                foreach (var cacheEntry in profileCache.Values)
                {
                    if (cacheEntry.State == CacheEntryState.Retrieved)
                    {
                        cacheEntry.State = CacheEntryState.Invalidated;
                    }
                }
            };
        }

        public ProfileCacheEntry GetProfile(Player player)
        {
            profileCache.TryGetValue(player, out var cacheEntry);

            if (cacheEntry != null && cacheEntry.State != CacheEntryState.Failed)
            {
                // Check invalidated flag
                if (cacheEntry.State == CacheEntryState.Invalidated)
                {
                    PluginLog.Information("Cache expired - reloading profile");
                    cacheEntry.State = CacheEntryState.Updating;
                    _ = GetProfileInternal(player); // Retrieve profile asynchronously
                }

                return cacheEntry;
            }

            var newCacheEntry = new ProfileCacheEntry();
            profileCache[player] = newCacheEntry;
            _ = GetProfileInternal(player); // Retrieve profile asynchronously
            return newCacheEntry;
        }

        private async Task GetProfileInternal(Player player)
        {
            var cacheEntry = profileCache[player];

            try
            {
                var profile = await ApiClient.GetProfile(player);
                
                if (profile != null)
                {
                    PluginLog.Information("Succeeded!");
                    cacheEntry.State = CacheEntryState.Retrieved;
                    cacheEntry.Data = profile;
                }
                else
                {
                    cacheEntry.State = CacheEntryState.NotFound;
                    cacheEntry.Data = null;
                }
            }
            catch (Exception)
            {
                if (cacheEntry.State == CacheEntryState.Updating)
                {
                    // Retry later
                    cacheEntry.State = CacheEntryState.Invalidated;
                }
                else
                {
                    cacheEntry.State = CacheEntryState.Failed;
                    cacheEntry.Data = null;
                }
            }
        }

        public async Task SaveProfile(Player player, Profile profile)
        {
            if (Configuration.AccessToken == null)
            {
                return;
            }

            var cacheEntry = profileCache[player];

            if (cacheEntry == null)
            {
                return;
            }

            await ApiClient.UpdateProfile(player, profile, Configuration.AccessToken);
            cacheEntry.Data = profile;
        }

        public Player ToPlayer(PlayerCharacter character)
        {
            playerCache.TryGetValue(character, out var player);

            if (player != null)
            {
                return player;
            }

            var name = character.Name.ToString();
            var server = character.HomeWorld.GameData!.Name;
            var newPlayer = new Player(name, server);
            playerCache.Add(character, newPlayer);
            return newPlayer;
        }

        public Player? GetCurrentPlayer()
        {
            var localPlayer = clientState.LocalPlayer!;
            return localPlayer ? ToPlayer(localPlayer) : null;
        }

        public void Dispose()
        {
            profileCache.Clear();
            playerCache.Clear();
            ApiClient.Dispose();
        }
    }
}
