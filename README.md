# Old School Adventure

 <div align="center">
  <img height="384" alt="Old School Adventure" src="https://raw.githubusercontent.com/ZenithMoonStudios/OldSchoolAdventure/refs/heads/main/Logo.png">
</div>

**Version**: v1.2

**Visual Studio requirements**: version 2022 with the .NET 8.0 workload.

This is the main repository for [Old School Adventure](https://www.microsoft.com/store/apps/9NBLGGH0DLHC) which was originally published using XNA on Xbox (XBLIG) and Windows Phone.

Old School Adventure is coded in C# with [MonoGame](https://monogame.net/) 3.8.2.

## All rights reserved

Old School Adventure © 2017-2024 ZenithMoon Studios. All rights reserved.

This is **not** a free software. You can't use code or assets for your own projects.

Old School Adventure was originally created by [Chris Hughes](https://x.com/chrishughes2d) of [Chris Hughes Games](https://chrishughes.games/) and redistributed on Windows Phone by [ZenithMoon Studios](https://zenithmoon.com/)

## Educational fair use

ZenithMoon Studios is allowing derivative works for **educational purposes** only (e.g. learning MonoGame or learning to make games) under these conditions:

- No commercial use can be made
- Obligation to credit Chris Hughes Games and ZenithMoon Studios
- Obligation to detail which part is being used or modified

## Repository content

This repository contains both the game source code and its level editor.

The code has been migrated to MonoGame from its original XNA source, updated and tested to be working.

The code and content is found in the [Source](/Source/) folder with the following directories:

- [Source\Destiny](/Source/Destiny/) - Engine style code for the Old School Adventure.
- [Source\OldSchoolAdventure](/Source/OldSchoolAdventure/) - Main game implementation code.
- [Source\OldSchoolAdventure\Content](/Source/OldSchoolAdventure/Content/) - All game content and xml configuration files.
- [Source\ThirdParty](/Source/ThirdParty/) - Original platform addons, no longer used but added for reference.

> [!NOTE]
> While the solution uses XML for the configuration of the title, the implementation is not "best practice" for future MonoGame development as XML serializers are not recommended for MonoGame titles due to their limited support for consoles and NativeAoT.

## Documentation - a work in progress

Documentation will follow the initial release.

Some elements of the functionality are included but are no longer in use as they were added for the Windows Phone release, namely:

- ThirdParty\AccelerometerHelper - uses redundant Accelerometer API and needs updating.
- ThirdParty\EasyStorage - Relies on XNA StorageDevice implementation, needs new storage access (although saving/loading functionality has been updated for MonoGame)

## This is not a community project

This repository sole purpose is for educational use.

If you wish to contribute fixes or enhancements to the product to further MonoGame education, then pull requests are welcome.

## Please consider thanking us

Please consider supporting [Chris Hughes Games](https://chrishughes.games/) with their titles released on iOS.

As well as supporting [Simon "Darkside" Jackson](https://github.com/sponsors/SimonDarksideJ) through GitHub sponsors in his efforts to build our MonoGame educational material for the [MonoGame Foundation](https://monogame.net/).

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/S6S84ZRM4)