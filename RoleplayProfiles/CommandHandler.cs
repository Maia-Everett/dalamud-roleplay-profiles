using System;
using Dalamud.Game.Gui;
using RoleplayProfiles.State;
using RoleplayProfiles.Windows;

namespace RoleplayProfiles;

internal class CommandHandler
{
    internal const string CommandName = "/rpp";

	private const string ArgEdit = "edit";
	private const string ArgProfile = "profile";
	private const string ArgConfig = "config";
	private const string ArgHide = "hide";
	private const string ArgShow = "show";
	private const string ArgDutyHide = "duty hide";
	private const string ArgDutyShow = "duty show";

    private readonly PluginState pluginState;
    private readonly ConfigWindow configWindow;
	private readonly ProfileWindow profileWindow;
    private readonly EditProfileWindow editProfileWindow;
    private readonly ChatGui chatGui;

    internal CommandHandler(PluginState pluginState, ConfigWindow configWindow, ProfileWindow profileWindow,
		EditProfileWindow editProfileWindow, ChatGui chatGui)
    {
        this.pluginState = pluginState;
        this.configWindow = configWindow;
		this.profileWindow = profileWindow;
        this.editProfileWindow = editProfileWindow;
        this.chatGui = chatGui;
    }

    internal void OnCommand(string command, string args)
    {
        args = args.Trim();

        if (args.StartsWith(ArgEdit))
        {
			OnEdit();
        }
        else if (args.StartsWith(ArgProfile))
        {
			OnProfile();
        }
        else if (args.StartsWith(ArgConfig))
        {
			OnConfig();
        }
        else if (args.StartsWith(ArgHide))
        {
			OnHide();
        }
        else if (args.StartsWith(ArgShow))
        {
			OnShow();
        }
        else if (args.StartsWith(ArgDutyHide))
        {
			OnDutyHide();
        }
        else if (args.StartsWith(ArgDutyShow))
        {
			OnDutyShow();
        }
        else
        {
            OnHelp();
        }
    }

    private void OnEdit()
    {
        if (pluginState.Configuration.AccessToken != null)
		{
			editProfileWindow.IsOpen = true;
			chatGui.Print("RPP profile editor opened.");
		}
		else
		{
			chatGui.Print("You are not logged in. Opening configuration to let you log in.");
			configWindow.IsOpen = true;
		}
    }

    private void OnProfile()
    {
		var targetPlayer = pluginState.TargetPlayer;

        if (targetPlayer != null)
		{
			pluginState.ProfilePlayer = targetPlayer;
			profileWindow.IsOpen = true;
			chatGui.Print(string.Format("RPP profile opened for {0}.", targetPlayer));
		}
		else
		{
			chatGui.Print("You have not targeted any player with the RP flag.");
		}
    }

    private void OnConfig()
    {
        configWindow.IsOpen = true;
		chatGui.Print("RPP configuration window opened.");
    }

    private void OnHide()
    {
        pluginState.Configuration.Enable = false;
		pluginState.Configuration.Save();
		chatGui.Print("RPP tooltip disabled. Type /rpp show to show it again.");
    }

    private void OnShow()
    {
        pluginState.Configuration.Enable = true;
		pluginState.Configuration.Save();
		chatGui.Print("RPP tooltip enabled. Type /rpp hide to hide it.");
    }

    private void OnDutyHide()
    {
        pluginState.Configuration.EnableInDuties = false;
		pluginState.Configuration.Save();
		chatGui.Print("RPP tooltip is no longer shown during duties. Type /rpp duty show to show it again.");
    }

    private void OnDutyShow()
    {
        pluginState.Configuration.EnableInDuties = true;
		pluginState.Configuration.Save();
		chatGui.Print("RPP tooltip is now shown during duties. Type /rpp duty hide to hide it.");
    }

	private void OnHelp()
	{
		PrintHelp(ArgEdit, "Edit your own profile (if you are logged in)");
		PrintHelp(ArgProfile, "Open the full profile of your target");
		PrintHelp(ArgConfig, "Open the configuration window");
		PrintHelp(ArgHide, "Hide the tooltip window");
		PrintHelp(ArgShow, "Show the tooltip window");
		PrintHelp(ArgDutyHide, "Disable the tooltip window during duties");
		PrintHelp(ArgDutyShow, "Enable the tooltip window during duties");
		PrintHelp("", "Show this help");
	}

    private void PrintHelp(string argument, string message)
    {
        var fullCommand = argument == "" ? CommandName : CommandName + " " + argument;
        chatGui.Print(string.Format("{0} → {1}", fullCommand, message));
    }
}
