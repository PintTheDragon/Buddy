using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace Buddy
{
    //[CommandHandler(typeof(GameConsoleCommandHandler))]
    class BuddyAcceptCommand : ICommandPint
    {
        public string Command => Buddy.singleton.getLang("buddyAcceptCommand");

        public string[] Aliases => null;

        public string Description => "A command to accept a pending buddy request.";

        public bool Execute(ArraySegment<string> arguments, /*ICommandSender sender*/ Player player, out string response)
        {
            response = "";
            string[] args = arguments.ToArray();
            //if (sender is PlayerCommandSender p)
            //{
                //Player player = Player.Get(p.Processor._hub);
                response = handleBuddyAcceptCommand(player, new string[] { });
                //return true;
            //}
            return true;
        }

        private string handleBuddyAcceptCommand(Player p, string[] args)
        {
            //checks
            if (!Buddy.singleton.buddyRequests.ContainsKey(p.UserId))
            {
                return Buddy.singleton.getLang("noBuddyRequestsMessage");
            }

            //set the buddy
            Player buddy = null;
            try
            {
                Buddy.singleton.buddyRequests.TryGetValue(p.UserId, out buddy);
            }
            catch (ArgumentNullException e)
            {
                Log.Error(e.ToString());
                return Buddy.singleton.getLang("errorMessage");
            }
            if (buddy == null)
            {
                Buddy.singleton.buddies.Remove(p.UserId);
                Buddy.singleton.removePerson(p.UserId);
                return Buddy.singleton.getLang("errorMessage");

            }
            try
            {
                if (Buddy.singleton.buddies.ContainsKey(p.UserId))
                {
                    string refh = null;
                    Buddy.singleton.buddies.TryGetValue(p.UserId, out refh);
                    if (refh != null) Buddy.singleton.buddies.Remove(refh);
                    else Buddy.singleton.removePerson(p.UserId);
                    Buddy.singleton.buddies.Remove(p.UserId);
                }
            }
            catch (ArgumentNullException e)
            {
                Log.Error(e.ToString());
                return Buddy.singleton.getLang("errorMessage");
            }

            Buddy.singleton.buddies.Add(p.UserId, buddy.UserId);
            Buddy.singleton.buddies.Add(buddy.UserId, p.UserId);
            Buddy.singleton.buddyRequests.Remove(p.UserId);
            buddy.SendConsoleMessage(Buddy.singleton.getLang("buddyRequestAcceptMessage").Replace("$name", p.Nickname), "yellow");
            if (Buddy.singleton.Config.sendBuddyAcceptedBroadcast)
                buddy.Broadcast(5, Buddy.singleton.getLang("buddyRequestAcceptMessage").Replace("$name", p.Nickname), Broadcast.BroadcastFlags.Normal);
            return Buddy.singleton.getLang("successMessage");
        }
    }
}
