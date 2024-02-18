using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tile
{
    public enum TILE_TYPE
    {
        CGA,
        EGA,
        APPLE2,
        PNG,
        MAX
    };

    public enum TILE
    {
        /*deep water*/
        DEEP_WATER = 0x00,
        /*medium water*/
        MEDIUM_WATER = 0x01,
        /*shallow water*/
        SHALLOW_WATER = 0x02,
        /*swamp*/
        SWAMP = 0x03,
        /*grass*/
        GRASS = 0x04,
        /*scrub*/
        BRUSH = 0x05,
        /*forest*/
        FOREST = 0x06,
        /*hills*/
        HILLS = 0x07,
        /*mountains*/
        MOUNTAINS = 0x08,
        /*dungeon*/
        DUNGEON = 0x09,
        /*town*/
        TOWN = 0x0A,
        /*castle*/
        CASTLE = 0x0B,
        /*village*/
        VILLAGE = 0x0C,
        /*LB castle left wing*/
        CASTLE_LEFT = 0x0D,
        /*LB castle entrance*/
        CASTLE_ENTRANCE = 0x0E,
        /*LB castle right wing*/
        CASTLE_RIGHT = 0x0F,

        /*ship W N E S*/
        SHIP = 0x10,
        SHIP_WEST = 0x10,
        SHIP_NORTH = 0x11,
        SHIP_EAST = 0x12,
        SHIP_SOUTH = 0x13,

        /*horse W/E*/
        HORSE = 0x14,
        HORSE_WEST = 0x14,
        HORSE_EAST = 0x15,

        /*tiled floor*/
        TILED_FLOOR = 0x16,
        /*bridge*/
        BRIDGE = 0x17,
        /*balloon*/
        BALOON = 0x18,
        /* bridge upper */
        BRIDGE_TOP = 0x19,
        /* bridge lower */
        BRIDGE_BOTTOM = 0x1A,
        /*ladder up*/
        LADDER_UP = 0x1B,
        /*ladder down*/
        LADDER_DOWN = 0x1C,
        /*ruins*/
        RUINS = 0x1D,
        /*shrine*/
        SHRINE = 0x1E,
        /*on foot party*/
        PARTY = 0x1F,

        /* 2-tile animation character */
        MAGE = 0x20,
        MAGE2 = 0x21,

        BARD = 0x22,
        BARD2 = 0x21,

        FIGHTER = 0x24,
        FIGHTER2 = 0x25,

        DRUID = 0x26,
        DRUID2 = 0x27,

        TINKER = 0x28,
        TINKER2 = 0x29,

        PALADIN = 0x2A,
        PALADIN2 = 0x2B,

        RANGER = 0x2C,
        RANGER2 = 0x2D,

        SHEPHERD = 0x2E,
        SHEPHERD2 = 0x2F,

        /* architecture/misc tiles */
        BRICK_FLOOR_COLUMN = 0x30,
        DIAGONAL_WATER_ARCHITECTURE1 = 0x31,
        DIAGONAL_WATER_ARCHITECTURE2 = 0x32,
        DIAGONAL_WATER_ARCHITECTURE3 = 0x33,
        DIAGONAL_WATER_ARCHITECTURE4 = 0x34,
        SHIP_MAST = 0x35,
        SHIP_WHEEL = 0x36,
        SMALL_ROCKS = 0x37,

        /*sleep*/
        SLEEP = 0x38,
        /* large rocks */
        LARGE_ROCKS = 0x39,
        /*locked door*/
        LOCKED_DOOR = 0x3A,
        /*door*/
        DOOR = 0x3B,
        /*chest*/
        CHEST = 0x3C,
        /*ankh*/
        ANKH = 0x3D,
        /*brick floor*/
        BRICK_FLOOR = 0x3E,
        /*wood floor*/
        WOOD_FLOOR = 0x3F,

        /*moongate 4 phases*/
        MOONGATE = 0x40,
        MOONGATE1 = 0x40,
        MOONGATE2 = 0x41,
        MOONGATE3 = 0x42,
        MOONGATE4 = 0x43,

        /*poison field*/
        POISON_FIELD = 0x44,
        /*energy field*/
        ENERGY_FIELD = 0x45,
        /*fire field*/
        FIRE_FIELD = 0x46,
        /*sleep field*/
        SLEEP_FIELD = 0x47,

        /* used for boats and building features */
        ARCHITECTURE = 0x48,
        /* Secret brick wall */
        SECRET_BRICK_WALL = 0x49,
        /* Altar */
        ALTAR = 0x4A,
        /* cooking/camp fire */
        COOKING_FIRE = 0x4B,
        /* lava */
        LAVA = 0x4C,

        /* missiles */
        MISSLE_ATTACK_SMALL = 0x4D,
        MISSLE_ATTACK_BLUE = 0x4E,
        MISSLE_ATTACK_RED = 0x4F,

        /* 2-tile animation NPCs */
        GUARD = 0x50,
        GUARD2 = 0x51,

        MERCHANT = 0x52,
        MERCHANT2 = 0x53,

        BARD_NPC = 0x54,
        BARD_NPC2 = 0x55,

        JESTER = 0x56,
        JESTER2 = 0x57,

        BEGGAR = 0x58,
        BEGGAR2 = 0x59,

        CHILD = 0x5A,
        CHILD2 = 0x5B,

        BULL = 0x5C,
        BULL2 = 0x5D,

        LORD_BRITISH = 0x5E,
        LORD_BRITISH2 = 0x5F,

        /* Letters */
        A = 0x60,
        B = 0x61,
        C = 0x62,
        D = 0x63,
        E = 0x64,
        F = 0x65,
        G = 0x66,
        H = 0x67,
        I = 0x68,
        J = 0x69,
        K = 0x6A,
        L = 0x6B,
        M = 0x6C,
        N = 0x6D,
        O = 0x6E,
        P = 0x6F,
        Q = 0x70,
        R = 0x71,
        S = 0x72,
        T = 0x73,
        U = 0x74,
        V = 0x75,
        W = 0x76,
        X = 0x77,
        Y = 0x78,
        Z = 0x79,

        /* <space> */
        SPACE = 0x7A,

        /* brackets */
        BRACKET_RIGHT = 0x7B,
        BRACKET_LEFT = 0x7C,
        BRACKET_SQUARE = 0x7D,

        /* blank */
        BLANK = 0x7E,

        /* brick wall */
        BRICK_WALL = 0x7F,

        /*pirate W N E S*/
        PIRATE = 0x80,
        PIRATE_WEST = 0x80,
        PIRATE_NORTH = 0x81,
        PIRATE_EAST = 0x82,
        PIRATE_SOUTH = 0x83,

        /* 2-tile monsters */
        NIXIE = 0x84,
        NIXIE2 = 0x85,

        SQUID = 0x86,
        SQUID2 = 0x87,

        SERPENT = 0x88,
        SERPENT2 = 0x89,

        SEAHORSE = 0x8A,
        SEAHORSE2 = 0x8B,

        WHIRLPOOL = 0x8C,
        WHIRLPOOL2 = 0x8D,

        WATER_SPOUT = 0x8E,
        WATER_SPOUT2 = 0x8F,

        /* 4-tile monsters */
        RAT = 0x90,
        RAT2 = 0x91,
        RAT3 = 0x92,
        RAT4 = 0x93,

        BAT = 0x94,
        BAT2 = 0x95,
        BAT3 = 0x96,
        BAT4 = 0x97,

        SPIDER = 0x98,
        SPIDER2 = 0x99,
        SPIDER3 = 0x9a,
        SPIDER4 = 0x9b,

        GHOST = 0x9C,
        GHOST2 = 0x9D,
        GHOST3 = 0x9E,
        GHOST4 = 0x9F,

        SLIME = 0xA0,
        SLIME2 = 0xA1,
        SLIME3 = 0xA2,
        SLIME4 = 0xA3,

        TROLL = 0xA4,
        TROLL2 = 0xA5,
        TROLL3 = 0xA6,
        TROLL4 = 0xA7,

        GREMLIN = 0xA8,
        GREMLIN2 = 0xA9,
        GREMLIN3 = 0xAa,
        GREMLIN4 = 0xAb,

        MIMIC = 0xAC,
        MIMIC2 = 0xAd,
        MIMIC3 = 0xAe,
        MIMIC4 = 0xAf,

        REAPER = 0xB0,
        REAPER2 = 0xB1,
        REAPER3 = 0xB2,
        REAPER4 = 0xB3,

        INSECTS = 0xB4,
        INSECTS2 = 0xB5,
        INSECTS3 = 0xB6,
        INSECTS4 = 0xB7,

        GAZER = 0xB8,
        GAZER2 = 0xB9,
        GAZER3 = 0xBa,
        GAZER4 = 0xBb,

        PHANTOM = 0xBC,
        PHANTOM2 = 0xBD,
        PHANTOM3 = 0xBE,
        PHANTOM4 = 0xBF,

        ORC = 0xC0,
        ORC2 = 0xC1,
        ORC3 = 0xC2,
        ORC4 = 0xC3,

        SKELETON = 0xC4,
        SKELETON2 = 0xC5,
        SKELETON3 = 0xC6,
        SKELETON4 = 0xC7,

        ROGUE = 0xC8,
        ROGUE2 = 0xC9,
        ROGUE3 = 0xCa,
        ROGUE4 = 0xCb,

        PYTHON = 0xCC,
        PYTHON2 = 0xCd,
        PYTHON3 = 0xCe,
        PYTHON4 = 0xCf,

        ETTIN = 0xD0,
        ETTIN2 = 0xD1,
        ETTIN3 = 0xD2,
        ETTIN4 = 0xD3,

        HEADLESS = 0xD4,
        HEADLESS2 = 0xD5,
        HEADLESS3 = 0xD6,
        HEADLESS4 = 0xD7,

        CYCLOPS = 0xD8,
        CYCLOPS2 = 0xD9,
        CYCLOPS3 = 0xDa,
        CYCLOPS4 = 0xDb,

        WISP = 0xDC,
        WISP2 = 0xDD,
        WISP3 = 0xDE,
        WISP4 = 0xDF,

        MAGE_NPC = 0xE0,
        MAGE_NPC2 = 0xE1,
        MAGE_NPC3 = 0xE2,
        MAGE_NPC4 = 0xE3,

        LYCHE = 0xE4,
        LYCHE2 = 0xE5,
        LYCHE3 = 0xE6,
        LYCHE4 = 0xE7,

        LAVA_LIZARD = 0xE8,
        LAVA_LIZARD2 = 0xE9,
        LAVA_LIZARD3 = 0xEa,
        LAVA_LIZARD4 = 0xEb,

        ZORN = 0xEC,
        ZORN2 = 0xEd,
        ZORN3 = 0xEe,
        ZORN4 = 0xEf,

        DAEMON = 0xF0,
        DAEMON2 = 0xF1,
        DAEMON3 = 0xF2,
        DAEMON4 = 0xF3,

        HYDRA = 0xF4,
        HYDRA2 = 0xF5,
        HYDRA3 = 0xF6,
        HYDRA4 = 0xF7,

        DRAGON = 0xF8,
        DRAGON2 = 0xF9,
        DRAGON3 = 0xFa,
        DRAGON4 = 0xFb,

        BALRON = 0xFC,
        BALRON2 = 0xFd,
        BALRON3 = 0xFe,
        BALRON4 = 0xFf,

        MAX = 0x100
    };

    public static Texture2D[] originalTiles;
    public static Texture2D[] expandedTiles;

    public static Texture2D PNGAtlas;
    public static string PNGFilepath = "/u4/SHAPES.PNG";
    public static string tileApple2Filepath1 = "/u4/SHP0.B";
    public static string tileApple2Filepath2 = "/u4/SHP1.B";
    public static string tileEGAFilepath = "/u4/SHAPES.EGA";
    public static string tileCGAFilepath = "/u4/SHAPES.CGA";

    public static Texture2D combinedExpandedTexture;
    public static Material combinedExpandedMaterial;
    public static int textureExpandedAtlasPowerOf2;

    public static Texture2D combinedLinearTexture;
    public static Material combinedLinearMaterial;

    // NOTE certain shaders used for things like sprites and unlit textures do not
    // do well with edges and leave ghosts of the nearby textures from the texture atlas
    // to solve this issue I need to create at least a one pixel mirror border around the
    // tile, this function creates a larger tile texture and adds this border around the tile placed in the center.
    // Special care must be made when combining meshes with textures like this and the Combine()
    // function has been updated to handle this situation and update the uv's. Given that some
    // platforms may require textures be certain integer multiples of 2 this function will allow
    // a larger than one pixel border around the tile.
    public const int TILE_BORDER_SIZE = 1;
    public static int expandedTileWidth;
    public static int expandedTileHeight;
    public static int originalTileWidth;
    public static int originalTileHeight;

    public static TILE_TYPE currentTileType;

    public static void ExpandTiles()
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

    public static void AnimateFlags()
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
            Tile.TILE tileIndex = Tile.TILE.CASTLE_ENTRANCE;

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
            Tile.TILE tileIndex = Tile.TILE.TOWN;

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
            Tile.TILE tileIndex = Tile.TILE.CASTLE;

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
            Tile.TILE tileIndex = Tile.TILE.SHIP_WEST;

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
            Tile.TILE tileIndex = Tile.TILE.SHIP_EAST;

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
            Tile.TILE tileIndex = Tile.TILE.SHIP_WEST;

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
            Tile.TILE tileIndex = Tile.TILE.SHIP_EAST;

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
            Tile.TILE tileIndex = Tile.TILE.SHIP_WEST;
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
            Tile.TILE tileIndex = Tile.TILE.SHIP_EAST;
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
            Tile.TILE tileIndex = Tile.TILE.SHIP_WEST;
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
            Tile.TILE tileIndex = Tile.TILE.SHIP_EAST;
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
            Tile.TILE tileIndex = Tile.TILE.COOKING_FIRE;

            int offset_x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth + TILE_BORDER_SIZE;
            int offset_y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight + TILE_BORDER_SIZE;

            Color alpha = new Color(0, 0, 0, 0);
            //Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BLACK];

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

        // TODO Dungeons use the expanded tile and include some cooking fires but they create their own tile map for each
        // dungeon map block, will probbaly need to be a special case for the cooking fire in the dungeon
        // or switch to the Combine3() method used in the outdoors and settlements which is animated in the code above
        /*
        {
            const int x1 = 3;
            const int x2 = 9;
            const int y1 = 9;
            const int y2 = 16;
            Tile.TILE tileIndex = Tile.TILE.COOKING_FIRE;

            Color alpha = new Color(0, 0, 0, 0);

            for (int y = y1; y <= y2; y++)
            {
                for (int x = x1; x <= x2; x++)
                {
                    Color color = expandedTiles[(int)tileIndex].GetPixel(x, expandedTileHeight - 1 - y);
                    if ((color == Palette.EGAColorPalette[(int)Palette.EGA_COLOR.RED]) || (color == alpha))
                    {
                        if (Random.Range(0, 100) <= 50)
                        {
                            combinedExpandedTexture.SetPixel(x, expandedTileHeight - 1 - y, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.RED]);
                        }
                        else
                        {
                            combinedExpandedTexture.SetPixel(x, expandedTileHeight - 1 - y, alpha);
                        }
                    }
                }
            }
            expandedTiles[(int)tileIndex].Apply();
        }
        */
    }

    // TODO fix horse tile also
    public static void FixMageTile3()
    {
        // adjust the pixels on mage tile #3
        Texture2D currentTile = originalTiles[(int)Tile.TILE.MAGE_NPC3];

        // go through all the pixels in the source texture and shift them one pixel
        for (int height = 0; height < currentTile.height; height++)
        {
            for (int width = currentTile.width - 1; width > 0; width--)
            {
                currentTile.SetPixel(width, height, currentTile.GetPixel((width - 1 + currentTile.width) % currentTile.width, height));
            }
        }

        // apply all the previous SetPixel() calls to the texture
        currentTile.Apply();
    }
    public static void CreateLinearTextureAtlas(ref Texture2D[] tilesTextures, bool useMipMaps = false, TextureFormat textureFormat = TextureFormat.RGBA32)
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
        combinedLinearMaterial = new Material(Shader.Find("Unlit/Transparent Cutout 2"));
        combinedLinearMaterial.mainTexture = combinedLinearTexture;
    }

    public static Texture2D combinedTexture;
    public static Material combinedMaterial;
    public static Hashtable textureAtlasHashTable = new Hashtable();
    public static int textureAtlasPowerOf2;
    public static void CreateSquareTextureAtlas(ref Texture2D[] tilesTextures, bool useMipMaps = false, TextureFormat textureFormat = TextureFormat.RGBA32)
    {
        int sizeW;
        int sizeH;
        int originalSizeW;
        int originalSizeH;
        Texture2D texture;

        // figure out the square size power of 2 factor for the number of textures we have
        if (tilesTextures.Length == 0)
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
        combinedMaterial = new Material(Shader.Find("Unlit/Transparent Cutout 2"));
        combinedMaterial.mainTexture = combinedTexture;
    }
    public static void CreateExpandedTextureAtlas(ref Texture2D[] tilesTextures, bool useMipMaps = false, TextureFormat textureFormat = TextureFormat.RGBA32)
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
        combinedExpandedMaterial = new Material(Shader.Find("Unlit/Transparent Cutout 2"));
        combinedExpandedMaterial.mainTexture = combinedExpandedTexture;
    }

    public static void LoadTilesPNG()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + PNGFilepath))
        {
            Debug.Log("Could not find PNG tiles atlas file " + Application.persistentDataPath + PNGFilepath);
            return;
        }

        // read the file
        byte[] fileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + PNGFilepath);

        // allocate something to start with so the loadImage can resize it to file the actual file
        PNGAtlas = new Texture2D(32, 32 * 256, TextureFormat.RGBA32, false);

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

            int y = (255 - tile) * originalTileHeight;

            currentTile.SetPixels(0, 0, originalTileWidth, originalTileHeight, PNGAtlas.GetPixels(0, y, originalTileWidth, originalTileHeight));
            currentTile.Apply();
        }
    }

    public static void LoadTilesApple2()
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

    public static void LoadTiles(TILE_TYPE tileType)
    {
        switch (tileType)
        {
            case TILE_TYPE.EGA:
                LoadTilesEGA();
                currentTileType = TILE_TYPE.EGA;
                break;
            case TILE_TYPE.CGA:
                LoadTilesCGA();
                currentTileType = TILE_TYPE.CGA;
                break;
            case TILE_TYPE.APPLE2:
                LoadTilesApple2();
                currentTileType = TILE_TYPE.APPLE2;
                break;
            case TILE_TYPE.PNG:
                LoadTilesPNG();
                currentTileType = TILE_TYPE.PNG;
                break;
            default:
                Debug.Log("LoadTiles() unknown tile type " + (int)tileType);
                break;
         }
    }

    public static void LoadTilesEGA()
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
                        (tile == (int)Tile.TILE.ANKH ||
                        tile == (int)Tile.TILE.LADDER_UP ||
                        tile == (int)Tile.TILE.LADDER_DOWN ||
                        tile == (int)Tile.TILE.FOREST ||
                        tile == (int)Tile.TILE.COOKING_FIRE ||
                        tile == (int)Tile.TILE.SHRINE ||
                        tile == (int)Tile.TILE.ALTAR ||
                        tile == (int)Tile.TILE.BALOON ||
                        //tile == (int)U4_Decompiled.TILE.CHEST ||
                        tile == (int)Tile.TILE.CASTLE ||
                        tile == (int)Tile.TILE.CASTLE_LEFT ||
                        tile == (int)Tile.TILE.CASTLE_ENTRANCE ||
                        tile == (int)Tile.TILE.CASTLE_RIGHT ||
                        tile == (int)Tile.TILE.VILLAGE ||
                        tile == (int)Tile.TILE.BRIDGE ||
                        tile == (int)Tile.TILE.BRIDGE_BOTTOM ||
                        tile == (int)Tile.TILE.BRIDGE_TOP ||
                        tile == (int)Tile.TILE.NIXIE ||
                        tile == (int)Tile.TILE.NIXIE2 ||
                        (tile >= (int)Tile.TILE.MISSLE_ATTACK_SMALL && tile <= (int)Tile.TILE.MISSLE_ATTACK_RED) ||
                        (tile >= (int)Tile.TILE.PARTY && tile <= (int)Tile.TILE.SHEPHERD2) ||
                        (tile >= (int)Tile.TILE.GUARD && tile <= (int)Tile.TILE.LORD_BRITISH2) ||
                        (tile >= (int)Tile.TILE.SERPENT && tile <= (int)Tile.TILE.WATER_SPOUT2) ||
                        (tile >= (int)Tile.TILE.BAT && tile <= (int)Tile.TILE.TROLL4) ||
                        (tile >= (int)Tile.TILE.INSECTS && tile <= (int)Tile.TILE.INSECTS4) ||
                        (tile >= (int)Tile.TILE.PHANTOM && tile <= (int)Tile.TILE.MAGE_NPC4) ||
                        (tile >= (int)Tile.TILE.LAVA_LIZARD && tile <= (int)Tile.TILE.ZORN4) ||
                        (tile >= (int)Tile.TILE.HYDRA && tile <= (int)Tile.TILE.BALRON4)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // remove the brown line that overlaps the bridge support
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BROWN) && tile == (int)Tile.TILE.BRIDGE_BOTTOM && height == 9)
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // special case the horse black eyes
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BLACK) &&
                        (tile == (int)Tile.TILE.HORSE_EAST))
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
                        (tile == (int)Tile.TILE.HORSE_WEST))
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
                        (tile == (int)Tile.TILE.VILLAGE ||
                        tile == (int)Tile.TILE.TOWN ||
                        tile == (int)Tile.TILE.RUINS))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // remove green grass specals from moongates 
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BLACK) || (colorIndex == (int)Palette.EGA_COLOR.GREEN)) &&
                        (tile >= (int)Tile.TILE.MOONGATE1 && tile <= (int)Tile.TILE.MOONGATE4))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // make the moongates blue and yellow transparent
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BRIGHT_YELLOW) || (colorIndex == (int)Palette.EGA_COLOR.BRIGHT_BLUE)) &&
                        (tile >= (int)Tile.TILE.MOONGATE1 && tile <= (int)Tile.TILE.MOONGATE4))
                    {
                        color.a = 0.75f;
                        currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                    }
                    // remove blue water from these squid tiles and make black and blue into alpha
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BLACK) || (colorIndex == (int)Palette.EGA_COLOR.BRIGHT_BLUE)) &&
                        (tile == (int)Tile.TILE.SQUID || tile == (int)Tile.TILE.SQUID2))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // ships
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BLACK) &&
                        ((tile >= (int)Tile.TILE.SHIP_WEST && tile <= (int)Tile.TILE.SHIP_SOUTH) ||
                        (tile >= (int)Tile.TILE.PIRATE_WEST && tile <= (int)Tile.TILE.PIRATE_SOUTH)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // make energy fields are transparent
                    else if (tile >= (int)Tile.TILE.POISON_FIELD && tile <= (int)Tile.TILE.SLEEP_FIELD)
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
                        (tile == (int)Tile.TILE.ANKH ||
                        tile == (int)Tile.TILE.LADDER_UP ||
                        tile == (int)Tile.TILE.LADDER_DOWN ||
                        tile == (int)Tile.TILE.FOREST ||
                        tile == (int)Tile.TILE.COOKING_FIRE ||
                        tile == (int)Tile.TILE.SHRINE ||
                        tile == (int)Tile.TILE.ALTAR ||
                        tile == (int)Tile.TILE.BALOON ||
                        //tile == (int)U4_Decompiled.TILE.CHEST ||
                        tile == (int)Tile.TILE.CASTLE ||
                        tile == (int)Tile.TILE.CASTLE_LEFT ||
                        tile == (int)Tile.TILE.CASTLE_ENTRANCE ||
                        tile == (int)Tile.TILE.CASTLE_RIGHT ||
                        tile == (int)Tile.TILE.VILLAGE ||
                        tile == (int)Tile.TILE.BRIDGE ||
                        tile == (int)Tile.TILE.BRIDGE_BOTTOM ||
                        tile == (int)Tile.TILE.BRIDGE_TOP ||
                        tile == (int)Tile.TILE.NIXIE ||
                        tile == (int)Tile.TILE.NIXIE2 ||
                        (tile >= (int)Tile.TILE.MISSLE_ATTACK_SMALL && tile <= (int)Tile.TILE.MISSLE_ATTACK_RED) ||
                        (tile >= (int)Tile.TILE.PARTY && tile <= (int)Tile.TILE.SHEPHERD2) ||
                        (tile >= (int)Tile.TILE.GUARD && tile <= (int)Tile.TILE.LORD_BRITISH2) ||
                        (tile >= (int)Tile.TILE.SERPENT && tile <= (int)Tile.TILE.WATER_SPOUT2) ||
                        (tile >= (int)Tile.TILE.BAT && tile <= (int)Tile.TILE.TROLL4) ||
                        (tile >= (int)Tile.TILE.INSECTS && tile <= (int)Tile.TILE.INSECTS4) ||
                        (tile >= (int)Tile.TILE.PHANTOM && tile <= (int)Tile.TILE.MAGE_NPC4) ||
                        (tile >= (int)Tile.TILE.LAVA_LIZARD && tile <= (int)Tile.TILE.ZORN4) ||
                        (tile >= (int)Tile.TILE.HYDRA && tile <= (int)Tile.TILE.BALRON4)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // remove the brown line that overlaps the bridge support
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BROWN) && tile == (int)Tile.TILE.BRIDGE_BOTTOM && height == 9)
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // others where we need to make green an alpha channel also like towns/ruins/villages
                    // so the green grass speckels don't show when we use the tile standing upright
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BLACK) || (colorIndex == (int)Palette.EGA_COLOR.GREEN)) &&
                        (tile == (int)Tile.TILE.VILLAGE ||
                        tile == (int)Tile.TILE.TOWN ||
                        tile == (int)Tile.TILE.RUINS))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // remove green grass specals from moongates 
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BLACK) || (colorIndex == (int)Palette.EGA_COLOR.GREEN)) &&
                        (tile >= (int)Tile.TILE.MOONGATE1 && tile <= (int)Tile.TILE.MOONGATE4))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // make the moongates blue and yellow transparent TODO make black inside portal transparent also
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BRIGHT_YELLOW) || (colorIndex == (int)Palette.EGA_COLOR.BRIGHT_BLUE)) &&
                        (tile >= (int)Tile.TILE.MOONGATE1 && tile <= (int)Tile.TILE.MOONGATE4))
                    {
                        color.a = 0.75f;
                        currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                    }
                    // remove blue water from these tiles and make black and blue into alpha
                    else if (((colorIndex == (int)Palette.EGA_COLOR.BLACK) || (colorIndex == (int)Palette.EGA_COLOR.BRIGHT_BLUE)) &&
                        (tile == (int)Tile.TILE.SQUID || tile == (int)Tile.TILE.SQUID2))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // special case the horse black eyes
                    else if ((colorIndex == (int)Palette.EGA_COLOR.BLACK) &&
                        (tile == (int)Tile.TILE.HORSE_EAST))
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
                        (tile == (int)Tile.TILE.HORSE_WEST))
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
                        ((tile >= (int)Tile.TILE.SHIP_WEST && tile <= (int)Tile.TILE.SHIP_SOUTH) ||
                        (tile >= (int)Tile.TILE.PIRATE_WEST && tile <= (int)Tile.TILE.PIRATE_SOUTH)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // energy fields are transparent
                    else if (tile >= (int)Tile.TILE.POISON_FIELD && tile <= (int)Tile.TILE.SLEEP_FIELD)
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

    public static void LoadTilesCGA()
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
}
