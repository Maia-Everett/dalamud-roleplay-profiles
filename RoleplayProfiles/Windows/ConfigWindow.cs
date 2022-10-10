using System;
using System.Diagnostics;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using ImGuiNET;
using RoleplayProfiles.Api;
using RoleplayProfiles.State;

namespace RoleplayProfiles.Windows;

public class ConfigWindow : Window, IDisposable
{
    public static readonly string Title = "Roleplay Profiles Configuration";

    private readonly Configuration configuration;
    private readonly ApiClient apiClient;
    private readonly EditProfileWindow editProfileWindow;

    private string userEmail;
    private string userPassword = "";
    private string exceptionMessage = "";
    private volatile bool loading = false;

    public ConfigWindow(PluginState pluginState, EditProfileWindow editProfileWindow) : base(
        Title, ImGuiWindowFlags.NoResize)
    {
        this.Size = ImGuiHelpers.ScaledVector2(232, 120);
        this.SizeCondition = ImGuiCond.Always;

        this.configuration = pluginState.Configuration;
        this.apiClient = pluginState.ApiClient;
        userEmail = configuration.UserEmail ?? "";

        this.editProfileWindow = editProfileWindow;
    }

    public void Dispose()
    {
        // Do nothing
    }

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

            var disabled = userEmail == "" || userPassword == "" || loading;

            if (disabled)
            {
                ImGui.BeginDisabled();
            }

            var loginButtonText = loading ? "Logging in..." : "Log in";
            var loginButtonSize = ImGuiHelpers.GetButtonSize(loginButtonText);
            ImGui.Text(" ");
            ImGui.SameLine(ImGui.GetWindowWidth() - loginButtonSize.X - ImGuiHelpers.ScaledVector2(8, 0).X);

            if (ImGui.Button(loginButtonText))
            {
                _ = Login();
            }
            
            if (disabled)
            {
                ImGui.EndDisabled();
            }

            ImGui.TextColored(Colors.Error, exceptionMessage);
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

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            if (ImGui.Button("Edit your profile"))
            {
                editProfileWindow.IsOpen = true;
            }
        }
    }

    private async Task Login()
    {
        loading = true;

        try
        {
            var email = userEmail;
            var response = await apiClient.Login(userEmail, userPassword);
            configuration.AccessToken = response.AccessToken;
            configuration.UserEmail = email;
        }
        catch (HttpRequestException e)
        {
            PluginLog.Information("Error logging in: " + e.Message);

            if (e.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                exceptionMessage = "Invalid email or password";
            }
            else
            {
                exceptionMessage = e.Message;
            }
        }
        catch (Exception e)
        {
            PluginLog.Information("Error logging in: " + e.Message);
            exceptionMessage = e.Message;
        }
        finally
        {
            loading = false;
        }
    }
}
