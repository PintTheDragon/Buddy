using Smod2.API;
using Smod2.Commands;
using Smod2.EventHandlers;
using Smod2.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PintBuddy
{
    internal class CommandHandler : IEventHandlerCallCommand
    {
        private BuddyPlugin buddyPlugin;

        public CommandHandler(BuddyPlugin buddyPlugin)
        {
            this.buddyPlugin = buddyPlugin;
        }

        public void OnCallCommand(PlayerCallCommandEvent ev)
        {
            string[] cmd = ev.Command.Split(' ');

            //run command handlers
            if (cmd[0].ToLower().Equals(buddyPlugin.buddyCommand))
            {
                string[] args = cmd.Skip(1).ToArray<string>();
                ev.ReturnMessage = handleBuddyCommand(ev.Player, args);
                return;
            }
            if (cmd[0].ToLower().Equals(buddyPlugin.buddyAcceptCommand))
            {
                string[] args = cmd.Skip(1).ToArray<string>();
                ev.ReturnMessage = handleBuddyAcceptCommand(ev.Player, args);
                return;
            }
        }

        private string handleBuddyCommand(Player p, string[] args)
        {
            //checks
            if (buddyPlugin.Round.Duration != 0)
            {
                return "This command can only be ran before the round starts.";
            }
            if (buddyPlugin.buddies.ContainsKey(p.UserId))
            {
                return "You already have a buddy." ;
            }

            //get the player who the request was sent to
            List<Player> players = buddyPlugin.Server.GetPlayers();
            Player buddy = null;
            string lower = args[0].ToLower();
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Name.ToLower().Contains(lower) && players[i].UserId != p.UserId)
                {
                    buddy = players[i];
                    break;
                }
            }
            if (buddy == null)
            {
                return "The player was not found.";
            }

            if (buddyPlugin.buddyRequests.ContainsKey(buddy.UserId)) buddyPlugin.buddyRequests.Remove(buddy.UserId);
            buddyPlugin.buddyRequests.Add(buddy.UserId, p);
            buddy.SendConsoleMessage(buddyPlugin.BuddyMessagePrompt.Replace("%name", p.Name).Replace("%buddyAcceptCMD", "."+buddyPlugin.buddyAcceptCommand), "yellow");
            return "Request sent!";
        }

        private string handleBuddyAcceptCommand(Player p, string[] args)
        {
            //checks
            if (buddyPlugin.Round.Duration != 0)
            {
                return "This command can only be ran before the round starts.";
            }
            if (!buddyPlugin.buddyRequests.ContainsKey(p.UserId))
            {
                return "You do not have any buddy requests.";
            }
            if (buddyPlugin.buddies.ContainsKey(p.UserId))
            {
                return "You already have a buddy.";
            }

            //set the buddy
            Player buddy = null;
            try
            {
                buddyPlugin.buddyRequests.TryGetValue(p.UserId, out buddy);
            }
            catch (ArgumentNullException e)
            {
                buddyPlugin.Error(e.ToString());
                return "An error occured.";
            }
            if (buddy == null)
            {
                return "An error occured.";

            }
            buddyPlugin.buddies.Add(p.UserId, buddy);
            buddyPlugin.buddies.Add(buddy.UserId, p);
            buddyPlugin.buddyRequests.Remove(p.UserId);
            buddy.SendConsoleMessage("Your buddy request was accepted!", "yellow");
            return "Success!";
        }
    }
}