using System;
using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;

using ImGuiNET;

using RoleplayProfiles.State;

namespace RoleplayProfiles.Windows;

public class ProfileWindow : Window, IDisposable
{
    public static readonly string DefaultTitle = "Roleplay profile: (no target player)###Roleplay Profile";

    private static readonly float LabelWidth = ImGuiHelpers.ScaledVector2(100, 0).X;

    private readonly PluginState pluginState;

    public ProfileWindow(PluginState pluginState) : base(DefaultTitle)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(500, 400),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.pluginState = pluginState;
    }

    public void Dispose()
    {
        // Do nothing
    }

    public override void Draw()
    {
        var player = pluginState.ProfilePlayer;

        if (player == null)
        {
            this.WindowName = DefaultTitle;
            return;
        }

        this.WindowName = $"Roleplay profile: {player.Name}###Roleplay Profile";

        var cacheEntry = pluginState.GetProfile(player);

        if (cacheEntry.State != CacheEntryState.Retrieved)
        {
            return;
        }

        var profile = cacheEntry.Data!;

        ImGui.Columns(2, null, false);

        ShowField(profile.Occupation, "Occupation");
        ShowField(profile.Race, "Race");
        ShowField(profile.Birthplace, "Birthplace");
        ShowField(profile.Age, "Age");
        ShowField(profile.Residence, "Residence");
        ShowField(profile.Pronouns, "Pronouns");

        ImGui.Separator();

        ShowOptionalField(profile.Friends, "Friends");
        ShowOptionalField(profile.Relatives, "Relatives");
        ShowOptionalField(profile.Enemies, "Rivals/Enemies");
        ShowOptionalField(profile.Loves, "Loves");
        ShowOptionalField(profile.Hates, "Hates");
        ShowOptionalField(profile.Motto, "Motto");
        ShowOptionalField(profile.Motivation, "Motivation");

        ImGui.Columns(1);
        ImGui.Separator();
        ImGui.Spacing();

        if (!string.IsNullOrEmpty(profile.Background))
        {
            ImGui.BeginTabBar("ProfileTabs");

            if (ImGui.BeginTabItem("Outward Appearance"))
            {
                ImGui.BeginChild("ScrollRegionAppearance");
                ImGui.TextWrapped(WindowUtils.StripWikitext(profile.Appearance));
                ImGui.EndChild();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Background"))
            {
                ImGui.BeginChild("ScrollRegionBackground");
                ImGui.TextWrapped(WindowUtils.StripWikitext(profile.Background));
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
        else
        {
            ImGui.BeginChild("ScrollRegionAppearance");
            ImGui.TextWrapped(WindowUtils.StripWikitext(profile.Appearance));
            ImGui.EndChild();
            ImGui.EndTabItem();
        }        
    }

    private static void ShowField(string field, string label)
    {
        ImGui.TextColored(Colors.Label, label);
        ImGui.SameLine(LabelWidth);
        ImGui.TextWrapped(WindowUtils.StripWikitext(field));
        ImGui.NextColumn();
    }

    private static void ShowOptionalField(string field, string label)
    {
        if (!string.IsNullOrEmpty(field))
        {
            ShowField(field, label);
        }
    }
}
