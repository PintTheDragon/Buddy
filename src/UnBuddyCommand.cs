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
        public string Command => Buddy.singleton.Config.GetLang("buddyUnbuddyCommand");

        public string[] Aliases => null;

        public string Description => "A command to remove your current buddy.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "";
            if (sender is PlayerCommandSender)
            {
                Player player = Player.Get(((CommandSender)sender).SenderId);
                response = HandleUnBuddyCommand(player);
                return true;
            }
            return true;
        }

        private string HandleUnBuddyCommand(Player p)
        {
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
            return Buddy.singleton.Config.GetLang("unBuddySuccess");
        }
    }
}
