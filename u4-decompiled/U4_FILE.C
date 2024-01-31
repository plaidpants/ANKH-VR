/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "U4_FILE.H"

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#ifndef _WINDOWS
#include <android/log.h>
#endif

#include "U4_DOS.H"

extern char AVATAR[];

/*C_0812*/Console(bp04)
char *bp04;
{
	int bp_02;

#ifndef _WINDOWS
	__android_log_print(ANDROID_LOG_INFO, "ANKH", bp04);
#endif

#if 0
	if((bp_02 = dwrite(1, bp04, strlen(bp04))) == -1 || strlen(bp04) != bp_02) {
		exit(3);
	}
#endif
}

int /*C_0855*/DisableLoad(char* filename, int filesize, char * filebuffer)
{
#if 0
	FILE* filepointer;
	int byteswritten;

	if((filepointer = fopen(filename, "rb")) == -1) {
		Console(&AVATAR[0xF71D + 0x5] /* "\r\nOpening " */);
		Console(filename);
#ifndef WIN32
		__android_log_print(ANDROID_LOG_INFO, "ANKH Console", "return %d", filepointer);
#endif
		return -1;
	}
	if((byteswritten = fread(filepointer, 1, filebuffer, filesize)) == -1 || filesize != byteswritten) {
		Console(&AVATAR[0xF728 + 0x5] /* "\r\nReading " */);
		if(byteswritten == -1) {
			Console(filename);
		} else {
			Console(filename);
			Console(&AVATAR[0xF733 + 0x5] /* ": File wrong size\r\n" */);
		}
		return -1;
	}
	if(fclose(filepointer) == -1) {
		Console(&AVATAR[0xF747 + 0x5] /* "\r\nClosing " */);
		Console(filename);
		return -1;
	}
#endif
	return 0;
}

int /*C_08D1*/DisableSave(char* bp0a, int bp08, char * bp04)
{
#if 0
	FILE* bp_02;
	int bp_04;

	if((bp_02 = dopen(bp0a, 1)) == -1) {
		Console(&AVATAR[0xF71D + 0x5] /* "\r\nOpening " */);
		Console(bp0a);
		return -1;
	}
	if((bp_04 = dwrite(bp_02, bp04, bp08)) == -1 || bp08 != bp_04) {
		Console(&AVATAR[0xF75D + 0x5] /* "\r\nWriting " */);
		if(bp_04 == -1) {
			Console(bp0a);
		} else {
			Console(bp0a);
			Console(&AVATAR[0xF733 + 0x5] /* ": File wrong size\r\n" */);
		}
		exit(1);
	}
	if(dclose(bp_02) == -1) {
		Console(&AVATAR[0xF747 + 0x5] /* "\r\nClosing " */);
		Console(bp0a);
		return -1;
	}
#endif
	return 0;
}
