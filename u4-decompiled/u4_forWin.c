#include "U4.H"

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
//#include <io.h>
#include <fcntl.h>
#include <stdlib.h>

#ifndef _WINDOWS
#include <android/log.h>
#endif

#ifdef ENABLE_WINDOWS
#include <windows.h>
#endif

#include "common.h"

extern const char* getDataPath();


int camera_shake_accumulator = 0;
// simple arrary to keep track of any highlight characters in the list
char char_highlight[8] = { 0,0,0,0,0,0,0,0 };

int screen_xor_state = 0;

#ifdef ENABLE_WINDOWS
unsigned speed_info;
unsigned equip_flags;
void far *patch_tandy;
struct t_500 D_8742;
char cdecl SoundFlag;

struct tPSP far *far_psp;

unsigned cursor_rate;

//#define CONSOLE printf
#define CONSOLE()

/*load gr driver*/
C_20C1(char *fname) {
	CONSOLE("C_20C1\"load gr driver\"(\"%s\")\n", fname);

	FAKE_RET;
}
/*draw game screen frame*/
C_213B() {
	int fd;
	static char path[256];

	CONSOLE("C_213B \"draw game screen frame\"\n");

	strcpy(path, getDataPath());
	strcat(path, &AVATAR[0xFB61 + 0x0005] /* "START.PIC" */); // TODO: this is lower case in the AVATAR.EXE but upper case in the file system, will cause trouble when moving to android
//	fd = _open(U4_ROOT"START.PIC", _O_RDONLY | _O_BINARY);
	fd = _open(path, _O_RDONLY | _O_BINARY);
	if (fd) {
#pragma pack(1)
		struct tHeader {//size 0x11
			/*+00*/unsigned short magic;//0x1234
			/*+02*/unsigned short xsize, ysize;//320,200
			/*+06*/char __06[4];
			/*+0a*/unsigned char bitsinf;//2
			/*+0b*/unsigned char emark;//0xff
			/*+0c*/char evideo;//'A'
			/*+0d*/unsigned char edesc;//[1 == edata is pallet]
			/*+0e*/unsigned char __0e;
			/*+0f*/unsigned short esize;
		};

		struct tBlock {
			/*+00*/unsigned short PBSIZE;
			/*+02*/unsigned short size;
			/*+05*/unsigned char MBYTE;
		};
#pragma pack()
		struct tHeader header = { 0,0,{0},0,0,0,0,0,0};
		char edata[2];
		static char work[0x800];
		static char dest[0x3e80];
		unsigned char *pdest;
		unsigned char byt;
		int i,j;

		//should calls Gra_16?
		//-- load/unpack --
		pdest = dest;
		_read(fd, &header, sizeof(struct tHeader));
		if(
			header.magic == 0x1234 &&
			header.xsize == 320 && header.ysize == 200 &&
			header.bitsinf == 2 &&
			header.emark == 0xff &&
			header.evideo == 'A' &&
			header.edesc == 1
		) {
			unsigned short numblks;

			_read(fd, edata, header.esize);
			_read(fd, &numblks, sizeof(unsigned short));
			for(i = 0; i < numblks; i ++) {
				struct tBlock  BLOCK;
				int len;
				unsigned char *p, *block_end;

				_read(fd, &BLOCK, sizeof(BLOCK));
				_read(fd, work, BLOCK.PBSIZE - sizeof(struct tBlock));
				p = work;
				block_end = work + BLOCK.PBSIZE - sizeof(struct tBlock);
				do {
					len = 1;
					byt = *(p++);
					if(byt == BLOCK.MBYTE) {
						len = *(p++);
						if(len == 0) {
							len = *(short *)p; p += sizeof(short);
						}
						byt = (*p++);
					}
					while(len --)
						*(pdest ++) = byt;
				}while(p < block_end);
			}
		}
		_close(fd);
		//-- draw --
		pdest = dest;
		CMN_beginScene();
		for(i = 0; i < header.ysize; i ++) {
			for(j = 0; j < header.xsize; j += 4) {
				byt = *(pdest ++);
				CMN_putpixel(j + 0, header.ysize - 1 - i, (byt & 0xc0) >> 6);
				CMN_putpixel(j + 1, header.ysize - 1 - i, (byt & 0x30) >> 4);
				CMN_putpixel(j + 2, header.ysize - 1 - i, (byt & 0x0c) >> 2);
				CMN_putpixel(j + 3, header.ysize - 1 - i, (byt & 0x03) >> 0);
			}
		}
		CMN_endScene();
	}

	FAKE_RET;
}
/*highlight cursor*/
__cdecl Gra_00(int char_Y, int char_X) {
	int i,j;
	int top,left;

	CONSOLE("Gra_00 \"highlight cursor\"(%d,%d)\n", char_Y, char_X);
	top = 8 + char_Y * 16;
	left = 8 + char_X * 16;
	CMN_beginScene();
	for(j = 0; j < 2; j ++) {
		for(i = 0; i < 16; i ++) {
			CMN_xorpixel(left + i, top + j, 3);
			CMN_xorpixel(left + i, top + (16 - 2) + j, 3);
		}
	}
	for(j = 2; j < (16 - 2); j ++) {
		for(i = 0; i < 2; i ++) {
			CMN_xorpixel(left + i, top + j, 3);
			CMN_xorpixel(left + (16 - 2) + i, top + j, 3);
		}
	}
	CMN_endScene();

	FAKE_RET;
}
/*render sprite(2)*/
__cdecl Gra_01(int tile, int trow, int srow, int scol, int exp) {
	unsigned char *p;
	int i;
	int y;
	
	p = CMN_getShapeData(tile) + (trow & 1) * 4 * 8 + (trow >> 1) * 4;
	CMN_beginScene();
	y = scol * 4;
	switch(exp) {
		case 1:
			for(i = 0; i < 16; i += 4) {
				unsigned char byt = *(p ++);
				CMN_putpixel(y + 0, srow, (byt & 0xc0) >> 6);
				CMN_putpixel(y + 1, srow, (byt & 0x30) >> 4);
				CMN_putpixel(y + 2, srow, (byt & 0x0c) >> 2);
				CMN_putpixel(y + 3, srow, (byt & 0x03) >> 0);
				y += 4;
			}
		break;
		case 2:
			for(i = 0; i < 16; i += 4) {
				unsigned char byt = *(p ++);
				CMN_putpixel(y + 0, srow, (byt & 0xc0) >> 6); CMN_putpixel(y + 1, srow, (byt & 0xc0) >> 6);
				CMN_putpixel(y + 2, srow, (byt & 0x30) >> 4); CMN_putpixel(y + 3, srow, (byt & 0x30) >> 4);
				CMN_putpixel(y + 4, srow, (byt & 0x0c) >> 2); CMN_putpixel(y + 5, srow, (byt & 0x0c) >> 2);
				CMN_putpixel(y + 6, srow, (byt & 0x03) >> 0); CMN_putpixel(y + 7, srow, (byt & 0x03) >> 0);
				y += 4 * 2;
			}
		break;
		default:
			;
	}
	CMN_endScene();

	FAKE_RET;
}
/*render sprite(2)*/
__cdecl Gra_02(int width, int height, int tile, int srow, int scol) {
	int i,j;

	CMN_beginScene();
	for(i = 0; i < height; i ++) {
		for(j = 0; j < width; j ++) {
			CMN_puttile(tile, srow + j * 16, scol * 4 + i * 16);
		}
	}
	CMN_endScene();

	FAKE_RET;
}
/*shake related(1)*/
__cdecl Gra_03() {
	//TODO
	FAKE_RET;
}
/*shake related(2)*/
__cdecl Gra_04() {
	//TODO
	FAKE_RET;
}
__cdecl Gra_05(int height, int width, unsigned char *pC, int d) {
	int i, j;

//	CONSOLE("Gra_05(%d, %d, %08x, %d)//TODO\n", height, width, pC, d);
	CMN_beginScene();
	for(i = 0; i < height; i ++) {
		for(j = 0; j < width; j ++) {
			CMN_puttile(pC[i * width + j], 8 + j * 16, 8 + i * 16);
		}
	}
	CMN_endScene();

	FAKE_RET;
}
//XOR SCREEN
Gra_09() {
	int i,j;

	//TODO:too fast
	CONSOLE("Gra_09\"XOR SCREEN\"\n");
	if(CMN_beginScene()) {
		extern HDC currentHDC;

		BitBlt(
			currentHDC, 0, 0,//dest
			320 * RATIO, 200 * RATIO,//w,h
			currentHDC, 0, 0,//src
			DSTINVERT
		);
		CMN_endScene();
	}

	FAKE_RET;
}
/*clear map zone*/
Gra_10() {
	CONSOLE("Gra_10\"clear map zone\"\n");
	CMN_clear(1, 1, 11 * 2, 11 * 2, 0);

	FAKE_RET;
}
/*highlight char's status*/
Gra_11(int a) {
	int i,j,k;

	CONSOLE("Gra_11\"highlight char's status\"(%d)\n", a);
	CMN_beginScene();
	for(i = 0; i < 15; i ++) {
		for(j = 0; j < 8; j ++) {
			for(k = 0; k < 8; k ++) {
				CMN_xorpixel((24 + i) * 8 + k, (1 + a) * 8 + j, 3);
			}
		}
	}
	CMN_endScene();

	FAKE_RET;
}
/*clear status zone*/
Gra_13() {
	CONSOLE("Gra_13\"clear status zone\"\n");
	CMN_clear(24, 1, 15, 8, 0);

	FAKE_RET;
}

char current_vision[256] = "";

__cdecl Gra_16(char* filename, int unknown1, int* unknown2)
{
	strncpy(current_vision, filename, 255);
	current_vision[255] = 0;
	printf(current_vision);
	// loads a pic
	//TODO
	FAKE_RET;
}

__cdecl Gra_17() {
	//TODO
	// infinity symbol
	FAKE_RET;
}
/*draw a "3D" wall*/
__cdecl Gra_18(int x0, int x1, int y0, int y1, int y0p, int y1p, int col0, int col1) {
	POINT points[4];

	points[0].x = x0 * RATIO; points[0].y = y0 * RATIO;
	points[1].x = x1 * RATIO; points[1].y = y0p * RATIO;
	points[2].x = x1 * RATIO; points[2].y = y1p * RATIO;
	points[3].x = x0 * RATIO; points[3].y = y1 * RATIO;
	if(CMN_beginScene()) {
		extern HDC currentHDC;

		SelectObject(currentHDC, U4_BRUSH[col0]);
		SelectObject(currentHDC, U4_PEN[col0]);
		Polygon(currentHDC, points, 4);
		CMN_endScene();
	}

	FAKE_RET;
}
/*clear food/gold zone*/
__cdecl Gra_19() {
	CONSOLE("Gra_19\"clear food/gold zone\"\n");
	CMN_clear(24, 10, 15, 1, 0);

	FAKE_RET;
}

__cdecl Gra_init(void **ppShp, void **ppChar, void (*pExit)(int)) {
	CONSOLE("Gra_init(%08x,%08x,%08x)//TODO\n", ppShp, ppChar, pExit);
	CMN_createWindow(320 * RATIO, 200 * RATIO, *ppShp, *ppChar);

	FAKE_RET;
}

__cdecl Gra_clrscr() {
	CONSOLE("Gra_clrscr\n");
	CMN_clear(0, 0, 40, 25, 0);

	FAKE_RET;
}

__cdecl Gra_dot(int y, int x, int col) {
	CMN_beginScene();
	CMN_putpixel(x, y, col);
	CMN_endScene();

	FAKE_RET;
}
__cdecl Gra_dot_OR() {
	//TODO

	FAKE_RET;
}
__cdecl Gra_dot_XOR(int y, int x, int c) {
	//COLORREF src;
	
	CMN_beginScene();
	CMN_xorpixel(x, y, c);
	CMN_endScene();

	FAKE_RET;
}
__cdecl Gra_line(int x0, int y0, int x1, int y1, int color) {
	if(CMN_beginScene()) {
		extern HDC currentHDC;

		SelectObject(currentHDC, U4_PEN[color]);
		MoveToEx(currentHDC, x0 * RATIO, y0 * RATIO, 0);
		LineTo(currentHDC, x1 * RATIO, y1 * RATIO);
		CMN_endScene();
	}

	FAKE_RET;
}

// Need to split this function as there are some places we want to add text to the buffer when
// we do a CR and some places we do not so the resulting text passed up to Unity is clean and without
// excessive line breaks.
Gra_CR()
{
	if ((txt_Y == 23) && (txt_X >= 24))
	{
		//printf("\n");
		add_char_to_text_buffer('\n');
	}
	Gra_CR2();
}

Gra_CR2() {
	RECT rect;

	//CONSOLE("Gra_CR\n");
	txt_Y = 23;
	txt_X = 24;

#define CONSOLE_X (24 * 8 * RATIO)
#define CONSOLE_Y (12 * 8 * RATIO)
	if(CMN_beginScene()) {
		extern HDC currentHDC;

		BitBlt(
			currentHDC, CONSOLE_X, CONSOLE_Y,//dest
			16 * 8 * RATIO, 11 * 8 * RATIO,//w,h
			currentHDC, CONSOLE_X, CONSOLE_Y + 1 * 8 * RATIO,//src
			SRCCOPY
		);
		CMN_endScene();
	}
#undef CONSOLE_X
#undef CONSOLE_Y

	CMN_clear(24, txt_Y, 16, 1, 0);

	FAKE_RET;
}
__cdecl Gra_animSpit() {
	//TODO
	CONSOLE("Gra_animSpit\n");

	FAKE_RET;
}

Gra_animFlow(unsigned tile) {
	int i;
	unsigned char *p;
	unsigned short AX,DX;
	CONSOLE("Gra_animFlow(%x)\n", tile);

	p = CMN_getShapeData(tile);
	AX = *(unsigned short *)(p + 0x3c);
	DX = *(unsigned short *)(p + 0x3e);
	for(i = 0; i < 8; i ++) {
		unsigned short temp;
		DO_SWAP(AX, *(unsigned short *)(p + 0x00), temp);
		DO_SWAP(AX, *(unsigned short *)(p + 0x20), temp);
		DO_SWAP(DX, *(unsigned short *)(p + 0x02), temp);
		DO_SWAP(DX, *(unsigned short *)(p + 0x22), temp);
		p += 4;
	}

	FAKE_RET;
}
static char random() { return (char)(rand() & 0xff); }
Gra_animFlag() {
	unsigned char *p;
	unsigned short temp;
	CONSOLE("Gra_animFlag\n");

	p = CMN_getShapeData(0);
	if(random() >= 0) {
		DO_SWAP(*(unsigned short *)(p + 0x2A4), *(unsigned short *)(p + 0x288), temp);
	}
	if(random() >= 0) {
		DO_SWAP(*(unsigned short *)(p + 0x2E2), *(unsigned short *)(p + 0x2c6), temp);
	}
	if(random() >= 0) {
		DO_SWAP(*(unsigned short *)(p + 0x404), *(unsigned short *)(p + 0x424), temp);
		DO_SWAP(*(unsigned short *)(p + 0x406), *(unsigned short *)(p + 0x426), temp);
	}
	if(random() >= 0) {
		DO_SWAP(*(unsigned short *)(p + 0x484), *(unsigned short *)(p + 0x4A4), temp);
		DO_SWAP(*(unsigned short *)(p + 0x486), *(unsigned short *)(p + 0x4A6), temp);
	}
	if(random() >= 0) {
		DO_SWAP(*(unsigned short *)(p + 0x3A0), *(unsigned short *)(p + 0x384), temp);
	}

	FAKE_RET;
}
Gra_putchar(char c) {
#if 0
	CONSOLE("Gra_putchar(%d,%d,", txt_X, txt_Y);
	switch(c) {
		case 7:
			CONSOLE("0x07");
		break;
		default:
			CONSOLE("'%c'", c);
	}
	CONSOLE(")\n");
#endif
	CMN_beginScene();
	CMN_putchar(c, txt_X * 8, txt_Y * 8);
	CMN_endScene();

	FAKE_RET;
}

sizzleShapes() {
	//TODO
	FAKE_RET;
}
sizzleCharset() {
	//TODO
	FAKE_RET;
}

//----------------------------------------

int __cdecl u_kbhit() {
	CONSOLE("u_kbhit\n");
#ifdef ENABLE_WINDOWS
	CMN_pumpmessages();
#else
	Sleep(1 * SLEEPFACTOR);
#endif
	Party.f_000 ++;

	t_callback();
	return CMN_kbhit != 0;
}
int __cdecl u_kbread() {
	int ret;

	CONSOLE("u_kbread\n");
//	u_delay(0x8081, 0x8081);
	while (CMN_kbhit == 0) {
#ifdef ENABLE_WINDOWS
		CMN_pumpmessages();
#else
		Sleep(1 * SLEEPFACTOR);
#endif
	}
	ret = CMN_kbhit;
	CMN_kbhit = 0;

	return ret;
}
void __cdecl u_kbflush() {
	CONSOLE("u_kbflush\n");
	CMN_kbhit = 0;
}
void __cdecl u_delay(int a, int b) {
	CONSOLE("u_delay(%d, %d)\n", a, b);
	if(a == 0)
		a = 1000;
	a *= 50;
	while(a) {
		a --;
		t_callback();
#ifdef ENABLE_WINDOWS
		CMN_pumpmessages();
#else
		Sleep(1 * SLEEPFACTOR);
#endif
		Sleep(2 * SLEEPFACTOR);
		if(b && u_kbhit())
			break;
		//cursor
		Gra_putchar(0x1c + (~a & 3));
	}
}

__cdecl sound(unsigned char s, unsigned char length) {
	//TODO
	CONSOLE("sound(%d, %d)\n", s, length);
	play_sound_effect(s, length);
	FAKE_RET;
}

//----------------------------------------


int __cdecl dopen(char *fname, int mode) {
	int ret;
	static char path[256];

	CONSOLE("dopen(\"%s\", %d)\n", fname, mode);
	//strcpy(path, U4_ROOT);
	strcpy(path, getDataPath());
	strcat(path, fname);
	switch(mode) {
		case 0: break;//Read
		case 1: break;//Write
		default:
			CONSOLE("TODO\n");
	}
	//ret = _open(path, _O_RDONLY | _O_BINARY);
	ret = _open(path, _O_RDWR | _O_BINARY); // Not sure why writing is disallowed. Fixed to allow writing.
	if(ret <= 0) {
		CONSOLE("<err>\n");
	}

	return ret;
}

int __cdecl dlseek(int f, unsigned long ofs) {
	int ret;

	CONSOLE("dlseek(%d,0x%08x)\n", f, ofs);
	ret = _lseek(f, ofs, SEEK_SET);

	return ret;
}

int __cdecl dread(int fd, void *buffer, int count) {
	int ret;

	CONSOLE("dread(%d,0x%08x,0x%08x)\n", fd, buffer, count);
	ret = read(fd, buffer, count);
	if(ret != count) {
		CONSOLE("<err>\n");
	}

	return ret;
}
int __cdecl dwrite(int f, void *b, int sz) {
	int ret;

//	CONSOLE("dwrite(%d,0x%08x,0x%08x)\n", f, b, sz);
	//-- --
	/* Not sure why the file reference being larger in 2 should result in the write not happening. Comment out for now so saves will work.
	if(f > 2) {
		CONSOLE("dwrite(%d,0x%08x,0x%08x)\n", f, b, sz);
		return sz;
	}
	*/
	//-- --
	ret = _write(f, b, sz);

	return ret;
}
int __cdecl dclose(int f) {
	int ret;

	CONSOLE("dclose(%d)\n", f);
	ret = _close(f);

	return ret;
}

void * __cdecl dalloc(int n) {
	void *ret;

	CONSOLE("dalloc(%08x)\n", n);
	ret = malloc(n);

	return ret;
}
__cdecl dfree(void *p) {
	CONSOLE("dfree(%08x)\n", p);
	free(p);

	FAKE_RET;
}

//----------------------------------------

unsigned char u_rand_a() {
	CONSOLE("u_rand_a\n");

	return rand();
}
unsigned char u_rand_b() {
	CONSOLE("u_rand_b\n");

	return rand();
}

//----------------------------------------

struct tPSP the_psp;

low_init() {
	CONSOLE("low_init\n");
	//TODO
	far_psp = &the_psp;
	the_psp._80[0] =
	the_psp._80[2] =
	the_psp._80[4] = 0;

	FAKE_RET;
}
low_clean() {
	//TODO

	FAKE_RET;
}
int low_gra() {
	//TODO
	CONSOLE("low_gra\n");

	return 1;
}

//----------------------------------------

//current floppy?
unsigned char C_1814() {
	//TODO
	CONSOLE("C_1814\"current floppy?\"//TODO\n");

	return 0;
}
C_181D(unsigned a) {
	//TODO
	FAKE_RET;
}
/*verify some file?*/
C_182F(char *fname) {
	//TODO
	CONSOLE("C_182F(\"%s\")//TODO\n", fname);

	return 1;
}
/*anti-piracy function*/
C_184F() {
	//TODO
	CONSOLE("C_184F\"anti-piracy function\"//TODO\n");

	return 0;
}
C_18A2() {
	//TODO
	CONSOLE("C_18A2//TODO\n");

	FAKE_RET;
}

#else // #ifdef ENABLE_WINDOWS

struct tPSP the_psp;

unsigned cursor_rate;
unsigned equip_flags; // TODO see if I can remove this windows thing
struct tPSP * far_psp; // TODO see if I can remove this windows thing
void * patch_tandy; // TODO see if I can remove this windows thing

struct t_500 D_8742;
char  SoundFlag;
unsigned speed_info;

unsigned char C_1814() // current floppy
{
	return 0;
}

C_181D(unsigned a) 
{
	FAKE_RET;
}

C_182F(char* fname)  // verify some file
{
	return 1;
}

C_184F() // anti-piracy function
{
	return 0;
}

C_18A2() 
{
	FAKE_RET;
}

C_20C1(char* fname) //load graphics driver
{
	FAKE_RET;
}


C_213B() // draw game screen frame
{
	FAKE_RET;
}

void* dalloc(int n)
{
	void* ret;

	ret = malloc(n);

	return ret;
}

int  dclose_deprecated(FILE* f)
{
	int ret;

	//ret = _close(f);
	ret = fclose(f);

	return ret;
}

int dfree(void* p) 
{
	free(p);
	FAKE_RET;
}

int dlseek_deprecated(FILE* f, unsigned long ofs)
{
	int ret;

	//ret = _lseek(f, ofs, SEEK_SET);
	ret = fseek(f, ofs, SEEK_SET);

	return ret;
}

FILE* dopen_deprecated(char* filename, int mode)
{
	FILE* filepointer;
	static char path[256]; // TODO is this big enough

	strcpy(path, getDataPath());
	strcat(path, filename);

#ifndef _WINDOWS
	__android_log_print(ANDROID_LOG_INFO, "ANKH path", path);
#endif

	//ret = _open(path, _O_RDWR | _O_BINARY);
	filepointer = fopen(path, "rb");

#ifndef _WINDOWS
	__android_log_print(ANDROID_LOG_INFO, "ANKH File", "File * %d", filepointer);
#endif

	return filepointer;
}

int dread_deprecated(FILE* filepointer, void* buffer, int buffersize)
{
	int byteswritten;

#ifndef _WINDOWS
	__android_log_print(ANDROID_LOG_INFO, "ANKH count", "count A %d, FD %d, buffer %d", buffersize, filepointer, buffer);
#endif

	byteswritten = fread(buffer, 1, buffersize, filepointer);
	if (byteswritten != buffersize) 
	{
		// error
	}
#ifndef _WINDOWS
	__android_log_print(ANDROID_LOG_INFO, "ANKH count", "count B %d ret %d", buffersize, byteswritten);
#endif

	return byteswritten;
}

int dwrite_deprecated(FILE* f, void* b, int sz)
{
	int ret;

	//ret = _write(f, b, sz);
	ret = fwrite(b, 1, sz, f);

	return ret;
}

int Gra_00(int char_Y, int char_X) // highlight cursor
{
	FAKE_RET;
}

int Gra_01(int tile, int trow, int srow, int scol, int exp) // render sprite(2)
{
	FAKE_RET;
}

int Gra_02(int width, int height, int tile, int srow, int scol) // render sprite(2)
{
	FAKE_RET;
}

// each of these two shake functions move the screen up and down
// we are going to use an accumulator to measure the amount of shake
// so we can pass this to Unity to perform a camera shake of appropiate
// magnitude.

int Gra_03() // shake related(1)
{
	camera_shake_accumulator++;
	FAKE_RET;
}

int Gra_04() // shake related(2)
{
	camera_shake_accumulator++;
	FAKE_RET;
}

int Gra_05(int height, int width, unsigned char* pC, int d)
{
	FAKE_RET;
}

int Gra_09() // XOR SCREEN
{
	screen_xor_state = 1 - screen_xor_state;
	FAKE_RET;
}

extern const char* get_current_vision();

int Gra_10() // clear map zone
{
	// clear the vision if any
	strncpy(get_current_vision(), "", 255);

	FAKE_RET;
}

int Gra_11(int a) // highlight char's status
{
	// flip the highlight as this function replaces an pixel xor function
	char_highlight[a] = 1 - char_highlight[a];
	FAKE_RET;
}

int Gra_13(void) // clear status zone
{
	int i;

	for (i = 0; i < 8; i++)
	{
		char_highlight[i] = 0;
	}
	FAKE_RET;
}

extern const char* get_current_vision();

int Gra_16(char * filename, int unknown1, int* unknown2)
{
	strncpy(get_current_vision(), filename, 255);

	// TODO instrument this
	// loads a pic
	FAKE_RET;
}

int Gra_17()
{
	Gra_10();
	u_delay(1, 1);
	strncpy(get_current_vision(), "rune_5.pic", 255);

	// TODO instrument this
	//infinity symbol
	FAKE_RET;
}

int Gra_18(int x0, int x1, int y0, int y1, int y0p, int y1p, int col0, int col1) // draw a "3D" wall
{
	FAKE_RET;
}

int Gra_19() // clear food/gold zone
{
	FAKE_RET;
}

int Gra_init(void** ppShp, void** ppChar, void (*pExit)(int)) 
{
	FAKE_RET;
}

int Gra_clrscr() // Gra_clrscr
{
	FAKE_RET;
}

int Gra_dot(int y, int x, int col) 
{
	FAKE_RET;
}
int Gra_dot_OR() 
{
	FAKE_RET;
}
int Gra_dot_XOR(int y, int x, int c) 
{
	FAKE_RET;
}
int Gra_line(int x0, int y0, int x1, int y1, int color) 
{
	FAKE_RET;
}

int Gra_animSpit() 
{
	FAKE_RET;
}

int Gra_animFlow(unsigned tile) 
{
	FAKE_RET;
}

static char Random1() 
{ 
	return (char)(rand() & 0xff); 
}

int Gra_animFlag() 
{
	FAKE_RET;
}

int Gra_putchar(char c) 
{
	FAKE_RET;
}

int sizzleShapes() 
{
	FAKE_RET;
}

int sizzleCharset() 
{
	FAKE_RET;
}

extern void add_char_to_text_buffer(char ch);

int Gra_CR()
{
	if ((txt_Y == 23) && (txt_X >= 24))
	{
		add_char_to_text_buffer('\n');
	}
	Gra_CR2();

	FAKE_RET;
}

int Gra_CR2() 
{
	txt_Y = 23;
	txt_X = 24;

	FAKE_RET;
}

int low_init() 
{
	far_psp = &the_psp;
	the_psp._80[0] =
	the_psp._80[2] =
	the_psp._80[4] = 0;

	FAKE_RET;
}

int low_clean() 
{
	FAKE_RET;
}

int low_gra() 
{
	return 1;
}

// sound effect playing is actually synchronous in the game engine, 
// the sound effect must complete playing before the game continues.
// this function will pend until it gets a response from Unity that 
// the sound has completed playing
int sound(unsigned char s, unsigned char length)
{
	play_sound_effect(s, length);
	FAKE_RET;
}

void u_delay(int a, int b)
{
	if (a == 0)
		a = 1000;
	a *= 20; //50
	a /= 3;
	while (a) 
	{
		a--;
		t_callback();
		//CMN_pumpmessages();
		Sleep(150 * SLEEPFACTOR); // 2
		if (b && u_kbhit())
			break;
		//cursor
		Gra_putchar(0x1c + (~a & 3));
	}
}

void u_kbflush() 
{
	CMN_kbhit = 0;
}

int u_kbhit() 
{
	char ch;

	Party.f_000++;

	return CMN_kbhit != 0;
}

int u_kbread() 
{
	int ret;

	while (u_kbhit() == 0)
		Sleep(20 * SLEEPFACTOR);
	ret = CMN_kbhit;
	CMN_kbhit = 0;

	return ret;
}

unsigned char u_rand_a() 
{
	return rand();
}

unsigned char u_rand_b() 
{
	return rand();
}

#ifndef _WINDOWS
int _strnicmp(const char* s1, const char* s2, size_t n) {
	if (n == 0) {
		return 0;
	}

	do {
		if (tolower((unsigned char)*s1) != tolower((unsigned char)*s2++)) {
			return tolower((unsigned char)*s1) - tolower((unsigned char)*--s2);
		}

		if (*s1++ == 0) {
			break;
		}
	} while (--n != 0);

	return 0;
}

int Sleep(int time)
{
	usleep(time * 1000);
	return 0;
}
#endif
#endif // #ifdef ENABLE_WINDOWS