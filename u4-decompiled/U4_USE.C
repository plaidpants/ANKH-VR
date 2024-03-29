/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "U4.H"

#include <string.h>

char * D_00EE = &AVATAR[0xF3AB + 0x0005] /* "Hmm...No effect!\n" */;
char * D_0100 = &AVATAR[0xF3BD + 0x0005] /* "None owned!\n" */;
char * D_010E = &AVATAR[0xF3CB + 0x0005]; // {/*colors masks*/
#if 0
	(1<<6)|(1<<5)|(1<<3)|(1<<0),/*truth*/ // 0x69
	(1<<6)|(1<<4)|(1<<3)|(1<<1),/*love*/  // 0x5A
	(1<<6)|(1<<5)|(1<<4)|(1<<2) /*courage*/ // 0x74??
};
#endif

void C_01E1()
{
	int bp_10, bp_02;
	char bp_12[12];
	char bp_04;

	if(Combat._charaX[activeChara] != 5 || Combat._charaY[activeChara] != 5 || D_943E == -1) {
		u4_puts(D_00EE);
		return;
	}
	u4_puts(/*D_0080*/&AVATAR[0xF33D + 0x0005] /* "\nThere are holes for 4 stones. What colors:\n" */);
	bp_04 = 0;
	for(bp_02 = 1; bp_02 <= 4; bp_02++) {
		u4_putc(bp_02 + 'A' - 1); u4_putc(':');
		add_char_to_text_buffer(bp_02 + 'A' - 1);
		add_char_to_text_buffer(':');
		set_input_mode(INPUT_MODE_USE_STONE_COLOR_WORD);
		u4_gets(bp_12, 11);
		Gra_CR();
		for(bp_10 = 7; bp_10 >= 0; bp_10--) {
			if(_strnicmp(D_0884[bp_10], bp_12, 12) == 0)
				break;
		}
		if(bp_10 == -1 || !TST_MSK(Party.mStones, bp_10)) {
			if(*bp_12 != '\0')
				u4_puts(D_0100);
			return;
		}
		if((1 << bp_10) & bp_04) {
			u4_puts(/*D_00AD*/&AVATAR[0xF36A + 0x0005] /* "Already used!\n" */);
			return;
		}
		bp_04 |= (1 << bp_10);
	}
	if(D_010E[D_943E] != bp_04) {
		u4_puts(D_00EE);
		return;
	}
	switch(D_943E) {
		case 0:
			if(TST_MSK(Party.mItems, ST_KEY_T)) {
				u4_puts(D_00EE);
				return;
			}
			SET_MSK(Party.mItems, ST_KEY_T);
		break;
		case 1:
			if(TST_MSK(Party.mItems, ST_KEY_L)) {
				u4_puts(D_00EE);
				return;
			}
			SET_MSK(Party.mItems, ST_KEY_L);
		break;
		case 2:
			if(TST_MSK(Party.mItems, ST_KEY_C)) {
				u4_puts(D_00EE);
				return;
			}
			SET_MSK(Party.mItems, ST_KEY_C);
		break;
	}
	u4_puts(/*D_00BC*/&AVATAR[0xF379 + 0x0005] /* "Thou doth find one third of the Three Part Key!\n" */);
}

char *D_0284[] = {
	/*D_0111*/&AVATAR[0xF3CE + 0x0005] /* "Truth" */,
	/*D_0117*/&AVATAR[0xF3D4 + 0x0005] /* "Love" */,
	/*D_011C*/&AVATAR[0xF3D9 + 0x0005] /* "Courage" */,
	/*D_0124*/&AVATAR[0xF3E1 + 0x0005] /* "Truth and Love" */,
	/*D_0133*/&AVATAR[0xF3F0 + 0x0005] /* "Love and Courage" */,
	/*D_0144*/&AVATAR[0xF401 + 0x0005] /* "Courage and Truth" */,
	/*D_0156*/&AVATAR[0xF413 + 0x0005] /* "Truth, Love and Courage" */,
	/*D_016E*/&AVATAR[0xF42B + 0x0005] /* "\nA voice rings out:  What Virtue exists independently of Truth, Love and Courage" */
};

/*use stone(s)*/
C_0311() {
	int bp_02;

	if(Party.mStones) {
		char bp_10[14];
		if(CurMode != MOD_COM_ROOM && CurMode != MOD_DUNGEON)
			u4_puts(/*D_01BF*/&AVATAR[0xF47C + 0x0005] /* "No place to Use them!\n" */);
		if(CurMode == MOD_DUNGEON) {
			if(tile_cur != 0xb0 || Party._loc != 0x18) {
				u4_puts(D_00EE);
				return 0;
			}
			if(Party._z != 7)
				u4_puts(/*D_01D6*/&AVATAR[0xF493 + 0x0005] /* "\nAs thou doth approach, a voice rings out: What virtue dost stem from " */);
			u4_puts(D_0284[Party._z]);
			u4_puts(/*D_021D*/&AVATAR[0xF4DA + 0x0005] /* "?\n\n" */);
			set_input_mode(INPUT_MODE_VIRTUE_WORD);
			u4_gets(bp_10, 13);
			Gra_CR();
			if(_strnicmp(bp_10, D_1E98[151 + Party._z], 14)) {
C_03A1:
				if(bp_10[0])
					u4_puts(D_00EE);
				return 0;
			}
			u4_puts(/*D_0221*/&AVATAR[0xF4DE + 0x0005] /* "\nThe Voice says: Use thy Stone.\n\nColor:\n" */);
			set_input_mode(INPUT_MODE_USE_STONE_COLOR_WORD);
			u4_gets(bp_10, 11);
			Gra_CR();
			bp_02 = 7;
			while(
				_strnicmp(bp_10, D_0884[bp_02], 12) &&
				--bp_02 >= 0
			);
			if(bp_02 == -1)
				goto C_03A1;
			if(!TST_MSK(Party.mStones, bp_02)) {
				u4_puts(/*D_024A*/&AVATAR[0xF507 + 0x0005] /* "\nYou have none!\n\n" */);
				return 0;
			}
			if(Party._z != bp_02) {
				u4_puts(D_00EE);
				return 0;
			}
			if(Party._z == 7) {
				C_31F4();/*last phase of game*/
				return 0;
			}
			D_8742._map.x8x8x8[Party._z][Party._y][Party._x] = 0x20; // changes to dungeon ladder down tile
			u4_puts(/*D_025C*/&AVATAR[0xF519 + 0x0005] /* "\nThe altar changes before thyne eyes!\n" */);
		} else {
			C_01E1();
		}
	} else {
		u4_puts(D_0100);
	}
}

/*use key(s)*/
C_044C() {
	if(
		((Party.mItems >> ST_KEY_T) & 1) |
		((Party.mItems >> ST_KEY_L) & 1) |
		((Party.mItems >> ST_KEY_C) & 1)
	)
		u4_puts(&AVATAR[0xF47C + 0x0005] /* "No place to Use them!\n" */);
	else
		u4_puts(D_0100);
}

/*use bell*/
C_0487() {
	if(!TST_MSK(Party.mItems, ST_BELL)) {
		u4_puts(D_0100);
		return 0;
	}
	if(Party._loc != 0 || Party._x != 0xe9 || Party._y != 0xe9) {
		u4_puts(D_00EE);
		return 0;
	}
	SET_MSK(Party.mItems, ST_USE_BELL);
	u4_puts(&AVATAR[0xF568 + 0x0005] /* "The Bell rings on and on!\n" */);
}

/*use book*/
C_04C0() {
	if(!TST_MSK(Party.mItems, ST_BOOK)) {
		u4_puts(D_0100);
		return 0;
	}
	if(Party._loc != 0 || Party._x != 0xe9 || Party._y != 0xe9 ||
		!TST_MSK(Party.mItems, ST_USE_BELL)
	) {
		u4_puts(D_00EE);
		return 0;
	}
	SET_MSK(Party.mItems, ST_USE_BOOK);
	u4_puts(&AVATAR[0xF583 + 0x0005] /* "The words resonate with the ringing!\n" */);
}

/*use candle*/
C_0501() {
	if(!TST_MSK(Party.mItems, ST_CANDLE)) {
		u4_puts(D_0100);
		return 0;
	}
	if(Party._loc != 0 || Party._x != 0xe9 || Party._y != 0xe9 ||
		!TST_MSK(Party.mItems, ST_USE_BOOK)
	) {
		u4_puts(D_00EE);
		return 0;
	}
	SET_MSK(Party.mItems, ST_USE_CANDLE);
	u4_puts(&AVATAR[0xF5A9 + 0x0005] /* "As you light the Candle the Earth Trembles!\n" */);
	sound(6,0);
	shakefx();
}

/*use horn*/
C_0553() {
	if(!TST_MSK(Party.mItems, ST_HORN)) {
		u4_puts(D_0100);
		return 0;
	}
	if(Party._loc != 0) {
		u4_puts(D_00EE);
		return 0;
	}
	u4_puts(&AVATAR[0xF5D6 + 0x0005] /* "The Horn sounds an eerie tone!\n" */);
	spell_sta = 1;
	spell_cnt = 10;
	dspl_Stats();
}

/*use wheel*/
C_058C() {
	if(!TST_MSK(Party.mItems, ST_WHEEL)) {
		u4_puts(D_0100);
		return 0;
	}
	if(Party._loc != 0 || Party._tile > TIL_13 || Party._ship != 50) {
		u4_puts(D_00EE);
		return 0;
	}
	u4_puts(&AVATAR[0xF5F6 + 0x0005] /* "Once mounted, the Wheel glows with a blue light!\n" */);
	Party._ship = 99;
	dspl_Stats();
}

/*use skull*/
C_05CE() {
	int bp_02;

	if(!TST_MSK(Party.mItems, ST_SKULL)) {
		u4_puts(D_0100);
		return 0;
	}
	if(Party._loc != 0 || Party._x != 0xe9 || Party._y != 0xe9) {
		u4_puts(&AVATAR[0xF628 + 0x0005] /* "\nYou hold the evil Skull of Mondain the Wizard aloft....\n" */);
		sound(6,0); shakefx();
		Gra_09();
		sound(6,0); shakefx();
		Gra_09();
		sound(6,0); shakefx();
		if(CurMode >= MOD_COMBAT) {
			for(bp_02 = 31; bp_02 >= 0; bp_02 --) {
				if(D_8742._npc._tile[bp_02] != TIL_5E)
					D_8742._npc._tile[bp_02] = D_8742._npc._gtile[bp_02] = 0;
			}
			for(bp_02 = 15; bp_02 >= 0; bp_02 --) {
				if(Fighters._tile[bp_02] != TIL_5E)
					Fighters._tile[bp_02] = Fighters._gtile[bp_02] = 0;
			}
		} else {
			for(bp_02 = (Party._loc == 0)?7:31; bp_02 >= 0; bp_02 --) {
				if(D_8742._npc._tile[bp_02] != TIL_5E)
					D_8742._npc._tile[bp_02] = D_8742._npc._gtile[bp_02] = 0;
			}
		}
/*C_06B2:*/
		t_callback();
		karma_dec((U16 *)&(Party._hones), 5);
		karma_dec((U16 *)&(Party._compa), 5);
		karma_dec((U16 *)&(Party._valor), 5);
		karma_dec((U16 *)&(Party._justi), 5);
		karma_dec((U16 *)&(Party._sacri), 5);
		karma_dec((U16 *)&(Party._honor), 5);
		karma_dec((U16 *)&(Party._spiri), 5);
		karma_dec((U16 *)&(Party._humil), 5);
		dspl_Stats();
	} else {
		u4_puts(&AVATAR[0xF662 + 0x0005] /* "\nYou cast the Skull of Mondain into the Abyss!\n" */);
		SET_MSK(Party.mItems, ST_CAST_SKULL);
		karma_inc((char *)&(Party._hones), 10);
		karma_inc((char *)&(Party._compa), 10);
		karma_inc((char *)&(Party._valor), 10);
		karma_inc((char *)&(Party._justi), 10);
		karma_inc((char *)&(Party._sacri), 10);
		karma_inc((char *)&(Party._honor), 10);
		karma_inc((char *)&(Party._spiri), 10);
		karma_inc((char *)&(Party._humil), 10);
		sound(6,0); shakefx();
		Gra_09();
		sound(6,0); shakefx();
		Gra_09();
		sound(6,0); shakefx();
	}
	RST_MSK(Party.mItems, ST_SKULL);
}

struct {
	char *_00;
	void (*_02)();
} D_0434[] = {
	{/*D_03D5*/&AVATAR[0xF354 + 0x0005] /* "stone" */,  C_0311},
	{/*D_03DB*/&AVATAR[0xF354 + 0x0005] /* "stones" */, C_0311},
	{/*D_03E2*/&AVATAR[0xF69F + 0x0005] /* "bell" */,   C_0487},
	{/*D_03E7*/&AVATAR[0xF6A4 + 0x0005] /* "book" */,   C_04C0},
	{/*D_03EC*/&AVATAR[0xF6A9 + 0x0005] /* "candle" */, C_0501},
	{/*D_03F3*/&AVATAR[0xF309 + 0x0005] /* "key" */,    C_044C},
	{/*D_03F7*/&AVATAR[0xF6B4 + 0x0005] /* "keys" */,   C_044C},
	{/*D_03FC*/&AVATAR[0xF6B9 + 0x0005] /* "horn" */,   C_0553},
	{/*D_0401*/&AVATAR[0xF6BE + 0x0005] /* "wheel" */,  C_058C},
	{/*D_0407*/&AVATAR[0xF6C4 + 0x0005] /* "skull" */,  C_05CE},
	{/*D_040D*/"",       0}
};

/*C_07AE*/CMD_Use() {
	register int si;
	char bp_0e[12];

	u4_puts(&AVATAR[0xF6CB + 0x0005] /* "Use which item:\n" */);
	set_input_mode(INPUT_MODE_USE_WORD);
	u4_gets(bp_0e, 11);
	Gra_CR();
	for(si = 0; D_0434[si]._00[0]; si++) {
		if(_strnicmp(D_0434[si]._00, bp_0e, 12) == 0) {
			(*(D_0434[si]._02))();
			return 0;
		}
	}
	u4_puts(&AVATAR[0xF6DC + 0x0005] /* "Not a Usable item!\n" */);
}
