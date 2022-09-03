using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoleplayProfiles.State
{
    public class PluginState
    {
        public Configuration Configuration { get; init; }

        public PluginState(Configuration configuration)
        {
            this.Configuration = configuration;
        }
    }
}
