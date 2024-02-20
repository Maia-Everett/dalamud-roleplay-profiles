using System;
using System.Text.Json.Serialization;

namespace RoleplayProfiles.State;

public class RegionSite
{
    public static readonly RegionSite EU = new("Chaos Archives", "https://chaosarchives.org");
    public static readonly RegionSite NA = new("Crystal Archives", "https://crystalarchives.org");

    public string Name { get; init; }
    public string Url { get; init; }

    [JsonConstructor]
    public RegionSite(string name, string url)
    {
        this.Name = name;
        this.Url = url;
    }

    public override bool Equals(object? obj)
    {
        return obj is RegionSite rs &&
               Name == rs.Name &&
               Url == rs.Url;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Url);
    }

    public override string ToString()
    {
        return Name;
    }
}
