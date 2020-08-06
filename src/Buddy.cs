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

        public Dictionary<string, List<Player>> buddyRequests = new Dictionary<string, List<Player>>();

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

            Config.OnReload();

            EventHandlers = new EventHandlers();
            Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStart;
            Exiled.Events.Handlers.Player.Joined += EventHandlers.OnPlayerJoin;
            Exiled.Events.Handlers.Server.RestartingRound += EventHandlers.OnRoundRestart;
            Exiled.Events.Handlers.Server.ReloadedConfigs += Config.OnReload;

            base.OnEnabled();
        }

        public override void OnReloaded()
        {
            Config.OnReload();

            base.OnReloaded();
        }

        public void RemovePerson(string userID)
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
    }
}
