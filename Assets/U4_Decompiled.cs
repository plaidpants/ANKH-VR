//#define USE_UNITY_DLL_FUNCTION

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;

public class U4_Decompiled : MonoBehaviour
{
    private Thread trd;

    public AudioSource specialEffectAudioSource;
    public string gameText;
    public string npcText;
    public TALK_INDEX npcTalkIndex = TALK_INDEX.INVALID;
    public bool started_playing_sound_effect = false;
    [SerializeField]
    public List<string> wordList = new List<string>();

    public enum TALK_INDEX
    {
        LORD_BRITISH = 0xff,
        HAWKKWIND = 0xfe,
        VENDOR_PUB = 0xfd,
        VENDOR_REAGENT = 0xfc,
        VENDOR_ARMOR = 0xfb,
        VENDOR_WEAPON = 0xfa,
        VENDOR_FOOD = 0xf9,
        VENDOR_HORSE = 0xf8,
        VENDOR_HEALER = 0xf7,
        VENDOR_INN = 0xf6,
        VENDOR_GUILD= 0xf5,
        INVALID = 0x7f,
        CITIZEN_31 = 31,
        CITIZEN_30 = 30,
        CITIZEN_29 = 29,
        CITIZEN_28 = 28,
        CITIZEN_27 = 27,
        CITIZEN_26 = 26,
        CITIZEN_25 = 25,
        CITIZEN_24 = 24,
        CITIZEN_23 = 23,
        CITIZEN_22 = 22,
        CITIZEN_21 = 21,
        CITIZEN_20 = 20,
        CITIZEN_19 = 19,
        CITIZEN_18 = 18,
        CITIZEN_17 = 17,
        CITIZEN_16 = 16,
        CITIZEN_15 = 15,
        CITIZEN_14 = 14,
        CITIZEN_13 = 13,
        CITIZEN_12 = 12,
        CITIZEN_11 = 11,
        CITIZEN_10 = 10,
        CITIZEN_9 = 9,
        CITIZEN_8 = 8,
        CITIZEN_7 = 7,
        CITIZEN_6 = 6,
        CITIZEN_5 = 5,
        CITIZEN_4 = 4,
        CITIZEN_3 = 3,
        CITIZEN_2 = 2,
        CITIZEN_1 = 1,
        CITIZEN_0 = 0,
    }

    public INPUT_MODE inputMode;

    public enum INPUT_MODE
    {
        // talk with people word input
        CITIZEN_WORD = 1,
        LOAD_BRITISH_WORD = 4,
        HAWKWIND_WORD = 6,
        PUB_WORD = 41,
        // other word input, end game, shrine, dungeon etc.
        VIRTUE_WORD = 38,
        MANTRA_WORD = 40,
        USE_WORD = 36,
        USE_STONE_COLOR_WORD = 37,
        END_GAME_INFINITY_WORD = 42,
        END_GAME_VERAMOCOR_WORD = 43,
        // general input
        GENERAL_YES_NO_WORD = 39,
        GENERAL_YES_NO = 10,
        GENERAL_NUMBER_INPUT_0_1_2_3 = 32,
        GENERAL_NUMBER_INPUT_1_DIGITS = 48,
        GENERAL_NUMBER_INPUT_2_DIGITS = 15,
        GENERAL_NUMBER_INPUT_3_DIGITS = 16,
        GENERAL_CONTINUE = 9,
        GENERAL_BUY_SELL = 29,
        GENERAL_DIRECTION = 11,
        GENERAL_ASK_CHARACTER_NUMBER = 35,
        // magic letter input
        ENERGY_TYPE_POISON_FIRE_LIGHTNING_SLEEP = 27,
        // vendor main input purchase menus using single letter inputs
        ASK_LETTER_FOOD_OR_ALE = 24,
        ASK_LETTER_REAGENT = 28,
        ASK_LETTER_ARMOR = 30,
        ASK_LETTER_WEAPON = 34,
        ASK_LETTER_GUILD = 31,
        ASK_LETTER_HEALER = 33,
        ASK_LETTER_SPELL = 47,
        ASK_LETTER_PHASE = 49,
        ASK_LETTER_TELESCOPE = 50,
        // main game engine input loops
        MAIN_LOOP = 18,
        DUNGEON_LOOP = 17,
        COMBAT_LOOP = 44,
        // delay is active, no input
        DELAY = 46,
        // drive letter for PCs, not really useful here
        DRIVE_LETTER = 45,
    }


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

    public TILE current_tile;

    public enum DIRECTION
    {
        WEST = 0,
        NORTH = 1,
        EAST = 2,
        SOUTH = 3
    }

    public enum MODE
    {
        VISION = 0,
        OUTDOORS = 1,
        BUILDING = 2,
        DUNGEON = 3,
        COMBAT = 4,
        COMBAT_CAMP = 5,
        COMBAT_ROOM = 6,
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
        MYSTIC_ROBE = 7,
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
        BRITANNIA = 1, // first and second floor is determined by the ladder up or down direction at [3, 3] in the map
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
        DUNGEONS = 17,
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

    static System.IntPtr nativeLibraryPtr;

#if !USE_UNITY_DLL_FUNCTION
    delegate void main();
    delegate MODE main_CurMode();
    delegate TILE main_D_96F8();
    delegate TILE main_D_946C();
    delegate int main_D_95A5_x();
    delegate int main_D_95A5_y();
    delegate void main_keyboardHit(char key);
    delegate void main_CurMap(byte[] buffer, int length);
    delegate void main_Combat(byte[] buffer, int length);
    delegate void main_Fighters(byte[] buffer, int length);
    delegate void main_D_96F9(byte[] buffer, int length);
    delegate void main_Party(byte[] buffer, int length);
    delegate void main_Hit(byte[] buffer, int length);
    delegate void main_ActiveChar(byte[] buffer, int length);
    delegate TILE main_tile_cur();
    delegate DIRECTION main_WindDir();
    delegate int main_spell_sta();
    delegate int main_Text(byte[] buffer, int length);
    delegate int main_D_9445(); // moongate x
    delegate int main_D_9448(); // moongate y
    delegate TILE main_D_9141(); // moongate tile
    delegate int main_NPC_Text(byte[] buffer, int length);
    delegate int main_D_17FA();
    delegate int main_D_17FC();
    delegate int main_D_17FE();
    delegate int main_SoundFlag();
    delegate void main_SetDataPath(byte[] buffer, int length);
    delegate void main_char_highlight(byte[] buffer, int length);
    delegate int main_sound_effect();
    delegate int main_sound_effect_length();
    delegate void main_sound_effect_done();
    delegate int main_screen_xor_state();
    delegate int main_camera_shake_accumulator();
    delegate int main_D_1665();
    delegate int main_D_1666();
    delegate int main_input_mode();
#endif

    void Awake()
    {
        //Debug.Log("Load songs");
        LoadSongs();

        //Debug.Log("Patch AVATAR.EXE to AVATAR.DLL");
        // create a DLL file from the original DOS AVATAR.EXE file by patching it
        var sourceFile = new FileInfo(Application.persistentDataPath + "/u4/AVATAR.EXE");
        var patchFile = new FileInfo(Application.persistentDataPath + "/u4/AVATAR.bps");
#if PLATFORM_ANDROID && !UNITY_EDITOR
        var targetFile = new FileInfo(Application.persistentDataPath + "/u4/AVATAR.so");
#else
        var targetFile = new FileInfo(Application.persistentDataPath + "/u4/AVATAR.DLL");
#endif

        DecoderBSP.ApplyPatch(sourceFile, patchFile, targetFile);


#if USE_UNITY_DLL_FUNCTION
        //SetDllDirectory(Application.persistentDataPath + "/u4/");
        //LoadLibrary(Application.persistentDataPath + "/u4/AVATAR.DLL");
#else
        //Debug.Log("Load AVATAR.DLL");
        // now attempt to load this DLL
        if (nativeLibraryPtr != System.IntPtr.Zero)
        {
            return;
        }

#if PLATFORM_ANDROID && !UNITY_EDITOR
        nativeLibraryPtr = Native.dlopen(Application.persistentDataPath + "/u4/AVATAR", (int)Native.PosixDlopenFlags.Now);
#else
        nativeLibraryPtr = Native.LoadLibrary(Application.persistentDataPath + "/u4/AVATAR.DLL");
#endif
        if (nativeLibraryPtr == System.IntPtr.Zero)
        {
            Debug.LogError("Failed to load native library");
        }
#endif

        // Set the data path for the DLL before we start the thread,
        // cstring are hard so we will just send the string buffer and a length.
        string path = Application.persistentDataPath + "/u4/";
        for (int i = 0; i < path.Length; i++)
        {
            buffer[i] = (byte)path[i];
        }
        buffer[path.Length] = 0;
#if USE_UNITY_DLL_FUNCTION
        main_SetDataPath(buffer, path.Length);
#else
        Native.Invoke<main_SetDataPath>(nativeLibraryPtr, buffer, path.Length); 
#endif
    }

#if USE_UNITY_DLL_FUNCTION
#if PLATFORM_ANDROID && !UNITY_EDITOR
    // interface to the game engine
    [DllImport("AVATAR")]
    public static extern void main();
    [DllImport("AVATAR")]
    public static extern MODE main_CurMode();
    [DllImport("AVATAR")]
    public static extern TILE main_D_96F8();
    [DllImport("AVATAR")]
    public static extern TILE main_D_946C();
    [DllImport("AVATAR")]
    public static extern int main_D_95A5_x();
    [DllImport("AVATAR")]
    public static extern int main_D_95A5_y();
    [DllImport("AVATAR")]
    public static extern void main_keyboardHit(char key);
    [DllImport("AVATAR")]
    public static extern void main_CurMap(byte[] buffer, int length);
    [DllImport("AVATAR")]
    public static extern void main_Combat(byte[] buffer, int length);
    [DllImport("AVATAR")]
    public static extern void main_Fighters(byte[] buffer, int length);
    [DllImport("AVATAR")]
    public static extern void main_D_96F9(byte[] buffer, int length);
    [DllImport("AVATAR")]
    public static extern void main_Party(byte[] buffer, int length);
    [DllImport("AVATAR")]
    public static extern void main_Hit(byte[] buffer, int length);
    [DllImport("AVATAR")]
    public static extern void main_ActiveChar(byte[] buffer, int length);
    [DllImport("AVATAR")]
    public static extern TILE main_tile_cur();
    [DllImport("AVATAR")]
    public static extern DIRECTION main_WindDir();
    [DllImport("AVATAR")]
    public static extern int main_spell_sta();
    [DllImport("AVATAR")]
    public static extern int main_Text(byte[] buffer, int length);
    [DllImport("AVATAR")]
    public static extern int main_D_9445(); // moongate x
    [DllImport("AVATAR")]
    public static extern int main_D_9448(); // moongate y
    [DllImport("AVATAR")]
    public static extern TILE main_D_9141(); // moongate tile
    [DllImport("AVATAR")]
    public static extern int main_NPC_Text(byte[] buffer, int length);
    [DllImport("AVATAR")]
    public static extern int main_D_17FA();
    [DllImport("AVATAR")]
    public static extern int main_D_17FC();
    [DllImport("AVATAR")]
    public static extern int main_D_17FE();
    [DllImport("AVATAR")]
    public static extern int main_SoundFlag();
    [DllImport("AVATAR")]
    public static extern void main_SetDataPath(byte[] buffer, int length);
    [DllImport("AVATAR")]
    public static extern void main_char_highlight(byte[] buffer, int length);
    [DllImport("AVATAR")]   
    public static extern int main_sound_effect();
    [DllImport("AVATAR")]   
    public static extern int main_sound_effect_length();
    [DllImport("AVATAR")]   
    public static extern void main_sound_effect_done();
    [DllImport("AVATAR")]   
    public static extern int main_screen_xor_state();  
    [DllImport("AVATAR")]   
    public static extern int main_camera_shake_accumulator();  
    [DllImport("AVATAR")]   
    public static extern int main_D_1665();  
    [DllImport("AVATAR")]   
    public static extern int main_D_1666();  
#else
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern bool SetDllDirectory(string lpPathName);
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern System.IntPtr LoadLibrary(string lpFileName);
    // interface to the game engine
    [DllImport("AVATAR.DLL")]
    public static extern void main();
    [DllImport("AVATAR.DLL")]
    public static extern MODE main_CurMode();
    [DllImport("AVATAR.DLL")]
    public static extern TILE main_D_96F8();
    [DllImport("AVATAR.DLL")]
    public static extern TILE main_D_946C();
    [DllImport("AVATAR.DLL")]
    public static extern int main_D_95A5_x();
    [DllImport("AVATAR.DLL")]
    public static extern int main_D_95A5_y();
    [DllImport("AVATAR.DLL")]
    public static extern void main_keyboardHit(char key);
    [DllImport("AVATAR.DLL")]
    public static extern void main_CurMap(byte[] buffer, int length);
    [DllImport("AVATAR.DLL")]
    public static extern void main_Combat(byte[] buffer, int length);
    [DllImport("AVATAR.DLL")]
    public static extern void main_Fighters(byte[] buffer, int length);
    [DllImport("AVATAR.DLL")]
    public static extern void main_D_96F9(byte[] buffer, int length);
    [DllImport("AVATAR.DLL")]
    public static extern void main_Party(byte[] buffer, int length);
    [DllImport("AVATAR.DLL")]
    public static extern void main_Hit(byte[] buffer, int length);
    [DllImport("AVATAR.DLL")]
    public static extern void main_ActiveChar(byte[] buffer, int length);
    [DllImport("AVATAR.DLL")]
    public static extern TILE main_tile_cur();
    [DllImport("AVATAR.DLL")]
    public static extern DIRECTION main_WindDir();
    [DllImport("AVATAR.DLL")]
    public static extern int main_spell_sta();
    [DllImport("AVATAR.DLL")]
    public static extern int main_Text(byte[] buffer, int length);
    [DllImport("AVATAR.DLL")]
    public static extern int main_D_9445(); // moongate x
    [DllImport("AVATAR.DLL")]
    public static extern int main_D_9448(); // moongate y
    [DllImport("AVATAR.DLL")]
    public static extern TILE  main_D_9141(); // moongate tile
    [DllImport("AVATAR.DLL")]
    public static extern int main_NPC_Text(byte[] buffer, int length);
    [DllImport("AVATAR.DLL")]
    public static extern int main_D_17FA(); 
    [DllImport("AVATAR.DLL")]
    public static extern int main_D_17FC();
    [DllImport("AVATAR.DLL")]
    public static extern int main_D_17FE();
    [DllImport("AVATAR.DLL")]
    public static extern int main_SoundFlag();
    [DllImport("AVATAR.DLL")]
    public static extern void main_SetDataPath(byte[] buffer, int length);
    [DllImport("AVATAR.DLL")]
    public static extern void main_char_highlight(byte[] buffer, int length);   
    [DllImport("AVATAR.DLL")]
    public static extern int main_sound_effect();
    [DllImport("AVATAR.DLL")]   
    public static extern int main_sound_effect_length();
    [DllImport("AVATAR.DLL")]   
    public static extern void main_sound_effect_done();
    [DllImport("AVATAR.DLL")]   
    public static extern int main_screen_xor_state();
    [DllImport("AVATAR.DLL")]   
    public static extern int main_camera_shake_accumulator();  
    [DllImport("AVATAR.DLL")]   
    public static extern int main_D_1665();  
    [DllImport("AVATAR.DLL")]   
    public static extern int main_D_1666();  
#endif
#endif

    float timer = 0.0f;
    float timerExpired = 0.0f;
    public float timerPeriod = 0.02f; // the game operates on a 300ms Sleep() so we want to update things faster than that

    // buffer used to read stuff from the game engine
    byte[] buffer = new byte[10000];

    // game engine map buffers
    public TILE[,] tMap32x32 = new TILE[32, 32];
    public byte[,,] tMap8x8x8 = new byte[8, 8, 8];

    // game engine game mode
    public MODE current_mode;

    // game engine active moongate infomation
    public TILE moongate_tile;
    public int moongate_x; 
    public int moongate_y;

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

    public enum SEX
    {
        MALE = 0xb,
        FEMALE = 0xc
    };

    public enum STATE
    {
        GOOD = 'G',
        POISIONED = 'P',
        SLEEPING = 'S',
        DEAD = 'D'
    };

    public enum CLASS
    {
        MAGE = 0,
        BARD = 1,
        FIGHTER = 2,
        DRUID = 3,
        TINKER = 4,
        PALADIN = 5,
        RANGER = 6,
        SHEPHERD = 7
    };

    [System.Serializable]
    public struct Character
    {
        public ushort hitPoint;
        public ushort hitPointsMaximum;
        public ushort experiencePoints;
        public ushort strength;
        public ushort dexterity;
        public ushort intelligence;
        public ushort magicPoints;
        public byte[] __0e; //2
        public WEAPON currentWeapon;
        public ARMOR currentArmor;
        public string name; // char _name[16];
        public SEX sex;
        public CLASS Class;
        public STATE state;
        public bool highlight;
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
        public Character[] chara; //8
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
        public DIRECTION _dir;/*[dungeon]*/
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

    public TILE[,] displayTileMap = new TILE[11, 11];
    public enum COMBAT_TERRAIN
    {
        // this order and numbering is important up to at least CAMP
        // the names are also important as they are used to load the filenames from the original game
        GRASS = 0,
        BRIDGE = 1,
        BRICK = 2,
        DUNGEON = 3, // just all tiles, used outside when on dungeon entrance, not inside a dungeon
        HILL = 4,
        FOREST = 5,
        BRUSH = 6,
        MARSH = 7,
        SHIPSEA = 8,
        SHIPSHOR = 9,
        SHORE = 10,
        SHIPSHIP = 11,
        SHORSHIP = 12,
        CAMP = 13,

        INN = 14,
        SHRINE = 15,
        DNG0 = 16, // hallway
        DNG1 = 17, // ladder up
        DNG2 = 18, // ladder down
        DNG3 = 19, // ladder up and down
        DNG4 = 20, // chest
        DNG5 = 21, // doorway
        DNG6 = 22, // secret doorway
        MAX = 23
    };

    // Separate thread to run the game, we could attempt to make the data gathering function thread safe but for now this will do
    private void ThreadTask()
    {
        // start the DLL main thread
#if USE_UNITY_DLL_FUNCTION
        main();
#else
        Native.Invoke<main>(nativeLibraryPtr);
#endif
    }


    // Start is called before the first frame update
    void Start()
    {
        // allocate storage for Party globals
        Party.chara = new Character[8];
        for (int i = 0; i < 8; i++)
        {
            Party.chara[i].__0e = new byte[2];
        }
        Party._armors = new ushort[8];
        Party._weapons = new ushort[16];
        Party._reagents = new ushort[8];
        Party._mixtures = new ushort[26];
    }

   public void StartThread()
    {
        // start a thread with the DLL main task
        trd = new Thread(new ThreadStart(this.ThreadTask));
        trd.IsBackground = true;
        trd.Start();
    }


    public void CommandAttack()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('A');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'A');
#endif
    }

    public void CommandCharacter1()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('1');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'1');
#endif
    }

    public void CommandCharacter2()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('2');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'2');
#endif
    }
    public void CommandCharacter3()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('3');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'3');
#endif
    }
    public void CommandCharacter4()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('4');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'4');
#endif
    }
    public void CommandCharacter5()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('5');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'5');
#endif
    }
    public void CommandCharacter6()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('6');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'6');
#endif
    }
    public void CommandCharacter7()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('7');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'7');
#endif
    }
    public void CommandCharacter8()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('8');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'8');
#endif
    }

    public void CommandBoard()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('B');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'B');
#endif
    }

    public void CommandCast()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('C');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'C');
#endif
    }

    public void CommandDecsend()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('D');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'D');
#endif
    }
    public void CommandEnter()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('E');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'E');
#endif
    }

    public void CommandFire()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('F');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'F');
#endif
    }
    public void CommandGet()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('G');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'G');
#endif
    }
    public void CommandHoleUp()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('H');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'H');
#endif
    }
    public void CommandIgnight()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('I');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'I');
#endif
    }
    public void CommandJimmy()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('J');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'J');
#endif
    }

    public void CommandKlimb()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('K');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'K');
#endif
    }

    public void CommandLocate()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('L');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'L');
#endif
    }

    public void CommandMix()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('M');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'M');
#endif
    }

    public void CommandNewOrder()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('N');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'N');
#endif
    }
    public void CommandOpen()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('O');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'O');
#endif
    }
    public void CommandPeer()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('P');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'P');
#endif
    }

    public void CommandPass()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit(' ');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, ' ');
#endif
    }

    public void CommandQuit()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('Q');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'Q');
#endif
    }
    public void CommandReady()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('R');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'R');
#endif
    }
    public void CommandSearch()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('S');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'S');
#endif
    }
    public void CommandTalk()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('T');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'T');
#endif
    }

    public void CommandUse()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('U');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'U');
#endif
    }

    public void CommandVolume()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('V');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'V');
#endif
    }

    public void CommandWear()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('W');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'W');
#endif
    }

    public void CommandXit()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('X');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'X');
#endif
    }

    public void CommandYell()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('Y');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'Y');
#endif
    }

    public void CommandZStas()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('Z');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'Z');
#endif
    }

    IEnumerator SayWordCoroutine(string word)
    {
        string upper = word.ToUpper();

        for (int i = 0; i < upper.Length; i++)
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)upper[i]);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)upper[i]);
#endif
            //yield on a new YieldInstruction that waits for 5 seconds.
            yield return new WaitForSeconds(0.05f);
        }

#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit((char)KEYS.VK_RETURN);
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RETURN);
#endif
    }

    public void CommandSayName()
    {
        string word = "Name\n";

        StartCoroutine(SayWordCoroutine(word));
    }

    public static string clickedButtonName = "";

    public void CommandSayButtonName()
    {
        StartCoroutine(SayWordCoroutine(clickedButtonName));
    }
    public void CommandSayWord(string word)
    {
        StartCoroutine(SayWordCoroutine(word));
    }
    public void CommandSayCharacter(char character)
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit(character);
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, character);
#endif
    }

    public void CommandSayJob()
    {
        string word = "Job\n";
        StartCoroutine(SayWordCoroutine(word));
    }

    public void CommandSayHealth()
    {
        string word = "Health\n";
        StartCoroutine(SayWordCoroutine(word));
    }

    public void CommandSayJoin()
    {
        string word = "Join\n";
        StartCoroutine(SayWordCoroutine(word));
    }

    public void CommandSayGive()
    {
        string word = "Give\n";
        StartCoroutine(SayWordCoroutine(word));
    }

    public void CommandSayYes()
    {
        string word = "Yes\n";
        StartCoroutine(SayWordCoroutine(word));
    }
    public void CommandSayNo()
    {
        string word = "No\n";
        StartCoroutine(SayWordCoroutine(word));
    }

    public void CommandSayContinue()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit((char)KEYS.VK_RETURN);
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RETURN);
#endif
    }

    public void CommandSayLook()
    {
        string word = "Look\n";
        StartCoroutine(SayWordCoroutine(word));
    }
    public void CommandSayBye()
    {
        string word = "Bye\n";
        StartCoroutine(SayWordCoroutine(word));
    }

    public void CommandSayHonesty()
    {
        string word = "Honesty\n";
        StartCoroutine(SayWordCoroutine(word));
    }
    public void CommandSayCompassion()
    {
        string word = "Compassion\n";
        StartCoroutine(SayWordCoroutine(word));
    }
    public void CommandSayValor()
    {
        string word = "Valor\n";
        StartCoroutine(SayWordCoroutine(word));
    }
    public void CommandSayJustice()
    {
        string word = "Justice\n";
        StartCoroutine(SayWordCoroutine(word));
    }
    public void CommandSaySacrifice()
    {
        string word = "Sacrifice\n";
        StartCoroutine(SayWordCoroutine(word));
    }
    public void CommandSayHonor()
    {
        string word = "Honor\n";
        StartCoroutine(SayWordCoroutine(word));
    }
    public void CommandSaySpirituality()
    {
        string word = "Spirituality\n";
        StartCoroutine(SayWordCoroutine(word));
    }
    public void CommandSayHumility()
    {
        string word = "Humility\n";
        StartCoroutine(SayWordCoroutine(word));
    }
    public void CommandSayNone()
    {
        string word = "None\n";
        StartCoroutine(SayWordCoroutine(word));
    }

    public string keyword1;
    public string keyword2;

    public void CommandSayKeyword1()
    {
        StartCoroutine(SayWordCoroutine(keyword1));
    }
    public void CommandSayKeyword2()
    {
        StartCoroutine(SayWordCoroutine(keyword2));
    }

    public void CommandSayY()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('Y');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'Y');
#endif
    }
    public void CommandSayFood()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('F');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'F');
#endif
    }

    public void CommandSayAle()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('A');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'A');
#endif
    }
    public void CommandSayBuy()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('B');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'B');
#endif
    }

    public void CommandSaySell()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('S');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'S');
#endif
    }

    public void CommandSayCuring()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('A');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'A');
#endif
    }

    public void CommandSayHealing()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('B');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'B');
#endif
    }
    public void CommandSayResurection()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('C');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'C');
#endif
    }
    public void CommandSayN()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('N');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'N');
#endif
    }

    void OnApplicationQuit()
    {
        // CAUTION: this will cleanly exit the DLL in most cases, however is some areas such as combat this will not work and you will
        // need to kill the unity editor process to be able to restart. The abort function will cause the Unity editor to exit which
        // is not desired. Using the exit() function in the DLL will also exit the unity editor which is also not desired. This
        // was the best solution I could come up with for the moment. In the final app all threads will be exiting when quiting so
        // it is not an issue there, only when using the Unity editor do we have trouble. If Unity would load and unload the DLL at runtime
        // this would be a better solution but Unity 3D unfortantly loads any project DLLs at editor launch and keeps them loaded until you
        // quit the editor.

/* this doesn't work well
try
{
    trd.Abort();
}
catch
{ 
    Debug.Log("error");
}
*/

// signal to the game engine thread to exit all forever loops and return
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit((char)KEYS.VK_ESCAPE);
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_ESCAPE);
#endif

        // wait for the game engine thread to complete/return
        while (trd.IsAlive == true)
        {
            ;
        }

        // It is now safe to unload the DLL
        if (nativeLibraryPtr != System.IntPtr.Zero)
        {
            //Debug.Log("Unload AVATAR.DLL");
#if PLATFORM_ANDROID && !UNITY_EDITOR
            Debug.Log(Native.dlclose(nativeLibraryPtr) == 0
                          ? "Native library successfully unloaded."
                          : "Native library could not be unloaded.");
#else
            Debug.Log(Native.FreeLibrary(nativeLibraryPtr)
                          ? "Native library successfully unloaded."
                          : "Native library could not be unloaded.");
#endif
        }
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

    [SerializeField]
    public struct activeCharacter
    {
        public bool active;
        public byte characterIndex;
        public byte x, y;
    }

    [SerializeField]
    public activeCharacter currentActiveCharacter;

    public float hit_time_period = 0.25f;

    public struct hit
    {
        public TILE tile;
        public byte x;
        public byte y;
        public float time;
    }

    public List<hit> currentHits = new List<hit>() { };
    public TILE D_96F8; // tile under attacker tile (used when pirate attack to determine combat map to use)
    public TILE D_946C; // attacker tile (used when pirate attack to determine combat map to use)

    public struct mapPosition
    {
        public byte x;
        public byte y;
    }

    public mapPosition D_95A5;

    public DIRECTION WindDir;
    public int spell_sta;

    public System.Text.ASCIIEncoding enc;

    AudioClip[] music = new AudioClip[(int)MUSIC.MAX];

    // order played in the original intro, from http://www.applevault.com/ultima/
    public enum MUSIC
    {
        TOWNS = 0,
        SHOPPING = 1,
        DUNGEON = 2,
        CASTLES = 3,
        RULEBRIT = 4,
        WANDERER = 5, // OUTSIDE
        COMBAT = 6,
        SHRINES = 7,
        FANFARE = 8,
        MAX = 9
    }

    public AudioClip[] soundEffects = new AudioClip[(int)SOUND_EFFECT.MAX];

    public enum SOUND_EFFECT
    {
        FOOTSTEP = 0,
        BLOCKED = 1,
        WHAT = 2,
        CANNON = 3,
        PLAYER_ATTACK = 4,
        FOE_ATTACK = 5,
        PLAYER_HITS = 6,
        FOE_HITS = 7,
        FLEE = 8,
        SPELL_EFFECT = 9, // length is the fequency of the magic high pitch tone
        SPELL_CAST = 10, // length is the number or short random tones
        WHIRL_POOL = 11,
        TWISTER = 12,
        MAX = 13
    };

    public void LoadSongs()
    {
        for( int i = 0; i < (int)MUSIC.MAX; i++)
        {
            StartCoroutine(LoadSongCoroutine(Application.persistentDataPath + "/u4/" + ((MUSIC)i).ToString() + ".OGG", (MUSIC)i));
        }
    }

    IEnumerator LoadSongCoroutine(string path, MUSIC index)
    {
        string url = string.Format("file://{0}", path);
        //Debug.Log("Load #" + (int)index + " " + url);
        WWW www = new WWW(url);
        yield return www;
        // note the updated interface does not seem to work with local files so don't bother updating until Unity fixes this
        //Debug.Log("Loaded #" + (int)index + " " + url);
        music[(int)index] = www.GetAudioClip(false, false);
    }

    /* 
    sound sample information from DOS version for sound 9 & 10, measured manually

    40 up down cycles per random tone for sound 10

    cure
    sound 10 length 5 followed by
    sound 9 length 98

    heal
    sound 10 length 10 followed by
    sound 9 length 103, total length 1.103s 51 pulses in .022 seconds or 2318 pulses per second for a total of 2557 pulses over 1.103s
    103 -> 1.103s -> ratio is 93

    moongate 
    sound 9 lenth 160, total length 1.685s  42 pulses in .028 seconds or 1500 pulses per second or 2527.5 pulses over 1.685s
    160 -> 1.685 -> ratio is 95

    assumed 2560 total pulses over ~length/94 seconds
    */

    /* from apple2 assembly
sfx_magic1:
	stx zp_sfx_duration
@1:
	jsr rand // get a random value
	ldx #$28 // 40
@2:
	tay  // y = a i.e. use random value in y
@3:
	dey // y--
	bne @3 // if y == 0, delay loop
	bit hw_SPEAKER
	dex // x-- count to 40
	bne @2
	dec zp_sfx_duration  // length
	bne @1
	rts
    */
    AudioClip CreateMagicSpecialEffectSound(int length)
    {
        float sampleRate = 44100;
        float max = 500f;
        float min = 200f;
        int channels = 2;
        int cycles = 20;
        int count = 0;
        float frequency = min;
        float[] fequencies = new float[length];
        float[] data;
        float phase = 0;
        float sampleCount = 0f;

        // make some random frequencies and calc total samples based on those frequencies
        for (int i = 0; i < length; i++)
        {
            frequency = Random.Range(min, max);
            sampleCount += (float)cycles * sampleRate / frequency;
            fequencies[i] = frequency;
        }

        // allocate total clip size based on above
        data = new float[(int)sampleCount * 2];

        // create the samples
        for (int i = 0; i < data.Length; i += channels)
        {
            phase += 2 * Mathf.PI * frequency / sampleRate;

            // output on all available channels
            for (int j = 0; j < channels; j++)
            {
                // sine wave
                //data[i + j] = Mathf.Sin(phase);

                // square wave
                data[i + j] = Mathf.Sin(phase) > 0 ? 0.5f : -0.5f;
            }

            // reset the phase so the numbers don't get too big
            if (phase >= 2 * Mathf.PI)
            {
                phase -= 2 * Mathf.PI;
                count++;
            }

            // switch to a new frequency after so many cycles
            if (count > cycles)
            {
                count = 0;
                frequency = Random.Range(min, max);
            }
        }

        // create the audio clip
        AudioClip soundEffect = AudioClip.Create("magic", data.Length, 2, (int)sampleRate, false);

        // set the sample data
        soundEffect.SetData(data, 0);

        // return the audio clip
        return soundEffect;
    }

    /* from apple2 assembly
sfx_magic2:
	stx sfx_m2_ctr2
	lda #$01
	sta sfx_m2_ctr1
@1:
	lda #$30
	sta zp_sfx_duration
@2:
	ldx sfx_m2_ctr2
@3:
	dex
	bne @3
	bit hw_SPEAKER
	ldx sfx_m2_ctr1
@4:
	dex
	bne @4
	bit hw_SPEAKER
	dec zp_sfx_duration
	bne @2
	dec sfx_m2_ctr2
	inc sfx_m2_ctr1
	lda sfx_m2_ctr1
	cmp #$1b
	bne @1
@5:
	lda #$30
	sta zp_sfx_duration
@6:
	ldx sfx_m2_ctr2
@7:
	dex
	bne @7
	bit hw_SPEAKER
	ldx sfx_m2_ctr1
@8:
	dex
	bne @8
	bit hw_SPEAKER
	dec zp_sfx_duration
	bne @6
	dec sfx_m2_ctr1
	inc sfx_m2_ctr2
	lda sfx_m2_ctr1
	cmp #$00
	bne @5
	rts
    */

    // used to tune the magic effect tone
    public float adjustSound = 94f;

    AudioClip CreateMagicEffectsSpecialEffectSound(int length)
    {
        float sampleRate = 44100;
        int channels = 2;
        int cycles = 2560;
        int count = 0;
        float frequency;
        float[] data;
        float phase1 = 0;
        float phase2 = 0;
        float sampleCount;

        frequency = cycles / ((float)length / adjustSound);
        sampleCount = (float)cycles * sampleRate / frequency;

        // allocate total clip size based on above
        data = new float[(int)sampleCount * 2];

        // create the samples
        for (int i = 0; i < data.Length; i += channels)
        {
            phase1 += 2 * Mathf.PI * frequency / sampleRate;
            phase2 += Mathf.PI / sampleCount;

            // output on all available channels
            for (int j = 0; j < channels; j++)
            {
                // 1/2 sine wave
                float data1 = Mathf.Abs(Mathf.Sin(phase1)) * Mathf.Sin(phase2);

                // clamp it
                if (data1 > 0.5f)
                {
                    data1 = 0.5f;
                }

                // store it
                data[i + j] = data1;

                // square wave
                //data[i + j] = Mathf.Sin(phase) > 0 ? 0.5f : -0.5f;
            }

            // reset the phase so the numbers don't get too big
            if (phase1 >= 2 * Mathf.PI)
            {
                phase1 -= 2 * Mathf.PI;
                count++;
            }
        }

        // create the audio clip
        AudioClip soundEffect = AudioClip.Create("magic", data.Length, 2, (int)sampleRate, false);

        // set the sample data
        soundEffect.SetData(data, 0);

        // return the audio clip
        return soundEffect;
    }

    // used to detect game mode changes and change the music
    MODE lastMode = (MODE)(-1);
    // used to detect when we should play the lord british music
    TALK_INDEX lastNPCTalkIndex = (TALK_INDEX)(-1);

    // extra surface rotation feature maintained outside of the game engine
    public U4_Decompiled.DIRECTION surface_party_direction = DIRECTION.NORTH;

    public float resetJoystick1 = 0f;
    public float resetJoystick2 = 0f;
    public float joystickResetTime = 0.1f;

    // Update is called once per frame
    void Update()
    {
        int buffer_index;

        timer += Time.deltaTime;

        // reset the joysticks if they are idle
        if ((Input.GetAxis("Horizontal 1") < 0.05f) && (Input.GetAxis("Horizontal 1") > -0.05f) && (Input.GetAxis("Vertical 1") < 0.05f) && (Input.GetAxis("Vertical 1") > -0.05f))
        {
            resetJoystick1 = Time.time;
        }
        if ((Input.GetAxis("Horizontal 2") < 0.05f) && (Input.GetAxis("Horizontal 2") > -0.05f) && (Input.GetAxis("Vertical 2") < 0.05f) && (Input.GetAxis("Vertical 2") > -0.05f))
        {
            resetJoystick2 = Time.time;
        }

        // ignore joystick input if we are using the grip to do VR interactions
        if (Input.GetAxis("Grip 1") > 0.5)
        {
            resetJoystick1 = Time.time + joystickResetTime;
        }
        if (Input.GetAxis("Grip 2") > 0.5)
        {
            resetJoystick2 = Time.time + joystickResetTime;
        }

        // check input
        if (Input.GetKeyDown(KeyCode.PageDown) || (Input.GetAxis("Horizontal 1") > 0.99f && (resetJoystick1 < Time.time)))
        {
            resetJoystick1 = Time.time + joystickResetTime;
            if (surface_party_direction == DIRECTION.NORTH)
            {
                surface_party_direction = DIRECTION.EAST;
            }
            else if (surface_party_direction == DIRECTION.EAST)
            {
                surface_party_direction = DIRECTION.SOUTH;
            }
            else if (surface_party_direction == DIRECTION.SOUTH)
            {
                surface_party_direction = DIRECTION.WEST;
            }
            else if (surface_party_direction == DIRECTION.WEST)
            {
                surface_party_direction = DIRECTION.NORTH;
            }
        }
        else if (Input.GetKeyDown(KeyCode.PageUp) || Input.GetAxis("Horizontal 1") < -0.99f && (resetJoystick1 < Time.time))
        {
            resetJoystick1 = Time.time + joystickResetTime;
            if (surface_party_direction == DIRECTION.NORTH)
            {
                surface_party_direction = DIRECTION.WEST;
            }
            else if (surface_party_direction == DIRECTION.WEST)
            {
                surface_party_direction = DIRECTION.SOUTH;
            }
            else if (surface_party_direction == DIRECTION.SOUTH)
            {
                surface_party_direction = DIRECTION.EAST;
            }
            else if (surface_party_direction == DIRECTION.EAST)
            {
                surface_party_direction = DIRECTION.NORTH;
            }
        }
        else

        // send some keyboard codes down to the engine,
        // Unity keydown is only active for a single frame so it cannot be in the timer check if
        if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.Z)) // need to check this first as it overrides the normal Z keypress
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'9'); // currently the windows implementation of this engine does not support this
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'9');
#endif

        }
        else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.S)) // need to check this first as it overrides the normal S keypress
        {

#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'9'); // currently the windows implementation of this engine does not support this
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'9');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.End))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_END);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_END);
#endif
        }
        else if (Input.GetKeyDown(KeyCode.Home))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_HOME);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_HOME);
#endif
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
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_RETURN);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RETURN);
#endif
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || (Input.GetAxis("Vertical 2") > 0.99f && (resetJoystick2 < Time.time)) || Input.GetAxis("Vertical 1") > 0.99f && (resetJoystick1 < Time.time))
        {
            resetJoystick1 = Time.time + joystickResetTime;
            resetJoystick2 = Time.time + joystickResetTime;
            if ((current_mode == MODE.COMBAT_ROOM) ||
                    ((current_mode == MODE.COMBAT) && (Party._loc >= U4_Decompiled.LOCATIONS.DECEIT) && (Party._loc <= U4_Decompiled.LOCATIONS.THE_GREAT_STYGIAN_ABYSS)))
            {
                if (Party._dir == DIRECTION.NORTH)
                {

#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_DOWN);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_DOWN);
#endif
                }
                else if (Party._dir == DIRECTION.EAST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_LEFT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_LEFT);
#endif
                }
                else if (Party._dir == DIRECTION.SOUTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_UP);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_UP);
#endif
                }
                else if (Party._dir == DIRECTION.WEST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_RIGHT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RIGHT);
#endif
                }
            }
            else if ((current_mode == MODE.OUTDOORS) || (current_mode == MODE.BUILDING))
            {
                if (surface_party_direction == DIRECTION.NORTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_DOWN);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_DOWN);
#endif
                }
                else if (surface_party_direction == DIRECTION.EAST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_LEFT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_LEFT);
#endif
                }
                else if (surface_party_direction == DIRECTION.SOUTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_UP);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_UP);
#endif
                }
                else if (surface_party_direction == DIRECTION.WEST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_RIGHT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RIGHT);
#endif
                }
            }
            else
            {
#if USE_UNITY_DLL_FUNCTION
                main_keyboardHit((char)KEYS.VK_DOWN);
#else
                Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_DOWN);
#endif
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || (Input.GetAxis("Vertical 2") < -0.99f && (resetJoystick2 < Time.time)) || (Input.GetAxis("Vertical 1") < -0.99f && (resetJoystick1 < Time.time)))
        {
            resetJoystick1 = Time.time + joystickResetTime;
            resetJoystick2 = Time.time + joystickResetTime;
            if ((current_mode == MODE.COMBAT_ROOM) ||
                    ((current_mode == MODE.COMBAT) && (Party._loc >= U4_Decompiled.LOCATIONS.DECEIT) && (Party._loc <= U4_Decompiled.LOCATIONS.THE_GREAT_STYGIAN_ABYSS)))
            {
                if (Party._dir == DIRECTION.NORTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_UP);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_UP);
#endif
                }
                else if (Party._dir == DIRECTION.EAST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_RIGHT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RIGHT);
#endif
                }
                else if (Party._dir == DIRECTION.SOUTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_DOWN);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_DOWN);
#endif
                }
                else if (Party._dir == DIRECTION.WEST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_LEFT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_LEFT);
#endif
                }
            }
            else if ((current_mode == MODE.OUTDOORS) || (current_mode == MODE.BUILDING))
            {
                if (surface_party_direction == DIRECTION.NORTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_UP);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_UP);
#endif
                }
                else if (surface_party_direction == DIRECTION.EAST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_RIGHT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RIGHT);
#endif
                }
                else if (surface_party_direction == DIRECTION.SOUTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_DOWN);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_DOWN);
#endif

                }
                else if (surface_party_direction == DIRECTION.WEST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_LEFT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_LEFT);
#endif
                }
            }
            else
            {
#if USE_UNITY_DLL_FUNCTION
                //main_keyboardHit((char)KEYS.VK_UP);
#else
                Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_UP);
#endif
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetAxis("Horizontal 2") < -0.99f && (resetJoystick2 < Time.time))
        {
            resetJoystick2 = Time.time + joystickResetTime;
            if ((current_mode == MODE.COMBAT_ROOM) ||
                    ((current_mode == MODE.COMBAT) && (Party._loc >= U4_Decompiled.LOCATIONS.DECEIT) && (Party._loc <= U4_Decompiled.LOCATIONS.THE_GREAT_STYGIAN_ABYSS)))
            {
                if (Party._dir == DIRECTION.NORTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_LEFT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_LEFT);
#endif
                }
                else if (Party._dir == DIRECTION.EAST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_UP);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_UP);
#endif
                }
                else if (Party._dir == DIRECTION.SOUTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_RIGHT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RIGHT);
#endif
                }
                else if (Party._dir == DIRECTION.WEST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_DOWN);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_DOWN);
#endif
                }
            }
            else if ((current_mode == MODE.OUTDOORS) || (current_mode == MODE.BUILDING))
            {
                if (surface_party_direction == DIRECTION.NORTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_LEFT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_LEFT);
#endif
                }
                else if (surface_party_direction == DIRECTION.EAST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_UP);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_UP);
#endif
                }
                else if (surface_party_direction == DIRECTION.SOUTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_RIGHT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RIGHT);
#endif
                }
                else if (surface_party_direction == DIRECTION.WEST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_DOWN);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_DOWN);
#endif
                }
            }
            else
            {
#if USE_UNITY_DLL_FUNCTION
                main_keyboardHit((char)KEYS.VK_LEFT);
#else
                Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_LEFT);
#endif
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetAxis("Horizontal 2") > 0.99f && (resetJoystick2 < Time.time))
        {
            resetJoystick2 = Time.time + joystickResetTime;
            if ((current_mode == MODE.COMBAT_ROOM) ||
                    ((current_mode == MODE.COMBAT) && (Party._loc >= U4_Decompiled.LOCATIONS.DECEIT) && (Party._loc <= U4_Decompiled.LOCATIONS.THE_GREAT_STYGIAN_ABYSS)))
            {
                if (Party._dir == DIRECTION.NORTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_RIGHT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RIGHT);
#endif
                }
                else if (Party._dir == DIRECTION.EAST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_DOWN);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_DOWN);
#endif
                }
                else if (Party._dir == DIRECTION.SOUTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_LEFT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_LEFT);
#endif
                }
                else if (Party._dir == DIRECTION.WEST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_UP);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_UP);
#endif
                }
            }
            else if ((current_mode == MODE.OUTDOORS) || (current_mode == MODE.BUILDING))
            {
                if (surface_party_direction == DIRECTION.NORTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_RIGHT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RIGHT);
#endif
                }
                else if (surface_party_direction == DIRECTION.EAST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_DOWN);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_DOWN);
#endif
                }
                else if (surface_party_direction == DIRECTION.SOUTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_LEFT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_LEFT);
#endif
                }
                else if (surface_party_direction == DIRECTION.WEST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_UP);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_UP);
#endif
                }
            }
            else
            {
#if USE_UNITY_DLL_FUNCTION
                //main_keyboardHit((char)KEYS.VK_RIGHT);
#else
                Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RIGHT);
#endif
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_ESCAPE);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_ESCAPE);
#endif
            Application.Quit();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_RETURN);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RETURN);
#endif
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            // TODO make this work with the text onscreen
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_BACK);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_BACK);
#endif
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_SPACE);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_SPACE);
#endif
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'A');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'A');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
#if USE_UNITY_DLL_FUNCTION
            //main_keyboardHit((char)'B');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'B');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'C');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'C');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'D');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'D');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'E');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'E');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'F');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'F');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'G');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'G');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'H');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'H');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'I');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'I');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'J');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'J');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'K');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'K');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'L');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'L');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'M');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'M');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'N');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'N');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'O');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'O');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'P');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'P');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'Q');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'Q');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'R');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'R');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'S');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'S');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'T');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'T');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'U');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'U');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'V');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'V');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'W');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'W');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'X');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'X');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'Y');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'Y');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'Z');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'Z');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'0');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'0');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'1');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'1');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'2');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'2');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'3');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'3');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'4');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'4');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'5');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'5');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'6');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'6');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'7');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'7');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'8');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'8');
#endif
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'9');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'9');
#endif
        }

        // check if we just finished playing a sound effect
        if (started_playing_sound_effect == true && specialEffectAudioSource.isPlaying == false)
        {
            // we just finished playing a sound effect
            started_playing_sound_effect = false;

            // let the game engine we finished playing the sound effect
#if USE_UNITY_DLL_FUNCTION
            main_sound_effect_done();
#else
            Native.Invoke<main_sound_effect_done>(nativeLibraryPtr);
#endif
        }

        // TODO move this out of the game engine interface
        // TODO programatically create all the sounds instead of relying on sampled sounds
        // TODO cache sounds already created
        // TODO pre-create basic sfx at startup
        // TODO get source information of sound to better position it for 3D sound, monster, moongate, player, etc. e.g. currently moongate plays at old moongate position not at new position

        // is sound enabled in the game engine
        if (SoundFlag == 1)
        {
            // are we currently already playing a sound effect
            if (started_playing_sound_effect == false)
            {
                // get if any sound effects are active from the game engine
#if USE_UNITY_DLL_FUNCTION
                    int sound = main_sound_effect();
                    int length = main_sound_effect_length();
#else
                int sound = Native.Invoke<int, main_sound_effect>(nativeLibraryPtr);
                int length = Native.Invoke<int, main_sound_effect_length>(nativeLibraryPtr);
#endif
                // see if the sound effect from the game engine is valid
                if (sound != -1)
                {
                    // create if nessesary the audio clip and play the sound effect
                    if (sound == (int)SOUND_EFFECT.SPELL_CAST)
                    {
                        AudioClip clip = CreateMagicSpecialEffectSound(length);
                        specialEffectAudioSource.PlayOneShot(clip);
                    }
                    else if (sound == (int)SOUND_EFFECT.SPELL_EFFECT)
                    {
                        AudioClip clip = CreateMagicEffectsSpecialEffectSound(length);
                        specialEffectAudioSource.PlayOneShot(clip);
                    }
                    else
                    {
                        specialEffectAudioSource.PlayOneShot(soundEffects[sound]);
                    }

                    started_playing_sound_effect = true;
                }
            }
        }
        // we still need to respond to sound calls even if sound is off otherwise the game engine will pend
        else
        {
#if USE_UNITY_DLL_FUNCTION
            int sound = main_sound_effect();
            int length = main_sound_effect_length();
#else
            int sound = Native.Invoke<int, main_sound_effect>(nativeLibraryPtr);
            int length = Native.Invoke<int, main_sound_effect_length>(nativeLibraryPtr);
#endif
            // immediately respond to the game engine as if the sound has been played for now
            // TODO need to take the same amount of time as the original sound before responding
            if (sound != -1)
            {
#if USE_UNITY_DLL_FUNCTION
                main_sound_effect_done();
#else
                Native.Invoke<main_sound_effect_done>(nativeLibraryPtr);
#endif
            }
        }

        // only get data from the game engine periodically
        if (timer > timerExpired)
        {
            // update the timer
            timer = timer - timerExpired;
            timerExpired = timerPeriod;

            // read the input mode
#if USE_UNITY_DLL_FUNCTION
            inputMode = main_input_mode();
#else
            inputMode = Native.Invoke<INPUT_MODE, main_input_mode>(nativeLibraryPtr);
#endif

            // read the sound flag
#if USE_UNITY_DLL_FUNCTION
            SoundFlag = main_SoundFlag();
#else
            SoundFlag = Native.Invoke<int, main_SoundFlag>(nativeLibraryPtr);
#endif

            AudioSource musicSource = Camera.main.GetComponent<AudioSource>();
            if (musicSource != null)
            {
                // is sound disabled in the game engine
                if (SoundFlag == 0)
                {
                    // are we playing music
                    if (musicSource.isPlaying == true)
                    {
                        // stop playing music when sound is disabled in the game engine and we are currently playing music
                        musicSource.Stop();
                    }
                }
                // is sound enabled in the game engine
                else
                {
                    // are we not playing music
                    if (musicSource.isPlaying == false)
                    {
                        // start playing music when sound is enabled in the game engine and we are currently not playing music
                        musicSource.Play();
                    }
                }

                // check if sound is enabled
                if (SoundFlag != 0)
                {
                    // check if we started talking to someone new
                    if (lastNPCTalkIndex != npcTalkIndex)
                    {
                        // check we just started talking to lord british
                        if (npcTalkIndex == TALK_INDEX.LORD_BRITISH)
                        {
                            // select the lord british music when in the castle
                            musicSource.clip = music[(int)MUSIC.RULEBRIT];
                        }
                        // check if we are talking to a vendor
                        else if ((npcTalkIndex == TALK_INDEX.VENDOR_ARMOR) ||
                            (npcTalkIndex == TALK_INDEX.VENDOR_FOOD) ||
                            (npcTalkIndex == TALK_INDEX.VENDOR_GUILD) ||
                            (npcTalkIndex == TALK_INDEX.VENDOR_HEALER) ||
                            (npcTalkIndex == TALK_INDEX.VENDOR_HORSE) ||
                            (npcTalkIndex == TALK_INDEX.VENDOR_INN) ||
                            (npcTalkIndex == TALK_INDEX.VENDOR_PUB) ||
                            (npcTalkIndex == TALK_INDEX.VENDOR_REAGENT) ||
                            (npcTalkIndex == TALK_INDEX.VENDOR_WEAPON) ||
                            (npcTalkIndex == TALK_INDEX.HAWKKWIND))
                        {
                            musicSource.clip = music[(int)MUSIC.SHOPPING];
                        }

                        // check if we just finished talking to lord british or a vendor
                        if ((lastNPCTalkIndex == TALK_INDEX.VENDOR_ARMOR) ||
                            (lastNPCTalkIndex == TALK_INDEX.VENDOR_FOOD) ||
                            (lastNPCTalkIndex == TALK_INDEX.VENDOR_GUILD) ||
                            (lastNPCTalkIndex == TALK_INDEX.VENDOR_HEALER) ||
                            (lastNPCTalkIndex == TALK_INDEX.VENDOR_HORSE) ||
                            (lastNPCTalkIndex == TALK_INDEX.VENDOR_INN) ||
                            (lastNPCTalkIndex == TALK_INDEX.VENDOR_PUB) ||
                            (lastNPCTalkIndex == TALK_INDEX.VENDOR_REAGENT) ||
                            (lastNPCTalkIndex == TALK_INDEX.VENDOR_WEAPON) ||
                            (lastNPCTalkIndex == TALK_INDEX.LORD_BRITISH) ||
                            (lastNPCTalkIndex == TALK_INDEX.HAWKKWIND))
                        {
                            // go back to the original town or castle music based on location
                            if ((Party._loc == LOCATIONS.BRITANNIA) ||
                                 (Party._loc == LOCATIONS.THE_LYCAEUM) ||
                                 (Party._loc == LOCATIONS.EMPATH_ABBY) ||
                                 (Party._loc == LOCATIONS.SERPENT_HOLD))
                            {
                                // select the castle music when in the castle
                                musicSource.clip = music[(int)MUSIC.CASTLES];
                            }
                            else
                            {
                                // select the town or village music when in a town or village
                                musicSource.clip = music[(int)MUSIC.TOWNS];
                            }
                        }

                        // update the last talk index
                        lastNPCTalkIndex = npcTalkIndex;
                    }

                    // check if the game engine game mode has changed and the game engine has sound enabled
                    if (lastMode != current_mode)
                    {
                        // update the last game mode to the current game mode
                        lastMode = current_mode;

                        // TODO add better cross fade between musics?
                        // TODO move this out of the game engine interface

                        // stop the currently playing music track
                        musicSource.Stop();

                        // check if we are outdoors
                        if (current_mode == U4_Decompiled.MODE.OUTDOORS)
                        {
                            // select the outdoor music clip
                            musicSource.clip = music[(int)MUSIC.WANDERER];
                        }
                        // check if we are in a building
                        else if (current_mode == U4_Decompiled.MODE.BUILDING)
                        {
                            // is the building a castle or town or village
                            if ((Party._loc == LOCATIONS.BRITANNIA) ||
                                (Party._loc == LOCATIONS.THE_LYCAEUM) ||
                                (Party._loc == LOCATIONS.EMPATH_ABBY) ||
                                (Party._loc == LOCATIONS.SERPENT_HOLD))
                            {
                                // select the castle music when in the castle
                                musicSource.clip = music[(int)MUSIC.CASTLES];
                            }
                            else
                            {
                                // select the town or village music when in a town or village
                                musicSource.clip = music[(int)MUSIC.TOWNS];
                            }
                        }
                        // check if we are in a dungoen
                        else if (current_mode == U4_Decompiled.MODE.DUNGEON)
                        {
                            // select the dungeon music
                            musicSource.clip = music[(int)MUSIC.DUNGEON];
                        }
                        // check if we are in combat
                        else if (current_mode == U4_Decompiled.MODE.COMBAT)
                        {
                            // select the combat music
                            musicSource.clip = music[(int)MUSIC.COMBAT];
                        }
                        // check if we are in camp
                        // TODO: the campe music seems a little off for the sleeping party unless
                        // there is an unexpected combat while sleeping
                        else if (current_mode == U4_Decompiled.MODE.COMBAT_CAMP)
                        {
                            // select the combat music
                            musicSource.clip = music[(int)MUSIC.COMBAT];
                        }
                        // check if we are in combat in a dungeon room
                        else if (current_mode == U4_Decompiled.MODE.COMBAT_ROOM)
                        {
                            // select the combat music
                            musicSource.clip = music[(int)MUSIC.COMBAT];
                        }
                        // check if we are in a shrine
                        else if (current_mode == U4_Decompiled.MODE.SHRINE)
                        {
                            // select the shrine music
                            musicSource.clip = music[(int)MUSIC.SHRINES];
                        }
                        // unknown game mode
                        else
                        {
                            // select no music
                            musicSource.clip = music[(int)MUSIC.FANFARE];
                        }

                        // start the music selection
                        musicSource.Play();
                    }
                }
            }

            // create an ASCII encoder if needed for text processing
            if (enc == null)
            {
                enc = new System.Text.ASCIIEncoding();
            }

            // read the circular text buffer from the game engine
#if USE_UNITY_DLL_FUNCTION
            int text_size = main_Text(buffer, buffer.Length);
#else
            int text_size = Native.Invoke<int, main_Text>(nativeLibraryPtr, buffer, buffer.Length);
#endif
            // check if we have any new text to add
            if (text_size != 0)
            {
                // remove the animated whirlpool from the text last character if we have some new text
                if (gameText.Length > 0)
                {
                    gameText = gameText.Remove(gameText.Length - 1);
                }

                string engineText = enc.GetString(buffer, 0, text_size);

                // add the ACSII encoded text to the display text plus read the whirlpool character
                gameText = gameText + engineText + (char)(0x1f - ((int)(Time.time * 3) % 4));

                // use regex to separate words from the game engine
                var matches = Regex.Matches(engineText, @"\w+[^\s]*\w+|\w");

                // add any new word to the word list
                foreach (Match match in matches)
                {
                    var word = match.Value;

                    if (wordList.Contains(word) == false)
                    {
                        wordList.Add(word);
                    }
                }

                // remove all but the last 20 lines of text from the text buffer
                int newline_count = 0;
                int i;
                const int MAX_NEWLINES = 20;
                for (i = gameText.Length - 1; (i > 0) && (newline_count < MAX_NEWLINES); i--)
                {
                    // check for a newline
                    if (gameText[i] == '\n')
                    {
                        // count the newlines
                        newline_count++;
                    }
                }

                // if we have enough lines cut the string 
                if (newline_count == MAX_NEWLINES)
                {
                    gameText = gameText.Substring(i + 2);
                }
            }

            // animate the spining whirlpool character by removing and adding to the end of the text, the update whirlpool character
            if (gameText.Length > 0)
            {
                gameText = gameText.Remove(gameText.Length - 1) + (char)(0x1f - ((int)(Time.time * 3) % 4));
            }
            else
            {
                gameText = "" + (char)(0x1f  - ((int)(Time.time * 3) % 4));
            }

            // read the circular npc text buffer from the game engine
            // this is different than the text above as it has speaking information
            // and inside the game engine it has been modified to remove first person references like "he says..."
#if USE_UNITY_DLL_FUNCTION
            text_size = main_NPC_Text(buffer, buffer.Length);
#else
            text_size = Native.Invoke<int, main_NPC_Text>(nativeLibraryPtr, buffer, buffer.Length);
#endif

            // check if we have any new npc text to add
            if (text_size != 0)
            {
                npcText = "";

                for (int i = 0; i < text_size; i++)
                {
                    npcTalkIndex = (TALK_INDEX)(buffer[i * 500]);

                    //int npcIndex = buffer[i * 500];
                    //partyText.text = partyText.text + npcIndex + " : " + /* Settlements[(int)Party._loc].GetComponent<Settlement>().npcStrings[_npc[npcIndex]._tlkidx - 1][0] + " says : " + */
                    string npcTalk = enc.GetString(buffer, i * 500 + 1, 500);

                    int firstNull = npcTalk.IndexOf('\0');
                    npcTalk = npcTalk.Substring(0, firstNull);
                    npcText = npcText + npcTalk;
                }

                // TODO move this out of the game engine monitor
                // TODO need to collect enough text til the newline so we don't have broken speech patterns in the middle of constructed sentences e.g. "I am" ... "a guard."...
                //WindowsVoice.speak(npcText); 
            }

            // attacker tile and tile under attacker (used to determine combat map to use when pirates attack)
#if USE_UNITY_DLL_FUNCTION
            D_96F8 = main_D_96F8();
#else
            D_96F8 = Native.Invoke<U4_Decompiled.TILE, main_D_96F8>(nativeLibraryPtr);
#endif

#if USE_UNITY_DLL_FUNCTION
            D_946C = main_D_946C();
#else
            D_946C = Native.Invoke<U4_Decompiled.TILE, main_D_946C>(nativeLibraryPtr);
#endif


            // 8x8? chunk tile location on main map
#if USE_UNITY_DLL_FUNCTION
            D_95A5.x = (byte)main_D_95A5_x();
#else
            D_95A5.x = (byte)Native.Invoke<int, main_D_95A5_x>(nativeLibraryPtr);
#endif

#if USE_UNITY_DLL_FUNCTION
            D_95A5.y = (byte)main_D_95A5_y();
#else
            D_95A5.y = (byte)Native.Invoke<int, main_D_95A5_y>(nativeLibraryPtr);
#endif


            // read current moongate information
#if USE_UNITY_DLL_FUNCTION
            moongate_tile = main_D_9141();
#else
            moongate_tile = Native.Invoke<U4_Decompiled.TILE, main_D_9141>(nativeLibraryPtr);
#endif

#if USE_UNITY_DLL_FUNCTION
            //moongate_x = main_D_9445();
#else
            moongate_x = Native.Invoke<int, main_D_9445>(nativeLibraryPtr);
#endif
#if USE_UNITY_DLL_FUNCTION
            moongate_y = main_D_9448();
#else
            moongate_y = Native.Invoke<int, main_D_9448>(nativeLibraryPtr);
#endif

            // get any hilighted character, used during combat
#if USE_UNITY_DLL_FUNCTION
            main_ActiveChar(buffer, buffer.Length);
#else
            Native.Invoke<main_ActiveChar>(nativeLibraryPtr, buffer, buffer.Length);
#endif

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
#if USE_UNITY_DLL_FUNCTION
            current_tile = main_tile_cur();
#else
            current_tile = Native.Invoke<U4_Decompiled.TILE, main_tile_cur>(nativeLibraryPtr);
#endif

            // read in current hit info list, the tile draws occurr out of the main draw sequence
            // and for only a short time before the playfield is repainted
            // the DLL saves the list of hits and coords to display later
#if USE_UNITY_DLL_FUNCTION
            main_Hit(buffer, buffer.Length);
#else
            Native.Invoke<main_Hit>(nativeLibraryPtr, buffer, buffer.Length);
#endif

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
#if USE_UNITY_DLL_FUNCTION
            main_CurMap(buffer, buffer.Length);
#else
            Native.Invoke<main_CurMap>(nativeLibraryPtr, buffer, buffer.Length);
#endif

            // get the current game mode;
#if USE_UNITY_DLL_FUNCTION
            current_mode = main_CurMode();
#else
            current_mode = Native.Invoke<U4_Decompiled.MODE, main_CurMode>(nativeLibraryPtr);
#endif

            // extract the map data depending on the game mode
            // in the game engine this memory is overlaped with the dungeon map below
            // but we can keep them separate here as we have pleanty of memory
            if ((current_mode == MODE.OUTDOORS) || (current_mode == MODE.BUILDING))
            {
                buffer_index = 0;
                for (int y = 0; y < 32; y++)
                {
                    for (int x = 0; x < 32; x++)
                    {
                        tMap32x32[x, y] = (TILE)buffer[buffer_index++];
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
#if USE_UNITY_DLL_FUNCTION
            main_Party(buffer, buffer.Length);
#else
            Native.Invoke<main_Party>(nativeLibraryPtr, buffer, buffer.Length);
#endif
            //System.IO.File.WriteAllBytes("Asset/partybuffer.bin", buffer);

            Party.f_000 = System.BitConverter.ToUInt32(buffer, 0x000);
            Party._moves = System.BitConverter.ToUInt32(buffer, 0x004);
            for (int i = 0; i < 8; i++)
            {
                buffer_index = 0x008 + i * 0x27;
                Party.chara[i].hitPoint = System.BitConverter.ToUInt16(buffer, buffer_index + 0x00);
                Party.chara[i].hitPointsMaximum = System.BitConverter.ToUInt16(buffer, buffer_index + 0x02);
                Party.chara[i].experiencePoints = System.BitConverter.ToUInt16(buffer, buffer_index + 0x04);
                Party.chara[i].strength = System.BitConverter.ToUInt16(buffer, buffer_index + 0x06);
                Party.chara[i].dexterity = System.BitConverter.ToUInt16(buffer, buffer_index + 0x08);
                Party.chara[i].intelligence = System.BitConverter.ToUInt16(buffer, buffer_index + 0x0a);
                Party.chara[i].magicPoints = System.BitConverter.ToUInt16(buffer, buffer_index + 0x0c);
                Party.chara[i].__0e[0] = buffer[buffer_index + 0xe];
                Party.chara[i].__0e[1] = buffer[buffer_index + 0xf];

                Party.chara[i].currentWeapon = (WEAPON)System.BitConverter.ToUInt16(buffer, buffer_index + 0x10);
                Party.chara[i].currentArmor = (ARMOR)System.BitConverter.ToUInt16(buffer, buffer_index + 0x12);
                Party.chara[i].name = "";
                for (int j = 0; j < 16; j++)
                {
                    char c = (char)buffer[buffer_index + 0x14 + j];
                    if (c != 0)
                    {
                        Party.chara[i].name += c;
                    }
                    else
                    {
                        break;
                    }
                }

                Party.chara[i].sex = (SEX)buffer[buffer_index + 0x24];
                Party.chara[i].Class = (CLASS)buffer[buffer_index + 0x25];
                Party.chara[i].state = (STATE)buffer[buffer_index + 0x26];
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
            Party._dir = (DIRECTION)System.BitConverter.ToUInt16(buffer, 0x1f0);
            Party._z = System.BitConverter.ToInt16(buffer, 0x1f2);
            Party._loc = (LOCATIONS)(System.BitConverter.ToUInt16(buffer, 0x1f4));

            // read in the Combat global
#if USE_UNITY_DLL_FUNCTION
            main_Combat(buffer, buffer.Length);
#else
            Native.Invoke<main_Combat>(nativeLibraryPtr, buffer, buffer.Length);
#endif

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
                    Combat_map[i, j] = buffer[buffer_index++];
                }
            }

            // read in the Fighters global
#if USE_UNITY_DLL_FUNCTION
            main_Fighters(buffer, buffer.Length);
#else
            Native.Invoke<main_Fighters>(nativeLibraryPtr, buffer, buffer.Length);
#endif

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
#if USE_UNITY_DLL_FUNCTION
            main_D_96F9(buffer, buffer.Length);
#else
            Native.Invoke<main_D_96F9>(nativeLibraryPtr, buffer, buffer.Length);
#endif

            // read the main display tile buffer
            buffer_index = 0;
            for (int y = 0; y < 11; y++)
            {
                for (int x = 0; x < 11; x++)
                {
                    displayTileMap[x, y] = (TILE)buffer[buffer_index++];
                }
            }

            // read the current open door information
#if USE_UNITY_DLL_FUNCTION
            open_door_x = main_D_17FA();
#else
            open_door_x = Native.Invoke<int, main_D_17FA>(nativeLibraryPtr);
#endif
#if USE_UNITY_DLL_FUNCTION
            open_door_y =  main_D_17FC();
#else
            open_door_y = Native.Invoke<int, main_D_17FC>(nativeLibraryPtr);
#endif
#if USE_UNITY_DLL_FUNCTION
            open_door_timer = main_D_17FE();
#else
            open_door_timer = Native.Invoke<int, main_D_17FE>(nativeLibraryPtr);
#endif

#if USE_UNITY_DLL_FUNCTION
            WindDir = main_WindDir();
#else
            WindDir = Native.Invoke<DIRECTION, main_WindDir>(nativeLibraryPtr);
#endif

#if USE_UNITY_DLL_FUNCTION
            spell_sta = main_spell_sta();
#else
            spell_sta = Native.Invoke<int, main_spell_sta>(nativeLibraryPtr);
#endif        

#if USE_UNITY_DLL_FUNCTION
            main_char_highlight(buffer, buffer.Length);
#else
            Native.Invoke<main_char_highlight>(nativeLibraryPtr, buffer, buffer.Length);
#endif        

            for (int i = 0; i < 8; i++)
            {
                Party.chara[i].highlight = (buffer[i] == 1);
            }

#if USE_UNITY_DLL_FUNCTION
            screen_xor_state = main_screen_xor_state();
#else
            screen_xor_state = Native.Invoke<int, main_screen_xor_state>(nativeLibraryPtr);
#endif   

            if (screen_xor_state == 1)
            {
                Camera.main.GetComponent<Effects>().EnableFx();
            }
            else
            {
                Camera.main.GetComponent<Effects>().DisableFx();
            }

#if USE_UNITY_DLL_FUNCTION
            camera_shake = main_camera_shake_accumulator();
#else
            camera_shake = Native.Invoke<int, main_camera_shake_accumulator>(nativeLibraryPtr);
#endif   

            if (camera_shake > 0)
            {
                Camera.main.GetComponent<ScreenShakeVR>().Shake((float)camera_shake / 8f, (float)camera_shake/8f);
            }


#if USE_UNITY_DLL_FUNCTION
            D_1665 = main_D_1665();
#else
            D_1665 = Native.Invoke<int, main_D_1665>(nativeLibraryPtr);
#endif   
#if USE_UNITY_DLL_FUNCTION
            D_1666 = main_D_1666();
#else
            D_1666 = Native.Invoke<int, main_D_1666>(nativeLibraryPtr);
#endif 
        }
    }


    public int open_door_x;
    public int open_door_y;
    public int open_door_timer;
    public int SoundFlag;
    public int screen_xor_state;
    public int camera_shake; 
    public int D_1665;
    public int D_1666;
}
