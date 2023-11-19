<p align="center">
  <img src="https://github.com/SGiygas/CrewBoom/assets/50772474/e079fa47-307a-4b40-a449-f6152a5c11d8" />
</p>
    
This is a repository for a BepInEx plugin to load custom characters for the game *Bomb Rush Cyberfunk*  

Example:  
![image](https://github.com/SGiygas/BrcCustomCharacters/assets/50772474/43ff8ca8-0805-4409-9547-234e26fcedda)

## Features

The plugin allows for the following:
- Custom character name/title
- Custom character model (gameplay & cutscenes)
- Custom outfits (up to four)
- Custom outfit names
- Multiple meshes and materials per outfit
- Custom character graffiti
- Custom character voice clips
- Multiple skins at once (for different characters and per character)
- Additional character slots for any character bundle

## Downloads, instructions and help

### [Plugin](https://github.com/SGiygas/CrewBoom/releases/download/v3.2.2/plugin.zip)
### [Unity project](https://github.com/SGiygas/CrewBoom/releases/download/v3.2.2/unityProject.zip)

To create a character mod you will need both of these. If you just want to use a character you downloaded, you only need the plugin.  

### [Documentation & Help here](https://github.com/SGiygas/CrewBoom/wiki)
### [Troubleshooting](https://github.com/SGiygas/CrewBoom/wiki/Troubleshooting)

## Installation

This plugin requires [BepInEx](https://thunderstore.io/package/bbepis/BepInExPack/), a plugin framework for Unity games.
The easiest way to get BepInEx going for Bomb Rush Cyberfunk is to use the [r2modman mod manager](https://thunderstore.io/package/ebkr/r2modman/).  

*r2modman was used to test this plugin, so other solutions may not work*

### Using r2modman

To install BepInEx, follow these steps:  
1. Open r2modman and select *Bomb Rush Cyberfunk* as the game and choose any profile
2. Navigate to the *Online* tab
3. Find *BepInEx Pack* by BepInEx and install it

To install the plugin for BepInEx, follow these steps:

**Automatic Install**  
Crew Boom is also available on the [Thunderstore](https://thunderstore.io/c/bomb-rush-cyberfunk/p/SoftGoat/CrewBoom/) to download directly from r2modman.  

**Manual Install**
1. Download the [plugin](https://github.com/SGiygas/CrewBoom/releases/download/v3.2.2/plugin.zip)
2. Open r2modman and select *Bomb Rush Cyberfunk* as the game and choose any profile
3. Navigate to the *Settings* tab 
4. Find the option labeled "Browse profile folder" and click it
5. Navigate to the folder `BepInEx/plugins/`
6. Copy the folder `CrewBoom` from the plugin archive to `BepInEx/plugins/`

## Adding characters

1. Navigate to your BepInEx installation for the game and open the `BepInEx/config/CrewBoom/` folder
2. Copy your character files (.cbb and .json) into the `CrewBoom` folder
3. **Optionally**, you can put the character files in `BepInEx/config/CrewBoom/no_cypher` to have characters only show up when synced via the API (SlopCrew) and not in your cypher selection.

Optionally, you can edit/create a json file for your character to change whether they replace a character or not.  
You can find instructions on how to do this [here](https://github.com/SGiygas/CrewBoom/wiki/Character-Configuration#editing-the-json-file)

## Building the plugin

This pertains to if you want to build the plugin yourself, not the Unity project.  

You will need to copy Bomb Rush Cyberfunk's `Assembly-CSharp.dll` into the `Libraries` folder of the project, as it is not provided.  
It can be found at `<path to where your games are stored>\BombRushCyberfunk\Bomb Rush Cyberfunk_Data\Managed`

## Authors and acknowledgment
- Programming and Unity SDK: SGiygas
- Test model and assets: minty_cups

This project was loosely based on [BRCModelReplacement](https://github.com/TheSmallBlue/BRC-ModelReplacement) by TheSmallBlue  
and [BRCMods](https://github.com/Ikeiwa/BRCMods) by Ikeiwa & TheSmallBlue

## License
This project is licensed under the GNU General Public License v3.0
