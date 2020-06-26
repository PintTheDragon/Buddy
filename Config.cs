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

        public Boolean forceExactRole { get; set; } = false;

        public Boolean disallowGuardScientistCombo { get; set; } = true;

        public Boolean resetBuddiesEveryRound { get; set; } = true;

        public Boolean sendInfoBroadcast { get; set; } = true;

        public Boolean sendBuddyBroadcast { get; set; } = true;

        public Boolean sendBuddyRequestBroadcast { get; set; } = true;

        public Boolean sendBuddyAcceptedBroadcast { get; set; } = true;
    }
}
