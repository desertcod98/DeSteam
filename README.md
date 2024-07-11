# DeSteam
<div align="center">
    <img width="200" src="https://github.com/desertcod98/DeSteam/blob/main/assets/DeSteamLogo.png" alt="DeSteamLogo">
    </br>
</div>


DeSteam purpose is to unpack files packed with SteamDrm. It currently works on only one of the SteamDrm versions, tested only on three games as of now. I created this project for the sole purpose of learning about DRMs and getting better at reverse engineering.

**I do not take responsibility for the improper use of the software, you should only use this on games/applications that you legally own and you should not distribute unpacked files. This software was made for educational purposes ONLY.**

## Installation
Download the last release from the [Releases page](https://github.com/desertcod98/DeSteam/releases/), then just run the .exe according to the [Usage section](#Usage).
Alternately you can compile the C# code yourself, it is originally compiled in Visual Studio 2022.

## Usage
In a terminal you have to execute the program with this argument:
`./DeSteam.exe [path_to_file_to_unpack]`

Optional arguments are:
```
  -v, --verbose            Shows additional info when executing. (Default: false)
  -o, --output             Specify unpacked file name. (Default: [original_name].unpacked.exe)
```

## Dependencies
Currently DeSteam depends on:
* [Unicorn emulator](https://github.com/unicorn-engine/unicorn), to emulate a part of the unpacking code extracted from the SteamDrm.
* [unicorn-net](https://github.com/FICTURE7/unicorn-net), a .NET wrapper of the unicorn emulator.
* [commandlineparser](https://github.com/commandlineparser/commandline/), a library allowing easy management of arguments passed to the program.

## What exactly is the Steam DRM?
Quoting from Steam Steamworks documentation:
> The Steam DRM wrapper is an important part of Steam platform because it verifies game ownership and ensures that Steamworks features work properly by launching Steam before launching the game.

> The Steam DRM wrapper by itself is not an anti-piracy solution. The Steam DRM wrapper protects against extremely casual piracy (i.e. copying all game files to another computer) and has some obfuscation, but it is easily removed by a motivated attacker.

> We suggest enhancing the value of legitimate copies of your game by using Steamworks features which won't work on non-legitimate copies (e.g. online multiplayer, achievements, leaderboards, trading cards, etc.).

