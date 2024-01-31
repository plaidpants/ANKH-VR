/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "U4.H"

#include <string.h>
#include <stdlib.h>
#ifndef _WINDOWS
#include <android/log.h>
#endif
C_C445() {}
C_C44A() {}
C_C44F() {}

static int D_9138;

/*confirmFileInDrive?*/
C_C454(bp06, bp04)
char *bp06;
char *bp04;
{
	// always have either disk inserted
	return 0;
	int bp_02;

	if(!C_182F(bp04)) {
		CurMode = MOD_VISION;
		txt_Y = 10;
		txt_X = (unsigned int)(40 - strlen(bp06)) >> 1;
		u4_puts(bp06);
		txt_Y ++;
		txt_X = 18;
		u4_puts(/*D_3024*/&AVATAR[0xF339 + 0x5] /* "and" */);
		txt_Y ++;
		txt_X = 11;
		u4_puts(/*D_3028*/&AVATAR[0x122A3 + 0x2E2E] /* "press drive letter" */);
		set_input_mode(INPUT_MODE_DRIVE_LETTER);
		while(!u_kbhit());
		do {
			set_input_mode(INPUT_MODE_DRIVE_LETTER);
			bp_02 = (unsigned char)u_kbread();
			u4_toupper(bp_02);
			if(bp_02 != 'B' || D_9138 != 0) {
				if(bp_02 >= 'A' && bp_02 <= 'P') {
					bp_02 &= 0xf;
					C_181D(bp_02);
				}
			}
			if(C_1814() == bp_02 && C_182F(bp04))
				return 0;
			sound(1,0);
			set_input_mode(INPUT_MODE_DRIVE_LETTER);
			while(!u_kbhit());
		} while(1);
	}
}

#define NPARAM (far_psp->_80[0])
#define PARAM0 (far_psp->_80[2])
#define PARAM1 (far_psp->_80[4])

extern int low_gra();

#if 0
// Function to dump a binary buffer
void dumpBuffer(const char* buffer, size_t size) {
	const int chunk_size = 16; // Number of bytes to print per line
	char hex_chunk[chunk_size * 3 + 1]; // Each byte takes 2 hex characters + space

	for (size_t i = 0; i < size; i += chunk_size) {
		// Initialize the hex_chunk buffer for this iteration
		memset(hex_chunk, 0, sizeof(hex_chunk));

		// Print up to chunk_size bytes in hexadecimal format
		for (int j = 0; j < chunk_size && i + j < size; j++) {
			snprintf(hex_chunk + j * 3, 4, "%02X ", (unsigned char)buffer[i + j]);
		}

		// Log the chunk as an info message
		__android_log_print(ANDROID_LOG_INFO, "ANKH", "Dump buffer: %s", hex_chunk);
	}
}

// Function to dump a tChara structure
void dumpChara(const struct tChara* chara) {
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_HP[0]: %u\n", chara->_HP[0]);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_HP[1]: %u\n", chara->_HP[1]);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_XP: %u\n", chara->_XP);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_str: %u\n", chara->_str);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_dex: %u\n", chara->_dex);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_int: %u\n", chara->_int);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_MP: %u\n", chara->_MP);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_weapon: %u\n", chara->_weapon);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_armor: %u\n", chara->_armor);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_name: %s\n", chara->_name);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "p_24: %c\n", chara->p_24);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_class: %c\n", chara->_class);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_stat: %c\n", chara->_stat);
}

// Function to dump a tParty structure
void dumpParty(const struct tParty* party) {
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "f_000: %u\n", party->f_000);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_moves: %u\n", party->_moves);
	for (int i = 0; i < 8; i++) {
		__android_log_print(ANDROID_LOG_INFO, "ANKH", "chara[%d]:\n", i);
		dumpChara(&(party->chara[i]));
	}
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_food: %d\n", party->_food);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_gold: %u\n", party->_gold);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_hones: %u\n", party->_hones);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_compa: %u\n", party->_compa);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_valor: %u\n", party->_valor);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_justi: %u\n", party->_justi);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_sacri: %u\n", party->_sacri);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_honor: %u\n", party->_honor);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_spiri: %u\n", party->_spiri);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_humil: %u\n", party->_humil);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_torches: %u\n", party->_torches);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_gems: %u\n", party->_gems);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_keys: %u\n", party->_keys);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_sextants: %u\n", party->_sextants);

	for (int i = 0; i < 8; i++) {
		__android_log_print(ANDROID_LOG_INFO, "ANKH", "_armors[%d]: %u\n", i, party->_armors[i]);
	}

	for (int i = 0; i < 16; i++) {
		__android_log_print(ANDROID_LOG_INFO, "ANKH", "_weapons[%d]: %u\n", i, party->_weapons[i]);
	}

	for (int i = 0; i < 8; i++) {
		__android_log_print(ANDROID_LOG_INFO, "ANKH", "_reagents[%d]: %u\n", i, party->_reagents[i]);
	}

	for (int i = 0; i < 26; i++) {
		__android_log_print(ANDROID_LOG_INFO, "ANKH", "_mixtures[%d]: %u\n", i, party->_mixtures[i]);
	}

	__android_log_print(ANDROID_LOG_INFO, "ANKH", "mItems: %u\n", party->mItems);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_x: %u\n", party->_x);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_y: %u\n", party->_y);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "mStones: %u\n", party->mStones);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "mRunes: %u\n", party->mRunes);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "f_1d8: %u\n", party->f_1d8);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_tile: %u\n", party->_tile);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "f_1dc: %u\n", party->f_1dc);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_trammel: %u\n", party->_trammel);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_felucca: %u\n", party->_felucca);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_ship: %u\n", party->_ship);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "f_1e4: %u\n", party->f_1e4);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "f_1e6: %u\n", party->f_1e6);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "f_1e8: %u\n", party->f_1e8);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "f_1ea: %u\n", party->f_1ea);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "f_1ec: %u\n", party->f_1ec);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "out_x: %u\n", party->out_x);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "out_y: %u\n", party->out_y);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_dir: %u\n", party->_dir);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_z: %d\n", party->_z);
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "_loc: %u\n", party->_loc);
}
#endif

C_C51C()
{
	int bp_02, bp_04;

	u4_toupper(PARAM1);
	u4_toupper(PARAM0);
	/*set drive number*/
	if(NPARAM < 4) {
		D_9138 = ((equip_flags & 0xc0) >> 6)?1:0;
		PARAM1 = D_9138 + '0';
	} else if (PARAM1 == '0') {
		D_9138 = 0;
	} else if (PARAM1 == '1') {
		D_9138 = 1;
	} else {
		D_9138 = ((equip_flags & 0xc0) >> 6)?1:0;
	}
	/*set graphic type*/
	if(NPARAM < 2) {
		D_943A = low_gra();
	} else if(PARAM0 == 'C') {
		D_943A = 1;
	} else if(PARAM0 == 'E') {
		D_943A = 2;
	} else if(PARAM0 == 'T') {
		D_943A = 3;
	} else {
		D_943A = low_gra();
	}

	/* */
	pShapes = dalloc((D_943A == 1)?0x4000:0x8000);
	pCharset = dalloc((D_943A == 1)?0x0800:0x1000);
	if(pShapes == 0 || pCharset == 0)
		exit(3);
	if(Load((D_943A == 1)?/*D_303B*/&AVATAR[0x122B6 + 0x2E2E] /* "CHARSET.CGA" */:/*D_3047*/&AVATAR[0x122C2 + 0x2E2E] /* "CHARSET.EGA" */, (D_943A == 1)?0x0800:0x1000, pCharset) == -1)
		exit(3);
	if(D_943A == 1) {
		if(Load(/*D_3053*/&AVATAR[0x122CE + 0x2E2E] /* "SHAPES.CGA" */, 0x4000, pShapes) == -1)
			exit(3);
	}
	bp_02 = C_1814();
	switch(D_943A) {
		case 1:
			dfree(patch_tandy);
			patch_tandy = 0;
			C_20C1(/*D_305E*/&AVATAR[0x122D9 + 0x2E2E] /* "CGA.DRV" */);
		break;
		case 2:
			dfree(patch_tandy);
			patch_tandy = 0;
			C_20C1(/*D_3066*/&AVATAR[0x122E1 + 0x2E2E] /* "EGA.DRV" */);
		break;
		case 3:
			C_20C1(/*D_306E*/&AVATAR[0x122E9 + 0x2E2E] /* "TANDY.DRV" */);
		break;
		case -1:
			//Console(/*D_3078*/&AVATAR[0x122F3 + 0x2E2E] /* "I can't find a color graphics card.\r\n" */);
			exit(2);
		break;
	}
	bp_04 = C_184F();/*piracy check function ?*/
	Gra_init(&pShapes, &pCharset, exit);
	if(D_943A == 2)
		sizzleCharset();
	C_18A2();
	if(D_943A == 1)
		speed_info ++;
	Gra_clrscr();
	if(PARAM1 >= (char)'A' && PARAM1 <= (char)'P') {
		if(C_1814() == (PARAM1 & 0x0f)) {
			C_C454(/*D_30A8*/&AVATAR[0x12323 + 0x2E2E] /* "Re-Insert PLAYER Disk" */, /*D_309E*/&AVATAR[0x12319 + 0x2E2E] /* "WORLD.MAP" */);
		} else {
			C_181D((unsigned char)PARAM1 & 0x0f);
		}
	}
	C_C454(/*D_30C8*/&AVATAR[0x12343 + 0x2E2E] /* "Insert PLAYER disk" */, /*D_30BE*/&AVATAR[0x12319 + 0x2E2E] /* "WORLD.MAP" */);
	if(D_943A != 1) {
		if(Load(/*D_30DB*/&AVATAR[0x12356 + 0x2E2E] /* "SHAPES.EGA" */, 0x8000, pShapes) == -1)
			exit(3);
	}
	if(D_943A == 2)
		sizzleShapes();
	if(Load(/*D_30E6*/&AVATAR[0x11478 + 0x200F] /* "PARTY.SAV" */, sizeof(struct tParty), &Party) == -1)
		exit(3);
#if 0/*def WIN32*/
	/*for debug*/
	Party._x = 0x5b; Party._y = 0x44;
#endif

	//dumpParty(&Party);

	if(Party.f_1d8 == 0) {
		Gra_clrscr();
		txt_Y = 8;
		txt_X = 12;
		u4_puts(/*D_30F0*/&AVATAR[0x1236B + 0x2E2E] /* "No party formed!" */);
		sound(8,0);
		set_input_mode(INPUT_MODE_DELAY_NO_CONTINUE);
		u_delay(3, 0);
		u_kbflush();
		if(bp_02 == C_1814()) {
			C_C454(/*D_310B*/&AVATAR[0x12386 + 0x2E2E] /* "Re-Insert PROGRAM Disk" */, /*D_3010*/&AVATAR[0x1237C + 0x2E2E] /* "TITLE.EXE" */);
		} else {
			C_181D(bp_02);
			C_C454(/*D_312C*/&AVATAR[0x123A7 + 0x2E2E] /* "Insert PROGRAM disk" */, /*D_3122*/&AVATAR[0x1237C + 0x2E2E] /* "TITLE.EXE" */);
		}
		low_clean();
		exit(4);
	}

	if(Load(/*D_3140*/&AVATAR[0x11482 + 0x200F] /* "MONSTERS.SAV" */, sizeof(struct tNPC), &(D_8742._npc)) == -1)
		exit(3);
#if 0 // we will open and close this as needed instead of leaving it open
	if((File_MAP = dopen(/*D_314D*/&AVATAR[0x12319 + 0x2E2E] /* "WORLD.MAP" */, 0)) == -1) {
		//Console(/*D_3157*/&AVATAR[0x123D2 + 0x2E2E] /* "Can't open map file\r\n" */);
		exit(3);
	}
#endif

	//File_TLK = 0;
	D_9445 = D_0814[Party._trammel];
	D_9448 = D_081C[Party._felucca];
	C_213B();/*draw game screen frame*/
	Gra_CR();
	spell_sta = 7;
	spell_cnt = 0;
	D_9440 = (Party.f_1dc == 0);/*flying status*/
	D_95C6 = 0;
	CurMode = MOD_OUTDOORS;
	WindDir = DIR_N;
	SoundFlag = 1;
	dspl_Stats();
	C_26B6();
}
