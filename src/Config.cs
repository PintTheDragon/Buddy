using Exiled.API.Interfaces;
using System.ComponentModel;

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
    }
}
