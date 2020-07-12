using CommandSystem;
using Exiled.Events.EventArgs;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buddy
{
    class CommandHandler
    {
        private static Dictionary<string, ICommandPint> commands = new Dictionary<string, ICommandPint>();

        public static bool isEnabled = false;

        public static void Register(ICommandPint cmd)
        {
            string commandStr = cmd.Command;
            commands.Add(commandStr, cmd);
        }

        public static void RunCommand(SendingConsoleCommandEventArgs ev)
        {
            if (!isEnabled) return;
            ICommandPint cmd = null;
            if (!commands.TryGetValue(ev.Name, out cmd) || cmd == null) return;
            string outText = ev.ReturnMessage;
            cmd.Execute(new ArraySegment<string>(ev.Arguments.ToArray()), ev.Player, out outText);
            ev.ReturnMessage = outText;
        }
    }
}
