using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    // used for automatic klimb and decsend ladders
    public U4_Decompiled_AVATAR.TILE lastCurrentTile;

    public Texture2D[] originalTiles;
    public Texture2D[] expandedTiles;

    public Font myFont;
    public Font myTransparentFont;

    // mainTerrain holds the terrain, animatedTerrrain, billboardTerrrain
    public GameObject mainTerrain;
    public GameObject terrain;
    public GameObject animatedTerrrain;
    public GameObject billboardTerrrain;

    // these are fixed in space 
    public GameObject npcs;
    //public GameObject bubblePrefab;
    public GameObject party;
    public GameObject fighters;
    public GameObject characters;
    public GameObject activeCharacter;
    public GameObject hits;
    public GameObject moongate;
    public GameObject dungeon;

    public GameObject dungeonMonsters;
    public GameObject partyGameObject;
    public GameObject skyGameObject;

    //public GameObject[] Settlements;
    public GameObject[] CombatTerrains;
    public GameObject CenterOfCombatTerrain;

    public List<string> talkWordList = new List<string>();

    public string tileApple2Filepath1 = "/u4/SHP0.B"; 
    public string tileApple2Filepath2 = "/u4/SHP1.B";
    public string tileEGAFilepath = "/u4/SHAPES.EGA";
    public string tileCGAFilepath = "/u4/SHAPES.CGA";
    public string worldMapFilepath = "/u4/WORLD.MAP";

    public Text keyword1ButtonText;
    public Text keyword2ButtonText;
    public GameObject keyword1Button;
    public GameObject keyword2Button;
    public GameObject InputPanel;
    public GameObject StatsPanel;
    public GameObject TextPanel;
    public GameObject Talk;
    public GameObject Action;
    public GameObject ActionMainLoop;
    public GameObject ActionDungeonLoop;
    public GameObject ActionCombatLoop;
    public GameObject TalkCitizen;
    public GameObject TalkHealer;
    public GameObject TalkContinue;
    public Button TalkContinueButton;
    public GameObject TalkYN;
    public GameObject TalkYesNo;
    public GameObject TalkBuySell;
    public GameObject TalkFoodAle;
    public GameObject TalkHawWind;
    public GameObject TalkPartyCharacter;
    public GameObject TalkArmor;
    public GameObject TalkWeapon;
    public GameObject TalkGuild;
    public GameObject Talk2DigitInput;
    public GameObject Talk3DigitInput;
    public GameObject TalkLordBritish;

    public GameObject TalkMantras;
    public GameObject Talk1DigitInput;
    public GameObject TalkColors;
    public GameObject TalkUseItem;
    public GameObject TalkSpells;
    public GameObject TalkEnergy;
    public GameObject TalkReagents;
    public GameObject TalkTelescope;
    public GameObject TalkPhase;

    public GameObject TalkPubWord;
    public GameObject TalkDigit0123;
    public GameObject TalkVirtue;
    public GameObject TalkDirection;
    public GameObject TalkEndGame;


    // reference to game engine
    public U4_Decompiled_AVATAR u4;

    // this array size can be adjusted to display more or less of the map at runtime
    U4_Decompiled_AVATAR.TILE[,] raycastSettlementMap = new U4_Decompiled_AVATAR.TILE[64, 64];

    // this array size can be adjusted to display more or less of the map at runtime
    U4_Decompiled_AVATAR.TILE[,] raycastOutdoorMap = new U4_Decompiled_AVATAR.TILE[128, 128];
 //   U4_Decompiled.TILE[,] raycastOutdoorMap = new U4_Decompiled.TILE[256, 256];

   

    // unfortuantly the game engine never saves this information after loading the combat terrain in function C_7C65()
    // the code is not re-entrant so I cannot just expose and call the function directly so I need to re-implement the 
    // logic here so I can on the fly determine the combat terrain to display by exposing the interal variables used in the
    // original function. The INN or shop or camp case is handled elsewhere (e.g. In the middle of the night, while out for a stroll...)

    public U4_Decompiled_AVATAR.COMBAT_TERRAIN Convert_Tile_to_Combat_Terrian(U4_Decompiled_AVATAR.TILE tile)
    {
        U4_Decompiled_AVATAR.COMBAT_TERRAIN combat_terrain;


        if (u4.Party._tile <= U4_Decompiled_AVATAR.TILE.SHIP_SOUTH || (U4_Decompiled_AVATAR.TILE)((byte)u4.current_tile & ~3) == U4_Decompiled_AVATAR.TILE.SHIP_WEST)
        {
            if (u4.D_96F8 == U4_Decompiled_AVATAR.TILE.PIRATE_WEST)
            {
                combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.SHIPSHIP;
            }
            else if (u4.D_946C <= U4_Decompiled_AVATAR.TILE.SHALLOW_WATER)
            {
                combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.SHIPSEA;
            }
            else
            {
                combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.SHIPSHOR;
            }
        }
        else
        {
            if (u4.D_96F8 == U4_Decompiled_AVATAR.TILE.PIRATE_WEST)
            {
                combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.SHORSHIP;
            }
            else if (u4.D_946C <= U4_Decompiled_AVATAR.TILE.SHALLOW_WATER)
            {
                combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.SHORE;
            }
            else
            {
                switch (tile)
                {
                    case U4_Decompiled_AVATAR.TILE.SWAMP:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.MARSH;
                            break;
                        }
                    case U4_Decompiled_AVATAR.TILE.BRUSH:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.BRUSH;
                            break;
                        }
                    case U4_Decompiled_AVATAR.TILE.FOREST:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.FOREST;
                            break;
                        }
                    case U4_Decompiled_AVATAR.TILE.HILLS:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.HILL;
                            break;
                        }
                    case U4_Decompiled_AVATAR.TILE.DUNGEON:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.DUNGEON;
                            break;
                        }
                    case U4_Decompiled_AVATAR.TILE.BRICK_FLOOR:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.BRICK;
                            break;
                        }
                    case U4_Decompiled_AVATAR.TILE.BRIDGE:
                    case U4_Decompiled_AVATAR.TILE.BRIDGE_TOP:
                    case U4_Decompiled_AVATAR.TILE.BRIDGE_BOTTOM:
                    case U4_Decompiled_AVATAR.TILE.WOOD_FLOOR:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.BRIDGE;
                            break;
                        }
                    default:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.GRASS;
                            break;
                        }
                }
            }
        }

        return combat_terrain;
    }


    public Texture2D PNGAtlas;
    public string PNGFilepath;

    void LoadTilesPNG()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + PNGFilepath))
        {
            Debug.Log("Could not find PNG tiles atlas file " + Application.persistentDataPath + PNGFilepath);
            return;
        }

        // read the file
        byte[] fileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + PNGFilepath);

        // allocate something to start with so the loadImage can resize it to file the actual file
        PNGAtlas = new Texture2D(32, 32*256, TextureFormat.RGBA32, false);

        if (!PNGAtlas.LoadImage(fileData))
        {
            Debug.Log("Could not load PNG tiles atlas file " + Application.persistentDataPath + PNGFilepath);
            return;
        }

        originalTileWidth = PNGAtlas.width;
        originalTileHeight = PNGAtlas.height / 256;
        originalTiles = new Texture2D[256];

        // there are 256 tiles in the file
        for (int tile = 0; tile < 256; tile++)
        {
            // create a texture for this tile
            Texture2D currentTile = new Texture2D(originalTileWidth, originalTileHeight, TextureFormat.RGBA32, false);

            // we want pixles not fuzzy images
            currentTile.filterMode = FilterMode.Point;

            // assign this texture to the tile array index
            originalTiles[tile] = currentTile;

            int y = (255-tile) * originalTileHeight;

            currentTile.SetPixels(0, 0, originalTileWidth, originalTileHeight, PNGAtlas.GetPixels(0, y, originalTileWidth, originalTileHeight));
            currentTile.Apply();
        }
    }

    void LoadTilesApple2()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + tileApple2Filepath1))
        {
            Debug.Log("Could not find Apple2 tiles file " + Application.persistentDataPath + tileApple2Filepath1);
            return;
        }

        // read the file
        byte[] fileData1 = System.IO.File.ReadAllBytes(Application.persistentDataPath + tileApple2Filepath1);

        if (fileData1.Length != 4 * 1024)
        {
            Debug.Log("Apple2 Tiles file incorrect length " + fileData1.Length);
            return;
        }

        if (!System.IO.File.Exists(Application.persistentDataPath + tileApple2Filepath2))
        {
            Debug.Log("Could not find Apple2 tiles file " + Application.persistentDataPath + tileApple2Filepath2);
            return;
        }

        // read the file
        byte[] fileData2 = System.IO.File.ReadAllBytes(Application.persistentDataPath + tileApple2Filepath2);

        if (fileData2.Length != 4 * 1024)
        {
            Debug.Log("Apple2 Tiles file incorrect length " + fileData2.Length);
            return;
        }

        // allocate an array of textures
        originalTiles = new Texture2D[256];

        // there are 256 tiles in the file
        for (int tile = 0; tile < 256; tile++)
        {
            // create a texture for this tile
            Texture2D currentTile = new Texture2D(14, 16, TextureFormat.RGBA32, false);

            // we want pixles not fuzzy images
            currentTile.filterMode = FilterMode.Point;

            // assign this texture to the tile array index
            originalTiles[tile] = currentTile;

            // manually go through the data and set the (x,y) pixels to the tile based on the input file using the Apple2 color palette
            for (int height = 0; height < currentTile.height; height++)
            {
                for (int width = 0; width < currentTile.width - 2; /* width incremented below */ )
                {
                    bool highBitSet; 
                    bool previousPixel;
                    bool pixel;
                    bool nextPixel;
                    Color color;

                    int y = currentTile.height - height - 1;

                    // go to the next byte in the file
                    int index = height * 256 + tile;

                    // get the first file pixel block
                    int pixelBlock = fileData1[index];

                    // go through the first half of the tile from the first file
                    highBitSet = (pixelBlock & 0x80) != 0;

                    // assume no tiling on the side of the tile
                    previousPixel = false;
                    pixel = (pixelBlock & 0x01) != 0;
                    nextPixel = (pixelBlock & 0x02) != 0;
                    color = Palette.Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x04) != 0;
                    color = Palette.Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x08) != 0;
                    color = Palette.Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x10) != 0;
                    color = Palette.Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x20) != 0;
                    color = Palette.Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x40) != 0;
                    color = Palette.Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    // next pixel is in the other file
                    pixelBlock = fileData2[index];
                    nextPixel = (pixelBlock & 0x01) != 0;
                    color = Palette.Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);

                    // do the second half of the tile from the other file
                    highBitSet = (pixelBlock & 0x80) != 0;

                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x02) != 0;
                    color = Palette.Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x04) != 0;
                    color = Palette.Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x08) != 0;
                    color = Palette.Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x10) != 0;
                    color = Palette.Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x20) != 0;
                    color = Palette.Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x40) != 0;
                    color = Palette.Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    // assume no tiling on the side of the tile
                    nextPixel = false;
                    color = Palette.Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                }
            }

            // Actually apply all previous SetPixel and SetPixels changes from above
            currentTile.Apply();
        }
    }

    void LoadTilesEGA()
    {
        Color alpha = new Color(0, 0, 0, 0);

        if (!System.IO.File.Exists(Application.persistentDataPath + tileEGAFilepath))
        {
            Debug.Log("Could not find EGA tiles file " + Application.persistentDataPath + tileEGAFilepath);
            return;
        }

        // read the file
        byte[] fileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + tileEGAFilepath);

        if (fileData.Length != 32 * 1024)
        {
            Debug.Log("EGA tiles file incorrect length " + fileData.Length);
            return;
        }

        // allocate an array of textures
        originalTiles = new Texture2D[256];

        // use and index to walk through the file
        int index = 0;

        // there are 256 tiles in the file
        for (int tile = 0; tile < 256; tile++)
        {
            // create a texture for this tile
            Texture2D currentTile = new Texture2D(16, 16, TextureFormat.RGBA32, false);

            // we want pixles not fuzzy images
            currentTile.filterMode = FilterMode.Point;

            // assign this texture to the tile array index
            originalTiles[tile] = currentTile;

            // manually go through the data and set the (x,y) pixels to the tile based on the input file using the EGA color palette
            for (int height = 0; height < currentTile.height; height++)
            {
                for (int width = 0; width < currentTile.width; /* width incremented below */ )
                {
                    // set the color of the first half of the nibble
                    int colorIndex = fileData[index] >> 4;
                    Color color = Palette.EGAColorPalette[colorIndex];

                    // check if these are people/creatures/ladders/anhk and use black as alpha channel 61
                    if ((colorIndex == (int)Palette.EGA_COLOR.BLACK) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.ANKH ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.LADDER_UP ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.LADDER_DOWN ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.FOREST ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.COOKING_FIRE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.SHRINE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.ALTAR ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BALOON ||
                        //tile == (int)U4_Decompiled.TILE.CHEST ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.CASTLE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.CASTLE_LEFT ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.CASTLE_ENTRANCE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.CASTLE_RIGHT ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.VILLAGE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BRIDGE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BRIDGE_BOTTOM ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BRIDGE_TOP ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BRUSH ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.NIXIE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.NIXIE2 ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.MISSLE_ATTACK_SMALL && tile <= (int)U4_Decompiled_AVATAR.TILE.MISSLE_ATTACK_RED) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.PARTY && tile <= (int)U4_Decompiled_AVATAR.TILE.SHEPHERD2) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.GUARD && tile <= (int)U4_Decompiled_AVATAR.TILE.LORD_BRITISH2) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.SERPENT && tile <= (int)U4_Decompiled_AVATAR.TILE.WATER_SPOUT2) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.BAT && tile <= (int)U4_Decompiled_AVATAR.TILE.TROLL4) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.INSECTS && tile <= (int)U4_Decompiled_AVATAR.TILE.INSECTS4) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.PHANTOM && tile <= (int)U4_Decompiled_AVATAR.TILE.MAGE_NPC4) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.LAVA_LIZARD && tile <= (int)U4_Decompiled_AVATAR.TILE.ZORN4) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.HYDRA && tile <= (int)U4_Decompiled_AVATAR.TILE.BALRON4)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // remove the brown line that overlaps the bridge support
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BROWN) && tile == (int)U4_Decompiled_AVATAR.TILE.BRIDGE_BOTTOM && height == 9)
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // special case the horse black eyes
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BLACK) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.HORSE_EAST))
                    {
                        if ((width == 13) && (height == 4))
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                        else
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                    }
                    // special case the horse black eyes
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BLACK) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.HORSE_WEST))
                    {
                        if ((width == 3) && (height == 5))
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                        else
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                    }
                    // others where we need to make green an alpha channel also like towns/ruins/villages
                    // so the grass speckels don't show when we use the tile as a billboard
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BLACK) || (colorIndex == (int)Palette.EGA_COLOR.GREEN)) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.VILLAGE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.TOWN ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.RUINS))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // remove green grass specals from moongates 
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BLACK) || (colorIndex == (int)Palette.EGA_COLOR.GREEN)) &&
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.MOONGATE1 && tile <= (int)U4_Decompiled_AVATAR.TILE.MOONGATE4))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // make the moongates blue and yellow transparent
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BRIGHT_YELLOW) || (colorIndex == (int)Palette.EGA_COLOR.BRIGHT_BLUE)) &&
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.MOONGATE1 && tile <= (int)U4_Decompiled_AVATAR.TILE.MOONGATE4))
                    {
                        color.a = 0.75f;
                        currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                    }
                    // remove blue water from these squid tiles and make black and blue into alpha
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BLACK) || (colorIndex == (int)Palette.EGA_COLOR.BRIGHT_BLUE)) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.SQUID || tile == (int)U4_Decompiled_AVATAR.TILE.SQUID2))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // ships
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BLACK) &&
                        ((tile >= (int)U4_Decompiled_AVATAR.TILE.SHIP_WEST && tile <= (int)U4_Decompiled_AVATAR.TILE.SHIP_SOUTH) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.PIRATE_WEST && tile <= (int)U4_Decompiled_AVATAR.TILE.PIRATE_SOUTH)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // make energy fields are transparent
                    else if (tile >= (int)U4_Decompiled_AVATAR.TILE.POISON_FIELD && tile <= (int)U4_Decompiled_AVATAR.TILE.SLEEP_FIELD)
                    {
                        if (colorIndex == (int)Palette.EGA_COLOR.BLACK)
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                        else
                        {
                            color.a = 0.75f;
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                    }
                    // everything else just copy all the pixels with no modifications for now
                    else
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                    }

                    // set the color of the second half of the nibble
                    colorIndex = fileData[index] & 0xf;
                    color = Palette.EGAColorPalette[colorIndex];

                    // check if these are people/creatures and use black as alpha channel
                    // check if these are people/creatures/ladders/anhk and use black as alpha channel
                    if ((colorIndex == (int)Palette.EGA_COLOR.BLACK) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.ANKH ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.LADDER_UP ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.LADDER_DOWN ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.FOREST ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.COOKING_FIRE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.SHRINE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.ALTAR ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BALOON ||
                        //tile == (int)U4_Decompiled.TILE.CHEST ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.CASTLE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.CASTLE_LEFT ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.CASTLE_ENTRANCE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.CASTLE_RIGHT ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.VILLAGE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BRIDGE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BRIDGE_BOTTOM ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BRIDGE_TOP ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.BRUSH ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.NIXIE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.NIXIE2 ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.MISSLE_ATTACK_SMALL && tile <= (int)U4_Decompiled_AVATAR.TILE.MISSLE_ATTACK_RED) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.PARTY && tile <= (int)U4_Decompiled_AVATAR.TILE.SHEPHERD2) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.GUARD && tile <= (int)U4_Decompiled_AVATAR.TILE.LORD_BRITISH2) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.SERPENT && tile <= (int)U4_Decompiled_AVATAR.TILE.WATER_SPOUT2) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.BAT && tile <= (int)U4_Decompiled_AVATAR.TILE.TROLL4) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.INSECTS && tile <= (int)U4_Decompiled_AVATAR.TILE.INSECTS4) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.PHANTOM && tile <= (int)U4_Decompiled_AVATAR.TILE.MAGE_NPC4) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.LAVA_LIZARD && tile <= (int)U4_Decompiled_AVATAR.TILE.ZORN4) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.HYDRA && tile <= (int)U4_Decompiled_AVATAR.TILE.BALRON4)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // remove the brown line that overlaps the bridge support
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BROWN) && tile == (int)U4_Decompiled_AVATAR.TILE.BRIDGE_BOTTOM && height == 9)
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // others where we need to make green an alpha channel also like towns/ruins/villages
                    // so the green grass speckels don't show when we use the tile standing upright
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BLACK) || (colorIndex == (int)Palette.EGA_COLOR.GREEN)) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.VILLAGE ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.TOWN ||
                        tile == (int)U4_Decompiled_AVATAR.TILE.RUINS))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // remove green grass specals from moongates 
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BLACK) || (colorIndex == (int)Palette.EGA_COLOR.GREEN)) &&
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.MOONGATE1 && tile <= (int)U4_Decompiled_AVATAR.TILE.MOONGATE4))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // make the moongates blue and yellow transparent TODO make black inside portal transparent also
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BRIGHT_YELLOW) || (colorIndex == (int)Palette.EGA_COLOR.BRIGHT_BLUE)) &&
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.MOONGATE1 && tile <= (int)U4_Decompiled_AVATAR.TILE.MOONGATE4))
                    {
                        color.a = 0.75f;
                        currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                    }
                    // remove blue water from these tiles and make black and blue into alpha
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BLACK) || (colorIndex == (int)Palette.EGA_COLOR.BRIGHT_BLUE)) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.SQUID || tile == (int)U4_Decompiled_AVATAR.TILE.SQUID2))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // special case the horse black eyes
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BLACK) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.HORSE_EAST))
                    {
                        if ((width == 13) && (height == 4))
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                        else
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                    }
                    // special case the horse black eyes
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BLACK) &&
                        (tile == (int)U4_Decompiled_AVATAR.TILE.HORSE_WEST))
                    {
                        if ((width == 3) && (height == 5))
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                        else
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                    }
                    // ships
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BLACK) &&
                        ((tile >= (int)U4_Decompiled_AVATAR.TILE.SHIP_WEST && tile <= (int)U4_Decompiled_AVATAR.TILE.SHIP_SOUTH) ||
                        (tile >= (int)U4_Decompiled_AVATAR.TILE.PIRATE_WEST && tile <= (int)U4_Decompiled_AVATAR.TILE.PIRATE_SOUTH)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // energy fields are transparent
                    else if (tile >= (int)U4_Decompiled_AVATAR.TILE.POISON_FIELD && tile <= (int)U4_Decompiled_AVATAR.TILE.SLEEP_FIELD)
                    {
                        if (colorIndex == (int)Palette.EGA_COLOR.BLACK)
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                        else
                        {
                            color.a = 0.75f;
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                    }
                    // everything else has no alpha channel defined yet
                    else
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                    }

                    // go to the next byte in the file
                    index++;
                }
            }

            // Actually apply all previous SetPixel and SetPixels changes from above
            currentTile.Apply();
        }
    }

    void LoadTilesCGA()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + tileCGAFilepath))
        {
            Debug.Log("Could not find CGA tiles file " + Application.persistentDataPath + tileCGAFilepath);
            return;
        }

        // read the file
        byte[] fileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + tileCGAFilepath);

        if (fileData.Length != 16 * 1024)
        {
            Debug.Log("CGA Tiles file incorrect length " + fileData.Length);
            return;
        }

        // allocate an array of textures
        originalTiles = new Texture2D[256];

        // use and index to walk through the file
        int index = 0;

        // there are 256 tiles in the file
        for (int tile = 0; tile < 256; tile++)
        {
            // create a texture for this tile
            Texture2D currentTile = new Texture2D(16, 16, TextureFormat.RGBA32, false);

            // we want pixles not fuzzy images
            currentTile.filterMode = FilterMode.Point;

            // assign this texture to the tile array index
            originalTiles[tile] = currentTile;

            // manually go through the data and set the (x,y) pixels to the tile based on the input file using the EGA color palette
            for (int height = 0; height < currentTile.height; height += 2)
            {
                for (int width = 0; width < currentTile.width; /* width incremented below */ )
                {
                    int colorIndex;

                    colorIndex = (fileData[index + 0x20] & 0xC0) >> 6;
                    Color color = Palette.CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index + 0x00] & 0xC0) >> 6;
                    color = Palette.CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    colorIndex = (fileData[index + 0x20] & 0x30) >> 4;
                    color = Palette.CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index + 0x00] & 0x30) >> 4;
                    color = Palette.CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    colorIndex = (fileData[index + 0x20] & 0x0C) >> 2;
                    color = Palette.CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index + 0x00] & 0x0C) >> 2;
                    color = Palette.CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    colorIndex = (fileData[index + 0x20] & 0x03) >> 0;
                    color = Palette.CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index + 0x00] & 0x03) >> 0;
                    color = Palette.CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    // go to the next byte in the file
                    index++;
                }
            }

            // Actually apply all previous SetPixel and SetPixels changes from above
            currentTile.Apply();

            // skip ahead
            index += 0x20;
        }
    }

    public struct CombatMonsterStartPositions
    {
        public int start_x;
        public int start_y;
    }

    public struct CombatPartyStartPositions
    {
        public int start_x;
        public int start_y;
    }
    void LoadCombatMap(string combatMapFilepath, 
        ref U4_Decompiled_AVATAR.TILE[,] combatMap, 
        ref CombatMonsterStartPositions [] monsterStartPositions, 
        ref CombatPartyStartPositions [] partyStartPositions)
    {
        /*
        These files contain the 11x11 battleground maps shown when combat starts. It has the map itself plus starting positions for up to 16 monsters and 8 party members.
        Offset 	Length (in bytes) 	Purpose
        0x0 	16 	start_x for monsters 0-15
        0x10 	16 	start_y for monsters 0-15
        0x20 	8 	start_x for party members 0-7
        0x28 	8 	start_y for party members 0-7
        0x30 	16 	Purpose unknown; seems to be a constant: 08 AD 83 C0 AD 83 C0 AD 83 C0 A0 00 B9 A6 08 F0
        0x40 	121 	11x11 Map Matrix
        0xB9 	7 	Purpose unknown; seems to be a constant: 8D 00 00 00 00 47 09 
        */

        if (!System.IO.File.Exists(Application.persistentDataPath + combatMapFilepath))
        {
            Debug.Log("Could not find combat map file " + Application.persistentDataPath + combatMapFilepath);
            return;
        }

        // read the file
        byte[] combatMapFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + combatMapFilepath);

        if (combatMapFileData.Length != 0xc0)
        {
            Debug.Log("Combat map file incorrect length " + combatMapFileData.Length);
            return;
        }

        int fileIndex = 0;

        // the shrine does not have any start positions and omits those and goes right to the map,
        // the file size appear to be the same though
        if (combatMapFilepath != "/u4/SHRINE.CON")
        {
            for (int i = 0; i < 16; i++)
            {
                monsterStartPositions[i].start_x = combatMapFileData[fileIndex++];
            }

            for (int i = 0; i < 16; i++)
            {
                monsterStartPositions[i].start_y = combatMapFileData[fileIndex++];
            }

            for (int i = 0; i < 8; i++)
            {
                partyStartPositions[i].start_x = combatMapFileData[fileIndex++];
            }

            for (int i = 0; i < 8; i++)
            {
                partyStartPositions[i].start_y = combatMapFileData[fileIndex++];
            }

            // skip over
            fileIndex += 16;
        }

        // read in the tile map
        for (int y = 0; y < 11; y++)
        {
            for (int x = 0; x < 11; x++)
            {
                combatMap[x, y] = (U4_Decompiled_AVATAR.TILE)combatMapFileData[fileIndex++];
            }
        }
    }

    void CreateCombatTerrains()
    { 
        // create a game object to store the combat terrain game objects, this should be at the top with no parent same as the world
        GameObject combatTerrainsObject = new GameObject();
        combatTerrainsObject.name = "Combat Terrains";

        CombatTerrains = new GameObject[(int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX];

        // go through all the combat terrains and load their maps and create a game object to hold them
        // as a child of the above combat terrains game object
        for (int i = 0; i<(int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX; i++)
        {
            // allocate space for the individual map
            combatMaps[i] = new U4_Decompiled_AVATAR.TILE[11, 11];
            // allocate space for the monster and party starting positions
            combatMonsterStartPositions[i] = new CombatMonsterStartPositions[16];
            combatPartyStartPositions[i] = new CombatPartyStartPositions[8];

            if (i == (int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.CAMP_DNG)
            {
                // this one has a different name format
                LoadCombatMap("/u4/CAMP.DNG",
                    ref combatMaps[i],
                    ref combatMonsterStartPositions[i],
                    ref combatPartyStartPositions[i]);
            }
            else
            {
                // load the combat map from the original files
                LoadCombatMap("/u4/" + ((U4_Decompiled_AVATAR.COMBAT_TERRAIN)i).ToString() + ".CON",
                    ref combatMaps[i],
                    ref combatMonsterStartPositions[i],
                    ref combatPartyStartPositions[i]);
            }

            // create a game object to hold it and set it as a child of the combat terrains game object
            GameObject gameObject = new GameObject();
            gameObject.transform.SetParent(combatTerrainsObject.transform);

            // set it's name to match the combat terrain being created
            gameObject.name = ((U4_Decompiled_AVATAR.COMBAT_TERRAIN) i).ToString();

            // create the combat terrain based on the loaded map
            CreateMap(gameObject, combatMaps[i]);

            // Disable it initially
            gameObject.SetActive(false);

            // Position the combat map in place
            gameObject.transform.position = new Vector3(0, 0, entireMapTILEs.GetLength(1) - combatMaps[i].GetLength(1)); ;

            // rotate map into place
            gameObject.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

            // save the game object in the array
            CombatTerrains[i] = gameObject;
        }

        CenterOfCombatTerrain = new GameObject();
        CenterOfCombatTerrain.transform.position = new Vector3(5f, 0, 250f);
        CenterOfCombatTerrain.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        CenterOfCombatTerrain.transform.SetParent(combatTerrainsObject.transform);
    }

    [SerializeField]
    U4_Decompiled_AVATAR.TILE[,] entireMapTILEs = new U4_Decompiled_AVATAR.TILE[32 * 8, 32 * 8];

    [SerializeField]
    GameObject[,] entireMapGameObjects = new GameObject[32 * 8, 32 * 8];
    GameObject[][,] settlementsMapGameObjects = new GameObject[(int)SETTLEMENT.MAX][,];

    void LoadWorldMap()
    {
        /*
        This is the map of Britannia. It is 256x256 tiles in total and broken up into 64 32x32 chunks; 
        the total file is 65,536 bytes long. The first chunk is in the top left corner; 
        the next is just to the right of it, and so on. The last chunk is in the bottom right corner. 
        Each tile is stored as a byte that maps to a tile in SHAPES.EGA.The chunks are stored in the same way as the overall map: 
        left to right and top to bottom.

        The "chunked" layout is an artifact of the limited memory on the original machines that ran Ultima IV. 
        The whole map would take 64kb, too much for a C64 or an Apple II, so the game would keep a limited number of 1k chunks in memory 
        at a time.As the player moved around, old chunks were thrown out as new ones were swapped in.
        Offset  Length(in bytes)   Notes
        0x0     1024    32x32 map matrix for chunk 0
        0x400   1024    32x32 map matrix for chunk 1... 	... 	...
        0xFC00  1024    32x32 map matrix for chunk 63
        */

        if (!System.IO.File.Exists(Application.persistentDataPath + worldMapFilepath))
        {
            Debug.Log("Could not find world map file " + Application.persistentDataPath + worldMapFilepath);
            return;
        }

        // read the file
        byte[] worldMapFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + worldMapFilepath);

        if (worldMapFileData.Length != 32 * 32 * 64)
        {
            Debug.Log("World map file incorrect length " + worldMapFileData.Length);
            return;
        }

        int fileIndex = 0;

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                for (int height = 0; height < 32; height++)
                {
                    for (int width = 0; width < 32; width++)
                    {
                        entireMapTILEs[x * 32 + width, y * 32 + height] = (U4_Decompiled_AVATAR.TILE)worldMapFileData[fileIndex++];
                    }
                }
            }
        }
    }

    // These are different than the map tiles
    public enum DUNGEON_TILE
    {
        HALLWAY = 0x00,
        LADDER_UP = 0x10, // 	Ladder Up
        LADDER_DOWN = 0x20, // 	Ladder Down
        LADDER_UP_AND_DOWN = 0x30, // 	Laddr Up & Down
        TREASURE_CHEST = 0x40, // 	Treasure Chest
        CEILING_HOLE = 0x50, //	Ceiling Hole
        FLOOR_HOLE = 0x60, // 	Floor Hole (unused)
        MAGIC_ORB = 0x70, // 	Magic Orb
        TRAP = 0x80,
        TRAP_WIND_DARKNESS = 0x80, // 	Winds/Darknes Trap
        TRAP_FALLING_ROCKS = 0x81, // 	Falling Rock Trap
        TRAP_PIT = 0x8E, //	Pit Trap
        FOUNTAIN = 0x90, // 	Plain Fountain
        FOUNTAIN_HEALING = 0x91, // 	Healing Fountain
        FOUNTAIN_ACID = 0x92, // 	Acid Fountain
        FOUNTAIN_CURE = 0x93, // 	Cure Fountain
        FOUNTAIN_POISIN = 0x94, // 	Poison Fountain
        FIELD = 0xA0,
        FIELD_POISON = 0xA0, // Poison Field
        FIELD_ENERGY = 0xA1, //  Energy Field
        FIELD_FIRE = 0xA2, // Fire Field
        FIELD_SLEEP = 0xA3, //  Sleep Field
        ALTAR = 0xB0, // Altar
        DOOR = 0xC0, // Door
        DUNGEON_ROOM = 0xD0,
        DUNGEON_ROOM_0 = 0xD0, // D0-DF Dungeon Rooms 0-15
        DUNGEON_ROOM_1 = 0xD1,
        DUNGEON_ROOM_2 = 0xD2,
        DUNGEON_ROOM_3 = 0xD3,
        DUNGEON_ROOM_4 = 0xD4,
        DUNGEON_ROOM_5 = 0xD5,
        DUNGEON_ROOM_6 = 0xD6,
        DUNGEON_ROOM_7 = 0xD7,
        DUNGEON_ROOM_8 = 0xD8,
        DUNGEON_ROOM_9 = 0xD9,
        DUNGEON_ROOM_10 = 0xDA,
        DUNGEON_ROOM_11 = 0xDB,
        DUNGEON_ROOM_12 = 0xDC,
        DUNGEON_ROOM_13 = 0xDD,
        DUNGEON_ROOM_14 = 0xDE,
        DUNGEON_ROOM_15 = 0xDF,
        DOOR_SECRECT = 0xE0, // Secret Door
        WALL = 0xF0 //  Wall
    }

    public enum DUNGEONS
    {
        DECEIT = 0,
        DESPISE = 1,
        DESTARD = 2,
        WRONG = 3,
        COVETOUS = 4,
        SHAME = 5,
        HYTHLOTH = 6,
        ABYSS = 7,
        MAX = 8
    };

    [SerializeField]
    public struct FLOOR_TRIGGER
    {
        public U4_Decompiled_AVATAR.TILE changeTile;
        public int trigger_x, trigger_y;
        public int changeTile_x1, changeTile_y1;
        public int changeTile_x2, changeTile_y2;
    }

    [SerializeField]
    public struct DUNGEON_MONSTER
    {
        public U4_Decompiled_AVATAR.TILE monster;
        public int x, y;
    }

    [SerializeField]
    public struct DUNGEON_PARTY_START_LOCATION
    {
        public int x, y;
    }

    [SerializeField]
    public struct DUNGEON_ROOM
    {
        public FLOOR_TRIGGER[] triggers; // (4 bytes each X 4 triggers possible)
        public DUNGEON_MONSTER[] monsters; // 16 of them, (0 means no monster and 0's come FIRST)
        public DUNGEON_PARTY_START_LOCATION[] partyNorthEntry; // 0-7 (north entry)
        public DUNGEON_PARTY_START_LOCATION[] partyEastEntry; // 0-7 (east entry)
        public DUNGEON_PARTY_START_LOCATION[] partySouthEntry; // 0-7 (south entry)
        public DUNGEON_PARTY_START_LOCATION[] partyWestEntry; // 0-7 (west entry)
        public U4_Decompiled_AVATAR.TILE[,] dungeonRoomMap; // 11x11 map matrix for room
    }

    [SerializeField]
    public struct DUNGEON
    {
        public string name;
        public DUNGEON_TILE[][,] dungeonTILEs; // 8x8x8 map
        public DUNGEON_ROOM[] dungeonRooms; // 16 or 64 rooms
    }

    // this is needed to determine which combat map to use for which dungeon tile
    /*
        DNG0 = 16, // hallway
        DNG1 = 17, // ladder up
        DNG2 = 18, // ladder down
        DNG3 = 19, // ladder up and down
        DNG4 = 20, // chest
        DNG5 = 21, // doorway
        DNG6 = 22, // secret doorway
    */
    /*
        HALLWAY = 0x00,
        LADDER_UP = 0x10, 
        LADDER_DOWN = 0x20, 
        LADDER_UP_AND_DOWN = 0x30, 
        TREASURE_CHEST = 0x40, 
        CEILING_HOLE = 0x50,
        FLOOR_HOLE = 0x60,
        MAGIC_ORB = 0x70,
        TRAP = 0x80,
        FOUNTAIN = 0x90,
        FIELD = 0xa0,
        ALTAR = 0xB0, 
        DOOR = 0xC0, 
        DUNGEON_ROOM = 0xd0,
        DOOR_SECRECT = 0xE0, 
        WALL = 0xF0
    */
    // only the upper nibble defines the dungeon tile, the lower nibble is used for active dungeon monsters
    public U4_Decompiled_AVATAR.COMBAT_TERRAIN[] convertDungeonTileToCombat  =
    { 
        U4_Decompiled_AVATAR.COMBAT_TERRAIN.DNG0, // HALLWAY -> hallway
        U4_Decompiled_AVATAR.COMBAT_TERRAIN.DNG1, // LADDER_UP -> ladder up
        U4_Decompiled_AVATAR.COMBAT_TERRAIN.DNG2, // LADDER_DOWN -> ladder down
        U4_Decompiled_AVATAR.COMBAT_TERRAIN.DNG3, // LADDER_UP_AND_DOWN -> ladder up and down
        U4_Decompiled_AVATAR.COMBAT_TERRAIN.DNG4, // TREASURE_CHEST -> chest
        U4_Decompiled_AVATAR.COMBAT_TERRAIN.DNG0, // CEILING_HOLE -> hallway
        U4_Decompiled_AVATAR.COMBAT_TERRAIN.DNG0, // FLOOR_HOLE -> hallway
        U4_Decompiled_AVATAR.COMBAT_TERRAIN.DNG0, // MAGIC_ORB -> hallway
        U4_Decompiled_AVATAR.COMBAT_TERRAIN.DNG0, // TRAP -> hallway
        U4_Decompiled_AVATAR.COMBAT_TERRAIN.DNG0, // FOUNTAIN -> hallway
        U4_Decompiled_AVATAR.COMBAT_TERRAIN.DNG0, // FIELD -> hallway
        U4_Decompiled_AVATAR.COMBAT_TERRAIN.DNG0, // ALTAR -> hallway
        U4_Decompiled_AVATAR.COMBAT_TERRAIN.DNG5, // DOOR -> doorway 
        U4_Decompiled_AVATAR.COMBAT_TERRAIN.DNG0, // DUNGEON_ROOM -> hallway
        U4_Decompiled_AVATAR.COMBAT_TERRAIN.DNG0, // DOOR_SECRECT -> secret doorway  /* DNG6 make secret door just regular hallway as we do the secret door/wall on the ajacent room/hallway */
        U4_Decompiled_AVATAR.COMBAT_TERRAIN.DNG0  // WALL -> hallway, just in case
    };

    public enum NPC_MOVEMENT_MODE
    {
        FIXED = 0x00,
        WANDER = 0x01,
        FOLLOW = 0x80,
        ATTACK = 0xff
    };

    public enum NPC_STRING_INDEX
    {
        NAME = 0,
        PRONOUN = 1, //  (He, She or It)
        LOOK_DESCRIPTION = 2,
        JOB_RESPONSE = 3,
        HEALTH_RESPONSE = 4,
        KEYWORD1_RESPONSE = 5,
        KEYWORD2_RESPONSE = 6,
        QUESTION = 7,
        QUESTION_YES_RESPONSE = 8,
        QUESTION_NO_RESPONSE = 9,
        KEYWORD1 = 10,
        KEYWORD2 = 11,
        MAX = 12
    };

    // these need to line up with U4_Decompiled.LOCATIONS so we can convert from Party._loc to this enum and the LOCATIONS enum, LBC_1 is a special case
    public enum SETTLEMENT
    {
        // Castles
        LCB_1 = 0,
        LCB_2 = 1,
        LYCAEUM = 2,
        EMPATH = 3,
        SERPENT = 4,

        // Townes
        MOONGLOW = 5,
        BRITAIN = 6,
        JHELOM = 7,
        YEW = 8,
        MINOC = 9,
        TRINSIC = 10,
        SKARA = 11,
        MAGINCIA = 12,

        // Villages
        PAWS = 13,
        DEN = 14,
        VESPER = 15,
        COVE = 16,
        MAX = 17
    }

    public struct npc
    {
        public U4_Decompiled_AVATAR.TILE tile;
        public byte pos_x;
        public byte pos_y;
        public NPC_MOVEMENT_MODE movement;
        public int conversationIndex;
        public List<string> strings;
        public int probabilityOfTurningAway;
        public bool questionAffectHumility;
        public int questionTriggerIndex;
    };

    public npc[][] settlementNPCs = new npc[(int)SETTLEMENT.MAX][]; //32
    public int[][] npcQuestionTriggerIndex = new int[(int)SETTLEMENT.MAX][]; //16
    public bool[][] npcQuestionAffectHumility = new bool[(int)SETTLEMENT.MAX][]; //16
    public int[][] npcProbabilityOfTurningAway = new int[(int)SETTLEMENT.MAX][]; //16
    public List<string>[][] npcStrings = new List<string>[(int)SETTLEMENT.MAX][]; //16
    public U4_Decompiled_AVATAR.TILE[][,] settlementMap = new U4_Decompiled_AVATAR.TILE[(int)SETTLEMENT.MAX][,]; //32,32

    void LoadSettlements()
    {
        /* 
           Offset 	Length (in bytes) 	Purpose
           0x0 	    1024 	32x32 town map matrix
           0x400 	32 	    Tile for NPCs 0-31
           0x420 	32 	    Start_x for NPCs 0-31
           0x440 	32 	    Start_y for NPCs 0-31
           0x460 	32 	    Repetition of 0x400-0x41F
           0x480 	32 	    Repetition of 0x420-0x43F
           0x4A0 	32 	    Repetition of 0x440-0x45F
           0x4C0 	32 	    Movement_behavior for NPCs 0-31 (0x0-fixed, 0x1-wander, 0x80-follow, 0xFF-attack)
           0x4E0 	32 	    Conversion index (tlk file) for NPCs 0-31 
        */

        for (int settlement = 0; settlement < (int)SETTLEMENT.MAX; settlement++)
        {
            settlementNPCs[settlement] = new npc[32];
            npcQuestionTriggerIndex[settlement] = new int[16];
            npcQuestionAffectHumility[settlement] = new bool[16];
            npcProbabilityOfTurningAway[settlement] = new int[16];
            npcStrings[settlement] = new List<string>[16];
            settlementMap[settlement] = new U4_Decompiled_AVATAR.TILE[32,32];

            if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + ((SETTLEMENT)settlement).ToString() + ".ULT"))
            {
                Debug.Log("Could not find settlement file " + Application.persistentDataPath + "/u4/" + ((SETTLEMENT)settlement).ToString() + ".ULT");
                continue;
            }

            // read the file
            byte[] settlementFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + ((SETTLEMENT)settlement).ToString() + ".ULT");

            if (settlementFileData.Length != 1280)
            {
                Debug.Log("Settlement file incorrect length " + settlementFileData.Length);
                continue;
            }

            /*
                Offset 	Length (in bytes) 	Purpose
                0x0 	1 	Question Flag (3=JOB, 4=HEALTH, 5=KEYWORD1, 6=KEYWORD2)
                0x1 	1 	Does Response Affect Humility? (0=No, 1=Yes)
                0x2 	1 	Probability of Turning Away (out of 256)
                0x3 	Varies 	Name
                Varies 	Varies 	Pronoun (He, She or It)
                Varies 	Varies 	LOOK Description
                Varies 	Varies 	JOB Response
                Varies 	Varies 	HEALTH Response
                Varies 	Varies 	KEYWORD 1 Response
                Varies 	Varies 	KEYWORD 2 Response
                Varies 	Varies 	Yes/No Question
                Varies 	Varies 	YES Response
                Varies 	Varies 	NO Response
                Varies 	Varies 	KEYWORD 1
                Varies 	Varies 	KEYWORD 2
                Varies-0x119 	Varies 	00000....  
            */

            if (settlement == (int)SETTLEMENT.LCB_1 || settlement == (int)SETTLEMENT.LCB_2)
            {
                if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + "LCB" + ".TLK"))
                {
                    Debug.Log("Could not find settlement talk file " + Application.persistentDataPath + "/u4/" + "LCB" + ".TLK");
                    continue;
                }
            }
            else
            {
                if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + (SETTLEMENT)settlement + ".TLK"))
                {
                    Debug.Log("Could not find settlement talk file " + Application.persistentDataPath + "/u4/" + (SETTLEMENT)settlement + ".TLK");
                    continue;
                }
            }

            byte[] talkFileData;

            if (settlement == (int)SETTLEMENT.LCB_1 || settlement == (int)SETTLEMENT.LCB_2)
            {
                // read the file
                talkFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + "LCB" + ".TLK");
            }
            else
            {
                // read the file
                talkFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + (SETTLEMENT)settlement + ".TLK");
            }

            if (talkFileData.Length != 4608)
            {
                Debug.Log("Settlement talk file incorrect length " + talkFileData.Length);
                continue;
            }

            for (int talkIndex = 0; talkIndex < 16; talkIndex++)
            {
                npcStrings[settlement][talkIndex] = new List<string>();

                npcQuestionTriggerIndex[settlement][talkIndex] = talkFileData[talkIndex * 288];
                if (talkFileData[(talkIndex * 288) + 1] != 0)
                {
                    npcQuestionAffectHumility[settlement][talkIndex] = true;
                }
                else
                {
                    npcQuestionAffectHumility[settlement][talkIndex] = false;
                }
                npcProbabilityOfTurningAway[settlement][talkIndex] = talkFileData[talkIndex * 288 + 2];

                string s;
                int stringBufferIndex = 3;

                // search for strings in the .TLK file
                for (int stringIndex = 0; stringIndex < (int)NPC_STRING_INDEX.MAX; stringIndex++)
                {
                    // reset string
                    s = "";

                    // manually construct the string because C# doesn't work with null terminated C strings well
                    for (int i = 0; (i < 100) && (talkFileData[talkIndex * 288 + stringBufferIndex] != 0); i++)
                    {
                        s += (char)talkFileData[talkIndex * 288 + stringBufferIndex++];
                    }

                    // add it to the list even if it is empty
                    npcStrings[settlement][talkIndex].Add(s);

                    // skip over null terminator to go to the next string
                    stringBufferIndex++;
                }
            }

            // load settlement map data
            int bufferIndex = 0;

            for (int height = 0; height < 32; height++)
            {
                for (int width = 0; width < 32; width++)
                {
                    U4_Decompiled_AVATAR.TILE tileIndex = (U4_Decompiled_AVATAR.TILE)settlementFileData[bufferIndex++];
                    settlementMap[settlement][width, height] = tileIndex;
                }
            }

            // load npc data from the map data
            for (int npcIndex = 0; npcIndex < 32; npcIndex++)
            {
                U4_Decompiled_AVATAR.TILE npcTile = (U4_Decompiled_AVATAR.TILE)settlementFileData[0x400 + npcIndex];
                settlementNPCs[settlement][npcIndex].tile = npcTile;

                // zero indicates unused
                if (npcTile != 0)
                {
                    settlementNPCs[settlement][npcIndex].pos_x = settlementFileData[0x420 + npcIndex];
                    settlementNPCs[settlement][npcIndex].pos_y = settlementFileData[0x440 + npcIndex];
                    settlementNPCs[settlement][npcIndex].movement = (NPC_MOVEMENT_MODE)settlementFileData[0x4C0 + npcIndex];
                    int conversationIndex = settlementFileData[0x4E0 + npcIndex];
                    settlementNPCs[settlement][npcIndex].conversationIndex = conversationIndex;
                    // grab the talk data and add it to this structure
                    // zero indicates unused
                    if (conversationIndex != 0)
                    {
                        // this can be 128 for one vendor in Vincent, not sure why? TODO need to check this after I fix the npx talk loader
                        if ((conversationIndex - 1) < npcStrings.Length)
                        {
                            settlementNPCs[settlement][npcIndex].strings = npcStrings[settlement][conversationIndex - 1];
                            settlementNPCs[settlement][npcIndex].questionAffectHumility = npcQuestionAffectHumility[settlement][conversationIndex - 1];
                            settlementNPCs[settlement][npcIndex].probabilityOfTurningAway = npcProbabilityOfTurningAway[settlement][conversationIndex - 1];
                            settlementNPCs[settlement][npcIndex].questionTriggerIndex = npcQuestionTriggerIndex[settlement][conversationIndex - 1];
                        }
                    }
                }
            }
        }
    }

    public struct tHeader
    {//size 0x11
        /*+00*/
        public ushort magic;//0x1234
        /*+02*/
        public ushort xsize, ysize;//320,200
        /*+06*/
        public byte __06;
        public byte __07;
        public byte __08;
        public byte __09;
        /*+0a*/
        public byte bitsinf;//2
        /*+0b*/
        public byte emark;//0xff
        /*+0c*/
        public char evideo;//'A'
        /*+0d*/
        public byte edesc;//[1 == edata is pallet]
        /*+0e*/
        public byte __0e;
        /*+0f*/
        public ushort esize;
    };

    public struct tBlock
    {
        /*+00*/
        public ushort PBSIZE;
        /*+02*/
        public ushort size;
        /*+05*/
        public byte MBYTE;
    };

    void ClearTexture(Texture2D texture, Color color)
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

    void LoadAVATARPicFile(string file, Texture2D texture)
    {
        int fileIndex = 0;

        if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + file))
        {
            Debug.Log("Could not find pic file " + Application.persistentDataPath + "/u4/" + file);
            return;
        }

        // read the file
        byte[] picFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + file);

        // check if the file is at least the size of the header plus the length
        if (picFileData.Length < 0x11 + 0x02)
        {
            Debug.Log("Picture file incorrect length " + picFileData.Length);
            return;
        }

        tHeader header = new tHeader();

        header.magic = System.BitConverter.ToUInt16(picFileData, fileIndex); //0x1234
        fileIndex += 2;
        header.xsize = System.BitConverter.ToUInt16(picFileData, fileIndex); //320
        fileIndex += 2;
        header.ysize = System.BitConverter.ToUInt16(picFileData, fileIndex); //200
        fileIndex += 2;
        header.__06 = picFileData[fileIndex++];
        header.__07 = picFileData[fileIndex++];
        header.__08 = picFileData[fileIndex++];
        header.__09 = picFileData[fileIndex++];
        header.bitsinf = picFileData[fileIndex++];  //2
        header.emark = picFileData[fileIndex++]; //0xff 
        header.evideo = (char)picFileData[fileIndex++]; //'A'
        header.edesc = picFileData[fileIndex++]; //[1 == edata is pallet]
        header.__0e = picFileData[fileIndex++];
        header.esize = System.BitConverter.ToUInt16(picFileData, fileIndex);
        fileIndex += 2;

        // check header
        if (header.magic != 0x1234 ||
            //header.xsize != 320 || // vertical pixels
            //header.ysize != 200 || // horizonal pixels
            header.bitsinf != 2 || // bits per pixel
            header.emark != 0xff ||
            header.evideo != 'A' ||
            header.edesc != 1)
        {
            return;
        }

        // allocate destination for unpacked data
        byte[] dest = new byte[header.xsize * header.ysize / (8 / header.bitsinf) /* 0x3e80 or 16000*/];

        byte[] eData = new byte[header.esize];

        for (int i = 0; i < header.esize; i ++)
        {
            eData[i] = picFileData[fileIndex++];
        }

        // destination index
        int pdest = 0;

        // read in the number of blocks to process
        ushort numblks = System.BitConverter.ToUInt16(picFileData, fileIndex);
        fileIndex += 2;

        for (int i = 0; i < numblks; i ++) 
        {
			tBlock BLOCK = new tBlock();
			int len;

            // read in the block info
            BLOCK.PBSIZE = System.BitConverter.ToUInt16(picFileData, fileIndex);
            fileIndex += 2;
            BLOCK.size = System.BitConverter.ToUInt16(picFileData, fileIndex);
            fileIndex += 2;
            /*+05*/
            BLOCK.MBYTE = picFileData[fileIndex++];

            int work = fileIndex;
            int p = work;
			int block_end = work + BLOCK.PBSIZE - 5 /* size of tBlock */;
            fileIndex += (BLOCK.PBSIZE - 5 /* size of tBlock */);
            do 
            {
				len = 1;
				byte byt = picFileData[p++];
				if (byt == BLOCK.MBYTE) 
                {
					len = picFileData[p++];
					if(len == 0) 
                    {
                        len = System.BitConverter.ToUInt16(picFileData, p);
                        p += 2;

                    }
                    byt = picFileData[p++];
				} 
                while (len-- != 0)
                {
                    dest[pdest++] = byt;
                }
			} while (p < block_end);
		}

        // reset destination pointer
        pdest = 0;

        // transfer to texture
        // The end game sequence relies on images overlapping each other so we
        // will only copy if the pixel is not black
        for (int i = 0; i < header.ysize; i++)
        {
            for (int j = 0; j < header.xsize; j += 4)
            {
                byte byt = dest[pdest++];
                Color color = Palette.CGAColorPalette[(byt & 0xc0) >> 6];
                if (color != Color.black)
                {
                    texture.SetPixel(j + 0, i, color);
                }
                color = Palette.CGAColorPalette[(byt & 0x30) >> 4];
                if (color != Color.black)
                {
                    texture.SetPixel(j + 1, i, color);
                }
                color = Palette.CGAColorPalette[(byt & 0x0c) >> 2];
                if (color != Color.black)
                {
                    texture.SetPixel(j + 2, i, color);
                }
                color = Palette.CGAColorPalette[(byt & 0x03) >> 0];
                if (color != Color.black)
                {
                    texture.SetPixel(j + 3, i, color);
                }
            }
        }
        texture.Apply();
    }

    void LoadAVATAREGAFile(string file, Texture2D texture)
    {
        // check if the file exists
        if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + file))
        {
            Debug.Log("Could not find pic file " + Application.persistentDataPath + "/u4/" + file);
            return;
        }

        // read the file
        byte[] picFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + file);

        // check if the file at least has some size
        if (picFileData.Length == 0)
        {
            Debug.Log("Picture file incorrect length " + picFileData.Length);
            return;
        }

        // allocate destination for unpacked data, 2 pixels per byte
        byte[] dest = new byte[320 * 200 / 2];

        // reset source and destination indexes
        int destinationIndex = 0;
        int fileIndex = 0;

        // simple rle using magic code
        while (fileIndex < picFileData.Length)
        {
            // magic code is 2
            const byte MAGIC_CODE = 0x02;

            // read in the block info
            byte data = picFileData[fileIndex++];

            // check if data is the magic code
            if (data == MAGIC_CODE)
            {
                // get the length of the rle
                byte length = picFileData[fileIndex++];

                // get the data byte to use
                data = picFileData[fileIndex++];

                // fill destination with a runo of length of data 
                while (length-- != 0)
                {
                    // copy the data
                    dest[destinationIndex++] = data;
                }
            }
            else
            {
                // just copy the data to the destination
                dest[destinationIndex++] = data;
            }
        }

        // reset destination pointer
        destinationIndex = 0;

        // transfer to texture
        // The end game sequence relies on images overlapping each other so we
        // will only copy if the pixel is not black
        for (int y = 0; y < 200; y++)
        {
            for (int x = 0; x < 320; x += 4)
            {
                ushort word = System.BitConverter.ToUInt16(dest, destinationIndex);

                destinationIndex += 2;

                Color color = Palette.EGAColorPalette[(word & 0x000F) >> 0];
                if (color != Color.black)
                {
                    texture.SetPixel(x + 1, texture.height - 1 - y, color);
                }
                color = Palette.EGAColorPalette[(word & 0x00F0) >> 4];
                if (color != Color.black)
                {
                    texture.SetPixel(x + 0, texture.height - 1 - y, color);
                }
                color = Palette.EGAColorPalette[(word & 0x0F00) >> 8];
                if (color != Color.black)
                {
                    texture.SetPixel(x + 3, texture.height - 1 - y, color);
                }
                color = Palette.EGAColorPalette[(word & 0xF000) >> 12];
                if (color != Color.black)
                {
                    texture.SetPixel(x + 2, texture.height - 1 - y, color);
                }
            }
        }

        // apply pixels set above to texture
        texture.Apply();
    }

    Texture2D LoadTITLEPicPictureFile(string file, Texture2D texture)
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + file))
        {
            Debug.Log("Could not find pic file " + Application.persistentDataPath + "/u4/" + file);
            return null;
        }

        // read the file
        byte[] picFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + file);

        lzw l = new lzw();

        long s = l.GetDecompressedSize(picFileData, picFileData.Length);
        byte[] uncompressedFileData = new byte[s];
        l.Decompress(picFileData, uncompressedFileData, picFileData.Length);

        return PIC_To_Texture2D(uncompressedFileData, -1, texture);
    }

    Texture2D LoadTITLEEGAPictureFile(string file, Texture2D texture)
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + file))
        {
            Debug.Log("Could not find pic file " + Application.persistentDataPath + "/u4/" + file);
            return null;
        }

        // read the file
        byte[] picFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + file);

        lzw l = new lzw();

        long s = l.GetDecompressedSize(picFileData, picFileData.Length);
        byte[] dest = new byte[s];
        l.Decompress(picFileData, dest, picFileData.Length);

        return EGA_To_Texture2D(dest, -1, texture);
    }

    // TODO get this from the exe or create my own
    ushort[] RANDOM = {
                    0x0000,0x0000,0x0000,0x0000,0x0000,0x0000,0x0000,0x0000,
                    0x0000,0x0001,0xC004,0x0010,0x0080,0x0104,0x0400,0x0401,
                    0x0100,0xC3C0,0x1000,0xC410,0x1004,0x0411,0x0110,0x03C4,
                    0x1080,0xC481,0x0110,0xC104,0x1010,0xD101,0x1084,0x13C1,
                    0x1010,0x1304,0xC484,0x3CF0,0x0F3C,0x0F3F,0x3C84,0x3C3F,
                    0x10F1,0x13FC,0xCFC1,0x3CF1,0xCFCF,0x3CFF,0xD13F,0x3FF1,
                    0xD3F1,0xF3FC,0x3C8F,0xCF8F,0x3F3F,0xFFCF,0x13FC,0xCFFF,
                    0xFFFF,0xFFFF,0xFFFF,0xFFFF,0xFFFF,0xFFFF,0xFFFF,0xFFFF
                };
    public Texture2D PIC_To_Texture2D(
        byte [] raw,
        int randomStuff,
        Texture2D texture)
    {
        int x, y;

        for (y = 0; y < texture.height; y++)
        {
            int src;

            // Note: there is a 7 byte header on these files, not sure what it contains
            // Note: this is interlaced
            src = (y % 2) * 0x2000 + (y / 2) * (texture.width / 4) + 7;
            for (x = 0; x < texture.width; x += 8)
            {
                ushort word;

                word = System.BitConverter.ToUInt16(raw, src);

                src += 2;

                if (randomStuff != -1)
                {
                    if (word == 0)
                    {
                        continue;
                    }

                    word &= RANDOM[(randomStuff & 0xff) + (Random.Range(0, 7))];
                }

                // two bits per pixel
                texture.SetPixel(x + 0, texture.height - 1 - y, Palette.CGAColorPalette[(word & 0x00c0) >> 6]);
                texture.SetPixel(x + 1, texture.height - 1 - y, Palette.CGAColorPalette[(word & 0x0030) >> 4]);
                texture.SetPixel(x + 2, texture.height - 1 - y, Palette.CGAColorPalette[(word & 0x000c) >> 2]);
                texture.SetPixel(x + 3, texture.height - 1 - y, Palette.CGAColorPalette[(word & 0x0003) >> 0]);
                texture.SetPixel(x + 4, texture.height - 1 - y, Palette.CGAColorPalette[(word & 0xc000) >> 14]);
                texture.SetPixel(x + 5, texture.height - 1 - y, Palette.CGAColorPalette[(word & 0x3000) >> 12]);
                texture.SetPixel(x + 6, texture.height - 1 - y, Palette.CGAColorPalette[(word & 0x0c00) >> 10]);
                texture.SetPixel(x + 7, texture.height - 1 - y, Palette.CGAColorPalette[(word & 0x0300) >> 8]);
            }
        }

        texture.Apply();

        return texture;
    }

    public Texture2D EGA_To_Texture2D(
        byte[] raw,
        int randomStuff, 
        Texture2D texture)
    {
        int x, y;

        for (y = 0; y < texture.height; y++)
        {
            int src;

            src = y * texture.width / 2;
            for (x = 0; x < texture.width; x += 4)
            {
                ushort word;

                word = System.BitConverter.ToUInt16(raw, src);

                src += 2;

                if (randomStuff != -1)
                {
                    if (word != 0)
                    {
                        continue;
                    }

                    word &= RANDOM[(randomStuff & 0xff) + (Random.Range(0, 7))];
                }

                // 4 bits per pixel, these pixels are swapped
                Color color = Palette.EGAColorPalette[(word & 0x000F) >> 0];
                texture.SetPixel(x + 1, texture.height - 1 - y, color);
                color = Palette.EGAColorPalette[(word & 0x00F0) >> 4];
                texture.SetPixel(x + 0, texture.height - 1 - y, color);
                color = Palette.EGAColorPalette[(word & 0x0F00) >> 8];
                texture.SetPixel(x + 3, texture.height - 1 - y, color);
                color = Palette.EGAColorPalette[(word & 0xF000) >> 12];
                texture.SetPixel(x + 2, texture.height - 1 - y, color);
            }
        }

        texture.Apply();

        return texture;
    }

    [SerializeField]
    public DUNGEON[] dungeons = new DUNGEON[(int)DUNGEONS.MAX];
    void LoadDungeons()
    {
        for (int index = 0; index < (int)DUNGEONS.MAX; index++)
        {
            int rooms = 0;
            dungeons[index].name = ((DUNGEONS)index).ToString();
            dungeons[index].dungeonTILEs = new DUNGEON_TILE[8][,];
            if (index == (int)DUNGEONS.ABYSS)
            {
                rooms = 64;
            }
            else
            {
                rooms = 16;
            }

            if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + (DUNGEONS)index + ".DNG"))
            {
                Debug.Log("Could not find dungeon file " + Application.persistentDataPath + "/u4/" + (DUNGEONS)index + ".DNG");
                return;
            }

            // read the file
            byte[] dungeonFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + (DUNGEONS)index + ".DNG");

            if (dungeonFileData.Length != 0x200 + 0x100*rooms)
            {
                Debug.Log("dungeon file incorrect length " + dungeonFileData.Length);
                return;
            }

            int fileIndex = 0;

            for (int level = 0; level < 8; level++)
            {
                dungeons[index].dungeonTILEs[level] = new DUNGEON_TILE[8, 8];
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        dungeons[index].dungeonTILEs[level][x, 7 - y] = (DUNGEON_TILE)dungeonFileData[fileIndex++];
                    }
                }
            }

            dungeons[index].dungeonRooms = new DUNGEON_ROOM[rooms];

            for (int room = 0; room < rooms; room++)
            {
                dungeons[index].dungeonRooms[room].triggers = new FLOOR_TRIGGER[4];
                dungeons[index].dungeonRooms[room].monsters = new DUNGEON_MONSTER[16];
                dungeons[index].dungeonRooms[room].partyNorthEntry = new DUNGEON_PARTY_START_LOCATION[8];
                dungeons[index].dungeonRooms[room].partyEastEntry = new DUNGEON_PARTY_START_LOCATION[8];
                dungeons[index].dungeonRooms[room].partySouthEntry = new DUNGEON_PARTY_START_LOCATION[8];
                dungeons[index].dungeonRooms[room].partyWestEntry = new DUNGEON_PARTY_START_LOCATION[8];
                dungeons[index].dungeonRooms[room].dungeonRoomMap = new U4_Decompiled_AVATAR.TILE[11, 11];

                // get the triggers
                for (int i = 0; i < 4; i++)
                {
                    dungeons[index].dungeonRooms[room].triggers[i].changeTile = (U4_Decompiled_AVATAR.TILE)dungeonFileData[fileIndex++];
                    dungeons[index].dungeonRooms[room].triggers[i].trigger_x = dungeonFileData[fileIndex] >> 4;
                    dungeons[index].dungeonRooms[room].triggers[i].trigger_y = dungeonFileData[fileIndex++] & 0xf;
                    dungeons[index].dungeonRooms[room].triggers[i].changeTile_x1 = dungeonFileData[fileIndex] >> 4;
                    dungeons[index].dungeonRooms[room].triggers[i].changeTile_y1 = dungeonFileData[fileIndex++] & 0xf;
                    dungeons[index].dungeonRooms[room].triggers[i].changeTile_x2 = dungeonFileData[fileIndex] >> 4;
                    dungeons[index].dungeonRooms[room].triggers[i].changeTile_y2 = dungeonFileData[fileIndex++] & 0xf;
                }

                // get the monsters
                for (int i = 0; i < 16; i++)
                {
                    dungeons[index].dungeonRooms[room].monsters[i].monster = (U4_Decompiled_AVATAR.TILE)dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < 16; i++)
                {
                    dungeons[index].dungeonRooms[room].monsters[i].x = dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < 16; i++)
                {
                    dungeons[index].dungeonRooms[room].monsters[i].y = dungeonFileData[fileIndex++];
                }

                // get party start positions for each room entry direction
                for (int i = 0; i < 8; i++)
                {
                    dungeons[index].dungeonRooms[room].partyNorthEntry[i].x = dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < 8; i++)
                {
                    dungeons[index].dungeonRooms[room].partyNorthEntry[i].y = dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < 8; i++)
                {
                    dungeons[index].dungeonRooms[room].partyEastEntry[i].x = dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < 8; i++)
                {
                    dungeons[index].dungeonRooms[room].partyEastEntry[i].y = dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < 8; i++)
                {
                    dungeons[index].dungeonRooms[room].partySouthEntry[i].x = dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < 8; i++)
                {
                    dungeons[index].dungeonRooms[room].partySouthEntry[i].y = dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < 8; i++)
                {
                    dungeons[index].dungeonRooms[room].partyWestEntry[i].x = dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < 8; i++)
                {
                    dungeons[index].dungeonRooms[room].partyWestEntry[i].y = dungeonFileData[fileIndex++];
                }

                for (int y = 0; y < 11; y++)
                {
                    for (int x = 0; x < 11; x++)
                    {
                        dungeons[index].dungeonRooms[room].dungeonRoomMap[x, y] = (U4_Decompiled_AVATAR.TILE)dungeonFileData[fileIndex++];
                    }
                }

                fileIndex += 7; // skip over unused buffer
            }
        }
    }

    public void AddMonsters(GameObject dungeonRoomGameObject, ref DUNGEON_ROOM dungeonRoom)
    {
        GameObject monstersGameObject = new GameObject("Monsters");
        monstersGameObject.transform.SetParent(dungeonRoomGameObject.transform);

        // add all the monsters
        for (int i = 0; i < 16; i++)
        {
            U4_Decompiled_AVATAR.TILE  monsterTile = dungeonRoom.monsters[i].monster;

            if (monsterTile != 0)
            {
                GameObject monsterGameObject = Primitive.CreateQuad();
                monsterGameObject.name = monsterTile.ToString();

                // get the renderer
                MeshRenderer renderer = monsterGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

                // there is at least one case where the dungeon monster tile refers to an energy field.
                // TODO: see if these are actually monsters or just static objects in the actual game,
                // for now billboard them like actual monsters.
                if ((monsterTile >= U4_Decompiled_AVATAR.TILE.POISON_FIELD) && (monsterTile <= U4_Decompiled_AVATAR.TILE.SLEEP_FIELD))
                {
                    renderer.material.mainTexture = combinedLinearTexture;
                    renderer.material.mainTextureOffset = new Vector2((float)((int)monsterTile * originalTileWidth) / (float)renderer.material.mainTexture.width, 0.0f);
                    renderer.material.mainTextureScale = new Vector2((float)originalTileWidth / (float)renderer.material.mainTexture.width, 1.0f);

                    Animate1 animate = monsterGameObject.AddComponent<Animate1>();
                }
                else
                {
                    // add our little animator script and set the tile
                    Animate3 animate = monsterGameObject.AddComponent<Animate3>();
                    animate.npcTile = 0;
                    animate.world = this;
                    animate.ObjectRenderer = renderer;

                    animate.SetNPCTile(monsterTile);
                }

                // rotate the monster game object into position after creating
                monsterGameObject.transform.position = new Vector3(dungeonRoom.monsters[i].x, 10 - dungeonRoom.monsters[i].y, 0);
                monsterGameObject.transform.eulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

                // make it billboard
                //Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
                //look.position = new Vector3(look.position.x, 0.0f, look.position.z);
                //monsterGameObject.transform.LookAt(look.transform);
                //Vector3 rot = monsterGameObject.transform.eulerAngles;
                //monsterGameObject.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y, rot.z + 180.0f);

                // set this as a parent of the monsters game object
                monsterGameObject.transform.SetParent(monstersGameObject.transform);
            }
        }
    }
    public void AddDungeonMapMonsters()
    {
        // have we finished creating the world
        if (dungeonMonsters == null)
        {
            return;
        }

        // need to create npc game objects if none are present
        if (dungeonMonsters.transform.childCount != 16)
        {
            for (int i = 0; i < 16; i++) // TODO figure out how many can be in the dungeon level, for now assume 16 max
            {
                // a child object for each dungeon monster entry in the table
                GameObject dungeonMonsterGameObject = Primitive.CreateQuad();

                // get the renderer
                MeshRenderer renderer = dungeonMonsterGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

                // add our little animator script and set the tile to zero
                Animate3 animate = dungeonMonsterGameObject.AddComponent<Animate3>();
                animate.npcTile = 0;
                animate.world = this;
                animate.ObjectRenderer = renderer;

                // rotate the fighters game object into position after creating
                Vector3 fightersLocation = new Vector3(0, 255, 0);
                dungeonMonsterGameObject.transform.localPosition = fightersLocation;
                dungeonMonsterGameObject.transform.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

                // set this as a parent of the fighters game object
                dungeonMonsterGameObject.transform.SetParent(dungeonMonsters.transform);

                // set as intially disabled
                dungeonMonsterGameObject.SetActive(false);
            }

            // rotate characters into place
            dungeonMonsters.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }

        int monsterIndex = 0;

        // add all the monsters found in the dungeon map
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                // get a dungeonTile in the dungeon map
                DUNGEON_TILE dungeonTile = (DUNGEON_TILE)u4.tMap8x8x8[u4.Party._z, y, x];

                // check upper nibble to see if there is anything to render
                int checkDungeonTile = (int)dungeonTile & 0xf0;

                if (checkDungeonTile == (int)DUNGEON_TILE.TRAP ||
                    checkDungeonTile == (int)DUNGEON_TILE.FOUNTAIN ||
                    checkDungeonTile == (int)DUNGEON_TILE.FIELD ||
                    checkDungeonTile == (int)DUNGEON_TILE.ALTAR || // extra because I don't render these as dungeon monster sprites like the original game
                    checkDungeonTile == (int)DUNGEON_TILE.MAGIC_ORB || // extra because I don't render these as dungeon monster sprites like the original game
                    checkDungeonTile >= (int)DUNGEON_TILE.DUNGEON_ROOM)
                {
                    continue;
                }

                // dungeon monster is stored in the low nibble of the dungeon tile
                // TODO DUNGEONTILE does not account for this.
                checkDungeonTile = (int)dungeonTile & 0x0f;

                // zero means no monster at that dungeon location
                if (checkDungeonTile == 0)
                {
                    continue;
                }

                // convert the monster nibble into a map tile
                U4_Decompiled_AVATAR.TILE monsterTile = (U4_Decompiled_AVATAR.TILE)((checkDungeonTile << 2) - 4 + U4_Decompiled_AVATAR.TILE.RAT);

                // did we create enough monster child game objects
                if (monsterIndex < dungeonMonsters.transform.childCount)
                {
                    // get the corresponding monster game object
                    Transform childofmonsters = dungeonMonsters.transform.GetChild(monsterIndex++);

                    // set it active
                    childofmonsters.gameObject.SetActive(true);

                    // update the tile of the game object
                    childofmonsters.GetComponent<Animate3>().SetNPCTile(monsterTile);

                    // update the position
                    childofmonsters.localPosition = new Vector3(x * 11 + 5, (7 - y) * 11 + 5, 0);
                    childofmonsters.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

                    // make it billboard
                    Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
                    look.position = new Vector3(look.position.x, 0.0f, look.position.z);
                    childofmonsters.transform.LookAt(look.transform);
                    Vector3 rot = childofmonsters.transform.eulerAngles;
                    childofmonsters.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y, rot.z + 180.0f);
                } 
            }
        }

        // set any remaining monsters to not active
        for (; monsterIndex < dungeonMonsters.transform.childCount; monsterIndex++)
        {
            // get the corresponding monster game object
            Transform childofmonsters = dungeonMonsters.transform.GetChild(monsterIndex);
            childofmonsters.gameObject.SetActive(false);
        }
    }
    public GameObject CreateDungeonRoom(ref DUNGEON_ROOM dungeonRoom)
    {
        GameObject mapGameObject = new GameObject();
        CreateMap(mapGameObject, dungeonRoom.dungeonRoomMap, false);
        mapGameObject.transform.position = Vector3.zero;
        mapGameObject.transform.localEulerAngles = Vector3.zero;
        AddMonsters(mapGameObject, ref dungeonRoom);
        return mapGameObject;
    }
    public void CreateDungeonRooms(GameObject dungeonsRoomsObject)
    {
        for (int i = 0; i < (int)DUNGEONS.MAX; i++)
        {
            for (int room = 0; room < dungeons[i].dungeonRooms.Length; room++)
            {
                GameObject dungeonRoomObject = CreateDungeonRoom(ref dungeons[i].dungeonRooms[room]);
                dungeonRoomObject.transform.SetParent(dungeonsRoomsObject.transform);
                dungeonRoomObject.name = ((DUNGEONS)i).ToString() + " room #" + room;
                dungeonRoomObject.transform.localPosition = new Vector3((room % 16) * 11, 0, (i + (room/16)) * 11);
                dungeonRoomObject.transform.eulerAngles = new Vector3(90.0f, 0f, 0f);
            }
        }
    }
    public void CreateDungeons(GameObject dungeonsGameObject)
    {
        for (int index = 0; index < (int)DUNGEONS.MAX; index++)
        {
            GameObject dungeonGameObject = new GameObject();
            dungeonGameObject.name = ((DUNGEONS)index).ToString();
            dungeonGameObject.transform.SetParent(dungeonsGameObject.transform);

            for (int z = 0; z < 8; z++)
            {
                GameObject dungeonLevelGameObject = new GameObject();
                dungeonLevelGameObject.name = ((DUNGEONS)index).ToString() + " Level #" + z;
                dungeonLevelGameObject.transform.SetParent(dungeonGameObject.transform);
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        GameObject mapTile;
                        U4_Decompiled_AVATAR.TILE tileIndex;
                        DUNGEON_TILE dungeonTile = dungeons[index].dungeonTILEs[z][x, y];

                        if (dungeonTile == DUNGEON_TILE.WALL)
                        {
                            mapTile = Primitive.CreatePartialCube(true, true, true, true);
                            tileIndex = U4_Decompiled_AVATAR.TILE.BRICK_WALL;
                        }
                        else if (dungeonTile == DUNGEON_TILE.HALLWAY)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = U4_Decompiled_AVATAR.TILE.TILED_FLOOR;
                        }
                        else if (dungeonTile == DUNGEON_TILE.LADDER_UP)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = U4_Decompiled_AVATAR.TILE.LADDER_UP;
                        }
                        else if (dungeonTile == DUNGEON_TILE.LADDER_DOWN)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = U4_Decompiled_AVATAR.TILE.LADDER_DOWN;
                        }
                        else if (dungeonTile == DUNGEON_TILE.LADDER_UP_AND_DOWN)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = U4_Decompiled_AVATAR.TILE.LADDER_DOWN; // TODO: need to overlap the up and down tiles, but this will do for now
                        }
                        else if (dungeonTile == DUNGEON_TILE.TREASURE_CHEST)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = U4_Decompiled_AVATAR.TILE.CHEST;
                        }
                        else if ((dungeonTile == DUNGEON_TILE.FOUNTAIN) ||
                                (dungeonTile == DUNGEON_TILE.FOUNTAIN_CURE) ||
                                (dungeonTile == DUNGEON_TILE.FOUNTAIN_HEALING) ||
                                (dungeonTile == DUNGEON_TILE.FOUNTAIN_POISIN) ||
                                (dungeonTile == DUNGEON_TILE.FOUNTAIN_ACID))
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = U4_Decompiled_AVATAR.TILE.SHALLOW_WATER;
                        }
                        else if (dungeonTile == DUNGEON_TILE.FIELD_ENERGY)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = U4_Decompiled_AVATAR.TILE.ENERGY_FIELD;
                        }
                        else if (dungeonTile == DUNGEON_TILE.FIELD_FIRE)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = U4_Decompiled_AVATAR.TILE.FIRE_FIELD;
                        }
                        else if (dungeonTile == DUNGEON_TILE.FIELD_POISON)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = U4_Decompiled_AVATAR.TILE.POISON_FIELD;
                        }
                        else if (dungeonTile == DUNGEON_TILE.FIELD_SLEEP)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = U4_Decompiled_AVATAR.TILE.SLEEP_FIELD;
                        }
                        else if (dungeonTile == DUNGEON_TILE.DOOR)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = U4_Decompiled_AVATAR.TILE.DOOR;
                        }
                        else if (dungeonTile == DUNGEON_TILE.DOOR_SECRECT)
                        {
                            mapTile = Primitive.CreatePartialCube(true, true, true, true);
                            tileIndex = U4_Decompiled_AVATAR.TILE.SECRET_BRICK_WALL;
                        }
                        else if (dungeonTile == DUNGEON_TILE.ALTAR)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = U4_Decompiled_AVATAR.TILE.ALTAR;
                        }
                        else if (dungeonTile == DUNGEON_TILE.MAGIC_ORB)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = U4_Decompiled_AVATAR.TILE.MISSLE_ATTACK_BLUE;
                        }
                        else if ((dungeonTile == DUNGEON_TILE.TRAP_FALLING_ROCKS) ||
                            (dungeonTile == DUNGEON_TILE.TRAP_WIND_DARKNESS) ||
                            (dungeonTile == DUNGEON_TILE.TRAP_PIT))
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = U4_Decompiled_AVATAR.TILE.TILED_FLOOR;
                        }
                        else
                        {
                            mapTile = Primitive.CreateQuad(); 
                            tileIndex = U4_Decompiled_AVATAR.TILE.TILED_FLOOR;
                        }

                        mapTile.transform.SetParent(dungeonLevelGameObject.transform);
                        mapTile.transform.localPosition = new Vector3(x, y, 7-z);
                        Renderer renderer = mapTile.GetComponent<MeshRenderer>();
                        renderer.material.shader = Shader.Find("Unlit/Transparent Cutout"); 
                        renderer.material.mainTexture = expandedTiles[(int)tileIndex];
                        renderer.material.mainTextureOffset = new Vector2((float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
                        renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);
                    }
                }
            }

            // rotate dungeon into place
            dungeonGameObject.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }
    }

    U4_Decompiled_AVATAR.TILE[,] CreateDungeonHallway(ref DUNGEON_TILE[,] dungeonTileMap, ref DUNGEON_ROOM[] dungeonRooms, int posx, int posy, int posz,
        U4_Decompiled_AVATAR.TILE centerTile = U4_Decompiled_AVATAR.TILE.TILED_FLOOR)
    {
        DUNGEON_TILE left = dungeonTileMap[posx, (posy + 1) % 8]; //  0,-1
        DUNGEON_TILE above = dungeonTileMap[(posx + 8 - 1) % 8, posy]; // -1, 0
        DUNGEON_TILE right = dungeonTileMap[(posx + 1) % 8, posy];     //  1, 0
        DUNGEON_TILE below = dungeonTileMap[posx, (posy + 8 - 1) % 8];     //  0, 1

        DUNGEON_TILE diagonalLeftBelow = dungeonTileMap[(posx + 8 - 1) % 8, (posy + 8 - 1) % 8 ];  //  0,-1 ->  -1, 1
        DUNGEON_TILE diagonalRightBelow = dungeonTileMap[(posx + 1) % 8, (posy + 8 - 1) % 8];  // -1, 0 ->   1, 1
        DUNGEON_TILE diagonalRightAbove = dungeonTileMap[(posx + 1) % 8, (posy + 1) % 8];      //  1, 0 ->   1,-1
        DUNGEON_TILE diagonalLeftAbove = dungeonTileMap[(posx + 8 - 1) % 8, (posy + 1) % 8];   //  0, 1 ->  -1,-1

        U4_Decompiled_AVATAR.TILE tileIndex;

        U4_Decompiled_AVATAR.TILE[,] map = new U4_Decompiled_AVATAR.TILE[11, 11];

        for (int y = 0; y < 11; y++)
        {
            for (int x = 0; x < 11; x++)
            {
                tileIndex = U4_Decompiled_AVATAR.TILE.TILED_FLOOR;

                // center
                if ((x == 5) && (y == 5))
                {
                    tileIndex = centerTile;
                }

                // walls
                if (((left == DUNGEON_TILE.WALL) || (left == DUNGEON_TILE.DOOR_SECRECT)) && (y == 1))
                {
                    if ((x == 5) && (left == DUNGEON_TILE.DOOR_SECRECT))
                    {
                        tileIndex = U4_Decompiled_AVATAR.TILE.SECRET_BRICK_WALL;
                    }
                    else
                    {
                        tileIndex = U4_Decompiled_AVATAR.TILE.BRICK_WALL;
                    }
                }
                if (((above == DUNGEON_TILE.WALL) || (above == DUNGEON_TILE.DOOR_SECRECT)) && (x == 1))
                {
                    if ((y == 5) && (above == DUNGEON_TILE.DOOR_SECRECT))
                    {
                        tileIndex = U4_Decompiled_AVATAR.TILE.SECRET_BRICK_WALL;
                    }
                    else
                    {
                        tileIndex = U4_Decompiled_AVATAR.TILE.BRICK_WALL;
                    }
                }
                if (((right == DUNGEON_TILE.WALL) || (right == DUNGEON_TILE.DOOR_SECRECT)) && (x == 9))
                {
                    if ((y == 5) && (right == DUNGEON_TILE.DOOR_SECRECT))
                    {
                        tileIndex = U4_Decompiled_AVATAR.TILE.SECRET_BRICK_WALL;
                    }
                    else
                    {
                        tileIndex = U4_Decompiled_AVATAR.TILE.BRICK_WALL;
                    }
                }
                if (((below == DUNGEON_TILE.WALL) || (below == DUNGEON_TILE.DOOR_SECRECT)) && (y == 9))
                {
                    if ((x == 5) && (below == DUNGEON_TILE.DOOR_SECRECT))
                    {
                        tileIndex = U4_Decompiled_AVATAR.TILE.SECRET_BRICK_WALL;
                    }
                    else
                    {
                        tileIndex = U4_Decompiled_AVATAR.TILE.BRICK_WALL;
                    }
                }

                // rooms
                if (((left >= DUNGEON_TILE.DUNGEON_ROOM_0) && (left <= DUNGEON_TILE.DUNGEON_ROOM_15)) && (y == 0))
                {

                    // create brick walls on the other side of the room if it is not a tiled floor
                    int room = (int)left - (int)DUNGEON_TILE.DUNGEON_ROOM_0;
                    if (dungeonRooms.Length == 64)
                    {
                        room += (posz >> 1) * 16;
                    }

                    U4_Decompiled_AVATAR.TILE roomTileIndex = dungeonRooms[room].dungeonRoomMap[x, 10];
                    if ((roomTileIndex == U4_Decompiled_AVATAR.TILE.TILED_FLOOR) || (roomTileIndex == U4_Decompiled_AVATAR.TILE.BRICK_FLOOR))
                    {
                        tileIndex = U4_Decompiled_AVATAR.TILE.TILED_FLOOR;
                    }
                    else
                    {
                        tileIndex = U4_Decompiled_AVATAR.TILE.BRICK_WALL;
                    }
                }
                if (((above >= DUNGEON_TILE.DUNGEON_ROOM_0) && (above <= DUNGEON_TILE.DUNGEON_ROOM_15)) && (x == 0))
                {
                    // create brick walls on the other side of the room if it is not a tiled floor
                    int room = (int)above - (int)DUNGEON_TILE.DUNGEON_ROOM_0;
                    if (dungeonRooms.Length == 64)
                    {
                        room += (posz >> 1) * 16;
                    }

                    U4_Decompiled_AVATAR.TILE roomTileIndex = dungeonRooms[room].dungeonRoomMap[10, y];
                    if ((roomTileIndex == U4_Decompiled_AVATAR.TILE.TILED_FLOOR) || (roomTileIndex == U4_Decompiled_AVATAR.TILE.BRICK_FLOOR))
                    {
                        tileIndex = U4_Decompiled_AVATAR.TILE.TILED_FLOOR;
                    }
                    else
                    {
                        tileIndex = U4_Decompiled_AVATAR.TILE.BRICK_WALL;
                    }

                    //tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                }
                if (((right >= DUNGEON_TILE.DUNGEON_ROOM_0) && (right <= DUNGEON_TILE.DUNGEON_ROOM_15)) && (x == 10))
                {
                    // create brick walls on the other side of the room if it is not a tiled floor
                    int room = (int)right - (int)DUNGEON_TILE.DUNGEON_ROOM_0;
                    if (dungeonRooms.Length == 64)
                    {
                        room += (posz >> 1) * 16;
                    }

                    U4_Decompiled_AVATAR.TILE roomTileIndex = dungeonRooms[room].dungeonRoomMap[0, y];
                    if ((roomTileIndex == U4_Decompiled_AVATAR.TILE.TILED_FLOOR) || (roomTileIndex == U4_Decompiled_AVATAR.TILE.BRICK_FLOOR))
                    {
                        tileIndex = U4_Decompiled_AVATAR.TILE.TILED_FLOOR;
                    }
                    else
                    {
                        tileIndex = U4_Decompiled_AVATAR.TILE.BRICK_WALL;
                    }

                    // tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                }
                if (((below >= DUNGEON_TILE.DUNGEON_ROOM_0) && (below <= DUNGEON_TILE.DUNGEON_ROOM_15)) && (y == 10))
                {
                    // create brick walls on the other side of the room if it is not a tiled floor
                    int room = (int)below - (int)DUNGEON_TILE.DUNGEON_ROOM_0;
                    if (dungeonRooms.Length == 64)
                    {
                        room += (posz >> 1) * 16;
                    }

                    U4_Decompiled_AVATAR.TILE roomTileIndex = dungeonRooms[room].dungeonRoomMap[x, 0];
                    if ((roomTileIndex == U4_Decompiled_AVATAR.TILE.TILED_FLOOR) || (roomTileIndex == U4_Decompiled_AVATAR.TILE.BRICK_FLOOR))
                    {
                        tileIndex = U4_Decompiled_AVATAR.TILE.TILED_FLOOR;
                    }
                    else
                    {
                        tileIndex = U4_Decompiled_AVATAR.TILE.BRICK_WALL;
                    }
                    // tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                }

                //corners
                if ((x <= 1) && (y <= 1) && ((diagonalLeftAbove == DUNGEON_TILE.WALL) || (diagonalLeftAbove == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalLeftAbove >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalLeftAbove <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = U4_Decompiled_AVATAR.TILE.BRICK_WALL;
                }
                if ((x >= 9) && (y <= 1) && ((diagonalRightAbove == DUNGEON_TILE.WALL) || (diagonalRightAbove == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalRightAbove >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalRightAbove <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = U4_Decompiled_AVATAR.TILE.BRICK_WALL;
                }
                if ((x >= 9) && (y >= 9) && ((diagonalRightBelow == DUNGEON_TILE.WALL) || (diagonalRightBelow == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalRightBelow >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalRightBelow <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = U4_Decompiled_AVATAR.TILE.BRICK_WALL;
                }
                if ((x <= 1) && (y >= 9) && ((diagonalLeftBelow == DUNGEON_TILE.WALL) || (diagonalLeftBelow == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalLeftBelow >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalLeftBelow <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = U4_Decompiled_AVATAR.TILE.BRICK_WALL;
                }
                if ((x == 0) && (y == 0) && ((diagonalLeftAbove == DUNGEON_TILE.WALL) || (diagonalLeftAbove == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalLeftAbove >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalLeftAbove <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = U4_Decompiled_AVATAR.TILE.BLANK;
                }
                if ((x == 10) && (y == 0) && ((diagonalRightAbove == DUNGEON_TILE.WALL) || (diagonalRightAbove == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalRightAbove >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalRightAbove <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = U4_Decompiled_AVATAR.TILE.BLANK;
                }
                if ((x == 10) && (y == 10) && ((diagonalRightBelow == DUNGEON_TILE.WALL) || (diagonalRightBelow == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalRightBelow >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalRightBelow <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = U4_Decompiled_AVATAR.TILE.BLANK;
                }
                if ((x == 0) && (y == 10) && ((diagonalLeftBelow == DUNGEON_TILE.WALL) || (diagonalLeftBelow == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalLeftBelow >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalLeftBelow <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = U4_Decompiled_AVATAR.TILE.BLANK;
                }

                // override
                if ((left == DUNGEON_TILE.WALL) && (y == 0))
                {
                    tileIndex = U4_Decompiled_AVATAR.TILE.BLANK;
                }
                if ((above == DUNGEON_TILE.WALL) && (x == 0))
                {
                    tileIndex = U4_Decompiled_AVATAR.TILE.BLANK;
                }
                if ((right == DUNGEON_TILE.WALL) && (x == 10))
                {
                    tileIndex = U4_Decompiled_AVATAR.TILE.BLANK;
                }
                if ((below == DUNGEON_TILE.WALL) && (y == 10))
                {
                    tileIndex = U4_Decompiled_AVATAR.TILE.BLANK;
                }

                map[x, y] = tileIndex;
            }
        }

        return map;
    }
    GameObject CreateDungeonBlock(U4_Decompiled_AVATAR.TILE tileIndex)
    {
        GameObject dungeonBlockGameObject = new GameObject();
        dungeonBlockGameObject.name = tileIndex.ToString();

        for (int y = 0; y < 11; y++)
        {
            for (int x = 0; x < 11; x++)
            {
                GameObject mapTile;

                if (tileIndex == U4_Decompiled_AVATAR.TILE.BRICK_WALL)
                {
                    mapTile = Primitive.CreatePartialCube(true, true, true, true);
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.TILED_FLOOR)
                {
                    mapTile = Primitive.CreateQuad();
                }
                else
                {
                    mapTile = Primitive.CreateQuad();
                }

                mapTile.transform.SetParent(dungeonBlockGameObject.transform);
                mapTile.transform.localPosition = new Vector3(x, y, 0);
                Renderer renderer = mapTile.GetComponent<MeshRenderer>();
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");
                renderer.material.mainTexture = expandedTiles[(int)tileIndex];
                renderer.material.mainTextureOffset = new Vector2((float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
                renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);
            }
        }

        // rotate dungeon into place
        // do this after creating all the blocks
        //dungeonBlockGameObject.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

        return dungeonBlockGameObject;
    }
    GameObject CreateDungeonExpandedLevel(DUNGEONS dungeon, int level)
    {
        GameObject dungeonLevel = new GameObject();
        dungeonLevel.name = dungeon.ToString() + " Level #" + level;

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                GameObject dungeonBlockGameObject;

                DUNGEON_TILE dungeonTile = dungeons[(int)dungeon].dungeonTILEs[level][x, y];
                DUNGEON_TILE aboveDungeonTile = dungeons[(int)dungeon].dungeonTILEs[level][x, (y - 1 + 8) % 8];
                DUNGEON_TILE belowDungeonTile = dungeons[(int)dungeon].dungeonTILEs[level][x, (y + 1) % 8];
                DUNGEON_TILE leftDungeonTile = dungeons[(int)dungeon].dungeonTILEs[level][(x - 1 + 8) % 8, y];
                DUNGEON_TILE rightDungeonTile = dungeons[(int)dungeon].dungeonTILEs[level][(x + 1) % 8, y];

                if (dungeonTile == DUNGEON_TILE.WALL)
                {
                    //dungeonBlockGameObject = CreateDungeonBlock(U4_Decompiled.TILE.BRICK_WALL);
                    //dungeonBlockGameObject.name = dungeonTile.ToString();
                    continue;
                }
                else if ((dungeonTile >= DUNGEON_TILE.DUNGEON_ROOM_0) &&
                        (dungeonTile <= DUNGEON_TILE.DUNGEON_ROOM_15))
                {
                    int room;
                    // special case the ABYSS as it has 64 rooms instead of 16 like the others
                    if (dungeon == DUNGEONS.ABYSS)
                    {
                        room = (int)dungeonTile - (int)DUNGEON_TILE.DUNGEON_ROOM_0 + (level >> 1) * 16;
                    }
                    else
                    {
                        room = (int)dungeonTile - (int)DUNGEON_TILE.DUNGEON_ROOM_0;
                    }
                    dungeonBlockGameObject = CreateDungeonRoom(ref dungeons[(int)dungeon].dungeonRooms[room]);
                    dungeonBlockGameObject.name = "Room #" + room;
                }
                else if ((dungeonTile == DUNGEON_TILE.HALLWAY) 
                        || (dungeonTile == DUNGEON_TILE.TRAP_FALLING_ROCKS)
                        || (dungeonTile == DUNGEON_TILE.TRAP_PIT)
                        || (dungeonTile == DUNGEON_TILE.TRAP_WIND_DARKNESS)
                        )
                {
                    // TODO figure out what to do with these traps
                    U4_Decompiled_AVATAR.TILE[,] map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level], 
                        ref dungeons[(int)dungeon].dungeonRooms, 
                        x, y, level, U4_Decompiled_AVATAR.TILE.TILED_FLOOR);

                    dungeonBlockGameObject = new GameObject();
                    CreateMap(dungeonBlockGameObject, map);

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if ((dungeonTile == DUNGEON_TILE.FOUNTAIN)
                        || (dungeonTile == DUNGEON_TILE.FOUNTAIN_ACID)
                        || (dungeonTile == DUNGEON_TILE.FOUNTAIN_CURE)
                        || (dungeonTile == DUNGEON_TILE.FOUNTAIN_HEALING)
                        || (dungeonTile == DUNGEON_TILE.FOUNTAIN_POISIN))
                {
                    // TODO make a pretty fountain
                    U4_Decompiled_AVATAR.TILE[,] map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level],
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level, U4_Decompiled_AVATAR.TILE.SHALLOW_WATER);

                    dungeonBlockGameObject = new GameObject();
                    CreateMap(dungeonBlockGameObject, map);

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if (dungeonTile == DUNGEON_TILE.FIELD_ENERGY)
                {
                    U4_Decompiled_AVATAR.TILE[,] map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level],
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level, U4_Decompiled_AVATAR.TILE.ENERGY_FIELD);

                    dungeonBlockGameObject = new GameObject();
                    CreateMap(dungeonBlockGameObject, map);

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if (dungeonTile == DUNGEON_TILE.FIELD_FIRE)
                {
                    U4_Decompiled_AVATAR.TILE[,] map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level],
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level, U4_Decompiled_AVATAR.TILE.FIRE_FIELD);

                    dungeonBlockGameObject = new GameObject();
                    CreateMap(dungeonBlockGameObject, map);

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if (dungeonTile == DUNGEON_TILE.FIELD_POISON)
                {
                    U4_Decompiled_AVATAR.TILE[,] map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level],
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level, U4_Decompiled_AVATAR.TILE.POISON_FIELD);

                    dungeonBlockGameObject = new GameObject();
                    CreateMap(dungeonBlockGameObject, map);

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if (dungeonTile == DUNGEON_TILE.FIELD_SLEEP)
                {
                    // TODO make a pretty fountain
                    U4_Decompiled_AVATAR.TILE[,] map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level],
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level, U4_Decompiled_AVATAR.TILE.SLEEP_FIELD);

                    dungeonBlockGameObject = new GameObject();
                    CreateMap(dungeonBlockGameObject, map);

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if (dungeonTile == DUNGEON_TILE.TREASURE_CHEST)
                {
                    U4_Decompiled_AVATAR.TILE[,] map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level], 
                        ref dungeons[(int)dungeon].dungeonRooms, 
                        x, y, level, U4_Decompiled_AVATAR.TILE.CHEST);

                    dungeonBlockGameObject = new GameObject();
                    CreateMap(dungeonBlockGameObject, map);

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if (dungeonTile == DUNGEON_TILE.MAGIC_ORB)
                {
                    // TODO make orb into a billboard
                    U4_Decompiled_AVATAR.TILE[,] map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level], 
                        ref dungeons[(int)dungeon].dungeonRooms, 
                        x, y, level, U4_Decompiled_AVATAR.TILE.MISSLE_ATTACK_BLUE);

                    dungeonBlockGameObject = new GameObject();
                    CreateMap(dungeonBlockGameObject, map);

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                    
                }
                else if (dungeonTile == DUNGEON_TILE.ALTAR)
                {
                    // TODO make altar into a billboard
                    U4_Decompiled_AVATAR.TILE[,] map = CreateDungeonHallway(ref dungeons[(int)dungeon].dungeonTILEs[level], 
                        ref dungeons[(int)dungeon].dungeonRooms, 
                        x, y, level, U4_Decompiled_AVATAR.TILE.ALTAR);

                    dungeonBlockGameObject = new GameObject();
                    CreateMap(dungeonBlockGameObject, map);

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else
                {
                    // use a combat map as the dungeon room base on the dungeon tile
                    int combat = (int)convertDungeonTileToCombat[(int)dungeons[(int)dungeon].dungeonTILEs[level][x, y] >> 4];

                    // get a halway that fits
                    U4_Decompiled_AVATAR.TILE[,] map = CreateDungeonHallway(ref dungeons[(int)dungeon].dungeonTILEs[level], ref dungeons[(int)dungeon].dungeonRooms, x, y, level);

                    // check if we need to flip the door map
                    if ((dungeonTile == DUNGEON_TILE.DOOR) && 
                        (aboveDungeonTile == DUNGEON_TILE.WALL) && 
                        (belowDungeonTile == DUNGEON_TILE.WALL) && 
                        (leftDungeonTile != DUNGEON_TILE.WALL) && 
                        (rightDungeonTile != DUNGEON_TILE.WALL))
                    {
                        // copy the center of the combat map into the hallway
                        for (int copy_y = 2; copy_y < 9; copy_y++)
                        {
                            for (int copy_x = 2; copy_x < 9; copy_x++)
                            {
                                map[copy_x, copy_y] = combatMaps[combat][copy_y, copy_x]; // need to flip the door map
                            }
                        }
                    }
                    else if ((dungeonTile == DUNGEON_TILE.DOOR) &&
                        (aboveDungeonTile == DUNGEON_TILE.WALL) &&
                        (belowDungeonTile != DUNGEON_TILE.WALL) &&
                        (leftDungeonTile != DUNGEON_TILE.WALL) &&
                        (rightDungeonTile != DUNGEON_TILE.WALL))
                    {
                        // copy half of the combat map into the hallway
                        for (int copy_y = 5; copy_y < 9; copy_y++)
                        {
                            for (int copy_x = 2; copy_x < 9; copy_x++)
                            {
                                map[copy_x, copy_y] = combatMaps[combat][copy_y, copy_x];
                            }
                        }
                    }
                    else if ((dungeonTile == DUNGEON_TILE.DOOR) &&
                       (aboveDungeonTile != DUNGEON_TILE.WALL) &&
                       (belowDungeonTile == DUNGEON_TILE.WALL) &&
                       (leftDungeonTile != DUNGEON_TILE.WALL) &&
                       (rightDungeonTile != DUNGEON_TILE.WALL))
                    {
                        // copy half of the combat map into the hallway
                        for (int copy_y = 2; copy_y < 6; copy_y++)
                        {
                            for (int copy_x = 2; copy_x < 9; copy_x++)
                            {
                                map[copy_x, copy_y] = combatMaps[combat][copy_y, copy_x];
                            }
                        }
                    }
                    else if ((dungeonTile == DUNGEON_TILE.DOOR) &&
                      (aboveDungeonTile != DUNGEON_TILE.WALL) &&
                      (belowDungeonTile != DUNGEON_TILE.WALL) &&
                      (leftDungeonTile == DUNGEON_TILE.WALL) &&
                      (rightDungeonTile != DUNGEON_TILE.WALL))
                    {
                        // copy half of the combat map into the hallway
                        for (int copy_y = 5; copy_y < 9; copy_y++)
                        {
                            for (int copy_x = 2; copy_x < 9; copy_x++)
                            {
                                map[copy_x, copy_y] = combatMaps[combat][copy_x, copy_y];
                            }
                        }
                    }
                    else if ((dungeonTile == DUNGEON_TILE.DOOR) &&
                      (aboveDungeonTile != DUNGEON_TILE.WALL) &&
                      (belowDungeonTile != DUNGEON_TILE.WALL) &&
                      (leftDungeonTile != DUNGEON_TILE.WALL) &&
                      (rightDungeonTile == DUNGEON_TILE.WALL))
                    {
                        // copy half of the combat map into the hallway
                        for (int copy_y = 2; copy_y < 6; copy_y++)
                        {
                            for (int copy_x = 2; copy_x < 9; copy_x++)
                            {
                                map[copy_x, copy_y] = combatMaps[combat][copy_x, copy_y];
                            }
                        }
                    }
                    else
                    {
                        // copy the center of the combat map into the hallway
                        for (int copy_y = 2; copy_y < 9; copy_y++)
                        {
                            for (int copy_x = 2; copy_x < 9; copy_x++)
                            {
                                map[copy_x, copy_y] = combatMaps[combat][copy_x, copy_y];
                            }
                        }
                    }

                    dungeonBlockGameObject = new GameObject();
                    CreateMap(dungeonBlockGameObject, map);
                    dungeonBlockGameObject.name = "Combat map hallway " + ((U4_Decompiled_AVATAR.COMBAT_TERRAIN)combat).ToString();

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);
                }
                //else
                //{
                //    dungeonBlockGameObject = CreateDungeonBlock(U4_Decompiled.TILE.TILED_FLOOR);
                //}

                dungeonBlockGameObject.transform.SetParent(dungeonLevel.transform);
                dungeonBlockGameObject.transform.localPosition = new Vector3(x * 11, y * 11, 0);
            }
        }

        // TODO cannot combine yet as the  areas are already combined, need to rebuild them into the dungeon
        //Combine(dungeonLevel);

        dungeonLevel.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

        return dungeonLevel;
    }

    public GameObject GameText;

    void CreateParty()
    {
        // create player/party object to display texture
        //partyGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
        partyGameObject = Primitive.CreateQuad();
        partyGameObject.transform.SetParent(party.transform);
        
        // rotate the npc game object after creating and addition of child
        partyGameObject.transform.localPosition = new Vector3(0, 0, 0); 
        //partyGameObject.transform.localEulerAngles = new Vector3(90.0f, 180.0f, 0);
        partyGameObject.transform.localEulerAngles = new Vector3(270.0f, 180.0f, 180.0f);

        // create child object for texture
        MeshRenderer renderer = partyGameObject.GetComponent<MeshRenderer>();

        // set the tile
        renderer.material.mainTexture = expandedTiles[(int)U4_Decompiled_AVATAR.TILE.PARTY];
        renderer.material.mainTextureOffset = new Vector2((float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
        renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);

        // set the shader
        renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

        // add so speech works
        partyGameObject.AddComponent<UnityEngine.UI.Text>();

        /*
        // create the bubble text
        GameObject BubbleText = Instantiate(bubblePrefab);
        BubbleText.transform.SetParent(party.transform);
        bubblePrefab.GetComponent<Canvas>().worldCamera = Camera.main;
        bubblePrefab.transform.localPosition = Vector3.zero;
        bubblePrefab.GetComponent<RectTransform>().localPosition = new Vector3(-2.0f, 0.5f, -2.0f);
        bubblePrefab.transform.localEulerAngles = new Vector3(-90.0f, 0.0f, 0.0f);
        */
}

bool CheckTileForOpacity(U4_Decompiled_AVATAR.TILE tileIndex)
    {
        return (tileIndex == U4_Decompiled_AVATAR.TILE.BRICK_WALL
                    || tileIndex == U4_Decompiled_AVATAR.TILE.LARGE_ROCKS
                    || tileIndex == U4_Decompiled_AVATAR.TILE.SECRET_BRICK_WALL);
    }

    bool CheckShortTileForOpacity(U4_Decompiled_AVATAR.TILE tileIndex)
    {
        return (CheckTileForOpacity(tileIndex) ||
                    ((tileIndex >= U4_Decompiled_AVATAR.TILE.A) && (tileIndex <= U4_Decompiled_AVATAR.TILE.BRACKET_SQUARE)));
    }

    // NOTE certain shaders used for things like sprites and unlit textures do not
    // do well with edges and leave ghosts of the nearby textures from the texture atlas
    // to solve this issue I need to create at least a one pixel mirror border around the
    // tile, this function creates a larger tile texture and adds this border around the tile placed in the center.
    // Special care must be made when combining meshes with textures like this and the Combine()
    // function has been updated to handle this situation and update the uv's. Given that some
    // platforms may require textures be certain integer multiples of 2 this function will allow
    // a larger than one pixel border around the tile.
    const int TILE_BORDER_SIZE = 1;
    int expandedTileWidth;
    int expandedTileHeight;
    int originalTileWidth;
    int originalTileHeight;

    public void ExpandTiles()
    {
        // allocate some textures pointers
        expandedTiles = new Texture2D[256];
        expandedTileWidth = originalTiles[0].width + 2 * TILE_BORDER_SIZE;
        expandedTileHeight = originalTiles[0].height + 2 * TILE_BORDER_SIZE;
        originalTileWidth = originalTiles[0].width;
        originalTileHeight = originalTiles[0].height;

        // go through all the original tiles
        for (int i = 0; i < 256; i++)
        {
            // create a new slightly larger texture with boarder for this tile
            Texture2D currentTile = originalTiles[i];
            Texture2D newTile = new Texture2D(expandedTileWidth, expandedTileHeight, TextureFormat.RGBA32, false);

            // we want pixles not fuzzy images
            newTile.filterMode = FilterMode.Point;

            // go through all the pixels in the source texture
            for (int height = 0; height < currentTile.height; height++)
            {
                for (int width = 0; width < currentTile.width; width++)
                {
                    // copy the pixles into the middle
                    newTile.SetPixel(width + TILE_BORDER_SIZE, height + TILE_BORDER_SIZE, currentTile.GetPixel(width, height));
                }
            }

            // mirror the pixles on either side
            for (int height = 0; height < currentTile.height; height++)
            {
                // left side
                newTile.SetPixel(TILE_BORDER_SIZE - 1, height + TILE_BORDER_SIZE, currentTile.GetPixel(0, height));
                // right side
                newTile.SetPixel(TILE_BORDER_SIZE + currentTile.width, height + TILE_BORDER_SIZE, currentTile.GetPixel(currentTile.width - 1, height));
            }

            // mirror the pixles on top and bottom
            for (int width = 0; width < currentTile.width; width++)
            {
                // left side
                newTile.SetPixel(width + TILE_BORDER_SIZE, TILE_BORDER_SIZE - 1, currentTile.GetPixel(width, 0));
                // right side
                newTile.SetPixel(width + TILE_BORDER_SIZE, TILE_BORDER_SIZE + currentTile.height, currentTile.GetPixel(width, currentTile.height - 1));
            }

            // now the four corners
            newTile.SetPixel(TILE_BORDER_SIZE - 1, TILE_BORDER_SIZE - 1, currentTile.GetPixel(0, 0));
            newTile.SetPixel(TILE_BORDER_SIZE + currentTile.width, TILE_BORDER_SIZE - 1, currentTile.GetPixel(currentTile.width - 1, 0));
            newTile.SetPixel(TILE_BORDER_SIZE + currentTile.width, TILE_BORDER_SIZE + currentTile.height, currentTile.GetPixel(currentTile.width - 1, currentTile.height - 1));
            newTile.SetPixel(TILE_BORDER_SIZE - 1, TILE_BORDER_SIZE + currentTile.height, currentTile.GetPixel(0, currentTile.height - 1));

            // apply all the previous SetPixel() calls to the texture
            newTile.Apply();

            // save the new expanded texture
            expandedTiles[i] = newTile;
        }
    }

    // TODO fix horse tile also
    public void FixMageTile3()
    {
        // adjust the pixels on mage tile #3
        Texture2D currentTile = originalTiles[(int)U4_Decompiled_AVATAR.TILE.MAGE_NPC3];

        // go through all the pixels in the source texture and shift them one pixel
        for (int height = 0; height < currentTile.height; height++)
        {
            for (int width = currentTile.width - 1; width > 0 ; width--)
            {
                currentTile.SetPixel(width, height, currentTile.GetPixel((width - 1 + currentTile.width) % currentTile.width, height));
            }
        }

        // apply all the previous SetPixel() calls to the texture
        currentTile.Apply();
    }

    public void CreateMap(GameObject mapGameObject, U4_Decompiled_AVATAR.TILE[,] map, bool lookAtCamera = true)
    {
        GameObject terrainGameObject;
        GameObject animatedTerrrainGameObject;
        GameObject billboardTerrrainGameObject;
        bool useExpandedTile;

        // create the terrain child object if it does not exist
        Transform terrainTransform = mapGameObject.transform.Find("terrain");
        if (terrainTransform == null)
        {
            terrainGameObject = new GameObject("terrain");
            terrainGameObject.transform.SetParent(mapGameObject.transform);
            terrainGameObject.transform.localPosition = Vector3.zero;
            terrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            terrainGameObject = terrainTransform.gameObject;
        }

        // create the water child object if it does not exist
        Transform waterTransform = mapGameObject.transform.Find("water");
        if (waterTransform == null)
        {
            animatedTerrrainGameObject = new GameObject("water");
            animatedTerrrainGameObject.transform.SetParent(mapGameObject.transform);
            animatedTerrrainGameObject.transform.localPosition = Vector3.zero;
            animatedTerrrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            animatedTerrrainGameObject = waterTransform.gameObject;
        }

        // create the billboard child object if it does not exist
        Transform billboardTransform = mapGameObject.transform.Find("billboard");
        if (billboardTransform == null)
        {
            billboardTerrrainGameObject = new GameObject("billboard");
            billboardTerrrainGameObject.transform.SetParent(mapGameObject.transform);
            billboardTerrrainGameObject.transform.localPosition = Vector3.zero;
            billboardTerrrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            billboardTerrrainGameObject = billboardTransform.gameObject;
        }

        // remove any children if present
        foreach (Transform child in terrainGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in animatedTerrrainGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in billboardTerrrainGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }

        // this takes about 1/2 second for the 64x64 outside grid.
        // go through the map tiles and create game objects for each
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                GameObject mapTile;
                Vector3 location;
                Vector3 rotation;
                U4_Decompiled_AVATAR.TILE tileIndex;

                tileIndex = map[x, y];

                // check if it tile is blank
                if (tileIndex == U4_Decompiled_AVATAR.TILE.BLANK)
                {
                    // skip it
                    continue;
                }
                // solid object, brick, rocks etc. make into cubes
                else if (CheckTileForOpacity(tileIndex))
                {
                    U4_Decompiled_AVATAR.TILE aboveTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE belowTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE leftTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE rightTile = U4_Decompiled_AVATAR.TILE.BLANK;

                    if (y > 0)
                        aboveTile = map[x, y - 1];
                    if (y < map.GetLength(1) - 1)
                        belowTile = map[x, y + 1];
                    if (x > 0)
                        leftTile = map[x - 1, y];
                    if (x < map.GetLength(0) - 1)
                        rightTile = map[x + 1, y];

                    mapTile = Primitive.CreatePartialCube(!CheckTileForOpacity(leftTile), !CheckTileForOpacity(rightTile), !CheckTileForOpacity(aboveTile), !CheckTileForOpacity(belowTile));
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    rotation = Vector3.zero;
                    useExpandedTile = true;
                }
                // Letters, make into short cubes
                else if (((tileIndex >= U4_Decompiled_AVATAR.TILE.A) && (tileIndex <= U4_Decompiled_AVATAR.TILE.BRACKET_SQUARE))
                    || (tileIndex == U4_Decompiled_AVATAR.TILE.ARCHITECTURE))
                {
                    U4_Decompiled_AVATAR.TILE aboveTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE belowTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE leftTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE rightTile = U4_Decompiled_AVATAR.TILE.BLANK;

                    if (y > 0)
                        aboveTile = map[x, y - 1];
                    if (y < map.GetLength(1) - 1)
                        belowTile = map[x, y + 1];
                    if (x > 0)
                        leftTile = map[x - 1, y];
                    if (x < map.GetLength(0) - 1)
                        rightTile = map[x + 1, y];

                    mapTile = Primitive.CreatePartialCube(!CheckShortTileForOpacity(leftTile), !CheckShortTileForOpacity(rightTile), !CheckShortTileForOpacity(aboveTile), !CheckShortTileForOpacity(belowTile));
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    mapTile.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.25f);
                    rotation = Vector3.zero;
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.DIAGONAL_WATER_ARCHITECTURE1)
                {
                    mapTile = Primitive.CreateWedge();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.25f);
                    rotation = Vector3.zero;
                    useExpandedTile = true;
                }
                // make mountains into pyramids
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.MOUNTAINS)
                {
                    mapTile = Primitive.CreatePyramid(1.0f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 0.0f); // rotate mountains to show their best side
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // make dungeon entrace into pyramid, rotate so it faces the right direction
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.DUNGEON)
                {
                    mapTile = Primitive.CreatePyramid(0.2f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // make brush and hills into short pyramids
                else if ((tileIndex == U4_Decompiled_AVATAR.TILE.BRUSH) || (tileIndex == U4_Decompiled_AVATAR.TILE.HILLS))
                {
                    mapTile = Primitive.CreatePyramid(0.15f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // make rocks into little bigger short pyramids since you cannot walk over them
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.SMALL_ROCKS)
                {
                    mapTile = Primitive.CreatePyramid(0.25f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // trees we need to stand upright and face the camera
                else if ((tileIndex == U4_Decompiled_AVATAR.TILE.FOREST) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.TOWN) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.VILLAGE) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.RUINS) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.ANKH) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.ALTAR) ||
                   // (tileIndex == U4_Decompiled.TILE.CHEST) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.LADDER_UP) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.LADDER_DOWN) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.COOKING_FIRE) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.PARTY) || // the shrine map uses a fixed party tile instead of putting the party characters into the map
                    (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE))
                {
                    // create a billboard gameobject
                    //mapTile = GameObject.CreatePrimitive(PrimitiveType.Quad); 
                    mapTile = Primitive.CreateQuad();
                    mapTile.name = tileIndex.ToString();
                    mapTile.transform.SetParent(billboardTerrrainGameObject.transform);
                    location = new Vector3(x, map.GetLength(1) - 1 - y + 0.001f, 0.0f); // move it just a bit into the back
                    // need to move it here first and rotate it into place before we can get the results of LookAt()
                    mapTile.transform.localPosition = location;
                    //mapTile.transform.localEulerAngles = new Vector3(-180.0f, -90.0f, 90.0f);
                    rotation = new Vector3(-90.0f, 180.0f, 180.0f);

                    if (lookAtCamera)
                    {
                        Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these bulboards
                        look.position = new Vector3(look.position.x, 0.0f, look.position.z);
                        mapTile.transform.LookAt(look.transform);
                        //mapTile.transform.forward = new Vector3(Camera.main.transform.forward.x, transform.forward.y, Camera.main.transform.forward.z);
                        rotation = mapTile.transform.localEulerAngles; // new Vector3(rotx, -90f, 90.0f);
                        rotation.x = rotation.x - 180.0f;
                    }

                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRIDGE)
                {
                    mapTile = Primitive.CreateBridge();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRIDGE_TOP)
                {
                    mapTile = Primitive.CreateBridgeUpper();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRIDGE_BOTTOM)
                {
                    mapTile = Primitive.CreateBridgeLower();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if ((tileIndex == U4_Decompiled_AVATAR.TILE.DOOR) || (tileIndex == U4_Decompiled_AVATAR.TILE.LOCKED_DOOR))
                {
                    mapTile = Primitive.CreateDoor();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRICK_FLOOR_COLUMN)
                {
                    mapTile = Primitive.CreatePillar();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.SHIP_MAST)
                {
                    mapTile = Primitive.CreateMast();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.SHIP_WHEEL)
                {
                    mapTile = Primitive.CreateWheel();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.CHEST)
                {
                    mapTile = Primitive.CreateChest();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE_LEFT)
                {
                    mapTile = Primitive.CreateCastleLeft();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE_RIGHT)
                {
                    mapTile = Primitive.CreateCastleRight();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE_ENTRANCE)
                {
                    mapTile = Primitive.CreateCastleCenter();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                // all other terrain tiles are flat
                else
                {
                    mapTile = Primitive.CreateQuad();
                    // water, lava and entergy fields need to be handled separately so we can animate the texture using UV
                    // TODO may need to have single textures for the three water tiles if we want to use UV animation to show wind direction
                    if ((tileIndex <= U4_Decompiled_AVATAR.TILE.SHALLOW_WATER) ||
                        (tileIndex >= U4_Decompiled_AVATAR.TILE.POISON_FIELD && tileIndex <= U4_Decompiled_AVATAR.TILE.SLEEP_FIELD)
                        || (tileIndex == U4_Decompiled_AVATAR.TILE.LAVA))
                    {
                        mapTile.transform.SetParent(animatedTerrrainGameObject.transform);
                        location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                        rotation = Vector3.zero;
                        // since we animate the texture using uv we cannot use the expanded tiles and need to use the original ones
                        useExpandedTile = false;
                    }
                    else
                    {
                        mapTile.transform.SetParent(terrainGameObject.transform);
                        location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                        rotation = Vector3.zero;
                        useExpandedTile = true;
                    }
                }

                mapTile.transform.localEulerAngles = rotation;
                mapTile.transform.localPosition = location;

                // all terrain is static, used by combine below to merge meshes
                mapTile.isStatic = true;

                // set the shader
                Renderer renderer = mapTile.GetComponent<MeshRenderer>();
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

                // set the tile and texture offset and scale
                if (useExpandedTile)
                {
                    renderer.material.mainTexture = expandedTiles[(int)tileIndex];
                    renderer.material.mainTextureOffset = new Vector2((float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
                    renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);
                }
                else
                {
                    renderer.material.mainTexture = originalTiles[(int)tileIndex];
                    renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                    renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);
                }
            }
        }

        // this takes about 150ms for the 64x64 outside grid.
        Combine.Combine1(terrainGameObject);
        Combine.Combine2(animatedTerrrainGameObject);
        Combine.Combine1(billboardTerrrainGameObject); // combine separately from terrain above as we need to point these towards the player

        // add our little water animator script
        // adding a script component in the editor is a significant performance hit, avoid adding if already present
        if (animatedTerrrainGameObject.GetComponent<Animate1>() == null)
        {
            animatedTerrrainGameObject.AddComponent<Animate1>();
        }

        // Position the settlement in place
        mapGameObject.transform.position = new Vector3(0, 0, 224);

        // rotate settlement into place
        mapGameObject.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
    }

    public void CreateMapSubset(GameObject mapGameObject, U4_Decompiled_AVATAR.TILE[,] map)
    {
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                GameObject mapTile;
                Vector3 location = Vector3.zero;
                Vector3 rotation = Vector3.zero;
                U4_Decompiled_AVATAR.TILE tileIndex;

                tileIndex = map[x, y];

                // check if it tile is blank
                if (tileIndex == U4_Decompiled_AVATAR.TILE.BLANK)
                {
                    // skip it
                    continue;
                }
                // solid object, brick, rocks etc. make into cubes
                else if (CheckTileForOpacity(tileIndex))
                {
                    U4_Decompiled_AVATAR.TILE aboveTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE belowTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE leftTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE rightTile = U4_Decompiled_AVATAR.TILE.BLANK;

                    if (y > 0)
                        aboveTile = map[x, y - 1];
                    if (y < map.GetLength(1) - 1)
                        belowTile = map[x, y + 1];
                    if (x > 0)
                        leftTile = map[x - 1, y];
                    if (x < map.GetLength(0) - 1)
                        rightTile = map[x + 1, y];

                    mapTile = Primitive.CreatePartialCube(!CheckTileForOpacity(leftTile), !CheckTileForOpacity(rightTile), !CheckTileForOpacity(aboveTile), !CheckTileForOpacity(belowTile));
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    rotation = Vector3.zero;
                }
                // Letters, make into short cubes
                else if (((tileIndex >= U4_Decompiled_AVATAR.TILE.A) && (tileIndex <= U4_Decompiled_AVATAR.TILE.BRACKET_SQUARE))
                    || (tileIndex == U4_Decompiled_AVATAR.TILE.ARCHITECTURE))
                {
                    U4_Decompiled_AVATAR.TILE aboveTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE belowTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE leftTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE rightTile = U4_Decompiled_AVATAR.TILE.BLANK;

                    if (y > 0)
                        aboveTile = map[x, y - 1];
                    if (y < map.GetLength(1) - 1)
                        belowTile = map[x, y + 1];
                    if (x > 0)
                        leftTile = map[x - 1, y];
                    if (x < map.GetLength(0) - 1)
                        rightTile = map[x + 1, y];

                    mapTile = Primitive.CreatePartialCube(!CheckShortTileForOpacity(leftTile), !CheckShortTileForOpacity(rightTile), !CheckShortTileForOpacity(aboveTile), !CheckShortTileForOpacity(belowTile));
                    mapTile.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.25f);
                    rotation = Vector3.zero;
                }
                // make mountains into pyramids
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.MOUNTAINS)
                {
                    mapTile = Primitive.CreatePyramid(1.0f);
                    rotation = new Vector3(0.0f, 180.0f, 0.0f); // rotate mountains to show their best side
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // make dungeon entrance into pyramid, rotate so it faces the right direction
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.DUNGEON)
                {
                    mapTile = Primitive.CreatePyramid(0.2f);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // make brush and hills into short pyramids
                else if ((tileIndex == U4_Decompiled_AVATAR.TILE.BRUSH) || (tileIndex == U4_Decompiled_AVATAR.TILE.HILLS))
                {
                    mapTile = Primitive.CreatePyramid(0.15f);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // make rocks into little bigger short pyramids since you cannot walk over them
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.SMALL_ROCKS)
                {
                    mapTile = Primitive.CreatePyramid(0.25f);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // tress we need to stand upright and face the camera
                else if ((tileIndex == U4_Decompiled_AVATAR.TILE.FOREST) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.TOWN) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.ANKH) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.LADDER_UP) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.LADDER_DOWN) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.COOKING_FIRE) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE))
                {
                    // create a billboard gameobject
                    //mapTile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    mapTile = Primitive.CreateQuad();

                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    // need to move it here first and rotate it into place before we can get the results of LookAt()
                    mapTile.transform.localPosition = location;
                    mapTile.transform.localEulerAngles = new Vector3(-180.0f, -90.0f, 90.0f);
                    Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these bulboards
                    look.position = new Vector3(look.position.x, 0.0f, look.position.z);
                    mapTile.transform.LookAt(look.transform);
                    //mapTile.transform.forward = new Vector3(Camera.main.transform.forward.x, transform.forward.y, Camera.main.transform.forward.z);
                    rotation = mapTile.transform.localEulerAngles; // new Vector3(rotx, -90f, 90.0f);
                    rotation.x = rotation.x - 180.0f;
                }
                // all other terrain tiles are flat
                else
                {
                    mapTile = Primitive.CreateQuad();

                    // water, lava and entergy fields need to be handled separately so we can animate the texture using UV
                    // TODO may need to have single textures for the three water tiles if we want to use UV animation to show wind direction
                    if ((tileIndex <= U4_Decompiled_AVATAR.TILE.SHALLOW_WATER) ||
                        (tileIndex >= U4_Decompiled_AVATAR.TILE.POISON_FIELD && tileIndex <= U4_Decompiled_AVATAR.TILE.SLEEP_FIELD)
                        || (tileIndex == U4_Decompiled_AVATAR.TILE.LAVA))
                    {
                        location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                        rotation = Vector3.zero;
                        // since we animate the texture using uv we cannot use the expanded tiles and need to use the original ones
                    }
                    else
                    {
                        location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                        rotation = Vector3.zero;
                    }
                }

                mapTile.transform.localEulerAngles = rotation;
                mapTile.transform.localPosition = location;

                // all terrain is static, used by combine below to merge meshes
                mapTile.isStatic = true;

                Renderer renderer = mapTile.GetComponent<MeshRenderer>();

                renderer.material.mainTexture = originalTiles[(int)tileIndex];
                renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);

                // stash the object mesh, transform & texture information
                entireMapGameObjects[x, y] = mapTile;
            }
        }
    }

    public GameObject CreateMapTileObject(GameObject terrainGameObject, GameObject billboardTerrrainGameObject, GameObject animatedTerrrainGameObject, U4_Decompiled_AVATAR.TILE tileIndex, ref U4_Decompiled_AVATAR.TILE[,] map, int x, int y, bool allWalls)
    {
        GameObject mapTile;
        Vector3 location = Vector3.zero;
        Vector3 rotation = Vector3.zero;

        bool useExpandedTile;
        bool useLinearTile;

        // solid object, brick, rocks etc. make into cubes
        if (CheckTileForOpacity(tileIndex))
        {
            if (allWalls == false)
            {
                U4_Decompiled_AVATAR.TILE aboveTile = U4_Decompiled_AVATAR.TILE.BLANK;
                U4_Decompiled_AVATAR.TILE belowTile = U4_Decompiled_AVATAR.TILE.BLANK;
                U4_Decompiled_AVATAR.TILE leftTile = U4_Decompiled_AVATAR.TILE.BLANK;
                U4_Decompiled_AVATAR.TILE rightTile = U4_Decompiled_AVATAR.TILE.BLANK;

                if (y > 0)
                    aboveTile = map[x, y - 1];
                if (y < map.GetLength(1) - 1)
                    belowTile = map[x, y + 1];
                if (x > 0)
                    leftTile = map[x - 1, y];
                if (x < map.GetLength(0) - 1)
                    rightTile = map[x + 1, y];

                mapTile = Primitive.CreatePartialCube(!CheckTileForOpacity(leftTile), !CheckTileForOpacity(rightTile), !CheckTileForOpacity(aboveTile), !CheckTileForOpacity(belowTile));
            }
            else
            {
                mapTile = Primitive.CreatePartialCube();
            }
            mapTile.transform.SetParent(terrainGameObject.transform);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            rotation = Vector3.zero;
            useExpandedTile = true;
            useLinearTile = false;
        }
        // Letters, make into short cubes
        else if (((tileIndex >= U4_Decompiled_AVATAR.TILE.A) && (tileIndex <= U4_Decompiled_AVATAR.TILE.BRACKET_SQUARE))
            || (tileIndex == U4_Decompiled_AVATAR.TILE.ARCHITECTURE))
        {
            if (allWalls == false)
            {
                U4_Decompiled_AVATAR.TILE aboveTile = U4_Decompiled_AVATAR.TILE.BLANK;
                U4_Decompiled_AVATAR.TILE belowTile = U4_Decompiled_AVATAR.TILE.BLANK;
                U4_Decompiled_AVATAR.TILE leftTile = U4_Decompiled_AVATAR.TILE.BLANK;
                U4_Decompiled_AVATAR.TILE rightTile = U4_Decompiled_AVATAR.TILE.BLANK;

                if (y > 0)
                    aboveTile = map[x, y - 1];
                if (y < map.GetLength(1) - 1)
                    belowTile = map[x, y + 1];
                if (x > 0)
                    leftTile = map[x - 1, y];
                if (x < map.GetLength(0) - 1)
                    rightTile = map[x + 1, y];

                mapTile = Primitive.CreatePartialCube(!CheckShortTileForOpacity(leftTile), !CheckShortTileForOpacity(rightTile), !CheckShortTileForOpacity(aboveTile), !CheckShortTileForOpacity(belowTile));
            }
            else
            {
                mapTile = Primitive.CreatePartialCube();

            }
            mapTile.transform.SetParent(terrainGameObject.transform);
            mapTile.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.25f);
            rotation = Vector3.zero;
            useExpandedTile = true;
            useLinearTile = false;
        }
        // make mountains into pyramids
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.MOUNTAINS)
        {
            mapTile = Primitive.CreatePyramid(1.0f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 0.0f); // rotate mountatins to show their best side
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // make dungeon entrace into pyramid, rotate so it faces the right direction
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.DUNGEON)
        {
            mapTile = Primitive.CreatePyramid(0.2f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 90.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // make brush and hills into short pyramids
        else if ((tileIndex == U4_Decompiled_AVATAR.TILE.BRUSH) || (tileIndex == U4_Decompiled_AVATAR.TILE.HILLS))
        {
            mapTile = Primitive.CreatePyramid(0.15f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 90.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // make rocks into little bigger short pyramids since you cannot walk over them
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.SMALL_ROCKS)
        {
            mapTile = Primitive.CreatePyramid(0.25f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 90.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // tress we need to stand upright and face the camera
        else if ((tileIndex == U4_Decompiled_AVATAR.TILE.FOREST) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.TOWN) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.VILLAGE) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.RUINS) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.SHRINE) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.ANKH) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.LADDER_UP) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.LADDER_DOWN) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.COOKING_FIRE) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE))
        {
            mapTile = Primitive.CreateQuad();
            mapTile.transform.SetParent(billboardTerrrainGameObject.transform);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            // put this in a resonable rotation, combine3() will do the actual lookat rotaion just before displaying
            rotation = new Vector3(-90.0f, -90.0f, 90.0f);

            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRIDGE)
        {
            mapTile = Primitive.CreateBridge();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRIDGE_TOP)
        {
            mapTile = Primitive.CreateBridgeUpper();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRIDGE_BOTTOM)
        {
            mapTile = Primitive.CreateBridgeLower();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if ((tileIndex == U4_Decompiled_AVATAR.TILE.DOOR) || (tileIndex == U4_Decompiled_AVATAR.TILE.LOCKED_DOOR))
        {
            mapTile = Primitive.CreateDoor();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRICK_FLOOR_COLUMN)
        {
            mapTile = Primitive.CreatePillar();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.SHIP_MAST)
        {
            mapTile = Primitive.CreateMast();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.SHIP_WHEEL)
        {
            mapTile = Primitive.CreateWheel();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.CHEST)
        {
            mapTile = Primitive.CreateChest();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE_LEFT)
        {
            mapTile = Primitive.CreateCastleLeft();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE_RIGHT)
        {
            mapTile = Primitive.CreateCastleRight();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE_ENTRANCE)
        {
            mapTile = Primitive.CreateCastleCenter();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // all other terrain tiles are flat
        else
        {
            mapTile = Primitive.CreateQuad();

            // water, lava and entergy fields need to be handled separately so we can animate the texture using UV
            // TODO may need to have single textures for the three water tiles if we want to use UV animation to show wind direction
            if ((tileIndex <= U4_Decompiled_AVATAR.TILE.SHALLOW_WATER) ||
                (tileIndex >= U4_Decompiled_AVATAR.TILE.POISON_FIELD && tileIndex <= U4_Decompiled_AVATAR.TILE.SLEEP_FIELD)
                || (tileIndex == U4_Decompiled_AVATAR.TILE.LAVA))
            {
                mapTile.transform.SetParent(animatedTerrrainGameObject.transform);
                location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                rotation = Vector3.zero;
                // since we animate the texture using uv we cannot use the expanded tiles and need to use the linear ones
                useExpandedTile = false;
                useLinearTile = true;
            }
            else
            {
                mapTile.transform.SetParent(terrainGameObject.transform);
                location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                rotation = Vector3.zero;
                useExpandedTile = true;
                useLinearTile = false;
            }
        }

        mapTile.transform.localEulerAngles = rotation;
        mapTile.transform.localPosition = location;

        // all terrain is static, used by combine below to merge meshes
        mapTile.isStatic = true;

        // set the shader
        Renderer renderer = mapTile.GetComponent<MeshRenderer>();
        renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

        // set the tile and texture offset and scale

        if (useExpandedTile)
        {
            renderer.material = combinedExpandedMaterial;
            renderer.material.mainTexture = combinedExpandedTexture;
            renderer.material.mainTextureOffset = new Vector2((float)(TILE_BORDER_SIZE + (((int)tileIndex % 16) * expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(TILE_BORDER_SIZE + (((int)tileIndex / 16) * expandedTileHeight)) / (float)renderer.material.mainTexture.height);
            renderer.material.mainTextureScale = new Vector2((float)(originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(originalTileHeight) / (float)renderer.material.mainTexture.height);

        }
        else if (useLinearTile)
        {
            renderer.material = combinedLinearMaterial;
            renderer.material.mainTexture = combinedLinearTexture;
            renderer.material.mainTextureOffset = new Vector2((float)((int)tileIndex * originalTileWidth) / (float)renderer.material.mainTexture.width, 0.0f);
            renderer.material.mainTextureScale = new Vector2((float)originalTileWidth / (float)renderer.material.mainTexture.width, 1.0f);
        }
        else
        {
            renderer.material.mainTexture = originalTiles[(int)tileIndex];
            renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
            renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);
        }

        Mesh mesh = mapTile.GetComponent<MeshFilter>().mesh;
        Vector2[] uv = new Vector2[mesh.uv.Length];
        Vector2 textureAtlasOffset;

        textureAtlasOffset = new Vector2((int)tileIndex % textureExpandedAtlasPowerOf2 * expandedTileWidth, (int)tileIndex / textureExpandedAtlasPowerOf2 * expandedTileHeight);
        for (int u = 0; u < mesh.uv.Length; u++)
        {
            Vector2 mainTextureOffset;
            Vector2 mainTextureScale;

            if (useExpandedTile)
            {
                mainTextureOffset = new Vector2((float)(TILE_BORDER_SIZE + (((int)tileIndex % 16) * expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(TILE_BORDER_SIZE + (((int)tileIndex / 16) * expandedTileHeight)) / (float)renderer.material.mainTexture.height);
                mainTextureScale = new Vector2((float)(originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(originalTileHeight) / (float)renderer.material.mainTexture.height);
            }
            else if (useLinearTile)
            {
                mainTextureOffset = new Vector2((float)((int)tileIndex * originalTileWidth) / (float)renderer.material.mainTexture.width, 0.0f);
                mainTextureScale = new Vector2((float)originalTileWidth / (float)renderer.material.mainTexture.width, 1.0f);
            }
            else
            {
                mainTextureOffset = new Vector2(0.0f, 0.0f);
                mainTextureScale = new Vector2(1.0f, 1.0f);
            }

            uv[u] = Vector2.Scale(mesh.uv[u], mainTextureScale);
            uv[u] += (textureAtlasOffset + mainTextureOffset);
        }
        mesh.uv = uv;

        renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
        renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);

        // disable these as we don't need them in the actual game only for mesh combine
        mapTile.SetActive(false);

        return mapTile;
    }

    GameObject[] allMapTilesGameObjects = null;

    GameObject GetCachedTileGameObject(GameObject terrainGameObject, GameObject billboardTerrrainGameObject, GameObject animatedTerrrainGameObject, U4_Decompiled_AVATAR.TILE tileIndex, ref U4_Decompiled_AVATAR.TILE[,] map, int x, int y, bool allWalls)
    {
        if (allMapTilesGameObjects == null)
        {
            allMapTilesGameObjects = new GameObject[(int)U4_Decompiled_AVATAR.TILE.MAX];
            for (int i = 0; i < (int)U4_Decompiled_AVATAR.TILE.MAX; i++)
            {
                allMapTilesGameObjects[i] = CreateMapTileObject(terrainGameObject, billboardTerrrainGameObject, animatedTerrrainGameObject, (U4_Decompiled_AVATAR.TILE)i, ref map, 0, 0, true);
            }
        }

        return allMapTilesGameObjects[(int)tileIndex];
    }

    public struct Labels
    {
        public List<Label> labels;// = new List<Words>();
    }

    public struct Label
    {
        public List<Letter> letters;// = new List<Letter>();
    }

    public struct Letter
    {
        public U4_Decompiled_AVATAR.TILE tileIndex;
        public int x;
        public int y;
    }

    public void CreateMapLabels(GameObject mapGameObject, ref U4_Decompiled_AVATAR.TILE[,] map)
    {
        Labels mapLabels = new Labels();
        mapLabels.labels = new List<Label>();

        GameObject mapLabelsGameObject;
        GameObject mapLabelsReverseGameObject;

        // create the labels child object if it does not exist
        Transform mapLabelsTransform = mapGameObject.transform.Find("labels");
        if (mapLabelsTransform == null)
        {
            mapLabelsGameObject = new GameObject("labels");
            mapLabelsGameObject.transform.SetParent(mapGameObject.transform);
            mapLabelsGameObject.transform.localPosition = Vector3.zero;
            mapLabelsGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            mapLabelsGameObject = mapLabelsTransform.gameObject;
        }

        // create the reverse labels child object if it does not exist
        Transform mapLabelsReverseTransform = mapGameObject.transform.Find("labels reverse");
        if (mapLabelsReverseTransform == null)
        {
            mapLabelsReverseGameObject = new GameObject("labels reverse");
            mapLabelsReverseGameObject.transform.SetParent(mapGameObject.transform);
            mapLabelsReverseGameObject.transform.localPosition = Vector3.zero;
            mapLabelsReverseGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            mapLabelsReverseGameObject = mapLabelsReverseTransform.gameObject;
        }

        // remove any children if present
        foreach (Transform child in mapLabelsGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in mapLabelsReverseGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }

        // go through the map tiles and find words
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                U4_Decompiled_AVATAR.TILE tileIndex;

                tileIndex = map[x, y];

                // check if a tile is a letter
                if ((tileIndex >= U4_Decompiled_AVATAR.TILE.A) && (tileIndex <= U4_Decompiled_AVATAR.TILE.Z) || (tileIndex == U4_Decompiled_AVATAR.TILE.SPACE))
                {
                    // create a new label
                    Label label = new Label();
                    label.letters = new List<Letter>();
                    Letter letter;

                    // find all the letters in this label
                    for (; x < map.GetLength(0); x++)
                    {
                        tileIndex = map[x, y];

                        // check if a tile is a letter
                        if ((tileIndex >= U4_Decompiled_AVATAR.TILE.A) && (tileIndex <= U4_Decompiled_AVATAR.TILE.Z) || (tileIndex == U4_Decompiled_AVATAR.TILE.SPACE))
                        {
                            letter.tileIndex = tileIndex;
                            letter.x = x;
                            letter.y = y;

                            // add these letters to the label
                            label.letters.Add(letter);
                        }
                        else
                        {
                            // we are done with this label, drop out
                            break;
                        }
                    }

                    // add this label to the label list and look for more
                    mapLabels.labels.Add(label);
                }
            }
        }

        // go through labels we found above and create a game object label and reverse label for each
        foreach (Label label in mapLabels.labels)
        {
            GameObject labelGameObject = new GameObject("");
            GameObject reverseLabelGameObject = new GameObject("");

            labelGameObject.transform.SetParent(mapLabelsGameObject.transform);
            reverseLabelGameObject.transform.SetParent(mapLabelsReverseGameObject.transform);

            for (int i = 0; i < label.letters.Count; i++)
            {
                Letter letter = label.letters[i];
                U4_Decompiled_AVATAR.TILE reverseTileIndex = label.letters[label.letters.Count - 1 - i].tileIndex;

                if (letter.tileIndex != U4_Decompiled_AVATAR.TILE.SPACE)
                {
                    labelGameObject.name += (char)(letter.tileIndex - U4_Decompiled_AVATAR.TILE.A + 'A');
                }
                else
                {
                    labelGameObject.name += ' ';
                }
                if (reverseTileIndex != U4_Decompiled_AVATAR.TILE.SPACE)
                {
                    //reverseLabelGameObject.name += (char)(reverseTileIndex - U4_Decompiled.TILE.A + 'A');
                    reverseLabelGameObject.name += (char)(letter.tileIndex - U4_Decompiled_AVATAR.TILE.A + 'A');
                }
                else
                {
                    reverseLabelGameObject.name += ' ';
                }

                // create the gameObject tile
                GameObject mapTile = Primitive.CreatePartialCube();
                mapTile.transform.SetParent(labelGameObject.transform);
                mapTile.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
                mapTile.transform.localPosition = new Vector3(letter.x, map.GetLength(1) - 1 - letter.y, 0.25f);
                mapTile.transform.localEulerAngles = Vector3.zero;

                // all terrain is static, used by combine below to merge meshes
                mapTile.isStatic = true;

                // set the shader
                Renderer renderer = mapTile.GetComponent<MeshRenderer>();
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

                // set the tile and texture offset and scale
                renderer.material = combinedExpandedMaterial;
                renderer.material.mainTexture = combinedExpandedTexture;
                renderer.material.mainTextureOffset = new Vector2((float)(TILE_BORDER_SIZE + (((int)letter.tileIndex % 16) * expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(TILE_BORDER_SIZE + (((int)letter.tileIndex / 16) * expandedTileHeight)) / (float)renderer.material.mainTexture.height);
                renderer.material.mainTextureScale = new Vector2((float)(originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(originalTileHeight) / (float)renderer.material.mainTexture.height);

                Mesh mesh = mapTile.GetComponent<MeshFilter>().mesh;
                Vector2[] uv = new Vector2[mesh.uv.Length];
                Vector2 textureAtlasOffset;

                textureAtlasOffset = new Vector2((int)letter.tileIndex % textureExpandedAtlasPowerOf2 * expandedTileWidth, (int)letter.tileIndex / textureExpandedAtlasPowerOf2 * expandedTileHeight);
                for (int u = 0; u < mesh.uv.Length; u++)
                {
                    Vector2 mainTextureOffset;
                    Vector2 mainTextureScale;

                    mainTextureOffset = new Vector2((float)(TILE_BORDER_SIZE + (((int)letter.tileIndex % 16) * expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(TILE_BORDER_SIZE + (((int)letter.tileIndex / 16) * expandedTileHeight)) / (float)renderer.material.mainTexture.height);
                    mainTextureScale = new Vector2((float)(originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(originalTileHeight) / (float)renderer.material.mainTexture.height);

                    uv[u] = Vector2.Scale(mesh.uv[u], mainTextureScale);
                    uv[u] += (textureAtlasOffset + mainTextureOffset);
                }
                mesh.uv = uv;

                renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);

                // create another game object to match the first but we will reverse the letters
                GameObject reverseMapTile = Primitive.CreatePartialCube();
                reverseMapTile.transform.SetParent(reverseLabelGameObject.transform);
                reverseMapTile.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
                reverseMapTile.transform.localPosition = new Vector3(letter.x, map.GetLength(1) - 1 - letter.y, 0.25f);
                reverseMapTile.transform.localEulerAngles = new Vector3(0, 0, 180); // flip the letter around

                // all terrain is static, used by combine below to merge meshes
                reverseMapTile.isStatic = true;

                // set the shader
                renderer = reverseMapTile.GetComponent<MeshRenderer>();
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

                // set the tile and texture offset and scale
                renderer.material = combinedExpandedMaterial;
                renderer.material.mainTexture = combinedExpandedTexture;
                renderer.material.mainTextureOffset = new Vector2((float)(TILE_BORDER_SIZE + (((int)reverseTileIndex % 16) * expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(TILE_BORDER_SIZE + (((int)reverseTileIndex / 16) * expandedTileHeight)) / (float)renderer.material.mainTexture.height);
                renderer.material.mainTextureScale = new Vector2((float)(originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(originalTileHeight) / (float)renderer.material.mainTexture.height);

                mesh = reverseMapTile.GetComponent<MeshFilter>().mesh;
                uv = new Vector2[mesh.uv.Length];

                textureAtlasOffset = new Vector2((int)reverseTileIndex % textureExpandedAtlasPowerOf2 * expandedTileWidth, (int)reverseTileIndex / textureExpandedAtlasPowerOf2 * expandedTileHeight);
                for (int u = 0; u < mesh.uv.Length; u++)
                {
                    Vector2 mainTextureOffset;
                    Vector2 mainTextureScale;

                    mainTextureOffset = new Vector2((float)(TILE_BORDER_SIZE + (((int)reverseTileIndex % 16) * expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(TILE_BORDER_SIZE + (((int)reverseTileIndex / 16) * expandedTileHeight)) / (float)renderer.material.mainTexture.height);
                    mainTextureScale = new Vector2((float)(originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(originalTileHeight) / (float)renderer.material.mainTexture.height);

                    uv[u] = Vector2.Scale(mesh.uv[u], mainTextureScale);
                    uv[u] += (textureAtlasOffset + mainTextureOffset);
                }
                mesh.uv = uv;

                renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);
            }

            Combine.Combine1(labelGameObject);
            Combine.Combine1(reverseLabelGameObject);
        }
    }


    public void CreateMapSubsetPass2(GameObject mapGameObject, ref U4_Decompiled_AVATAR.TILE[,] map, ref GameObject[,] mapGameObjects, bool allWalls = false)
    {
        GameObject terrainGameObject;
        GameObject animatedTerrrainGameObject;
        GameObject billboardTerrrainGameObject;

        // create the terrain child object if it does not exist
        Transform terrainTransform = mapGameObject.transform.Find("terrain");
        if (terrainTransform == null)
        {
            terrainGameObject = new GameObject("terrain");
            terrainGameObject.transform.SetParent(mapGameObject.transform);
            terrainGameObject.transform.localPosition = Vector3.zero;
            terrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            terrainGameObject = terrainTransform.gameObject;
        }

        // create the water child object if it does not exist
        Transform waterTransform = mapGameObject.transform.Find("water");
        if (waterTransform == null)
        {
            animatedTerrrainGameObject = new GameObject("water");
            animatedTerrrainGameObject.transform.SetParent(mapGameObject.transform);
            animatedTerrrainGameObject.transform.localPosition = Vector3.zero;
            animatedTerrrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            animatedTerrrainGameObject = waterTransform.gameObject;
        }

        // create the billboard child object if it does not exist
        Transform billboardTransform = mapGameObject.transform.Find("billboard");
        if (billboardTransform == null)
        {
            billboardTerrrainGameObject = new GameObject("billboard");
            billboardTerrrainGameObject.transform.SetParent(mapGameObject.transform);
            billboardTerrrainGameObject.transform.localPosition = Vector3.zero;
            billboardTerrrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            billboardTerrrainGameObject = billboardTransform.gameObject;
        }

        // remove any children if present
        foreach (Transform child in terrainGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in animatedTerrrainGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in billboardTerrrainGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }

        // go through the map tiles and create game objects for each
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                U4_Decompiled_AVATAR.TILE tileIndex;

                tileIndex = map[x, y];

                // check if it tile is blank
                if (tileIndex == U4_Decompiled_AVATAR.TILE.BLANK)
                {
                    // skip it
                    continue;
                }

                // create the gameObject tile
                //GameObject mapTile = CreateMapTileObject(terrainGameObject, billboardTerrrainGameObject, animatedTerrrainGameObject, tileIndex, ref map, x, y, allWalls);
                GameObject mapTile = GetCachedTileGameObject(terrainGameObject, billboardTerrrainGameObject, animatedTerrrainGameObject, tileIndex, ref map, x, y, allWalls);

                // stash the game object in the array
                mapGameObjects[x, y] = mapTile;
            }
        }
    }

    public void followWorld(GameObject follow)
    {
        // hook the player game object into the camera and the game engine
        MySmoothFollow myScript = FindObjectsOfType<MySmoothFollow>()[0];

        if (myScript.target != follow.transform)
        {
            myScript.target = follow.transform;
        }
    }

    public Texture2D combinedTexture;
    public Material combinedMaterial;
    public Hashtable textureAtlasHashTable = new Hashtable();
    public int textureAtlasPowerOf2;
    void CreateSquareTextureAtlas(ref Texture2D[] tilesTextures, bool useMipMaps = false, TextureFormat textureFormat = TextureFormat.RGBA32)
    {
        int sizeW;
        int sizeH;
        int originalSizeW;
        int originalSizeH;
        Texture2D texture;

        // figure out the square size power of 2 factor for the number of textures we have
        if (tilesTextures.Length == 0 )
        {
            Debug.Log("empty tilesTextures");
            return;
        }
        else if (tilesTextures.Length == 1)
        {
            textureAtlasPowerOf2 = 1;
        }
        else if (tilesTextures.Length <= 4)
        {
            textureAtlasPowerOf2 = 2;
        }
        else if (tilesTextures.Length <= 16)
        {
            textureAtlasPowerOf2 = 4;
        }
        else if (tilesTextures.Length <= 64)
        {
            textureAtlasPowerOf2 = 8;
        }
        else if (tilesTextures.Length <= 256)
        {
            textureAtlasPowerOf2 = 16;
        }
        // extend as needed
        else
        {
            Debug.Log("too many tilesTextures");
            return;
        }

        // check for null texture in array
        if (tilesTextures[0] == null)
        {
            Debug.Log("empty tilesTextures[0]");
            return;
        }

        // get the texture size used from the first one
        originalSizeW = tilesTextures[0].width;
        originalSizeH = tilesTextures[0].height;

        // calc full size of texture atlas
        sizeW = textureAtlasPowerOf2 * originalSizeW;
        sizeH = textureAtlasPowerOf2 * originalSizeH;

        // creare a new combined texture big enough to hold all the textures
        combinedTexture = new Texture2D(sizeW, sizeH, textureFormat, useMipMaps);
        // we want our pixels
        combinedTexture.filterMode = FilterMode.Point;

        // Create the combined square texture using the existing textures (remember to ensure the total size of the texture isn't
        // larger than the platform supports)
        int index = 0;
        for (int i = 0; i < tilesTextures.Length; i++)
        {
            texture = (Texture2D)tilesTextures[i];
            if (texture && !textureAtlasHashTable.ContainsKey(texture))
            {
                int x = (index % textureAtlasPowerOf2) * originalSizeW;
                int y = (index / textureAtlasPowerOf2) * originalSizeH;

                combinedTexture.SetPixels(x, y, originalSizeW, originalSizeH, texture.GetPixels());

                x = index % textureAtlasPowerOf2;
                y = index / textureAtlasPowerOf2;
                textureAtlasHashTable.Add(texture, new Vector2(x, y));
                index++;
            }
        }
        // apply all the pixles we copied in the loop above
        combinedTexture.Apply();

        // create a material based on this texture atlas
        combinedMaterial = new Material(Shader.Find("Unlit/Transparent Cutout"));
        combinedMaterial.mainTexture = combinedTexture;
    }

    public Texture2D combinedLinearTexture;
    public Material combinedLinearMaterial;
    void CreateLinearTextureAtlas(ref Texture2D[] tilesTextures, bool useMipMaps = false, TextureFormat textureFormat = TextureFormat.RGBA32)
    {
        int sizeW;
        int originalSizeW;
        int originalSizeH;
        Texture2D texture;

        // figure out the square size power of 2 factor for the number of textures we have
        if (tilesTextures.Length == 0)
        {
            Debug.Log("empty tilesTextures");
            return;
        }
        // extend as needed
        else if (tilesTextures.Length > 256)
        {
            Debug.Log("too many tilesTextures");
            return;
        }

        // check for null texture in array
        if (tilesTextures[0] == null)
        {
            Debug.Log("empty tilesTextures[0]");
            return;
        }

        // get the texture size used from the first one
        originalSizeW = tilesTextures[0].width; 
        originalSizeH = tilesTextures[0].height;

        // calc full size of texture atlas
        sizeW = tilesTextures.Length * originalSizeW;

        // creare a new combined texture big enough to hold all the textures
        combinedLinearTexture = new Texture2D(sizeW, originalSizeH, textureFormat, useMipMaps);
        // we want our pixels
        combinedLinearTexture.filterMode = FilterMode.Point;

        // Create the combined square texture using the existing textures (remember to ensure the total size of the texture isn't
        // larger than the platform supports)
        int index = 0;
        for (int i = 0; i < tilesTextures.Length; i++)
        {
            texture = (Texture2D)tilesTextures[i];
            if (texture)
            {
                int x = index * originalSizeW;
                int y = 0;

                combinedLinearTexture.SetPixels(x, y, originalSizeW, originalSizeH, texture.GetPixels());
                index++;
            }
        }
        // apply all the pixles we copied in the loop above
        combinedLinearTexture.Apply();

        // create a material based on this texture atlas
        combinedLinearMaterial = new Material(Shader.Find("Unlit/Transparent Cutout"));
        combinedLinearMaterial.mainTexture = combinedLinearTexture;
    }

    public Texture2D combinedExpandedTexture;
    public Material combinedExpandedMaterial;
    public int textureExpandedAtlasPowerOf2;

    void CreateExpandedTextureAtlas(ref Texture2D[] tilesTextures, bool useMipMaps = false, TextureFormat textureFormat = TextureFormat.RGBA32)
    {
        int sizeW; 
        int sizeH;
        int originalSizeW; 
        int originalSizeH;

        // figure out the square size power of 2 factor for the number of textures we have
        if (tilesTextures.Length == 0)
        {
            Debug.Log("empty tilesTextures");
            return;
        }
        else if (tilesTextures.Length == 1)
        {
            textureExpandedAtlasPowerOf2 = 1;
        }
        else if (tilesTextures.Length <= 4)
        {
            textureExpandedAtlasPowerOf2 = 2;
        }
        else if (tilesTextures.Length <= 16)
        {
            textureExpandedAtlasPowerOf2 = 4;
        }
        else if (tilesTextures.Length <= 64)
        {
            textureExpandedAtlasPowerOf2 = 8;
        }
        else if (tilesTextures.Length <= 256)
        {
            textureExpandedAtlasPowerOf2 = 16;
        }
        // extend as needed
        else
        {
            Debug.Log("too many tilesTextures");
            return;
        }

        // check for null texture in array
        if (tilesTextures[0] == null)
        {
            Debug.Log("empty tilesTextures[0]");
            return;
        }

        // get the texture size used from the first one
        originalSizeW = tilesTextures[0].width;
        originalSizeH = tilesTextures[0].height;

        // calc full square size of texture atlas
        sizeW = textureExpandedAtlasPowerOf2 * originalSizeW;
        sizeH = textureExpandedAtlasPowerOf2 * originalSizeH;

        // creare a new combined texture big enough to hold all the textures
        combinedExpandedTexture = new Texture2D(sizeW, sizeH, textureFormat, useMipMaps);
        // we want our pixels
        combinedExpandedTexture.filterMode = FilterMode.Point;

        // Create the combined square texture using the existing textures (remember to ensure the total size of the texture isn't
        // larger than the platform supports)
        for (int i = 0; i < tilesTextures.Length; i++)
        {
            Texture2D texture = tilesTextures[i];
            if (texture)
            {
                int x = (i % textureExpandedAtlasPowerOf2) * originalSizeW;
                int y = (i / textureExpandedAtlasPowerOf2) * originalSizeH;

                combinedExpandedTexture.SetPixels(x, y, originalSizeW, originalSizeH, texture.GetPixels());
            }
        }
        // apply all the pixles we copied in the loop above
        combinedExpandedTexture.Apply();

        // create a material based on this texture atlas
        combinedExpandedMaterial = new Material(Shader.Find("Unlit/Transparent Cutout"));
        combinedExpandedMaterial.mainTexture = combinedExpandedTexture;
    }

    public void AddFighters(U4_Decompiled_AVATAR.t_68[] currentFighters, U4_Decompiled_AVATAR.tCombat1[] currentCombat, int offsetx = 0, int offsety = 0)
    {
        // have we finished creating the world
        if (fighters == null)
        {
            return;
        }

        // need to create npc game objects if none are present
        if (fighters.transform.childCount != 16)
        {
            for (int i = 0; i < 16; i++)
            {
                // a child object for each fighters entry in the table
                //GameObject fighterGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                GameObject fighterGameObject = Primitive.CreateQuad();

                // get the renderer
                MeshRenderer renderer = fighterGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

                // add our little animator script and set the tile
                Animate3 animate = fighterGameObject.AddComponent<Animate3>();
                animate.npcTile = 0;
                animate.world = this;
                animate.ObjectRenderer = renderer;

                // rotate the fighters game object into position after creating
                Vector3 fightersLocation = new Vector3(0, 255, 0);
                fighterGameObject.transform.localPosition = fightersLocation;
                fighterGameObject.transform.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

                // set this as a parent of the fighters game object
                fighterGameObject.transform.SetParent(fighters.transform);

                // set as intially disabled
                fighterGameObject.SetActive(false);
            }

            // rotate characters into place
            fighters.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }

        // update all fighters in the table
        for (int fighterIndex = 0; fighterIndex < 16; fighterIndex++)
        {
            // get the tile
            U4_Decompiled_AVATAR.TILE npcTile = currentFighters[fighterIndex]._tile;
            U4_Decompiled_AVATAR.TILE npcCurrentTile = currentFighters[fighterIndex]._gtile;

            // get the corresponding fighters game object
            Transform childoffighters = fighters.transform.GetChild(fighterIndex);

            if (npcTile == U4_Decompiled_AVATAR.TILE.DEEP_WATER)
            {
                childoffighters.gameObject.SetActive(false);
            }
            else
            {
                childoffighters.gameObject.SetActive(true);
            }

            // update the tile of the game object
            if (currentFighters[fighterIndex]._sleeping == 0)
            {
                childoffighters.GetComponent<Animate3>().SetNPCTile(npcCurrentTile);
            }
            else
            {
                childoffighters.GetComponent<Animate3>().SetNPCTile(U4_Decompiled_AVATAR.TILE.SLEEP);
            }

            // update the position
            childoffighters.localPosition = new Vector3(currentCombat[fighterIndex]._npcX + offsetx, 255 - currentCombat[fighterIndex]._npcY + offsety, 0);
            childoffighters.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

            // make it billboard
            Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
            look.position = new Vector3(look.position.x, 0.0f, look.position.z);
            childoffighters.transform.LookAt(look.transform);
            Vector3 rot = childoffighters.transform.eulerAngles;
            childoffighters.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y, rot.z + 180.0f);
        }
    }

    public void AddCharacters(U4_Decompiled_AVATAR.tCombat2[] currentCombat2, U4_Decompiled_AVATAR.tParty currentParty, U4_Decompiled_AVATAR.t_68[] currentFighters, int offsetx = 0, int offsety = 0)
    {
        // have we finished creating the world
        if (characters == null)
        {
            return;
        }

        // need to create character game objects if none are present
        if (characters.transform.childCount != 8)
        {
            for (int i = 0; i < 8; i++)
            {
                // a child object for each character entry in the table
                //GameObject characterGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                GameObject characterGameObject = Primitive.CreateQuad();

                // get the renderer
                MeshRenderer renderer = characterGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

                // add our little animator script and set the tile
                Animate3 animate = characterGameObject.AddComponent<Animate3>();
                animate.npcTile = 0;
                animate.world = this;
                animate.ObjectRenderer = renderer;

                // rotate the character game object into position after creating
                Vector3 characterLocation = new Vector3(0, 255, 0);
                characterGameObject.transform.localPosition = characterLocation;
                characterGameObject.transform.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

                // set this as a parent of the fighters game object
                characterGameObject.transform.SetParent(characters.transform);

                // set as intially disabled
                characterGameObject.SetActive(false);
            }

            // rotate characters into place
            characters.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }

        // update all characters in the party table
        for (int characterIndex = 0; characterIndex < 8; characterIndex++)
        {
            U4_Decompiled_AVATAR.TILE npcTile;

            if (characterIndex < currentParty.f_1d8)
            {
                // get the tile ???, use class or something?
                npcTile = currentFighters[characterIndex]._chtile;
            }
            else
            {
                // set unused characters to 0
                npcTile = 0;
            }

            // get the corresponding character game object
            Transform childofcharacters = characters.transform.GetChild(characterIndex);

            if (npcTile == U4_Decompiled_AVATAR.TILE.DEEP_WATER)
            {
                childofcharacters.gameObject.SetActive(false);
            }
            else
            {
                childofcharacters.gameObject.SetActive(true);
            }

            // update the tile of the game object
            childofcharacters.GetComponent<Animate3>().SetNPCTile(npcTile);
  
            // update the position
            childofcharacters.localPosition = new Vector3(currentCombat2[characterIndex]._charaX + offsetx, 255 - currentCombat2[characterIndex]._charaY + offsety, 0); // appears to be one off in the Y from the fighters
            childofcharacters.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

            // make it billboard
            Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
            look.position = new Vector3(look.position.x, 0.0f, look.position.z);
            childofcharacters.transform.LookAt(look.transform);
            Vector3 rot = childofcharacters.transform.eulerAngles;
            childofcharacters.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y, rot.z + 180.0f);
        }

        FindObjectsOfType<MySmoothFollow>()[0].target = characters.transform.GetChild(0);
    }

    public void AddMoongate()
    {
        MeshRenderer renderer;
        
        // have we finished creating the world
        if (moongate == null)
        {
            return;
        }

        // need to create moongate child game objects if none is present
        if (moongate.transform.childCount != 1)
        {
            // create the moongate object
            GameObject moongateGameObject = Primitive.CreateQuad();

            // get the renderer
            renderer = moongateGameObject.GetComponent<MeshRenderer>();

            // intially the texture is null
            renderer.material.mainTexture = null;

            // set the shader
            renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

            // rotate the moongate game object into position after creating
            Vector3 moongateLocation = new Vector3(0, 255, 0);
            moongateGameObject.transform.localPosition = moongateLocation;
            moongateGameObject.transform.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

            // set this as a parent of the moongate game object
            moongateGameObject.transform.SetParent(moongate.transform);

            // rotate moongate into place
            moongate.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }

        // get the corresponding moongate game object
        Transform childofmoongate = moongate.transform.GetChild(0);

        renderer = childofmoongate.GetComponent<MeshRenderer>();

        renderer.material.mainTexture = originalTiles[(int)u4.moongate_tile];

        // get adjusted position based on the offset of the raycastOutdoorMap due to the player position
        int posx = u4.moongate_x - (lastRaycastPlayer_posx - raycastOutdoorMap.GetLength(0) / 2 - 1);
        int posy = u4.moongate_y - (lastRaycastPlayer_posy - raycastOutdoorMap.GetLength(1) / 2 - 1);
        // can we see the npc
        if (posx < 0 || posy < 0 || posx >= raycastOutdoorMap.GetLength(0) || posy >= raycastOutdoorMap.GetLength(1) || raycastOutdoorMap[posx, posy] == U4_Decompiled_AVATAR.TILE.BLANK)
        {
            childofmoongate.gameObject.SetActive(false);
        }
        else
        {
            childofmoongate.gameObject.SetActive(true);
        }

        // update the position
        childofmoongate.localPosition = new Vector3(u4.moongate_x, entireMapTILEs.GetLength(1) - 1 - u4.moongate_y, 0); 

        //childofmoongate.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);
        // make it billboard
        Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
        look.position = new Vector3(look.position.x, 0.0f, look.position.z);
        childofmoongate.transform.LookAt(look.transform);
        Vector3 rot = childofmoongate.transform.eulerAngles;
        childofmoongate.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y, rot.z + 180.0f);
    }

    public void AddNPCs(U4_Decompiled_AVATAR.tNPC[] currentNpcs)
    {
        // have we finished creating the world
        if (npcs == null)
        {
            return;
        }

        // need to create npc game objects if none are present
        if (npcs.transform.childCount != 32)
        {
            for (int i = 0; i < 32; i++)
            {
                // a child object for each npc entry in the table
                //GameObject npcGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                GameObject npcGameObject = Primitive.CreateQuad();

                // get the renderer
                MeshRenderer renderer = npcGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

                // add our little animator script and set the tile
                Animate3 animate = npcGameObject.AddComponent<Animate3>();
                animate.npcTile = 0;
                animate.world = this;
                animate.ObjectRenderer = renderer;

                // rotate the npc game object into position after creating
                Vector3 npcLocation = new Vector3(0, 255, 0);
                npcGameObject.transform.localPosition = npcLocation;
                npcGameObject.transform.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

                // set this as a parent of the npcs game object
                npcGameObject.transform.SetParent(npcs.transform);

                // set as intially disabled
                npcGameObject.SetActive(false);
            }

            // rotate npcs into place
            npcs.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }

        // update all npcs in the table
        for (int npcIndex = 0; npcIndex < 32; npcIndex++)
        {
            // get the corresponding npc game object
            Transform childofnpcs = npcs.transform.GetChild(npcIndex);

            // get the npc tile
            U4_Decompiled_AVATAR.TILE npcTile = currentNpcs[npcIndex]._tile;
            U4_Decompiled_AVATAR.TILE npcCurrentTile = currentNpcs[npcIndex]._gtile;

            // check if npc is active
            if (npcTile == U4_Decompiled_AVATAR.TILE.DEEP_WATER)
            {
                // disable object if not active
                childofnpcs.gameObject.SetActive(false);
            }
            else
            {
                // get the npc position
                int posx = currentNpcs[npcIndex]._x;
                int posy = currentNpcs[npcIndex]._y;

                // inside buildings we need to check extra stuff
                if (u4.current_mode == U4_Decompiled_AVATAR.MODE.BUILDING)
                {
                    SETTLEMENT settlement;

                    // get the current settlement, need to special case BRITANNIA as the castle has two levels, use the ladder to determine which level
                    if ((u4.Party._loc == U4_Decompiled_AVATAR.LOCATIONS.BRITANNIA) && (u4.tMap32x32[3, 3] == U4_Decompiled_AVATAR.TILE.LADDER_UP))
                    {
                        settlement = SETTLEMENT.LCB_1;
                    }
                    else
                    {
                        settlement = (SETTLEMENT)u4.Party._loc;
                    }

                    // set the name of the game object to match the npc
                    if ((currentNpcs[npcIndex]._tlkidx == 0) || (currentNpcs[npcIndex]._tlkidx > 16 /* sometimes this is 127 */))
                    {
                        childofnpcs.name = npcTile.ToString();
                    }
                    else
                    {
                        childofnpcs.name = npcStrings[(int)settlement][currentNpcs[npcIndex]._tlkidx - 1][(int)NPC_STRING_INDEX.NAME];
                    }

                    // adjust position based on the offset of the raycastSettlementMap due to the player position
                    posx = posx - (lastRaycastPlayer_posx - raycastSettlementMap.GetLength(0) / 2 - 1);
                    posy = posy - (lastRaycastPlayer_posy - raycastSettlementMap.GetLength(1) / 2 - 1);
                    // can we see the npc
                    if (posx < 0 || posy < 0 || posx >= raycastSettlementMap.GetLength(0) || posy >= raycastSettlementMap.GetLength(1) || raycastSettlementMap[posx, posy] == U4_Decompiled_AVATAR.TILE.BLANK)
                    {
                        childofnpcs.gameObject.SetActive(false);
                    }
                    else
                    {
                        childofnpcs.gameObject.SetActive(true);
                    }
                }
                else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.OUTDOORS)
                {
                    // adjust position based on the offset of the raycastSettlementMap due to the player position
                    posx = posx - (lastRaycastPlayer_posx - raycastOutdoorMap.GetLength(0) / 2 - 1);
                    posy = posy - (lastRaycastPlayer_posy - raycastOutdoorMap.GetLength(1) / 2 - 1);
                    // can we see the npc
                    if (posx < 0 || posy < 0 || posx >= raycastOutdoorMap.GetLength(0) || posy >= raycastOutdoorMap.GetLength(1) || raycastOutdoorMap[posx, posy] == U4_Decompiled_AVATAR.TILE.BLANK)
                    {
                        childofnpcs.gameObject.SetActive(false);
                    }
                    else
                    {
                        childofnpcs.gameObject.SetActive(true);
                    }

                    // set the name of the game object to match the npc
                    childofnpcs.name = npcTile.ToString();
                }

                // update the tile of the game object
                //childofnpcs.GetComponent<Animate3>().SetNPCTile(npcTile);
                childofnpcs.GetComponent<Animate3>().SetNPCTile(npcCurrentTile);
                
                // update the position
                childofnpcs.localPosition = new Vector3(currentNpcs[npcIndex]._x, entireMapTILEs.GetLength(1) - 1 - currentNpcs[npcIndex]._y, 0);

                // make it billboard
                Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
                look.position = new Vector3(look.position.x, 0.0f, look.position.z);
                childofnpcs.transform.LookAt(look.transform);
                Vector3 rot = childofnpcs.transform.eulerAngles;
                childofnpcs.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y , rot.z + 180.0f);
            }
        }
    }
    public void AddHits(List<U4_Decompiled_AVATAR.hit> currentHitList, int offsetx = 0, int offsety = 0)
    {
        // have we finished creating the world
        if (hits == null)
        {
            return;
        }

        // need to create hit game objects if none are present, will will use a pool of 10
        if (hits.transform.childCount != 10)
        {
            for (int i = 0; i < 10; i++)
            {
                // a child object for each npc entry in the table
                //GameObject hitGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                GameObject hitGameObject = Primitive.CreateQuad();

                // get the renderer
                MeshRenderer renderer = hitGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

                // rotate the hit game object into position after creating
                Vector3 npcLocation = new Vector3(0, 255, 0);
                hitGameObject.transform.localPosition = npcLocation;
                hitGameObject.transform.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

                // set this as a parent of the hits game object
                hitGameObject.transform.SetParent(hits.transform);

                // set as intially disabled
                hitGameObject.SetActive(false);
            }

            // rotate npcs into place
            hits.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }

        // update all hit games with data from the table
        for (int hitIndex = 0; hitIndex < 10; hitIndex++)
        {
            // get the corresponding hit game object
            Transform childofhits = hits.transform.GetChild(hitIndex);
            
            // do we need to use the pool game object
            if (hitIndex < currentHitList.Count)
            {
                // get the tile
                U4_Decompiled_AVATAR.TILE hitTile = currentHitList[hitIndex].tile;

                // update the tile of the game object
                childofhits.GetComponent<Renderer>().material.mainTexture = originalTiles[(int)hitTile];

                // update the position
                childofhits.localPosition = new Vector3(currentHitList[hitIndex].x + offsetx, 255 - currentHitList[hitIndex].y - 0.01f + offsety, 0); // move it slightly in from of the characters and fighters so we can see it.

                // make it billboard
                Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
                look.position = new Vector3(look.position.x, 0.0f, look.position.z);
                childofhits.transform.LookAt(look.transform);
                Vector3 rot = childofhits.transform.eulerAngles;
                childofhits.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y, rot.z + 180.0f);

                // set as enabled
                childofhits.gameObject.SetActive(true);
            }
            else
            {
                // set as disabled
                childofhits.gameObject.SetActive(false);
            }
        }
    }

    public void AddActiveCharacter(U4_Decompiled_AVATAR.activeCharacter currentActiveCharacter, int offsetx = 0, int offsety = 0)
    {
        if (activeCharacter == null)
        {
            activeCharacter = GameObject.CreatePrimitive(PrimitiveType.Cube);
            activeCharacter.transform.SetParent(transform);
            activeCharacter.transform.localPosition = Vector3.zero;
            activeCharacter.transform.localRotation = Quaternion.identity;
            activeCharacter.name = "Active Character";
            MeshRenderer renderer = activeCharacter.GetComponent<MeshRenderer>();
            // set the shader
            renderer.material.shader = Shader.Find("Custom/Geometry/Wireframe");
            //Shader.Find("Custom/Geometry/Wireframe").EnableKeyword("_REMOVEDIAG_ON")
            renderer.material.SetFloat("_WireframeVal", 0.03f);
            renderer.material.SetColor("_FrontColor", Color.yellow);
            renderer.material.SetColor("_BackColor", Color.yellow);
            renderer.material.SetColor("_BackColor", Color.yellow);

            // rotate active character box into place
            characters.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }

        if (currentActiveCharacter.active)
        {
            Vector3 location = new Vector3(currentActiveCharacter.x + offsetx, 0.01f, entireMapTILEs.GetLength(1) - 1 - currentActiveCharacter.y + offsety);
            activeCharacter.transform.localPosition = location;
            activeCharacter.SetActive(true);
        }
        else
        {
            activeCharacter.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
 
    }

    // cast one ray
    void Cast_Ray(ref U4_Decompiled_AVATAR.TILE[,] map, 
        int diff_x,
        int diff_y,
        int pos_x,
        int pos_y, 
        ref U4_Decompiled_AVATAR.TILE[,] raycastMap, int offset_x, int offset_y, U4_Decompiled_AVATAR.TILE wrapTile)
    {
        U4_Decompiled_AVATAR.TILE temp_tile;

        // are we outside the destination raycast map area, stop here
        if (pos_x - offset_x >= raycastMap.GetLength(0) || pos_y - offset_y >= raycastMap.GetLength(1) || pos_x - offset_x < 0 || pos_y - offset_y < 0)
        {
            return;
        }

        // has the tile already been copied, if so stop here
        if (raycastMap[pos_x - offset_x, pos_y - offset_y] != U4_Decompiled_AVATAR.TILE.BLANK)
        {
            return;
        }

        // check if we should wrap the source map or if we should fill
        // any tile outside of the map area with a specific tile such as GRASS
        // are we outside the source map?
        if ((wrapTile != U4_Decompiled_AVATAR.TILE.BLANK) && (pos_x >= map.GetLength(0) || pos_y >= map.GetLength(1) || pos_x < 0 || pos_y < 0))
        {
            temp_tile = wrapTile;
            raycastMap[pos_x - offset_x, pos_y - offset_y] = temp_tile;
        }
        else
        {
            // get the tile and copy it to the raycast map
            temp_tile = map[(pos_x + map.GetLength(0)) % map.GetLength(0), (pos_y + map.GetLength(1)) % map.GetLength(1)];
            raycastMap[pos_x - offset_x, pos_y - offset_y] = temp_tile;
        }

        // check the tile for opaque tiles
        if ((temp_tile == U4_Decompiled_AVATAR.TILE.FOREST) ||
            (temp_tile == U4_Decompiled_AVATAR.TILE.MOUNTAINS) ||
            (temp_tile == U4_Decompiled_AVATAR.TILE.BLANK) ||
            (temp_tile == U4_Decompiled_AVATAR.TILE.SECRET_BRICK_WALL) ||
            (temp_tile == U4_Decompiled_AVATAR.TILE.BRICK_WALL))
        {
            return;
        }

        // continue the ray cast recursively
        pos_x = (pos_x + diff_x);
        pos_y = (pos_y + diff_y);
        Cast_Ray(ref map, diff_x, diff_y, pos_x, pos_y, ref raycastMap, offset_x, offset_y, wrapTile);
        
        if ((diff_x & diff_y) != 0)
        {
            Cast_Ray(ref map, 
                diff_x, 
                diff_y, 
                pos_x, 
                (pos_y - diff_y), 
                ref raycastMap, offset_x, offset_y, wrapTile);
            Cast_Ray(ref map, 
                diff_x, 
                diff_y, 
                (pos_x - diff_x), 
                pos_y, 
                ref raycastMap, offset_x, offset_y, wrapTile);
        }
        else
        {
            Cast_Ray(ref map, 
                (((diff_x == 0) ? 1 : 0) * diff_y + diff_x), 
                (diff_y - ((diff_y == 0) ? 1 : 0) * diff_x), 
                (diff_y + pos_x), 
                (pos_y - diff_x), 
                ref raycastMap, offset_x, offset_y, wrapTile);
            Cast_Ray(ref map, 
                (diff_x - ((diff_x == 0) ? 1 : 0) * diff_y), 
                (((diff_y == 0) ? 1 : 0) * diff_x + diff_y), 
                (pos_x - diff_y), 
                (diff_x + pos_y), 
                ref raycastMap, offset_x, offset_y, wrapTile);
        }
    }

    // visible area (raycast)
    void raycast(ref U4_Decompiled_AVATAR.TILE[,] map, int pos_x, int pos_y, ref U4_Decompiled_AVATAR.TILE[,] raycastMap, int offset_x, int offset_y, U4_Decompiled_AVATAR.TILE wrapTile)
    {
        if (pos_x < 0 || pos_y < 0 || pos_x >= map.GetLength(0) || pos_y >= map.GetLength(1))
        {
            Debug.Log("start position is outside of source map ( " + pos_x + ", " + pos_y + ")");
            return;
        }

        if (pos_x - offset_x < 0 || pos_y - offset_y < 0 || pos_x - offset_x >= raycastMap.GetLength(0) || pos_y - offset_y >= raycastMap.GetLength(1))
        {
            Debug.Log("offset does not contain the starting position given the dimensions of the destination raycast map " 
                + "position ( " + pos_x + ", " + pos_y + ")" 
                + " offset (" + offset_x + ", " + offset_y + ")" 
                + " dimensions (" + raycastMap.GetLength(0) + ", " + raycastMap.GetLength(1) + ")");
            return;
        }

        // disable raycast outdoors when in the ballon and it is airborne, just copy all the tiles instead
        if ((u4.Party._tile == U4_Decompiled_AVATAR.TILE.BALOON) && (u4.Party.f_1dc == 1))
        {
            for (int y = 0; y < raycastMap.GetLength(1); y++)
            {
                for (int x = 0; x < raycastMap.GetLength(0); x++)
                {
                     raycastMap[x, y] = map[(x + offset_x + map.GetLength(0)) % map.GetLength(0), (y + offset_y + map.GetLength(1)) % map.GetLength(1)];
                }
            }
        }
        else
        {
            // set all visible tiles in the destination raycast map to blank to start
            for (int y = 0; y < raycastMap.GetLength(1); y++)
            {
                for (int x = 0; x < raycastMap.GetLength(0); x++)
                {
                    raycastMap[x, y] = U4_Decompiled_AVATAR.TILE.BLANK;
                }
            }

            // copy the starting position as it is alway visible given the map offset
            U4_Decompiled_AVATAR.TILE currentTile = map[pos_x, pos_y];
            raycastMap[pos_x - offset_x, pos_y - offset_y] = currentTile;

            // cast out recusively from the starting position
            Cast_Ray(ref map, 0, -1, pos_x, (pos_y - 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray UP
            Cast_Ray(ref map, 0, 1, pos_x, (pos_y + 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray DOWN
            Cast_Ray(ref map, -1, 0, (pos_x - 1), pos_y, ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray LEFT
            Cast_Ray(ref map, 1, 0, (pos_x + 1), pos_y, ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray RIGHT
            Cast_Ray(ref map, 1, 1, (pos_x + 1), (pos_y + 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray DOWN and to the RIGHT
            Cast_Ray(ref map, 1, -1, (pos_x + 1), (pos_y - 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray UP and to the RIGHT
            Cast_Ray(ref map, -1, 1, (pos_x - 1), (pos_y + 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray DOWN and to the LEFT
            Cast_Ray(ref map, -1, -1, (pos_x - 1), (pos_y - 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray UP and to the LEFT
        }
    }

    // changes in these require redrawing the map
    int lastRaycastPlayer_posx = -1;
    int lastRaycastPlayer_posy = -1;
    int lastRaycastPlayer_f_1dc = -1;
    U4_Decompiled_AVATAR.DIRECTION lastRaycastP_surface_party_direction = (U4_Decompiled_AVATAR.DIRECTION )(-1);
    bool last_door_timer = false;

    // create a temp TILE map array to hold the combat terrains as we load them
    U4_Decompiled_AVATAR.TILE[][,] combatMaps = new U4_Decompiled_AVATAR.TILE[(int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX][,];
    CombatMonsterStartPositions[][] combatMonsterStartPositions = new CombatMonsterStartPositions[(int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX][];
    CombatPartyStartPositions[][] combatPartyStartPositions = new CombatPartyStartPositions[(int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX][];

    public Image vision;
    Texture2D visionTexture;

    public Texture2D[] picture1;
    public Texture2D[] picture2;
    public Texture2D[] picture3;
    public Texture2D[] picture4;
    public enum PICTURE
    {
        /* The use a special format for the AVATAR.EXE both PIC and EGA files are different than TITLE.EXE */
        RUNE_0 = 0, // I
        RUNE_1 = 1, // N
        RUNE_2 = 2, // F
        RUNE_3 = 3, // T
        RUNE_4 = 4, // Y
        RUNE_5 = 5, // infinity symbol
        START = 6, // main screen layout
        KEY7 = 7, // keyhole
        STONCRCL = 8, // end game, return to stone circle
        TRUTH = 9, // these are combined and displayed in the end game
        LOVE = 10,
        COURAGE = 11,
        HONESTY = 12,
        COMPASSN = 13,
        VALOR = 14,
        JUSTICE = 15,
        SACRIFIC = 16,
        HONOR = 17,
        SPIRIT = 18,
        HUMILITY = 19,
        MAX = 20
    };

    public enum PICTURE2
    {
        /* The use a special format for the TITLE.EXE both PIC and EGA files are different than AVATAR.EXE */
        OUTSIDE = 0, // outside circus
        PORTAL = 1, // stone circle for portal
        TREE = 2, // outside tree
        INSIDE = 3, // inside circus
        WAGON = 4, // gypsy wagon
        GYPSY = 5, // gypsy fortune teller
        ABACUS = 6, // abacus and beads
        HONCOM = 7, // honor and compassion tarot cards
        VALJUS = 8, // valor and justice tarot cards
        SACHONOR = 9, // sacrafice and honor tarot cards
        SPIRHUM = 10, // spirit and humility tarot cards
        TITLE = 11, // title screen
        ANIMATE = 12, // large animated character frames from the title screen
        MAX = 13
    };

    private void Start()
    {
        // this object needs to move around so it needs to be above the other which are based on the whole world map
        mainTerrain = new GameObject("Main Terrain");

        // create game object under us to hold these sub categories of things
        terrain = new GameObject("terrain");
        terrain.transform.SetParent(mainTerrain.transform);
        terrain.transform.localPosition = Vector3.zero;
        terrain.transform.localRotation = Quaternion.identity;
        animatedTerrrain = new GameObject("water");
        animatedTerrrain.transform.SetParent(mainTerrain.transform);
        animatedTerrrain.transform.localPosition = Vector3.zero;
        animatedTerrrain.transform.localRotation = Quaternion.identity;
        billboardTerrrain = new GameObject("billboard");
        billboardTerrrain.transform.SetParent(mainTerrain.transform);
        billboardTerrrain.transform.localPosition = Vector3.zero;
        billboardTerrrain.transform.localRotation = Quaternion.identity;

        npcs = new GameObject("npc");
        npcs.transform.SetParent(transform);
        npcs.transform.localPosition = Vector3.zero;
        npcs.transform.localRotation = Quaternion.identity;
        party = new GameObject("party");
        party.transform.localPosition = new Vector3(0.0f, 0.0f, -0.02f);// move it out a bit so it overlaps horses, chests etc.
        party.transform.localRotation = Quaternion.identity;
        party.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        fighters = new GameObject("fighters");
        fighters.transform.SetParent(transform);
        fighters.transform.localPosition = Vector3.zero;
        fighters.transform.localRotation = Quaternion.identity;
        characters = new GameObject("characters");
        characters.transform.SetParent(transform);
        characters.transform.localPosition = Vector3.zero;
        characters.transform.localRotation = Quaternion.identity;
        hits = new GameObject("hits");
        hits.transform.SetParent(transform);
        hits.transform.localPosition = Vector3.zero;
        hits.transform.localRotation = Quaternion.identity;
        moongate = new GameObject("moongate");
        moongate.transform.SetParent(transform);
        moongate.transform.localPosition = Vector3.zero;
        moongate.transform.localRotation = Quaternion.identity;
        dungeon = new GameObject("dungeon");
        dungeon.transform.SetParent(transform);
        dungeon.transform.localPosition = Vector3.zero;
        dungeon.transform.localRotation = Quaternion.identity;
        dungeonMonsters = new GameObject("dungeon monsters");
        dungeonMonsters.transform.SetParent(transform);
        dungeonMonsters.transform.localPosition = Vector3.zero;
        dungeonMonsters.transform.localRotation = Quaternion.identity;

        // initialize the palette and load the tiles
        Palette.InitializeEGAPalette();
        Palette.InitializeCGAPalette();
        Palette.InitializeApple2Palette();
        LoadTilesEGA();
        //LoadTilesCGA();
        //LoadTilesApple2();
        //LoadTilesPNG();

        // fix a tile
        FixMageTile3();

        // expand the tiles
        ExpandTiles();

        // create texture atlas
        CreateLinearTextureAtlas(ref originalTiles);
        CreateSquareTextureAtlas(ref originalTiles);
        CreateExpandedTextureAtlas(ref expandedTiles);

        // get the font
        GameFont.LoadCharSetEGA();
        //LoadCharSetCGA();
        GameFont.ImportFontFromTexture(myFont, myTransparentFont, GameFont.fontAtlas, GameFont.fontTransparentAtlas);

        // set all the text objects to myFont in the input panel
        Text[] text = InputPanel.GetComponentsInChildren<Text>(true);
        foreach (Text t in text)
        {
            t.font = myFont;
        }

        // set all the text objects to myFont in the stats panel
        text = StatsPanel.GetComponentsInChildren<Text>(true);
        foreach (Text t in text)
        {
            t.font = myFont;
        }

        // set all the text objects to myFont in the game text panel
        text = TextPanel.GetComponentsInChildren<Text>(true);
        foreach (Text t in text)
        {
            t.font = myFont;
        }

        // set again all the button text objects in the input panel to myTransparentFont
        Button[] buttons = InputPanel.GetComponentsInChildren<Button>(true);

        foreach (Button b in buttons)
        {
            Text[] texts = b.GetComponentsInChildren<Text>(true);
            foreach (Text t in texts)
            {
                t.font = myTransparentFont;
            }
        }

        // load the entire world map
        LoadWorldMap();

        //load all settlements
        LoadSettlements();

        // load all dungeons
        LoadDungeons();

        // create the part game object
        CreateParty();

        // Create the combat terrains
        CreateCombatTerrains();

        // get a reference to the game engine
        u4 = FindObjectOfType<U4_Decompiled_AVATAR>();

        // initialize hidden map
        hiddenWorldMapGameObject = new GameObject("Hidden World Map");
        CreateMapSubsetPass2(hiddenWorldMapGameObject, ref entireMapTILEs, ref entireMapGameObjects);

        GameObject hiddenSettlementsMaps = new GameObject("Hidden Settlements Maps");
        for (int i = 0; i < (int)SETTLEMENT.MAX; i++)
        {
            GameObject settlementGameObject = new GameObject(((SETTLEMENT)i).ToString());
            settlementGameObject.transform.SetParent(hiddenSettlementsMaps.transform);
            settlementsMapGameObjects[i] = new GameObject[32, 32];
            CreateMapSubsetPass2(settlementGameObject, ref settlementMap[i], ref settlementsMapGameObjects[i], true);
            //CreateMapLabels(settlementGameObject, ref settlementMap[i]);
        }

        // set the vision to blank
        vision.sprite = null;
        vision.color = new Color(0f, 0f, 0f, 0f);

        // allocate vision texture that we can overlap pictures onto
        visionTexture = new Texture2D(320, 200);
        ClearTexture(visionTexture, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BLACK]);

        // everything I need it now loaded, start the game engine thread
        u4.StartThread();

        //GameObject dungeonExpandedLevelGameObject = CreateDungeonExpandedLevel(DUNGEONS.HYTHLOTH, 4);

        // Some test stuff, can commented out as needed

        picture1 = new Texture2D[(int)PICTURE.MAX];
        picture2 = new Texture2D[(int)PICTURE.MAX];

        picture3 = new Texture2D[(int)PICTURE2.MAX];
        picture4 = new Texture2D[(int)PICTURE2.MAX];


        for (int i = 0; i < (int)PICTURE.MAX; i++)
        {
            picture1[i] = new Texture2D(320, 200);
            ClearTexture(picture1[i], Palette.CGAColorPalette[(int)Palette.CGA_COLOR.BLACK]);
            LoadAVATARPicFile(((PICTURE)i).ToString() + ".PIC", picture1[i]);
            picture2[i] = new Texture2D(320, 200);
            ClearTexture(picture2[i], Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BLACK]);
            LoadAVATAREGAFile(((PICTURE)i).ToString() + ".EGA", picture2[i]);
        }

        for (int i = (int)PICTURE.TRUTH; i <= (int)PICTURE.HUMILITY; i++)
        {
            LoadAVATARPicFile(((PICTURE)i).ToString() + ".PIC", picture1[(int)PICTURE.TRUTH]);
        }

        for (int i = (int)PICTURE.TRUTH; i <= (int)PICTURE.HUMILITY; i++)
        {
            LoadAVATAREGAFile(((PICTURE)i).ToString() + ".EGA", picture2[(int)PICTURE.TRUTH]);
        }

        for (int i = 0; i < (int)PICTURE2.MAX; i++)
        {
            picture3[i] = new Texture2D(320, 200);
            ClearTexture(picture3[i], Palette.CGAColorPalette[(int)Palette.CGA_COLOR.BLACK]);
            LoadTITLEPicPictureFile(((PICTURE2)i).ToString() + ".PIC", picture3[i]);
            picture4[i] = new Texture2D(320, 200);
            ClearTexture(picture4[i], Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BLACK]);
            LoadTITLEEGAPictureFile(((PICTURE2)i).ToString() + ".EGA", picture4[i]);
        }



        //GameObject dungeonsRoomsGameObject = new GameObject("Dungeon Rooms");
        //CreateDungeonRooms(dungeonsRoomsGameObject);

        /*
        //GameObject dungeonsGameObject = new GameObject("Dungeons");
        //CreateDungeons(dungeonsGameObject);
        */

        // create all the dungeons and all the levels, this will take a while so be prepared to wait, but it is cool and worth the wait
        /*
        for (int dungeon = 0; dungeon < (int)DUNGEONS.MAX; dungeon++)
        {
            for (int level = 0; level < 8; level++)
            {
                GameObject dungeonExpandedLevelGameObject = CreateDungeonExpandedLevel((DUNGEONS)dungeon, level);
                dungeonExpandedLevelGameObject.transform.position = new Vector3(dungeon * 100, -level * 10, 0);
            }
        }
        */

        //GameObject dungeonExpandedLevelGameObject = CreateDungeonExpandedLevel(DUNGEONS.DESPISE, 0);

        //GameObject dr = CreateDungeonRoom(ref dungeons[(int)DUNGEONS.WRONG].dungeonRooms[5]);

        /*
        GameObject wedge = CreateWedge();
        Renderer renderer = wedge.GetComponent<Renderer>();
        renderer.material.mainTexture = expandedTiles[(int)U4_Decompiled.TILE.DIAGONAL_WATER_ARCHITECTURE4];
        renderer.material.mainTextureOffset = new Vector2((float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
        renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);

        GameObject wedge2 = CreateWedge();
        renderer = wedge2.GetComponent<Renderer>();
        renderer.material.mainTexture = expandedTiles[(int)U4_Decompiled.TILE.DIAGONAL_WATER_ARCHITECTURE1];
        renderer.material.mainTextureOffset = new Vector2((float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
        renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);
        
        GameObject wedge3 = CreateWedge();
        renderer = wedge3.GetComponent<Renderer>();
        renderer.material.mainTexture = expandedTiles[(int)U4_Decompiled.TILE.WOOD_FLOOR];
        renderer.material.mainTextureOffset = new Vector2((float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
        renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);
    */
    }


    void AnimateFlags()
    {
        //animate flags
        // TODO there is probably a better way to do this than mucking around
        // with the 2d texture pixels like the original game did but it will work for now.
        const int flagSpeed = 25;

        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 6;
            const int swap_x2 = 8;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.CASTLE_ENTRANCE;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 6;
            const int swap_x2 = 8;
            const int swap_y1 = 4;
            const int swap_y2 = 5;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.TOWN;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 10;
            const int swap_x2 = 12;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.CASTLE;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 6;
            const int swap_x2 = 8;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.SHIP_WEST;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 7;
            const int swap_x2 = 9;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.SHIP_EAST;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }

        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 6;
            const int swap_x2 = 8;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.SHIP_WEST;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 7;
            const int swap_x2 = 9;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.SHIP_EAST;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }

        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 6;
            const int swap_x2 = 8;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.SHIP_WEST;
            Texture2D tex = originalTiles[(int)tileIndex];
            Color color1 = tex.GetPixel(swap_x1, originalTileHeight - 1 - swap_y2);
            Color color2 = tex.GetPixel(swap_x1, originalTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x1, originalTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x1, originalTileHeight - 1 - swap_y2, color2);
            color1 = tex.GetPixel(swap_x2, originalTileHeight - 1 - swap_y2);
            color2 = tex.GetPixel(swap_x2, originalTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x2, originalTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x2, originalTileHeight - 1 - swap_y2, color2);
            tex.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 7;
            const int swap_x2 = 9;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.SHIP_EAST;
            Texture2D tex = originalTiles[(int)tileIndex];
            Color color1 = tex.GetPixel(swap_x1, originalTileHeight - 1 - swap_y2);
            Color color2 = tex.GetPixel(swap_x1, originalTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x1, originalTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x1, originalTileHeight - 1 - swap_y2, color2);
            color1 = tex.GetPixel(swap_x2, originalTileHeight - 1 - swap_y2);
            color2 = tex.GetPixel(swap_x2, originalTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x2, originalTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x2, originalTileHeight - 1 - swap_y2, color2);
            tex.Apply();
        }

        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 6 + TILE_BORDER_SIZE;
            const int swap_x2 = 8 + TILE_BORDER_SIZE;
            const int swap_y1 = 2 + TILE_BORDER_SIZE;
            const int swap_y2 = 3 + TILE_BORDER_SIZE;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.SHIP_WEST;
            Texture2D tex = expandedTiles[(int)tileIndex];
            Color color1 = tex.GetPixel(swap_x1, expandedTileHeight - 1 - swap_y2);
            Color color2 = tex.GetPixel(swap_x1, expandedTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x1, expandedTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x1, expandedTileHeight - 1 - swap_y2, color2);
            color1 = tex.GetPixel(swap_x2, expandedTileHeight - 1 - swap_y2);
            color2 = tex.GetPixel(swap_x2, expandedTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x2, expandedTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x2, expandedTileHeight - 1 - swap_y2, color2);
            tex.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 7 + TILE_BORDER_SIZE;
            const int swap_x2 = 9 + TILE_BORDER_SIZE;
            const int swap_y1 = 2 + TILE_BORDER_SIZE;
            const int swap_y2 = 3 + TILE_BORDER_SIZE;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.SHIP_EAST;
            Texture2D tex = expandedTiles[(int)tileIndex];
            Color color1 = tex.GetPixel(swap_x1, expandedTileHeight - 1 - swap_y2);
            Color color2 = tex.GetPixel(swap_x1, expandedTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x1, expandedTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x1, expandedTileHeight - 1 - swap_y2, color2);
            color1 = tex.GetPixel(swap_x2, expandedTileHeight - 1 - swap_y2);
            color2 = tex.GetPixel(swap_x2, expandedTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x2, expandedTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x2, expandedTileHeight - 1 - swap_y2, color2);
            tex.Apply();
        }

        //if (Random.Range(0, 100) <= flagSpeed)
        {
            const int x1 = 3;
            const int x2 = 9;
            const int y1 = 9;
            const int y2 = 16;
            U4_Decompiled_AVATAR.TILE tileIndex = U4_Decompiled_AVATAR.TILE.COOKING_FIRE;

            int offset_x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth + TILE_BORDER_SIZE;
            int offset_y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight + TILE_BORDER_SIZE;


            Color alpha = new Color(0, 0, 0, 0);
            //Palette.EGAColorPalette[(int)EGA_COLOR.BLACK];

            for (int y = y1; y <= y2; y++)
            {
                for (int x = x1; x <= x2; x++)
                {
                    Color color = combinedExpandedTexture.GetPixel(offset_x + x, offset_y + expandedTileHeight - 1 - y);
                    if ((color == Palette.EGAColorPalette[(int)Palette.EGA_COLOR.RED]) || (color == alpha))
                    {
                        if (Random.Range(0, 100) <= 50)
                        {
                            combinedExpandedTexture.SetPixel(offset_x + x, offset_y + expandedTileHeight - 1 - y, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.RED]);
                        }
                        else
                        {
                            combinedExpandedTexture.SetPixel(offset_x + x, offset_y + expandedTileHeight - 1 - y, alpha);
                        }
                    }
                }
            }
            combinedExpandedTexture.Apply();
        }
    }

    // Update is called once per frame
    float timer = 0.0f;
    float timerExpired = 0.0f;
    public float timerPeriod = 0.02f;

    // used for a flag animation timer
    float flagTimer = 0.0f;
    float flagTimerExpired = 0.0f;
    public float flagTimerPeriod = 0.10f;

    GameObject hiddenWorldMapGameObject;

    // Update is called once per frame
    void Update()
    {
        // update the timer
        flagTimer += Time.deltaTime;

        // only update periodically
        if (flagTimer > flagTimerExpired)
        {
            // reset the expired timer
            flagTimer -= flagTimerExpired;
            flagTimerExpired = flagTimerPeriod;
            if (textureExpandedAtlasPowerOf2 != 0)
            {
                AnimateFlags();
            }
        }

        // update the timer
        timer += Time.deltaTime;

        // only update periodically
        if (timer > timerExpired)
        {
            // reset the expired timer
            timer -= timerExpired;
            timerExpired = timerPeriod;

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.CITIZEN_WORD)
            {
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkCitizen.SetActive(true);

                bool keyword1found = false;
                bool keyword2found = false;

                SETTLEMENT settlement;
                if ((u4.Party._loc == U4_Decompiled_AVATAR.LOCATIONS.BRITANNIA) && (u4.tMap32x32[3, 3] == U4_Decompiled_AVATAR.TILE.LADDER_UP))
                {
                    settlement = SETTLEMENT.LCB_1;
                }
                else
                {
                    settlement = (SETTLEMENT)u4.Party._loc;
                }

                foreach (string word in u4.wordList)
                {
                    // only add the special keywords if we already know them
                    // TODO don't need to do this so often, only when we get new text
                    // TODO need to clear npcTalkIndex when switching levels or settlements as the index might not be valid for the other location
                    if (word.Length >= 4)
                    {
                        string lower = word.ToLower();
                        //Debug.Log(lower);
                        string sub = lower.Substring(0, 4);
                        //Debug.Log(sub);
                        if (sub ==
                            settlementNPCs[(int)settlement][(int)u4.npcTalkIndex].strings[(int)NPC_STRING_INDEX.KEYWORD1].ToLower().Substring(0, 4))
                        {
                            u4.keyword1 = lower;
                            lower = char.ToUpper(lower[0]) + lower.Substring(1, lower.Length - 1);
                            keyword1ButtonText.text = lower;
                            keyword1found = true;
                            keyword1Button.SetActive(true);
                        }
                        if (sub ==
                            settlementNPCs[(int)settlement][(int)u4.npcTalkIndex].strings[(int)NPC_STRING_INDEX.KEYWORD2].ToLower().Substring(0, 4))
                        {
                            u4.keyword2 = lower;
                            lower = char.ToUpper(lower[0]) + lower.Substring(1, lower.Length - 1);
                            keyword2ButtonText.text = lower;
                            keyword2found = true;
                            keyword2Button.SetActive(true);
                        }
                    }

                    if (keyword1found == false)
                    {
                        u4.keyword1 = "";
                        keyword1ButtonText.text = "";
                        keyword1Button.SetActive(false);
                    }

                    if (keyword2found == false)
                    {
                        u4.keyword2 = "";
                        keyword2ButtonText.text = "";
                        keyword2Button.SetActive(false);
                    }
                }
            }
            else
            {
                TalkCitizen.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_YES_NO)
            {
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkYN.SetActive(true);
            }
            else
            {
                TalkYN.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_YES_NO_WORD)
            {
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkYesNo.SetActive(true);
            }
            else
            {
                TalkYesNo.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ASK_LETTER_HEALER)
            {
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkHealer.SetActive(true);
            }
            else
            {
                TalkHealer.SetActive(false);
            }

            if ((u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_CONTINUE) ||
                (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.DELAY_CONTINUE) ||
                (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.DELAY_NO_CONTINUE))
            {
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkContinue.SetActive(true);
                if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.DELAY_CONTINUE)
                {
                    TalkContinueButton.gameObject.SetActive(true);
                }
                else if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.DELAY_NO_CONTINUE)
                {
                    TalkContinueButton.gameObject.SetActive(false);

                    if (vision.sprite == null)
                    {
                        // no continue button and nothing to display so just disable the panel entirely
                        InputPanel.SetActive(false);
                    }
                }
                else
                {
                    TalkContinueButton.gameObject.SetActive(true);
                }
            }
            else
            {
                TalkContinue.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.MAIN_LOOP)
            {
                InputPanel.SetActive(true);
                Action.SetActive(true);
                Talk.SetActive(false);
                ActionMainLoop.SetActive(true);
            }
            else
            {
                ActionMainLoop.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.DUNGEON_LOOP)
            {
                InputPanel.SetActive(true);
                Action.SetActive(true);
                Talk.SetActive(false);
                ActionDungeonLoop.SetActive(true);
            }
            else
            {
                ActionDungeonLoop.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.HAWKWIND_WORD)
            {
                InputPanel.SetActive(true);
                // TODO: need to filter buttons like citizen talk with word list
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkHawWind.SetActive(true);
            }
            else
            {
                TalkHawWind.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ASK_LETTER_FOOD_OR_ALE)
            {
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkFoodAle.SetActive(true);
            }
            else
            {
                TalkFoodAle.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_BUY_SELL)
            {
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkBuySell.SetActive(true);
            }
            else
            {
                TalkBuySell.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_ASK_CHARACTER_NUMBER)
            {
                InputPanel.SetActive(true);
                Talk.SetActive(false);
                Action.SetActive(false);
                TalkPartyCharacter.SetActive(true);
            }
            else
            {
                TalkPartyCharacter.SetActive(false);
            }



            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.PUB_WORD)
            {
                /* TODO search for these words
                "black stone",
	            "sextant",
	            "white stone",
	            "mandrake",
	            "skull",
	            "nightshade",
	            "mandrake root"
                "nothing"
                */
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkPubWord.SetActive(true);
            }
            else
            {
                TalkPubWord.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.MANTRA_WORD)
            {
                /*
                "ahm", Honesty
                "mu", Compassion
                "ra", Valor
                "beh", Justice
                "cah", Sacrifice
                "summ", Honor
                "om", Spirituality
                "lum" Humility
                */

                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkMantras.SetActive(true);
            }
            else
            {
                TalkMantras.SetActive(false);
            }
            
            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.LOAD_BRITISH_WORD)
            {
                /*
                char* D_6FF0[28] = {
                "bye",
                "help",
                "health",
                "name",
                "look",
                "job",
                "truth",
                "love",
                "courage",
                "honesty",
                "compassion",
                "valor",
                "justice",
                "sacrifice",
                "honor",
                "spirituality",
                "humility",
                "pride",
                "avatar",
                "quest",
                "britannia",
                "ankh",
                "abyss",
                "mondain",
                "minax",
                "exodus",
                "virtue",
                ""
                };
                */

                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkLordBritish.SetActive(true);
            }
            else
            {
                TalkLordBritish.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.VIRTUE_WORD)
            {
                /*
                Honesty
                Compassion
                Valor
                Justice
                Sacrifice
                Honor
                Spirituality
                Humility
                */

                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkVirtue.SetActive(true);
            }
            else
            {
                TalkVirtue.SetActive(false);
            }
            
            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.END_GAME_WORD)
            {
                /*
                Honesty
                Compassion
                Valor
                Justice
                Sacrifice
                Honor
                Spirituality
                Humility
                Love
                Truth
                Courage
                */

                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkEndGame.SetActive(true);
            }
            else
            {
                TalkEndGame.SetActive(false);
            }
            

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.USE_ITEM_WORD)
            {
                /*
                "stone"
                "stones"
                "bell"
                "book"
                "candle",
                "key"
                "keys"
                "horn"
                "wheel"
                "skull"
                */

                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkUseItem.SetActive(true);
            }
            else
            {
                TalkUseItem.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.USE_STONE_COLOR_WORD)
            {
                /*
                "Blue",
                "Yellow",
                "Red",
                "Green",
                "Orange",
                "Purple",
                "White",
                "Black"
                */

                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkColors.SetActive(true);
            }
            else
            {
                TalkColors.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_NUMBER_INPUT_2_DIGITS)
            {
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                Talk2DigitInput.SetActive(true);
            }
            else
            {
                Talk2DigitInput.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_NUMBER_INPUT_3_DIGITS)
            {
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                Talk3DigitInput.SetActive(true);
            }
            else
            {
                Talk3DigitInput.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.COMBAT_LOOP)
            {
                InputPanel.SetActive(true);
                Action.SetActive(true);
                Talk.SetActive(false);
                ActionCombatLoop.SetActive(true);
            }
            else
            {
                ActionCombatLoop.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ASK_LETTER_WEAPON)
            {
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkWeapon.SetActive(true);
            }
            else
            {
                TalkWeapon.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ASK_LETTER_ARMOR)
            {
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkArmor.SetActive(true);
            }
            else
            {
                TalkArmor.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ASK_LETTER_GUILD)
            {
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkGuild.SetActive(true);
            }
            else
            {
                TalkGuild.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ASK_LETTER_REAGENT)
            {
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkReagents.SetActive(true);
            }
            else
            {
                TalkReagents.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ASK_LETTER_SPELL)
            {
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkSpells.SetActive(true);
            }
            else
            {
                TalkSpells.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_NUMBER_INPUT_1_DIGITS)
            {
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                Talk1DigitInput.SetActive(true);
            }
            else
            {
                Talk1DigitInput.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ENERGY_TYPE_POISON_FIRE_LIGHTNING_SLEEP)
            {
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkEnergy.SetActive(true);
            }
            else
            {
                TalkEnergy.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ASK_LETTER_TELESCOPE)
            {
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkTelescope.SetActive(true);
            }
            else
            {
                TalkTelescope.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ASK_LETTER_PHASE)
            {
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkPhase.SetActive(true);
            }
            else
            {
                TalkPhase.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_NUMBER_INPUT_0_1_2_3)
            {
                InputPanel.SetActive(true);
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkDigit0123.SetActive(true);
            }
            else
            {
                TalkDigit0123.SetActive(false);
            }

            // disable this for now
            // TODO if this is going to be an actual input panel it needs to support
            // rotation and direction like the controller do
            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_DIRECTION)
            {
                InputPanel.SetActive(false);
                Talk.SetActive(false);
                Action.SetActive(false);
                TalkDirection.SetActive(false);
            }
            else
            {
                TalkDirection.SetActive(false);
            }

            // did we just change modes
            if (lastMode != u4.current_mode)
            {
                // did we just come out of somewhere to the outdoors
                if (u4.current_mode == U4_Decompiled_AVATAR.MODE.OUTDOORS)
                {
                    // flag that we were just inside
                    wasJustInside = true;
                }

                // update last mode
                lastMode = u4.current_mode;
            }

            if (u4.current_mode == U4_Decompiled_AVATAR.MODE.OUTDOORS)
            {
                AddNPCs(u4._npc);
                AddMoongate();
                AddHits(u4.currentHits);
                AddActiveCharacter(u4.currentActiveCharacter);
                followWorld(partyGameObject);
                terrain.SetActive(true);
                animatedTerrrain.SetActive(true);
                billboardTerrrain.SetActive(true);
                fighters.SetActive(false);
                characters.SetActive(false);
                npcs.SetActive(true);
                party.SetActive(true);
                moongate.SetActive(true);
                dungeon.SetActive(false);
                dungeonMonsters.SetActive(false);
                skyGameObject.SetActive(true);

                for (int i = 0; i < (int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX; i++)
                {
                    CombatTerrains[i].gameObject.SetActive(false);
                }

                // automatically enter things when you are on an enterable tile unless just left somewhere or you are flying in the balloon
                if ((readyToAutomaticallyEnter == true) && (u4.Party.f_1dc == 0) &&
                    ((u4.current_tile == U4_Decompiled_AVATAR.TILE.CASTLE_ENTRANCE) ||
                    (u4.current_tile == U4_Decompiled_AVATAR.TILE.CASTLE) ||
                    (u4.current_tile == U4_Decompiled_AVATAR.TILE.TOWN) ||
                    (u4.current_tile == U4_Decompiled_AVATAR.TILE.VILLAGE) ||
                    (u4.current_tile == U4_Decompiled_AVATAR.TILE.DUNGEON) ||
                    (u4.current_tile == U4_Decompiled_AVATAR.TILE.RUINS) ||
                    (u4.current_tile == U4_Decompiled_AVATAR.TILE.SHRINE)))
                {
                    u4.CommandEnter();
                    readyToAutomaticallyEnter = false;
                }

                // wait until we move off of an entrance tile after leaving somewhere
                if ((wasJustInside == true) &&
                    (u4.current_tile != U4_Decompiled_AVATAR.TILE.CASTLE_ENTRANCE) &&
                    (u4.current_tile != U4_Decompiled_AVATAR.TILE.CASTLE) &&
                    (u4.current_tile != U4_Decompiled_AVATAR.TILE.TOWN) &&
                    (u4.current_tile != U4_Decompiled_AVATAR.TILE.VILLAGE) &&
                    (u4.current_tile != U4_Decompiled_AVATAR.TILE.DUNGEON) &&
                    (u4.current_tile != U4_Decompiled_AVATAR.TILE.RUINS) &&
                    (u4.current_tile!= U4_Decompiled_AVATAR.TILE.SHRINE))
                {
                    readyToAutomaticallyEnter = true;
                    wasJustInside = false;
                }

                // automatically board horse, ship and balloon
                if (((u4.current_tile == U4_Decompiled_AVATAR.TILE.HORSE_EAST) ||
                    (u4.current_tile == U4_Decompiled_AVATAR.TILE.HORSE_EAST) ||
                    (u4.current_tile == U4_Decompiled_AVATAR.TILE.SHIP_EAST) ||
                    (u4.current_tile == U4_Decompiled_AVATAR.TILE.SHIP_NORTH) ||
                    (u4.current_tile == U4_Decompiled_AVATAR.TILE.SHIP_WEST) ||
                    (u4.current_tile == U4_Decompiled_AVATAR.TILE.SHIP_SOUTH) ||
                    (u4.current_tile == U4_Decompiled_AVATAR.TILE.BALOON)) && 
                    (lastCurrentTile != U4_Decompiled_AVATAR.TILE.HORSE_EAST) &&
                    (lastCurrentTile != U4_Decompiled_AVATAR.TILE.HORSE_EAST) &&
                    (lastCurrentTile != U4_Decompiled_AVATAR.TILE.SHIP_EAST) &&
                    (lastCurrentTile != U4_Decompiled_AVATAR.TILE.SHIP_NORTH) &&
                    (lastCurrentTile != U4_Decompiled_AVATAR.TILE.SHIP_WEST) &&
                    (lastCurrentTile != U4_Decompiled_AVATAR.TILE.SHIP_SOUTH) &&
                    (lastCurrentTile != U4_Decompiled_AVATAR.TILE.BALOON)  && 
                    (u4.lastKeyboardHit != 'X'))
                {
                    u4.CommandBoard();
                }                
                
                // update last tile so we don't get stuck in a loop
                lastCurrentTile = u4.current_tile; 
                
                if (Camera.main.clearFlags != CameraClearFlags.Skybox)
                {
                    Camera.main.clearFlags = CameraClearFlags.Skybox;
                }
            }
            else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.BUILDING)
            {
                AddNPCs(u4._npc);
                AddMoongate();
                AddHits(u4.currentHits);
                AddActiveCharacter(u4.currentActiveCharacter);
                followWorld(partyGameObject);
                terrain.SetActive(true);
                animatedTerrrain.SetActive(true);
                billboardTerrrain.SetActive(true);
                fighters.SetActive(false);
                characters.SetActive(false);
                npcs.SetActive(true);
                party.SetActive(true);
                moongate.SetActive(false);
                dungeon.SetActive(false);
                dungeonMonsters.SetActive(false);
                skyGameObject.SetActive(true);

                for (int i = 0; i < (int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX; i++)
                {
                    CombatTerrains[i].gameObject.SetActive(false);
                }

                // automatic Klimb and Descend ladders
                if ((u4.current_tile == U4_Decompiled_AVATAR.TILE.LADDER_UP) &&
                    (lastCurrentTile != U4_Decompiled_AVATAR.TILE.LADDER_UP) &&
                    (lastCurrentTile != U4_Decompiled_AVATAR.TILE.LADDER_DOWN))
                {
                    u4.CommandKlimb();
                }

                // automatic Klimb and Descend ladders
                if ((u4.current_tile == U4_Decompiled_AVATAR.TILE.LADDER_DOWN) &&
                    (lastCurrentTile != U4_Decompiled_AVATAR.TILE.LADDER_UP) &&
                    (lastCurrentTile != U4_Decompiled_AVATAR.TILE.LADDER_DOWN))
                {
                    u4.CommandDecsend();
                }

                // update last tile so we don't get stuck in a loop
                lastCurrentTile = u4.current_tile;

                if (Camera.main.clearFlags != CameraClearFlags.Skybox)
                {
                    Camera.main.clearFlags = CameraClearFlags.Skybox;
                }
            }
            else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.COMBAT)
            {
                if ((u4.Party._loc >= U4_Decompiled_AVATAR.LOCATIONS.DECEIT) && (u4.Party._loc <= U4_Decompiled_AVATAR.LOCATIONS.THE_GREAT_STYGIAN_ABYSS))
                {
                    AddFighters(u4.Fighters, u4.Combat1, u4.Party._x * 11, -255 + (7 - u4.Party._y + 1) * 11 - 1);
                    AddCharacters(u4.Combat2, u4.Party, u4.Fighters, u4.Party._x * 11, -255 + (7 - u4.Party._y + 1) * 11 - 1);
                    AddHits(u4.currentHits, u4.Party._x * 11, -255 + (7 - u4.Party._y + 1) * 11 - 1);
                    AddActiveCharacter(u4.currentActiveCharacter, u4.Party._x * 11, -255 + (7 - u4.Party._y + 1) * 11 - 1);
                    followWorld(activeCharacter);
                    terrain.SetActive(false);
                    animatedTerrrain.SetActive(false);
                    billboardTerrrain.SetActive(false);
                    fighters.SetActive(true);
                    characters.SetActive(true);
                    npcs.SetActive(false);
                    party.SetActive(false);
                    moongate.SetActive(false);
                    dungeonMonsters.SetActive(false);
                    skyGameObject.SetActive(false);

                    // check if we have the dungeon already created, create it if not
                    DUNGEONS dun = (DUNGEONS)((int)u4.Party._loc - (int)U4_Decompiled_AVATAR.LOCATIONS.DUNGEONS);
                    if (dungeon.name != dun.ToString() + " Level #" + u4.Party._z)
                    {
                        Destroy(dungeon);
                        dungeon = CreateDungeonExpandedLevel(dun, u4.Party._z);
                    }
                    dungeon.SetActive(true);

                    for (int i = 0; i < (int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX; i++)
                    {
                        CombatTerrains[i].gameObject.SetActive(false);
                    }

                    if (Camera.main.clearFlags != CameraClearFlags.SolidColor)
                    {
                        Camera.main.clearFlags = CameraClearFlags.SolidColor;
                        Camera.main.backgroundColor = Color.black;
                    }
                }
                else
                {
                    AddFighters(u4.Fighters, u4.Combat1);
                    AddCharacters(u4.Combat2, u4.Party, u4.Fighters);
                    AddHits(u4.currentHits);
                    AddActiveCharacter(u4.currentActiveCharacter);
                    followWorld(activeCharacter);
                    terrain.SetActive(false);
                    animatedTerrrain.SetActive(false);
                    billboardTerrrain.SetActive(false);
                    fighters.SetActive(true);
                    characters.SetActive(true);
                    npcs.SetActive(false);
                    party.SetActive(false);
                    moongate.SetActive(false);
                    dungeon.SetActive(false);
                    dungeonMonsters.SetActive(false);
                    skyGameObject.SetActive(true);

                    int currentCombatTerrain = (int)Convert_Tile_to_Combat_Terrian(u4.current_tile);

                    for (int i = 0; i < (int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX; i++)
                    {
                        if (i == currentCombatTerrain)
                        {
                            CombatTerrains[i].gameObject.SetActive(true);
                        }
                        else
                        {
                            CombatTerrains[i].gameObject.SetActive(false);
                        }
                    }

                    if (Camera.main.clearFlags != CameraClearFlags.Skybox)
                    {
                        Camera.main.clearFlags = CameraClearFlags.Skybox;
                    }
                }
            }
            else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.COMBAT_ROOM) /* this is a dungeon room */
            {
                AddFighters(u4.Fighters, u4.Combat1, u4.Party._x * 11, -255 + (7 - u4.Party._y + 1) * 11 - 1);
                AddCharacters(u4.Combat2, u4.Party, u4.Fighters, u4.Party._x * 11, -255 + (7 - u4.Party._y + 1) * 11 - 1);
                AddHits(u4.currentHits, u4.Party._x * 11, -255 + (7 - u4.Party._y + 1) * 11 - 1);
                AddActiveCharacter(u4.currentActiveCharacter, u4.Party._x * 11, -255 + (7 - u4.Party._y + 1) * 11 - 1);
                followWorld(activeCharacter);
                terrain.SetActive(false);
                animatedTerrrain.SetActive(false);
                billboardTerrrain.SetActive(false);
                fighters.SetActive(true);
                characters.SetActive(true);
                npcs.SetActive(false);
                party.SetActive(false);
                moongate.SetActive(false);
                dungeonMonsters.SetActive(false);
                skyGameObject.SetActive(false);

                // check if we have the dungeon already created, create it if not
                DUNGEONS dun = (DUNGEONS)((int)u4.Party._loc - (int)U4_Decompiled_AVATAR.LOCATIONS.DUNGEONS);
                if (dungeon.name != dun.ToString() + " Level #" + u4.Party._z)
                {
                    Destroy(dungeon);
                    dungeon = CreateDungeonExpandedLevel(dun, u4.Party._z);
                }
                dungeon.SetActive(true);

                for (int i = 0; i < (int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX; i++)
                {
                    CombatTerrains[i].gameObject.SetActive(false);
                }

                if (Camera.main.clearFlags != CameraClearFlags.SolidColor)
                {
                    Camera.main.clearFlags = CameraClearFlags.SolidColor;
                    Camera.main.backgroundColor = Color.black;
                }
            }
            else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.DUNGEON)
            {
                AddNPCs(u4._npc);
                AddHits(u4.currentHits);
                AddActiveCharacter(u4.currentActiveCharacter);
                AddDungeonMapMonsters();
                followWorld(partyGameObject);
                terrain.SetActive(false);
                animatedTerrrain.SetActive(false);
                billboardTerrrain.SetActive(false);
                fighters.SetActive(false);
                characters.SetActive(false);
                npcs.SetActive(false);
                party.SetActive(true);
                moongate.SetActive(false);
                skyGameObject.SetActive(false);

                // check if we have the dungeon already created, create it if not
                DUNGEONS dun = (DUNGEONS)((int)u4.Party._loc - (int)U4_Decompiled_AVATAR.LOCATIONS.DUNGEONS);
                if (dungeon.name != dun.ToString() + " Level #" + u4.Party._z)
                {
                    // not the right dungeon, create a new dungeon
                    Destroy(dungeon);
                    dungeon = CreateDungeonExpandedLevel(dun, u4.Party._z);
                }

                if (u4.Party.f_1dc > 0) // torch active
                {
                    dungeon.SetActive(true);
                    dungeonMonsters.SetActive(true);
                }
                else
                {
                    dungeon.SetActive(false);
                    dungeonMonsters.SetActive(false);
                }

                for (int i = 0; i < (int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX; i++)
                {
                    CombatTerrains[i].gameObject.SetActive(false);
                }

                if (Camera.main.clearFlags != CameraClearFlags.SolidColor)
                {
                    Camera.main.clearFlags = CameraClearFlags.SolidColor;
                    Camera.main.backgroundColor = Color.black;
                }
            }
            else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.SHRINE)
            {
                AddFighters(u4.Fighters, u4.Combat1);
                AddCharacters(u4.Combat2, u4.Party, u4.Fighters);
                AddHits(u4.currentHits);
                AddActiveCharacter(u4.currentActiveCharacter);
                
                terrain.SetActive(false);
                animatedTerrrain.SetActive(false);
                billboardTerrrain.SetActive(false);
                fighters.SetActive(false);
                characters.SetActive(false);
                npcs.SetActive(false);
                party.SetActive(false);
                moongate.SetActive(false);
                dungeon.SetActive(false);
                dungeonMonsters.SetActive(false);
                skyGameObject.SetActive(true);

                for (int i = 0; i < (int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX; i++)
                {
                    if (i == (int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.SHRINE)
                    {
                        CombatTerrains[i].gameObject.SetActive(true); 
                        followWorld(CenterOfCombatTerrain);
                    }
                    else
                    {
                        CombatTerrains[i].gameObject.SetActive(false);
                    }
                }

                if (Camera.main.clearFlags != CameraClearFlags.Skybox)
                {
                    Camera.main.clearFlags = CameraClearFlags.Skybox;
                }
            } 
            else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.COMBAT_CAMP)
            {
                AddFighters(u4.Fighters, u4.Combat1);
                AddCharacters(u4.Combat2, u4.Party, u4.Fighters);
                AddHits(u4.currentHits);
                AddActiveCharacter(u4.currentActiveCharacter);

                terrain.SetActive(false);
                animatedTerrrain.SetActive(false);
                billboardTerrrain.SetActive(false);
                fighters.SetActive(true);
                characters.SetActive(true);
                npcs.SetActive(false);
                party.SetActive(false);
                moongate.SetActive(false);
                dungeon.SetActive(false);
                dungeonMonsters.SetActive(false);
                followWorld(activeCharacter);

                int currentCombatTerrain;
                // need to special case the combat when in the inn and in combat camp mode outside or in dungeon
                if (u4.current_tile == U4_Decompiled_AVATAR.TILE.BRICK_FLOOR)
                {
                    currentCombatTerrain = (int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.INN;
                    skyGameObject.SetActive(true);
                    if (Camera.main.clearFlags != CameraClearFlags.Skybox)
                    {
                        Camera.main.clearFlags = CameraClearFlags.Skybox;
                    }
                }
                else if ((u4.Party._loc >= U4_Decompiled_AVATAR.LOCATIONS.DECEIT) && (u4.Party._loc <= U4_Decompiled_AVATAR.LOCATIONS.THE_GREAT_STYGIAN_ABYSS))
                {
                    currentCombatTerrain = (int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.CAMP_DNG;
                    skyGameObject.SetActive(false); 
                    if (Camera.main.clearFlags != CameraClearFlags.SolidColor)
                    {
                        Camera.main.clearFlags = CameraClearFlags.SolidColor;
                        Camera.main.backgroundColor = Color.black;
                    }
                }
                else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.DUNGEON)
                {
                    currentCombatTerrain = (int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.CAMP;
                    skyGameObject.SetActive(true);
                    if (Camera.main.clearFlags != CameraClearFlags.Skybox)
                    {
                        Camera.main.clearFlags = CameraClearFlags.Skybox;
                    }
                }
                else
                {
                    currentCombatTerrain = (int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.CAMP;
                    skyGameObject.SetActive(true);
                    if (Camera.main.clearFlags != CameraClearFlags.Skybox)
                    {
                        Camera.main.clearFlags = CameraClearFlags.Skybox;
                    }
                }

                for (int i = 0; i < (int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX; i++)
                {
                    if (i == currentCombatTerrain)
                    {
                        CombatTerrains[i].gameObject.SetActive(true);
                        if (u4.currentActiveCharacter.active)
                        {
                            followWorld(activeCharacter);
                        }
                        else
                        {
                            followWorld(CenterOfCombatTerrain);
                        }
                    }
                    else
                    {
                        CombatTerrains[i].gameObject.SetActive(false);
                    }
                }
            }

            if ((party != null) && (originalTiles != null))
            {
                // set the party tile, person, horse, ballon, ship, etc.
                Renderer renderer = party.GetComponentInChildren<Renderer>();
                if (renderer)
                {
                    party.GetComponentInChildren<Renderer>().material.mainTexture = expandedTiles[(int)u4.Party._tile];
                    party.name = u4.Party._tile.ToString();

                    if ((u4.Party._tile == U4_Decompiled_AVATAR.TILE.BALOON) && (u4.Party.f_1dc == 1))
                    {
                        party.transform.position = new Vector3(party.transform.position.x, 1, party.transform.position.z);
                    }
                    else
                    {
                        party.transform.position = new Vector3(party.transform.position.x, 0, party.transform.position.z);
                    }
                }
            }


            // keep the sky game objects in sync with the game
            if (skyGameObject)
            {
                if ((u4.current_mode == U4_Decompiled_AVATAR.MODE.OUTDOORS) || (u4.current_mode == U4_Decompiled_AVATAR.MODE.BUILDING))
                {
                    skyGameObject.transform.localPosition = new Vector3(u4.Party._x, 0, 255 - u4.Party._y);
                }
                else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.COMBAT)
                {
                    skyGameObject.transform.localPosition = new Vector3(u4.currentActiveCharacter.x, 0, 255 - u4.currentActiveCharacter.y);
                }
                else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.COMBAT_CAMP)
                {
                    if (u4.currentActiveCharacter.active)
                    {
                        skyGameObject.transform.localPosition = new Vector3(u4.currentActiveCharacter.x, 0, 255 - u4.currentActiveCharacter.y);
                    }
                    else
                    {
                        skyGameObject.transform.localPosition = CenterOfCombatTerrain.transform.localPosition;
                    }
                }
            }

            // keep the party game object in sync with the game
            if (partyGameObject)
            {
                if ((u4.current_mode == U4_Decompiled_AVATAR.MODE.OUTDOORS) ||
                    (u4.current_mode == U4_Decompiled_AVATAR.MODE.BUILDING) ||
                    (u4.current_mode == U4_Decompiled_AVATAR.MODE.COMBAT_CAMP))
                {
                    partyGameObject.transform.localPosition = new Vector3(u4.Party._x, 255 - u4.Party._y, 0);
                    if (Camera.main.transform.eulerAngles.y != 0)
                    {
                        if (rotateTransform)
                        {
                            if (u4.surface_party_direction == U4_Decompiled_AVATAR.DIRECTION.WEST && Camera.main.transform.eulerAngles.y != 270)
                            {
                                rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 270, Camera.main.transform.eulerAngles.z);
                            }
                            else if (u4.surface_party_direction == U4_Decompiled_AVATAR.DIRECTION.NORTH && Camera.main.transform.eulerAngles.y != 0)
                            {
                                rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);
                            }
                            else if (u4.surface_party_direction == U4_Decompiled_AVATAR.DIRECTION.EAST && Camera.main.transform.eulerAngles.y != 90)
                            {
                                rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 90, Camera.main.transform.eulerAngles.z);
                            }
                            else if (u4.surface_party_direction == U4_Decompiled_AVATAR.DIRECTION.SOUTH && Camera.main.transform.eulerAngles.y != 180)
                            {
                                rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 180, Camera.main.transform.eulerAngles.z);
                            }
                        }
                        else
                        {
                            rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);
                        }
                    }
                }
                else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.COMBAT)
                {
                    if ((u4.Party._loc >= U4_Decompiled_AVATAR.LOCATIONS.DECEIT) && (u4.Party._loc <= U4_Decompiled_AVATAR.LOCATIONS.THE_GREAT_STYGIAN_ABYSS))
                    {
                        //partyGameObject.transform.localPosition = new Vector3(Party._x * 11 + 5, (7 - Party._y) * 11 + 5, 0);
                        if (Camera.main.transform.eulerAngles.y != 0)
                        {
                            //rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);

                            // if we are going to do rotation then we need to adjust the directional controls when in combat in the dungeon also
                            if (rotateTransform)
                            {
                                if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.WEST && Camera.main.transform.eulerAngles.y != 270)
                                {
                                    rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 270, Camera.main.transform.eulerAngles.z);
                                }
                                else if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.NORTH && Camera.main.transform.eulerAngles.y != 0)
                                {
                                    rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);
                                }
                                else if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.EAST && Camera.main.transform.eulerAngles.y != 90)
                                {
                                    rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 90, Camera.main.transform.eulerAngles.z);
                                }
                                else if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.SOUTH && Camera.main.transform.eulerAngles.y != 180)
                                {
                                    rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 180, Camera.main.transform.eulerAngles.z);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Camera.main.transform.eulerAngles.y != 0)
                        {
                            if (rotateTransform)
                            {
                                if (u4.surface_party_direction == U4_Decompiled_AVATAR.DIRECTION.WEST && Camera.main.transform.eulerAngles.y != 270)
                                {
                                    rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 270, Camera.main.transform.eulerAngles.z);
                                }
                                else if (u4.surface_party_direction == U4_Decompiled_AVATAR.DIRECTION.NORTH && Camera.main.transform.eulerAngles.y != 0)
                                {
                                    rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);
                                }
                                else if (u4.surface_party_direction == U4_Decompiled_AVATAR.DIRECTION.EAST && Camera.main.transform.eulerAngles.y != 90)
                                {
                                    rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 90, Camera.main.transform.eulerAngles.z);
                                }
                                else if (u4.surface_party_direction == U4_Decompiled_AVATAR.DIRECTION.SOUTH && Camera.main.transform.eulerAngles.y != 180)
                                {
                                    rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 180, Camera.main.transform.eulerAngles.z);
                                }
                            }
                            else
                            {
                                rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);
                            }
                        }
                    }
                }
                else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.COMBAT_ROOM)
                {
                    //partyGameObject.transform.localPosition = new Vector3(Party._x * 11 + 5, (7 - Party._y) * 11 + 5, 0);
                    if (Camera.main.transform.eulerAngles.y != 0)
                    {
                        //rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);

                        if (rotateTransform)
                        {
                            if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.WEST && Camera.main.transform.eulerAngles.y != 270)
                            {
                                rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 270, Camera.main.transform.eulerAngles.z);
                            }
                            else if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.NORTH && Camera.main.transform.eulerAngles.y != 0)
                            {
                                rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);
                            }
                            else if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.EAST && Camera.main.transform.eulerAngles.y != 90)
                            {
                                rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 90, Camera.main.transform.eulerAngles.z);
                            }
                            else if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.SOUTH && Camera.main.transform.eulerAngles.y != 180)
                            {
                                rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 180, Camera.main.transform.eulerAngles.z);
                            }
                        }
                    }
                }
                else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.DUNGEON)
                {
                    partyGameObject.transform.localPosition = new Vector3(u4.Party._x * 11 + 5, (7 - u4.Party._y) * 11 + 5, 0);
                    if (rotateTransform)
                    {
                        if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.WEST && Camera.main.transform.eulerAngles.y != 270)
                        {
                            rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 270, Camera.main.transform.eulerAngles.z);
                        }
                        else if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.NORTH && Camera.main.transform.eulerAngles.y != 0)
                        {
                            rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);
                        }
                        else if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.EAST && Camera.main.transform.eulerAngles.y != 90)
                        {
                            rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 90, Camera.main.transform.eulerAngles.z);
                        }
                        else if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.SOUTH && Camera.main.transform.eulerAngles.y != 180)
                        {
                            rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 180, Camera.main.transform.eulerAngles.z);
                        }
                    }
                }

                if (u4.gameText != null && GameText != null)
                {
                    GameText.GetComponent<UnityEngine.UI.Text>().text = u4.gameText;
                }

                if ((u4.current_mode == U4_Decompiled_AVATAR.MODE.DUNGEON) ||
                    ((u4.Party._loc >= U4_Decompiled_AVATAR.LOCATIONS.DECEIT) && (u4.Party._loc <= U4_Decompiled_AVATAR.LOCATIONS.THE_GREAT_STYGIAN_ABYSS)))
                {
                    windDirection.GetComponent<UnityEngine.UI.Text>().text = (char)(0x10) + "Level" + (char)(0x12) + (char)(u4.Party._z + '1') + (char)(0x11);
                }
                else
                {
                    windDirection.GetComponent<UnityEngine.UI.Text>().text = (char)(0x10) + "Wind" + (char)(0x12);

                    switch (u4.WindDir)
                    {
                        case U4_Decompiled_AVATAR.DIRECTION.NORTH:
                            windDirection.GetComponent<UnityEngine.UI.Text>().text += "North" + (char)(0x11);
                            break;
                        case U4_Decompiled_AVATAR.DIRECTION.SOUTH:
                            windDirection.GetComponent<UnityEngine.UI.Text>().text += "South" + (char)(0x11);
                            break;
                        case U4_Decompiled_AVATAR.DIRECTION.EAST:
                            windDirection.GetComponent<UnityEngine.UI.Text>().text += (char)(0x12) + "East" + (char)(0x11);
                            break;
                        case U4_Decompiled_AVATAR.DIRECTION.WEST:
                            windDirection.GetComponent<UnityEngine.UI.Text>().text += (char)(0x12) + "West" + (char)(0x11);
                            break;
                    }
                }
                moons.GetComponent<UnityEngine.UI.Text>().text = "" + (char)(0x10) + (char)(((u4.Party._trammel - 1) & 7) + 0x14) + (char)(0x12) + (char)(((u4.Party._felucca - 1) & 7) + 0x14) + (char)(0x11);

                //trammelLight.GetComponent<Light>().transform.eulerAngles = new Vector3(0f, 180f - (float)u4.Party._trammel * (360f / 8f), 0f);
                //feluccaLight.GetComponent<Light>().transform.eulerAngles = new Vector3(0f, 180f - (float)u4.Party._felucca * (360f / 8f), 0f);
                //trammelLight.GetComponent<Light>().transform.eulerAngles = new Vector3(0f, 180f - (float)u4.D_1665 * (360f / 256f), 0f);
                //feluccaLight.GetComponent<Light>().transform.eulerAngles = new Vector3(0f, 180f - (float)u4.D_1666 * (360f / 256f), 0f);

                trammelLight.GetComponent<Light>().transform.eulerAngles = new Vector3(0f, Mathf.LerpAngle(trammelLight.GetComponent<Light>().transform.eulerAngles.y, 180f - (float)u4.D_1665 * (360f / 256f), Time.deltaTime), 0f);
                feluccaLight.GetComponent<Light>().transform.eulerAngles = new Vector3(0f, Mathf.LerpAngle(feluccaLight.GetComponent<Light>().transform.eulerAngles.y, 180f - (float)u4.D_1666 * (360f / 256f), Time.deltaTime), 0f);
                //sunLight.GetComponent<Light>().transform.eulerAngles = new Vector3(0f, Mathf.LerpAngle(sunLight.GetComponent<Light>().transform.eulerAngles.y, 180f - (float)u4.D_1666 * (360f / 256f), Time.deltaTime), 0f);


                System.Globalization.TextInfo myTI = new System.Globalization.CultureInfo("en-US", false).TextInfo;

                statsOverview.GetComponent<UnityEngine.UI.Text>().text = "" + '\n';

                for (int i = 0; i < 8; i++)
                {
                    if (i < u4.Party.f_1d8)
                    {
                        if (u4.Party.chara[i].highlight)
                        {
                            statsOverview.GetComponent<UnityEngine.UI.Text>().text += highlight((i + 1) + "-" + u4.Party.chara[i].name.PadRight(18, ' ') + u4.Party.chara[i].hitPoint.ToString().PadLeft(4, ' ') + (char)(u4.Party.chara[i].state) + '\n');
                        }
                        else
                        {
                            statsOverview.GetComponent<UnityEngine.UI.Text>().text += (i + 1) + "-" + u4.Party.chara[i].name.PadRight(18, ' ') + u4.Party.chara[i].hitPoint.ToString().PadLeft(4, ' ') + (char)(u4.Party.chara[i].state) + '\n';
                        }
                    }
                    else
                    {
                        statsOverview.GetComponent<UnityEngine.UI.Text>().text += '\n';
                    }
                }

                string bottomStatus = "" + '\n' + ("Food:" + (int)(u4.Party._food / 100)).ToString().PadRight(12, ' ') + (char)(u4.spell_sta);

                if ((u4.Party._tile == U4_Decompiled_AVATAR.TILE.SHIP_EAST) ||
                    (u4.Party._tile == U4_Decompiled_AVATAR.TILE.SHIP_WEST) ||
                    (u4.Party._tile == U4_Decompiled_AVATAR.TILE.SHIP_NORTH) ||
                    (u4.Party._tile == U4_Decompiled_AVATAR.TILE.SHIP_SOUTH))
                {
                    bottomStatus += ("Ship:" + u4.Party._ship).PadLeft(12, ' ');
                }
                else
                {
                    bottomStatus += ("Gold:" + u4.Party._gold).PadLeft(12, ' '); ;
                }

                statsOverview.GetComponent<UnityEngine.UI.Text>().text += bottomStatus;

                for (int i = 0; i < characterStatus.Length; i++)
                {
                    if (i < u4.Party.f_1d8)
                    {
                        int classLength = u4.Party.chara[i].Class.ToString().Length;

                        characterStatus[i].GetComponent<UnityEngine.UI.Text>().text = "" +
                            (char)(0x10) + u4.Party.chara[i].name + (char)(0x11) + '\n' +
                            (char)(u4.Party.chara[i].sex) + myTI.ToTitleCase(u4.Party.chara[i].Class.ToString().ToLower()).PadLeft(12 + classLength / 2, ' ').PadRight(23, ' ') + (char)u4.Party.chara[i].state + '\n' +
                            '\n' +
                            " MP:" + u4.Party.chara[i].magicPoints.ToString().PadLeft(2, '0').PadRight(14, ' ') + "LV:" + ((int)(u4.Party.chara[i].hitPointsMaximum / 100)).ToString().PadRight(4, ' ') + '\n' +
                            "STR:" + u4.Party.chara[i].strength.ToString().PadLeft(2, '0').PadRight(14, ' ') + "HP:" + u4.Party.chara[i].hitPoint.ToString().PadLeft(4, '0') + '\n' +
                            "DEX:" + u4.Party.chara[i].dexterity.ToString().PadLeft(2, '0').PadRight(14, ' ') + "HM:" + u4.Party.chara[i].hitPointsMaximum.ToString().PadLeft(4, '0') + '\n' +
                            "INT:" + u4.Party.chara[i].intelligence.ToString().PadLeft(2, '0').PadRight(14, ' ') + "EX:" + u4.Party.chara[i].experiencePoints.ToString().PadLeft(4, '0') + '\n' +
                            "W:" + myTI.ToTitleCase(u4.Party.chara[i].currentWeapon.ToString().Replace('_', ' ').ToLower()).PadRight(23, ' ') + '\n' +
                            "A:" + myTI.ToTitleCase(u4.Party.chara[i].currentArmor.ToString().Replace('_', ' ').ToLower()).PadRight(23, ' ') + '\n' +
                            bottomStatus;
                    }
                    else
                    {
                        characterStatus[i].GetComponent<UnityEngine.UI.Text>().text = "\n\n\n\n\n\n\n\n" + bottomStatus;
                    }
                }

                weaponsStatus.GetComponent<UnityEngine.UI.Text>().text = "" +
                    (char)(0x10) + "Weapons" + (char)(0x11) + '\n' +
                    "A  -Hands   Cross Bow-I" + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.CROSSBOW].ToString().PadLeft(2, '0') + '\n' +
                    'B' + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.STAFF].ToString().PadLeft(2, '0') + "-Staff Flaming Oil-J" + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.FLAMING_OIL].ToString().PadLeft(2, '0') + '\n' +
                    'C' + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.DAGGER].ToString().PadLeft(2, '0') + "-Dagger    Halbert-K" + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.HALBERD].ToString().PadLeft(2, '0') + '\n' +
                    'D' + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.SLING].ToString().PadLeft(2, '0') + "-Sling   Magic Axe-L" + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.MAGIC_AXE].ToString().PadLeft(2, '0') + '\n' +
                    'E' + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.MACE].ToString().PadLeft(2, '0') + "-Mace  Magic Sword-M" + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.MAGIC_SWORD].ToString().PadLeft(2, '0') + '\n' +
                    'F' + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.AXE].ToString().PadLeft(2, '0') + "-Axe     Magic Bow-N" + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.MAGIC_BOW].ToString().PadLeft(2, '0') + '\n' +
                    'G' + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.SWORD].ToString().PadLeft(2, '0') + "-Sword  Magic Wand-O" + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.MAGIC_WAND].ToString().PadLeft(2, '0') + '\n' +
                    'H' + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.BOW].ToString().PadLeft(2, '0') + "-Bow  Mystic Sword-P" + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.MYSTIC_SWORD].ToString().PadLeft(2, '0') + '\n' +
                    bottomStatus;

                armourStatus.GetComponent<UnityEngine.UI.Text>().text = "" +
                    (char)(0x10) + "Armour" + (char)(0x11) + '\n' +
                    "A  " + "-No Armour".PadRight(22, ' ') + '\n' +
                    'B' + u4.Party._armors[(int)U4_Decompiled_AVATAR.ARMOR.CLOTH].ToString().PadLeft(2, '0') + "-Clothing".PadRight(22, ' ') + '\n' +
                    'C' + u4.Party._armors[(int)U4_Decompiled_AVATAR.ARMOR.LEATHER].ToString().PadLeft(2, '0') + "-Leather".PadRight(22, ' ') + '\n' +
                    'D' + u4.Party._armors[(int)U4_Decompiled_AVATAR.ARMOR.CHAIN_MAIL].ToString().PadLeft(2, '0') + "-Chain Mail".PadRight(22, ' ') + '\n' +
                    'E' + u4.Party._armors[(int)U4_Decompiled_AVATAR.ARMOR.PLATE_MAIL].ToString().PadLeft(2, '0') + "-Plate Mail".PadRight(22, ' ') + '\n' +
                    'F' + u4.Party._armors[(int)U4_Decompiled_AVATAR.ARMOR.MAGIC_CHAIN].ToString().PadLeft(2, '0') + "-Magic Chain Mail".PadRight(22, ' ') + '\n' +
                    'G' + u4.Party._armors[(int)U4_Decompiled_AVATAR.ARMOR.MAGIC_PLATE].ToString().PadLeft(2, '0') + "-Magic Plate Mail".PadRight(22, ' ') + '\n' +
                    'H' + u4.Party._armors[(int)U4_Decompiled_AVATAR.ARMOR.MYSTIC_ROBE].ToString().PadLeft(2, '0') + "-Mystic Robe".PadRight(22, ' ') + '\n' +
                    bottomStatus;

                reagentsStatus.GetComponent<UnityEngine.UI.Text>().text = "" +
                    (char)(0x10) + "Reagents" + (char)(0x11) + '\n' +
                    'A' + u4.Party._reagents[(int)U4_Decompiled_AVATAR.REAGENT.SULFER_ASH].ToString().PadLeft(2, '0') + "-Sulfer Ash".PadRight(22, ' ') + '\n' +
                    'B' + u4.Party._reagents[(int)U4_Decompiled_AVATAR.REAGENT.GINSENG].ToString().PadLeft(2, '0') + "-Ginseng".PadRight(22, ' ') + '\n' +
                    'C' + u4.Party._reagents[(int)U4_Decompiled_AVATAR.REAGENT.GARLIC].ToString().PadLeft(2, '0') + "-Galic".PadRight(22, ' ') + '\n' +
                    'D' + u4.Party._reagents[(int)U4_Decompiled_AVATAR.REAGENT.SPIDER_SILK].ToString().PadLeft(2, '0') + "-Spider Silk".PadRight(22, ' ') + '\n' +
                    'E' + u4.Party._reagents[(int)U4_Decompiled_AVATAR.REAGENT.BLOOD_MOSS].ToString().PadLeft(2, '0') + "-Blood Moss".PadRight(22, ' ') + '\n' +
                    'F' + u4.Party._reagents[(int)U4_Decompiled_AVATAR.REAGENT.BLACK_PEARL].ToString().PadLeft(2, '0') + "-Black Pearl".PadRight(22, ' ') + '\n' +
                    'G' + u4.Party._reagents[(int)U4_Decompiled_AVATAR.REAGENT.NIGHTSHADE].ToString().PadLeft(2, '0') + "-Nightshade".PadRight(22, ' ') + '\n' +
                    'H' + u4.Party._reagents[(int)U4_Decompiled_AVATAR.REAGENT.MANDRAKE].ToString().PadLeft(2, '0') + "-Mandrake Root".PadRight(22, ' ') + '\n' +
                    bottomStatus;

                mixturesStatus.GetComponent<UnityEngine.UI.Text>().text = "" +
                    (char)(0x10) + "Mixtures" + (char)(0x11) + '\n' +
                    "Awak-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.AWAKEN].ToString().PadLeft(2, '0') + " IceBa-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.ICEBALLS].ToString().PadLeft(2, '0') + " Quick-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.QUICKNESS].ToString().PadLeft(2, '0') + '\n' +
                    "Blin-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.BLINK].ToString().PadLeft(2, '0') + "  Jinx-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.JINX].ToString().PadLeft(2, '0') + "  Resu-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.RESURECTION].ToString().PadLeft(2, '0') + '\n' +
                    "Cure-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.CURE].ToString().PadLeft(2, '0') + "  Kill-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.KILL].ToString().PadLeft(2, '0') + " Sleep-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.SLEEP].ToString().PadLeft(2, '0') + '\n' +
                    "Disp-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.DISPELL].ToString().PadLeft(2, '0') + " Light-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.LIGHT].ToString().PadLeft(2, '0') + " Tremo-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.TREMOR].ToString().PadLeft(2, '0') + '\n' +
                    "Eneg-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.ENERGY].ToString().PadLeft(2, '0') + " Missl-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.MAGIC_MISSLE].ToString().PadLeft(2, '0') + " Undea-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.UNDEAD].ToString().PadLeft(2, '0') + '\n' +
                    "Fire-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.FIREBALL].ToString().PadLeft(2, '0') + " Negat-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.NEGATE].ToString().PadLeft(2, '0') + "  View-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.VIEW].ToString().PadLeft(2, '0') + '\n' +
                    "Gate-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.GATE].ToString().PadLeft(2, '0') + "  Open-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.OPEN].ToString().PadLeft(2, '0') + " Winds-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.WINDS].ToString().PadLeft(2, '0') + '\n' +
                    "Heal-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.HEAL].ToString().PadLeft(2, '0') + " Prote-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.PROTECT].ToString().PadLeft(2, '0') + "  X-It-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.X_IT].ToString().PadLeft(2, '0') + '\n' +
                    bottomStatus;

                equipmentStatus.GetComponent<UnityEngine.UI.Text>().text = "" +
                    (char)(0x10) + "Equipment" + (char)(0x11) + '\n' +
                    'A' + u4.Party._torches.ToString().PadLeft(2, '0') + "-Torches".PadRight(22, ' ') + '\n' +
                    'B' + u4.Party._gems.ToString().PadLeft(2, '0') + "-Gems".PadRight(22, ' ') + '\n' +
                    'C' + u4.Party._keys.ToString().PadLeft(2, '0') + "-Keys".PadRight(22, ' ') + '\n' +
                    'D' + u4.Party._sextants.ToString().PadLeft(2, '0') + "-Sextants".PadRight(22, ' ') + "\n\n\n\n\n" +
                    bottomStatus;

                itemsStatus.GetComponent<UnityEngine.UI.Text>().text = "" + '\n' +
                    "Stones: ";

                if (u4.Party.mStones.HasFlag(U4_Decompiled_AVATAR.STONES.BLUE))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "<color=blue>Bl</color>";
                }
                if (u4.Party.mStones.HasFlag(U4_Decompiled_AVATAR.STONES.YELLOW))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "<color=yellow>Ye</color>";
                }
                if (u4.Party.mStones.HasFlag(U4_Decompiled_AVATAR.STONES.RED))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "<color=red>Re</color>";
                }
                if (u4.Party.mStones.HasFlag(U4_Decompiled_AVATAR.STONES.GREEN))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "<color=green>Gr</color>";
                }
                if (u4.Party.mStones.HasFlag(U4_Decompiled_AVATAR.STONES.ORANGE))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "<color=orange>Or</color>";
                }
                if (u4.Party.mStones.HasFlag(U4_Decompiled_AVATAR.STONES.PURPLE))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "<color=purple>Pu</color>";
                }
                if (u4.Party.mStones.HasFlag(U4_Decompiled_AVATAR.STONES.WHITE))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "<color=white>Wh</color>";
                }
                if (u4.Party.mStones.HasFlag(U4_Decompiled_AVATAR.STONES.BLACK))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "<color=grey>Bl</color>";
                }

                itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "" + '\n' +
                    "Runes: ";

                if (u4.Party.mRunes.HasFlag(U4_Decompiled_AVATAR.RUNES.HONOR))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Honor ";
                }
                if (u4.Party.mRunes.HasFlag(U4_Decompiled_AVATAR.RUNES.COMPASSION))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Compassion ";
                }
                if (u4.Party.mRunes.HasFlag(U4_Decompiled_AVATAR.RUNES.VALOR))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Valor ";
                }
                if (u4.Party.mRunes.HasFlag(U4_Decompiled_AVATAR.RUNES.JUSTICE))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Justice ";
                }
                if (u4.Party.mRunes.HasFlag(U4_Decompiled_AVATAR.RUNES.HUMILITY))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Humility ";
                }
                if (u4.Party.mRunes.HasFlag(U4_Decompiled_AVATAR.RUNES.HONESTY))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Honesty ";
                }
                if (u4.Party.mRunes.HasFlag(U4_Decompiled_AVATAR.RUNES.SPIRITUALITY))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Spirituality ";
                }
                if (u4.Party.mRunes.HasFlag(U4_Decompiled_AVATAR.RUNES.SACRIFICE))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Sacrifice";
                }

                itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "" + '\n' +
                   "Items: ";

                if (u4.Party.mItems.HasFlag(U4_Decompiled_AVATAR.ITEMS.BELL))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Bell ";
                }
                if (u4.Party.mItems.HasFlag(U4_Decompiled_AVATAR.ITEMS.BOOK))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Book ";
                }
                if (u4.Party.mItems.HasFlag(U4_Decompiled_AVATAR.ITEMS.WHEEL))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Wheel ";
                }
                if (u4.Party.mItems.HasFlag(U4_Decompiled_AVATAR.ITEMS.HORN))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Horn ";
                }
                if (u4.Party.mItems.HasFlag(U4_Decompiled_AVATAR.ITEMS.CANDLE))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Candle ";
                }
                if (u4.Party.mItems.HasFlag(U4_Decompiled_AVATAR.ITEMS.SKULL))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Skull ";
                }

                itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "" + '\n' +
                   "Key:";

                if (u4.Party.mItems.HasFlag(U4_Decompiled_AVATAR.ITEMS.LOVE_KEY))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Love ";
                }
                if (u4.Party.mItems.HasFlag(U4_Decompiled_AVATAR.ITEMS.TRUTH_KEY))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Truth ";
                }
                if (u4.Party.mItems.HasFlag(U4_Decompiled_AVATAR.ITEMS.COMPASSION_KEY))
                {
                    itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Compassion";
                }

                itemsStatusHeading.GetComponent<UnityEngine.UI.Text>().text = "" +
                    (char)(0x10) + "Items" + (char)(0x11) + "\n\n\n\n\n\n\n\n\n" +
                    bottomStatus;

                if (u4.zstats_mode == U4_Decompiled_AVATAR.ZSTATS_MODE.CHARACTER_OVERVIEW)
                {
                    statsOverview.SetActive(true);
                }
                else
                {
                    statsOverview.SetActive(false);
                }
                if (u4.zstats_mode == U4_Decompiled_AVATAR.ZSTATS_MODE.CHARACTER_DETAIL)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (u4.zstats_character == i)
                        {
                            characterStatus[i].SetActive(true);
                        }
                        else
                        {
                            characterStatus[i].SetActive(false);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                    {
                        characterStatus[i].SetActive(false);
                    }

                }

                if (u4.zstats_mode == U4_Decompiled_AVATAR.ZSTATS_MODE.WEAPONS)
                {
                    weaponsStatus.SetActive(true);
                }
                else
                {
                    weaponsStatus.SetActive(false);
                }
                if (u4.zstats_mode == U4_Decompiled_AVATAR.ZSTATS_MODE.ARMOUR)
                {
                    armourStatus.SetActive(true);
                }
                else
                {
                    armourStatus.SetActive(false);
                }

                if (u4.zstats_mode == U4_Decompiled_AVATAR.ZSTATS_MODE.EQUIPMENT)
                {
                    equipmentStatus.SetActive(true);
                }
                else
                {
                    equipmentStatus.SetActive(false);
                }

                if (u4.zstats_mode == U4_Decompiled_AVATAR.ZSTATS_MODE.ITEMS)
                {
                    itemsStatus.SetActive(true);
                }
                else
                {
                    itemsStatus.SetActive(false);
                }

                if (u4.zstats_mode == U4_Decompiled_AVATAR.ZSTATS_MODE.MIXTURES)
                {
                    mixturesStatus.SetActive(true);
                }
                else
                {
                    mixturesStatus.SetActive(false);
                }

                if (u4.zstats_mode == U4_Decompiled_AVATAR.ZSTATS_MODE.REAGENTS)
                {
                    reagentsStatus.SetActive(true);
                }
                else
                {
                    reagentsStatus.SetActive(false);
                }

                if (lastVisionFilename != u4.visionFilename)
                {
                    if (u4.visionFilename.Length > 0)
                    {
                        LoadAVATAREGAFile(u4.visionFilename.Replace(".pic", ".EGA"), visionTexture);
                        vision.sprite = Sprite.Create(visionTexture, new Rect(0.0f, 0.0f, visionTexture.width, visionTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                        vision.color = new Color(255f, 255f, 255f, 255f);

                        lastVisionFilename = u4.visionFilename;
                    }
                    else
                    {
                        vision.sprite = null;
                        vision.color = new Color(0f, 0f, 0f, 0f);
                        ClearTexture(visionTexture, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BLACK]);
                    }
                }
            }
        }

        // make party a billboard
        Transform look = Camera.main.transform;
        look.position = new Vector3(look.position.x, 0.0f, look.position.z);
        partyGameObject.transform.LookAt(look.transform);
        Vector3 rot = partyGameObject.transform.eulerAngles;
        partyGameObject.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y, rot.z + 180.0f);

        U4_Decompiled_AVATAR.MODE currentMode = u4.current_mode;

        // we've moved, regenerate the raycast, TODO NPCs can also affect the raycast when moving, need to check them also or redo raycast more often
        if ((u4.Party._x != lastRaycastPlayer_posx) || // player moved
            (u4.Party._y != lastRaycastPlayer_posy) || // player moved
            (u4.Party.f_1dc != lastRaycastPlayer_f_1dc) || // balloon flying or grounded or dungeon torch active
            ((u4.open_door_timer > 0) != last_door_timer) || // door has opened or closed
           (u4.surface_party_direction != lastRaycastP_surface_party_direction)) // have we rotated the camera
        {
            Vector3 location = Vector3.zero;

            // update the last raycast position
            lastRaycastPlayer_posx = u4.Party._x;
            lastRaycastPlayer_posy = u4.Party._y;
            lastRaycastP_surface_party_direction = u4.surface_party_direction;
            lastRaycastPlayer_f_1dc = u4.Party.f_1dc; // flying in the balloon or not or dungeon torch active
            last_door_timer = (u4.open_door_timer > 0);

            if (u4.current_mode == U4_Decompiled_AVATAR.MODE.OUTDOORS)
            {
                // generate a new raycast
                raycast(ref entireMapTILEs,
                    u4.Party._x, 
                    u4.Party._y,
                    ref raycastOutdoorMap, 
                    ((u4.Party._x - raycastOutdoorMap.GetLength(0) / 2 - 1) ) ,
                    ((u4.Party._y - raycastOutdoorMap.GetLength(1) / 2 - 1) ) , 
                    U4_Decompiled_AVATAR.TILE.BLANK);
                location = new Vector3(
                    ((u4.Party._x - raycastOutdoorMap.GetLength(0) / 2) - 1) , 0, 
                    entireMapTILEs.GetLength(1) - ((u4.Party._y - raycastOutdoorMap.GetLength(1) / 2 - 1) ) - raycastOutdoorMap.GetLength(1));
            }
            else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.BUILDING)
            {
                // generate a new raycast based on game engine map
                raycast(ref u4.tMap32x32, 
                    u4.Party._x, 
                    u4.Party._y, 
                    ref raycastSettlementMap, 
                    ((u4.Party._x - raycastSettlementMap.GetLength(0) / 2 - 1) ), 
                    ((u4.Party._y - raycastSettlementMap.GetLength(1) / 2 - 1) ), 
                    U4_Decompiled_AVATAR.TILE.GRASS);
                location = new Vector3(
                    ((u4.Party._x - raycastSettlementMap.GetLength(0) / 2 - 1) ) , 0,
                    entireMapTILEs.GetLength(1) - ((u4.Party._y - raycastSettlementMap.GetLength(1) / 2 - 1) )  - raycastSettlementMap.GetLength(1));
            }

            // create the game object children with meshes and textures
            if (u4.current_mode == U4_Decompiled_AVATAR.MODE.OUTDOORS)
            {
                Combine.Combine3(mainTerrain, 
                    ref raycastOutdoorMap, 
                    u4.Party._x - raycastOutdoorMap.GetLength(0) / 2 - 1, 
                    u4.Party._y - raycastOutdoorMap.GetLength(1) / 2 - 1, 
                    ref entireMapGameObjects,
                    false, 
                    TextureFormat.RGBA32, 
                    true,
                    combinedExpandedMaterial,
                    combinedLinearMaterial,
                    u4.Party._x,
                    u4.Party._y,
                    u4.surface_party_direction);
                    
                location = Vector3.zero;
            }
            else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.BUILDING)
            {
                    
                SETTLEMENT settlement;
                // get the current settlement, need to special case BRITANNIA as the castle has two levels, use the ladder to determine which level
                if ((u4.Party._loc == U4_Decompiled_AVATAR.LOCATIONS.BRITANNIA) && (u4.tMap32x32[3, 3] == U4_Decompiled_AVATAR.TILE.LADDER_UP))
                {
                    settlement = SETTLEMENT.LCB_1;
                }
                else
                {
                    settlement = (SETTLEMENT)u4.Party._loc;
                }

                Combine.Combine3(mainTerrain, 
                    ref raycastSettlementMap, 
                    u4.Party._x - raycastSettlementMap.GetLength(0) / 2 - 1, 
                    u4.Party._y - raycastSettlementMap.GetLength(1) / 2 - 1, 
                    ref settlementsMapGameObjects[(int)settlement],
                    false,
                    TextureFormat.RGBA32,
                    false,
                    combinedExpandedMaterial,
                    combinedLinearMaterial,
                    u4.Party._x,
                    u4.Party._y,
                    u4.surface_party_direction);

                //CreateMapLabels(mainTerrain, ref raycastSettlementMap);

                location = new Vector3(0, 0, 224);
                    
                /*
                CreateMap(mainTerrain, raycastSettlementMap);
                location = new Vector3(
                    ((u4.Party._x - raycastSettlementMap.GetLength(0) / 2 - 1) ) , 0,
                    entireMapTILEs.GetLength(1) - ((u4.Party._y - raycastSettlementMap.GetLength(1) / 2 - 1) )  - raycastSettlementMap.GetLength(1));
                */
            }

            // Position the map in place
            mainTerrain.transform.position = location;

            // rotate map into place
            mainTerrain.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

            /* used to manually create meshes
            if (once)
            {
                if (convertMe)
                {
                    Combine(convertMe, false, TextureFormat.RGBA32, false);

                    MeshFilter meshFilter = convertMe.GetComponent<MeshFilter>();
                    for (int i = 0; i < meshFilter.mesh.vertices.Length; i++)
                    {
                        Debug.Log("new Vector3(" + meshFilter.mesh.vertices[i].x + "f, " + meshFilter.mesh.vertices[i].y + "f, " + meshFilter.mesh.vertices[i].z + "f),");
                    }
                    for (int i = 0; i < meshFilter.mesh.triangles.Length; i += 3)
                    {
                        Debug.Log(meshFilter.mesh.triangles[i] + ", " + meshFilter.mesh.triangles[i + 1] + ", " + meshFilter.mesh.triangles[i + 2] + ",");
                    }
                    for (int i = 0; i < meshFilter.mesh.uv.Length; i++)
                    {
                        Debug.Log("new Vector2(" + meshFilter.mesh.uv[i].x + "f, " + meshFilter.mesh.uv[i].y + "f),");
                    }

                    once = false;
                }
            }
            */
        }
    }

    public string lastVisionFilename;

    // The font is setup so if the high bit is set it will use the inverse highlighted text
    // this function will set the high bit on all the characters in a string so when displayed with the font
    // it will be highlighted
    public string highlight(string s)
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

    public GameObject statsOverview;
    public GameObject windDirection;
    public GameObject moons;
    public GameObject trammelLight;
    public GameObject feluccaLight;
    public GameObject sunLight;

    public GameObject[] characterStatus = new GameObject[8];
    public GameObject weaponsStatus;
    public GameObject armourStatus;
    public GameObject equipmentStatus;
    public GameObject itemsStatus;
    public GameObject itemsStatusHeading;
    public GameObject reagentsStatus;
    public GameObject mixturesStatus;

    public U4_Decompiled_AVATAR.MODE lastMode = (U4_Decompiled_AVATAR.MODE )(-1);
    public bool wasJustInside = false;
    public bool readyToAutomaticallyEnter = true;

    public Transform rotateTransform;
    //public GameObject convertMe;
}
