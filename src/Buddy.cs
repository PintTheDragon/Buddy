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

        public EventHandlers EventHandlers;

        public Dictionary<string, string> buddies = new Dictionary<string, string>();

        public Dictionary<string, Player> buddyRequests = new Dictionary<string, Player>();

        public static Buddy singleton;

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnRoundStart;
            Exiled.Events.Handlers.Player.Joined -= EventHandlers.OnPlayerJoin;
            Exiled.Events.Handlers.Server.RestartingRound -= EventHandlers.OnRoundRestart;
            Exiled.Events.Handlers.Server.ReloadedConfigs -= Config.OnReload;

            base.OnDisabled();
        }

        public override void OnEnabled()
        {
            singleton = this;

            this.setLang("BuddyMessage", this.getLang("BuddyMessage").Replace("$buddyCMD", "." + this.getLang("buddyCommand")));
            this.setLang("invalidUsage", this.getLang("invalidUsage").Replace("$buddyCMD", "." + this.getLang("buddyCommand")));
            this.setLang("buddyRequestAcceptMessage", this.getLang("buddyRequestAcceptMessage").Replace("$unBuddyCMD", "." + this.getLang("buddyUnbuddyCommand")));
            this.setLang("successMessage", this.getLang("successMessage").Replace("$unBuddyCMD", "." + this.getLang("buddyUnbuddyCommand")));
            EventHandlers = new EventHandlers(this);
            Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStart;
            Exiled.Events.Handlers.Player.Joined += EventHandlers.OnPlayerJoin;
            Exiled.Events.Handlers.Server.RestartingRound += EventHandlers.OnRoundRestart;
            Exiled.Events.Handlers.Server.ReloadedConfigs += Config.OnReload;

            base.OnEnabled();
        }

        public override void OnReloaded()
        {
            base.OnReloaded();
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
            if (!Config.Messages.TryGetValue(key, out outVal)) return "";
            return outVal;
        }

        public void setLang(string key, string value)
        {
            Config.Messages[key] = value;
        }
    }
}
