using EXILED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buddy
{
    class Buddy : EXILED.Plugin
    {
        public string VERSION = "1.1.3";

        public override string getName => "Buddy";

        public EventHandlers EventHandlers;

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

        public string invalidUsage = "Usage: $buddyCMD <friend's name>";

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
            Log.Info("Buddy v" + VERSION + " (by PintTheDragon) has unloaded.");
        }

        public override void OnEnable()
        {
            this.enabled = Config.GetBool("buddy_enabled", this.enabled);
            this.forceExactRole = Config.GetBool("buddy_force_exact_role", this.forceExactRole);
            this.disallowGuardScientistCombo = Config.GetBool("buddy_disallow_guard_scientist_combo", this.disallowGuardScientistCombo);
            
            if (!this.enabled)
            {
                this.OnDisable();
                Log.Info("Disregard any further messages about the plugin being enabled. It has been disabled.");
                return;
            }
            this.prefixedMessage = this.BuddyMessage.Replace("$buddyCMD", "." + buddyCommand);
            this.invalidUsage = this.invalidUsage.Replace("$buddyCMD", "." + buddyCommand);
            EventHandlers = new EventHandlers(this);
            Events.RoundStartEvent += EventHandlers.OnRoundStart;
            Events.PlayerJoinEvent += EventHandlers.OnPlayerJoin;
            Events.ConsoleCommandEvent += EventHandlers.OnConsoleCommand;
            Log.Info("Buddy v" + VERSION + " (by PintTheDragon) has loaded.");
        }

        public override void OnReload()
        {
            //Really nothing to do here for me
        }
    }
}
