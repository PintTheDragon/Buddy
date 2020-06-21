using EXILED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buddy
{
    class BuddyPluginEXILED : EXILED.Plugin
    {
        public override string getName => "Buddy";

        public EventHandlersEXILED EventHandlers;

        public string BuddyMessage = "Hey! If you would like to play with a friend, type $buddyCMD <friend's name>.";

        public  string BuddyMessagePrompt = "Hey! %name wants to play with you. Type %buddyAcceptCMD to accept!";

        public string buddyCommand = "buddy";

        public string buddyAcceptCommand = "baccept";

        public string roundAlreadyStartedMessage = "This command can only be ran before the round starts.";

        public string alreadyHaveBuddyMessage = "You already have a buddy.";

        public string playerNotFoundMessage = "The player was not found.";

        public string buddyRequestSentMessage = "Request sent!";

        public string noBuddyRequestsMessage = "You do not have any buddy requests.";

        public string errorMessage = "An error occured.";

        public string buddyRequestAcceptMessage = "Your buddy request was accepted!";

        public string successMessage = "Success!";

        public Boolean enabled = true;

        public Boolean forceExactRole = false;

        public Boolean disallowGuardScientistCombo = true;

        public Dictionary<string, ReferenceHub> buddies = new Dictionary<string, ReferenceHub>();

        public Dictionary<string, ReferenceHub> buddyRequests = new Dictionary<string, ReferenceHub>();

        public string prefixedMessage = "";

        public override void OnDisable()
        {
            Events.RoundStartEvent -= EventHandlers.OnRoundStart;
            Events.PlayerJoinEvent -= EventHandlers.OnPlayerJoin;
            Events.ConsoleCommandEvent -= EventHandlers.OnConsoleCommand;
            Log.Info("Buddy (by PintTheDragon) has unloaded.");
        }

        public override void OnEnable()
        {
            //Need to add config later

            /*
            this.AddConfig(new ConfigSetting("buddy_enabled", enabled, true, "Enables/disables the plugin."));
            this.AddConfig(new ConfigSetting("buddy_force_exact_role", forceExactRole, true, "Makes a player the exact role as their buddy."));
            this.AddConfig(new ConfigSetting("buddy_disallow_guard_scientist_combo", disallowGuardScientistCombo, true, "If true, buddies will never spawn in as a guard and scientist. Only both a guard or both a scientist."));
            this.enabled = this.GetConfigBool("buddy_enabled");
            this.forceExactRole = this.GetConfigBool("buddy_force_exact_role");
            this.disallowGuardScientistCombo = this.GetConfigBool("buddy_disallow_guard_scientist_combo");
            */
            if (!this.enabled)
            {
                this.OnDisable();
                Log.Info("Disregard any further messages about the plugin being enabled. It has been disabled.");
                return;
            }
            EventHandlers = new EventHandlersEXILED(this);
            Events.RoundStartEvent += EventHandlers.OnRoundStart;
            Events.PlayerJoinEvent += EventHandlers.OnPlayerJoin;
            Events.ConsoleCommandEvent += EventHandlers.OnConsoleCommand;
            this.prefixedMessage = BuddyMessage.Replace("$buddyCMD", "." + buddyCommand);
        }

        public override void OnReload()
        {
            //Really nothing to do here for me
        }
    }
}
