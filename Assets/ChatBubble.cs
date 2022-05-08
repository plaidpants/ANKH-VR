using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ChatBubble : MonoBehaviour
{
    public Text text;
    public SpriteRenderer background;
	public Texture2D fontAtlas;
	public Font myFont;

    public string charsetEGAFilepath = "/u4/CHARSET.EGA";
    public string charsetCGAFilepath = "/u4/CHARSET.CGA";

    public Color[] CGAColorPalette;
    public Color[] EGAColorPalette;

    void InitializeEGAPalette()
    {
        // create a EGA color palette
        EGAColorPalette = new Color[16];
        ColorUtility.TryParseHtmlString("#000000", out EGAColorPalette[0]);
        ColorUtility.TryParseHtmlString("#0000AA", out EGAColorPalette[1]);
        ColorUtility.TryParseHtmlString("#00AA00", out EGAColorPalette[2]);
        ColorUtility.TryParseHtmlString("#00AAAA", out EGAColorPalette[3]);
        ColorUtility.TryParseHtmlString("#AA0000", out EGAColorPalette[4]);
        ColorUtility.TryParseHtmlString("#AA00AA", out EGAColorPalette[5]);
        ColorUtility.TryParseHtmlString("#AA5500", out EGAColorPalette[6]);
        ColorUtility.TryParseHtmlString("#AAAAAA", out EGAColorPalette[7]);
        ColorUtility.TryParseHtmlString("#555555", out EGAColorPalette[8]);
        ColorUtility.TryParseHtmlString("#5555FF", out EGAColorPalette[9]);
        ColorUtility.TryParseHtmlString("#55FF55", out EGAColorPalette[10]);
        ColorUtility.TryParseHtmlString("#55FFFF", out EGAColorPalette[11]);
        ColorUtility.TryParseHtmlString("#FF5555", out EGAColorPalette[12]);
        ColorUtility.TryParseHtmlString("#FF55FF", out EGAColorPalette[13]);
        ColorUtility.TryParseHtmlString("#FFFF55", out EGAColorPalette[14]);
        ColorUtility.TryParseHtmlString("#FFFFFF", out EGAColorPalette[15]);
    }

    void InitializeCGAPalette()
    {
        // create CGA color palette
        CGAColorPalette = new Color[8];
        ColorUtility.TryParseHtmlString("#000000", out CGAColorPalette[0]);
        ColorUtility.TryParseHtmlString("#0000AA", out CGAColorPalette[1]);
        ColorUtility.TryParseHtmlString("#00AA00", out CGAColorPalette[2]);
        ColorUtility.TryParseHtmlString("#00AAAA", out CGAColorPalette[3]);
        ColorUtility.TryParseHtmlString("#AA0000", out CGAColorPalette[4]);
        ColorUtility.TryParseHtmlString("#AA00AA", out CGAColorPalette[5]);
        ColorUtility.TryParseHtmlString("#AA5500", out CGAColorPalette[6]);
        ColorUtility.TryParseHtmlString("#AAAAAA", out CGAColorPalette[7]);
    }

    void LoadCharSetEGA()
    {
        Color alpha = new Color(0, 0, 0, 0);

        if (!System.IO.File.Exists(Application.persistentDataPath + charsetEGAFilepath))
        {
            Debug.Log("Could not find EGA charset file " + Application.persistentDataPath + charsetEGAFilepath);
            return;
        }

        // read the file
        byte[] fileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + charsetEGAFilepath);

        if (fileData.Length != 8 * 1024)
        {
            Debug.Log("EGA charset file incorrect length " + fileData.Length);
            return;
        }

        // create a texture for this tile
        fontAtlas = new Texture2D(8*16, 8*8, TextureFormat.RGBA32, false);

        // we want pixles not fuzzy images
        fontAtlas.filterMode = FilterMode.Point;

        // use and index to walk through the file
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
                    Color color = EGAColorPalette[colorIndex];

                    // use black as alpha channel
                    if (colorIndex == 0)
                    {
                        fontAtlas.SetPixel((character % 16) * 8 + width++, (7 - (character / 16)) * 8 + 7 - height, alpha);
                    }
                    else
                    {
                        fontAtlas.SetPixel((character % 16) * 8 + width++, (7 - (character / 16)) * 8 + 7 - height, color);
                    }

                    // set the color of the second half of the nibble
                    colorIndex = fileData[fileIndex] & 0xf;
                    color = EGAColorPalette[colorIndex];
                    if (colorIndex == 0)
                    {
                        fontAtlas.SetPixel((character % 16) * 8 + width++, (7 - (character / 16)) * 8 + 7 - height, alpha);
                    }
                    else
                    { 
                        fontAtlas.SetPixel((character % 16) * 8 + width++, (7 - (character / 16)) * 8 + 7 - height, color);
                    }

                    // go to the next byte in the file
                    fileIndex++;
                }
            }
        }

        // Actually apply all previous SetPixel and SetPixels changes from above
        fontAtlas.Apply();
    }

#if GHTTD
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
                    Color color = EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index] & 0xC0) >> 6;
                    color = EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    colorIndex = (fileData[index + 0x20] & 0x30) >> 4;
                    color = EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index] & 0x30) >> 4;
                    color = EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    colorIndex = (fileData[index + 0x20] & 0x0C) >> 2;
                    color = EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index] & 0x0C) >> 2;
                    color = EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    colorIndex = (fileData[index + 0x20] & 0x03) >> 0;
                    color = EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index] & 0x03) >> 0;
                    color = EGAColorPalette[colorIndex];
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

    // Awake is called when the scipt instance is being loaded
    private void Awake()
    {
        InitializeCGAPalette();
        InitializeEGAPalette();
        LoadCharSetEGA();
        ImportFontFromTexture(fontAtlas);
	}

	// Start is called before the first frame update
	void Start()
    {
		//SetBubble("Hello world how are you");
	}

    void SetBubble(string bubbleText)
    {
        text.text = bubbleText;
        Rect textSize = text.GetPixelAdjustedRect();
        //Vector2 padding = new Vector2(4.0f, 4.0f);
        //background.size = new Vector2(textSize.width, textSize.width);// + padding;
    }

	public void ImportFontFromTexture(Texture2D texture)
	{
		float texW = texture.width;
		float texH = texture.height;

		CharacterInfo[] charInfos = new CharacterInfo[128];
		Rect r;

		for (int i = 0; i < charInfos.Length; i++)
		{
			CharacterInfo charInfo = new CharacterInfo();

			charInfo.index = i;
			charInfo.advance = 8;

			r = new Rect();
			r.x = (i % 16) * (8 / texW);
			r.y = (i / 16) * (8 / texH);
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

			charInfos[i] = charInfo;
		}

		// Create material
		Shader shader = Shader.Find("UI/Default");
		Material material = new Material(shader);
		material.mainTexture = texture;

		// Create font
		myFont = new Font();
		myFont.material = material;
		myFont.name = "font";
		myFont.characterInfo = charInfos;
		text.font = myFont;
	}

	// Update is called once per frame
	void Update()
    {
        /*
        // Align floating text perpendicular to Camera.
        if (!lastPOS.Compare(m_cameraTransform.position, 1000) || !lastRotation.Compare(m_cameraTransform.rotation, 1000))
        {
            lastPOS = m_cameraTransform.position;
            lastRotation = m_cameraTransform.rotation;
            m_floatingText_Transform.rotation = lastRotation;
            Vector3 dir = m_transform.position - lastPOS;
            m_transform.forward = new Vector3(dir.x, 0, dir.z);
        }
        */

        //Rect textSize = text.GetPixelAdjustedRect();
        //background.size = new Vector2(textSize.width, textSize.width);
    }
}
