using System;
using EPaperHatCore;
using EPaperHatCore.GUI;
using EPaperHatCore.GUI.Fonts;
using Shouldly;
using Xunit;
using static EPaperHatCore.GUI.Enums;

namespace EPaperHatCore.Tests;
public class ScreenTests
{
    [Fact]
    public void Unsupperted_Character_Throws()
    {
        var screen = new Screen(176, 264, Rotate.ROTATE_270, 0);

        var exception = Record.Exception(() =>
        {
            screen.DrawString(10, 10, "ł", new Font16(), Color.WHITE, Color.BLACK);
        });

        exception.ShouldBeOfType(typeof(ArgumentException));
    }

    [Fact]
    public void String_XPosition_Biggen_Than_Width_Throws()
    {
        var screen = new Screen(176, 264, Rotate.ROTATE_270, 0);

        var exception = Record.Exception(() =>
        {
            screen.DrawString(1000, 10, "ScreenTest", new Font16(), Color.WHITE, Color.BLACK);
        });

        exception.ShouldBeOfType(typeof(ArgumentOutOfRangeException));
    }

    [Fact]
    public void String_YPosition_Biggen_Than_Width_Throws()
    {
        var screen = new Screen(176, 264, Rotate.ROTATE_270, 0);

        var exception = Record.Exception(() =>
        {
            screen.DrawString(0, 1000, "ScreenTest", new Font16(), Color.WHITE, Color.BLACK);
        });

        exception.ShouldBeOfType(typeof(ArgumentOutOfRangeException));
    }

    [Fact]
    public void Charater_XPosition_Biggen_Than_Width_Throws()
    {
        var screen = new Screen(176, 264, Rotate.ROTATE_270, 0);

        var exception = Record.Exception(() =>
        {
            screen.DrawCharachter(1000, 10, 'e', new Font16(), Color.WHITE, Color.BLACK);
        });

        exception.ShouldBeOfType(typeof(ArgumentOutOfRangeException));
    }

    [Fact]
    public void Charater_YPosition_Biggen_Than_Width_Throws()
    {
        var screen = new Screen(176, 264, Rotate.ROTATE_270, 0);

        var exception = Record.Exception(() =>
        {
            screen.DrawCharachter(0, 1000, 'e', new Font16(), Color.WHITE, Color.BLACK);
        });

        exception.ShouldBeOfType(typeof(ArgumentOutOfRangeException));
    }

    [Fact]
    public void Pixel_XPosition_Biggen_Than_Width_Throws()
    {
        var screen = new Screen(176, 264, Rotate.ROTATE_270, 0);

        var exception = Record.Exception(() =>
        {
            screen.SetPixel(1000, 10, Color.RED);
        });

        exception.ShouldBeOfType(typeof(ArgumentOutOfRangeException));
    }

    [Fact]
    public void Pixel_YPosition_Biggen_Than_Width_Throws()
    {
        var screen = new Screen(176, 264, Rotate.ROTATE_270, 0);

        var exception = Record.Exception(() =>
        {
            screen.SetPixel(0, 1000, Color.RED);
        });

        exception.ShouldBeOfType(typeof(ArgumentOutOfRangeException));
    }
}
