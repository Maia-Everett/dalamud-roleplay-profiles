using System;
using System.Diagnostics;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using RoleplayProfiles.State;

namespace RoleplayProfiles.Windows;

public class ConfigWindow : Window, IDisposable
{
    public static readonly string Title = "Roleplay Profiles Configuration";

    private Configuration configuration;
    private string userEmail;
    private string userPassword = "";

    public ConfigWindow(PluginState pluginState) : base(
        Title, ImGuiWindowFlags.NoResize)
    {
        this.Size = ImGuiHelpers.ScaledVector2(232, 100);
        this.SizeCondition = ImGuiCond.Always;

        this.configuration = pluginState.Configuration;
        userEmail = configuration.UserEmail ?? "";
    }

    public void Dispose() { }

    public override void Draw()
    {
        if (configuration.AccessToken == null)
        {
            ImGui.TextWrapped("To edit your character profiles in-game, you need to log in to Chaos Archives.");
            ImGui.Spacing();

            var labelWidth = ImGuiHelpers.ScaledVector2(160, 0).X;

            ImGui.Text("Chaos Archives email:");
            ImGui.SameLine(labelWidth);
            ImGui.InputText("###Email", ref userEmail, 255);

            ImGui.Text("Chaos Archives password:");
            ImGui.SameLine(labelWidth);
            ImGui.InputText("###Password", ref userPassword, 255, ImGuiInputTextFlags.Password);
            ImGui.Spacing();

            var disabled = userEmail == "" || userPassword == "";

            if (disabled)
            {
                ImGui.BeginDisabled();
            }

            var loginButtonText = "Log in";
            var loginButtonSize = ImGuiHelpers.GetButtonSize(loginButtonText);
            ImGui.Text(" ");
            ImGui.SameLine(ImGui.GetWindowWidth() - loginButtonSize.X - ImGuiHelpers.ScaledVector2(8, 0).X);

            if (ImGui.Button(loginButtonText))
            {
                configuration.UserEmail = userEmail;
                configuration.AccessToken = "test";
            }
            
            if (disabled)
            {
                ImGui.EndDisabled();
            }

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.TextWrapped("Not yet registered on Chaos Archives? Sign up now! " +
                "(The button will open the signup page in your web browser.)");
            ImGui.TextWrapped("Currently signups are only available to characters in the Europe region.");

            var signUpButtonText = "Sign up";
            var signUpButtonSize = ImGuiHelpers.GetButtonSize(signUpButtonText);
            ImGui.Text(" ");
            ImGui.SameLine(ImGui.GetWindowWidth() - signUpButtonSize.X - ImGuiHelpers.ScaledVector2(8, 0).X);

            if (ImGui.Button(signUpButtonText))
            {
                Process.Start(new ProcessStartInfo { FileName = "https://chaosarchives.org/signup", UseShellExecute = true });
            }
        }
        else
        {
            ImGui.TextWrapped("You are logged in to Chaos Archives as:");
            ImGui.TextWrapped(configuration.UserEmail);

            if (ImGui.Button("Log out"))
            {
                configuration.UserEmail = null;
                configuration.AccessToken = null;
            }
        }
    }
}
