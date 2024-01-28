/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

/*Thanks to Fenyx4 for the
 comment on the rune names*/

#include "U4.H"

#include <stdlib.h>

char * D_27A6 = &AVATAR[0x11A51 + 0x2017] /* "Nothing Here!\n" */;

/* "You find..." & karma+=5*/
C_8D4B()
{
	u4_puts(/*D_2798*/&AVATAR[0x11A43 + 0x2017] /* "You find...\n" */);
	karma_inc(&(Party._honor), 5);
	Party.f_1e8 = Party._moves & 0xf0;
}

/*get random amount of reagent (max 99)*/
C_8D6D(bp04)
int bp04;
{
	Party._reagents[bp04] += U4_RND1(7) + 2;
	if(Party._reagents[bp04] > 99) {
		Party._reagents[bp04] = 99;
		u4_puts(/*D_27B5*/&AVATAR[0x11A60 + 0x2017] /* "Dropped some!\n" */);
		sound(1,0);
	}
}

C_8DAA()
{
	if((Party._trammel | Party._felucca) || (Party._moves & 0xf0) == Party.f_1e8) {
		u4_puts(D_27A6);
		return 0;
	}
	C_8D4B();
	u4_puts(/*D_27C4*/&AVATAR[0x11A6F + 0x2017] /* "Mandrake Root!\n" */);
	C_8D6D(7);
}

C_8DE0()
{
	if((Party._trammel | Party._felucca) || (Party._moves & 0xf0) == Party.f_1e8) {
		u4_puts(D_27A6);
		return 0;
	}
	C_8D4B();
	u4_puts(/*D_27D4*/&AVATAR[0x11A7F + 0x2017] /* "Nightshade!\n" */);
	C_8D6D(6);
}

C_8E16()
{
	if(TST_MSK(Party.mItems, 4)) {
		u4_puts(D_27A6);
		return 0;
	}
	SET_MSK(Party.mItems, 4);
	C_8D4B();
	u4_puts(/*D_27E1*/&AVATAR[0x11A8C + 0x2017] /* "The Bell of Courage!\n" */);
	XP_inc(0, 400);
}

C_8E46()
{
	if(TST_MSK(Party.mItems, 8)) {
		u4_puts(D_27A6);
		return 0;
	}
	SET_MSK(Party.mItems, 8);
	C_8D4B();
	u4_puts(/*D_27F7*/&AVATAR[0x11AA2 + 0x2017] /* "A Silver Horn!\n" */);
	XP_inc(0, 400);
}

C_8E77()
{
	if(TST_MSK(Party.mItems, 9)) {
		u4_puts(D_27A6);
		return 0;
	}
	SET_MSK(Party.mItems, 9);
	C_8D4B();
	u4_puts(/*D_2807*/&AVATAR[0x11AB2 + 0x2017] /* "The Wheel from the H.M.S. Cape!\n" */);
	XP_inc(0, 400);
}

C_8EA8()
{
	if(
		TST_MSK(Party.mItems, 0) ||
		(Party._trammel | Party._felucca) ||
		TST_MSK(Party.mItems, 1)
	) {
		u4_puts(D_27A6);
		return 0;
	}
	SET_MSK(Party.mItems, 0);
	C_8D4B();
	u4_puts(/*D_2828*/&AVATAR[0x11AD3 + 0x2017] /* "The Skull of Mondain the Wizard!\n" */);
	XP_inc(0, 400);
}

C_8EE8()
{
	if(
		TST_MSK(Party.mStones, 7) ||
		(Party._trammel | Party._felucca)
	) {
		u4_puts(D_27A6);
		return 0;
	}
	SET_MSK(Party.mStones, 7);
	C_8D4B();
	u4_puts(/*D_284A*/&AVATAR[0x11AF5 + 0x2017] /* "The Black Stone!\n" */);
	XP_inc(0, 200);
}

C_8F21()
{
	if(TST_MSK(Party.mStones, 6)) {
		u4_puts(D_27A6);
		return 0;
	}
	SET_MSK(Party.mStones, 6);
	C_8D4B();
	u4_puts(/*D_285C*/&AVATAR[0x11B07 + 0x2017] /* "The White Stone!\n" */);
	XP_inc(0, 200);
}

C_8F51()
{
	if(TST_MSK(Party.mItems, 3)) {
		u4_puts(D_27A6);
		return 0;
	}
	SET_MSK(Party.mItems, 3);
	C_8D4B();
	u4_puts(/*D_286E*/&AVATAR[0x11B19 + 0x2017] /* "The Book of Truth!\n" */);
	XP_inc(0, 400);
}

C_8F81()
{
	if(TST_MSK(Party.mItems, 2)) {
		u4_puts(D_27A6);
		return 0;
	}
	SET_MSK(Party.mItems, 2);
	C_8D4B();
	u4_puts(/*D_2882*/&AVATAR[0x11B2D + 0x2017] /* "The Candle of Love!\n" */);
	XP_inc(0, 400);
}

/*use telescope*/
C_8FB1()
{
	int bp_02;

	u4_puts(/*D_2897*/&AVATAR[0x11B42 + 0x2017] /* "You see a knob on the Telescope marked A-P\n" */);
	set_input_mode(INPUT_MODE_TELESCOPE);
	if((bp_02 = AskLetter(/*D_28C3*/&AVATAR[0x11B6E + 0x2017] /* "You Select:\x12\x12\b" */, 'A', 'P')) < 0)
		return 0;
	bp_02 -= 'A';
	if(Load(D_0824[bp_02], sizeof(tMap32x32), &(D_8742._map)) == -1)
		exit(3);
	C_B9EF(0);
	if(Load(D_0824[Party._loc - 1], sizeof(tMap32x32), &(D_8742._map)) == -1)
		exit(3);
}

C_9027()
{
	if(
		Party._armors[7] |
		Party._hones | Party._compa |
		Party._valor | Party._justi |
		Party._sacri | Party._honor |
		Party._spiri | Party._humil
	) {
		u4_puts(D_27A6);
		return 0;
	}
	Party._armors[7] = 8;
	C_8D4B();
	u4_puts(/*D_28D2*/&AVATAR[0x11B7D + 0x2017] /* "Mystic Armour!\n" */);
	XP_inc(0, 400);
}

C_9076()
{
	if(
		Party._weapons[15] |
		Party._hones | Party._compa |
		Party._valor | Party._justi |
		Party._sacri | Party._honor |
		Party._spiri | Party._humil
	) {
		u4_puts(D_27A6);
		return 0;
	}
	Party._weapons[15] = 8;
	C_8D4B();
	u4_puts(/*D_28E2*/&AVATAR[0x11B8D + 0x2017] /* "Mystic Weapons!\n" */);
	XP_inc(0, 400);
}

unsigned char * D_2904[]= { 
	&AVATAR[0x11BAF + 0x2017] /*{0x05,0x01}*/,
	&AVATAR[0x11BB1 + 0x2017] /*{0x06,0x02}*/,
	&AVATAR[0x11BB3 + 0x2017] /*{0x07,0x04}*/,
	&AVATAR[0x11BB5 + 0x2017] /*{0x08,0x08}*/,
	&AVATAR[0x11BB7 + 0x2017] /*{0x09,0x10}*/,
	&AVATAR[0x11BB9 + 0x2017] /*{0x0A,0x20}*/,
	&AVATAR[0x11BBB + 0x2017] /*{0x01,0x40}*/,
	&AVATAR[0x11BBD + 0x2017] /*{0x0D,0x80}*/
};

C_90C5()
{
	register int si;

	for(si = 7; si >= 0; si --)
		if(D_2904[si][0] == Party._loc)
			break;
	if(D_2904[si][1] & Party.mRunes) {
		u4_puts(D_27A6);
		return 0;
	}
	Party.mRunes |= D_2904[si][1];
	C_8D4B();
	u4_puts(/*D_28F3*/&AVATAR[0x11B9E + 0x2017] /* "The rune of " */);
	u4_puts(D_1E98[151 + si]);
	u4_puts(/*D_2900*/&AVATAR[0xF376 + 0x5] /* "!\n" */);
	XP_inc(0, 100);
}

typedef s_handler(void);
typedef s_handler *pS_handler;

// This one has function pointers so I cannot directly copy this from the AVATAR.EXE at offset 0x11BCB
// need to extract the data into this structure and then create another for the function pointers
struct t_05_original {
	unsigned char _00, _01, _02;
	short _03;
} * D_2920_original[] = {
	&AVATAR[0x11BCB + 0x2017], // {0x00,0xB6,0x36,C_8DAA},/*Mandrake root*/
	&AVATAR[0x11BD0 + 0x2017], // {0x00,0x64,0xA5,C_8DAA},/*Mandrake root*/
	&AVATAR[0x11BD5 + 0x2017], // {0x00,0x2E,0x95,C_8DE0},/*Nightshade*/
	&AVATAR[0x11BDA + 0x2017], // {0x00,0xCD,0x2C,C_8DE0},/*Nightshade*/
	&AVATAR[0x11BDF + 0x2017], // {0x00,0xB0,0xD0,C_8E16},/*The Bell of Courage!*/
	&AVATAR[0x11BE4 + 0x2017], // {0x00,0x2D,0xAD,C_8E46},/*'A Silver Horn!'*/
	&AVATAR[0x11BE9 + 0x2017], // {0x00,0x60,0xD7,C_8E77},/*The Wheel from the H.M.S. Cape!*/
	&AVATAR[0x11BEE + 0x2017], // {0x00,0xC5,0xF5,C_8EA8},/*The Skull of Mondain the Wizard!*/
	&AVATAR[0x11BF3 + 0x2017], // {0x00,0xE0,0x85,C_8EE8},/*The Black Stone!*/
	&AVATAR[0x11BF8 + 0x2017], // {0x00,0x40,0x50,C_8F21},/*The White Stone!*/
	&AVATAR[0x11BFD + 0x2017], // {0x02,0x06,0x06,C_8F51},/*The Book of Truth!*/
	&AVATAR[0x11C02 + 0x2017], // {0x10,0x16,0x01,C_8F81},/*The Candle of Love!*/
	&AVATAR[0x11C07 + 0x2017], // {0x02,0x16,0x03,C_8FB1},/*use Telescope*/
	&AVATAR[0x11C0C + 0x2017], // {0x03,0x16,0x04,C_9027},/*Mystic Armour!*/
	&AVATAR[0x11C11 + 0x2017], // {0x04,0x08,0x0F,C_9076},/*Mystic Weapons!*/
	&AVATAR[0x11C16 + 0x2017], // {0x05,0x08,0x06,C_90C5},/*rune of Honesty*/
	&AVATAR[0x11C1B + 0x2017], // {0x06,0x19,0x01,C_90C5},/*rune of Compassion*/
	&AVATAR[0x11C20 + 0x2017], // {0x07,0x1E,0x1E,C_90C5},/*rune of Valor*/
	&AVATAR[0x11C25 + 0x2017], // {0x08,0x0D,0x06,C_90C5},/*rune of Justice*/
	&AVATAR[0x11C2A + 0x2017], // {0x09,0x1C,0x1E,C_90C5},/*rune of Sacrifice*/
	&AVATAR[0x11C2F + 0x2017], // {0x0A,0x02,0x1D,C_90C5},/*rune of Honor*/
	&AVATAR[0x11C34 + 0x2017], // {0x01,0x11,0x08,C_90C5},/*rune of Spirituality*/
	&AVATAR[0x11C39 + 0x2017], // {0x0D,0x1D,0x1D,C_90C5},/*rune of Humility*/
	&AVATAR[0x11C3E + 0x2017]  // {0x00,0x00,0x00,0}
};


pS_handler D_2920_handler[] = {
	C_8DAA,/*Mandrake root*/
	C_8DE0,/*Nightshade*/
	C_8DE0,/*Nightshade*/
	C_8E16,/*The Bell of Courage!*/
	C_8E46,/*'A Silver Horn!'*/
	C_8E77,/*The Wheel from the H.M.S. Cape!*/
	C_8EA8,/*The Skull of Mondain the Wizard!*/
	C_8EE8,/*The Black Stone!*/
	C_8F21,/*The White Stone!*/
	C_8F51,/*The Book of Truth!*/
	C_8F81,/*The Candle of Love!*/
	C_8FB1,/*use Telescope*/
	C_9027,/*Mystic Armour!*/
	C_9076,/*Mystic Weapons!*/
	C_90C5,/*rune of Honesty*/
	C_90C5,/*rune of Compassion*/
	C_90C5,/*rune of Valor*/
	C_90C5,/*rune of Justice*/
	C_90C5,/*rune of Sacrifice*/
	C_90C5,/*rune of Honor*/
	C_90C5,/*rune of Spirituality*/
	C_90C5,/*rune of Humility*/
	0
};

/*C_913A*/CMD_Search()
{
	int si;

	u4_puts(/*D_2914*/&AVATAR[0x11BBF + 0x2017] /* "Search...\n" */);
	if(CurMode <= MOD_BUILDING && Party.f_1dc != 0) {
		w_DriftOnly();
		return 0;
	}
	for(si = 0; D_2920_original[si]->_03; si ++) {
		if(D_2920_original[si]->_00 == Party._loc && D_2920_original[si]->_01 == Party._x && D_2920_original[si]->_02 == Party._y)
			break;
	}
	if(D_2920_original[si]->_03 == 0) {
		u4_puts(D_27A6);
	} else {
		(D_2920_handler[si])();
	}
}
