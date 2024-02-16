/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "U4.H"

#include <string.h>

/*----------------------------------------*/

char *D_3F0C[][8] = {
	{/* < 20*/
		/*D_316E*/&AVATAR[0x123E9 + 0x002e] /* "Thou art a thief and a scoundrel. Thou may not ever become an Avatar!" */,
		/*D_31B4*/&AVATAR[0x1242F + 0x002e] /* "Thou art a cold and cruel brute.  Thou shouldst go to prison for thy crimes!" */,
		/*D_3201*/&AVATAR[0x1247C + 0x002e] /* "Thou art a coward, thou dost flee from the hint of danger!" */,
		/*D_323C*/&AVATAR[0x124B7 + 0x002e] /* "Thou art an unjust wretch. Thou are a fulsome meddler!" */,
		/*D_3273*/&AVATAR[0x124EE + 0x002e] /* "Thou art a self-serving Tufthunter. Thou deservest not my help, yet I grant it!" */,
		/*D_32C3*/&AVATAR[0x1253E + 0x002e] /* "Thou art a cad and a bounder. Thy presence is an affront. Thou art low as a slug!" */,
		/*D_3315*/&AVATAR[0x12590 + 0x002e] /* "Thy spirit is weak and feeble. Thou dost not strive for Perfection!" */,
		/*D_3359*/&AVATAR[0x125D4 + 0x002e] /* "Thou art proud and vain. All other virtue in thee is a loss!" */,
	}, {/* < 40*/
		/*D_3396*/&AVATAR[0x12611 + 0x002e] /* "Thou art not an honest soul. Thou must live a more honest life to be an Avatar!" */,
		/*D_33E6*/&AVATAR[0x12661 + 0x002e] /* "Thou dost kill where there is no need and give too little unto others!" */,
		/*D_342D*/&AVATAR[0x126A8 + 0x002e] /* "Thou dost not display a great deal of Valor. Thou dost flee before the need!" */,
		/*D_347A*/&AVATAR[0x126F5 + 0x002e] /* "Thou art cruel and unjust. In time thou will suffer for thy crimes!" */,
		/*D_34BE*/&AVATAR[0x12739 + 0x002e] /* "Thou dost need to think more of the life of others and less of thy own!" */,
		/*D_3506*/&AVATAR[0x12781 + 0x002e] /* "Thou dost not fight with honor but with malice and deceit!" */,
		/*D_3541*/&AVATAR[0x127BC + 0x002e] /* "Thou dost not take time to care about thy inner being, a must to be an Avatar!" */,
		/*D_3590*/&AVATAR[0x1280B + 0x002e] /* "Thou art too proud of thy little deeds. Humility is the root of all Virtue!" */,
	}, {/* < 60*/
		/*D_35DC*/&AVATAR[0x12857 + 0x002e] /* "Thou hast made little progress on the paths of Honesty. Strive to prove thy worth!" */,
		/*D_362F*/&AVATAR[0x128AA + 0x002e] /* "Thou hast not shown thy compassion well. Be more kind unto others!" */,
		/*D_3672*/&AVATAR[0x128ED + 0x002e] /* "Thou art not yet a valiant warrior.  Fight to defeat evil and prove thyself!" */,
		/*D_36BF*/&AVATAR[0x1293A + 0x002e] /* "Thou hast not proven thyself to be just. Strive to do justice unto all things!" */,
		/*D_370E*/&AVATAR[0x12989 + 0x002e] /* "Thy sacrifice is small. Give of thy life's blood so that others may live." */,
		/*D_3758*/&AVATAR[0x129D3 + 0x002e] /* "Thou dost need to show thyself to be more honorable.  The path lies before thee!" */,
		/*D_37A9*/&AVATAR[0x12A24 + 0x002e] /* "Strive to know and master more of thine inner being. Meditation lights the path!" */,
		/*D_37FA*/&AVATAR[0x12A75 + 0x002e] /* "Thy progress on this path is most uncertain. Without Humility thou art empty!" */,
	}, {/* < 99*/
		/*D_3848*/&AVATAR[0x12AC3 + 0x002e] /* "Thou dost seem to be an honest soul.  Continued honesty will reward thee!" */,
		/*D_3892*/&AVATAR[0x12B0D + 0x002e] /* "Thou dost show thy compassion well.  Continued goodwill should be thy guide!" */,
		/*D_38DF*/&AVATAR[0x12B5A + 0x002e] /* "Thou art showing Valor in the face of danger. Strive to become yet more so!" */,
		/*D_392B*/&AVATAR[0x12BA6 + 0x002e] /* "Thou dost seem fair and just. Strive to uphold Justice even more sternly!" */,
		/*D_3975*/&AVATAR[0x12BF0 + 0x002e] /* "Thou art giving of thyself in some ways. Seek ye now to find yet more!" */,
		/*D_39BC*/&AVATAR[0x12C37 + 0x002e] /* "Thou dost seem to be Honorable in nature.  Seek to bring Honor upon others as well!" */,
		/*D_3A10*/&AVATAR[0x12C8B + 0x002e] /* "Thou art doing well on the path to inner sight continue to seek the inner light!" */,
		/*D_3A61*/&AVATAR[0x12CDC + 0x002e] /* "Thou dost seem a humble soul.  Thou art setting strong stones to build virtues upon!" */,
	}, {/* >= 99*/
		/*D_3AB6*/&AVATAR[0x12D31 + 0x002e] /* "Thou art truly an honest soul. Seek ye now to reach Elevation!" */,
		/*D_3AF5*/&AVATAR[0x12D70 + 0x002e] /* "Compassion is a virtue that thou hast shown well.  Seek ye now Elevation!" */,
		/*D_3B3F*/&AVATAR[0x12DBA + 0x002e] /* "Thou art a truly valiant warrior. Seek ye now Elevation in the virtue of valor!" */,
		/*D_3B8F*/&AVATAR[0x12E0A + 0x002e] /* "Thou art just and fair.  Seek ye now the Elevation!" */,
		/*D_3BC3*/&AVATAR[0x12E3E + 0x002e] /* "Thou art giving and good.  Thy self-sacrifice is great.  Seek now Elevation!" */,
		/*D_3C10*/&AVATAR[0x12E8B + 0x002e] /* "Thou hast proven thyself to be Honorable. Seek ye now for the Elevation!" */,
		/*D_3C59*/&AVATAR[0x12ED4 + 0x002e] /* "Spirituality is in thy nature. Seek ye now the Elevation!" */,
		/*D_3C93*/&AVATAR[0x12F0E + 0x002e] /* "Thy Humility shines bright upon thy being. Seek ye now for Elevation!" */
	}
};

static unsigned char D_913A;
static unsigned char D_913B, D_913C;
static unsigned short D_913E;/*# of drinks in pub*/

/*SHOP #10 - The Seer*/
/*C_C922*/SHP_hawkwind()
{
	int bp_02, bp_04;
	char bp_14[16];

	if(!isCharaConscious(0)) {
		u4_puts(/*D_3CD9*/&AVATAR[0x12F54 + 0x002e] /* "The Seer says: I will speak only with " */);
		add_npc_talk(VENDOR_HAWKKWIND, &AVATAR[0x12F63 + 0x002e] /* "I will speak only with " */);
		u4_puts(Party.chara[0]._name);
		add_npc_talk(VENDOR_HAWKKWIND, Party.chara[0]._name);
		u4_puts(/*D_3D00*/&AVATAR[0x12F7B + 0x002e] /* ".\nReturn when " */);
		add_npc_talk(VENDOR_HAWKKWIND, &AVATAR[0x12F7B + 0x002e] /* ".\nReturn when " */);
		u4_puts(Party.chara[0]._name);
		add_npc_talk(VENDOR_HAWKKWIND, Party.chara[0]._name);
		u4_puts(/*D_3D0F*/&AVATAR[0x12F8A + 0x002e] /* " is revived!\n" */);
		add_npc_talk(VENDOR_HAWKKWIND, &AVATAR[0x12F8A + 0x002e] /* " is revived!\n" */);
		add_npc_talk(VENDOR_INVALID, "");
		return 0;
	}
	u4_puts(/*D_3D1D*/&AVATAR[0x12F98 + 0x002e] /* "\n\nWelcome, " */);
	add_npc_talk(VENDOR_HAWKKWIND, &AVATAR[0x12F9A + 0x002e] /* "Welcome, " */);
	u4_puts(Party.chara[0]._name);
	add_npc_talk(VENDOR_HAWKKWIND, Party.chara[0]._name);
	u4_puts(". ");
	add_npc_talk(". ");
	u4_puts(/*D_3D29*/ &AVATAR[0x12FA4 + 0x002e] /* ". \nI am Hawkwind, Seer of Souls. I see that which is within thee and drives thee to deeds of good or evil...\n" */);
	add_npc_talk(VENDOR_HAWKKWIND, &AVATAR[0x12FA4 + 0x002e] /* ". \nI am Hawkwind, Seer of Souls. I see that which is within thee and drives thee to deeds of good or evil...\n" */);
	set_input_mode(INPUT_MODE_GENERAL_CONTINUE);
	u_kbflush();
	u_kbread();
	bp_14[0] = 0;
	while(1) {
		u4_puts(bp_14[0] == 0?
			/*D_3D95*/&AVATAR[0x13010 + 0x002e] /* "\nFor what path dost thou seek enlightenment?\n" */:
			/*D_3DC3*/&AVATAR[0x1303E + 0x002e] /* "\nHawkwind asks: What other path seeks clarity?\n" */
		);
		add_npc_talk(VENDOR_HAWKKWIND, bp_14[0] == 0 ?
			/*D_3D95*/&AVATAR[0x13010 + 0x002e] /* "\nFor what path dost thou seek enlightenment?\n" */ :
			/*D_3DC3*/&AVATAR[0x1304E + 0x002e] /* "\nWhat other path seeks clarity?\n" */);
		set_input_mode(INPUT_MODE_HAWKWIND_WORD);
		u4_gets(bp_14, 15);
		if (
			bp_14[0] == 0 ||
			_strnicmp(bp_14, /*D_3DF3*/&AVATAR[0x1306E + 0x002e] /* "none" */, 16) == 0 ||
			_strnicmp(bp_14, /*D_3DF8*/&AVATAR[0x13073 + 0x002e] /* "bye" */, 16) == 0
		) break;
		for(bp_04 = 7; bp_04 >= 0; bp_04 --) {
			if(_strnicmp(bp_14, D_1E98[151 + bp_04], 4) == 0)
				break;
		}
		if(bp_04 == -1) {
			u4_puts(/*D_3DFC*/&AVATAR[0x13077 + 0x002e] /* "\nHe says: That is not a subject for enlightenment.\n" */);
			add_npc_talk(VENDOR_HAWKKWIND, &AVATAR[0x13081 + 0x002e] /* "\nThat is not a subject for enlightenment.\n" */);
			continue;
		}
		Gra_CR();
		if(*(pKarmas[bp_04]) == 0) {
			u4_puts(/*D_3E30*/&AVATAR[0x130AB + 0x002e] /* "He says:\nThou hast become a partial Avatar in that attribute. Thou need not my insights.\n" */);
			add_npc_talk(VENDOR_HAWKKWIND, &AVATAR[0x130B4 + 0x002e] /* "Thou hast become a partial Avatar in that attribute. Thou need not my insights.\n" */);
			continue;
		}
		if(*(pKarmas[bp_04]) < 20) {
			bp_02 = 0;
		} else  if(*(pKarmas[bp_04]) < 40) {
			bp_02 = 1;
		} else  if(*(pKarmas[bp_04]) < 60) {
			bp_02 = 2;
		} else  if(*(pKarmas[bp_04]) < 99) {
			bp_02 = 3;
		} else {
			bp_02 = 4;
		}
		Gra_CR();
		u4_puts(D_3F0C[bp_02][bp_04]);
		add_npc_talk(VENDOR_HAWKKWIND, D_3F0C[bp_02][bp_04]);
		Gra_CR();
		if(bp_02 == 4) {
			u4_puts(/*D_3E8A*/&AVATAR[0x13105 + 0x002e] /* "\nGo to the Shrine and meditate for three Cycles!\n" */);
			add_npc_talk(VENDOR_HAWKKWIND, &AVATAR[0x13105 + 0x002e] /* "\nGo to the Shrine and meditate for three Cycles!\n" */);
			set_input_mode(INPUT_MODE_GENERAL_CONTINUE);
			u_kbflush();
			u_kbread();
		}
	}
	u4_puts(/*D_3EBC*/&AVATAR[0x13137 + 0x002e] /* "\nHawkwind says: Fare thee well and may thou complete the Quest of the Avatar!\n" */);
	add_npc_talk(VENDOR_HAWKKWIND, &AVATAR[0x13147 + 0x002e] /* "Fare thee well and may thou complete the Quest of the Avatar!\n" */);
	if(Party._moves / 100 != Party.f_1ea) {
		karma_inc(&(Party._spiri), 3);
		Party.f_1ea = Party._moves / 100;
	}
	add_npc_talk(VENDOR_INVALID, "");
}

/*----------------------------------------*/

/*shops indexes*/
unsigned char * D_4170 = &AVATAR[0x133EB + 0x002e] /*{0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 2, 0, 3, 4, 0, 0} */;

char *D_4180[] = {
	/*D_3F5C*/&AVATAR[0x131D7 + 0x002e] /* "Magical Herbs" */,
	/*D_3F6A*/&AVATAR[0x131E5 + 0x002e] /* "Herbs and Spice" */,
	/*D_3F7A*/&AVATAR[0x131F5 + 0x002e] /* "The Magics" */,
	/*D_3F85*/&AVATAR[0x13200 + 0x002e] /* "Magic Mentar" */
};

char *D_4188[] = {
	/*D_3F92*/&AVATAR[0x1320D + 0x002e] /* "Margot" */,
	/*D_3F99*/&AVATAR[0x13214 + 0x002e] /* "Sasha" */,
	/*D_3F9F*/&AVATAR[0x1321A + 0x002e] /* "Shiela" */,
	/*D_3FA6*/&AVATAR[0x13221 + 0x002e] /* "Shannon" */
};

unsigned char * D_4190[] = {
	&AVATAR[0x1340B + 0x002e] /*{2, 5, 6, 3, 6, 9}*/,
	&AVATAR[0x13411 + 0x002e] /*{2, 4, 9, 6, 4, 8}*/,
	&AVATAR[0x13417 + 0x002e] /*{3, 4, 2, 9, 6, 7}*/,
	&AVATAR[0x1341D + 0x002e] /*{6, 7, 9, 9, 9, 1}*/
};

/*SHOP #5 - reagents*/
/*C_CAF6*/SHP_reagent()
{
	char loc_C;
	int loc_A, loc_B, loc_D, loc_E;

	D_9142 = D_4170[Party._loc - 1] - 1;
	u4_puts(/*D_3FAE*/&AVATAR[0x13229 + 0x002e] /* "A blind woman turns to you and says: Welcome to " */);
	add_npc_talk(VENDOR_REAGENT, &AVATAR[0x1324E + 0x002e] /* "Welcome to " */);
	u4_puts(D_4180[D_9142]);
	add_npc_talk(VENDOR_REAGENT, D_4180[D_9142]);
	u4_puts(". ");
	add_npc_talk(VENDOR_REAGENT, ". ");
	Gra_13();
	C_4BC7();
	u4_puts(/*D_3FDF*/ &AVATAR[0x1325A + 0x002e] /* ". \n\nI am " */);
	add_npc_talk(VENDOR_REAGENT, &AVATAR[0x1325A + 0x002e] /* ". I am " */);
	u4_puts(D_4188[D_9142]);
	add_npc_talk(VENDOR_REAGENT, D_4188[D_9142]);
	u4_puts(". \n");
	add_npc_talk(VENDOR_REAGENT, ". \n");
	u4_puts(/*D_3FE7*/&AVATAR[0x13262 + 0x002e] /* "Are you in need of Reagents?\x12\x12\b" */);
	add_npc_talk(VENDOR_REAGENT, &AVATAR[0x13262 + 0x002e] /* ". \nAre you in need of Reagents?\n" */);
	set_input_mode(INPUT_MODE_GENERAL_YES_NO);
	loc_C = AskY_N();
	if(loc_C == 'Y') {
		u4_puts(/*D_4008*/&AVATAR[0x13283 + 0x002e] /* "Very well,\n" */);
		add_npc_talk(VENDOR_REAGENT, &AVATAR[0x13283 + 0x002e] /* "Very well,\n" */);
		do {
			u4_puts(/*D_4014*/&AVATAR[0x1328F + 0x002e] /* "I have\n\
A-Sulfurous Ash\n\
B-Ginseng\n\
C-Garlic\n\
D-Spider Silk\n\
E-Blood Moss\n\
F-Black Pearl\n" */
			);
			add_npc_talk(VENDOR_REAGENT, &AVATAR[0x1328F + 0x002e] /* "I have \
Sulfurous Ash, \
Ginseng, \
Garlic, \
Spider Silk, \
Blood Moss and \
Black Pearls.\n" */);
			set_input_mode(INPUT_MODE_GENERAL_ASK_LETTER_REAGENT);
			loc_B = AskLetter(/*D_4068*/&AVATAR[0x132E3 + 0x002e] /* "Your Interest:\x12\x12\b" */, 'A', 'F');
			add_npc_talk(VENDOR_REAGENT, &AVATAR[0x139B2 + 0x002e] /* "Your Interest?" */);
			loc_B -= 'A';
			if(loc_B < 0)
				break;
			u4_puts(/*D_407A*/&AVATAR[0x132F5 + 0x002e] /* "\nVery well, we sell " */);
			add_npc_talk(VENDOR_REAGENT, &AVATAR[0x132F5 + 0x002e] /* "\nVery well, we sell " */);
			u4_puts(D_1E98[93 + loc_B]);
			add_npc_talk(VENDOR_REAGENT, D_1E98[93 + loc_B]);
			u4_puts(/*D_408F*/&AVATAR[0x1330A + 0x002e] /* " for " */);
			add_npc_talk(VENDOR_REAGENT, &AVATAR[0x1330A + 0x002e] /* " for " */);
			u4_putl(D_4190[D_9142][loc_B], 1, '0');
			add_npc_talk_long(VENDOR_REAGENT, D_4190[D_9142][loc_B]);
			// TODO need putl npc talk
			u4_puts(/*D_4095*/&AVATAR[0x13310 + 0x002e] /* "gp. How many would you like?\x12\x12\x12\b\b" */);
			add_npc_talk(VENDOR_REAGENT, &AVATAR[0x13310 + 0x002e] /* "gold pieces. How many would you like?" */);
			set_input_mode(INPUT_MODE_NUMBER_INPUT_2_DIGITS);
			loc_A = AskInt(2);
			if(loc_A > 0) {
				u4_puts(/*D_40B7*/&AVATAR[0x13332 + 0x002e] /* "\nVery good, that will be " */);
				add_npc_talk(VENDOR_REAGENT, &AVATAR[0x13332 + 0x002e] /* "\nVery good, that will be " */);
				u4_putl((long)loc_A * D_4190[D_9142][loc_B], 1, '0');
				add_npc_talk_long(VENDOR_REAGENT, (long)loc_A * D_4190[D_9142][loc_B]);
				u4_puts(/*D_40D1*/&AVATAR[0x1334C + 0x002e] /* "gp. You pay:\x12\x12\x12\x12\b\b\b" */);
				add_npc_talk(VENDOR_REAGENT, &AVATAR[0x1334C + 0x002e] /* "gold pieces." */);
				set_input_mode(INPUT_MODE_NUMBER_INPUT_3_DIGITS);
				loc_D = AskInt(3);
				if(loc_D <= 0) {
					if(loc_D < 0)
						sound(1,0);
				} else {
					if(loc_D < loc_A * D_4190[D_9142][loc_B]) {
						loc_E = loc_A * D_4190[D_9142][loc_B] - loc_D;
						loc_E = (loc_E < 12)?4:(loc_E / 3);
						karma_dec(&(Party._hones), loc_E);
						karma_dec(&(Party._justi), loc_E);
						karma_dec(&(Party._honor), loc_E);
					}
					if(loc_D > Party._gold) {
						u4_puts(/*D_40E5*/&AVATAR[0x13360 + 0x002e] /* "It seems you have not the gold!\n" */);
						add_npc_talk(VENDOR_REAGENT, &AVATAR[0x13360 + 0x002e] /* "It seems you have not the gold!\n" */);
					} else {
						karma_inc(&(Party._hones), 2);
						karma_inc(&(Party._justi), 2);
						karma_inc(&(Party._honor), 2);
						Party._gold -= loc_D; dspl_Gold();
						Party._reagents[loc_B] += loc_A;
						if(Party._reagents[loc_B] > 99)
							Party._reagents[loc_B] = 99;
						C_4BC7();
						u4_puts(/*D_4106*/&AVATAR[0x13381 + 0x002e] /* "\nVery good.\n" */);
						add_npc_talk(VENDOR_REAGENT, &AVATAR[0x13381 + 0x002e] /* "\nVery good.\n" */);
					}
				}
			} else {
				u4_puts(/*D_4113*/&AVATAR[0x1338E + 0x002e] /* "\nI see, then\n" */); 
				add_npc_talk(VENDOR_REAGENT, &AVATAR[0x1338E + 0x002e] /* "\nI see, then\n" */);
			}
			u4_puts(/*D_4121*/&AVATAR[0x1339C + 0x002e] /* "Anything else?\x12\x12\b" */);
			add_npc_talk(VENDOR_REAGENT, &AVATAR[0x1339C + 0x002e] /* "Anything else?" */);
			set_input_mode(INPUT_MODE_GENERAL_YES_NO);
			loc_C = AskY_N();
		} while(loc_C == 'Y');
	}
	Gra_13();
	C_4649();
	dspl_Stats();
	Gra_CR();
	u4_puts(D_4188[D_9142]);
	u4_puts(/*D_4133*/&AVATAR[0x133AE + 0x002e] /* " says: Perhaps another time then.... and slowly turns away.\n" */);
	add_npc_talk(VENDOR_REAGENT, &AVATAR[0x133B5 + 0x002e] /* "Perhaps another time then.\n" */);
	add_npc_talk(VENDOR_INVALID, "" );
}

/*----------------------------------------*/

char *D_46A2[] = {
	/*D_41A8*/&AVATAR[0x13423 + 0x002e] /* "Windsor Weaponry" */,
	/*D_41B9*/&AVATAR[0x13434 + 0x002e] /* "Willard's Weaponry" */,
	/*D_41CC*/&AVATAR[0x13447 + 0x002e] /* "The Iron Works" */,
	/*D_41DB*/&AVATAR[0x13456 + 0x002e] /* "Duelling Weapons" */,
	/*D_41EC*/&AVATAR[0x13467 + 0x002e] /* "Hook's Arms" */,
	/*D_41F8*/&AVATAR[0x13473 + 0x002e] /* "Village Arms" */
};

char *D_46AE[] = {
	/*D_4205*/&AVATAR[0x13480 + 0x002e] /* "Winston" */,
	/*D_420D*/&AVATAR[0x13434 + 0x002e] /* "Willard" */,
	/*D_4215*/&AVATAR[0x13490 + 0x002e] /* "Peter" */,
	/*D_421B*/&AVATAR[0x13496 + 0x002e] /* "Jumar" */,
	/*D_4221*/&AVATAR[0x13467 + 0x002e] /* "Hook" */,
	/*D_4226*/&AVATAR[0x134A1 + 0x002e] /* "Wendy" */
};

unsigned char * D_46BA[] = {
	&AVATAR[0x13935 + 0x002e] /*{1,  2,  3,  6}*/,
	&AVATAR[0x13939 + 0x002e] /*{5,  6,  8, 10}*/,
	&AVATAR[0x1393D + 0x002e] /*{4, 10, 11, 12}*/,
	&AVATAR[0x13941 + 0x002e] /*{4,  5,  6,  7}*/,
	&AVATAR[0x13945 + 0x002e] /*{8,  9, 13, 14}*/,
	&AVATAR[0x13949 + 0x002e] /*{2,  3,  7,  9}*/
};

/*prices[weapons]*/
short * D_46D2 = &AVATAR[0x1394D + 0x002e]; //{
#if 0
	   0,
	  20,
	   2,
	  25,
	 100,
	 225,
	 300,
	 250,
	 600,
	   5,
	 350,
	1500,
	2500,
	2000,
	5000,

	7000/*what is this price for?*/
};
#endif

/*shops indexes*/
unsigned char * D_46F2 = &AVATAR[0x1396D + 0x002e] /* {0, 0, 0, 0, 0, 1, 2, 0, 3, 4, 0, 0, 0, 5, 6, 0} */;

char *D_4702[] = {
	/*D_422C*/&AVATAR[0x134A7 + 0x002e] /* "We are the only staff makers in Britannia, yet sell them for only 20gp.\n" */,
	/*D_4275*/&AVATAR[0x134F0 + 0x002e] /* "We sell the most deadly of daggers, a bargain at only 2gp each.\n" */,
	/*D_42B6*/&AVATAR[0x13531 + 0x002e] /* "Our slings are made from only the finest gut and leather, 'Tis yours for 25gp.\n" */,
	/*D_4306*/&AVATAR[0x13581 + 0x002e] /* "These maces have a hardened shaft and a 5lb head fairly priced at 100gp.\n" */,
	/*D_4350*/&AVATAR[0x135CB + 0x002e] /* "Notice the fine workmanship on this axe, you'll agree 225gp is a good price.\n" */,
	/*D_439E*/&AVATAR[0x13619 + 0x002e] /* "The fine work on these swords will be the dread of thy foes, for 300gp.\n" */,
	/*D_43E7*/&AVATAR[0x13662 + 0x002e] /* "Our bows are made of finest yew, and the arrows willow, a steal at 250gp.\n" */,
	/*D_4432*/&AVATAR[0x136AD + 0x002e] /* "Crossbows made by Iolo the Bard are the finest in the world, yours for 600gp.\n" */,
	/*D_4481*/&AVATAR[0x136FC + 0x002e] /* "Flasks of oil make great weapons and creates a wall of flame too. 5gp each.\n" */,
	/*D_44CE*/&AVATAR[0x13749 + 0x002e] /* "A Halberd is a mighty weapon to attack over obstacles; a must and only 350gp.\n" */,
	/*D_451D*/&AVATAR[0x13798 + 0x002e] /* "This magical axe can be thrown at thy enemy and will then return all for 1500gp.\n" */,
	/*D_456F*/&AVATAR[0x137EA + 0x002e] /* "Magical swords such as these are rare indeed I will part with one for 2500gp.\n" */,
	/*D_45BE*/&AVATAR[0x13839 + 0x002e] /* "A magical bow will keep thy enemies far away or dead! A must for 2000gp!\n" */,
	/*D_4608*/&AVATAR[0x13883 + 0x002e] /* "This magic wand casts mighty blue bolts to strike down thy foes, 5000gp.\n" */
};

/*finalize deal[weapon]*/
C_CD1D(bp06, bp04)
int bp06;
int bp04;
{
// SHOP overflow bug fix
#if 0
	if (D_46D2[bp06] * bp04 > Party._gold) {
		u4_puts(/*D_4652*/&AVATAR[0x138CD + 0x002e] /* "I fear you have not the funds, perhaps something else.\n" */);
		add_npc_talk(VENDOR_WEAPON, &AVATAR[0x138CD + 0x002e] /* "I fear you have not the funds, perhaps something else." */);
		return 0;
}
#else
	register int si;
	/*Loop so it can't jump to an overflow and allow purchasing a lot of items for cheap. */
	for (si = 0; si <= bp04; si++) {
		if (D_46D2[bp06] * si > Party._gold) {
			u4_puts(/*D_4652*/&AVATAR[0x138CD + 0x002e] /* "I fear you have not the funds, perhaps something else.\n" */);
			add_npc_talk(VENDOR_WEAPON, &AVATAR[0x138CD + 0x002e] /* "I fear you have not the funds, perhaps something else." */);
			return 0;
		}
	}
#endif

	Party._gold -= D_46D2[bp06] * bp04; dspl_Gold();
	Party._weapons[bp06] += bp04;
	if(Party._weapons[bp06] > 99)
		Party._weapons[bp06] = 99;
	C_4832();
	Gra_CR();
	u4_puts(D_46AE[D_9142]);
	u4_puts(/*D_468A*/&AVATAR[0x13905 + 0x002e] /* " says: A fine choice!\n" */);
	add_npc_talk(VENDOR_WEAPON, &AVATAR[0x1390C + 0x002e] /* "A fine choice!" */);
}

/*choose item[weapon]*/
C_CD80()
{
	int loc_A, loc_B, loc_C;

	Gra_CR();
	u4_puts(/*D_471E*/&AVATAR[0x13999 + 0x002e] /* "\n\nVery Good!\n\n" */);
	add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13999 + 0x002e] /* "Very Good!" */);
	do {
		u4_puts(/*D_472D*/&AVATAR[0x139A8 + 0x002e] /* "We have:\n" */);
		add_npc_talk(VENDOR_WEAPON, &AVATAR[0x139A8 + 0x002e] /* "We have " */);
		for(loc_B = 0; loc_B < 4; loc_B ++) {
			u4_putc(D_46BA[D_9142][loc_B] + 'A');
			add_char_to_text_buffer(D_46BA[D_9142][loc_B] + 'A');
			u4_putc('-');
			add_char_to_text_buffer('-');
			u4_puts(D_1E98[37 + D_46BA[D_9142][loc_B]]);
			add_npc_talk(VENDOR_WEAPON, D_1E98[37 + D_46BA[D_9142][loc_B]]);
			u4_putc('s');
			add_char_to_text_buffer('s');
			add_npc_talk(VENDOR_WEAPON, "s");
			if (loc_B < 2)
			{
				add_npc_talk(VENDOR_WEAPON, ", ");
			}
			else if (loc_B == 2)
			{
				add_npc_talk(VENDOR_ARMOR, " and ");
			}
			else
			{
				add_npc_talk(VENDOR_ARMOR, ". ");
			}

			Gra_CR();
		}


		do {
			set_input_mode(INPUT_MODE_GENERAL_ASK_LETTER_WEAPON);
			add_npc_talk(VENDOR_WEAPON, &AVATAR[0x139B2 + 0x002e] /* "Your Interest?" */);
			loc_C = AskLetter(/*D_4737*/&AVATAR[0x139B2 + 0x002e] /* "Your Interest?\x12\x12\b" */, 'B', 'O');
			loc_C -= 'A';
			if(loc_C <= 0)
				break;
			for(loc_B = 0; loc_B < 4; loc_B ++) {
				if(D_46BA[D_9142][loc_B] == loc_C)
					break;
			}
			if(loc_B == 4)
				sound(1,0);
		} while(loc_B == 4);
		if(loc_C < 0)
			break;
		if(D_46D2[loc_C] > Party._gold) {
			u4_puts(/*D_4749*/&AVATAR[0x139C4 + 0x002e] /* "You have not the funds for even one!\n" */);
			add_npc_talk(VENDOR_WEAPON, &AVATAR[0x139C4 + 0x002e] /* "You have not the funds for even one!" */);
		} else {
			Gra_CR();
			u4_puts(D_4702[loc_C-1]);
			add_npc_talk(VENDOR_WEAPON, D_4702[loc_C-1]);
			Gra_CR();
			add_npc_talk(VENDOR_WEAPON, "\n");
			u4_puts(/*D_476F*/&AVATAR[0x13314 + 0x002e] /* "How many would you like?\x12\x12\x12\b\b" */);
			add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13314 + 0x002e] /* "How many would you like?" */);
			set_input_mode(INPUT_MODE_NUMBER_INPUT_2_DIGITS);
			loc_A = AskInt(2);
			if(loc_A <= 0) {
				if (loc_A == 0)
				{
					u4_puts(/*D_478D*/&AVATAR[0x13A08 + 0x002e] /* "\nToo bad.\n" */);
					add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13A09 + 0x002e] /* "Too bad." */);
				}
				else
					sound(1,0);
			} else {
				C_CD1D(loc_C, loc_A);
			}
		}
		u4_puts(/*D_4798*/&AVATAR[0x13A13 + 0x002e] /* "\nAnything else?\x12\x12\b" */);
		add_npc_talk(VENDOR_WEAPON, &AVATAR[0x1339C + 0x002e] /* "Anything else?" */);
		set_input_mode(INPUT_MODE_GENERAL_YES_NO);
		loc_C = (char)AskY_N();
	} while(loc_C == 'Y');
	Gra_CR();
}

#define clamp(a, b, c) if((a)+(b) > (c)) (a) = (c); else (a) += (b)

/*sell item[weapon]*/
C_CEBE()
{
	int loc_A, loc_B;
	char loc_C = 0;

	u4_puts(/*D_47AB*/&AVATAR[0x13A26 + 0x002e] /* "\nExcellent! Which wouldst\n" */);
	add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13A27 + 0x002e] /* "Excellent! Which wouldst" */);
	loc_B = -1;
	do {
		txt_Y = 23;
		txt_X = 24;
		set_input_mode(INPUT_MODE_GENERAL_ASK_LETTER_WEAPON);
		add_npc_talk(VENDOR_WEAPON, /*D_47C6*/&AVATAR[0x13A41 + 0x002e] /* "You sell:\x12\x12\b" */);
		loc_B = AskLetter(/*D_47C6*/&AVATAR[0x13A41 + 0x002e] /* "You sell:\x12\x12\b" */, 'B', 'P');
		loc_B -= 'A';
		Gra_CR();
		if(loc_B <= 0)
			break;
		if(Party._weapons[loc_B] == 0) {
			u4_puts(/*D_47D3*/&AVATAR[0x13A4E + 0x002e] /* "Thou dost not own that. What else might\n" */);
			add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13A4E + 0x002e] /* "Thou dost not own that. What else might" */);
		} else {
			if (Party._weapons[loc_B] > 1) {
				u4_puts(/*D_47FC*/&AVATAR[0x13A77 + 0x002e] /* "How many " */);
				add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13A77 + 0x002e] /* "How many " */);
				u4_puts(D_1E98[37 + loc_B]);
				add_npc_talk(VENDOR_WEAPON, D_1E98[37 + loc_B]);
				u4_puts(/*D_4806*/&AVATAR[0x13A81 + 0x002e] /* "s would you wish to sell?\x12\x12\x12\b\b" */);
				add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13A81 + 0x002e] /* "s would you wish to sell?" */);
				set_input_mode(INPUT_MODE_NUMBER_INPUT_2_DIGITS);
				loc_A = AskInt(2);
				if (loc_A <= 0) {
					if (loc_A == 0)
					{
						u4_puts(/*D_4825*/&AVATAR[0x13A08 + 0x002e] /* "\nToo bad.\n" */);
						add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13A09 + 0x002e] /* "Too bad." */);
					}
					else
						sound(1,0);
					break;
				}
				if (Party._weapons[loc_B] < loc_A) {
					u4_puts(/*D_4830*/&AVATAR[0x13AAB + 0x002e] /* "You don't have that many swine!\n" */);
					add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13AAB + 0x002e] /* "You don't have that many swine!" */);
					break;
				}
				u4_puts(/*D_4851*/&AVATAR[0x13ACC + 0x002e] /* "I will give you " */);
				add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13ACC + 0x002e] /* "I will give you " */);
				u4_putl(((long)loc_A * D_46D2[loc_B]) >> 1, 1, '0');
				add_npc_talk_long(VENDOR_WEAPON, ((long)loc_A * D_46D2[loc_B]) >> 1);
				u4_puts(/*D_4862*/&AVATAR[0x13ADD + 0x002e] /* "gp for " */);
				add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13ADD + 0x002e] /* "gold pieces for " */);
				if (loc_A > 1)
				{
					u4_puts(/*D_486A*/&AVATAR[0x13AE5 + 0x002e] /* "them." */);
					add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13AE5 + 0x002e] /* "them." */);
				}
				else
				{
					u4_puts(/*D_4870*/&AVATAR[0x13AEB + 0x002e] /* "it." */);
					add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13AEB + 0x002e] /* "it." */);
				}
			} else {
				loc_A = 1;
				u4_puts(/*D_4874*/&AVATAR[0x13ACC + 0x002e] /* "I will give you " */);
				add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13ACC + 0x002e] /* "I will give you " */);
				u4_putl((long)D_46D2[loc_B] >> 1, 1, '0');
				add_npc_talk_long(VENDOR_WEAPON, (long)D_46D2[loc_B] >> 1);
				u4_puts(/*D_4885*/&AVATAR[0x13B00 + 0x002e] /* "gp for that " */);
				add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13B00 + 0x002e] /* "gold pieces for that " */);
				u4_puts(D_1E98[37 + loc_B]);
				add_npc_talk(VENDOR_WEAPON, D_1E98[37 + loc_B]);
			}
			u4_putc(' ');
			add_char_to_text_buffer(' ');
			add_npc_talk(VENDOR_WEAPON, " ");
			u4_puts(/*D_4892*/&AVATAR[0x13B0D + 0x002e] /* "\nDeal?\x12\x12\b" */);
			add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13B0E + 0x002e] /* "Deal?" */);
			set_input_mode(INPUT_MODE_GENERAL_YES_NO);
			loc_C = AskY_N();
			if(loc_C < 'N')
				break;
			if(loc_C == 'N') {
				u4_puts(/*D_489C*/&AVATAR[0x13B17 + 0x002e] /* "Hmmph. What else would\n" */);
				add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13B17 + 0x002e] /* "Hmmph. What else would" */);
			} else {
				Party._weapons[loc_B] -= loc_A;
				clamp(Party._gold, (((long)loc_A * D_46D2[loc_B]) >> 1), 9999); dspl_Gold();
				C_4832();
				u4_puts(/*D_48B4*/&AVATAR[0x13B2F + 0x002e] /* "\nFine! What else?\n" */);
				add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13B30 + 0x002e] /* "Fine! What else?" */);
			}
		}
	} while(loc_C == 'Y' || loc_C == 'N');
}

/*SHOP #1 - weapons*/
/*C_D085*/SHP_weapon()
{
	char bp_02;

	Gra_13();
	C_4832();
	D_9142 = D_46F2[Party._loc - 1] - 1;
	u4_puts(/*D_48C7*/&AVATAR[0x13B42 + 0x002e] /* "\n\n\n\nWelcome to " */);
	add_npc_talk(VENDOR_WEAPON, &AVATAR[0x1324E + 0x002e] /* "Welcome to " */);
	u4_puts(D_46A2[D_9142]);
	add_npc_talk(VENDOR_WEAPON, D_46A2[D_9142]);	
	add_npc_talk(VENDOR_WEAPON, ". ");
	u4_puts(/*D_48D7*/&AVATAR[0x13B52 + 0x002e] /* "\n\n" */);
	u4_puts(D_46AE[D_9142]);
	u4_puts(/*D_48DA*/&AVATAR[0x13B55 + 0x002e] /* " says: Welcome friend! Art thou here to Buy or Sell? " */);
	add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13B5C + 0x002e] /* "Welcome friend! Art thou here to Buy or Sell?" */);
	do {
		set_input_mode(INPUT_MODE_GENERAL_BUY_SELL);
		switch(bp_02 = u_kbread()) {
			case 'B': case 'b': bp_02 = 'b'; break;
			case 'S': case 's': bp_02 = 's'; break;
			case ' ': case '\n': case '\r': case 0x1b:
				bp_02 = 0;
				Gra_CR();
			break;
			default:
				bp_02 = -1;
				sound(1,0);
		}
	} while(bp_02 == -1);
	if(bp_02) {
		u4_putc(bp_02);
		Gra_CR();
		if(bp_02 == 'b')
			C_CD80();
		else
			C_CEBE();
	}
	C_4649();
	Gra_13();
	dspl_Stats();
	Gra_CR();
	u4_puts(D_46AE[D_9142]);
	u4_puts(/*D_4910*/&AVATAR[0x13B8B + 0x002e] /* " says: Fare thee well!\n" */);
	add_npc_talk(VENDOR_WEAPON, &AVATAR[0x13B92 + 0x002e] /* "Fare thee well!" */);
	add_npc_talk(VENDOR_INVALID, "");
}

/*----------------------------------------*/

char *D_4BAE[] = {
	/*D_4928*/&AVATAR[0x13BA3 + 0x002e] /* "Windsor Armour" */,
	/*D_4937*/&AVATAR[0x13BB2 + 0x002e] /* "Valiant's Armour" */,
	/*D_4948*/&AVATAR[0x13BC3 + 0x002e] /* "Duelling Armour" */,
	/*D_4958*/&AVATAR[0x13BD3 + 0x002e] /* "Light Armour" */,
	/*D_4965*/&AVATAR[0x13BE0 + 0x002e] /* "Basic Armour" */
};

char *D_4BB8[] = {
	/*D_4972*/&AVATAR[0x13480 + 0x002e] /* "Winston" */,
	/*D_497A*/&AVATAR[0x13BB2 + 0x002e] /* "Valiant" */,
	/*D_4982*/&AVATAR[0x13BFD + 0x002e] /* "Jean" */,
	/*D_4987*/&AVATAR[0x13C02 + 0x002e] /* "Pierre" */,
	/*D_498E*/&AVATAR[0x13C09 + 0x002e] /* "Limpy" */,

	/*D_4994*/&AVATAR[0x13C0F + 0x002e] /* "Big John" *//*?!?who is that?!?*/
};

// armor shop inventory
unsigned char * D_4BC4[] = {
	&AVATAR[0x13E3F + 0x002e] /*{1, 2, 3, 0}*/,
	&AVATAR[0x13E43 + 0x002e] /*{3, 4, 5, 6}*/,
	&AVATAR[0x13E47 + 0x002e] /*{1, 3, 5, 0}*/,
	&AVATAR[0x13E4B + 0x002e] /*{1, 2, 0, 0}*/,
	&AVATAR[0x13E4F + 0x002e] /*{1, 2, 3, 0}*/,

	&AVATAR[0x13E53 + 0x002e] /*{0, 0, 0, 0}*/
};

/*prices[armour]*/
short * D_4BDC = &AVATAR[0x13E57 + 0x002e]; //{
#if 0
	   0,
	  50,
	 200,
	 600,
	2000,
	4000,
	7000,

	9000/*what is this price for?*/
};
#endif

/*shops indexes*/
unsigned char * D_4BEC = &AVATAR[0x13E67 + 0x002e] /*{0, 0, 0, 0, 0, 1, 2, 0, 0, 3, 0, 0, 4, 5, 0, 0}*/;

char *D_4BFC[] = {
	/*D_499D*/&AVATAR[0x13C18 + 0x002e] /* "Cloth Armour is good for a tight budget, Fairly priced at 50gp.\n" */,
	/*D_49DE*/&AVATAR[0x13C59 + 0x002e] /* "Leather Armour is both supple and strong, and costs a mere 200gp.\nA Bargain!\n" */,
	/*D_4A2C*/&AVATAR[0x13CA7 + 0x002e] /* "Chain Mail is the armour used by more warriors than all others. Ours costs 600gp.\n" */,
	/*D_4A7F*/&AVATAR[0x13CFA + 0x002e] /* "Full Plate armour is the ultimate in non-magical armour. Get yours for 2000gp.\n" */,
	/*D_4ACF*/&AVATAR[0x13D4A + 0x002e] /* "Magic Armour is rare and expensive.  This chain sells for 4000gp.\n" */,
	/*D_4B12*/&AVATAR[0x13D8D + 0x002e] /* "Magic Plate Armour is the best known protection. Only we have it. Cost: 7000gp.\n" */
};

/*finalize deal[armor]*/
C_D16D(bp06, bp04)
int bp06;
int bp04;
{
	if(D_4BDC[bp06] * bp04 > Party._gold) {
		u4_puts(/*D_4B63*/&AVATAR[0x13DDE + 0x002e] /* "You don't have enough gold.\nMaybe something cheaper?\n" */);
		add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13DDE + 0x002e] /* "You don't have enough gold.\nMaybe something cheaper?" */);
		return 0;
	}
	Party._gold -= D_4BDC[bp06] * bp04; dspl_Gold();
	Party._armors[bp06] += bp04;
	if(Party._armors[bp06] > 99)
		Party._armors[bp06] = 99;
	C_48F8();
	Gra_CR();
	u4_puts(D_4BB8[D_9142]);
	u4_puts(/*D_4B99*/&AVATAR[0x13E14 + 0x002e] /* " says: Good choice!\n" */);
	add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13E1B + 0x002e] /* "Good choice!" */);
}

/*choose item[armor]*/
C_D1D0()
{
	int loc_A, loc_B, loc_C;

	Gra_CR();
	u4_puts(/*D_4C08*/&AVATAR[0x13E83 + 0x002e] /* "\n\n\nWell then,\n" */);
	add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13E86 + 0x002e] /* "Well then," */);
	do {
		u4_puts(/*D_4C17*/&AVATAR[0x13E92 + 0x002e] /* "We've got:\n" */);
		add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13E92 + 0x002e] /* "We've got " */);
		for(loc_B = 0; loc_B < 4; loc_B ++) {
			if(D_4BC4[D_9142][loc_B]) {
				u4_putc(D_4BC4[D_9142][loc_B] + 'A');
				add_char_to_text_buffer(D_4BC4[D_9142][loc_B] + 'A');
				u4_putc(' ');
				add_char_to_text_buffer(' ');
				u4_puts(D_1E98[53 + D_4BC4[D_9142][loc_B]]);
				add_npc_talk(VENDOR_ARMOR, D_1E98[53 + D_4BC4[D_9142][loc_B]]);
				// at the end of 4, add a period
				if (loc_B == 3)
				{
					add_npc_talk(VENDOR_ARMOR, ". ");
				}
				// at the end of list indicated by 0, add a period
				else if (D_4BC4[D_9142][loc_B + 1] == 0)
				{
					add_npc_talk(VENDOR_ARMOR, ". ");
				}
				// just before the end add " and "
				else if (loc_B == 2)
				{
					add_npc_talk(VENDOR_ARMOR, " and ");
				}
				// just before the end indicated by 0 add " and "
				else if (D_4BC4[D_9142][loc_B + 2] == 0)
				{
					add_npc_talk(VENDOR_ARMOR, " and ");
				}
				// anywhere else add a ", "
				else
				{
					add_npc_talk(VENDOR_ARMOR, ", ");
				}

				Gra_CR();
			}
		}
		Gra_CR();
		do {
			set_input_mode(INPUT_MODE_GENERAL_ASK_LETTER_ARMOR);
			add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13E9E + 0x002e] /* "What'll it be?" */);
			loc_C = AskLetter(/*D_4C23*/&AVATAR[0x13E9E + 0x002e] /* "What'll it be?\x12\x12\b" */, 'B', 'G');
			loc_C -= 'A';
			if(loc_C <= 0)
				break;
			for(loc_B = 0; loc_B < 4; loc_B ++) {
				if(D_4BC4[D_9142][loc_B] == loc_C)
					break;
			}
			if(loc_B == 4)
				sound(1,0);
		} while(loc_B == 4);
		if(loc_C < 0)
			break;
		if(D_4BDC[loc_C] > Party._gold) {
			u4_puts(/*D_4C35*/&AVATAR[0x13EB0 + 0x002e] /* "What you try'n to pull? You can't pay!\n" */);
			add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13EB0 + 0x002e] /* "What you try'n to pull? You can't pay!" */);
		} else {
			Gra_CR();
			u4_puts(D_4BFC[loc_C-1]);
			add_npc_talk(VENDOR_ARMOR, D_4BFC[loc_C - 1]);
			Gra_CR();
			u4_puts(/*D_4C5D*/&AVATAR[0x13314 + 0x002e] /* "How many would you like?\x12\x12\x12\b\b" */);
			add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13314 + 0x002e] /* "How many would you like?" */);
			set_input_mode(INPUT_MODE_NUMBER_INPUT_2_DIGITS);
			loc_A = AskInt(2);
			if(loc_A <= 0) {

				u4_puts(/*D_4C7B*/&AVATAR[0x13A08 + 0x002e] /* "\nToo bad.\n" */);
				add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13A09 + 0x002e] /* "Too bad." */);


			} else {
				C_D16D(loc_C, loc_A);
			}
		}
		u4_puts(/*D_4C86*/&AVATAR[0x13A13 + 0x002e] /* "\nAnything else?\x12\x12\b" */);
		add_npc_talk(VENDOR_ARMOR, &AVATAR[0x1339C + 0x002e] /* "Anything else?" */);
		set_input_mode(INPUT_MODE_GENERAL_YES_NO);
		loc_C = (char)AskY_N();
	} while(loc_C == 'Y');

}

/*sell item[armor]*/
C_D2F8()
{
	int loc_A = 0, loc_B = 0;
	char loc_C = 0;

	u4_puts(/*D_4C99*/&AVATAR[0x13F14 + 0x002e] /* "\nWhat will\n" */);
	add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13F15 + 0x002e] /* "What will" */);
	loc_B = -1;
	do {
		txt_Y = 23;
		txt_X = 24; 
		set_input_mode(INPUT_MODE_GENERAL_ASK_LETTER_ARMOR);
		add_npc_talk(VENDOR_ARMOR, " ");
		add_npc_talk(VENDOR_ARMOR, /*D_4CA5*/&AVATAR[0x13A41 + 0x002e] /* "You sell:\x12\x12\b" */);
		loc_B = AskLetter(/*D_4CA5*/&AVATAR[0x13A41 + 0x002e] /* "You sell:\x12\x12\b" */, 'B', 'H');
		loc_B -= 'A';
		Gra_CR();
		if(loc_B <= 0)
			break;
		if(Party._armors[loc_B] == 0) {
			u4_puts(/*D_4CB2*/&AVATAR[0x13F2D + 0x002e] /* "Come on, you don't own any.\n" */);
			add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13F2D + 0x002e] /* "Come on, you don't own any." */);
		} else {
			if(Party._armors[loc_B] > 1) {
				u4_puts(/*D_4CCF*/&AVATAR[0x13F4A + 0x002e] /* "How many " */);
				add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13F4A + 0x002e] /* "How many " */);
				u4_puts(D_1E98[53 + loc_B]);
				add_npc_talk(VENDOR_ARMOR, D_1E98[53 + loc_B]);
				u4_puts(/*D_4CD9*/&AVATAR[0x13F54 + 0x002e] /* "s would you like to sell?\x12\x12\x12\b\b" */);
				add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13F54 + 0x002e] /* "s would you like to sell" */);
				set_input_mode(INPUT_MODE_NUMBER_INPUT_2_DIGITS);
				loc_A = AskInt(2);
				if(loc_A <= 0) {

					u4_puts(/*D_4CF8*/&AVATAR[0x13A08 + 0x002e] /* "\nToo bad.\n" */);
					add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13A09 + 0x002e] /* "Too bad." */);


					break;
				}
				if(Party._armors[loc_B] < loc_A) {
					u4_puts(/*D_4D03*/&AVATAR[0x13AAB + 0x002e] /* "You don't have that many swine!\n" */);
					add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13AAB + 0x002e] /* "You don't have that many swine!" */);
					break;
				}
				u4_puts(/*D_4D24*/&AVATAR[0x13F9F + 0x002e] /* "I'll give ya\n" */);
				add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13F9F + 0x002e] /* "I'll give ya" */);
				u4_putl(((long)loc_A * D_4BDC[loc_B]) >> 1, 1, '0');
				add_npc_talk_long(VENDOR_ARMOR, ((long)loc_A * D_4BDC[loc_B]) >> 1);
				u4_puts(/*D_4D32*/&AVATAR[0x13ADD + 0x002e] /* "gp for " */);
				add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13ADD + 0x002e] /* "gold pieces for " */);
				if (loc_A > 1)
				{
					u4_puts(/*D_4D3A*/&AVATAR[0x13AE5 + 0x002e] /* "them." */);
					add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13AE5 + 0x002e] /* "them." */);
				}
				else
				{
					u4_puts(/*D_4D40*/&AVATAR[0x13AEB + 0x002e] /* "it." */);
					add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13AEB + 0x002e] /* "it." */);
				}
			} else {
				loc_A = 1;
				u4_puts(/*D_4D44*/&AVATAR[0x13FBF + 0x002e] /* "I will give you\n" */);
				add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13ACC + 0x002e] /* "I will give you" */);
				u4_putl((long)D_4BDC[loc_B] >> 1, 1, '0');
				add_npc_talk_long(VENDOR_ARMOR, (long)D_4BDC[loc_B] >> 1);
				u4_puts(/*D_4D55*/&AVATAR[0x13B00 + 0x002e] /* "gp for that " */);
				add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13B00 + 0x002e] /* "gold pieces for that " */);
				u4_puts(D_1E98[53 + loc_B]);
				add_npc_talk(VENDOR_ARMOR, D_1E98[53 + loc_B]);
			}
			u4_puts(". ");
			add_npc_talk(VENDOR_ARMOR, ". ");
			u4_puts(/*D_4D62*/&AVATAR[0x13B0D + 0x002e] /* "\nDeal?\x12\x12\b" */);
			add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13B0E + 0x002e] /* "Deal?" */);
			set_input_mode(INPUT_MODE_GENERAL_YES_NO);
			loc_C = AskY_N();
			if(loc_C < 'N')
				break;
			if(loc_C == 'N') {
				u4_puts(/*D_4D6C*/&AVATAR[0x13FE7 + 0x002e] /* "Harumph. What else would\n" */);
				add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13FE7 + 0x002e] /* "Harumph. What else would" */);
			} else {
				Party._armors[loc_B] -= loc_A;
				clamp(Party._gold, (((long)loc_A * D_4BDC[loc_B]) >> 1), 9999); dspl_Gold();
				C_48F8();
				u4_puts(/*D_4D86*/&AVATAR[0x13B2F + 0x002e] /* "\nFine! What else?\n" */);
				add_npc_talk(VENDOR_ARMOR, &AVATAR[0x13B30 + 0x002e] /* "Fine! What else?" */);
			}
		}
	} while(loc_C == 'Y' || loc_C == 'N');
}

/*SHOP #2 - armor*/
/*C_D4AE*/SHP_armor()
{
	char bp_02;

	Gra_13();
	C_48F8();
	D_9142 = D_4BEC[Party._loc - 1] - 1;
	u4_puts(/*D_4D99*/&AVATAR[0x14014 + 0x002e] /* "\n\n\n\nWelcome to\n" */);
	add_npc_talk(VENDOR_ARMOR, &AVATAR[0x1324E + 0x002e] /* "Welcome to " */);
	u4_puts(D_4BAE[D_9142]);
	add_npc_talk(VENDOR_ARMOR, D_4BAE[D_9142]);
	add_npc_talk(VENDOR_ARMOR, ". ");
	u4_puts(/*D_4DA9*/&AVATAR[0x14024 + 0x002e] /* "\n\n" */);
	u4_puts(D_4BB8[D_9142]);
	u4_puts(/*D_4DAC*/&AVATAR[0x14027 + 0x002e] /* " says: Welcome friend! Want to Buy or Sell? " */);
	add_npc_talk(VENDOR_ARMOR, &AVATAR[0x1402D + 0x002e] /* " Welcome friend! Want to Buy or Sell? " */);
	do {
		set_input_mode(INPUT_MODE_GENERAL_BUY_SELL);
		switch(bp_02 = u_kbread()) {
			case 'B': case 'b': bp_02 = 'b'; break;
			case 'S': case 's': bp_02 = 's'; break;
			case ' ': case '\n': case '\r': case 0x1b:
				bp_02 = 0;
				Gra_CR();
			break;
			default:
				bp_02 = -1;
				sound(1,0);
		}
	} while(bp_02 == -1);
	if(bp_02) {
		u4_putc(bp_02);
		Gra_CR();
		if(bp_02 == 'b')
			C_D1D0();
		else
			C_D2F8();
	}
	C_4649();
	Gra_13();
	dspl_Stats();
	Gra_CR();
	u4_puts(D_4BB8[D_9142]);
	u4_puts(/*D_4DD9*/&AVATAR[0x14054 + 0x002e] /* " says: Good Bye.\n\0" */);/*TODO:0 is for padding*/
	add_npc_talk(VENDOR_ARMOR, &AVATAR[0x1405B + 0x002e] /* "Good Bye." */);
	add_npc_talk(VENDOR_INVALID, "");
}

/*----------------------------------------*/

/*SHOP #9 - horses*/
/*C_D596*/SHP_horse()
{
	char bp_02;

	u4_puts(/*D_4DEC*/&AVATAR[0x14067 + 0x002e] /* "\nWelcome friend! Can I interest thee in horses?\x12\x12\b" */);
	add_npc_talk(VENDOR_HORSE, &AVATAR[0x14068 + 0x002e] /* "Welcome friend! Can I interest thee in horses?" */);
	set_input_mode(INPUT_MODE_GENERAL_YES_NO);
	bp_02 = AskY_N();
	if(bp_02 != 'Y') 
	{
		if (bp_02 == 'N')
		{
			add_npc_talk(VENDOR_HORSE, &AVATAR[0x1409B + 0x002e] /* "A shame, thou looks like thou could use a good horse!" */);
			u4_puts(/*D_4E1F*/&AVATAR[0x1409A + 0x002e] /* "\nA shame, thou looks like thou could use a good horse!\n" */);
		}
		add_npc_talk(VENDOR_HORSE, &AVATAR[0x1409B + 0x002e] /* "A shame, thou looks like thou could use a good horse!" */);
		add_npc_talk(VENDOR_INVALID, "");
		return 0;
	}
	u4_puts(/*D_4E57*/&AVATAR[0x140D2 + 0x002e] /* "\nFor only " */);
	add_npc_talk(VENDOR_HORSE, &AVATAR[0x140D3 + 0x002e] /* "For only " */);
	u4_putl(Party.f_1d8, 1, '0');
	add_npc_talk_long(VENDOR_HORSE, Party.f_1d8);
	u4_puts(/*D_4E62*/&AVATAR[0x140DD + 0x002e] /* "00g.p. Thou can have the best!  Wilt thou buy?\x12\x12\b" */);
	add_npc_talk(VENDOR_HORSE, &AVATAR[0x140DD + 0x002e] /* "00 gold pieces. Thou can have the best!  Wilt thou buy?" */);
	set_input_mode(INPUT_MODE_GENERAL_YES_NO);
	bp_02 = AskY_N();
	if(bp_02 != 'Y') 
	{
		if (bp_02 == 'N')
		{
			u4_puts(/*D_4E94*/&AVATAR[0x1409A + 0x002e] /* "\nA shame, thou looks like thou could use a good horse!\n" */);
			add_npc_talk(VENDOR_HORSE, &AVATAR[0x1409B + 0x002e] /* "A shame, thou looks like thou could use a good horse!" */);
		}
		add_npc_talk(VENDOR_INVALID, "");
		return 0;
	}
	if(100 * Party.f_1d8 > Party._gold) {
		u4_puts(/*D_4ECC*/&AVATAR[0x14147 + 0x002e] /* "\nIt seems thou hast not gold enough to pay!\n" */);
		add_npc_talk(VENDOR_HORSE, &AVATAR[0x14148 + 0x002e] /* "It seems thou hast not gold enough to pay!" */);
		add_npc_talk(VENDOR_INVALID, "");
		return 0;
	}
	Party._gold -= 100 * Party.f_1d8; dspl_Gold();
	u4_puts(/*D_4EF9*/&AVATAR[0x14174 + 0x002e] /* "\nHere, a better breed thou shalt not find ever!\n" */);
	add_npc_talk(VENDOR_HORSE, &AVATAR[0x14175 + 0x002e] /* "Here, a better breed thou shalt not find ever!" */);
	Party._tile  = TIL_14;
	add_npc_talk(VENDOR_INVALID, "");
}

/*----------------------------------------*/

/*shops indexes*/
unsigned char * D_5196 = &AVATAR[0xF27B + 0x0005196 + 0x002e] /* {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 0} */;  // doesn't match exe, need to declare it here  61 74 69 65 21 0A A5 21 B2 00 0F 00 B0 01 02 00

/*items prices*/
short * D_51A6 = &AVATAR[0xF273 + 0x00051A6 + 0x0036] /* {50, 60, 60, 900} */;
/*items quantities*/
short * D_51AE = &AVATAR[0xF273 + 0x00051ae + 0x0036] /*{5,  5,  6,   1}*/;

char *D_51B6[] = {
	/*D_4F2A*/&AVATAR[0xF27B + 0x4F2A + 0x002e] /* "Pirate's Guild" */,
	/*D_4F39*/&AVATAR[0x141B4 + 0x002e] /* "The Guild Shop" */
};
char *D_51BA[] = {
	/*D_4F48*/&AVATAR[0x141C3 + 0x002e] /* "One Eyed Willey" */,
	/*D_4F58*/&AVATAR[0x141D3 + 0x002e] /* "Long John Leary" */
};

char *D_51BE[] = {
	/*D_4F68*/&AVATAR[0x141E3 + 0x002e] /* "I can give ya 5 long lasting Torches for a mere 50gp.\n" */,
	/*D_4F9F*/&AVATAR[0x1421A + 0x002e] /* "I've got magical mapping Gems, 5 for only 60gp.\n" */,
	/*D_4FD0*/&AVATAR[0x1424B + 0x002e] /* "Magical Keys 1 use each, a fair price at 60gp for 6.\n" */,
	/*D_5006*/&AVATAR[0x14281 + 0x002e] /* "So...Ya want a Sextant...Well I gots one which I might part with fer 900 gold!\n" */
};

/*SHOP #8 - guild*/
/*C_D61E*/SHP_guild()
{
	int loc_A = 0;
	char loc_B = 0;
	U16 *loc_C = 0;

	D_9142 = D_5196[Party._loc - 1] - 1;
	Gra_13();
	C_4987();
	u4_puts(/*D_5056*/&AVATAR[0x142D1 + 0x002e] /* "\n\n\n\nAvast ye mate! Shure ye wishes to buy from ol' " */);
	add_npc_talk(VENDOR_GUILD, &AVATAR[0x142D5 + 0x002e] /* "Avast ye mate! Shure ye wishes to buy from ol'" */);
	u4_puts(D_51BA[D_9142]);
	add_npc_talk(VENDOR_GUILD, D_51BA[D_9142]);
	u4_puts(/*D_508A*/&AVATAR[0x14305 + 0x002e] /* "?\n\n\n" */);
	add_npc_talk(VENDOR_GUILD, "?");
	u4_puts(D_51BA[D_9142]);
	u4_puts(/*D_508F*/&AVATAR[0x13247 + 0x002e] /* " says: Welcome to " */); 
	add_npc_talk(VENDOR_GUILD, &AVATAR[0x1324E + 0x002e] /* "Welcome to " */);
	u4_puts(D_51B6[D_9142]);
	add_npc_talk(VENDOR_GUILD, D_51B6[D_9142]);
	u4_puts(". ");
	add_npc_talk(VENDOR_GUILD, ". ");
	u4_puts(/*D_50A2*/&AVATAR[0x1431D + 0x002e] /* "\nLike to see my goods?\x12\x12\b" */);
	add_npc_talk(VENDOR_GUILD, &AVATAR[0x1431E + 0x002e] /* "Like to see my goods?" */);
	set_input_mode(INPUT_MODE_GENERAL_YES_NO);
	while((loc_B = AskY_N()) == 'Y') {
		Gra_CR();
		u4_puts(D_51BA[D_9142]);
		u4_puts(/*D_50BC*/ &AVATAR[0x14337 + 0x002e] /* " says: Good Mate! Ya see I gots:\n\
A-Torches\n\
B-Magic Gems\n\
C-Magic Keys\n"*/);
		// Need to replace this text for voice
		add_npc_talk(VENDOR_GUILD, /* &AVATAR[0x14337 + 0x002e] */ "Good Mate! Ya see I gots Torches, Magic Gems and Magic Keys. ");
		add_npc_talk(VENDOR_GUILD, &AVATAR[0x1437D + 0x002e] /* "Wat'l it be?" */);
		set_input_mode(INPUT_MODE_GENERAL_ASK_LETTER_GUILD);
		loc_A = AskLetter(/*D_5102*/&AVATAR[0x1437D + 0x002e] /* "Wat'l it be?\x12\x12\b" */, 'A', 'D') - 'A';
		if(loc_A < 0)
			break;
		Gra_CR();
		u4_puts(D_51BE[loc_A]);
		add_npc_talk(VENDOR_GUILD, D_51BE[loc_A]);
		u4_puts(/*D_5112*/&AVATAR[0x1438D + 0x002e] /* "\nWill ya buy?\x12\x12\b" */);
		add_npc_talk(VENDOR_GUILD, &AVATAR[0x1438D + 0x002e] /* ". Will ya buy?" */);
		set_input_mode(INPUT_MODE_GENERAL_YES_NO);
		loc_B = AskY_N();
		if(loc_B != 'Y') {
			if (loc_B == 'N')
			{
				u4_puts(/*D_5123*/&AVATAR[0x1439E + 0x002e] /* "\nHmmm...Grmbl...\n" */);
				add_npc_talk(VENDOR_GUILD, &AVATAR[0x1439F + 0x002e] /* "Hmmm...Grmbl..." */);
			}
			break;
		}
		if(Party._gold < D_51A6[loc_A]) {
			Gra_13();
			C_4649();
			dspl_Stats();
			u4_puts(/*D_5135*/&AVATAR[0x143B0 + 0x002e] /* "\nWhat? Can't pay! Buzz off swine!\n" */);
			add_npc_talk(VENDOR_GUILD, &AVATAR[0x143B1 + 0x002e] /* "What? Can't pay! Buzz off swine!" */);
			add_npc_talk(VENDOR_INVALID, "");
			return 0;
		}
		switch(loc_A) {
			case 0: loc_C = &(Party._torches); break;
			case 1: loc_C = &(Party._gems); break;
			case 2: loc_C = &(Party._keys); break;
			case 3: loc_C = &(Party._sextants); break;
			default:loc_C = &(Party._torches); break;
		}
		*loc_C += D_51AE[loc_A];
		if(*loc_C > 99)
			*loc_C = 99;
		Party._gold -= D_51A6[loc_A];
		u4_puts(/*D_5158*/&AVATAR[0x143D3 + 0x002e] /* "\nFine...fine...\n\n" */);
		add_npc_talk(VENDOR_GUILD, &AVATAR[0x143D4 + 0x002e] /* "Fine...fine..." */);
		dspl_Gold();
		C_4987();
		Gra_CR();
		u4_puts(D_51BA[D_9142]);
		u4_puts(/*D_516A*/&AVATAR[0x143E5 + 0x002e] /* " says: See more?\x12\x12\b" */);
		add_npc_talk(VENDOR_GUILD, &AVATAR[0x143EC + 0x002e] /* "See more?" */);
		set_input_mode(INPUT_MODE_GENERAL_YES_NO);
	}
	Gra_13();
	C_4649();
	dspl_Stats();
	Gra_CR();
	u4_puts(D_51BA[D_9142]);
	u4_puts(/*D_517E*/&AVATAR[0x143F9 + 0x002e] /* " says:  See ya matie!\n" */);
	//add_npc_talk(VENDOR_GUILD, &AVATAR[0x14401 + 0x002e] /* "See ya matie!" */);
	add_npc_talk(VENDOR_GUILD, "See yah matey!" ); // try a different spelling to see if it is better
	add_npc_talk(VENDOR_INVALID, "");
}

/*----------------------------------------*/

/*shops indexes*/
unsigned char * D_5484 = &AVATAR[0x146F7 + 0x0036] /* {0, 0, 0, 0, 1, 2, 3, 0, 4, 5, 6, 0, 0, 0, 7, 0} */;

unsigned char * D_5494 = &AVATAR[0x14707 + 0x0036] /* {0x1C, 0x1D, 0x0A, 0x02, 0x1D, 0x1C, 0x19, 0} */;
unsigned char * D_549C = &AVATAR[0x1470F + 0x0036] /* {0x06, 0x06, 0x1A, 0x06, 0x02, 0x0B, 0x17, 0} */;
unsigned char * D_54A4 = &AVATAR[0x14717 + 0x0036] /* {  20,   15,   10,   30,   15,    5,    1, 0} */;

char *D_54AC[] = {
	/*D_51C6*/&AVATAR[0x14439 + 0x0036] /* "The Honest Inn" */,
	/*D_51D5*/&AVATAR[0x14448 + 0x0036] /* "Britannia Manor" */,
	/*D_51E5*/&AVATAR[0x14458 + 0x0036] /* "The Inn of Ends" */,
	/*D_51F5*/&AVATAR[0x14468 + 0x0036] /* "Wayfarer's Inn" */,
	/*D_5204*/&AVATAR[0x14477 + 0x0036] /* "Honorable Inn" */,
	/*D_5212*/&AVATAR[0x14485 + 0x0036] /* "The Inn of the Spirits" */,
	/*D_5229*/&AVATAR[0x1449C + 0x0036] /* "The Sleep Shop" */
};

char *D_54BA[] = {
	/*D_5238*/&AVATAR[0x144AB + 0x0036] /* "Scatu" */,
	/*D_523E*/&AVATAR[0x144B1 + 0x0036] /* "Jason" */,
	/*D_5244*/&AVATAR[0x144B7 + 0x0036] /* "Smirk" */,
	/*D_524A*/&AVATAR[0x144BD + 0x0036] /* "Estro" */,
	/*D_5250*/&AVATAR[0x144C3 + 0x0036] /* "Zajac" */,
	/*D_5256*/&AVATAR[0x144C9 + 0x0036] /* "Tyrone" */,
	/*D_525D*/&AVATAR[0x144D0 + 0x0036] /* "Tymus" */
};

char *D_54C8[] = {
	/*D_5263*/&AVATAR[0x144D6 + 0x0036] /* "We have a room with 2 beds that rents for 20gp." */,
	/*D_5293*/&AVATAR[0x14506 + 0x0036] /* "We have a modest sized room with 1 bed for 15gp." */,
	/*D_52C4*/&AVATAR[0x14537 + 0x0036] /* "We have a very secure room of modest size and 1 bed for 10gp." */,
	/*D_5302*/&AVATAR[0x14575 + 0x0036] /* "We have three rooms available, a 1, 2 and 3 bed room for 30, 60 and 90gp each." */,
	/*D_5351*/&AVATAR[0x145C4 + 0x0036] /* "We have a single bed room with a back door for 15g." */,
	/*D_5385*/&AVATAR[0x145F8 + 0x0036] /* "Unfortunately, I have but only a very small room with 1 bed: worse yet, it is haunted!  If you do wish to stay it costs 5gp." */,
	/*D_5402*/&AVATAR[0x14675 + 0x0036] /* "All we have is that cot over there. But it is comfortable, and only 1gp." */
};

C_D7A8()
{
	int bp_02;

	u4_puts(/*D_544B*/&AVATAR[0x146BE + 0x0036] /* "\nIn the middle of the night, while out for a stroll...\n" */);
	bp_02 = CurMode;
	CurMode = MOD_COM_CAMP;
	D_96F8 = D_9452 = TIL_C8;
	C_7D92();
	CurMode = bp_02;
}

C_D7D6()
{
	int bp_02;

	set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
	u_delay(1, 0);
	Party._x = D_913B;
	Party._y = D_913C;
	set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
	u_delay(1, 0);
	for(bp_02 = Party.f_1d8 - 1; bp_02 >= 0; bp_02 --) {
		if(isCharaAlive(bp_02))
			Party.chara[bp_02]._status = 'S';
	}
	dspl_Stats();
	Party._tile = TIL_38;
	set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
	u_delay(5, 0);
	Party._tile = TIL_1F;
	for(bp_02 = Party.f_1d8 - 1; bp_02 >= 0; bp_02 --) {
		if(Party.chara[bp_02]._status == 'S') {
			Party.chara[bp_02]._status = 'G';
			HP_inc(bp_02, U4_RND3(50) * 2 + 100);
		}
	}
	MP_recover();
	dspl_Stats();
	if(isCharaAlive(0) && U4_RND1(7) == 0) {
		C_D7A8();
	} else if(D_9142 == 5 && U4_RND1(3) == 0) {
		/*Isaac the ghost appears in skara brae*/
		D_8742._npc._tile[0] = D_8742._npc._gtile[0] = TIL_9C;
		D_8742._npc._x[0] = Party._x - 1;
		D_8742._npc._y[0] = Party._y;
		D_8742._npc._var[0] = 1;
		D_8742._npc._tlkidx[0] = 0x10;
	}
	u4_puts(/*D_54D6*/&AVATAR[0x14749 + 0x0036] /* "\nMorning!\n" */);
}

unsigned char * D_5672 = &AVATAR[0x148E5 + 0x0036] /*{ 2, 2, 8, 0}*/;
unsigned char * D_5676 = &AVATAR[0x148E9 + 0x0036] /*{ 6, 2, 2, 0}*/;
unsigned char * D_567A = &AVATAR[0x148ED + 0x0036] /*{30,60,90, 0}*/;

/*SHOP #7 - inn*/
/*C_D8DD*/SHP_inn()
{
	int bp_02;
	char bp_04;

	if(Party._tile != TIL_1F) {
		u4_puts(/*D_54E1*/&AVATAR[0x14754 + 0x0036] /* "The Innkeeper says: Get that horse out of here!!!\n" */);
		add_npc_talk(VENDOR_INN, &AVATAR[0x14768 + 0x0036] /* "Get that horse out of here!!!" */);
		add_npc_talk(VENDOR_INVALID, "");
		return 0;
	}
	u4_puts(/*D_5514*/&AVATAR[0x14787 + 0x0036] /* "The Innkeeper says: Welcome to " */);
	add_npc_talk(VENDOR_INN, &AVATAR[0x1324E + 0x002e] /* "Welcome to " */);
	D_9142 = D_5484[Party._loc - 1] - 1;
	u4_puts(D_54AC[D_9142]);
	add_npc_talk(VENDOR_INN, D_54AC[D_9142]);
	u4_puts(". ");
	add_npc_talk(VENDOR_INN, ". ");
	u4_puts(/*D_5534*/  &AVATAR[0x11D6A + 0x0017] /* "I am " */);
	add_npc_talk(VENDOR_INN, &AVATAR[0x11D6A + 0x0017] /* "I am " */);
	u4_puts(D_54BA[D_9142]);
	add_npc_talk(VENDOR_INN, D_54BA[D_9142]);	
	u4_puts(". ");
	add_npc_talk(VENDOR_INN, ". ");
	u4_puts(/*D_553C*/ &AVATAR[0x147B0 + 0x0036] /* ".\n\n Are you in need of lodging?\x12\x12\b" */);
	add_npc_talk(VENDOR_INN, &AVATAR[0x147B2 + 0x0036] /* "Are you in need of lodging?" */);
	set_input_mode(INPUT_MODE_GENERAL_YES_NO);
	bp_04 = AskY_N();
	if (bp_04 == 'Y') {
		Gra_CR();
		u4_puts(D_54C8[D_9142]);
		add_npc_talk(VENDOR_INN, D_54C8[D_9142]);
		Gra_CR();
		if (D_9142 == 3) {
			add_npc_talk(VENDOR_INN, &AVATAR[0x147D1 + 0x0036] /* "1, 2 or 3 beds?" */);
			set_input_mode(INPUT_MODE_GENERAL_ASK_NUMBER_OF_BEDS);
			bp_02 = AskLetter(/*D_555E*/&AVATAR[0x147D1 + 0x0036] /* "1, 2 or 3 beds?\x12\x12\b" */, '1', '3');
			bp_02 -= '1';
			if (bp_02 < 0)
			{
				add_npc_talk(VENDOR_INVALID, "");
				return 0;
			}
			D_913B = D_5672[bp_02];
			D_913C = D_5676[bp_02];
			D_913A = D_567A[bp_02];
		}
		else {
			u4_puts(/*D_5571*/&AVATAR[0x147E4 + 0x0036] /* "\nTake it?\x12\x12\b" */);
			add_npc_talk(VENDOR_INN, &AVATAR[0x147E5 + 0x0036] /* "Take it?" */);
			set_input_mode(INPUT_MODE_GENERAL_YES_NO);
			bp_04 = AskY_N();
			if (bp_04 != 'Y') {
				if (bp_04 == 'N')
				{
					u4_puts(/*D_557E*/&AVATAR[0x147F1 + 0x0036] /* "\nYou won't find a better deal in this towne!\n" */);
					add_npc_talk(VENDOR_INN, &AVATAR[0x147F2 + 0x0036] /* "You won't find a better deal in this towne!" */);
				}
				add_npc_talk(VENDOR_INVALID, "");
				return 0;
			}
			D_913B = D_5494[D_9142];
			D_913C = D_549C[D_9142];
			D_913A = D_54A4[D_9142];
		}
		if (D_913A > Party._gold) {
			u4_puts(/*D_55AC*/&AVATAR[0x1481F + 0x0036] /* "\nIf you can't pay, you can't stay! Good Bye.\n" */);
			add_npc_talk(VENDOR_INN, &AVATAR[0x14820 + 0x0036] /* "If you can't pay, you can't stay! Good Bye." */);
			add_npc_talk(VENDOR_INVALID, "");
			return 0;
		}
		Party._gold -= D_913A; dspl_Gold();
		u4_puts(/*D_55DA*/&AVATAR[0x1484D + 0x0036] /* "\n\nVery good.  Have a pleasant night.\n" */);
		add_npc_talk(VENDOR_INN, &AVATAR[0x1484F + 0x0036] /* "Very good.  Have a pleasant night." */);
		if (U4_RND1(3) == 0)
		{
			u4_puts(/*D_5600*/&AVATAR[0x14873 + 0x0036] /* "\nOh, and don't mind the strange noises, it's only rats!\n" */);
			add_npc_talk(VENDOR_INN, &AVATAR[0x14874 + 0x0036] /* "Oh, and don't mind the strange noises, it's only rats!" */);
		}
		C_D7D6();
		} else  {
		Gra_CR();
		u4_puts(D_54BA[D_9142]);
		u4_puts(/*D_5639*/&AVATAR[0x148AC + 0x0036] /* " says: Then you have come to the wrong place! Good Day.\n" */);
		add_npc_talk(VENDOR_INN, &AVATAR[0x148B3 + 0x0036] /* "Then you have come to the wrong place! Good Day." */);
	}
	add_npc_talk(VENDOR_INVALID, "");
}

/*----------------------------------------*/

/*shops indexes*/
unsigned char * D_5788 = &AVATAR[0x149FB + 0x0036] /*{1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 9, 0, 0, 0, 0, 10}*/;

char *D_5798[] = {
	/*D_567E*/&AVATAR[0x148F1 + 0x0036] /* "The Royal Healer" */,
	/*D_568F*/&AVATAR[0x14902 + 0x0036] /* "The Truth Healer" */,
	/*D_56A0*/&AVATAR[0x14913 + 0x0036] /* "The Love Healer" */,
	/*D_56B0*/&AVATAR[0x14923 + 0x0036] /* "Courage Healer" */,
	/*D_56BF*/&AVATAR[0x14932 + 0x0036] /* "The Healer" */,
	/*D_56CA*/&AVATAR[0x1493D + 0x0036] /* "Wound Healing" */,
	/*D_56D8*/&AVATAR[0x1494B + 0x0036] /* "Heal and Health" */,
	/*D_56E8*/&AVATAR[0x1495B + 0x0036] /* "Just Healing" */,
	/*D_56F5*/&AVATAR[0x14968 + 0x0036] /* "The Mystic Heal" */,
	/*D_5705*/&AVATAR[0x14978 + 0x0036] /* "The Healer Shop" */
};

char *D_57AC[] = {
	/*D_5715*/&AVATAR[0x14988 + 0x0036] /* "Pendragon" */,
	/*D_571F*/&AVATAR[0x14992 + 0x0036] /* "Starfire" */,
	/*D_5728*/&AVATAR[0x1499B + 0x0036] /* "Salle'" */,
	/*D_572F*/&AVATAR[0x149A2 + 0x0036] /* "Windwalker" */,
	/*D_573A*/&AVATAR[0x149AD + 0x0036] /* "Harmony" */,
	/*D_5742*/&AVATAR[0x149B5 + 0x0036] /* "Celest" */,
	/*D_5749*/&AVATAR[0x149BC + 0x0036] /* "Triplet" */,
	/*D_5751*/&AVATAR[0x149C4 + 0x0036] /* "Justin" */,
	/*D_5758*/&AVATAR[0x149CB + 0x0036] /* "Spiran" */,
	/*D_575F*/&AVATAR[0x149D2 + 0x0036] /* "Quat" */
};

C_DA05()
{
	register int si;

	Gra_CR();
	u4_puts(D_57AC[D_9142]);
	u4_puts(/*D_5764*/&AVATAR[0x149D7 + 0x0036] /* " asks:\n" */);
	set_input_mode(INPUT_MODE_GENERAL_ASK_CHARACTER_NUMBER);
	add_npc_talk(VENDOR_HEALER, &AVATAR[0x149DF + 0x0036] /* "Who is in need?" */);
	si = AskChara(/*D_576C*/&AVATAR[0x149DF + 0x0036] /* "Who is in need?\x12\x12\b" */);
	if (si < 0)
	{
		u4_puts(/*D_577F*/&AVATAR[0x149F2 + 0x0036] /* "No one?\n" */);
		add_npc_talk(VENDOR_HEALER, &AVATAR[0x149F2 + 0x0036] /* "No one?" */);
	}
	return si;
}

C_DA3E(bp04)
int bp04;
{
	char bp_02;

	u4_puts(/*D_57C0*/&AVATAR[0x14A33 + 0x0036] /* "\nWilt thou pay?\x12\x12\b" */);
	add_npc_talk(VENDOR_HEALER, &AVATAR[0x14A34 + 0x0036] /* "Wilt thou pay?" */);
	set_input_mode(INPUT_MODE_GENERAL_YES_NO);
	bp_02 = AskY_N();
	if(bp_02 == 'Y') {
		Party._gold -= bp04; dspl_Gold();
		return 1;
	}
	if (bp_02 == 'N')
	{
		u4_puts(/*D_57D3*/&AVATAR[0x14A46 + 0x0036] /* "\nThen I cannot aid thee.\n" */);
		add_npc_talk(VENDOR_HEALER, &AVATAR[0x14A47 + 0x0036] /* "Then I cannot aid thee." */);
	}

	return 0;
}

C_DA79(bp04)
int bp04;
{
	Gra_09();
	Gra_11(bp04);
	sound(9, 0xc0);
	Gra_11(bp04);
	Gra_09();
}

/*Curing*/
C_DAA2()
{
	int bp_02;

	bp_02 = C_DA05();
	if(bp_02 == -1)
		return 0;
	if(Party.chara[bp_02]._status != 'P') {
		u4_puts(/*D_57ED*/&AVATAR[0x14A60 + 0x0036] /* "Thou suffers not from Poison!\n" */);
		add_npc_talk(VENDOR_HEALER, &AVATAR[0x14A60 + 0x0036] /* "Thou suffers not from Poison!" */);
		return 0;
	}
	u4_puts(/*D_580C*/&AVATAR[0x14A7F + 0x0036] /* "A curing will cost thee 100gp.\n" */);
	add_npc_talk(VENDOR_HEALER, &AVATAR[0x14A7F + 0x0036] /*"A curing will cost thee 100 gold pieces." */);
	if(Party._gold < 100) {
		u4_puts(/*D_582C*/&AVATAR[0x14A9F + 0x0036] /* "\nI see by thy purse that thou hast not enough gold." */);
		add_npc_talk(VENDOR_HEALER, &AVATAR[0x14AA0 + 0x0036] /* "I see by thy purse that thou hast not enough gold." */);
		set_input_mode(INPUT_MODE_DELAY_CONTINUE);
		u_kbflush();
		u_delay(5, 1);
		if(u_kbhit())
			u_kbread();
		u4_puts(/*D_5860*/&AVATAR[0x14AD3 + 0x0036] /* "\n\nI will cure thee for free, but give unto others whenever thou may!\n" */);
		add_npc_talk(VENDOR_HEALER, &AVATAR[0x14AD5 + 0x0036] /* "I will cure thee for free, but give unto others whenever thou may!" */);
	} else {
		if (!C_DA3E(100))
		{
			return 0;
		}
	}
	Party.chara[bp_02]._status = 'G';
	C_DA79(bp_02);
	dspl_Stats();
}

/*Healing*/
C_DB29()
{
	int bp_02;

	bp_02 = C_DA05();
	if(bp_02 == -1)
		return 0;
	if(Party.chara[bp_02]._HP[0] == Party.chara[bp_02]._HP[1]) {
		u4_puts(/*D_58A6*/&AVATAR[0x14B19 + 0x0036] /* "Thou art already quite healthy!\n" */);
		add_npc_talk(VENDOR_HEALER, &AVATAR[0x14B19 + 0x0036] /* "Thou art already quite healthy!" */);
		return 0;
	}
	u4_puts(/*D_58C7*/&AVATAR[0x14B3A + 0x0036] /* "\nA healing will cost thee 200gp.\n" */);
	add_npc_talk(VENDOR_HEALER, &AVATAR[0x14B3A + 0x0036] /*"A healing will cost thee 200 gold pieces." */);
	if(Party._gold < 200) {
		u4_puts(/*D_58E9*/&AVATAR[0x14B5C + 0x0036] /* "\nI see by thy purse that thou hast not enough gold. I cannot aid thee.\n" */);
		add_npc_talk(VENDOR_HEALER, &AVATAR[0x14B5D + 0x0036] /* "I see by thy purse that thou hast not enough gold. I cannot aid thee." */);
		return 0;
	}
	if(C_DA3E(200)) {
		Party.chara[bp_02]._HP[0] = Party.chara[bp_02]._HP[1];
		C_DA79(bp_02);
		dspl_Stats();
	}
}

/*Resurrection*/
C_DB93()
{
	int bp_02;

	bp_02 = C_DA05();
	if(bp_02 == -1)
		return 0;
	if(Party.chara[bp_02]._status != 'D') {
		u4_puts(/*D_5931*/&AVATAR[0x14BA4 + 0x0036] /* "Thou art not dead fool!\n" */);
		add_npc_talk(VENDOR_HEALER, &AVATAR[0x14BA4 + 0x0036] /* "Thou art not dead fool!" */);
		return 0;
	}
	u4_puts(/*D_594A*/&AVATAR[0x14BBD + 0x0036] /* "Resurrection will cost thee 300gp.\n" */);
	add_npc_talk(VENDOR_HEALER, &AVATAR[0x14BBD + 0x0036] /* "Resurrection will cost thee 300 gold pieces." */);
	if(Party._gold < 300) {
		u4_puts(/*D_596E*/&AVATAR[0x14B5C + 0x0036] /* "\nI see by thy purse that thou hast not enough gold. I cannot aid thee.\n" */);
		add_npc_talk(VENDOR_HEALER, &AVATAR[0x14B5D + 0x0036] /* "I see by thy purse that thou hast not enough gold. I cannot aid thee." */);
		return 0;
	}
	if(C_DA3E(300)) {
		Party.chara[bp_02]._status = 'G';
		C_DA79(bp_02);
		dspl_Stats();
	}
}

/*blood donation*/
C_DBF5()
{
	char bp_04;
	int dummy;

	u4_puts(/*D_59B6*/&AVATAR[0x14C29 + 0x0036] /* "Art thou willing to give 100pts of thy blood to aid others?\x12\x12\b" */);
	add_npc_talk(VENDOR_HEALER, &AVATAR[0x14C29 + 0x0036] /* "Art thou willing to give 100pts of thy blood to aid others?" */);
	set_input_mode(INPUT_MODE_GENERAL_YES_NO);
	bp_04 = AskY_N();
	if(bp_04 == 'Y') {
		karma_inc(&(Party._sacri), 5);
		u4_puts(/*D_59F5*/&AVATAR[0x14C68 + 0x0036] /* "Thou art a great help.  We are in dire need!\n" */);
		add_npc_talk(VENDOR_HEALER, &AVATAR[0x14C68 + 0x0036] /* "Thou art a great help.  We are in dire need!" */);
		Party.chara[0]._HP[0] -= 100;
		dspl_Stats();
		set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
		u_delay(1, 0);
	} else if(bp_04 == 'N') {
		karma_dec(&(Party._sacri), 5);
		dspl_Stats();
	}
}

/*SHOP #6 - healer*/
/*C_DC4D*/SHP_healer()
{
	int bp_02;
	char bp_04;

	D_9142 = D_5788[Party._loc - 1] - 1;
	u4_puts(/*D_5A23*/&AVATAR[0x14C96 + 0x0036] /* "Welcome unto\n" */);
	add_npc_talk(VENDOR_HEALER, &AVATAR[0x14C96 + 0x0036] /* "Welcome unto" */);
	u4_puts(D_5798[D_9142]);
	add_npc_talk(VENDOR_HEALER, D_5798[D_9142]);
	u4_puts(". ");
	add_npc_talk(VENDOR_HEALER, ". ");
	Gra_CR();
	u4_puts(D_57AC[D_9142]);
	u4_puts(/*D_5A31*/&AVATAR[0x14CA4 + 0x0036] /* " says: Peace and Joy be with you friend. Are you in need of help?\x12\x12\b" */);
	add_npc_talk(VENDOR_HEALER, &AVATAR[0x14CAB + 0x0036] /* "Peace and Joy be with you friend. Are you in need of help?" */);
	set_input_mode(INPUT_MODE_GENERAL_YES_NO);
	while((bp_04 = AskY_N()) == 'Y') {
		Gra_CR();
		u4_puts(D_57AC[D_9142]);
		u4_puts(/*D_5A76*/ &AVATAR[0x14CE9 + 0x0036] /*" says: We can perform:\n\
A-Curing\n\
B-Healing\n\
C-Resurrection\n" */);

		add_npc_talk(VENDOR_HEALER, "We can perform curing, healing and resurrection."); // re-write this text for TTS
		add_npc_talk(VENDOR_HEALER, &AVATAR[0x14D23 + 0x0036] /* "Your need?" */);
		set_input_mode(INPUT_MODE_GENERAL_ASK_LETTER_HEALER);
		bp_02 = AskLetter(/*D_5AB0*/&AVATAR[0x14D23 + 0x0036] /* "Your need:\x12\x12\b" */, 'A', 'C');
		if(bp_02 < 'A')
			break;
		switch((char)bp_02) {
			case 'A': C_DAA2(); break;
			case 'B': C_DB29(); break;
			case 'C': C_DB93(); break;
		}
		Gra_CR();
		u4_puts(D_57AC[D_9142]);
		u4_puts(/*D_5ABE*/&AVATAR[0x14D31 + 0x0036] /* " asks: Do you need more help?\x12\x12\b" */);
		add_npc_talk(VENDOR_HEALER, &AVATAR[0x14D38 + 0x0036] /* "Do you need more help?" */);
		set_input_mode(INPUT_MODE_GENERAL_YES_NO);
	}
	if(Party.chara[0]._HP[0] >= 400)
		C_DBF5();
	dspl_Stats();
	Gra_CR();
	u4_puts(D_57AC[D_9142]);
	u4_puts(/*D_5ADF*/&AVATAR[0x14D52 + 0x0036] /* " says: May thy life be guarded by the powers of good.\n" */);
	add_npc_talk(VENDOR_HEALER, &AVATAR[0x14D59 + 0x0036] /* "May thy life be guarded by the powers of good." */);
	add_npc_talk(VENDOR_INVALID, "");
}

/*----------------------------------------*/

/*shops indexes*/
unsigned char * D_5EE8 = &AVATAR[0x1515C + 0x0036] /*{0, 0, 0, 0, 0, 1, 2, 0, 0, 3, 0, 0, 4, 5, 6, 0}*/;

short * D_5EF8 = &AVATAR[0x1516B + 0x0036] /*{20, 30, 10, 40, 99, 25}*/;

char *D_5F04[] = {
	/*D_5B16*/&AVATAR[0x14D89 + 0x0036] /* "Jolly Spirits" */,
	/*D_5B24*/&AVATAR[0x14D97 + 0x0036] /* "The Bloody Pub" */,
	/*D_5B33*/&AVATAR[0x14DA6 + 0x0036] /* "The Keg Tap" */,
	/*D_5B3F*/&AVATAR[0x14DB2 + 0x0036] /* "Folley Tavern" */,
	/*D_5B4D*/&AVATAR[0x14DC0 + 0x0036] /* "Captain Black Tavern" */,
	/*D_5B62*/&AVATAR[0x14DD5 + 0x0036] /* "Axe 'n Ale" */
};

char *D_5F10[] = {
	/*D_5B6D*/&AVATAR[0x14DE0 + 0x0036] /* "Sam" */,
	/*D_5B71*/&AVATAR[0x14DE4 + 0x0036] /* "Celestial" */,
	/*D_5B7B*/&AVATAR[0x14DEE + 0x0036] /* "Terran" */,
	/*D_5B82*/&AVATAR[0x14DF5 + 0x0036] /* "Greg 'n Rob" */,
	/*D_5B8E*/&AVATAR[0x14E01 + 0x0036] /* "The Cap'n" */,
	/*D_5B98*/&AVATAR[0x14E0B + 0x0036] /* "Arron" */
};

char *D_5F1C[] = {
	/*D_5B9E*/&AVATAR[0x14E11 + 0x0036] /* "Lamb Chops" */,
	/*D_5BA9*/&AVATAR[0x14E1C + 0x0036] /* "Dragon Tartar" */,
	/*D_5BB7*/&AVATAR[0x14E2A + 0x0036] /* "Brown Beans" */,
	/*D_5BC3*/&AVATAR[0x14E36 + 0x0036] /* "Folley Filet" */,
	/*D_5BD0*/&AVATAR[0x14E43 + 0x0036] /* "Dog Meat Pie" */,
	/*D_5BDD*/&AVATAR[0x14E50 + 0x0036] /* "Green Granukit" */
};

short * D_5F28 = &AVATAR[0x1519B + 0x0036] /*{4,2,3,2,4,2}*/;

char *D_5F34[] = {
	/*D_5BEC*/&AVATAR[0x14E5F + 0x0036] /* "black stone" */,
	/*D_5BF8*/&AVATAR[0x14E6B + 0x0036] /* "sextant" */,
	/*D_5C00*/&AVATAR[0x14E73 + 0x0036] /* "white stone" */,
	/*D_5C0C*/&AVATAR[0x14E7F + 0x0036] /* "mandrake" */,
	/*D_5C15*/&AVATAR[0x14E88 + 0x0036] /* "skull" */,
	/*D_5C1B*/&AVATAR[0x14E8E + 0x0036] /* "nightshade" */,
	/*D_5C26*/&AVATAR[0x14E99 + 0x0036] /* "mandrake root" */
};

char *D_5F42[] = {
	/*D_5C34*/&AVATAR[0x14EA7 + 0x0036] /* "Ah, the Black Stone.  Yes I've heard of it. But, the only one who knows where it lies is the wizard Merlin.\n" */,
	/*D_5CA1*/&AVATAR[0x14F14 + 0x0036] /* "For navigation a Sextant is vital...  Ask for item \"D\" in the Guild shops!\n" */,
	/*D_5CED*/&AVATAR[0x14F60 + 0x0036] /* "Hmmm, Now let me see... Yes, it was the old Hermit... Sloven!  He is tough to find, lives near Lock Lake I hear.\n" */,
	/*D_5D5F*/&AVATAR[0x14FD2 + 0x0036] /* "The last person I knew that had any Mandrake was an old alchemist named Calumny.\n" */,
	/*D_5DB1*/&AVATAR[0x15024 + 0x0036] /* "If thou must know of that evilest of all things... find the beggar Jude.  He is very very poor!\n" */,
	/*D_5E12*/&AVATAR[0x15085 + 0x0036] /* "Of Nightshade I know but this... Seek out Virgil or thou shalt miss! Try in Trinsic!\n" */
};

/*pub food*/
C_DD24()
{
	int bp_02, bp_04;

	u4_puts(/*D_5E68*/&AVATAR[0x150DB + 0x0036] /* "Our specialty is " */);
	add_npc_talk(VENDOR_PUB, &AVATAR[0x150DB + 0x0036] /* "Our specialty is " */);
	u4_puts(D_5F1C[D_9142]); 
	add_npc_talk(VENDOR_PUB, D_5F1C[D_9142]);
	u4_puts(/*D_5E7A*/&AVATAR[0x150ED + 0x0036] /* ", which costs " */); 
	add_npc_talk(VENDOR_PUB, &AVATAR[0x150ED + 0x0036] /* ", which costs " */);
	u4_putl(D_5F28[D_9142], 1, '0');
	add_npc_talk_long(VENDOR_PUB, D_5F28[D_9142]);
	u4_puts(/*D_5E89*/&AVATAR[0x150FC + 0x0036] /* "gp.  How many plates would you like?\x12\x12\x12\b\b" */);
	add_npc_talk(VENDOR_PUB, &AVATAR[0x150FC + 0x0036] /* "gold pieces.  How many plates would you like?" */);
	set_input_mode(INPUT_MODE_NUMBER_INPUT_2_DIGITS);
	bp_02 = AskInt(2);
	if(bp_02 <= 0) {
		if(bp_02 < 0)
			sound(1,0);
		return 1;
	}
	bp_04 = 0;
	while(bp_02) {
		if(Party._gold >= D_5F28[D_9142]) {
			Party._gold -= D_5F28[D_9142];
			Party._food = (Party._food + 100 > 999900L)?999900L:(Party._food + 100);
			dspl_Stats();
			bp_02 --;
			bp_04 ++;
		} else {
			if(bp_04 == 0) {
				u4_puts(/*D_5EB3*/&AVATAR[0x15126 + 0x0036] /* "\nYa cannot afford any!\n" */);
				add_npc_talk(VENDOR_PUB, &AVATAR[0x15127 + 0x0036] /* "Ya cannot afford any!" */);
				return 1;
			}
			u4_puts(/*D_5ECB*/&AVATAR[0x1513E + 0x0036] /* "\nYa can only afford " */);
			add_npc_talk(VENDOR_PUB, &AVATAR[0x1513F + 0x0036] /* "Ya can only afford " */);
			u4_putl(bp_04, 1, '0');
			add_npc_talk_long(VENDOR_PUB, bp_04);
			u4_puts(/*D_5EE0*/&AVATAR[0x15109 + 0x0036] /* " plate" */);
			add_npc_talk(VENDOR_PUB, &AVATAR[0x15109 + 0x0036] /* " plate" */);
			if (bp_04 != 1)
			{
				u4_putc('s');
				add_char_to_text_buffer('s');
				add_npc_talk(VENDOR_PUB, "s");
			}
			u4_putc('.');
			add_npc_talk(VENDOR_PUB, ".\n");
			add_char_to_text_buffer('.');
			Gra_CR();
			add_char_to_text_buffer('\n');
			break;
		}
	}

	return 0;
}

/*pub ale*/
C_DE35()
{
	int loc_A;
	char loc_B;
	int loc_C;
	char loc_D[16];
	int loc_E;

	if(D_913E == 3) {
		Gra_CR();
		u4_puts(D_5F10[D_9142]);
		u4_puts(/*D_5F4E*/&AVATAR[0x151C1 + 0x0036] /* " says: Sorry, you seem to have too many.  Bye!\n" */);
		add_npc_talk(VENDOR_PUB, &AVATAR[0x151C8 + 0x0036] /* "Sorry, you seem to have too many.  Bye!\n" */);
		return 1;
	}
	D_913E ++;
	u4_puts(/*D_5F7E*/&AVATAR[0x151F1 + 0x0036] /* "\nHere's a mug of our best. That'll be 2gp. You pay?\x12\x12\x12\b\b" */);
	add_npc_talk(VENDOR_PUB, &AVATAR[0x151F1 + 0x0036] /* "Here's a mug of our best. That'll be 2 gold pieces.\n" */);
	set_input_mode(INPUT_MODE_NUMBER_INPUT_2_DIGITS);
	loc_A = AskInt(2);
	if(loc_A <= 0) {
		if(loc_A < 0)
			sound(1,0);
		return 1;
	}
	if(loc_A < 2) {
		u4_puts(/*D_5FB7*/&AVATAR[0x1522A + 0x0036] /* "\nWon't pay, eh. Ya scum, be gone fore ey call the guards!\n" */);
		add_npc_talk(VENDOR_PUB, &AVATAR[0x1522B + 0x0036] /* "Won't pay, eh. Ya scum, be gone fore ey call the guards!\n" */);
		return 1;
	}
	if(Party._gold < loc_A) {
		u4_puts(/*D_5FF2*/&AVATAR[0x15265 + 0x0036] /* "\nIt seems that you have not the gold. Good Day!\n" */);
		add_npc_talk(VENDOR_PUB, &AVATAR[0x15266 + 0x0036] /* "It seems that you have not the gold. Good Day!\n" */);
		return 1;
	}
	Party._gold -= loc_A; dspl_Gold();
	if(loc_A >= 3) {
		u4_puts(/*D_6023*/&AVATAR[0x15296 + 0x0036] /* "\nWhat'd ya like to know friend?\n" */);
		add_npc_talk(VENDOR_PUB, &AVATAR[0x15297 + 0x0036] /* "What'd ya like to know friend?\n" */);
		set_input_mode(INPUT_MODE_PUB_WORD);
		u4_gets(loc_D, 15);
		for(loc_C = 6; loc_C >= 0; loc_C --) {
			if(_strnicmp(loc_D, D_5F34[loc_C], 16) == 0)
			break;
		}
		if(loc_C == 6)
			loc_C = 3;
		if(loc_C < D_9142) {
			u4_puts(/*D_6044*/&AVATAR[0x152B7 + 0x0036] /* "\n'fraid I can't help ya there friend!\n" */);
			add_npc_talk(VENDOR_PUB, &AVATAR[0x152B8 + 0x0036] /* "'fraid I can't help ya there friend!\n" */);
		} else {
			while(D_5EF8[loc_C] > loc_A) {
				u4_puts(/*D_606B*/&AVATAR[0x152DE + 0x0036] /* "\nThat subject is a bit foggy, perhaps more gold will refresh my memory. You give:\x12\x12\x12\b\b" */);
				add_npc_talk(VENDOR_PUB, &AVATAR[0x152DE + 0x0036] /* "That subject is a bit foggy, perhaps more gold will refresh my memory.\n" */);
				set_input_mode(INPUT_MODE_NUMBER_INPUT_2_DIGITS);
				loc_E = AskInt(2);
				if(loc_E <= 0) {
					if(loc_A < 0)
						sound(1,0);
					else
						u4_puts(/*D_60C2*/&AVATAR[0x15335 + 0x0036] /* "\nSorry, I could not hep ya mate!\n" */);
					add_npc_talk(VENDOR_PUB, &AVATAR[0x15336 + 0x0036] /* "Sorry, I could not hep ya mate!\n" */);
					return 1;
				}
				if(Party._gold < loc_E) {
					u4_puts(/*D_60E4*/&AVATAR[0x15357 + 0x0036] /* "\nYer don't have that mate!\n" */);
					add_npc_talk(VENDOR_PUB, &AVATAR[0x15357 + 0x0036] /* "Yer don't have that mate!" */);
					u4_puts(/*D_6100*/&AVATAR[0x15335 + 0x0036] /* "\nSorry, I could not hep ya mate!\n" */);
					add_npc_talk(VENDOR_PUB, &AVATAR[0x15335 + 0x0036]  /* "Sorry, I could not hep ya mate!\n" */);
					return 1;
				}
				Party._gold -= loc_E; dspl_Gold();
				loc_A += loc_E;
			}
			if(D_5EF8[loc_C] <= loc_A) {
				Gra_CR();
				u4_puts(D_5F10[D_9142]);
				u4_puts(/*D_6122*/&AVATAR[0x15395 + 0x0036] /* " says: " */);
				u4_puts(D_5F42[loc_C]);
				add_npc_talk(VENDOR_PUB, D_5F42[loc_C]);
				Gra_CR();
			}
		}
	}
	u4_puts(/*D_612A*/&AVATAR[0x1539D + 0x0036] /* "\nAnythin' else?\x12\x12\b" */);
	add_npc_talk(VENDOR_PUB, &AVATAR[0x1539E + 0x0036] /* "Anythin' else?" */);
	set_input_mode(INPUT_MODE_GENERAL_YES_NO);
	loc_B = AskY_N();
	if (loc_B != 'Y')
	{
		return 1;
	}

	return 0;
}

/*SHOP #4 - pub*/
/*C_DFAF*/SHP_pub()
{
	char bp_02;

	D_913E = 0;
	D_9142 = D_5EE8[Party._loc - 1] - 1;
	Gra_CR();
	u4_puts(D_5F10[D_9142]);
	u4_puts(/*D_613D*/&AVATAR[0x153B0 + 0x0036] /* " says:\nWelcome to " */);
	add_npc_talk(VENDOR_PUB, &AVATAR[0x1324E + 0x002e] /* "Welcome to " */);
	u4_puts(D_5F04[D_9142]);
	add_npc_talk(VENDOR_PUB, D_5F04[D_9142]);
	u4_puts(". ");
	add_npc_talk(VENDOR_PUB, ". ");
	Gra_CR();
	do {
		u4_puts(D_5F10[D_9142]);
		u4_puts(/*D_6150*/&AVATAR[0x153C3 + 0x0036] /* " says: What'll it be, Food er Ale?\x12\x12\b" */);
		add_npc_talk(VENDOR_PUB,&AVATAR[0x153CA + 0x0036] /* "What'll it be, Food er Ale?" */);
		{
C_E004:
			set_input_mode(INPUT_MODE_PUB_FOOD_OR_ALE);
			bp_02 = u_kbread();
			u4_toupper(bp_02);
			if(bp_02 == ' ' || bp_02 == 0x1b || bp_02 == '\r') {
				Gra_CR();
				add_npc_talk(VENDOR_INVALID, "");
				return 0;
			}
			if(bp_02 != 'F' && bp_02 != 'A') {
				sound(1,0);
				goto C_E004;
			}
		}
		u4_putc(bp_02);
		Gra_CR();
		if(bp_02 == 'F') {
			if(C_DD24())
				break;
		} else {
			if(C_DE35())
				break;
		}
		dspl_Stats();
		u4_puts(/*D_6176*/&AVATAR[0x153E9 + 0x0036] /* "Here ye arr. Somethin' else?\x12\x12\b" */);
		add_npc_talk(VENDOR_PUB, &AVATAR[0x153E9 + 0x0036] /* "Here ye arr. Somethin' else?" */);
		set_input_mode(INPUT_MODE_GENERAL_YES_NO);
		bp_02 = AskY_N();
	} while(bp_02 == 'Y');
	u4_puts(/*D_6196*/&AVATAR[0x15409 + 0x0036] /* "See ya mate!\n" */);
	add_npc_talk(VENDOR_PUB, &AVATAR[0x15409 + 0x0036] /* "See ya mate!" */);
	add_npc_talk(VENDOR_INVALID, "");
}

/*----------------------------------------*/

/*shops indexes*/
unsigned char * D_636C = &AVATAR[0x155DF + 0x0036] /* {0, 0, 0, 0, 1, 2, 0, 3, 0, 0, 4, 0, 5, 0, 0, 0} */;

short * D_637C = &AVATAR[0x155EF + 0x0036] /* {25, 40, 35, 20, 30} */;

char *D_6386[] = {
	/*D_61A4*/&AVATAR[0x15417 + 0x0036] /* "The Sage Deli" */,
	/*D_61B2*/&AVATAR[0x15425 + 0x0036] /* "Adventure Food" */,
	/*D_61C1*/&AVATAR[0x15434 + 0x0036] /* "The Dry Goods" */,
	/*D_61CF*/&AVATAR[0x15442 + 0x0036] /* "Food for Thought" */,
	/*D_61E0*/&AVATAR[0x15453 + 0x0036] /* "The Market" */
};

char *D_6390[] = {
	/*D_61EB*/&AVATAR[0x1545E + 0x0036] /* "Shaman" */,
	/*D_61F2*/&AVATAR[0x15465 + 0x0036] /* "Windrick" */,
	/*D_61FB*/&AVATAR[0x1546E + 0x0036] /* "Donnar" */,
	/*D_6102*/&AVATAR[0x15475 + 0x0036] /* "Mintol" */,
	/*D_6109*/&AVATAR[0x1547C + 0x0036] /* "Max" */
};

/*SHOP #3 - food*/
/*C_E088*/SHP_food()
{
	char bp_02;
	char bp_04;

	D_9142 = D_636C[Party._loc - 1] - 1;
	u4_puts(/*D_620D*/&AVATAR[0x13B42 + 0x002e] /* "\n\n\n\nWelcome to " */);
	add_npc_talk(VENDOR_FOOD, &AVATAR[0x1324E + 0x002e] /* "Welcome to " */);
	u4_puts(D_6386[D_9142]);
	add_npc_talk(VENDOR_FOOD, D_6386[D_9142]);
	u4_puts(/*D_621D*/&AVATAR[0x15490 + 0x002e] /* "\n\n" */);
	u4_puts(D_6390[D_9142]);
	u4_puts(/*D_6220*/&AVATAR[0x15493 + 0x0036] /* " says: Good day, and Welcome friend.\n\nMay I interest you in some rations?\x12\x12\b" */);
	add_npc_talk(VENDOR_FOOD, &AVATAR[0x15493 + 0x0036] /* "Good day, and Welcome friend. May I interest you in some rations?" */);
	set_input_mode(INPUT_MODE_GENERAL_YES_NO);
	bp_04 = AskY_N();
	if(bp_04 != 'Y') {
		if (bp_04 == 'N')
		{
			u4_puts(/*D_626D*/&AVATAR[0x154E0 + 0x0036] /* "Goodbye, Come again!\n" */);
			add_npc_talk(VENDOR_FOOD, &AVATAR[0x154E0 + 0x0036] /* "Goodbye, Come again!" */);
		}
		add_npc_talk(VENDOR_INVALID, "");
		return 0;
	}
	u4_puts(/*D_6283*/&AVATAR[0x154F6 + 0x0036] /* "We have the best adventure rations, 25 for only " */);
	add_npc_talk(VENDOR_FOOD, &AVATAR[0x154F6 + 0x0036] /* "We have the best adventure rations, 25 for only " */);
	u4_putl(D_637C[D_9142], 1, '0');
	add_npc_talk_long(VENDOR_FOOD, D_637C[D_9142]);
	u4_puts(/*D_62B4*/&AVATAR[0x13310 + 0x002e] /* "gp." */);
	add_npc_talk(VENDOR_FOOD, &AVATAR[0x13310 + 0x002e] /* "gold pieces." */);
	do {
		u4_puts(/*D_62B8*/&AVATAR[0x1552B + 0x0036] /* "\nHow many packs of 25 would you like?\x12\x12\x12\x12\b\b\b" */);
		add_npc_talk(VENDOR_FOOD, &AVATAR[0x1552C + 0x0036] /* "How many packs of 25 would you like?" */);
		set_input_mode(INPUT_MODE_NUMBER_INPUT_3_DIGITS);
		bp_02 = AskInt(3);
		if(bp_02 <= 0) {
			u4_puts(/*D_62E5*/&AVATAR[0x15558 + 0x0036] /* "\nToo bad. Maybe next time.\n" */);
			add_npc_talk(VENDOR_FOOD, &AVATAR[0x15559 + 0x0036] /* "Too bad. Maybe next time." */);
			bp_04 = 'N';
			continue;
		}
		if(D_637C[D_9142] * bp_02 > Party._gold) {
			if(D_637C[D_9142] > Party._gold) {
				u4_puts(/*D_6301*/&AVATAR[0x15574 + 0x0036] /* "You cannot afford any!\n" */);
				add_npc_talk(VENDOR_FOOD, &AVATAR[0x15574 + 0x0036] /* "You cannot afford any!" */);
				break;
			}
			u4_puts(/*D_6319*/&AVATAR[0x1558C + 0x0036] /* "You can only afford " */);
			add_npc_talk(VENDOR_FOOD, &AVATAR[0x1558C + 0x0036] /* "You can only afford " */);
			u4_putl((long)Party._gold / (long)D_637C[D_9142], 1, '0');
			add_npc_talk_long(VENDOR_FOOD, (long)Party._gold / (long)D_637C[D_9142]);
			u4_puts(/*D_632E*/&AVATAR[0x155A1 + 0x0036] /* " packs.\n" */);
			add_npc_talk(VENDOR_FOOD, &AVATAR[0x155A1 + 0x0036] /* " packs." */);
			bp_04 = 'Y';
			continue;
		}
		Party._food += 2500L * bp_02;
		if(Party._food > 999900L)
			Party._food = 999900L;
		Party._gold -= D_637C[D_9142] * bp_02;
		dspl_Stats();
		u4_puts(/*D_6337*/&AVATAR[0x155AA + 0x0036] /* "\nThank you. Anything else?\x12\x12\b" */);
		add_npc_talk(VENDOR_FOOD, &AVATAR[0x155AB + 0x0036] /* "Thank you. Anything else?" */);
		set_input_mode(INPUT_MODE_GENERAL_YES_NO);
		bp_04 = AskY_N();
	} while(bp_04 == 'Y');
	u4_puts(/*D_6355*/&AVATAR[0x155C8 + 0x0036] /* "\nGoodbye. Come again!\n" */);
	add_npc_talk(VENDOR_FOOD, &AVATAR[0x155C9 + 0x0036] /* "Goodbye. Come again" */);
	add_npc_talk(VENDOR_INVALID, "");
}
