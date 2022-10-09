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

    private PluginState pluginState;
    private ProfileWindow profileWindow;
    private ConfigWindow configWindow;

    public TooltipWindow(PluginState pluginState, ProfileWindow profileWindow, ConfigWindow configWindow) : base(DefaultTitle)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 250),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.pluginState = pluginState;
        this.profileWindow = profileWindow;
        this.configWindow = configWindow;
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

                ImGui.PushStyleColor(ImGuiCol.Text, WindowUtils.ToImGuiColor(0x5ae0b9));
                ImGui.Text("Retrieving profile...");
                ImGui.PopStyleColor();
                break;
            case CacheEntryState.NotFound:
                ImGui.SetWindowFontScale(nameScale);
                ImGui.Text(targetPlayer.Name);
                ImGui.SetWindowFontScale(1);
                ImGui.Spacing();

                ImGui.PushStyleColor(ImGuiCol.Text, WindowUtils.ToImGuiColor(0xffc8ed));
                ImGui.Text("Profile not found");
                ImGui.PopStyleColor();
                break;
            case CacheEntryState.Retrieved:
                ImGui.BeginChild("ScrollRegion", ImGuiHelpers.ScaledVector2(0, -32));

                var profile = cacheEntry.Data!;
                var name = profile.Title != "" ? $"{profile.Title} {targetPlayer.Name}" : targetPlayer.Name;

                ImGui.SetWindowFontScale(nameScale);
                ImGui.Text(targetPlayer.Name);
                ImGui.SetWindowFontScale(1);
                ImGui.Spacing();

                if (profile.Nickname != "")
                {
                    ImGui.TextWrapped($"\"{profile.Nickname}\"");
                }

                if (profile.Occupation != "")
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, WindowUtils.ToImGuiColor(0xff9d20));
                    ImGui.TextWrapped($"< {profile.Occupation} >");
                    ImGui.PopStyleColor();
                }

                if (profile.Pronouns != "")
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, WindowUtils.ToImGuiColor(0x7e7e7e));
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
                    ImGui.SetWindowFontScale(1.2f);
                    ImGui.Text("OOC Info");
                    ImGui.SetWindowFontScale(1);
                    ImGui.TextWrapped(profile.OocInfo);
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
