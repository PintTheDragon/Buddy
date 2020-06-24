using EXILED;
using EXILED.Extensions;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using ProductionStackTrace;
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

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            if (RoundStarted) return;
            Timing.RunCoroutine(sendJoinMessage(ev.Player));
        }

        public IEnumerator<float> sendJoinMessage(ReferenceHub p)
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
            RoundStarted = false;
            if(buddyPlugin.resetBuddiesEveryRound)
            buddyPlugin.buddies = new Dictionary<string, ReferenceHub>();
        }

        private IEnumerator<float> doTheSCPThing()
        {
            yield return Timing.WaitForSeconds(1f);

            List<String> doneIDs = new List<String>();
            IEnumerable<string> onlinePlayers = Player.GetHubs().Select(x => x.GetUserId());
            //reload buddy list
            IEnumerable<ReferenceHub> Thehubs = buddyPlugin.buddies.Values;
            Dictionary<string, ReferenceHub> buddies = new Dictionary<string, ReferenceHub>();
            for (int i = 0; i < Thehubs.Count(); i++)
            {
                ReferenceHub newH = Player.GetPlayer(Thehubs.ElementAt(i).GetUserId());
                ReferenceHub oldBuddy = null;
                buddyPlugin.buddies.TryGetValue(newH.GetUserId(), out oldBuddy);
                if(oldBuddy == null)
                {
                    buddyPlugin.buddies.Remove(newH.GetUserId());
                    doneIDs.Add(newH.GetUserId());
                    continue;
                }
                ReferenceHub newBuddy = Player.GetPlayer(oldBuddy.GetUserId());
                buddies.Add(newBuddy.GetUserId(), newH);
                buddies.Add(newH.GetUserId(), newBuddy);
            }
            buddyPlugin.buddies = buddies;

            IEnumerable<ReferenceHub> hubs = buddyPlugin.buddies.Values;
            for (int i = 0; i < hubs.Count(); i++)
            {
                ReferenceHub player = hubs.ElementAt(i);
                //check if player has a buddy
                if (buddyPlugin.buddies.ContainsKey(player.GetUserId()))
                {
                    try
                    {
                        ReferenceHub buddy = null;
                        buddyPlugin.buddies.TryGetValue(player.GetUserId(), out buddy);
                        if (buddy == null) continue;
                        if (doneIDs.Contains(player.GetUserId()) || doneIDs.Contains(buddy.GetUserId())) continue;
                        if(!onlinePlayers.Contains(player.GetUserId()) || !onlinePlayers.Contains(player.GetUserId()))
                        {
                            buddyPlugin.buddies.Remove(player.GetUserId());
                            buddyPlugin.buddies.Remove(buddy.GetUserId());
                            doneIDs.Add(buddy.GetUserId());
                            doneIDs.Add(player.GetUserId());
                            continue;
                        }
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
                                doneIDs.Add(buddy.GetUserId());
                                doneIDs.Add(player.GetUserId());
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
                                    if (player1.GetUserId() != player.GetUserId() && player1.GetUserId() != buddy.GetUserId() && !buddyPlugin.buddies.ContainsKey(player1.GetUserId()) && player1.GetTeam() == Team.SCP)
                                    {
                                        //set the buddy to that player's role and set the player to classd
                                        buddy.Kill();
                                        buddy.SetRole(player1.GetRole());
                                        player1.Kill();
                                        player1.SetRole(RoleType.ClassD);
                                        setRole = true;
                                        doneIDs.Add(buddy.GetUserId());
                                        doneIDs.Add(player.GetUserId());
                                        break;
                                    }
                                }
                                if (!setRole)
                                {
                                    List<RoleType> roles = new List<RoleType>(tmpArr);
                                    roles.Remove(player.GetRole());
                                    buddy.Kill();
                                    buddy.SetRole(roles[rnd.Next(roles.Count)]);
                                    doneIDs.Add(buddy.GetUserId());
                                    doneIDs.Add(player.GetUserId());
                                }
                                continue;
                            }
                            //if they are not an scp, we can just set them to the same role as their buddy
                            buddy.Kill();
                            buddy.SetRole(player.GetRole());
                            doneIDs.Add(buddy.GetUserId());
                            doneIDs.Add(player.GetUserId());
                        }
                    }
                    catch (ArgumentException e)
                    {
                        Log.Error(ExceptionReporting.GetExceptionReport(e));
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
                    Log.Error(ExceptionReporting.GetExceptionReport(e));
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
                    ReferenceHub refh = null;
                    buddyPlugin.buddies.TryGetValue(p.GetUserId(), out refh);
                    if (refh != null) buddyPlugin.buddies.Remove(refh.GetUserId());
                    buddyPlugin.buddies.Remove(p.GetUserId());
                }
            }
            catch (ArgumentNullException e)
            {
                Log.Error(ExceptionReporting.GetExceptionReport(e));
                return buddyPlugin.errorMessage;
            }
            return buddyPlugin.unBuddySuccess;
        }

        private string handleBuddyCommand(ReferenceHub p, string[] args)
        {
            //checks
            if (RoundStarted)
            {
                return buddyPlugin.roundAlreadyStartedMessage;
            }
            /*if (buddyPlugin.buddies.ContainsKey(p.GetUserId()))
            {
                return buddyPlugin.alreadyHaveBuddyMessage;
            }*/

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
            buddy.SendConsoleMessage(buddyPlugin.BuddyMessagePrompt.Replace("%name", p.nicknameSync.Network_myNickSync).Replace("%buddyAcceptCMD", "." + buddyPlugin.buddyAcceptCommand), "yellow");
            return buddyPlugin.buddyRequestSentMessage;
        }

        private string handleBuddyAcceptCommand(ReferenceHub p, string[] args)
        {
            //checks
            if (RoundStarted)
            {
                return buddyPlugin.roundAlreadyStartedMessage;
            }
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
                Log.Error(ExceptionReporting.GetExceptionReport(e));
                return buddyPlugin.errorMessage;
            }
            if (buddy == null)
            {
                return buddyPlugin.errorMessage;

            }
            try
            {
                if (buddyPlugin.buddies.ContainsKey(p.GetUserId()))
                {
                    ReferenceHub refh = null;
                    buddyPlugin.buddies.TryGetValue(p.GetUserId(), out refh);
                    if(refh != null) buddyPlugin.buddies.Remove(refh.GetUserId());
                    buddyPlugin.buddies.Remove(p.GetUserId());
                }
            }
            catch(ArgumentNullException e)
            {
                Log.Error(ExceptionReporting.GetExceptionReport(e));
                return buddyPlugin.errorMessage;
            }

            buddyPlugin.buddies.Add(p.GetUserId(), buddy);
            buddyPlugin.buddies.Add(buddy.GetUserId(), p);
            buddyPlugin.buddyRequests.Remove(p.GetUserId());
            buddy.SendConsoleMessage(buddyPlugin.buddyRequestAcceptMessage, "yellow");
            return buddyPlugin.successMessage;
        }

    }
}
