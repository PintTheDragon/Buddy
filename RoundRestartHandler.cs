using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PintBuddy
{
    class RoundRestartHandler : IEventHandlerRoundRestart
    {
        private Buddy buddyPlugin;

        public RoundRestartHandler(Buddy buddyPlugin)
        {
            this.buddyPlugin = buddyPlugin;
        }

        public void OnRoundRestart(RoundRestartEvent ev)
        {
            buddyPlugin.buddies = new Dictionary<string, Player>();
        }
    }
}
