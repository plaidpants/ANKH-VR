/*
 * Ultima IV - Quest Of The Avatar
 * (c) Copyright 1987 Lord British
 * reverse-coded by Ergonomy Joe in 2012
 */
#ifndef __U4_GRAPH_H__
#define __U4_GRAPH_H__

extern int C_20C1(char *);
extern int C_213B();
extern int Gra_line(int, int, int, int, int);
extern int /*C_2201*/Gra_00(int, int);
extern int /*C_2206*/Gra_01(/**/);
extern int /*C_220B*/Gra_02(/**/);
extern int /*C_2210*/Gra_03();
extern int /*C_221B*/Gra_04();
extern int /*C_2226*/Gra_05(/*int h, int w, void *src, int dst_ofs*/);
extern int /*C_222B*/Gra_dot(/*int, int, int*/);
extern int /*C_2230*/Gra_CR();
extern int /*C_2241*/Gra_09();
extern int /*C_2246*/Gra_10();
extern int /*C_224B*/Gra_11(int);
extern int /*C_2250*/Gra_12(int, int);/*(uncalled)*/
extern int /*C_226C*/Gra_13();
extern int /*C_2271*/Gra_dot_OR(/*int, int, int*/);
extern int /*C_2276*/Gra_dot_XOR(/*int, int, int*/);
extern int /*C_227B*/Gra_16(/*char *, int, int*/);
extern int /*C_2280*/Gra_17();
extern int /*C_2297*/Gra_18(/**/);
extern int /*C_229C*/Gra_19();
extern int /*C_22A7*/Gra_animSpit();
extern int /*C_22AC*/Gra_init(/*char *, char *, callback*/);
extern int /*C_22B1*/Gra_clrscr();
extern int /*C_22B6*/Gra_animFlow(unsigned);/*bug?*/
extern int /*C_22BB*/Gra_animFlag();
extern int /*C_22C0*/Gra_putchar(char);
extern int /*C_22CF*/sizzleShapes();
extern int /*C_2341*/sizzleCharset();

#endif
