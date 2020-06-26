using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Exiled.API.Extensions;
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
        public Buddy buddyPlugin;
        public EventHandlers(Buddy plugin) => this.buddyPlugin = plugin;

        private RoleType[] tmpArr = { RoleType.Scp049, RoleType.Scp079, RoleType.Scp096, RoleType.Scp106, RoleType.Scp173, RoleType.Scp93953, RoleType.Scp93989 };
        private Random rnd = new Random();
        public bool RoundStarted = false;

        private void removePerson(string userID)
        {
            try
            {
                foreach (var item in buddyPlugin.buddies.Where(x => x.Value == userID).ToList())
                {
                    buddyPlugin.buddies.Remove(item.Key);
                }
            }
            catch (ArgumentException)
            {

            }
        }

        public void OnPlayerJoin(JoinedEventArgs ev)
        {
            Timing.RunCoroutine(sendJoinMessage(ev.Player));
            Timing.RunCoroutine(sendBroadcast(ev.Player));
        }

        public IEnumerator<float> sendJoinMessage(Exiled.API.Features.Player p)
        {
            yield return Timing.WaitForSeconds(1f);
            if (!buddyPlugin.buddies.ContainsKey(p.UserId))
            {
                p.SendConsoleMessage(buddyPlugin.prefixedMessage, "yellow");
            }
            else
            {
                string buddy1 = null;
                buddyPlugin.buddies.TryGetValue(p.UserId, out buddy1);
                if (buddy1 == null)
                {
                    buddyPlugin.buddies.Remove(p.UserId);
                    removePerson(p.UserId);
                }
                else
                {
                    p.SendConsoleMessage(buddyPlugin.broadcastBuddy.Replace("$buddy", Player.Get(buddy1).Nickname), "yellow");
                }
            }
        }

        private IEnumerator<float> sendBroadcast(Exiled.API.Features.Player p)
        {
            yield return Timing.WaitForSeconds(2f);
            if (!buddyPlugin.buddies.ContainsKey(p.UserId) && buddyPlugin.Config.sendInfoBroadcast)
            {
                p.Broadcast(5, buddyPlugin.useBuddyCommandBroadcast, Broadcast.BroadcastFlags.Normal);
            }
            if (buddyPlugin.buddies.ContainsKey(p.UserId) && buddyPlugin.Config.sendBuddyBroadcast)
            {
                string buddy1 = null;
                buddyPlugin.buddies.TryGetValue(p.UserId, out buddy1);
                if (buddy1 == null)
                {
                    buddyPlugin.buddies.Remove(p.UserId);
                    removePerson(p.UserId);
                }
                else
                {
                    p.Broadcast(5, buddyPlugin.broadcastBuddy.Replace("$buddy", Player.Get(buddy1).Nickname), Broadcast.BroadcastFlags.Normal);
                }
            }
        }

        public void OnRoundStart()
        {
            RoundStarted = true;
            Timing.RunCoroutine(doTheSCPThing());
        }

        public void OnRoundRestart()
        {
            RoundStarted = false;
            if (buddyPlugin.Config.resetBuddiesEveryRound)
                buddyPlugin.buddies = new Dictionary<string, string>();
        }

        private IEnumerator<float> doTheSCPThing()
        {
            yield return Timing.WaitForSeconds(1f);

            List<string> doneIDs = new List<string>();
            IEnumerable<string> onlinePlayers = Player.List.Select(x => x.UserId);

            IEnumerable<string> hubs = buddyPlugin.buddies.Values;
            for (int i = 0; i < hubs.Count(); i++)
            {
                string id = hubs.ElementAt(i);
                Player player = Player.Get(id);
                //check if player has a buddy
                if (buddyPlugin.buddies.ContainsKey(player.UserId))
                {
                    try
                    {
                        string buddy1 = null;
                        buddyPlugin.buddies.TryGetValue(player.UserId, out buddy1);
                        if (buddy1 == null || (!onlinePlayers.Contains(id) || !onlinePlayers.Contains(buddy1)))
                        {
                            buddyPlugin.buddies.Remove(id);
                            if (buddy1 != null) buddyPlugin.buddies.Remove(buddy1);
                            else removePerson(id);
                            doneIDs.Add(buddy1);
                            doneIDs.Add(id);
                            continue;
                        }
                        if ((doneIDs.Contains(id) || doneIDs.Contains(buddy1))) continue;
                        Player buddy = Player.Get(buddy1);
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
                                doneIDs.Add(buddy1);
                                doneIDs.Add(id);
                                continue;
                            }
                            //if they are an scp, we need to remove another scp first
                            if (player.Team == Team.SCP)
                            {
                                //loop through every scp and swap the buddy with one of them
                                Boolean setRole = false;
                                foreach (Player hub in Player.List)
                                {
                                    Player player1 = hub;
                                    //check if the player is an scp
                                    if (player1.UserId != id && player1.UserId != buddy1 && !buddyPlugin.buddies.ContainsKey(player1.UserId) && player1.Team == Team.SCP)
                                    {
                                        //set the buddy to that player's role and set the player to classd
                                        buddy.Kill();
                                        buddy.SetRole(player1.Role);
                                        player1.Kill();
                                        player1.SetRole(RoleType.ClassD);
                                        setRole = true;
                                        doneIDs.Add(buddy1);
                                        doneIDs.Add(id);
                                        break;
                                    }
                                }
                                if (!setRole)
                                {
                                    List<RoleType> roles = new List<RoleType>(tmpArr);
                                    roles.Remove(player.Role);
                                    buddy.Kill();
                                    buddy.SetRole(roles[rnd.Next(roles.Count)]);
                                    doneIDs.Add(buddy1);
                                    doneIDs.Add(id);
                                }
                                continue;
                            }
                            //if they are not an scp, we can just set them to the same role as their buddy
                            buddy.Kill();
                            buddy.SetRole(player.Role);
                            doneIDs.Add(buddy1);
                            doneIDs.Add(id);
                        }
                    }
                    catch (ArgumentException e)
                    {
                        buddyPlugin.buddies.Remove(id);
                        removePerson(id);
                        doneIDs.Add(id);
                        Log.Error(e.ToString());
                        continue;
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
                if (args.Length != 1)
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
            if (ev.Name.ToLower().Equals(buddyPlugin.buddyUnbuddyCommand))
            {
                ev.ReturnMessage = handleUnBuddyCommand(ev.Player);
            }
        }

        private string handleUnBuddyCommand(Exiled.API.Features.Player p)
        {
            try
            {
                if (buddyPlugin.buddies.ContainsKey(p.UserId))
                {
                    string refh = null;
                    buddyPlugin.buddies.TryGetValue(p.UserId, out refh);
                    if (refh != null) buddyPlugin.buddies.Remove(refh);
                    else removePerson(p.UserId);
                    buddyPlugin.buddies.Remove(p.UserId);
                }
            }
            catch (ArgumentNullException e)
            {
                Log.Error(e.ToString());
                return buddyPlugin.errorMessage;
            }
            return buddyPlugin.unBuddySuccess;
        }

        private string handleBuddyCommand(Exiled.API.Features.Player p, string[] args)
        {
            //get the player who the request was sent to
            Exiled.API.Features.Player buddy = null;
            string lower = args[0].ToLower();
            foreach (Exiled.API.Features.Player hub in Exiled.API.Features.Player.List)
            {
                if (hub == null) continue;
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
            buddy.SendConsoleMessage(buddyPlugin.BuddyMessagePrompt.Replace("$name", p.Nickname).Replace("$buddyAcceptCMD", "." + buddyPlugin.buddyAcceptCommand), "yellow");
            if (buddyPlugin.Config.sendBuddyRequestBroadcast && !RoundStarted)
                buddy.Broadcast(5, buddyPlugin.broadcastBuddyRequest.Replace("$name", p.Nickname), Broadcast.BroadcastFlags.Normal);
            return buddyPlugin.buddyRequestSentMessage;
        }

        private string handleBuddyAcceptCommand(Exiled.API.Features.Player p, string[] args)
        {
            //checks
            if (!buddyPlugin.buddyRequests.ContainsKey(p.UserId))
            {
                return buddyPlugin.noBuddyRequestsMessage;
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
                buddyPlugin.buddies.Remove(p.UserId);
                removePerson(p.UserId);
                return buddyPlugin.errorMessage;

            }
            try
            {
                if (buddyPlugin.buddies.ContainsKey(p.UserId))
                {
                    string refh = null;
                    buddyPlugin.buddies.TryGetValue(p.UserId, out refh);
                    if (refh != null) buddyPlugin.buddies.Remove(refh);
                    else removePerson(p.UserId);
                    buddyPlugin.buddies.Remove(p.UserId);
                }
            }
            catch (ArgumentNullException e)
            {
                Log.Error(e.ToString());
                return buddyPlugin.errorMessage;
            }

            buddyPlugin.buddies.Add(p.UserId, buddy.UserId);
            buddyPlugin.buddies.Add(buddy.UserId, p.UserId);
            buddyPlugin.buddyRequests.Remove(p.UserId);
            buddy.SendConsoleMessage(buddyPlugin.buddyRequestAcceptMessage.Replace("$name", p.Nickname), "yellow");
            if (buddyPlugin.Config.sendBuddyAcceptedBroadcast)
                buddy.Broadcast(5, buddyPlugin.buddyRequestAcceptMessage.Replace("$name", p.Nickname), Broadcast.BroadcastFlags.Normal);
            return buddyPlugin.successMessage;
        }

    }
}
