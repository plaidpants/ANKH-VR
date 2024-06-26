/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#ifndef __U4_H__
#define __U4_H__

#include <stdint.h>
#include <setjmp.h>

#ifdef WIN32
	#define far
	#pragma pack(1)
	typedef unsigned short U16;
	typedef signed short S16;
#else
	#define far
	typedef uint16_t U16;
	typedef int16_t S16;
#endif

#include "U4_SCAN.H"
#include "U4_SHAPE.H"

#define LORD_BRITISH 0xff
#define VENDOR_HAWKKWIND 0xfe
#define VENDOR_PUB 0xfd
#define VENDOR_REAGENT 0xfc
#define VENDOR_ARMOR 0xfb
#define VENDOR_WEAPON 0xfa
#define VENDOR_FOOD 0xf9
#define VENDOR_HORSE 0xf8
#define VENDOR_HEALER 0xf7
#define VENDOR_INN 0xf6
#define VENDOR_GUILD 0xf5
#define VENDOR_INVALID 0x7f

#define INPUT_MODE_CITIZEN_WORD 1
#define INPUT_MODE_LOAD_BRITISH_WORD 4
#define INPUT_MODE_HAWKWIND_WORD 6
#define INPUT_MODE_GENERAL_CONTINUE 9
#define INPUT_MODE_GENERAL_YES_NO 10
#define INPUT_MODE_GENERAL_YES_NO_WORD 39
#define INPUT_MODE_GENERAL_DIRECTION 11
#define INPUT_MODE_NUMBER_INPUT_2_DIGITS 15
#define INPUT_MODE_NUMBER_INPUT_3_DIGITS 16
#define INPUT_MODE_DUNGEON_INPUT 17
#define INPUT_MODE_MAIN_INPUT 18
#define INPUT_MODE_ENERGY_TYPE_POISON_FIRE_LIGHTNING_SLEEP 27
#define INPUT_MODE_GENERAL_BUY_SELL 29
#define INPUT_MODE_PUB_FOOD_OR_ALE 24
#define INPUT_MODE_GENERAL_ASK_LETTER_REAGENT 28
#define INPUT_MODE_GENERAL_ASK_LETTER_ARMOR 30
#define INPUT_MODE_GENERAL_ASK_LETTER_GUILD 31
#define INPUT_MODE_GENERAL_ASK_NUMBER_OF_BEDS 32
#define INPUT_MODE_GENERAL_ASK_LETTER_HEALER 33
#define INPUT_MODE_GENERAL_ASK_LETTER_WEAPON 34
#define INPUT_MODE_GENERAL_ASK_CHARACTER_NUMBER 35
#define INPUT_MODE_USE_WORD 36
#define INPUT_MODE_USE_STONE_COLOR_WORD 37
#define INPUT_MODE_VIRTUE_WORD 38
#define INPUT_MODE_MANTRA_WORD 40
#define INPUT_MODE_PUB_WORD 41
#define INPUT_MODE_COMBAT 44
#define INPUT_MODE_DRIVE_LETTER 45
#define INPUT_MODE_DELAY_CONTINUE 46
#define INPUT_MODE_DELAY_NO_CONTINUE 52
#define INPUT_MODE_GENERAL_ASK_SPELL 47
#define INPUT_MODE_NUMBER_INPUT_1_DIGITS 48
#define INPUT_MODE_GENERAL_ASK_PHASE 49
#define INPUT_MODE_TELESCOPE 50
#define INPUT_MODE_END_GAME_WORD 51
	
#define ZSTATS_MODE_CHARACTER_OVERVIEW 0
#define ZSTATS_MODE_CHARACTER_DETAIL 1
#define ZSTATS_MODE_WEAPONS 2
#define ZSTATS_MODE_ARMOUR 3
#define ZSTATS_MODE_EQUIPMENT 4
#define ZSTATS_MODE_ITEMS 5
#define ZSTATS_MODE_REAGENTS 6
#define ZSTATS_MODE_MIXTURES 7
#define ZSTATS_MODE_SPELLS 8

#define DIR_W 0
#define DIR_N 1
#define DIR_E 2
#define DIR_S 3
#define ST_USE_BELL   12
#define ST_USE_BOOK   11
#define ST_USE_CANDLE 10
#define ST_WHEEL       9
#define ST_HORN        8
#define ST_KEY_T       7
#define ST_KEY_L       6
#define ST_KEY_C       5
#define ST_BELL        4
#define ST_BOOK        3
#define ST_CANDLE      2
#define ST_CAST_SKULL  1
#define ST_SKULL       0

#define DNG_X_p(x, dir) (((x) + D_080C[(dir)]) & 7)
#define DNG_X_m(x, dir) (((x) - D_080C[(dir)]) & 7)
#define DNG_Y_p(y, dir) (((y) + D_0810[(dir)]) & 7)
#define DNG_Y_m(y, dir) (((y) - D_0810[(dir)]) & 7)

#define u4_lower(c) ((c) + 'a' - 'A')
#define u4_isupper(c) ((c) >= 'A' && (c) <= 'Z')
#define u4_islower(c) ((c) >= 'a' && (c) <= 'z')
#define u4_toupper(c) if(u4_islower(c)) (c) -= 'a' - 'A'
#define u4_toupper2(c) if(u4_islower(c)) (c) += 'A' - 'a'
#define u4_tolower(c) if(u4_isupper(c)) (c) += 'a' - 'A'

#define u4_max(a,b) ((a) >= (b)?(a):(b))
#define u4_min(a,b) ((a) >  (b)?(b):(a))

typedef unsigned char tMap32x32[32][32];
typedef unsigned char tMap8x8x8[8][8][8];

struct tNPC /*size:0x100*/ {
	/*_00*/unsigned char _gtile[32];
	/*_20*/unsigned char _x[32],_y[32];
	/*_60*/unsigned char _tile[32];
	/*_80*/unsigned char _old_x[32], _old_y[32];
	/*_c0*/unsigned char _var[32];/*_agressivity (or _z in dungeon)*/
	/*_e0*/unsigned char _tlkidx[32];
}
#ifndef _WINDOWS
__attribute__((packed))
#endif
;

/* *.ULT or *.DNG */
struct t_500 {
	/*DNGMAP.SAV*/
	union {
		tMap8x8x8 x8x8x8;
		tMap32x32 x32x32;
	} /*__8742.__000*/_map;
	/*MONSTERS.SAV or OUTMONST.SAV or .ULT(last 0x100 bytes)*/
	struct tNPC /*__8B42/__8742.__400*/_npc;
} 
#ifndef _WINDOWS
__attribute__((packed))
#endif
;

struct tChara /*size:0x27*/ {
	/*+00*/U16 _HP[2];
	/*+04*/U16 _XP;
	/*+06*/U16 _str;
	/*+08*/U16 _dex;
	/*+0a*/U16 _int;
	/*+0c*/U16 _MP;
	/*+0e*/char __0e[2];
	/*+10*/U16 _weapon;
	/*+12*/U16 _armor;
	/*+14*/char _name[16];
	/*+24*/char p_24;/*sex char*/
	/*+25*/char _class;
	/*+26*/char _status;
}
#ifndef _WINDOWS
__attribute__((packed))
#endif
;


struct tParty /*size:0x1f6*/ {
//	/*+000*/unsigned long f_000;/*a counter*/
	/*+000*/uint32_t f_000;/*a counter*/
	/* */
//	/*+004*/unsigned long _moves;
	/*+004*/uint32_t _moves;
	/*+008*/struct tChara chara[8];
//	/*+140*/long _food;
	/*+140*/int32_t _food;
	/*+144*/U16 _gold;
	/*karmas*/
	/*+146*/U16 _hones;
	/*+148*/U16 _compa;
	/*+14a*/U16 _valor;
	/*+14c*/U16 _justi;
	/*+14e*/U16 _sacri;
	/*+150*/U16 _honor;
	/*+152*/U16 _spiri;
	/*+154*/U16 _humil;
	/* */
	/*+156*/U16 _torches;
	/*+158*/U16 _gems;
	/*+15a*/U16 _keys;
	/*+15c*/U16 _sextants;
	/*+15e*/U16 _armors[8];
	/*+16e*/U16 _weapons[16];
	/*+18e*/U16 _reagents[8];
	/*+19e*/U16 _mixtures[26];
	/*+1d2*/U16 mItems;
	/*+1d4,+1d5*/unsigned char _x, _y;
	/*+1d6*/unsigned char mStones;
	/*+1d7*/unsigned char mRunes;
	/*+1d8*/U16 f_1d8;/*characters #*/
	/*+1da*/U16 _tile;
	/*+1dc*/U16 f_1dc;/*isFlying or light[dungeon]*/
	/*+1de/+1e0*/U16 _trammel,_felucca;/*moons phase*/
	/*+1e2*/U16 _ship;/*hull integrity*/
	/*+1e4*/U16 f_1e4;/*did met with LB*/
	/*+1e6*/U16 f_1e6;/*last hole up&camp*/
	/*+1e8*/U16 f_1e8;/*last found*/
	/*+1ea*/U16 f_1ea;/*last meditation/Hawkwind*/
	/*+1ec*/U16 f_1ec;/*last karma-conversation*/
	/*+1ee,+1ef*/unsigned char out_x, out_y;
	/*+1f0*/U16 _dir;/*[dungeon]*/
	/*+1f2*/S16 _z;/*[dungeon]*/
	/*+1f4*/U16 _loc;
}
#ifndef _WINDOWS
__attribute__((packed))
#endif
;


#define TST_MSK(mask, bit) ((mask) & (1 << (bit)))
#define SET_MSK(mask, bit) (mask) |= (1 << (bit))
#define RST_MSK(mask, bit) (mask) &= ~(1 << (bit))

/*CON file structure*/
struct tCombat/*size:0xc0*/ {
	char _npcX[16],_npcY[16];/*_000/_010 D_9470/D_9480*/
	unsigned char _charaX[8],_charaY[8];/*_20/_28 D_9490/D_9498*/
	char _030[16];/*padding?*/
	unsigned char _map[11*11];/*_040 D_94B0*/
	char _0b9[7];/*padding?*/
}
#ifndef _WINDOWS
__attribute__((packed))
#endif
;

// added protection to access outside the map, will use Combat._map[0,0] if outside map

#define Combat_MAP(a,b) (Combat._map[(a < 11 && b < 11) ? (a)*11+(b) : 0])

struct t_68 {
	/*000*/char _x[16],_y[16];
	/*020*/char _HP[16];
	/*030*/char _tile[16],_gtile[16];
	/*050*/char _sleeping[16];
	/*060*/char _chtile[8];
}
#ifndef _WINDOWS
__attribute__((packed))
#endif
;


/***************************************************************/
/* note: pascal functions restore the stack by themsevles      */
/* for C functions(cdecl), it is the caller's responsability   */
/***************************************************************/

/*u4_low*/
extern /*C_0012*/low_init();
extern /*C_0115*/low_clean();
extern /*C_0147*/int low_gra();
/*u4_use*/
extern /*C_07AE*/CMD_Use();

#include "U4_FILE.H"

#define cdecl

/*u4_util*/
extern /*C_095E*/shakefx();
extern /*C_097D*/XP_inc(int, int);
extern /*C_09B1*/HP_inc(int, int);
extern C_09D9(int);
extern /*C_09A3*/w_Pass();
extern /*C_09F8*/karma_inc(U16*, unsigned);
extern /*C_0A17*/karma_dec(U16*, unsigned);
extern C_0A58(int, unsigned char, unsigned char);
extern C_0A8B(unsigned char, unsigned char);
extern /*C_0AB1*/SafeModulo(int, int);
extern C_0ACF(int);
extern C_0AFE(unsigned char);
extern /*C_0B38*/u4_puts(char *);
extern /*C_0C03*/u4_putl(long, int, char);
extern /*C_0C9F*/u4_putc(int);
extern /*C_0CF7*/dspl_Stats();
extern /*C_0E0E*/dspl_Gold();
extern /*C_0E4E*/isCharaAlive(int);
extern /*C_0E82*/isCharaConscious(int);
extern C_10FD();
extern /*C_1135*/hitChara(unsigned, int);
extern /*C_1188*/w_What();
extern /*C_1199*/w_Cant_t();
extern /*C_11AA*/w_NotHere();
extern /*C_11BB*/w_Disabled();
extern /*C_11CC*/w_OnlyOnFoot();
extern /*C_11DD*/w_SlowProgress();
extern /*C_11EB*/w_NoneLeft();
extern /*C_11F9*/AskLetter(char *, char, char);
extern /*C_1287*/AskChara(char *);
extern /*C_12D6*/AskDir(char *, int *, int *);
extern /*C_138B*/food_dec(int);
extern /*C_13B6*/MP_recover();
extern /*C_1445*/u4_gets(register char *, unsigned);
extern char *C_1513(int);
extern C_1584();
extern /*C_162F*/AskY_N();
extern long /*C_169C*/AskInt(int);
/*u4_util2*/
extern void cdecl /*C_16CD*/u_delay(int, int);

extern unsigned char u_rand_a();
#define U4_RND1(a) (u_rand_a()&(a))
#define U4_RND3(a) (u_rand_a()%(a))
#define U4_RND4(a) (SafeModulo(u_rand_a(), (a)))

/*used by U4_ANIM.C only?*/
extern unsigned char u_rand_b();
#define U4_RND2(a) (u_rand_b()&(a))

extern int /*C_17C5*/u_kbhit();
extern int /*C_17F4*/u_kbread();
extern void /*C_1804*/u_kbflush();

extern unsigned char C_1814();
extern C_181D(unsigned);
extern C_182F(char *);
extern C_184F();
extern C_18A2();

extern int sound(unsigned char s, unsigned char length);

#include "U4_DOS.H"

#include "U4_GRAPH.H"

/*u4_map*/
extern C_26B6();
extern C_2747();
extern C_2839();
extern C_2891();
extern C_28E9();
extern C_2941();
extern /*C_29C3*/w_Blocked();
extern /*C_29DE*/w_DriftOnly();
extern C_29EF(unsigned char);
extern C_2999(unsigned char);
extern C_2A5A(unsigned int);
extern /*C_2B8C*/CMDDIR_Up();
extern /*C_2C8A*/CMDDIR_Down();
extern /*C_2DA9*/CMDDIR_Left();
extern /*C_2EC5*/CMDDIR_Right();
/*u4_end*/
extern C_31F4();
/**/
extern C_36C7();
extern C_3A80();
extern C_3C54();
extern /*C_3DC8*/t_callback();
extern C_3E94();
extern /*C_4018*/CMD_Enter();
extern /*C_41C0*/CMD_X_it();
extern /*C_4253*/CMD_Board();
extern /*C_42E8*/CMD_Yell();
extern C_431D();
extern /*C_4353*/CMD_Open();
extern /*C_43F8*/CMD_Jimmy();
extern /*C_4477*/CMD_Klimb();
extern /*C_44EE*/CMD_Descend();
extern C_4649();
extern C_4832();
extern C_48F8();
extern C_4987();
extern C_4BC7();
extern C_4C42();
extern /*C_4E45*/CMD_Ztats();
extern /*C_4FBE*/u4_abs(int);
extern /*C_4FD7*/u4_sign(int);
extern C_5834();
extern C_5851();
extern /*C_59D5*/COM_GetFighterId(unsigned char, unsigned char);
extern /*C_5A6B*/COM_main();
extern /*C_5DAB*/COM_DoDamage(int, int, unsigned char);
extern /*C_5EA2*/COM_CheckHitable(unsigned char, unsigned char);
extern /*C_628F*/CMD_Attack();
extern C_636D(int);
extern /*C_6E4A*/CMD_Cast();
extern /*C_6F29*/CMD_Quit();
extern C_6FF9(struct tChara* , struct tChara* );
extern C_722F(int);
extern C_7337();
extern /*C_7631*/CMD_Ready();
extern /*C_7525*/CMD_Ignite();
extern /*C_7034*/CMD_NewOrder();
extern /*C_70AD*/CMD_Volume();
extern /*C_72EC*/CMD_Get();
extern C_73B8();
extern /*C_73C9*/CMD_Fire();
extern /*C_755E*/CMD_Locate();
extern /*C_7732**/CMD_Wear();
extern C_7918();
extern C_7B89();
extern C_7BAF();
extern C_7BD5();
extern C_7BFD();
extern C_7C25(unsigned char);
extern C_7D50();
extern C_7D92();
extern C_7DBC();
extern C_7DFE(register unsigned);
extern C_7FD7();
extern C_7FFD();
extern C_837A();
extern /*C_84D2*/DNG_main();
extern /*C_8AB0*/CMD_HoleUp();
extern /*C_8C08*/CMD_Mix();
extern /*C_913A*/CMD_Search();
extern C_9209();
extern C_9414();
extern C_95AA();
extern C_9F7B();
extern /*C_A6F3*/CMD_Talk();
extern C_ADEF();
extern C_AE41();
extern C_B677();
extern C_B9B2();
extern C_B9EF(unsigned);
extern C_C403();
extern /*C_C41D*/CMD_Peer();
/*u4_init*/
extern C_C51C();
/* */
extern C_E59B();
extern C_E72C();

/*====---- initialized ----====*/

struct tPSP {
	unsigned char _00[0x80];
	unsigned char _80[0x80];
}
#ifndef _WINDOWS
__attribute__((packed))
#endif
;

extern struct tPSP far *far_psp;/*D_0078*/

extern unsigned /*D_0672*/cursor_rate;
extern char cdecl /*D_06A6*/SoundFlag;
extern unsigned D_07F8;
extern U16 /*D_07FA*/*pKarmas[];
extern int /*D_080A*/activeCharaX;
extern char * D_080C, * D_0810;
extern unsigned char * D_0814, * D_081C;/*moongates positions*/
extern char *D_0824[];
extern char * D_0844;
extern char * D_0864;
extern char *D_0884[];
extern char *D_0894[];
/*strings*/
extern char *D_1E98[];

extern unsigned char * D_23D2;
extern unsigned char * D_2450;
extern unsigned char * D_2460;
extern unsigned char * D_2468;

/*====---- uninitialized ----====*/
extern unsigned /*D_8728*/speed_info;
extern unsigned /*D_872B*/equip_flags;
extern void far *patch_tandy;/*D_872D*/
extern struct t_500 D_8742;

/*====---- c_common ----====*/
extern unsigned char D_9140;
extern unsigned char D_9141;
extern int D_9142;
extern struct tParty /*D_9144*/Party;
extern tMap32x32 far *D_933A[][8];
extern int D_943A;/* graphic adaptater type */
extern unsigned char /*D_943C*/tile_forward;
extern int D_943E;
extern unsigned D_9440;
extern unsigned char /*D_9442,D_9443*/ship_x, ship_y;
extern unsigned char /*D_9444*/tile_cur;
extern unsigned char D_9445;
extern unsigned D_9446;
extern unsigned char D_9448;
extern char D_944A[];
extern unsigned char D_9452;
extern void far /*D_9454*/*pShapes;
extern jmp_buf D_9458;

#define MOD_VISION   0
#define MOD_OUTDOORS 1
#define MOD_BUILDING 2
#define MOD_DUNGEON  3
#define MOD_COMBAT   4
#define MOD_COM_CAMP 5
#define MOD_COM_ROOM 6
#define MOD_SHRINE   7
extern unsigned /*D_946A*/CurMode;

struct aStruct {
	unsigned char x, y;
}
#ifndef _WINDOWS
__attribute__((packed))
#endif
;


extern unsigned char D_946C;
extern unsigned /*D_946E*/spell_cnt;
extern struct tCombat /*D_9470*/Combat;
extern struct t_68 /*D_9530*/Fighters;
//extern int /*D_9598*/File_DNG;
extern int /*D_959A*/activeChara;
extern struct aStruct D_959C;
extern unsigned char /*D_959E*/tile_east;
extern void far /*D_95A0*/*pCharset;
extern char /*D_95A4*/spell_sta;
extern struct aStruct D_95A5;
extern int /*D_95A8*/txt_X;
extern char /*D_95AA*/tile_north;
extern unsigned /*D_95AC*/activeCharaY;
extern unsigned /*D_95AE*/hit_tile;
extern unsigned char /*D_95B0,D_95B1*/hit_x, hit_y;
extern unsigned char D_95B2[];
extern unsigned char /*D_95C2*/tile_west;
extern unsigned char /*D_95C3*/tile_south;
extern int /*D_95C4*/txt_Y;
extern unsigned D_95C6;
extern int D_95C8;
extern unsigned char /*D_95CA*/tile_back;
extern int D_95CC;
extern unsigned char D_95CE[];/*current .tlk data*/
extern char D_96EE;/*room exit x[dungeon]*/
//extern int /*D_96F0*/File_MAP;
extern unsigned /*D_96F2*/WindDir;
extern char D_96F4;/*room exit y[dungeon]*/
//extern int /*D_96F6*/File_TLK;
extern unsigned char D_96F8;
extern unsigned char D_96F9[11*11];
extern unsigned char D_9772;

extern unsigned char AVATAR[];

#ifdef WIN32
	#pragma pack()
#endif

#endif
