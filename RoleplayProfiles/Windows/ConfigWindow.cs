using System;
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

    public ConfigWindow(PluginState pluginState) : base(
        Title, ImGuiWindowFlags.NoResize)
    {
        this.Size = ImGuiHelpers.ScaledVector2(232, 75);
        this.SizeCondition = ImGuiCond.Always;

        this.configuration = pluginState.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        
    }
}
