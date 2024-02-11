/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

/*thanks to Fenyx4 for the variable
 name issue fix*/

#include "TITLE.H"

#include <malloc.h>
#include <stdlib.h>
#include <string.h>

/*====---- _BSS ----====*/
/*D_6938*/static unsigned static_A;
/*D_693A*/static int static_B;
/*D_693C*/static int static_C;
/*D_693E*/static unsigned char static_D;
/*D_6940*/static void far *static_E;
/*D_6944*/static unsigned char static_F;
/*D_6946*/static unsigned static_G;
/*D_6948*/static char static_H[20];
/*D_695C*/static unsigned char static_I;
/*D_695E*/static unsigned char static_J[8];

#define D_6938 static_A
#define lastVirtue static_B
#define M_or_F static_C
#define tmp_str static_D
#define D_6940 static_E
#define tmp_dex static_F
#define curQuestionIndex static_G
#define player_name static_H
#define tmp_int static_I
#define tmp_karma static_J
/*====---- ----====*/

extern void* _fmalloc(int sz);

#define STR(i) (D_2EE6[(i) - 1])

char *D_2EE6[0x43] = {
	&TITLE[0x4424],
/*0x01*//*D_0424*/
/*
"\n\nEntrusted to deliver an uncounted purse \
of gold, thou dost meet a poor beggar. \
Dost thou \n\nA) deliver the gold knowing \
the Trust in thee was well placed, or \
\n\nB) show Compassion, giving the beggar a \
coin, knowing it won't be missed?",
*/
	&TITLE[0x4509],
/*0x02*//*D_0509*/
/*
"\n\nThou has been prohibited by thy absent \
Lord from joining thy friends in a close \
pitched battle. Dost thou \n\nA) refrain, so \
thou may Honesty claim obedience, or \
\n\nB) show Valor, and aid thy comrades, \
knowing thou may deny it later?",
*/
	&TITLE[0x45EC],
/*0x03*//*D_05EC*/
/*
"\n\nA merchant owes thy friend money, now \
long past due. Thou dost see the same \
merchant drop a purse of gold. Dost thou \
\n\nA) Honestly return the purse intact, or \
\n\nB) Justly give thy friend a portion of \
the gold first?",
*/

	& TITLE[0x46C1],
/*0x04*//*D_06C1*/
/*
"\n\nThee and thy friend are valiant but \
penniless warriors. Thou both go out to \
slay a mighty dragon. Thy friend thinks \
he slew it; thee did. When asked, dost \
thou \n\nA) Truthfully claim the gold, or \
\n\nB) Allow thy friend the large reward?",
*/

	& TITLE[0x47AB],
/*0x05*//*D_07AB*/
/*
"\n\nThou art sworn to protect thy Lord at \
any cost, yet thou know he hath \
committed a crime. Authorities ask thee \
of the affair. Dost thou \n\nA) break thine \
oath by Honestly speaking, or \n\nB) uphold \
Honor by silently keeping thine oath?",
*/

	& TITLE[0x4891],
/*0x06*//*D_0891*/
/*
"\n\nThy friend seeks admittance to thy \
Spiritual order. Thou art asked to vouch \
for his purity of Spirit, of which thou \
art unsure. Dost thou \n\nA) Honestly \
express thy doubt, or \n\nB) Vouch for him \
hoping for his Spiritual improvement?",
*/

	& TITLE[0x4975],
/*0x07*//*D_0975*/
/*
"\n\nThy Lord mistakenly believes he slew a \
dragon. Thou hast proof that thy lance \
felled the beast. When asked, dost thou \
\n\nA) Honestly claim the kill and the \
prize, or \n\nB) Humbly permit thy Lord his \
belief?",
*/

	& TITLE[0x4A40],
/*0x08*//*D_0A40*/
/*
"\n\nThou dost manage to disarm thy mortal \
enemy in a duel. He is at thy mercy. \
Dost thou \n\nA) show Compassion by \
permitting him to yield, or \n\nB) slay him \
as expected of a Valiant duelist?",
*/

	& TITLE[0x4AF6],
/*0x09*//*D_0AF6*/
/*
"\n\nAfter 20 years thou hast found the \
slayer of thy best friends. The villain \
proves to be a man who provides the sole \
support for a young girl. Dost thou \
\n\nA) spare him in Compassion for the girl, \
or \n\nB) slay him in the name of Justice?",
*/

	& TITLE[0x4BE0],
/*0x0a*//*D_0BE0*/
/*
"\n\nThee and thy friends have been routed \
and ordered to retreat. In defiance of \
thy orders, dost thou \n\nA) stop in \
Compassion to aid a wounded companion, \
or \n\nB) Sacrifice thyself to slow the \
pursuing enemy, so others may escape?",
*/

	& TITLE[0x4CC0],
/*0x0b*//*D_0CC0*/
/*
"\n\nThou art sworn to uphold a Lord who \
participates in the forbidden torture of \
prisoners. Each night their cries of \
pain reach thee. Dost thou \n\nA) show \
Compassion by reporting the deeds, or \
\n\nB) Honor thy oath and ignore the deeds?",
*/

	& TITLE[0x4DA5],
/*0x0c*//*D_0DA5*/
/*
"\n\nThou hast been taught to preserve all \
life as sacred. A man lies fatally stung \
by a venomous serpent. He pleads for a \
merciful death. Dost thou \n\nA) show \
Compassion and end his pain, or \n\nB) heed \
thy Spiritual beliefs and refuse?",
*/

	& TITLE[0x4E89],
/*0x0d*//*D_0E89*/
/*
"\n\nAs one of the King's Guard, thy Captain \
has asked that one amongst you visit \
a hospital to cheer the children with \
tales of thy valiant deeds. Dost thou \
\n\nA) Show thy Compassion and play the \
braggart, or \n\nB) Humbly let another go?",
*/

	& TITLE[0x4F6E],
/*0x0e*//*D_0F6E*/
/*
"\n\nThou hast been sent to secure a needed \
treaty with a distant Lord. Thy host is \
agreeable to the proposal but insults \
thy country at dinner. Dost thou \
\n\nA) Valiantly bear the slurs, or \
\n\nB) Justly rise and demand an apology?",
*/

	& TITLE[0x504C],
/*0x0f*//*D_104C*/
/*
"\n\nA mighty knight accosts thee and demands \
thy food. Dost thou \n\nA) Valiantly refuse \
and engage the knight, or \n\nB) Sacrifice \
thy food unto the hungry knight?",
*/

	& TITLE[0x50E6],
/*0x10*//*D_10E6*/
/*
"\n\nDuring battle thou art ordered to guard \
thy commander's empty tent. The battle \
goes poorly and thou dost yearn to aid \
thy fellows. Dost thou \n\nA) Valiantly \
enter the battle to aid thy companions, \
or \n\nB) Honor thy post as guard?",
*/

	& TITLE[0x51C9],
/*0x11*//*D_11C9*/
/*
"\n\nA local bully pushes for a fight. Dost \
thou \n\nA) Valiantly trounce the rogue, or \
\n\nB) Decline, knowing in thy Spirit that \
no lasting good will come of it?",
*/

	& TITLE[0x5261],
/*0x12*//*D_1261*/
/*
"\n\nAlthough a teacher of music, thou art \
a skillful wrestler. Thou hast been \
asked to fight in a local championship. \
Dost thou \n\nA) accept the invitation and \
Valiantly fight to win, or \n\nB) Humbly \
decline knowing thou art sure to win?",
*/

	& TITLE[0x5346],
/*0x13*//*D_1346*/
/*
"\n\nDuring a pitched battle, thou dost see \
a fellow desert his post, endangering \
many. As he flees, he is set upon by \
several enemies. Dost thou \n\nA) Justly let \
him fight alone, or \n\nB) Risk Sacrificing \
thine own life to aid him?",
*/

	& TITLE[0x5425],
/*0x14*//*D_1425*/
/*
"\n\nThou hast sworn to do thy Lord's \
bidding in all. He covets a piece of \
land and orders the owner removed. Dost \
thou \n\nA) serve Justice refusing to act, \
thus being disgraced, or \n\nB) Honor thine \
oath and unfairly evict the landowner?",
*/

	& TITLE[0x550B],
/*0x15*//*D_150B*/
/*
"\n\nThou dost believe that virtue resides \
in all people. Thou dost see a rogue \
steal from thy Lord. Dost thou \n\nA) call \
him to Justice, or \n\nB) personally try to \
sway him back to the Spiritual path of \
good?",
*/

	& TITLE[0x55D4],
/*0x16*//*D_15D4*/
/*
"\n\nUnwitnessed, thou hast slain a great \
dragon in self defense. A poor warrior \
claims the offered reward. Dost thou \
\n\nA) Justly step forward to claim the \
reward, or \n\nB) Humbly go about life, \
secure in thy self esteem?",
*/

	& TITLE[0x56AA],
/*0x17*//*D_16AA*/
/*
"\n\nThou art a bounty hunter sworn to return \
an alleged murder. After his capture \
thou believest him to be innocent. Dost \
thou \n\nA) Sacrifice thy sizable bounty \
for thy belief, or \n\nB) Honor thy oath to \
return him as thou hast promised?",
*/

	& TITLE[0x5791],
/*0x18*//*D_1791*/
/*
"\n\nThou hast spent thy life in charitable \
and righteous work. Thine uncle the \
innkeeper lies ill and asks you to take \
over his tavern. Dost thou \n\nA) Sacrifice \
thy life of purity to aid thy kin, or \
\n\nB) decline & follow thy Spirit's call?",
*/

	& TITLE[0x587B],
/*0x19*//*D_187B*/
/*
"\n\nThou art an elderly, wealthy eccentric. \
Thy end is near. Dost thou \n\nA) donate all \
thy wealth to feed hundreds of starving \
children, and receive public adulation, \
or \n\nB) Humbly live out thy life, willing \
thy fortune to thy heirs?",
*/

	& TITLE[0x595E],
/*0x1a*//*D_195E*/
/*
"\n\nIn thy youth thou pledged to marry thy \
sweetheart. Now thou art on a sacred \
quest in distant lands. Thy sweetheart \
asks thee to keep thy vow. Dost thou \
\n\nA) Honor thy pledge to wed, or \n\nB) follow \
thy Spiritual crusade?",
*/

	& TITLE[0x5A39],
/*0x1b*//*D_1A39*/
/*
"\n\nThou art at a crossroads in thy life. \
Dost thou \n\nA) Choose the Honorable life \
of a Paladin, striving for Truth and \
Courage, or \n\nB) Choose the Humble life \
of a Shepherd, and a world of simplicity \
and peace?",
*/

	& TITLE[0x5B05],
/*0x1c*//*D_1B05*/
/*
"\n\nThy parents wish thee to become an \
apprentice. Two positions are available. \
Dost thou \n\nA) Become an acolyte in the \
Spiritual order, or \n\nB) Become an \
assistant to a humble village cobbler?",
*/

	& TITLE[0x5BBF],
/*0x1d*//*D_1BBF*/
/*
"\n\nThe day is warm, yet there is a \
cooling breeze. The latest in a series \
of personal crises seems insurmountable. \
You are being pulled apart in all \
directions.",
*/

	& TITLE[0x5C60],
/*0x1e*//*D_1C60*/
/*
"\n\nYet this afternoon walk in the country-\
side slowly brings relaxation to your \
harried mind. The soil and strain of \
modern high-tech living begins to wash \
off in layers. That willow tree near the \
stream looks comfortable and inviting.",
*/

	& TITLE[0x5D4B],
/*0x1f*//*D_1D4B*/
/*
"\n\nThe buzz of dragonflies and the whisper \
of the willow's swaying branches bring \
a deep peace. Searching inward for \
tranquility and happiness, you close \
your eyes.",
*/

	& TITLE[0x5DEE],
/*0x20*//*D_1DEE*/
/*
"\n\nA high-pitched cascading sound like \
crystal wind-chimes impinges on your \
floating awareness. As you open your \
eyes, you see a shimmering blueness rise \
from the ground. The sound seems to be \
emanating from this glowing portal.",
*/

	& TITLE[0x5ED2],
/*0x21*//*D_1ED2*/
/*
"\n\nIt is difficult to look at the blueness. \
Light seems to bend and distort around \
it, while the sound waves become so \
intense, they appear to become visible.",
*/

	& TITLE[0x5F6E],
/*0x22*//*D_1F6E*/
/*
"\n\nThe portal hangs there for a moment; \
then, with the rush of an imploding \
vacuum, it sinks into the ground. \
Something remains suspended in mid-air \
for a moment before falling to earth \
with a heavy thud.",
*/

	& TITLE[0x6038],
/*0x23*//*D_2038*/
/*
"\n\nSomewhat shaken by this vision, you \
rise to your feet to investigate. A \
crude circle of stones surrounds the \
spot where the portal appeared. There is \
something glinting in the grass.",
*/

	& TITLE[0x60F0],
/*0x24*//*D_20F0*/
/*
"\n\nYou pick up an amulet shaped like a \
cross with a loop at the top. It is an \
Ankh, the sacred symbol of life and \
rebirth. But this could not have made \
the thud, so you look again and find \
a large book wrapped in thick cloth!",
*/

	& TITLE[0x61D1],
/*0x25*//*D_21D1*/
/*
"\n\nWith trembling hands you unwrap the \
book. Behold, the cloth is a map, and \
within lies not one book, but two. The \
map is of a land strange to you, and the \
style speaks of ancient cartography.",
*/

	& TITLE[0x6292],
/*0x26*//*D_2292*/
/*
"\n\nThe script on the cover of the first \
book is arcane but readable. The title \
is: \n\
        The History of Britannia \n\
               as told by \n\
            Kyle the Younger",
*/

	& TITLE[0x633B],
/*0x27*//*D_233B*/
/*
"\n\nThe other book is disturbing to look at. \
Its small cover appears to be fashioned \
out of some sort of leathery hide, but \
from what creature is uncertain. The \
reddish-black skin radiates an intense \
aura suggestive of ancient power.",
*/

	& TITLE[0x6422],
/*0x28*//*D_2422*/
/*
"\n\nThe tongue of the title is beyond your \
ken. You dare not open the book and \
disturb whatever sleeps within. You \
decide to peruse the History. Settling \
back under the willow tree, you open the \
book.",
*/

	& TITLE[0x64EA],
/*0x29*//*D_24EA*/
/*
"\n \
\n \
\n(You read the Book of History)",
*/

	& TITLE[0x650F],
/*0x2a*//*D_250F*/
/*
"\n \
\n \
\n(No, really! Read the Book of History!)",
*/

	& TITLE[0x6539],
/*0x2b*//*D_2539*/
/*
"\n\nClosing the book, you again pick up \
the Ankh. As you hold it, you begin to \
hear a hauntingly familiar, lute-like \
sound wafting over a nearby hill. Still \
clutching the strange artifacts, you \
rise unbidden and climb the slope.",
*/

	& TITLE[0x661C],
/*0x2c*//*D_261C*/
/*
"\n\nIn the valley below you see what appears \
to be a fair. It seems strange that you \
came that way earlier and noticed \
nothing. As you mull this over, your \
feet carry you down towards the site.",
*/

	& TITLE[0x66DC],
/*0x2d*//*D_26DC*/
/*
"\n\nThis is no ordinary travelling \
carnival, but a Renaissance Fair. The \
pennants on the tent tops blow briskly \
in the late afternoon breeze.",
*/

	& TITLE[0x6767],
/*0x2e*//*D_2767*/
/*
"\n\nThe ticket taker at the Renaissance Fair's gate \
starts to ask you for money, but upon \
spotting your Ankh says, \"Welcome, \
friend. Enter in peace and find your \
path.\n",
*/

	& TITLE[0x6804],
/*0x2f*//*D_2804*/
/*
"\n\nThe music continues to pull you \
forward amongst the merchants and \
vendors. Glimpses of fabulous treasures \
can be seen in some of the shadowy \
booths.",
*/

	& TITLE[0x689A],
/*0x30*//*D_289A*/
/*
"\n\nThese people are very happy. They seem \
to glow with an inner light. Some look \
up as you pass and smile, but you cannot \
stop - the music compels you to move \
onward through the crowd.",
*/

	& TITLE[0x6952],
/*0x31*//*D_2952*/
/*
"\n\nThrough the gathering dusk you see a \
secluded gypsy wagon sitting off in the \
woods. The music seems to emanate from \
the wagon. As you draw near, a woman's \
voice weaves into the music, saying: \
\"You may approach, O seeker.\"",
*/

	& TITLE[0x6A32],
/*0x32*//*D_2A32*/
/*
"\n\nYou enter to find an old gypsy sitting \
in a small curtained room. She wears an \
Ankh around her neck. In front of her is \
a round table covered in deep green \
velvet. The room smells so heavily of \
incense that you feel dizzy.",
*/

	& TITLE[0x6B13],
/*0x33*//*D_2B13*/
/*
"\n\nSeeing the Ankh, the ancient gypsy \
smiles and warns you never to part with \
it. \"We have been waiting such a long \
time, but at last you have come. Sit \
here and I shall read the path of your \
future.\n",
*/

	& TITLE[0x6BDB],
/*0x34*//*D_2BDB*/
/*
"\n\nUpon the table she places a curious \
wooden object like an abacus but without \
beads. In her hands she holds eight \
unusual cards. \"Let us begin the \
casting.\n",
*/

/*0x35*//*D_2C79*/& TITLE[0x6C79], //"\n\nThe gypsy places the first two cards ",
/*0x36*//*D_2C9F*/& TITLE[0x6C9F], //"\n\nThe gypsy places two more of the cards ",
/*0x37*//*D_2CC7*/& TITLE[0x6CC7], //"\n\nThe gypsy places the last two cards ",
/*0x38*//*D_2CEC*/& TITLE[0x6CEC], //"upon the table. They are the cards of ",

/*0x39*//*D_2D14*/&TITLE[0x6D14] /* "Honesty" */,
/*0x3a*//*D_2D1C*/&TITLE[0x6D1C] /* "Compassion" */,
/*0x3b*//*D_2D27*/&TITLE[0x6D27] /* "Valor" */,
/*0x3c*//*D_2D2D*/&TITLE[0x6D2D] /* "Justice" */,
/*0x3d*//*D_2D35*/&TITLE[0x6D35] /* "Sacrifice" */,
/*0x3e*//*D_2D3F*/&TITLE[0x6D3F] /* "Honor" */,
/*0x3f*//*D_2D45*/&TITLE[0x6D45] /* "Spirituality" */,
/*0x40*//*D_2D52*/&TITLE[0x6D52] /* "Humility" */,
/*0x41*//*D_2D5B*/"",

	& TITLE[0x6D5C],
/*0x42*//*D_2D5C*/
/*
"\n\nWith the final choice, the incense \
swells up around you. The gypsy speaks \
as if from a great distance, her voice \
growing fainter with each word:\n\
\"So be it! Thy path is chosen!\"",
*/

	& TITLE[0x6E0F],
/*0x43*//*D_2E0F*/
/*
"\n\nThere is a moment of intense, wrenching \
vertigo. As you open your eyes, a \
voice whispers within your mind, \"Seek \
the counsel of thy sovereign.\" After a \
moment, the spinning subsides, and you \
open your eyes to..."
*/
};

extern void add_char_to_text_buffer(char ch);

/*C_261D*/u4_puts(bp04)
char *bp04;
{
	char bp_02;

	while(bp_02 = *bp04) {
		if ((*bp04 == 'A' || *bp04 == 'B') & (*(bp04 + 1) == ')')) {
			// add a newlines between questions for better text formattng
			add_char_to_text_buffer('\n');			
		}

		if ((*bp04 == ' ') && (*(bp04 + 1) == ' ')) {
			// remove double spaces
		} else if(bp_02 == '\n') {
			add_char_to_text_buffer(bp_02);
			txt_Y ++;
			txt_X = 0;
		} else {
			add_char_to_text_buffer(*bp04);
			u4_putc(*bp04);
		}
		bp04 ++;
	}
}

/*text input?*/
C_2656(si/*bp06*/, bp04)
register char *si;
unsigned bp04;
{
	register unsigned di;
	unsigned bp_04;

	set_input_mode(INPUT_MODE_NAME);
	di = 0;
	do {
		bp_04 = u_kbread();
		switch(bp_04) {
			case KBD_BS: case KBD_0e7f: case KBD_LEFT:
				if(di == 0) {
					sound_1();
				} else {
					u4_putc(8);
					add_char_to_text_buffer(8);
					di--;
					si[di] = ' ';
				}
			break;
			default:
				bp_04 &= 0x00ff;
				if(bp04 - 1 == di || bp_04 < 0x20 || bp_04 >= 0x80) {
					sound_1();
				} else {
					si[di] = bp_04;
					u4_putc(bp_04);
					add_char_to_text_buffer(bp_04);
					di ++;
				}
			break;
			case KBD_ENTER:
				si[di] = 0;
			break;
		}
	} while(bp_04 != KBD_ENTER);
	bp_04 = 0;
	while(si[bp_04++] == ' ');
	if(si[--bp_04]) {
		di = 0;
		while(si[bp_04])
			si[di++] = si[bp_04++];
	} else {
		di = bp_04;
	}
	do
		si[di] = 0;
	while(si[--di] == ' ');
}

C_271D(bp04)
char *bp04;
{
	Gra_5();
	txt_X = 0;
	txt_Y = 19;
	u4_puts(bp04);
	set_input_mode(INPUT_MODE_GENERAL_TEXT_CONTINUE);
	u_kbread();
}

/*moongate animation #1*/
C_273E()
{
	int bp_02, bp_04;

	for(bp_02 = 1; bp_02 <= 23; bp_02++) {
		Gra_3(3,      bp_02, 0, 152, D_6940, 92 - bp_02, -1, 9);
		Gra_3(3, 24 - bp_02, 9,  68, D_6940, 68,         -1, 9);
		for(bp_04 = 1; bp_04 <= 10000; bp_04 ++);
	}
	Gra_3(3, 24, 0, 152, D_6940, 68, -1, 9);
}

/*moongate animation #2*/
C_27E0()
{
	int bp_02, bp_04;

	for(bp_02 = 1; bp_02 <= 23; bp_02 ++) {
		Gra_3(3,      bp_02, 3, 152, D_6940,         68, -1, 9);
		Gra_3(3, 24 - bp_02, 0, 152, D_6940, bp_02 + 68, -1, 9);
		for(bp_04 = 1; bp_04 <= 10000; bp_04 ++);
	}
	Gra_3(3, 24, 3, 152, D_6940, 68, -1, 9);
}

void * get_screen_buffer()
{
	return D_6940;
}

C_2883()
{
	unsigned bp_02;

	if((D_6940 = _fmalloc((D_7078 == 1)?0x4000:0x8000)) == 0)
		exit(0x5e);
	bp_02 = 0x1d;
	Gra_inflate((D_7078 == 1)?/*D_2F6C*/ &TITLE[0x6F6C] /* "tree.pic" */:/*D_2F75*/ &TITLE[0x6F75] /* "tree.ega" */, D_6940);
	Gra_clrscr();
	Gra_3(40, 152, 0, 0, D_6940, 0, -1, 0);
	while(bp_02 != 0x35) {
		Gra_5();
		txt_X = 0;
		txt_Y = 19;
		u4_puts("\n\n");
		u4_puts(STR(bp_02++));
		switch(bp_02 - 1) {
			case 0x20:
				C_273E();/*moongate animation #1*/
			break;
			case 0x22:
				C_27E0();/*moongate animation #2*/
				Gra_inflate((D_7078 == 1)?/*D_2F7E*/&TITLE[0x6F7E] /* "portal.pic" */:/*D_2F89*/&TITLE[0x6F89] /* "portal.ega" */, D_6940);
			break;
			case 0x23:
				Gra_3(40, 152, 0, 0, D_6940, 0, -1, 0);
				Gra_inflate((D_7078 == 1)?/*D_2F94*/&TITLE[0x6F6C] /* "tree.pic" */:/*D_2F9D*/&TITLE[0x6F75] /* "tree.ega" */, D_6940);
			break;
			case 0x28:
				Gra_3(40, 152, 0, 0, D_6940, 0, -1, 0);
				Gra_inflate((D_7078 == 1)?/*D_2FA6*/&TITLE[0x6FA6] /* "outside.pic" */:/*D_2FB2*/&TITLE[0x6FB2] /* "outside.ega" */, D_6940);
			break;
			case 0x2c:
				Gra_3(40, 152, 0, 0, D_6940, 0, -1, 0);
				Gra_inflate((D_7078 == 1)?/*D_2FBE*/&TITLE[0x6FBE] /* "inside.pic" */:/*D_2FC9*/&TITLE[0x6FC9] /* "inside.ega" */, D_6940);
			break;
			case 0x2e:
				Gra_3(40, 152, 0, 0, D_6940, 0, -1, 0);
				Gra_inflate((D_7078 == 1)?/*D_2FDE*/&TITLE[0x6FD4] /* "wagon.pic" */:/*D_2FD4*/&TITLE[0x6FDE] /* "wagon.ega" */, D_6940);
			break;
			case 0x31:
				Gra_3(40, 152, 0, 0, D_6940, 0, -1, 0);
				Gra_inflate((D_7078 == 1)?/*D_2FE8*/&TITLE[0x6FE8] /* "gypsy.pic" */:/*D_2FF2*/&TITLE[0x6FF2] /* "gypsy.ega" */, D_6940);
			break;
			case 0x32:
				Gra_3(40, 152, 0, 0, D_6940, 0, -1, 0);
				Gra_inflate((D_7078 == 1)?/*D_2FFC*/&TITLE[0x6FFC] /* "abacus.pic" */:/*D_3007*/&TITLE[0x7007] /* "abacus.ega" */, D_6940);
			break;
			case 0x34:
				Gra_3(40, 152, 0, 0, D_6940, 0, -1, 0);
			break;
		}
		set_input_mode(INPUT_MODE_GENERAL_TEXT_CONTINUE);
		u_kbflush();
		u_kbread();
	}
}

U16 * D_3012 = &TITLE[0x7012]; // {0x18, 0x28, 0x36, 0x44, 0x52, 0x62, 0x70};

/*draw abacus beas*/
C_2B2A(bp08, bp06, bp04)
unsigned bp08;/*0:chosen, 1:discarded ?*/
unsigned bp06;/*virtue index*/
unsigned bp04;/*line*/
{
	Gra_3(1, 12, (bp08 == 0)?1:3, 187, D_6940, D_3012[bp04], -1, bp06 + 16);
}

char *D_307E[] = {
	/*D_3020*/&TITLE[0x7020] /* "HONCOM.EGA" */,
	/*D_302B*/&TITLE[0x702B] /* "VALJUS.EGA" */,
	/*D_3036*/&TITLE[0x7036] /* "SACHONOR.EGA" */,
	/*D_3043*/&TITLE[0x7043] /* "SPIRHUM.EGA" */,

	/*D_304F*/&TITLE[0x704F] /* "HONCOM.PIC" */,
	/*D_305A*/&TITLE[0x705A] /* "VALJUS.PIC" */,
	/*D_3065*/&TITLE[0x7065] /* "SACHONOR.PIC" */,
	/*D_3072*/&TITLE[0x7072] /* "SPIRHUM.PIC" */
};

/*draw virtue card*/
C_2B6D(bp06, bp04)
unsigned bp06;/*virtue index*/
unsigned bp04;/*left or right*/
{
	void far *bp_04;

	bp06 += ((D_7078 == 1)?1:0) * 8;
	if((bp_04 = _fmalloc((D_7078 == 1)?0x4000:0x8000)) == 0)
		exit(0x5d);
	Gra_inflate(D_307E[bp06/2], bp_04);
	Gra_3(12, 124, ((bp06&1) == 0)?1:27, 12, bp_04, 12, -1, (bp04 == 0)?1:27);
	_ffree(bp_04);
}

/*characterisics increments by virtue*/
unsigned char * D_30B2 = &TITLE[0x70B2]; // {0, 0, 3, 0, 1, 1, 1, 0};/*str*/
unsigned char * D_30BA = &TITLE[0x70BA]; // {0, 3, 0, 1, 1, 0, 1, 0};/*dex*/
unsigned char * D_30C2 = &TITLE[0x70C2]; // {3, 0, 0, 1, 0, 1, 1, 0};/*int*/

/*dilemmas text indexes*/
unsigned char * D_30CA = &TITLE[0x70CA]; // {
#if 0
    /*0x00*/0x01 - 1,/*Honesty vs {Compassion|...|Humility}*/
	/*0x06*/0x08 - 2,/*Compassion vs {Valor|...|Humility}*/
	/*0x0B*/0x0e - 3,/*Valor vs {Justice|...|Humility}*/
	/*0x0F*/0x13 - 4,/*Justice vs {Sacrifice|...|Humility}*/
	/*0x12*/0x17 - 5,/*Sacrifice vs {Honor|...|Humility}*/
	/*0x14*/0x1a - 6,/*Honor vs {Spirituality|Humility}*/
	/*0x15*/0x1c - 7 /*Spirituality vs Humility*/
};
#endif

C_2C12()
{
	int loc_A;
	unsigned char loc_B, loc_C;
	unsigned char loc_D[8];

	tmp_str = tmp_int = tmp_dex = 15;
	curQuestionIndex = 0;
	for(loc_A = 7; loc_A >= 0; loc_A--) {
		loc_D[loc_A] = 0;
		tmp_karma[loc_A] = 50;
	}
	do {
		if(curQuestionIndex == 4 || curQuestionIndex == 6) {
			for(loc_A = 7; loc_A >= 0; loc_A --)
				if(loc_D[loc_A] < 0x80)
					loc_D[loc_A] = 0;
		}
		Gra_5();
		txt_X = 0;
		txt_Y = 19;
		do
			loc_B = u_rand_a() & 7;
		while(loc_D[loc_B&0xff]);
		do
			loc_C = u_rand_a() & 7;
		while(loc_D[loc_C&0xff] || loc_C == loc_B);
		if(loc_C < loc_B) {
			/*swap(loc_B,loc_C);*/
			loc_A = loc_B; loc_B = loc_C; loc_C = loc_A;
		}
		u4_puts("\n\n");
		if(curQuestionIndex == 0)
			u4_puts(/*D_2F4E*/STR(0x35));
		else if(curQuestionIndex == 6)
			u4_puts(/*D_2F52*/STR(0x37));
		else
			u4_puts(/*D_2F50*/STR(0x36));
		u4_puts(/*D_2F54*/STR(0x38));
		C_2B6D(loc_B, 0);
		u4_puts(STR(0x39 + loc_B));
		u4_puts(/*D_308E*/&TITLE[0x708E] /* " and " */);
		C_2B6D(loc_C, 1);
		u4_puts(STR(0x39 + loc_C));
		u4_puts(/*D_3094*/ &TITLE[0x7094] /* ". She says \"Consider this:\"" */);
		set_input_mode(INPUT_MODE_GENERAL_TEXT_CONTINUE);
		u_kbflush();
		u_kbread();
		Gra_5();
		txt_X = 0;
		txt_Y = 19;
		u4_puts("\n\n");
		set_input_mode(INPUT_MODE_A_OR_B_CHOICE);
		u4_puts(STR(D_30CA[loc_B] + loc_C));
		do {
			loc_A = u_kbread();
			if(u4_isupper((unsigned char)loc_A))
				loc_A += 'a' - 'A';
		} while(loc_A != KBD_a && loc_A != KBD_b);
		set_input_mode(-1);
		if(loc_A == KBD_b) {
			/*swap(loc_B,loc_C);*/
			loc_A = loc_B; loc_B = loc_C; loc_C = loc_A;
		}
		/*chosen virtue*/
		lastVirtue = loc_B;
		loc_D[lastVirtue] = 1;
		tmp_karma[lastVirtue] += 5;
		tmp_str += D_30B2[lastVirtue];
		tmp_dex += D_30BA[lastVirtue];
		tmp_int += D_30C2[lastVirtue];
		C_2B2A(0, lastVirtue, curQuestionIndex);
		/*discarded virtue*/
		loc_D[loc_C] = 0xff;
		C_2B2A(1, loc_C, curQuestionIndex);
	} while(++curQuestionIndex != 7);
	u4_puts("\n\n");
	C_271D(/*D_2F68*/STR(0x42));
	u4_puts("\n\n");
	C_271D(/*D_2F6A*/STR(0x43));
}

unsigned char * D_30DC = &TITLE[0x70DC]; // {0xE7, 0x53, 0x23, 0x3B, 0x9E, 0x69, 0x17, 0xBA};
unsigned char * D_30E4 = &TITLE[0x70E4]; // {0x88, 0x69, 0xDD, 0x2C, 0x15, 0xB7, 0x81, 0xAB};

U16 /*D_30EC*/*pKarmas[] = {
	&(Party._hones),
	&(Party._compa),
	&(Party._valor),
	&(Party._justi),
	&(Party._sacri),
	&(Party._honor),
	&(Party._spiri),
	&(Party._humil)
};

C_2E04()
{
	int loc_A;
	struct tChara loc_B;

	if(Load(/*D_30D1*/ &TITLE[0x70D1] /* "PARTY.NEW" */, sizeof(struct tParty), &Party) == -1)
		exit(3);
	Party._x = D_30DC[lastVirtue];
	Party._y = D_30E4[lastVirtue];
	Party.f_1d8 = 1;
	for(loc_A = 7; loc_A >= 0; loc_A --)
		*(pKarmas[loc_A]) = tmp_karma[loc_A];
	/*-- swap(&(Party.chara[lastVirtue]), &(Party.chara[0])) --*/
	memcpy(&loc_B, &(Party.chara[lastVirtue]), sizeof(struct tChara));
	memcpy(&(Party.chara[lastVirtue]), &(Party.chara[0]), sizeof(struct tChara));
	memcpy(&(Party.chara[0]), &loc_B, sizeof(struct tChara));
	/*-- --*/
	strcpy(Party.chara[0]._name, player_name);
	Party.chara[0]._str = tmp_str;
	Party.chara[0]._int = tmp_int;
	Party.chara[0]._dex = tmp_dex;
	Party.chara[0].p_24 = (M_or_F == 'M')?0x0b:0x0c;
	for(loc_A = 31; loc_A >= 0; loc_A --) {
		D_6976._npc._000[loc_A] =
		D_6976._npc._020[loc_A] =
		D_6976._npc._040[loc_A] =
		D_6976._npc._060[loc_A] =
		D_6976._npc._080[loc_A] =
		D_6976._npc._0A0[loc_A] =
		D_6976._npc._0C0[loc_A] =
		D_6976._npc._0E0[loc_A] = 0;
	}
}

/*confirmFileInDrive?*/
C_2F07(bp06, bp04)/*%%% hacked %%%*/
char *bp06;
char *bp04;
{
	// always have either disk inserted
	return 0;
#if DISABLED
	int bp_02;

	Gra_clrscr();
	txt_Y = 10;
	txt_X = (40 - (unsigned)strlen(bp06)) / 2;
	u4_puts(bp06);
	txt_Y ++;
	txt_X = 18;
	u4_puts(/*D_30FC*/&TITLE[0x4420] /* "and" */);
	txt_Y ++;
	txt_X = 11;
	u4_puts(/*D_3100*/&TITLE[0x7100] /* "press drive letter" */);
	set_input_mode(INPUT_MODE_DRIVE_LETTER);
	while(!u_kbhit());
	do {
		set_input_mode(INPUT_MODE_DRIVE_LETTER);
		bp_02 = u_kbread() & 0xff;
		u4_toupper(bp_02);
		if(bp_02 != 'B' || D_7082 != 0) {
			if(bp_02 >= 'A' && bp_02 <= 'P') {
				bp_02 &= 0xf;
				C_3299(bp_02);
			}
		}
		if((C_3290() & 0xff) == bp_02 && C_32AB(bp04))
			return 0;
		sound_1();
		set_input_mode(INPUT_MODE_DRIVE_LETTER);
		while(!u_kbhit());
	} while(1);
#endif
}

/*prepare player disk?*/
C_2FB8()
{
	unsigned bp_02;

	bp_02 = C_3290() & 0xff;
	C_2F07(/*D_311D*/&TITLE[0x711D] /* "Insert PLAYER disk" */, /*D_3113*/&TITLE[0x7113] /* "WORLD.MAP" */);
	D_6938 = C_3290() & 0xff;
	if(Save(/*D_3130*/&TITLE[0x7130] /* "PARTY.SAV" */, sizeof(struct tParty), &Party) == -1)
		exit(3);
	if(Save(/*D_313A*/&TITLE[0x713A] /* "MONSTERS.SAV" */, sizeof(struct tNPC), &(D_6976._npc)) == -1)
		exit(3);
	if(D_6938 == bp_02)
		C_2F07(/*D_3152*/&TITLE[0x7152] /* "Re-Insert PROGRAM disk" */, /*D_3147*/&TITLE[0x7147] /* "AVATAR.EXE" */);
	else
		C_3299(bp_02);
}

extern C_0B1E(int, int, char *);
extern int QUIT;

C_3030()
{
	Gra_2();
	C_0B1E(16, 4, /*D_3169*/&TITLE[0x7169] /* "By what name shalt thou be known" */);
	C_0B1E(17, 4, /*D_318A*/&TITLE[0x718A] /* "in this world and time?" */);
	txt_X = 12;
	txt_Y = 19;
	C_2656(player_name, 12);
	if(player_name[0] == 0) {
		_ffree(pAnim);
		_ffree(pShapes);
		return 0;
	}
	Gra_2();
	C_0B1E(17, 4, /*D_31A2*/&TITLE[0x71A2] /* "Art thou Male or Female? " */);
	set_input_mode(INPUT_MODE_M_OR_F);
	M_or_F = u_kbread() & 0xff;
	u4_toupper(M_or_F);
	while(M_or_F != 'M' && M_or_F != 'F') {
		sound_1();
		M_or_F = u_kbread() & 0xff;
		if(M_or_F == 0x1b || M_or_F == KBD_ENTER || M_or_F == KBD_SPACE) {
			_ffree(pAnim);
			_ffree(pShapes);
			return 0;
		}
		u4_toupper(M_or_F);
	}
	u4_putc(M_or_F);
	_ffree(pAnim);
	_ffree(pShapes);
	D_6E80 = 0;

	C_2883();/*introduction*/
	C_2C12();/*gypsy*/
	C_2E04();/*prepare party structure*/
	C_2FB8();/*prepare player disk?*/
	_ffree(D_6940);
	/*-- clean & return to dos --*/
	_ffree(pCharset);
	low_clean();
	QUIT = 1; //exit('A' + D_6938 - 1);
}
