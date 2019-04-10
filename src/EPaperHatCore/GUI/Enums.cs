namespace BetaSoft.EPaperHatCore.GUI
{
    public class Enums
    {
        public enum MirrorImage
        {
            MIRROR_NONE = 0x00,
            MIRROR_HORIZONTAL = 0x01,
            MIRROR_VERTICAL = 0x02,
            MIRROR_ORIGIN = 0x03
        }
        
        public enum Rotate
        {
            ROTATE_0 = 0,
            ROTATE_90 = 90,
            ROTATE_180 = 180,
            ROTATE_270 = 270
        }

        public enum Color
        {
            WHITE = 0xFF,
            BLACK = 0x00,
            RED = Color.BLACK
        }

        public enum DotStyle
        {
            DOT_FILL_AROUND,
            DOT_FILL_RIGHTUP
        }

        public enum LineStyle
        {
            LINE_STYLE_SOLID,
            LINE_STYLE_DOTTED
        }
    }
}