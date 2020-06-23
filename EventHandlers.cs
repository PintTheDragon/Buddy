using Exiled.API.Features;
using Exiled.Events.EventArgs;
using EXILED.Extensions;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buddy
{
    class EventHandlers
    {
        public BuddyPlugin buddyPlugin;
        public EventHandlers(BuddyPlugin plugin) => this.buddyPlugin = plugin;

        private RoleType[] tmpArr = { RoleType.Scp049, RoleType.Scp079, RoleType.Scp096, RoleType.Scp106, RoleType.Scp173, RoleType.Scp93953, RoleType.Scp93989 };
        private Random rnd = new Random();
        public bool RoundStarted = false;

        public void OnPlayerJoin(JoinedEventArgs ev)
        {
            if (RoundStarted) return;
            Timing.RunCoroutine(sendJoinMessage(ev.Player));
        }

        public IEnumerator<float> sendJoinMessage(Exiled.API.Features.Player p)
        {
            yield return Timing.WaitForSeconds(1f);
            p.SendConsoleMessage(buddyPlugin.prefixedMessage, "yellow");
        }

        public void OnRoundStart()
        {
            RoundStarted = true;
            Timing.RunCoroutine(doTheSCPThing());
        }

        public void OnRoundRestart()
        {
            buddyPlugin.buddies = new Dictionary<string, Exiled.API.Features.Player>();
            RoundStarted = false;
        }

        private IEnumerator<float> doTheSCPThing()
        {
            yield return Timing.WaitForSeconds(1f);
            IEnumerable<Exiled.API.Features.Player> hubs = buddyPlugin.buddies.Values;
            List<String> doneIDs = new List<String>();
            for (int i = 0; i < hubs.Count(); i++)
            {
                Exiled.API.Features.Player player = hubs.ElementAt(i);
                //check if player has a buddy
                if (buddyPlugin.buddies.ContainsKey(player.UserId))
                {
                    try
                    {
                        Exiled.API.Features.Player buddy = null;
                        buddyPlugin.buddies.TryGetValue(player.UserId, out buddy);
                        if (buddy == null) continue;
                        if (doneIDs.Contains(player.UserId) || doneIDs.Contains(buddy.UserId)) continue;
                        //take action if they have different roles
                        if (player.Role != buddy.Role &&
                            /* massive check for scientist/guard combo */
                            !(!buddyPlugin.Config.disallowGuardScientistCombo && ((player.Role == RoleType.FacilityGuard && buddy.Role == RoleType.Scientist) || (player.Role == RoleType.Scientist && buddy.Role == RoleType.FacilityGuard)))
                            )
                        {
                            //SCPs take priority
                            if (buddy.Team == Team.SCP) continue;

                            //if force exact role is on we can just set the buddy to the other player's role
                            if (buddyPlugin.Config.forceExactRole)
                            {
                                buddy.Kill();
                                buddy.SetRole(player.Role);
                                doneIDs.Add(buddy.UserId);
                                doneIDs.Add(player.UserId);
                                continue;
                            }
                            //if they are an scp, we need to remove another scp first
                            if (player.Team == Team.SCP)
                            {
                                //loop through every scp and swap the buddy with one of them
                                Boolean setRole = false;
                                foreach (Exiled.API.Features.Player hub in Exiled.API.Features.Player.List)
                                {
                                    Exiled.API.Features.Player player1 = hub;
                                    //check if the player is an scp
                                    if (player1.UserId != player.UserId && player1.UserId != buddy.UserId && !buddyPlugin.buddies.ContainsKey(player1.UserId) && player1.Team == Team.SCP)
                                    {
                                        //set the buddy to that player's role and set the player to classd
                                        buddy.Kill();
                                        buddy.SetRole(player1.Role);
                                        player1.Kill();
                                        player1.SetRole(RoleType.ClassD);
                                        doneIDs.Add(buddy.UserId);
                                        doneIDs.Add(player.UserId);
                                        setRole = true;
                                        break;
                                    }
                                }
                                if (!setRole)
                                {
                                    List<RoleType> roles = new List<RoleType>(tmpArr);
                                    roles.Remove(player.Role);
                                    buddy.Kill();
                                    buddy.SetRole(roles[rnd.Next(roles.Count)]);
                                    doneIDs.Add(buddy.UserId);
                                    doneIDs.Add(player.UserId);
                                }
                                continue;
                            }
                            //if they are not an scp, we can just set them to the same role as their buddy
                            buddy.Kill();
                            buddy.SetRole(player.Role);
                            doneIDs.Add(buddy.UserId);
                            doneIDs.Add(player.UserId);
                        }
                    }
                    catch (ArgumentException e)
                    {
                        Log.Error(e.ToString());
                    }
                }

            }
        }

        public void OnConsoleCommand(SendingConsoleCommandEventArgs ev)
        {
            string[] args = ev.Arguments.ToArray();

            //run command handlers
            if (ev.Name.ToLower().Equals(buddyPlugin.buddyCommand))
            {
                if (args.Length == 1)
                {
                    ev.ReturnMessage = buddyPlugin.invalidUsage;
                    return;
                }
                try
                {
                    ev.ReturnMessage = handleBuddyCommand(ev.Player, args);
                    return;
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    ev.ReturnMessage = buddyPlugin.errorMessage;
                }
            }
            if (ev.Name.ToLower().Equals(buddyPlugin.buddyAcceptCommand))
            {
                ev.ReturnMessage = handleBuddyAcceptCommand(ev.Player, new string[] { });
                return;
            }
        }

        private string handleBuddyCommand(Exiled.API.Features.Player p, string[] args)
        {
            //checks
            if (RoundStarted)
            {
                return buddyPlugin.roundAlreadyStartedMessage;
            }
            if (buddyPlugin.buddies.ContainsKey(p.UserId))
            {
                return buddyPlugin.alreadyHaveBuddyMessage;
            }

            //get the player who the request was sent to
            Exiled.API.Features.Player buddy = null;
            string lower = args[0].ToLower();
            foreach (Exiled.API.Features.Player hub in Exiled.API.Features.Player.List)
            {
                if (hub.ReferenceHub.nicknameSync.Network_myNickSync.ToLower().Contains(lower) && hub.UserId != p.UserId)
                {
                    buddy = hub;
                    break;
                }
            }
            if (buddy == null)
            {
                return buddyPlugin.playerNotFoundMessage;
            }

            if (buddyPlugin.buddyRequests.ContainsKey(buddy.UserId)) buddyPlugin.buddyRequests.Remove(buddy.UserId);
            buddyPlugin.buddyRequests.Add(buddy.UserId, p);
            buddy.SendConsoleMessage(buddyPlugin.BuddyMessagePrompt.Replace("%name", p.ReferenceHub.nicknameSync.Network_myNickSync).Replace("%buddyAcceptCMD", "." + buddyPlugin.buddyAcceptCommand), "yellow");
            return buddyPlugin.buddyRequestSentMessage;
        }

        private string handleBuddyAcceptCommand(Exiled.API.Features.Player p, string[] args)
        {
            //checks
            if (RoundStarted)
            {
                return buddyPlugin.roundAlreadyStartedMessage;
            }
            if (!buddyPlugin.buddyRequests.ContainsKey(p.UserId))
            {
                return buddyPlugin.noBuddyRequestsMessage;
            }
            if (buddyPlugin.buddies.ContainsKey(p.UserId))
            {
                return buddyPlugin.alreadyHaveBuddyMessage;
            }

            //set the buddy
            Exiled.API.Features.Player buddy = null;
            try
            {
                buddyPlugin.buddyRequests.TryGetValue(p.UserId, out buddy);
            }
            catch (ArgumentNullException e)
            {
                Log.Error(e.ToString());
                return buddyPlugin.errorMessage;
            }
            if (buddy == null)
            {
                return buddyPlugin.errorMessage;

            }
            buddyPlugin.buddies.Add(p.UserId, buddy);
            buddyPlugin.buddies.Add(buddy.UserId, p);
            buddyPlugin.buddyRequests.Remove(p.UserId);
            buddy.SendConsoleMessage(buddyPlugin.buddyRequestAcceptMessage, "yellow");
            return buddyPlugin.successMessage;
        }

    }
}
