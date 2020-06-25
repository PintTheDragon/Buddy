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
        public string VERSION = "1.1.6";

        public override string getName => "Buddy";

        public EventHandlers EventHandlers;

        public string BuddyMessage = "Hey! If you would like to play with a friend, type $buddyCMD <friend's name>.";

        public  string BuddyMessagePrompt = "Hey! %name wants to play with you. Type %buddyAcceptCMD to accept!";

        public string buddyCommand = "buddy";

        public string buddyAcceptCommand = "baccept";

        public string buddyUnbuddyCommand = "unbuddy";

        public string alreadyHaveBuddyMessage = "You already have a buddy.";

        public string playerNotFoundMessage = "The player was not found.";

        public string buddyRequestSentMessage = "Request sent!";

        public string noBuddyRequestsMessage = "You do not have any buddy requests.";

        public string errorMessage = "An error occured.";

        public string buddyRequestAcceptMessage = "Your buddy request was accepted! Type $unBuddyCMD to get rid of your buddy.";

        public string successMessage = "Success! Type $unBuddyCMD to get rid of your buddy.";

        public string invalidUsage = "Usage: $buddyCMD <friend's name>";

        public string unBuddySuccess = "You no longer have a buddy.";

        public string useBuddyCommandBroadcast = "If you want to play on the same team as a friend, open up your console with the ~ key.";

        public string broadcastBuddy = "Your buddy is $buddy.";

        public string broadcastBuddyRequest = "$name wants to play with you. Open the console with ~ to accept their request.";

        public Boolean enabled = true;

        public Boolean forceExactRole = false;

        public Boolean disallowGuardScientistCombo = true;

        public Boolean resetBuddiesEveryRound = true;

        public Boolean sendInfoBroadcast = true;

        public Boolean sendBuddyBroadcast = true;

        public Boolean sendBuddyRequestBroadcast = true;

        public Dictionary<string, string> buddies = new Dictionary<string, string>();

        public Dictionary<string, ReferenceHub> buddyRequests = new Dictionary<string, ReferenceHub>();

        public string prefixedMessage = "";

        public override void OnDisable()
        {
            Events.RoundStartEvent -= EventHandlers.OnRoundStart;
            Events.PlayerJoinEvent -= EventHandlers.OnPlayerJoin;
            Events.ConsoleCommandEvent -= EventHandlers.OnConsoleCommand;
            Events.RoundRestartEvent -= EventHandlers.OnRoundRestart;
            Log.Info("Buddy v" + VERSION + " (by PintTheDragon) has unloaded.");
        }

        public override void OnEnable()
        {
            this.enabled = Config.GetBool("buddy_enabled", this.enabled);
            this.forceExactRole = Config.GetBool("buddy_force_exact_role", this.forceExactRole);
            this.disallowGuardScientistCombo = Config.GetBool("buddy_disallow_guard_scientist_combo", this.disallowGuardScientistCombo);
            this.resetBuddiesEveryRound = Config.GetBool("buddy_reset_buddies_every_round", this.resetBuddiesEveryRound);
            this.sendInfoBroadcast = Config.GetBool("buddy_send_info_broadcast", this.sendInfoBroadcast);
            this.sendBuddyBroadcast = Config.GetBool("buddy_send_buddy_broadcast", this.sendBuddyBroadcast);
            this.sendBuddyRequestBroadcast = Config.GetBool("buddy_send_buddy_request_broadcast", this.sendBuddyRequestBroadcast);

            if (!this.enabled)
            {
                this.OnDisable();
                Log.Info("Disregard any further messages about the plugin being enabled. It has been disabled.");
                return;
            }
            this.prefixedMessage = this.BuddyMessage.Replace("$buddyCMD", "." + buddyCommand);
            this.invalidUsage = this.invalidUsage.Replace("$buddyCMD", "." + buddyCommand);
            this.buddyRequestAcceptMessage = this.buddyRequestAcceptMessage.Replace("$unBuddyCMD", "." + buddyUnbuddyCommand);
            this.successMessage = this.successMessage.Replace("$unBuddyCMD", "." + buddyUnbuddyCommand);
            EventHandlers = new EventHandlers(this);
            Events.RoundStartEvent += EventHandlers.OnRoundStart;
            Events.PlayerJoinEvent += EventHandlers.OnPlayerJoin;
            Events.ConsoleCommandEvent += EventHandlers.OnConsoleCommand;
            Events.RoundRestartEvent += EventHandlers.OnRoundRestart;
            Log.Info("Buddy v" + VERSION + " (by PintTheDragon) has loaded.");
        }

        public override void OnReload()
        {
            
        }
    }
}
