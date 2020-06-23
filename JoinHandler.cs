using MEC;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System.Collections.Generic;

namespace PintBuddy
{
    internal class JoinHandler : IEventHandlerPlayerJoin
    {
        private BuddyPlugin buddyPlugin;

        public JoinHandler(BuddyPlugin buddyPlugin)
        {
            this.buddyPlugin = buddyPlugin;
        }

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            Timing.RunCoroutine(sendJoinMessage(ev.Player));
        }
        public IEnumerator<float> sendJoinMessage(Player p)
        {
            yield return Timing.WaitForSeconds(1f);
            p.SendConsoleMessage(buddyPlugin.prefixedMessage, "yellow");
        }
    }
}