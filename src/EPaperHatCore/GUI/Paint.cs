using static BetaSoft.EPaperHatCore.GUI.Enums;

namespace BetaSoft.EPaperHatCore.GUI
{
    public class Paint
    {
        public byte[] Image { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public uint WidthMemory { get; set; }
        public uint HeightMemory { get; set; }
        public uint Color { get; set; }
        public Rotate Rotate { get; set; }
        public MirrorImage Mirror { get; set; }
        public uint WidthByte { get; set; }
        public uint HeightByte { get; set; }

        public Paint(uint width, uint height, Rotate rotate, uint color)
        {
            var imageSize = ((width % 8 == 0) ? (width / 8) : (width / 8 + 1)) * height;
            Image = new byte[imageSize];

            WidthMemory = width;
            HeightMemory = height;
            Color = color;
            WidthByte = (width % 8 == 0) ? (width / 8) : (width / 8 + 1);
            HeightByte = height;
            Rotate = rotate;
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
        }
        public void SetPixel(uint xPoint, uint yPoint, uint color)
        {
            if (xPoint > Width || yPoint > Height)
            {
                // exception
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
                    x = WidthMemory - yPoint - 1;
                    y = xPoint;
                    break;
                case Rotate.ROTATE_180:
                    x = WidthMemory - xPoint - 1;
                    y = HeightMemory - yPoint - 1;
                    break;
                case Rotate.ROTATE_270:
                    x = yPoint;
                    y = HeightMemory - xPoint - 1;
                    break;
            }

            switch (Mirror)
            {
                case MirrorImage.MIRROR_NONE:
                    break;
                case MirrorImage.MIRROR_HORIZONTAL:
                    x = WidthMemory - x - 1;
                    break;
                case MirrorImage.MIRROR_VERTICAL:
                    y = HeightMemory - y - 1;
                    break;
                case MirrorImage.MIRROR_ORIGIN:
                    x = WidthMemory - x - 1;
                    y = HeightMemory - y - 1;
                    break;
            }

            if (x > WidthMemory || y > HeightMemory)
            {
                return;
            }

            int address = (int)(x / 8 + y * WidthByte);
            byte rData = Image[address];
            if (color == Constans.BLACK)
            {
                Image[address] = (byte)(rData & ~(0x80 >> (int)(x % 8)));
            }
            else
            {
                Image[address] = (byte)(rData | (0x80 >> (int)(x % 8)));
            }
        }

        public void Paint_DrawChar(uint Xpoint, uint Ypoint, char Acsii_Char,
                            Font font, uint Color_Background, uint Color_Foreground)
        {
            uint Page, Column;

            if (Xpoint > Width || Ypoint > Height)
            {
                //Debug("Paint_DrawChar Input exceeds the normal display range\r\n");
                return;
            }

            int Char_Offset = (int)((Acsii_Char - ' ') * font.Height * (font.Width / 8 + (font.Width % 8 > 0 ? 1 : 0)));
            for (Page = 0; Page < font.Height; Page++)
            {
                for (Column = 0; Column < font.Width; Column++)
                {
                    char character = font.Table[Char_Offset];
                    //To determine whether the font background color and screen background color is consistent
                    if (Constans.FONT_BACKGROUND == Color_Background)
                    { //this process is to speed up the scan

                        if ((character & (0x80 >> (int)(Column % 8))) != 0)
                            SetPixel(Xpoint + Column, Ypoint + Page, Color_Foreground);
                    }
                    else
                    {
                        if ((character & (0x80 >> (int)(Column % 8))) != 0)
                        {
                            SetPixel(Xpoint + Column, Ypoint + Page, Color_Foreground);
                        }
                        else
                        {
                            SetPixel(Xpoint + Column, Ypoint + Page, Color_Background);
                        }
                    }
                    //One pixel is 8 bits
                    if (Column % 8 == 7)
                        Char_Offset++;
                }// Write a line
                if (font.Width % 8 != 0)
                    Char_Offset++;
            }// Write all
        }

        public void Paint_DrawString_EN(uint Xstart, uint Ystart, string pString,
                         Font font, uint Color_Background, uint Color_Foreground)
        {
            uint xPoint = Xstart;
            uint yPoint = Ystart;

            if (Xstart > this.Width || Ystart > this.Height)
            {
                //Debug("Paint_DrawString_EN Input exceeds the normal display range\r\n");
                return;
            }

            foreach (char c in pString)
            {
                //if X direction filled , reposition to(Xstart,Ypoint),Ypoint is Y direction plus the Height of the character
                if ((xPoint + font.Width) > this.Width)
                {
                    xPoint = Xstart;
                    yPoint += font.Height;
                }

                // If the Y direction is full, reposition to(Xstart, Ystart)
                if ((yPoint + font.Height) > this.Height)
                {
                    xPoint = Xstart;
                    yPoint = Ystart;
                }
                Paint_DrawChar(xPoint, yPoint, c, font, Color_Background, Color_Foreground);

                //The next word of the abscissa increases the font of the broadband
                xPoint += font.Width;
            }
        }

        public void Paint_Clear(byte Color)
        {
            // Debug("x = %d, y = %d\r\n", Paint.WidthByte, Paint.Height);
            for (int Y = 0; Y < this.HeightByte; Y++) {
                for (int X = 0; X < this.WidthByte; X++ ) {//8 pixel =  1 byte
                    int Addr = (int)(X + Y*this.WidthByte);
                    this.Image[Addr] = Color;
                }
            }
        }
    }
}