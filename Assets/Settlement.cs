using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Settlement : MonoBehaviour
{
    public Color[] CGAColorPalette;
    public Color[] EGAColorPalette;
    public Texture2D[] tiles;

    //GameObject npcs;
    GameObject terrain;
    GameObject animatedTerrrain;
    GameObject shadow;

    public string tileEGAFilepath = "/u4/SHAPES.EGA";
    public string tileCGAFilepath = "/u4/SHAPES.CGA";
    public string settlementFilepath = "/u4/BRITAIN.ULT";
    public string talkFilepath = "/u4/BRITAIN.TLK";

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

    void LoadTilesEGA()
    {
        Color alpha = new Color(0, 0, 0, 0);

        if (!System.IO.File.Exists(Application.persistentDataPath + tileEGAFilepath))
        {
            Debug.Log("Could not find EGA tiles file " + Application.persistentDataPath + tileEGAFilepath);
            return;
        }

        // read the file
        byte [] fileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + tileEGAFilepath);

        if (fileData.Length != 32*1024)
        {
            Debug.Log("EGA tiles file incorrect length " + fileData.Length);
            return;
        }

        // allocate an array of textures
        tiles = new Texture2D[256];

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
            tiles[tile] = currentTile;

            // manually go through the data and set the (x,y) pixels to the tile based on the input file using the CGA color palette
            for (int height = 0; height < currentTile.height; height++)
            {
                for (int width = 0; width < currentTile.width; /* width incremented below */ )
                {
                    // set the color of the first half of the nibble
                    int colorIndex = fileData[index] >> 4; 
                    Color color = EGAColorPalette[colorIndex];

                    // check if these are people/creatures and use black as alpha channel
                    if ((colorIndex == 0) && ((tile >= 32 && tile <= 47) || (tile >= 80 && tile <= 95)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // energy fields are transparent
                    else if (tile >= 68 && tile <= 71)
                    {
                        if (colorIndex == 0)
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                        else
                        {
                            color.a = 0.75f;
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                    }
                    else
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                    }

                    // set the color of the second half of the nibble
                    colorIndex = fileData[index] & 0xf;
                    color = EGAColorPalette[colorIndex];

                    // check if these are people/creatures and use black as alpha channel
                    if ((colorIndex == 0) && ((tile >= 32 && tile <= 47) || (tile >= 80 && tile <= 95)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // energy fields are transparent
                    else if (tile >= 68 && tile <= 71)
                    {
                        if (colorIndex == 0)
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                        else
                        {
                            color.a = 0.75f;
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                    }
                    // everything else has not alpha channel
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
        tiles = new Texture2D[256];

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
                    currentTile.SetPixel(width, currentTile.height - height -2, color);

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

            // Actually apply all previous SetPixel and SetPixels changes from above
            currentTile.Apply();

            // skip ahead
            index += 0x20;
        }
    }

    public enum NPC_MOVEMENT_MODE
    {
        FIXED = 0x00,
        WANDER = 0x01,
        FOLLOW = 0x80,
        ATTACK = 0xff
    };

    public int[] npcQuestionTriggerIndex = new int[16];
    public bool[] npcQuestionAffectHumility = new bool[16];
    public int[] npcProbabilityOfTurningAway = new int[16];
    public List<string>[] npcStrings = new List<string>[16];
    public U4_Decompiled.TILE[,] settlementMap = new U4_Decompiled.TILE[32,32];

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
        KEYWORD2 = 11
    };

    public struct npc
    {
        public U4_Decompiled.TILE tile;
        public byte pos_x;
        public byte pos_y;
        public NPC_MOVEMENT_MODE movement;
        public int conversationIndex;
        public List<string> strings;
        public int probabilityOfTurningAway;
        public bool questionAffectHumility;
        public int questionTriggerIndex;
    };

    public npc[] settlementNPCs = new npc[32];

    void LoadSettlementTalkAndMap()
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

        if (!System.IO.File.Exists(Application.persistentDataPath + settlementFilepath))
        {
            Debug.Log("Could not find settlement file " + Application.persistentDataPath + settlementFilepath);
            return;
        }

        // read the file
        byte[] settlementFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + settlementFilepath);

        if (settlementFileData.Length != 1280)
        {
            Debug.Log("Settlement file incorrect length " + settlementFileData.Length);
            return;
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

        if (!System.IO.File.Exists(Application.persistentDataPath + talkFilepath))
        {
            Debug.Log("Could not find settlement talk file " + Application.persistentDataPath + talkFilepath);
            return;
        }

        // read the file
        byte[] talkFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + talkFilepath);

        if (talkFileData.Length != 4608)
        {
            Debug.Log("Settlement talk file incorrect length " + talkFileData.Length);
            return;
        }

        for (int talkIndex = 0; talkIndex < 16; talkIndex++)
        {
            npcStrings[talkIndex] = new List<string>();

            npcQuestionTriggerIndex[talkIndex] = talkFileData[talkIndex * 288];
            if (talkFileData[(talkIndex * 288) + 1] != 0)
            {
                npcQuestionAffectHumility[talkIndex] = true;
            }
            else
            {
                npcQuestionAffectHumility[talkIndex] = false;
            }
            npcProbabilityOfTurningAway[talkIndex] = talkFileData[talkIndex * 288 + 2];

            string s;
            int stringIndex = 3;

            // search for strings in the .TLK file
            do
            {
                // reset string
                s = "";

                // manually construct the string because C# doesn't work with null terminated C strings well
                for (int i = 0; (i < 100) && (talkFileData[talkIndex * 288 + stringIndex] != 0); i++)
                {
                    s += (char)talkFileData[talkIndex * 288 + stringIndex++];
                }

                // if the string is of any size add it to the list
                if (s.Length != 0)
                {
                    npcStrings[talkIndex].Add(s);
                }

                // skip over null terminator to go to the next string
                stringIndex++;

                // continue to search for strings until we are past the end of the record or the last string is zero length 
            } while ((s.Length != 0) && (stringIndex < 285));
        }

        // load settlement map data
        int bufferIndex = 0;

        for (int height = 0; height < 32; height++)
        {
            for (int width = 0; width < 32; width++)
            {
                U4_Decompiled.TILE tileIndex = (U4_Decompiled.TILE)settlementFileData[bufferIndex++];
                settlementMap[width, height] = tileIndex;
            }
        }

        // load npc data from the map data
        for (int npcIndex = 0; npcIndex < 32; npcIndex++)
        {
            U4_Decompiled.TILE npcTile = (U4_Decompiled.TILE)settlementFileData[0x400 + npcIndex];
            settlementNPCs[npcIndex].tile = npcTile;

            // zero indicates unused
            if (npcTile != 0)
            {
                settlementNPCs[npcIndex].pos_x = settlementFileData[0x420 + npcIndex];
                settlementNPCs[npcIndex].pos_y = settlementFileData[0x440 + npcIndex];
                settlementNPCs[npcIndex].movement = (NPC_MOVEMENT_MODE)settlementFileData[0x4C0 + npcIndex];
                int conversationIndex = settlementFileData[0x4E0 + npcIndex];
                settlementNPCs[npcIndex].conversationIndex = conversationIndex;
                // grab the talk data and add it to this structure
                // zero indicates unused
                if (conversationIndex != 0)
                {
                    // this can be 128 for one vendor in Vincent, not sure why? TODO
                    if ((conversationIndex - 1) < npcStrings.Length)
                    {
                        settlementNPCs[npcIndex].strings = npcStrings[conversationIndex - 1];
                        settlementNPCs[npcIndex].questionAffectHumility = npcQuestionAffectHumility[conversationIndex - 1];
                        settlementNPCs[npcIndex].probabilityOfTurningAway = npcProbabilityOfTurningAway[conversationIndex - 1];
                        settlementNPCs[npcIndex].questionTriggerIndex = npcQuestionTriggerIndex[conversationIndex - 1];
                    }
                }
            }
        }
    }

    void CreateSettlementGameObjects(U4_Decompiled.TILE[,] map)
    {
        // create three game object under us to hold these sub categories of things
        if (terrain == null)
        {
            terrain = new GameObject("terrain");
            terrain.transform.SetParent(transform);
            terrain.transform.localPosition = Vector3.zero;
            terrain.transform.localRotation = Quaternion.identity;
        }

        /*
        npcs = new GameObject("npc");
        npcs.transform.SetParent(transform);
        npcs.transform.localPosition = Vector3.zero;
        npcs.transform.localRotation = Quaternion.identity;
        */

        if (animatedTerrrain == null)
        {
            animatedTerrrain = new GameObject("water");
            animatedTerrrain.transform.SetParent(transform);
            animatedTerrrain.transform.localPosition = Vector3.zero;
            animatedTerrrain.transform.localRotation = Quaternion.identity;

            // add our little animator script to the animated terrain game object
            animatedTerrrain.AddComponent<Animate1>();
        }

        // start over each update
        foreach (Transform child in terrain.transform)
        {
            Object.Destroy(child.gameObject);
        }
        foreach (Transform child in animatedTerrrain.transform)
        {
            Object.Destroy(child.gameObject);
        }

        for (int height = 0; height < 32; height++)
        {
            for (int width = 0; width < 32; width++)
            {
                GameObject mapTileGameObject;
                Vector3 mapTileLocation;
                U4_Decompiled.TILE tileIndex = map[width, height];

                // check if it tile is blank
                if (tileIndex == U4_Decompiled.TILE.BLANK)
                {
                    continue;
                }
                // solid object, brick, rocks etc. make into cubes
                else if (tileIndex == U4_Decompiled.TILE.BRICK_WALL 
                    || tileIndex == U4_Decompiled.TILE.LARGE_ROCKS 
                    || tileIndex == U4_Decompiled.TILE.SECRET_BRICK_WALL)
                {
                    mapTileGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    mapTileGameObject.transform.SetParent(terrain.transform);
                    mapTileLocation = new Vector3(width, 31 - height, 0.0f);
                }
                // Letters, make into short cubes
                else if (tileIndex >= U4_Decompiled.TILE.A && tileIndex <= U4_Decompiled.TILE.BRACKET_SQUARE)
                {
                    mapTileGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    mapTileGameObject.transform.SetParent(terrain.transform);
                    mapTileGameObject.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
                    mapTileLocation = new Vector3(width, 31 - height, 0.25f);
                }
                // all other terrain tiles are flat
                else
                {
                    mapTileGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);

                    // water, lava and entergy fields need to be handled separately so we can animate the texture using UV
                    if ((tileIndex <= U4_Decompiled.TILE.SHALLOW_WATER) 
                        || (tileIndex >= U4_Decompiled.TILE.POISON_FIELD && tileIndex <= U4_Decompiled.TILE.SLEEP_FIELD) 
                        || ( tileIndex == U4_Decompiled.TILE.LAVA))
                    {
                        mapTileGameObject.transform.SetParent(animatedTerrrain.transform);
                        
                        // engery fields are above
                        if (tileIndex >= U4_Decompiled.TILE.POISON_FIELD && tileIndex <= U4_Decompiled.TILE.SLEEP_FIELD)
                        {
                            mapTileLocation = new Vector3(width, 31 - height, -0.5f);
                        }
                        else
                        {
                            mapTileLocation = new Vector3(width, 31 - height, 0.5f);
                        }


                    }
                    else
                    {
                        mapTileGameObject.transform.SetParent(terrain.transform);
                        mapTileLocation = new Vector3(width, 31 - height, 0.5f);
                    }
                }

                mapTileGameObject.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                mapTileGameObject.transform.localPosition = mapTileLocation;

                // all terrain is static, used by combine function below to merge static meshes
                mapTileGameObject.isStatic = true;

                // set the shader
                Shader unlit = Shader.Find("Mobile/Unlit (Supports Lightmap)");

                MeshRenderer render = mapTileGameObject.GetComponent<MeshRenderer>();

                render.material.mainTexture = tiles[(int)tileIndex];
                render.material.shader = unlit;
            }
        }

        // we use the game engine to populate the npcs so we do not need to do this here
        /*
        for (int npcIndex = 0; npcIndex < 32; npcIndex++)
        {
            int npcTile = settlementFileData[0x400 + npcIndex];

            // zero indicated unused
            if (npcTile != 0)
            {
                int npcLocationX = settlementFileData[0x420 + npcIndex];
                int npcLocationY = settlementFileData[0x440 + npcIndex];
                int npcMovementMode = settlementFileData[0x4C0 + npcIndex];
                int npcConversation = settlementFileData[0x4E0 + npcIndex];

                Vector3 npcLocation = new Vector3(npcLocationX, 31 - npcLocationY, 0);

                // this is 1 based with 0 meaning no talk entry
                GameObject npcGameObject;
                
                if(npcConversation != 0)
                {
// this can be 128 for one vendor in Vincent, not sure why?
                    if ((npcConversation - 1) < npcStrings.Length)
                    {
                        npcGameObject = new GameObject(npcStrings[npcConversation - 1][0]);
                    }
                    else
                    {
                        npcGameObject = new GameObject("unamed + " + npcConversation);
                    }
                }
                else
                {
                    // need to check hardcoded vendor npcs and other special npcs here
                    npcGameObject = new GameObject("unamed" + npcConversation);
                }

                // set this as a parent of the npcs game object
                npcGameObject.transform.SetParent(npcs.transform);

                // the npc has two animation tiles in these tile ranges
                if ((npcTile >= 32 && npcTile <= 47) || (npcTile >= 80 && npcTile <= 95) || (npcTile >= 132 && npcTile <= 143))
                {
                    // create multiple child objects to alternate between for animation
                    GameObject npcGameObject1 = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    npcGameObject1.transform.SetParent(npcGameObject.transform);

                    GameObject npcGameObject2 = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    npcGameObject2.transform.SetParent(npcGameObject.transform);

                    // rotate the npc game object after creating and addition all the animation tiles
                    npcGameObject.transform.localPosition = npcLocation;
                    npcGameObject.transform.eulerAngles = new Vector3(90.0f, 0.0f, 180.0f);

                    // set the shader
                    Shader unlit = Shader.Find("Sprites/Default");

                    // create two objects to alternate between for animation
                    MeshRenderer render1 = npcGameObject1.GetComponent<MeshRenderer>();
                    MeshRenderer render2 = npcGameObject2.GetComponent<MeshRenderer>();

                    render1.material.mainTexture = tiles[npcTile];

                    if ((npcTile % 2) == 1)
                    {
                        render2.material.mainTexture = tiles[npcTile - 1];
                    }
                    else
                    {
                        render2.material.mainTexture = tiles[npcTile + 1];
                    }

                    // add our little animator script
                    npcGameObject.AddComponent<Animate2>();

                    render1.material.shader = unlit;
                    render2.material.shader = unlit;
                }
                // the npc has four animation tiles in this tile range
                else if (npcTile >= 144 && npcTile <= 255) 
                {
                    // create multiple child objects to alternate between for animation
                    GameObject npcGameObject1 = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    npcGameObject1.transform.SetParent(npcGameObject.transform);
                    MeshRenderer render1 = npcGameObject1.GetComponent<MeshRenderer>();

                    GameObject npcGameObject2 = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    npcGameObject2.transform.SetParent(npcGameObject.transform);
                    MeshRenderer render2 = npcGameObject2.GetComponent<MeshRenderer>();

                    GameObject npcGameObject3 = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    npcGameObject3.transform.SetParent(npcGameObject.transform);
                    MeshRenderer render3 = npcGameObject3.GetComponent<MeshRenderer>();

                    GameObject npcGameObject4 = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    npcGameObject4.transform.SetParent(npcGameObject.transform);
                    MeshRenderer render4 = npcGameObject4.GetComponent<MeshRenderer>();

                    // rotate the npc game object after creating and addition all the animation tiles
                    npcGameObject.transform.localPosition = npcLocation;
                    npcGameObject.transform.eulerAngles = new Vector3(90.0f, 0.0f, 180.0f);
                    
                    // set the tiles
                    if ((npcTile % 4) == 1)
                    {
                        render1.material.mainTexture = tiles[npcTile];
                        render2.material.mainTexture = tiles[npcTile + 1];
                        render3.material.mainTexture = tiles[npcTile + 2];
                        render4.material.mainTexture = tiles[npcTile - 1];
                    }
                    else if ((npcTile % 4) == 2)
                    {
                        render1.material.mainTexture = tiles[npcTile];
                        render2.material.mainTexture = tiles[npcTile + 1];
                        render3.material.mainTexture = tiles[npcTile - 2];
                        render4.material.mainTexture = tiles[npcTile - 1];
                    }
                    else if ((npcTile % 4) == 3)
                    {
                        render1.material.mainTexture = tiles[npcTile];
                        render2.material.mainTexture = tiles[npcTile - 3];
                        render3.material.mainTexture = tiles[npcTile - 2];
                        render4.material.mainTexture = tiles[npcTile - 1];
                    }
                    else
                    {
                        render1.material.mainTexture = tiles[npcTile];
                        render2.material.mainTexture = tiles[npcTile + 1];
                        render3.material.mainTexture = tiles[npcTile + 2];
                        render4.material.mainTexture = tiles[npcTile + 3];
                    }

                    // set the shader
                    Shader unlit = Shader.Find("Sprites/Default");

                    render1.material.shader = unlit;
                    render2.material.shader = unlit;
                    render3.material.shader = unlit;
                    render4.material.shader = unlit;

                    // add our little animator script
                    npcGameObject.AddComponent<Animate2>();
                }
                // npc does not have any animation tiles
                else
                {
                    // create child object to display texture
                    GameObject npcGameObject1 = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    npcGameObject1.transform.SetParent(npcGameObject.transform);

                    // rotate the npc game object after creating and addition of child
                    npcGameObject.transform.localPosition = npcLocation;
                    npcGameObject.transform.eulerAngles = new Vector3(90.0f, 0.0f, 180.0f);
                    
                    // create child object for texture
                    MeshRenderer render1 = npcGameObject1.GetComponent<MeshRenderer>();

                    // set the tile
                    render1.material.mainTexture = tiles[npcTile];

                    // set the shader
                    Shader unlit = Shader.Find("Sprites/Default");

                    render1.material.shader = unlit;

                    // don't add the script as this npc does not have any animated tiles
                }
            }
        }
        */

        // rotate entire settlement into place
        transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        //transform.Rotate(90.0f, 0.0f, 0.0f, Space.World);
    }

    void CreateSettlementShadowGameObjects()
    {
        // create three game object under us to hold these sub categories of things
        if (shadow == null)
        {
            shadow = new GameObject("shadow");
            shadow.transform.SetParent(transform);
            shadow.transform.localPosition = Vector3.zero;
            shadow.transform.localRotation = Quaternion.identity;
        }

        // start over each update
        foreach (Transform child in shadow.transform)
        {
            Object.Destroy(child.gameObject);
        }

        for (int height = 0; height < 32; height++)
        {
            for (int width = 0; width < 32; width++)
            {
                GameObject mapTile;
                U4_Decompiled.TILE tileIndex = raycastSettlementMap[width, height];

                // solid object, brick, rocks etc. make into cubes
                if (tileIndex == U4_Decompiled.TILE.BLANK)
                {
                    mapTile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    mapTile.transform.SetParent(shadow.transform);
                    Vector3 location = new Vector3(width, 31 - height, 0.0f);
                    mapTile.transform.localPosition = location;

                    // all terrain is static, used by combine function below to merge static meshes
                    mapTile.isStatic = true;

                    // set the shader
                    Shader unlit = Shader.Find("Mobile/Unlit (Supports Lightmap)");

                    MeshRenderer render = mapTile.GetComponent<MeshRenderer>();

                    render.material.mainTexture = tiles[(int)tileIndex];
                    render.material.shader = unlit;
                }
            }
        }

        transform.position = new Vector3(0, 0, 224);

        // rotate settlement into place
        //transform.Rotate(90.0f, 0.0f, 0.0f, Space.World);
        transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
    }

    // this one will go two layers deep to avoid an implementation that relies on recursion
    GameObject[] GetAllChildrenWithMeshRenderers(GameObject gameObject)
    {
        int count = 0;

        foreach (Transform child in gameObject.transform)
        {
            if (child.transform.GetComponent<MeshRenderer>())
            {
                count++;
            }

            foreach (Transform childofchild in child.transform)
            {
                if (childofchild.transform.GetComponent<MeshRenderer>())
                {
                    count++;
                }
            }
        }

        GameObject[] objectsToCombine = new GameObject[count];

        count = 0;
        foreach (Transform child in gameObject.transform)
        {
            if (child.transform.GetComponent<MeshRenderer>())
            {
                objectsToCombine[count++] = child.gameObject;
            }
            foreach (Transform childofchild in child.transform)
            {
                if (childofchild.transform.GetComponent<MeshRenderer>())
                {
                    objectsToCombine[count++] = childofchild.gameObject;
                }
            }
        }

        return objectsToCombine;
    }

    GameObject[] GetAllChildrenWithMeshRenderers2(GameObject gameObject)
    {
        int count = 0;

        GameObject[] objectsToCheck = new GameObject[gameObject.transform.childCount];
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>())
            {
                count++;
            }
        }

        GameObject[] objectsToCombine = new GameObject[count];
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>())
            {
                objectsToCombine[i] = gameObject.transform.GetChild(i).gameObject;
            }
        }

        return objectsToCombine;
    }
    private int GetTextureSquareSize(GameObject[] o)
    {
        ArrayList textures = new ArrayList();

        // Find unique textures
        for (int i = 0; i < o.Length; i++)
        {
            if (!textures.Contains(o[i].GetComponent<MeshRenderer>().material.mainTexture))
            {
                textures.Add(o[i].GetComponent<MeshRenderer>().material.mainTexture);
            }
        }

        if (textures.Count == 1) return 1;
        if (textures.Count <= 4) return 2;
        if (textures.Count <= 16) return 4;
        if (textures.Count <= 64) return 8;
        if (textures.Count <= 128) return 16;
        if (textures.Count <= 256) return 32;

        // Doesn't handle more than 256 different textures
        return 0;
    }
    private int GetTextureSize(GameObject[] o)
    {
        ArrayList textures = new ArrayList();

        // Find unique textures
        for (int i = 0; i < o.Length; i++)
        {
            MeshRenderer meshRenderer = o[i].GetComponent<MeshRenderer>();

            if (meshRenderer)
            {
                if (!textures.Contains(meshRenderer.material.mainTexture))
                {
                    textures.Add(meshRenderer.material.mainTexture);
                }
            }
        }

        return textures.Count;
    }

    /*
     * Combines all object textures into a single texture then creates a material used by all objects.
     * The materials properties are based on those of the material of the top level object.
     * 
     * Also combines any meshes marked as static into a single mesh.
     * 
     * https://forum.unity.com/threads/combine-textures-and-meshes-reduce-draw-calls.117155/
     * 
     * License is MIT ... you can do whatever you want with this, If you don't want to have to include the MIT license send me a PM and I will waive that requirement. 
     * Links or credits to www.jnamobile.com are appreciated but not required.
     * 
     */
    private void Combine(GameObject gameObject, bool useMipMaps = false, TextureFormat textureFormat = TextureFormat.RGBA32, bool destroy = true)
    {
        int size;
        int originalSize;
        int pow2;
        Texture2D combinedTexture;
        Material material;
        Texture2D texture;
        Mesh mesh;
        Hashtable textureAtlas = new Hashtable();

        GameObject[] objectsToCombine = GetAllChildrenWithMeshRenderers(gameObject);

        // save current position so we can set it to zero so the localToWorldMatrix works correctly below
        Vector3 position = gameObject.transform.position;
        Quaternion rotation = gameObject.transform.rotation;

        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = Quaternion.identity;

        if (objectsToCombine.Length > 1)
        {
            originalSize = objectsToCombine[0].GetComponent<MeshRenderer>().material.mainTexture.width;
            pow2 = GetTextureSquareSize(objectsToCombine);
            size = pow2 * originalSize;
            combinedTexture = new Texture2D(size, size, textureFormat, useMipMaps);
            combinedTexture.filterMode = FilterMode.Point;

            // Create the combined square texture (remember to ensure the total size of the texture isn't
            // larger than the platform supports)
            int index = 0;
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                texture = (Texture2D)objectsToCombine[i].GetComponent<MeshRenderer>().material.mainTexture;
                if (!textureAtlas.ContainsKey(texture))
                {
                    int x = (index % pow2) * originalSize;
                    int y = (index / pow2) * originalSize;

                    combinedTexture.SetPixels(x, y, originalSize, originalSize, texture.GetPixels());

                    x = index % pow2;
                    y = index / pow2;
                    textureAtlas.Add(texture, new Vector2(x, y));
                    index++;
                }
            }
            combinedTexture.Apply();
            material = new Material(objectsToCombine[0].GetComponent<MeshRenderer>().material);
            material.mainTexture = combinedTexture;

            // Update texture co-ords for each mesh (this will only work for meshes with coords betwen 0 and 1).
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                mesh = objectsToCombine[i].GetComponent<MeshFilter>().mesh;
                Vector2[] uv = new Vector2[mesh.uv.Length];
                Vector2 offset;
                if (textureAtlas.ContainsKey(objectsToCombine[i].GetComponent<MeshRenderer>().material.mainTexture))
                {
                    offset = (Vector2)textureAtlas[objectsToCombine[i].GetComponent<MeshRenderer>().material.mainTexture];
                    for (int u = 0; u < mesh.uv.Length; u++)
                    {
                        uv[u] = mesh.uv[u] / (float)pow2;
                        uv[u].x += ((float)offset.x) / (float)pow2;
                        uv[u].y += ((float)offset.y) / (float)pow2;
                    }
                }
                else
                {
                    // This happens if you use the same object more than once, don't do it :)
                }

                mesh.uv = uv;
                objectsToCombine[i].GetComponent<MeshRenderer>().material = material;
            }

            // Combine each mesh marked as static
            int staticCount = 0;
            CombineInstance[] combine = new CombineInstance[objectsToCombine.Length];
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                if (objectsToCombine[i].isStatic)
                {
                    staticCount++;
                    combine[i].mesh = objectsToCombine[i].GetComponent<MeshFilter>().mesh;
                    combine[i].transform = objectsToCombine[i].transform.localToWorldMatrix;
                }
            }

            // Create a mesh filter and renderer
            if (staticCount > 1)
            {
                MeshFilter filter = gameObject.GetComponent<MeshFilter>();
                if (filter == null)
                {
                    filter = gameObject.AddComponent<MeshFilter>();
                }
                MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
                if (renderer == null)
                {
                    renderer = gameObject.AddComponent<MeshRenderer>();
                }
                filter.mesh = new Mesh();
                filter.mesh.CombineMeshes(combine);
                renderer.material = material;

                // Disable all the static object renderers
                for (int i = 0; i < objectsToCombine.Length; i++)
                {
                    if (objectsToCombine[i].isStatic)
                    {
                        if (destroy)
                        {
                            Destroy(objectsToCombine[i]);
                        }
                        else
                        {
                            objectsToCombine[i].GetComponent<MeshFilter>().mesh = null;
                            objectsToCombine[i].GetComponent<MeshRenderer>().material = null;
                            objectsToCombine[i].GetComponent<MeshRenderer>().enabled = false;
                        }
                    }
                }
            }

            Resources.UnloadUnusedAssets();
        }

        // Restore position
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
    }

    // This version creates a horizontal texture atlas so we can do UV animation for things like water and lava.
    private void Combine2(GameObject gameObject, bool useMipMaps = false, TextureFormat textureFormat = TextureFormat.RGBA32, bool destroy = true)
    {
        int size;
        int originalSize;
        int textureCount;
        Texture2D combinedTexture;
        Material material;
        Texture2D texture;
        Mesh mesh;
        Hashtable textureAtlas = new Hashtable();

        GameObject[] objectsToCombine = GetAllChildrenWithMeshRenderers(gameObject);

        // save current position so we can set it to zero so the localToWorldMatrix works correctly below
        Vector3 position = gameObject.transform.position;
        Quaternion rotation = gameObject.transform.rotation;

        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = Quaternion.identity;

        if (objectsToCombine.Length > 1)
        {
            originalSize = objectsToCombine[0].GetComponent<MeshRenderer>().material.mainTexture.width;
            textureCount = GetTextureSize(objectsToCombine);
            size = textureCount * originalSize;
            combinedTexture = new Texture2D(size, originalSize, textureFormat, useMipMaps);
            combinedTexture.filterMode = FilterMode.Point;

            // Create the combined texture (remember to ensure the total size of the texture isn't
            // larger than the platform supports)
            int index = 0;
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                texture = (Texture2D)objectsToCombine[i].GetComponent<MeshRenderer>().material.mainTexture;
                if (!textureAtlas.ContainsKey(texture))
                {
                    int x = index  * originalSize;
                    int y = 0;

                    combinedTexture.SetPixels(x, y, originalSize, originalSize, texture.GetPixels());

                    x = index;
                    y = 0;
                    textureAtlas.Add(texture, new Vector2(x, y));
                    index++;
                }
            }
            combinedTexture.Apply();
            material = new Material(objectsToCombine[0].GetComponent<MeshRenderer>().material);
            material.mainTexture = combinedTexture;

            // Update texture co-ords for each mesh (this will only work for meshes with coords betwen 0 and 1).
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                mesh = objectsToCombine[i].GetComponent<MeshFilter>().mesh;
                Vector2[] uv = new Vector2[mesh.uv.Length];
                Vector2 offset;
                if (textureAtlas.ContainsKey(objectsToCombine[i].GetComponent<MeshRenderer>().material.mainTexture))
                {
                    offset = (Vector2)textureAtlas[objectsToCombine[i].GetComponent<MeshRenderer>().material.mainTexture];
                    for (int u = 0; u < mesh.uv.Length; u++)
                    {
                        uv[u] = mesh.uv[u];
                        uv[u].x = uv[u].x / (float)textureCount;
                        uv[u].x += ((float)offset.x) / (float)textureCount;
                    }
                }
                else
                {
                    // This happens if you use the same object more than once, don't do it :)
                }

                mesh.uv = uv;
                objectsToCombine[i].GetComponent<MeshRenderer>().material = material;
            }

            // Combine each mesh marked as static
            int staticCount = 0;
            CombineInstance[] combine = new CombineInstance[objectsToCombine.Length];
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                if (objectsToCombine[i].isStatic)
                {
                    staticCount++;
                    combine[i].mesh = objectsToCombine[i].GetComponent<MeshFilter>().mesh;
                    combine[i].transform = objectsToCombine[i].transform.localToWorldMatrix;
                }
            }

            // Create a mesh filter and renderer
            if (staticCount > 1)
            {
                MeshFilter filter = gameObject.GetComponent<MeshFilter>();
                if (filter == null)
                {
                    filter = gameObject.AddComponent<MeshFilter>();
                }
                MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
                if (renderer == null)
                {
                    renderer = gameObject.AddComponent<MeshRenderer>();
                }
                filter.mesh = new Mesh();
                filter.mesh.CombineMeshes(combine);
                renderer.material = material;

                // Disable all the static object renderers
                for (int i = 0; i < objectsToCombine.Length; i++)
                {
                    if (objectsToCombine[i].isStatic)
                    {
                        if (destroy)
                        {
                            Destroy(objectsToCombine[i]);
                        }
                        else
                        {
                            objectsToCombine[i].GetComponent<MeshFilter>().mesh = null;
                            objectsToCombine[i].GetComponent<MeshRenderer>().material = null;
                            objectsToCombine[i].GetComponent<MeshRenderer>().enabled = false;
                        }
                    }
                }
            }

            Resources.UnloadUnusedAssets();
        }

        // Restore position
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
    }

    // Start is called before the first frame update
   // void Start()
   void Awake()
    {
        InitializeEGAPalette();
        LoadTilesEGA();

        //InitializeCGAPalette();
        //LoadTilesCGA();

        LoadSettlementTalkAndMap();


        // enter town start position
        //raycast(1, 15);
        // enter castle start position
        raycast(0x0f, 0x1e);

        CreateSettlementGameObjects(raycastSettlementMap);

        // Position the settlement in place
        transform.position = new Vector3(0, 0, 224);

        // rotate settlement into place
        //transform.Rotate(90.0f, 0.0f, 0.0f, Space.World);
        transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

        if (terrain)
        {
            Combine(terrain.gameObject);
            Shader unlit = Shader.Find("Mobile/Unlit (Supports Lightmap)");
            MeshRenderer render = terrain.GetComponent<MeshRenderer>();
            if (render)
            {
                render.material.shader = unlit;
            }
        }

        /*
        if (npcs)
        {
            Combine(npcs.gameObject);
        }
        */

        if (animatedTerrrain)
        { 
            Combine2(animatedTerrrain.gameObject);
            Shader unlit = Shader.Find("UI/Unlit/Detail");
            MeshRenderer render = animatedTerrrain.GetComponent<MeshRenderer>();
            if (render)
            {
                render.material.shader = unlit;
            }
        }
    }


    // cast one ray
    void Cast_Ray(sbyte diff_x, sbyte diff_y, byte pos_x, byte pos_y)
    {
        U4_Decompiled.TILE temp_tile;

        // are we outside the area we want to check
        if (pos_x > 31 || pos_y > 31)
        {
            return;
        }

        // is the tile already been copied
        if (raycastSettlementMap[pos_x, pos_y] != U4_Decompiled.TILE.BLANK)
        {
            return;
        }

        // get the tile and copy it to the raycast map
        temp_tile = settlementMap[pos_x, pos_y];
        raycastSettlementMap[pos_x, pos_y] = temp_tile;

        // check the tile for opaque tiles
        if ((temp_tile == U4_Decompiled.TILE.FOREST) ||
            (temp_tile == U4_Decompiled.TILE.MOUNTAINS) ||
            (temp_tile == U4_Decompiled.TILE.BLANK) ||
            (temp_tile == U4_Decompiled.TILE.SECRET_BRICK_WALL) ||
            (temp_tile == U4_Decompiled.TILE.BRICK_WALL))
        {
            return;
        }

        // continue the ray cast recursively
        pos_x = (byte)(pos_x + diff_x);
        pos_y = (byte)(pos_y + diff_y);
        Cast_Ray(diff_x, diff_y, pos_x, pos_y);
        if ((diff_x & diff_y) != 0)
        {
            Cast_Ray(diff_x, diff_y, pos_x, (byte)(pos_y - diff_y));
            Cast_Ray(diff_x, diff_y, (byte)(pos_x - diff_x), pos_y);
        }
        else
        {
            Cast_Ray((sbyte)(((diff_x != 0) ? 1 : 0) * diff_y + diff_x), (sbyte)(diff_y - ((diff_y != 0) ? 1 : 0) * diff_x), (byte)(diff_y + pos_x), (byte)(pos_y - diff_x));
            Cast_Ray((sbyte)(diff_x - ((diff_x != 0) ? 1 : 0) * diff_y), (sbyte)(((diff_y != 0) ? 1 : 0) * diff_x + diff_y), (byte)(pos_x - diff_y), (byte)(diff_x + pos_y));
        }
    }

    public U4_Decompiled.TILE[,] raycastSettlementMap = new U4_Decompiled.TILE[32, 32];

    // visible area (raycast)
    void raycast(int pos_x, int pos_y)
    {
        // set all visible tiles to blank to start
        for (int i = 0; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                raycastSettlementMap[i, j] = U4_Decompiled.TILE.BLANK;
            }
        }

        raycastSettlementMap[pos_x, pos_y] = settlementMap[pos_x, pos_y]; // copy the center party's tile as it is always visible

//        if (pos_y > 1)
        {
            Cast_Ray(0, -1, (byte)pos_x, (byte)(pos_y - 1)); // Cast a ray UP
        }
        if (pos_y < 31)
        {
            Cast_Ray(0, 1, (byte)pos_x, (byte)(pos_y + 1)); // Cast a ray DOWN
        }
//        if (pos_x > 1)
        {
            Cast_Ray(-1, 0, (byte)(pos_x - 1), (byte)pos_y); // Cast a ray LEFT
        }
//        if (pos_x < 31)
        {
            Cast_Ray(1, 0, (byte)(pos_x + 1), (byte)pos_y); // Cast a ray RIGHT
        }
//        if ((pos_x < 31) && (pos_y < 31))
        {
            Cast_Ray(1, 1, (byte)(pos_x + 1), (byte)(pos_y + 1)); // Cast a ray DOWN and to the RIGHT
        }
 //       if ((pos_x < 31) && (pos_y > 1))
        {
            Cast_Ray(1, -1, (byte)(pos_x + 1), (byte)(pos_y - 1)); // Cast a ray UP and to the RIGHT
        }
 //       if ((pos_x > 1) && (pos_y < 31))
        {
            Cast_Ray(-1, 1, (byte)(pos_x - 1), (byte)(pos_y + 1)); // Cast a ray DOWN and to the LEFT
        }
  //      if ((pos_x > 1) && (pos_y > 1))
        {
            Cast_Ray(-1, -1, (byte)(pos_x - 1), (byte)(pos_y - 1)); // Cast a ray UP and to the LEFT
        }
    }

    public U4_Decompiled u4;

    int lastPlayer_posx = 0;
    int lastPlayer_posy = 0;

    private void Start()
    {
        // get a reference to the game engine
        u4 = FindObjectOfType<U4_Decompiled>();
    }

    // Update is called once per frame
    void Update()
    {
        // we've moved, regenerate the raycast and resulting mesh
        if ((u4.Party._x != lastPlayer_posx) || (u4.Party._y != lastPlayer_posy))
        {
            // generate a new raycast
            raycast(u4.Party._x, u4.Party._y);
            // create the game objects with meshes and textures
            CreateSettlementGameObjects(raycastSettlementMap);

            // combine the meshes and textures for terrain
            Combine(terrain.gameObject);
            // combine the meshes and textures for water
            Combine2(animatedTerrrain.gameObject);

            // Position the settlement in place
            transform.position = new Vector3(0, 0, 224);

            // rotate settlement into place
            transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

            // update the lst position
            lastPlayer_posx = u4.Party._x;
            lastPlayer_posy = u4.Party._y;
        }
    }
}
