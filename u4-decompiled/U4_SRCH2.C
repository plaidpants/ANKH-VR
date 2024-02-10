/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "U4.H"

/*----------------------------------------
MODULE SEARCH[dungeon]
----------------------------------------*/

char *D_2E04[] = {/*D_2DF0*/&AVATAR[0x12098 + 0x001a] /* "\nHmmm--No Effect!\n" */};

C_B730(bp06, bp04)
int bp06;
int bp04;
{
	Gra_11(bp06);
	sound(7,0);
	sound(7,0);
	sound(7,0);
	Gra_11(bp06);
	hitChara(bp06, 100 * bp04);
}

/*increment characteristic (max 50)*/
C_B770(bp06, bp04)
int *bp06;
int bp04;
{
	*bp06 += bp04;
	*bp06 = (*bp06 > 50)?50:*bp06;
}

short * D_2E6E = &AVATAR[0x12115 + 0x001a] /*{2,2,2,4,4,4,6,6}*/;/*magical ball hits points*/
short * D_2E7E[] = {/*magical ball characteristic*/
	&AVATAR[0x12125 + 0x001a] /*{0,0,5,0,5,5,5,0}*/,
	&AVATAR[0x12135 + 0x001a] /*{0,5,0,5,5,0,5,0}*/,
	&AVATAR[0x12145 + 0x001a] /*{5,0,0,5,0,5,5,0}*/
};

/*magical ball*/
C_B795()
{
	int bp_02;

	u4_puts(/*D_2E06*/&AVATAR[0x120AE + 0x001a] /* "\nYou find a Magical Ball...\n" */);
	set_input_mode(INPUT_MODE_GENERAL_ASK_CHARACTER_NUMBER);
	if((bp_02 = AskChara(/*D_2E23*/&AVATAR[0x120CB + 0x001a] /* "Who touches?\x12\x12\b" */)) < 0)
		return 0;
	if(!isCharaConscious(bp_02)) {
		u4_puts(/*D_2E33*/&AVATAR[0x120DB + 0x001a] /* "\nDisabled!\n" */);
		return 0;
	}
	D_8742._map.x8x8x8[Party._z][Party._y][Party._x] = 0;
	C_B730(bp_02, D_2E6E[Party._loc - 0x11]);
	/*-- which characteristic(s) ? --*/
	if(D_2E7E[0][Party._loc - 0x11]) {
		C_B770(&(Party.chara[bp_02]._str), 5);
		u4_puts(/*D_2E3F*/&AVATAR[0x120E7 + 0x001a] /* "Strength + 5\n" */);
	}
	if(D_2E7E[1][Party._loc - 0x11]) {
		C_B770(&(Party.chara[bp_02]._dex), 5);
		u4_puts(/*D_2E4D*/&AVATAR[0x120F5 + 0x001a] /* "Dexterity + 5\n" */);
	}
	if(D_2E7E[2][Party._loc - 0x11]) {
		C_B770(&(Party.chara[bp_02]._int), 5);
		u4_puts(/*D_2E5C*/&AVATAR[0x12104 + 0x001a] /* "Intelligence + 5\n" */);
	}
}

/*fountain*/
C_B863()
{
	int bp_02;

	u4_puts(/*D_2EAE*/&AVATAR[0x12156 + 0x001a] /* "You find a Fountain.\n" */);
	set_input_mode(INPUT_MODE_GENERAL_ASK_CHARACTER_NUMBER);
	if((bp_02 = AskChara(/*D_2EC4*/&AVATAR[0x1216C + 0x001a] /* "Who drinks?\x12\x12\b" */)) < 0)
		return 0;
	if(!isCharaConscious(bp_02)) {
		u4_puts(/*D_2ED3*/&AVATAR[0x120DB + 0x001a] /* "\nDisabled!\n" */);
		return 0;
	}
	switch(tile_cur & 0xf) {
		case 0:	u4_puts(D_2E04[0]); break;
		case 1:
			if(Party.chara[bp_02]._HP[0] == Party.chara[bp_02]._HP[1]) {
				u4_puts(D_2E04[0]);
			} else {
				u4_puts(/*D_2EDF*/&AVATAR[0x12187 + 0x001a] /* "\nAhh-Refreshing!\n" */);
				Party.chara[bp_02]._HP[0] = Party.chara[bp_02]._HP[1];
			}
		break;
		case 2:
			u4_puts(/*D_2EF1*/&AVATAR[0x12199 + 0x001a] /* "\nBleck--Nasty!\n" */);
			C_B730(bp_02, 1);
		return 0;
		case 3:
			if(Party.chara[bp_02]._status != 'P') {
				u4_puts(D_2E04[0]);
			} else {
				Party.chara[bp_02]._status = 'G';
				u4_puts(/*D_2F01*/&AVATAR[0x121A9 + 0x001a] /* "\nHmm--Delicious!\n" */);
			}
		break;
		case 4:
			if(Party.chara[bp_02]._status == 'P') {
				u4_puts(D_2E04[0]);
			} else {
				Party.chara[bp_02]._status = 'P';
				u4_puts(/*D_2F13*/&AVATAR[0x121BB + 0x001a] /* "\nArgh-Choke-Gasp!\n" */);
				C_B730(bp_02, 1);
			};
		break;
	}
}

/*found stone*/
C_B93F()
{
	if(
		Party._loc == 0x17 || Party._loc == 0x18 ||
		TST_MSK(Party.mStones, Party._loc - 0x11)
	) {
		u4_puts(/*D_2F26*/&AVATAR[0x121CE + 0x001a] /* "\nYou find Nothing!\n" */);
		return 0;
	}
	SET_MSK(Party.mStones, Party._loc - 0x11);
	u4_puts(/*D_2F3A*/&AVATAR[0x121E2 + 0x001a] /* "\nYou find the " */);
	u4_puts(D_0884[Party._loc - 0x11]);
	u4_puts(/*D_2F49*/&AVATAR[0x121F1 + 0x001a] /* " stone!\n" */);
	karma_inc(&(Party._honor), 5);
	XP_inc(0, 200);
}

/*(S)earch [dungeon]*/
C_B9B2()
{
	u4_puts(/*D_2F52*/&AVATAR[0x11BBF + 0x0017] /* "Search...\n" */);
	switch(tile_cur & 0xf0) {
		case 0x70: C_B795(); break;
		case 0x90: C_B863(); break;
		case 0xB0: C_B93F(); break;
		default: u4_puts(/*D_2F5D*/&AVATAR[0x121CE + 0x001a] /* "\nYou find Nothing!\n" */);
	}
}
