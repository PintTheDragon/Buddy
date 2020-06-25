# Buddy
Buddy lets you pair up with another player before the round begins, and spawn in on the same team as them. This can let friends play together more easily.

## Installation
To install Buddy, just download the latest release at https://github.com/PintTheDragon/Buddy/releases (make sure to get the EXILED version, not the SMOD version), then stick Buddy.dll in your EXILED Plugins folder directory. After you launch your server, you can configure it.

## Configuration
For the most part, Buddy works out of the box, but you may want to configure it.
To configure Buddy, you'll need to add the config options to your EXILED config file (or wherever your config goes if you're using MultiAdmin, etc):
| Config | Description | Default Value |
| ------ | ------------- | ------ |
| buddy_enabled | Enables/disables the plugin. | true |
| buddy_force_exact_role | Makes a player the exact role as their buddy. | false |
| buddy_disallow_guard_scientist_combo | If true, buddies will never spawn in as a guard and scientist. Only both a guard or both a scientist. | true |
| buddy_reset_buddies_every_round | Should buddies be reset every round. | true |
| buddy_send_info_broadcast | Should a broadcast be sent be sent telling players how to use the plugin. | true |
| buddy_send_buddy_broadcast | Should a broadcast be sent be sent telling players who their buddy is. | true |
| buddy_send_buddy_request_broadcast | Should a broadcast be sent be sent to a player when someone requests to be their buddy. | true |
| buddy_send_buddy_accepted_broadcast | Should a broadcast be sent be sent telling players that their buddy request was accepted. | true |

## Usage
To use buddy plugin, run ".buddy <friend's name>" before the round starts (friend's name does not need to be exact). Then, your friend needs to run ".baccept". That's it! You will both spawn in on the same team.
