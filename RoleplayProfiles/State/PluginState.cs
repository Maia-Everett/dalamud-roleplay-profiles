using Dalamud.Logging;
using RestSharp;
using RoleplayProfiles.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoleplayProfiles.State
{
    public class PluginState: IDisposable
    {
        private readonly Dictionary<Player, ProfileCacheEntry> profileCache = new();

        public ApiClient ApiClient { get; init; } = new();
        public Configuration Configuration { get; init; }

        public Player? TargetPlayer { get; set; } = null;
        public Player? ProfilePlayer { get; set; } = null;
        public bool TargetPlayerSelected { get; set; } = false;

        public PluginState(Configuration configuration)
        {
            this.Configuration = configuration;
        }

        public ProfileCacheEntry GetProfile(Player player)
        {
            profileCache.TryGetValue(player, out var cacheEntry);

            if (cacheEntry != null && cacheEntry.State != CacheEntryState.Failed)
            {
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
                var profile = await ApiClient.GetProfile(player.Name, player.Server);
                
                if (profile != null)
                {
                    PluginLog.Information("Succeeded!");
                    cacheEntry.State = CacheEntryState.Retrieved;
                    cacheEntry.Data = profile;
                }
                else
                {
                    PluginLog.Information("Not found!");
                    cacheEntry.State = CacheEntryState.NotFound;
                    cacheEntry.Data = null;
                }
            }
            catch (Exception)
            {
                PluginLog.Information("Failed!");
                cacheEntry.State = CacheEntryState.Failed;
                cacheEntry.Data = null;
            }
        }

        public void Dispose()
        {
            profileCache.Clear();
        }
    }
}
