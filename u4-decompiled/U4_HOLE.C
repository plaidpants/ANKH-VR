/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "U4.H"

#include <stdlib.h>

unsigned char * D_26B8 = &AVATAR[0x11963 + 0x2017] /*{TIL_C0,TIL_C4,TIL_C8,TIL_CC,TIL_B4,TIL_A0,TIL_A4,TIL_DC}*/;

C_8A5A()
{
	register int si;

	u4_puts(/*D_26AC*/&AVATAR[0x11957 + 0x2017] /* "Ambushed!\n" */);
	D_9772 = Party._x;
	D_9140 = Party._y;
	D_96F8 = D_9452 = D_26B8[U4_RND1(7)];
	for(si = Party.f_1d8; --si >= 0; ) {
		if(Party.chara[si]._status == 'G')
			Party.chara[si]._status = 'S';
	}
	dspl_Stats();
	C_7FD7();
}

/*C_8AB0*/CMD_HoleUp() {
	register int si;

	u4_puts(/*D_26C0*/&AVATAR[0x1196B + 0x2017] /* "Hole up & Camp\n" */);
	if(CurMode != MOD_DUNGEON && Party._loc != 0) {
		w_NotHere();
		return 0;
	}
	if(Party._tile != TIL_1F) {
		w_OnlyOnFoot();
		return 0;
	}
#ifdef WIN32
	if(Load(CurMode == MOD_DUNGEON?/*D_26D0*/&AVATAR[0x1197B + 0x2017] /* "CAMP.DNG" */:/*D_26D9*/&AVATAR[0x117BB + 0x200F] /* "CAMP.CON" */, sizeof(struct tCombat), &Combat) == -1)
#else
	if (Load(CurMode == MOD_DUNGEON ?/*D_26D0*/&AVATAR[0x1197B + 0x2017] /* "CAMP.DNG" */ :/*D_26D9*/&AVATAR[0x117BB + 0x200F] /* "CAMP.CON" */, sizeof(struct tCombat), &Combat) == -1)
//	if(Load(CurMode == MOD_DUNGEON?/*D_26D0*/&AVATAR[0x1197B + 0x2017] /* "CAMP.DNG" */:/*D_26D9*/&AVATAR[0x117BB + 0x200F] /* "CAMP.CON" */, (char *)&Fighters - (char *)&Combat, &Combat) == -1)
#endif
		exit(3);
	for (si = 15; si >= 0; si--)  // TODO, this originally cleared 32 which is an overrun
	{
		Fighters._tile[si] = 0;
		Fighters._gtile[si] = 0;
	}
	for(si = Party.f_1d8; --si >= 0; ) {
		D_944A[si] = Fighters._chtile[si] = isCharaAlive(si)?TIL_38:0;
	}
	dspl_Stats();
	u4_puts(/*D_26E2*/&AVATAR[0x1198D + 0x2017] /* "Resting...\n" */);
	D_95CC = CurMode;
	CurMode = MOD_COM_CAMP;
	set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
	u_delay(10, 0);
	if(U4_RND1(7) == 0) {
		C_8A5A();
	} else if(Party._moves / 100 == Party.f_1e6) {
		u4_puts(/*D_26EE*/&AVATAR[0x11999 + 0x2017] /* "No effect.\n" */);
	} else {
		Party.f_1e6 = Party._moves / 100;
		for(si = Party.f_1d8; --si >= 0; ) {
			if(isCharaAlive(si)) {
				if(Party.chara[si]._status == 'S')
					Party.chara[si]._status = 'G';
				HP_inc(si, U4_RND1(119));
				HP_inc(si, 99);
				Party.chara[si]._MP = 99;
			}
		}
		MP_recover();
		u4_puts(/*D_26FA*/&AVATAR[0x119A5 + 0x2017] /* "Players Healed!\n" */);
	}
	dspl_Stats();
	CurMode = D_95CC;
	if(CurMode == MOD_DUNGEON)
		D_9446 = 1;
}
