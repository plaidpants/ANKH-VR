using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Threading;



public class U4_Decompiled : MonoBehaviour
{
    private Thread trd;

    // tiles
    public enum TILE
    {
        /*deep water*/
        TILE_DEEP_WATER = 0x00,
        /*medium water*/
        TILE_MEDIUM_WATER = 0x01,
        /*shallow water*/
        TILE_SHALLOW_WATER = 0x02,
        /*swamp*/
        TILE_SWAMP = 0x03,
        /*grass*/
        TILE_GRASS = 0x04,
        /*scrub*/
        TILE_SCRUB = 0x05,
        /*forest*/
        TILE_FOREST = 0x06,
        /*hills*/
        TILE_HILLS = 0x07,
        /*mountains*/
        TILE_MOUNTAINS = 0x08,
        /*dungeon*/
        TILE_DUNGEON = 0x09,
        /*town*/
        TILE_TOWN = 0x0A,
        /*castle*/
        TILE_CASTLE = 0x0B,
        /*village*/
        TILE_VILLAGE = 0x0C,
        /*LB castle left wing*/
        TILE_CASTLE_LEFT = 0x0D,
        /*LB castle entrance*/
        TILE_CASTLE_ENTRANCE = 0x0E,
        /*LB castle right wing*/
        TILE_CASTLE_RIGHT = 0x0F,

        /*ship W N E S*/
        TILE_SHIP_WEST = 0x10,
        TILE_SHIP_NORTH = 0x11,
        TILE_SHIP_EAST = 0x12,
        TILE_SHIP_SOUTH = 0x13,

        /*horse W/E*/
        TILE_HORSE_WEST = 0x14,
        TILE_HORSE_EAST = 0x15,

        /*tiled floor*/
        TILE_TILED_FLOOR = 0x16,
        /*bridge*/
        TILE_BRIDGE = 0x17,
        /*balloon*/
        TILE_BALOON = 0x18,
        /**/
        TILE_BRIDGE_TOP = 0x19,
        /**/
        TILE_BRIDGE_BOTTOM = 0x1A,
        /*ladder up*/
        TILE_LADDER_UP = 0x1B,
        /*ladder down*/
        TILE_LADDER_DOWN = 0x1C,
        /*ruins*/
        TILE_RUINS = 0x1D,
        /*shrine*/
        TILE_SHRINE = 0x1E,
        /*on foot party*/
        TILE_PARTY = 0x1F,

        /* 2-tile animation character */

        /*mage*/
        TILE_MAGE = 0x20,
        TILE_MAGE2 = 0x21,
        /*bard*/
        TILE_BARD = 0x22,
        TILE_BARD2 = 0x21,
        /*fighter*/
        TILE_FIGHTER = 0x24,
        TILE_FIGHTER2 = 0x25,
        /*druid*/
        TILE_DRUID = 0x26,
        TILE_DRUID2 = 0x27,
        /*tinker*/
        TILE_TINKER = 0x28,
        TILE_TINKER2 = 0x29,
        /*paladin*/
        TILE_PALADIN = 0x2A,
        TILE_PALADIN2 = 0x2B,
        /*ranger*/
        TILE_RANGER = 0x2C,
        TILE_RANGER2 = 0x2D,
        /*shepherd*/
        TILE_SHEPHERD = 0x2E,
        TILE_SHEPHERD2 = 0x2F,

        /* architecture/misc tiles */
        TILE_BRICK_FLOOR_COLUMN = 0x30,
        TILE_DIAGONAL_WATER_ARCHITECTURE1 = 0x31,
        TILE_DIAGONAL_WATER_ARCHITECTURE2 = 0x32,
        TILE_DIAGONAL_WATER_ARCHITECTURE3 = 0x33,
        TILE_DIAGONAL_WATER_ARCHITECTURE4 = 0x34,
        TILE_SHIP_MAST = 0x35,
        TILE_SHIP_WHEEL = 0x36,
        TILE_SMALL_ROCKS = 0x37,
        /*sleep*/
        TILE_SLEEP = 0x38,
        /* large rocks */
        TILE_LARGE_ROCKS = 0x39,
        /*locked door*/
        TILE_LOCKED_DOOR = 0x3A,
        /*door*/
        TILE_DOOR = 0x3B,
        /*chest*/
        TILE_CHEST = 0x3C,
        /*ankh*/
        TILE_ANKH = 0x3D,
        /*brick floor*/
        TILE_BRICK_FLOOR = 0x3E,
        /*wood floor*/
        TILE_WOOD_FLOOR = 0x3F,

        /*moongate 4 phases*/
        TILE_MOONGATE1 = 0x40,
        TILE_MOONGATE2 = 0x41,
        TILE_MOONGATE3 = 0x42,
        TILE_MOONGATE4 = 0x43,

        /*poison field*/
        TILE_POISON_FIELD = 0x44,
        /*energy field*/
        TILE_ENERGY_FIELD = 0x45,
        /*fire field*/
        TILE_FIRE_FIELD = 0x46,
        /*sleep field*/
        TILE_SLEEP_FIELD = 0x47,

        /* used for boats and building features */
        TILE_ARCHITECTURE = 0x48,
        /* Secret brick wall */
        TILE_SECRET_BRICK_WALL = 0x49,
        /* unknown ??? */
        TILE_4A = 0x4A,
        /* cooking/camp fire */
        TILE_COOKING_FIRE = 0x4B,
        /* lava */
        TILE_LAVA = 0x4C,

        /* missiles */
        TILE_MISSLE1 = 0x4D,
        TILE_MISSLE2 = 0x4E,
        TILE_MISSLE3 = 0x4F,

        /* 2-tile animation NPCs */

        /*guard*/
        TILE_GUARD = 0x50,
        TILE_GUARD2 = 0x51,
        /*merchant*/
        TILE_MERCHANT = 0x52,
        TILE_MERCHANT2 = 0x53,
        /*bard*/
        TILE_BARD_NPC = 0x54,
        TILE_BARD_NPC2 = 0x55,
        /*jester*/
        TILE_JESTER = 0x56,
        TILE_JESTER2 = 0x57,
        /*beggar*/
        TILE_BEGGAR = 0x58,
        TILE_BEGGAR2 = 0x59,
        /*child*/
        TILE_CHILD = 0x5A,
        TILE_CHILD2 = 0x5B,
        /*bull*/
        TILE_BULL = 0x5C,
        TILE_BULL2 = 0x5D,
        /*lord british*/
        TILE_LORD_BRITISH = 0x5E,
        TILE_LORD_BRITISH2 = 0x5F,

        /* Letters */
        TILE_A = 0x60,
        TILE_B = 0x61,
        TILE_C = 0x62,
        TILE_D = 0x63,
        TILE_E = 0x64,
        TILE_F = 0x65,
        TILE_G = 0x66,
        TILE_H = 0x67,
        TILE_I = 0x68,
        TILE_J = 0x69,
        TILE_K = 0x6A,
        TILE_L = 0x6B,
        TILE_M = 0x6C,
        TILE_N = 0x6D,
        TILE_O = 0x6E,
        TILE_P = 0x6F,
        TILE_Q = 0x70,
        TILE_R = 0x71,
        TILE_S = 0x72,
        TILE_T = 0x73,
        TILE_U = 0x74,
        TILE_V = 0x75,
        TILE_W = 0x76,
        TILE_X = 0x77,
        TILE_Y = 0x78,
        TILE_Z = 0x79,

        /*<space>*/
        TILE_SPACE = 0x7A,

        /* Purple brackets */
        TILE_BRACKET_RIGHT = 0x7B,
        TILE_BRACKET_LEFT = 0x7C,
        TILE_BRACKET_SQUARE = 0x7D,
        TILE_BLANK = 0x7E,
        TILE_BRICK_WALL = 0x7F,

        /* 2-tile monsters */

        /*pirate W N E S*/
        TILE_PIRATE_WEST = 0x80,
        TILE_PIRATE_NORTH = 0x81,
        TILE_PIRATE_EAST = 0x82,
        TILE_PIRATE_SOUTH = 0x83,
        /*nixie*/
        TILE_NIXIE = 0x84,
        TILE_NIXIE2 = 0x85,
        /*squid*/
        TILE_SQUID = 0x86,
        TILE_SQUID2 = 0x87,
        /*serpent*/
        TILE_SERPENT = 0x88,
        TILE_SERPENT2 = 0x89,
        /*seahorse*/
        TILE_SEAHORSE = 0x8A,
        TILE_SEAHORSE2 = 0x8B,
        /*whirlpool*/
        TILE_WHIRLPOOL = 0x8C,
        TILE_WHIRLPOOL2 = 0x8D,
        /*twister*/
        TILE_WATER_SPOUT = 0x8E,
        TILE_WATER_SPOUT2 = 0x8F,

        /* 4-tile monsters */

        /*rat*/
        TILE_RAT = 0x90,
        TILE_RAT2 = 0x91,
        TILE_RAT3 = 0x92,
        TILE_RAT4 = 0x93,
        /*bat*/
        TILE_BAT = 0x94,
        TILE_BAT2 = 0x95,
        TILE_BAT3 = 0x96,
        TILE_BAT4 = 0x97,
        /*spider*/
        TILE_SPIDER = 0x98,
        TILE_SPIDER2 = 0x99,
        TILE_SPIDER3 = 0x9a,
        TILE_SPIDER4 = 0x9b,
        /*ghost*/
        TILE_GHOST = 0x9C,
        TILE_GHOST2 = 0x9D,
        TILE_GHOST3 = 0x9E,
        TILE_GHOST4 = 0x9F,
        /*slime*/
        TILE_SLIME = 0xA0,
        TILE_SLIME2 = 0xA1,
        TILE_SLIME3 = 0xA2,
        TILE_SLIME4 = 0xA3,
        /*troll*/
        TILE_TROLL = 0xA4,
        TILE_TROLL2 = 0xA5,
        TILE_TROLL3 = 0xA6,
        TILE_TROLL4 = 0xA7,
        /*gremlin*/
        TILE_GREMLIN = 0xA8,
        TILE_GREMLIN2 = 0xA9,
        TILE_GREMLIN3 = 0xAa,
        TILE_GREMLIN4 = 0xAb,
        /*mimic*/
        TILE_MIMIC = 0xAC,
        TILE_MIMIC2 = 0xAd,
        TILE_MIMIC3 = 0xAe,
        TILE_MIMIC4 = 0xAf,
        /*reaper*/
        TILE_REAPER = 0xB0,
        TILE_REAPER2 = 0xB1,
        TILE_REAPER3 = 0xB2,
        TILE_REAPER4 = 0xB3,
        /*insects*/
        TILE_INSECTS = 0xB4,
        TILE_INSECTS2 = 0xB5,
        TILE_INSECTS3 = 0xB6,
        TILE_INSECTS4 = 0xB7,
        /*gazer*/
        TILE_GAZER = 0xB8,
        TILE_GAZER2 = 0xB9,
        TILE_GAZER3 = 0xBa,
        TILE_GAZER4 = 0xBb,
        /*phantom*/
        TILE_PHANTOM = 0xBC,
        TILE_PHANTOM2 = 0xBD,
        TILE_PHANTOM3 = 0xBE,
        TILE_PHANTOM4 = 0xBF,
        /*orc*/
        TILE_ORC = 0xC0,
        TILE_ORC2 = 0xC1,
        TILE_ORC3 = 0xC2,
        TILE_ORC4 = 0xC3,
        /*skeleton*/
        TILE_SKELETON = 0xC4,
        TILE_SKELETON2 = 0xC5,
        TILE_SKELETON3 = 0xC6,
        TILE_SKELETON4 = 0xC7,
        /*rogue*/
        TILE_ROGUE = 0xC8,
        TILE_ROGUE2 = 0xC9,
        TILE_ROGUE3 = 0xCa,
        TILE_ROGUE4 = 0xCb,
        /*python*/
        TILE_PYTHON = 0xCC,
        TILE_PYTHON2 = 0xCd,
        TILE_PYTHON3 = 0xCe,
        TILE_PYTHON4 = 0xCf,
        /*ettin*/
        TILE_ETTIN = 0xD0,
        TILE_ETTIN2 = 0xD1,
        TILE_ETTIN3 = 0xD2,
        TILE_ETTIN4 = 0xD3,
        /*headless*/
        TILE_HEADLESS = 0xD4,
        TILE_HEADLESS2 = 0xD5,
        TILE_HEADLESS3 = 0xD6,
        TILE_HEADLESS4 = 0xD7,
        /*cyclops*/
        TILE_CYCLOPS = 0xD8,
        TILE_CYCLOPS2 = 0xD9,
        TILE_CYCLOPS3 = 0xDa,
        TILE_CYCLOPS4 = 0xDb,
        /*wisp*/
        TILE_WISP = 0xDC,
        TILE_WISP2 = 0xDD,
        TILE_WISP3 = 0xDE,
        TILE_WISP4 = 0xDF,
        /*mage*/
        TILE_MAGE_NPC = 0xE0,
        TILE_MAGE_NPC2 = 0xE1,
        TILE_MAGE_NPC3 = 0xE2,
        TILE_MAGE_NPC4 = 0xE3,
        /*lyche*/
        TILE_LYCHE = 0xE4,
        TILE_LYCHE2 = 0xE5,
        TILE_LYCHE3 = 0xE6,
        TILE_LYCHE4 = 0xE7,
        /*lava lizard*/
        TILE_LAVA_LIZARD = 0xE8,
        TILE_LAVA_LIZARD2 = 0xE9,
        TILE_LAVA_LIZARD3 = 0xEa,
        TILE_LAVA_LIZARD4 = 0xEb,
        /*zorn*/
        TILE_ZORN = 0xEC,
        TILE_ZORN2 = 0xEd,
        TILE_ZORN3 = 0xEe,
        TILE_ZORN4 = 0xEf,
        /*daemon*/
        TILE_DAEMON = 0xF0,
        TILE_DAEMON2 = 0xF1,
        TILE_DAEMON3 = 0xF2,
        TILE_DAEMON4 = 0xF3,
        /*hydra*/
        TILE_HYDRA = 0xF4,
        TILE_HYDRA2 = 0xF5,
        TILE_HYDRA3 = 0xF6,
        TILE_HYDRA4 = 0xF7,
        /*dragon*/
        TILE_DRAGON = 0xF8,
        TILE_DRAGON2 = 0xF9,
        TILE_DRAGON3 = 0xFa,
        TILE_DRAGON4 = 0xFb,
        /*balron*/
        TILE_BALRON = 0xFC,
        TILE_BALRON2 = 0xFd,
        TILE_BALRON3 = 0xFe,
        TILE_BALRON4 = 0xFf,
    };

    public enum DIRECTION
    {
        DIR_W = 0,
        DIR_N = 1,
        DIR_E = 2,
        DIR_S = 3
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


    
    public GameObject partyGameObject;

    float timer = 0.0f;
    float timerExpired = 0.0f;
    public float timerPeriod = 0.02f; // the game operates on a 300ms Sleep() so we want to update things faster than that

    volatile byte[] buffer = new byte[2000];

    public byte[,] tMap32x32 = new byte[32, 32];
    public byte[,,] tMap8x8x8 = new byte[8, 8, 8];

    //public TILE hit_tile;
    //public byte hit_x;
    //public byte hit_y;

    [System.Serializable]
    public struct tCombat /*size:0xc0*/
    {
        public byte[] _npcX, _npcY; //16,16 /*_000/_010 D_9470/D_9480*/
        public byte[] _charaX, _charaY; //8,8 /*_20/_28 D_9490/D_9498*/
        //public byte[,] _map; //11 * 11 /*_040 D_94B0*/
    };

    tCombat Combat;

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

    //public t_68[] Fighters = new t_68[16];

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
        public ARMOR[] _armors; //8
        /*+16e*/
        public WEAPON[] _weapons; //16
        /*+18e*/
        public REAGENT[] _reagents; //8
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



    // Separate thread to run the game, we could attempt to make the data gathering function thread safe but for now this will do
    private void ThreadTask()
    {
        // start the DLL
        main();
    }

    // Start is called before the first frame update
    void Start()
    {
        // allocate storage for Party global
        Party.chara = new tChara[8];
        for (int i = 0; i < 8; i++)
        {
            Party.chara[i]._HP = new ushort[2];
            Party.chara[i].__0e = new byte[2];
        }
        Party._armors = new ARMOR[8];
        Party._weapons = new WEAPON[16];
        Party._reagents = new REAGENT[8];
        Party._mixtures = new ushort[26];

        // allocate storage for Combat global
        Combat._npcX = new byte[16];
        Combat._npcY = new byte[16];
        Combat._charaX = new byte[8];
        Combat._charaY = new byte[8];
        //Combat._map = new byte[11,11];

        // start a thread with the DLL main task
        trd = new Thread(new ThreadStart(this.ThreadTask));
        trd.IsBackground = true;
        trd.Start();
    }

    void OnApplicationQuit()
    {
        //trd.Abort();
        main_keyboardHit((char)KEYS.VK_ESCAPE);
    }

    public enum KEYS
    {
        VK_LEFT = 0x25,
        VK_UP = 0x26,
        VK_RIGHT = 0x27,
        VK_DOWN = 0x28,
        VK_ESCAPE = 0x1B,
        VK_SPACE          =0x20,
        VK_RETURN         =0x0D,
        VK_BACK           =0x08,
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

    // Update is called once per frame
    void Update()
    {
        int buffer_index;

        timer += Time.deltaTime;

        // get any hilighted character
        main_ActiveChar(buffer, buffer.Length);

        if (buffer[1] != 0xff)
        {
            currentActiveCharacter.active = true;
            currentActiveCharacter.characterIndex = buffer[0];
            currentActiveCharacter.x = buffer[1];
            currentActiveCharacter.y = buffer[2];
        }
        else
        {
            currentActiveCharacter.active = false;
        }

        // read in current hit info, this occurrs out of the main draw squence and for only a short time,
        // the DLL now saves the last hit tile as the hit tile is cleared very quickly but the x & y of the hit are not
        main_Hit(buffer, buffer.Length);

        int hit_length = buffer[0];

        // get all the new hits from the buffer and add them to the list
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

        // check for timer expiration
        for(int i = 0; i < currentHits.Count; i++)
        {
            hit checkHit = currentHits[i];

            // we will leave this hit up until we get another or the timer expires
            if (checkHit.time + hit_time_period < Time.time)
            {
                currentHits.Remove(checkHit);
            }
        }
 
        // send some keyboard codes down to the engine,
        // keydown is only active for a single frame so it cannot be in the timer
        if (Input.GetKeyDown(KeyCode.DownArrow))
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
        else if (Input.GetKeyDown(KeyCode.E))
        {
            main_keyboardHit((char)'E');
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            main_keyboardHit((char)'A');
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            main_keyboardHit((char)'G');
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            main_keyboardHit((char)'O');
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            main_keyboardHit((char)KEYS.VK_SPACE);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            main_keyboardHit((char)'C');
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            main_keyboardHit((char)'D');
        }

        // only get data periodically
        if (timer > timerExpired)
        {
            timer = timer - timerExpired;
            timerExpired = timerPeriod;

            // get the current game mode;
            MODE current_mode = main_CurMode();

            if ((current_mode == MODE.COMBAT) || (current_mode == MODE.COM_CAMP) || (current_mode == MODE.COM_ROOM))
            {

            }

            // get the current map and npc data
            main_CurMap(buffer, buffer.Length);

            // extract the map data
            if ((current_mode == MODE.OUTDOORS) || (current_mode == MODE.BUILDING))
            {
                buffer_index = 0;
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        tMap32x32[i, j] = buffer[buffer_index++];
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
                Party.chara[i]._name = "" + buffer[buffer_index++] + buffer[buffer_index++] + buffer[buffer_index++] + buffer[buffer_index++]
                    + buffer[buffer_index++] + buffer[buffer_index++] + buffer[buffer_index++] + buffer[buffer_index++];
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
                Party._armors[i] = (ARMOR)System.BitConverter.ToUInt16(buffer, 0x15e + i * 2);
            }
            for (int i = 0; i < 16; i++)
            {
                Party._weapons[i] = (WEAPON)System.BitConverter.ToUInt16(buffer, 0x16e + i * 2);
            }
            for (int i = 0; i < 8; i++)
            {
                Party._reagents[i] = (REAGENT)System.BitConverter.ToUInt16(buffer, 0x18e + i * 2);
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

            for (int i = 0; i < 16; i++)
            {
                Combat._npcX[i] = buffer[0x00 + i];
                Combat._npcY[i] = buffer[0x10 + i];
            }
            for (int i = 0; i < 8; i++)
            {
                Combat._charaX[i] = buffer[0x20 + i];
                Combat._charaY[i] = buffer[0x28 + i];
            }

            //skip the map for now as Unity will not serialize and display the stucture with this present
            /*
            buffer_index = 0x40;
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    Combat._map[i,j] = buffer[buffer_index++];
                }
            }
            */

            // read in the Fighters global
            main_Fighters(buffer, buffer.Length);

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
                worldList[0].DrawMap(map, currentHits, currentActiveCharacter);

                if ((current_mode == MODE.OUTDOORS) || (current_mode == MODE.BUILDING))
                {
                    worldList[0].AddNPCs(_npc);
                    worldList[0].followWorld();
                }
                else
                {
                    worldList[0].AddFighters(Fighters, Combat);
                    worldList[0].AddCharacters(Combat, Party, Fighters);
                }

                if ((worldList[0].party != null) && (worldList[0].tiles != null))
                {
                    // set the party tile, person, horse, ballon, ship, etc.
                    Renderer renderer = worldList[0].party.GetComponentInChildren<Renderer>();
                    if (renderer)
                    {
                        worldList[0].party.GetComponentInChildren<Renderer>().material.mainTexture = worldList[0].tiles[(int)Party._tile];
                    }
                }
            }
        }
    }
}
