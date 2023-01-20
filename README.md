<p>
  <a href="https://steamcommunity.com/sharedfiles/filedetails/?id=2916348220" alt="Steam Workshop Link">
  <img src="https://img.shields.io/static/v1?label=Steam&message=Workshop&color=blue&logo=steam&link=https://steamcommunity.com/sharedfiles/filedetails/?id=2916348220"/>
  </a>
</p>

# RPG Adventure Flavour Pack

![Mod Version](https://img.shields.io/badge/Mod_Version-1.0.1-blue.svg)
![RimWorld Version](https://img.shields.io/badge/Built_for_RimWorld-1.4-blue.svg)
![Harmony Version](https://img.shields.io/badge/Powered_by_Harmony-2.2-blue.svg)\
![Steam Downloads](https://img.shields.io/steam/downloads/2916348220?colorB=blue&label=Steam+Downloads)
![GitHub Downloads](https://img.shields.io/github/downloads/Roll1D2Games/RPGFantasyFlavourMod/total?colorB=blue&label=GitHub+Downloads)

This mod adds some Medieval RPG flavour to Rimworld and select other mods with the aim of making an RPG style playthrough feel more alive.
This is mostly done via simple renames with a few extra patches and re-textures to make some mods gel with other supporting mods.

This mod was made to support the Mr Samuel Streamer [Rimworld: RPG Adventure World collection](https://steamcommunity.com/sharedfiles/filedetails/?id=2908695387) for his [YouTube series](https://www.youtube.com/playlist?list=PLNWGkqCSwkOHznnLAMzwpy-pO0pR7Wr6r)  

This mod builds on many others including:
* [Vanilla Psycasts expanded](https://steamcommunity.com/sharedfiles/filedetails/?id=2842502659)
* [Alpha Animals](https://steamcommunity.com/sharedfiles/filedetails/?id=1541721856)
* [Combat Psycasts](https://steamcommunity.com/sharedfiles/filedetails/?id=2679831053)
* [Medieval Quest Rewards](https://steamcommunity.com/sharedfiles/filedetails/?id=2599672901)
* [RimWorld - Witcher Monster Hunt](https://steamcommunity.com/sharedfiles/filedetails/?id=2008529522)
* [RimQuest](https://steamcommunity.com/sharedfiles/filedetails/?id=2263331727)
* [Pawn Badges](https://steamcommunity.com/workshop/filedetails/?id=2526040241)

## Contributing
If you think you have a good idea for a fun translation to make the setting more cohesive or hilarious we are all ears!
Please make a pull request with any suggestions you might have and we will be glad to consider them for inclusion.

If you are not confident with GitHub but would like to contribute consider opening an issue and one of our modding team may action it.

### Rules
While we appreciate you may be overflowing with ideas we need to follow a strict code of conduct.
* Please keep your suggestions inclusive and uncontroversial.
* We would like this mod to be accessible to as many people as possible so your suggestions must remain YouTube friendly, so no slurs or cursing.
* All pull requests will be reviewed by a member of the team, please be patient, it may take a few days for your suggestion to be accepted and included. 

For more details please see our [Contributing Guide](CONTRIBUTING.md)

## Building and Releasing

*Please Note that there is no need to build the project if you just want to use it, as long as it's in your mods folder it'll work fine. You just need to make sure you're using the _Dev_ version in your mod list.*

To build the release version of this project, download this project and place it in your mods folder.
Once you have it locally, open the [solution file](1.4/Source/RPGAdventureFlavourPack.sln) in Visual Studio, Rider or your tool of choice and hit build.

Assuming all the paths are right this will build the latest C# binaries and collate all the files you actually need into a separate folder named `RPGAdventureFlavourPack-Release` which will appear next to the folder where you have this mod checked out.
It will also then zip that up for you into a zip file ready to be sent to someone or played in the game.
For example if I have the project checked out in `D:\Epic\RimWorld\Mods\RPGAdventureFlavourPack` after I hit build I'll magically have `D:\Epic\RimWorld\Mods\RPGAdventureFlavourPack-Release` and `D:\Epic\RimWorld\Mods\RPGAdventureFlavourPack\RPGAdventureFlavourPack.zip`

Note that to support this and make it easier to release to steam we have 2 About files a [dev one](About/About.xml) and a [release one](About/About-Release.xml).
The release folder will only have the release one, renamed to `About.xml`. This allows you to have both the dev and release versions on your system without them interfering with each other.
If you want to see the current unreleased version you will need to add the `.DEV` version of this mod rather than the normal one to your game.

All of the magic that does this is in the special [Files.csproj file](1.4/Source/Files.csproj)

Only the mod owner can release so this is mostly for our own documentation but anyone can build it.
If you have build issues please make a ticket here, or ask in `#rimworld-mod-making` on the official discord.

## Disclaimer
Portions of the materials used to create this content/mod are trademarks and/or copyrighted works of Ludeon Studios Inc. All rights reserved by Ludeon. This content/mod is not official and is not endorsed by Ludeon.

## Thanks
* Thanks to Sandwish for providing the Ranger tree for [Vanilla Psycasts expanded](https://steamcommunity.com/sharedfiles/filedetails/?id=2842502659)
* Thanks to Skullywag for providing various patches for mod compatibility
