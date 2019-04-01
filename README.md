# EPaperHatCore 
EPaperHatCore is a dotnet core library for Waveshare e-Paper HAT display

This library provides a simple way to access e-Paper display on the Raspberry Pi.

## Installation
EPaperHatCore will be available as a package on MyGet soonish.

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