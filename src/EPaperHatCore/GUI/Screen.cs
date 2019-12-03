using System;
using static BetaSoft.EPaperHatCore.GUI.Enums;
using BetaSoft.EPaperHatCore.GUI.Fonts;

namespace BetaSoft.EPaperHatCore.GUI
{
    public class Screen
    {
        public Screen(uint width, uint height, Rotate? rotate = null, Color? backgroundColor = null)
        {
            var imageSize = ((width % 8 == 0) ? (width / 8) : (width / 8 + 1)) * height;
            Image = new byte[imageSize];

            _widthMemory = width;
            _heightMemory = height;
            _widthByte = (width % 8 == 0) ? (width / 8) : (width / 8 + 1);
            _heightByte = height;

            Rotate = rotate ?? Rotate.ROTATE_0;
            Mirror = MirrorImage.MIRROR_NONE;

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

            if (backgroundColor.HasValue)
            {
                Clear(backgroundColor.Value);
            }
        }

        public byte[] Image { get; }
        public uint Width { get; }
        public uint Height { get; }
        public Rotate Rotate { get; }
        public MirrorImage Mirror { get; }
        private readonly uint _widthMemory;
        private readonly uint _heightMemory;
        private readonly uint _widthByte;
        private readonly uint _heightByte;

        public void SetPixel(uint xPoint, uint yPoint, Color color)
        {
            if (xPoint > Width)
                throw new ArgumentOutOfRangeException(nameof(xPoint), $"{nameof(xPoint)} cannot be bigger than {nameof(Width)}");
            if (yPoint > Height)
                throw new ArgumentOutOfRangeException(nameof(yPoint), $"{nameof(yPoint)} cannot be bigger than {nameof(Height)}");
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

            if (x > _widthMemory)
                throw new ArgumentOutOfRangeException(nameof(x), $"{nameof(x)} cannot be bigger than width");
            if (y > _heightMemory)
                throw new ArgumentOutOfRangeException(nameof(y), $"{nameof(y)} cannot be bigger than height");

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
            IFont font, Color backgroundColor, Color foregroundColor)
        {
            if (xPoint > Width)
                throw new ArgumentOutOfRangeException(nameof(xPoint), $"{nameof(xPoint)} cannot be bigger than {nameof(Width)}");
            if (yPoint > Height)
                throw new ArgumentOutOfRangeException(nameof(yPoint), $"{nameof(yPoint)} cannot be bigger than {nameof(Height)}");

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
            IFont font, Color backgroundColor, Color foregroundColor)
        {
            uint xPoint = xStart;
            uint yPoint = yStart;

            if (xStart > Width)
                throw new ArgumentOutOfRangeException(nameof(xStart), $"{nameof(xStart)} cannot be bigger than {nameof(Width)}");
            if (yStart > Height)
                throw new ArgumentOutOfRangeException(nameof(yStart), $"{nameof(yStart)} cannot be bigger than {nameof(Height)}");

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

        public void DrawPoint(uint xStart, uint yStart, Color Color, uint dotSize, DotStyle dotStyle)
        {
            if (xStart > Width)
                throw new ArgumentOutOfRangeException(nameof(xStart), $"{nameof(xStart)} cannot be bigger than {nameof(Width)}");
            if (yStart > Height)
                throw new ArgumentOutOfRangeException(nameof(yStart), $"{nameof(yStart)} cannot be bigger than {nameof(Height)}");

            if (dotSize <= 0 || dotSize >= 8)
                throw new ArgumentOutOfRangeException(nameof(dotSize), $"{nameof(yStart)} cannot be less than 0 and bigger than 8");

            if (dotStyle == DotStyle.DOT_FILL_AROUND) 
            {
                for (uint xDir = 0; xDir < 2 * dotSize - 1; xDir++)
                {
                    for (uint yDir = 0; yDir < 2 * dotSize - 1; yDir++)
                    {
                        if(xStart + xDir - dotSize < 0 || yStart + yDir - dotSize < 0)
                            break;
                        SetPixel(xStart + xDir - dotSize, yStart + yDir - dotSize, Color);
                    }
                }
            } 
            else 
            {
                for (uint xDir = 0; xDir <  dotSize; xDir++)
                {
                    for (uint yDir = 0; yDir <  dotSize; yDir++)
                    {
                        SetPixel(xStart + xDir - 1, yStart + yDir - 1, Color);
                    }
                }
            }
        }

        public void DrawLine(uint xStart, uint yStart, uint xEnd, uint yEnd, Color Color, LineStyle lineStyle, uint dotSize)
        {
            if (xStart > Width)
                throw new ArgumentOutOfRangeException(nameof(xStart), $"{nameof(xStart)} cannot be bigger than {nameof(Width)}");
            if (yStart > Height)
                throw new ArgumentOutOfRangeException(nameof(yStart), $"{nameof(yStart)} cannot be bigger than {nameof(Height)}");
            if (xEnd > Width)
                throw new ArgumentOutOfRangeException(nameof(xEnd), $"{nameof(xEnd)} cannot be bigger than {nameof(Width)}");
            if (yEnd > Height)
                throw new ArgumentOutOfRangeException(nameof(yEnd), $"{nameof(yEnd)} cannot be bigger than {nameof(Height)}");

            if (dotSize <= 0 || dotSize >= 8)
                throw new ArgumentOutOfRangeException(nameof(dotSize), $"{nameof(yStart)} cannot be less than 0 and bigger than 8");

            uint xPoint = xStart;
            uint yPoint = yStart;
            int dx = (int)(xEnd - xStart) >= 0 ? (int)(xEnd - xStart) : (int)(xStart - xEnd);
            int dy = (int)(yEnd - yStart) <= 0 ? (int)(yEnd - yStart) : (int)(yStart - yEnd);

            int xIncrDecr = xStart < xEnd ? 1 : -1;
            int yIncDecr = yStart < yEnd ? 1 : -1;

            int esp = dx + dy;
            uint dotLen = 0;

            while(true) {
                dotLen++;

                if (lineStyle == LineStyle.LINE_STYLE_DOTTED && dotLen % 3 == 0) {
                    DrawPoint(xPoint, yPoint, Color.WHITE, dotSize, DotStyle.DOT_FILL_AROUND);
                    dotLen = 0;
                } else {
                    DrawPoint(xPoint, yPoint, Color, dotSize, DotStyle.DOT_FILL_AROUND);
                }

                if (2 * esp >= dy) {
                    if (xPoint == xEnd)
                        break;
                    esp += dy;
                    xPoint = (uint)(xPoint + xIncrDecr);
                }
                if (2 * esp <= dx) {
                    if (yPoint == yEnd)
                        break;
                    esp += dx;
                    yPoint = (uint)(yPoint + yIncDecr);
                }
            }
        }

        public void DrawRectangle(uint xStart, uint yStart, uint xEnd, uint yEnd, Color color, bool filled, uint dotSize)
        {
            if (xStart > Width)
                throw new ArgumentOutOfRangeException(nameof(xStart), $"{nameof(xStart)} cannot be bigger than {nameof(Width)}");
            if (yStart > Height)
                throw new ArgumentOutOfRangeException(nameof(yStart), $"{nameof(yStart)} cannot be bigger than {nameof(Height)}");
            if (xEnd > Width)
                throw new ArgumentOutOfRangeException(nameof(xEnd), $"{nameof(xEnd)} cannot be bigger than {nameof(Width)}");
            if (yEnd > Height)
                throw new ArgumentOutOfRangeException(nameof(yEnd), $"{nameof(yEnd)} cannot be bigger than {nameof(Height)}");

            if (filled ) {
                uint Ypoint;
                for(Ypoint = yStart; Ypoint < yEnd; Ypoint++) {
                    DrawLine(xStart, Ypoint, xEnd, Ypoint, color , LineStyle.LINE_STYLE_SOLID, dotSize);
                }
            } else {
                DrawLine(xStart, yStart, xEnd, yStart, color , LineStyle.LINE_STYLE_SOLID, dotSize);
                DrawLine(xStart, yStart, xStart, yEnd, color , LineStyle.LINE_STYLE_SOLID, dotSize);
                DrawLine(xEnd, yEnd, xEnd, yStart, color , LineStyle.LINE_STYLE_SOLID, dotSize);
                DrawLine(xEnd, yEnd, xStart, yEnd, color , LineStyle.LINE_STYLE_SOLID, dotSize);
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