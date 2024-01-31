/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "U4.H"

/*C_891E();
C_895F();
C_899F();
C_89BD();
C_89DB();
C_8A1F();
C_87E2();*/

extern int QUIT;

/*C_84D2*/DNG_main()
{
	register unsigned si = 0;
	int bp_04;

	D_9446 = 1;
	u_kbflush();
	bp_04 = 0;
	do {
		set_input_mode(INPUT_MODE_DUNGEON_INPUT);
		if (QUIT)
		{
			return 0;
		}
		C_9209();
		u4_putc(0x10);
		t_callback();
		if(C_10FD()) {
			set_input_mode(INPUT_MODE_DUNGEON_INPUT);
			u_delay(25, 1);
			si = u_kbhit()?u_kbread():KBD_SPACE;
			if(u4_isupper((unsigned char)si))
				si = (si & 0xff00) | u4_lower((unsigned char)si);
			switch(si) {
				case KBD_ESC: QUIT = 1;  return 0;
				case KBD_SPACE: w_Pass(); break;
				case KBD_UP:
				case 0x487e: C_891E(); break;
				case KBD_LEFT:
				case 0x4b7c: C_899F(); break;
				case KBD_DOWN:
				case 0x5060: C_895F(); break;
				case KBD_RIGHT:
				case 0xf400: C_89BD(); break;
				case KBD_c: CMD_Cast(); break;
				case KBD_d: C_8A1F(); break;/*(D)escend [dungeon]*/
				case KBD_g: CMD_Get(); break;
				case KBD_h: CMD_HoleUp(); break;
				case KBD_i: CMD_Ignite(); break;
				case KBD_k: C_89DB(); break;/*(K)limb [dungeon]*/
				case KBD_m: CMD_Mix(); break;
				case KBD_n: CMD_NewOrder(); break;
				case KBD_p: CMD_Peer(); break;
				case KBD_q: CMD_Quit(); break;
				case KBD_r: CMD_Ready(); break;
				case KBD_s: C_B9B2(); break;/*(S)earch [dungeon]*/
				case KBD_u: CMD_Use(); break;
				case KBD_v: CMD_Volume(); break;
				case KBD_w: CMD_Wear(); break;
				case KBD_z: CMD_Ztats(); break;
				case KBD_a:
				case KBD_b: 
				case KBD_e:
				case KBD_f:
				case KBD_j:
				case KBD_l:
				case KBD_o:
				case KBD_t:
				case KBD_x:
				case KBD_y: w_NotHere(); sound(2,0); break;
				case KBD_CTRL_S:
					if(bp_04 == KBD_ALT_Z) {
						C_1C21();
						break;
					}
				default:
					u4_puts(/*D_2626*/&AVATAR[0x118D1 + 0x2017] /* "Bad command\n" */);
					sound(2,0);
					D_07F8 = 0;
			}
			C_AE41();
		} else {
			u4_puts(/*D_2633*/&AVATAR[0x118DE + 0x2017] /* "Zzzzz\n" */);
		}
		if(D_07F8 != 0 && Party._z >= 0)
			C_87E2();
		else
			D_07F8 = 1;
		bp_04 = si;
	} while(Party._z >= 0);
	C_2747();
	//dclose(File_DNG);
	//File_DNG = 0;
}

C_8743()
{
	register int si;

	if(
		(tile_cur & 0x0f) == 0 ||
		(tile_cur & 0xf0) == 0x80 ||
		(tile_cur & 0xf0) == 0x90 ||
		(tile_cur & 0xf0) == 0xA0 ||
		(tile_cur & 0xf0) == 0xD0 ||
		(tile_cur & 0xf0) == 0xF0
	) return 0;
	D_96F8 = D_9452 = TIL_90 + (tile_cur & 0x0f) * 4 - 4;
	D_8742._map.x8x8x8[Party._z][Party._y][Party._x] &= 0xf0;
	for(si = 0x1f; si >= 0; si --) {
		if(
			D_8742._npc._var[si] == Party._z &&
			D_8742._npc._x[si] == Party._x &&
			D_8742._npc._y[si] == Party._y
		) {
			D_8742._npc._var[si] =
			D_8742._npc._tile[si] =
			D_8742._npc._x[si] =
			D_8742._npc._y[si] = 0;
			break;
		}
	}
	C_7D50();

	return 1;
}

C_87E2()
{
	register int si;
	register struct tChara *di;

	C_10FD();
	for(di = &(Party.chara[0]), si = 0; si < Party.f_1d8; di ++, si++) {
		if(di->_status == 'S' && U4_RND1(7) == 0) {
			di->_status = 'G';
		} else if(di->_status == 'P') {
			hitChara(si, 2);
			Gra_11(si);
			sound(6,0);
			Gra_11(si);
		}
	}
	if(food_dec(Party.f_1d8)) {
		u4_puts(/*D_263A*/&AVATAR[0x118E5 + 0x2017] /* "\nStarving!!!\n" */); 
		for(si = 0; si < Party.f_1d8; si++) {
			if(isCharaAlive(si))
				hitChara(si, 2);
		}
		for(si = Party.f_1d8; --si >= 0; )
			Gra_11(si);
		sound(6,0);
		for(si = Party.f_1d8; --si >= 0; )
			Gra_11(si);
	}
	MP_recover();
	C_9414();
	C_95AA();
	if(Party.f_1dc)
		Party.f_1dc --;
	Party._moves ++;
	if(spell_cnt) {
		if(--spell_cnt == 0)
			spell_sta = 7;
	}
	if(Party.f_1dc == 0)
		u4_puts(/*D_2648*/&AVATAR[0x118F3 + 0x2017] /* "It's Dark!\n" */);
	dspl_Stats();
	if(C_8743() == 0) {
		while((tile_cur & 0xf0) == 0xd0) {
			C_10FD();/*check death*/
			C_7FFD();/*enter dungeon room*/
			C_B677();/*render 3d*/
		}
	}
}

C_88E8(bp06, bp04)
unsigned char bp06;
unsigned bp04;
{
	if(bp04 == 0) {
		if(
			(bp06 & 0xf0) == 0xc0 ||
			(bp06 & 0xf0) == 0xe0 ||
			(bp06 & 0xf0) == 0xd0
		) return 1;
	}
	if(bp06 != 0xa1 && bp06 < 0xc0)
		return 1;
	return 0;
}

/*Advance*/
C_891E()
{
	u4_puts(/*D_2654*/&AVATAR[0x118FF + 0x2017] /* "Advance\n" */);
	if(!C_88E8(tile_forward, 0)) {
		w_Blocked();
		return 0;
	}
	Party._x = DNG_X_p(Party._x, Party._dir);
	Party._y = DNG_Y_p(Party._y, Party._dir);
}

/*Retreat*/
C_895F()
{
	u4_puts(/*D_265D*/&AVATAR[0x11908 + 0x2017] /* "Retreat\n" */);
	if(!C_88E8(tile_back, 1)) {
		w_Blocked();
		return 0;
	}
	Party._x = DNG_X_m(Party._x, Party._dir);
	Party._y = DNG_Y_m(Party._y, Party._dir);
}

/*Turn Left*/
C_899F()
{
	u4_puts(/*D_2666*/&AVATAR[0x11911 + 0x2017] /* "Turn Left\n" */);
	Party._dir = (Party._dir - 1) & 3;
	D_07F8 = 0;
}

/*Turn Right*/
C_89BD()
{
	u4_puts(/*D_2671*/&AVATAR[0x1191C + 0x2017] /* "Turn Right\n" */);
	Party._dir = (Party._dir + 1) & 3;
	D_07F8 = 0;
}

/*(K)limb [dungeon]*/
C_89DB()
{
	u4_puts(/*D_267D*/&AVATAR[0x11928 + 0x2017] /* "Klimb " */);
	if(
		(tile_cur & 0xf0) == 0x10 ||
		(tile_cur & 0xf0) == 0x30
	) {
		u4_puts(/*D_2684*/&AVATAR[0x1192F + 0x2017] /* "up!\n" */);
		if(--Party._z >= 0) {
			u4_puts(/*D_2689*/&AVATAR[0x11934 + 0x2017] /* "To level " */);
			u4_putc(Party._z + '1');
			add_char_to_text_buffer(Party._z + '1');
			Gra_CR();
			add_char_to_text_buffer('\n');
		}
	} else {
		w_What();
	}
}

/*(D)escend [dungeon]*/
C_8A1F()
{
	u4_puts(/*D_2693*/&AVATAR[0x1193E + 0x2017] /* "Descend " */);
	if(
		(tile_cur & 0xf0) == 0x20 ||
		(tile_cur & 0xf0) == 0x30
	) {
		Party._z ++;
		u4_puts(/*D_269C*/&AVATAR[0x11947 + 0x2017] /* "down to level " */);
		u4_putc(Party._z + '1');
		add_char_to_text_buffer(Party._z + '1');
		Gra_CR();
		add_char_to_text_buffer('\n');
	} else {
		w_What();
	}
}
