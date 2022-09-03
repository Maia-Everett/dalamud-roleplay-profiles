using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;

namespace SamplePlugin.Windows;

public class TooltipWindow : Window, IDisposable
{
    public static readonly string Title = "Roleplay Profile";

    private Plugin plugin;

    public TooltipWindow(Plugin plugin) : base(Title)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.plugin = plugin;
    }

    public void Dispose()
    {
        // Do nothing
    }

    public override void Draw()
    {
        ImGui.Text($"The random config bool is {this.plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");

        if (ImGui.Button("Show Settings"))
        {
            this.plugin.DrawConfigUI();
        }

        ImGui.Spacing();

    }
}
