using System;
using System.IO;
using BetaSoft.EPaperHatCore;
using BetaSoft.EPaperHatCore.GUI;
using Shouldly;
using Xunit;
using static BetaSoft.EPaperHatCore.GUI.Enums;

namespace Betasoft.EPaperHatCore.Tests
{
    public class ScreenTests
    {
        [Fact]
        public void Unsupperted_Character_Throws()
        {
            var screen = new Screen(176, 264, Rotate.ROTATE_270, 0);

            var exception = Record.Exception(() => {
                screen.DrawString(10, 10, "ł", new Font(), Color.WHITE, Color.BLACK);
            });

            exception.ShouldBeOfType(typeof(ArgumentException));
        }

        [Fact]
        public void String_XPosition_Biggen_Than_Width_Throws()
        {
            var screen = new Screen(176, 264, Rotate.ROTATE_270, 0);

            var exception = Record.Exception(() => {
                screen.DrawString(1000, 10, "ScreenTest", new Font(), Color.WHITE, Color.BLACK);
            });

            exception.ShouldBeOfType(typeof(ArgumentOutOfRangeException));
        }

        [Fact]
        public void String_YPosition_Biggen_Than_Width_Throws()
        {
            var screen = new Screen(176, 264, Rotate.ROTATE_270, 0);

            var exception = Record.Exception(() => {
                screen.DrawString(0, 1000, "ScreenTest", new Font(), Color.WHITE, Color.BLACK);
            });

            exception.ShouldBeOfType(typeof(ArgumentOutOfRangeException));
        }

        [Fact]
        public void Charater_XPosition_Biggen_Than_Width_Throws()
        {
            var screen = new Screen(176, 264, Rotate.ROTATE_270, 0);

            var exception = Record.Exception(() => {
                screen.DrawCharachter(1000, 10, 'e', new Font(), Color.WHITE, Color.BLACK);
            });

            exception.ShouldBeOfType(typeof(ArgumentOutOfRangeException));
        }

        [Fact]
        public void Charater_YPosition_Biggen_Than_Width_Throws()
        {
            var screen = new Screen(176, 264, Rotate.ROTATE_270, 0);

            var exception = Record.Exception(() => {
                screen.DrawCharachter(0, 1000, 'e', new Font(), Color.WHITE, Color.BLACK);
            });

            exception.ShouldBeOfType(typeof(ArgumentOutOfRangeException));
        }
    }
}
