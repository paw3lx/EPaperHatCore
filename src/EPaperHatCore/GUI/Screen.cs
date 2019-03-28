using System;
using static BetaSoft.EPaperHatCore.GUI.Enums;

namespace BetaSoft.EPaperHatCore.GUI
{
    public class Screen
    {
        public Screen(uint width, uint height, Rotate rotate, uint color)
        {
            var imageSize = ((width % 8 == 0) ? (width / 8) : (width / 8 + 1)) * height;
            Image = new byte[imageSize];

            _widthMemory = width;
            _heightMemory = height;
            _widthByte = (width % 8 == 0) ? (width / 8) : (width / 8 + 1);
            _heightByte = height;

            Rotate = rotate;
            Mirror = MirrorImage.MIRROR_NONE;
            //Color = color;

            if (rotate == Rotate.ROTATE_0 || rotate == Rotate.ROTATE_180)
            {
                Width = width;
                Height = height;
            }
            else
            {
                Width = height;
                Height = width;
            }
        }

        public byte[] Image { get; }
        public uint Width { get; }
        public uint Height { get; }
        //public uint Color { get; }
        public Rotate Rotate { get; }
        public MirrorImage Mirror { get; }
        private readonly uint _widthMemory;
        private readonly uint _heightMemory;
        private readonly uint _widthByte;
        private readonly uint _heightByte;

        public void SetPixel(uint xPoint, uint yPoint, Color color)
        {
            if (xPoint > Width || yPoint > Height)
            {
                //TODO: throw exception
                return;
            }
            uint x = 0, y = 0;
            switch (Rotate)
            {
                case Rotate.ROTATE_0:
                    x = xPoint;
                    y = yPoint;
                    break;
                case Rotate.ROTATE_90:
                    x = _widthMemory - yPoint - 1;
                    y = xPoint;
                    break;
                case Rotate.ROTATE_180:
                    x = _widthMemory - xPoint - 1;
                    y = _heightMemory - yPoint - 1;
                    break;
                case Rotate.ROTATE_270:
                    x = yPoint;
                    y = _heightMemory - xPoint - 1;
                    break;
            }

            switch (Mirror)
            {
                case MirrorImage.MIRROR_NONE:
                    break;
                case MirrorImage.MIRROR_HORIZONTAL:
                    x = _widthMemory - x - 1;
                    break;
                case MirrorImage.MIRROR_VERTICAL:
                    y = _heightMemory - y - 1;
                    break;
                case MirrorImage.MIRROR_ORIGIN:
                    x = _widthMemory - x - 1;
                    y = _heightMemory - y - 1;
                    break;
            }

            if (x > _widthMemory || y > _heightMemory)
            {
                //TODO: throw exception
                return;
            }

            int address = (int)(x / 8 + y * _widthByte);
            byte rData = Image[address];
            if (color == Color.BLACK)
            {
                Image[address] = (byte)(rData & ~(0x80 >> (int)(x % 8)));
            }
            else
            {
                Image[address] = (byte)(rData | (0x80 >> (int)(x % 8)));
            }
        }

        public void DrawCharachter(uint xPoint, uint yPoint, char asciiChar,
            Font font, Color backgroundColor, Color foregroundColor)
        {
            if (xPoint > Width || yPoint > Height)
            {
                //TODO: throw exception
                return;
            }

            int charOffset = (int)((asciiChar - ' ') * font.Height * (font.Width / 8 + (font.Width % 8 > 0 ? 1 : 0)));
            for (uint page = 0; page < font.Height; page++)
            {
                for (uint column = 0; column < font.Width; column++)
                {
                    if (charOffset >= font.Table.Length)
                    {
                        throw new ArgumentException($"Character '{asciiChar}' is not available in the font");
                    }
                    char character = font.Table[charOffset];
                    //To determine whether the font background color and screen background color is consistent
                    if (backgroundColor == Color.WHITE)
                    { //this process is to speed up the scan

                        if ((character & (0x80 >> (int)(column % 8))) != 0)
                            SetPixel(xPoint + column, yPoint + page, foregroundColor);
                    }
                    else
                    {
                        if ((character & (0x80 >> (int)(column % 8))) != 0)
                        {
                            SetPixel(xPoint + column, yPoint + page, foregroundColor);
                        }
                        else
                        {
                            SetPixel(xPoint + column, yPoint + page, backgroundColor);
                        }
                    }
                    //One pixel is 8 bits
                    if (column % 8 == 7)
                        charOffset++;
                }// Write a line
                if (font.Width % 8 != 0)
                    charOffset++;
            }// Write all
        }

        public void DrawString(uint xStart, uint yStart, string text,
            Font font, Color backgroundColor, Color foregroundColor)
        {
            uint xPoint = xStart;
            uint yPoint = yStart;

            if (xStart > Width || yStart > Height)
            {
                //TODO: throw exception
                return;
            }

            foreach (char c in text)
            {
                //if X direction filled , reposition to(Xstart,Ypoint),Ypoint is Y direction plus the Height of the character
                if ((xPoint + font.Width) > Width)
                {
                    xPoint = xStart;
                    yPoint += font.Height;
                }

                // If the Y direction is full, reposition to(Xstart, Ystart)
                if ((yPoint + font.Height) > Height)
                {
                    xPoint = xStart;
                    yPoint = yStart;
                }
                DrawCharachter(xPoint, yPoint, c, font, backgroundColor, foregroundColor);

                //The next word of the abscissa increases the font of the broadband
                xPoint += font.Width;
            }
        }

        public void Clear(Color color)
        {
            for (int Y = 0; Y < _heightByte; Y++)
            {
                for (int X = 0; X < _widthByte; X++)
                {
                    int Addr = (int)(X + Y * _widthByte);
                    byte byteColor = (byte)color;
                    this.Image[Addr] = byteColor;
                }
            }
        }
    }
}