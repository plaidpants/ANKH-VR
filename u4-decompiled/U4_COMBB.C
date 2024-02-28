/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "U4.H"

/*----------------------------------------
Whirlpool and Twister management?
----------------------------------------*/

/*remove monster?*/
C_7821(si/*bp06*/, bp04)
register int si;
int bp04;
{
	register int di;

	for(di = 31; di >= 0; di --) {
		if(
			di != si &&
			D_8742._npc._tile[di] &&
			D_8742._npc._x[di] == D_8742._npc._x[si] &&
			D_8742._npc._y[di] == D_8742._npc._y[si]
		) {
			D_8742._npc._tile[di] = D_8742._npc._gtile[di] = 0;
			t_callback();
			sound(bp04,0);
		}
	}
}

/*whirlpool contact?*/
C_786F(si/*bp04*/)
register int si;
{
	if(Party._x == D_8742._npc._x[si] && Party._y == D_8742._npc._y[si]) {
		t_callback();
		hit_y = hit_x = 5;
		hit_tile = TIL_8C;
		C_36C7();
		Party._x = 0x7f;
		Party._y = 0x4e;
		sound(11,0);
		C_1584();
		Party._tile = TIL_10;
		hit_tile = 0;
		C_26B6();
	}
	C_7821(si, 11);
}

/*twister contact?*/
C_78D1(si/*bp04*/)
register int si;
{
	if(Party._x == D_8742._npc._x[si] && Party._y == D_8742._npc._y[si]) {
		t_callback();
		sound(12,0);
		C_1584();
		C_1584();
		C_1584();
		C_1584();
		hit_tile = 0;
	}
	C_7821(si, 12);
}

C_7918()
{
	register int si;

	if(CurMode != MOD_OUTDOORS)
		return 0;

	for(si = 3; si >= 0; si--) {
		if(D_8742._npc._tile[si] == TIL_8C)
			C_786F(si);
		else if(D_8742._npc._tile[si] == TIL_8E)
			C_78D1(si);
	}
}

/*----------------------------------------

----------------------------------------*/

/*C_794D*/w_SameExit()
{
	u4_puts(/*D_239E*/&AVATAR[0x11651 + 0x000f] /* "All must use same exit!\n" */);
	sound(1,0);
}

/*exit combat zone*/
C_7962()
{
	Combat._charaX[activeChara] = Combat._charaY[activeChara] = 0x80;
	Fighters._chtile[activeChara] = 0;
	sound(8,0);
	if(CurMode == MOD_COMBAT && !C_0AFE(D_96F8) && Party.chara[activeChara]._HP[0] == Party.chara[activeChara]._HP[1]) {
		karma_dec(&(Party._valor), 2);
		karma_dec(&(Party._sacri), 2);
	}
}

/*exiting fight*/
C_79C9(bp06, bp04)
unsigned char bp06;
unsigned char bp04;
{
	if (CurMode == MOD_COM_ROOM) {
		/*-- dungeon room --*/
		if (D_96EE != 0) {
			if (bp06 == D_96EE)
				goto C_7A0C;
			w_SameExit();
			return 0;
		}
		else if (D_96F4 != 0) {
			if (bp04 == D_96F4)
				goto C_7A0C;
			w_SameExit();
			return 0;
		}
		else if (bp06 > 10) {
			D_96EE = bp06;
		}
		else {
			D_96F4 = bp04;
		}
	}
C_7A0C:
	C_7962();
}

//TODO need to instrument these secret dungeon room tiles
/*room special tiles*/
C_7A15(bp06, bp04)
unsigned char bp06;
unsigned char bp04;
{
	register int /*si*/loc_A;
	int loc_B, loc_C, loc_D, loc_E, loc_AShift/*bp_04, bp_06*/;

	for(loc_A = 12; loc_A >= 0; loc_A = loc_A - 4) {
		if(D_95B2[loc_A]) {

			if (
				(bp06 << 12) == (*(U16 *)(D_95B2 + loc_A) & 0xf000) &&
				(bp04 <<  8) == (*(U16 *)(D_95B2 + loc_A) & 0x0f00)
				) {
				loc_AShift = 0;
				while (loc_AShift <= 8) {

					loc_B = (*(U16 *)(D_95B2 + loc_A + 2) >> loc_AShift) & 0xf;
					if (
						loc_B |
						(loc_C = (*(U16 *)(D_95B2 + loc_A + 2) >> (loc_AShift + 4)) & 0xf)
						)
					{
						/*DAEMON TRIGGER FIX*/
						if (U4_RND1(7) < 8 || D_95B2[loc_A] < TIL_80) {
							Combat_MAP(loc_B, loc_C) = D_95B2[loc_A];
						}
						else {
							loc_D = 15;
							while (loc_D >= 0 && Fighters._tile[loc_D]) {
								loc_D--;
							}
							if (loc_D != -1) {

								PrepFighters(loc_D, D_95B2[loc_A]);
								/*Fighters._tile[loc_D] = Fighters._gtile[loc_D] = D_95B2[loc_A];
								loc_E = D_23D2[C_7C25(D_95B2[loc_A])];
								Fighters._HP[loc_D] = (loc_E >> 1) | U4_RND4(loc_E);*/

								Combat._npcX[loc_D] = loc_C;
								Combat._npcY[loc_D] = loc_B;
							}
						}

					}
					loc_AShift = loc_AShift + 8;
				}
			}
		}
	}/* while(--loc_A >= 0);*/
}

/*move [fight]*/
C_7AE3(si/*bp08*/, bp06, bp04)
register int si;
unsigned char bp06;
unsigned char bp04;
{
	register unsigned char *di;

	sound(0,0);
	/*-- out of bounds=>exiting*/
	if((unsigned)bp06 > 10 || (unsigned)bp04 > 10) {
		C_79C9(bp06, bp04);
		return 0;
	}
	di = &(D_96F9[bp04*11 + bp06]);
	if(!C_2999(*di)) {
		w_Blocked();
		return 0;
	}
	if(C_29EF(*di)) {
		w_SlowProgress();
		return 0;
	}
	Combat._charaX[si] = bp06;
	Combat._charaY[si] = bp04;
	sound(0,0);
	if(CurMode == MOD_COM_ROOM)
		C_7A15(bp06, bp04);
}

/*North [combat]*/
C_7B89()
{
	u4_puts(/*D_23B7*/&AVATAR[0xFC17 + 0x0005] /* "North\n" */);
	C_7AE3(activeChara, Combat._charaX[activeChara], Combat._charaY[activeChara] - 1);
}

/*South [combat]*/
C_7BAF()
{
	u4_puts(/*D_23BE*/&AVATAR[0xFC38 + 0x0005] /* "South\n" */);
	C_7AE3(activeChara, Combat._charaX[activeChara], Combat._charaY[activeChara] + 1);
}

/*East [combat]*/
C_7BD5()
{
	u4_puts(/*D_23C5*/&AVATAR[0xFC75 + 0x0005] /* "East\n" */);
	C_7AE3(activeChara, Combat._charaX[activeChara] + 1, Combat._charaY[activeChara]);
}

/*West [combat]*/
C_7BFD()
{
	u4_puts(/*D_23CB*/&AVATAR[0xFC57 + 0x0005] /* "West\n" */);
	C_7AE3(activeChara, Combat._charaX[activeChara] - 1, Combat._charaY[activeChara]);
}
