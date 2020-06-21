using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System;
using System.Collections.Generic;

namespace PintBuddy
{
    internal class RoundStartHandler : IEventHandlerRoundStart
    {
        private RoleType[] tmpArr = { RoleType.SCP_049, RoleType.SCP_079, RoleType.SCP_096, RoleType.SCP_106, RoleType.SCP_173, RoleType.SCP_939_53, RoleType.SCP_939_89 };
        private Random rnd = new Random();
        private BuddyPlugin buddyPlugin;

        public RoundStartHandler(BuddyPlugin buddyPlugin)
        {
            this.buddyPlugin = buddyPlugin;
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            List<Player> players = ev.Server.GetPlayers();

            //loop through all players
            for(int i = 0; i < players.Count; i++)
            {
                Player player = players[i];
                //check if player has a buddy
                if (buddyPlugin.buddies.ContainsKey(player.UserId))
                {
                    try
                    {
                        Player buddy = null;
                        buddyPlugin.buddies.TryGetValue(player.UserId, out buddy);
                        if (buddy == null) continue;
                        //take action if they have different roles
                        if(player.TeamRole.Role != buddy.TeamRole.Role && 
                            /* massive check for scientist/guard combo */
                            !(!buddyPlugin.disallowGuardScientistCombo && ((player.TeamRole.Role == RoleType.FACILITY_GUARD && buddy.TeamRole.Role == RoleType.SCIENTIST) || (player.TeamRole.Role == RoleType.SCIENTIST && buddy.TeamRole.Role == RoleType.FACILITY_GUARD)))
                            )
                        {
                            //if force exact role is on we can just set the buddy to the other player's role
                            if (buddyPlugin.forceExactRole)
                            {
                                buddy.ChangeRole(player.TeamRole.Role);
                                continue;
                            }

                            //if they are an scp, we need to remove another scp first
                            if(player.TeamRole.Team == TeamType.SCP)
                            {
                                //check if their buddy is the only scp, in which case, they will be set to a different scp
                                Boolean onlySCP = true;
                                for(int y = 0; y < players.Count; y++)
                                {
                                    //if they are an scp and they are not the buddy, the buddy is not a sole scp
                                    if (players[y].UserId != player.UserId && players[y].TeamRole.Team == TeamType.SCP) onlySCP = false;
                                }
                                if (onlySCP)
                                {
                                    //create array of all scp types and remove the buddy's scp from it
                                    List<RoleType> roles = new List<RoleType>(tmpArr);
                                    roles.Remove(player.TeamRole.Role);
                                    buddy.ChangeRole(roles[rnd.Next(roles.Count)]);
                                    continue;
                                }

                                //loop through every scp and swap the buddy with one of them
                                for (int y = 0; y < players.Count; y++)
                                {
                                    Player player1 = players[y];
                                    //check if the player is an scp
                                    if (player1.UserId != player.UserId && player1.UserId != buddy.UserId && !buddyPlugin.buddies.ContainsKey(player1.UserId) && player1.TeamRole.Team == TeamType.SCP)
                                    {
                                        //set the buddy to that player's role and set the player to classd
                                        buddy.ChangeRole(player1.TeamRole.Role);
                                        player1.ChangeRole(RoleType.CLASSD);
                                        break;
                                    }
                                }
                                continue;
                            }
                            //if they are not an scp, we can just set them to the same role as their buddy
                            buddy.ChangeRole(player.TeamRole.Role);
                        }
                    }
                    catch (ArgumentException e)
                    {
                        buddyPlugin.Error(e.ToString());
                    }
                }
            }
        }
    }
}