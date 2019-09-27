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
 * \file $RCSfile: base64.c,v $
 * \version $Revision: 1.9 $
 * \date $Date: 2002/05/31 12:58:18 $
 * \author $Author: pooh $
 *
 * \brief Процедуры перекодировки BASE64 (BASE64HDR) <-> DER
 *
 */

#include "tmain.h"
#include "base64.h"
#include <assert.h>
#include <string.h>

#ifdef UNIX
extern DWORD GetLastError(void);
extern void SetLastError(DWORD);
#endif /* UNIX */


const BASE64HEADERS Base64CertificateHdrs = {
    sizeof(Base64CertificateHdrs),
    (const BYTE *)"-----BEGIN CERTIFICATE-----",
    (const BYTE *)"-----END CERTIFICATE-----"
};

const BASE64HEADERS Base64RequestHdrs = {
    sizeof(Base64RequestHdrs),
    (const BYTE *)"-----BEGIN NEW CERTIFICATE REQUEST-----",
    (const BYTE *)"-----END NEW CERTIFICATE REQUEST-----"
};

#ifndef CHARSET_EBCDIC
#define conv_bin2ascii(a)       (data_bin2ascii[(a)&0x3f])
#define conv_ascii2bin(a)       (data_ascii2bin[(a)&0x7f])
#else
/* We assume that PEM encoded files are EBCDIC files
 * (i.e., printable text files). Convert them here while decoding.
 * When encoding, output is EBCDIC (text) format again.
 * (No need for conversion in the conv_bin2ascii macro, as the
 * underlying textstring data_bin2ascii[] is already EBCDIC)
 */
#define conv_bin2ascii(a)       (data_bin2ascii[(a)&0x3f])
#define conv_ascii2bin(a)       (data_ascii2bin[os_toascii[a]&0x7f])
#endif

/* 64 char lines
 * pad input with 0
 * left over chars are set to =
 * 1 byte  => xx==
 * 2 bytes => xxx=
 * 3 bytes => xxxx
 */
#define BIN_PER_LINE    (64/4*3)
#define CHUNKS_PER_LINE (64/4)
#define CHAR_PER_LINE   (64+1)

static unsigned char data_bin2ascii[65]="ABCDEFGHIJKLMNOPQRSTUVWXYZ\
abcdefghijklmnopqrstuvwxyz0123456789+/";

/* 0xF0 is a EOLN
 * 0xF1 is ignore but next needs to be 0xF0 (for \r\n processing).
 * 0xF2 is EOF
 * 0xE0 is ignore at start of line.
 * 0xFF is error
 */

#define B64_EOLN                0xF0
#define B64_CR                  0xF1
#define B64_EOF                 0xF2
#define B64_WS                  0xE0
#define B64_ERROR               0xFF
#define B64_NOT_BASE64(a)       (((a)|0x13) == 0xF3)

static unsigned char data_ascii2bin[128]={
        0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
        0xFF,0xE0,0xF0,0xFF,0xFF,0xF1,0xFF,0xFF,
        0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
        0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
        0xE0,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
        0xFF,0xFF,0xFF,0x3E,0xFF,0xF2,0xFF,0x3F,
        0x34,0x35,0x36,0x37,0x38,0x39,0x3A,0x3B,
        0x3C,0x3D,0xFF,0xFF,0xFF,0x00,0xFF,0xFF,
        0xFF,0x00,0x01,0x02,0x03,0x04,0x05,0x06,
        0x07,0x08,0x09,0x0A,0x0B,0x0C,0x0D,0x0E,
        0x0F,0x10,0x11,0x12,0x13,0x14,0x15,0x16,
        0x17,0x18,0x19,0xFF,0xFF,0xFF,0xFF,0xFF,
        0xFF,0x1A,0x1B,0x1C,0x1D,0x1E,0x1F,0x20,
        0x21,0x22,0x23,0x24,0x25,0x26,0x27,0x28,
        0x29,0x2A,0x2B,0x2C,0x2D,0x2E,0x2F,0x30,
        0x31,0x32,0x33,0xFF,0xFF,0xFF,0xFF,0xFF,
        };


static DWORD Base64DecodeBlock(BYTE *pbDer, DWORD cbDer,
    const BYTE *pb64, DWORD cb64);

BOOL base64_decode(
    const BYTE *pb64, 
    DWORD cb64, 
    BYTE *pbDer, 
    DWORD *pcbDer) {
    int eof=0,tmp,n=0,ln=0,tmp2,exp_nl=0;
    DWORD i,v;
    BYTE d[64];
    BYTE  *pbsDer = pbDer;
    DWORD cbDer;

    if (!pb64 || !pcbDer) {
        SetLastError(ERROR_BAD_ARGUMENTS);
        return FALSE;
    }
    cbDer = *pcbDer;
    *pcbDer = 0;

    /* We parse the input data */
    for (i=0; i < cb64; i++) {
        /* If the current line is > 80 characters, scream alot */
        if (ln >= 80) { 
            goto badcode; 
        }

        /* Get char and put it into the buffer */
        tmp = *(pb64++);
        v = conv_ascii2bin(tmp);
        /* only save the good data :-) */
        if (!B64_NOT_BASE64(v)) {
            d[n++] = (BYTE)tmp;
            ln++;
        } else if (v == B64_ERROR) {
            goto badcode; 
        }

        /* have we seen a '=' which is 'definitly' the last
         * input line.  seof will point to the character that
         * holds it. and eof will hold how many characters to
         * chop off. */
        if (tmp == '=') {
            eof++;
        }

        /* eoln */
        if (v == B64_EOLN) {
            ln = 0;
            if (exp_nl) {
                exp_nl = 0;
                continue;
            }
        }
        exp_nl=0;

        /* If we are at the end of input and it looks like a
         * line, process it. */
        if (((i+1) == cb64) && (((n&3) == 0) || eof)) {
            v = B64_EOF;
        }

        if ((v == B64_EOF) || (n >= sizeof(d))) {
            /* This is needed to work correctly on 64 byte input
             * lines.  We process the line and then need to
             * accept the '\n' */
            if ((v != B64_EOF) && (n >= sizeof(d))) {
                exp_nl = 1;
            }
            tmp2 = v;
            if (n > 0) {
                v = Base64DecodeBlock(pbDer, cbDer, d, n);
                if (v == -1) {
                    goto badcode;
                }
                if ((DWORD)(v - eof) > cbDer) {
                    pbDer = NULL;
                    cbDer = 0;
                }
                n = 0;
                v -= eof;
                (*pcbDer) += v;
                if (pbDer) {
                    pbDer += v;
                    if (v <= cbDer) {
                        cbDer -= v;
                    } else {
                        pbDer = NULL;
                        cbDer = 0;
                    }
                }
            } else {
                eof = 1;
                v = 0;
            }
        }    
    }

    if (pbsDer == NULL) {
        SetLastError(ERROR_SUCCESS);
        return TRUE;
    }

    if (pbDer == NULL) {
        SetLastError(ERROR_MORE_DATA);
        return FALSE;
    }

    SetLastError(ERROR_SUCCESS);
    return TRUE;

badcode:
    *pcbDer = 0;
    SetLastError((DWORD)CRYPT_E_BAD_ENCODE);
    return FALSE;
}

BOOL base64_encode(
    const BYTE *pbDer, 
    DWORD cbDer, 
    BYTE *pb64, 
    DWORD *pcb64) {
    DWORD cb64;
    int i;
    unsigned long l;

    if (!pbDer || !pcb64) {
        SetLastError(ERROR_BAD_ARGUMENTS);
        return FALSE;
    }
    cb64 = (cbDer/48)*(64 + 1);
    //cb64 += (((cbDer%48) + 3 - 1)/3)*4 + 1;
    cb64 += ((cbDer%48)/3)*4 + ((cbDer%48)%3 ? 4 : 0) + 1;

    if (!pb64) {
        SetLastError(ERROR_SUCCESS);
        *pcb64 = cb64;
        return TRUE;
    } else if (*pcb64 < cb64) {
        *pcb64 = cb64;
        SetLastError(ERROR_MORE_DATA);
        return FALSE;
    }

    *pcb64 = 0;
    while(cbDer > 0) {
        for (i = (cbDer > 48 ? 48 : cbDer); i > 0; i -= 3) {
            if (i >= 3) {
                l=      (((unsigned long)pbDer[0])<<16L)|
                        (((unsigned long)pbDer[1])<< 8L)|pbDer[2];
                *pb64++ = conv_bin2ascii(l>>18L);
                *pb64++ = conv_bin2ascii(l>>12L);
                *pb64++ = conv_bin2ascii(l>> 6L);
                *pb64++ = conv_bin2ascii(l     );
		pbDer += 3;
		cbDer -= 3;
            } else {
                l = ((unsigned long)pbDer[0])<<16L;
                if (i == 2) {
                    l |= ((unsigned long)pbDer[1]<<8L);
                }

                *pb64++ = conv_bin2ascii(l>>18L);
                *pb64++ = conv_bin2ascii(l>>12L);
                *pb64++ = (BYTE)(i == 1 ? '=' : conv_bin2ascii(l>> 6L));
                *pb64++ = '=';

		pbDer += i;
		cbDer -= i;
            }
            *pcb64 += 4;
        }
        *pb64++ = '\n';
        *pcb64 += 1;
    }

    assert (*pcb64 == cb64);
    assert (cbDer == 0);

    SetLastError(ERROR_SUCCESS);
    return TRUE;
}

BOOL base64hdr_decode(
    const BASE64HEADERS *phdrs, 
    const BYTE *pb64h, 
    DWORD cb64h, 
    BYTE *pbDer, 
    DWORD *pcbDer) {
    size_t i, l;

    if (phdrs->mSizeOF != sizeof(*phdrs) || !phdrs->mBegin || !phdrs->mEnd ||
        !pb64h || !pcbDer) {
        SetLastError(ERROR_BAD_ARGUMENTS);
        return FALSE;
    }
    l = strlen((char *)phdrs->mBegin);
    if (cb64h < l || strncmp((char *)pb64h, (char *)phdrs->mBegin, l) != 0) {
        goto badcode;
    }
    pb64h += l;
    cb64h -= l;
    l = strlen((char *)phdrs->mEnd);
    for (i = 0; i < 3 && l+i <= cb64h; i++) {
        if (strncmp((char *)(pb64h+cb64h-l-i), (char *)phdrs->mEnd, l) == 0) {
            goto found_end;
        }
    }
    goto badcode;
found_end:
    cb64h -= l+i; 

    return base64_decode(pb64h, cb64h, pbDer, pcbDer);

badcode:
    *pcbDer = 0;
    SetLastError((DWORD)CRYPT_E_BAD_ENCODE);
    return FALSE;
}

BOOL base64hdr_encode(
    const BASE64HEADERS *phdrs, 
    const BYTE *pbDer, 
    DWORD cbDer, 
    BYTE *pb64h, 
    DWORD *pcb64h) {

    DWORD cb64h;
    size_t lb;
    size_t le;

    if (phdrs->mSizeOF != sizeof(*phdrs) || !phdrs->mBegin || !phdrs->mEnd ||
        !pbDer || !pcb64h) {
        SetLastError(ERROR_BAD_ARGUMENTS);
        return FALSE;
    }

    cb64h = *pcb64h;
    lb = strlen((char *)phdrs->mBegin);
    le = strlen((char *)phdrs->mEnd);

    if (!pb64h) {
        if (!base64_encode(pbDer, cbDer, NULL, pcb64h)) {
            *pcb64h = 0;
            return FALSE;
        }
        *pcb64h += lb + 1 + le + 1;
        return TRUE;
    }

    if (cb64h < lb + 1 + le + 1) {
        if (!base64_encode(pbDer, cbDer, NULL, pcb64h)) {
            *pcb64h = 0;
            return FALSE;
        }
        *pcb64h += lb + 1 + le + 1;
        SetLastError(ERROR_MORE_DATA);
        return FALSE;
    }

    cb64h -= lb;
    memcpy(pb64h, phdrs->mBegin, lb);
    pb64h += lb;
    *pcb64h = lb;

    cb64h -= 1;
    *pb64h++ = '\n';
    *pcb64h += 1;
    
    cb64h -= le + 1;
    if (!base64_encode(pbDer, cbDer, pb64h, &cb64h)) {
        if (GetLastError() == ERROR_MORE_DATA) {
            *pcb64h += cb64h + le + 1;
            return FALSE;
        }
        *pcb64h = 0;
        return FALSE;
    }
    pb64h += cb64h;
    *pcb64h += cb64h;

    memcpy(pb64h, phdrs->mEnd, le);
    pb64h += le;
    *pcb64h += le;

    *pb64h++ = '\n';
    *pcb64h += 1;

    return TRUE;
}

static DWORD
Base64DecodeBlock(BYTE *pbDer, DWORD cbDer, const BYTE *pb64, DWORD cb64)
{
    DWORD i;
    int ret=0,a,b,c,d;
    unsigned long l;

    /* trim white space from the start of the line. */
    while ((conv_ascii2bin(*pb64) == B64_WS) && (cb64 > 0)) {
        pb64++;
        cb64--;
    }

    /* strip off stuff at the end of the line
     * ascii2bin values B64_WS, B64_EOLN, B64_EOLN and B64_EOF */
    while ((cb64 > 3) && (B64_NOT_BASE64(conv_ascii2bin(pb64[cb64-1])))) {
        cb64--;
    }

    if (cb64%4 != 0) {
        return (DWORD)-1;
    }

    for (i=0; i<cb64; i+=4) {
        a=conv_ascii2bin(*(pb64++));
        b=conv_ascii2bin(*(pb64++));
        c=conv_ascii2bin(*(pb64++));
        d=conv_ascii2bin(*(pb64++));
        if (    (a & 0x80) || (b & 0x80) ||
                (c & 0x80) || (d & 0x80))
                return (DWORD)-1;
        l=(     (((unsigned long)a)<<18L)|
                (((unsigned long)b)<<12L)|
                (((unsigned long)c)<< 6L)|
                (((unsigned long)d)     ));
        if (pbDer) {
            if ((cbDer--) == 0) {
                pbDer = NULL;
                cbDer = 0;
                goto next;
            }
            *(pbDer++)=(unsigned char)((l>>16L)&0xff);
            if ((cbDer--) == 0) {
                pbDer = NULL;
                cbDer = 0;
                goto next;
            }
            *(pbDer++)=(unsigned char)((l>> 8L)&0xff);
            if ((cbDer--) == 0) {
                pbDer = NULL;
                cbDer = 0;
                goto next;
            }
            *(pbDer++)=(unsigned char)((l     )&0xff);
        }
next:
        ret+=3;
    }
    return(ret);
}

#if !defined(lint) && !defined(NO_RCSID) && !defined(OK_MODULE_VERSION)
static volatile const char rcsid[] = "\n$Id: base64.c,v 1.9 2002/05/31 12:58:18 pooh Exp $";
#endif
