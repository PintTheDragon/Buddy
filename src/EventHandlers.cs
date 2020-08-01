﻿using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Buddy
{
    class EventHandlers
    {
        public Buddy buddyPlugin;
        public EventHandlers(Buddy plugin) => this.buddyPlugin = plugin;

        private RoleType[] tmpArr = { RoleType.Scp049, RoleType.Scp079, RoleType.Scp096, RoleType.Scp106, RoleType.Scp173, RoleType.Scp93953, RoleType.Scp93989 };
        private Random rnd = new Random();

        public void OnPlayerJoin(JoinedEventArgs ev)
        {
            Timing.RunCoroutine(sendJoinMessage(ev.Player));
            Timing.RunCoroutine(sendBroadcast(ev.Player));
        }

        public IEnumerator<float> sendJoinMessage(Player p)
        {
            yield return Timing.WaitForSeconds(1f);
            if (!buddyPlugin.buddies.ContainsKey(p.UserId))
            {
                p.SendConsoleMessage(buddyPlugin.getLang("BuddyMessage"), "yellow");
            }
            else
            {
                string buddy1 = null;
                if (!buddyPlugin.buddies.TryGetValue(p.UserId, out buddy1) || buddy1 == null)
                {
                    buddyPlugin.buddies.Remove(p.UserId);
                    buddyPlugin.removePerson(p.UserId);
                }
                else
                {
                    Player player = Player.Get(buddy1);
                    if (player == null) yield break;
                    p.SendConsoleMessage(buddyPlugin.getLang("broadcastBuddy").Replace("$buddy", player.Nickname), "yellow");
                }
            }
        }

        private IEnumerator<float> sendBroadcast(Player p)
        {
            yield return Timing.WaitForSeconds(2f);
            if (!buddyPlugin.buddies.ContainsKey(p.UserId) && buddyPlugin.Config.sendInfoBroadcast)
            {
                p.Broadcast(5, buddyPlugin.getLang("useBuddyCommandBroadcast"), Broadcast.BroadcastFlags.Normal);
            }
            if (buddyPlugin.buddies.ContainsKey(p.UserId) && buddyPlugin.Config.sendBuddyBroadcast)
            {
                string buddy1 = null;
                if (!buddyPlugin.buddies.TryGetValue(p.UserId, out buddy1) || buddy1 == null)
                {
                    buddyPlugin.buddies.Remove(p.UserId);
                    buddyPlugin.removePerson(p.UserId);
                }
                else
                {
                    Player player = Player.Get(buddy1);
                    if (player == null) yield break;
                    p.Broadcast(5, buddyPlugin.getLang("broadcastBuddy").Replace("$buddy", player.Nickname), Broadcast.BroadcastFlags.Normal);
                }
            }
        }

        public void OnRoundStart()
        {
            Timing.RunCoroutine(setRoles());
        }

        public void OnRoundRestart()
        {
            if (buddyPlugin.Config.resetBuddiesEveryRound)
                buddyPlugin.buddies = new Dictionary<string, string>();
        }

        private IEnumerator<float> setRoles()
        {
            yield return Timing.WaitForSeconds(1f);

            List<string> doneIDs = new List<string>();
            IEnumerable<string> onlinePlayers = Player.List.Select(x => x.UserId);

            IEnumerable<string> hubs = buddyPlugin.buddies.Values;
            for (int i = 0; i < hubs.Count(); i++)
            {
                string id = hubs.ElementAt(i);
                Player player = Player.Get(id);
                if (player == null) continue;
                //check if player has a buddy
                if (buddyPlugin.buddies.ContainsKey(player.UserId))
                {
                    try
                    {
                        string buddy1 = null;
                        if (!buddyPlugin.buddies.TryGetValue(player.UserId, out buddy1) || buddy1 == null || (!onlinePlayers.Contains(id) || !onlinePlayers.Contains(buddy1)))
                        {
                            buddyPlugin.buddies.Remove(id);
                            if (buddy1 != null) buddyPlugin.buddies.Remove(buddy1);
                            else buddyPlugin.removePerson(id);
                            doneIDs.Add(buddy1);
                            doneIDs.Add(id);
                            continue;
                        }
                        if ((doneIDs.Contains(id) || doneIDs.Contains(buddy1))) continue;
                        Player buddy = Player.Get(buddy1);
                        if (buddy == null) continue;
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
                                        buddy.SetRole(player1.Role);
                                        player1.SetRole(RoleType.ClassD);
                                        setRole = true;
                                        doneIDs.Add(buddy1);
                                        doneIDs.Add(id);
                                        break;
                                    }
                                }
                                //if their role is not set (their buddy is the sole scp), set them to a random scp
                                if (!setRole)
                                {
                                    List<RoleType> roles = new List<RoleType>(tmpArr);
                                    roles.Remove(player.Role);
                                    buddy.SetRole(roles[rnd.Next(roles.Count)]);
                                    doneIDs.Add(buddy1);
                                    doneIDs.Add(id);
                                }
                                continue;
                            }
                            //if they are not an scp, we can just set them to the same role as their buddy
                            buddy.SetRole(player.Role);
                            doneIDs.Add(buddy1);
                            doneIDs.Add(id);
                        }
                    }
                    catch (ArgumentException e)
                    {
                        try
                        {
                            buddyPlugin.buddies.Remove(id);
                        }
                        catch (ArgumentException) { }
                        buddyPlugin.removePerson(id);
                        doneIDs.Add(id);
                        Log.Error(e.ToString());
                        continue;
                    }
                }

            }
        }
    }
}