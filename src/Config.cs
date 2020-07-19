using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using Exiled.Events.EventArgs;

namespace Buddy
{
    class Config : IConfig
    {
        [Description("Enables/disables the plugin.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Makes a player the exact role as their buddy.")]
        public bool forceExactRole { get; set; } = false;

        [Description("If true, buddies will never spawn in as a guard and scientist. Only both a guard or both a scientist.")]
        public bool disallowGuardScientistCombo { get; set; } = true;

        [Description("Should buddies be reset every round.")]
        public bool resetBuddiesEveryRound { get; set; } = true;

        [Description("Should a broadcast be sent be sent telling players how to use the plugin.")]
        public bool sendInfoBroadcast { get; set; } = true;

        [Description("Should a broadcast be sent be sent telling players who their buddy is..")]
        public bool sendBuddyBroadcast { get; set; } = true;

        [Description("Should a broadcast be sent be sent to a player when someone requests to be their buddy.")]
        public bool sendBuddyRequestBroadcast { get; set; } = true;

        [Description("Should a broadcast be sent be sent telling players that their buddy request was accepted.")]
        public bool sendBuddyAcceptedBroadcast { get; set; } = true;

        [Description("Change the messages that Buddy sends to players.")]
        public Dictionary<string, string> Messages { get; set; } = new Dictionary<string, string> {
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

        public void OnReload()
        {
            Buddy.singleton.setLang("BuddyMessage", Buddy.singleton.getLang("BuddyMessage").Replace("$buddyCMD", "." + Buddy.singleton.getLang("buddyCommand")));
            Buddy.singleton.setLang("invalidUsage", Buddy.singleton.getLang("invalidUsage").Replace("$buddyCMD", "." + Buddy.singleton.getLang("buddyCommand")));
            Buddy.singleton.setLang("buddyRequestAcceptMessage", Buddy.singleton.getLang("buddyRequestAcceptMessage").Replace("$unBuddyCMD", "." + Buddy.singleton.getLang("buddyUnbuddyCommand")));
            Buddy.singleton.setLang("successMessage", Buddy.singleton.getLang("successMessage").Replace("$unBuddyCMD", "." + Buddy.singleton.getLang("buddyUnbuddyCommand")));
        }
    }
}
