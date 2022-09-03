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
        public override string ToString()
        {
            return $"{Name} ({Server})";
        }
    }
}
