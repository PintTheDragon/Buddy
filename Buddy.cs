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
    version = "1.1.5",
    SmodMajor = 3,
    SmodMinor = 7,
    SmodRevision = 0
    )]
    public class Buddy : Plugin
    {
        [LangOption]
        public readonly string BuddyMessage = "Hey! If you would like to play with a friend, type $buddyCMD <friend's name>.";

        [LangOption]
        public readonly string BuddyMessagePrompt = "Hey! %name wants to play with you. Type %buddyAcceptCMD to accept!";

        [LangOption]
        public readonly string buddyCommand = "buddy";

        [LangOption]
        public readonly string buddyAcceptCommand = "baccept";

        [LangOption]
        public readonly string roundAlreadyStartedMessage = "This command can only be ran before the round starts.";

        [LangOption]
        public readonly string alreadyHaveBuddyMessage = "You already have a buddy.";

        [LangOption]
        public readonly string playerNotFoundMessage = "The player was not found.";

        [LangOption]
        public readonly string buddyRequestSentMessage = "Request sent!";

        [LangOption]
        public readonly string noBuddyRequestsMessage = "You do not have any buddy requests.";

        [LangOption]
        public readonly string errorMessage = "An error occured.";

        [LangOption]
        public readonly string buddyRequestAcceptMessage = "Your buddy request was accepted!";

        [LangOption]
        public readonly string successMessage = "Success!";

        [LangOption]
        public string invalidUsage = "Usage: $buddyCMD <friend's name>";

        public Boolean enabled = true;

        public Boolean forceExactRole = false;

        public Boolean disallowGuardScientistCombo = true;

        public Dictionary<string, Player> buddies = new Dictionary<string, Player>();

        public Dictionary<string, Player> buddyRequests = new Dictionary<string, Player>();

        public string prefixedMessage = "";

        public override void OnDisable()
        {
            this.Info(this.Details.name + " v" + this.Details.version + " (by PintTheDragon) has unloaded.");
        }

        public override void OnEnable()
        {
            if (!this.enabled) return;
            this.Info(this.Details.name + " v" + this.Details.version + " (by PintTheDragon) has loaded.");
        }

        public override void Register()
        {
            this.AddConfig(new ConfigSetting("buddy_enabled", enabled, true, "Enables/disables the plugin."));
            this.AddConfig(new ConfigSetting("buddy_force_exact_role", forceExactRole, true, "Makes a player the exact role as their buddy."));
            this.AddConfig(new ConfigSetting("buddy_disallow_guard_scientist_combo", disallowGuardScientistCombo, true, "If true, buddies will never spawn in as a guard and scientist. Only both a guard or both a scientist."));
            this.enabled = this.GetConfigBool("buddy_enabled");
            this.forceExactRole = this.GetConfigBool("buddy_force_exact_role");
            this.disallowGuardScientistCombo = this.GetConfigBool("buddy_disallow_guard_scientist_combo");
            if (!this.enabled)
            {
                this.OnDisable();
                this.Info("Disregard any further messages about the plugin being enabled. It has been disabled.");
                return;
            }
            this.AddEventHandler(typeof(IEventHandlerRoundStart), new RoundStartHandler(this), Priority.Highest);
            this.AddEventHandler(typeof(IEventHandlerCallCommand), new CommandHandler(this), Priority.Normal);
            this.AddEventHandler(typeof(IEventHandlerPlayerJoin), new JoinHandler(this), Priority.Normal);
            this.AddEventHandler(typeof(IEventHandlerRoundRestart), new RoundRestartHandler(this), Priority.Normal);
            this.prefixedMessage = BuddyMessage.Replace("$buddyCMD", "."+buddyCommand);
            this.invalidUsage = invalidUsage.Replace("$buddyCMD", "." + buddyCommand);
        }
    }
}
