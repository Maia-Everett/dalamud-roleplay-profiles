using RoleplayProfiles.Api;

namespace RoleplayProfiles.State
{
    public class ProfileCacheEntry
    {
        public CacheEntryState State { get; set; } = CacheEntryState.Pending;
        public Profile? Data { get; set; } = null;
    }
}
