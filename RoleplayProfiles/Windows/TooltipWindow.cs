using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;
using RoleplayProfiles.State;

namespace RoleplayProfiles.Windows;

public class TooltipWindow : Window, IDisposable
{
    public static readonly string Title = "Roleplay Profile";

    private PluginState pluginState;

    public TooltipWindow(PluginState pluginState) : base(Title)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
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
        ImGui.Text("Yep, this is a roleplayer");
    }
}
