using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace Buddy
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class BuddyCommand : ICommand
    {
        public string Command => Buddy.singleton.Config.GetLang("buddyCommand");

        public string[] Aliases => null;

        public string Description => "Allows you to pair up with another player and play on the same team.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "";
            string[] args = arguments.ToArray();
            if(sender is PlayerCommandSender)
            {
                Player player = Player.Get(((CommandSender)sender).SenderId);
                if (args.Length != 1)
                {
                    response = Buddy.singleton.Config.GetLang("invalidUsage");
                    return true;
                }
                try
                {
                    response = HandleBuddyCommand(player, args);
                    return true;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    response = Buddy.singleton.Config.GetLang("errorMessage");
                }
            }
            return true;
        }

        private string HandleBuddyCommand(Player p, string[] args)
        {
            //get the player who the request was sent to
            Player buddy = null;
            string lower = args[0].ToLower();
            foreach (Player player in Player.List)
            {
                if (player == null) continue;
                if (player.Nickname.ToLower().Contains(lower) && player.UserId != p.UserId)
                {
                    buddy = player;
                    break;
                }
            }
            if (buddy == null)
            {
                return Buddy.singleton.Config.GetLang("playerNotFoundMessage");
            }
            if(Buddy.singleton.buddyRequests.ContainsKey(p.UserId) && Buddy.singleton.buddyRequests.TryGetValue(p.UserId, out List<Player> buddies) && buddies.Where((player) => player.UserId == buddy.UserId).Any() && !Buddy.singleton.buddies.ContainsKey(buddy.UserId))
            {
                Buddy.singleton.buddies[p.UserId] = buddy.UserId;
                Buddy.singleton.buddies[buddy.UserId] = p.UserId;
                Buddy.singleton.buddyRequests.Remove(p.UserId);
                buddy.SendConsoleMessage(Buddy.singleton.Config.GetLang("buddyRequestAcceptMessage").Replace("$name", p.Nickname), "yellow");
                if (Buddy.singleton.Config.SendBuddyAcceptedBroadcast)
                    buddy.Broadcast(5, Buddy.singleton.Config.GetLang("buddyRequestAcceptMessage").Replace("$name", p.Nickname), Broadcast.BroadcastFlags.Normal);
                return Buddy.singleton.Config.GetLang("successMessage");
            }
            Buddy.singleton.buddyRequests[buddy.UserId].Add(p);
            buddy.SendConsoleMessage(Buddy.singleton.Config.GetLang("BuddyMessagePrompt").Replace("$name", p.Nickname), "yellow");
            if (Buddy.singleton.Config.SendBuddyRequestBroadcast && !Round.IsStarted)
                buddy.Broadcast(5, Buddy.singleton.Config.GetLang("broadcastBuddyRequest").Replace("$name", p.Nickname), Broadcast.BroadcastFlags.Normal);
            return Buddy.singleton.Config.GetLang("buddyRequestSentMessage");
        }
    }
}
