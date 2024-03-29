/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "u4.h"

#define RG_ash        (0x80>>0)
#define RG_ginseng    (0x80>>1)
#define RG_garlic     (0x80>>2)
#define RG_spiderSilk (0x80>>3)
#define RG_bloodMoss  (0x80>>4)
#define RG_blackPearl (0x80>>5)
#define RG_nightshade (0x80>>6)
#define RG_mandrake   (0x80>>7)

unsigned char * D_277E = &AVATAR[0x11A29 + 0x0017]; // {
#if 0
	/*A*/RG_ginseng|RG_garlic,       // 0x40|0x20 = 0x60
	/*B*/RG_spiderSilk|RG_bloodMoss, // 0x10|0x08 = 0x18
	/*C*/RG_ginseng|RG_garlic,       // 0x40|0x20 = 0x60
	/*D*/RG_ash|RG_garlic|RG_blackPearl, // 0x80|0x20|0x04 = 0xA4
	/*E*/RG_ash|RG_spiderSilk|RG_blackPearl,
	/*F*/RG_ash|RG_blackPearl,
	/*G*/RG_ash|RG_blackPearl|RG_mandrake,
	/*H*/RG_ginseng|RG_spiderSilk,
	/*I*/RG_blackPearl|RG_mandrake,
	/*J*/RG_blackPearl|RG_nightshade|RG_mandrake,
	/*K*/RG_blackPearl|RG_nightshade,
	/*L*/RG_ash,
	/*M*/RG_ash|RG_blackPearl,
	/*N*/RG_ash|RG_garlic|RG_mandrake,
	/*O*/RG_ash|RG_bloodMoss,
	/*P*/RG_ash|RG_ginseng|RG_garlic,
	/*Q*/RG_ash|RG_ginseng|RG_bloodMoss,
	/*R*/RG_ash|RG_ginseng|RG_garlic|RG_spiderSilk|RG_bloodMoss|RG_mandrake,
	/*S*/RG_ginseng|RG_spiderSilk,
	/*T*/RG_ash|RG_bloodMoss|RG_mandrake,
	/*U*/RG_ash|RG_garlic,
	/*V*/RG_nightshade|RG_mandrake,
	/*W*/RG_ash|RG_bloodMoss,
	/*X*/RG_ash|RG_spiderSilk|RG_bloodMoss,
	/*Y*/RG_spiderSilk|RG_bloodMoss,
	/*Z*/RG_spiderSilk|RG_bloodMoss
};
#endif

/*C_8C08*/CMD_Mix()
{
	register int si;
	unsigned char loc_A[8];
	int  loc_B, loc_C, loc_D;
	unsigned char loc_E;

	loc_D = 0;
	do {
		Gra_13();
		C_4649();
		C_4C42();
		txt_Y = 23;
		txt_X = 24;
		u4_puts(/*D_270C*/&AVATAR[0x119B7 + 0x0017] /* "Mix Reagents\n" */);
		set_input_mode(INPUT_MODE_GENERAL_ASK_SPELL);
		if((loc_C = AskLetter(/*D_271A*/&AVATAR[0x119C5 + 0x0017] /* "For Spell:\x12\x12\b" */, 'A', 'Z')) < 0)
			break;
		loc_C -= 'A';
		u4_puts(D_1E98[101 + loc_C]);
		Gra_CR();
		Gra_13();
		C_4649();
		for(si = 7; si >= 0; si --)
			loc_A[si] = Party._reagents[si];
		loc_E = 0;
		while(1) {
			C_4BC7();
			txt_Y = 23;
			txt_X = 24;
			set_input_mode(INPUT_MODE_GENERAL_ASK_LETTER_REAGENT);
			if((loc_B = AskLetter(/*D_2728*/&AVATAR[0x119D3 + 0x0017] /* "Reagent:\x12\x12\b" */, 'A', 'H')) == -2) {
				for(si = 7; si >= 0; si --)
					Party._reagents[si] = loc_A[si];
				loc_D = 1;
				break;
			}
			if(loc_B == -1) {
				if(loc_E == 0) {
					u4_puts(/*D_2734*/&AVATAR[0x119DF + 0x0017] /* "\nNothing mixed!\n" */);
				} else {
					u4_puts(/*D_2745*/&AVATAR[0x119F0 + 0x0017] /* "\nYou mix the Reagents, and...\n" */);
					loc_B = 0;
					if(D_277E[loc_C] == loc_E) {
						u4_puts(/*D_2764*/&AVATAR[0x11A0F + 0x0017] /* "Success!\n\n" */);
						Party._mixtures[loc_C] ++;
						if(Party._mixtures[loc_C] > 99)
							Party._mixtures[loc_C] = 99;
					} else {
						u4_puts(/*D_276F*/&AVATAR[0x11A1A + 0x0017] /* "It Fizzles!\n\n" */);
					}
				}
				break;
			}
			loc_B -= 'A';
			if(Party._reagents[loc_B] == 0) {
				w_NoneLeft();
				break;
			}
			loc_E |= 0x80 >> loc_B;
			Party._reagents[loc_B] --;
		}
	} while(loc_D == 0);
	Gra_13();
	C_4649();
	dspl_Stats();
}
