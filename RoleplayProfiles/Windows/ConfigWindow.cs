using System;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace SamplePlugin.Windows;

public class ConfigWindow : Window, IDisposable
{
    public static readonly string Title = "Roleplay Profiles Configuration";

    private Configuration configuration;

    public ConfigWindow(Plugin plugin) : base(
        Title, ImGuiWindowFlags.NoResize)
    {
        this.Size = ImGuiHelpers.ScaledVector2(232, 75);
        this.SizeCondition = ImGuiCond.Always;

        this.configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        // can't ref a property, so use a local copy
        var configValue = this.configuration.SomePropertyToBeSavedAndWithADefault;
        if (ImGui.Checkbox("Random Config Bool", ref configValue))
        {
            this.configuration.SomePropertyToBeSavedAndWithADefault = configValue;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            this.configuration.Save();
        }
    }
}
