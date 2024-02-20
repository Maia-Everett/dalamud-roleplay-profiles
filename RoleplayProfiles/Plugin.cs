using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using RoleplayProfiles.State;
using RoleplayProfiles.Windows;

namespace RoleplayProfiles
{
    public sealed class Plugin : IDalamudPlugin
    {
        private const uint PlayerStatusRoleplayer = 22;

        public string Name => "Roleplay Profiles";

        private readonly ITargetManager targetManager;
        private readonly ICondition condition;
        private readonly ICommandManager commandManager;
        private readonly IClientState clientState;

        private readonly PluginState pluginState;

        private readonly WindowSystem windowSystem = new("RoleplayProfiles");
        private readonly TooltipWindow tooltipWindow;
        private readonly ProfileWindow profileWindow;
        private readonly ConfigWindow configWindow;
        private readonly EditProfileWindow editProfileWindow;

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ITargetManager targetManager,
            [RequiredVersion("1.0")] IClientState clientState,
            [RequiredVersion("1.0")] ICondition condition,
            [RequiredVersion("1.0")] ICommandManager commandManager,
            [RequiredVersion("1.0")] IChatGui chatGui)
        {
            this.clientState = clientState;
            this.targetManager = targetManager;
            this.condition = condition;
            this.commandManager = commandManager;

            var configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            configuration.Initialize(pluginInterface);

            pluginState = new PluginState(configuration, clientState);

            editProfileWindow = new EditProfileWindow(pluginState);
            windowSystem.AddWindow(editProfileWindow);

            configWindow = new ConfigWindow(pluginState, editProfileWindow);
            windowSystem.AddWindow(configWindow);

            profileWindow = new ProfileWindow(pluginState);
            windowSystem.AddWindow(profileWindow);

            tooltipWindow = new TooltipWindow(pluginState, profileWindow, configWindow, editProfileWindow);
            windowSystem.AddWindow(tooltipWindow);

            var commandHandler = new CommandHandler(
                pluginState, configWindow, profileWindow, editProfileWindow, chatGui);
            commandManager.AddHandler(CommandHandler.CommandName, new CommandInfo(commandHandler.OnCommand)
            {
                HelpMessage = "Controls the Roleplay Profiles plugin."
            });

            pluginInterface.UiBuilder.Draw += DrawUI;
            pluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            clientState.Login += DeterminePlayerRegion;

            DeterminePlayerRegion();
            pluginState.RefreshSessionIfNecessary();
        }

        public void Dispose()
        {
            clientState.Login -= DeterminePlayerRegion;
            commandManager.RemoveHandler(CommandHandler.CommandName);
            windowSystem.RemoveAllWindows();
            pluginState.Dispose();
        }

        private void DeterminePlayerRegion()
        {
            var player = clientState.LocalPlayer;

            if (player != null)
            {
                byte playerRegion = player.HomeWorld.GameData!.DataCenter.Value!.Region;

                if (playerRegion == 2)
                {
                    // North America
                    pluginState.RegionSite = RegionSite.NA;
                }
                else
                {
                    // Everywhere else - point to Europe for now
                    pluginState.RegionSite = RegionSite.EU;
                }
            }
        }

        private void DrawUI()
        {
            var target = targetManager.MouseOverTarget;

            if (!pluginState.Configuration.Enable)
            {
                // Plugin is disabled - hide plugin UI
                pluginState.TargetPlayerSelected = false;
                pluginState.TargetPlayer = null;
            }
            else if (!pluginState.Configuration.EnableInDuties && condition[ConditionFlag.BoundByDuty])
            {
                // We're in a duty - hide plugin UI
                pluginState.TargetPlayerSelected = false;
                pluginState.TargetPlayer = null;
            }
            else if (IsRoleplayer(target))
            {
                pluginState.TargetPlayerSelected = target == targetManager.Target;
                pluginState.TargetPlayer = pluginState.ToPlayer((PlayerCharacter)target!);
            }
            else
            {
                target = targetManager.Target;

                if (IsRoleplayer(target))
                {
                    pluginState.TargetPlayerSelected = true;
                    pluginState.TargetPlayer = pluginState.ToPlayer((PlayerCharacter)target!);
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

        private void DrawConfigUI()
        {
            configWindow.IsOpen = true;
        }

        private static bool IsRoleplayer(GameObject? target)
        {
            if (target is PlayerCharacter character)
            {
                return character.OnlineStatus.Id == PlayerStatusRoleplayer;
            }

            return false;
        }
    }
}
