using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    // used for automatic klimb and decsend ladders
    public Tile.TILE lastCurrentTile;

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
    public GameObject partyGameObject;
    public GameObject skyGameObject;

    //public GameObject[] Settlements;
    public GameObject[] CombatTerrains;
    public GameObject CenterOfCombatTerrain;

    public List<string> talkWordList = new List<string>();

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
    Tile.TILE[,] raycastSettlementMap = new Tile.TILE[64, 64];

    // this array size can be adjusted to display more or less of the map at runtime
    Tile.TILE[,] raycastOutdoorMap = new Tile.TILE[128, 128];
 //   U4_Decompiled.TILE[,] raycastOutdoorMap = new U4_Decompiled.TILE[256, 256];

   

    // unfortuantly the game engine never saves this information after loading the combat terrain in function C_7C65()
    // the code is not re-entrant so I cannot just expose and call the function directly so I need to re-implement the 
    // logic here so I can on the fly determine the combat terrain to display by exposing the interal variables used in the
    // original function. The INN or shop or camp case is handled elsewhere (e.g. In the middle of the night, while out for a stroll...)

    public U4_Decompiled_AVATAR.COMBAT_TERRAIN Convert_Tile_to_Combat_Terrian(Tile.TILE tile)
    {
        U4_Decompiled_AVATAR.COMBAT_TERRAIN combat_terrain;


        if (u4.Party._tile <= Tile.TILE.SHIP_SOUTH || (Tile.TILE)((byte)u4.current_tile & ~3) == Tile.TILE.SHIP_WEST)
        {
            if (u4.D_96F8 == Tile.TILE.PIRATE_WEST)
            {
                combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.SHIPSHIP;
            }
            else if (u4.D_946C <= Tile.TILE.SHALLOW_WATER)
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
            if (u4.D_96F8 == Tile.TILE.PIRATE_WEST)
            {
                combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.SHORSHIP;
            }
            else if (u4.D_946C <= Tile.TILE.SHALLOW_WATER)
            {
                combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.SHORE;
            }
            else
            {
                switch (tile)
                {
                    case Tile.TILE.SWAMP:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.MARSH;
                            break;
                        }
                    case Tile.TILE.BRUSH:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.BRUSH;
                            break;
                        }
                    case Tile.TILE.FOREST:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.FOREST;
                            break;
                        }
                    case Tile.TILE.HILLS:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.HILL;
                            break;
                        }
                    case Tile.TILE.DUNGEON:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.DUNGEON;
                            break;
                        }
                    case Tile.TILE.BRICK_FLOOR:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.BRICK;
                            break;
                        }
                    case Tile.TILE.BRIDGE:
                    case Tile.TILE.BRIDGE_TOP:
                    case Tile.TILE.BRIDGE_BOTTOM:
                    case Tile.TILE.WOOD_FLOOR:
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
        ref Tile.TILE[,] combatMap, 
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
                combatMap[x, y] = (Tile.TILE)combatMapFileData[fileIndex++];
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
            combatMaps[i] = new Tile.TILE[11, 11];
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
            Map.CreateMap(gameObject, combatMaps[i]);

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
    Tile.TILE[,] entireMapTILEs = new Tile.TILE[32 * 8, 32 * 8];

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
                        entireMapTILEs[x * 32 + width, y * 32 + height] = (Tile.TILE)worldMapFileData[fileIndex++];
                    }
                }
            }
        }
    }

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
        public Tile.TILE tile;
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
    public Tile.TILE[][,] settlementMap = new Tile.TILE[(int)SETTLEMENT.MAX][,]; //32,32

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
            settlementMap[settlement] = new Tile.TILE[32,32];

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
                    Tile.TILE tileIndex = (Tile.TILE)settlementFileData[bufferIndex++];
                    settlementMap[settlement][width, height] = tileIndex;
                }
            }

            // load npc data from the map data
            for (int npcIndex = 0; npcIndex < 32; npcIndex++)
            {
                Tile.TILE npcTile = (Tile.TILE)settlementFileData[0x400 + npcIndex];
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
        renderer.material.mainTexture = Tile.expandedTiles[(int)Tile.TILE.PARTY];
        renderer.material.mainTextureOffset = new Vector2((float)Tile.TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)Tile.TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
        renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * Tile.TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * Tile.TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);

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



    public void followWorld(GameObject follow)
    {
        // hook the player game object into the camera and the game engine
        MySmoothFollow myScript = FindObjectsOfType<MySmoothFollow>()[0];

        if (myScript.target != follow.transform)
        {
            myScript.target = follow.transform;
        }
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
            Tile.TILE npcTile = currentFighters[fighterIndex]._tile;
            Tile.TILE npcCurrentTile = currentFighters[fighterIndex]._gtile;

            // get the corresponding fighters game object
            Transform childoffighters = fighters.transform.GetChild(fighterIndex);

            if (npcTile == Tile.TILE.DEEP_WATER)
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
                childoffighters.GetComponent<Animate3>().SetNPCTile(Tile.TILE.SLEEP);
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
            Tile.TILE npcTile;

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

            if (npcTile == Tile.TILE.DEEP_WATER)
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

        renderer.material.mainTexture = Tile.originalTiles[(int)u4.moongate_tile];

        // get adjusted position based on the offset of the raycastOutdoorMap due to the player position
        int posx = u4.moongate_x - (lastRaycastPlayer_posx - raycastOutdoorMap.GetLength(0) / 2 - 1);
        int posy = u4.moongate_y - (lastRaycastPlayer_posy - raycastOutdoorMap.GetLength(1) / 2 - 1);
        // can we see the npc
        if (posx < 0 || posy < 0 || posx >= raycastOutdoorMap.GetLength(0) || posy >= raycastOutdoorMap.GetLength(1) || raycastOutdoorMap[posx, posy] == Tile.TILE.BLANK)
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
            Tile.TILE npcTile = currentNpcs[npcIndex]._tile;
            Tile.TILE npcCurrentTile = currentNpcs[npcIndex]._gtile;

            // check if npc is active
            if (npcTile == Tile.TILE.DEEP_WATER)
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
                    if ((u4.Party._loc == U4_Decompiled_AVATAR.LOCATIONS.BRITANNIA) && (u4.tMap32x32[3, 3] == Tile.TILE.LADDER_UP))
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
                    if (posx < 0 || posy < 0 || posx >= raycastSettlementMap.GetLength(0) || posy >= raycastSettlementMap.GetLength(1) || raycastSettlementMap[posx, posy] == Tile.TILE.BLANK)
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
                    if (posx < 0 || posy < 0 || posx >= raycastOutdoorMap.GetLength(0) || posy >= raycastOutdoorMap.GetLength(1) || raycastOutdoorMap[posx, posy] == Tile.TILE.BLANK)
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
                Tile.TILE hitTile = currentHitList[hitIndex].tile;

                // update the tile of the game object
                childofhits.GetComponent<Renderer>().material.mainTexture = Tile.originalTiles[(int)hitTile];

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


    // changes in these require redrawing the map
    int lastRaycastPlayer_posx = -1;
    int lastRaycastPlayer_posy = -1;
    int lastRaycastPlayer_f_1dc = -1;
    U4_Decompiled_AVATAR.DIRECTION lastRaycastP_surface_party_direction = (U4_Decompiled_AVATAR.DIRECTION )(-1);
    bool last_door_timer = false;

    // create a temp TILE map array to hold the combat terrains as we load them
    Tile.TILE[][,] combatMaps = new Tile.TILE[(int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX][,];
    CombatMonsterStartPositions[][] combatMonsterStartPositions = new CombatMonsterStartPositions[(int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX][];
    CombatPartyStartPositions[][] combatPartyStartPositions = new CombatPartyStartPositions[(int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX][];

    public Image vision;
    Texture2D visionTexture;

    //public Texture2D[] picture1;
    //public Texture2D[] picture2;
    //public Texture2D[] picture3;
    //public Texture2D[] picture4;
   
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
        Dungeon.dungeon = new GameObject("dungeon");
        Dungeon.dungeon.transform.SetParent(transform);
        Dungeon.dungeon.transform.localPosition = Vector3.zero;
        Dungeon.dungeon.transform.localRotation = Quaternion.identity;
        Dungeon.dungeonMonsters = new GameObject("dungeon monsters");
        Dungeon.dungeonMonsters.transform.SetParent(transform);
        Dungeon.dungeonMonsters.transform.localPosition = Vector3.zero;
        Dungeon.dungeonMonsters.transform.localRotation = Quaternion.identity;

        // initialize the palette and load the tiles
        Palette.InitializeEGAPalette();
        Palette.InitializeCGAPalette();
        Palette.InitializeApple2Palette();
        Tile.LoadTilesEGA();
        //Tile.LoadTilesCGA();
        //Tile.LoadTilesApple2();
        //Tile.LoadTilesPNG();

        // fix a tile
        Tile.FixMageTile3();

        // expand the tiles
        Tile.ExpandTiles();

        // create texture atlas
        Tile.CreateLinearTextureAtlas(ref Tile.originalTiles);
        Tile.CreateSquareTextureAtlas(ref Tile.originalTiles);
        Tile.CreateExpandedTextureAtlas(ref Tile.expandedTiles);

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
        Dungeon.LoadDungeons();

        // create the part game object
        CreateParty();

        // Create the combat terrains
        CreateCombatTerrains();

        // get a reference to the game engine
        u4 = FindObjectOfType<U4_Decompiled_AVATAR>();

        // initialize hidden map
        hiddenWorldMapGameObject = new GameObject("Hidden World Map");
        Map.CreateMapSubsetPass2(hiddenWorldMapGameObject, ref entireMapTILEs, ref entireMapGameObjects);

        GameObject hiddenSettlementsMaps = new GameObject("Hidden Settlements Maps");
        for (int i = 0; i < (int)SETTLEMENT.MAX; i++)
        {
            GameObject settlementGameObject = new GameObject(((SETTLEMENT)i).ToString());
            settlementGameObject.transform.SetParent(hiddenSettlementsMaps.transform);
            settlementsMapGameObjects[i] = new GameObject[32, 32];
            Map.CreateMapSubsetPass2(settlementGameObject, ref settlementMap[i], ref settlementsMapGameObjects[i], true);
            //CreateMapLabels(settlementGameObject, ref settlementMap[i]);
        }

        // set the vision to blank
        vision.sprite = null;
        vision.color = new Color(0f, 0f, 0f, 0f);

        // allocate vision texture that we can overlap pictures onto
        visionTexture = new Texture2D(320, 200);
        Picture.ClearTexture(visionTexture, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BLACK]);

        // everything I need it now loaded, start the game engine thread
        u4.StartThread();

        //GameObject dungeonExpandedLevelGameObject = CreateDungeonExpandedLevel(DUNGEONS.HYTHLOTH, 4);

        // Some test stuff, can commented out as needed

        /*
        picture1 = new Texture2D[(int)PICTURE.MAX];
        picture2 = new Texture2D[(int)PICTURE.MAX];

        picture3 = new Texture2D[(int)PICTURE2.MAX];
        picture4 = new Texture2D[(int)PICTURE2.MAX];


        for (int i = 0; i < (int)PICTURE.MAX; i++)
        {
            picture1[i] = new Texture2D(320, 200);
            Picture.ClearTexture(picture1[i], Palette.CGAColorPalette[(int)Palette.CGA_COLOR.BLACK]);
            Picture.LoadAVATARPicFile(((PICTURE)i).ToString() + ".PIC", picture1[i]);
            picture2[i] = new Texture2D(320, 200);
            Picture.ClearTexture(picture2[i], Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BLACK]);
            Picture.LoadAVATAREGAFile(((PICTURE)i).ToString() + ".EGA", picture2[i]);
        }

        for (int i = (int)PICTURE.TRUTH; i <= (int)PICTURE.HUMILITY; i++)
        {
            Picture.LoadAVATARPicFile(((PICTURE)i).ToString() + ".PIC", picture1[(int)PICTURE.TRUTH]);
        }

        for (int i = (int)PICTURE.TRUTH; i <= (int)PICTURE.HUMILITY; i++)
        {
            Picture.LoadAVATAREGAFile(((PICTURE)i).ToString() + ".EGA", picture2[(int)PICTURE.TRUTH]);
        }

        for (int i = 0; i < (int)PICTURE2.MAX; i++)
        {
            picture3[i] = new Texture2D(320, 200);
            Picture.ClearTexture(picture3[i], Palette.CGAColorPalette[(int)Palette.CGA_COLOR.BLACK]);
            Picture.LoadTITLEPicPictureFile(((PICTURE2)i).ToString() + ".PIC", picture3[i]);
            picture4[i] = new Texture2D(320, 200);
            Picture.ClearTexture(picture4[i], Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BLACK]);
            Picture.LoadTITLEEGAPictureFile(((PICTURE2)i).ToString() + ".EGA", picture4[i]);
        }
        */

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
            if (Tile.textureExpandedAtlasPowerOf2 != 0)
            {
                Tile.AnimateFlags();
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
                if ((u4.Party._loc == U4_Decompiled_AVATAR.LOCATIONS.BRITANNIA) && (u4.tMap32x32[3, 3] == Tile.TILE.LADDER_UP))
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
                Dungeon.dungeon.SetActive(false);
                Dungeon.dungeonMonsters.SetActive(false);
                skyGameObject.SetActive(true);

                for (int i = 0; i < (int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX; i++)
                {
                    CombatTerrains[i].gameObject.SetActive(false);
                }

                // automatically enter things when you are on an enterable tile unless just left somewhere or you are flying in the balloon
                if ((readyToAutomaticallyEnter == true) && (u4.Party.f_1dc == 0) &&
                    ((u4.current_tile == Tile.TILE.CASTLE_ENTRANCE) ||
                    (u4.current_tile == Tile.TILE.CASTLE) ||
                    (u4.current_tile == Tile.TILE.TOWN) ||
                    (u4.current_tile == Tile.TILE.VILLAGE) ||
                    (u4.current_tile == Tile.TILE.DUNGEON) ||
                    (u4.current_tile == Tile.TILE.RUINS) ||
                    (u4.current_tile == Tile.TILE.SHRINE)))
                {
                    u4.CommandEnter();
                    readyToAutomaticallyEnter = false;
                }

                // wait until we move off of an entrance tile after leaving somewhere
                if ((wasJustInside == true) &&
                    (u4.current_tile != Tile.TILE.CASTLE_ENTRANCE) &&
                    (u4.current_tile != Tile.TILE.CASTLE) &&
                    (u4.current_tile != Tile.TILE.TOWN) &&
                    (u4.current_tile != Tile.TILE.VILLAGE) &&
                    (u4.current_tile != Tile.TILE.DUNGEON) &&
                    (u4.current_tile != Tile.TILE.RUINS) &&
                    (u4.current_tile!= Tile.TILE.SHRINE))
                {
                    readyToAutomaticallyEnter = true;
                    wasJustInside = false;
                }

                // automatically board horse, ship and balloon
                if (((u4.current_tile == Tile.TILE.HORSE_EAST) ||
                    (u4.current_tile == Tile.TILE.HORSE_EAST) ||
                    (u4.current_tile == Tile.TILE.SHIP_EAST) ||
                    (u4.current_tile == Tile.TILE.SHIP_NORTH) ||
                    (u4.current_tile == Tile.TILE.SHIP_WEST) ||
                    (u4.current_tile == Tile.TILE.SHIP_SOUTH) ||
                    (u4.current_tile == Tile.TILE.BALOON)) && 
                    (lastCurrentTile != Tile.TILE.HORSE_EAST) &&
                    (lastCurrentTile != Tile.TILE.HORSE_EAST) &&
                    (lastCurrentTile != Tile.TILE.SHIP_EAST) &&
                    (lastCurrentTile != Tile.TILE.SHIP_NORTH) &&
                    (lastCurrentTile != Tile.TILE.SHIP_WEST) &&
                    (lastCurrentTile != Tile.TILE.SHIP_SOUTH) &&
                    (lastCurrentTile != Tile.TILE.BALOON)  && 
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
                Dungeon.dungeon.SetActive(false);
                Dungeon.dungeonMonsters.SetActive(false);
                skyGameObject.SetActive(true);

                for (int i = 0; i < (int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX; i++)
                {
                    CombatTerrains[i].gameObject.SetActive(false);
                }

                // automatic Klimb and Descend ladders
                if ((u4.current_tile == Tile.TILE.LADDER_UP) &&
                    (lastCurrentTile != Tile.TILE.LADDER_UP) &&
                    (lastCurrentTile != Tile.TILE.LADDER_DOWN))
                {
                    u4.CommandKlimb();
                }

                // automatic Klimb and Descend ladders
                if ((u4.current_tile == Tile.TILE.LADDER_DOWN) &&
                    (lastCurrentTile != Tile.TILE.LADDER_UP) &&
                    (lastCurrentTile != Tile.TILE.LADDER_DOWN))
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
                    Dungeon.dungeonMonsters.SetActive(false);
                    skyGameObject.SetActive(false);

                    // check if we have the dungeon already created, create it if not
                    Dungeon.DUNGEONS dun = (Dungeon.DUNGEONS)((int)u4.Party._loc - (int)U4_Decompiled_AVATAR.LOCATIONS.DUNGEONS);
                    if (Dungeon.dungeon.name != dun.ToString() + " Level #" + u4.Party._z)
                    {
                        Destroy(Dungeon.dungeon);
                        Dungeon.dungeon = Dungeon.CreateDungeonExpandedLevel(dun, u4.Party._z, combatMaps);
                    }
                    Dungeon.dungeon.SetActive(true);

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
                    Dungeon.dungeon.SetActive(false);
                    Dungeon.dungeonMonsters.SetActive(false);
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
                Dungeon.dungeonMonsters.SetActive(false);
                skyGameObject.SetActive(false);

                // check if we have the dungeon already created, create it if not
                Dungeon.DUNGEONS dun = (Dungeon.DUNGEONS)((int)u4.Party._loc - (int)U4_Decompiled_AVATAR.LOCATIONS.DUNGEONS);
                if (Dungeon.dungeon.name != dun.ToString() + " Level #" + u4.Party._z)
                {
                    Destroy(Dungeon.dungeon);
                    Dungeon.dungeon = Dungeon.CreateDungeonExpandedLevel(dun, u4.Party._z, combatMaps);
                }
                Dungeon.dungeon.SetActive(true);

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
                Dungeon.AddDungeonMapMonsters(u4);
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
                Dungeon.DUNGEONS dun = (Dungeon.DUNGEONS)((int)u4.Party._loc - (int)U4_Decompiled_AVATAR.LOCATIONS.DUNGEONS);
                if (Dungeon.dungeon.name != dun.ToString() + " Level #" + u4.Party._z)
                {
                    // not the right dungeon, create a new dungeon
                    Destroy(Dungeon.dungeon);
                    Dungeon.dungeon = Dungeon.CreateDungeonExpandedLevel(dun, u4.Party._z, combatMaps);
                }

                if (u4.Party.f_1dc > 0) // torch active
                {
                    Dungeon.dungeon.SetActive(true);
                    Dungeon.dungeonMonsters.SetActive(true);
                }
                else
                {
                    Dungeon.dungeon.SetActive(false);
                    Dungeon.dungeonMonsters.SetActive(false);
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
                Dungeon.dungeon.SetActive(false);
                Dungeon.dungeonMonsters.SetActive(false);
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
                Dungeon.dungeon.SetActive(false);
                Dungeon.dungeonMonsters.SetActive(false);
                followWorld(activeCharacter);

                int currentCombatTerrain;
                // need to special case the combat when in the inn and in combat camp mode outside or in dungeon
                if (u4.current_tile == Tile.TILE.BRICK_FLOOR)
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

            if ((party != null) && (Tile.originalTiles != null))
            {
                // set the party tile, person, horse, ballon, ship, etc.
                Renderer renderer = party.GetComponentInChildren<Renderer>();
                if (renderer)
                {
                    party.GetComponentInChildren<Renderer>().material.mainTexture = Tile.expandedTiles[(int)u4.Party._tile];
                    party.name = u4.Party._tile.ToString();

                    if ((u4.Party._tile == Tile.TILE.BALOON) && (u4.Party.f_1dc == 1))
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
                            statsOverview.GetComponent<UnityEngine.UI.Text>().text += GameFont.highlight((i + 1) + "-" + u4.Party.chara[i].name.PadRight(18, ' ') + u4.Party.chara[i].hitPoint.ToString().PadLeft(4, ' ') + (char)(u4.Party.chara[i].state) + '\n');
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

                if ((u4.Party._tile == Tile.TILE.SHIP_EAST) ||
                    (u4.Party._tile == Tile.TILE.SHIP_WEST) ||
                    (u4.Party._tile == Tile.TILE.SHIP_NORTH) ||
                    (u4.Party._tile == Tile.TILE.SHIP_SOUTH))
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
                        Picture.LoadAVATAREGAFile(u4.visionFilename.Replace(".pic", ".EGA"), visionTexture);
                        vision.sprite = Sprite.Create(visionTexture, new Rect(0.0f, 0.0f, visionTexture.width, visionTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                        vision.color = new Color(255f, 255f, 255f, 255f);

                        lastVisionFilename = u4.visionFilename;
                    }
                    else
                    {
                        vision.sprite = null;
                        vision.color = new Color(0f, 0f, 0f, 0f);
                        Picture.ClearTexture(visionTexture, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BLACK]);
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
                Map.raycast(ref entireMapTILEs,
                    u4.Party._x, 
                    u4.Party._y,
                    ref raycastOutdoorMap, 
                    ((u4.Party._x - raycastOutdoorMap.GetLength(0) / 2 - 1) ) ,
                    ((u4.Party._y - raycastOutdoorMap.GetLength(1) / 2 - 1) ) , 
                    Tile.TILE.BLANK);
                location = new Vector3(
                    ((u4.Party._x - raycastOutdoorMap.GetLength(0) / 2) - 1) , 0, 
                    entireMapTILEs.GetLength(1) - ((u4.Party._y - raycastOutdoorMap.GetLength(1) / 2 - 1) ) - raycastOutdoorMap.GetLength(1));
            }
            else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.BUILDING)
            {
                // generate a new raycast based on game engine map
                Map.raycast(ref u4.tMap32x32, 
                    u4.Party._x, 
                    u4.Party._y, 
                    ref raycastSettlementMap, 
                    ((u4.Party._x - raycastSettlementMap.GetLength(0) / 2 - 1) ), 
                    ((u4.Party._y - raycastSettlementMap.GetLength(1) / 2 - 1) ), 
                    Tile.TILE.GRASS);
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
                    Tile.combinedExpandedMaterial,
                    Tile.combinedLinearMaterial,
                    u4.Party._x,
                    u4.Party._y,
                    u4.surface_party_direction);
                    
                location = Vector3.zero;
            }
            else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.BUILDING)
            {
                    
                SETTLEMENT settlement;
                // get the current settlement, need to special case BRITANNIA as the castle has two levels, use the ladder to determine which level
                if ((u4.Party._loc == U4_Decompiled_AVATAR.LOCATIONS.BRITANNIA) && (u4.tMap32x32[3, 3] == Tile.TILE.LADDER_UP))
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
                    Tile.combinedExpandedMaterial,
                    Tile.combinedLinearMaterial,
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
