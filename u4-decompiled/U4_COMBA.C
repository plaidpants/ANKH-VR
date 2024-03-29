/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "U4.H"

/*C_59D5*/COM_GetFighterId(_x, _y)
unsigned char _x;
unsigned char _y;
{
	register int si;

	for(si = 15; si >= 0; si --)
		if(Fighters._tile[si] && Combat._npcX[si] == _x && Combat._npcY[si] == _y)
			break;
	return si;
}

/*party all out ?*/
C_5A04()
{
	register int si;

	for(si = Party.f_1d8 - 1; si >= 0; si --)
		if(isCharaAlive(si))
			return 0;
	return 1;
}

/*C_5A28*/IsCombatEnded()
{
	register int si;

	for(si = 7; si >= 0; si --) {
		if(Fighters._chtile[si] != 0)
			break;
	}
	if(si == -1)
		return 1;

	if(CurMode == MOD_COM_ROOM) {
		goto C_5A5F;
	}

	for(si = 15; si >= 0; si--) {
		if(Fighters._tile[si] != 0) {
C_5A5F:
			return 0;
		}
	}
	return 1;
}

/*C_5D14();
C_61D1();*/

extern int QUIT;

/*C_5A6B*/COM_main()
{
	register unsigned si = 0; // ADDED
	unsigned bp_04;

	u_kbflush();
	bp_04 = 0;
	D_96EE = D_96F4 = 0;
	do {
		set_input_mode(INPUT_MODE_COMBAT);
		if (QUIT)
		{
			return 0;
		}
		for(activeChara = 0; /*C_5A88:*/activeChara < Party.f_1d8 && !IsCombatEnded(); activeChara++) {
/*C_5A9E*/
			if(Fighters._chtile[activeChara] && isCharaConscious(activeChara)) {
				D_95C8 = 4;
				Gra_11(activeChara);
				activeCharaX = Combat._charaX[activeChara];
				activeCharaY = Combat._charaY[activeChara];
				u4_puts(Party.chara[activeChara]._name);
				u4_puts(/*D_1FE6*/&AVATAR[0x11299 + 0x000f] /* " with " */);
				u4_puts(D_1E98[37 + Party.chara[activeChara]._weapon]);
				Gra_CR();
				u4_putc(0x10);
				D_95C8 = 2;
				C_3C54();
				set_input_mode(INPUT_MODE_COMBAT);
				u_delay(25, 1);
				si = u_kbhit()?u_kbread():KBD_SPACE;
				if(u4_isupper((unsigned char)si))
					si = (si & 0xff00) | u4_lower((unsigned char)si);
				switch(si) {
					case KBD_ESC: QUIT = 1;  return 0;
					case KBD_SPACE: w_Pass(); break;
					case 0x487e:
					case KBD_UP: C_7B89(); break;
					case 0x4b7c:
					case KBD_LEFT: C_7BFD(); break;
					case 0x5060:
					case KBD_DOWN: C_7BAF(); break;
					case 0xf400:
					case KBD_RIGHT: C_7BD5(); break;
					case KBD_a: C_61D1(); break;/*(A)ttack(combat)*/
					case KBD_c: CMD_Cast(); break;
					case KBD_g: C_73B8(); break;/*(G)et chest(combat)*/
					case KBD_r: CMD_Ready(); break;
					case KBD_u: CMD_Use(); break;
					case KBD_v: CMD_Volume(); break;
					case KBD_z: CMD_Ztats(); break;
					case KBD_b:
					case KBD_d:
					case KBD_e:
					case KBD_f:
					case KBD_h:
					case KBD_i:
					case KBD_j:
					case KBD_k:
					case KBD_l:
					case KBD_m:
					case KBD_n:
					case KBD_o:
					case KBD_p:
					case KBD_q:
					case KBD_s:
					case KBD_t:
					case KBD_w:
					case KBD_x:
					case KBD_y: w_NotHere(); break;
					case KBD_CTRL_S:
						if(bp_04 == KBD_ALT_Z) {
							C_1C21();
							break;
						}
					default:
						u4_puts(/*D_1FED*/&AVATAR[0x112A0 + 0x000f] /* "Bad command\n" */);
						sound(2,0);
						Gra_11(activeChara);
						activeChara --;
					continue;
				}
				Gra_11(activeChara);
				activeCharaX = -1;
				dspl_Stats();
			}
			if(!D_07F8) {
				D_07F8 = 1;
				activeChara --;
			} else {
				C_5D14();
			}
/*C_5C09*/
		}
/*C_5CEF:*/
		C_9F7B();
		bp_04 = si;
	} while(!IsCombatEnded());
	if(!C_5A04())
		C_837A();
	D_07F8 = 1;
}

C_5D14()
{
	if(Fighters._chtile[activeChara] == 0)
		return 0;
	if(isCharaConscious(activeChara))
		C_9209();
	if(!isCharaAlive(activeChara)) {
		Fighters._chtile[activeChara] = 0;
	} else if(isCharaConscious(activeChara) && spell_sta == 'Q' && U4_RND1(1)) {
		/*Quickness!*/
		activeChara --;
		return 0;
	} else if(Party.chara[activeChara]._status == 'S' && !U4_RND1(7)) {
		Party.chara[activeChara]._status = 'G';
		D_944A[activeChara] = Fighters._chtile[activeChara] = C_0ACF(activeChara);
		dspl_Stats();
	}
	food_dec(1);
}

/*C_5DAB*/COM_DoDamage(_npcId, _charaId, _damage)
register int _npcId;
int _charaId;
unsigned char _damage;
{
	u4_puts(C_1513(Fighters._tile[_npcId]));
	u4_putc(' ');
	add_char_to_text_buffer(' ');
	if(Fighters._tile[_npcId] != TIL_5E && (Fighters._HP[_npcId] -= _damage) < 0) {
		u4_puts(/*D_1FFA*/&AVATAR[0x112AD + 0x000f] /* "Killed!\n" */);
		if(_charaId != -1) {
			register int di;

			di = D_23D2[C_7C25(Fighters._tile[_npcId])] / 16 + 1;
			XP_inc(_charaId, di);
			u4_puts(/*D_2003*/&AVATAR[0x112B6 + 0x000f] /* "Exp. " */);
			u4_putl(di, 1, ' ');
			Gra_CR();
		}
		Fighters._tile[_npcId] = 0;
	} else {
		int loc_D, loc_C, loc_B, loc_A;

		loc_B = loc_A = D_23D2[C_7C25(Fighters._tile[_npcId])] / 2;/*50% HP*/
		loc_B >>= 1;/*25% HP*/
		loc_C = loc_B + loc_A;/*75% HP*/
		loc_D = Fighters._HP[_npcId];
		if(loc_D < 24) {
			u4_puts(/*D_2009*/&AVATAR[0x112BC + 0x000f] /* "Fleeing!\n" */);
		} else if(loc_D < loc_B) {
			u4_puts(/*D_2013*/&AVATAR[0x112C6 + 0x000f] /* "Critical!\n" */);
		} else {
			Gra_CR();
			if(loc_D < loc_A) {
				u4_puts(/*D_201E*/&AVATAR[0x112D1 + 0x000f] /* "Heavily " */);
			} else if(loc_D < loc_C) {
				u4_puts(/*D_2027*/&AVATAR[0x112DA + 0x000f] /* "Lightly " */);
			} else {
				u4_puts(/*D_2030*/&AVATAR[0x112E3 + 0x000f] /* "Barely " */);
			}
			u4_puts(/*D_2038*/&AVATAR[0x112EB + 0x000f] /* "Wounded!\n" */);
		}
	}
}

/*C_5EA2*/COM_CheckHitable(_dir_x, _dir_y)
unsigned char /*bp06*/_dir_x;
unsigned char /*bp04*/_dir_y;
{
	unsigned char loc_C, loc_A;
	register int loc_B;

	loc_C = _dir_x + hit_x;
	loc_A = _dir_y + hit_y;
	if(loc_C > 10 || loc_A > 10)
		return 0;
	loc_B = Combat_MAP(loc_A, loc_C);
	if(loc_B == TIL_49)
		return 0;
	if(loc_B <= TIL_80 && loc_B > TIL_02 && loc_B != TIL_48)
		if(loc_B < TIL_31 || loc_B > TIL_34)
			if(!C_2999(loc_B))
				return 0;

	hit_x = loc_C;
	hit_y = loc_A;
	return 1;
}

/*C_5F26*/DrawBlow(_range)
int /*bp04*/_range;
{
	int bp_04, bp_02;

	if(Party.chara[activeChara]._weapon != 11 || _range < 2)
		return 0;
	bp_02 = u4_sign(Combat._charaX[activeChara] - hit_x);
	bp_04 = u4_sign(Combat._charaY[activeChara] - hit_y);
	hit_tile = TIL_4D;
	while(_range) {
		COM_CheckHitable(bp_02, bp_04);
		C_3C54();
		_range --;
	}
	hit_tile = 0;
}

/*C_5F9D*/w_missed(_range)
int /*bp04*/_range;
{
	if(_range == 0)
		sound(4,0);
	hit_tile = 0;
	u4_puts(/*D_2042*/&AVATAR[0x112F5 + 0x000f] /* "Missed!\n" */);
	if(Party.chara[activeChara]._weapon == 9 && hit_x < 11 && hit_y < 11 && Combat_MAP(hit_y, hit_x) >= TIL_03)
		Combat_MAP(hit_y, hit_x) = TIL_46;
	C_3C54();
	DrawBlow(_range);
}

/*C_6012*/TryDamage(_npcId, _range)
int /*bp06*/_npcId;
int /*bp04*/_range;
{
	register char *loc_B;
	register struct tChara *loc_C;
	int loc_A;
	
	loc_C = &(Party.chara[activeChara]);
	if(Party._loc == 0x18 && loc_C->_weapon < 11) {
		/*abyss without good weapons*/
		w_missed(_range);
		return 0;
	}
	hit_tile = (loc_C->_weapon == 14)?TIL_4E:TIL_4F;
	/*dexterity test*/
	if(loc_C->_dex < 40 && U4_RND1(0xff) > loc_C->_dex + 0x80) {
		w_missed(_range);
		return 0;
	}
	/*success*/
	loc_B = &(Combat_MAP(Combat._npcY[_npcId], Combat._npcX[_npcId]));
	if(loc_C->_weapon == 9 && *loc_B >= TIL_03)
		*loc_B = TIL_46;
	/*damage points*/
	loc_A = loc_C->_str;
	loc_A += D_2450[loc_C->_weapon];
	loc_A = SafeModulo(U4_RND1(0xff), u4_min(loc_A, 0xff));
	C_3C54();
	sound(6,0);
	hit_tile = 0;
	COM_DoDamage(_npcId, activeChara, loc_A);
	DrawBlow(_range);
}

/*projectile weapon?*/
C_60F1(_dir_x, _dir_y)
int _dir_x;
int _dir_y;
{
	register struct tChara *loc_A;
	int loc_D = 0, loc_B = 0, loc_C = 0;

	loc_D = 11;
	loc_C = 0;
	loc_A = &(Party.chara[activeChara]);
	/*-- For OIL, ask range --*/
	if(loc_A->_weapon == 9) {
		set_input_mode(INPUT_MODE_NUMBER_INPUT_1_DIGITS);
		loc_D = AskLetter(/*D_204B*/&AVATAR[0x112FE + 0x000f] /* "Range:\x12\x12\b" */, '0', '9');
		loc_D -= '0';
		if(loc_D < 0)
			return 0;
	}
	/*-- if denumbrable weapon, decrement --*/
	if(loc_A->_weapon == 2 || loc_A->_weapon == 9) {
		if(Party._weapons[loc_A->_weapon] == 0) {
			loc_A->_weapon = 0;
			u4_puts(/*D_2055*/&AVATAR[0x11308 + 0x000f] /* "Last one!\n" */);
		} else {
			Party._weapons[loc_A->_weapon] --;
		}
	}
	/*-- --*/
	hit_tile = (loc_A->_weapon != 14)?TIL_4D:TIL_4E;
	sound(4,0);
	/*note:when using flaming oil with range '0'
	 this loop is never entered, and so loc_B stays
	 uninitialized. This may lead to unexpected behavior
	 on the original version, or exception on modern
	 systems.
	*/
	for(; loc_D; loc_D--) {
		if(!COM_CheckHitable(_dir_x, _dir_y)) {
			w_missed(loc_C);
			return 0;
		}
		loc_C ++;
		C_3C54();
		if((loc_B = COM_GetFighterId(hit_x, hit_y)) != -1)
			break;
	}
	hit_tile = 0;
	if(loc_B == -1) {
		w_missed(loc_C);
		return 0;
	}
	TryDamage(loc_B, loc_C);
}

/*(A)ttack [combat]*/
C_61D1()
{
	register struct tChara *loc_C;
	int loc_A, loc_B, loc_D;

	loc_C = &(Party.chara[activeChara]);
	set_input_mode(INPUT_MODE_GENERAL_DIRECTION);
	AskDir(/*D_2060*/&AVATAR[0x10AE0 + 0x0007] /* "Dir: " */, &loc_A, &loc_B);
	if(!(loc_A | loc_B))
		return 0;
	hit_x = Combat._charaX[activeChara];
	hit_y = Combat._charaY[activeChara];

	if(!D_2468[loc_C->_weapon]) {/*is not range weapon?*/
		if(loc_C->_weapon == 10) {/*Halberd*/
			hit_x += loc_A;
			hit_y += loc_B;
		}
		if(COM_CheckHitable(loc_A, loc_B)) {
			hit_tile = TIL_4D;
			C_3C54();
			hit_tile = 0;
			sound(4,0);

			if((loc_D = COM_GetFighterId(hit_x, hit_y)) != -1) {
				TryDamage(loc_D, 1);
				return 0;
			}
			if(loc_C->_weapon == 2) {/*dagger?*/
				C_60F1(loc_A, loc_B);
				return 0;
			}
		}
		w_missed(0);
	} else {
		C_60F1(loc_A, loc_B);
		return 0;
	}
}

/*C_628F*/CMD_Attack()
{
	register int loc_C;
	int loc_A, loc_B;

	if(CurMode <= MOD_BUILDING && Party.f_1dc != 0) {
		u4_puts(/*D_2066*/&AVATAR[0x11319 + 0x000f] /* "Attack\n" */);
		w_DriftOnly();
		return 0;
	}

	set_input_mode(INPUT_MODE_GENERAL_DIRECTION);
	AskDir(/*D_206E*/&AVATAR[0x11321 + 0x000f] /* "Attack: " */, &loc_A, &loc_B);
	if(!(loc_A|loc_B))
		return 0;
	if(
		(loc_C = C_0A8B(Party._x + loc_A, Party._y + loc_B)) == -1 ||
		D_8742._npc._tile[loc_C] == TIL_8C ||
		D_8742._npc._tile[loc_C] == TIL_02 ||
		D_8742._npc._tile[loc_C] == TIL_8E
	) {
		u4_puts(/*D_2077*/&AVATAR[0x1132A + 0x000f] /* "Nothing to Attack!\n" */);
		return 0;
	}
	if(CurMode == MOD_BUILDING) {
		register int di;
		for(di = 31; di >= 0; di --)
			if(D_8742._npc._tile[di] == TIL_50 || D_8742._npc._tile[di] == TIL_5E) {
				D_8742._npc._var[di] = 0xff;
				D_8742._npc._tlkidx[di] = 0;
			}
	}
	if(CurMode == MOD_BUILDING || C_0AFE(D_8742._npc._tile[loc_C])) {
		karma_dec(&(Party._compa), 5);
		karma_dec(&(Party._justi), 5);
		karma_dec(&(Party._honor), 5);
	}
	C_7DFE(loc_C);
}
