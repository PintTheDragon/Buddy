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
        public bool IsEnabled { get; set; } = true;

        public bool forceExactRole { get; set; } = false;

        public bool disallowGuardScientistCombo { get; set; } = true;

        public bool resetBuddiesEveryRound { get; set; } = true;

        public bool sendInfoBroadcast { get; set; } = true;

        public bool sendBuddyBroadcast { get; set; } = true;

        public bool sendBuddyRequestBroadcast { get; set; } = true;

        public bool sendBuddyAcceptedBroadcast { get; set; } = true;
    }
}
