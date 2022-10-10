using RoleplayProfiles.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoleplayProfiles.State
{
    public class ProfileCacheEntry
    {
        public CacheEntryState State { get; set; } = CacheEntryState.Pending;
        public Profile? Data { get; set; } = null;
        internal DateTime Updated { get; set; } = DateTime.MinValue;
        internal bool Expired { get; set; } = false;
    }
}
