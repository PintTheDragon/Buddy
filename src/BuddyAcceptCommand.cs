using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace Buddy
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class BuddyAcceptCommand : ICommand
    {
        public string Command => Buddy.singleton.Config.GetLang("buddyAcceptCommand");

        public string[] Aliases => null;

        public string Description => "A command to accept a pending buddy request.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "";
            if (sender is PlayerCommandSender)
            {
                Player player = Player.Get(((CommandSender)sender).SenderId);
                response = HandleBuddyAcceptCommand(player);
                return true;
            }
            return true;
        }

        private string HandleBuddyAcceptCommand(Player p)
        {
            //checks
            if (!Buddy.singleton.buddyRequests.ContainsKey(p.UserId))
            {
                return Buddy.singleton.Config.GetLang("noBuddyRequestsMessage");
            }

            //set the buddy
            Player buddy;
            try
            {
                Buddy.singleton.buddyRequests.TryGetValue(p.UserId, out buddy);
            }
            catch (ArgumentNullException e)
            {
                Log.Error(e.ToString());
                return Buddy.singleton.Config.GetLang("errorMessage");
            }
            if (buddy == null)
            {
                Buddy.singleton.buddies.Remove(p.UserId);
                Buddy.singleton.RemovePerson(p.UserId);
                return Buddy.singleton.Config.GetLang("errorMessage");
            }
            try
            {
                if (Buddy.singleton.buddies.ContainsKey(p.UserId))
                {
                    Buddy.singleton.buddies.Remove(p.UserId);
                    Buddy.singleton.RemovePerson(p.UserId);
                }
            }
            catch (ArgumentNullException e)
            {
                Log.Error(e);
                return Buddy.singleton.Config.GetLang("errorMessage");
            }

            Buddy.singleton.buddies[p.UserId] = buddy.UserId;
            Buddy.singleton.buddies[buddy.UserId] = p.UserId;
            Buddy.singleton.buddyRequests.Remove(p.UserId);
            buddy.SendConsoleMessage(Buddy.singleton.Config.GetLang("buddyRequestAcceptMessage").Replace("$name", p.Nickname), "yellow");
            if (Buddy.singleton.Config.SendBuddyAcceptedBroadcast)
                buddy.Broadcast(5, Buddy.singleton.Config.GetLang("buddyRequestAcceptMessage").Replace("$name", p.Nickname), Broadcast.BroadcastFlags.Normal);
            return Buddy.singleton.Config.GetLang("successMessage");
        }
    }
}
