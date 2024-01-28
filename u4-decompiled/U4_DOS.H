/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */

#ifndef __U4_DOS_H__
#define __U4_DOS_H__

#include <stdio.h>

/*u4_dos*/
extern void * /*C_1FF4*/dalloc(int);
extern int /*C_2023*/dfree(void *);
extern FILE* /*C_2033*/dopen_deprecate(char *, int);
extern int /*C_2049*/dlseek_deprecate(FILE*, unsigned long);
extern int /*C_2067*/dread_deprecate(FILE*, void *, int);
extern int /*C_208B*/dwrite_deprecate(FILE*, void *, int);
extern int /*C_20AB*/dclose_deprecate(FILE*);

#endif
