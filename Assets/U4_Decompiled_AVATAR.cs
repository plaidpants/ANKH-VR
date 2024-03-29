﻿#define USE_UNITY_DLL_FUNCTION

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;
using Meta.WitAi.TTS.Utilities;
using Meta.WitAi.TTS;
using Meta.WitAi.TTS.Data;
using System.Linq;
using System.Text;
using UnityEngine.Networking; // Required for UnityWebRequest

public class U4_Decompiled_AVATAR : MonoBehaviour
{
    private Thread trd;
	public TTSSpeaker speaker;
    public AudioSource specialEffectAudioSource;
    public string gameText;
    public string npcText;
    public string visionFilename;
    public TALK_INDEX npcTalkIndex = TALK_INDEX.INVALID;
    public bool started_playing_sound_effect = false;
    [SerializeField]
    public List<string> wordList = new List<string>();
    public char lastKeyboardHit;
    public string[] maleVoiceNames;
    public string[] femaleVoiceNames;
    public string[] itVoiceNames;
    public string lordBritishVoiceName;
    public string hawkWindVoiceName;
    public string maleChildVoiceName; 
    public string femaleChildVoiceName;
    public string maleWizardVoiceName;
    public string femaleWizardVoiceName;
    public string malePirateVoiceName;
    public string femalePirateVoiceName;
    public string snakeVoiceName;

    public enum VOICE_CHARACTER_IDS { NONE, CHIPMUNK, MONSTER, ROBOT, DAEMON, ALIEN };
    public enum VOICE_ENVIRONMENT_IDS { NONE, REVERB, ROOM, PHONE, RADIO, PA, CATHEDRAL, HELMET };
    public enum VOICE_STYLE_IDS { NONE, DEFAULT, ENUNCIATED, FAST, HAPPY, PROJECTED, SAD, SLOW, SOFT, WHIPSPER };

    public VOICE_STYLE_IDS speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
    public VOICE_ENVIRONMENT_IDS speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
    public VOICE_CHARACTER_IDS speakerCharacter = VOICE_CHARACTER_IDS.NONE;

    private static readonly string[] YourInterestSubstituteResponses =
    {
        "I'm all ears",
        "I'm listening",
        "You have my attention",
        "Your interest",
        "Go on...",
        "Anything else",
        "Is that all",
        "Anything to add",
        "Speak thine mind",
        "Speak your piece",
        "What say you?"
    };
    public enum TALK_INDEX
    {
        LORD_BRITISH = 0xff,
        HAWKWIND = 0xfe,
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
        USE_ITEM_WORD = 36,
        USE_STONE_COLOR_WORD = 37,
        END_GAME_WORD = 51,
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
        // delay is active, can skip
        DELAY_CONTINUE = 46,
        // delay is active, no input
        DELAY_NO_CONTINUE = 52,
        // drive letter for PCs, not really useful here
        DRIVE_LETTER = 45,
    }

    public ZSTATS_MODE zstats_mode;
    public int zstats_character;
    
    public enum ZSTATS_MODE
    {
        CHARACTER_OVERVIEW = 0,
        CHARACTER_DETAIL = 1,
        WEAPONS = 2,
        ARMOUR = 3,
        EQUIPMENT = 4,
        ITEMS = 5,
        REAGENTS = 6,
        MIXTURES = 7
    }

    // tiles
    public Tile.TILE current_tile;

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
        SETTLEMENT = 2,
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
    public enum MIXTURES
    {
        AWAKEN,
        BLINK,
        CURE,
        DISPELL,
        ENERGY,
        FIREBALL,
        GATE,
        HEAL,
        ICEBALLS,
        JINX,
        KILL,
        LIGHT,
        MAGIC_MISSLE,
        NEGATE,
        OPEN,
        PROTECT,
        QUICKNESS,
        RESURECTION,
        SLEEP,
        TREMOR,
        UNDEAD,
        VIEW,
        WINDS,
        X_IT,
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

    [System.Flags]
    public enum STONES:byte
    {
        //NONE = 0x00,
        BLUE = 0x01,
        YELLOW = 0x02,
        RED = 0x04,
        GREEN = 0x08,
        ORANGE = 0x10,
        PURPLE = 0x20,
        WHITE = 0x40,
        BLACK = 0x80
    }

    [System.Flags]
    public enum RUNES:byte
    {
        //NONE = 0x00,
        HONESTY = 0x01,
        COMPASSION = 0x02,
        VALOR = 0x04,
        JUSTICE = 0x08,
        SACRIFICE = 0x10,
        HONOR = 0x20,
        SPIRITUALITY = 0x40,
        HUMILITY = 0x80,
    }
    [System.Flags]
    public enum ITEMS:ushort
    {
        //NONE = 0x0000,
        SKULL = 0x0001, // The Skull of Mondain the Wizard!
        SKULL_ABYSS = 0x0002, // You cast the Skull of Mondain into the Abyss!
        CANDLE = 0x0004, // The Candle of Love!
        BOOK = 0x0008, // The Book of Truth!
        BELL = 0x0010, // The Bell of Courage!
        COMPASSION_KEY = 0x0020,
        LOVE_KEY = 0x0040,
        TRUTH_KEY = 0x0080,
        HORN = 0x0100, // A Silver Horn!
        WHEEL = 0x0200, // The Wheel from the H.M.S. Cape!
        LIT_CANDLE = 0x0400, // As you light the Candle the Earth Trembles!
        READ_BOOK = 0x0800, // The words resonate with the ringing!
        RUNG_BELL = 0x1000 // The Bell rings on and on!
    }

#if !USE_UNITY_DLL_FUNCTION
    static System.IntPtr nativeLibraryPtr;

    delegate void main();
    delegate MODE main_CurMode();
    delegate Tile.TILE main_D_96F8();
    delegate Tile.TILE main_D_946C();
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
    delegate Tile.TILE main_tile_cur();
    delegate DIRECTION main_WindDir();
    delegate int main_spell_sta();
    delegate int main_Text(byte[] buffer, int length);
    delegate int main_D_9445(); // moongate x
    delegate int main_D_9448(); // moongate y
    delegate Tile.TILE main_D_9141(); // moongate tile
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
    delegate INPUT_MODE main_input_mode();
    delegate ZSTATS_MODE main_zstats_mode();
    delegate int main_zstats_character();
    delegate void main_set_dir(DIRECTION direction);
    delegate void main_GetVision(byte[] buffer, int length);
    delegate void main_Set_Combat(byte[] buffer, int length);
    delegate void main_Set_Fighters(byte[] buffer, int length);
#endif

    string wordSaveFilePath;

    public void LoadWordList()
    {
        if (File.Exists(wordSaveFilePath))
        {
            wordList = new List<string>(File.ReadAllLines(wordSaveFilePath));
            Debug.Log("Word list loaded from " + wordSaveFilePath);
        }
        else
        {
            Debug.LogWarning("Save file not found in " + wordSaveFilePath);
        }
    }

    public void SaveWordList()
    {
        File.WriteAllLines(wordSaveFilePath, wordList.ToArray());
        Debug.Log("Word list saved to " + wordSaveFilePath);
    }

    void Awake()
    {
        wordSaveFilePath = Application.persistentDataPath + "/u4/" + "WORDS.SAV";
        LoadWordList();

        //Debug.Log("Load songs");
        LoadSongs();

#if !USE_UNITY_DLL_FUNCTION
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


        //Debug.Log("Load AVATAR library");
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
    [DllImport("libAVATAR")]
    public static extern void main();
    [DllImport("libAVATAR")]
    public static extern MODE main_CurMode();
    [DllImport("libAVATAR")]
    public static extern Tile.TILE main_D_96F8();
    [DllImport("libAVATAR")]
    public static extern Tile.TILE main_D_946C();
    [DllImport("libAVATAR")]
    public static extern int main_D_95A5_x();
    [DllImport("libAVATAR")]
    public static extern int main_D_95A5_y();
    [DllImport("libAVATAR")]
    public static extern void main_keyboardHit(char key);
    [DllImport("libAVATAR")]
    public static extern void main_CurMap(byte[] buffer, int length);
    [DllImport("libAVATAR")]
    public static extern void main_Combat(byte[] buffer, int length);
    [DllImport("libAVATAR")]
    public static extern void main_Fighters(byte[] buffer, int length);
    [DllImport("libAVATAR")]
    public static extern void main_D_96F9(byte[] buffer, int length);
    [DllImport("libAVATAR")]
    public static extern void main_Party(byte[] buffer, int length);
    [DllImport("libAVATAR")]
    public static extern void main_Hit(byte[] buffer, int length);
    [DllImport("libAVATAR")]
    public static extern void main_ActiveChar(byte[] buffer, int length);
    [DllImport("libAVATAR")]
    public static extern Tile.TILE main_tile_cur();
    [DllImport("libAVATAR")]
    public static extern DIRECTION main_WindDir();
    [DllImport("libAVATAR")]
    public static extern int main_spell_sta();
    [DllImport("libAVATAR")]
    public static extern int main_Text(byte[] buffer, int length);
    [DllImport("libAVATAR")]
    public static extern int main_D_9445(); // moongate x
    [DllImport("libAVATAR")]
    public static extern int main_D_9448(); // moongate y
    [DllImport("libAVATAR")]
    public static extern Tile.TILE main_D_9141(); // moongate tile
    [DllImport("libAVATAR")]
    public static extern int main_NPC_Text(byte[] buffer, int length);
    [DllImport("libAVATAR")]
    public static extern int main_D_17FA();
    [DllImport("libAVATAR")]
    public static extern int main_D_17FC();
    [DllImport("libAVATAR")]
    public static extern int main_D_17FE();
    [DllImport("libAVATAR")]
    public static extern int main_SoundFlag();
    [DllImport("libAVATAR")]
    public static extern void main_SetDataPath(byte[] buffer, int length);
    [DllImport("libAVATAR")]
    public static extern void main_char_highlight(byte[] buffer, int length);
    [DllImport("libAVATAR")]   
    public static extern int main_sound_effect();
    [DllImport("libAVATAR")]   
    public static extern int main_sound_effect_length();
    [DllImport("libAVATAR")]   
    public static extern void main_sound_effect_done();
    [DllImport("libAVATAR")]   
    public static extern int main_screen_xor_state();  
    [DllImport("libAVATAR")]   
    public static extern int main_camera_shake_accumulator();  
    [DllImport("libAVATAR")]   
    public static extern int main_D_1665();  
    [DllImport("libAVATAR")]   
    public static extern int main_D_1666();  
    [DllImport("libAVATAR")]
    public static extern INPUT_MODE main_input_mode();
    [DllImport("libAVATAR")]
    public static extern ZSTATS_MODE main_zstats_mode();
    [DllImport("libAVATAR")]
    public static extern int main_zstats_character();
    [DllImport("libAVATAR")]
    public static extern void main_set_dir(DIRECTION direction);
    [DllImport("libAVATAR")]
    public static extern void main_GetVision(byte[] buffer, int length);
    [DllImport("libAVATAR")]
    public static extern void main_Set_Combat(byte[] buffer, int length);
    [DllImport("libAVATAR")]
    public static extern void main_Set_Fighters(byte[] buffer, int length);
#else
    // interface to the game engine
    [DllImport("AVATAR")]
    public static extern void main();
    [DllImport("AVATAR")]
    public static extern MODE main_CurMode();
    [DllImport("AVATAR")]
    public static extern Tile.TILE main_D_96F8();
    [DllImport("AVATAR")]
    public static extern Tile.TILE main_D_946C();
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
    public static extern Tile.TILE main_tile_cur();
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
    public static extern Tile.TILE  main_D_9141(); // moongate tile
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
    [DllImport("AVATAR")]
    public static extern INPUT_MODE main_input_mode();
    [DllImport("AVATAR")]
    public static extern ZSTATS_MODE main_zstats_mode();
    [DllImport("AVATAR")]
    public static extern int main_zstats_character();
    [DllImport("AVATAR")]
    public static extern void main_set_dir(DIRECTION direction);
    [DllImport("AVATAR")]
    public static extern void main_GetVision(byte[] buffer, int length);
    [DllImport("AVATAR")]
    public static extern void main_Set_Combat(byte[] buffer, int length);
    [DllImport("AVATAR")]
    public static extern void main_Set_Fighters(byte[] buffer, int length);
#endif
#endif

    float timer = 0.0f;
    float timerExpired = 0.0f;
    public float timerPeriod = 0.02f; // the game operates on a 300ms Sleep() so we want to update things faster than that

    // buffer used to read stuff from the game engine
    byte[] buffer = new byte[10000];

    // game engine map buffers
    public Tile.TILE[,] tMap32x32 = new Tile.TILE[32, 32];
    public Dungeon.DUNGEON_TILE[][,] tMap8x8x8 = new Dungeon.DUNGEON_TILE[8][,];

    // game engine game mode
    public MODE current_mode;

    // game engine active moongate infomation
    public Tile.TILE moongate_tile;
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
    public Tile.TILE [,] Combat_map = new Tile.TILE[11, 11]; //11 * 11 /*_040 D_94B0*/

    [System.Serializable]
    public struct tNPC /*size:0x100*/
    {
        /*_00*/
        public Tile.TILE _gtile;
        /*_20*/
        public byte _x, _y;
        /*_60*/
        public Tile.TILE _tile;
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
        public ITEMS mItems;
        /*+1d4,+1d5*/
        public byte _x, _y;
        /*+1d6*/
        public STONES mStones;
        /*+1d7*/
        public RUNES mRunes;
        /*+1d8*/
        public ushort f_1d8;/*characters #*/
        /*+1da*/
        public Tile.TILE _tile;
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
        public Tile.TILE _tile, _gtile;
        /*050*/
        public byte _sleeping;
        /*060*/
        public Tile.TILE _chtile;
    }

    public t_68[] Fighters = new t_68[16];

    public Tile.TILE[,] displayTileMap = new Tile.TILE[11, 11];
    
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

        for (int i = 0; i < 8; i++)
        {
            tMap8x8x8[i] = new Dungeon.DUNGEON_TILE[8, 8];
        }
    }

   public void StartThread()
   {
        // don't start it twice
        if (trd == null)
        {        
            // start a thread with the DLL main task
            trd = new Thread(new ThreadStart(this.ThreadTask));
            trd.IsBackground = true;
            trd.Start();
        }
   }


    public void CommandAttack()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('A');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'A');
#endif
        lastKeyboardHit = 'A';
    }

    public void CommandCharacter1()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('1');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'1');
#endif
        lastKeyboardHit = '1';
    }

    public void CommandCharacter2()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('2');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'2');
#endif
        lastKeyboardHit = '2';
    }
    public void CommandCharacter3()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('3');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'3');
#endif
        lastKeyboardHit = '3';
    }
    public void CommandCharacter4()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('4');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'4');
#endif
        lastKeyboardHit = '4';
    }
    public void CommandCharacter5()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('5');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'5');
#endif
        lastKeyboardHit = '5';
    }
    public void CommandCharacter6()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('6');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'6');
#endif
        lastKeyboardHit = '6';
    }
    public void CommandCharacter7()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('7');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'7');
#endif
        lastKeyboardHit = '7';
    }
    public void CommandCharacter8()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('8');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'8');
#endif
        lastKeyboardHit = '8';
    }
    public void CommandCharacter9()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('9');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'9');
#endif
        lastKeyboardHit = '9';
    }
    public void CommandBoard()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('B');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'B');
#endif
        lastKeyboardHit = 'B';
    }

    public void CommandCast()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('C');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'C');
#endif
        lastKeyboardHit = 'C';
    }

    public void CommandDecsend()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('D');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'D');
#endif
        lastKeyboardHit = 'D';
    }
    public void CommandEnter()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('E');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'E');
#endif
        lastKeyboardHit = 'E';
    }
    public void CommandBackspace()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit((char)KEYS.VK_BACK);
#else
        Native.Invoke<main_keyboardHit>(U4_Decompiled_TITLE.nativeLibraryPtr2, (char)KEYS.VK_BACK);
#endif
        lastKeyboardHit = (char)KEYS.VK_BACK;
    }

    public void CommandFire()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('F');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'F');
#endif
        lastKeyboardHit = 'F';
    }
    public void CommandGet()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('G');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'G');
#endif
        lastKeyboardHit = 'G';
    }
    public void CommandHoleUp()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('H');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'H');
#endif
        lastKeyboardHit = 'H';
    }
    public void CommandIgnight()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('I');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'I');
#endif
        lastKeyboardHit = 'I';
    }
    public void CommandJimmy()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('J');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'J');
#endif
        lastKeyboardHit = 'J';
    }

    public void CommandKlimb()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('K');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'K');
#endif
        lastKeyboardHit = 'K';
    }

    public void CommandLocate()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('L');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'L');
#endif
        lastKeyboardHit = 'L';
    }

    public void CommandMix()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('M');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'M');
#endif
        lastKeyboardHit = 'M';
    }

    public void CommandNewOrder()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('N');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'N');
#endif
        lastKeyboardHit = 'N';
    }
    public void CommandOpen()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('O');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'O');
#endif
        lastKeyboardHit = 'O';
    }
    public void CommandPeer()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('P');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'P');
#endif
        lastKeyboardHit = 'P';
    }

    public void CommandPass()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit(' ');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, ' ');
#endif
        lastKeyboardHit = ' ';
    }

    public void CommandQuit()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('Q');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'Q');
#endif
        lastKeyboardHit = 'Q';

        SaveWordList();
    }
    public void CommandReady()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('R');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'R');
#endif
        lastKeyboardHit = 'R';
    }
    public void CommandSearch()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('S');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'S');
#endif
        lastKeyboardHit = 'S';
    }
    public void CommandTalk()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('T');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'T');
#endif
        lastKeyboardHit = 'T';
    }

    public void CommandUse()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('U');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'U');
#endif
        lastKeyboardHit = 'U';
    }

    public void CommandVolume()
    {
        // if both sound and voice are on turn them both off
        if (SoundFlag == 1 && VoiceFlag == 1)
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit('V'); // this will turn off the soundFlag
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'V');
#endif
            lastKeyboardHit = 'V';

            // turn this off manually
            VoiceFlag = 0;
        }
        else if (SoundFlag == 0 && VoiceFlag == 0)
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit('V'); // this will turn on the soundFlag
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'V');
#endif
            lastKeyboardHit = 'V';

            // leave this off
            VoiceFlag = 0;
        }
        else if (SoundFlag == 1 && VoiceFlag == 0)
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit('V'); // this will turn off the soundFlag
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'V');
#endif
            lastKeyboardHit = 'V';

            // turn this on manually
            VoiceFlag = 1;
        }
        else if (SoundFlag == 0 && VoiceFlag == 1)
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit('V'); // this will turn on the soundFlag
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'V');
#endif
            lastKeyboardHit = 'V';

            // leave this on
            VoiceFlag = 1;
        }
    }

    public void CommandWear()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('W');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'W');
#endif
        lastKeyboardHit = 'W';
    }

    public void CommandXit()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('X');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'X');
#endif
        lastKeyboardHit = 'X';
    }

    public void CommandYell()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('Y');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'Y');
#endif
        lastKeyboardHit = 'Y';
    }

    public void CommandZStas()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('Z');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'Z');
#endif
        lastKeyboardHit = 'Z';
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
            lastKeyboardHit = upper[i];
            //yield on a new YieldInstruction that waits for 5 seconds.
            yield return new WaitForSeconds(0.15f);
        }

#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit((char)KEYS.VK_RETURN);
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RETURN);
#endif
        lastKeyboardHit = (char)KEYS.VK_RETURN;
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
        lastKeyboardHit = character;
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
        lastKeyboardHit = (char)KEYS.VK_RETURN;
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
        lastKeyboardHit = 'Y';
    }
    public void CommandSayFood()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('F');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'F');
#endif
        lastKeyboardHit = 'F';
    }

    public void CommandSayAle()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('A');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'A');
#endif
        lastKeyboardHit = 'A';
    }
    public void CommandSayBuy()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('B');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'B');
#endif
        lastKeyboardHit = 'B';
    }

    public void CommandSaySell()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('S');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'S');
#endif
        lastKeyboardHit = 'S';
    }

    public void CommandSayCuring()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('A');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'A');
#endif
        lastKeyboardHit = 'A';
    }

    public void CommandSayHealing()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('B');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'B');
#endif
        lastKeyboardHit = 'B';
    }
    public void CommandSayResurection()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('C');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'C');
#endif
        lastKeyboardHit = 'C';
    }
    public void CommandSayN()
    {
#if USE_UNITY_DLL_FUNCTION
        main_keyboardHit('N');
#else
        Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'N');
#endif
        lastKeyboardHit = 'N';
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

        lastKeyboardHit = (char)KEYS.VK_ESCAPE;

#if !USE_UNITY_DLL_FUNCTION
        // It is now safe to unload the DLL
        if (nativeLibraryPtr != System.IntPtr.Zero)
        {
#if UNITY_EDITOR
            // wait for the game engine thread to complete/return
            while (trd.IsAlive == true)
            {
                ;
            }
#endif

            // It is now safe to unload the DLL
            if (nativeLibraryPtr != System.IntPtr.Zero)
            {
                //Debug.Log("Unload AVATAR library");
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
#endif
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

    public float hit_time_period = 0.01f;
    public float last_hit_expire_time = 0.0f;

    public struct hit
    {
        public Tile.TILE tile;
        public byte x;
        public byte y;
        public float hit_expire_time;
    }

    public List<hit> currentHits = new List<hit>() { };
    public Tile.TILE D_96F8; // tile under attacker tile (used when pirate attack to determine combat map to use)
    public Tile.TILE D_946C; // attacker tile (used when pirate attack to determine combat map to use)

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
        FOOTSTEP = 0, // small pulse
        BLOCKED = 1, // one short low tone, 8 pulses 0.078s 102.564 fequency
        WHAT = 2, // two short tones (high then low) 23.5 pulses 0.084s 273.809 fequency, 8 pulses 0.078s 102.564 fequency
        CANNON = 3, // frequency decrease from 1000Hz to 200Hz over 0.27s, not linear frequency
        PLAYER_ATTACK = 4, // frequency increase from 250Hz to 500Hz over 0.2s, not linear frequency
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
            if (System.IO.File.Exists(Application.persistentDataPath + "/u4/" + ((MUSIC)i).ToString() + ".MP3"))
            {
                StartCoroutine(LoadSongCoroutine(Application.persistentDataPath + "/u4/" + ((MUSIC)i).ToString() + ".MP3", (MUSIC)i));
            }
            else if (System.IO.File.Exists(Application.persistentDataPath + "/u4/" + ((MUSIC)i).ToString() + ".OGG"))
            {
                StartCoroutine(LoadSongCoroutine(Application.persistentDataPath + "/u4/" + ((MUSIC)i).ToString() + ".OGG", (MUSIC)i));
            }
        }
    }

    IEnumerator LoadSongCoroutine(string path, MUSIC index)
{
    string url = string.Format("file://{0}", path);
    using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, DetermineAudioType(path)))
    {
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Not Loaded #" + (int)index + " " + url + ", Error: " + www.error);
        }
        else
        {
            try
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                music[(int)index] = clip;
                //Debug.Log("Loaded #" + (int)index + " " + url);
            }
            catch
            {
                Debug.Log("Not Loaded #" + (int)index + " " + url);
            }
        }
    }
}

// Helper method to determine the audio type based on file extension
private AudioType DetermineAudioType(string path)
{
    if (path.EndsWith(".mp3", System.StringComparison.OrdinalIgnoreCase))
    {
        return AudioType.MPEG;
    }
    else if (path.EndsWith(".ogg", System.StringComparison.OrdinalIgnoreCase))
    {
        return AudioType.OGGVORBIS;
    }
    // Add more formats as needed
    else
    {
        Debug.LogWarning("Unknown audio type for file: " + path);
        return AudioType.UNKNOWN;
    }
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
        int next = 0;

        // make some random frequencies and calc total samples based on those frequencies
        for (int i = 0; i < length; i++)
        {
            frequency = Random.Range(min, max);
            sampleCount += (float)cycles * sampleRate / frequency;
            fequencies[i] = frequency;
        }

        // allocate total clip size based on above
        data = new float[(int)sampleCount * 2];

        frequency = fequencies[next++];

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
                //frequency = Random.Range(min, max);
                frequency = fequencies[next++];
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

    /*
sfx_error2:
 ldy #$32
@delay:
 pha
 pla
 dex
 bne @delay
 bit hw_SPEAKER
 dey
 bne @delay
 rts
    */

    AudioClip CreateBlockedSound()
    {
        float sampleRate = 44100;
        int channels = 2;
        int count = 0;
        float[] data;
        float phase = 0;
        float sampleCount = 0f;

        float frequency = 104f;

        sampleCount += (float)8 * sampleRate / frequency;

        // allocate total clip size based on above
        data = new float[(int)sampleCount * 2];

        // create the samples
        for (int i = 0; i < data.Length; i += channels)
        {
            phase += 2 * Mathf.PI * frequency / sampleRate;

            // output on all available channels
            for (int j = 0; j < channels; j++)
            {
                // square wave
                data[i + j] = Mathf.Sin(phase) > 0 ? 0.5f : -0.5f;
            }

            // reset the phase so the numbers don't get too big
            if (phase >= 2 * Mathf.PI)
            {
                phase -= 2 * Mathf.PI;
                count++;
            }
        }

        // create the audio clip
        AudioClip soundEffect = AudioClip.Create("blocked", data.Length, 2, (int)sampleRate, false);

        // set the sample data
        soundEffect.SetData(data, 0);

        // return the audio clip
        return soundEffect;
    }

    /*
sfx_ship_fire:
	ldx #$00
	stx zp_sfx_freq
@delay:
	inx
	bne @delay
	bit hw_SPEAKER
	dec zp_sfx_freq
	ldx zp_sfx_freq
	bne @delay
	rts
*/

    /*
     * sample	frequency
        352	    2450
        826	    1392.631579
        2976.75	681.9587629
        5953.5	481.0909091
        11907	338.3631714
        17860.5	277.9411765
        23814	250

    ln(counts) vs ln(frequency) is a linear function so ln(frequency) = -0.5392*ln(sample) + 10.896, solve for frequency = 53960.1/sample^0.5392
    */

    AudioClip CreateCannonSound()
    {
        float sampleRate = 44100;
        int channels = 2;
        int count = 0;
        float frequency = 0;
        float[] data;
        float phase = 0;
        float sampleCount = 0f;
        float phaseDelta;

        sampleCount = 0.27f * sampleRate;

        // allocate total clip size based on above
        data = new float[(int)sampleCount * 2];

        // create the samples
        for (int i = 0; i < data.Length; i += channels)
        {
            frequency = 53960.1f / Mathf.Pow(i + 1 , 0.5392f); // i + 1 to avoid infinity at sample zero

            phaseDelta = 2 * Mathf.PI * frequency / sampleRate;

            phase += phaseDelta;

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
        }

        // create the audio clip
        AudioClip soundEffect = AudioClip.Create("cannon", data.Length, 2, (int)sampleRate, false);

        // set the sample data
        soundEffect.SetData(data, 0);

        // return the audio clip
        return soundEffect;
    }

    /*
sfx_error1:
ldy #$32
@delay:
nop
nop
dex
bne @delay
bit hw_SPEAKER
dey
bne @delay
jmp sfx_error2
*/
    AudioClip CreateWhatSound()
    {
        float sampleRate = 44100;
        int channels = 2;
        int count = 0;
        float frequency = 0;
        float[] frequencies = new float[2];
        float[] data;
        float phase = 0;
        float sampleCount = 0f;

        frequencies[0] = 278f;
        frequencies[1] = 104f;
        frequency = frequencies[0];

        sampleCount += (float)23 * sampleRate / frequencies[0];
        sampleCount += (float)8 * sampleRate / frequencies[1];

        // allocate total clip size based on above
        data = new float[(int)sampleCount * 2];

        // create the samples
        for (int i = 0; i < data.Length; i += channels)
        {
            phase += 2 * Mathf.PI * frequency / sampleRate;

            // output on all available channels
            for (int j = 0; j < channels; j++)
            {
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
            if (count > 23)
            {
                count = 0;
                frequency = frequencies[1];
            }
        }

        // create the audio clip
        AudioClip soundEffect = AudioClip.Create("what", data.Length, 2, (int)sampleRate, false);

        // set the sample data
        soundEffect.SetData(data, 0);

        // return the audio clip
        return soundEffect;
    }
    /*
     * ; VALUES for A when calling j_playsfx

sound_footstep = $00
sound_blocked = $01
sound_what = $02
sound_cannon = $03
sound_player_attack = $04
sound_foe_attack = $05
sound_damage = $06
sound_firewalk = $07
sound_alert = $08
sound_spell_effect = $09
sound_cast = $0a
sound_drown = $0b
sound_twister = $0c

    sfxtab:
	.addr sfx_walk-1
	.addr sfx_error2-1
	.addr sfx_error1-1
	.addr sfx_ship_fire-1
	.addr sfx_attack-1
	.addr sfx_unknown-1
	.addr sfx_player_hits-1
	.addr sfx_monster_hits-1
	.addr sfx_flee-1
	.addr sfx_magic2-1
	.addr sfx_magic1-1
	.addr sfx_whirlpool-1
	.addr sfx_storm-1
    */

    /*
sfx_walk:
	ldy #$06
@repeat:
	jsr rand
	and #$3f
	ora #$20
	tax
@delay:
	dex
	bne @delay
	bit hw_SPEAKER
	dey
	bne @repeat
	rts
    */
    AudioClip CreateFootStepSpecialEffectSound(float length, int pulse)
    {
        float sampleRate = 44100;
        float sampleCount;
        int channels = 2;
        float[] data;

        sampleCount = length * sampleRate;

        // allocate total clip size based on above one pulse
        data = new float[(int)sampleCount * channels];

        // create the samples
        for (int i = 0; i < data.Length; i += channels)
        {
            float value;
            // TODO add small ramp up and ramp down? 4 steps? random pusle width
            if ((i > (data.Length / 2) - (sampleCount / pulse / 2)) && (i < (data.Length / 2) + (sampleCount / pulse / 2)))
            {
                value = 0.5f;
            }
            else
            {
                value = -0.5f;
            }

            // output on all available channels
            for (int j = 0; j < channels; j++)
            {
                data[i + j] = value;
            }
        }

        // create the audio clip
        AudioClip soundEffect = AudioClip.Create("footstep", data.Length, 2, (int)sampleRate, false);

        // set the sample data
        soundEffect.SetData(data, 0);

        // return the audio clip
        return soundEffect;
    }

    /*
sfx_attack:
	lda #$ff
	tax
	tay
@delay:
	dex
	bne @delay
	bit hw_SPEAKER
	dey
	tya
	tax
	bmi @delay
	rts
    */
    AudioClip CreatePlayerAttackSound()
    {
        float sampleRate = 44100;
        int channels = 2;
        int count = 0;
        float frequency = 0;
        float[] data;
        float phase = 0;
        float sampleCount = 0f;
        float phaseDelta;

        sampleCount = 0.185f * sampleRate;

        // allocate total clip size based on above
        data = new float[(int)sampleCount * 2];

        // create the samples
        for (int i = 0; i < data.Length; i += channels)
        {
            frequency = 250f + 250f*((float)i / (float)data.Length);

            phaseDelta = 2 * Mathf.PI * frequency / sampleRate;

            phase += phaseDelta;

            // output on all available channels
            for (int j = 0; j < channels; j++)
            {
                // sine wave
                //data[i + j] = Mathf.Sin(phase);

                // square wave
                data[i + j] = Mathf.Sin(phase) > 0 ? 0.5f : -0.0f;
            }

            // reset the phase so the numbers don't get too big
            if (phase >= 2 * Mathf.PI)
            {
                phase -= 2 * Mathf.PI;
                count++;
            }
        }

        // create the audio clip
        AudioClip soundEffect = AudioClip.Create("player attack", data.Length, 2, (int)sampleRate, false);

        // set the sample data
        soundEffect.SetData(data, 0);

        // return the audio clip
        return soundEffect;
    }

    /*
sfx_unknown:
	lda #$80
	tax
	tay
@delay:
	dex
	bne @delay
	bit hw_SPEAKER
	iny
	tya
	tax
	bmi @delay
	rts
    */
    AudioClip CreateMonsterAttackSound()
    {
        float sampleRate = 44100;
        int channels = 2;
        int count = 0;
        float frequency = 0;
        float[] data;
        float phase = 0;
        float sampleCount = 0f;
        float phaseDelta;

        sampleCount = 0.3f * sampleRate;

        // allocate total clip size based on above
        data = new float[(int)sampleCount * 2];

        // create the samples
        for (int i = 0; i < data.Length; i += channels)
        {
            frequency = 315f - 150f * ((float)i / (float)data.Length);

            phaseDelta = 2 * Mathf.PI * frequency / sampleRate;

            phase += phaseDelta;

            // output on all available channels
            for (int j = 0; j < channels; j++)
            {
                // sine wave
                //data[i + j] = Mathf.Sin(phase);

                // square wave
                data[i + j] = Mathf.Sin(phase) > 0 ? 0.5f : -0.0f;
            }

            // reset the phase so the numbers don't get too big
            if (phase >= 2 * Mathf.PI)
            {
                phase -= 2 * Mathf.PI;
                count++;
            }
        }

        // create the audio clip
        AudioClip soundEffect = AudioClip.Create("monster attack", data.Length, 2, (int)sampleRate, false);

        // set the sample data
        soundEffect.SetData(data, 0);

        // return the audio clip
        return soundEffect;
    }

    /*
sfx_player_hits:
	ldy #$ff
@repeat:
	jsr rand
	and #$7f
	tax
@delay:
	dex
	bne @delay
	bit hw_SPEAKER
	dey
	bne @repeat
	rts
    */

    AudioClip CreatePlayerHitSound(float length)
    {
        float sampleRate = 44100;
        float max = 3675f;
        float min = 919f;
        int channels = 2;
        int cycles = 0;
        int count = 0;
        float frequency;
        float[] data;
        float phase = 0;
        float sampleCount;

        sampleCount = length * sampleRate;

        frequency = Random.Range(min, max);

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
        AudioClip soundEffect = AudioClip.Create("player hit", data.Length, 2, (int)sampleRate, false);

        // set the sample data
        soundEffect.SetData(data, 0);

        // return the audio clip
        return soundEffect;
    }

    /*
sfx_monster_hits:
	ldy #$ff
@repeat:
	jsr rand
	ora #$80
	tax
@delay:
	dex
	bne @delay
	bit hw_SPEAKER
	dey
	bne @repeat
	rts
    */

    AudioClip CreateMonsterHitSound(float length)
    {
        float sampleRate = 44100;
        float max = 760f;
        float min = 416f;
        int channels = 2;
        int cycles = 0;
        int count = 0;
        float frequency;
        float[] data;
        float phase = 0;
        float sampleCount;

        sampleCount = length * sampleRate;

        frequency = Random.Range(min, max);

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
        AudioClip soundEffect = AudioClip.Create("player hit", data.Length, 2, (int)sampleRate, false);

        // set the sample data
        soundEffect.SetData(data, 0);

        // return the audio clip
        return soundEffect;
    }

    /*
sfx_flee:
	ldx #$7f
	stx zp_sfx_freq
@delay:
	dex
	bne @delay
	bit hw_SPEAKER
	dec zp_sfx_freq
	ldx zp_sfx_freq
	bne @delay
	rts
    */

    /*
     * counts   frequncy
     * 179*2        474
     * 1374*2       639
     * 2070*2       832
     * 2987*2       1575
     * expontial best fit frequency = 398.3*e^(0.0002*counts)
     */
    AudioClip CreateFleeSound()
    {
        float sampleRate = 44100;
        int channels = 2;
        int count = 0;
        float frequency = 0;
        float[] data;
        float phase = 0;
        float sampleCount = 0f;
        float phaseDelta;

        sampleCount = 0.07f * sampleRate;

        // allocate total clip size based on above
        data = new float[(int)sampleCount * 2];

        // create the samples
        for (int i = 0; i < data.Length; i += channels)
        {
            frequency = 398.3f * Mathf.Exp(0.0002f * i);

            phaseDelta = 2 * Mathf.PI * frequency / sampleRate;

            phase += phaseDelta;

            // output on all available channels
            for (int j = 0; j < channels; j++)
            {
                // sine wave
                //data[i + j] = Mathf.Sin(phase);

                // square wave
                data[i + j] = Mathf.Sin(phase) > 0 ? 0.5f : -0.0f;
            }

            // reset the phase so the numbers don't get too big
            if (phase >= 2 * Mathf.PI)
            {
                phase -= 2 * Mathf.PI;
                count++;
            }
        }

        // create the audio clip
        AudioClip soundEffect = AudioClip.Create("player attack", data.Length, 2, (int)sampleRate, false);

        // set the sample data
        soundEffect.SetData(data, 0);

        // return the audio clip
        return soundEffect;
    }

    /*
    sfx_whirlpool:
	lda #$40
@1:
	ldy #$20
@2:
	tax
@3:
	pha
	pla
	dex
	bne @3
	bit hw_SPEAKER
	dey
	bne @2
	clc
	adc #$01
	cmp #$c0
	bcc @1
	rts
    */
    /*
     * count	freq
        0	    216
        175164	290
        350328	618
    */

    AudioClip CreateWhirlpoolSound()
    {
        float sampleRate = 44100;
        int channels = 2;
        int count = 0;
        float frequency = 0;
        float[] data;
        float phase = 0;
        float sampleCount = 0f;
        float phaseDelta;

        sampleCount = 3.97f * sampleRate;

        // allocate total clip size based on above
        data = new float[(int)sampleCount * 2];

        // create the samples
        for (int i = 0; i < data.Length; i += channels)
        {
            frequency = 4E-09f*i*i - 0.0026f* i + 618f;

            phaseDelta = 2 * Mathf.PI * frequency / sampleRate;

            phase += phaseDelta;

            // output on all available channels
            for (int j = 0; j < channels; j++)
            {
                // sine wave
                //data[i + j] = Mathf.Sin(phase);

                // square wave
                data[i + j] = Mathf.Sin(phase) > 0 ? 0.5f : -0.0f;
            }

            // reset the phase so the numbers don't get too big
            if (phase >= 2 * Mathf.PI)
            {
                phase -= 2 * Mathf.PI;
                count++;
            }
        }

        // create the audio clip
        AudioClip soundEffect = AudioClip.Create("player attack", data.Length, 2, (int)sampleRate, false);

        // set the sample data
        soundEffect.SetData(data, 0);

        // return the audio clip
        return soundEffect;
    }
    /*
sfx_storm:
	lda #$c0
@1:
	ldy #$20
@2:
	tax
@3:
	pha
	pla
	dex
	bne @3
	bit hw_SPEAKER
	dey
	bne @2
	sec
	sbc #$01
	cmp #$40
	bcs @1
	rts
    */

    AudioClip CreateTwisterSound()
    {
        float sampleRate = 44100;
        int channels = 2;
        int count = 0;
        float frequency = 0;
        float[] data;
        float phase = 0;
        float sampleCount = 0f;
        float phaseDelta;

        sampleCount = 3.97f * sampleRate;

        // allocate total clip size based on above
        data = new float[(int)sampleCount * 2];

        // create the samples
        for (int i = 0; i < data.Length; i += channels)
        {
            frequency = 4E-09f * i * i - 0.0003f * i + 216f;

            phaseDelta = 2 * Mathf.PI * frequency / sampleRate;

            phase += phaseDelta;

            // output on all available channels
            for (int j = 0; j < channels; j++)
            {
                // sine wave
                //data[i + j] = Mathf.Sin(phase);

                // square wave
                data[i + j] = Mathf.Sin(phase) > 0 ? 0.5f : -0.0f;
            }

            // reset the phase so the numbers don't get too big
            if (phase >= 2 * Mathf.PI)
            {
                phase -= 2 * Mathf.PI;
                count++;
            }
        }

        // create the audio clip
        AudioClip soundEffect = AudioClip.Create("player attack", data.Length, 2, (int)sampleRate, false);

        // set the sample data
        soundEffect.SetData(data, 0);

        // return the audio clip
        return soundEffect;
    }

    public void SetCombat()
    {
        // write the npc positions
        for (int i = 0; i < 16; i++)
        {
            buffer[0x00 + i] = Combat1[i]._npcX;
            buffer[0x10 + i] = Combat1[i]._npcY;
        }
        // write the character positions
        for (int i = 0; i < 8; i++)
        {
            buffer[0x20 + i] = Combat2[i]._charaX;
            buffer[0x28 + i] = Combat2[i]._charaY;
        }
        // write the combat map
        int buffer_index = 0x40;
        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                buffer[buffer_index++] = (byte)Combat_map[i, j];
            }
        }

        // write the Combat global
#if USE_UNITY_DLL_FUNCTION
            main_Set_Combat(buffer, buffer.Length);
#else
        Native.Invoke<main_Set_Combat>(nativeLibraryPtr, buffer, buffer.Length);
#endif

        // write the fighter data
        for (int i = 0; i < 16; i++)
        {
            buffer[0x00 + i] = Fighters[i]._x;
            buffer[0x10 + i] = Fighters[i]._y;
            buffer[0x20 + i] = Fighters[i]._HP;
            buffer[0x30 + i] = (byte)Fighters[i]._tile;
            buffer[0x40 + i] = (byte)Fighters[i]._gtile;
            buffer[0x50 + i] = Fighters[i]._sleeping;
            buffer[0x60 + i] = (byte)Fighters[i]._chtile;
        }

        // write the Fighters global
#if USE_UNITY_DLL_FUNCTION
        main_Set_Fighters(buffer, buffer.Length);
#else
        Native.Invoke<main_Set_Fighters>(nativeLibraryPtr, buffer, buffer.Length);
#endif
    }

    // used to detect game mode changes and change the music
    MODE lastModeForMusic = (MODE)(-1);
    // checked just after getting the mode from the game engine
    public MODE lastMode = (MODE)(-1);
    // used to detect when we should play the lord british music
    TALK_INDEX lastNPCTalkIndex = (TALK_INDEX)(-1);

    // extra surface rotation feature maintained outside of the game engine
    public U4_Decompiled_AVATAR.DIRECTION surface_party_direction = DIRECTION.NORTH;

    public float resetJoystick1 = 0f;
    public float resetJoystick2 = 0f;
    public float joystickResetTime = 0.25f;

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
            lastKeyboardHit = '9';
        }
        else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.S)) // need to check this first as it overrides the normal S keypress
        {

#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'9'); // currently the windows implementation of this engine does not support this
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'9');
#endif
            lastKeyboardHit = '9';
        }
        else if (Input.GetKeyDown(KeyCode.End))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_END);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_END);
#endif
            lastKeyboardHit = (char)KEYS.VK_END;
        }
        else if (Input.GetKeyDown(KeyCode.Home))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_HOME);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_HOME);
#endif
            lastKeyboardHit = (char)KEYS.VK_HOME;
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
            lastKeyboardHit = (char)KEYS.VK_RETURN;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || (Input.GetAxis("Vertical 2") > 0.99f && (resetJoystick2 < Time.time)) || Input.GetAxis("Vertical 1") > 0.99f && (resetJoystick1 < Time.time))
        {
            resetJoystick1 = Time.time + joystickResetTime;
            resetJoystick2 = Time.time + joystickResetTime;
            if ((current_mode == MODE.COMBAT_ROOM) ||
                    (((current_mode == MODE.COMBAT) || (current_mode == MODE.COMBAT_CAMP))
                    && (Party._loc >= U4_Decompiled_AVATAR.LOCATIONS.DECEIT) && (Party._loc <= U4_Decompiled_AVATAR.LOCATIONS.THE_GREAT_STYGIAN_ABYSS)))
            {
                if (Party._dir == DIRECTION.NORTH)
                {

#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_DOWN);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_DOWN);
#endif
                    lastKeyboardHit = (char)KEYS.VK_DOWN;
                }
                else if (Party._dir == DIRECTION.EAST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_LEFT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_LEFT);
#endif
                    lastKeyboardHit = (char)KEYS.VK_LEFT;
                }
                else if (Party._dir == DIRECTION.SOUTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_UP);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_UP);
#endif
                    lastKeyboardHit = (char)KEYS.VK_UP;
                }
                else if (Party._dir == DIRECTION.WEST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_RIGHT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RIGHT);
#endif
                    lastKeyboardHit = (char)KEYS.VK_RIGHT;
                }
            }
            else if ((current_mode == MODE.OUTDOORS) || (current_mode == MODE.SETTLEMENT) || (current_mode == MODE.COMBAT_CAMP) || (current_mode == MODE.COMBAT))
            {
                if (surface_party_direction == DIRECTION.NORTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_DOWN);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_DOWN);
#endif
                    lastKeyboardHit = (char)KEYS.VK_DOWN;
                }
                else if (surface_party_direction == DIRECTION.EAST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_LEFT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_LEFT);
#endif
                    lastKeyboardHit = (char)KEYS.VK_LEFT;
                }
                else if (surface_party_direction == DIRECTION.SOUTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_UP);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_UP);
#endif
                    lastKeyboardHit = (char)KEYS.VK_UP;
                }
                else if (surface_party_direction == DIRECTION.WEST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_RIGHT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RIGHT);
#endif
                    lastKeyboardHit = (char)KEYS.VK_RIGHT;
                }
            }
            else
            {
#if USE_UNITY_DLL_FUNCTION
                main_keyboardHit((char)KEYS.VK_DOWN);
#else
                Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_DOWN);
#endif
                lastKeyboardHit = (char)KEYS.VK_DOWN;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || (Input.GetAxis("Vertical 2") < -0.99f && (resetJoystick2 < Time.time)) || (Input.GetAxis("Vertical 1") < -0.99f && (resetJoystick1 < Time.time)))
        {
            resetJoystick1 = Time.time + joystickResetTime;
            resetJoystick2 = Time.time + joystickResetTime;
            if ((current_mode == MODE.COMBAT_ROOM) ||
                    (((current_mode == MODE.COMBAT) || (current_mode == MODE.COMBAT_CAMP))
                    && (Party._loc >= U4_Decompiled_AVATAR.LOCATIONS.DECEIT) && (Party._loc <= U4_Decompiled_AVATAR.LOCATIONS.THE_GREAT_STYGIAN_ABYSS)))
            {
                if (Party._dir == DIRECTION.NORTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_UP);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_UP);
#endif
                    lastKeyboardHit = (char)KEYS.VK_UP;
                }
                else if (Party._dir == DIRECTION.EAST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_RIGHT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RIGHT);
#endif
                    lastKeyboardHit = (char)KEYS.VK_RIGHT;
                }
                else if (Party._dir == DIRECTION.SOUTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_DOWN);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_DOWN);
#endif
                    lastKeyboardHit = (char)KEYS.VK_DOWN;
                }
                else if (Party._dir == DIRECTION.WEST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_LEFT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_LEFT);
#endif
                    lastKeyboardHit = (char)KEYS.VK_LEFT;
                }
            }
            else if ((current_mode == MODE.OUTDOORS) || (current_mode == MODE.SETTLEMENT) || (current_mode == MODE.COMBAT_CAMP) || (current_mode == MODE.COMBAT))
            {
                if (surface_party_direction == DIRECTION.NORTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_UP);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_UP);
#endif
                    lastKeyboardHit = (char)KEYS.VK_UP;
                }
                else if (surface_party_direction == DIRECTION.EAST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_RIGHT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RIGHT);
#endif
                    lastKeyboardHit = (char)KEYS.VK_RIGHT;
                }
                else if (surface_party_direction == DIRECTION.SOUTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_DOWN);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_DOWN);
#endif
                    lastKeyboardHit = (char)KEYS.VK_DOWN;
                }
                else if (surface_party_direction == DIRECTION.WEST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_LEFT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_LEFT);
#endif
                    lastKeyboardHit = (char)KEYS.VK_LEFT;
                }
            }
            else
            {
#if USE_UNITY_DLL_FUNCTION
                main_keyboardHit((char)KEYS.VK_UP);
#else
                Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_UP);
#endif
                lastKeyboardHit = (char)KEYS.VK_UP;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetAxis("Horizontal 2") < -0.99f && (resetJoystick2 < Time.time))
        {
            resetJoystick2 = Time.time + joystickResetTime;
            if ((current_mode == MODE.COMBAT_ROOM) ||
                    (((current_mode == MODE.COMBAT) || (current_mode == MODE.COMBAT_CAMP)) 
                    && (Party._loc >= U4_Decompiled_AVATAR.LOCATIONS.DECEIT) && (Party._loc <= U4_Decompiled_AVATAR.LOCATIONS.THE_GREAT_STYGIAN_ABYSS)))
            {
                if (Party._dir == DIRECTION.NORTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_LEFT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_LEFT);
#endif
                    lastKeyboardHit = (char)KEYS.VK_LEFT;
                }
                else if (Party._dir == DIRECTION.EAST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_UP);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_UP);
#endif
                    lastKeyboardHit = (char)KEYS.VK_UP;
                }
                else if (Party._dir == DIRECTION.SOUTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_RIGHT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RIGHT);
#endif
                    lastKeyboardHit = (char)KEYS.VK_RIGHT;
                }
                else if (Party._dir == DIRECTION.WEST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_DOWN);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_DOWN);
#endif
                    lastKeyboardHit = (char)KEYS.VK_DOWN;
                }
            }
            else if ((current_mode == MODE.OUTDOORS) || (current_mode == MODE.SETTLEMENT) || (current_mode == MODE.COMBAT_CAMP) || (current_mode == MODE.COMBAT))
            {
                if (surface_party_direction == DIRECTION.NORTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_LEFT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_LEFT);
#endif
                    lastKeyboardHit = (char)KEYS.VK_LEFT;
                }
                else if (surface_party_direction == DIRECTION.EAST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_UP);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_UP);
#endif
                    lastKeyboardHit = (char)KEYS.VK_UP;
                }
                else if (surface_party_direction == DIRECTION.SOUTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_RIGHT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RIGHT);
#endif
                    lastKeyboardHit = (char)KEYS.VK_RIGHT;
                }
                else if (surface_party_direction == DIRECTION.WEST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_DOWN);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_DOWN);
#endif
                    lastKeyboardHit = (char)KEYS.VK_DOWN;
                }
            }
            else if (current_mode == MODE.DUNGEON)
            {
                // ignore left and right in the dungeon
            }
            else
            {
#if USE_UNITY_DLL_FUNCTION
                main_keyboardHit((char)KEYS.VK_LEFT);
#else
                Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_LEFT);
#endif
                lastKeyboardHit = (char)KEYS.VK_LEFT;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetAxis("Horizontal 2") > 0.99f && (resetJoystick2 < Time.time))
        {
            resetJoystick2 = Time.time + joystickResetTime;
            if ((current_mode == MODE.COMBAT_ROOM) ||
                    (((current_mode == MODE.COMBAT) || (current_mode == MODE.COMBAT_CAMP)) 
                    && (Party._loc >= U4_Decompiled_AVATAR.LOCATIONS.DECEIT) && (Party._loc <= U4_Decompiled_AVATAR.LOCATIONS.THE_GREAT_STYGIAN_ABYSS)))
            {
                if (Party._dir == DIRECTION.NORTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_RIGHT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RIGHT);
#endif
                    lastKeyboardHit = (char)KEYS.VK_RIGHT;
                }
                else if (Party._dir == DIRECTION.EAST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_DOWN);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_DOWN);
#endif
                    lastKeyboardHit = (char)KEYS.VK_DOWN;
                }
                else if (Party._dir == DIRECTION.SOUTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_LEFT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_LEFT);
#endif
                    lastKeyboardHit = (char)KEYS.VK_LEFT;
                }
                else if (Party._dir == DIRECTION.WEST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_UP);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_UP);
#endif
                    lastKeyboardHit = (char)KEYS.VK_UP;
                }
            }
            else if ((current_mode == MODE.OUTDOORS) || (current_mode == MODE.SETTLEMENT) || (current_mode == MODE.COMBAT) || (current_mode == MODE.COMBAT_CAMP))
            {
                if (surface_party_direction == DIRECTION.NORTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_RIGHT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RIGHT);
#endif
                    lastKeyboardHit = (char)KEYS.VK_RIGHT;
                }
                else if (surface_party_direction == DIRECTION.EAST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_DOWN);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_DOWN);
#endif
                    lastKeyboardHit = (char)KEYS.VK_DOWN;
                }
                else if (surface_party_direction == DIRECTION.SOUTH)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_LEFT);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_LEFT);
#endif
                    lastKeyboardHit = (char)KEYS.VK_LEFT;
                }
                else if (surface_party_direction == DIRECTION.WEST)
                {
#if USE_UNITY_DLL_FUNCTION
                    main_keyboardHit((char)KEYS.VK_UP);
#else
                    Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_UP);
#endif
                    lastKeyboardHit = (char)KEYS.VK_UP;
                }
            }
            else if (current_mode == MODE.DUNGEON)
            {
                // ignore left and right in the dungeon
            }
            else
            {
#if USE_UNITY_DLL_FUNCTION
                //main_keyboardHit((char)KEYS.VK_RIGHT);
#else
                Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RIGHT);
#endif
                lastKeyboardHit = (char)KEYS.VK_RIGHT;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_ESCAPE);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_ESCAPE);
#endif
            lastKeyboardHit = (char)KEYS.VK_ESCAPE;
            Application.Quit();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_RETURN);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_RETURN);
#endif
            lastKeyboardHit = (char)KEYS.VK_RETURN;
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_BACK);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_BACK);
#endif
            lastKeyboardHit = (char)KEYS.VK_BACK;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)KEYS.VK_SPACE);
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)KEYS.VK_SPACE);
#endif
            lastKeyboardHit = (char)KEYS.VK_SPACE;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'A');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'A');
#endif
            lastKeyboardHit = 'A';
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
#if USE_UNITY_DLL_FUNCTION
            //main_keyboardHit((char)'B');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'B');
#endif
            lastKeyboardHit = 'B';
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'C');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'C');
#endif
            lastKeyboardHit = 'C';
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'D');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'D');
#endif
            lastKeyboardHit = 'D';
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'E');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'E');
#endif
            lastKeyboardHit = 'E';
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'F');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'F');
#endif
            lastKeyboardHit = 'F';
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'G');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'G');
#endif
            lastKeyboardHit = 'G';
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'H');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'H');
#endif
            lastKeyboardHit = 'H';
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'I');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'I');
#endif
            lastKeyboardHit = 'I';
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'J');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'J');
#endif
            lastKeyboardHit = 'J';
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'K');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'K');
#endif
            lastKeyboardHit = 'K';
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'L');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'L');
#endif
            lastKeyboardHit = 'L';
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'M');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'M');
#endif
            lastKeyboardHit = 'M';
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'N');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'N');
#endif
            lastKeyboardHit = 'N';
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'O');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'O');
#endif
            lastKeyboardHit = 'O';
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'P');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'P');
#endif
            lastKeyboardHit = 'P';
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'Q');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'Q');
#endif
            lastKeyboardHit = 'Q';
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'R');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'R');
#endif
            lastKeyboardHit = 'R';
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'S');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'S');
#endif
            lastKeyboardHit = 'S';
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'T');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'T');
#endif
            lastKeyboardHit = 'T';
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'U');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'U');
#endif
            lastKeyboardHit = 'U';
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'V');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'V');
#endif
            lastKeyboardHit = 'V';
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'W');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'W');
#endif
            lastKeyboardHit = 'W';
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'X');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'X');
#endif
            lastKeyboardHit = 'X';
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'Y');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'Y');
#endif
            lastKeyboardHit = 'Y';
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'Z');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'Z');
#endif
            lastKeyboardHit = 'Z';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'0');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'0');
#endif
            lastKeyboardHit = '0';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'1');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'1');
#endif
            lastKeyboardHit = '1';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'2');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'2');
#endif
            lastKeyboardHit = '2';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'3');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'3');
#endif
            lastKeyboardHit = '3';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'4');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'4');
#endif
            lastKeyboardHit = '4';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'5');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'5');
#endif
            lastKeyboardHit = '5';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'6');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'6');
#endif
            lastKeyboardHit = '6';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'7');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'7');
#endif
            lastKeyboardHit = '7';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'8');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'8');
#endif
            lastKeyboardHit = '8';
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
        {
#if USE_UNITY_DLL_FUNCTION
            main_keyboardHit((char)'9');
#else
            Native.Invoke<main_keyboardHit>(nativeLibraryPtr, (char)'9');
#endif
            lastKeyboardHit = '9';
        }

#if USE_UNITY_DLL_FUNCTION
        main_set_dir(surface_party_direction);
#else
        Native.Invoke<main_set_dir>(nativeLibraryPtr, surface_party_direction);
#endif

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
                    else if (sound == (int)SOUND_EFFECT.FOOTSTEP)
                    {
                        AudioClip clip = CreateFootStepSpecialEffectSound(0.005f, Random.Range(15,25));
                        specialEffectAudioSource.PlayOneShot(clip);
                    }
                    // TODO: cache the non=random/dynamic sounds below
                    else if (sound == (int)SOUND_EFFECT.BLOCKED)
                    {
                        AudioClip clip = CreateBlockedSound();
                        specialEffectAudioSource.PlayOneShot(clip);
                    }
                    else if (sound == (int)SOUND_EFFECT.WHAT)
                    {
                        AudioClip clip = CreateWhatSound();
                        specialEffectAudioSource.PlayOneShot(clip);
                    }
                    else if (sound == (int)SOUND_EFFECT.CANNON)
                    {
                        AudioClip clip = CreateCannonSound();
                        specialEffectAudioSource.PlayOneShot(clip);
                    }
                    else if (sound == (int)SOUND_EFFECT.PLAYER_ATTACK)
                    {
                        AudioClip clip = CreatePlayerAttackSound();
                        specialEffectAudioSource.PlayOneShot(clip);
                    }
                    else if (sound == (int)SOUND_EFFECT.FOE_ATTACK)
                    {
                        AudioClip clip = CreateMonsterAttackSound();
                        specialEffectAudioSource.PlayOneShot(clip);
                    }
                    else if (sound == (int)SOUND_EFFECT.FLEE)
                    {
                        AudioClip clip = CreateFleeSound();
                        specialEffectAudioSource.PlayOneShot(clip);
                    }
                    else if (sound == (int)SOUND_EFFECT.PLAYER_HITS)
                    {
                        AudioClip clip = CreatePlayerHitSound(0.094f);
                        specialEffectAudioSource.PlayOneShot(clip);
                    }
                    else if (sound == (int)SOUND_EFFECT.FOE_HITS)
                    {
                        AudioClip clip = CreateMonsterHitSound(0.233f);
                        specialEffectAudioSource.PlayOneShot(clip);
                    }
                    else if (sound == (int)SOUND_EFFECT.WHIRL_POOL)
                    {
                        AudioClip clip = CreateWhirlpoolSound();
                        specialEffectAudioSource.PlayOneShot(clip);
                    }
                    else if (sound == (int)SOUND_EFFECT.TWISTER)
                    {
                        AudioClip clip = CreateTwisterSound();
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
#if USE_UNITY_DLL_FUNCTION
            zstats_mode = main_zstats_mode();
#else
            zstats_mode = Native.Invoke<ZSTATS_MODE, main_zstats_mode>(nativeLibraryPtr);
#endif

#if USE_UNITY_DLL_FUNCTION
            zstats_character = main_zstats_character();
#else
            zstats_character = Native.Invoke<int, main_zstats_character>(nativeLibraryPtr);
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
                    if ((lastNPCTalkIndex != npcTalkIndex) || (musicSource.clip == null))
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
                            (npcTalkIndex == TALK_INDEX.HAWKWIND))
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
                            (lastNPCTalkIndex == TALK_INDEX.HAWKWIND))
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
                    if ((lastModeForMusic != current_mode) || (musicSource.clip == null))
                    {
                        // update the last game mode to the current game mode
                        lastModeForMusic = current_mode;

                        // TODO add better cross fade between musics?
                        // TODO move this out of the game engine interface

                        // stop the currently playing music track
                        musicSource.Stop();

                        // check if we are outdoors
                        if (current_mode == U4_Decompiled_AVATAR.MODE.OUTDOORS)
                        {
                            // select the outdoor music clip
                            musicSource.clip = music[(int)MUSIC.WANDERER];
                        }
                        // check if we are in a building
                        else if (current_mode == U4_Decompiled_AVATAR.MODE.SETTLEMENT)
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
                        else if (current_mode == U4_Decompiled_AVATAR.MODE.DUNGEON)
                        {
                            // select the dungeon music
                            musicSource.clip = music[(int)MUSIC.DUNGEON];
                        }
                        // check if we are in combat
                        else if (current_mode == U4_Decompiled_AVATAR.MODE.COMBAT)
                        {
                            // select the combat music
                            musicSource.clip = music[(int)MUSIC.COMBAT];
                        }
                        // check if we are in camp
                        // TODO: the camp music seems a little off for the sleeping party unless
                        // there is an unexpected combat while sleeping
                        else if (current_mode == U4_Decompiled_AVATAR.MODE.COMBAT_CAMP)
                        {
                            // select the combat music
                            musicSource.clip = music[(int)MUSIC.COMBAT];
                        }
                        // check if we are in combat in a dungeon room
                        else if (current_mode == U4_Decompiled_AVATAR.MODE.COMBAT_ROOM)
                        {
                            // select the combat music
                            musicSource.clip = music[(int)MUSIC.COMBAT];
                        }
                        // check if we are in a shrine
                        else if (current_mode == U4_Decompiled_AVATAR.MODE.SHRINE)
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

                int i;

                // remove any backspace characters
                for (i = 1; i < gameText.Length; i++)
                {
                    // check for a backspace
                    if (gameText[i] == (char)8)
                    {
                        gameText = gameText.Remove(i - 1, 2);
                    }
                }
                
                // special case this two word keyword
                if (gameText.Contains("Ho eyo", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    wordList.Add("Ho eyo");
                }

                // use regex to separate words from the game engine
                var matches = Regex.Matches(engineText, @"\w+[^\s]*\w+|\w");

                // add any new word to the word list
                foreach (Match match in matches)
                {
                    var word = match.Value;

                    // only add if not already in list and its at least a 2 letter word
                    if ((word.Length > 1) && (wordList.Contains(word) == false))
                    {
                        // we will add words to the end of the list, the keyword search will search all words and use the last one that
                        // matches so we can use this method to use the most recent keyword in on the button, it will also update as you
                        // talk to people.
                        wordList.Add(word);
                    }
                }

                // remove all but the last 20 lines of text from the text buffer
                int newline_count = 0;

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

            System.Array.Clear(buffer, 0, buffer.Length);

#if USE_UNITY_DLL_FUNCTION
            main_GetVision(buffer, buffer.Length);
#else
            Native.Invoke<main_GetVision>(nativeLibraryPtr, buffer, buffer.Length);
#endif
            int len;
            for (len = 0; len < 255; len++)
            {
                if (buffer[len] == 0)
                {
                    break;
                }
            }

            visionFilename = enc.GetString(buffer, 0, len);

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
                TALK_INDEX currentTalkIndex = TALK_INDEX.INVALID;

                for (int i = 0; i < text_size; i++)
                {
                    // check the current talk index, assign only if it is not invalid
                    npcTalkIndex = (TALK_INDEX)(buffer[i * 500]);
                    if (npcTalkIndex != TALK_INDEX.INVALID)
                    {
                        currentTalkIndex = npcTalkIndex;
                    }

                    //int npcIndex = buffer[i * 500];
                    //partyText.text = partyText.text + npcIndex + " : " + /* Settlements[(int)Party._loc].GetComponent<Settlement>().npcStrings[_npc[npcIndex]._tlkidx - 1][0] + " says : " + */
                    string npcTalk = enc.GetString(buffer, i * 500 + 1, 500);
                    Debug.Log(npcTalk);
                    int firstNull = npcTalk.IndexOf('\0');
                    npcTalk = npcTalk.Substring(0, firstNull);
                    npcText = npcText + npcTalk;
                }

                // Fix gp -> gold pieces
                npcText = npcText.Replace("gp", " gold pieces");
                npcText = npcText.Replace("g.p.", " gold pieces");
                npcText = npcText.Replace("pts", " points");
                npcText = npcText.Replace("He asks:", " "); // remove extra interogative

                // TODO move this out of the game engine monitor
                // TODO need to collect enough text til the newline or period so we don't have broken speech patterns in the middle of constructed sentences e.g. "I am" ... "a guard."...

                if (currentTalkIndex == TALK_INDEX.LORD_BRITISH)
                {
                    // lord british is male
                    speaker.VoiceID = lordBritishVoiceName;
                    speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                    speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                    speakerCharacter = VOICE_CHARACTER_IDS.NONE;
                }
                else if (currentTalkIndex == TALK_INDEX.HAWKWIND)
                {
                    // hawkwind is male
                    speaker.VoiceID = hawkWindVoiceName;
                    speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                    speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                    speakerCharacter = VOICE_CHARACTER_IDS.NONE;
                }
                else if (currentTalkIndex == TALK_INDEX.VENDOR_REAGENT)
                {
                    // all the reagent vendors are female
                    /* Margot, Sasha, Shiela, Shannon" */
                    speaker.VoiceID = femaleVoiceNames[Party._y % femaleVoiceNames.Length];
                    speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                    speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                    speakerCharacter = VOICE_CHARACTER_IDS.NONE;
                }
                else if (currentTalkIndex == TALK_INDEX.VENDOR_WEAPON)
                {
                    /* Winston, Willard, Peter, Jumar, Hook, Wendy */
                    if (Party._loc == LOCATIONS.VESPER)
                    {
                        // Wendy is female
                        speaker.VoiceID = femaleVoiceNames[Party._y % femaleVoiceNames.Length];
                    }
                    else
                    {
                        speaker.VoiceID = maleVoiceNames[Party._y % maleVoiceNames.Length];
                    }
                    speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                    speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                    speakerCharacter = VOICE_CHARACTER_IDS.NONE;
                }
                else if (currentTalkIndex == TALK_INDEX.VENDOR_ARMOR)
                {
                    /* Winston, Valiant, Jean, Pierre, Limpy are all male*/
                    speaker.VoiceID = maleVoiceNames[Party._y % maleVoiceNames.Length];
                    speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                    speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                    speakerCharacter = VOICE_CHARACTER_IDS.NONE;
                }
                else if (currentTalkIndex == TALK_INDEX.VENDOR_GUILD)
                {
                    /* One Eyed Willey, Long John Leary are male pirates */
                    speaker.VoiceID = malePirateVoiceName;
                    speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                    speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                    speakerCharacter = VOICE_CHARACTER_IDS.NONE;
                }
                else if (currentTalkIndex == TALK_INDEX.VENDOR_INN)
                {
                    /* Scatu, Jason, Smirk, Estro, Zajac, Tyrone, Tymus we are going to assume these are all male */
                    speaker.VoiceID = maleVoiceNames[Party._y % maleVoiceNames.Length];
                    speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                    speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                    speakerCharacter = VOICE_CHARACTER_IDS.NONE;
                }
                else if (currentTalkIndex == TALK_INDEX.VENDOR_HEALER)
                {
                    /* Pendragon, Starfire, Salle', Windwalker, Harmony, Celest, Triplet, Justin, Spiran, Quat we are going to assume these are all female */
                    speaker.VoiceID = femaleVoiceNames[Party._y % femaleVoiceNames.Length];
                    speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                    speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                    speakerCharacter = VOICE_CHARACTER_IDS.NONE;
                }
                else if (currentTalkIndex == TALK_INDEX.VENDOR_PUB)
                {
                    /* Sam, Celestial, Terran, Greg 'n Rob, The Cap'n, Arron we are going to assume these are all male */
                    if (Party._loc == LOCATIONS.BUCCANEERS_DEN)
                    {
                        speaker.VoiceID = malePirateVoiceName;
                    }
                    else
                    {
                        speaker.VoiceID = maleVoiceNames[Party._y % maleVoiceNames.Length];
                    }
                    speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                    speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                    speakerCharacter = VOICE_CHARACTER_IDS.NONE;
                }
                else if (currentTalkIndex == TALK_INDEX.VENDOR_FOOD)
                {
                    /* Shaman, Windrick, Donnar, Mintol, Max we are going to assume these are all male */
                    speaker.VoiceID = maleVoiceNames[Party._y % maleVoiceNames.Length];
                    speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                    speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                    speakerCharacter = VOICE_CHARACTER_IDS.NONE;
                }
                else if (currentTalkIndex == TALK_INDEX.VENDOR_HORSE)
                {
                    /* Shaman, Windrick, Donnar, Mintol, Max we are going to assume these are all male */
                    speaker.VoiceID = maleVoiceNames[Party._y % maleVoiceNames.Length];
                    speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                    speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                    speakerCharacter = VOICE_CHARACTER_IDS.NONE;
                }
                else
                {

                    Settlement.SETTLEMENT settlement;

                    // get the current settlement, need to special case BRITANNIA as the castle has two levels, use the ladder to determine which level
                    if ((Party._loc == U4_Decompiled_AVATAR.LOCATIONS.BRITANNIA) && (tMap32x32[3, 3] == Tile.TILE.LADDER_UP))
                    {
                        settlement = Settlement.SETTLEMENT.LCB_1;
                    }
                    else
                    {
                        settlement = (Settlement.SETTLEMENT)Party._loc;
                    }

                    // who are we talking to in the settlement
                    string name = Settlement.settlementNPCs[(int)settlement][(int)currentTalkIndex].strings[(int)Settlement.NPC_STRING_INDEX.NAME];
                    // are they he or she or it or "the child"
                    string pronoun = Settlement.settlementNPCs[(int)settlement][(int)currentTalkIndex].strings[(int)Settlement.NPC_STRING_INDEX.PRONOUN];
                    // get a description of the character
                    string description = Settlement.settlementNPCs[(int)settlement][(int)currentTalkIndex].strings[(int)Settlement.NPC_STRING_INDEX.LOOK_DESCRIPTION];

                    // add jesters and ghosts?
                    if (name.Contains("nate", System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        speaker.VoiceID = snakeVoiceName;
                        speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                        speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                        speakerCharacter = VOICE_CHARACTER_IDS.NONE;
                    }
                    else if(description.Contains("jester", System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (pronoun.Contains("it", System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            speaker.VoiceID = itVoiceNames[(int)currentTalkIndex % itVoiceNames.Length];
                        }
                        else if (pronoun.Contains("she", System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            speaker.VoiceID = femaleVoiceNames[(int)currentTalkIndex % femaleVoiceNames.Length];
                        }
                        else // he
                        {
                            speaker.VoiceID = maleVoiceNames[(int)currentTalkIndex % maleVoiceNames.Length];
                        }
                        speakerVoiceStyle = VOICE_STYLE_IDS.HAPPY;
                        speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                        speakerCharacter = VOICE_CHARACTER_IDS.NONE;
                    }
                    else if (description.Contains("ghost", System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (pronoun.Contains("it", System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            speaker.VoiceID = itVoiceNames[(int)currentTalkIndex % itVoiceNames.Length];
                        }
                        else if (pronoun.Contains("she", System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            speaker.VoiceID = femaleVoiceNames[(int)currentTalkIndex % femaleVoiceNames.Length];
                        }
                        else // he
                        {
                            speaker.VoiceID = maleVoiceNames[(int)currentTalkIndex % maleVoiceNames.Length];
                        }
                        speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                        speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                        speakerCharacter = VOICE_CHARACTER_IDS.DAEMON;
                    }
                    else if (description.Contains("skeleton", System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (pronoun.Contains("it", System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            speaker.VoiceID = itVoiceNames[(int)currentTalkIndex % itVoiceNames.Length];
                        }
                        else if (pronoun.Contains("she", System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            speaker.VoiceID = femaleVoiceNames[(int)currentTalkIndex % femaleVoiceNames.Length];
                        }
                        else // he
                        {
                            speaker.VoiceID = maleVoiceNames[(int)currentTalkIndex % maleVoiceNames.Length];
                        }
                        speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                        speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                        speakerCharacter = VOICE_CHARACTER_IDS.MONSTER;
                    }
                    else if (description.Contains("child", System.StringComparison.CurrentCultureIgnoreCase) ||
                        description.Contains("girl", System.StringComparison.CurrentCultureIgnoreCase) ||
                        description.Contains("boy", System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (pronoun.Contains("she", System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            speaker.VoiceID = femaleChildVoiceName;
                        }
                        else // he or "the child"
                        {
                            speaker.VoiceID = maleChildVoiceName;
                        }
                        speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                        speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                        speakerCharacter = VOICE_CHARACTER_IDS.NONE;
                    }
                    else if (description.Contains("wizard", System.StringComparison.CurrentCultureIgnoreCase) || description.Contains("mage", System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (pronoun.Contains("she", System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            speaker.VoiceID = femaleWizardVoiceName;
                        }
                        else // he
                        {
                            speaker.VoiceID = maleWizardVoiceName;
                        }
                        speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                        speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                        speakerCharacter = VOICE_CHARACTER_IDS.NONE;
                    }
                    else if (description.Contains("pirate", System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (pronoun.Contains("she", System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            speaker.VoiceID = femalePirateVoiceName;
                        }
                        else // he
                        {
                            speaker.VoiceID = malePirateVoiceName;
                        }
                        speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                        speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                        speakerCharacter = VOICE_CHARACTER_IDS.NONE;
                    }
                    else
                    {
                        if (pronoun.Contains("it", System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            speaker.VoiceID = itVoiceNames[(int)currentTalkIndex % itVoiceNames.Length];
                            speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                            speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                            speakerCharacter = VOICE_CHARACTER_IDS.ALIEN;
                        }
                        else if (pronoun.Contains("she", System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            speaker.VoiceID = femaleVoiceNames[(int)currentTalkIndex % femaleVoiceNames.Length];
                            speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                            speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                            speakerCharacter = VOICE_CHARACTER_IDS.NONE;
                        }
                        else // he
                        {
                            speaker.VoiceID = maleVoiceNames[(int)currentTalkIndex % maleVoiceNames.Length];
                            speakerVoiceStyle = VOICE_STYLE_IDS.NONE;
                            speakerEnviorment = VOICE_ENVIRONMENT_IDS.NONE;
                            speakerCharacter = VOICE_CHARACTER_IDS.NONE;
                        }
                    }
                }

                // Get all voice name presets
                //string [] allVoiceNames = speaker.TTSService.GetAllPresetVoiceSettings()
                //    .Select((voiceSetting) => voiceSetting.SettingsId).ToArray();
                //speaker.VoiceID = allVoiceNames[(int)currentTalkIndex % allVoiceNames.Length];

                // Split the text into sentences. web api has a limit of 280 characters which can be reached pretty quickly as space and other special chars are expandaed to 
/* example
curl ^
More?   -H "Authorization: Bearer XXXXXXXXXXXXXXXXXXXXXX" ^
More?   "https://api.wit.ai/message?v=20240210&q=Even%20though%20the%20Great%20Evil%20Lords%20have%20been%20routed%20evil%20yet%20remains%20in%20Britannia.%20If%20but%20one%20soul%20could%20complete%20the%20Quest%20of%20the%20Avatar%2C%20our%20people%20would%20have%20a%20new%20hope%2C%20a%20new%20goal%20for%20life.%20There%20would%20be%20a%20shining%20example%20that%20there%20is%20more%20to%20life%20than%20the%20endless%20struggle%20for%20possessions%20and%20gold%21"
{
  "code": "msg-invalid",
  "error": "Message is too long: length is 303 (max is 280)"
}*/

                string[] sentences = npcText.Split(new char[] { '.', '!', '?' });
                foreach (string sentence in sentences)
                {
                    // Clean up the text before speaking it
                    string adjusted = sentence.Replace('\n', ' ');
                    adjusted = adjusted.Replace('\r', ' ');
                    // fix the pronounciation of Thee and tsetse
                    adjusted = adjusted.Replace("thee", "<phoneme ph=\"ði\" alphabet=\"ipa\">thee</phoneme>");
                    adjusted = adjusted.Replace("tsetse", "<phoneme ph=\"tsɛt si\" alphabet=\"ipa\">tsetse</phoneme>");

                    // add all the mantra's here as none seem to be pronounced properly
                    /* ahm,mu,ra,beh,cah,summ,om,lum */
                    adjusted = adjusted.Replace("ahm", "<phoneme ph=\"ɑːm\" alphabet=\"ipa\">ahm</phoneme>");
                    adjusted = adjusted.Replace("beh", "<phoneme ph=\"bɛ\" alphabet=\"ipa\">beh</phoneme>");
                    adjusted = adjusted.Replace("cah", "<phoneme ph=\"kɑː\" alphabet=\"ipa\">cah</phoneme>");
                    adjusted = adjusted.Replace("lum", "<phoneme ph=\"lʌm\" alphabet=\"ipa\">lum</phoneme>");
                    // these are not unique strings so we must only replace them if they are a single word.
                    string pattern;
                    pattern = $@"\b{Regex.Escape("om")}\b";
                    adjusted = Regex.Replace(adjusted, pattern, "<phoneme ph=\"oʊm\" alphabet=\"ipa\">om</phoneme>", RegexOptions.IgnoreCase);
                    pattern = $@"\b{Regex.Escape("ra")}\b";
                    adjusted = Regex.Replace(adjusted, pattern, "<phoneme ph=\"rɑː\" alphabet=\"ipa\">ra</phoneme>", RegexOptions.IgnoreCase);
                    pattern = $@"\b{Regex.Escape("mu")}\b";
                    adjusted = Regex.Replace(adjusted, pattern, "<phoneme ph=\"muː\" alphabet=\"ipa\">mu</phoneme>", RegexOptions.IgnoreCase);
                    pattern = $@"\b{Regex.Escape("summ")}\b";
                    adjusted = Regex.Replace(adjusted, pattern, "<phoneme ph=\"sʌm\" alphabet=\"ipa\">summ</phoneme>", RegexOptions.IgnoreCase);

                    // fix abbreviations for longitude and latitude
                    adjusted = adjusted.Replace("lat-", "latitude ");
                    adjusted = adjusted.Replace("long-", "longitude ");

                    // fix name pronoucing
                    adjusted = adjusted.Replace("Calumny", "<phoneme ph=\"kæləmni\" alphabet=\"ipa\">Calumny</phoneme>");

                    //fix pronouncing
                    adjusted = adjusted.Replace("Avatarhood", "<phoneme ph=\"ˈæv.ə.tɑːr.hʊd\" alphabet=\"ipa\">Avatarhood</phoneme>");
                    adjusted = adjusted.Replace("avatarhood", "<phoneme ph=\"ˈæv.ə.tɑːr.hʊd\" alphabet=\"ipa\">avatarhood</phoneme>");

                    // fix conflict with html <>
                    adjusted = adjusted.Replace(">must<", "<emphasis level=\"strong\">must</emphasis>");
                    adjusted = adjusted.Replace(">often<", "<emphasis level=\"strong\">often</emphasis>");

                    // change up the response text a bit
                    //adjusted = adjusted.Replace("Your Interest", YourInterestSubstituteResponses[Random.Range(0, YourInterestSubstituteResponses.Length)]);

                    // Add ssml tags to the front and back, this is always needed to support inline modifications such as <phoneme> above
                    string prepend = "<speak>";
                    string append = "</speak>";

                    // generate prepend and append text for speach character enviorment and styles
                    if ((speakerCharacter != VOICE_CHARACTER_IDS.NONE) ||
                        (speakerEnviorment != VOICE_ENVIRONMENT_IDS.NONE))
                    {
                        // Add prefix & postfix
                        prepend = prepend + "<sfx";
                        append = "</sfx>" + append;

                        // Add character
                        if (speakerCharacter != VOICE_CHARACTER_IDS.NONE)
                        {
                            prepend = prepend + " character=\"" + speakerCharacter + "\"";
                        }
                        // Add environment
                        if (speakerEnviorment != VOICE_ENVIRONMENT_IDS.NONE)
                        {
                            prepend = prepend + " environment=\"" + speakerEnviorment + "\"";
                        }

                        // Finalize
                        prepend = prepend + ">";
                    }

                    // add voice style
                    if (speakerVoiceStyle != VOICE_STYLE_IDS.NONE)
                    {
                        // Add ssml tags
                        prepend = prepend + "<voice";
                        append = "</voice>" + append;

                        // Add voice style
                        prepend = prepend + " style=\"" + speakerVoiceStyle + "\"";

                        // Finalize
                        prepend = prepend + ">";
                    }

                    // add the extra stuff
                    speaker.PrependedText = prepend;
                    speaker.AppendedText = append;

                    Debug.Log(prepend);
                    Debug.Log(adjusted);
                    Debug.Log(append);

                    // Speak the text
                    if (VoiceFlag == 1)
                    {
                        speaker.SpeakQueued(adjusted);
                    }
                }
            }

            // attacker tile and tile under attacker (used to determine combat map to use when pirates attack)
#if USE_UNITY_DLL_FUNCTION
            D_96F8 = main_D_96F8();
#else
            D_96F8 = Native.Invoke<Tile.TILE, main_D_96F8>(nativeLibraryPtr);
#endif

#if USE_UNITY_DLL_FUNCTION
            D_946C = main_D_946C();
#else
            D_946C = Native.Invoke<Tile.TILE, main_D_946C>(nativeLibraryPtr);
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
            moongate_tile = Native.Invoke<Tile.TILE, main_D_9141>(nativeLibraryPtr);
#endif

#if USE_UNITY_DLL_FUNCTION
            moongate_x = main_D_9445();
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
            current_tile = Native.Invoke<Tile.TILE, main_tile_cur>(nativeLibraryPtr);
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

                addHit.tile = (Tile.TILE)buffer[1 + i * 3];

                if (addHit.tile != 0)
                {
                    if (current_mode == MODE.OUTDOORS)
                    {
                        addHit.x = (byte)(buffer[2 + i * 3] + Party._x - 5);
                        addHit.y = (byte)(buffer[3 + i * 3] + Party._y - 5);
                    }
                    else
                    {
                        addHit.x = (byte)(buffer[2 + i * 3]);
                        addHit.y = (byte)(buffer[3 + i * 3]);
                    }
                    addHit.hit_expire_time = Time.time + hit_time_period + hit_time_period;
                    
                    // make sure this expiration time is greater than the last one by at least the hit time period
                    if (addHit.hit_expire_time < last_hit_expire_time + hit_time_period)
                    {
                        addHit.hit_expire_time = last_hit_expire_time + hit_time_period;
                    }

                    // remember the last hit expiration time
                    last_hit_expire_time = addHit.hit_expire_time;

                    currentHits.Add(addHit);
                }
            }

            // remove any hits with expired timers
            for (int i = 0; i < currentHits.Count; i++)
            {
                hit checkHit = currentHits[i];

                // we will leave this hit up until we get another or the timer expires
                if (checkHit.hit_expire_time < Time.time)
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
            current_mode = Native.Invoke<U4_Decompiled_AVATAR.MODE, main_CurMode>(nativeLibraryPtr);
#endif

            if (current_mode != lastMode)
            {
                lastMode = current_mode;
            }
            // extract the map data depending on the game mode
            // in the game engine this memory is overlaped with the dungeon map below
            // but we can keep them separate here as we have pleanty of memory
            if ((current_mode == MODE.OUTDOORS) || (current_mode == MODE.SETTLEMENT))
            {
                buffer_index = 0;
                for (int y = 0; y < 32; y++)
                {
                    for (int x = 0; x < 32; x++)
                    {
                        tMap32x32[x, y] = (Tile.TILE)buffer[buffer_index++];
                    }
                }
            }
            else if (current_mode == MODE.DUNGEON)
            {
                buffer_index = 0;
                for (int z = 0; z < 8; z++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            tMap8x8x8[z][x, 7 - y] = (Dungeon.DUNGEON_TILE)buffer[buffer_index++];
                        }
                    }
                }
            }

            // extract the npc data
            buffer_index = 1024;
            for (int i = 0; i < 32; i++)
            {
                _npc[i]._gtile = (Tile.TILE)buffer[buffer_index + 0x00];
                _npc[i]._x = buffer[buffer_index + 0x20];
                _npc[i]._y = buffer[buffer_index + 0x40];
                _npc[i]._tile = (Tile.TILE)buffer[buffer_index + 0x60];
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

            Party.mItems = (ITEMS)System.BitConverter.ToUInt16(buffer, 0x1d2);
            Party._x = buffer[0x1d4];
            Party._y = buffer[0x1d5];
            Party.mStones = (STONES)buffer[0x1d6];
            Party.mRunes = (RUNES)buffer[0x1d7];
            Party.f_1d8 = System.BitConverter.ToUInt16(buffer, 0x1d8); // number in party
            Party._tile = (Tile.TILE)System.BitConverter.ToUInt16(buffer, 0x1da);
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
                    Combat_map[i, j] = (Tile.TILE)buffer[buffer_index++];
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
                Fighters[i]._tile = (Tile.TILE)buffer[0x30 + i];
                Fighters[i]._gtile = (Tile.TILE)buffer[0x40 + i];
                Fighters[i]._sleeping = buffer[0x50 + i];
                Fighters[i]._chtile = (Tile.TILE)buffer[0x60 + i];
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
                    displayTileMap[x, y] = (Tile.TILE)buffer[buffer_index++];
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
                Camera.main.GetComponent<ScreenInvertVR>().enabled = true ;
            }
            else
            {
                Camera.main.GetComponent<ScreenInvertVR>().enabled = false;
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
    public int VoiceFlag = 1;
    public int screen_xor_state;
    public int camera_shake; 
    public int D_1665;
    public int D_1666;
}
