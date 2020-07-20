using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace Buddy
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class UnBuddyCommand : ICommand
    {
        public string Command => Buddy.singleton.getLang("buddyUnbuddyCommand");

        public string[] Aliases => null;

        public string Description => "A command to remove your current buddy.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "";
            string[] args = arguments.ToArray();
            if (sender is PlayerCommandSender)
            {
                Player player = Player.Get(((CommandSender)sender).SenderId);
                response = handleUnBuddyCommand(player);
                return true;
            }
            return true;
        }

        private string handleUnBuddyCommand(Player p)
        {
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
            return Buddy.singleton.getLang("unBuddySuccess");
        }
    }
}
