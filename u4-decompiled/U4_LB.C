/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "U4.H"

#include <string.h>

char *D_6FF0[28] = {
	/*D_639A*/&AVATAR[0x1560D + 0x2E36] /* "bye" */,
	/*D_639E*/&AVATAR[0x15611 + 0x2E36] /* "help" */,
	/*D_63A3*/&AVATAR[0x15616 + 0x2E36] /* "health" */,
	/*D_63AA*/&AVATAR[0x1561D + 0x2E36] /* "name" */,
	/*D_63AF*/&AVATAR[0x15622 + 0x2E36] /* "look" */,
	/*D_63B4*/&AVATAR[0x15627 + 0x2E36] /* "job" */,
	/*D_63B8*/&AVATAR[0x1562B + 0x2E36] /* "truth" */,
	/*D_63BE*/&AVATAR[0x15631 + 0x2E36] /* "love" */,
	/*D_63C3*/&AVATAR[0x15636 + 0x2E36] /* "courage" */,
	/*D_63CB*/&AVATAR[0x1563E + 0x2E36] /* "honesty" */,
	/*D_63D3*/&AVATAR[0x15646 + 0x2E36] /* "compassion" */,
	/*D_63DE*/&AVATAR[0x15651 + 0x2E36] /* "valor" */,
	/*D_63E4*/&AVATAR[0x15657 + 0x2E36] /* "justice" */,
	/*D_63EC*/&AVATAR[0x1298D + 0x2E36] /* "sacrifice" */,
	/*D_63F6*/&AVATAR[0x15669 + 0x2E36] /* "honor" */,
	/*D_63FC*/&AVATAR[0x1566F + 0x2E36] /* "spirituality" */,
	/*D_6409*/&AVATAR[0x1567C + 0x2E36] /* "humility" */,
	/*D_6412*/&AVATAR[0x15685 + 0x2E36] /* "pride" */,
	/*D_6418*/&AVATAR[0x1568B + 0x2E36] /* "avatar" */,
	/*D_641F*/&AVATAR[0x15692 + 0x2E36] /* "quest" */,
	/*D_6425*/&AVATAR[0x15698 + 0x2E36] /* "britannia" */,
	/*D_642F*/&AVATAR[0x156A2 + 0x2E36] /* "ankh" */,
	/*D_6434*/&AVATAR[0x156A7 + 0x2E36] /* "abyss" */,
	/*D_643A*/&AVATAR[0x156AD + 0x2E36] /* "mondain" */,
	/*D_6442*/&AVATAR[0x156B5 + 0x2E36] /* "minax" */,
	/*D_6448*/&AVATAR[0x156BB + 0x2E36] /* "exodus" */,
	/*D_644F*/&AVATAR[0x156C2 + 0x2E36] /* "virtue" */,
	/*D_6456*/ ""
};

char *D_7028[24] = {
	/*D_6457*/&AVATAR[0x156CA + 0x2E36] /* "\n\n\nHe says:\nMy name is\nLord British,\nSovereign of\nall Britannia!\n" */,
	/*D_6499*/&AVATAR[0x1570C + 0x2E36] /* "\n\n\n\n\nThou see the\nKing with the\nRoyal Sceptre.\n" */,
	/*D_64C9*/&AVATAR[0x1573C + 0x2E36] /* "\n\n\nHe says:\nI rule all\nBritannia, and\nshall do my best\nto help thee!\n" */,
	/*D_650F*/&AVATAR[0x15782 + 0x2E36] /* "He says:\nMany truths can\nbe learned at\nthe Lycaeum.  It\nlies on the\nnorthwestern\nshore of Verity\nIsle!\n" */,
	/*D_6577*/&AVATAR[0x157EA + 0x2E36] /* "He says:\nLook for the\nmeaning of Love\nat Empath Abbey.\nThe Abbey sits\non the western\nedge of the Deep\nForest!\n" */,
	/*D_65E6*/&AVATAR[0x15859 + 0x2E36] /* "\n\nHe says:\nSerpent's Castle\non the Isle of\nDeeds is where\nCourage should\nbe sought!\n" */,
	/*D_663B*/&AVATAR[0x158AE + 0x2E36] /* "\nHe says:\nThe fair towne\nof Moonglow on\nVerity Isle is\nwhere the virtue\nof Honesty\nthrives!\n" */,
	/*D_6698*/&AVATAR[0x1590B + 0x2E36] /* "\n\nHe says:\nThe bards in the\ntowne of Britain\nare well versed\nin the virtue of\nCompassion!\n" */,
	/*D_66F3*/&AVATAR[0x15966 + 0x2E36] /* "\n\nHe says:\nMany valiant\nfighters come\nfrom Jhelom\nin the Valarian\nIsles!\n" */,
	/*D_673D*/&AVATAR[0x159B0 + 0x2E36] /* "\n\n\nHe says:\nIn the city of\nYew, in the Deep\nForest, Justice\nis served!\n" */,
	/*D_6785*/&AVATAR[0x159F8 + 0x2E36] /* "\nHe says:\nMinoc, towne of\nself-sacrifice,\nlies on the\neastern shores\nof Lost Hope\nBay!\n" */,
	/*D_67DD*/&AVATAR[0x15A50 + 0x2E36] /* "\nHe says:\nThe Paladins who\nstrive for Honor\nare oft seen in\nTrinsic, north\nof the Cape of\nHeroes!\n" */,
	/*D_6840*/&AVATAR[0x15AB3 + 0x2E36] /* "\nHe says:\nIn Skara Brae\nthe Spiritual\npath is taught.\nFind it on an\nisle near\nSpiritwood!\n" */,
	/*D_689B*/&AVATAR[0x15B0E + 0x2E36] /* "\n\n\nHe says:\nHumility is the\nfoundation of\nVirtue!  The\nruins of proud\nMagincia are a\ntestimony unto\nthe Virtue of\nHumility!\n\nFind the Ruins\nof Magincia far\noff the shores\nof Britannia,\non a small isle\nin the vast\nOcean!\n" */,
	/*D_6978*/&AVATAR[0x15BEB + 0x2E36] /* "\n\n\nHe says:\nOf the eight\ncombinations of\nTruth, Love and\nCourage, that\nwhich contains\nneither Truth,\nLove nor Courage\nis Pride.\n\nPride being not\na Virtue must be\nshunned in favor\nof Humility, the\nVirtue which is\nthe antithesis\nof Pride!\n" */,
	/*D_6A66*/&AVATAR[0x15CD9 + 0x2E36] /* "\n\n\n\n\n\nLord British\nsays:\nTo be an Avatar\nis to be the\nembodiment of\nthe Eight\nVirtues.\n\n\nIt is to live a\nlife constantly\nand forever in\nthe Quest to\nbetter thyself\nand the world in\nwhich we live.\n" */,
	/*D_6B2B*/&AVATAR[0x15D9E + 0x2E36] /* "\n\n\nLord British\nsays:\nThe Quest of\nthe Avatar is\nto know and\nbecome the\nembodiment of\nthe Eight\nVirtues of\nGoodness!\nIt is known that\nall who take on\nthis Quest must\nprove themselves\nby conquering\nthe Abyss and\nViewing the\nCodex of\nUltimate Wisdom!\n" */,
	/*D_6C25*/&AVATAR[0x15E98 + 0x2E36] /* "\n\n\n\n\n\n\nHe says:\nEven though the\nGreat Evil Lords\nhave been routed\nevil yet remains\nin Britannia.\n\n\n\n\n\nIf but one soul\ncould complete\nthe Quest of the\nAvatar, our\npeople would\nhave a new hope,\na new goal for\nlife.\n\nThere would be a\nshining example\nthat there is\nmore to life\nthan the endless\nstruggle for\npossessions\nand gold!\n" */,
	/*D_6D6C*/&AVATAR[0x15FDF + 0x2E36] /* "He says:\nThe Ankh is the\nsymbol of one\nwho strives for\nVirtue.  Keep it\nwith thee at all\ntimes for by\nthis mark thou\nshalt be known!\n" */,
	/*D_6DF2*/&AVATAR[0x16065 + 0x2E36] /* "\n\n\n\n\n\nHe says:\nThe Great\nStygian Abyss\nis the darkest\npocket of evil\nremaining in\nBritannia!\n\n\n\n\n\n\n\nIt is said that\nin the deepest\nrecesses of the\nAbyss is the\nChamber of the\nCodex!\n\n\nIt is also said\nthat only one of\nhighest Virtue\nmay enter this\nChamber, one\nsuch as an\nAvatar!!!\n" */, // The AVATAR.EXE has a block of zeros after this string
	/*D_6F0C*/&AVATAR[0x1617E + 0x3837] /* "\n\n\n\n\n\nHe says:\nMondain is dead!\n" */,
	/*D_6F2D*/&AVATAR[0x1619F + 0x3837] /* "\n\n\n\n\n\nHe says:\nMinax is dead!\n" */,
	/*D_6F4C*/&AVATAR[0x161BE + 0x3837] /* "\n\n\n\n\n\nHe says:\nExodus is dead!\n" */,
	/*D_6F6C*/&AVATAR[0x161DE + 0x3837] /* "\nHe says:\nThe Eight\nVirtues of the\nAvatar are:\nHonesty,\nCompassion,\nValor,\nJustice,\nSacrifice,\nHonor,\nSpirituality,\nand Humility!\n" */
};

#if 0
char* D_7028_modified[24] = {
	/*D_6457*/"My name is Lord British, Sovereign of all Britannia!\n",
	/*D_6499*/"Thou see the King with the Royal Sceptre.\n",
	/*D_64C9*/"I rule all Britannia, and shall do my best to help thee!\n",
	/*D_650F*/"Many truths can be learned at the Lycaeum.  It lies on the northwestern shore of Verity Isle!\n",
	/*D_6577*/"Look for the meaning of Love at Empath Abbey. The Abbey sits on the western edge of the Deep Forest!\n",
	/*D_65E6*/"Serpent's Castle on the Isle of Deeds is where Courage should be sought!\n",
	/*D_663B*/"The fair towne of Moonglow on Verity Isle is where the virtue of Honesty thrives!\n",
	/*D_6698*/"The bards in the towne of Britain are well versed in the virtue of Compassion!\n",
	/*D_66F3*/"Many valiant fighters come from Jhelom nin the Valarian Isles!\n",
	/*D_673D*/"In the city of Yew, in the Deep Forest, Justice is served!\n",
	/*D_6785*/"Minoc, towne of self-sacrifice, lies on the eastern shores of Lost Hope Bay!\n",
	/*D_67DD*/"The Paladins who strive for Honor are oft seen in Trinsic, north of the Cape of Heroes!\n",
	/*D_6840*/"In Skara Brae the Spiritual path is taught. Find it on an isle near Spiritwood!\n",
	/*D_689B*/"Humility is the foundation of Virtue! The ruins of proud Magincia are a testimony unto the Virtue of Humility! Find the Ruins of Magincia far off the shores of Britannia, on a small isle in the vast Ocean!\n",
	/*D_6978*/"\n\n\nOf the eight\ncombinations of\nTruth, Love and\nCourage, that\nwhich contains\nneither Truth,\nLove nor Courage\nis Pride.\n\nPride being not\na Virtue must be\nshunned in favor\nof Humility, the\nVirtue which is\nthe antithesis\nof Pride!\n",
	/*D_6A66*/"\n\n\n\n\n\nTo be an Avatar\nis to be the\nembodiment of\nthe Eight\nVirtues.\n\n\nIt is to live a\nlife constantly\nand forever in\nthe Quest to\nbetter thyself\nand the world in\nwhich we live.\n",
	/*D_6B2B*/"\n\n\nThe Quest of\nthe Avatar is\nto know and\nbecome the\nembodiment of\nthe Eight\nVirtues of\nGoodness!\nIt is known that\nall who take on\nthis Quest must\nprove themselves\nby conquering\nthe Abyss and\nViewing the\nCodex of\nUltimate Wisdom!\n",
	/*D_6C25*/"\n\n\n\n\n\n\nEven though the\nGreat Evil Lords\nhave been routed\nevil yet remains\nin Britannia.\n\n\n\n\n\nIf but one soul\ncould complete\nthe Quest of the\nAvatar, our\npeople would\nhave a new hope,\na new goal for\nlife.\n\nThere would be a\nshining example\nthat there is\nmore to life\nthan the endless\nstruggle for\npossessions\nand gold!\n",
	/*D_6D6C*/&AVATAR[0x15FE8 + 0x2E36] /* "The Ankh is the\nsymbol of one\nwho strives for\nVirtue.  Keep it\nwith thee at all\ntimes for by\nthis mark thou\nshalt be known!\n" */,
	/*D_6DF2*/"\n\n\n\n\n\nThe Great\nStygian Abyss\nis the darkest\npocket of evil\nremaining in\nBritannia!\n\n\n\n\n\n\n\nIt is said that\nin the deepest\nrecesses of the\nAbyss is the\nChamber of the\nCodex!\n\n\nIt is also said\nthat only one of\nhighest Virtue\nmay enter this\nChamber, one\nsuch as an\nAvatar!!!\n",
	/*D_6F0C*/"\n\n\n\n\n\nMondain is dead!\n",
	/*D_6F2D*/"\n\n\n\n\n\nMinax is dead!\n",
	/*D_6F4C*/"\n\n\n\n\n\nExodus is dead!\n",
	/*D_6F6C*/&AVATAR[0x161E7 + 0x3837] /* "\nThe Eight\nVirtues of the\nAvatar are:\nHonesty,\nCompassion,\nValor,\nJustice,\nSacrifice,\nHonor,\nSpirituality,\nand Humility!\n" */
};
#endif

/*10 seconds delay*/
C_E1FC() {
	u_kbflush(); 
	set_input_mode(INPUT_MODE_DELAY_CONTINUE);
	u_delay(10, 1);
	if(u_kbhit())
		u_kbread();
}

/*when asked 'help'*/
C_E21E() {
	u4_puts(/*D_7058*/&AVATAR[0x162CA + 0x3837] /* "He says: " */);
	if(Party._moves < 1000) {
		add_npc_talk(LORD_BRITISH, &AVATAR[0x162D4 + 0x3837] /* "To survive in this hostile land thou must first know thyself! Seek ye to master thy weapons and thy magical ability!\n" */);
		u4_puts(/*D_7062*/&AVATAR[0x162D4 + 0x3837] /* "To survive in this hostile land thou must first know thyself! Seek ye to master thy weapons and thy magical ability!\n" */); C_E1FC();
		add_npc_talk(LORD_BRITISH, &AVATAR[0x1634A + 0x3837] /* "\nTake great care in these thy first travels in Britannia.\n" */);
		u4_puts(/*D_70D8*/&AVATAR[0x1634A + 0x3837] /* "\nTake great care in these thy first travels in Britannia.\n" */); C_E1FC();
		add_npc_talk(LORD_BRITISH, &AVATAR[0x16385 + 0x3837] /* "\nUntil thou dost well know thyself, travel not far from the safety of the townes!\n" */);
		u4_puts(/*D_7113*/&AVATAR[0x16385 + 0x3837] /* "\nUntil thou dost well know thyself, travel not far from the safety of the townes!\n" */);
	} else
	if(Party.f_1d8 == 1) {
		add_npc_talk(LORD_BRITISH, &AVATAR[0x163D8 + 0x3837] /* "Travel not the open lands alone. There are many worthy people in the diverse townes whom it would be wise to ask to Join thee!\n" */);
		u4_puts(/*D_7166*/&AVATAR[0x163D8 + 0x3837] /* "Travel not the open lands alone. There are many worthy people in the diverse townes whom it would be wise to ask to Join thee!\n" */); C_E1FC();
		add_npc_talk(LORD_BRITISH, &AVATAR[0x16458 + 0x3837] /* "\nBuild thy party unto eight travellers, for only a true leader can win the Quest!\n" */);
		u4_puts(/*D_71E6*/&AVATAR[0x16458 + 0x3837] /* "\nBuild thy party unto eight travellers, for only a true leader can win the Quest!\n" */);
	} else
	if(Party.mRunes == 0) {
		add_npc_talk(LORD_BRITISH, &AVATAR[0x164AB + 0x3837] /* "Learn ye the paths of virtue. Seek to gain entry unto the eight shrines!\n" */);
		u4_puts(/*D_7239*/&AVATAR[0x164AB + 0x3837] /* "Learn ye the paths of virtue. Seek to gain entry unto the eight shrines!\n" */); C_E1FC();
		add_npc_talk(LORD_BRITISH, &AVATAR[0x164F5 + 0x3837] /* "\nFind ye the Runes, needed for entry into each shrine, and learn each chant or \"Mantra\" used to focus thy meditations.\n" */);
		u4_puts(/*D_7283*/&AVATAR[0x164F5 + 0x3837] /* "\nFind ye the Runes, needed for entry into each shrine, and learn each chant or \"Mantra\" used to focus thy meditations.\n" */); C_E1FC();
		add_npc_talk(LORD_BRITISH, &AVATAR[0x1656D + 0x3837] /* "\nWithin the Shrines thou shalt learn of the deeds which show thy inner virtue or vice!\n" */);
		u4_puts(/*D_72FB*/&AVATAR[0x1656D + 0x3837] /* "\nWithin the Shrines thou shalt learn of the deeds which show thy inner virtue or vice!\n" */); C_E1FC();
		add_npc_talk(LORD_BRITISH, &AVATAR[0x165C5 + 0x3837] /* "\nChoose thy path wisely for all thy deeds of good and evil are remembered and can return to hinder thee!\n" */);
		u4_puts(/*D_7353*/&AVATAR[0x165C5 + 0x3837] /* "\nChoose thy path wisely for all thy deeds of good and evil are remembered and can return to hinder thee!\n" */);
	} else
	if(
		Party._hones & Party._compa &
		Party._valor & Party._justi &
		Party._sacri & Party._honor &
		Party._spiri & Party._humil
	) {
		add_npc_talk(LORD_BRITISH, &AVATAR[0x1662F + 0x3837] /* "Visit the Seer Hawkwind often and use his wisdom to help thee prove thy virtue.\n" */);
		u4_puts(/*D_73BD*/&AVATAR[0x1662F + 0x3837] /* "Visit the Seer Hawkwind often and use his wisdom to help thee prove thy virtue.\n" */); C_E1FC();
		add_npc_talk(LORD_BRITISH, &AVATAR[0x16680 + 0x3837] /* "\nWhen thou art ready, Hawkwind will advise thee to seek the Elevation unto partial Avatarhood in a virtue.\n" */);
		u4_puts(/*D_740E*/&AVATAR[0x16680 + 0x3837] /* "\nWhen thou art ready, Hawkwind will advise thee to seek the Elevation unto partial Avatarhood in a virtue.\n" */); C_E1FC();
		add_npc_talk(LORD_BRITISH, &AVATAR[0x166EC + 0x3837] /* "\nSeek ye to become a partial Avatar in all eight virtues, for only then shalt thou be ready to seek the codex!\n" */);
		u4_puts(/*D_747A*/&AVATAR[0x166EC + 0x3837] /* "\nSeek ye to become a partial Avatar in all eight virtues, for only then shalt thou be ready to seek the codex!\n" */);
	} else
	if(Party.mStones == 0) {
		add_npc_talk(LORD_BRITISH, &AVATAR[0x1675C + 0x3837] /* "Go ye now into the depths of the dungeons. Therein recover the 8 colored stones from the altar pedestals in the halls of the dungeons.\n" */);
		u4_puts(/*D_74EA*/&AVATAR[0x1675C + 0x3837] /* "Go ye now into the depths of the dungeons. Therein recover the 8 colored stones from the altar pedestals in the halls of the dungeons.\n" */); C_E1FC();
		add_npc_talk(LORD_BRITISH, &AVATAR[0x167E4 + 0x3837] /* "\nFind the uses of these stones for they can help thee in the Abyss!\n" */);
		u4_puts(/*D_7572*/&AVATAR[0x167E4 + 0x3837] /* "\nFind the uses of these stones for they can help thee in the Abyss!\n" */);
	} else
	if(
		Party._hones | Party._compa |
		Party._valor | Party._justi |
		Party._sacri | Party._honor |
		Party._spiri | Party._humil
	) {
		add_npc_talk(LORD_BRITISH, &AVATAR[0x16829 + 0x3837] /* "Thou art doing very well indeed on the path to Avatarhood! Strive ye to achieve the Elevation in all eight virtues!\n" */);
		u4_puts(/*D_75B7*/&AVATAR[0x16829 + 0x3837] /* "Thou art doing very well indeed on the path to Avatarhood! Strive ye to achieve the Elevation in all eight virtues!\n" */);
	} else
	if(!((Party.mItems >> 4) & (Party.mItems >> 3) & (Party.mItems >> 2) & 1)) {
		add_npc_talk(LORD_BRITISH, &AVATAR[0x1689E + 0x3837] /* "Find ye the Bell, Book and Candle!  With these three things, one may enter the Great Stygian Abyss!\n" */);
		u4_puts(/*D_762C*/&AVATAR[0x1689E + 0x3837] /* "Find ye the Bell, Book and Candle!  With these three things, one may enter the Great Stygian Abyss!\n" */);
	} else
	if(!((Party.mItems >> 5) & (Party.mItems >> 6) & (Party.mItems >> 7) & 1)) {
		add_npc_talk(LORD_BRITISH, &AVATAR[0x16903 + 0x3837] /* "Before thou dost enter the Abyss thou shalt need the Key of Three Parts, and the Word of Passage.\n" */);
		u4_puts(/*D_7691*/&AVATAR[0x16903 + 0x3837] /* "Before thou dost enter the Abyss thou shalt need the Key of Three Parts, and the Word of Passage.\n" */); C_E1FC();
		add_npc_talk(LORD_BRITISH, &AVATAR[0x16966 + 0x3837] /* "\nThen might thou enter the Chamber of the Codex of Ultimate Wisdom!\n" */);
		u4_puts(/*D_76F4*/&AVATAR[0x16966 + 0x3837] /* "\nThen might thou enter the Chamber of the Codex of Ultimate Wisdom!\n" */);
	} else {
		add_npc_talk(LORD_BRITISH, &AVATAR[0x169AB + 0x3837] /* "Thou dost now seem ready to make the final journey into the dark Abyss! Go only with a party of eight!\n" */);
		u4_puts(/*D_7739*/&AVATAR[0x169AB + 0x3837] /* "Thou dost now seem ready to make the final journey into the dark Abyss! Go only with a party of eight!\n" */); C_E1FC();
		add_npc_talk(LORD_BRITISH, &AVATAR[0x16A13 + 0x3837] /* "\nGood Luck, and may the powers of good watch over thee on this thy most perilous endeavor!\n" */);
		u4_puts(/*D_77A1*/&AVATAR[0x16A13 + 0x3837] /* "\nGood Luck, and may the powers of good watch over thee on this thy most perilous endeavor!\n" */); C_E1FC();
		add_npc_talk(LORD_BRITISH, &AVATAR[0x16A6F + 0x3837] /* "\nThe hearts and souls of all Britannia go with thee now. Take care, my friend.\n" */);
		u4_puts(/*D_77FD*/&AVATAR[0x16A6F + 0x3837] /* "\nThe hearts and souls of all Britannia go with thee now. Take care, my friend.\n" */);
	}
}

/*looks for the parameter in Lord British's keywords
 (4 char is enough)
 not found => -1
 else => index of the keywords*/
C_E37E(bp04)
char *bp04;
{
	int bp_02;

	for(bp_02 = 0; D_6FF0[bp_02][0]; bp_02++) {
		if(_strnicmp(bp04, D_6FF0[bp_02], 4) == 0)
			break;
	}
	if(D_6FF0[bp_02][0] == 0)
		return -1;
	return bp_02;
}

C_E3D2(bp04)
register char *bp04;
{
	register int di;

	add_npc_talk(LORD_BRITISH, bp04);
	di = 0;
	while(*bp04) {
		if(*bp04 == '\n') {
			if(di++ == 12) {
				C_E1FC();
				di = 0;
			}
		}
		u4_putc(*bp04);
		add_char_to_text_buffer(*bp04);
		bp04++;
	}
}

C_E408() {
	int bp_02;

	for(bp_02 = Party.f_1d8 - 1; bp_02 >= 0; bp_02 --) {
		if(Party.chara[bp_02]._stat != 'D') {
			Party.chara[bp_02]._stat = 'G';
			Party.chara[bp_02]._HP[0] = Party.chara[bp_02]._HP[1];
		}
	}
}

/*when asekd 'health'*/
C_E442() {
	char bp_02;

	u4_puts(/*D_784D*/&AVATAR[0x16ABF + 0x3837] /* "\n\n\n\n\n\nHe says: I am well, thank ye.\n\nHe asks: Art thou well?\x12\x12\b" */);
	add_npc_talk(LORD_BRITISH, &AVATAR[0x16ACE + 0x3837] /* "\nI am well, thank ye. Art thou well?\n" */);
	set_input_mode(INPUT_MODE_GENERAL_YES_NO);
	bp_02 = AskY_N();
	if(bp_02 == 'Y') {
		u4_puts(/*D_788D*/&AVATAR[0x16AFF + 0x3837] /* "\nHe says: That is good.\n" */);
		add_npc_talk(LORD_BRITISH, &AVATAR[0x16B09 + 0x3837] /* "\nThat is good.\n" */);
	} else if(bp_02 =='N') {
		u4_puts(/*D_78A6*/&AVATAR[0x16B18 + 0x3837] /* "\nHe says: Let me heal thy wounds!\n" */);
		add_npc_talk(LORD_BRITISH, &AVATAR[0x16B22 + 0x3837] /* "\nLet me heal thy wounds!\n" */);
		/*heal party*/
		sound(10, 10);
		Gra_09(); sound(9, 0xc0); Gra_09();
		C_E408();
		dspl_Stats();
	}
}

C_E498(bp04)
int *bp04;
{
	*bp04 += U4_RND1(7)+1;
	*bp04 = u4_min(*bp04, 50);
}

C_E4C3() {
	int loc_A, loc_B, loc_C;

	for(loc_B = 0; loc_B < Party.f_1d8; loc_B++) {
		for(loc_C = loc_A = 100; loc_A <= Party.chara[loc_B]._XP; loc_C += 100, loc_A <<= 1);
		if(Party.chara[loc_B]._HP[1] < loc_C) {
			Party.chara[loc_B]._HP[1] = loc_C;
			Party.chara[loc_B]._HP[0] = loc_C;
			Party.chara[loc_B]._stat = 'G';
			C_E498(&(Party.chara[loc_B]._str));
			C_E498(&(Party.chara[loc_B]._dex));
			C_E498(&(Party.chara[loc_B]._int));
			Gra_CR();
			u4_puts(Party.chara[loc_B]._name);
			add_npc_talk(LORD_BRITISH, Party.chara[loc_B]._name);
			u4_puts(/*D_78C9*/&AVATAR[0x16B3B + 0x3837] /* "\nThou art now Level " */);
			add_npc_talk(LORD_BRITISH, &AVATAR[0x16B3B + 0x3837] /* "\nThou art now Level " */);
			u4_putl(loc_C / 100, 1, '0');
			add_npc_talk_long(LORD_BRITISH, loc_C / 100);
			Gra_CR();
			add_npc_talk(LORD_BRITISH, "\n");
			Gra_09(); sound(9, 0xc0); Gra_09();
			dspl_Stats();
		}
	}
	u4_puts(/*D_78DE*/&AVATAR[0x16B50 + 0x3837] /* "What would thou ask of me?\n" */);
	add_npc_talk(LORD_BRITISH, &AVATAR[0x16B50 + 0x3837] /* "What would thou ask of me?\n" */);
}

/*conversation with Lord british*/
C_E59B()
{
	int bp_02;
	char bp_12[15];

	if(Party.f_1e4 == 0) {
		/*-- first time --*/
		Party.f_1e4 = 1;
		u4_puts(/*D_78FA*/&AVATAR[0x16B6C + 0x3837] /* "\n\n\nLord British rises and says: At long last!\n" */);
		add_npc_talk(LORD_BRITISH, &AVATAR[0x16B8C + 0x3837] /* "At long last!\n" */);
		u4_puts(Party.chara[0]._name);
		add_npc_talk(LORD_BRITISH, Party.chara[0]._name);
		u4_putc(',');
		add_char_to_text_buffer(',');
		add_npc_talk(LORD_BRITISH, ",");
		u4_puts(/*D_7929*/&AVATAR[0x16B9B + 0x3837] /* " thou hast come!  We have waited such a long, long time...\n\n" */);
		add_npc_talk(LORD_BRITISH, &AVATAR[0x16B9B + 0x3837] /* " thou hast come!  We have waited such a long, long time...\n" */);
		C_E1FC();
		add_npc_talk(LORD_BRITISH,  &AVATAR[0x16BF6 + 0x3837] /*"A new age is upon Britannia. The great evil Lords are gone but our people lack direction and purpose in their lives...\n\
A champion of virtue is called for. Thou may be this champion, but only time shall tell. I will aid thee any way that I can!\n"*/);
		u4_puts(/*D_7966*/ &AVATAR[0x16BD8 + 0x3837] /* "\n\nLord British sits and says: A new age is upon Britannia. The great evil Lords are gone but our people lack direction and purpose in their lives...\n\n\n\
A champion of virtue is called for. Thou may be this champion, but only time shall tell. I will aid thee any way that I can!\n"*/);
		u4_puts(/*D_7A7C*/&AVATAR[0x16CEE + 0x3837] /* "How may I help thee?\n" */);
		add_npc_talk(LORD_BRITISH, &AVATAR[0x16CEE + 0x3837] /* "How may I help thee?\n" */);
	} else {
		/*-- second time and after --*/
		if(Party.chara[0]._stat == 'D') {
			/*resurection*/
			Party.chara[0]._stat = 'G';
			u4_puts(Party.chara[0]._name);
			add_npc_talk(LORD_BRITISH, Party.chara[0]._name);
			u4_puts(/*D_7A92*/&AVATAR[0x16D04 + 0x3837] /* ", Thou shalt live again!\n" */);
			add_npc_talk(LORD_BRITISH, &AVATAR[0x16D04 + 0x3837] /* ", Thou shalt live again!\n" */);
			/*heal party*/
			sound(10, 20);
			Gra_09(); sound(9, 0xc0); Gra_09();
			C_E408();
			dspl_Stats();
		}
		u4_puts(/*D_7AAC*/&AVATAR[0x16D1E + 0x3837] /* "\n\n\nLord British says:  Welcome " */);
		add_npc_talk(LORD_BRITISH, &AVATAR[0x16D35 + 0x3837] /* "Welcome " */);
		u4_puts(Party.chara[0]._name);
		add_npc_talk(LORD_BRITISH, Party.chara[0]._name);
		if(Party.f_1d8 >= 3) {
			u4_puts(/*D_7ACC*/&AVATAR[0x16D3E + 0x3837] /* " and thy worthy Adventurers!\n" */);
			add_npc_talk(LORD_BRITISH, &AVATAR[0x16D3E + 0x3837] /* " and thy worthy Adventurers!" */);
		} else if(Party.f_1d8 == 2) {
			u4_puts(/*D_7AEA*/&AVATAR[0x16D5C + 0x3837] /* " and thee also " */);
			add_npc_talk(LORD_BRITISH, &AVATAR[0x16D5C + 0x3837] /* " and thee also " */);
			u4_puts(Party.chara[1]._name);
			add_npc_talk(LORD_BRITISH, Party.chara[1]._name);
			u4_puts(/*D_7AFA*/&AVATAR[0x16D6C + 0x3837] /* "!\n" */);
			add_npc_talk(LORD_BRITISH, "\n");
		} else {
			Gra_CR();
			add_npc_talk(LORD_BRITISH, "\n");
		}
		C_E4C3();
	}

	for(;;) {
		set_input_mode(INPUT_MODE_LOAD_BRITISH_WORD);
		u4_gets(bp_12, 15);
		Gra_CR();
		if(bp_12[0] == 0)
			bp_02 = 0;
		else
			bp_02 = C_E37E(bp_12);
		if(bp_02 == 0)/* "bye" */
			break;
		switch(bp_02) {
			case 2: C_E442(); break;
			case 1: C_E21E(); break;
			case -1: u4_puts(/*D_7AFD*/&AVATAR[0x16D6F + 0x3837] /* "\nHe says: I cannot help thee with that.\n" */); add_npc_talk(LORD_BRITISH, &AVATAR[0x16D79 + 0x3837] /* "I cannot help thee with that.\n" */); break;
			default: add_npc_talk(LORD_BRITISH, D_7028[bp_02 - 3]); 
				C_E3D2(D_7028[bp_02-3]);
		}
		u4_puts(/*D_7B26*/&AVATAR[0x16D98 + 0x3837] /* "\nWhat else?\n" */);
		add_npc_talk(LORD_BRITISH, &AVATAR[0x16D98 + 0x3837] /* "What else?\n" */);
	}

	u4_puts(/*D_7B33*/&AVATAR[0x16DA5 + 0x3837] /* "Lord British says: Fare thee well my friend" */);
	add_npc_talk(LORD_BRITISH, &AVATAR[0x16DB8 + 0x3837] /* "Fare thee well my friend" */);
	if (Party.f_1d8 > 1)
	{
		u4_putc('s');
		add_char_to_text_buffer('s');
		add_npc_talk(LORD_BRITISH, "s");
	}
	u4_puts(/*D_7B5F*/&AVATAR[0x16DD1 + 0x3837] /* "!\n" */);
	add_npc_talk(LORD_BRITISH, &AVATAR[0x16DD1 + 0x3837] /* "!\n" */);
	add_npc_talk(VENDOR_INVALID, "");
}
