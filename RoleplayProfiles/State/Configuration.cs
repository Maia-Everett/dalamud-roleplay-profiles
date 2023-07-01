using System;

using Dalamud.Configuration;
using Dalamud.Plugin;

namespace RoleplayProfiles.State
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;
        public string? UserEmail { get; set; } = null;
        public string? AccessToken { get; set; } = null;
        public bool AccessTokenExpired { get; set; } = false;
        public bool Enable { get; set; } = true;
        public bool EnableInDuties { get; set; } = false;

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.pluginInterface!.SavePluginConfig(this);
        }
    }
}
