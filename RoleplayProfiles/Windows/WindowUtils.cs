using ImGuiNET;
using System.Numerics;
using System.Text.RegularExpressions;

namespace RoleplayProfiles.Windows;

internal class WindowUtils
{
    private static readonly Regex WikitextRegex = new(@"\[\[(.+?\|)?(.+?)\]\]");

    internal static uint ToImGuiColor(uint rgb)
    {
        return ((rgb & 0xff) << 16) | (rgb & 0xff00) | ((rgb & 0xff0000) >> 16) | 0xff000000;
    }

    internal static Vector4 ToImGuiColorVec(uint rgb)
    {
        return ImGui.ColorConvertU32ToFloat4(ToImGuiColor(rgb));
    }

    internal static string StripWikitext(string str)
    {
        return WikitextRegex.Replace(str, "$2");
    }
}
