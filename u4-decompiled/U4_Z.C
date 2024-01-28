/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "U4.H"

#include <string.h>

C_45B5()
{
	for( ;txt_Y <= 8; txt_Y ++) {
		txt_X = 24;
		u4_puts(/*D_18C6*/"               ");
	}
}

/*display centered string*/
C_45D6(si/*bp06*/, bp04)
register char *si;
unsigned bp04;
{
	register int di;

	di = strlen(si);
	if(di > 12)
		di = 12;
	di -= (di & 1);

	txt_X = (di>=8) + ((12 - di)  / 2) + bp04 + 25;
	if(txt_Y == 0) {
		txt_X --;
		u4_putc(0x10);
	}
	u4_puts(si);
	if(txt_Y == 0)
		u4_putc(0x11);
}

C_4649()
{
	unsigned bp_02, bp_04;

	bp_02 = txt_Y;
	bp_04 = txt_X;
	txt_Y = 0;
	txt_X = 24;
	do
		Gra_putchar('\r');
	while(++txt_X < 39);
	txt_Y = bp_02;
	txt_X = bp_04;
}

C_4689(bp04)
int bp04;
{
	register struct tChara *si;

	set_zstats_mode(ZSTATS_MODE_CHARACTER_DETAIL);
	set_zstats_character(bp04);
	si = &(Party.chara[bp04]);
	txt_Y = 0; C_45D6(si->_name, 0);
	txt_X = 24;
	txt_Y = 1; u4_putc(si->p_24);
	txt_Y = 1; C_45D6(D_1E98[77 + si->_class], 0);
	txt_X = 38; u4_putc(si->_stat);
	txt_X = 25;
	txt_Y = 3; u4_puts(/*D_18D6*/&AVATAR[0x10B89 + 0x200F] /* "MP:" */); u4_putl(si->_MP, 2, '0');
	txt_X = 32; u4_puts(/*D_18DA*/&AVATAR[0x10B8D + 0x200F] /* "LV:" */); u4_putc((si->_HP[1]/100)+'0');
	txt_X = 24;
	txt_Y = 4; u4_puts(/*D_18DE*/&AVATAR[0x10B91 + 0x200F] /* "STR:" */); u4_putl(si->_str, 2, '0');
	txt_X = 32; u4_puts(/*D_18E3*/&AVATAR[0xF7AE + 0x5] /* "HP:" */); u4_putl(si->_HP[0], 4, '0');
	txt_X = 24;
	txt_Y = 5; u4_puts(/*D_18E7*/&AVATAR[0x10B9A + 0x200F] /* "DEX:" */); u4_putl(si->_dex, 2, '0');
	txt_X = 32; u4_puts(/*D_18EC*/&AVATAR[0x10B9F + 0x200F] /* "HM:" */); u4_putl(si->_HP[1], 4, '0');
	txt_X = 24;
	txt_Y = 6; u4_puts(/*D_18F0*/&AVATAR[0x10BA3 + 0x200F] /* "INT:" */); u4_putl(si->_int, 2, '0');
	txt_X = 32; u4_puts(/*D_18F5*/&AVATAR[0x10B9B + 0x200F] /* "EX:" */); u4_putl(si->_XP, 4, '0');
	txt_Y = 7;
	txt_X = 24; u4_puts(/*D_18F9*/&AVATAR[0x10BAC + 0x200F] /* "W:" */); u4_puts(D_1E98[37 + si->_weapon]);
	txt_Y = 8;
	txt_X = 24; u4_puts(/*D_18FC*/&AVATAR[0x10BAF + 0x200F] /* "A:" */); u4_puts(D_1E98[53 + si->_armor]);
}

C_4832()
{
	register int si;

	set_zstats_mode(ZSTATS_MODE_WEAPONS);
	txt_Y = 0;
	C_45D6(/*D_18FF*/&AVATAR[0x10BB2 + 0x200F] /* "Weapons" */, 0);
	txt_Y = 1;
	txt_X = 24;
	u4_puts(/*D_1907*/&AVATAR[0x10BBA + 0x200F] /* "A-Hands" */);
	txt_Y = 2;
	txt_X = 24;
	si = 1;
	do {
		if(Party._weapons[si]) {
			u4_putc(si+'A');
			u4_putl(Party._weapons[si], 2, '-');
			u4_putc('-');
			u4_puts(D_1E98[61 + si]);
			txt_X = (txt_X - 1) & ~7;
			if(++txt_Y == 9) {
				txt_Y = 1;
				txt_X += 8;
			}
		}
	} while(++si < 0x10);
	while(txt_X < 40) {
		u4_puts(/*D_190F*/&AVATAR[0x10BC2 + 0x200F] /* "       " */);
		txt_X = (txt_X - 1) & ~7;
		if(++txt_Y == 9) {
			txt_Y = 1;
			txt_X += 8;
		}
	}
}

C_48F8()
{
	register int si;

	set_zstats_mode(ZSTATS_MODE_ARMOUR);
	txt_Y = 0;
	C_45D6(/*D_1917*/&AVATAR[0x10BCA + 0x200F] /* "Armour" */, 0);
	txt_Y = 1;
	txt_X = 24;
	u4_puts(/*D_191E*/&AVATAR[0x10BD1 + 0x200F] /* "A  -No Armour" */);
	txt_Y = 2;
	for(si = 1; si < 8; si ++) {
		if(Party._armors[si]) {
			txt_X = 24;
			u4_putc(si + 'A');
			u4_putl(Party._armors[si], 2, '-');
			u4_putc('-');
			u4_puts(D_1E98[53 + si]);
			while(txt_X != 39)
				u4_putc(' ');
			txt_Y ++;
		}
	}
	C_45B5();
}

C_4987()
{
	set_zstats_mode(ZSTATS_MODE_EQUIPMENT);
	txt_Y = 0;
	C_45D6(/*D_192C*/&AVATAR[0x10BDF + 0x200F] /* "Equipment" */, -1);

	txt_X = 24; txt_Y = 1;
	u4_putl(Party._torches, 2, ' '); u4_puts(/*D_1936*/&AVATAR[0x10BE9 + 0x200F] /* " Torches" */);

	txt_X = 24; txt_Y ++;
	u4_putl(Party._gems, 2, ' '); u4_puts(/*D_193F*/&AVATAR[0x10BF2 + 0x200F] /* " Gems" */);

	txt_X = 24; txt_Y ++;
	u4_putl(Party._keys, 2, ' '); u4_puts(/*D_1945*/&AVATAR[0x10BF8 + 0x200F] /* " Keys" */);

	if(Party._sextants) {
		txt_X = 24; txt_Y = 4;
		u4_putl(Party._sextants, 2, ' '); u4_puts(/*D_194B*/&AVATAR[0x10BFE + 0x200F] /* " Sextants" */);
	}
	txt_Y ++;
	C_45B5();
}

char * D_199A = &AVATAR[0x10C4D + 0x200F] /* "BYRGOPWB" */;
char * D_19A4 = &AVATAR[0x10C57 + 0x200F] /* "HCVJSHSH" */;

C_4A3D()
{
	register int si;

	set_zstats_mode(ZSTATS_MODE_ITEMS);
	txt_Y = 0;
	C_45D6(/*D_1955*/&AVATAR[0x10C08 + 0x200F] /* "Items" */, 0);
	txt_Y = 1;
	if(Party.mStones) {
		txt_X = 24;
		u4_puts(/*D_195B*/&AVATAR[0x10C0E + 0x200F] /* "Stones:" */);
		si = 0;
		do {
			if(TST_MSK(Party.mStones, si))
				u4_putc(D_199A[si]);
		} while(++si < 8);
		txt_Y ++;
	}
	if(Party.mRunes) {
		txt_X = 24;
		u4_puts(/*D_1963*/&AVATAR[0x10C16 + 0x200F] /* "Runes:" */);
		si = 0;
		do {
			if(TST_MSK(Party.mRunes, si))
				u4_putc(D_19A4[si]);
		} while(++si < 8);
		txt_Y ++;
	}
	if(TST_MSK(Party.mItems, 4) || TST_MSK(Party.mItems, 3) || TST_MSK(Party.mItems, 2)) {
		txt_X = 24;
		if(TST_MSK(Party.mItems, 4))
			u4_puts(/*D_196A*/&AVATAR[0xF56C + 0x5] /* "Bell " */);
		if(TST_MSK(Party.mItems, 3))
			u4_puts(/*D_1970*/&AVATAR[0x10C23 + 0x200F] /* "Book " */);
		if(TST_MSK(Party.mItems, 2)) {
			u4_puts(/*D_1976*/&AVATAR[0xF5BA + 0x5] /* "Candl" */);
			if(!TST_MSK(Party.mItems, 3) || !TST_MSK(Party.mItems, 4))
				u4_putc('e');
		}
		txt_Y ++;
	}
	if(TST_MSK(Party.mItems, 5) || TST_MSK(Party.mItems, 6) || TST_MSK(Party.mItems, 7)) {
		txt_X = 24;
		u4_puts(/*D_197C*/&AVATAR[0x10C2F + 0x200F] /* "3 Part Key:" */);
		if(TST_MSK(Party.mItems, 7))
			u4_putc('T');
		if(TST_MSK(Party.mItems, 6))
			u4_putc('L');
		if(TST_MSK(Party.mItems, 5))
			u4_putc('C');
		txt_Y ++;
	}
	if(TST_MSK(Party.mItems, 8)) {
		txt_X = 24;
		u4_puts(/*D_1988*/&AVATAR[0xF5DA + 0x5] /* "Horn" */);
		txt_Y ++;
	}
	if(TST_MSK(Party.mItems, 9)) {
		txt_X = 24;
		u4_puts(/*D_198D*/&AVATAR[0xF608 + 0x5] /* "Wheel" */);
		txt_Y ++;
	}
	if(TST_MSK(Party.mItems, 0)) {
		txt_X = 24;
		u4_puts(/*D_1993*/&AVATAR[0xF63B + 0x5] /* "Skull" */);
	}
}

C_4BC7()
{
	register int si;

	set_zstats_mode(ZSTATS_MODE_REAGENTS);
	txt_Y = 0;
	C_45D6(/*D_19AD*/&AVATAR[0x10C60 + 0x200F] /* "Reagents" */, 0);
	txt_Y = 1;
	si = 0;
	do {
		if(Party._reagents[si]) {
			txt_X = 24;
			u4_putc(si+'A');
			u4_putl(Party._reagents[si], 2, '-');
			u4_putc('-');
			u4_puts(D_1E98[93 + si]);
			while(txt_X != 39)
				u4_putc(' ');
			txt_Y ++;
		}
	} while(++si < 8);
	C_45B5();
}

C_4C42()
{
	register int si;

	set_zstats_mode(ZSTATS_MODE_MIXTURES);
	txt_Y = 0;
	C_45D6(/*D_19B6*/&AVATAR[0x10C69 + 0x200F] /* "Mixtures" */, 0);
	txt_Y = 1;
	txt_X = 24;
	si = 0;
	do {
		if(Party._mixtures[si]) {
			u4_putc(si+'A');
			u4_putc('-');
			u4_putl(Party._mixtures[si], 2, '0');
			txt_X -= 4;
			txt_Y ++;
			if(txt_Y == 9) {
				txt_Y = 1;
				txt_X += 5;
				if(txt_X >= 39)
					return 0;
			}
		}
	} while(++si < 26);
}

typedef z_handler(int);
typedef z_handler *pZ_handler;

pZ_handler D_19C0[] = {
	C_4689,C_4689,C_4689,C_4689,
	C_4689,C_4689,C_4689,C_4689,
	C_4832,
	C_48F8,
	C_4987,
	C_4A3D,
	C_4BC7,
	C_4C42
};

C_4CC1(bp04)
int bp04;
{
	unsigned bp_02, bp_04;

	bp_02 = txt_Y;
	bp_04 = txt_X;
	do {
		register int si;

		Gra_13();
		C_4649();
		(*D_19C0[bp04])(bp04);
		set_input_mode(INPUT_MODE_GENERAL_ASK_CHARACTER_NUMBER);
		while(!u_kbhit())
			Sleep(20 * SLEEPFACTOR); // ADDED otherwise we will spin in too tight a loop for anything to happen and lock up the game engine
		switch(si = u_kbread()) {
			case KBD_1:
			case KBD_2:
			case KBD_3:
			case KBD_4:
			case KBD_5:
			case KBD_6:
			case KBD_7:
			case KBD_8:
				if((si&0xf) <= Party.f_1d8)
					bp04 = (si&0xf) - 1;
				else
					sound(1,0);
			break;
			case KBD_RIGHT: case KBD_DOWN:
				bp04 = (bp04 + 1) % 14;
				if(bp04 == Party.f_1d8)
					bp04 = 8;
			break;
			case KBD_UP: case KBD_LEFT:
				if(bp04 == 8)
					bp04 = Party.f_1d8 - 1;
				else
					bp04 = (bp04 + 13) % 14;
			break;
			case KBD_PGUP:
				if(bp04 >= 1 && bp04 <= 8)
					bp04 = 0;
				else if(bp04 > 8 && bp04 <= 13)
					bp04 = 8;
				else bp04 = 13;
			break;
			case KBD_HOME:
				bp04 = 0;
			break;
			case KBD_0:
				bp04 = 8;
			break;
			case KBD_PGDN:
				if(bp04 >= 0 && bp04 <= 7)
					bp04 = 8;
				else if(bp04 >= 8 && bp04 <= 12)
					bp04 = 13;
				else
					bp04 = 0;
			break;
			case KBD_END:
				bp04 = 13;
			break;
			default:
				bp04 = -1;
		}
	} while(bp04 != -1);
	C_4649();
	Gra_13();
	dspl_Stats();
	txt_Y = bp_02;
	txt_X = bp_04;
}

/*C_4E45*/CMD_Ztats()
{
	if(CurMode >= MOD_COMBAT) {
		u4_puts(/*D_19DC*/&AVATAR[0x10C8F + 0x200F] /* "Ztats\n" */);
		Gra_11(activeChara);
		C_4CC1(activeChara);
		Gra_11(activeChara);
	} else {
		int bp_02;

		set_input_mode(INPUT_MODE_GENERAL_ASK_CHARACTER_NUMBER);
		bp_02 = AskChara(/*D_19E3*/&AVATAR[0x10C96 + 0x200F] /* "Ztats for:\x12\x12\b" */);
		if(bp_02 != -1)
			C_4CC1((bp_02 == -2)?8:bp_02);
	}
}
