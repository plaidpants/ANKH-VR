#ifndef __TITLE_H__
#define __TITLE_H__


#define INPUT_MODE_M_OR_F 1
#define INPUT_MODE_NAME 2
#define INPUT_MODE_A_OR_B_CHOICE 3
#define INPUT_MODE_MAIN_MENU 4
#define INPUT_MODE_GENERAL_CONTINUE 5
#define INPUT_MODE_GENERAL_TEXT_CONTINUE 9
#define INPUT_MODE_GENERAL_NO_CONTINUE 7
#define INPUT_MODE_DRIVE_LETTER 6
#define INPUT_MODE_LAUNCH_GAME 8

extern void set_input_mode(int mode);

extern char AVATAR[];

#ifdef WIN32
	#define far
	#pragma pack(1)
	typedef unsigned short U16;
	typedef signed short S16;
#else
	#define far
	#define cdecl
//	#define Sleep usleep

	typedef unsigned short U16;
	typedef signed short S16;
#endif

#include "U4_SCAN.H"
#include <stdint.h>

#define u4_lower(c) ((c) + 'a' - 'A')
#define u4_isupper(c) ((c) >= 'A' && (c) <= 'Z')
#define u4_islower(c) ((c) >= 'a' && (c) <= 'z')
#define u4_toupper(c) if(u4_islower(c)) (c) -= 'a' - 'A'
#define u4_toupper2(c) if(u4_islower(c)) (c) += 'A' - 'a'
#define u4_tolower(c) if(u4_isupper(c)) (c) += 'a' - 'A'

extern unsigned D_0036;
extern unsigned D_0038;

struct tPSP {
	unsigned char _00[0x80];
	unsigned char _80[0x80];
};

extern struct tPSP far *far_psp;/*D_041C*/

extern unsigned /*D_31BC*/cursor_rate;
extern unsigned D_31C0;

extern unsigned char* D_3380;
extern unsigned char* D_33F8;
extern unsigned char* D_3438;
extern unsigned char* D_344A;
extern unsigned char* D_345C;
extern unsigned char* D_346E;
extern unsigned char* D_3683;//[19*5];
extern unsigned char* D_36E2;

/*====---- ----====*/

extern unsigned /*D_692E*/speed_info;
extern unsigned /*D_6931*/equip_flags;

struct tNPC {
	unsigned char _000[32];/*__6D76*/
	unsigned char _020[32];/*__6D96*/
	unsigned char _040[32];/*__6DB6*/
	unsigned char _060[32];/*__6DD6*/
	unsigned char _080[32];/*__6DF6*/
	unsigned char _0A0[32];/*__6E16*/
	unsigned char _0C0[32];/*__6E36*/
	unsigned char _0E0[32];/*__6E56*/
} __attribute__((packed));;

typedef unsigned char tMap32x32[32][32];
typedef unsigned char tMap8x8x8[8][8][8];

/* *.ULT or *.DNG */
struct t_500 {
	/*DNGMAP.SAV*/
	union {
		tMap8x8x8 x8x8x8;
		tMap32x32 x32x32;
	} /*__6976.__000*/_map;
	/*MONSTERS.SAV or OUTMONST.SAV or *.ULT(last 0x100 bytes)*/
	struct tNPC /*__6D76/__6976.__400*/_npc;
} __attribute__((packed));

extern struct t_500 D_6976;

/*====---- c_common ----====*/
extern unsigned /*D_6E80*/D_6E80;


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
	/*+26*/char _stat;
} __attribute__((packed));;

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
	/*+1de/+1e0*/U16 _trammel, _felucca;/*moons phase*/
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
} __attribute__((__packed__));

#ifdef WIN32
	#pragma pack()
#endif

extern struct tParty /*D_6E82*/Party;
extern unsigned /*D_7078*/D_7078;
extern void far /*D_707A*/*pShapes;
extern void far /*D_707E*/*pAnim;
extern unsigned /*D_7082*/D_7082;
extern void far /*D_7084*/*pCharset;
extern unsigned /*D_7088*/txt_X;
extern unsigned /*D_708A*/txt_Y;
extern void far /*D_708C*/*pTitle;
/*====---- ----====*/

/*====---- ----====*/

extern int /*C_0010*/Console(char *);
extern int /*C_0051*/Load(char *, int, char far *);
extern int /*C_0110*/Save(char *, int, char far *);

extern void /*C_027A*/u4_putc();

extern int cdecl Gra_0(
	int height/*bp04*/, int width/*bp06*/, unsigned char* pSrc_0/*bp08*/,
	int dst_y/*bp0a*/,
	unsigned dst_ofs/*bp0c*/, unsigned dst_seg/*bp0e*/,
	int dst_x_in_byte/*bp10*/
);
extern cdecl int Gra_dot();
extern int Gra_2();
extern cdecl int Gra_3(
#if 0
	int w/*in 8 pixels blocs*/, int h,
	int src_x/*in 8 pixels blocs*/, int src_y,
	void far *pic_data,
	int dst_y,
	int rand_pts,/*only works for cga?*/
	int dst_x
#endif
);
extern cdecl int Gra_inflate(
#if 0
#endif
);
extern int Gra_5();
extern int Gra_init();
extern int Gra_animFlow();
extern int Gra_animFlag();
extern int Gra_clrscr();
extern int Gra_putchar(char c);
extern cdecl int Gra_B(
#if 0
	int w/*in 8 pixels blocs*/, int h,
	int src_x/*in 8 pixels blocs*/, int src_y,
	void far *pic_data,
	int dst_y,
	int rand_pts,
	int dst_x
#endif
);
extern cdecl int Gra_C(
#if 0
#endif
);
extern int sizzleShapes();
extern int C_217E(char *, void far *);

/*low.asm*/
extern int /*C_244E*/low_init();
extern int /*C_2551*/low_clean();
extern int /*C_2583*/low_gra();

extern int C_3030();

extern cdecl int /*C_3149*/u_delay(unsigned, unsigned);
extern int /*C_31ED*/u_rand_a();
extern unsigned char /*C_321E*/u_rand_b();
extern int C_3290();
extern void  C_3299();
extern int C_32AB();
extern int C_331E();

extern int C_3F77(unsigned long, unsigned long*);

extern char TITLE[];

#endif
