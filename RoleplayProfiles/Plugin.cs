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
        private const string CommandName = "/rpp";

        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("RoleplayProfiles");

        private TooltipWindow tooltipWindow;
        private ConfigWindow configWindow;

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] TargetManager targetManager)
        {
            this.Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(pluginInterface);

            configWindow = new ConfigWindow(this);
            WindowSystem.AddWindow(configWindow);

            tooltipWindow = new TooltipWindow(this);
            WindowSystem.AddWindow(tooltipWindow);

            pluginInterface.UiBuilder.Draw += DrawUI;
            pluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            configWindow.IsOpen = true;
        }
    }
}
