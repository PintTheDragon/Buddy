using Exiled.API.Interfaces;
using Exiled.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buddy
{
    class Config : IConfig
    {
        public bool IsEnabled { get; set; }

        public Boolean forceExactRole { get; set; }

        public Boolean disallowGuardScientistCombo { get; set; }

        public Boolean resetBuddiesEveryRound { get; set; }

        public Boolean sendInfoBroadcast { get; set; }

        public Boolean sendBuddyBroadcast { get; set; }

        public Boolean sendBuddyRequestBroadcast { get; set; }

        public Boolean sendBuddyAcceptedBroadcast { get; set; }

        public string Prefix => "buddy_";

        public void Reload()
        {
            IsEnabled = PluginManager.YamlConfig.GetBool($"{Prefix}enabled", true);
            forceExactRole = PluginManager.YamlConfig.GetBool($"{Prefix}force_exact_role", false);
            disallowGuardScientistCombo = PluginManager.YamlConfig.GetBool($"{Prefix}disallow_guard_scientist_combo", true);
            resetBuddiesEveryRound = PluginManager.YamlConfig.GetBool($"{Prefix}reset_buddies_every_round", true);
            sendInfoBroadcast = PluginManager.YamlConfig.GetBool($"{Prefix}send_info_broadcast", true);
            sendBuddyBroadcast = PluginManager.YamlConfig.GetBool($"{Prefix}send_buddy_broadcast", true);
            sendBuddyRequestBroadcast = PluginManager.YamlConfig.GetBool($"{Prefix}send_buddy_request_broadcast", true);
            sendBuddyAcceptedBroadcast = PluginManager.YamlConfig.GetBool($"{Prefix}send_buddy_accepted_broadcast", true);
        }
    }
}
