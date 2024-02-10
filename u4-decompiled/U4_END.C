/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "U4.H"

#include <string.h>

char *D_0BDA[] = {
	/*D_09BE*/&AVATAR[0xFC7B + 0x0005] /* "What dost thou possess if all may rely upon your every word?" */,
	/*D_09FB*/&AVATAR[0xFCB8 + 0x0005] /* "What quality compels one to share in the journeys of others?" */,
	/*D_0A38*/&AVATAR[0xFCF5 + 0x0005] /* "What answers when great deeds are called for?" */,
	/*D_0A66*/&AVATAR[0xFD23 + 0x0005] /* "What should be the same for Lord and Serf alike?" */,
	/*D_0A97*/&AVATAR[0xFD54 + 0x0005] /* "What is loath to place the self above aught else?" */,
	/*D_0AC9*/&AVATAR[0xFD86 + 0x0005] /* "What shirks no duty?" */,
	/*D_0ADE*/&AVATAR[0xFD9B + 0x0005] /* "What, in knowing the true self, knows all?" */,
	/*D_0B09*/&AVATAR[0xFDC6 + 0x0005] /* "What is that which Serfs are born with but Nobles must strive to obtain?" */,
	/*D_0B52*/&AVATAR[0xFE0F + 0x0005] /* "If all else is imaginary, this is real..." */,
	/*D_0B7C*/&AVATAR[0xFE39 + 0x0005] /* "What plunges to the depths, while soaring on the heights?" */,
	/*D_0BB6*/&AVATAR[0xFE73 + 0x0005] /* "What turns not away from any peril?" */
};

C_2F7E()
{
	set_input_mode(INPUT_MODE_DELAY_CONTINUE);
	u_delay(15, 1);
	if(u_kbhit())
		u_kbread();
}

unsigned char * D_0BF0 = &AVATAR[0xFEAD + 0x0005] /*{0xE7,0x53,0x23,0x3B,0x9E,0x69,0x17,0xBA,0xD8,0x1D,0x91,0x59,0xE9}*/;
unsigned char * D_0BFE = &AVATAR[0xFEBB + 0x0005] /*{0x88,0x69,0xDD,0x2C,0x15,0xB7,0x81,0xAC,0x6A,0x30,0xF3,0x6A,0xE9}*/;

static unsigned D_8CCA;

/*return to surface*/
C_2F9D(bp04)
unsigned bp04;
{
	set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
	u_delay(5, 0);
	Gra_CR();
	Party._x = D_0BF0[bp04];
	Party._y = D_0BFE[bp04];
	Party.f_1d8 = D_8CCA;
	Party._loc = 0;
	CurMode = MOD_OUTDOORS;
	spell_cnt = 0;
	hit_tile = 0;
	spell_sta = 7;
	D_9440 = 1;
	D_95C6 = 0;
	Party.f_1dc = 0;
	C_26B6();
	dspl_Stats();
	u_kbflush();
	longjmp(D_9458, -1);
}

/*passage is not granted*/
C_3010()
{
	u4_puts(/*D_0C0B*/&AVATAR[0xFEC8 + 0x0005] /* "\nPassage is not granted.\n" */);
	C_2F9D(12);
}

/*Victory !*/
C_3025()
{
	set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
	u_delay(2, 0);
	shakefx();
	shakefx();
	shakefx();
	shakefx();
	set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
	u_delay(3, 0);
	dfree(pShapes);
	Gra_17();/*infinity symbol*/
	set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
	u_delay(3, 0);
	u4_puts(/*D_0C25*/&AVATAR[0xFEE2 + 0x0005] /* "\n\nThe boundless knowledge of the Codex of Ultimate Wisdom is revealed unto thee." */);
	C_2F7E();
	u4_puts(/*D_0C76*/&AVATAR[0xFF33 + 0x0005] /* "\n\nThe voice says: Thou hast proven thyself to be truly good in nature." */);
	C_2F7E();
	u4_puts(/*D_0CBD*/&AVATAR[0xFF7A + 0x0005] /* "\n\nThou must know that thy quest to become an Avatar is the endless quest of a lifetime." */);
	C_2F7E();
	u4_puts(/*D_0D15*/&AVATAR[0xFFD2 + 0x0005] /* "\n\nAvatarhood is a living gift.  It must always and forever be nurtured to flourish." */);
	C_2F7E();
	u4_puts(/*D_0D69*/&AVATAR[0x10026 + 0x0005] /* "\n\nFor if thou dost stray from the paths of virtue, thy way may be lost forever." */);
	C_2F7E();
	u4_puts(/*D_0DB9*/&AVATAR[0x10076 + 0x0005] /* "\n\nReturn now unto thine own world. Live there as an example to thy people, as our memory of thy gallant deeds serves us." */);
	C_2F7E();
	Gra_10();
	u4_puts(/*D_0E32*/&AVATAR[0x100EF + 0x0005] /* "\n\nAs the sound of the voice trails off, darkness seems to rise around you. There is a moment of intense, wrenching vertigo." */);
	C_2F7E();
	Gra_16((D_943A == 1)?/*D_0EAE*/&AVATAR[0x1016B + 0x0005] /* "stoncrcl.pic" */:/*D_0EBB*/&AVATAR[0x10178 + 0x0005] /* "stoncrcl.ega" */, 0, 0);
	u4_puts(/*D_0EC8*/&AVATAR[0x10185 + 0x0005] /* "\n\nYou open your eyes to a familiar circle of stones.  You wonder of your recent adventures." */);
	C_2F7E();
	u4_puts(/*D_0F24*/&AVATAR[0x101E1 + 0x0005] /* "\n\nIt seems a time and place very distant.  You wonder if it really happened. Then you realize that in your hand you hold The Ankh." */);
	C_2F7E();
	u4_puts(/*D_0FA7*/&AVATAR[0x10264 + 0x0005] /* "\n\nYou walk away from the circle, knowing that you can always return from whence you came, since you now know the secret of the gates." */);
	C_2F7E();
	u4_puts(/*D_102D*/&AVATAR[0x102EA + 0x0005] /* "\n\nCONGRATULATIONS!\n   Thou hast\n   completed\n   ULTIMA IV\n  Quest of the\n    AVATAR\n  in " */);
	u4_putl(Party._moves, 0, '0');
	u4_puts(/*D_1087*/&AVATAR[0x10344 + 0x0005] /* "\n turns! Report\n thy feat unto\nLord British at\nOrigin Systems!" */);
	while (1)
	{
		set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
		u_delay(2, 0); // TODO need to fix this forever loop
	}
}

C_310F(bp06, bp04)
char *bp06;
char *bp04;
{
	char bp_10[16];

	u4_puts(bp06);
	Gra_CR();
	Gra_CR();
	set_input_mode(INPUT_MODE_END_GAME_WORD);
	u4_gets(bp_10, 15);
	Gra_CR();
	if(_strnicmp(bp_10, bp04, 15) != 0) {
		set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
		u_delay(1, 0);
		u4_puts(/*D_10C6*/&AVATAR[0x10383 + 0x0005] /* "\nThy thoughts are not pure.\nI ask again.\n" */);
		set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
		u_delay(2, 0);

		u4_puts(bp06);
		Gra_CR();
		Gra_CR();
		set_input_mode(INPUT_MODE_END_GAME_WORD);
		u4_gets(bp_10, 15);
		Gra_CR();
		if(_strnicmp(bp_10, bp04, 15) != 0) {
			set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
			u_delay(1, 0);
			u4_puts(/*D_10F0*/&AVATAR[0x10383 + 0x0005] /* "\nThy thoughts are not pure.\nI ask again.\n" */);
			set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
			u_delay(2, 0);

			u4_puts(bp06);
			Gra_CR();
			Gra_CR();
			set_input_mode(INPUT_MODE_END_GAME_WORD);
			u4_gets(bp_10, 15);
			Gra_CR();
			if(_strnicmp(bp_10, bp04, 15) != 0)
				return 0;
		}
	}
	return 1;
}

char *D_161A[] = {
	/*D_111A*/&AVATAR[0x103D7 + 0x0005] /* "truth" */,
	/*D_1120*/&AVATAR[0x103DD + 0x0005] /* "love" */,
	/*D_1125*/&AVATAR[0x103E2 + 0x0005] /* "courage" */
};

char *D_1620[] = {
	/*D_112D*/&AVATAR[0x103EA + 0x0005] /* "honesty" */,
	/*D_1135*/&AVATAR[0x103F2 + 0x0005] /* "compassn" */,
	/*D_113E*/&AVATAR[0x103FB + 0x0005] /* "valor" */,
	/*D_1144*/&AVATAR[0x10401 + 0x0005] /* "justice" */,
	/*D_114C*/&AVATAR[0x10409 + 0x0005] /* "sacrific" */,
	/*D_1155*/&AVATAR[0x10412 + 0x0005] /* "honor" */,
	/*D_115B*/&AVATAR[0x10418 + 0x0005] /* "spirit" */,
	/*D_1162*/&AVATAR[0x1041F + 0x0005] /* "humility" */
};

/*last phase of game ?*/
C_31F4()
{
	int bp_02;
	char bp_12[16];

	D_8CCA = Party.f_1d8;
	Party.f_1d8 = 1;
	CurMode = MOD_VISION;
	Gra_13();
	dspl_Stats();
	Gra_10();
	u4_puts(/*D_116B*/&AVATAR[0x10428 + 0x0005] /* "\n\n\nThere is a sudden darkness, and you find yourself alone in an empty chamber.\n" */);
	set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
	u_delay(4, 0);
	Gra_16((D_943A == 1)?/*D_11BC*/&AVATAR[0x10479 + 0x0005] /* "key7.pic" */:/*D_11C5*/&AVATAR[0x10482 + 0x0005] /* "key7.ega" */, 0, 0);
	if(
		!TST_MSK(Party.mItems, ST_KEY_C) |
		!TST_MSK(Party.mItems, ST_KEY_T) |
		!TST_MSK(Party.mItems, ST_KEY_L)
	) {
		u4_puts(/*D_11CE*/&AVATAR[0x1048B + 0x0005] /* "\nThou dost not have the Key of Three Parts.\n" */);
		C_2F9D(12);
	}
	u4_puts(/*D_11FB*/&AVATAR[0x104B8 + 0x0005] /* "\nYou use your Key of Three Parts.\n" */);
	set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
	u_delay(3, 0);
	u4_puts(/*D_121E*/&AVATAR[0x104DB + 0x0005] /* "\nA voice rings out:\n" */);
	if(!C_310F(/*D_123D*/&AVATAR[0x104FA + 0x0005] /* "\"What is the Word of Passage?\"" */, /*D_1233*/&AVATAR[0x104F0 + 0x0005] /* "veramocor" */))
		C_3010();
#if 1 // Disable to skip end game checks
	if(D_8CCA != 8) {
		u4_puts(/*D_125C*/&AVATAR[0x10519 + 0x0005] /* "\nThou hast not proved thy leadership in all eight virtues.\n" */);
		set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
		u_delay(8, 0);
		C_3010();
	}
	for(bp_02 = 7; bp_02 >= 0; bp_02 --) {
		if(*(pKarmas[bp_02])) {
			u4_puts(/*D_1298*/&AVATAR[0x10555 + 0x0005] /* "\nThou art not ready.\n" */);
			C_3010();
		}
	}
#endif
	u4_puts(/*D_12AE*/&AVATAR[0x1056B + 0x0005] /* "\nPassage is granted.\n" */);
	set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
	u_delay(5, 0);
	Gra_10();
	for(bp_02 = 0; bp_02 < 11; bp_02 ++) {
		char bp_26[20];

		set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
		u_delay(2, 0);
		u4_puts(/*D_12C4*/&AVATAR[0x10581 + 0x0005] /* "\n\nThe voice asks:\n\n" */);
		set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
		u_delay(2, 0);
		if(!C_310F(D_0BDA[bp_02], (bp_02<=7)?D_1E98[151 + bp_02]:D_161A[bp_02-8])) {
			set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
			u_delay(1, 0);
			u4_puts(/*D_12D8*/&AVATAR[0x10595 + 0x0005] /* "\nThy quest is not yet complete.\n" */);
			C_2F9D(bp_02);
		}
		strcpy(bp_26, (bp_02 >= 8)?D_161A[bp_02-8]:D_1620[bp_02]);
		strcat(bp_26, (D_943A == 1)?/*D_12F9*/&AVATAR[0xFB66 + 0x0005] /* ".pic" */:/*D_12FE*/&AVATAR[0xFB70 + 0x0005] /* ".ega" */);
		Gra_16(bp_26, 0, 0);
		if(bp_02 == 7) {
			set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
			u_delay(3, 0);
			u4_puts(/*D_1303*/&AVATAR[0x105C0 + 0x0005] /* "\nThou art well versed in the virtues of the Avatar.\n" */);
			set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
			u_delay(5, 0);
		}
	}
	u4_puts(/*D_1338*/&AVATAR[0x105F5 + 0x0005] /* "\n\nThe floor rumbles beneath your feet.\n" */);
	shakefx();
	shakefx();
	set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
	u_delay(5, 0);
	u4_puts(/*D_1360*/&AVATAR[0x1061D + 0x0005] /* "\nAbove the din, the voice asks:\n\nIf all eight virtues of the Avatar combine into and are derived from the Three Principles of Truth, Love and Courage..." */);
	C_2F7E();
	u4_puts(/*D_13F9*/&AVATAR[0x106B6 + 0x0005] /* "\n\nThen what is the one thing which encompasses and is the whole of all undeniable Truth, unending Love, and unyielding Courage?\n\n" */);
	set_input_mode(INPUT_MODE_END_GAME_WORD);
	u4_gets(bp_12, 15);
	Gra_CR();
	if(_strnicmp(bp_12, /*D_147B*/&AVATAR[0x10738 + 0x0005] /* "infinity" */, 16) != 0) {
		u4_puts(/*D_1484*/&AVATAR[0x10383 + 0x0005] /* "\nThy thoughts are not pure.\nI ask again.\n" */);
		set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
		u_delay(3, 0);
		u4_puts(/*D_14AE*/&AVATAR[0x1076B + 0x0005] /* "\n\nWhat is the one thing which encompasses and is the whole of all undeniable Truth, unending Love, and unyielding Courage?\n\n" */);
		set_input_mode(INPUT_MODE_END_GAME_WORD);
		u4_gets(bp_12, 15);
		Gra_CR();
		if(_strnicmp(bp_12, /*D_152B*/&AVATAR[0x10738 + 0x0005] /* "infinity" */, 16) != 0) {
			u4_puts(/*D_1534*/&AVATAR[0x10383 + 0x0005] /* "\nThy thoughts are not pure.\nI ask again.\n" */);
			set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
			u_delay(3, 0);
			u4_puts(/*D_155E*/&AVATAR[0x1076B + 0x0005] /* "\n\nWhat is the one thing which encompasses and is the whole of all undeniable Truth, unending Love, and unyielding Courage?\n\n" */);
			set_input_mode(INPUT_MODE_END_GAME_WORD);
			u4_gets(bp_12, 15);
			Gra_CR();
			if(_strnicmp(bp_12, /*D_15DB*/&AVATAR[0x10738 + 0x0005] /* "infinity" */, 16) != 0) {
				set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
				u_delay(1, 0);
				u4_puts(/*D_15E4*/&AVATAR[0x108A1 + 0x0005] /* "\nThou dost not know the true nature of the Universe.\n" */);
				C_2F9D(11);
			}
		}
	}
	C_3025();
}
