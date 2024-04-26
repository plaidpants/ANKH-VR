/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#include "TITLE.H"

#include <malloc.h>

#ifndef WIN32
#include <android/log.h>
#endif

extern void* _fmalloc(int sz);

#include <stdlib.h>

int D_00C0 = 0;
int D_00C2 = 1;
int D_00C4 = -1;
int D_00C6 = 0;

/*base tiles for view*/
unsigned char * D_00C8 = (unsigned char*)&TITLE[0x40C8]; // {0x40, 0x80, 0x10, 0x38, 0x38, 0xC8, 0xC8, 0x24, 0x20, 0x88, 0xF0, 0xF8, 0x4D, 0x4F, 0x4E};

/*====---- _BSS ----====*/
static unsigned char D_3A24[19 * 5];
static unsigned D_3A84;

/*animate the 2 "monsters"*/
/*callback for u_delay*/
/*C_019A*/void t_callback()
{
	unsigned bp_02;

	if(D_3A84)
		D_00C0 = 0;
	if(D_00C0 --)
		return;
	D_00C0 = ((D_7078 == 1)?120:200) * speed_info;
	if(D_6E80) {
		cursor_rate = 200;

		bp_02 = D_3380[D_0036++]; D_0036 &= 0x7f;
		Gra_3(6, 32, D_344A[bp_02], D_3438[bp_02], pAnim, 0, -1, 0);
		bp_02 = D_33F8[D_0038++]; D_0038 &= 0x3f;
		Gra_3(6, 32, D_345C[bp_02], D_3438[bp_02], pAnim, 0, -1, 34);
	} else {
		cursor_rate = 233;
	}
}

/*C_027A*/void u4_putc(bp04)
unsigned char bp04;
{
	if(bp04 == 8) {
		txt_X --;
		Gra_putchar(' ');
	} else {
		Gra_putchar(bp04);
		txt_X ++;
	}
}

extern int u_kbhit();

void C_02A3(bp04)
long bp04;
{
	if(u_kbhit())
		return;
	//bp04 *= (long)speed_info;
	//while(-- bp04)
		Sleep(1 * SLEEPFACTOR);
}

/*handwriting?*/
void C_02D1(bp04)
unsigned bp04;
{
	register int si;

	for(si = 0; D_346E[si]; ) {
		Gra_dot(191 - D_346E[si+1], D_346E[si] + 20, 3);
		Gra_dot(191 - D_346E[si+1], D_346E[si] + 21, 3);
		si += 2;
		if(bp04)
			C_02A3((long)440);
	}
	if (bp04)
	{
		set_input_mode(INPUT_MODE_GENERAL_NO_CONTINUE);
		u_delay(1, 1);
	}
}

/*tiles animation ?*/
void C_034D()
{
	register int si;

	for(si = 0; si < 32; si ++) {
		register int di;

		di = D_6976._npc._060[si];
		if(
			(di >= 0x84 && di <= 0x8e) ||
			(di >= 0x20 && di <= 0x2e) ||
			(di >= 0x50 && di <= 0x5e)
		) {
			if(u_rand_b() < 0xc0)
				D_6976._npc._000[si] = ((D_6976._npc._000[si] & 1) | di) ^ 1;
		} else if(di >= 0x90) {
			if(u_rand_b() < 0xc0)
				D_6976._npc._000[si] = ((D_6976._npc._000[si] + 1) & 3) | di;
		} else if(di != 0x80) {
			D_6976._npc._000[si] = di;
		}
	}
	if(--D_00C2)
		return;
	D_00C2 = (speed_info == 1)?1:(speed_info - 1);
	Gra_animFlow(0x00);
	Gra_animFlow(0x01);
	Gra_animFlow(0x02);
	Gra_animFlow(0x4c);
	Gra_animFlow(0x44);
	Gra_animFlow(0x46);
	Gra_animFlow(0x47);
	Gra_animFlow(0x45);
	Gra_animFlag();
}

void C_041A()
{
	int bp_02;

	if(D_00C6) {
		D_00C6 --;
		return;
	}
	while(1) {
		switch(D_36E2[++D_00C4] & 0xf0) {
			case 0x00: case 0x10: case 0x20: case 0x30: case 0x40: case 0x50: case 0x60:
				D_6976._npc._040[D_36E2[D_00C4] & 0xf] = D_36E2[D_00C4] >> 4;
				D_6976._npc._020[D_36E2[D_00C4] & 0xf] = D_36E2[D_00C4 + 1] & 0x1f;
				D_6976._npc._000[D_36E2[D_00C4] & 0xf] = D_00C8[D_36E2[D_00C4] & 0xf] + (D_36E2[D_00C4 + 1] >> 5);
				D_6976._npc._060[D_36E2[D_00C4] & 0xf] = ((D_36E2[D_00C4] & 0xf) == 1)?
					D_00C8[D_36E2[D_00C4] & 0xf]:
					D_6976._npc._000[D_36E2[D_00C4] & 0xf]
				;
				D_00C4++;
			break;
			case 0x70:/**/
				D_6976._npc._000[D_36E2[D_00C4] & 0xf] =
				D_6976._npc._060[D_36E2[D_00C4] & 0xf] = 0;
			break;
			case 0x80:/*end frame*/
				for(bp_02 = 0; bp_02 < 19 * 5; bp_02 ++)
					D_3A24[bp_02] = D_3683[bp_02];
				for(bp_02 = 0; bp_02 < 32; bp_02 ++) {
					if(D_6976._npc._000[bp_02])
						D_3A24[19 * D_6976._npc._040[bp_02] + D_6976._npc._020[bp_02]] = D_6976._npc._000[bp_02];
				}
				D_00C6 = D_36E2[D_00C4] & 0xf;
			return;
			case 0x90: case 0xa0: case 0xb0: case 0xc0: case 0xd0: case 0xe0: case 0xf0:
				D_00C4 = -1;
			break;
		}
	}
}

extern int u_kbread();

/*the "view"*/
void C_05A4()
{
	register int si;

	D_3A84 = 1;
	D_31C0 = 0;
	set_input_mode(INPUT_MODE_GENERAL_CONTINUE);
	while(!u_kbhit()) {
		C_034D();
		C_041A();
		D_6E80 = 0;
		Gra_0(5, 19, D_3A24, 104, 0, 0, 2);
		//for(si = speed_info / 2; si && !u_kbhit(); si --)
		//	Gra_0(5, 19, D_3A24, 104, 0, 0, 2);
		C_034D();
		C_041A();
		Gra_0(5, 19, D_3A24, 104, 0, 0, 2);
		//for(si = speed_info / 2; si && !u_kbhit(); si --)
		//	Gra_0(5, 19, D_3A24, 104, 0, 0, 2);
		D_6E80 = 1;
	}
	D_3A84 = 0;
	set_input_mode(INPUT_MODE_GENERAL_CONTINUE);
	u_kbread();
}

/*logo + splash + animation*/
void C_068C()
{
	register unsigned loc_A, loc_B;
	int loc_C;
	set_input_mode(INPUT_MODE_GENERAL_NO_CONTINUE);
	u_delay(1, 1);
	/*"lord british"*/
	C_02D1(1);
	/*"and"*/
	Gra_3(4, 4, 19, 17, pTitle, 17, -1, 19);
	set_input_mode(INPUT_MODE_GENERAL_NO_CONTINUE);
	u_delay(1, 1);
	/*draw line*/
	for(loc_A = 86; loc_A < 238; loc_A++) {
		Gra_dot(31, loc_A, 2);
		C_02A3((long)320);
	}
	/*"origin systems inc."*/
	for(loc_A = 1; loc_A < 10; loc_A ++) {
		Gra_3(21, loc_A, 9, 21, pTitle, 30 - loc_A, -1, 9);
		C_02A3((long)5000);
	}
	set_input_mode(INPUT_MODE_GENERAL_CONTINUE);
	u_delay(1, 1);
	/* */
	if(!u_kbhit()) {
		/*"present"*/
		for(loc_A = 1; loc_A <= 5; loc_A++) {
			Gra_3(15, loc_A, 14, 5 - loc_A, pTitle, 33, -1, 14);
			C_02A3((long)5000);
		}
		set_input_mode(INPUT_MODE_GENERAL_CONTINUE);
		u_delay(1, 1);
		/*"Ultima iv"*/
		for(loc_A = 0; loc_A < 57; loc_A ++) {
			if(u_kbhit())
				loc_A = 56;
			Gra_B(30, 45, 5, 34, pTitle, 34, loc_A, 5);
			if(speed_info > 4)
				Gra_B(30, 45, 5, 34, pTitle, 34, loc_A, 5);
		}
		set_input_mode(INPUT_MODE_GENERAL_CONTINUE);
		u_delay(1, 1);
	} else {
		Gra_3(30, 45,  5, 34, pTitle, 34, -1,  5);
		Gra_3(15,  5, 14,  0, pTitle, 33, -1, 14);
	}
	/*"quest of the avatar"*/
	for(loc_A = 1; loc_A <= 6; loc_A ++) {
		Gra_3(33, loc_A, 3,      81, pTitle, 86 - loc_A, -1, 3);
		Gra_3(33, loc_A, 3, 93 - loc_A, pTitle,      86, -1, 3);
		C_02A3((long)4000);
	}
	/*open view like scroll*/
	for(loc_A = 1; loc_A <= 20; loc_A ++) {
		if(u_kbhit())
			loc_A = 20;
		C_034D();
		Gra_C(5, 19, D_3683, 104, pTitle, 2);
		if(speed_info > 4)
			Gra_C(5, 19, D_3683, 104, pTitle, 2);
		loc_C = 20 - loc_A;
		Gra_3(loc_A * 2, 96, loc_C, 96, pTitle, 96, -1, loc_C);
		C_02A3((long)680);
		C_034D();
		Gra_C(5, 19, D_3683, 104, pTitle, 2);
	}
	_ffree(pTitle);

	/*the 2 "monsters"*/
/*C_0978:*/
	if((pAnim = (void far *)_fmalloc((D_7078 == 1)?0x4000:0x8000)) == 0)
		exit(0x37);
	Gra_inflate((D_7078 == 1)?/*D_00D7*/ &TITLE[0x40D7] /* "ANIMATE.PIC" */:/*D_00E3*/  &TITLE[0x40E3] /* "ANIMATE.EGA" */, pAnim);
	for(loc_A = 1; loc_A <= 32; loc_A++) {
/*C_09C4:*/
		if(u_kbhit())
			loc_A = 32;

		/*the "2 monsters"*/
		loc_C = D_3380[D_0036++]; D_0036 &= 0x7f;
		Gra_3(6, loc_A, D_344A[loc_C], D_3438[loc_C] - loc_A + 32, pAnim, 0, -1, 0);
		loc_C = D_33F8[D_0038++]; D_0038 &= 0x3f;
		Gra_3(6, loc_A, D_345C[loc_C], D_3438[loc_C] - loc_A + 32, pAnim, 0, -1, 34);
		/*the "view"*/
		C_034D();
		C_041A();
		Gra_0(5, 19, D_3A24, 104, 0, 0, 2);
		//for(loc_B = speed_info / 2; loc_B && !u_kbhit(); loc_B --)
		//	Gra_0(5, 19, D_3A24, 104, 0, 0, 2);
		C_034D();
		C_041A();
		Gra_0(5, 19, D_3A24, 104, 0, 0, 2);
		//for(loc_B = speed_info / 2; loc_B && !u_kbhit(); loc_B --)
		//	Gra_0(5, 19, D_3A24, 104, 0, 0, 2);
	}
}

void C_0B1E(bp08, bp06, si/*bp04*/)
int bp08;
int bp06;
register char *si;
{
	txt_Y = bp08;
	txt_X = bp06;
	while(*si)
		u4_putc(*si++);
}

extern char TITLE[];
#include <stdlib.h>
//#include <strings.h>

void C_0B45()
{
	Gra_2();
	C_0B1E(13,  2, /*D_00EF*/ &TITLE[0x40EF] /* "In another world, in a time to come." */);
	C_0B1E(15, 15, /*D_0114*/ &TITLE[0x4114] /* "Options:@" */);
	C_0B1E(16, 11, /*D_011D*/ &TITLE[0x411D] /* "Return to the view" */);
	C_0B1E(17, 11, /*D_0130*/ &TITLE[0x4130] /* "Journey Onward" */);
	C_0B1E(18, 11, /*D_013F*/ &TITLE[0x413F] /* "Initiate New Game" */);
	C_0B1E(20, 5, "VR Conversion by James Surine");
	//C_0B1E(21,  3, /*D_0151*/ strcat("PC ", &TITLE[0x4151] /* "Conversion by James Van Artsdalen" */));
	C_0B1E(22,  5, /*D_0173*/ &TITLE[0x40EF] /* "\x9 Copyright 1987 Lord British" */);
	txt_X = 24;
	txt_Y = 16;
	D_31C0 = 1;
}

void C_0BCA()
{
	int bp_02;

	Gra_clrscr();
	if((pShapes = _fmalloc((D_7078 == 1)?0x4000:0x8000)) == 0)
		exit(0x3a);
	if(D_7078 == 1) {
		if(Load(/*D_0191*/&TITLE[0x4191] /*"SHAPES.CGA"*/, 0x4000, pShapes) == -1)
			exit(5);
	} else {
		C_217E(/*D_019C*/&TITLE[0x419C] /*"SHAPES.EGZ"*/, pShapes);
		sizzleShapes();
	}

	if((pTitle = _fmalloc((D_7078 == 1)?0x4000:0x8000)) == 0)
		exit(0x3b);
	Gra_inflate((D_7078 == 1)?/*D_01A7*/&TITLE[0x41A7] /*"TITLE.PIC"*/:/*D_01B1*/&TITLE[0x41B1] /*"TITLE.EGA"*/, pTitle);
	C_02D1(0);
	Gra_3(4, 4, 19, 17, pTitle, 17, -1, 19);
	for(bp_02 = 86; bp_02 < 238; bp_02 ++)
		Gra_dot(31, bp_02, 2);
	Gra_3(21,  9,  9, 21, pTitle, 21, -1, 9);
	Gra_3(30, 45,  5, 34, pTitle, 34, -1, 5);
	Gra_3(15,  5, 14,  0, pTitle, 33, -1, 14);
	Gra_3(33,  6,  3, 81, pTitle, 80, -1, 3);
	Gra_3(33,  6,  3, 87, pTitle, 86, -1, 3);
	Gra_3(40, 96,  0, 96, pTitle, 96, -1, 0);
	_ffree(pTitle);

	if((pAnim = _fmalloc((D_7078 == 1)?0x4000:0x8000)) == 0)
		exit(0x3b);
	Gra_inflate((D_7078 == 1)?/*D_01BB*/&TITLE[0x40D7] /*"ANIMATE.PIC"*/:/*D_01C7*/&TITLE[0x40E3]/*"ANIMATE.EGA"*/, pAnim);

	bp_02 = D_3380[D_0036++]; D_0036 &= 0x7f;
	Gra_3(6, 32, D_344A[bp_02], D_3438[bp_02], pAnim, 0, -1, 0);
	bp_02 = D_33F8[D_0038++]; D_0038 &= 0x3f;
	Gra_3(6, 32, D_345C[bp_02], D_3438[bp_02], pAnim, 0, -1, 34);

	D_6E80 = 1;
	C_0B45();
}

struct ScreenCopyFrame
{
	int width_in_char/*bp04*/;
	int height/*bp06*/;
	int src_x_in_char/*bp08*/;
	int src_y/*bp0a*/;
	void* p/*bp0e:bp0c*/;
	int dst_y/*bp10*/;
	int random_stuff/*bp12*/;
	int dst_x_in_char/*bp14*/;
};

#define MAX_SCREEN_COPY_FRAME 10
static struct ScreenCopyFrame screen_copy_frame[MAX_SCREEN_COPY_FRAME];
static int current_screen_copy_frame_pointer;
static int current_screen_copy_frame_size;
static int screen_copy_frame_simple_mutex = 0;

void add_screen_copy_frame_to_buffer(int width_in_char/*bp04*/,
	int height/*bp06*/,
	int src_x_in_char/*bp08*/,
	int src_y/*bp0a*/,
	void* p/*bp0e:bp0c*/,
	int dst_y/*bp10*/,
	int random_stuff/*bp12*/,
	int dst_x_in_char/*bp14*/)
{
	screen_copy_frame[current_screen_copy_frame_pointer].width_in_char = width_in_char;
	screen_copy_frame[current_screen_copy_frame_pointer].height = height;
	screen_copy_frame[current_screen_copy_frame_pointer].src_x_in_char = src_x_in_char;
	screen_copy_frame[current_screen_copy_frame_pointer].src_y = src_y;
	screen_copy_frame[current_screen_copy_frame_pointer].p = p;
	screen_copy_frame[current_screen_copy_frame_pointer].dst_y = dst_y;
	screen_copy_frame[current_screen_copy_frame_pointer].random_stuff = random_stuff;
	screen_copy_frame[current_screen_copy_frame_pointer].dst_x_in_char = dst_x_in_char;

	current_screen_copy_frame_pointer++;
	if (current_screen_copy_frame_pointer > MAX_SCREEN_COPY_FRAME - 1)
	{
		current_screen_copy_frame_pointer = 0;
	}

	current_screen_copy_frame_size++;
	if (current_screen_copy_frame_size > MAX_SCREEN_COPY_FRAME)
	{
		current_screen_copy_frame_size = MAX_SCREEN_COPY_FRAME;
	}

#ifndef ENABLE_WINDOWS
	// wait until frame is read before continuing
	screen_copy_frame_simple_mutex = 1;

	while (screen_copy_frame_simple_mutex == 1)
	{
		Sleep(1 * SLEEPFACTOR);
	}
#endif
}

__declspec(dllexport) int cdecl  main_screen_copy_frame(int buffer[], int length)
{
	// check if anything to return
	if (screen_copy_frame_simple_mutex == 0)
	{
		return 0;
	}

	// save these here
	int save_screen_copy_frame_size = current_screen_copy_frame_size;
	int save_screen_copy_frame_pointer = current_screen_copy_frame_pointer;

	// check if the buffer is big enough
	if (length < save_screen_copy_frame_size * 8)
	{
		// empty the buffer
		current_screen_copy_frame_size = 0;
		current_screen_copy_frame_pointer = 0;

		screen_copy_frame_simple_mutex = 0;

		// return nothing
		return 0;
	}

	//printf("Text buffer size %d\n", current_text_buffer_size);

	int buffer_index = 0;

	// we have not filled the buffer
	if (save_screen_copy_frame_size < MAX_SCREEN_COPY_FRAME - 1)
	{
		for (int i = 0; i < save_screen_copy_frame_size; i++)
		{
			buffer[buffer_index++] = screen_copy_frame[i].width_in_char;
			buffer[buffer_index++] = screen_copy_frame[i].height;
			buffer[buffer_index++] = screen_copy_frame[i].src_x_in_char;
			buffer[buffer_index++] = screen_copy_frame[i].src_y;
			buffer[buffer_index++] = screen_copy_frame[i].p;
			buffer[buffer_index++] = screen_copy_frame[i].dst_y;
			buffer[buffer_index++] = screen_copy_frame[i].random_stuff;
			buffer[buffer_index++] = screen_copy_frame[i].dst_x_in_char;
			save_screen_copy_frame_pointer++;
		}
	}
	// the buffer is full dump from the wrap
	else
	{
		for (int i = 0; i < save_screen_copy_frame_size; i++)
		{
			buffer[buffer_index++] = screen_copy_frame[save_screen_copy_frame_pointer].width_in_char;
			buffer[buffer_index++] = screen_copy_frame[save_screen_copy_frame_pointer].height;
			buffer[buffer_index++] = screen_copy_frame[save_screen_copy_frame_pointer].src_x_in_char;
			buffer[buffer_index++] = screen_copy_frame[save_screen_copy_frame_pointer].src_y;
			buffer[buffer_index++] = screen_copy_frame[save_screen_copy_frame_pointer].p;
			buffer[buffer_index++] = screen_copy_frame[save_screen_copy_frame_pointer].dst_y;
			buffer[buffer_index++] = screen_copy_frame[save_screen_copy_frame_pointer].random_stuff;
			buffer[buffer_index++] = screen_copy_frame[save_screen_copy_frame_pointer].dst_x_in_char;
			save_screen_copy_frame_pointer++;

			// wrap
			if (save_screen_copy_frame_pointer > MAX_SCREEN_COPY_FRAME - 1)
			{
				save_screen_copy_frame_pointer = 0;
			}
		}
	}

	// empty the buffer
	current_screen_copy_frame_size = 0;
	current_screen_copy_frame_pointer = 0;
	//printf("text buffer cleared\n");

	screen_copy_frame_simple_mutex = 0;

	return save_screen_copy_frame_size;
}

static int current_dot_x;
static int current_dot_y;
static int current_dot_col;
static int dot_simple_mutex = 0;

void add_dot(int x, int y, int col)
{
	current_dot_x = x;
	current_dot_y = y;
	current_dot_col = col;

#ifndef ENABLE_WINDOWS
	// wait until dot is read before continuing
	dot_simple_mutex = 1;

	while (dot_simple_mutex == 1)
	{
		Sleep(1 * SLEEPFACTOR);
	}
#endif
}

__declspec(dllexport) int cdecl  main_dot(int buffer[], int length)
{
	int buffer_index = 0;

	// check if anything to return
	if (dot_simple_mutex == 0)
	{
		return 0;
	}

	buffer[buffer_index++] = current_dot_x;
	buffer[buffer_index++] = current_dot_y;
	buffer[buffer_index++] = current_dot_col;

	dot_simple_mutex = 0;

	return 1;
}

#define MAX_TEXT 500
static char text_buffer[MAX_TEXT];
static int current_text_buffer_pointer;
static int current_text_buffer_size;

void add_char_to_text_buffer(char ch)
{
	//printf("%c", ch);
	text_buffer[current_text_buffer_pointer++] = ch;
	if (current_text_buffer_pointer > MAX_TEXT - 1)
	{
		current_text_buffer_pointer = 0;
	}
	current_text_buffer_size++;
	if (current_text_buffer_size > MAX_TEXT)
	{
		current_text_buffer_size = MAX_TEXT;
	}
}

__declspec(dllexport) int cdecl  main_Text(unsigned char buffer[], int length)
{
	int buffer_index = 0;
	int ret = 0;

	if (length >= current_text_buffer_size)
	{
		ret = current_text_buffer_size;

		//printf("Text buffer size %d\n", current_text_buffer_size);

		// we have not filled the text buffer
		if (current_text_buffer_size < MAX_TEXT - 1)
		{
			for (int i = 0; i < current_text_buffer_size; i++)
			{
				buffer[buffer_index++] = text_buffer[i];
			}
		}
		// the text buffer is full dump from the wrap
		else
		{
			for (int i = 0; i < current_text_buffer_size; i++)
			{
				buffer[buffer_index++] = text_buffer[current_text_buffer_pointer++];

				// wrap
				if (current_text_buffer_pointer > MAX_TEXT - 1)
				{
					current_text_buffer_pointer = 0;
				}
			}
		}

		// empty the text buffer
		current_text_buffer_size = 0;
		current_text_buffer_pointer = 0;
		//printf("text buffer cleared\n");
	}

	return ret;
}

static int input_mode = 0;

void set_input_mode(int mode)
{
	input_mode = mode;
}

static int current_sound_effect = -1;

void play_sound_effect()
{
	int timeout = 1000;

	current_sound_effect = 0;

#ifndef ENABLE_WINDOWS
	// wait for the sound effect to finish playing
	while (current_sound_effect != -1 && timeout > 0)
	{
		timeout--;
		Sleep(1 * SLEEPFACTOR);
	}
#endif

	current_sound_effect = -1;
}

#define VK_SPACE          0x20
#define VK_LEFT           0x25
#define VK_UP             0x26
#define VK_RIGHT          0x27
#define VK_DOWN           0x28
#define VK_ESCAPE         0x1B
#define VK_RETURN         0x0D
#define VK_BACK           0x08
extern int CMN_kbhit;

//TODO: probably need to make this sychonous so characters cannot be missed if sent too fast
__declspec(dllexport) void cdecl main_keyboardHit(char wParam)
{
	switch (wParam) {
	case VK_LEFT: CMN_kbhit = KBD_LEFT; break;
	case VK_RIGHT: CMN_kbhit = KBD_RIGHT; break;
	case VK_UP: CMN_kbhit = KBD_UP; break;
	case VK_DOWN: CMN_kbhit = KBD_DOWN; break;

	case VK_RETURN: CMN_kbhit = KBD_ENTER; break;
	case VK_ESCAPE: CMN_kbhit = KBD_ESC; break;
	case VK_SPACE: CMN_kbhit = KBD_SPACE; break;
	case VK_BACK: CMN_kbhit = KBD_BS; break;

	case 'A': CMN_kbhit = KBD_A; break;
	case 'B': CMN_kbhit = KBD_B; break;
	case 'C': CMN_kbhit = KBD_C; break;
	case 'D': CMN_kbhit = KBD_D; break;
	case 'E': CMN_kbhit = KBD_E; break;
	case 'F': CMN_kbhit = KBD_F; break;
	case 'G': CMN_kbhit = KBD_G; break;
	case 'H': CMN_kbhit = KBD_H; break;
	case 'I': CMN_kbhit = KBD_I; break;
	case 'J': CMN_kbhit = KBD_J; break;
	case 'K': CMN_kbhit = KBD_K; break;
	case 'L': CMN_kbhit = KBD_L; break;
	case 'M': CMN_kbhit = KBD_M; break;
	case 'N': CMN_kbhit = KBD_N; break;
	case 'O': CMN_kbhit = KBD_O; break;
	case 'P': CMN_kbhit = KBD_P; break;
	case 'Q': CMN_kbhit = KBD_Q; break;
	case 'R': CMN_kbhit = KBD_R; break;
	case 'S': CMN_kbhit = KBD_S; break;
	case 'T': CMN_kbhit = KBD_T; break;
	case 'U': CMN_kbhit = KBD_U; break;
	case 'V': CMN_kbhit = KBD_V; break;
	case 'W': CMN_kbhit = KBD_W; break;
	case 'X': CMN_kbhit = KBD_X; break;
	case 'Y': CMN_kbhit = KBD_Y; break;
	case 'Z': CMN_kbhit = KBD_Z; break;

	case 'a': CMN_kbhit = KBD_a; break;
	case 'b': CMN_kbhit = KBD_b; break;
	case 'c': CMN_kbhit = KBD_c; break;
	case 'd': CMN_kbhit = KBD_d; break;
	case 'e': CMN_kbhit = KBD_e; break;
	case 'f': CMN_kbhit = KBD_f; break;
	case 'g': CMN_kbhit = KBD_g; break;
	case 'h': CMN_kbhit = KBD_h; break;
	case 'i': CMN_kbhit = KBD_i; break;
	case 'j': CMN_kbhit = KBD_j; break;
	case 'k': CMN_kbhit = KBD_k; break;
	case 'l': CMN_kbhit = KBD_l; break;
	case 'm': CMN_kbhit = KBD_m; break;
	case 'n': CMN_kbhit = KBD_n; break;
	case 'o': CMN_kbhit = KBD_o; break;
	case 'p': CMN_kbhit = KBD_p; break;
	case 'q': CMN_kbhit = KBD_q; break;
	case 'r': CMN_kbhit = KBD_r; break;
	case 's': CMN_kbhit = KBD_s; break;
	case 't': CMN_kbhit = KBD_t; break;
	case 'u': CMN_kbhit = KBD_u; break;
	case 'v': CMN_kbhit = KBD_v; break;
	case 'w': CMN_kbhit = KBD_w; break;
	case 'x': CMN_kbhit = KBD_x; break;
	case 'y': CMN_kbhit = KBD_y; break;
	case 'z': CMN_kbhit = KBD_z; break;

	case '0': CMN_kbhit = KBD_0; break;
	case '1': CMN_kbhit = KBD_1; break;
	case '2': CMN_kbhit = KBD_2; break;
	case '3': CMN_kbhit = KBD_3; break;
	case '4': CMN_kbhit = KBD_4; break;
	case '5': CMN_kbhit = KBD_5; break;
	case '6': CMN_kbhit = KBD_6; break;
	case '7': CMN_kbhit = KBD_7; break;
	case '8': CMN_kbhit = KBD_8; break;
	case '9': CMN_kbhit = KBD_9; break;
	}
}

__declspec(dllexport) int cdecl  main_sound_effect()
{
	return current_sound_effect;
}

__declspec(dllexport) int cdecl  main_input_mode()
{
	return input_mode;
}

__declspec(dllexport) void cdecl  main_sound_effect_done()
{
	current_sound_effect = -1;
}

static char U4_ROOT[256] = "C:\\Users\\Jim\\AppData\\LocalLow\\SwivelChairGames\\ANKH-VR\\u4\\";
const char* getDataPath()
{
	return &U4_ROOT[0];
}

__declspec(dllexport) void cdecl main_SetDataPath(unsigned char path[], int length)
{
	// is the path too big for our buffer + null terminator?
	if (length > 255)
	{
		// just bail
		return;
	}

	// save the path from the controlling application
	strncpy(U4_ROOT, path, length);

	// make sure it's null terminated
	U4_ROOT[length] = 0; 
}

static char current_picture[256] = "";
void * current_picture_dest;

const char* get_current_picture()
{
	return &current_picture[0];
}

extern const char* get_current_picture();
extern void* current_picture_dest;
extern void* get_screen_buffer();

static int picture_simple_mutex = 0;

void add_picture(void* dest, char* fname)
{
	current_picture_dest = dest;
	strncpy(get_current_picture(), fname, 254);

#ifndef ENABLE_WINDOWS
	// wait until dot is read before continuing
	picture_simple_mutex = 1;

	while (picture_simple_mutex == 1)
	{
		Sleep(1 * SLEEPFACTOR);
	}
#endif
}

__declspec(dllexport) int cdecl main_GetPicture(unsigned char picture[], int length)
{
	// nothing to get
	if (picture_simple_mutex == 0)
	{
		return 0;
	}

	// is the vision too big for our buffer + null terminator?
	if (length < 256)
	{
		// reset this for the next one
		current_picture[0] = 0;
		current_picture_dest = 0;
		picture_simple_mutex = 0;

		// just bail
		return 0;
	}

	// return the vision to the controlling application
	memcpy(picture, get_current_picture(), 255);

	picture[254] = 0; // make sure it's null terminated

	// save the return value
	int ret = current_picture_dest;

	// reset this for the next one
	current_picture[0] = 0;
	current_picture_dest = 0;
	picture_simple_mutex = 0;

	// return the save return value
	return ret;
}

__declspec(dllexport) void cdecl  main_D_3683(unsigned char buffer[], int length)
{
	if (length >= 19*5 /*sizeof(D_3683)*/)
	{
		memcpy(buffer, &D_3683[0], 19*5 /*sizeof(D_3683)*/);
	}
}

static int the_view_mutex = 0;

void set_the_view_mutex()
{
	the_view_mutex = 1;
}

int get_the_view_mutex()
{
	return the_view_mutex;
}


__declspec(dllexport) int cdecl  main_D_3A24(unsigned char buffer[], int length)
{
	// nothing to get
	if (the_view_mutex == 0)
	{
//		return 0;  // TODO: This mutex is not working for some reason
	}

	if (length >= sizeof(D_3A24))
	{
		memcpy(buffer, &D_3A24[0], sizeof(D_3A24));
	}

	the_view_mutex = 0;
}

__declspec(dllexport) void cdecl  main_D_6976(unsigned char buffer[], int length)
{
	if (length >= sizeof(D_6976))
	{
		memcpy(buffer, &D_6976, sizeof(D_6976));
	}
}
int QUIT = 0;


char TITLE[44672];

__declspec(dllexport) cdecl /*C_0EAA*/main()
{
	QUIT = 0;
	CMN_kbhit = 0;

#ifndef WIN32
	__android_log_print(ANDROID_LOG_INFO, "ANKH", "ANKH TITLE says hello\n");
#endif

	// we will use this buffer to reference strings and other things originally in the EXE instead of embedding them in this code
	// this will allow release of the binary of this library/application on other platforms like oculus quest as we don't
	// need to dynamically load DLL at runtime which android does not allow anymore and this allows us to remove
	// all copyright text and data that was contained in the original EXE leaving just the reverse engineered logic of the game engine
	// in this executable code thus avoiding any copyright entanglements
	if (Load("TITLE.EXE", sizeof(TITLE), &(TITLE)) == -1)
		exit(3);

	set_input_mode(INPUT_MODE_GENERAL_NO_CONTINUE);

	D_31C0 = 0;
	low_init();

	u4_toupper(far_psp->_80[4]);
	u4_toupper(far_psp->_80[2]);
	/*set drive number*/
	if(far_psp->_80[0] < 4) {
		D_7082 = ((equip_flags & 0xc0) >> 6) != 0;
	} else if(far_psp->_80[4] == '0') {
		D_7082 = 0;
	} else if(far_psp->_80[4] == '1') {
		D_7082 = 1;
	} else {
		D_7082 = ((equip_flags & 0xc0) >> 6) != 0;
	}
	/*set graphic type*/
	if(far_psp->_80[0] < 2) {
		D_7078 = low_gra();
	} else if(far_psp->_80[2] == 'C') {
		D_7078 = 1;
	} else if(far_psp->_80[2] == 'E') {
		D_7078 = 2;
	} else if(far_psp->_80[2] == 'T') {
		D_7078 = 3;
	} else {
		D_7078 = low_gra();
	}

	if(D_7078 == -1) {
		Console(/*D_01D3*/&TITLE[0x41D3]/*"I can't find a color graphics card.\r\n"*/);
		exit(2);
	}
	/* */
	D_6E80 = 0;
	if((pCharset = _fmalloc((D_7078 == 1)?0x800:0x1000)) == 0)
		exit(0x3c);
	if(Load((D_7078 == 1)?/*D_01F9*/&TITLE[0x41F9]/*"CHARSET.CGA"*/:/*D_0205*/&TITLE[0x4205]/*"CHARSET.EGA"*/, (D_7078 == 1)?0x800:0x1000, pCharset) == -1)
		exit(6);
	if((pShapes = _fmalloc((D_7078 == 1)?0x4000:0x8000)) == 0)
		exit(0x3d);
	if(D_7078 == 1) {
		if(Load(/*D_0211*/&TITLE[0x4191]/*"SHAPES.CGA"*/, 0x4000, pShapes) == -1)
			exit(5);
	} else {
		C_217E(/*D_021C*/&TITLE[0x419C]/*"SHAPES.EGZ"*/, pShapes);
	}

	Gra_init();
	C_331E();/*set int 24h handler*/

	if((pTitle = _fmalloc((D_7078 == 1)?0x4000:0x8000)) == 0)
		exit(0x3e);
	Gra_inflate((D_7078 == 1)?/*D_0227*/&TITLE[0x41A7]/*"TITLE.PIC"*/:/**D_0231*/&TITLE[0x41B1]/*"TITLE.EGA"*/, pTitle);

	if(D_7078 == 1)
		speed_info ++;
	D_3A84 = 0;

	C_068C();/*logo + splash + animation*/

	D_6E80 = 1;
/*C_10E2:*/

	C_05A4();

	C_0B45();
	do {
		int bp_02;

/*C_10E8:*/
		set_input_mode(INPUT_MODE_MAIN_MENU);
		bp_02 = u_kbread();
		if(u4_isupper((unsigned char)bp_02))
			bp_02 = (bp_02 & 0xff00) | u4_lower((unsigned char)bp_02);
		if((unsigned char)bp_02 > ' ' && ((unsigned char)bp_02 & 0x80) == 0)
			Gra_putchar((unsigned char)bp_02);
		switch(bp_02) {
			case KBD_r:/*(R)eturn*/
				C_05A4();
				C_0B45();
			break;
			case KBD_i:/*(I)nitiate*/
				C_3030();
				if (QUIT == 0)
				{
					// redraw the title screen if we bail out of the initiate new game
					C_0BCA();
				}
			break;
			case KBD_j:/*(J)ourney*/
				_ffree(pAnim);
				_ffree(pShapes);
				_ffree(pCharset);
				low_clean();
				QUIT = 1;
				//exit(D_7082?'1':'0');
			break;
			default:
				sound_1();
			break;
		}
	//} while (1);
	} while (QUIT == 0);
	set_input_mode(INPUT_MODE_LAUNCH_GAME);
	//__android_log_print(ANDROID_LOG_INFO, "ANKH", "ANKH says hello 30\n");
}
