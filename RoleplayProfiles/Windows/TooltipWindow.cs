using System;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;
using RoleplayProfiles.State;

namespace RoleplayProfiles.Windows;

public class TooltipWindow : Window, IDisposable
{
    private static readonly string DefaultTitle = "At first glance: (no target player)###Roleplay Profile First Glance";

    private readonly PluginState pluginState;
    private readonly ProfileWindow profileWindow;
    private readonly ConfigWindow configWindow;
    private readonly EditProfileWindow editProfileWindow;

    public TooltipWindow(PluginState pluginState, ProfileWindow profileWindow, ConfigWindow configWindow,
        EditProfileWindow editProfileWindow) : base(DefaultTitle)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 250),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.pluginState = pluginState;
        this.profileWindow = profileWindow;
        this.configWindow = configWindow;
        this.editProfileWindow = editProfileWindow;
    }

    public void Dispose()
    {
        // Do nothing
    }

    public override void Draw()
    {
        var targetPlayer = pluginState.TargetPlayer;

        if (targetPlayer == null)
        {
            this.WindowName = DefaultTitle;
            return;
        }

        this.WindowName = $"At first glance: {targetPlayer.Name}###Roleplay Profile First Glance";

        // Render window

        var cacheEntry = pluginState.GetProfile(targetPlayer);
        var nameScale = 1.4f;

        switch (cacheEntry.State)
        {
            case CacheEntryState.Pending:
                ImGui.SetWindowFontScale(nameScale);
                ImGui.Text(targetPlayer.Name);
                ImGui.SetWindowFontScale(1);
                ImGui.Spacing();

                ImGui.PushStyleColor(ImGuiCol.Text, Colors.Loading);
                ImGui.Text("Retrieving profile...");
                ImGui.PopStyleColor();
                break;
            case CacheEntryState.NotFound:
                ImGui.SetWindowFontScale(nameScale);
                ImGui.Text(targetPlayer.Name);
                ImGui.SetWindowFontScale(1);
                ImGui.Spacing();

                ImGui.PushStyleColor(ImGuiCol.Text, Colors.Error);
                ImGui.Text("Profile not found");
                ImGui.PopStyleColor();
                break;
            case CacheEntryState.Failed:
                ImGui.SetWindowFontScale(nameScale);
                ImGui.Text(targetPlayer.Name);
                ImGui.SetWindowFontScale(1);
                ImGui.Spacing();

                ImGui.PushStyleColor(ImGuiCol.Text, Colors.Error);
                ImGui.Text("Error retrieving profile");
                ImGui.PopStyleColor();
                break;
            default:
                var profile = cacheEntry.Data;

                if (profile == null)
                {
                    return;
                }

                ImGui.BeginChild("ScrollRegion", ImGuiHelpers.ScaledVector2(0, -32));

                var name = profile.Title != "" ? $"{profile.Title} {targetPlayer.Name}" : targetPlayer.Name;

                ImGui.SetWindowFontScale(nameScale);
                ImGui.Text(name);
                ImGui.SetWindowFontScale(1);
                ImGui.Spacing();

                if (profile.Nickname != "")
                {
                    ImGui.TextWrapped($"\"{profile.Nickname}\"");
                }

                if (profile.Occupation != "")
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, Colors.Occupation);
                    ImGui.TextWrapped($"< {profile.Occupation} >");
                    ImGui.PopStyleColor();
                }

                if (profile.Pronouns != "")
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, Colors.Label);
                    ImGui.TextWrapped($"Pronouns: {profile.Pronouns}");
                    ImGui.PopStyleColor();
                }

                if (profile.Currently != "")
                {
                    ImGui.Spacing();
                    ImGui.SetWindowFontScale(1.2f);
                    ImGui.Text("Currently");
                    ImGui.SetWindowFontScale(1);
                    ImGui.TextWrapped(profile.Currently);
                }

                if (profile.OocInfo != "")
                {
                    ImGui.Spacing();
                    ImGui.PushStyleColor(ImGuiCol.Text, Colors.OocInfo);
                    ImGui.SetWindowFontScale(1.2f);
                    ImGui.Text("OOC Info");
                    ImGui.SetWindowFontScale(1);
                    ImGui.TextWrapped(profile.OocInfo);
                    ImGui.PopStyleColor();
                }

                ImGui.EndChild();
                ImGuiHelpers.ScaledDummy(ImGuiHelpers.ScaledVector2(0, 2));

                if (pluginState.Configuration.AccessToken == null)
                {
                    if (ImGui.Button("Log in to Chaos Archives"))
                    {
                        configWindow.IsOpen = true;
                    }

                    ImGui.SameLine();
                }
                else
                {
                    if (ImGui.Button("Edit your profile"))
                    {
                        editProfileWindow.IsOpen = true;
                    }

                    ImGui.SameLine();
                }

                if (pluginState.TargetPlayerSelected)
                {
                    var buttonText = "Full profile";
                    var buttonSize = ImGuiHelpers.GetButtonSize(buttonText);
                    ImGui.Text(" ");
                    ImGui.SameLine(ImGui.GetWindowWidth() - buttonSize.X - ImGuiHelpers.ScaledVector2(8, 0).X);
                    
                    if (ImGui.Button(buttonText))
                    {
                        pluginState.ProfilePlayer = targetPlayer;
                        profileWindow.IsOpen = true;
                    }
                }
                
                break;
        }
    }
}
