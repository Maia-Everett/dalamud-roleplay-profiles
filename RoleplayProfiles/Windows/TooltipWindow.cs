using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;
using RoleplayProfiles.State;

namespace RoleplayProfiles.Windows;

public class TooltipWindow : Window, IDisposable
{
    private static readonly string DefaultTitle = "At first glance: (no target player)###Roleplay Profile First Glance";

    private PluginState pluginState;

    public TooltipWindow(PluginState pluginState) : base(DefaultTitle)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 250),
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
        var targetPlayer = pluginState.TargetPlayer;

        if (targetPlayer == null)
        {
            this.WindowName = DefaultTitle;
            return;
        }

        this.WindowName = "At first glance: " + targetPlayer + "###Roleplay Profile First Glance";
        ImGui.Text("Yep, this is a roleplayer");
    }
}
