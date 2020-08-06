using System;
using System.Collections.Generic;
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
                response = HandleBuddyAcceptCommand(player, arguments.ToArray());
                return true;
            }
            return true;
        }

        private string HandleBuddyAcceptCommand(Player p, string[] args)
        {
            //checks
            if (!Buddy.singleton.buddyRequests.ContainsKey(p.UserId))
            {
                return Buddy.singleton.Config.GetLang("noBuddyRequestsMessage");
            }

            //set the buddy
            Player buddy = null;
            try
            {
                if (!Buddy.singleton.buddyRequests.TryGetValue(p.UserId, out List<Player> buddies) || buddies == null) return Buddy.singleton.Config.GetLang("errorMessage");
                if (args.Length != 1) buddy = buddies.Last();
                else
                {
                    string lower = args[0].ToLower();
                    foreach(Player player in buddies)
                    {
                        if (player == null) continue;
                        if (player.Nickname.ToLower().Contains(lower) && player.UserId != p.UserId)
                        {
                            buddy = player;
                            break;
                        }
                    }
                }
            }
            catch (ArgumentNullException e)
            {
                Log.Error(e.ToString());
                return Buddy.singleton.Config.GetLang("errorMessage");
            }
            if (buddy == null || (buddy != null && Buddy.singleton.buddies.ContainsKey(buddy.UserId)))
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
