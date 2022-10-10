using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RoleplayProfiles.Windows;

internal class Colors
{
    public static readonly Vector4 Error = WindowUtils.ToImGuiColorVec(0xffc8ed);
    public static readonly Vector4 Label = WindowUtils.ToImGuiColorVec(0x7e7e7e);
    public static readonly Vector4 Loading = WindowUtils.ToImGuiColorVec(0x5ae0b9);
    public static readonly Vector4 Occupation = WindowUtils.ToImGuiColorVec(0xff9d20);
}
