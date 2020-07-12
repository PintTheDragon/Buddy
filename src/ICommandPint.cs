using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buddy
{
    interface ICommandPint
    {
        string Command { get; }

        string[] Aliases { get; }

        string Description { get; }

        bool Execute(ArraySegment<string> arguments, Player sender, out string response);
    }
}
