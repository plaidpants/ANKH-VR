#if YETET

using System;
using System.Collections.Generic;
using UnityEngine;
using SharpFont;
using TMPro;

public class RuntimeFont : IDisposable
{
    readonly Texture2D fontTexture;

    readonly Material fontMaterial;

    public Font UnityFont { get; }

    public TMP_FontAsset TMProFont { get; }

    public Face Face { get; }

    public RuntimeFont(Face fontFace)
    {
        // Because Unity's Font API is rather limited, almost everything about
        // a font is read-only (except in the inspector!), despite the fact you're
        // supposed to be able to create custom bitmap fonts at runtime.
        //
        // The most annoying one is Font.lineHeight, because without being able to set this,
        // all custom fonts have non-functional newlines.
        //
        // The "solution"? A custom font is included directly in the project and instantiated.
        // This custom font has its lineHeight set to "1", but is otherwise completely empty.
        //
        // This means you can work around the newline issue by setting the lineSpacing property
        // on Text objects, as that property works as a factor of Font.lineHeight.

        Face = fontFace;
        fontTexture = new Texture2D(1, 1);
        fontMaterial = new Material(Shader.Find("TextMeshPro/Bitmap")) { mainTexture = fontTexture };
        UnityFont = UnityEngine.Object.Instantiate(Resources.Load<Font>("FontTemplate"));
        TMProFont = ScriptableObject.CreateInstance<TMP_FontAsset>();

        TMProFont.atlas = fontTexture;
        TMProFont.fontAssetType = TMP_FontAsset.FontAssetTypes.Bitmap;

        UnityFont.name = TMProFont.name = fontMaterial.name = fontTexture.name = Face.FamilyName;
        UnityFont.material = TMProFont.material = fontMaterial;

        Face.SetCharSize(0, 50, 72, 72);
        TMProFont.AddFaceInfo(new FaceInfo
        {
            Name = Face.FamilyName,
            PointSize = 50,
            LineHeight = Face.Size.Metrics.Ascender.ToSingle(),
            Ascender = Face.Size.Metrics.Ascender.ToSingle(),
            Descender = Face.Size.Metrics.Descender.ToSingle(),
        });

        TMProFont.AddGlyphInfo(new TMP_Glyph[0]);
        TMProFont.AddKerningInfo(new KerningTable());
        TMProFont.ReadFontDefinition();
    }

    public void RequestCharactersInTexture(string characters)
    {
        var codepoints = new HashSet<int>();

        foreach (CharacterInfo info in UnityFont.characterInfo)
            codepoints.Add(info.index);

        foreach (int codepoint in TMProFont.characterDictionary.Keys)
            codepoints.Add(codepoint);

        foreach (int codepoint in GetCodepoints(characters))
            codepoints.Add(codepoint);

        Face.SetCharSize(0, 50, 72, 72);

        // calculate a size for the font atlas
        // todo: this is really poor, improve this later
        int fontHeight = Face.Size.Metrics.Height.ToInt32();
        int maxSize = (int)(1 + fontHeight * Mathf.Ceil(Mathf.Sqrt(codepoints.Count)));

        // size must be a power of two
        int texSize = 1; for (; texSize < maxSize; texSize <<= 1) ;

        fontTexture.Resize(texSize, texSize);
        var pixels = new Color32[texSize * texSize];
        int atlasX = 0;
        int atlasY = 0;

        var charInfoIndex = 0;
        var unityCharInfo = new CharacterInfo[codepoints.Count];
        var tmproCharInfo = new TMP_Glyph[codepoints.Count];

        foreach (int codepoint in codepoints)
        {
            Face.LoadChar((uint)codepoint, LoadFlags.Default, LoadTarget.Normal);
            Face.Glyph.RenderGlyph(SharpFont.RenderMode.Normal);

            // todo: check Face.Glyph.Bitmap.PixelMode and handle all modes
            if (Face.Glyph.Bitmap.PixelMode != PixelMode.Gray) continue;

            if (atlasX + Face.Glyph.Bitmap.Width >= texSize)
            {
                atlasX = 0;
                atlasY += fontHeight + 1;
            }

            int rows = Face.Glyph.Bitmap.Rows;
            int cols = Face.Glyph.Bitmap.Width;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int x = atlasX + col;
                    int y = atlasY + rows - 1 - row;

                    var index = Face.Glyph.Bitmap.BufferData[row * Face.Glyph.Bitmap.Pitch + col];
                    var alpha = index / 255f;

                    pixels[y * texSize + x] = new Color32
                    {
                        r = (byte)(255 * alpha),
                        g = (byte)(255 * alpha),
                        b = (byte)(255 * alpha),
                        a = index
                    };
                }
            }

            unityCharInfo[charInfoIndex] = new CharacterInfo
            {
                index = codepoint,
                advance = Face.Glyph.Metrics.HorizontalAdvance.ToInt32(),
                bearing = Face.Glyph.Metrics.HorizontalBearingX.ToInt32(),
                glyphWidth = cols,
                glyphHeight = rows,
                minX = Face.Glyph.BitmapLeft,
                maxX = Face.Glyph.BitmapLeft + cols,
                minY = Face.Glyph.BitmapTop - rows,
                maxY = Face.Glyph.BitmapTop,
                uvBottomLeft = new Vector2(atlasX, atlasY) / texSize,
                uvBottomRight = new Vector2(atlasX + cols, atlasY) / texSize,
                uvTopLeft = new Vector2(atlasX, atlasY + rows) / texSize,
                uvTopRight = new Vector2(atlasX + cols, atlasY + rows) / texSize
            };

            tmproCharInfo[charInfoIndex] = new TMP_Glyph
            {
                id = codepoint,
                xAdvance = Face.Glyph.Metrics.HorizontalAdvance.ToSingle(),
                x = atlasX,
                y = atlasY,
                width = cols,
                height = rows,
                xOffset = Face.Glyph.BitmapLeft,
                yOffset = Face.Glyph.BitmapTop,
                scale = 1
            };

            atlasX += cols + 1;
            charInfoIndex++;
        }

        fontTexture.SetPixels32(pixels);
        fontTexture.Apply();

        UnityFont.characterInfo = unityCharInfo;
        TMProFont.fontInfo.AtlasWidth = texSize;
        TMProFont.fontInfo.AtlasHeight = texSize;
        TMProFont.AddGlyphInfo(tmproCharInfo);
        TMProFont.ReadFontDefinition();
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(UnityFont);
        UnityEngine.Object.Destroy(TMProFont);
        UnityEngine.Object.Destroy(fontTexture);
        UnityEngine.Object.Destroy(fontMaterial);
        Face.Dispose();
    }

    IEnumerable<int> GetCodepoints(string str)
    {
        var codepoints = new List<int>(str.Length);

        for (int i = 0; i < str.Length; i++)
        {
            int codepoint = Char.ConvertToUtf32(str, i);

            if (Char.IsHighSurrogate(str, i))
                i++;

            codepoints.Add(codepoint);
        }

        return codepoints;
    }
}
#endif