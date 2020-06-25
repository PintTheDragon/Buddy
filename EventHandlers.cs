using EXILED;
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
        public Buddy buddyPlugin;
        public EventHandlers(Buddy plugin) => this.buddyPlugin = plugin;

        private RoleType[] tmpArr = { RoleType.Scp049, RoleType.Scp079, RoleType.Scp096, RoleType.Scp106, RoleType.Scp173, RoleType.Scp93953, RoleType.Scp93989 };
        private Random rnd = new Random();

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            Timing.RunCoroutine(sendJoinMessage(ev.Player));
            Timing.RunCoroutine(sendBroadcast(ev.Player));
        }

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

        private IEnumerator<float> sendJoinMessage(ReferenceHub p)
        {
            yield return Timing.WaitForSeconds(1f);
            if (!buddyPlugin.buddies.ContainsKey(p.GetUserId()))
            {
                p.SendConsoleMessage(buddyPlugin.prefixedMessage, "yellow");
            }
            else
            {
                string buddy1 = null;
                buddyPlugin.buddies.TryGetValue(p.GetUserId(), out buddy1);
                if (buddy1 == null)
                {
                    buddyPlugin.buddies.Remove(p.GetUserId());
                    removePerson(p.GetUserId());
                }
                else
                {
                    p.SendConsoleMessage(buddyPlugin.broadcastBuddy.Replace("$buddy", Player.GetPlayer(buddy1).nicknameSync.Network_myNickSync), "yellow");
                }
            }
        }

        private IEnumerator<float> sendBroadcast(ReferenceHub p)
        {
            yield return Timing.WaitForSeconds(3f);
            ushort timeLeft = (ushort)GameCore.RoundStart.singleton.NetworkTimer;
            if (RoundSummary.RoundLock) timeLeft = 5;
            if (!buddyPlugin.buddies.ContainsKey(p.GetUserId()) && buddyPlugin.sendInfoBroadcast)
            {
                p.Broadcast(timeLeft, buddyPlugin.useBuddyCommandBroadcast);
            }
            if (buddyPlugin.buddies.ContainsKey(p.GetUserId()) && buddyPlugin.sendBuddyBroadcast)
            {
                string buddy1 = null;
                buddyPlugin.buddies.TryGetValue(p.GetUserId(), out buddy1);
                if (buddy1 == null)
                {
                    buddyPlugin.buddies.Remove(p.GetUserId());
                    removePerson(p.GetUserId());
                }
                else
                {
                    p.Broadcast(timeLeft, buddyPlugin.broadcastBuddy.Replace("$buddy", Player.GetPlayer(buddy1).nicknameSync.Network_myNickSync));
                }
            }
        }

        public void OnRoundStart()
        {
            Timing.RunCoroutine(doTheSCPThing());
        }

        public void OnRoundRestart()
        {
            if(buddyPlugin.resetBuddiesEveryRound)
            buddyPlugin.buddies = new Dictionary<string, string>();
        }

        private IEnumerator<float> doTheSCPThing()
        {
            yield return Timing.WaitForSeconds(1f);

            List<String> doneIDs = new List<String>();
            IEnumerable<string> onlinePlayers = Player.GetHubs().Select(x => x.GetUserId());

            IEnumerable<String> hubs = buddyPlugin.buddies.Values;
            for (int i = 0; i < hubs.Count(); i++)
            {
                string id = hubs.ElementAt(i);
                ReferenceHub player = Player.GetPlayer(id);
                //check if player has a buddy
                if (buddyPlugin.buddies.ContainsKey(player.GetUserId()))
                {
                    try
                    {
                        string buddy1 = null;
                        buddyPlugin.buddies.TryGetValue(player.GetUserId(), out buddy1);
                        if (buddy1 == null || (doneIDs.Contains(id) || doneIDs.Contains(buddy1)) || (!onlinePlayers.Contains(id) || !onlinePlayers.Contains(buddy1)))
                        {
                            buddyPlugin.buddies.Remove(id);
                            if(buddy1 != null) buddyPlugin.buddies.Remove(buddy1);
                            else removePerson(id);
                            doneIDs.Add(buddy1);
                            doneIDs.Add(id);
                            continue;
                        }
                        ReferenceHub buddy = Player.GetPlayer(buddy1);
                        //take action if they have different roles
                        if (player.GetRole() != buddy.GetRole() &&
                            /* massive check for scientist/guard combo */
                            !(!buddyPlugin.disallowGuardScientistCombo && ((player.GetRole() == RoleType.FacilityGuard && buddy.GetRole() == RoleType.Scientist) || (player.GetRole() == RoleType.Scientist && buddy.GetRole() == RoleType.FacilityGuard)))
                            )
                        {
                            //SCPs take priority
                            if (buddy.GetTeam() == Team.SCP) continue;

                            //if force exact role is on we can just set the buddy to the other player's role
                            if (buddyPlugin.forceExactRole)
                            {
                                buddy.Kill();
                                buddy.SetRole(player.GetRole());
                                doneIDs.Add(buddy1);
                                doneIDs.Add(id);
                                continue;
                            }
                            //if they are an scp, we need to remove another scp first
                            if (player.GetTeam() == Team.SCP)
                            {
                                //loop through every scp and swap the buddy with one of them
                                Boolean setRole = false;
                                foreach (ReferenceHub hub in Player.GetHubs())
                                {
                                    ReferenceHub player1 = hub;
                                    //check if the player is an scp
                                    if (player1.GetUserId() != id && player1.GetUserId() != buddy1 && !buddyPlugin.buddies.ContainsKey(player1.GetUserId()) && player1.GetTeam() == Team.SCP)
                                    {
                                        //set the buddy to that player's role and set the player to classd
                                        buddy.Kill();
                                        buddy.SetRole(player1.GetRole());
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
                                    roles.Remove(player.GetRole());
                                    buddy.Kill();
                                    buddy.SetRole(roles[rnd.Next(roles.Count)]);
                                    doneIDs.Add(buddy1);
                                    doneIDs.Add(id);
                                }
                                continue;
                            }
                            //if they are not an scp, we can just set them to the same role as their buddy
                            buddy.Kill();
                            buddy.SetRole(player.GetRole());
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

        public void OnConsoleCommand(EXILED.ConsoleCommandEvent ev)
        {
            string[] cmd = ev.Command.Split(' ');

            //run command handlers
            if (cmd[0].ToLower().Equals(buddyPlugin.buddyCommand))
            {
                if(cmd.Length == 1)
                {
                    ev.ReturnMessage = buddyPlugin.invalidUsage;
                    return;
                }
                try
                {
                    string[] args = cmd.Skip(1).ToArray<string>();
                    ev.ReturnMessage = handleBuddyCommand(ev.Player, args);
                    return;
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    ev.ReturnMessage = buddyPlugin.errorMessage;
                }
            }
            if (cmd[0].ToLower().Equals(buddyPlugin.buddyAcceptCommand))
            {
                ev.ReturnMessage = handleBuddyAcceptCommand(ev.Player, new string[] { });
                return;
            }
            if (cmd[0].ToLower().Equals(buddyPlugin.buddyUnbuddyCommand))
            {
                ev.ReturnMessage = handleUnBuddyCommand(ev.Player);
            }
        }

        private string handleUnBuddyCommand(ReferenceHub p)
        {
            try
            {
                if (buddyPlugin.buddies.ContainsKey(p.GetUserId()))
                {
                    string refh = null;
                    buddyPlugin.buddies.TryGetValue(p.GetUserId(), out refh);
                    if (refh != null) buddyPlugin.buddies.Remove(refh);
                    else removePerson(p.GetUserId());
                    buddyPlugin.buddies.Remove(p.GetUserId());
                }
            }
            catch (ArgumentNullException e)
            {
                Log.Error(e.ToString());
                return buddyPlugin.errorMessage;
            }
            return buddyPlugin.unBuddySuccess;
        }

        private string handleBuddyCommand(ReferenceHub p, string[] args)
        {
            //get the player who the request was sent to
            ReferenceHub buddy = null;
            string lower = args[0].ToLower();
            foreach (ReferenceHub hub in Player.GetHubs())
            {
                if (hub.nicknameSync.Network_myNickSync.ToLower().Contains(lower) && hub.GetUserId() != p.GetUserId())
                {
                    buddy = hub;
                    break;
                }
            }
            if (buddy == null)
            {
                return buddyPlugin.playerNotFoundMessage;
            }

            if (buddyPlugin.buddyRequests.ContainsKey(buddy.GetUserId())) buddyPlugin.buddyRequests.Remove(buddy.GetUserId());
            buddyPlugin.buddyRequests.Add(buddy.GetUserId(), p);
            buddy.SendConsoleMessage(buddyPlugin.BuddyMessagePrompt.Replace("$name", p.nicknameSync.Network_myNickSync).Replace("$buddyAcceptCMD", "." + buddyPlugin.buddyAcceptCommand), "yellow");
            if(buddyPlugin.sendBuddyRequestBroadcast)
            buddy.Broadcast(5, buddyPlugin.broadcastBuddyRequest.Replace("$name", p.nicknameSync.Network_myNickSync));
            return buddyPlugin.buddyRequestSentMessage;
        }

        private string handleBuddyAcceptCommand(ReferenceHub p, string[] args)
        {
            //checks
            if (!buddyPlugin.buddyRequests.ContainsKey(p.GetUserId()))
            {
                return buddyPlugin.noBuddyRequestsMessage;
            }

            //set the buddy
            ReferenceHub buddy = null;
            try
            {
                buddyPlugin.buddyRequests.TryGetValue(p.GetUserId(), out buddy);
            }
            catch (ArgumentNullException e)
            {
                Log.Error(e.ToString());
                return buddyPlugin.errorMessage;
            }
            if (buddy == null)
            {
                buddyPlugin.buddies.Remove(p.GetUserId());
                removePerson(p.GetUserId());
                return buddyPlugin.errorMessage;
            }
            try
            {
                if (buddyPlugin.buddies.ContainsKey(p.GetUserId()))
                {
                    string refh = null;
                    buddyPlugin.buddies.TryGetValue(p.GetUserId(), out refh);
                    if(refh != null) buddyPlugin.buddies.Remove(refh);
                    else removePerson(p.GetUserId());
                    buddyPlugin.buddies.Remove(p.GetUserId());
                }
            }
            catch(ArgumentNullException e)
            {
                Log.Error(e.ToString());
                return buddyPlugin.errorMessage;
            }

            buddyPlugin.buddies.Add(p.GetUserId(), buddy.GetUserId());
            buddyPlugin.buddies.Add(buddy.GetUserId(), p.GetUserId());
            buddyPlugin.buddyRequests.Remove(p.GetUserId());
            buddy.SendConsoleMessage(buddyPlugin.buddyRequestAcceptMessage, "yellow");
            return buddyPlugin.successMessage;
        }

    }
}
