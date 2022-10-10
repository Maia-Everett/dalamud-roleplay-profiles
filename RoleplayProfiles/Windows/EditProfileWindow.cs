using Dalamud.Interface.Windowing;
using RoleplayProfiles.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RoleplayProfiles.Windows;

public class EditProfileWindow : Window, IDisposable
{
    public static readonly string DefaultTitle = "Edit character profile###EditProfile";

    private readonly PluginState pluginState;

    public EditProfileWindow(PluginState pluginState) : base(DefaultTitle)
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
        throw new NotImplementedException();
    }
}