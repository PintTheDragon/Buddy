using Smod2.EventHandlers;
using Smod2.Events;

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
            ev.Player.SendConsoleMessage(buddyPlugin.prefixedMessage, "yellow");
        }
    }
}