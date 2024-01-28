/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "U4.H"

C_919A()
{
	register int si;

	for(si = Party.f_1d8; --si >= 0; ) {
		if(isCharaConscious(si) && U4_RND1(3) == 0) {
			Party.chara[si]._stat = 'S';
			C_09D9(si);
			dspl_Stats();
		}
	}
}

C_91D1()
{
	register int si;

	for(si = Party.f_1d8; --si >= 0; ) {
		if(Party.chara[si]._stat == 'G' && U4_RND1(7) == 0) {
			Party.chara[si]._stat = 'P';
			C_09D9(si);
			dspl_Stats();
		}
	}
}

/*randomEvent*/
C_9209()
{
	unsigned bp_02;

	if(CurMode == MOD_OUTDOORS && tile_cur == TIL_17 && U4_RND1(7) == 0) {	// TODO: add not flying in the balloon
		u4_puts(/*D_2998*/&AVATAR[0x11C43 + 0x2017] /* "\nBridge Trolls!\n" */);
		D_96F8 = TIL_A4;
		D_9772 = Party._x;
		D_9140 = Party._y;
		D_946C = TIL_17;
		C_7DBC();
		return 0;
	}
	if(CurMode <= MOD_BUILDING && Party.f_1dc != 0)
		return 0;
	if(CurMode >= MOD_COMBAT) {
		if(Fighters._chtile[activeChara] == 0)
			return 0;
		switch(Combat_MAP(Combat._charaY[activeChara], Combat._charaX[activeChara])) {
			case TIL_03: case TIL_44:
				if(Party.chara[activeChara]._stat == 'G') {
					Party.chara[activeChara]._stat = 'P';
					C_09D9(activeChara);
					dspl_Stats();
				}
			break;
			case TIL_47:
				if(isCharaConscious(activeChara)) {
					Party.chara[activeChara]._stat = 'S';
					Fighters._chtile[activeChara] = TIL_38;
					C_09D9(activeChara);
					dspl_Stats();
				}
			break;
			case TIL_46: case TIL_4C:
				C_09D9(activeChara);
				bp_02 = U4_RND3(32) + 16;
				hitChara(activeChara, bp_02);
			break;
		}
		return 0;
	}
	if(CurMode == MOD_DUNGEON) {
		if((tile_cur & 0xf0) == 0xa0) {
			switch(tile_cur & 3) {
				case 0: C_91D1(); break;
				case 2: C_1584(); break;
				case 3: C_919A(); break;
			}
		} else if((tile_cur & 0xf0) == 0x80) {
			if(tile_cur == 0x80) {
				u4_puts(/*D_29A9*/&AVATAR[0x11C54 + 0x2017] /* "\nWinds!\n" */);
				Party.f_1dc = 0;
			} else {
				u4_puts((tile_cur < 0x88)?/*D_29B2*/&AVATAR[0x11C5D + 0x2017] /* "\nFalling Rocks!\n" */:/*D_29C3*/ &AVATAR[0x11C6E + 0x2017] /* "\nPit!\n" */);
				C_1584();
			}
		}
		return 0;
	}
	switch(tile_cur) {
		case TIL_03: case TIL_44: C_91D1(); break;
		case TIL_46: case TIL_4C: C_1584(); break;
		case TIL_47: C_919A(); break;
	}
}
