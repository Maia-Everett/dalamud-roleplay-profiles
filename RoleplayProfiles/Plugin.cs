using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using RoleplayProfiles.Windows;
using Dalamud.Game.ClientState.Objects;
using RoleplayProfiles.State;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Data;
using Lumina.Excel.GeneratedSheets;
using System.Runtime.CompilerServices;

namespace RoleplayProfiles
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Roleplay Profiles";

        private readonly ConditionalWeakTable<PlayerCharacter, Player> playerCache = new();

        private readonly TargetManager targetManager;
        private readonly DataManager dataManager;

        private readonly PluginState pluginState;

        private readonly WindowSystem windowSystem = new("RoleplayProfiles");
        private readonly TooltipWindow tooltipWindow;
        private readonly ProfileWindow profileWindow;
        private readonly ConfigWindow configWindow;

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] TargetManager targetManager,
            [RequiredVersion("1.0")] DataManager dataManager)
        {
            this.targetManager = targetManager;
            this.dataManager = dataManager;

            var configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            configuration.Initialize(pluginInterface);

            pluginState = new PluginState(configuration);

            configWindow = new ConfigWindow(pluginState);
            windowSystem.AddWindow(configWindow);

            profileWindow = new ProfileWindow(pluginState);
            windowSystem.AddWindow(profileWindow);

            tooltipWindow = new TooltipWindow(pluginState, profileWindow, configWindow);
            windowSystem.AddWindow(tooltipWindow);

            pluginInterface.UiBuilder.Draw += DrawUI;
            pluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        public void Dispose()
        {
            windowSystem.RemoveAllWindows();
            playerCache.Clear();
            pluginState.Dispose();
        }

        private void DrawUI()
        {
            var target = targetManager.MouseOverTarget;

            if (IsRoleplayer(target))
            {
                pluginState.TargetPlayerSelected = target == targetManager.Target;
                pluginState.TargetPlayer = ToPlayer((PlayerCharacter) target!);
            }
            else
            {
                target = targetManager.Target;

                if (IsRoleplayer(target))
                {
                    pluginState.TargetPlayerSelected = true;
                    pluginState.TargetPlayer = ToPlayer((PlayerCharacter) target!);
                }
                else
                {
                    pluginState.TargetPlayerSelected = false;
                    pluginState.TargetPlayer = null;
                }
            }

            tooltipWindow.IsOpen = pluginState.TargetPlayer != null;

            windowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            configWindow.IsOpen = true;
        }

        private static bool IsRoleplayer(GameObject? target)
        {
            if (target is PlayerCharacter character)
            {
                return character.OnlineStatus.Id == 22;
            }

            return false;
        }

        private Player ToPlayer(PlayerCharacter character)
        {
            playerCache.TryGetValue(character, out Player? player);

            if (player != null)
            {
                return player;
            }

            var name = character.Name.ToString();
            var server = dataManager.GetExcelSheet<World>()!.GetRow(character.HomeWorld.Id)!.Name;
            var newPlayer = new Player(name, server);
            playerCache.Add(character, newPlayer);
            return newPlayer;
        }
    }
}
