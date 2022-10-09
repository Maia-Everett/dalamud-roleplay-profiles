using Dalamud.Interface.Windowing;
using RoleplayProfiles.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RoleplayProfiles.Windows;

public class ProfileWindow : Window, IDisposable
{
    public static readonly string DefaultTitle = "Roleplay profile: (no target player)###Roleplay Profile";

    private PluginState pluginState;

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
        
    }

    public override void Draw()
    {
        var targetPlayer = pluginState.TargetPlayer;

        if (targetPlayer == null)
        {
            this.WindowName = DefaultTitle;
            return;
        }

        this.WindowName = $"Roleplay profile: {targetPlayer.Name}###Roleplay Profile";
    }
}
