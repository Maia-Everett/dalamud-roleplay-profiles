using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoleplayProfiles.State
{
    public class Player
    {
        public string Name { get; init; }
        public string Server { get; init; }

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
}
