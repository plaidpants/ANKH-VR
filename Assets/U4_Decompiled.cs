using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;

public class U4_Decompiled : MonoBehaviour
{
    private Thread trd;

    public GameObject textGameObject;

    // tiles
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
        HORSE_WEST = 0x14,
        HORSE_EAST = 0x15,

        /*tiled floor*/
        TILED_FLOOR = 0x16,
        /*bridge*/
        BRIDGE = 0x17,
        /*balloon*/
        BALOON = 0x18,
        /**/
        BRIDGE_TOP = 0x19,
        /**/
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

        /*mage*/
        MAGE = 0x20,
        MAGE2 = 0x21,
        /*bard*/
        BARD = 0x22,
        BARD2 = 0x21,
        /*fighter*/
        FIGHTER = 0x24,
        FIGHTER2 = 0x25,
        /*druid*/
        DRUID = 0x26,
        DRUID2 = 0x27,
        /*tinker*/
        TINKER = 0x28,
        TINKER2 = 0x29,
        /*paladin*/
        PALADIN = 0x2A,
        PALADIN2 = 0x2B,
        /*ranger*/
        RANGER = 0x2C,
        RANGER2 = 0x2D,
        /*shepherd*/
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

        /*guard*/
        GUARD = 0x50,
        GUARD2 = 0x51,
        /*merchant*/
        MERCHANT = 0x52,
        MERCHANT2 = 0x53,
        /*bard*/
        BARD_NPC = 0x54,
        BARD_NPC2 = 0x55,
        /*jester*/
        JESTER = 0x56,
        JESTER2 = 0x57,
        /*beggar*/
        BEGGAR = 0x58,
        BEGGAR2 = 0x59,
        /*child*/
        CHILD = 0x5A,
        CHILD2 = 0x5B,
        /*bull*/
        BULL = 0x5C,
        BULL2 = 0x5D,
        /*lord british*/
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

        /*<space>*/
        SPACE = 0x7A,

        /* brackets */
        BRACKET_RIGHT = 0x7B,
        BRACKET_LEFT = 0x7C,
        BRACKET_SQUARE = 0x7D,
        BLANK = 0x7E,
        BRICK_WALL = 0x7F,

        /* 2-tile monsters */

        /*pirate W N E S*/
        PIRATE = 0x80,
        PIRATE_WEST = 0x80,
        PIRATE_NORTH = 0x81,
        PIRATE_EAST = 0x82,
        PIRATE_SOUTH = 0x83,
        /*nixie*/
        NIXIE = 0x84,
        NIXIE2 = 0x85,
        /*squid*/
        SQUID = 0x86,
        SQUID2 = 0x87,
        /*serpent*/
        SERPENT = 0x88,
        SERPENT2 = 0x89,
        /*seahorse*/
        SEAHORSE = 0x8A,
        SEAHORSE2 = 0x8B,
        /*whirlpool*/
        WHIRLPOOL = 0x8C,
        WHIRLPOOL2 = 0x8D,
        /*twister*/
        WATER_SPOUT = 0x8E,
        WATER_SPOUT2 = 0x8F,

        /* 4-tile monsters */

        /*rat*/
        RAT = 0x90,
        RAT2 = 0x91,
        RAT3 = 0x92,
        RAT4 = 0x93,
        /*bat*/
        BAT = 0x94,
        BAT2 = 0x95,
        BAT3 = 0x96,
        BAT4 = 0x97,
        /*spider*/
        SPIDER = 0x98,
        SPIDER2 = 0x99,
        SPIDER3 = 0x9a,
        SPIDER4 = 0x9b,
        /*ghost*/
        GHOST = 0x9C,
        GHOST2 = 0x9D,
        GHOST3 = 0x9E,
        GHOST4 = 0x9F,
        /*slime*/
        SLIME = 0xA0,
        SLIME2 = 0xA1,
        SLIME3 = 0xA2,
        SLIME4 = 0xA3,
        /*troll*/
        TROLL = 0xA4,
        TROLL2 = 0xA5,
        TROLL3 = 0xA6,
        TROLL4 = 0xA7,
        /*gremlin*/
        GREMLIN = 0xA8,
        GREMLIN2 = 0xA9,
        GREMLIN3 = 0xAa,
        GREMLIN4 = 0xAb,
        /*mimic*/
        MIMIC = 0xAC,
        MIMIC2 = 0xAd,
        MIMIC3 = 0xAe,
        MIMIC4 = 0xAf,
        /*reaper*/
        REAPER = 0xB0,
        REAPER2 = 0xB1,
        REAPER3 = 0xB2,
        REAPER4 = 0xB3,
        /*insects*/
        INSECTS = 0xB4,
        INSECTS2 = 0xB5,
        INSECTS3 = 0xB6,
        INSECTS4 = 0xB7,
        /*gazer*/
        GAZER = 0xB8,
        GAZER2 = 0xB9,
        GAZER3 = 0xBa,
        GAZER4 = 0xBb,
        /*phantom*/
        PHANTOM = 0xBC,
        PHANTOM2 = 0xBD,
        PHANTOM3 = 0xBE,
        PHANTOM4 = 0xBF,
        /*orc*/
        ORC = 0xC0,
        ORC2 = 0xC1,
        ORC3 = 0xC2,
        ORC4 = 0xC3,
        /*skeleton*/
        SKELETON = 0xC4,
        SKELETON2 = 0xC5,
        SKELETON3 = 0xC6,
        SKELETON4 = 0xC7,
        /*rogue*/
        ROGUE = 0xC8,
        ROGUE2 = 0xC9,
        ROGUE3 = 0xCa,
        ROGUE4 = 0xCb,
        /*python*/
        PYTHON = 0xCC,
        PYTHON2 = 0xCd,
        PYTHON3 = 0xCe,
        PYTHON4 = 0xCf,
        /*ettin*/
        ETTIN = 0xD0,
        ETTIN2 = 0xD1,
        ETTIN3 = 0xD2,
        ETTIN4 = 0xD3,
        /*headless*/
        HEADLESS = 0xD4,
        HEADLESS2 = 0xD5,
        HEADLESS3 = 0xD6,
        HEADLESS4 = 0xD7,
        /*cyclops*/
        CYCLOPS = 0xD8,
        CYCLOPS2 = 0xD9,
        CYCLOPS3 = 0xDa,
        CYCLOPS4 = 0xDb,
        /*wisp*/
        WISP = 0xDC,
        WISP2 = 0xDD,
        WISP3 = 0xDE,
        WISP4 = 0xDF,
        /*mage*/
        MAGE_NPC = 0xE0,
        MAGE_NPC2 = 0xE1,
        MAGE_NPC3 = 0xE2,
        MAGE_NPC4 = 0xE3,
        /*lyche*/
        LYCHE = 0xE4,
        LYCHE2 = 0xE5,
        LYCHE3 = 0xE6,
        LYCHE4 = 0xE7,
        /*lava lizard*/
        LAVA_LIZARD = 0xE8,
        LAVA_LIZARD2 = 0xE9,
        LAVA_LIZARD3 = 0xEa,
        LAVA_LIZARD4 = 0xEb,
        /*zorn*/
        ZORN = 0xEC,
        ZORN2 = 0xEd,
        ZORN3 = 0xEe,
        ZORN4 = 0xEf,
        /*daemon*/
        DAEMON = 0xF0,
        DAEMON2 = 0xF1,
        DAEMON3 = 0xF2,
        DAEMON4 = 0xF3,
        /*hydra*/
        HYDRA = 0xF4,
        HYDRA2 = 0xF5,
        HYDRA3 = 0xF6,
        HYDRA4 = 0xF7,
        /*dragon*/
        DRAGON = 0xF8,
        DRAGON2 = 0xF9,
        DRAGON3 = 0xFa,
        DRAGON4 = 0xFb,
        /*balron*/
        BALRON = 0xFC,
        BALRON2 = 0xFd,
        BALRON3 = 0xFe,
        BALRON4 = 0xFf,
    };

    public TILE current_tile;

    public enum DIRECTION
    {
        DIRECTION_W = 0,
        DIRECTION_N = 1,
        DIRECTION_E = 2,
        DIRECTION_S = 3
    }
    public enum MODE
    {
        VISION = 0,
        OUTDOORS = 1,
        BUILDING = 2,
        DUNGEON = 3,
        COMBAT = 4,
        COM_CAMP = 5,
        COM_ROOM = 6,
        SHRINE = 7
    };

    public enum WEAPON
    {
	    HANDS = 0,
        STAFF = 1,
        DAGGER = 2,
        SLING = 3,
        MACE = 4,
        AXE = 5,
        SWORD = 6,
        BOW = 7,
        CROSSBOW = 8,
        FLAMING_OIL = 9,
        HALBERD = 10,
        MAGIC_AXE = 11,
        MAGIC_SWORD = 12,
        MAGIC_BOW = 13,
        MAGIC_WAND = 14,
        MYSTIC_SWORD = 15
    };

    public enum ARMOR
    {
        SKIN = 0,
        CLOTH = 1,
        LEATHER = 2,
        CHAIN_MAIL = 3,
        PLATE_MAIL = 4,
        MAGIC_CHAIN = 5,
        MAGIC_PLATE = 6,
        MYSTIC_ROB = 7,
    };

    public enum REAGENT
    {
        SULFER_ASH = 0,
        GINSENG = 1,
        GARLIC = 2,
        SPIDER_SILK = 3,
        BLOOD_MOSS = 4,
        BLACK_PEARL = 5,
        NIGHTSHADE = 6,
        MANDRAKE = 7,
    };

    public enum LOCATIONS
    {
        // world map
        OUTDOORS = 0,

        // Castles
        BRITANNIA = 1,
        THE_LYCAEUM = 2,
        EMPATH_ABBY = 3,
        SERPENT_HOLD = 4,

        // Townes
        MOONGLOW = 5,
        BRITAIN = 6,
        JHELOM = 7,
        YEW = 8,
        MINOC = 9,
        TRINSIC = 10,
        SKARA_BRAE = 11,
        MAGINCIA = 12,

        // Villages
        PAWS = 13,
        BUCCANEERS_DEN = 14,
        VESPER = 15,
        COVE = 16,

        // Dungeons
        DECEIT = 17,
        DESPISE = 18,
        DESTARD = 19,
        WRONG = 20,
        COVETOUS = 21,
        SHAME = 22,
        HYTHLOTH = 23,
        THE_GREAT_STYGIAN_ABYSS = 24,

        //shrines
        HONESTY = 25,
        COMPASSION = 26,
        VALOR = 27,
        JUSTICE = 28,
        SACRIFICE = 29,
        HONOR = 30,
        SPIRITUALITY = 31,
        HUMILITY = 32
    };

    [DllImport("UN_U4.dll")]
    public static extern void main();
    [DllImport("UN_U4.dll")]
    public static extern MODE main_CurMode();
    [DllImport("UN_U4.dll")]
    public static extern TILE main_D_96F8();
    [DllImport("UN_U4.dll")]
    public static extern TILE main_D_946C();
    [DllImport("UN_U4.dll")]
    public static extern void main_keyboardHit(char key);
    [DllImport("UN_U4.dll")]
    public static extern void main_CurMap(byte[] buffer, int length);
    [DllImport("UN_U4.dll")]
    public static extern void main_Combat(byte[] buffer, int length);
    [DllImport("UN_U4.dll")]
    public static extern void main_Fighters(byte[] buffer, int length);
    [DllImport("UN_U4.dll")]
    public static extern void main_D_96F9(byte[] buffer, int length);
    [DllImport("UN_U4.dll")]
    public static extern void main_Party(byte[] buffer, int length);
    [DllImport("UN_U4.dll")]
    public static extern void main_Hit(byte[] buffer, int length);
    [DllImport("UN_U4.dll")]
    public static extern void main_ActiveChar(byte[] buffer, int length);
    [DllImport("UN_U4.dll")]
    public static extern TILE main_tile_cur();
    [DllImport("UN_U4.dll")]
    public static extern int main_Text(byte[] buffer, int length);

    

    public GameObject partyGameObject;

    float timer = 0.0f;
    float timerExpired = 0.0f;
    public float timerPeriod = 0.02f; // the game operates on a 300ms Sleep() so we want to update things faster than that

    byte[] buffer = new byte[2000];

    public TILE[,] tMap32x32 = new TILE[32, 32];
    public byte[,,] tMap8x8x8 = new byte[8, 8, 8];

    public MODE current_mode;

    [System.Serializable]
    public struct tCombat1 /*size:0xc0*/
    {
        public byte _npcX, _npcY; //16,16 /*_000/_010 D_9470/D_9480*/
    };

    [System.Serializable]
    public struct tCombat2 /*size:0xc0*/
    {
        public byte _charaX, _charaY; //8,8 /*_20/_28 D_9490/D_9498*/
    };

    // allocate storage for Combat global
    public tCombat1[] Combat1 = new tCombat1[16];
    public tCombat2[] Combat2 = new tCombat2[8];
    public byte[,] Combat_map = new byte[11, 11]; //11 * 11 /*_040 D_94B0*/

    [System.Serializable]
    public struct tNPC /*size:0x100*/
    {
        /*_00*/
        public TILE _gtile;
        /*_20*/
        public byte _x, _y;
        /*_60*/
        public TILE _tile;
        /*_80*/
        public byte _old_x, _old_y;
        /*_c0*/
        public byte _var;/*_agressivity (or _z in dungeon)*/
        /*_e0*/
        public byte _tlkidx;
    };

    public tNPC[] _npc = new tNPC[32];

    [System.Serializable]
    public struct tChara /*size:0x27*/
    {
        /*+00*/
        public ushort[] _HP; //2
        /*+04*/
        public ushort _XP;
        /*+06*/
        public ushort _str;
        /*+08*/
        public ushort _dex;
        /*+0a*/
        public ushort _int;
        /*+0c*/
        public ushort _MP;
        /*+0e*/
        public byte[] __0e; //2
        /*+10*/
        public WEAPON _weapon;
        /*+12*/
        public ARMOR _armor;
        /*+14*/
        public string _name; // char _name[16];
        /*+24*/
        public byte p_24;/*sex char*/
        /*+25*/
        public byte _class;
        /*+26*/
        public byte _stat;
    };

    [System.Serializable]
    public struct tParty /*size:0x1f6*/
    {
        /*+000*/
        public uint f_000;/*a counter*/
        /* */
        /*+004*/
        public uint _moves;
        /*+008*/
        public tChara[] chara; //8
        /*+140*/
        public uint _food;
        /*+144*/
        public ushort _gold;
        /*karmas*/
        /*+146*/
        public ushort _hones;
        /*+148*/
        public ushort _compa;
        /*+14a*/
        public ushort _valor;
        /*+14c*/
        public ushort _justi;
        /*+14e*/
        public ushort _sacri;
        /*+150*/
        public ushort _honor;
        /*+152*/
        public ushort _spiri;
        /*+154*/
        public ushort _humil;
        /* */
        /*+156*/
        public ushort _torches;
        /*+158*/
        public ushort _gems;
        /*+15a*/
        public ushort _keys;
        /*+15c*/
        public ushort _sextants;
        /*+15e*/
        public ushort[] _armors; //8
        /*+16e*/
        public ushort[] _weapons; //16
        /*+18e*/
        public ushort[] _reagents; //8
        /*+19e*/
        public ushort[] _mixtures; //26
        /*+1d2*/
        public ushort mItems;
        /*+1d4,+1d5*/
        public byte _x, _y;
        /*+1d6*/
        public byte mStones;
        /*+1d7*/
        public byte mRunes;
        /*+1d8*/
        public ushort f_1d8;/*characters #*/
        /*+1da*/
        public TILE _tile;
        /*+1dc*/
        public ushort f_1dc;/*isFlying or light[dungeon]*/
        /*+1de/+1e0*/
        public ushort _trammel, _felucca;/*moons phase*/
        /*+1e2*/
        public ushort _ship;/*hull integrity*/
        /*+1e4*/
        public ushort f_1e4;/*did met with LB*/
        /*+1e6*/
        public ushort f_1e6;/*last hole up&camp*/
        /*+1e8*/
        public ushort f_1e8;/*last found*/
        /*+1ea*/
        public ushort f_1ea;/*last meditation/Hawkwind*/
        /*+1ec*/
        public ushort f_1ec;/*last karma-conversation*/
        /*+1ee,+1ef*/
        public byte out_x, out_y;
        /*+1f0*/
        public ushort _dir;/*[dungeon]*/
        /*+1f2*/
        public short _z;/*[dungeon]*/
        /*+1f4*/
        public LOCATIONS _loc;
    };

    public tParty Party = new tParty();

    [System.Serializable]
    public struct t_68
    {
        /*000*/
        public byte _x, _y;
        /*020*/
        public byte _HP;
        /*030*/
        public TILE _tile, _gtile;
        /*050*/
        public byte _sleeping;
        /*060*/
        public TILE _chtile;
    }

    public t_68[] Fighters = new t_68[16];

    public TILE[,] map = new TILE[11, 11];
    public enum COMBAT_TERRAIN
    {
        GRASS = 0,
        BRIDGE = 1,
        BRICK = 2,
        DUNGEON = 3,
        HILL = 4,
        FOREST = 5,
        BRUSH = 6,
        MARSH = 7,
        SHIPSEA = 8,
        SHORE = 9,
        SHIPSHIP = 10,
        SHIPSHORE = 11,
        SHORESHIP = 22,
        INN = 21,
        CAMP = 12,
        SHRINE = 13,
        DNG0 = 14,
        DNG1 = 15,
        DNG2 = 16,
        DNG3 = 17,
        DNG4 = 18,
        DNG5 = 19,
        DNG6 = 20
     };

    // Separate thread to run the game, we could attempt to make the data gathering function thread safe but for now this will do
    private void ThreadTask()
    {
        // start the DLL main thread
        main();
    }

    public GameObject[] Settlements;
    public GameObject[] CombatTerrains;

    public Text textDisplay;
    //private TMP_FontAsset m_FontAsset;

    public string label = "The <#0050FF>count is: </color>{0:2}";
    public float m_frame;

    public GameObject TextGameObject;

    void StartText()
    {
        textDisplay = textGameObject.GetComponent<Text>();

        //m_textMeshPro.autoSizeTextContainer = true;

        // Load the Font Asset to be used.
        //m_FontAsset = Resources.Load("Fonts & Materials/LiberationSans SDF", typeof(TMP_FontAsset)) as TMP_FontAsset;
        //m_textMeshPro.font = m_FontAsset;

        // Assign Material to TextMesh Pro Component
        //m_textMeshPro.fontSharedMaterial = Resources.Load("Fonts & Materials/LiberationSans SDF - Bevel", typeof(Material)) as Material;
        //m_textMeshPro.fontSharedMaterial.EnableKeyword("BEVEL_ON");

        // Set various font settings.
        //m_textMeshPro.fontSize = 24;
        //m_textMeshPro
        //m_textMeshPro.alignment = TMPro.TextAlignmentOptions.Center;

        //m_textMeshPro.anchorDampening = true; // Has been deprecated but under consideration for re-implementation.
        //m_textMeshPro.enableAutoSizing = true;

        //m_textMeshPro.characterSpacing = 0.2f;
        //m_textMeshPro.wordSpacing = 0.1f;

        //m_textMeshPro.enableCulling = true;
        //m_textMeshPro.enableWordWrapping = true;

        //textMeshPro.fontColor = new Color32(255, 255, 255, 255);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartText();

        // allocate storage for Party global
        Party.chara = new tChara[8];
        for (int i = 0; i < 8; i++)
        {
            Party.chara[i]._HP = new ushort[2];
            Party.chara[i].__0e = new byte[2];
        }
        Party._armors = new ushort[8];
        Party._weapons = new ushort[16];
        Party._reagents = new ushort[8];
        Party._mixtures = new ushort[26];

        // start a thread with the DLL main task
        trd = new Thread(new ThreadStart(this.ThreadTask));
        trd.IsBackground = true;
        trd.Start();

        // assign settlement game objects
        Settlements = new GameObject[17];
        Settlements[(int)LOCATIONS.BRITANNIA] = GameObject.Find("LBC_1");
        Settlements[(int)0] = GameObject.Find("LBC_2"); // need to fingure out how to determine location of the upper and lower levels of the castle
        Settlements[(int)LOCATIONS.THE_LYCAEUM] = GameObject.Find("LYCAEUM");
        Settlements[(int)LOCATIONS.EMPATH_ABBY] = GameObject.Find("EMPATH");
        Settlements[(int)LOCATIONS.SERPENT_HOLD] = GameObject.Find("SERPENT");
        Settlements[(int)LOCATIONS.MOONGLOW] = GameObject.Find("MOONGLOW");
        Settlements[(int)LOCATIONS.BRITAIN] = GameObject.Find("BRITAIN");
        Settlements[(int)LOCATIONS.JHELOM] = GameObject.Find("JHELOM");
        Settlements[(int)LOCATIONS.YEW] = GameObject.Find("YEW");
        Settlements[(int)LOCATIONS.MINOC] = GameObject.Find("MINOC");
        Settlements[(int)LOCATIONS.TRINSIC] = GameObject.Find("TRINSIC");
        Settlements[(int)LOCATIONS.SKARA_BRAE] = GameObject.Find("SKARA");
        Settlements[(int)LOCATIONS.MAGINCIA] = GameObject.Find("MAGINCIA");
        Settlements[(int)LOCATIONS.PAWS] = GameObject.Find("PAWS");
        Settlements[(int)LOCATIONS.BUCCANEERS_DEN] = GameObject.Find("DEN");
        Settlements[(int)LOCATIONS.VESPER] = GameObject.Find("VESPER");
        Settlements[(int)LOCATIONS.COVE] = GameObject.Find("COVE");

        // assign combat terrain game objects
        CombatTerrains = new GameObject[24];
        CombatTerrains[(int)COMBAT_TERRAIN.GRASS] = GameObject.Find("GRASS");
        CombatTerrains[(int)COMBAT_TERRAIN.BRIDGE] = GameObject.Find("BRIDGE");
        CombatTerrains[(int)COMBAT_TERRAIN.BRICK] = GameObject.Find("BRICK");
        CombatTerrains[(int)COMBAT_TERRAIN.DUNGEON] = GameObject.Find("DUNGEON");
        CombatTerrains[(int)COMBAT_TERRAIN.HILL] = GameObject.Find("HILL");
        CombatTerrains[(int)COMBAT_TERRAIN.FOREST] = GameObject.Find("FOREST");
        CombatTerrains[(int)COMBAT_TERRAIN.BRUSH] = GameObject.Find("BRUSH");
        CombatTerrains[(int)COMBAT_TERRAIN.MARSH] = GameObject.Find("MARSH");
        CombatTerrains[(int)COMBAT_TERRAIN.SHIPSEA] = GameObject.Find("SHIPSEA");
        CombatTerrains[(int)COMBAT_TERRAIN.SHORE] = GameObject.Find("SHORE");
        CombatTerrains[(int)COMBAT_TERRAIN.SHIPSHIP] = GameObject.Find("SHIPSHIP");
        CombatTerrains[(int)COMBAT_TERRAIN.SHIPSHORE] = GameObject.Find("SHIPSHORE");
        CombatTerrains[(int)COMBAT_TERRAIN.SHORESHIP] = GameObject.Find("SHORESHIP");
        CombatTerrains[(int)COMBAT_TERRAIN.INN] = GameObject.Find("INN");
        CombatTerrains[(int)COMBAT_TERRAIN.CAMP] = GameObject.Find("CAMP");
        CombatTerrains[(int)COMBAT_TERRAIN.DUNGEON] = GameObject.Find("DUNGEON");
        CombatTerrains[(int)COMBAT_TERRAIN.SHRINE] = GameObject.Find("SHRINE");
        CombatTerrains[(int)COMBAT_TERRAIN.DNG0] = GameObject.Find("DNG0");
        CombatTerrains[(int)COMBAT_TERRAIN.DNG1] = GameObject.Find("DNG1");
        CombatTerrains[(int)COMBAT_TERRAIN.DNG2] = GameObject.Find("DNG2");
        CombatTerrains[(int)COMBAT_TERRAIN.DNG3] = GameObject.Find("DNG3");
        CombatTerrains[(int)COMBAT_TERRAIN.DNG4] = GameObject.Find("DNG4");
        CombatTerrains[(int)COMBAT_TERRAIN.DNG5] = GameObject.Find("DNG5");
        CombatTerrains[(int)COMBAT_TERRAIN.DNG6] = GameObject.Find("DNG6");
    }

    // unfortuantly the game engine never saves this information after loading the combat terrain in function C_7C65()
    // the code is not re-entrant so I cannot just expose and call the function directly so I need to re-implement the 
    // logic here so I can on the fly determine the combat terrain to display by exposing the interal variables used in the
    // original function.
    COMBAT_TERRAIN Convert_Tile_to_Combat_Terrian(TILE tile)
    {
        COMBAT_TERRAIN combat_terrain;

        if (Party._tile <= TILE.SHIP_SOUTH || (TILE)((byte)current_tile & ~3) == TILE.SHIP_WEST)
        {
            if (D_96F8 == TILE.PIRATE_WEST)
            {
                combat_terrain = COMBAT_TERRAIN.SHIPSHORE;
            }
            else if (D_946C <= TILE.SHALLOW_WATER)
            {
                combat_terrain = COMBAT_TERRAIN.SHIPSEA;
            }
            else
            {
                combat_terrain = COMBAT_TERRAIN.SHORE;
            }
        }
        else
        {
            if (D_96F8 == TILE.PIRATE_WEST)
            {
                combat_terrain = COMBAT_TERRAIN.SHORESHIP;
            }
            else if (D_946C <= TILE.SHALLOW_WATER)
            {
                combat_terrain = COMBAT_TERRAIN.SHIPSHIP;
            }
            else
            {
                switch (tile)
                {
                    case TILE.SWAMP:
                    {
                        combat_terrain = COMBAT_TERRAIN.MARSH;
                        break;
                    }
                    case TILE.BRUSH:
                    {
                        combat_terrain = COMBAT_TERRAIN.BRUSH;
                        break;
                    }
                    case TILE.FOREST:
                    {
                        combat_terrain = COMBAT_TERRAIN.FOREST;
                        break;
                    }
                    case TILE.HILLS:
                    {
                        combat_terrain = COMBAT_TERRAIN.HILL;
                        break;
                    }
                    case TILE.DUNGEON:
                    {
                        combat_terrain = COMBAT_TERRAIN.DUNGEON;
                        break;
                    }
                    case TILE.BRICK_FLOOR:
                    {
                        combat_terrain = COMBAT_TERRAIN.BRICK;
                        break;
                    }
                    case TILE.BRIDGE:
                    case TILE.BRIDGE_TOP:
                    case TILE.BRIDGE_BOTTOM:
                    case TILE.WOOD_FLOOR:
                    {
                        combat_terrain = COMBAT_TERRAIN.BRIDGE;
                        break;
                    }
                    default:
                    {
                        combat_terrain = COMBAT_TERRAIN.GRASS;
                        break;
                    }
                }
            }
        }

        return combat_terrain;
    }

    void OnApplicationQuit()
    {
        //trd.Abort();
        // CAUTION: this will cleanly exit the DLL, however is some areas such as combat this will not work and you will
        // need to kill the unity editor process to be able to restart. The abort function will cause the Unity editor to exit which
        // is not desired. Using the exit() function in the DLL will also exit the unity editore which is also not disired. This
        // was the best solution I could come up with for the moment. In the final app all threads will be exiting when quiting so
        // it is not an issue there, only when using the Unity editor do we have trouble. If Unity would load and unload the DLL at runtime
        // this would be a better solution but Unity 3D unfortantly loads any project DLLs at editor launch and keeps them loaded until you
        // quit the editor.
        main_keyboardHit((char)KEYS.VK_ESCAPE);
    }

    public enum KEYS
    {
        VK_LEFT = 0x25,
        VK_UP = 0x26,
        VK_RIGHT = 0x27,
        VK_DOWN = 0x28,
        VK_ESCAPE = 0x1B,
        VK_SPACE = 0x20,
        VK_RETURN = 0x0D,
        VK_BACK = 0x08,
        VK_END = 0x23,
        VK_HOME = 0x24
    };

    public struct activeCharacter
    {
        public bool active;
        public byte characterIndex;
        public byte x, y;
    }

    public activeCharacter currentActiveCharacter;

    //public float hit_time = 0.0f;
    public float hit_time_period = 0.25f;

    public struct hit
    {
        public TILE tile;
        public byte x;
        public byte y;
        public float time;
    }

    public List<hit> currentHits = new List<hit>() { };
    public TILE D_96F8;
    public TILE D_946C;
    public System.Text.ASCIIEncoding enc;

    // Update is called once per frame
    void Update()
    {
        int buffer_index;

        timer += Time.deltaTime;

        // send some keyboard codes down to the engine,
        // Unity keydown is only active for a single frame so it cannot be in the timer check if
        if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.Z)) // need to check this first as it overrides the normal Z keypress
        {
            main_keyboardHit((char)'9'); // currently the windows implementation of this engine does not support this
        }
        else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.S)) // need to check this first as it overrides the normal S keypress
        {
            main_keyboardHit((char)'9'); // currently the windows implementation of this engine does not support this
        }
        else if (Input.GetKeyDown(KeyCode.End))
        {
            main_keyboardHit((char)KEYS.VK_END);
        }
        else if (Input.GetKeyDown(KeyCode.Home))
        {
            main_keyboardHit((char)KEYS.VK_HOME);
        }
        //else if (Input.GetKeyDown(KeyCode.PageUp))
        //{
        //    main_keyboardHit((char)KEYS.VK_PGUP);
        //}
        //else if (Input.GetKeyDown(KeyCode.PageDown))
        //{
        //    main_keyboardHit((char)KEYS.VK_PGDN);
        //}
        else if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            main_keyboardHit((char)KEYS.VK_RETURN);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            main_keyboardHit((char)KEYS.VK_DOWN);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            main_keyboardHit((char)KEYS.VK_UP);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            main_keyboardHit((char)KEYS.VK_LEFT);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            main_keyboardHit((char)KEYS.VK_RIGHT);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            main_keyboardHit((char)KEYS.VK_ESCAPE);
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            main_keyboardHit((char)KEYS.VK_RETURN);
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            main_keyboardHit((char)KEYS.VK_BACK);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            main_keyboardHit((char)KEYS.VK_SPACE);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            main_keyboardHit((char)'A');
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            main_keyboardHit((char)'B');
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            main_keyboardHit((char)'C');
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            main_keyboardHit((char)'D');
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            main_keyboardHit((char)'E');
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            main_keyboardHit((char)'F');
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            main_keyboardHit((char)'G');
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            main_keyboardHit((char)'H');
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            main_keyboardHit((char)'I');
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            main_keyboardHit((char)'J');
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            main_keyboardHit((char)'K');
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            main_keyboardHit((char)'L');
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            main_keyboardHit((char)'M');
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            main_keyboardHit((char)'N');
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            main_keyboardHit((char)'O');
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            main_keyboardHit((char)'P');
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            main_keyboardHit((char)'Q');
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            main_keyboardHit((char)'R');
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            main_keyboardHit((char)'S');
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            main_keyboardHit((char)'T');
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            main_keyboardHit((char)'U');
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            main_keyboardHit((char)'V');
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            main_keyboardHit((char)'W');
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            main_keyboardHit((char)'X');
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            main_keyboardHit((char)'Y');
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            main_keyboardHit((char)'Z');
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        {
            main_keyboardHit((char)'0');
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            main_keyboardHit((char)'1');
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            main_keyboardHit((char)'2');
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            main_keyboardHit((char)'3');
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            main_keyboardHit((char)'4');
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            main_keyboardHit((char)'5');
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            main_keyboardHit((char)'6');
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
        {
            main_keyboardHit((char)'7');
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
        {
            main_keyboardHit((char)'8');
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
        {
            main_keyboardHit((char)'9');
        }


        // only get data from the game engine periodically
        if (timer > timerExpired)
        {
            // update the timer
            timer = timer - timerExpired;
            timerExpired = timerPeriod;


            int text_size = main_Text(buffer, buffer.Length);
            if (enc == null)
            {
                enc = new System.Text.ASCIIEncoding();
            }
            if (text_size != 0)
            {
                if (textDisplay == null)
                {
                    textDisplay = textGameObject.GetComponent<Text>();
                }
                textDisplay.text = textDisplay.text + enc.GetString(buffer, 0, text_size);
                //m_textMeshPro.textInfo.lineCount;
                //m_textMeshPro.maxVisibleLines = 10;
                int newline_count = 0;
                int i;
                for (i = textDisplay.text.Length - 1; (i > 0) && (newline_count < 20); i--)
                {
                    if (textDisplay.text[i] == '\n')
                    {
                        newline_count++;
                    }
                }
                if (newline_count == 10)
                {
                    textDisplay.text = textDisplay.text.Substring(i + 2);
                }
                //textDisplay.ForceMeshUpdate();
            }

            D_96F8 = main_D_96F8();
            D_946C = main_D_946C();

            // get any hilighted character, used during combat
            main_ActiveChar(buffer, buffer.Length);

            // check if active
            if (buffer[1] != 0xff)
            {
                currentActiveCharacter.active = true;
                currentActiveCharacter.characterIndex = buffer[0];
                currentActiveCharacter.x = buffer[1];
                currentActiveCharacter.y = buffer[2];
            }
            // else deactivate
            else
            {
                currentActiveCharacter.active = false;
            }

            // get the current tile under the party
            current_tile = main_tile_cur();

            // read in current hit info list, the tile draws occurr out of the main draw squence and for only a short time before the playfield is repainted
            // the DLL saves the list of hits and coords to display later
            main_Hit(buffer, buffer.Length);

            // get the hit list length from the buffer
            int hit_length = buffer[0];

            // add all the new hits to the list from the buffer 
            for (int i = 0; i < hit_length; i++)
            {
                hit addHit;

                addHit.tile = (TILE)buffer[1 + i * 3];

                if (addHit.tile != 0)
                {
                    addHit.x = buffer[2 + i * 3];
                    addHit.y = buffer[3 + i * 3];
                    addHit.time = Time.time;

                    currentHits.Add(addHit);
                }
            }

            // remove any hits with expired timers
            // TODO: make sure every hit is displayed at least a little while when overlapping hits occur
            for (int i = 0; i < currentHits.Count; i++)
            {
                hit checkHit = currentHits[i];

                // we will leave this hit up until we get another or the timer expires
                if (checkHit.time + hit_time_period < Time.time)
                {
                    currentHits.Remove(checkHit);
                }
            }

            // get the current map and npc data
            main_CurMap(buffer, buffer.Length);

            // get the current game mode;
            current_mode = main_CurMode();

            // extract the map data depending on the game mode
            if ((current_mode == MODE.OUTDOORS) || (current_mode == MODE.BUILDING))
            {
                buffer_index = 0;
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        tMap32x32[i, j] = (TILE)buffer[buffer_index++];
                    }
                }
            }
            else if (current_mode == MODE.DUNGEON)
            {
                buffer_index = 0;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        for (int k = 0; k < 8; k++)
                        {
                            tMap8x8x8[i, j, k] = buffer[buffer_index++];
                        }
                    }
                }
            }

            // extract the npc data
            buffer_index = 1024;
            for (int i = 0; i < 32; i++)
            {
                _npc[i]._gtile = (TILE)buffer[buffer_index + 0x00];
                _npc[i]._x = buffer[buffer_index + 0x20];
                _npc[i]._y = buffer[buffer_index + 0x40];
                _npc[i]._tile = (TILE)buffer[buffer_index + 0x60];
                _npc[i]._old_x = buffer[buffer_index + 0x80];
                _npc[i]._old_y = buffer[buffer_index + 0xa0];
                _npc[i]._var = buffer[buffer_index + 0xc0]; /*_agressivity (or _z in dungeon)*/
                _npc[i]._tlkidx = buffer[buffer_index + 0xe0];
                buffer_index++;
            }

            // get the current party data
            main_Party(buffer, buffer.Length);

            Party.f_000 = System.BitConverter.ToUInt32(buffer, 0x000);
            Party._moves = System.BitConverter.ToUInt32(buffer, 0x004);
            buffer_index = 0x008;
            for (int i = 0; i < 8; i++)
            {
                Party.chara[i]._HP[0] = buffer[buffer_index++];
                Party.chara[i]._HP[1] = buffer[buffer_index++];
                Party.chara[i]._XP = buffer[buffer_index++];
                Party.chara[i]._str = buffer[buffer_index++];
                Party.chara[i]._int = buffer[buffer_index++];
                Party.chara[i]._MP = buffer[buffer_index++];

                Party.chara[i].__0e[0] = buffer[buffer_index++];
                Party.chara[i].__0e[1] = buffer[buffer_index++];
                Party.chara[i]._weapon = (WEAPON)buffer[buffer_index++];
                Party.chara[i]._armor = (ARMOR)buffer[buffer_index++];
                Party.chara[i]._name = "" + (char)buffer[buffer_index++] + (char)buffer[buffer_index++] + (char)buffer[buffer_index++] + (char)buffer[buffer_index++]
                    + (char)buffer[buffer_index++] + (char)buffer[buffer_index++] + (char)buffer[buffer_index++] + (char)buffer[buffer_index++];
                Party.chara[i].p_24 = buffer[buffer_index++];
                Party.chara[i]._class = buffer[buffer_index++];
                Party.chara[i]._stat = buffer[buffer_index++];
            }
            Party._food = System.BitConverter.ToUInt32(buffer, 0x140);
            Party._gold = System.BitConverter.ToUInt16(buffer, 0x144);
            Party._hones = System.BitConverter.ToUInt16(buffer, 0x146);
            Party._compa = System.BitConverter.ToUInt16(buffer, 0x148);
            Party._valor = System.BitConverter.ToUInt16(buffer, 0x14a);
            Party._justi = System.BitConverter.ToUInt16(buffer, 0x14c);
            Party._sacri = System.BitConverter.ToUInt16(buffer, 0x14e);
            Party._honor = System.BitConverter.ToUInt16(buffer, 0x150);
            Party._spiri = System.BitConverter.ToUInt16(buffer, 0x152);
            Party._humil = System.BitConverter.ToUInt16(buffer, 0x154);
            Party._torches = System.BitConverter.ToUInt16(buffer, 0x156);
            Party._gems = System.BitConverter.ToUInt16(buffer, 0x158);
            Party._keys = System.BitConverter.ToUInt16(buffer, 0x15a);
            Party._sextants = System.BitConverter.ToUInt16(buffer, 0x15c);

            for (int i = 0; i < 8; i++)
            {
                Party._armors[i] = System.BitConverter.ToUInt16(buffer, 0x15e + i * 2);
            }
            for (int i = 0; i < 16; i++)
            {
                Party._weapons[i] = System.BitConverter.ToUInt16(buffer, 0x16e + i * 2);
            }
            for (int i = 0; i < 8; i++)
            {
                Party._reagents[i] = System.BitConverter.ToUInt16(buffer, 0x18e + i * 2);
            }
            for (int i = 0; i < 26; i++)
            {
                Party._mixtures[i] = System.BitConverter.ToUInt16(buffer, 0x19e + i * 2);
            }

            Party.mItems = System.BitConverter.ToUInt16(buffer, 0x1d2);
            Party._x = buffer[0x1d4];
            Party._y = buffer[0x1d5];
            Party.mStones = buffer[0x1d6];
            Party.mRunes = buffer[0x1d7];
            Party.f_1d8 = System.BitConverter.ToUInt16(buffer, 0x1d8); // number in party
            Party._tile = (TILE)System.BitConverter.ToUInt16(buffer, 0x1da);
            Party.f_1dc = System.BitConverter.ToUInt16(buffer, 0x1dc);
            Party._trammel = System.BitConverter.ToUInt16(buffer, 0x1de);
            Party._felucca = System.BitConverter.ToUInt16(buffer, 0x1e0);
            Party._ship = System.BitConverter.ToUInt16(buffer, 0x1e2);
            Party.f_1e4 = System.BitConverter.ToUInt16(buffer, 0x1e4);
            Party.f_1e6 = System.BitConverter.ToUInt16(buffer, 0x1e6);
            Party.f_1e8 = System.BitConverter.ToUInt16(buffer, 0x1e8);
            Party.f_1ea = System.BitConverter.ToUInt16(buffer, 0x1ea);
            Party.f_1ec = System.BitConverter.ToUInt16(buffer, 0x1ec);
            Party.out_x = buffer[0x1ee];
            Party.out_y = buffer[0x1ef];
            Party._dir = System.BitConverter.ToUInt16(buffer, 0x1e2);
            Party._z = System.BitConverter.ToInt16(buffer, 0x1f2);
            Party._loc = (LOCATIONS)(System.BitConverter.ToUInt16(buffer, 0x1f4));

            // read in the Combat global
            main_Combat(buffer, buffer.Length);

            // read the npc positions
            for (int i = 0; i < 16; i++)
            {
                Combat1[i]._npcX = buffer[0x00 + i];
                Combat1[i]._npcY = buffer[0x10 + i];
            }
            // read the character positions
            for (int i = 0; i < 8; i++)
            {
                Combat2[i]._charaX = buffer[0x20 + i];
                Combat2[i]._charaY = buffer[0x28 + i];
            }
            // read the combat map
            buffer_index = 0x40;
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    Combat_map[i,j] = buffer[buffer_index++];
                }
            }

            // read in the Fighters global
            main_Fighters(buffer, buffer.Length);

            // extract the fighter data
            for (int i = 0; i < 16; i++)
            {
                Fighters[i]._x = buffer[0x00 + i];
                Fighters[i]._y = buffer[0x10 + i];
                Fighters[i]._HP = buffer[0x20 + i];
                Fighters[i]._tile = (TILE)buffer[0x30 + i];
                Fighters[i]._gtile = (TILE)buffer[0x40 + i];
                Fighters[i]._sleeping = buffer[0x50 + i];
                Fighters[i]._chtile = (TILE)buffer[0x60 + i];
            }

            // read in the main_D_96F9 global
            main_D_96F9(buffer, buffer.Length);

            // read the main display buffer
            buffer_index = 0;
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    map[i, j] = (TILE)buffer[buffer_index++];
                }
            }

            // keep the part game object in sync with the game
            if (partyGameObject)
            {
                partyGameObject.transform.localPosition = new Vector3(Party._x, 255 - Party._y, 0);
            }

            World[] worldList = FindObjectsOfType<World>();

            if (worldList.Length != 0)
            {
//                worldList[0].DrawMap(map, currentHits, currentActiveCharacter);

                if (current_mode == MODE.OUTDOORS)
                {
                    worldList[0].AddNPCs(_npc);
                    worldList[0].followWorld();
                    worldList[0].AddHits(currentHits);
                    worldList[0].AddActiveCharacter(currentActiveCharacter);
                    worldList[0].terrain.SetActive(true);
                    worldList[0].animatedTerrrain.SetActive(true);
                    worldList[0].fighters.SetActive(false);
                    worldList[0].characters.SetActive(false);
                    worldList[0].npcs.SetActive(true);
                    worldList[0].party.SetActive(true);

                    for (int i = 0; i < 17; i++)
                    {
                        Settlements[i].gameObject.SetActive(false);
                    }
                    for (int i = 0; i < 23; i++)
                    {
                        CombatTerrains[i].gameObject.SetActive(false);
                    }
                }
                else if (current_mode == MODE.BUILDING)
                {
                    worldList[0].AddNPCs(_npc);
                    worldList[0].followWorld();
                    worldList[0].AddHits(currentHits);
                    worldList[0].AddActiveCharacter(currentActiveCharacter);
                    worldList[0].terrain.SetActive(false);
                    worldList[0].animatedTerrrain.SetActive(false);
                    worldList[0].fighters.SetActive(false);
                    worldList[0].characters.SetActive(false);
                    worldList[0].npcs.SetActive(true);
                    worldList[0].party.SetActive(true);

                    for (int i = 0; i < 17; i++)
                    {
                        GameObject set = Settlements[i].gameObject;

                        // need to special case the castle, this is how the engine figures it out using the ladder
                        if ((i == 0) && (Party._loc == LOCATIONS.BRITANNIA) && (tMap32x32[3,3] == TILE.LADDER_DOWN))
                        {
                            set.SetActive(true);
                        }
                        else if ((i == 0) && (Party._loc == LOCATIONS.BRITANNIA) && (tMap32x32[3, 3] == TILE.LADDER_UP))
                        {
                            set.SetActive(false);
                        }
                        else if ((i == 1) && (Party._loc == LOCATIONS.BRITANNIA) && (tMap32x32[3, 3] == TILE.LADDER_UP))
                        {
                            set.SetActive(true);
                        }
                        else if ((i == 1) && (Party._loc == LOCATIONS.BRITANNIA) && (tMap32x32[3, 3] == TILE.LADDER_DOWN))
                        {
                            set.SetActive(false);
                        }
                        else if (Party._loc == (LOCATIONS)i)
                        {
                            set.SetActive(true);
                        }
                        else
                        {
                            set.SetActive(false);
                        }
                    }
                    for (int i = 0; i < 23; i++)
                    {
                        CombatTerrains[i].gameObject.SetActive(false);
                    }
                }
                else if (current_mode == MODE.COMBAT)
                {
                    worldList[0].AddFighters(Fighters, Combat1);
                    worldList[0].AddCharacters(Combat2, Party, Fighters);
                    worldList[0].AddHits(currentHits);
                    worldList[0].AddActiveCharacter(currentActiveCharacter);
                    worldList[0].terrain.SetActive(false);
                    worldList[0].animatedTerrrain.SetActive(false);
                    worldList[0].fighters.SetActive(true);
                    worldList[0].characters.SetActive(true);
                    worldList[0].npcs.SetActive(false);
                    worldList[0].party.SetActive(false);

                    for (int i = 0; i < 17; i++)
                    {
                        Settlements[i].gameObject.SetActive(false);
                    }

                    int currentCombatTerrain = (int)Convert_Tile_to_Combat_Terrian(current_tile);

                    for (int i = 0; i < 23; i++)
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
                }

                if ((worldList[0].party != null) && (worldList[0].tiles != null))
                {
                    // set the party tile, person, horse, ballon, ship, etc.
                    Renderer renderer = worldList[0].party.GetComponentInChildren<Renderer>();
                    if (renderer)
                    {
                        worldList[0].party.GetComponentInChildren<Renderer>().material.mainTexture = worldList[0].tiles[(int)Party._tile];
                        worldList[0].party.name = Party._tile.ToString();
                    }
                }
            }
        }
    }
}
