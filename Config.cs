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

        public string Prefix => "buddy_";

        public void Reload()
        {
            IsEnabled = PluginManager.YamlConfig.GetBool($"{Prefix}enabled", true);
            forceExactRole = PluginManager.YamlConfig.GetBool($"{Prefix}force_exact_role", false);
            forceExactRole = PluginManager.YamlConfig.GetBool($"{Prefix}disallow_guard_scientist_combo", true);
        }
    }
}
