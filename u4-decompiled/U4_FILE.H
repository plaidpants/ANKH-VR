/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */
#ifndef __U4_FILE_H__
#define __U4_FILE_H__

#ifdef WIN32
#define far
#endif

#ifdef ENABLE_WINDOWS
extern /*C_0812*/Console(char *);
#endif
extern /*C_0855*/int Load(char*, int, char*);
extern /*C_0855*/int LoadSeek(char*, int, int, char*);
extern /*C_08D1*/int Save(char *, int, char*);
#endif
