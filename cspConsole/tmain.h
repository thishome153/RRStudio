
/*!
 * \file $RCSfile: tmain.h,v $
 * \version $Revision: 1.38.4.5 $
 * \date $Date: 2002/08/28 07:06:46 $
 * \author $Author: vasilij $
 *
 * \brief Интерфейс программы.
 */

#ifndef _TMAIN_H_INCLUDED
#define _TMAIN_H_INCLUDED



#ifdef WIN32
#pragma warning (disable:4115)
#endif /* WIN32 */

#ifdef UNIX
#define CTKEYSET 1
#define TSTRESS 1
#endif /* UNIX */

/* Для старого wincrypt.h нужна эта переменная */
#ifndef _WIN32_WINNT
#define _WIN32_WINNT 0x0400
#endif

#include <stdio.h>
#include <malloc.h>

#ifdef UNIX
#include <sys/timeb.h>
#include <dlfcn.h>
#include <link.h>
#include "CSP_WinDef.h"
#include "CSP_WinCrypt.h"
#include "CSP_WinError.h"
#else /* UNIX */

#include <windows.h>
#include <wincrypt.h>
#endif /* UNIX */

#include <memory.h>
#include <sys/stat.h>
#ifndef UNIX
#include <wincryptex.h>
#endif /* UNIX */
#include "getopt.h"
#include "base64.h"

#ifndef UNIX
#ifndef CERT_STORE_CREATE_NEW_FLAG
#error You need Platform SDK Jan 2001 or latter
#endif

#define HAVE_MAPVIEWOFFILE 1
#endif /* UNIX */

#define SoftName       "CSP Fixosoft Console"
#define ShortSoftName "cspcli Fixosoft"
#ifndef Fixosoft
#define Fixosoft "Fixosoft"
#define BurthDate 1975
#endif


#define TYPE_DER (X509_ASN_ENCODING | PKCS_7_ASN_ENCODING)

#define USES_CONVERSION \
    int _convert = 0; unsigned int _acp = CP_ACP; \
    LPCWSTR _lpw = NULL; LPCSTR _lpa = NULL
#define ATLA2WHELPER AtlA2WHelper
#define A2W(lpa) (((_lpa = lpa) == NULL) ? NULL : ( \
    _convert = (lstrlenA(_lpa)+1), \
    ATLA2WHELPER((LPWSTR) _alloca(_convert*2), _lpa, _convert, _acp)))
#define ATLW2AHELPER AtlW2AHelper
#define W2A(lpw) (((_lpw = lpw) == NULL) ? NULL : ( \
    _convert = (lstrlenW(_lpw)+1)*2, \
    ATLW2AHELPER((LPSTR) _alloca(_convert), _lpw, _convert, _acp)))

#define HandleErrorFL(s) (HandleError(s,__FILE__,__LINE__))
#define DebugErrorFL(s) (DebugError(s,__FILE__,__LINE__))

typedef struct tagPublicTime {
    size_t  len;
    int     id;
    char    buf[1];
}   PublicTime;

typedef enum tagFileFormat {DER, BASE64, BASE64HDR} FILEFORMAT;

typedef struct tagFmtFileDescr {
    size_t mSizeOF;
    const char *mFileName;
    FILEFORMAT mFormat;
    const BASE64HEADERS *mB64Hdrs;
} FMTFILEDESCR;

extern char *prog;
extern char *stdinfile;

extern LPWSTR WINAPI AtlA2WHelper(LPWSTR lpw, LPCSTR lpa, int nChars,
    UINT acp);
extern LPSTR  WINAPI AtlW2AHelper(LPSTR lpa, LPCWSTR lpw, int nChars,
    UINT acp);

extern int vstr2type (char *s);
extern void lookup_fail (char *name, char *tag);
extern int HandleError(const char *s, const char *f, int l);
extern int DebugError(const char *s, const char *f, int l);
extern char *abbr2provider (char *abbr);
extern DWORD abbr2provtype (char *abbr);
#if defined( DEBUG_PRO ) || defined( __BOUNDSCHECKER__ )
extern void cpcsp_reboot(HANDLE hCSPDLL);
#else /* defined( DEBUG_PRO ) */
#define cpcsp_reboot(a)
#endif /* defined( DEBUG_PRO ) */

#ifndef UNIX
extern PCCERT_CONTEXT GetRecipientCert(HCERTSTORE hCertStore);
extern PCCERT_CONTEXT read_cert_from_file (const char *fname);
extern BOOL WINAPI CryptAcquireProvider (char *pszStoreName,
    PCCERT_CONTEXT cert, HCRYPTPROV *phCryptProv, DWORD *keytype,
    BOOL *release);
extern PCCERT_CONTEXT read_cert_from_my (char *subject_name);
extern PCCERT_CONTEXT read_cert_from_MY (char *subject_name);
extern PCCERT_CONTEXT WINAPI global_my_get_cert (void *pvGetArg,
    DWORD dwCertEncodingType, PCERT_INFO pSignerId, HCERTSTORE hMsgCertStore);
extern PCCERT_CONTEXT WINAPI global_MY_get_cert (void *pvGetArg,
    DWORD dwCertEncodingType, PCERT_INFO pSignerId, HCERTSTORE hMsgCertStore);
#endif /* UNIX */

extern int get_file_data_pointer (const char *infile, size_t *len,
    unsigned char **buffer);
extern int release_file_data_pointer (unsigned char *buffer);

extern int get_file_data_pointer_fmt (const FMTFILEDESCR *infile, size_t *len,
    unsigned char **buffer);

extern int release_file_data_pointer_fmt (const FMTFILEDESCR *infile,
    unsigned char *buffer);

extern int write_file (const char *file, long len, const unsigned char *buffer);
extern int write_file_fmt (const FMTFILEDESCR *file, size_t len,
    const unsigned char *buffer);

#ifndef UNIX
extern PCCERT_CONTEXT read_cert_from_file_fmt (const FMTFILEDESCR *file);
#endif /* UNIX */

extern PublicTime *MTimeGet(const PublicTime  *base);

#ifdef UNIX
#define MTimeInit()
#define MTimePrint(time) printf("\n")
#define MTimePerfPrint(time,Nelem,Divider,Name) ptintf("\n")
#else
extern void MTimeInit(void);
extern void MTimePrint(const PublicTime  *time);
extern void MTimePerfPrint(const PublicTime  *time, int Nelem, int Divider,
    const char *Name);
#endif

extern int main_CMSsign (int argc, char **argv);
extern int main_CMSsign_sf (int argc, char **argv);
extern int main_sign (int argc, char **argv);
extern int main_sign_sf (int argc, char **argv);
extern int main_q (int argc, char **argv);
extern int main_makecert (int argc, char **argv);
extern int main_property (int argc, char **argv);
extern int main_property1 (int argc, char **argv);
extern int main_rc (int argc, char **argv);
extern int main_show (int argc, char **argv);
extern int main_sign_1 (int argc, char **argv);
extern int main_hash (int argc, char **argv);
extern int main_cms_encrypt (int argc, char **argv);
extern int main_encrypt (int argc, char **argv);
extern int main_encrypt_sf (int argc, char **argv);
extern int main_crypt_encrypt (int argc, char **argv);
extern int main_keyset (int argc, char **argv);
extern int main_newmsg (int argc, char **argv);
extern int main_cryptui (int argc, char **argv);

extern int main_stress (int argc, char **argv);
extern 
#if !defined(UNIX)
    HANDLE 
#else
    void *
#endif /* !defined(UNIX) */
        load_CSP_library (DWORD dwProvType, const char *szDllName);

extern int main_tlss (int argc, char **argv);
extern int main_tlsc (int argc, char **argv);
extern int main_prf (int argc, char **argv);
extern int main_csp_check (int argc, char **argv);
extern int main_export_public (int argc, char **argv);

#endif /* _TMAIN_H_INCLUDED */
