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
 * \file $RCSfile: ctkey.c,v $
 * \version $Revision: 1.68.4.5 $
 * \date $Date: 2002/08/22 13:59:51 $
 * \author $Author: lse $
 *
 * \brief Программа тестирования основных функицй различных криптопровайдеров.
 *
 */

#include "tmain.h"

#ifdef UNIX
#include "CSP_WinCrypt.h"
#include <pthread.h>
#include <stdlib.h>
#include <string.h>
#include <dlfcn.h>
#include <link.h>

extern DWORD GetLastError(void);
#endif /* UNIX */

#ifndef PROG
#define PROG main_keyset
#endif

#if !defined(UNIX)
extern HANDLE	hCSPDLL = 0;	    /* Дескриптор загруженного модуля криптопровайдера*/
#else
extern void	*hCSPDLL = NULL;    /* Дескриптор загруженного модуля криптопровайдера*/
#endif /* !defined(UNIX) */

#ifdef UNIX
static BOOL CPlevel=1;	/* Флаг непосредственного использования функций криптопровайдера*/
#else /* UNIX */
static BOOL CPlevel;	/* Флаг непосредственного использования функций криптопровайдера*/
#endif /* UNIX */

#ifndef UNIX
/*-------------------------------------------------------------------*/
/* Определение функции, загружающей криптопровайдер по заданному типу*/
typedef struct _VTABLEPROVSTRUC {
    DWORD Version;
    FARPROC FuncVerifyImage;
    FARPROC FuncReturnhWnd;
    DWORD dwProvType;
    BYTE *pbContextInfo;
    DWORD cbContextInfo;
    LPSTR pszProvName;
} VTABLEPROVSTRUC, *PVTABLEPROVSTRUC;
#endif /* UNIX */

/*------------------------------------------------------*/
/* Определение функции CPAcquireContext*/
typedef BOOL (WINAPI *CPAcquireContext_t) (
    HCRYPTPROV *phProv,       /* out*/
    CHAR *pszContainer,       /* in, out*/
    DWORD dwFlags,            /* in*/
    PVTABLEPROVSTRUC pVTable  /* in*/
);
static CPAcquireContext_t MyAcquireContext;

/*------------------------------------------------------*/
/* Определение функции CPGetProvParam */
typedef BOOL (WINAPI *CPGetProvParam_t) (
    HCRYPTPROV hProv,  /* in*/
    DWORD dwParam,     /* in*/
    BYTE *pbData,      /* out*/
    DWORD *pdwDataLen, /* in, out*/
    DWORD dwFlags      /* in*/
);
static CPGetProvParam_t MyGetProvParam;

/*------------------------------------------------------*/
/* Определение функции CPSetProvParam */
typedef BOOL (WINAPI *CPSetProvParam_t) (
    HCRYPTPROV hProv,  /* in*/
    DWORD dwParam,     /* in*/
    BYTE *pbData,      /* in*/
    DWORD dwFlags      /* in*/
);
static CPSetProvParam_t MySetProvParam;

/*------------------------------------------------------*/
/* Определение функции CPGetUserKey */
typedef BOOL (WINAPI *CPGetUserKey_t) (
    HCRYPTPROV hProv,      
    DWORD dwKeySpec,       
    HCRYPTKEY *phUserKey   
);
static CPGetUserKey_t MyGetUserKey;

/*------------------------------------------------------*/
/* Определение функции CPGenKey */
typedef BOOL (WINAPI *CPGenKey_t) (
    HCRYPTPROV hProv,     /* in*/
    ALG_ID Algid,         /* in*/
    DWORD dwFlags,        /* in*/
    HCRYPTKEY *phKey     /* out*/
);
static CPGenKey_t MyGenKey;

/*------------------------------------------------------*/
/* Определение функции CPExportKey */
typedef BOOL (WINAPI *CPExportKey_t) (
    HCRYPTPROV hProv,      /* in*/
    HCRYPTKEY hKey,        /* in*/
    HCRYPTKEY hPubKey,     /* in*/
    DWORD dwBlobType,      /* in*/
    DWORD dwFlags,         /* in*/
    BYTE *pbData,          /* out*/
    DWORD *pdwDataLen      /* in, out*/
);
static CPExportKey_t MyExportKey;

/*------------------------------------------------------*/
/* Определение функции CPImportKey */
typedef BOOL (WINAPI *CPImportKey_t) (
    HCRYPTPROV hProv,       /* in*/
#ifndef UNIX
    CONST 
#endif /* UNIX */
BYTE *pbData,     /* in*/
    DWORD  dwDataLen,       /* in*/
    HCRYPTKEY hPubKey,      /* in*/
    DWORD dwFlags,          /* in*/
HCRYPTKEY *phKey        /* out*/
);
static CPImportKey_t MyImportKey;

/*------------------------------------------------------*/
/* Определение функции CPGetKeyParam*/
typedef BOOL (WINAPI *CPGetKeyParam_t) (
    HCRYPTPROV hProv,       /* in*/
    HCRYPTKEY hKey,         /* in*/
    DWORD dwParam,          /* in*/
    BYTE *pbData,           /* out*/
    DWORD *pdwDataLen,      /* in, out*/
    DWORD dwFlags           /* in*/
);
static CPGetKeyParam_t MyGetKeyParam;

/*------------------------------------------------------*/
/* Определение функции CPSetKeyParam*/
typedef BOOL (WINAPI *CPSetKeyParam_t) (
    HCRYPTPROV hProv,       /* in*/
    HCRYPTKEY hKey,         /* in*/
    DWORD dwParam,          /* in*/
    BYTE *pbData,           /* in*/
    DWORD dwFlags           /* in*/
);
static CPSetKeyParam_t MySetKeyParam;

/*------------------------------------------------------*/
/* Определение функции CPCreateHash */
typedef BOOL (WINAPI *CPCreateHash_t) (
    HCRYPTPROV hProv,    /* in*/
    ALG_ID Algid,        /* in*/
    HCRYPTKEY hKey,      /* in*/
    DWORD dwFlags,       /* in*/
    HCRYPTHASH *phHash   /* out*/
);
static CPCreateHash_t MyCreateHash;

/*------------------------------------------------------*/
/* Определение функции CPGetHashParam */
typedef BOOL (WINAPI *CPGetHashParam_t) (
    HCRYPTPROV hProv,        /* in*/
    HCRYPTHASH hHash,        /* in*/
    DWORD dwParam,           /* in*/
    BYTE *pbData,            /* out*/
    DWORD *pdwDataLen,       /* in, out*/
    DWORD dwFlags            /* in*/
);
static CPGetHashParam_t MyGetHashParam;

/*------------------------------------------------------*/
/* Определение функции CPDestroyHash */
typedef BOOL (WINAPI *CPDestroyHash_t) (
    HCRYPTPROV hProv, /* in*/
    HCRYPTHASH hHash  /* in*/
);
static CPDestroyHash_t MyDestroyHash;

/*------------------------------------------------------*/
/* Определение функции CPHashData */
typedef BOOL (WINAPI *CPHashData_t) (
    HCRYPTPROV hProv,       /* in*/
    HCRYPTHASH hHash,       /* in*/
#ifndef UNIX
    CONST 
#endif /* UNIX */
BYTE *pbData,     /* in*/
    DWORD dwDataLen,        /* in*/
    DWORD dwFlags           /* in*/
);
static CPHashData_t MyHashData;

/*------------------------------------------------------*/
/* Определение функции CPSignHash */
typedef BOOL (WINAPI *CPSignHash_t) (
    HCRYPTPROV hProv,           /* in*/
    HCRYPTHASH hHash,           /* in*/
    DWORD dwKeySpec,            /* in*/
    LPCWSTR sDescription,       /* in*/
    DWORD dwFlags,              /* in*/
    BYTE *pbSignature,          /* out*/
    DWORD *pdwSigLen            /* in, out*/
);
static CPSignHash_t MySignHash;

/*------------------------------------------------------*/
/* Определение функции CPVerifySignature */
typedef BOOL (WINAPI *CPVerifySignature_t) (
    HCRYPTPROV hProv,         /* in*/
    HCRYPTHASH hHash,         /* in*/
#ifndef UNIX
    CONST
#endif /* UNIX */
    BYTE *pbSignature,  /* in*/
    DWORD dwSigLen,           /* in*/
    HCRYPTKEY hPubKey,        /* in*/
    LPCWSTR sDescription,     /* in*/
    DWORD dwFlags             /* in*/
);
static CPVerifySignature_t MyVerifySignature;

/*------------------------------------------------------*/
/* Определение функции CPDestroyKey */
typedef BOOL (WINAPI *CPDestroyKey_t) (
    HCRYPTPROV hProv,  /* in*/
    HCRYPTKEY  hKey    /* in*/
);
static CPDestroyKey_t MyDestroyKey;

/*------------------------------------------------------*/
/* Определение функции CPReleaseContext */
typedef BOOL (WINAPI *CPReleaseContext_t) (
    HCRYPTPROV hProv,  /* in*/
    DWORD dwFlags      /* in*/
);
static CPReleaseContext_t MyReleaseContext;

/*------------------------------------------------------*/
int show_info (HCRYPTPROV hProv, HCRYPTKEY hKeyS, HCRYPTKEY hKeyE);

static BOOL CopyKeyParam( HCRYPTPROV hSrcProv, HCRYPTPROV hDestProv, 
    DWORD param, HCRYPTKEY hSrcKey, HCRYPTKEY hDestKey );
static BOOL CopyPrivateKey( HCRYPTPROV hSrc, HCRYPTPROV hDest, 
    HCRYPTKEY hKey );
static BOOL CopyContainerParam( HCRYPTPROV hSrc, HCRYPTPROV hDest );
static BOOL CopyContainerParam1( HCRYPTPROV hSrc, HCRYPTPROV hDest, 
    DWORD pp );
static char *GetContainerName( int cplevel, PVTABLEPROVSTRUC st,
    char * szProvider, DWORD dwProvType );

#ifdef UNIX
#define UniSetProvParam(hP,p1,p2,p3) (MySetProvParam(hP,p1,p2,p3))
#define UniGetProvParam(hP,p1,p2,p3,p4) (MyGetProvParam(hP,p1,p2,p3,p4))
#define UniGetUserKey(hP,p1,p2) (MyGetUserKey(hP,p1,p2))
#define UniGenKey(hP,p1,p2,p3) (MyGenKey(hP,p1,p2,p3))
#define UniSignHash(hP,p1,p2,p3,p4,p5,p6) (MySignHash(hP,p1,p2,p3,p4,p5,p6))
#define UniExportKey(hP,p1,p2,p3,p4,p5,p6) (MyExportKey(hP,p1,p2,p3,p4,p5,p6))
#define UniDestroyKey(hP,p1) (MyDestroyKey(hP,p1))
#define UniImportKey(hP,p1,p2,p3,p4,p5) (MyImportKey(hP,p1,p2,p3,p4,p5))
#define UniGetKeyParam(hP,p1,p2,p3,p4,p5) (MyGetKeyParam(hP,p1,p2,p3,p4,p5))
#define UniSetKeyParam(hP,p1,p2,p3,p4) (MySetKeyParam(hP,p1,p2,p3,p4))
#define UniReleaseContext(hP,p1) (MyReleaseContext(hP,p1))
#define UniVerifySignature(hP,p1,p2,p3,p4,p5,p6) (MyVerifySignature(hP,p1,p2,p3,p4,p5,p6))
#else /* UNIX */
#define UniSetProvParam(hP,p1,p2,p3) (CPlevel?MySetProvParam(hP,p1,p2,p3):CryptSetProvParam(hP,p1,p2,p3))
#define UniGetProvParam(hP,p1,p2,p3,p4) (CPlevel?MyGetProvParam(hP,p1,p2,p3,p4):CryptGetProvParam(hP,p1,p2,p3,p4))
#define UniGetUserKey(hP,p1,p2) (CPlevel?MyGetUserKey(hP,p1,p2):CryptGetUserKey(hP,p1,p2))
#define UniGenKey(hP,p1,p2,p3) (CPlevel?MyGenKey(hP,p1,p2,p3):CryptGenKey(hP,p1,p2,p3))
#define UniSignHash(hP,p1,p2,p3,p4,p5,p6) (CPlevel?MySignHash(hP,p1,p2,p3,p4,p5,p6):CryptSignHash(p1,p2,p3,p4,p5,p6))
#define UniExportKey(hP,p1,p2,p3,p4,p5,p6) (CPlevel?MyExportKey(hP,p1,p2,p3,p4,p5,p6):CryptExportKey(p1,p2,p3,p4,p5,p6))
#define UniDestroyKey(hP,p1) (CPlevel?MyDestroyKey(hP,p1):CryptDestroyKey(p1))
#define UniImportKey(hP,p1,p2,p3,p4,p5) (CPlevel?MyImportKey(hP,p1,p2,p3,p4,p5):CryptImportKey(hP,p1,p2,p3,p4,p5))
#define UniGetKeyParam(hP,p1,p2,p3,p4,p5) (CPlevel?MyGetKeyParam(hP,p1,p2,p3,p4,p5):CryptGetKeyParam(p1,p2,p3,p4,p5))
#define UniSetKeyParam(hP,p1,p2,p3,p4) (CPlevel?MySetKeyParam(hP,p1,p2,p3,p4):CryptSetKeyParam(p1,p2,p3,p4))
#define UniReleaseContext(hP,p1) (CPlevel?MyReleaseContext(hP,p1):CryptReleaseContext(hP,p1))
#define UniVerifySignature(hP,p1,p2,p3,p4,p5,p6) (CPlevel?MyVerifySignature(hP,p1,p2,p3,p4,p5,p6):CryptVerifySignature(p1,p2,p3,p4,p5,p6))
#endif /* UNIX */

static void crc16( const char *text, unsigned char *CRC );

#ifdef UNIX
extern int num_passes, num_threads;

#define FN_VERIFY_SIGNATURE 1
#define FN_GET_PROV_PARAM 2
#define FN_STRESS 3

typedef struct pthr_param_ {
  int func;
  HCRYPTPROV hProv;
  HCRYPTHASH hHash;
  BYTE *pb1;
  DWORD dw1;
  DWORD dw2;
  HCRYPTKEY hKey;
  LPCWSTR str;
  char *str1;
} pthr_param;

void *threaded_func(void *p)
{
  pthr_param *ppar=(pthr_param *)p;
  BOOL ret;
  int count;

  for(count=0;count<num_passes;count++){
    switch(ppar->func){
    case FN_VERIFY_SIGNATURE:
      ret=CPVerifySignature(ppar->hProv, ppar->hHash, ppar->pb1, ppar->dw1, 
			    ppar->hKey, ppar->str, ppar->dw2);
      break;
    case FN_GET_PROV_PARAM:
      /*    ret=CPGetPRovParam */
      break;
    case FN_STRESS:{
      HCRYPTPROV hProv;
      VTABLEPROVSTRUC vTable;

      vTable.Version = 1; /* min*/
      vTable.FuncVerifyImage = NULL;
      vTable.FuncReturnhWnd = NULL;
      
      ret=CPAcquireContext(&hProv, ppar->str1, 0, &vTable);
      if(ret){
	ret=CPReleaseContext(hProv,0);
	if(!ret) printf("thread_func: CPReleaseContext: %d\n",ret);
      } else printf("thread_func: CPAcquireContext: %d\n",ret);
      break;
    }
    default:
      break;
    }
  }
  
  return (void*)ret;
}

BOOL do_threaded(pthr_param par)
{
  pthread_t *thread_ids;
  int i, ret = TRUE;
  void *pret;

  printf("Multithreaded: threads %d, passes %d\n",num_threads,num_passes);

  thread_ids=(pthread_t *)malloc(sizeof(pthread_t)*num_threads);
  if(!thread_ids) return FALSE;

  for(i=0;i<num_threads;i++){
    if(pthread_create(thread_ids+i, NULL, &threaded_func, (void*)&par)){
      ret=FALSE;
      goto exit;
    }
  }
  for(i=0;i<num_threads;i++){
    pthread_join(thread_ids[i], &pret);
    if(ret==TRUE) ret=(BOOL)pret;
  }
 exit:
  free(thread_ids);
  
  return ret;
}

BOOL ThrVerifySignature(    
		       HCRYPTPROV hProv,         /* in*/
		       HCRYPTHASH hHash,         /* in*/
		       BYTE *pbSignature,  /* in*/
		       DWORD dwSigLen,           /* in*/
		       HCRYPTKEY hPubKey,        /* in*/
		       LPCWSTR sDescription,     /* in*/
		       DWORD dwFlags             /* in*/
		       )
{
  pthr_param par;
  
  par.func=FN_VERIFY_SIGNATURE;
  par.hProv=hProv;
  par.hHash=hHash;
  par.pb1=pbSignature;
  par.dw1=dwSigLen;
  par.dw2=dwFlags;
  par.hKey=hPubKey;
  par.str=sDescription;

  return do_threaded(par);
}
#endif /* UNIX */

/*------------------------------------------------------*/
/* MAIN */
/*------------------------------------------------------*/
int
PROG (int argc, char **argv)
{
    HCRYPTPROV	hProv = 0;	    /* Дескриптор провайдера*/
    char *	szContainer = NULL; /* Имя ключевого контейнера*/
    char *	szCopyContainer = NULL; /* Имя результирующего ключевого контейнера. */
    char *	szProvider = NULL;  /* Имя провайдера*/
    DWORD dwProvType = PROV_GOST_DH; /*Тип провайдера по умолчанию*/

    HCRYPTKEY	hKeyS = 0;	    /* Дескриптор ключа ЭЦП AT_SIGNATURE*/
    HCRYPTKEY	hKeyE = 0;	    /* Дескриптор ключа обмена AT_EXCHANGE */
    BOOL	bResult = 0;	    /* Код возврата*/
    ALG_ID	Algid;		    /* Идентификатор алгоритма*/
    DWORD	dwFlags = 0;	    /* Флаги*/
     DWORD	dwParam;		    
    BYTE	pbProvName [1024*4];	    
    DWORD	dwProvNameLen;
    BYTE	pbProvContainer [1024*4];
    DWORD	dwProvContainerLen;
    int		exported = 0;
    int		ret = 0;
    int		print_help = 0;
    int		c;
    int		show = 0;
    char	*infilename = NULL;
    char	*outfilename = NULL;
    char	*exportfilename = NULL;
    char	*importfilename = NULL;
    char	*signaturefilename = NULL;
    int		keyType = AT_KEYEXCHANGE | AT_SIGNATURE;
    int		sign = 0;
    int		verify = 0;
#ifdef UNIX
    int stress=0;
#endif /* UNIX */
    char	*ptr = NULL;
    HCRYPTKEY	hImportedKey = 0;
    HCRYPTKEY	hSignatureKey = 0;
    unsigned char *in_data = NULL;
    size_t	in_data_len = 0;
    ALG_ID	my_alg = CALG_GR3411;
    char	*my_hash_alg = NULL;
    HCRYPTHASH	hHash = 0;
    char	*pubkeyfilename = NULL;
    int		hash = 0;
    ALG_ID	hash_alg = CALG_GR3411;
    char	*ptr_hash_alg = NULL;
    char	*hashfile = NULL;
    ALG_ID	some_alg = 0;
#ifndef UNIX
    PCCRYPT_OID_INFO info = NULL;
#endif /* UNIX */
    DWORD	key_length = 0;
    int counter = 0;
    int enum_containers = 0;
    int hard_rng = 0;
    char	*password = NULL;
    char	*new_password = NULL;
    char	*new_encryption_container = NULL;
    int		change_passwd_wnd = 0;
    char	*enc_passwd = NULL;
    char	*enc_passwd2 = NULL;
    int		reboot = 0;
    DWORD	get_param = 0;
    const char  *get_param_text = "";
    char	*set_param = 0;
    DWORD	enum_param = 0;
    const char  *enum_param_text = "";
    int		unique = 0;
    int		fqcn = 0;
    int		crc = 0;

    VTABLEPROVSTRUC vTable;

    /*-----------------------------------------------------------------------------*/
    /* определение опций разбора параметров*/
    static struct option long_options[] = {
	{"cplevel",		no_argument,		NULL, 'l'},
	{"container",		required_argument,	NULL, 'c'},
	{"provider",		required_argument,	NULL, 'p'},
	{"provtype",		required_argument,	NULL, 't'},
	{"verifycontext",	no_argument,		NULL, 'v'},
	{"newkeyset",		no_argument,		NULL, 'n'},
	{"machinekeyset",	no_argument,		NULL, 'm'},
	{"deletekeyset",	no_argument,		NULL, 'd'},
	{"info",	        no_argument,		NULL, 's'},
	{"keytype",		required_argument,	NULL, '1'},
	{"export",	        required_argument,	NULL, '2'},
	{"import",	        required_argument,	NULL, '3'},
	{"sign",	        required_argument,	NULL, '4'},
	{"verify",	        required_argument,	NULL, '5'},
	{"in",			required_argument,	NULL, '6'},
	{"out",			required_argument,	NULL, '7'},
	{"signature",		required_argument,	NULL, '8'},
	{"pubkey",		required_argument,	NULL, '9'},
	{"hash",		required_argument,	NULL, 'a'},
	{"hashout",		required_argument,	NULL, 'b'},
	{"alg",			required_argument,	NULL, 'f'},
	{"calg",		required_argument,	NULL, 'x'},
	{"enum_param",		required_argument,	NULL, 'E'},
	{"get_param",		required_argument,	NULL, 'P'},
	{"set_param",		required_argument,	NULL, 'T'},
	{"hard_rng",		no_argument,		NULL, 'k'},
	{"general",		no_argument,		NULL, 'G'},
#if(_WIN32_WINNT >= 0x0500)
	{"silent",		no_argument,		NULL, 'i'},
#endif /*(_WIN32_WINNT >= 0x0500)*/
	{"exported",	no_argument,		NULL, 'e'},
	{"length",	required_argument,	NULL, 'g'},
	{"enum_containers", no_argument,	NULL, 'j'},
	{"passwd",	required_argument,	NULL, 'w'},
	{"chpasswd",	required_argument,	NULL, '0'},
	{"qchpasswd",	no_argument,		NULL, 'Q'},
	{"enchpasswd",  required_argument,	NULL, 'H'},
	{"enc_passwd",  required_argument,	NULL, '_'},
	{"enc_passwd2",  required_argument,	NULL, '+'},
	{"counter",	required_argument,	NULL, 'u'},
	{"reboot",	no_argument,		NULL, 'R'},
	{"copy",	required_argument,	NULL, 'C'},
	{"unique",	no_argument,		NULL, 'q'},
	{"fqcn",	no_argument,		NULL, '\\'},
	{"crc",		no_argument,		NULL, 'r'},
#ifdef UNIX
	{"stress",	no_argument,		NULL, 'S'},
#endif /* UNIX */
	{"help",	no_argument,		NULL, 'h'},
	{0, 0, 0, 0}
    };

    /*-----------------------------------------------------------------------------*/
    /* разбор параметров*/
    while ((c = getopt_long_only (argc, argv, "", long_options, (int *)0)) != EOF) {
	switch (c) {
#ifndef UNIX
	case 'f':
	    some_alg = atol (optarg);

		some_alg = CALG_GR3411;

	    info = CryptFindOIDInfo(CRYPT_OID_INFO_ALGID_KEY,
		(void*) &some_alg, CRYPT_HASH_ALG_OID_GROUP_ID);
	    if (info) {
		printf ("Name: %S  OID: %s\n", info->pwszName, info->pszOID);
	    }
	    break;
	case 'x':
		printf ("alg: %s\n", optarg);
	    info = CryptFindOIDInfo(CRYPT_OID_INFO_OID_KEY, optarg,
		CRYPT_HASH_ALG_OID_GROUP_ID);
	    if (info) {
		printf ("Name: %S  OID: %s %d %d\n", info->pwszName,
		    info->pszOID, info->Algid);
	    }
	    break;
#endif /* UNIX */
	case 'l':
	    CPlevel = 1;
	    break;
	case 's':
	    show = 1;
	    break;
	case 'c':
	    szContainer = (char *)optarg;
	    break;
	case 'q':
	    unique = 1;
	    break;
	case 'a':
	    hash = 1;
	    ptr_hash_alg = optarg;
	    if (strcmp(ptr_hash_alg, "SHA1") == 0)
		hash_alg = CALG_SHA1;
	    else if (strcmp(ptr_hash_alg, "GOST") == 0)
		hash_alg = CALG_GR3411;
	    else if (strcmp(ptr_hash_alg, "MD2") == 0)
		hash_alg = CALG_MD2;
	    else if (strcmp(ptr_hash_alg, "MD5") == 0)
		hash_alg = CALG_MD5;
	    else {
		print_help = 1;
		goto bad;
	    }
	    break;
	case 'b':
	    hashfile = optarg;
	    break;
#ifndef UNIX
	case 'p':
	    szProvider = abbr2provider (optarg);
	    break;
	case 't':
	    dwProvType = abbr2provtype (optarg);
	    break;
#endif /* UNIX */
	case 'v':
	    dwFlags |= CRYPT_VERIFYCONTEXT;
	    break;
	case 'n':
	    dwFlags |= CRYPT_NEWKEYSET;
	    break;
	case 'm':
	    dwFlags |= CRYPT_MACHINE_KEYSET;
	    break;
	case 'd':
	    dwFlags |= CRYPT_DELETEKEYSET;
	    break;
	case 'G':
	    dwFlags |= CRYPT_GENERAL;
	    break;
	case '1':
	    ptr = optarg;
	    if (strcmp(ptr, "signature") == 0)
		keyType = AT_SIGNATURE;
	    else if (strcmp(ptr, "exchange") == 0)
		keyType = AT_KEYEXCHANGE;
	    else if (strcmp(ptr, "none") == 0)
		keyType = 0;
	    else {
		print_help = 1;
		goto bad;
	    }
	    break;
	case '2':
	    exportfilename = optarg;
	    break;
	case '3':
	    importfilename = optarg;
	    break;
	case '4':
	    sign = 1;
	    my_hash_alg = optarg;
	    if (strcmp(my_hash_alg, "SHA1") == 0)
		my_alg = CALG_SHA1;
	    else if (strcmp(my_hash_alg, "GOST") == 0)
		my_alg = CALG_GR3411;
	    else if (strcmp(my_hash_alg, "MD2") == 0)
		my_alg = CALG_MD2;
	    else if (strcmp(my_hash_alg, "MD5") == 0)
		my_alg = CALG_MD5;
	    else {
		print_help = 1;
		goto bad;
	    }
	    break;
	case '5':
	    verify = 1;
	    my_hash_alg = optarg;
	    if (strcmp(my_hash_alg, "SHA1") == 0)
		my_alg = CALG_SHA1;
	    else if (strcmp(my_hash_alg, "GOST") == 0)
		my_alg = CALG_GR3411;
	    else if (strcmp(my_hash_alg, "MD2") == 0)
		my_alg = CALG_MD2;
	    else if (strcmp(my_hash_alg, "MD5") == 0)
		my_alg = CALG_MD5;
	    else {
		print_help = 1;
		goto bad;
	    }
	    break;
	case '6':
	    infilename = optarg;
	    break;
	case '7':
	    outfilename = optarg;
	    break;
	case '8':
	    signaturefilename = optarg;
	    break;
	case '9':
	    pubkeyfilename = optarg;
	    break;
	case 'k':
	    hard_rng = 1;
	    break;
#if(_WIN32_WINNT >= 0x0500)
	case 'i':
	    dwFlags |= CRYPT_SILENT;
	    break;
#endif /*(_WIN32_WINNT >= 0x0500)*/
	case 'e':
	    exported = 1;
	    break;
	case 'h':
	    ret = 1;
	    print_help = 1;
	    goto bad;
	case 'g':
	    key_length = atol (optarg);
	    if( key_length != 1024 && key_length != 512 )
		goto bad;
	    break;
	case 'u':
	    counter = atol (optarg);
	    break;
	case 'j':
	    enum_containers = 1;
	    break;
	case 'w':
	    password = optarg;
	    break;
	case '0':
	    new_password = optarg;
	    break;
	case 'Q':
	    change_passwd_wnd = 1;
	    break;
	case 'H':
	    new_encryption_container = optarg;
	    break;
	case '_':
	    enc_passwd = optarg;
	    break;
	case '+':
	    enc_passwd2 = optarg;
	    break;
	case 'R':
	    reboot = 1;
	    break;
	case 'C':
	    szCopyContainer = (char *)optarg;
	    break;
#ifdef UNIX
	case 'S':
	    stress = 1;
	    break;
#endif /* UNIX */
	case 'E':
	    {
		char *arg = (char *)optarg;
		if( !strcmp( arg, "hash" ) )
		{
		    enum_param = PP_ENUM_HASHOID;
		    enum_param_text = "PP_ENUM_HASHOID";
		}
		else if( !strcmp( arg, "cipher" ) )
		{
		    enum_param = PP_ENUM_CIPHEROID;
		    enum_param_text = "PP_ENUM_CIPHEROID";
		}
		else if( !strcmp( arg, "sign" ) )
		{
		    enum_param = PP_ENUM_SIGNATUREOID;
		    enum_param_text = "PP_ENUM_SIGNATUREOID";
		}
		else if( !strcmp( arg, "dh" ) )
		{
		    enum_param = PP_ENUM_DHOID;
		    enum_param_text = "PP_ENUM_DHOID";
		}
		else
		    goto bad;
		break;
	    }
	case 'P':
	    {
		char *arg = (char *)optarg;
		if( !strcmp( arg, "hash" ) )
		{
		    get_param = PP_HASHOID;
		    get_param_text = "PP_HASHOID";
		}
		else if( !strcmp( arg, "cipher" ) )
		{
		    get_param = PP_CIPHEROID;
		    get_param_text = "PP_CIPHEROID";
		}
		else if( !strcmp( arg, "sign" ) )
		{
		    get_param = PP_SIGNATUREOID;
		    get_param_text = "PP_SIGNATUREOID";
		}
		else if( !strcmp( arg, "dh" ) )
		{
		    get_param = PP_DHOID;
		    get_param_text = "PP_DHOID";
		}
		else
		    goto bad;
		break;
	    }
	case 'T':
	    set_param = (char *)optarg;
	    break;
	case '\\':
	    fqcn = 1;
	    break;
	case 'r':
	    crc = 1;
	    break;
	case '?':
	default:
	    goto bad;
	}
    }
    if (c != EOF) {
	print_help = 1;
	goto bad;
    }
    if( crc )
    {
	unsigned char crc[2] = { 0, 0 };
	if( szContainer == NULL )
	{
	    printf("-container must be set.\n");
	    goto bad;
	}
	crc16( szContainer, crc );
	printf( "set crc=%02X%02X", (int)crc[0], (int)crc[1] );
	return 1;
    }

#ifdef UNIX
    if(stress){
      pthr_param par;
  
      par.func=FN_STRESS;
      par.str1=szContainer;

      return do_threaded(par);
    }
#endif /* UNIX */

#ifndef UNIX
    /*---------------------------------------------------------*/
    /* Проверим уровень использования функций криптопровайдера.*/
    if (CPlevel) {
	/* Если непосредственно используем криптопровайдер,
	 * загрузим соответствующий модуль.*/
	hCSPDLL = load_CSP_library (dwProvType, NULL);
	if (! hCSPDLL)
	    HandleErrorFL ("Cannot load CSP DLL.\n");
    }
#endif /* UNIX */

    if (CPlevel) {
	char *szContainerName = NULL;

	/* Для последующего использования CPAcquireContext нужно
	 * определить структуру. Подробнее см. CPAcquireContext */
	vTable.Version = 1; /* min*/
	vTable.FuncVerifyImage = NULL;
	vTable.FuncReturnhWnd = NULL;

#ifdef UNIX
	MyAcquireContext=&CPAcquireContext;
	MyReleaseContext=&CPReleaseContext;
	MyGetProvParam=&CPGetProvParam;
	MySetProvParam=&CPSetProvParam;
	MyGetUserKey=&CPGetUserKey;
	MyGetKeyParam=&CPGetKeyParam;
	MySetKeyParam=&CPSetKeyParam;
	MyExportKey=&CPExportKey;
	MyImportKey=&CPImportKey;
	MyGenKey=&CPGenKey;
	MyDestroyKey=&CPDestroyKey;
	MyCreateHash=&CPCreateHash;
	MyHashData=&CPHashData;
	MySignHash=&CPSignHash;
	MyVerifySignature=&ThrVerifySignature;
	MyGetHashParam=&CPGetHashParam;
	MyDestroyHash=&CPDestroyHash;
#else /* UNIX */
        MyAcquireContext = (CPAcquireContext_t) GetProcAddress (hCSPDLL,"CPAcquireContext");
        MyReleaseContext = (CPReleaseContext_t) GetProcAddress (hCSPDLL,"CPReleaseContext");
        MyGetProvParam = (CPGetProvParam_t) GetProcAddress (hCSPDLL,"CPGetProvParam");
        MySetProvParam = (CPSetProvParam_t) GetProcAddress (hCSPDLL,"CPSetProvParam");
        MyGetUserKey = (CPGetUserKey_t) GetProcAddress (hCSPDLL,"CPGetUserKey");
        MyGetKeyParam = (CPGetKeyParam_t) GetProcAddress (hCSPDLL,"CPGetKeyParam");
        MySetKeyParam = (CPSetKeyParam_t) GetProcAddress (hCSPDLL,"CPSetKeyParam");
	MyExportKey = (CPExportKey_t) GetProcAddress (hCSPDLL,"CPExportKey");
        MyImportKey = (CPImportKey_t) GetProcAddress (hCSPDLL,"CPImportKey");
        MyGenKey = (CPGenKey_t) GetProcAddress (hCSPDLL,"CPGenKey");
        MyDestroyKey = (CPDestroyKey_t) GetProcAddress (hCSPDLL,"CPDestroyKey");
        MyCreateHash = (CPCreateHash_t) GetProcAddress (hCSPDLL,"CPCreateHash");
        MyHashData = (CPHashData_t) GetProcAddress (hCSPDLL,"CPHashData");
        MySignHash = (CPSignHash_t) GetProcAddress (hCSPDLL,"CPSignHash");
        MyVerifySignature = (CPVerifySignature_t) GetProcAddress (hCSPDLL,"CPVerifySignature");
        MyGetHashParam = (CPGetHashParam_t) GetProcAddress (hCSPDLL,"CPGetHashParam");
        MyDestroyHash = (CPDestroyHash_t) GetProcAddress (hCSPDLL,"CPDestroyHash");
#endif /* UNIX */

	if (!MyAcquireContext || !MyReleaseContext || !MyDestroyKey || !MyCreateHash || !MySignHash
	    || !MyHashData || !MyVerifySignature || !MyGetProvParam || !MyImportKey || !MyExportKey
	    || !MyGetKeyParam || !MyGetHashParam || !MyDestroyHash || !MySetProvParam)
	    HandleErrorFL ("Cannot load CSP functions.\n");
	/* Инициализируем контекст криптопровайдера через функции CP*/
	if( szContainer && !strcmp( szContainer, "default" )
	    && !( dwFlags & CRYPT_NEWKEYSET )
	    && !( dwFlags & CRYPT_VERIFYCONTEXT )
	    && dwProvType == PROV_GOST_DH )
	    szContainerName = GetContainerName( CPlevel, &vTable, 
		szProvider, dwProvType );
	bResult = MyAcquireContext (&hProv, 
	    szContainerName ? szContainerName : szContainer, dwFlags, &vTable);
	if( szContainerName )
	    free( szContainerName );
    } else {
#ifndef UNIX
	/* Инициализируем контекст криптопровайдера через функции CryptoAPI 1.0*/
	int i;
	char *szContainerName = NULL;

	if( szContainer && !strcmp( szContainer, "default" )
	    && !( dwFlags & CRYPT_NEWKEYSET )
	    && !( dwFlags & CRYPT_VERIFYCONTEXT )
	    && dwProvType == PROV_GOST_DH )
	    szContainerName = GetContainerName( CPlevel, NULL, szProvider,
		dwProvType );
	if (counter < 0) {
	    CryptAcquireContext (&hProv, szContainer, szProvider,
		dwProvType, dwFlags);
	    counter = -counter;
	}
	if (!counter)
	    counter = 1;
	for (i=0; i<counter; i++) {
	    /* Инициализируем контекст криптопровайдера через функции CryptoAPI 1.0*/
	    bResult = CryptAcquireContext (&hProv, 
		szContainerName ? szContainerName : szContainer, szProvider,
		dwProvType, dwFlags);
	    if (counter > 1) {
		printf (".");
		if (i%70 == 69 || i == counter - 1)
		    printf ("\n");
	    }
	    if (i < counter - 1 && bResult)
		CryptReleaseContext (hProv, 0);
	}
	if (counter > 1) {
	    printf ("Wait for 10 sec before continue...\n");
	    Sleep (1000 * 10);
	    printf ("\n");
	}
	if( szContainerName )
	    free( szContainerName );
#endif /* UNIX */
    }

    if (bResult) {
	printf ("CryptAcquireContext succeeded.HCRYPTPROV: %lu\n", hProv);
    } else {
	HandleErrorFL ("Error during CryptAcquireContext.\n");
	goto bad;
    }
    if (hard_rng) {
	bResult = UniSetProvParam(hProv, PP_USE_HARDWARE_RNG, NULL, 0);
	if( !bResult )
	{
	    HandleErrorFL ("Error during CryptSetProvParam(PP_USE_HARDWARE_RNG).\n");
	    goto bad;
	}
    }
    if (enc_passwd) {
	CRYPT_CHANGE_PIN_PARAM set_pin;

	set_pin.type = CRYPT_CHANGE_PIN_CONTAINER;
	set_pin.dest.passwd = enc_passwd;
	bResult = UniSetProvParam( hProv, PP_ENCRYPTION_CONTAINER, 
	    (BYTE*)&set_pin, 0 );
	if( !bResult )
	{
	    HandleErrorFL ("Error during UniSetProvParam.\n");
	    goto bad;
	}
    }
    if (enc_passwd2) {
	HCRYPTPROV hEncProv;
	HCRYPTPROV hEncProvValue;
	CRYPT_CHANGE_PIN_PARAM set_pin;

	DWORD len = sizeof( HCRYPTPROV );
	if (CPlevel)
	    bResult = MyAcquireContext (&hEncProv, enc_passwd2, 
		dwFlags & ~CRYPT_NEWKEYSET, &vTable);
#ifndef UNIX
	else
	    bResult = CryptAcquireContext (&hEncProv, enc_passwd2, szProvider,
		dwProvType, dwFlags & ~CRYPT_NEWKEYSET);
#endif /* UNIX */
	if( !bResult )
	{
	    HandleErrorFL ("Error during CryptAcquireContext.\n");
	    goto bad;
	}
	if (password) {
	    DWORD flags = 0;
	    
	    bResult = UniSetProvParam(hEncProv, PP_KEYEXCHANGE_PIN,
		(BYTE*)password, flags);
	    if( !bResult )
	    {
		HandleErrorFL ("Error during CryptSetProvParam(PP_KEYEXCHANGE_PIN).\n");
		goto bad;
	    }
	    password = NULL;
	}
	bResult = UniGetProvParam(hEncProv,PP_HCRYPTPROV,
	    (BYTE*)&hEncProvValue, &len, 0 );
	if( !bResult )
	{
	    UniReleaseContext( hEncProv, 0 );
	    HandleErrorFL ("Error during CryptGetProvParam.\n");
	    goto bad;
	}
	set_pin.type = CRYPT_CHANGE_PIN_HANDLE_CONTAINER;
	set_pin.dest.prov = hEncProvValue;
	bResult = UniSetProvParam( hProv, PP_ENCRYPTION_CONTAINER, 
	    (BYTE*)&set_pin, 0 );
	if( !bResult )
	{
	    HandleErrorFL ("Error during UniSetProvParam.\n");
	    goto bad;
	}
    }
    if (password) {
	DWORD flags = 0;

	bResult = UniSetProvParam(hProv, PP_KEYEXCHANGE_PIN,
	    (BYTE*)password, flags);
	if( !bResult )
	{
	    HandleErrorFL ("Error during CryptSetProvParam(PP_KEYEXCHANGE_PIN).\n");
	    goto bad;
	}
    }
    if (new_password) {
	DWORD flags = 0;
	CRYPT_CHANGE_PIN_PARAM param;

	param.type = CRYPT_CHANGE_PIN_PASSWD;
	param.dest.passwd = new_password;
	bResult = UniSetProvParam(hProv, PP_CHANGE_PIN,
	    (BYTE*)&param, flags);
	if( !bResult )
	{
	    HandleErrorFL ("Error during CryptGetProvParam(PP_CHANGE_PIN).\n");
	    goto bad;
	}
    }
    if (change_passwd_wnd) {
	DWORD flags = 0;
	CRYPT_CHANGE_PIN_PARAM param;

	param.type = CRYPT_CHANGE_PIN_WINDOW;
	bResult = UniSetProvParam(hProv, PP_CHANGE_PIN,
	    (BYTE*)&param, flags);
	if( !bResult )
	{
	    HandleErrorFL ("Error during CryptGetProvParam(PP_CHANGE_PIN).\n");
	    goto bad;
	}
    }
    if (new_encryption_container) {
	DWORD flags = 0;
	CRYPT_CHANGE_PIN_PARAM param;

	param.type = CRYPT_CHANGE_PIN_CONTAINER;
	param.dest.passwd = new_encryption_container;
	bResult = UniSetProvParam(hProv, PP_CHANGE_PIN,
	    (BYTE*)&param, flags);
	if( !bResult )
	{
	    HandleErrorFL ("Error during CryptGetProvParam(PP_CHANGE_PASSWORD).\n");
	    goto bad;
	}
    }
    if (enum_param) {
	char *oid;
	DWORD length = 0;
	DWORD base_flags = CRYPT_FIRST;
	char t[80];

	bResult = UniGetProvParam(hProv, enum_param, NULL, 
	    &length, CRYPT_FIRST);
	if( !bResult )
	{
	    sprintf (t, "Error during CryptGetProvParam(%s).\n",
		enum_param_text );
	    HandleErrorFL( t );
	    goto bad;
	}
	oid = (char*)malloc( length );
	if( !oid )
	{
	    sprintf (t, "Error during CryptGetProvParam(%s).\n",
		enum_param_text );
	    HandleErrorFL( t );
	    goto bad;
	}
	printf( "%s:\n", enum_param_text );
	while( UniGetProvParam( hProv, enum_param, 
	    (BYTE*)oid, 
	    &length, base_flags ) )
	{
	    printf( "  %s\n", oid ); 
	    base_flags &= ~CRYPT_FIRST;
	}
	free( oid );
    }
    if (get_param) {
	char *oid;
	DWORD length = 0;
	char t[80];
	bResult = UniGetProvParam(hProv, get_param, NULL, 
	    &length, 0);
	if( !bResult )
	{
	    sprintf (t, "Error during CryptGetProvParam(%s).\n",
		get_param_text );
	    HandleErrorFL( t );
	    goto bad;
	}
	oid = (char*)malloc( length );
	if( !oid )
	{
	    sprintf (t, "Error during CryptGetProvParam(%s).\n",
		get_param_text );
	    HandleErrorFL( t );
	    goto bad;
	}
	bResult = UniGetProvParam(hProv, get_param, (BYTE*)oid, 
	    &length, 0);
	if( !bResult )
	{
	    free( oid );
	    sprintf (t, "Error during CryptGetProvParam(%s).\n",
		get_param_text );
	    HandleErrorFL( t );
	    goto bad;
	}
	printf( "%s:%s\n", get_param_text, oid );
	free( oid );
    }
    if (set_param)
    {
	char t[80];
	bResult = UniSetProvParam(hProv, get_param, (BYTE*)set_param, 0 );
	if( !bResult )
	{
	    sprintf (t, "Error during CryptSetProvParam(%s,%s).\n",
		get_param_text, set_param );
	    HandleErrorFL( t );
	    goto bad;
	}
	printf( "%s:%s\n", get_param_text, set_param );
    }
    if (enum_containers) {
	DWORD length = 0;
	char *container_name;

	DWORD base_flags = 
	    dwFlags & CRYPT_MACHINE_KEYSET ? CRYPT_MACHINE_KEYSET : 0;
	base_flags |= CRYPT_FIRST;
	if( unique )
	    base_flags |= CRYPT_UNIQUE;
	if( fqcn )
	    base_flags |= CRYPT_FQCN;
	
	bResult = UniGetProvParam(hProv, PP_ENUMCONTAINERS, NULL, 
	    &length, base_flags);
	container_name = (char*)malloc( length );
	if( !container_name )
	{
	    HandleErrorFL ("Error during CryptGetProvParam(PP_ENUMCONTAINERS).\n");
	    goto bad;
	}
	while( UniGetProvParam( hProv, PP_ENUMCONTAINERS, 
	    (BYTE*)container_name, 
	    &length, base_flags ) )
	{
	    if( base_flags & CRYPT_UNIQUE )
	    {
		if( strlen( container_name ) > 38 
		    || strlen( container_name + strlen( container_name ) + 1 )
		    > 38 )
		{
		    printf( "%38s|\n%76s\n", container_name,
			container_name + strlen( container_name ) + 1 );
		}
		else
		    printf( "%38s|%38s\n", container_name, 
			container_name + strlen( container_name ) + 1 );
	    }
	    else
		printf( "%s\n", container_name );
	    base_flags &= ~CRYPT_FIRST;
	}
	free( container_name );
	if( GetLastError() != ERROR_NO_MORE_ITEMS )
	{
	    UniReleaseContext (hProv, 0);
	    HandleErrorFL ("Error during GetProvParam.\n");
	    goto bad;
	}
	printf( "OK.\n" );
	ret = 1;
	bResult = UniReleaseContext (hProv, 0);
	if (!bResult) {
	    HandleErrorFL ("Error during CryptReleaseContext.\n");
	    goto bad;
	}
	if( reboot )
	    cpcsp_reboot( hCSPDLL );
#ifndef UNIX
	if( hCSPDLL ) FreeLibrary( hCSPDLL );
#endif /* UNIX */
	return ret;
    }

    /* Если был установлен флаг удаления ключевого контейнера, выводим сообщение.*/
    if (dwFlags & CRYPT_DELETEKEYSET) {
	printf ("Container %s deleted.\n",
	    szContainer ? szContainer : "(default)");
	ret = 1;
	goto bad;
    }
    /*-----------------------------------------------------------------------------*/
    /* Определим полное имя провайдера*/
    dwParam = PP_NAME;
    dwProvNameLen = sizeof (pbProvName);

    bResult = UniGetProvParam (hProv, dwParam, pbProvName, &dwProvNameLen, 0);
    if (bResult) {
	printf ("CryptGetProvParam succeeded.\n");
	printf ("Provider name: %s\n", pbProvName);
    } else {
	HandleErrorFL ("Error reading CSP name.\n");
	goto bad;
    }

    /*-----------------------------------------------------------------------------*/
    /* Определим имя ключевого контейнера*/
    dwProvContainerLen = sizeof (pbProvContainer);
    if( unique )
	dwParam = PP_UNIQUE_CONTAINER;
    else
	dwParam = PP_CONTAINER;
    bResult = UniGetProvParam (hProv, dwParam, pbProvContainer, &dwProvContainerLen, 0);
    if (bResult) {
	printf ("A crypto context has been acquired and \n");
	printf ("The name on the key container is \"%s\"\n\n", pbProvContainer);
    } else {
	HandleErrorFL("A context was acquired or created, but an error occurred getting the key container name.\n");
	goto bad;
    }

    /*-----------------------------------------------------------------------------*/
    /* Определим наличие ключа ЭЦП*/
    if( !hash && (keyType & AT_SIGNATURE) && !(dwFlags & CRYPT_VERIFYCONTEXT) )
    {
        bResult = UniGetUserKey (hProv, AT_SIGNATURE, &hKeyS);
	if (bResult) {
	    printf ("A signature key is available. HCRYPTKEY: %lu\n", hKeyS);
	}
	else {
	    printf ("No signature key is available.\n");
	    /*-----------------------------------------------------------------------------*/
	    /* Если ключа ЭЦП нет, создадим*/
	    if (GetLastError () == NTE_NO_KEY) {
		printf ("The signature key does not exist.\n");
		printf ("Create a signature key pair.\n");
		
		Algid = AT_SIGNATURE;
		bResult = UniGenKey (hProv, Algid, key_length << 16
		    | ( exported ? CRYPT_EXPORTABLE : 0 ), &hKeyS);
		if (bResult)
		    printf ("Created a signature key pair.\n");
		else {
		    HandleErrorFL ("Error occurred creating a signature key.\n");
		    goto bad;
		}
	    } else {
		HandleErrorFL ("An error other than NTE_NO_KEY getting signature key.\n");
		goto bad;
	    }
	    printf ("A signature key was created.\n");
	}
    }

    /*-----------------------------------------------------------------------------*/
    /* Определим наличие ключа обмена*/
    if( !hash && (keyType & AT_KEYEXCHANGE) && !(dwFlags & CRYPT_VERIFYCONTEXT) )
    {
        bResult = UniGetUserKey (hProv, AT_KEYEXCHANGE, &hKeyE);
	if (bResult)
	    printf ("\nAn exchange key exists. HCRYPTKEY: %lu\n", hKeyE);
	else {
	    printf ("\nNo exchange key is available.\n");
	    /*-----------------------------------------------------------------------------*/
	    /* Если ключа обмена нет, создадим*/
	    if (GetLastError () == NTE_NO_KEY) { 
		printf ("The exchange key does not exist.\n");
		printf ("Attempting to create an exchange key pair.\n");
		
		Algid = AT_KEYEXCHANGE;
		
		bResult = UniGenKey (hProv, Algid, key_length << 16
		    | ( exported ? CRYPT_EXPORTABLE : 0 ), &hKeyE);
		if (bResult)
		    printf ("Exchange key pair created.\n");
		else {
		    HandleErrorFL ("Error occurred attempting to create an exchange key.\n");
		    goto bad;
		}
	    } else {
		HandleErrorFL ("An error other than NTE_NO_KEY occurred.\n");
		goto bad;
	    }
	    printf ("An exchange key pair existed, or one was created.\n\n");
	}
    }

    /*-----------------------------------------------------------------------------*/
    /* Вызовем функцию, если задана опция отображения полной информации о провайдере.*/
    if (show) bResult = show_info (hProv, hKeyS, hKeyE);

    /*-----------------------------------------------------------------------------*/
    /* Дополнительные тесты на экспорт импорт ключа и */
    /* подпись/проверку подписи на них*/
    /*-----------------------------------------------------------------------------*/

    /*-----------------------------------------------------------------------------*/
    /* Экспорт открытого ключа заданного типа в файл, заданный переменной exportfilename*/
    if (exportfilename) {
	DWORD key_len = 0;
	BYTE *out_key = NULL;
	PUBLICKEYSTRUC str;
        /*-----------------------------------------------------------------------------*/
	/* Экспорт ключа ЭЦП*/
	if (keyType == AT_SIGNATURE) {
            ret = UniExportKey(hProv, hKeyS,0,PUBLICKEYBLOB,0,NULL,&key_len);

    	    if (!ret)
		HandleErrorFL ("Error occurred when exporting key.\n");
	    out_key = (BYTE*) malloc(key_len);
	    if (!out_key) 
		HandleErrorFL ("Error occurred when exporting key.\n");
	    ret = UniExportKey(hProv, hKeyS,0,PUBLICKEYBLOB,0,out_key,&key_len);
    	    if (!ret)
		HandleErrorFL ("Error occurred when exporting key.\n");
	    ret = write_file (exportfilename, key_len, out_key);
	    memcpy (&str, out_key, sizeof (PUBLICKEYSTRUC));
	    free (out_key);
	    if (ret) {
		printf ("Public key ALG_ID: %u %s was exported into file %s\n", str.aiKeyAlg, "AT_SIGNATURE", exportfilename);
	    }
	}
        /*-----------------------------------------------------------------------------*/
	/* Экспорт ключа обмена*/
	else if (keyType == AT_KEYEXCHANGE) {
	    ret = UniExportKey (hProv, hKeyE,0,PUBLICKEYBLOB,0,NULL,&key_len);
    	    if (!ret)
		HandleErrorFL ("Error occurred when exporting key.\n");
	    out_key = (BYTE*) malloc(key_len);
	    if (!out_key) 
		HandleErrorFL ("Error occurred when exporting key.\n");
	    ret = UniExportKey (hProv, hKeyE,0,PUBLICKEYBLOB,0,out_key,&key_len);
    	    if (!ret)
		HandleErrorFL ("Error occurred when exporting key.\n");
	    ret = write_file (exportfilename, key_len, out_key);
	    memcpy (&str, out_key, sizeof (PUBLICKEYSTRUC));
	    free (out_key);
	    if (ret)
		printf ("Public key ALG_ID: %u %s was exported into file %s\n\n", str.aiKeyAlg, "AT_KEYEXCHANGE", exportfilename);
	}
	else
	    HandleErrorFL ("No keytype specified.\n" );
    }

    /*-----------------------------------------------------------------------------*/
    /* Импорт открытого ключа из файла.*/
    if (importfilename) {
	PUBLICKEYSTRUC str;
#ifndef UNIX
	PCCRYPT_OID_INFO info = NULL;
#endif /* UNIX */
	BYTE * in_key = NULL;
	BYTE tmp[1024];
	size_t key_len = 0;
	DWORD tmp_par;
	HCRYPTKEY hPubKey = 0;

	/*-----------------------------------------------------------------------------*/
	/* При выполнении импорта может быть использован дополнительных ключ*/
	/* для алгоритма Diffi-Helmana.*/
	/* Если это специфигированно флагом -pubkey filename, предварительно прочитаем ключ в hPubKey*/
	if (pubkeyfilename) {
	    ret = get_file_data_pointer (pubkeyfilename, &key_len, &in_key);
	    if (!ret)
		HandleErrorFL ("Error occurred when importing key.\n");
	    /*-----------------------------------------------------------------------------*/
	    /* Выполним импорт дополнительного ключа*/
	    ret = UniImportKey (hProv,in_key,(DWORD) key_len,0,0,&hPubKey);
	    if (!ret)
		HandleErrorFL ("Error occurred when importing key.\n");
	    /*-----------------------------------------------------------------------------*/
	    /* Отобразим идентификатор алгоритма ключа*/
	    memcpy (&str, in_key, sizeof (PUBLICKEYSTRUC));
	    printf ("Public key ALG_ID: %u was imported from file %s\n", str.aiKeyAlg, pubkeyfilename);
	    release_file_data_pointer (in_key);
	    in_key = NULL;
	}

	/*-----------------------------------------------------------------------------*/
	/* Если при выполнении функции импорта в опциях задан флаг использования ключа обмена */
	/* -keytype exchange, и ранне прочитали открытый ключ, */
	/* импорт будет использовать ранее считанный ключ*/
	if (! hPubKey && keyType == AT_KEYEXCHANGE)
	    hPubKey = hKeyE;
	key_len = 0;
	ret = get_file_data_pointer (importfilename, &key_len, &in_key);
	if (!ret)
	    HandleErrorFL ("Error occurred when importing key.\n");
        /*-----------------------------------------------------------------------------*/
        /* Выполним импорт ключа с использование ранее считанного*/
	ret = UniImportKey (hProv,in_key,(DWORD) key_len,hPubKey,0,&hImportedKey);
	if (!ret)
	    HandleErrorFL ("Error occurred when exporting key.\n");
	/*-----------------------------------------------------------------------------*/
	/* Отобразим идентификатор алгоритма ключа в блобе*/
	memcpy (&str, in_key, sizeof (PUBLICKEYSTRUC));
	release_file_data_pointer (in_key);
	printf ("Public key ALG_ID: %u was imported from file %s\n", str.aiKeyAlg, importfilename);
	
	/*-----------------------------------------------------------------------------*/
	/* Отобразим идентификатор алгоритма ключа после преобразования*/
	if (!CPlevel) {
#ifndef UNIX
	    info = CryptFindOIDInfo(CRYPT_OID_INFO_ALGID_KEY, (void*) &str.aiKeyAlg, 0);
	    if (info) {
		wprintf (L"OID : %S  Name : %s\n", info->pszOID, info->pwszName);
	    }
#endif /* UNIX */
	}
	tmp_par=(DWORD)key_len; /* 32/64 bit problem; size_t is 8 bytes */
	ret = UniGetKeyParam (hProv,hImportedKey,KP_ALGID,tmp,&tmp_par,0);
	key_len=tmp_par;
	if (!ret)
	    HandleErrorFL ("Error occurred when importing key.\n");
	printf ("Imported Key ALG_ID: %u(%x) \n\n", *(ALG_ID*) tmp, *(ALG_ID*) tmp);

	printf ("\n");
    }

    /*-----------------------------------------------------------------------------*/
    /* Прочитаем файл, используемый как входные данные*/
    if (infilename) {
	ret = get_file_data_pointer (infilename, &in_data_len, &in_data);
	if (!ret)
	    HandleErrorFL ("Error occurred when reading infilename.\n");
    }

    /*-----------------------------------------------------------------------------*/
    /* Вычислим значение хэша входных данных по алгоритму, */
    /* специфицированному переменной hash_alg*/
    if (hash) {
	BYTE hash_val[64];
	DWORD hash_len = 64;
	DWORD len = sizeof (DWORD);
	
	if (infilename == NULL)
	    HandleErrorFL ("No data to be hashed\n");
	
        /*-----------------------------------------------------------------------------*/
	/* Создание дескриптора хэша*/
	if (CPlevel)
	    ret = MyCreateHash(hProv, hash_alg, 0, 0, &hHash);
	else{
#ifndef UNIX
	    ret = CryptCreateHash(hProv, hash_alg, 0, 0, &hHash);
#endif /* UNIX */
	}
	if (!ret)
	    HandleErrorFL("Error during CryptCreateHash.");

        /*-----------------------------------------------------------------------------*/
	/* Хэширование данных*/
	if (CPlevel)
	    ret = MyHashData(hProv, hHash, in_data, in_data_len, 0);
	else{
#ifndef UNIX
	    ret = CryptHashData(hHash, in_data, in_data_len, 0);
#endif /* UNIX */
	}
	if (!ret)
	    HandleErrorFL("Error during CryptHashData.");
    
	printf("Hash object created with alg: %s %X\n", ptr_hash_alg, hash_alg);

        /*-----------------------------------------------------------------------------*/
	/* Определение требуемого размера памяти для возврата значения хэша*/
	/* и вернем значение*/
	if (CPlevel) {
	    ret = MyGetHashParam(hProv, hHash, HP_HASHSIZE, (BYTE*)&hash_len, &len, 0);
	    if (!ret)
		HandleErrorFL("Error during CryptGetHashParam.");
	    
	    ret = MyGetHashParam(hProv, hHash, HP_HASHVAL, hash_val, &hash_len, 0);
	    MyDestroyHash (hProv, hHash);
	}
	else {
#ifndef UNIX
	    ret = CryptGetHashParam(hHash, HP_HASHSIZE, (BYTE*)&hash_len, &len, 0);
	    if (!ret)
		HandleErrorFL("Error during CryptGetHashParam.");

	    ret = CryptGetHashParam(hHash, HP_HASHVAL, hash_val, &hash_len, 0);
	    CryptDestroyHash (hHash);
#endif /* UNIX */
	}
	if (!ret)
	    HandleErrorFL("Error during CryptGetHashParam.");

        /*-----------------------------------------------------------------------------*/
	/* Если задано, сохраним значение хэша в файле*/
	if (ret && hashfile) {
	    write_file (hashfile, hash_len, hash_val);
	    printf ("Hash value (%d bytes) saved file %s\n", hash_len, hashfile);
	}
    }
    /*-----------------------------------------------------------------------------*/
    /* Формирование и проверка ЭЦП */
    if (sign || verify) {
	if (infilename == NULL)
	    HandleErrorFL ("No data to be signed\n");
	
	/*-----------------------------------------------------------------------------*/
	/* Создадим дескриптор хеша*/
	if (CPlevel)
	    ret = MyCreateHash(hProv, my_alg, 0, 0, &hHash);
	else{
#ifndef UNIX
	    ret = CryptCreateHash(hProv, my_alg, 0, 0, &hHash);
#endif /* UNIX */
	}

	if (ret) {
	    printf("Hash object created with alg: %s %X\n", my_hash_alg, my_alg);
	}
	else
	{
	    HandleErrorFL("Error during CryptCreateHash.");
	}
	/*-----------------------------------------------------------------------------*/
	/* Вычислим значение хэша*/
	if (CPlevel)
	    ret = MyHashData(hProv, hHash, in_data, in_data_len, 0);
	else{
#ifndef UNIX
	    ret = CryptHashData(hHash, in_data, in_data_len, 0);
#endif /* UNIX */
	}
	if (ret)
	    printf("The data buffer has been hashed.\n");
	else
	    HandleErrorFL("Error during CryptHashData.");
    }
    release_file_data_pointer (in_data);
    /*-----------------------------------------------------------------------------*/
    /* Формирование ЭЦП*/
    if (sign) {
	DWORD dwSigLen= 0;
	BYTE *pbSignature = NULL;
	int kt;

	if (!hHash)
	    HandleErrorFL("No HASH");
	/*-----------------------------------------------------------------------------*/
	/* Определим тип ключа, используемый для формирования ЭЦП*/
	hSignatureKey = (keyType == AT_SIGNATURE) ? hKeyS : hKeyE;
	kt = (keyType == AT_SIGNATURE) ? keyType : AT_KEYEXCHANGE;
	/* Определим размер памяти, требуемый для значения ЭЦП*/
	ret=UniSignHash(hProv,hHash,kt,NULL,0,NULL,&dwSigLen);
	if (ret) 
	    printf("Signature length %d found.\n",dwSigLen);
	else
	    HandleErrorFL("Error during CryptSignHash.");
	/* Резервируем память*/
	pbSignature = (BYTE *)malloc(dwSigLen);
	if (!pbSignature)
	    HandleErrorFL("Out of memory.");
	/*-----------------------------------------------------------------------------*/
	/* Подпишем значение хеша*/
	
	ret = UniSignHash(hProv, hHash, kt,NULL, 0, pbSignature, &dwSigLen);
	if (ret) {
	    printf("Signature was done.\n");
	    if (outfilename) {
		write_file (outfilename, dwSigLen, pbSignature);
		printf ("Signature was saved into file %s\n", outfilename);
	    }
	}
	else
	    HandleErrorFL("Error during CryptSignHash.");
	printf ("\n");
    }

    /*-----------------------------------------------------------------------------*/
    /* Проверка ЭЦП. Проводится только на ключе импорта, прочитанного из файла.*/
    if (verify) {
	BYTE *signature = NULL;
	size_t signature_len = 0;

	if (!hHash)
	    HandleErrorFL("No HASH");
	if (! hImportedKey)
	    HandleErrorFL("No Imported Key");
	if (! signaturefilename) 
	    HandleErrorFL("No signature file was specified");
	ret = get_file_data_pointer (signaturefilename, &signature_len, &signature);
	if (! ret) 
	    HandleErrorFL("Cannot read signature file");

	/*-----------------------------------------------------------------------------*/
	/* Проверка ЭЦП. */
	ret = UniVerifySignature (hProv, hHash,signature,(DWORD)signature_len,hImportedKey,NULL,0);
	if (ret)
	    printf ("Signature was verified OK\n");
	else
	    HandleErrorFL("Bad Signature");
	release_file_data_pointer (signature);
        printf ("\n");
    }

    if( !hash && szCopyContainer && !(dwFlags & CRYPT_VERIFYCONTEXT) )
    {
	HCRYPTPROV hDestProv = 0;
	if (CPlevel)
	    bResult = MyAcquireContext (&hDestProv, szCopyContainer, 
	    dwFlags | CRYPT_NEWKEYSET, &vTable );
#ifndef UNIX
	else
	    bResult = CryptAcquireContext (&hDestProv, szCopyContainer, szProvider,
		dwProvType, dwFlags | CRYPT_NEWKEYSET );
#endif /* UNIX */
	if (bResult) {
	    printf ("CryptAcquireContext succeeded.HCRYPTPROV: %lu\n", hProv);
	} else {
	    HandleErrorFL ("Error during CryptAcquireContext.\n");
	    goto bad;
	}
	bResult = CopyContainerParam( hProv, hDestProv );
	if (!bResult)
	    goto bad;
	if( keyType & AT_KEYEXCHANGE )
	{
	    bResult = CopyPrivateKey( hProv, hDestProv, hKeyE );
	    if (!bResult)
		goto bad;
	}
	if( keyType & AT_SIGNATURE )
	{
	    bResult = CopyPrivateKey( hProv, hDestProv, hKeyS );
	    if (!bResult)
		goto bad;
	}
    }
    /*-----------------------------------------------------------------------------*/
    /* Очистка памяти и удаление дескрипторов. */
    if( !hash && (keyType & AT_SIGNATURE) && !(dwFlags & CRYPT_VERIFYCONTEXT) )
    {
        bResult = UniDestroyKey (hProv, hKeyS); 
        if (!bResult) {
	    HandleErrorFL ("Error during CryptDestroyKey.\n");
	    goto bad;
	}
    }
    if( !hash && (keyType & AT_KEYEXCHANGE) && !(dwFlags & CRYPT_VERIFYCONTEXT) )
    {
        bResult = UniDestroyKey (hProv, hKeyE); 
	if (!bResult) {
	    HandleErrorFL ("Error during CryptDestroyKey.\n");
	    goto bad;
	}
    }

    bResult = UniReleaseContext (hProv, 0);
    if (!bResult) {
	HandleErrorFL ("Error during CryptReleaseContext.\n");
	goto bad;
    }
    if( reboot )
	cpcsp_reboot( hCSPDLL );
    hProv = 0;
    printf ("Everything is OK.\nA ");
    if( !hash && !(dwFlags & CRYPT_VERIFYCONTEXT) )
    {
	if( keyType & AT_SIGNATURE )
	    printf ("signature key pair ");
	if( (keyType & AT_SIGNATURE) && (keyType & AT_KEYEXCHANGE) )
	    printf ("and ");
	if( keyType & AT_KEYEXCHANGE )
	    printf ("an exchange key");
	if( keyType == 0 )
	    printf ("No keys" );
	printf ("\nexist in the \"%s\" key container.\n", pbProvContainer);
    }
    ret = 1;
bad:

    if (print_help) {
	printf ("%s -keyset [options]\n", prog);
	printf (SoftName " key set manipulation options:\n");
	printf ("  -cplevel              load CSP dll and use CP functions instead Crypt\n");
	printf ("  -provider name        specify provider name or next abbriviation:\n");
	printf ("                       ");
	printf ("   cpDef\n");
	printf ("                       ");
	printf ("   msDef ");
	printf ("msEnhanced ");
#ifdef MS_STRONG_PROV
	printf ("msStrong ");
#endif /* MS_STRONG_PROV */
	printf ("\n                       ");
	printf ("   msDefRsaSig ");
	printf ("msDefRsaSchannel ");
#ifdef MS_ENHANCED_RSA_SCHANNEL_PROV
	printf ("msEnhancedRsaSchannel ");
#endif /* MS_ENHANCED_RSA_SCHANNEL_PROV */
	printf ("msDefDss ");
	printf ("\n                       ");
	printf ("   msDefDssDh ");
#ifdef MS_ENH_DSS_DH_PROV
	printf ("msEnhDssDh ");
#endif /* MS_ENH_DSS_DH_PROV */
#ifdef MS_DEF_DH_SCHANNEL_PROV
	printf ("msDefDhSchannel ");
#endif /* MS_DEF_DH_SCHANNEL_PROV */
#ifdef MS_SCARD_PROV
	printf ("msScard ");
#endif /* MS_SCARD_PROV */
	printf ("\n");
	printf ("  -provtype type        specify provider type or next abbriviation:\n");
	printf ("                       ");
	printf ("   CProCSP\n");
	printf ("                       ");
	printf ("   RsaFull ");
	printf ("RsaSig ");
	printf ("Dss ");
	printf ("Fortezza ");
	printf ("MsExchange ");
	printf ("Ssl ");
	printf ("\n                       ");
	printf ("   RsaSchannel ");
	printf ("DssDh ");
	printf ("EcEcdsaSig ");
	printf ("EcEcnraSig ");
	printf ("EcEcdsaFull ");
	printf ("\n                       ");
	printf ("   EcEcnraFull ");
#ifdef PROV_DH_SCHANNEL
	printf ("DhSchannel ");
#endif /* PROV_DH_SCHANNEL */
	printf ("SpyrusLynks ");
#ifdef PROV_RNG
	printf ("Rng ");
#endif /* PROV_RNG */
#ifdef PROV_INTEL_SEC
	printf ("IntelSec ");
#endif /* PROV_INTEL_SEC */
	printf ("\n");
	printf ("  -container name       specify container name\n");
	printf ("  -verifycontext        open context for verification only\n");
	printf ("  -newkeyset            create new key set\n");
	printf ("  -machinekeyset        open HKLM\n");
	printf ("  -deletekeyset         delete key set\n");
	printf ("  -exported             generated key need to be exported\n");
	printf ("  -info                 show provider's info\n");
	printf ("  -keytype type         public key type to be used for signing or exporting\n");
	printf ("                        (signature, exchange,none)\n");
	printf ("  -export filename      filename contained public keytype key\n");
	printf ("  -import filename      filename contained public key, if keytype EQ exchange,\n");
	printf ("                        user exchange secret key will be used with import as\n");
	printf ("                        hPubKey\n");
	printf ("  -hash hashalg         hash in file data with hash (SHA1, MD5, MD2,\n");
	printf ("                        GOST - default)\n");
	printf ("  -hashout filename     out hash value into filename\n");
	printf ("  -sign hashalg         sign in file with keyType and hash (SHA1, MD5, MD2,\n");
	printf ("                        GOST - default)\n");
	printf ("  -verify hashalg       verify in file with signature file on imported key\n");
	printf ("                        with hash (SHA1, MD5, MD2, GOST - default)\n");
	printf ("  -in filename          data filename to be signed or verified\n");
	printf ("  -out filename         save signature in file\n");
	printf ("  -signature filename   in/out signature file name\n");
	printf ("  -pubkey filename      additional public key to be used with CpImportKey\n");
	printf ("                        function as hPubKey, if public key not specified and\n");
	printf ("                        keytype EQ exchange user secret key wiil be used as\n");
	printf ("                        hPubKey\n");
	printf ("  -alg ALG_ID           obtain alghorithm info by alg_id\n");
	printf ("  -enum_containers      enumerate containers\n");
	printf ("  -unique               output unique container name instead name\n");
#ifdef UNIX
	printf ("  -stress               CP{Acquire/Relase}Context\n");
#endif /* UNIX */
	printf ("  -passwd password      use password on container\n");
	printf ("  -chpasswd password    change password on container\n");
	printf ("  -qchpasswd            windowing change password on container\n");
	printf ("  -enchpasswd container change password to encryption container\n" );
	printf ("  -enc_passwd container set encryption container\n" );
	printf ("  -hard_rng             use hardware RNG\n");

#if(_WIN32_WINNT >= 0x0500)
	printf ("  -silent               do not display any user interface\n");
#endif /*(_WIN32_WINNT >= 0x0500)*/
	printf ("  -length keylength     set key length to 1024 / 512 bits\n" );
	printf ("  -enum_param type      enumerate algothm's parameters hash/cipher/dh/sign\n" );
	printf ("  -set_param type       set default algothm's parameters hash/cipher/dh/sign\n" );
	printf ("  -get_param type       get default algothm's parameters hash/cipher/dh/sign\n" );
	printf ("  -help                 print this help\n");
    }
#ifndef UNIX
    if( hCSPDLL ) FreeLibrary( hCSPDLL );
#endif /* UNIX */

    return ret;
}

/*----------------------------------------------------------------------*/
/* Функия вывода информации по алгоритмам, реализуемым криптопровайдером*/
int show_info (HCRYPTPROV hProv, HCRYPTKEY hKeyS, HCRYPTKEY hKeyE)
{
    int ret = 0;
    PROV_ENUMALGS_EX *pbData = NULL;
    DWORD pdwDataLen = 1000;
    CHAR *pszAlgType = NULL;
    BYTE *dataS = NULL;
    DWORD dlS;
    BYTE *dataE = NULL;
    DWORD dlE;

    if (! hProv ) goto err;

    pbData = (PROV_ENUMALGS_EX*) malloc (1000);

    UniGetProvParam (hProv, PP_ENUMALGS_EX, (BYTE*)pbData, &pdwDataLen, CRYPT_FIRST);
    do {
      switch(GET_ALG_CLASS(pbData->aiAlgid)) {
      case ALG_CLASS_DATA_ENCRYPT: pszAlgType = "Encrypt  ";	break;
      case ALG_CLASS_HASH:	 pszAlgType = "Hash     ";	break;
      case ALG_CLASS_KEY_EXCHANGE: pszAlgType = "Exchange ";	break;
      case ALG_CLASS_SIGNATURE:	 pszAlgType = "Signature";	break;
      default:			 pszAlgType = "Unknown  ";
      }

      printf("\n\nName:%-14s Type:%s  DefaultLen:%-4d MinLen:%-4d \nMaxLen:%-4d Prot:%-4d Name:'%s'(%d) Long:'%s'(%d) Algid:%8.8lu\n",
	     pbData->szLongName, pszAlgType, 
	     pbData->dwDefaultLen, 
	     pbData->dwMinLen, 
	     pbData->dwMaxLen,
	     pbData->dwProtocols,
	     pbData->szName,
	     pbData->dwNameLen,
	     pbData->szLongName,
	     pbData->dwLongNameLen,
	     pbData->aiAlgid);
      
    } while (UniGetProvParam(hProv, PP_ENUMALGS_EX, (BYTE*)pbData, &pdwDataLen, 0));
    
    printf ("\n");
    
#ifndef UNIX        
    /*----------------------------------------------------------------------*/
    /* Определение длин констант (ключей) для ключей ЭЦП и обмена*/
    if (hKeyS) {
	/* ============== P*/
	if (CryptGetKeyParam(hKeyS, KP_P, NULL, &dlS, 0)) {
	    dataS = (BYTE*) malloc (dlS);
	    printf ("Signature P Len  : %d %d\n", dlS, dlS*8);
	    CryptGetKeyParam(hKeyS, KP_P, dataS, &dlS, 0);
	}
	
	/* ============== Q*/
	if (CryptGetKeyParam(hKeyS, KP_Q, NULL, &dlS, 0)) {
	    dataS = (BYTE*) malloc (dlS);
	    printf ("Signature Q Len  : %d %d\n", dlS, dlS*8);
	    CryptGetKeyParam(hKeyS, KP_Q, dataS, &dlS, 0);
	}

	/* ============== G*/
	if (CryptGetKeyParam(hKeyS, KP_G, NULL, &dlS, 0)) {
	    dataS = (BYTE*) malloc (dlS);
	    printf ("Signature G Len  : %d %d\n", dlS, dlS*8);
	    CryptGetKeyParam(hKeyS, KP_G, dataS, &dlS, 0);
	}
    }
    if (hKeyE) {
	/* ============== P*/
	if (CryptGetKeyParam(hKeyE, KP_P, NULL, &dlE, 0)) {
	    dataE = (BYTE*) malloc (dlE);
	    printf ("DH Exchange P Len: %d %d\n", dlE, dlE*8);
	    CryptGetKeyParam(hKeyE, KP_P, dataE, &dlE, 0);
	}
	
	/* ============== Q*/
	if (CryptGetKeyParam(hKeyE, KP_Q, NULL, &dlE, 0)) {
	    dataE = (BYTE*) malloc (dlE);
	    printf ("DH Exchange Q Len: %d %d\n", dlE, dlE*8);
	    CryptGetKeyParam(hKeyE, KP_Q, dataE, &dlE, 0);
	}
	
	/* ============== G*/
	if (CryptGetKeyParam(hKeyE, KP_G, NULL, &dlE, 0)) {
	    dataE = (BYTE*) malloc (dlE);
	    printf ("DH Exchange G Len: %d %d\n", dlE, dlE*8);
	    CryptGetKeyParam(hKeyE, KP_G, dataE, &dlE, 0);
	}
	printf ("\n");
    }
#endif /* UNIX */

err:
    if( pbData )
	free( pbData );
    return ret;
}

/*--------------------------------------------------------------------*/
/* Функция загрузки криптопровайдера по заданному типу*/
#if !defined(UNIX)
HANDLE 
#else
void *
#endif /* !defined(UNIX) */
load_CSP_library (DWORD dwProvType, const char *szDllName) {

#if !defined(UNIX)
    char CSPDLLName [1024];
    HKEY hKey = NULL;
   
    if (NULL != szDllName) {
        strncpy(CSPDLLName, szDllName, sizeof(CSPDLLName));
        CSPDLLName[sizeof(CSPDLLName)-1] = '\0';
    } else {
            /* Определяем имя DLL по типу CSP */
        char CSPName [1024];
        DWORD dwDataSize;
        char type[1024];
        char   num[32];

            /* Все криптопровайдеры зарегистрированы по следующему ключу registry*/
#       define PROVIDER_REGKEY_FULLPATH_A "SOFTWARE\\Microsoft\\Cryptography\\Defaults"

        strcpy (type, (const char*) PROVIDER_REGKEY_FULLPATH_A);
        strcat (type, "\\Provider Types");
        sprintf (num, "\\Type %03d", dwProvType);
        strcat (type, num);
  
        if (ERROR_SUCCESS != RegOpenKeyEx (
	    HKEY_LOCAL_MACHINE,
	    type,
	    0,
	    KEY_QUERY_VALUE,
	    &hKey)) {
	    goto ReleaseResource;
        }
        dwDataSize = sizeof(CSPName);
        if (ERROR_SUCCESS != RegQueryValueEx (
	    hKey,
	    "Name",
	    NULL,
	    NULL,
	    (LPBYTE)CSPName,
	    &dwDataSize)) {
	    goto ReleaseResource;
        }
        if (hKey) {
            RegCloseKey(hKey);
        }
        /* Определим имя DLL, для провайдера соответствующего типа*/
        strcpy (type, (const char*) PROVIDER_REGKEY_FULLPATH_A);
        strcat (type, "\\Provider\\");
        strcat (type, CSPName);
    
        if (ERROR_SUCCESS != RegOpenKeyEx (
	    HKEY_LOCAL_MACHINE,
	    type,
	    0,
	    KEY_QUERY_VALUE,
	    &hKey)) {
	    goto ReleaseResource;
        }
        dwDataSize = sizeof(CSPName);
        if (ERROR_SUCCESS != RegQueryValueEx (
	    hKey,
	    "Image Path",
	    NULL,
	    NULL,
	    (LPBYTE)CSPDLLName,
	    &dwDataSize)) {
	    goto ReleaseResource;
        }
        if (hKey) {
            RegCloseKey(hKey);
        }
    }
    /* Загрузим DLL*/
    return (LoadLibrary(CSPDLLName));

ReleaseResource:
    if (hKey) RegCloseKey(hKey);

    return 0;   /* Ошибка */
#else

    if (PROV_GOST_94_DH != dwProvType) {
        /* ???? I think about PROV_GOST_2001_DH ???? */
        return 0;
    }

    return dlopen(szDllName, RTLD_LAZY);
#endif /* !defined(UNIX) */
}

static BOOL CopyKeyParam( HCRYPTPROV hSrcProv, HCRYPTPROV hDestProv, 
    DWORD param, HCRYPTKEY hSrcKey, HCRYPTKEY hDestKey )
{
    DWORD dwBlobLen = 0;
    BYTE *pbKeyBlob = NULL;
    const char *err_fun = NULL;
#if defined( BOURGEOIS )
    CRYPT_DATA_BLOB Blob;
    BYTE *CopyKeyParamBlob = (BYTE*)&Blob;
#else /* defined( BOURGEOIS ) */
#define CopyKeyParamBlob pbKeyBlob
#endif /* defined( BOURGEOIS ) */

	/* -- передача параметра G из исходного контейнера; */
    if( !UniGetKeyParam( hSrcProv, hSrcKey, param, 
	NULL, &dwBlobLen, 0 ) )
    {
	err_fun = "CryptGetKeyParam";
	goto err;
    }
    pbKeyBlob = (BYTE*)malloc( dwBlobLen );
    if( !pbKeyBlob )
    {
	err_fun = "malloc";
	goto err;
    }
    if( !UniGetKeyParam( hSrcProv, hSrcKey, param, 
	pbKeyBlob, &dwBlobLen, 0 ) )
    {
	err_fun = "CryptGetKeyParam";
	goto err;
    }
#if defined( BOURGEOIS )
    Blob.cbData = dwBlobLen;
    Blob.pbData = pbKeyBlob;
#endif /* defined( BOURGEOIS ) */
    if( !UniSetKeyParam( hDestProv, hDestKey, param, 
	CopyKeyParamBlob, 0 ) )
    {
	err_fun = "CryptSetKeyParam";
	goto err;
    }
err:
    if( pbKeyBlob )
	free( pbKeyBlob ); 
    if( err_fun )
    {
	printf( "%s fail. LastError:0x%x\n", err_fun, GetLastError() );
	return FALSE;
    }
    return TRUE;
}
#if !defined( BOURGEOIS )
#undef CopyKeyParamBlob 
#endif /* !defined( BOURGEOIS ) */

static BOOL CopyPrivateKey( HCRYPTPROV hSrcProv, HCRYPTPROV hDestProv, 
    HCRYPTKEY hSrcKey )
{
    HCRYPTKEY hDestEphemKey = 0;
    HCRYPTKEY hSrcEphemKey = 0;
    HCRYPTKEY hDestAgreeKey = 0;
    HCRYPTKEY hSrcAgreeKey = 0;
    HCRYPTKEY hDestKey = 0;
    DWORD dwBlobLen = 0;
    BYTE *pbKeyBlob = NULL;
    const char *err_fun = NULL;
#if defined( BOURGEOIS )
    DWORD algid_export = CYLINK_MEK;
    DWORD nsigned algid_ephem = CALG_DH_EPHEM;
    DWORD kp1 = KP_G;
    DWORD kp2 = KP_P;
#else
    DWORD algid_export = CALG_PRO_EXPORT;
    DWORD algid_ephem = CALG_DH_EX_EPHEM;
    DWORD kp1 = KP_HASHOID;
    DWORD kp2 = KP_DHOID;
#endif /* defined( BOURGEOIS ) */
    /* Генерация эфимерных ключей: */
	/* -- в исходном контейнере */
    if( !UniGenKey( hSrcProv, algid_ephem, CRYPT_EXPORTABLE, 
	&hSrcEphemKey ) )
    {
	err_fun = "CryptGenKey";
	goto err;
    }
	/* -- в результирующем контейнере. */
    if( !UniGenKey( hDestProv, algid_ephem, CRYPT_EXPORTABLE | CRYPT_PREGEN, 
	&hDestEphemKey ) )
    {
	err_fun = "CryptGenKey";
	goto err;
    }
	/* -- передача параметра G из исходного контейнера; */
    if( !CopyKeyParam( hSrcProv, hDestProv, kp1, hSrcEphemKey, 
	hDestEphemKey ) )
    {
	err_fun = "CopyKeyParam";
	goto err;
    }
	/* -- передача параметра P из исходного контейнера; */
    if( !CopyKeyParam( hSrcProv, hDestProv, kp2, hSrcEphemKey, 
	hDestEphemKey ) )
    {
	err_fun = "CopyKeyParam";
	goto err;
    }
	/* -- генерация параметра X; */
    if( !UniSetKeyParam( hDestProv, hDestEphemKey, KP_X, NULL, 0 ) )
    {
	err_fun = "CryptSetKeyParam";
	goto err;
    }
    /* Получение agree key в результирующем контейнере: */
	/* -- экспорт из исходного контейнера; */
    if( !UniExportKey( hSrcProv, hSrcEphemKey, 0, PUBLICKEYBLOB, 0, 
	NULL, &dwBlobLen ) )
    {
	err_fun = "CryptExportKey";
	goto err;
    }
    pbKeyBlob = (BYTE*)malloc( dwBlobLen );
    if( !pbKeyBlob )
    {
	err_fun = "malloc";
	goto err;
    }
    if( !UniExportKey( hSrcProv, hSrcEphemKey, 0, PUBLICKEYBLOB, 0, 
	pbKeyBlob, &dwBlobLen ) )
    {
	err_fun = "CryptExportKey";
	goto err;
    }
	/* -- импорт в результирующий; */
    if( !UniImportKey( hDestProv, pbKeyBlob, dwBlobLen, 
	hDestEphemKey, 0, &hDestAgreeKey ) )
    {
	err_fun = "CryptImportKey";
	goto err;
    }
    free( pbKeyBlob ); pbKeyBlob = NULL; dwBlobLen = 0;
    /* Получение agree key в исходном контейнере: */
	/* -- экспорт из результирующего контейнера; */
    if( !UniExportKey( hDestProv, hDestEphemKey, 0, PUBLICKEYBLOB, 0, 
	NULL, &dwBlobLen ) )
    {
	err_fun = "CryptExportKey";
	goto err;
    }
    pbKeyBlob = (BYTE*)malloc( dwBlobLen );
    if( !pbKeyBlob )
    {
	err_fun = "malloc";
	goto err;
    }
    if( !UniExportKey( hDestProv, hDestEphemKey, 0, PUBLICKEYBLOB, 0, 
	pbKeyBlob, &dwBlobLen ) )
    {
	err_fun = "CryptExportKey";
	goto err;
    }
	/* -- импорт в исходный; */
    if( !UniImportKey( hSrcProv, pbKeyBlob, dwBlobLen, 
	hSrcEphemKey, 0, &hSrcAgreeKey ) )
    {
	err_fun = "CryptImportKey";
	goto err;
    }
    free( pbKeyBlob ); pbKeyBlob = NULL; dwBlobLen = 0;
    /* Конвертирование agree key в сессионный */
	/* -- в исходном контейнере; */
    if( !UniSetKeyParam( hSrcProv, hSrcAgreeKey, KP_ALGID, 
	(BYTE*)&algid_export,
	0 ) )
    {
	err_fun = "CryptSetKeyParam";
	goto err;
    }
	/* -- в результирующем контейнере; */
    if( !UniSetKeyParam( hDestProv, hDestAgreeKey, KP_ALGID, 
	(BYTE*)&algid_export,
	0 ) )
    {
	err_fun = "CryptSetKeyParam";
	goto err;
    }
    /* Экспорт пользовательского ключа на сессионном. */
    if( !UniExportKey( hSrcProv, hSrcKey, hSrcAgreeKey, PRIVATEKEYBLOB,
	0, NULL, &dwBlobLen ) )
    {
	err_fun = "CryptExportKey";
	goto err;
    }
    pbKeyBlob = (BYTE*)malloc( dwBlobLen );
    if( !pbKeyBlob )
    {
	err_fun = "malloc";
	goto err;
    }
#if defined( TEST_EPHEM )
    if( !UniExportKey( hSrcProv, hSrcEphemKey, hSrcAgreeKey, PRIVATEKEYBLOB, 
	0, pbKeyBlob, &dwBlobLen ) )
    {
	err_fun = "CryptExportKey";
	goto err;
    }
    if( !UniImportKey( hDestProv, pbKeyBlob, dwBlobLen, hDestAgreeKey, 
	CRYPT_EXPORTABLE, &hDestKey ) )
    {
	err_fun = "CryptImportKey";
	goto err;
    }
#else
    if( !UniExportKey( hSrcProv, hSrcKey, hSrcAgreeKey, PRIVATEKEYBLOB, 
	0, pbKeyBlob, &dwBlobLen ) )
    {
	err_fun = "CryptExportKey";
	goto err;
    }
    /* Импотр пользовательского ключа. */
    if( !UniImportKey( hDestProv, pbKeyBlob, dwBlobLen, hDestAgreeKey, 
	CRYPT_EXPORTABLE, &hDestKey ) )
    {
	err_fun = "CryptImportKey";
	goto err;
    }
    if( !CopyKeyParam( hSrcProv, hDestProv, KP_CERTIFICATE, hSrcKey,
	hDestKey ) )
    {
	printf( "Certificate not found fail.\n" );
    }
#endif
err:
    if( hDestEphemKey )	UniDestroyKey( hDestProv, hDestEphemKey );
    if( hSrcEphemKey )	UniDestroyKey( hSrcProv, hSrcEphemKey );
    if( hDestAgreeKey )	UniDestroyKey( hDestProv, hDestAgreeKey );
    if( hSrcAgreeKey )	UniDestroyKey( hSrcProv, hSrcAgreeKey );
    if( hDestKey )	UniDestroyKey( hDestProv, hDestKey );
    if( pbKeyBlob )
	free( pbKeyBlob );
    if( err_fun )
    {
	printf( "%s fail. LastError:0x%x\n", err_fun, GetLastError() );
	return FALSE;
    }
    return TRUE;
}

BOOL CopyContainerParam1( HCRYPTPROV hSrc, HCRYPTPROV hDest, DWORD pp )
{
    DWORD data_len = 0;
    BYTE *oid = NULL;
    const char *err_fun = NULL;

    if( !UniGetProvParam( hSrc, pp, NULL, &data_len, 0 ) )
    {
	err_fun = "CryptGetProvParam";
	goto err;
    }
    oid = malloc( data_len );
    if( !oid )
    {
	err_fun = "malloc";
	goto err;
    }
    if( !UniGetProvParam( hSrc, pp, oid, &data_len, 0 ) )
    {
	err_fun = "CryptGetProvParam";
	goto err;
    }
    if( !UniSetProvParam( hDest, pp, oid, 0 ) )
    {
	err_fun = "CryptGetProvParam";
	goto err;
    }

err:
    if( oid )
	free( oid );
    if( err_fun )
    {
	printf( "%s fail. LastError:0x%x\n", err_fun, GetLastError() );
	return FALSE;
    }
    return TRUE;
}

BOOL CopyContainerParam( HCRYPTPROV hSrc, HCRYPTPROV hDest )
{
    if( !CopyContainerParam1( hSrc, hDest, PP_HASHOID ) )
	return FALSE;
    if( !CopyContainerParam1( hSrc, hDest, PP_CIPHEROID ) )
	return FALSE;
    return TRUE;
}

char *GetContainerName( int cplevel, PVTABLEPROVSTRUC pvTable,
    char * szProvider, DWORD dwProvType )
{
    HCRYPTPROV hProv;
    BOOL res;
    TCHAR *select;
    DWORD select_length = 64;

    if( cplevel )
	res = MyAcquireContext( &hProv, NULL, CRYPT_VERIFYCONTEXT, pvTable );
#ifndef UNIX
    else
	res = CryptAcquireContext( &hProv, NULL, szProvider, dwProvType,
	CRYPT_VERIFYCONTEXT );
#endif UNIX
    if( !res )
	return NULL;
    select = malloc( 64 );
    if( !select )
    {
	UniReleaseContext( hProv, 0 );
	return NULL;
    }
    if( !UniGetProvParam( hProv, PP_SELECT_CONTAINER, (BYTE*)select,
	&select_length, 0 ) )
    {
	UniReleaseContext( hProv, 0 );
	free( select );
	return NULL;
    }
    UniReleaseContext( hProv, 0 );
    return select;
}

static const unsigned short TABLE[256] = {
    0x0000, 0xC0C1, 0xC181, 0x0140, 0xC301, 0x03C0, 0x0280, 0xC241,
    0xC601, 0x06C0, 0x0780, 0xC741, 0x0500, 0xC5C1, 0xC481, 0x0440,
    0xCC01, 0x0CC0, 0x0D80, 0xCD41, 0x0F00, 0xCFC1, 0xCE81, 0x0E40,
    0x0A00, 0xCAC1, 0xCB81, 0x0B40, 0xC901, 0x09C0, 0x0880, 0xC841,
    0xD801, 0x18C0, 0x1980, 0xD941, 0x1B00, 0xDBC1, 0xDA81, 0x1A40,
    0x1E00, 0xDEC1, 0xDF81, 0x1F40, 0xDD01, 0x1DC0, 0x1C80, 0xDC41,
    0x1400, 0xD4C1, 0xD581, 0x1540, 0xD701, 0x17C0, 0x1680, 0xD641,
    0xD201, 0x12C0, 0x1380, 0xD341, 0x1100, 0xD1C1, 0xD081, 0x1040,
    0xF001, 0x30C0, 0x3180, 0xF141, 0x3300, 0xF3C1, 0xF281, 0x3240,
    0x3600, 0xF6C1, 0xF781, 0x3740, 0xF501, 0x35C0, 0x3480, 0xF441,
    0x3C00, 0xFCC1, 0xFD81, 0x3D40, 0xFF01, 0x3FC0, 0x3E80, 0xFE41,
    0xFA01, 0x3AC0, 0x3B80, 0xFB41, 0x3900, 0xF9C1, 0xF881, 0x3840,
    0x2800, 0xE8C1, 0xE981, 0x2940, 0xEB01, 0x2BC0, 0x2A80, 0xEA41,
    0xEE01, 0x2EC0, 0x2F80, 0xEF41, 0x2D00, 0xEDC1, 0xEC81, 0x2C40,
    0xE401, 0x24C0, 0x2580, 0xE541, 0x2700, 0xE7C1, 0xE681, 0x2640,
    0x2200, 0xE2C1, 0xE381, 0x2340, 0xE101, 0x21C0, 0x2080, 0xE041,
    0xA001, 0x60C0, 0x6180, 0xA141, 0x6300, 0xA3C1, 0xA281, 0x6240,
    0x6600, 0xA6C1, 0xA781, 0x6740, 0xA501, 0x65C0, 0x6480, 0xA441,
    0x6C00, 0xACC1, 0xAD81, 0x6D40, 0xAF01, 0x6FC0, 0x6E80, 0xAE41,
    0xAA01, 0x6AC0, 0x6B80, 0xAB41, 0x6900, 0xA9C1, 0xA881, 0x6840,
    0x7800, 0xB8C1, 0xB981, 0x7940, 0xBB01, 0x7BC0, 0x7A80, 0xBA41,
    0xBE01, 0x7EC0, 0x7F80, 0xBF41, 0x7D00, 0xBDC1, 0xBC81, 0x7C40,
    0xB401, 0x74C0, 0x7580, 0xB541, 0x7700, 0xB7C1, 0xB681, 0x7640,
    0x7200, 0xB2C1, 0xB381, 0x7340, 0xB101, 0x71C0, 0x7080, 0xB041,
    0x5000, 0x90C1, 0x9181, 0x5140, 0x9301, 0x53C0, 0x5280, 0x9241,
    0x9601, 0x56C0, 0x5780, 0x9741, 0x5500, 0x95C1, 0x9481, 0x5440,
    0x9C01, 0x5CC0, 0x5D80, 0x9D41, 0x5F00, 0x9FC1, 0x9E81, 0x5E40,
    0x5A00, 0x9AC1, 0x9B81, 0x5B40, 0x9901, 0x59C0, 0x5880, 0x9841,
    0x8801, 0x48C0, 0x4980, 0x8941, 0x4B00, 0x8BC1, 0x8A81, 0x4A40,
    0x4E00, 0x8EC1, 0x8F81, 0x4F40, 0x8D01, 0x4DC0, 0x4C80, 0x8C41,
    0x4400, 0x84C1, 0x8581, 0x4540, 0x8701, 0x47C0, 0x4680, 0x8641,
    0x8201, 0x42C0, 0x4380, 0x8341, 0x4100, 0x81C1, 0x8081, 0x4040
};

void crc16( const char *text, unsigned char *CRC )
{
    while( *text )
	*(unsigned short*)CRC = (unsigned short)(
	TABLE[CRC[0] ^ tolower( *text++ )] ^ CRC[1] );
}

#if !defined(lint) && !defined(NO_RCSID) && !defined(OK_MODULE_VERSION)
static volatile const char rcsid[] = "\n$Id: ctkey.c,v 1.68.4.5 2002/08/22 13:59:51 lse Exp $";
#endif
