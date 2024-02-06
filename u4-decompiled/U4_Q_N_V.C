/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "U4.H"

#include <string.h>
#include <stdlib.h>

/*C_6F29*/CMD_Quit()
{
	u4_puts(/*D_21A2*/&AVATAR[0x11455 + 0x200F] /* "Quit & Save...\n" */);
	u4_putl(Party._moves, 1, '0');
	u4_puts(/*D_21B2*/&AVATAR[0x11465 + 0x200F] /* " moves\n" */);
	if(Party._loc) {
/*ENABLE_TOWN_SAVE4*/
#if 1
// always allow saves
#else
		if(Party._loc < 0x11 || Party._loc > 0x18) {
			u4_puts(/*D_21BA*/&AVATAR[0xF8C3 + 0x5] /* "Not Here!\n" */);
			return 0;
		}
#endif
	}
	if(Save(/*D_21C5*/&AVATAR[0x11478 + 0x200F] /* "PARTY.SAV" */, sizeof(struct tParty), &Party) == -1)
		exit(3);
	if(Party._loc == 0) {
		if(Save(/*D_21CF*/&AVATAR[0x11482 + 0x200F] /* "MONSTERS.SAV" */, sizeof(struct tNPC), &(D_8742._npc)) == -1)
			exit(3);
	}
/*ENABLE_TOWN_SAVE4*/
#if 1
	if (Party._loc != 0 && Party._loc < 0x11)
	{
		/*Force doors to close before saving*/
		C_431D(); C_431D(); C_431D(); C_431D(); C_431D();
		if(Save("TOWNMAP.SAV", sizeof(struct t_500), &D_8742) == -1)
			exit(3);
		return 0;
	}
	if(Party._loc == 0 || Party._loc > 0x18)
#else
	if(Party._loc < 0x11 || Party._loc > 0x18)
#endif
		return 0;
	if(Save(/*D_21DC*/&AVATAR[0x11482 + 0x200F] /* "MONSTERS.SAV" */, sizeof(struct tNPC), &(D_8742._npc)) == -1)
		exit(3);
	if(Save(/*D_21E9*/&AVATAR[0xF935 + 0x5] /* "DNGMAP.SAV" */, sizeof(tMap8x8x8), &(D_8742._map)) == -1)
		exit(3);
}

char * D_21F4 = &AVATAR[0x114A7 + 0x200F] /* ", You must lead!\n" */;

/*exchange two party members ?*/
C_6FF9(bp06, bp04)
struct tChara *bp06;
struct tChara *bp04;
{
	struct tChara bp_28;
	memcpy(&bp_28, bp06, sizeof(struct tChara));
	memcpy(bp06, bp04, sizeof(struct tChara));
	memcpy(bp04, &bp_28, sizeof(struct tChara));
}

/*C_7034*/CMD_NewOrder()
{
	register struct tChara *loc_A, *loc_B;
	struct tChara loc_E;/*not used*/
	int loc_C, loc_D;

	u4_puts(/*D_2206*/&AVATAR[0x114B9 + 0x200F] /* "New Order!\n" */);
	set_input_mode(INPUT_MODE_GENERAL_ASK_CHARACTER_NUMBER);
	if((loc_C = AskChara(/*D_2212*/&AVATAR[0x114C5 + 0x200F] /* "Exchange #\x12\x12\b" */)) < 0)
		return 0;
	loc_A = &(Party.chara[loc_C]);
	if(loc_A == &(Party.chara[0])) {
		u4_puts(Party.chara[0]._name);
		u4_puts(D_21F4);
		return 0;
	}
	set_input_mode(INPUT_MODE_GENERAL_ASK_CHARACTER_NUMBER);
	if((loc_D = AskChara(/*D_2220*/&AVATAR[0x114D3 + 0x200F] /* "    with #\x12\x12\b" */)) < 0)
		return 0;
	loc_B = &(Party.chara[loc_D]);
	if(loc_B == &(Party.chara[0])) {
		u4_puts(Party.chara[0]._name);
		u4_puts(D_21F4);
		return 0;
	}
	if(loc_A == loc_B) {
		w_What();
		return 0;
	}
	C_6FF9(loc_A, loc_B);
	Gra_13();
	dspl_Stats();
}

/*C_70AD*/CMD_Volume()
{
	u4_puts(/*D_222E*/&AVATAR[0x114E1 + 0x200F] /* "Volume O" */);
	u4_puts((SoundFlag ^= 1)?/*D_2237*/&AVATAR[0xF57F + 0x5] /* "n!\n" */:/*D_223B*/"ff!\n");
}
