/* Интерфейс перекодировки BASE64 (BASE64HDR) <-> DER
 */

#ifndef _BASE64_H_INCLUDED
#define _BASE64_H_INCLUDED

#ifdef __cplusplus
extern "C" {
#endif

#ifdef UNIX
#include "CSP_WinDef.h"
#endif /* UNIX */

typedef struct tagBase64Headers {
    size_t      mSizeOF;
    const BYTE *mBegin;
    const BYTE *mEnd;
}   BASE64HEADERS;

extern const BASE64HEADERS Base64CertificateHdrs;
extern const BASE64HEADERS Base64RequestHdrs;

BOOL base64_decode(const BYTE *pb64, DWORD cb64, BYTE *pbDer, DWORD *pcbDer);
BOOL base64_encode(const BYTE *pbDer, DWORD cbDer, BYTE *pb64, DWORD *pcb64);
BOOL base64hdr_decode(const BASE64HEADERS *phdrs, const BYTE *pb64h, DWORD cb64h, BYTE *pbDer, DWORD *pcbDer);
BOOL base64hdr_encode(const BASE64HEADERS *phdrs, const BYTE *pbDer, DWORD cbDer, BYTE *pb64h, DWORD *pcb64h);

#ifdef __cplusplus
}
#endif

#endif /* _BASE64_H_INCLUDED */
