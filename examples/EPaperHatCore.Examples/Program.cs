using System;
using EPaperHatCore;
using EPaperHatCore.GUI;
using EPaperHatCore.GUI.Fonts;
using static EPaperHatCore.GUI.Enums;

namespace EPaperHatCore.Examples;
    class Program
    {
        static void Main(string[] args)
        {
            //initialize ePaper display
            var ePaper = new Epaper27(176, 264);
            ePaper.Initialize();

            //create black and red screens
            var blackScreen = new Screen(176, 264, Rotate.ROTATE_270, Color.WHITE);
            var redScreen = new Screen(176, 264, Rotate.ROTATE_270, Color.WHITE);

            //draw something on screen using a font
            var font = new Font8();
            var font2 = new Font20();
            blackScreen.DrawString(10, 20, "Text", font, Color.WHITE, Color.BLACK);
            redScreen.DrawString(10, 50, "text", font2, Color.WHITE, Color.RED);

            ePaper.DisplayScreens(blackScreen, redScreen);
        }
    }
