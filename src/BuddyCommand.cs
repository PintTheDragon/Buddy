using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace Buddy
{
    //[CommandHandler(typeof(GameConsoleCommandHandler))]
    class BuddyCommand : ICommandPint
    {
        public string Command => Buddy.singleton.getLang("buddyCommand");

        public string[] Aliases => null;

        public string Description => "Allows you to pair up with another player and play on the same team.";

        public bool Execute(ArraySegment<string> arguments, /*ICommandSender sender*/ Player player, out string response)
        {
            response = "";
            string[] args = arguments.ToArray();
            //if(sender is PlayerCommandSender p)
            //{
                //Player player = Player.Get(p.Processor._hub);
                if (args.Length != 1)
                {
                    response = Buddy.singleton.getLang("invalidUsage");
                    return true;
                }
                try
                {
                    response = handleBuddyCommand(player, args);
                    return true;
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    response = Buddy.singleton.getLang("errorMessage");
                }
            //}
            return true;
        }

        private string handleBuddyCommand(Player p, string[] args)
        {
            //get the player who the request was sent to
            Player buddy = null;
            string lower = args[0].ToLower();
            foreach (Player hub in Player.List)
            {
                if (hub == null) continue;
                if (hub.ReferenceHub.nicknameSync.Network_myNickSync.ToLower().Contains(lower) && hub.UserId != p.UserId)
                {
                    buddy = hub;
                    break;
                }
            }
            if (buddy == null)
            {
                return Buddy.singleton.getLang("playerNotFoundMessage");
            }

            if (Buddy.singleton.buddyRequests.ContainsKey(buddy.UserId)) Buddy.singleton.buddyRequests.Remove(buddy.UserId);
            Buddy.singleton.buddyRequests.Add(buddy.UserId, p);
            buddy.SendConsoleMessage(Buddy.singleton.getLang("BuddyMessagePrompt").Replace("$name", p.Nickname).Replace("$buddyAcceptCMD", "." + Buddy.singleton.getLang("buddyAcceptCommand")), "yellow");
            if (Buddy.singleton.Config.sendBuddyRequestBroadcast && !Round.IsStarted)
                buddy.Broadcast(5, Buddy.singleton.getLang("broadcastBuddyRequest").Replace("$name", p.Nickname), Broadcast.BroadcastFlags.Normal);
            return Buddy.singleton.getLang("buddyRequestSentMessage");
        }
    }
}
