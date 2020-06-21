# Buddy
Buddy lets you pair up with another player before the round begins, and spawn in on the same team as them. This can let friends play together more easily.

## Installation
To install Buddy, just download the latest release at https://github.com/PintTheDragon/Buddy/releases , then stick Buddy.dll in your sm_plugins directory. After you launch your server, you can configure it.

## Configuration
For the most part, Buddy works out of the box, but you may want to configure it. To change the commands and things it says, go to sm_translations and open buddyplugin.txt. From there, you can modify the commands/responses.
To configure Buddy, you'll need to add the config options to config_gameplay.txt (or wherever your config goes if you're using MultiAdmin, etc):
| Config | Description | Default Value |
| ------ | ------------- | ------ |
| buddy_enabled | Enables/disables the plugin. | true |
| buddy_force_exact_role | Makes a player the exact role as their buddy. | false |
| buddy_disallow_guard_scientist_combo | If true, buddies will never spawn in as a guard and scientist. Only both a guard or both a scientist. | true |

## Usage
To use buddy plugin, run ".buddy <friend's name>" before the round starts (friend's name does not need to be exact). Then, your friend needs to run ".baccept". That's it! You will both spawn in on the same team.

## I FOUND AN ISSUE!!
Great! If you have a github account, please make an issue describing the problem that's occuring. Otherwise, you can contact me on discord at PintTheDragon#9203 and I'll make the issue for you.
