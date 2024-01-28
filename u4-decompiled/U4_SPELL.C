/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "U4.H"

/*spells m.p. cost*/
char* D_208C = &AVATAR[0x1133F + 0x200F]; //{
#if 0
	 5,/*Awaken*/
	15,/*Blink*/
	 5,/*Cure*/
	20,/*Dispel*/
	10,/*Energy*/
	15,/*Fireball*/
	40,/*Gate*/
	10,/*Heal*/
	20,/*Iceball*/
	30,/*Jinx*/
	25,/*Kill*/
	 5,/*Light*/
	 5,/*Magic Misl*/
	20,/*Negate*/
	 5,/*Open*/
	15,/*Protection*/
	20,/*Quickness*/
	45,/*Resurrect*/
	15,/*Sleep*/
	30,/*Tremor*/
	15,/*Undead*/
	15,/*View*/
	10,/*Winds*/
	15,/*X-it*/
	10,/*Y-up*/
	 5 /*Z-down*/
};
#endif

static int D_8CCC;/*current spell*/

/*isUndead?*/
C_636D(bp04)
int bp04;
{
	if(bp04 == TIL_9C || bp04 == TIL_BC || bp04 == TIL_C4 || bp04 == TIL_E4)
		return 1;
	return 0;
}

/*C_6399*/w_Failed()
{
	u4_puts(/*D_20A6*/&AVATAR[0x11359 + 0x200F] /* "Failed!\n" */);
	hit_tile = 0;
	sound(8,0);
}

/*try "cast"*/
/*
	.consume m.p
	.check 'N'egate
*/
C_63B4()
{
	Party.chara[activeChara]._MP -= D_208C[D_8CCC];
	sound(10, D_208C[D_8CCC]);
	Gra_09(); sound(9, D_8CCC+0x60); Gra_09();
	if(spell_sta == 'N') {
		w_Failed();
		return 0;
	}
	return 1;
}

/*assert outdoors only*/
C_6409()
{
	if(CurMode != MOD_OUTDOORS) {
		u4_puts(/*D_20AF*/&AVATAR[0x11362 + 0x200F] /* "Outdoors Only!\n" */);
		w_Failed();
		return 0;
	}
	return 1;
}

/*assert combat only*/
C_6428()
{
	if(CurMode < MOD_COMBAT) {
		u4_puts(/*D_20BF*/&AVATAR[0x11372 + 0x200F] /* "Combat Only!\n" */);
		w_Failed();
		return 0;
	}
	return 1;
}

/*assert dungeon only*/
C_6447()
{
	if(CurMode != MOD_DUNGEON) {
		u4_puts(/*D_20CD*/&AVATAR[0x11380 + 0x200F] /* "Dungeon Only!\n" */);
		w_Failed();
		return 0;
	}
	return 1;
}
 

/*magic projectile ?*/
C_6466(bp04)
int bp04;
{
	register int loc_C, loc_D;
	int loc_A, loc_B;

	if(!C_6428())
		return 0;
	set_input_mode(INPUT_MODE_GENERAL_DIRECTION);
	AskDir(/*D_20DC*/&AVATAR[0x10AE0 + 0x7] /* "Dir: " */, &loc_A, &loc_B);
	if(!(loc_A|loc_B))
		return 0;
	if(!C_63B4())
		return 0;
	hit_tile = bp04;
	hit_x = Combat._charaX[activeChara];
	hit_y = Combat._charaY[activeChara];
	loc_C = -1;
	do {
		if(!COM_CheckHitable(loc_A, loc_B))
			break;
		C_3C54();
	} while((loc_C = COM_GetFighterId(hit_x, hit_y)) == -1);
	if(loc_C == -1) {
		w_Failed();
		return 0;
	}
	sound(6,0);
	switch(hit_tile) {
		case TIL_4D: loc_D = U4_RND3(64) | 16; break;
		case TIL_4E: loc_D = U4_RND3(224) | 32; break;
		case TIL_4F: loc_D = U4_RND3(128) | 24; break;
		default: loc_D = 224 | 8;
	}
	hit_tile = 0;
	COM_DoDamage(loc_C, activeChara, loc_D);
}


/*C_6558*/SPL_Awaken()
{
	register int si;

	set_input_mode(INPUT_MODE_GENERAL_ASK_CHARACTER_NUMBER);
	si = AskChara(/*D_20E2*/&AVATAR[0x11395 + 0x200F] /* "Who:\x12\x12\b" */);
	if(si < 0)
		return 0;
	if(!C_63B4())
		return 0;
	if(Party.chara[si]._stat != 'S') {
		w_Failed();
		return 0;
	}
	Party.chara[si]._stat = 'G';
	if(CurMode >= MOD_COMBAT) {
		D_944A[si] = Fighters._chtile[si] = C_0ACF(si);
	}
}

/*C_65AA*/SPL_Blink()
{
	int loc_A = 0, loc_B = 0;
	unsigned char loc_C = 0, loc_D = 0;

	if(Party._tile >= TIL_14 && Party._tile != TIL_18) {
		if(!C_6409())
			return 0;
		set_input_mode(INPUT_MODE_GENERAL_DIRECTION);
		AskDir(/*D_20EA*/&AVATAR[0x10AE0 + 0x7] /* "Dir: " */, &loc_A, &loc_B);
		if(!(loc_A|loc_B))
			return 0;
		if(!C_63B4())
			return 0;
		if((Party._x & Party._y) < 0xc0) {
			loc_C = D_959C.x;
			loc_D = D_959C.y;
			while(loc_C < 32 && loc_D < 32) {
				loc_C += loc_A;
				loc_D += loc_B;
			}
			do {
				loc_C -= loc_A;
				loc_D -= loc_B;
			} while((loc_C != D_959C.x || loc_D != D_959C.y) && !C_2999(D_8742._map.x32x32[loc_D][loc_C]));
			if(D_959C.x != loc_C || D_959C.y != loc_D) {
				Party._x += loc_C - D_959C.x;
				Party._y += loc_D - D_959C.y;
				C_26B6();
				return 0;
			}
		}
	}
	w_Failed();
}

/*C_669B*/SPL_Cure()
{
	register int si;

	set_input_mode(INPUT_MODE_GENERAL_ASK_CHARACTER_NUMBER);
	if((si = AskChara(/*D_20F0*/&AVATAR[0x11395 + 0x200F] /* "Who:\x12\x12\b" */)) < 0)
		return 0;
	if(!C_63B4())
		return 0;
	if(Party.chara[si]._stat != 'P') {
		w_Failed();
		return 0;
	}
	Party.chara[si]._stat = 'G';
}

/*C_66DA*/SPL_Dispel()
{
	int loc_A, loc_B;
	unsigned char *loc_C;

	if(CurMode == MOD_DUNGEON) {
		if(!C_63B4())
			return 0;
		loc_C = &(D_8742._map.x8x8x8[Party._z][DNG_Y_p(Party._y, Party._dir)][DNG_X_p(Party._x, Party._dir)]);
		if((*loc_C & 0xf0) != 0xa0) {
			w_Failed();
			return 0;
		}
		*loc_C = 0;
		return 0;
	}
	if(CurMode <= MOD_BUILDING && Party.f_1dc) {
		w_Failed();
		return 0;
	}
	set_input_mode(INPUT_MODE_GENERAL_DIRECTION);
	AskDir(/*D_20F8*/&AVATAR[0x10AE0 + 0x7] /* "Dir: " */, &loc_A, &loc_B);
	if(!(loc_A|loc_B))
		return 0;
	if(!C_63B4())
		return 0;
	if(CurMode >= MOD_COMBAT) {
		if(Combat._charaY[activeChara] + loc_B >= 11 || Combat._charaX[activeChara] + loc_A >= 11) {
			w_Failed();
			return 0;
		}
		loc_C = &(Combat_MAP(Combat._charaY[activeChara]+loc_B, Combat._charaX[activeChara]+loc_A));
		if(*loc_C < TIL_44 || *loc_C > TIL_47) {
			w_Failed();
			return 0;
		}
		*loc_C = Combat_MAP(Combat._charaY[activeChara], Combat._charaX[activeChara]);
		return 0;
	}
	if(CurMode == MOD_OUTDOORS) {
		loc_C = &(D_8742._map.x32x32[D_959C.y+loc_B][D_959C.x+loc_A]);
	} else {
		if(Party._y + loc_B >= 32 || Party._x + loc_A >= 32) {
			w_Failed();
			return 0;
		}
		loc_C = &(D_8742._map.x32x32[Party._y + loc_B][Party._x + loc_A]);
	}
	if(*loc_C < TIL_44 || *loc_C > TIL_47) {
		w_Failed();
		return 0;
	}
	*loc_C = tile_cur;
}

/*C_6882*/SPL_Energy()
{
	unsigned char *loc_D;
	int loc_B;
	int loc_A, loc_C, loc_E;

	u4_puts(/*D_20FE*/&AVATAR[0x113B1 + 0x200F] /* "Energy type? " */);
	set_input_mode(INPUT_MODE_ENERGY_TYPE_POISON_FIRE_LIGHTNING_SLEEP);
	loc_B = (unsigned char)u_kbread();
/*bad hack:I must force the redefinition of u4_islower
int order to get the compiler to provide
the original bytecode*/
#undef u4_islower
#define u4_islower(c) ((c) >= (int)'a' && (c) <= (int)'z')
	u4_toupper(loc_B);
	switch(loc_B) {
		case 'P'/*oison*/:    loc_E = TIL_44; break;
		case 'L'/*ightning*/: loc_E = TIL_45; break;
		case 'F'/*ire*/:      loc_E = TIL_46; break;
		case 'S'/*leep*/:     loc_E = TIL_47; break;
		default: loc_E = -1;
	}
	if(loc_E == -1) {
		w_Failed();
		return 0;
	}
	u4_putc(loc_B);
	add_char_to_text_buffer(loc_B);
	Gra_CR();
	add_char_to_text_buffer('\n');
	if(CurMode == MOD_DUNGEON) {
		if(!C_63B4())
			return 0;
		loc_D = &(D_8742._map.x8x8x8[Party._z][DNG_Y_p(Party._y, Party._dir)][DNG_X_p(Party._x, Party._dir)]);
		if(*loc_D)
			w_Failed();
		*loc_D = (loc_E&3) | 0xa0;
		return 0;
	}
	if(CurMode >= MOD_COMBAT) {
		set_input_mode(INPUT_MODE_GENERAL_DIRECTION);
		AskDir(/*D_210C*/&AVATAR[0x10AE0 + 0x7] /* "Dir: " */, &loc_A, &loc_C);
		if(!(loc_A|loc_C))
			return 0;
		if(!C_63B4())
			return 0;
		if(Combat._charaY[activeChara] + loc_C < 11 && Combat._charaX[activeChara] + loc_A < 11) {
			if(C_2999(*(loc_D = &(Combat_MAP(loc_C + Combat._charaY[activeChara], loc_A + Combat._charaX[activeChara]))))) {
				*loc_D = loc_E;
				return 0;
			}
		}
	}
	w_Failed();
}

/*C_69D7*/SPL_Fireball() {
	C_6466(TIL_4F);
}

/*C_69E5*/SPL_Gate()
{
	register int si;

	if(Party._tile < TIL_14 || Party._tile == TIL_18) {
		w_Failed();
		return 0;
	}
	if(!C_6409())
		return 0;
	set_input_mode(INPUT_MODE_GENERAL_ASK_PHASE);
	si = AskLetter(/*D_2112*/&AVATAR[0x113C5 + 0x200F] /* "To Phase:\x12\x12\b" */, '0', '8');
	if(si < 0 || si == (int)'0')
		return 0;
	if(!C_63B4())
		return 0;
	si -= '1';
	Party._x = D_0814[si];
	Party._y = D_081C[si];
	C_26B6();
}

/*C_6A40*/SPL_Heal()
{
	register int si;

	set_input_mode(INPUT_MODE_GENERAL_ASK_CHARACTER_NUMBER);
	if((si = AskChara(/*D_211F*/&AVATAR[0x113D2 + 0x200F] /* "Who?\x12\x12\b" */)) < 0)
		return 0;
	if(!C_63B4())
		return 0;
	if(!isCharaAlive(si)) {
		w_Failed();
		return 0;
	}
	HP_inc(si, U4_RND3(25) + 75);
}

/*C_6A82*/SPL_Iceball()
{
	C_6466(TIL_4E);
}

/*C_6A90*/SPL_Jinx()
{
	if(!C_63B4())
		return 0;
	spell_sta = 'J';
	spell_cnt = 10;
}

/*C_6AA9*/SPL_Kill()
{
	C_6466(TIL_8C);
}

/*C_6AB7*/SPL_Light()
{
	if(!C_6447())
		return 0;
	if(!C_63B4())
		return 0;
	Party.f_1dc += 100;
	if(CurMode == MOD_DUNGEON)
		C_AE41();
}

/*C_6ADB*/SPL_MagicMisl()
{
	C_6466(TIL_4D);
}
  
/*C_6AE9*/SPL_Negate()
{
	if(!C_63B4())
		return 0;
	spell_sta = 'N';
	spell_cnt = 10;
}

/*C_6B02*/SPL_Open()
{
	if(!C_63B4())
		return 0;
	if(CurMode == MOD_OUTDOORS && Party._tile == TIL_18) {
		w_Failed();
		return 0;
	}
	if(CurMode >= MOD_COMBAT) {
		C_7337();
	} else {
		C_722F(-1);/*100% chance evasion*/
	}
}

/*C_6B36*/SPL_Protection()
{
	if(!C_63B4())
		return 0;
	spell_sta = 'P';
	spell_cnt = 10;
}

/*C_6B4F*/SPL_Quickness()
{
	if(!C_63B4())
		return 0;
	spell_sta = 'Q';
	spell_cnt = 10;
}

/*C_6B68*/SPL_Resurrect()
{
	register int si;

	if(CurMode >= MOD_COMBAT) {
		w_Failed();
		return 0;
	}
	set_input_mode(INPUT_MODE_GENERAL_ASK_CHARACTER_NUMBER);
	si = AskChara(/*D_2127*/&AVATAR[0x11395 + 0x200F] /* "Who:\x12\x12\b" */);
	if(si < 0)
		return 0;
	if(!C_63B4())
		return 0;
	if(Party.chara[si]._stat != 'D') {
		w_Failed();
		return 0;
	}
	Party.chara[si]._stat = 'G';
}

/*C_6BAE*/SPL_Sleep()
{
	register int si;

	if(!C_6428())
		return 0;
	if(!C_63B4())
		return 0;
	for(si = 15; si >= 0; si--)
		if(Fighters._tile[si] && C_636D(Fighters._tile[si]) == 0 && Fighters._tile[si] != TIL_FC && Fighters._HP[si] < u_rand_a())
			Fighters._sleeping[si] = 1;
}

/*C_6BF8*/SPL_Tremor()
{
	register int si;

	if(!C_6428())
		return 0;
	if(!C_63B4())
		return 0;
	shakefx();
	hit_tile = TIL_4F;
	for(si = 15; si >= 0; si --) {
		if(Fighters._tile[si] != 0 && Fighters._HP[si] < 192) {
			if(U4_RND1(1)) {
				if(U4_RND1(1))
					Fighters._HP[si] = 23;
				continue;
			}
			hit_x = Combat._npcX[si];
			hit_y = Combat._npcY[si];
			C_3C54();
			sound(6,0);
			COM_DoDamage(si, activeChara, 0xff);
		}
	}
	hit_tile = 0;
}

/*C_6C71*/SPL_Undead()
{
	register int si;

	if(!C_6428())
		return 0;
	if(!C_63B4())
		return 0;
	for(si = 15; si >= 0; si--)
		if(C_636D(Fighters._tile[si]) && U4_RND1(1) && Fighters._HP[si] > 23)
			Fighters._HP[si] = 23;
}

/*C_6CB2*/SPL_View()
{
	if(!C_63B4())
		return 0;
	C_C403();
}


/*C_6CC3*/SPL_Winds()
{
	int bp_02, bp_04;

	if(!C_6409())
		return 0;
	set_input_mode(INPUT_MODE_GENERAL_DIRECTION);
	AskDir(/*D_212F*/&AVATAR[0x113E2 + 0x200F] /* "From Dir: " */, &bp_02, &bp_04);
	if(!(bp_02|bp_04))
		return 0;
	if(!C_63B4())
		return 0;
	if(bp_02) {
		if(bp_02 == 1)
			WindDir = DIR_E;
		else
			WindDir = DIR_W;
	} else {
		if(bp_04 == 1)
			WindDir = DIR_S;
		else
			WindDir = DIR_N;
	}
}

/*C_6D22*/SPL_X_it()
{
	if(!C_6447())
		return 0;
	if(!C_63B4())
		return 0;
	Party._z = -1;
}

/*C_6D3D*/SPL_Y_up()
{
	int loc_A;
	unsigned char loc_B, loc_C;

	if(!C_6447())
		return 0;
	if(!C_63B4())
		return 0;
	if(Party._loc == 0x18) {
		w_Failed();
		return 0;
	}
	if(--Party._z < 0)
		return 0;
	for(loc_A = 32; loc_A > 0; loc_A --) {
		loc_B = U4_RND1(7);
		loc_C = U4_RND1(7);
		if(D_8742._map.x8x8x8[Party._z][loc_C][loc_B] == 0)
			break;
	}
	if(loc_A == 0) {
		Party._z ++;
		w_Failed();
		return 0;
	}
	Party._x = loc_B;
	Party._y = loc_C;
}

/*C_6DC1*/SPL_Z_down()
{
	int loc_A;
	unsigned char loc_B, loc_C;

	if(!C_6447())
		return 0;
	if(!C_63B4())
		return 0;
	if(Party._loc  == 0x18 || Party._z == 7) {
		w_Failed();
		return 0;
	}
	Party._z ++;
	for(loc_A = 32; loc_A > 0; loc_A --) {
		loc_B = U4_RND1(7);
		loc_C = U4_RND1(7);
		if(D_8742._map.x8x8x8[Party._z][loc_C][loc_B] == 0)
			break;
	}
	if(loc_A == 0) {
		Party._z --;
		w_Failed();
		return 0;
	}
	Party._x = loc_B;
	Party._y = loc_C;
}

typedef spell_handler(void);
typedef spell_handler *pSpell_handler;

pSpell_handler D_216E[] = {
	SPL_Awaken,
	SPL_Blink,
	SPL_Cure,
	SPL_Dispel,
	SPL_Energy,
	SPL_Fireball,
	SPL_Gate,
	SPL_Heal,
	SPL_Iceball,
	SPL_Jinx,
	SPL_Kill,
	SPL_Light,
	SPL_MagicMisl,
	SPL_Negate,
	SPL_Open,
	SPL_Protection,
	SPL_Quickness,
	SPL_Resurrect,
	SPL_Sleep,
	SPL_Tremor,
	SPL_Undead,
	SPL_View,
	SPL_Winds,
	SPL_X_it,
	SPL_Y_up,
	SPL_Z_down
};

/*C_6E4A*/CMD_Cast()
{
	u4_puts(/*D_213A*/&AVATAR[0x113ED + 0x200F] /* "Cast Spell!\n" */);
	if(CurMode < MOD_COMBAT) {
		set_input_mode(INPUT_MODE_GENERAL_ASK_CHARACTER_NUMBER);
		if((activeChara = AskChara(/*D_2147*/&AVATAR[0x113FA + 0x200F] /* "Player:\x12\x12\b" */)) < 0)
			return 0;
		if(!isCharaConscious(activeChara)) {
			w_Disabled();
			return 0;
		}
	}
	Gra_13();
	C_4C42();
	txt_Y = 23;
	txt_X = 24;
	set_input_mode(INPUT_MODE_GENERAL_ASK_SPELL);
	D_8CCC = AskLetter(/*D_2152*/&AVATAR[0x11405 + 0x200F] /* "Spell:\x12\x12\b" */, 'A', 'Z');
	Gra_13();
	C_4649();
	dspl_Stats();
	if(CurMode >= MOD_COMBAT)
		Gra_11(activeChara);
	if(D_8CCC < 0)
		return 0;
	D_8CCC -= 'A';
	u4_puts(D_1E98[101 + D_8CCC]); u4_puts(/*D_215C*/&AVATAR[0xF376 + 0x5] /* "!\n" */);
	/*-- check mixture --*/
	if(Party._mixtures[D_8CCC] == 0) {
		w_NoneLeft();
		return 0;
	}
	Party._mixtures[D_8CCC] --;
	/*-- check m.p --*/
	if(Party.chara[activeChara]._MP < D_208C[D_8CCC]) {
		u4_puts(/*D_215F*/&AVATAR[0x11412 + 0x200F] /* "M.P. too low!\n" */);
		w_Failed();
		return 0;
	}
	/*-- cast spell --*/
	(*D_216E[D_8CCC])();
	dspl_Stats();
}
