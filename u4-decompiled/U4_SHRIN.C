/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "U4.H"

#include <string.h>
#include <stdlib.h>

/*---- mantras*/
char *D_8322[] = {
	/*D_7B62*/&AVATAR[0x16DD4 + 0x0037] /* "ahm" */,
	/*D_7B66*/&AVATAR[0x16DD8 + 0x0037] /* "mu" */,
	/*D_7B69*/&AVATAR[0x16DDB + 0x0037] /* "ra" */,
	/*D_7B6C*/&AVATAR[0x16DDE + 0x0037] /* "beh" */,
	/*D_7B70*/&AVATAR[0x16DE2 + 0x0037] /* "cah" */,
	/*D_7B74*/&AVATAR[0x16DE6 + 0x0037] /* "summ" */,
	/*D_7B79*/&AVATAR[0x16DEB + 0x0037] /* "om" */,
	/*D_7B7C*/&AVATAR[0x16DEE + 0x0037] /* "lum" */
};
/*---- visions*/
char *D_8332[][3] = {
	{
		/*D_7B80*/&AVATAR[0x16DF2 + 0x0037] /* "Take not the gold of others found in towns and castles for yours it is not!\n" */,
		/*D_7BCD*/&AVATAR[0x16E3F + 0x0037] /* "Cheat not the merchants and peddlers for tis an evil thing to do!\n" */,
		/*D_7C10*/&AVATAR[0x16E82 + 0x0037] /* "Second, read the Book of Truth at the entrance to the Great Stygian Abyss!\n" */,
	}, {
		/*D_7C5C*/&AVATAR[0x16ECE + 0x0037] /* "Kill not the non-evil beasts of the land, and do not attack the fair people!\n" */,
		/*D_7CAA*/&AVATAR[0x16F1C + 0x0037] /* "Give of thy purse to those who beg and thy deed shall not be forgotten!\n" */,
		/*D_7CF3*/&AVATAR[0x16F65 + 0x0037] /* "Third, light the Candle of Love at the entrance to the Great Stygian Abyss!\n" */,
	}, {
		/*D_7D40*/&AVATAR[0x16FB2 + 0x0037] /* "Victories scored over evil creatures help to build a valorous soul!\n" */,
		/*D_7D85*/&AVATAR[0x16FF7 + 0x0037] /* "To flee from battle with less than grievous wounds often shows a coward!\n" */,
		/*D_7DCF*/&AVATAR[0x17041 + 0x0037] /* "First, ring the Bell of Courage at the entrance to the Great Stygian Abyss!\n" */,
	}, {
		/*D_7E1C*/&AVATAR[0x1708E + 0x0037] /* "To take the gold of others is injustice not soon forgotten. Take only thy due!\n" */,
		/*D_7E6C*/&AVATAR[0x170DE + 0x0037] /* "Attack not a peaceful citizen for that action deserves strict punishment!\n" */,
		/*D_7EB7*/&AVATAR[0x17129 + 0x0037] /* "Kill not a non-evil beast for they deserve not death, even if in hunger they attack thee!" */,
	}, {
		/*D_7F11*/&AVATAR[0x17183 + 0x0037] /* "To give thy last gold piece unto the needy shows good measure of self-sacrifice!\n" */,
		/*D_7F63*/&AVATAR[0x171D5 + 0x0037] /* "For thee to flee and leave thy companions is a self-serving action to be avoided!\n" */,
		/*D_7FB6*/&AVATAR[0x17228 + 0x0037] /* "To give of thy life's blood so that others may live is a virtue of great praise!\n" */,
	}, {
		/*D_8008*/&AVATAR[0x1727A + 0x0037] /* "Take not the gold of others for this shall bring dishonor upon thee!\n" */,
		/*D_804E*/&AVATAR[0x172C0 + 0x0037] /* "To strike first a non-evil being is by no means an honorable deed!\n" */,
		/*D_8092*/&AVATAR[0x17304 + 0x0037] /* "Seek ye to solve the many Quests before thee, and honor shall be a reward!\n" */,
	}, {
		/*D_80DE*/&AVATAR[0x17350 + 0x0037] /* "Seek ye to know thyself.  Visit the seer often for he can see into thy inner being!\n" */,
		/*D_8133*/&AVATAR[0x173A5 + 0x0037] /* "Meditation leads to enlightenment Seek ye all Wisdom and Knowledge!\n" */,
		/*D_8178*/&AVATAR[0x173EA + 0x0037] /* "If thou dost seek the White Stone, search ye not under the ground, but in Serpent's Spine!\n" */,
	}, {
		/*D_81D4*/&AVATAR[0x17446 + 0x0037] /* "Claim not to be that which thou art not.  Humble actions speak well of thee!\n" */,
		/*D_8222*/&AVATAR[0x17494 + 0x0037] /* "Strive not to wield the Great Force of Evil for its power will overcome thee!\n" */,
		/*D_8271*/&AVATAR[0x174E3 + 0x0037] /* "If thou dost seek the Black Stone, search ye at the Time and Place of the Gate on the darkest of all nights!\n" */
	}
};
/*---- visions 2*/
char *D_8362[] = {
	/*D_82DF*/&AVATAR[0x17551 + 0x0037] /* "rune_1" */,/*I*/
	/*D_82E6*/&AVATAR[0x17558 + 0x0037] /* "rune_2" */,/*N*/
	/*D_82ED*/&AVATAR[0x1755F + 0x0037] /* "rune_0" */,/*F*/
	/*D_82F4*/&AVATAR[0x17551 + 0x0037] /* "rune_1" */,/*I*/
	/*D_82FB*/&AVATAR[0x17558 + 0x0037] /* "rune_2" */,/*N*/
	/*D_8302*/&AVATAR[0x17551 + 0x0037] /* "rune_1" */,/*I*/
	/*D_8309*/&AVATAR[0x1757B + 0x0037] /* "rune_3" */,/*T*/
	/*D_8310*/&AVATAR[0x17582 + 0x0037] /* "rune_4" */ /*Y*/
};

/*display avatarhood vision I.N.F.I.N.I.T.Y*/
C_E6DF(bp04)
int bp04;
{
	char bp_14[20];

	CurMode = MOD_VISION;
	strcpy(bp_14, D_8362[bp04]);
	strcat(bp_14, (D_943A == 1)?/*D_8317*/&AVATAR[0x17589 + 0x0037] /* ".pic" */:/*D_831C*/&AVATAR[0xFB70 + 0x0005] /* ".ega" */);
	Gra_10();
	Gra_16(bp_14, 0, 0);
}

/*enter a shrine*/
C_E72C()
{
	int loc_A, loc_B, loc_C;
	char loc_D[16];
	int loc_E;

	loc_A = Party._loc - 0x19;/*shrine "number"*/
	if(!TST_MSK(Party.mRunes, loc_A)) {
		u4_puts(/*D_8372*/&AVATAR[0x175E4 + 0x0037] /* "\nThou dost not bear the rune of entry!  A strange force keeps you out!\n" */);
C_E755:
		CurMode = MOD_OUTDOORS;
		Party._loc = 0;
		u_kbflush();
		return 0;
	}
	if(Load(/*D_83BA*/&AVATAR[0x1762C + 0x0037] /* "SHRINE.CON" */, 11 * 11, Combat._map) == -1)
		exit(3);
	CurMode = MOD_SHRINE;
	u4_puts(/*D_83C5*/&AVATAR[0x17637 + 0x0037] /* "\nYou enter the ancient shrine and sit before the altar...\nUpon what virtue dost thou meditate?\n" */);
	set_input_mode(INPUT_MODE_VIRTUE_WORD);
	u4_gets(loc_D, 15);
	Gra_CR();
	u4_puts(/*D_8425*/&AVATAR[0x17697 + 0x0037] /* "\nFor how many\n" */);
	set_input_mode(INPUT_MODE_GENERAL_ASK_NUMBER_OF_BEDS);
	loc_E = AskLetter(/*D_8434*/&AVATAR[0x176A6 + 0x0037] /* "Cycles (0-3)?\x12\x12\b" */, '0', '3');
	if(loc_E < 0) {
		goto C_E755;
	}
	loc_E -= '0';
	if(loc_E == 0 || _strnicmp(loc_D, D_1E98[151 + loc_A], 16)) {
		u4_puts(/*D_8445*/&AVATAR[0x176B7 + 0x0037] /* "\nThou art unable to focus thy thoughts on this subject!\n" */);
		goto C_E755;
	}
	if(Party._moves / 100 == Party.f_1ea) {
		u4_puts(/*D_847E*/&AVATAR[0x176F0 + 0x0037] /* "\nThy mind is still weary from thy last Meditation!\n" */);
		goto C_E755;
	}
	Party.f_1ea = Party._moves / 100;
	u4_puts(/*D_84B2*/&AVATAR[0x17724 + 0x0037] /* "Begin Meditation\n" */);
	for(loc_B = 0; loc_B < loc_E; loc_B ++) {
		for(loc_C = 0; loc_C < 0x10; loc_C ++) {
			set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
			u_delay(1, 0);
			u4_putc('.');
			add_char_to_text_buffer('.');
		}
		u_kbflush();
		u4_putc('\n');
		add_char_to_text_buffer('\n');
		u4_puts(/*D_84C4*/&AVATAR[0x17736 + 0x0037] /* "\nMantra: " */);
		set_input_mode(INPUT_MODE_MANTRA_WORD);
		u4_gets(loc_D, 15);
		Gra_CR();
		add_char_to_text_buffer('\n');
		if(_strnicmp(loc_D, D_8322[loc_A], 0x10)) {
			u4_puts(/*D_84CE*/&AVATAR[0x17740 + 0x0037] /* "\nThou art not able to focus thy thoughts with that Mantra!\n" */);
			karma_dec(&(Party._spiri), 3);
			goto C_E755;
		}
	}
	if(loc_E != 3 || *(pKarmas[loc_A]) != 99) {
		u4_puts(/*D_850A*/&AVATAR[0x1777C + 0x0037] /* "\nThy thoughts are pure. Thou art granted a vision!\n" */);
		karma_inc(&(Party._spiri), loc_E * 3);
		u_kbflush();
		set_input_mode(INPUT_MODE_GENERAL_CONTINUE);
		u_kbread();
		Gra_CR();
		u4_puts(D_8332[loc_A][loc_E - 1]);
	} else {
		u4_puts(/*D_853E*/&AVATAR[0x177B0 + 0x0037] /* "\nThou hast achieved partial Avatarhood in the Virtue of\n" */);
		u4_puts(D_1E98[151 + loc_A]);
		Gra_09(); sound(9, 0xff); Gra_09();
		Gra_CR();
		*(pKarmas[loc_A]) = 0;
		dspl_Stats();
		u_kbflush();
		set_input_mode(INPUT_MODE_GENERAL_CONTINUE);
		u_kbread();
		u4_puts(/*D_8577*/&AVATAR[0x177E9 + 0x0037] /* "\n\nThou art granted a vision!\n" */);
		C_E6DF(loc_A);
		u_kbflush();
	}
	set_input_mode(INPUT_MODE_GENERAL_CONTINUE);
	u_kbread();
	Gra_CR();
	goto C_E755;
}
