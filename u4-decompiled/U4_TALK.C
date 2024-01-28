/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "U4.H"

#include <string.h>
#include <android/log.h>

typedef tHandler_tlk(void);
typedef tHandler_tlk *pHandler_tlk;

TLK_name();
TLK_look();
TLK_job();
TLK_health();
TLK_special1();
TLK_special2();
TLK_join();
TLK_give();

unsigned D_8CE6;/*type? no npc index*/

char * D_2A7A = &AVATAR[0x11D25 + 0x2017] /* "Funny, no response!\n" */;

struct {
	char *_00;
	pHandler_tlk _02;
} D_2A90[] = {
	{/*D_2A3E*/&AVATAR[0x11CE9 + 0x2017] /* "bye" */,    0},
	{/*D_2A42*/&AVATAR[0x11CED + 0x2017] /* "name" */,   TLK_name},
	{/*D_2A47*/&AVATAR[0x11CF2 + 0x2017] /* "look" */,   TLK_look},
	{/*D_2A4C*/&AVATAR[0x11CF7 + 0x2017] /* "job" */,    TLK_job},
	{/*D_2A50*/&AVATAR[0x11CFB + 0x2017] /* "health" */, TLK_health},
	{0,                  TLK_special1},
	{0,                  TLK_special2},
	{/*D_2A57*/&AVATAR[0x11D02 + 0x2017] /* "join" */,   TLK_join},
	{/*D_2A5C*/&AVATAR[0x11D07 + 0x2017] /* "give" */,   TLK_give},
	{/*D_2A61*/"",       0}
};

/*D_8CCE:
	0.name
	1.pronoun
	2.look
	3.job
	4.health
	5.personnal answer1
	6.personnal answer2
	7.special question
	8.special answer Y
	9.special answer N
	10.personnal question1
	11.personnal question2*/
static char *D_8CCE[12];

/*special question*/
C_A163()
{
	char bp_04[4];

	Gra_CR();
	set_input_mode(INPUT_MODE_GENERAL_CONTINUE);
	u_kbread();
	u4_puts(D_8CCE[7]);
	u4_puts("\n");
	add_npc_talk(D_8CE6, D_8CCE[7]);
	u4_puts(/*D_2A62*/&AVATAR[0x11D0D + 0x2017] /* "\n\nYou say: " */);
	do {
		set_input_mode(INPUT_MODE_GENERAL_YES_NO_WORD);
		u4_gets(bp_04, 4);
		Gra_CR();
		if(bp_04[0] == 0)
			break;
		u4_toupper2(bp_04[0]);
		if(bp_04[0] != 'N' && bp_04[0] != 'Y')
			u4_puts(/*D_2A6E*/&AVATAR[0x11D19 + 0x2017] /* "Yes or no!\n" */);
	} while(bp_04[0] != 'N' && bp_04[0] != 'Y');
	if(bp_04[0] == 0)
		return 0;
	add_npc_talk(D_8CE6, "\n");
	if(bp_04[0] == 'Y') {
		if(D_95CE[1])
			karma_dec(&(Party._humil), 5);
		add_npc_talk(D_8CE6, D_8CCE[8]);
		u4_puts(D_8CCE[8]);
	} else {
		if(D_95CE[1] && (Party._moves >> 4) != Party.f_1ec)
			karma_inc(&(Party._humil), 10);
		add_npc_talk(D_8CE6, D_8CCE[9]);
		u4_puts(D_8CCE[9]);
	}
	Party.f_1ec = (Party._moves >> 4);
	Gra_CR();
	add_npc_talk(D_8CE6, "\n");
}

C_A22D(bp06, bp04)
int bp06;
char *bp04;
{
	if(!bp04)
		return 0;
	u4_puts(bp04);
	Gra_CR();
}

/*C_A245*/TLK_special1()
{
	C_A22D(6, D_8CCE[5]);
	add_npc_talk(D_8CE6, D_8CCE[5]);
	add_npc_talk(D_8CE6, "\n");
}

/*C_A253*/TLK_special2()
{
	C_A22D(7, D_8CCE[6]);
	add_npc_talk(D_8CE6, D_8CCE[6]);
	add_npc_talk(D_8CE6, "\n");
}

/*C_A261*/TLK_name()
{
	u4_puts(D_8CCE[1]);
	u4_puts(/*D_2AB8*/&AVATAR[0x11D63 + 0x2017] /* " says: I am " */);
	u4_puts(D_8CCE[0]);
	Gra_CR();
	add_npc_talk(D_8CE6, &AVATAR[0x11D6A + 0x2017] /* "I am " */);
	add_npc_talk(D_8CE6, D_8CCE[0]);
	add_npc_talk(D_8CE6, "\n");
}

/*C_A280*/TLK_look()
{
	u4_puts(/*D_2AC5*/&AVATAR[0x11D70 + 0x2017] /* "You see " */);
	C_A22D(3, D_8CCE[2]);
}

/*C_A299*/TLK_job()
{
	C_A22D(4, D_8CCE[3]);	
	add_npc_talk(D_8CE6, D_8CCE[3]);
	add_npc_talk(D_8CE6, "\n");
}

/*C_A2AB*/TLK_health()
{
	C_A22D(5, D_8CCE[4]);
	add_npc_talk(D_8CE6, D_8CCE[4]);
	add_npc_talk(D_8CE6, "\n");
}

char *D_2BB2[] = {
	/*D_2ACE*/&AVATAR[0x11D79 + 0x2017] /* "honest" */,
	/*D_2AD5*/&AVATAR[0x11D80 + 0x2017] /* "compassionate" */,
	/*D_2AE3*/&AVATAR[0x11D8E + 0x2017] /* "valiant" */,
	/*D_2AEB*/&AVATAR[0x11D96 + 0x2017] /* "just" */,
	/*D_2AF0*/&AVATAR[0x11D9B + 0x2017] /* "sacrificial" */,
	/*D_2AFC*/&AVATAR[0x11DA7 + 0x2017] /* "honorable" */,
	/*D_2B06*/&AVATAR[0x11DB1 + 0x2017] /* "spiritual" */,
	/*D_2B10*/&AVATAR[0x11DBB + 0x2017] /* "humble" */
};

/*C_A2BD*/TLK_join()
{
	int bp_02;

	if(
		(unsigned)D_8742._npc._tlkidx[D_8CE6] != 1 ||
		(unsigned)(Party._loc - 0x05) >= 8 ||
		(unsigned)(Party._loc - 0x05) == Party.chara[0]._class
	) {
		u4_puts(D_8CCE[1]);
		u4_puts(/*D_2B17*/&AVATAR[0x11DC2 + 0x2017] /* " says: I cannot join thee.\n" */);
		add_npc_talk(D_8CE6, &AVATAR[0x11DC9 + 0x2017] /* "I cannot join thee.\n" */);
		return 0;
	}
	if (*pKarmas[Party._loc - 0x05] < 40 && *pKarmas[Party._loc - 0x05] != 0) {
		u4_puts(/*D_2B33*/&AVATAR[0x11DDE + 0x2017] /* "Thou art not " */);
		add_npc_talk(D_8CE6, &AVATAR[0x11DDE + 0x2017] /* "Thou art not " */);
		u4_puts(D_2BB2[Party._loc - 0x05]);
		add_npc_talk(D_8CE6, D_2BB2[Party._loc - 0x05]);
		u4_puts(/*D_2B41*/&AVATAR[0x11DEC + 0x2017] /* " enough for me to join thee.\n" */);
		add_npc_talk(D_8CE6, &AVATAR[0x11DEC + 0x2017] /* " enough for me to join thee.\n" */);
		return 0;
	}
	if(100 * Party.f_1d8 + 100 > Party.chara[0]._HP[1]) {
		u4_puts(/*D_2B5F*/&AVATAR[0x11E0A + 0x2017] /* "Thou art not experienced enough for me to join thee.\n" */);
		add_npc_talk(D_8CE6, &AVATAR[0x11E0A + 0x2017] /* "Thou art not experienced enough for me to join thee.\n" */);
		return 0;
	}
	u4_puts(/*D_2B95*/&AVATAR[0x11E40 + 0x2017] /* "I am honored to join thee!\n" */);
	add_npc_talk(D_8CE6, &AVATAR[0x11E40 + 0x2017] /* "I am honored to join thee!\n" */);
	D_8742._npc._tile[31] =
	D_8742._npc._gtile[31] =
	D_8742._npc._var[31] =
	D_8742._npc._tlkidx[31] = 0;
	t_callback();
	for(bp_02 = 7; bp_02 >= 0; bp_02 --) {
		if(Party.chara[bp_02]._class == (Party._loc - 0x05))
			break;
	}
	if(bp_02 != -1)
		C_6FF9(&(Party.chara[Party.f_1d8]), &(Party.chara[bp_02]));
	Party.f_1d8 ++;
	Gra_13();
	dspl_Stats();
}

/*C_A3A2*/TLK_give()
{
	int bp_02;

	if(D_8742._npc._tile[D_8CE6] != TIL_58) {
		u4_puts(D_8CCE[1]);
		u4_puts(/*D_2BC2*/&AVATAR[0x11E6D + 0x2017] /* " says: I do not need thy gold.  Keep it!\n" */);
		add_npc_talk(D_8CE6, &AVATAR[0x11E74 + 0x2017] /* "I do not need thy gold.  Keep it!\n" */);
		return 0;
	}
	u4_puts(/*D_2BEC*/&AVATAR[0x11E97 + 0x2017] /* "How much?\x12\x12\x12\b\b" */);
	add_npc_talk(D_8CE6, &AVATAR[0x11E97 + 0x2017] /* "How much?\n" */);
	set_input_mode(INPUT_MODE_NUMBER_INPUT_2_DIGITS);
	if((bp_02 = AskInt(2)) > 0) {
		if(Party._gold < bp_02) {
			u4_puts(/*D_2BFB*/&AVATAR[0x11EA6 + 0x2017] /* "Thou hast not that much gold!\n" */);
			add_npc_talk(D_8CE6, &AVATAR[0x11EA6 + 0x2017] /* "Thou hast not that much gold!\n" */);
		} else {
			Party._gold -= bp_02;
			dspl_Stats();
			u4_puts(D_8CCE[1]);
			u4_puts(/*D_2C1A*/&AVATAR[0x11EC5 + 0x2017] /* " says: Oh, Thank thee! I shall never forget thy kindness!\n" */);
			add_npc_talk(D_8CE6, &AVATAR[0x11ECC + 0x2017] /* "Oh, Thank thee! I shall never forget thy kindness!\n" */);
			if((Party._moves >> 4) != Party.f_1ec)
				karma_inc(&(Party._compa), 2);
			Party.f_1ec = Party._moves >> 4;
		}
	}
	C_A22D(6, 0);
}

/*parse talk info*/
C_A443(si/*bp04*/)
register char *si;
{
	register char **di;

	for(di = &(D_8CCE[0]); di != &(D_8CCE[12]); ) {
		*(di++) = si;
		while(*(si++));
	}
	u4_tolower(D_8CCE[2][0]);
}

C_A47F(bp04)
char *bp04;
{
	char *bp_02;

	for(bp_02 = strchr(bp04, 0) - 1; bp_02 + 1 != bp04 && *bp_02 == ' '; )
		*(bp_02--) = 0;
}

extern char* D_1738[];

/*talk to citizen*/
C_A4B4(bp04)
int bp04;
{
	int bp_02, bp_04;
#if 0
	File_TLK = dopen(D_1738[Party._loc - 1], 0);
	dlseek(File_TLK, (D_8742._npc._tlkidx[bp04] - 1) * 0x120);
	dread(File_TLK, D_95CE, 0x120);	
	if (File_TLK) {
		dclose(File_TLK);
		File_TLK = 0;
	}
#else
	LoadSeek(D_1738[Party._loc - 1], 0x120, (D_8742._npc._tlkidx[bp04] - 1) * 0x120, D_95CE);
#endif
	bp_04 = Party.f_1d8;
	D_9452 = D_8742._npc._tile[bp04];
	C_A443(D_95CE+3);

	// There is a bug in the original citizen talk files
	// Calabrini in Moonglow if you ask Job he asks "Dost thou seek an inn or healing?"
	// and you respond "Yes" he asks "Which?" The two keywords are INN and HEALING but this conflicts
	// with the standard keywork HEALTH as only the 1st 4 characters are checked, also the keyword is HEALER not HEALING 
	// so it would never have worked. We will on the fly patch the data here when we detect the issue.
	// Replace the question with "DOST THOU SEEK\nAN INN OR ART\nTHOU INJURED?" and replace the keyword with "INJURED"
	// Also the keyword INN has a space after the word so need to be careful when comparing strings 
	// in the unity part of the code as some keywords are only one character
	// The issue also exists with Michelle
	if ((strncmp(D_8CCE[11], "HEAL", 4) == 0) && (strncmp(D_8CCE[0], "Calabrini", 4) == 0))
	{
		D_8CCE[7] = "Dost thou seek\nan inn or art\nthou injured ?";
		D_8CCE[11] = "INJU";
	}

	if ((strncmp(D_8CCE[11], "HEAL", 4) == 0) && (strncmp(D_8CCE[0], "Michelle", 4) == 0))
	{
		D_8CCE[11] = "VISI";
	}

	/*personnal question 1 & 2*/
	D_2A90[5]._00 = D_8CCE[10];
	D_2A90[6]._00 = D_8CCE[11];
	C_A47F(D_2A90[5]._00);
	C_A47F(D_2A90[6]._00);
	/* */
	u4_puts(/*D_2C55*/&AVATAR[0x11F00 + 0x2017] /* "\nYou meet " */);

	C_A22D(3, D_8CCE[2]);

	/*randomly says his name*/
	if(U4_RND1(1)) {
		Gra_CR();
		u4_puts(D_8CCE[1]);
		u4_puts(/*D_2C60*/&AVATAR[0x11D63 + 0x2017] /* " says: I am " */);
		u4_puts(D_8CCE[0]);
		Gra_CR();
		add_npc_talk(bp04, &AVATAR[0x11D6A + 0x2017] /* "I am " */);
		add_npc_talk(bp04, D_8CCE[0]);
		add_npc_talk(bp04, "\n");
	}

	bp_02 = 0;
	do {
		register int si;
		char bp_12[12];
		u4_puts(/*D_2C6D*/&AVATAR[0x11F18 + 0x2017] /* "\nYour Interest:\n" */);
		add_npc_talk(bp04, &AVATAR[0x11F18 + 0x2017] /* "Your Interest?\n" */);
		set_input_mode(INPUT_MODE_CITIZEN_WORD);
		u4_gets(bp_12, 11);
		Gra_CR();
		if(bp_12[0] == 0)
			break;
		if((si = u_rand_a()) < D_95CE[2]) {
			if (D_95CE[2] - si >= 0x40) {
				/*he/she gets upset*/
				if (_strnicmp(D_8CCE[0], /*D_2C7E*/&AVATAR[0x11F29 + 0x2017] /* "a " */, 2) && _strnicmp(D_8CCE[0], /*D_2C81*/&AVATAR[0x11F2C + 0x2017] /* "the " */, 4))
					u4_puts(D_8CCE[0]);
				else
					u4_puts(D_8CCE[1]);
				u4_puts(/*D_2C86*/&AVATAR[0x11F31 + 0x2017] /* " says: On guard! Fool!\n" */);
				add_npc_talk(bp04, &AVATAR[0x11F37 + 0x2017] /* " On guard! Fool!\n" */);
				D_8742._npc._var[bp04] = 0xff;
			} else {
				u4_puts(D_8CCE[1]);
				u4_puts(/*D_2C9E*/&AVATAR[0x11F49 + 0x2017] /* " turns away!\n\n" */);
			}
			add_npc_talk(VENDOR_INVALID, "");
			return 0;
		}
		for(si = 0; D_2A90[si]._00[0]; si++) {
			if(_strnicmp(D_2A90[si]._00, bp_12, 4) == 0) {
				if(D_2A90[si]._02) {
					Gra_CR();
					D_8CE6 = bp04;
					(*(D_2A90[si]._02))();
					if (Party.f_1d8 != bp_04)
					{
						add_npc_talk(VENDOR_INVALID, "");
						return 0;/*looks like he/she has joined*/
					}
				} else {
					bp_02 = 1;
				}
				if(!bp_02) {
					if(D_95CE[0] == si)
						C_A163();
				}
				break;
			}
		}
		if (D_2A90[si]._00[0] == 0)
		{
			u4_puts(/*D_2CAD*/&AVATAR[0x11F58 + 0x2017] /* "That I cannot help thee with.\n" */);
			add_npc_talk(bp04, &AVATAR[0x11F58 + 0x2017] /* "That I cannot help thee with.\n" */);
		}
	} while(bp_02 == 0);
	u4_puts(/*D_2CCC*/&AVATAR[0x11F77 + 0x2017] /* "\nBye.\n" */);
	add_npc_talk(bp04, &AVATAR[0x11F78 + 0x2017] /* "Bye.\n" */);
	add_npc_talk(VENDOR_INVALID, "");
}

/*shops'y positions by town*/
unsigned char * D_2CD4[] = {
	&AVATAR[0x11F7F + 0x2017] /*{0x00,0x00,0x00,0x00,0x00,0x1A,0x00,0x00}*/,/*LB*/

	&AVATAR[0x11F87 + 0x2017] /*{0x00,0x00,0x00,0x00,0x00,0x0C,0x00,0x00}*/,/*lycaeum*/
	&AVATAR[0x11F8F + 0x2017] /*{0x00,0x00,0x00,0x00,0x00,0x0F,0x00,0x00}*/,/*empath*/
	&AVATAR[0x11F97 + 0x2017] /*{0x00,0x00,0x00,0x00,0x00,0x0C,0x00,0x00}*/,/*serpent*/

	&AVATAR[0x11F9F + 0x2017] /*{0x00,0x00,0x0E,0x00,0x1A,0x1B,0x02,0x00}*/,/*MOONGLOW*/
	&AVATAR[0x11FA7 + 0x2017] /*{0x03,0x07,0x06,0x02,0x00,0x1D,0x0C,0x00}*/,/*BRITAIN*/
	&AVATAR[0x11FAF + 0x2017] /*{0x09,0x05,0x00,0x13,0x00,0x06,0x1A,0x00}*/,/*JHELOM*/
	&AVATAR[0x11FB7 + 0x2017] /*{0x00,0x00,0x18,0x00,0x00,0x19,0x00,0x00}*/,/*YEW.ULT*/
	&AVATAR[0x11FBF + 0x2017] /*{0x1C,0x00,0x00,0x00,0x00,0x00,0x03,0x00}*/,/*MINOC*/
	&AVATAR[0x11FC7 + 0x2017] /*{0x14,0x18,0x00,0x02,0x00,0x00,0x03,0x00}*/,/*TRINSIC*/
	&AVATAR[0x11FCF + 0x2017] /*{0x00,0x00,0x11,0x00,0x04,0x1B,0x0D,0x00}*/,/*SKARA*/    /* 00 00 11 00 04 1B 0D A5 ??? doesn't match EXE*/
	&AVATAR[0x11FD7 + 0x201A] /*{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}*/,/*MAGINCIA*/ /* 07 B2 00 0A 00 B0 04 1A ??? doesn't match EXE*/
	&AVATAR[0x11FDC + 0x201A] /*{0x00,0x04,0x1A,0x05,0x07,0x00,0x00,0x00}*/,/*paws*/     /* B0 04 1A 05 07 00 00 00 ??? doesn't match EXE*/

	&AVATAR[0x11FE4 + 0x201A] /*{0x0B,0x11,0x00,0x19,0x08,0x00,0x00,0x07}*/,/*den*/
	&AVATAR[0x11FEC + 0x201A] /*{0x14,0x00,0x00,0x16,0x00,0x00,0x15,0x1A}*/,/*vesper*/
	&AVATAR[0x11FF4 + 0x201A] /*{0x00,0x00,0x00,0x00,0x00,0x1A,0x00,0x00}*/ /*cove*/
};

extern SHP_weapon();
extern SHP_armor();
extern SHP_food();
extern SHP_pub();
extern SHP_reagent();
extern SHP_healer();
extern SHP_inn();
extern SHP_guild();
extern SHP_horse();
extern SHP_hawkwind();

/*shops functions callbacks*/
pHandler_tlk D_2D54[] = {
	SHP_weapon,
	SHP_armor,
	SHP_food,
	SHP_pub,
	SHP_reagent,
	SHP_healer,
	SHP_inn,
	SHP_guild,

	SHP_horse,
	SHP_hawkwind
};

/*shop at Y*/
C_A686(bp04)
unsigned char bp04;
{
	register int si;

	for(si = 7; si >= 0;si--) {
		if(D_2CD4[Party._loc - 1][si] == bp04)
			break;
	}
	if(si == -1 && bp04 == 0x18 && Party._loc == 0x0d)
		si = 8;/*patch! paws' horses*/
	if(si == -1 && bp04 == 0x19 && Party._loc == 0x01)
		si = 9;/*patch! LB's hawkwind*/
	if(si == -1) {
		u4_puts(D_2A7A); // funny no response
		return 0;
	}
	(*(D_2D54[si]))();
}

/*C_A6F3*/CMD_Talk() {
	unsigned char loc_A, loc_E;
	int loc_B, loc_C, loc_D;

	u4_puts(/*D_2D68*/&AVATAR[0x12010 + 0x201A] /* "Talk\n" */);
	if(Party.f_1dc != 0) {
		w_DriftOnly();
		return 0;
	}
	set_input_mode(INPUT_MODE_GENERAL_DIRECTION);

	AskDir(/*D_2D6E*/&AVATAR[0x12016 + 0x201A] /* "Dir: " */, &loc_B, &loc_D);
	if(!(loc_B | loc_D))
		return 0;

	loc_E = Party._x + loc_B;
	loc_A = Party._y + loc_D;
	loc_C = C_0A8B(loc_E, loc_A);
	if(loc_E > 31 || loc_A > 31 || Party._loc == 0) {
		u4_puts(D_2A7A);
		return 0;
	}

	if(
		D_8742._map.x32x32[loc_A][loc_E] >= TIL_60 &&
		D_8742._map.x32x32[loc_A][loc_E] <= TIL_7E
	) {/*alphabet*/
		loc_E += loc_B;
		loc_A += loc_D;
		if(loc_E > 31 || loc_A > 31) {
			u4_puts(D_2A7A);
			return 0;
		}
		if((loc_C = C_0A8B(loc_E, loc_A)) == -1 || D_8742._npc._tile[loc_C] != TIL_52) {
			u4_puts(D_2A7A);
			return 0;
		}
		C_A686(loc_A - loc_D);
		return 0;
	}

	if(loc_C == -1 || D_8742._npc._tlkidx[loc_C] == 0) {
		u4_puts(D_2A7A);
		return 0;
	}

	if(D_8742._npc._tile[loc_C] == TIL_5E) {
		C_E59B();
		return 0;
	}

	C_A4B4(loc_C);
}
