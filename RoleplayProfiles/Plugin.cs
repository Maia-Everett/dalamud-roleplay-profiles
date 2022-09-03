using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using System.Reflection;
using Dalamud.Interface.Windowing;
using RoleplayProfiles.Windows;
using Dalamud.Game.ClientState.Objects;
using RoleplayProfiles.State;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;

namespace RoleplayProfiles
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Roleplay Profiles";

        private readonly TargetManager targetManager;

        private readonly PluginState pluginState;

        private readonly WindowSystem windowSystem = new("RoleplayProfiles");
        private readonly TooltipWindow tooltipWindow;
        private readonly ConfigWindow configWindow;

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] TargetManager targetManager)
        {
            this.targetManager = targetManager;

            var configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            configuration.Initialize(pluginInterface);

            pluginState = new PluginState(configuration);

            configWindow = new ConfigWindow(pluginState);
            windowSystem.AddWindow(configWindow);

            tooltipWindow = new TooltipWindow(pluginState);
            windowSystem.AddWindow(tooltipWindow);

            pluginInterface.UiBuilder.Draw += DrawUI;
            pluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        public void Dispose()
        {
            windowSystem.RemoveAllWindows();
        }

        private void DrawUI()
        {
            var target = targetManager.MouseOverTarget;

            if (IsRoleplayer(target))
            {
                pluginState.TargetPlayerSelected = target == targetManager.Target;
                pluginState.TargetPlayer = new Player(target!.Name.ToString(), "Omega");
            }
            else
            {
                target = targetManager.Target;

                if (IsRoleplayer(target))
                {
                    pluginState.TargetPlayerSelected = true;
                    pluginState.TargetPlayer = new Player(target!.Name.ToString(), "Omega");
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
            if (target is Character character)
            {
                return character.OnlineStatus.Id == 22;
            }

            return false;
        }
    }
}
