//#define CREATE_DUMMY_FONT
using UnityEngine;

public class GameFont
{
    public static Texture2D fontAtlas;
    public static Texture2D fontTransparentAtlas;
    public static string charsetEGAFilepath = "/u4/CHARSET.EGA";
    public static string charsetCGAFilepath = "/u4/CHARSET.CGA";
    const int fontHeight = 10;
    const int fontWidth = 10;
    const int fontXOffset = 1;
    const int fontYOffset = 1;

    static void ClearTexture(Texture2D texture, Color color)
    {
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
    }

    static void ClearHalfTexture(Texture2D texture, Color color1, Color color2)
    {
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                if (y < texture.height / 2)
                {
                    texture.SetPixel(x, y, color1);
                }
                else
                {
                    texture.SetPixel(x, y, color2);
                }
            }
        }
        texture.Apply();
    }

    public static void LoadCharSetEGA()
    {
        Color alpha = new Color(0, 0, 0, 0);

        // check if the file exists
        if (!System.IO.File.Exists(Application.persistentDataPath + charsetEGAFilepath))
        {
            Debug.Log("Could not find EGA charset file " + Application.persistentDataPath + charsetEGAFilepath);
            return;
        }

        // read the file
        byte[] fileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + charsetEGAFilepath);

        // check if the file is the right size
        if (fileData.Length != 8 * 1024)
        {
            Debug.Log("EGA charset file incorrect length " + fileData.Length);
            return;
        }

        // create a texture for this font
        fontAtlas = new Texture2D(fontWidth * 16, fontHeight * 8 * 2, TextureFormat.RGBA32, false);

        // set half the texture to black, leave the other half white so the inverted chars don't have fringes
        ClearHalfTexture(fontAtlas, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.WHITE], Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BLACK]);

        // create a texture for the transparent version of this font
        fontTransparentAtlas = new Texture2D(fontWidth * 16, fontHeight * 8, TextureFormat.RGBA32, false);

        // clear the texture with a black alpha so anything we don't set pixels on is transparent
        ClearTexture(fontTransparentAtlas, alpha);

        // we want pixels not fuzzy images for these fonts
        fontAtlas.filterMode = FilterMode.Point;
        fontTransparentAtlas.filterMode = FilterMode.Point;

        // use an index to walk through the file
        int fileIndex = 0;

        // there are 128 characters in the file,
        // we will lay them out on a 16x8 texture atlas
        // so we can convert them into a font below
        for (int character = 0; character < 128; character++)
        {
            // manually go through the data and set the (x,y) pixels to the character based on the input file using the EGA color palette
            for (int height = 0; height < 8; height++)
            {
                for (int width = 0; width < 8; /* width incremented below because we need to do nibbles */ )
                {
                    // set the color of the first half of the nibble
                    int colorIndex = fileData[fileIndex] >> 4;
                    Color color = Palette.EGAColorPalette[colorIndex];

                    // use black as alpha channel
                    //if (colorIndex == 0)
                    //{
                    //    fontAtlas.SetPixel((character % 16) * fontWidth + fontXOffset + width++, (7 - (character / 16)) * fontHeight + fontYOffset + 7 - height + fontHeight * 8, alpha);
                    //}
                    //else
                    {
                        fontAtlas.SetPixel((character % 16) * fontWidth + fontXOffset + width++, (7 - (character / 16)) * fontHeight + fontYOffset + 7 - height + fontHeight * 8, color);
                    }

                    // set the color of the second half of the nibble
                    colorIndex = fileData[fileIndex] & 0xf;
                    color = Palette.EGAColorPalette[colorIndex];
                    //if (colorIndex == 0)
                    //{
                    //    fontAtlas.SetPixel((character % 16) * fontWidth + fontXOffset + width++, (7 - (character / 16)) * fontHeight + fontYOffset + 7 - height + fontHeight * 8, alpha);
                    //}
                    //else
                    {
                        fontAtlas.SetPixel((character % 16) * fontWidth + fontXOffset + width++, (7 - (character / 16)) * fontHeight + fontYOffset + 7 - height + fontHeight * 8, color);
                    }

                    // go to the next byte in the file
                    fileIndex++;
                }
            }
        }

        // need inverse characters also for highlighting
        // so we will add an additional 128 chars to the end and use them
        // dynamically by setting the high bit in the characters
        // in the strings we display

        // reset the file index to generate inverse chars
        fileIndex = 0;

        for (int character = 0; character < 128; character++)
        {
            // manually go through the data and set the (x,y) pixels to the character based on the input file using the EGA color palette
            for (int height = 0; height < 8; height++)
            {
                for (int width = 0; width < 8; /* width incremented below because we need to do nibbles */ )
                {
                    // set the color of the first half of the nibble
                    int colorIndex = fileData[fileIndex] >> 4;
                    Color color = Palette.EGAColorPalette[15 - colorIndex]; // flip the colors

                    // use black as alpha channel
                    //if (colorIndex == 0)
                    //{
                    //    fontAtlas.SetPixel((character % 16) * fontWidth + fontXOffset + width++, (7 - (character / 16)) * fontHeight + fontYOffset + 7 - height, alpha);
                    //}
                    //else
                    {
                        fontAtlas.SetPixel((character % 16) * fontWidth + fontXOffset + width++, (7 - (character / 16)) * fontHeight + fontYOffset + 7 - height, color);
                    }

                    // set the color of the second half of the nibble
                    colorIndex = fileData[fileIndex] & 0xf;
                    color = Palette.EGAColorPalette[15 - colorIndex];
                    //if (colorIndex == 0)
                    //{
                    //    fontAtlas.SetPixel((character % 16) * fontWidth + fontXOffset + width++, (7 - (character / 16)) * fontHeight + fontYOffset + 7 - height, alpha);
                    //}
                    //else
                    {
                        fontAtlas.SetPixel((character % 16) * fontWidth + fontXOffset + width++, (7 - (character / 16)) * fontHeight + fontYOffset + 7 - height, color);
                    }

                    // go to the next byte in the file
                    fileIndex++;
                }
            }
        }

        // Actually apply all previous SetPixel and SetPixels changes from above for the first font
        fontAtlas.Apply();

        // need another set of characters for buttons that are transparent where black
        // so button highlighting works correctly.
        // We will add these to a new font texture that we will use in a different font
        // since these will not be used dynamically like the inverse characters above.

        // reset the file index to generate transparent chars
        fileIndex = 0;

        for (int character = 0; character < 128; character++)
        {
            // manually go through the data and set the (x,y) pixels to the character based on the input file using the EGA color palette
            for (int height = 0; height < 8; height++)
            {
                for (int width = 0; width < 8; /* width incremented below because we need to do nibbles */ )
                {
                    // set the color of the first half of the nibble
                    int colorIndex = fileData[fileIndex] >> 4;
                    Color color = Palette.EGAColorPalette[colorIndex];

                    // use black as alpha channel
                    if (colorIndex == 0)
                    {
                        fontTransparentAtlas.SetPixel((character % 16) * fontWidth + fontXOffset + width++, (7 - (character / 16)) * fontHeight + fontYOffset + 7 - height, alpha);
                    }
                    else
                    {
                        fontTransparentAtlas.SetPixel((character % 16) * fontWidth + fontXOffset + width++, (7 - (character / 16)) * fontHeight + fontYOffset + 7 - height, color);
                    }

                    // set the color of the second half of the nibble
                    colorIndex = fileData[fileIndex] & 0xf;
                    color = Palette.EGAColorPalette[colorIndex];
                    if (colorIndex == 0)
                    {
                        fontTransparentAtlas.SetPixel((character % 16) * fontWidth + fontXOffset + width++, (7 - (character / 16)) * fontHeight + fontYOffset + 7 - height, alpha);
                    }
                    else
                    {
                        fontTransparentAtlas.SetPixel((character % 16) * fontWidth + fontXOffset + width++, (7 - (character / 16)) * fontHeight + fontYOffset + 7 - height, color);
                    }

                    // go to the next byte in the file
                    fileIndex++;
                }
            }
        }

        // Actually apply all previous SetPixel and SetPixels changes from above
        fontTransparentAtlas.Apply();
    }

#if DISABLED
// TODO use the CGA tileset loading code as a reference to load the CGA font
    void LoadCharsetCGA()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + charsetCGAFilepath))
        {
            Debug.Log("Could not find CGA tiles file " + Application.persistentDataPath + charsetCGAFilepath);
            return;
        }

        // read the file
        byte[] fileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + charsetCGAFilepath);

        if (fileData.Length != 16 * 1024)
        {
            Debug.Log("CGA Tiles file incorrect length " + fileData.Length);
            return;
        }

        // allocate an array of textures
        tiles = new Texture2D(16*8, 16, TextureFormat.RGBA32, false);

        // we want pixles not fuzzy images
        currentChacter.filterMode = FilterMode.Point;

        // use and index to walk through the file
        int index = 0;

        // there are 256 tiles in the file
        for (int tile = 0; tile < 256; tile++)
        {

            // assign this texture to the tile array index
            tiles[tile] = currentTile;

            // manually go through the data and set the (x,y) pixels to the tile based on the input file using the EGA color palette
            for (int height = 0; height < currentTile.height; height += 2)
            {
                for (int width = 0; width < currentTile.width; /* width incremented below */ )
                {
                    int colorIndex = (fileData[index + 0x20] & 0xC0) >> 6;
                    Color color = Palette.EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index] & 0xC0) >> 6;
                    color = Palette.EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    colorIndex = (fileData[index + 0x20] & 0x30) >> 4;
                    color = Palette.EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index] & 0x30) >> 4;
                    color = Palette.EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    colorIndex = (fileData[index + 0x20] & 0x0C) >> 2;
                    color = Palette.EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index] & 0x0C) >> 2;
                    color = Palette.EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    colorIndex = (fileData[index + 0x20] & 0x03) >> 0;
                    color = Palette.EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index] & 0x03) >> 0;
                    color = Palette.EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    // go to the next byte in the file
                    index++;
                }
            }

            // skip ahead
            index += 0x20;
        }

        // Actually apply all previous SetPixel and SetPixels changes from above
        currentTile.Apply();
    }
#endif

    public static void ImportFontFromTexture(Font myFont, Font myTransparentFont, Texture2D texture, Texture2D transparentTexture)
    {
        float texW = texture.width;
        float texH = texture.height;

        CharacterInfo[] charInfos = new CharacterInfo[256];
        Rect r;

        for (int i = 0; i < charInfos.Length; i++)
        {
            CharacterInfo charInfo = new CharacterInfo();

            charInfo.index = i;
            charInfo.advance = 8;

            r = new Rect();
            r.x = (i % 16) * (fontWidth / texW) + 1.0f / texW;
            r.y = (i / 16) * (fontHeight / texH) + 1.0f / texH;
            r.width = 8 / texW;
            r.height = 8 / texH;
            r.y = 1f - r.y - r.height;
            charInfo.uvBottomLeft = new Vector2(r.xMin, r.yMin);
            charInfo.uvBottomRight = new Vector2(r.xMax, r.yMin);
            charInfo.uvTopLeft = new Vector2(r.xMin, r.yMax);
            charInfo.uvTopRight = new Vector2(r.xMax, r.yMax);

            r = new Rect();
            r.x = 0;
            r.y = 0;
            r.width = 8;
            r.height = 8;
            r.y = -r.y;
            r.height = -r.height;
            charInfo.minX = (int)r.xMin;
            charInfo.maxX = (int)r.xMax;
            charInfo.minY = (int)r.yMax;
            charInfo.maxY = (int)r.yMin;
            //charInfo.size = 1000;
            charInfo.glyphHeight = 8;
            charInfo.glyphWidth = 8;
            //charInfo.maxX = 8;
            //charInfo.maxY = 8;
            charInfos[i] = charInfo;
        }

        texW = transparentTexture.width;
        texH = transparentTexture.height;

        CharacterInfo[] charInfosTransparent = new CharacterInfo[128];

        for (int i = 0; i < charInfosTransparent.Length; i++)
        {
            CharacterInfo charInfo = new CharacterInfo();

            charInfo.index = i;
            charInfo.advance = 8;

            r = new Rect();
            r.x = (i % 16) * (fontWidth / texW) + 1.0f / texW;
            r.y = (i / 16) * (fontHeight / texH) + 1.0f / texH;
            r.width = 8 / texW;
            r.height = 8 / texH;
            r.y = 1f - r.y - r.height;
            charInfo.uvBottomLeft = new Vector2(r.xMin, r.yMin);
            charInfo.uvBottomRight = new Vector2(r.xMax, r.yMin);
            charInfo.uvTopLeft = new Vector2(r.xMin, r.yMax);
            charInfo.uvTopRight = new Vector2(r.xMax, r.yMax);

            r = new Rect();
            r.x = 0;
            r.y = 0;
            r.width = 8;
            r.height = 8;
            r.y = -r.y;
            r.height = -r.height;
            charInfo.minX = (int)r.xMin;
            charInfo.maxX = (int)r.xMax;
            charInfo.minY = (int)r.yMax;
            charInfo.maxY = (int)r.yMin;
            //charInfo.size = 1000;
            charInfo.glyphHeight = 8;
            charInfo.glyphWidth = 8;
            //charInfo.maxX = 8;
            //charInfo.maxY = 8;
            charInfosTransparent[i] = charInfo;
        }

#if CREATE_DUMMY_FONT
        // WARNING: painful hack below
        // Font creation can only be done in the Unity Editor because SerializedObject
        // is not availible in the final release build so we don't have the ability
        // to set any of the Font() class properties thus making the the Font class kind of
        // useless for runtime font creation applications like this.
        // Need to create a dummy empty/blank font that is setup correctly in the Editor and then
        // replace the font texture atlas at runtime with the real one from the original game instead. 
        // Arrgh!

        // Create material
        Material material = new Material(Shader.Find("UI/Default"));

        // create an empty/blank texture
        material.mainTexture = new Texture2D(16*8, 16, TextureFormat.RGBA32, false);

        // create a new font
        myFont = new Font();
        myFont.material = material;
        myFont.name = "font";
        myFont.characterInfo = charInfos;

        // Set the magic properties we cannot be set in the release build
        // by adjusting font info so it spacing is correct. Begin Magic stuff.
        UnityEditor.SerializedObject mFont = new UnityEditor.SerializedObject(myFont);
        mFont.FindProperty("m_FontSize").floatValue = 10.0f;
        mFont.FindProperty("m_LineSpacing").floatValue = 8.0f;
        mFont.ApplyModifiedProperties();
        // End magic stuff

        // save the font to the unity assets folder, 
        // need to reference this asset in the public Font myFont variable
        // in the Unity Editor so we can use it in the release build below
        UnityEditor.AssetDatabase.CreateAsset (myFont, "Assets/font.fontsettings");
        System.IO.File.WriteAllBytes("Assets/u4font.png", texture.EncodeToPNG());
#else
        // Create font materials based on the font textures we created above
        Material material = new Material(Shader.Find("UI/Default"));
        material.mainTexture = texture;
        Material materialTransparent = new Material(Shader.Find("UI/Default"));
        materialTransparent.mainTexture = transparentTexture;

        // update font with original game textures,
        // everything else should already be set from when we created the asset file passed in above
        myFont.material = material;
        myTransparentFont.material = materialTransparent;

        myFont.characterInfo = charInfos;
        myTransparentFont.characterInfo = charInfosTransparent;
#endif
    }

    // The font is setup so if the high bit is set it will use the inverse highlighted text
    // this function will set the high bit on all the characters in a string so when displayed with the font
    // it will be highlighted
    public static string highlight(string s)
    {
        string temp = "";
        for (int j = 0; j < s.Length; j++)
        {
            char c = s[j];

            if (c == '\n')
            {
                temp += '\n';
            }
            else if (c == ' ')
            {
                temp += (char)(0x12 + 0x80);
            }
            else
            {
                temp += (char)(s[j] + 0x80);
            }
        }

        return temp;
    }
}
