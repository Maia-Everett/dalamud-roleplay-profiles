using System;
using System.Text.Json.Serialization;

namespace RoleplayProfiles.Api;

public class Player
{
    public string Name { get; init; }
    public string Server { get; init; }

    [JsonConstructor]
    public Player(string name, string server)
    {
        this.Name = name;
        this.Server = server;
    }

    public override bool Equals(object? obj)
    {
        return obj is Player player &&
               Name == player.Name &&
               Server == player.Server;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Server);
    }

    public override string ToString()
    {
        return $"{Name} ({Server})";
    }
}
