using Smod2;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Lang;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PintBuddy
{
    [PluginDetails(
    author = "PintTheDragon",
    name = "Buddy",
    description = "A plugin to let you play with a friend.",
    id = "com.PintTheDragon.BuddyPlugin",
    configPrefix = "buddy",
    langFile = "buddyplugin",
    version = "1.0",
    SmodMajor = 3,
    SmodMinor = 7,
    SmodRevision = 0
    )]
    public class BuddyPlugin : Plugin
    {
        [LangOption]
        public readonly string BuddyMessage = "Hey! If you would like to play with a friend, type $buddyCMD <friend's name>.";

        [LangOption]
        public readonly string BuddyMessagePrompt = "Hey! %name wants to play with you. Type %buddyAcceptCMD to accept!";

        [ConfigOption]
        public readonly Boolean enabled = true;

        [ConfigOption]
        public readonly Boolean forceExactRole = false;

        [ConfigOption]
        public readonly string buddyCommand = "buddy";

        [ConfigOption]
        public readonly string buddyAcceptCommand = "baccept";

        public Dictionary<string, Player> buddies = new Dictionary<string, Player>();

        public Dictionary<string, Player> buddyRequests = new Dictionary<string, Player>();

        public string prefixedMessage = "";

        public override void OnDisable()
        {
            this.Info(this.Details.name + "(by PintTheDragon) has unloaded.");
        }

        public override void OnEnable()
        {
            if (!enabled) return;
            this.Info(this.Details.name + "(by PintTheDragon) has loaded.");
        }

        public override void Register()
        {
            if (!enabled) return;
            this.AddEventHandler(typeof(IEventHandlerRoundStart), new RoundStartHandler(this), Priority.Highest);
            this.AddEventHandler(typeof(IEventHandlerCallCommand), new CommandHandler(this), Priority.Normal);
            this.AddEventHandler(typeof(IEventHandlerPlayerJoin), new JoinHandler(this), Priority.Normal);
            prefixedMessage = BuddyMessage.Replace("$buddyCMD", "."+buddyCommand);
        }
    }
}
