# EPaperHatCore 
EPaperHatCore is a dotnet core library for Waveshare e-Paper HAT display

This library provides a simple way to access e-Paper display on the Raspberry Pi.

[![Build status](https://ci.appveyor.com/api/projects/status/nol7v0n0y2hjipl0?svg=true)](https://ci.appveyor.com/project/paw3lx/epaperhatcore)


## Installation

EPaperHatCore is available on [MyGet](https://www.myget.org/F/epaperhatcore/api/v3/index.json)

[![MyGet](https://img.shields.io/myget/epaperhatcore/v/EPaperHatCore.svg)](https://www.myget.org/feed/epaperhatcore/package/nuget/EPaperHatCore) 

### Package manager
```bash
Install-Package EPaperHatCore -Version 0.1.0 -Source https://www.myget.org/F/epaperhatcore/api/v3/index.json
```

### .NET CLI
```bash
dotnet add package EPaperHatCore --version 0.1.0 --source https://www.myget.org/F/epaperhatcore/api/v3/index.json
```

## Getting started

```cs
//initialize ePaper display
var ePaper = new Epaper(176, 264);
ePaper.Initialize();

//create black and red screens
var blackScreen = new Screen(176, 264, Rotate.ROTATE_270, Constants.WHITE);
var redScreen = new Screen(176, 264, Rotate.ROTATE_270, Constants.WHITE);

//draw something on screen using a font
var font = new Font();
blackScreen.DrawString(10, 20, "Blac text", font, Color.WHITE, Color.BLACK);
blackScreen.DrawString(10, 50, "Red text", font, Color.WHITE, Color.RED);

ePaper.DisplayScreens(blackScreen, redScreen);
```