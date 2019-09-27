/*
 * Copyright(C) 2000-2001 Проект ИОК
 *
 * Этот файл содержит информацию, являющуюся
 * собственностью компании Крипто Про.
 *
 * Любая часть этого файла не может быть скопирована,
 * исправлена, переведена на другие языки,
 * локализована или модифицирована любым способом,
 * откомпилирована, передана по сети с или на
 * любую компьютерную систему без предварительного
 * заключения соглашения с компанией Крипто Про.
 *
 * Программный код, содержащийся в этом файле, предназначен
 * исключительно для целей обучения и не может быть использован
 * для защиты информации.
 *
 * Компания Крипто-Про не несет никакой
 * ответственности за функционирование этого кода.
 */

/*!
 * \file $RCSfile: mtime.c,v $
 * \version $Revision: 1.3 $
 * \date $Date: 2001/12/25 15:57:52 $
 * \author $Author: pre $
 *
 * \brief Процедуры измерения и печати времени
 *
 */

#include "tmain.h"
#include <assert.h>
#include <stdio.h>
#include <sys/timeb.h>
#include <time.h>

#define IT_ID   (0x18c6b07a)
typedef struct tagInternalTime {
    size_t  len;
    int     id;
    struct tagIT {
        clock_t         clk;
        ULARGE_INTEGER  kt;
        ULARGE_INTEGER  ut;
    }       i;
}   InternalTime;

InternalTime    *StartTime = NULL;

static _inline
InternalTime *NewInternalTime(void) {
    InternalTime *it = malloc(sizeof(InternalTime));

    if (it) {
        it->len = sizeof(InternalTime);
        it->id = IT_ID;
    }
    return it;
}

static _inline
const InternalTime *PT2IT(const PublicTime *pt) {
    if (pt && pt->len == sizeof(InternalTime) && pt->id == IT_ID) {
        return (const InternalTime *)pt;
    }
    return NULL;
}

static _inline
PublicTime *IT2PT(const InternalTime *it) {
    if (it && it->len == sizeof(InternalTime) && it->id == IT_ID) {
        return (PublicTime *)it;
    }
    return NULL;
}

static _inline
void GetCurrentMTime(InternalTime *c) {
    FILETIME a, b;
    c->i.clk = clock();
    GetThreadTimes(GetCurrentThread(), &a, &b, (LPFILETIME)&c->i.kt, (LPFILETIME)&c->i.ut);
}

static
void SubTime(InternalTime *a, const InternalTime *b) {
    a->i.clk -= b->i.clk;
    a->i.kt.QuadPart -= b->i.kt.QuadPart;
    a->i.ut.QuadPart -= b->i.ut.QuadPart;
}

void MTimeInit(void){
    if ((StartTime = NewInternalTime()) != NULL) {
        GetCurrentMTime(StartTime);
    }
}

PublicTime  *MTimeGet(const PublicTime  *base) {
    InternalTime        *c = NewInternalTime();
    const InternalTime  *ib = PT2IT(base);

    if (!c) {
        return NULL;
    }
    GetCurrentMTime(c);
    SubTime(c, StartTime);
    if (ib) {
        SubTime(c, ib);
    }
    return IT2PT(c);
}

void MTimePrint(const PublicTime  *time) {
    const InternalTime  *t = PT2IT(time);

    fprintf(stderr, "SYS: %.3f sec USR: %.3f sec UTC: %.3f sec\n", 
                (double)(__int64)t->i.kt.QuadPart/10000000.0,
                (double)(__int64)t->i.ut.QuadPart/10000000.0,
                (double)t->i.clk/(double)CLOCKS_PER_SEC);
}


void MTimePerfPrint(const PublicTime  *time, int Nelem, int Divider, const char *Name) {
    const InternalTime  *t = PT2IT(time);

    fprintf(stderr, "SYS: %.3f %s/sec USR: %.3f %s/sec UTC: %.3f %s/sec\n", 
            10000000.0*(double)Nelem/((double)(__int64)t->i.kt.QuadPart*(double)Divider), Name,
            10000000.0*(double)Nelem/((double)(__int64)t->i.ut.QuadPart*(double)Divider), Name,
            (double)CLOCKS_PER_SEC*(double)Nelem/((double)t->i.clk*(double)Divider), Name);
}
