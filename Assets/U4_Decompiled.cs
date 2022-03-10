using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Threading;



public class U4_Decompiled : MonoBehaviour
{
    private Thread trd;

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
    public static extern void main_start();
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


    public GameObject partyGameObject;

    float timer = 0.0f;
    float timerExpired = 0.0f;
    public float timerPeriod = 0.05f; // the game operates on a 300ms Sleep() so we want to update things at least twice a fast

    byte[] buffer = new byte[2000];

    public byte[,] tMap32x32 = new byte[32, 32];
    public byte[,,] tMap8x8x8 = new byte[8, 8, 8];

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
        public byte _gtile;
        /*_20*/
        public byte _x, _y;
        /*_60*/
        public byte _tile;
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
        public ushort _tile;
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
        public byte _tile, _gtile;
        /*050*/
        public byte _sleeping;
        /*060*/
        public byte _chtile;
    }

    public t_68[] Fighters = new t_68[16];

    // Separate thread to run the game, we could attempt to make the data gathering function thread safe but for now this will do
    private void ThreadTask()
    {
        // start the DLL
        main_start();
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

    // Update is called once per frame
    void Update()
    {
        int buffer_index;

        timer += Time.deltaTime;

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
                _npc[i]._gtile = buffer[buffer_index + 0x00];
                _npc[i]._x = buffer[buffer_index + 0x20];
                _npc[i]._y = buffer[buffer_index + 0x40];
                _npc[i]._tile = buffer[buffer_index + 0x60];
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
            Party._tile = System.BitConverter.ToUInt16(buffer, 0x1da);
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
                Fighters[i]._tile = buffer[0x30 + i];
                Fighters[i]._gtile = buffer[0x40 + i];
                Fighters[i]._sleeping = buffer[0x50 + i];
                Fighters[i]._chtile = buffer[0x60 + i];
            }

            // read in the main_D_96F9 global
            main_D_96F9(buffer, buffer.Length);

            // keep the part game object in sync with the game
            if (partyGameObject)
            {
                partyGameObject.transform.localPosition = new Vector3(Party._x, 255 - Party._y, 0);
            }

            World[] worldList = FindObjectsOfType<World>();

            if (worldList.Length != 0)
            {
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
            }
        }
    }
}
