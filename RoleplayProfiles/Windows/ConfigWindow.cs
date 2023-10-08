using System;
using System.Diagnostics;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;

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
    private bool enable;
    private bool enableInDuties;
    private volatile bool loading = false;

    public ConfigWindow(PluginState pluginState, EditProfileWindow editProfileWindow) : base(Title)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(232, 160),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.configuration = pluginState.Configuration;
        this.apiClient = pluginState.ApiClient;
        userEmail = configuration.UserEmail ?? "";
        enable = configuration.Enable;
        enableInDuties = configuration.EnableInDuties;

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
            if (!configuration.AccessTokenExpired)
            {
                ImGui.TextWrapped("To edit your character profiles in-game, you need to log in to Chaos Archives.");
            }
            else
            {
                ImGui.TextWrapped("Your Chaos Archives login session has expired. Please log in again.");
            }
            
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
                configuration.AccessTokenExpired = false;
            }

            ImGui.Spacing();
            ImGui.Separator();
            ImGuiHelpers.ScaledDummy(12);

            if (ImGui.Button("Edit your profile"))
            {
                editProfileWindow.IsOpen = true;
            }
        }

        ImGuiHelpers.ScaledDummy(12);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(12);

        // Checkboxes area
        
        if (ImGui.Checkbox("Enable", ref enable) != configuration.Enable) {
            configuration.Enable = enable;
            configuration.Save();
        }

        if (!enable)
        {
            ImGui.BeginDisabled();
        }
        
        if (ImGui.Checkbox("Enable during duties", ref enableInDuties) != configuration.EnableInDuties) {
            configuration.EnableInDuties = enableInDuties;
            configuration.Save();
        }

        if (!enable)
        {
            ImGui.EndDisabled();
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
            configuration.AccessTokenExpired = false;
            configuration.UserEmail = email;
            configuration.Save();
        }
        catch (HttpRequestException e)
        {
            // PluginLog.Information("Error logging in: " + e.Message);

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
            // PluginLog.Information("Error logging in: " + e.Message);
            exceptionMessage = e.Message;
        }
        finally
        {
            loading = false;
        }
    }
}
