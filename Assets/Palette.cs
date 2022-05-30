using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Palette
{
    // color pallettes and 2D tile textures
    public static Color[] CGAColorPalette;
    public static Color[] EGAColorPalette;
    public static Color[] Apple2ColorPalette;

    public enum CGA_COLOR
    {
        BLACK = 0,
        CYAN = 1,
        MEGENTA = 2,
        WHITE = 3,
        MAX = 4
    };

    public enum EGA_COLOR
    {
        BLACK = 0,
        BLUE = 1,
        GREEN = 2,
        CYAN = 3,
        RED = 4,
        MEGENTA = 5,
        BROWN = 6,
        LIGHT_GRAY = 7,
        DARK_GRAY = 8,
        BRIGHT_BLUE = 9,
        BRIGHT_GREEN = 10,
        BRIGHT_CYAN = 11,
        BRIGHT_RED = 12,
        BRIGHT_MEGENTA = 13,
        BRIGHT_YELLOW = 14,
        WHITE = 15,
        MAX = 16
    };

    public enum APPLE2_COLOR
    {
        BLACK = 0,
        GREEN = 1,
        PURPLE = 2,
        BLUE = 3,
        ORANGE = 4,
        GREENWHITE = 5,
        PURPLEWHITE = 6,
        BLUEWHITE = 7,
        ORANGEWHITE = 8,
        GREENBLACK = 9,
        PURPLEBLACK = 10,
        BLUEBLACK = 11,
        ORANGEBLACK = 12,
        WHITE = 13,
        MAX = 14
    };

    public static void InitializeEGAPalette()
    {
        // create a EGA color palette
        EGAColorPalette = new Color[(int)EGA_COLOR.MAX];
        ColorUtility.TryParseHtmlString("#000000", out EGAColorPalette[(int)EGA_COLOR.BLACK]);
        ColorUtility.TryParseHtmlString("#0000AA", out EGAColorPalette[(int)EGA_COLOR.BLUE]);
        ColorUtility.TryParseHtmlString("#00AA00", out EGAColorPalette[(int)EGA_COLOR.GREEN]);
        ColorUtility.TryParseHtmlString("#00AAAA", out EGAColorPalette[(int)EGA_COLOR.CYAN]);
        ColorUtility.TryParseHtmlString("#AA0000", out EGAColorPalette[(int)EGA_COLOR.RED]);
        ColorUtility.TryParseHtmlString("#AA00AA", out EGAColorPalette[(int)EGA_COLOR.MEGENTA]);
        ColorUtility.TryParseHtmlString("#AA5500", out EGAColorPalette[(int)EGA_COLOR.BROWN]);
        ColorUtility.TryParseHtmlString("#AAAAAA", out EGAColorPalette[(int)EGA_COLOR.LIGHT_GRAY]);
        ColorUtility.TryParseHtmlString("#555555", out EGAColorPalette[(int)EGA_COLOR.DARK_GRAY]);
        ColorUtility.TryParseHtmlString("#5555FF", out EGAColorPalette[(int)EGA_COLOR.BRIGHT_BLUE]);
        ColorUtility.TryParseHtmlString("#55FF55", out EGAColorPalette[(int)EGA_COLOR.BRIGHT_GREEN]);
        ColorUtility.TryParseHtmlString("#55FFFF", out EGAColorPalette[(int)EGA_COLOR.BRIGHT_CYAN]);
        ColorUtility.TryParseHtmlString("#FF5555", out EGAColorPalette[(int)EGA_COLOR.BRIGHT_RED]);
        ColorUtility.TryParseHtmlString("#FF55FF", out EGAColorPalette[(int)EGA_COLOR.BRIGHT_MEGENTA]);
        ColorUtility.TryParseHtmlString("#FFFF55", out EGAColorPalette[(int)EGA_COLOR.BRIGHT_YELLOW]);
        ColorUtility.TryParseHtmlString("#FFFFFF", out EGAColorPalette[(int)EGA_COLOR.WHITE]);
    }

    public static void InitializeCGAPalette()
    {
        // create CGA color palette
        CGAColorPalette = new Color[(int)CGA_COLOR.MAX];
        ColorUtility.TryParseHtmlString("#000000", out CGAColorPalette[(int)CGA_COLOR.BLACK]);
        ColorUtility.TryParseHtmlString("#00AAAA", out CGAColorPalette[(int)CGA_COLOR.CYAN]);
        ColorUtility.TryParseHtmlString("#AA00AA", out CGAColorPalette[(int)CGA_COLOR.MEGENTA]);
        ColorUtility.TryParseHtmlString("#AAAAAA", out CGAColorPalette[(int)CGA_COLOR.WHITE]);
    }

    public static void InitializeApple2Palette()
    {
        // create CGA color palette
        Apple2ColorPalette = new Color[(int)APPLE2_COLOR.MAX];
        ColorUtility.TryParseHtmlString("#000000", out Apple2ColorPalette[(int)APPLE2_COLOR.BLACK]);
        ColorUtility.TryParseHtmlString("#38CB00", out Apple2ColorPalette[(int)APPLE2_COLOR.GREEN]);
        ColorUtility.TryParseHtmlString("#C734FF", out Apple2ColorPalette[(int)APPLE2_COLOR.PURPLE]);
        ColorUtility.TryParseHtmlString("#0DA1FF", out Apple2ColorPalette[(int)APPLE2_COLOR.BLUE]);
        ColorUtility.TryParseHtmlString("#F25E00", out Apple2ColorPalette[(int)APPLE2_COLOR.ORANGE]);

        ColorUtility.TryParseHtmlString("#9ACB88", out Apple2ColorPalette[(int)APPLE2_COLOR.GREENWHITE]);
        ColorUtility.TryParseHtmlString("#E8ABFF", out Apple2ColorPalette[(int)APPLE2_COLOR.PURPLEWHITE]);
        ColorUtility.TryParseHtmlString("#ABDFFF", out Apple2ColorPalette[(int)APPLE2_COLOR.BLUEWHITE]);
        ColorUtility.TryParseHtmlString("#F2C1A2", out Apple2ColorPalette[(int)APPLE2_COLOR.ORANGEWHITE]);

        ColorUtility.TryParseHtmlString("#124000", out Apple2ColorPalette[(int)APPLE2_COLOR.GREENBLACK]);
        ColorUtility.TryParseHtmlString("#320D40", out Apple2ColorPalette[(int)APPLE2_COLOR.PURPLEBLACK]);
        ColorUtility.TryParseHtmlString("#042940", out Apple2ColorPalette[(int)APPLE2_COLOR.BLUEBLACK]);
        ColorUtility.TryParseHtmlString("#401900", out Apple2ColorPalette[(int)APPLE2_COLOR.ORANGEBLACK]);

        ColorUtility.TryParseHtmlString("#FFFFFF", out Apple2ColorPalette[(int)APPLE2_COLOR.WHITE]);
    }

    public static Color Apple2ColorOdd(bool highBitSet, bool previousPixel, bool pixel, bool nextPixel)
    {
        Color color;

        if (!previousPixel && !pixel && !nextPixel) //000
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.BLACK];
        }
        else if (previousPixel && !pixel && !nextPixel) //100
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.BLACK];
        }
        else if (!previousPixel && pixel && !nextPixel) //010
        {
            if (highBitSet)
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.BLUE];
            }
            else
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.PURPLE];
            }
        }
        else if (previousPixel && pixel && !nextPixel) //110
        {
            if (highBitSet)
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.BLUEWHITE];
            }
            else
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.PURPLEWHITE];
            }
        }
        else if (!previousPixel && !pixel && nextPixel) //001
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.BLACK];
        }
        else if (previousPixel && !pixel && nextPixel) //101
        {
            if (highBitSet)
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.ORANGEBLACK];
            }
            else
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.GREENBLACK];
            }
        }
        else if (!previousPixel && pixel && nextPixel) //011
        {
            if (highBitSet)
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.BLUEWHITE];
            }
            else
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.PURPLEWHITE];
            }
        }
        else if (previousPixel && pixel && nextPixel) //111
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.WHITE];
        }
        else
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.BLACK];
        }

        return color;
    }

    public static Color Apple2ColorEven(bool highBitSet, bool previousPixel, bool pixel, bool nextPixel)
    {
        Color color;

        if (!previousPixel && !pixel && !nextPixel) //000
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.BLACK];
        }
        else if (previousPixel && !pixel && !nextPixel) //100
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.BLACK];
        }
        else if (!previousPixel && pixel && !nextPixel) //010
        {
            if (highBitSet)
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.ORANGE];
            }
            else
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.GREEN];
            }
        }
        else if (previousPixel && pixel && !nextPixel) //110
        {
            if (highBitSet)
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.ORANGEWHITE];
            }
            else
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.GREENWHITE];
            }
        }
        else if (!previousPixel && !pixel && nextPixel) //001
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.BLACK];
        }
        else if (previousPixel && !pixel && nextPixel) //101
        {
            if (highBitSet)
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.BLUEBLACK];
            }
            else
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.PURPLEBLACK];
            }
        }
        else if (!previousPixel && pixel && nextPixel) //011
        {
            if (highBitSet)
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.ORANGEWHITE];
            }
            else
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.GREENWHITE];
            }
        }
        else if (previousPixel && pixel && nextPixel)
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.WHITE];
        }
        else
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.BLACK];
        }

        return color;
    }

    public static Color Apple2ColorBW(bool highBitSet, bool previousPixel, bool pixel, bool nextPixel)
    {
        Color color;

        if (!pixel) //000
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.BLACK];
        }
        else //111
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.WHITE];
        }

        return color;
    }
}
