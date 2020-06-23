﻿using Exiled.API.Features;
using System;
using System.Collections.Generic;

namespace Buddy
{
    class BuddyPlugin : Plugin<Config>
    {
        public string VERSION = "1.1.5";

        public EventHandlers EventHandlers;

        public string BuddyMessage = "Hey! If you would like to play with a friend, type $buddyCMD <friend's name>.";

        public string BuddyMessagePrompt = "Hey! %name wants to play with you. Type %buddyAcceptCMD to accept!";

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

        public Dictionary<string, Player> buddies = new Dictionary<string, Player>();

        public Dictionary<string, Player> buddyRequests = new Dictionary<string, Player>();

        public string prefixedMessage = "";

        private bool shouldSetRoundStartedTrue = false;

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnRoundStart;
            Exiled.Events.Handlers.Player.Joined -= EventHandlers.OnPlayerJoin;
            Exiled.Events.Handlers.Server.SendingConsoleCommand -= EventHandlers.OnConsoleCommand;
            Exiled.Events.Handlers.Server.RestartingRound -= EventHandlers.OnRoundRestart;
            Log.Info("Buddy v"+VERSION+" (by PintTheDragon) has unloaded.");
        }

        public override void OnEnabled()
        {
            
            if (!Config.IsEnabled)
            {
                this.OnDisabled();
                Log.Info("Disregard any further messages about the plugin being enabled. It has been disabled.");
                return;
            }
            this.prefixedMessage = this.BuddyMessage.Replace("$buddyCMD", "." + buddyCommand);
            this.invalidUsage = this.invalidUsage.Replace("$buddyCMD", "." + buddyCommand);
            EventHandlers = new EventHandlers(this);
            if (shouldSetRoundStartedTrue)
            {
                EventHandlers.RoundStarted = true;
                shouldSetRoundStartedTrue = false;
            }
            Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStart;
            Exiled.Events.Handlers.Player.Joined += EventHandlers.OnPlayerJoin;
            Exiled.Events.Handlers.Server.SendingConsoleCommand += EventHandlers.OnConsoleCommand;
            Exiled.Events.Handlers.Server.RestartingRound += EventHandlers.OnRoundRestart;
            Log.Info("Buddy v" + VERSION + " (by PintTheDragon) has loaded.");
        }

        public override void OnReloaded()
        {
            //I'm going to assume a reload happens during the middle of a game
            shouldSetRoundStartedTrue = true;
        }
    }
}
