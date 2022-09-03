using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using System.Reflection;
using Dalamud.Interface.Windowing;
using SamplePlugin.Windows;
using Dalamud.Game.ClientState.Objects;

namespace SamplePlugin
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Roleplay Profiles";

        private readonly WindowSystem windowSystem = new("RoleplayProfiles");
        private readonly TooltipWindow tooltipWindow;
        private readonly ConfigWindow configWindow;

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] TargetManager targetManager)
        {
            var configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            configuration.Initialize(pluginInterface);

            configWindow = new ConfigWindow(this);
            windowSystem.AddWindow(configWindow);

            tooltipWindow = new TooltipWindow(this);
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
            windowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            configWindow.IsOpen = true;
        }
    }
}
