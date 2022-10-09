using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RoleplayProfiles.Windows;

internal class WindowUtils
{
    private static readonly Regex WikitextRegex = new Regex(@"\[\[(.+?\|)?(.+?)\]\]");

    internal static uint ToImGuiColor(uint rgb)
    {
        return (rgb & 0xff) << 16 | (rgb & 0xff00) | (rgb & 0xff0000) >> 16 | 0xff000000;
    }

    internal static string StripWikitext(string str)
    {
        return WikitextRegex.Replace(str, "$2");
    }
}
