/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "U4.H"

#include <stdlib.h>

char *D_1738[] ={
	/*Castles*/
	/*D_1682*/&AVATAR[0xF839 + 0x5] /* "LCB.TLK" */,
	/*D_168A*/&AVATAR[0x10945 + 0x7] /* "LYCAEUM.TLK" */,
	/*D_1696*/&AVATAR[0x10951 + 0x7] /* "EMPATH.TLK" */,
	/*D_16A1*/&AVATAR[0x1095C + 0x7] /* "SERPENT.TLK" */,
	/*Townes*/
	/*D_16AD*/&AVATAR[0x10968 + 0x7] /* "MOONGLOW.TLK" */,
	/*D_16BA*/&AVATAR[0x10975 + 0x7] /* "BRITAIN.TLK" */,
	/*D_16C6*/&AVATAR[0x10981 + 0x7] /* "JHELOM.TLK" */,
	/*D_16D1*/&AVATAR[0x1098C + 0x7] /* "YEW.TLK" */,
	/*D_16D9*/&AVATAR[0x10994 + 0x7] /* "MINOC.TLK" */,
	/*D_16E3*/&AVATAR[0x1099E + 0x7] /* "TRINSIC.TLK" */,
	/*D_16EF*/&AVATAR[0x109AA + 0x7] /* "SKARA.TLK" */,
	/*D_16F9*/&AVATAR[0x109B4 + 0x7] /* "MAGINCIA.TLK" */,
	/*Villages*/
	/*D_1706*/&AVATAR[0x109C1 + 0x7] /* "PAWS.TLK" */,
	/*D_170F*/&AVATAR[0x109CA + 0x7] /* "DEN.TLK" */,
	/*D_1717*/&AVATAR[0x109D2 + 0x7] /* "VESPER.TLK" */,
	/*D_1722*/&AVATAR[0x109DD + 0x7] /* "COVE.TLK" */
};

/*load casle/towne/village files*/
C_3E30(bp04)
unsigned bp04;
{
	if(Save(/*D_172B*/&AVATAR[0xF822 + 0x5] /* "OUTMONST.SAV" */, sizeof(struct tNPC), &(D_8742._npc)) == -1)
		exit(3);
	if(Load(D_0824[bp04 - 0x01], sizeof(struct t_500), &D_8742) == -1)
		exit(3);
	//File_TLK = dopen(D_1738[Party._loc - 1], 0);
	CurMode = MOD_BUILDING;
}

/*load dungeon files*/
C_3E94()
{
	register int si;

	if(Load(D_0894[Party._loc - 0x11], sizeof(tMap8x8x8), &(D_8742._map)) == -1)
		exit(3);
	//File_DNG = dopen(D_0894[Party._loc - 0x11], 0);
	for(si = 0x1f; si >= 0; si --)
		D_8742._npc._tile[si] = 0;
}

C_3EE4()
{
	C_3E94();
	CurMode = MOD_DUNGEON;
	Party._dir = DIR_E;
	Party._z = 0;
	DNG_main();
}

/*enter dungeon*/
C_3F03()
{
	Gra_CR();
	Gra_CR();
	if(Party._tile != TIL_1F) {
		Party._loc = 0;
		w_OnlyOnFoot();
		return 0;
	}
	Party._x = Party._y = 1;
	if(Save(/*D_1758*/&AVATAR[0xF822 + 0x5] /* "OUTMONST.SAV" */, sizeof(struct tNPC), &(D_8742._npc)) == -1)
		exit(3);
	C_3EE4();
}

/*enter towne*/
C_3F4A()
{
	register int si;

	u4_puts(D_1E98[127 + Party._loc - 1]);
	Gra_CR();
	Gra_CR();
	Party._y = D_959C.y = 15;
	Party._x = D_959C.x = 1;
	C_3E30(Party._loc);

	if((unsigned)(Party._loc - 0x05) < 8) {
		for(si = Party.f_1d8; --si >= 1; ) {
			if(Party.chara[si]._class == (Party._loc - 0x05)) {
				D_8742._npc._tile[31] = D_8742._npc._gtile[31] = D_8742._npc._var[31] = 0;
			}
		}
	}
}

/*enter the abyss?*/
C_3FB9()
{
	if((
		(Party.mItems >> 3) &
		(Party.mItems >> 4) &
		(Party.mItems >> 2) &
		(Party.mItems >> 11) &
		(Party.mItems >> 12) &
		(Party.mItems >> 10)
	) & 1) {
		u4_puts(D_1E98[127 + Party._loc - 1]);
		C_3F03();
	} else {
		Party._loc = 0;
		w_Cant_t();
	}
}

/*C_4018*/CMD_Enter()
{
	register int si;

	u4_puts(/*D_1765*/&AVATAR[0x10A20 + 0x7] /* "Enter " */);
	if(Party._loc != 0 || Party._tile == TIL_18) {
		w_What();
		return 0;
	}
	si = 31;
	while(si >= 0) {
#ifdef WIN32
		if(Party._x == (unsigned char)D_0844[si] && Party._y == (unsigned char)D_0864[si])
#else
		if (Party._x == (unsigned char)D_0844[si] && Party._y == (unsigned char)D_0864[si])
//		if(Party._x == D_0844[si] && Party._y == D_0864[si])
#endif
			break;
		si--;
	}
	if(si == -1) {
		w_What();
		return 0;
	}
	Party._loc = si + 1;
	Party.out_x = Party._x;
	Party.out_y = Party._y;
	switch(D_8742._map.x32x32[D_959C.y][D_959C.x]) {
		case TIL_09:
			u4_puts(/*D_176C*/&AVATAR[0x10A27 + 0x7] /* "dungeon!\n\n" */);
			u4_puts(D_1E98[127 + Party._loc - 1]);
			C_3F03();
		break;
		case TIL_0A:
			u4_puts(/*D_1777*/&AVATAR[0x10A32 + 0x7] /* "towne!\n\n" */);
			C_3F4A();
		break;
		case TIL_0B: case TIL_0E:
			u4_puts(/*D_1780*/&AVATAR[0x10A3B + 0x7] /* "castle!\n\n" */);
			u4_puts(D_1E98[127 + Party._loc - 1]);
			Gra_CR();
			Gra_CR();
			Party._y = D_959C.y = 0x1e;
			Party._x = D_959C.x = 0x0f;
			Party._z = 0;
			C_3E30(Party._loc);
		break;
		case TIL_0C:
			u4_puts(/*D_178A*/&AVATAR[0x10A45 + 0x7] /* "village!\n\n" */);
			C_3F4A();
		break;
		case TIL_1D:
			u4_puts(/*D_1795*/&AVATAR[0x10A50 + 0x7] /* "ruin!\n\n" */);
			C_3F4A();
		break;
		case TIL_4C:
			if(Party._x != 0xe9 || Party._y != 0xe9) {
				w_What();
				return 0;
			}
		case TIL_46:
			C_3FB9();
		break;
		case TIL_1E:
			u4_puts(/*D_179D*/&AVATAR[0x10A58 + 0x7] /* "the Shrine of\n" */);
			u4_puts(D_1E98[151 + Party._loc - 0x19]);
			Gra_CR();
			C_E72C();
		break;
		default:
			w_What();
			return 0;
	}
}

C_4164()
{
	register int si;

	for(si = 31; si >= 8 ;si --) {
		if(D_8742._npc._tile[si] == 0)
			break;
	}
	if(si == 7)
		si = U4_RND1(15) | 0x10;
	D_8742._npc._gtile[si] = D_8742._npc._tile[si] = Party._tile;
	D_8742._npc._x[si] = Party._x;
	D_8742._npc._y[si] = Party._y;
	D_8742._npc._var[si] = D_8742._npc._tlkidx[si] = 0;
	Party._tile = TIL_1F;
	Gra_CR();
}

/*C_41C0*/CMD_X_it()
{
	u4_puts(/*D_17AC*/&AVATAR[0x10A67 + 0x7] /* "X-it " */);
	if(Party._tile < TIL_14) {
		ship_x = Party._x;
		ship_y = Party._y;
		C_4164();
	} else if(Party._tile < TIL_16) {
		D_95C6 = 0;
		C_4164();
	} else if(Party._tile == TIL_18 && Party.f_1dc == 0) {
		C_4164();
	} else {
		w_What();
	}
}

C_4206(bp04)
unsigned bp04;
{
	register int si;

	for(si = 31; si >= 8; si --) {
		if(
			Party._x == D_8742._npc._x[si] &&
			Party._y == D_8742._npc._y[si] &&
			bp04 == (D_8742._npc._tile[si] & ~3)
		) break;
	}
	if(si != 7)
		D_8742._npc._gtile[si] = D_8742._npc._tile[si] = 0;
		Party._tile = tile_cur;
}

/*C_4253*/CMD_Board()
{
	if(Party._tile != TIL_1F) {
		u4_puts(/*D_17B2*/&AVATAR[0x10A6D + 0x7] /* "Board: " */);
		w_Cant_t();
		return 0;
	}
	if(tile_cur == TIL_14 || tile_cur == TIL_15) {
		C_4206(TIL_14);
		u4_puts(/*D_17BA*/&AVATAR[0x10A75 + 0x7] /* "Mount Horse!\n" */);
		return 0;
	}
	u4_puts(/*D_17C8*/&AVATAR[0x10A83 + 0x7] /* "Board " */);
	if(tile_cur == TIL_18) {
		C_4206(TIL_18);
		u4_puts(/*D_17CF*/&AVATAR[0x10A8A + 0x7] /* "Balloon!\n" */);
		Party.f_1dc = 0;
		return 0;
	}
	if(tile_cur < TIL_10 || tile_cur > TIL_13) {
		w_What();
		return 0;
	}
	C_4206(TIL_10);
	u4_puts(/*D_17D9*/&AVATAR[0x10A94 + 0x7] /* "Frigate!\n" */);
	if(ship_x != Party._x || ship_y != Party._y)
		Party._ship = 50;
}

/*C_42E8*/CMD_Yell()
{
	u4_puts(/*D_17E3*/&AVATAR[0x10A9E + 0x7] /* "Yell " */);
	if(Party._tile == TIL_14 || Party._tile == TIL_15) {
		if(D_95C6 ^= 1)
			u4_puts(/*D_17E9*/&AVATAR[0x10AA4 + 0x7] /* "Giddyup!\n" */);
		else
			u4_puts(/*D_17F3*/&AVATAR[0x10AAE + 0x7] /* "Whoa!\n" */);
	} else {
		w_What();
	}
}

unsigned D_17FA = 0;/*current opened door's x*/
unsigned D_17FC = 0;/*current opened door's y*/
unsigned D_17FE = 0;/*current opened door's delay*/

/*automatic door close*/
C_431D()
{
	if(CurMode == MOD_BUILDING && D_17FE) {
		if(--D_17FE == 0)
			D_8742._map.x32x32[D_17FC][D_17FA] = TIL_3B;
	} else {
		D_17FE = 0;
	}
}

/*C_4353*/CMD_Open()
{
	unsigned bp_02, bp_04;

	while(D_17FE)
		C_431D();
	if(CurMode != MOD_BUILDING) {
		u4_puts(/*D_1800*/&AVATAR[0x10ABB + 0x7] /* "Open; " */);
		w_NotHere();
		return 0;
	}
	set_input_mode(INPUT_MODE_GENERAL_DIRECTION);
	AskDir(/*D_1807*/&AVATAR[0x10AC2 + 0x7] /* "Open: " */, &bp_02, &bp_04);
	if(!(bp_02 | bp_04))
		return 0;
	if((bp_02 + Party._x) > 0x1f || bp_04) {
		w_NotHere();
	} else {
		register unsigned char *si;

		si = &(D_8742._map.x32x32[Party._y][bp_02 + Party._x]);
		if(*si == TIL_3A) {
			w_Cant_t();
		} else if(*si == TIL_3B) {
			D_17FA = (bp_02 + Party._x);
			D_17FC = Party._y;
			D_17FE = 5;
			*si = TIL_3E;
			u4_puts(/*D_180E*/&AVATAR[0x10AC9 + 0x7] /* "\nOpened!\n" */);
		} else {
			w_NotHere();
		}
	}
}

/*C_43F8*/CMD_Jimmy()
{
	int bp_04, bp_02;

	u4_puts(/*D_1818*/&AVATAR[0x10AD3 + 0x7] /* "Jimmy lock!\n" */);
	set_input_mode(INPUT_MODE_GENERAL_DIRECTION);
	AskDir(/*D_1825*/&AVATAR[0x10AE0 + 0x7] /* "Dir: " */, &bp_02, &bp_04);
	if(!(bp_02 | bp_04))
		return 0;
	if(CurMode != MOD_BUILDING || (bp_02 + Party._x) > 0x1f || bp_04) {
		w_NotHere();
	} else {
		register unsigned char *si;

		si = &(D_8742._map.x32x32[Party._y][bp_02 + Party._x]);
		if(*si != TIL_3A) {
			w_NotHere();
			return 0;
		}
		if(Party._keys == 0) {
			u4_puts(/*D_182B*/&AVATAR[0x10AE6 + 0x7] /* "No keys left!\n" */);
			return 0;
		}
		Party._keys --;
		*si = TIL_3B;
	}
}

/*C_4477*/CMD_Klimb()
{
	u4_puts(/*D_183A*/&AVATAR[0x10AF5 + 0x7] /* "Klimb " */);
	if(Party._loc == 0) {
		if(Party._tile != TIL_18) {
			w_What();
			return 0;
		}
		u4_puts(/*D_1841*/&AVATAR[0x10AFC + 0x7] /* "altitude\n" */);
		Party.f_1dc = 1;
		D_9440 = 0;
		return 0;
	}
	if(Party._loc == 1 && tile_cur == TIL_1B) {
		if(Party._tile != TIL_1F) {
			Gra_CR();
			w_OnlyOnFoot();
			return 0;
		}
		u4_puts(/*D_184B*/&AVATAR[0x10B06 + 0x7] /* "to second floor!\n" */);
		if(Load(/*D_185D*/&AVATAR[0xF82F + 0x5] /* "LCB_2.ULT" */, sizeof(struct t_500), &D_8742) == -1)
			exit(3);
	} else {
		w_What();
	}
}

/*C_44EE*/CMD_Descend()
{
	if(Party._tile == TIL_18) {
		u4_puts(/*D_1867*/&AVATAR[0x10B22 + 0x7] /* "Land Balloon\n" */);
		if(tile_cur != TIL_04) {
			sound(1,0);
			w_NotHere();
			return 0;
		}
		if(Party.f_1dc == 0) {
			u4_puts(/*D_1875*/&AVATAR[0x10B30 + 0x7] /* "Already Landed!\n" */);
			return 0;
		}
		Party.f_1dc = 0;
		D_9440 = 1;
		return 0;
	}
	if(Party._loc != 0x01) {
		u4_puts(/*D_1886*/&AVATAR[0x10B41 + 0x7] /* "Descend " */);
		w_What();
		return 0;
	}
	/*in LB's castle*/
	if(Party._tile != TIL_1F) {
		w_OnlyOnFoot();
		return 0;
	}
	u4_puts(/*D_188F*/&AVATAR[0x10B41 + 0x7] /* "Descend " */);
	if(tile_cur != TIL_1C) {
		w_What();
		return 0;
	}
	if(Party._y == 2) {
		u4_puts(/*D_1898*/&AVATAR[0x10B53 + 0x7] /* "into the depths!\n" */);
		Party.out_x = 0xef;
		Party.out_y = 0xf0;
		Party._y = Party._x = 5;
		Party._loc = 0x17;
		C_3EE4();
		return 0;
	}
	u4_puts(/*D_18AA*/&AVATAR[0x10B65 + 0x7] /* "to first floor!\n" */);
	if(Load(/*D_18BB*/&AVATAR[0xF97F + 0x5] /* "LCB_1.ULT" */, sizeof(struct t_500), &D_8742) == -1)
		exit(3);
}
