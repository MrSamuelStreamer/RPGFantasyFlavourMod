# Contributing To The RPG Adventure Flavour Pack
Thank you for your interest in contributing to the RPG Adventure Flavour Pack. This project is very open to contributions,
from people of all skill levels and backgrounds.

Please note we have a [code of conduct](#contributor-covenant---code-of-conduct), please follow it in all your interactions with the project.

## Pull Request Process

1. When adding compatibility with another mod, make sure it is appropriately isolated using `MayRequire`, `PatchOperationFindMod` or an entry in [loadFolders.xml](loadFolders.xml)
2. Ensure your contribution is YouTube friendly and follows our code of conduct. 
3. Update the README.md if necessary.
4. Make sure you add any `loadAfter` entries in [About.xml](About/About.xml) AND [About-Release.xml](About/About-Release.xml) 

## How to add translations
Adding translations is mostly pretty easy, and a great first step for an aspiring modder.
In most cases it simply needs you to put the right key and what you want to translate it too.
This section aims to give a brief overview but is by no means a perfect and complete guide.
If you aren't sure experiment and feel free to ask questions and don't be afraid to open an incomplete pull request to ask for input.

Firstly if you're new to modding you can get a lot of useful info off the [Wiki](https://rimworldwiki.com/wiki/Modding_Tutorials)
But if you're just here to add some funny renames it may be more than you need.

There are four main ways of renaming things.
* [Keyed translations](#keyed-translations)
* [DefInjected translations](#definjected-translations)
* [XML Patches](#xml-patches)
* [In Code](#in-code)

### Basics Glossary
* $MOD_ROOT refers to the main mod folder, it's what you'd see in your local mods folder and what you see when you first go to the GitHub page for this project.
* A `*` means "anything" so `*.xml` would be any xml file.
* Most language bits don’t need to be version specific but if the list order changes between versions… This can cause a few errors so it's best to not use the `Common` folder and instead use `1.4` or whatever version of Rimworld you want. This actually goes even further. If you want to translate something in a mod instead of base game even an expansion, you should split it into it's own folder. You'll see we've done this in the [Compatibility](Compatibility) folder. There's one folder per mod and we named the folder using the mod ID. Please keep to this standard. Within each named mod folder the structure is the same as for the Common folder. There's one extra hiccup here though. You must tell the game to load your new folder. That is what [loadFolders.xml](loadFolders.xml) is for. This file tells the game when you see mod XYZ please load this extra set of folders. You will need an entry like `<li IfModActive="my.cool.mod">Compatibility/my.cool.mod</li>` to add your cool translations.  

### Keyed translations
In [$MOD_ROOT/Common/Languages/English/Keyed/*.xml](Common/Languages/English/Keyed/Misc.xml) you get literally key value pairs. Most of the UI labels are in the keyed folder somewhere.

All the files you work with in the `Keyed` folder MUST have this sort of shell. Then all your silly renames go between the language data.
```xml
<?xml version="1.0" encoding="utf-8" ?>
<LanguageData>
  
</LanguageData>
```

Many mods that are done in C# and much of the base game will have its text set this way. Like the text from motes, the menus, certain letters etc. All you have to do is copy the original entry from the keyed file tweaking as you want. E.g. Renaming PsyFocus to mana is as simple as
```xml
<?xml version="1.0" encoding="utf-8" ?>
<LanguageData>
  <Psyfocus>Mana</Psyfocus>
</LanguageData>
```

How do you find the special key, in this case it was `Psyfocus`. Simple, find your main game folder and in it next to `RimWorldWin64.exe` you will see a folder called `Data`. In here is the base game XML files that contain the current names for most stuff, split into `Core` and one for each expansion. If you see a word or phrase in game just search for it in all the files in those folders. Once you see the original you know what to put in your own file in the Keyed folder in our mod. Note that this ONLY works if where you find it is in a similar `Keyed` folder e.g. `.../RimWorld/Data/Core/Languages/English/Keyed`. If it's anywhere else this won't cut it.

Also be aware that some things appear many times so you may need many keyed translations or to use a different option to get a specific thing in game renamed.

### DefInjected translations
This is one step up. Most things in the game are defined in Defs. For base game these will be in `.../RimWorld/Data/Core/Defs`.
Most items, most stats etc all of them are Defs. Many parts of a Def are strings and you can use DefInjected to reach into a Def and change it!

For us, [$MOD_ROOT/Common/Languages/English/DefInjected/*.xml](Common/Languages/English/DefInjected) contains overrides for most text contained in a def. So eg if you want to change the label on GlitterTech Medicine you could do so by adding an entry here. These are sort of pattern based instead of simple key value.

* Your rename MUST be in a folder named after the type of Def it is.
* Your XML keys should be sort of a path or selector through the original def’s XML but the first part of the key is the defName. So for example if you wanted to rename the shooting skill in the Pawn's Bio page you'd find the Def in the base game which it turns out is in `.../RimWorld/Data/Core/Defs/SkillDefs/Skills.xml` and looks like this:

```xml
  <SkillDef>
    <defName>Shooting</defName>
    <description>Ranged combat, with both high-tech and low-tech weapons.</description>
    <skillLabel>shooting</skillLabel>
    ...
  </SkillDef>
```

* Note that the first line `<SkillDef>` tells us we must put it in a folder named `SkillDef` EXACTLY and that must be at `Languages/English/DefInjected` in our mod. The file in here can be anything you like, I usually name it after the file I pulled it out of.
* The second line gives us the defName we need for the key. In this case it's `Shooting`.
* The remaining lines are the actual things we want to change, the skillLabel is what is in the Bio and we don't want the description to still say shooting.

So if we want to change the skillLabel we first say what def we're changing then what field within it separated by a `.`.
So the entry will be `<Shooting.skillLabel>Our fun translation</Shooting.skillLabel>`

So our final result is already [in this mod](Common/Languages/English/DefInjected/SkillDef/Skills.xml)
Note the path matches the required `Common/Languages/English/DefInjected/SkillDef/*.xml`
And we see a complete example below. 

```xml
<?xml version="1.0" encoding="utf-8" ?>
<LanguageData>
    <Shooting.description>Ranged combat, with any type of coward's weapon.</Shooting.description>
    <Shooting.skillLabel>archery</Shooting.skillLabel>
</LanguageData>
```

This can go as deep as you need into an xml def.

There are two caveats here.
* Some Defs contain lists of stuff, If you come across a list (represent by a set of `<li>` entries) you can pick a specific item in the list by adding `.0` or `.1` or whichever item you need, note that this starts at zero so the fist item is `.0` e.g. `<SomeDef.comps.0.label>`.
* Because these are a bit more specific you probably need to put your translations in a Compatibility folder and use [loadFolders.xml](loadFolders.xml). This is explained better in the [Basics section](#basics-glossary) 

### XML Patches
Sometimes its hard to pick what you want to change, or you want to make a more general change, renaming all of something maybe across many mods.
This is where patching comes in, you can also use this to actually mess with the defs but that's not the purpose of this guide.
In much the same way as with DefInjected you can select a bit of XML and change it based on a pattern.

I won't go into too much detail on these. You need to write an XPath to select the bits you need to change and that's more complexity than is reasonable for this quick guide. The wiki has a good primer on [XML Patches with examples](https://rimworldwiki.com/wiki/Modding_Tutorials/PatchOperations).

In terms of organisation this follows much the same rules as DefInjected. Make sure it's in a loadFolder, be careful of versions and make sure you test it. Rather than using a loadFolder you can use `PatchOperationFindMod` to only apply a patch in the case where a mod is found. This is essentially the same as using a load folder. Either is fine. I tend to lean towards loadFolders because they are a little more explicit. 

### In Code
This is the section of lst resort. Some things in the game like meat Defs are actually auto-generated by the game and are not in XML so all these XML options are useless. In many cases you can still change things with that first Keyed option but failing that it's time to break out some C#.

Again this is too complex to cover here in detail I merely wish to list the option. You can make a Harmony patch which will mess with the results before, after or even during the code running. If you can find the bit of code that makes a name you don't like you can intercept and change it. [The wiki is your friend here](https://rimworldwiki.com/wiki/Modding_Tutorials/Harmony) and the [Harmony docs](https://harmony.pardeike.net/articles/basics.html).

Be warned we have to be strict here, no stolen code! Breaches will be dealt will as laid out in the code of conduct.
Also please follow good code hygiene. If you make a mistake our reviewers should be able to help you out but it will be a rare feature that needs this. Please make an issue for any change you think needs code to discuss first to make sure it won't be wasted effort and get help.

As with the other systems be careful not to force other mods to be required. Use loadFolders and you may even need a separate C# project. There is already an example of this being used to mess with [RimQuest](1.4/Source/RPGAdventureFlavourPack.RimQuest.csproj). Make sure to add any new projects to the main C# [Solution](1.4/Source/RPGAdventureFlavourPack.sln) so they can all be built together and set the build dependencies. Be sure your `OutputPath` points at a folder controlled by loadFolders unless it's for basegame only.

We will also be very strict about DLLs. When you use another mod to build on C# likes to take a copy of the DLLs and put them in it's own output folder. Any PR with someone else's DLLs in it will be rejected immediately. You can simply select `Copy Local` to be false in Visual Studio or set `<Private>False</Private>` in the `.csproj` file for the `Reference` entry achieves this e.g. to include HAR without accidentally copying their DLL:

```xml
<Reference Include="AlienRace">
  <HintPath>..\..\..\..\839005762\1.4\Assemblies\AlienRace.dll</HintPath>
  <Private>False</Private>
</Reference>
```

Please use relative paths like the above, any PR with your personal Steam folder in a path will be rejected. This can be tricky to get right. It's often added with an exact path. We have people working on this that do not use Steam so any steam paths would just be broken for them. Fortunately you can go pretty wild with these things as you can add conditions for example this will use the RimWorld dll if it's where it is expected to be, however if not it will default to using a NuGet package. You can use this same trick to add more options for paths if your machine is a bit different and the example paths aren't working for you. Please note that the given paths expect the code to be checked out into your mods folder.

#### A note on Debug Configurations
The Debug configurations in this project generate portable pdb files which can be used to set breakpoints and debug the C# code in this project when using [RimWorld4Debugging](https://github.com/pardeike/RimWorld4Debugging). When submitting a PR with changed C# code it is expected that you will have built the code with the Release configuration. If you see any pdb files hanging around that's a good indication you haven't run the release build because the release is set to clean up any pdb files. Please continue this tradition in any new projects.

#### What is that Zip file?
The C# project is set to when run as a release configuration generate a Zip file which _should_ contain all the files needed to use the mod without any of the coding guff like C# and compiled class files. The zip can be distributed instead of downloading via steam. The community leaders will take care of uploading this when there is a new version. 

# Contributor Covenant - Code of Conduct

## Our Pledge

We as members, contributors, and leaders pledge to make participation in our
community a harassment-free experience for everyone, regardless of age, body
size, visible or invisible disability, ethnicity, sex characteristics, gender
identity and expression, level of experience, education, socio-economic status,
nationality, personal appearance, race, caste, color, religion, or sexual
identity and orientation.

We pledge to act and interact in ways that contribute to an open, welcoming,
diverse, inclusive, and healthy community.

## Our Standards

Examples of behavior that contributes to a positive environment for our
community include:

* Demonstrating empathy and kindness toward other people
* Being respectful of differing opinions, viewpoints, and experiences
* Giving and gracefully accepting constructive feedback
* Accepting responsibility and apologizing to those affected by our mistakes,
  and learning from the experience
* Focusing on what is best not just for us as individuals, but for the overall
  community

Examples of unacceptable behavior include:

* The use of sexual language or imagery, and sexual attention or advances of
  any kind
* Trolling, insulting or derogatory comments, and personal or political attacks
* Public or private harassment
* Publishing others' private information, such as a physical or email address,
  without their explicit permission
* Other conduct which could reasonably be considered inappropriate in a
  professional setting

## Enforcement Responsibilities

Community leaders are responsible for clarifying and enforcing our standards of
acceptable behavior and will take appropriate and fair corrective action in
response to any behavior that they deem inappropriate, threatening, offensive,
or harmful.

Community leaders have the right and responsibility to remove, edit, or reject
comments, commits, code, wiki edits, issues, and other contributions that are
not aligned to this Code of Conduct, and will communicate reasons for moderation
decisions when appropriate.

## Scope

This Code of Conduct applies within all community spaces, and also applies when
an individual is officially representing the community in public spaces.
Examples of representing our community include using an official e-mail address,
posting via an official social media account, or acting as an appointed
representative at an online or offline event.

## Enforcement

Instances of abusive, harassing, or otherwise unacceptable behavior may be
reported to the community leaders responsible for enforcement via ModMail in
[Mr Samuel Streamer's official discord](https://discord.gg/fkreDYG).
All complaints will be reviewed and investigated promptly and fairly.

All community leaders are obligated to respect the privacy and security of the
reporter of any incident.

## Enforcement Guidelines

Community leaders will follow these Community Impact Guidelines in determining
the consequences for any action they deem in violation of this Code of Conduct:

### 1. Correction

**Community Impact**: Use of inappropriate language or other behavior deemed
unprofessional or unwelcome in the community.

**Consequence**: A private, written warning from community leaders, providing
clarity around the nature of the violation and an explanation of why the
behavior was inappropriate. A public apology may be requested.

### 2. Warning

**Community Impact**: A violation through a single incident or series of
actions.

**Consequence**: A warning with consequences for continued behavior. No
interaction with the people involved, including unsolicited interaction with
those enforcing the Code of Conduct, for a specified period of time. This
includes avoiding interactions in community spaces as well as external channels
like social media. Violating these terms may lead to a temporary or permanent
ban.

### 3. Temporary Ban

**Community Impact**: A serious violation of community standards, including
sustained inappropriate behavior.

**Consequence**: A temporary ban from any sort of interaction or public
communication with the community for a specified period of time. No public or
private interaction with the people involved, including unsolicited interaction
with those enforcing the Code of Conduct, is allowed during this period.
Violating these terms may lead to a permanent ban.

### 4. Permanent Ban

**Community Impact**: Demonstrating a pattern of violation of community
standards, including sustained inappropriate behavior, harassment of an
individual, or aggression toward or disparagement of classes of individuals.

**Consequence**: A permanent ban from any sort of public interaction within the
community.

## Attribution

This Code of Conduct is adapted from the [Contributor Covenant][homepage],
version 2.1, available at
[https://www.contributor-covenant.org/version/2/1/code_of_conduct.html][v2.1].

Community Impact Guidelines were inspired by
[Mozilla's code of conduct enforcement ladder][Mozilla CoC].

For answers to common questions about this code of conduct, see the FAQ at
[https://www.contributor-covenant.org/faq][FAQ]. Translations are available at
[https://www.contributor-covenant.org/translations][translations].

[homepage]: https://www.contributor-covenant.org
[v2.1]: https://www.contributor-covenant.org/version/2/1/code_of_conduct.html
[Mozilla CoC]: https://github.com/mozilla/diversity
[FAQ]: https://www.contributor-covenant.org/faq
[translations]: https://www.contributor-covenant.org/translations
