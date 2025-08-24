# EPaperHatCore

EPaperHatCore is a .NET 8.0 library for controlling Waveshare e-Paper HAT displays on Raspberry Pi. This library provides a simple and intuitive API for working with e-Paper displays, supporting various screen sizes and drawing operations.

[![CI](https://github.com/paw3lx/epaperhatcore/actions/workflows/ci.yml/badge.svg)](https://github.com/paw3lx/epaperhatcore/actions/workflows/ci.yml)

## Features

- 🖥️ Support for multiple e-Paper display sizes
- 🎨 Drawing operations: text, lines, rectangles, and points
- 🔄 Screen rotation support (0°, 90°, 180°, 270°)
- 📝 Multiple font sizes (8pt, 12pt, 16pt, 20pt)
- 🎯 Two-color display support (black and red)
- 🔧 Built on System.Device.Gpio for reliable GPIO communication
- ⚡ .NET 8.0 performance and compatibility

## Supported E-Paper Displays

| Display Size | Resolution | Class Name | Colors |
|-------------|------------|------------|--------|
| 2.7 inch | 176×264 | `Epaper27` | Black, Red |
| 2.13 inch v2 | 128×250 | `Epaper213v2` | Black |

## Dependencies

- .NET 8.0 or later
- System.Device.Gpio 3.1.0 (automatically installed)
- Raspberry Pi with GPIO support

## Installation

EPaperHatCore is available on [MyGet](https://www.myget.org/feed/epaperhatcore/package/nuget/EPaperHatCore)

[![MyGet](https://img.shields.io/myget/epaperhatcore/v/EPaperHatCore.svg??style=flat-square)](https://www.myget.org/feed/epaperhatcore/package/nuget/EPaperHatCore) 

### Package Manager
```bash
Install-Package EPaperHatCore -Source https://www.myget.org/F/epaperhatcore/api/v3/index.json
```

### .NET CLI
```bash
dotnet add package EPaperHatCore --source https://www.myget.org/F/epaperhatcore/api/v3/index.json
```

## Getting Started

### Basic Example - 2.7 inch Display
```csharp
using EPaperHatCore;
using EPaperHatCore.GUI;
using EPaperHatCore.GUI.Fonts;
using static EPaperHatCore.GUI.Enums;

// Initialize the e-Paper display (2.7 inch)
var ePaper = new Epaper27(176, 264);
ePaper.Initialize();

// Create black and red screens
var blackScreen = new Screen(176, 264, Rotate.ROTATE_270, Color.WHITE);
var redScreen = new Screen(176, 264, Rotate.ROTATE_270, Color.WHITE);

// Draw text using different fonts
var font = new Font8();
var font2 = new Font20();
blackScreen.DrawString(10, 20, "Hello World!", font, Color.WHITE, Color.BLACK);
redScreen.DrawString(10, 50, "EPaperHatCore", font2, Color.WHITE, Color.RED);

// Display the screens
ePaper.DisplayScreens(blackScreen, redScreen);
```

### Basic Example - 2.13 inch v2 Display
```csharp
using EPaperHatCore;
using EPaperHatCore.GUI;
using EPaperHatCore.GUI.Fonts;
using static EPaperHatCore.GUI.Enums;

// Initialize the e-Paper display (2.13 inch v2)
var ePaper = new Epaper213v2(128, 250);
ePaper.Initialize();

// Create a black and white screen
var screen = new Screen(128, 250, Rotate.ROTATE_0, Color.WHITE);

// Draw text and shapes
var font = new Font16();
screen.DrawString(10, 10, "EPaperHatCore", font, Color.WHITE, Color.BLACK);
screen.DrawRectangle(10, 40, 100, 60, Color.BLACK, false, 1);
screen.DrawLine(10, 70, 100, 90, Color.BLACK, LineStyle.LINE_STYLE_SOLID, 1);

// Display the screen
ePaper.DisplayScreens(screen);
```

### Advanced Drawing Operations
```csharp
// Drawing shapes and text with various options
var screen = new Screen(176, 264, Rotate.ROTATE_0, Color.WHITE);

// Draw filled rectangle
screen.DrawRectangle(20, 20, 80, 60, Color.BLACK, filled: true, dotSize: 1);

// Draw dotted line
screen.DrawLine(10, 100, 150, 120, Color.BLACK, LineStyle.LINE_STYLE_DOTTED, 2);

// Draw points with different sizes
screen.DrawPoint(50, 150, Color.BLACK, dotSize: 3, DotStyle.DOT_FILL_AROUND);

// Text with different font sizes
var font8 = new Font8();
var font12 = new Font12();
var font16 = new Font16();
var font20 = new Font20();

screen.DrawString(10, 180, "Font 8pt", font8, Color.WHITE, Color.BLACK);
screen.DrawString(10, 200, "Font 12pt", font12, Color.WHITE, Color.BLACK);
screen.DrawString(10, 220, "Font 16pt", font16, Color.WHITE, Color.BLACK);
screen.DrawString(10, 240, "Font 20pt", font20, Color.WHITE, Color.BLACK);
```

## API Reference

### Core Classes

#### `Epaper27(int width, int height)`
Driver for 2.7 inch e-Paper displays with black and red color support.

#### `Epaper213v2(int width, int height)`
Driver for 2.13 inch v2 e-Paper displays with black and white support.

#### `Screen(uint width, uint height, Rotate rotation, Color backgroundColor)`
Represents a drawable screen buffer with various drawing methods.

### Drawing Methods

- `DrawString(x, y, text, font, bgColor, fgColor)` - Draw text
- `DrawRectangle(x1, y1, x2, y2, color, filled, dotSize)` - Draw rectangles
- `DrawLine(x1, y1, x2, y2, color, lineStyle, dotSize)` - Draw lines
- `DrawPoint(x, y, color, dotSize, dotStyle)` - Draw points
- `SetPixel(x, y, color)` - Set individual pixels
- `Clear(color)` - Clear screen with specified color

### Enumerations

- `Color`: `BLACK`, `WHITE`, `RED`
- `Rotate`: `ROTATE_0`, `ROTATE_90`, `ROTATE_180`, `ROTATE_270`
- `LineStyle`: `LINE_STYLE_SOLID`, `LINE_STYLE_DOTTED`
- `DotStyle`: `DOT_FILL_AROUND`, `DOT_FILL_RIGHTUP`

## Hardware Setup

1. Connect the e-Paper HAT to your Raspberry Pi GPIO pins
2. Ensure your Raspberry Pi has .NET 8.0 runtime installed
3. Run your application with appropriate GPIO permissions

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.