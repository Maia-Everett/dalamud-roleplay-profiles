using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoleplayProfiles.State
{
    public enum CacheEntryState
    {
        Pending,
        Retrieved,
        NotFound,
        Failed,
        Invalidated,
        Updating,
    }
}
