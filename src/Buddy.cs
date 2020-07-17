using Exiled.API.Enums;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Buddy
{
    class Buddy : Plugin<Config>
    {
        //plugins that deal with spawning (like scp-035) will break if this is not highest priority
        public override PluginPriority Priority => PluginPriority.Highest;

        public override Version Version { get; } = new Version(1, 1, 8);

        public EventHandlers EventHandlers;

        public Dictionary<string, string> lang = new Dictionary<string, string> {
            { "BuddyMessage", "Hey! If you would like to play with a friend, type $buddyCMD <friend's name>."},
            { "BuddyMessagePrompt", "Hey! $name wants to play with you. Type $buddyAcceptCMD to accept!"},
            { "buddyCommand", "buddy"},
            { "buddyAcceptCommand", "baccept"},
            { "buddyUnbuddyCommand", "unbuddy"},
            { "alreadyHaveBuddyMessage", "You already have a buddy."},
            { "playerNotFoundMessage", "The player was not found."},
            { "buddyRequestSentMessage", "Request sent!"},
            { "noBuddyRequestsMessage", "You do not have any buddy requests."},
            { "errorMessage", "An error occured."},
            { "buddyRequestAcceptMessage", "$name accepted your buddy request! Type $unBuddyCMD to get rid of your buddy."},
            { "successMessage", "Success! Type $unBuddyCMD to get rid of your buddy."},
            { "invalidUsage", "Usage: $buddyCMD <friend's name>"},
            { "unBuddySuccess", "You no longer have a buddy."},
            { "useBuddyCommandBroadcast", "If you want to play on the same team as a friend, open up your console with the ~ key."},
            { "broadcastBuddy", "Your buddy is $buddy."},
            { "broadcastBuddyRequest", "$name wants to play with you. Open the console with ~ to accept their request."},
        };

        public Dictionary<string, string> buddies = new Dictionary<string, string>();

        public Dictionary<string, Player> buddyRequests = new Dictionary<string, Player>();

        public string prefixedMessage = "";

        public static Buddy singleton;

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnRoundStart;
            Exiled.Events.Handlers.Player.Joined -= EventHandlers.OnPlayerJoin;
            Exiled.Events.Handlers.Server.RestartingRound -= EventHandlers.OnRoundRestart;
            Exiled.Events.Handlers.Server.SendingConsoleCommand -= EventHandlers.OnConsoleCommand;
            CommandHandler.isEnabled = false;

            Log.Info("Buddy v"+Version+" (by PintTheDragon) has unloaded.");
        }

        public override void OnEnabled()
        {
            singleton = this;
            this.lang = Config.Messages;

            this.prefixedMessage = this.getLang("BuddyMessage").Replace("$buddyCMD", "." + this.getLang("buddyCommand"));
            this.setLang("invalidUsage", this.getLang("invalidUsage").Replace("$buddyCMD", "." + this.getLang("buddyCommand")));
            this.setLang("buddyRequestAcceptMessage", this.getLang("buddyRequestAcceptMessage").Replace("$unBuddyCMD", "." + this.getLang("buddyUnbuddyCommand")));
            this.setLang("successMessage", this.getLang("successMessage").Replace("$unBuddyCMD", "." + this.getLang("buddyUnbuddyCommand")));
            EventHandlers = new EventHandlers(this);
            Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStart;
            Exiled.Events.Handlers.Player.Joined += EventHandlers.OnPlayerJoin;
            Exiled.Events.Handlers.Server.RestartingRound += EventHandlers.OnRoundRestart;
            Exiled.Events.Handlers.Server.SendingConsoleCommand += EventHandlers.OnConsoleCommand;

            CommandHandler.isEnabled = true;
            CommandHandler.Register(new BuddyCommand());
            CommandHandler.Register(new BuddyAcceptCommand());
            CommandHandler.Register(new UnBuddyCommand());

            Log.Info("Buddy v" + Version + " (by PintTheDragon) has loaded.");
        }

        public override void OnReloaded()
        {
        }

        public void removePerson(string userID)
        {
            try
            {
                foreach (var item in buddies.Where(x => x.Value == userID).ToList())
                {
                    try
                    {
                        buddies.Remove(item.Key);
                    }
                    catch (ArgumentException) { }
                }
            }
            catch (ArgumentException) { }
        }

        public string getLang(string key)
        {
            string outVal = "";
            if (!this.lang.TryGetValue(key, out outVal)) return "";
            return outVal;
        }

        public void setLang(string key, string value)
        {
            this.lang.Remove(key);
            this.lang.Add(key, value);
        }
    }
}
