
/*!
 * \file $RCSfile: tmain.c,v $
 * \version $Revision: 1.59.4.4 $
 * \date $Date: 2002/08/28 07:06:29 $
 * \author $Author: vasilij $
 *
 * \brief Программа работы тестирования криптопровайдера CrpyptoPro CSP.
 *
 * Подробное описание модуля.
 */
#include "tmain.h"


#include "mycert.h"



#ifndef MAIN
#define MAIN main
#endif /* MAIN */

#ifndef EXIT
#define EXIT exit
#endif /* EXIT */

//Глобальные переменные
char *prog = NULL;
char *stdinfile = NULL;
static int vstr2fmt (char *s);



int no_error_wait = 0;


//****************************************MAIN********************************************************  
int MAIN (int argc, char **argv)
{
    int	ret = 1; /* код возврата плохо по умолчанию */
    int	print_help = 0;

  
#ifdef _DEBUG
    int debug = 0;
#endif
    int c;
#ifndef UNIX
    int notime = 0;
    PublicTime * curTime = NULL;
#endif /* UNIX */

    static struct option long_options[] = {
#ifdef _DEBUG
	{"debug",	no_argument,		NULL, 'd'},
	{"csp-check",	no_argument,		NULL, 'C'},
#endif
#ifdef CTKEYSET
	{"keyset",	no_argument,		NULL, 'k'},

#endif /* CTKEYSET */


	{"notime",	no_argument,		NULL, 'm'},
	{"noerrorwait",	no_argument,		NULL, 'w'},


#ifdef CTMSG
	{"sfse",	no_argument,		NULL, 'p'},
#endif /* CTMSG */

#ifdef TSIGN
	{"cmslowsign",	no_argument,		NULL, 'S'},
	{"cmssfsign",	no_argument,		NULL, '2'},
	{"lowsign",	no_argument,		NULL, 's'},
	{"lowsignc",	no_argument,		NULL, '4'},
	{"sfsign",	no_argument,		NULL, '1'},
	{"show",	no_argument,		NULL, 'o'},
	{"makecert",	no_argument,		NULL, 'c'},
	{"property",	no_argument,		NULL, 'r'},
	{"certprop",	no_argument,		NULL, '3'},
	{"rc",		no_argument,		NULL, 'R'},
#endif /* TSIGN */

#ifdef THASH
	{"hash",	no_argument,		NULL, 'a'},
#endif /* THASH */

#ifdef TENCRYPT
	{"lowenc",	no_argument,		NULL, 'e'},
	{"cmsenclow",	no_argument,		NULL, 'M'},
#endif /* TENCRYPT */

#ifdef TENCRYPTSF
	{"sfenc",	no_argument,		NULL, '7'},
	{"crenc",	no_argument,		NULL, '8'},
#endif /* TENCRYPTSF */

#ifdef CRYPTUI
	{"cryptui",	no_argument,		NULL, 'u'},
#endif /* CRYPTUI */

#ifdef TSTRESS
	{"stress",	no_argument,		NULL, 't'},
#endif /* TSTRESS */

#ifdef TTLS
	{"tlss",	no_argument,		NULL, '5'},
	{"tlsc",	no_argument,		NULL, '6'},
	{"prf",		no_argument,		NULL, 'P'},
#endif /* TTLS */

#ifdef TEXPORT
	{"ep",		no_argument,		NULL, 'E'},
#endif /* EXPORT */

	{"help",	no_argument,		NULL, 'h'},
	{0, 0, 0, 0}
    };


    MTimeInit();




    prog = argv[0];
    optarg = 0;
    optind = 0;
    opterr = 1;
	printf("\n\n\n\n");
	printf("**********************************************************\n");
	printf("my CSP cli. 2019 VisualStudio 2019 CE recompilation \n\n");
#ifdef SoftName    printf(SoftName " program: %s \n");
   printf(" parse comand line.... \n");
#endif

    while ((c = getopt_long_only (argc, argv, "", long_options, (int *)0)) != EOF) {
	switch (c) {
	case 'd':
#ifdef _DEBUG
	    debug = 1;
#endif
	    continue;


	case 'm':
	    notime = 1;
	    continue;
	case 'w':
	    no_error_wait = 1;
	    continue;




	case 'h':
	    ret = 0;
	    print_help = 1;
	    /* no break */
	case '?':
	    goto bad;
	}
	break;
    }

    if (c == EOF) {
	print_help = 1;
	goto bad;
    }

    switch (c) {
#ifdef CTKEYSET
    case 'k':
	ret = !main_keyset (argc, argv);
	break;
#endif /* CTKEYSET */
#ifdef CTMSG
    case 'p':
	ret = !main_newmsg (argc, argv);
	break;
#endif /* CTMSG */
#ifdef TSIGN
    case 'S':
	/* Работа с ЭЦП */
	ret = !main_CMSsign (argc, argv);
	break;
    case '2':
	/* Работа с ЭЦП */
	ret = !main_CMSsign_sf (argc, argv);
	break;
    case 's':
	/* Работа с ЭЦП */
	ret = !main_sign (argc, argv);
	break;
    case '4':
	/* Работа с ЭЦП. Дополнительный тест в цикле */
	ret = !main_sign_1 (argc, argv);
	break;
    case '1':
	/* Работа с ЭЦП */
	ret = !main_sign_sf (argc, argv);
	break;
    case 'c':
	/* Создание сертификата */
	ret = !main_makecert (argc-1, ++argv);
	break;
    case 'r':
	/* Определение/установка свойст сертификата в справочнике */
	ret = !main_property(argc, argv);
	break;
    case '3':
	/* Определение свойств сертификата в справочнике */
	ret = !main_property1(argc, argv);
	break;
    case 'R':
	/* Проверка ЭЦП запроса на сертификат/сертификата */
	ret = !main_rc(argc, argv);
	break;
#endif /* TSIGN */
#ifdef TSHOWCERT
    case 'o':
	/* Отображение сертификата, CRL */
	ret = !main_show (argc, argv);
	break;
#endif /* TSHOWCERT */
#ifdef THASH
    case 'a':
	/* Тест хеш функций */
	ret = !main_hash (argc, argv);
	break;
#endif /* THASH */
#ifdef TENCRYPT
    case 'e':
	/* Тест шифрования PKCS#7 */
	ret = !main_encrypt (argc, argv);
	break;
    case 'M':
	/* Тест шифрования CMS */
	ret = !main_cms_encrypt (argc, argv);
	break;
#endif /* TENCRYPT */
#ifdef TENCRYPTSF
    case '7':
	/* Тест хеш функций */
	ret = !main_encrypt_sf (argc, argv);
	break;
    case '8':
	/* Пример на шифрование на самом низком уровне */
	ret = !main_crypt_encrypt (argc, argv);
	break;
#endif /* TENCRYPT */
#ifdef CRYPTUI
    case 'u':
	/* Тест GUI */
	ret = !main_cryptui (argc, argv);
	break;
#endif /* CRYPTUI */
#ifdef TSTRESS
    case 't':
	/* стресс-тест */
	ret = !main_stress (argc, argv);
	break;
#endif /* TSTRESS */
#ifdef TTLS
    case '5':
	/* TLS-тест */
	ret = !main_tlss (argc, argv);
	break;
    case '6':
	/* TLS-тест */
	ret = !main_tlsc (argc, argv);
	break;
    case 'P':
	ret = !main_prf (argc, argv);
	break;
#endif /* TTLS */
#ifdef TEXPORT
    case 'E':
    ret = !main_export_public(argc, argv);
    break;
#endif /* TEXPORT */
#ifdef TCSPCHACK
    case 'C':
	/* проверка функционирования обходов */
	ret = !main_csp_check (argc, argv);
	break;
#endif /* TCSPCHACK */
    }
bad:    
    if (print_help) {
		printf(" command line bad. Use these papams: \n");
	printf("%s [global options] [mode] [options]\n",prog);

/*
#ifdef Fixosoft
	printf(Fixosoft " defined \n");
#endif
*/

	printf("select [global options] from:\n");
	fprintf(stdout,"  -help         display this help\n");
#ifdef _DEBUG
	fprintf(stdout,"  -debug        debug mode\n");
#endif
	printf("select [mode] from:\n");
#ifdef CTKEYSET
	fprintf(stdout,"  -keyset       create (open) keyset\n");



#endif /* CTKEYSET */
#ifdef THASH
	fprintf(stdout,"  -hash         hash test\n");
#endif /* THASH */
#ifdef TENCRYPT
	fprintf(stdout,"  -lowenc       low level encryption/decryption test\n");
#endif /* TENCRYPT */
#ifdef TSIGN
	fprintf(stdout,"  -cmslowsign   CMS low level message signging test\n");
	fprintf(stdout,"  -cmssfsign    CMS simplefied level message signing/verifying test\n");
	fprintf(stdout,"  -lowsign      low level message signging test\n");
	fprintf(stdout,"  -lowsignc     low level message signging test with cicle\n");
	fprintf(stdout,"  -sfsign       simplefied level message signing/verifying test\n");
	/*fprintf(stdout,"  -msq          signing/verifying test with MSQUEUE\n");*/
	fprintf(stdout,"  -makecert     certificate issuing test\n");
	fprintf(stdout,"  -property     certificate obtain/install propery for secret key linking\n");
	fprintf(stdout,"  -rc           verify pkcs#10/certificate signature\n");
#endif /* TSIGN */

#ifdef TSHOWCERT
	fprintf(stdout,"  -show         show x.509 certificate, CRL with GUI\n");
#endif /* TSHOWCERT */
#ifdef TENCRYPTSF
	fprintf(stdout,"  -sfenc        simplefied level message encryption/decryption test\n");
	fprintf(stdout,"  -crenc        CSP level message encryption/decryption test\n");
#endif /* TENCRYPTSF */
#ifdef CTMSG
	fprintf(stdout,"  -sfse         simplefied level message SignedAndEnveloped test\n");
#endif /* CTMSG */

#ifdef CRYPTUI
	fprintf(stdout,"  -cryptui      GUI test\n");
#endif /* CRYPTUI */

#ifdef TSTRESS
	fprintf(stdout,"  -stress       stress test for Acquire/ReleaseContext\n");
#endif /* TSTRESS */

#ifdef TEXPORT
    fprintf(stdout,"  -ep           public key export test \n");
#endif /* TEXPORT */

#ifdef TCSPCHECK
	fprintf(stdout,"  -csp-check    check CSP functionality\n");
#endif /* TCSPCHECK */
    }

 // Вывод времени... необяхательное, но красиво
	if( !notime )
    {
	curTime = MTimeGet(NULL);
	fprintf (stderr, "Total: "); 
	MTimePrint (curTime);
	free (curTime);
    }
	fprintf(stdout,"Program " ShortSoftName" end. \n");

    return ret;
}


//-------------------------------------------------------------------------------- MAIN конец--------------------------------------

int TestString (char *str)
{
	if (!strcmp (str, "Fixosoft")) return 1;
	else return 0;
}


//--------------------------------------

char *abbr2provider (char *abbr)
{
    if (!abbr) return NULL;
#ifndef UNIX 
    if (!strcmp (abbr, "msDef")) return MS_DEF_PROV;
    if (!strcmp (abbr, "msEnhanced")) return MS_ENHANCED_PROV;
    if (!strcmp (abbr, "eTokenOS4")) return "eToken Base Cryptographic Provider";
    if (!strcmp (abbr, "jbutton")) return "Dallas Semiconductor Provider 1.0";
#ifdef MS_STRONG_PROV
    if (!strcmp (abbr, "msStrong")) return MS_STRONG_PROV;
#endif /* MS_STRONG_PROV */
    if (!strcmp (abbr, "msDefRsaSig")) return MS_DEF_RSA_SIG_PROV;
    if (!strcmp (abbr, "msDefRsaSchannel")) return MS_DEF_RSA_SCHANNEL_PROV;
#ifdef MS_ENHANCED_RSA_SCHANNEL_PROV
    if (!strcmp (abbr, "msEnhancedRsaSchannel")) return MS_ENHANCED_RSA_SCHANNEL_PROV;
#endif /* MS_ENHANCED_RSA_SCHANNEL_PROV */
    if (!strcmp (abbr, "msDefDss")) return MS_DEF_DSS_PROV;
    if (!strcmp (abbr, "msDefDssDh")) return MS_DEF_DSS_DH_PROV;
#ifdef MS_ENH_DSS_DH_PROV
    if (!strcmp (abbr, "msEnhDssDh")) return MS_ENH_DSS_DH_PROV;
#endif /* MS_ENH_DSS_DH_PROV */
#ifdef MS_DEF_DH_SCHANNEL_PROV
    if (!strcmp (abbr, "msDefDhSchannel")) return MS_DEF_DH_SCHANNEL_PROV;
#endif /* MS_DEF_DH_SCHANNEL_PROV */
#ifdef MS_SCARD_PROV
    if (!strcmp (abbr, "msScard")) return MS_SCARD_PROV;
#endif /* MS_SCARD_PROV */
#ifdef CP_DEF_PROV
    if (!strcmp (abbr, "cpDef")) return CP_DEF_PROV;
#endif /* CP_DEF_PROV */
#ifdef CP_GR3410_2001_PROV
    if (!strcmp (abbr, "cpEl")) return CP_GR3410_2001_PROV;
#endif /* CP_GR3410_2001_PROV */
#endif /* UNIX */
    
    return abbr;
}

DWORD abbr2provtype (char *abbr)
{
#ifndef UNIX
    if (!abbr) return PROV_RSA_FULL;
    if (!strcmp (abbr, "RsaFull")) return PROV_RSA_FULL;
    if (!strcmp (abbr, "RsaSig")) return PROV_RSA_SIG;
    if (!strcmp (abbr, "Dss")) return PROV_DSS;
    if (!strcmp (abbr, "Fortezza")) return PROV_FORTEZZA;
    if (!strcmp (abbr, "MsExchange")) return PROV_MS_EXCHANGE;
    if (!strcmp (abbr, "Ssl")) return PROV_SSL;
    if (!strcmp (abbr, "RsaSchannel")) return PROV_RSA_SCHANNEL;
    if (!strcmp (abbr, "DssDh")) return PROV_DSS_DH;
    if (!strcmp (abbr, "EcEcdsaSig")) return PROV_EC_ECDSA_SIG;
    if (!strcmp (abbr, "EcEcnraSig")) return PROV_EC_ECNRA_SIG;
    if (!strcmp (abbr, "EcEcdsaFull")) return PROV_EC_ECDSA_FULL;
    if (!strcmp (abbr, "EcEcnraFull")) return PROV_EC_ECNRA_FULL;
    if (!strcmp (abbr, "eTokenOS4")) return PROV_RSA_FULL;
    if (!strcmp (abbr, "jbutton")) return PROV_RSA_FULL;
#ifdef PROV_DH_SCHANNEL
    if (!strcmp (abbr, "DhSchannel")) return PROV_DH_SCHANNEL;
#endif /* PROV_DH_SCHANNEL */
    if (!strcmp (abbr, "SpyrusLynks")) return PROV_SPYRUS_LYNKS;
#ifdef PROV_RNG
    if (!strcmp (abbr, "Rng")) return PROV_RNG;
#endif /* PROV_RNG */
#ifdef PROV_INTEL_SEC
    if (!strcmp (abbr, "IntelSec")) return PROV_INTEL_SEC;
#endif /* PROV_INTEL_SEC */
#ifdef PROV_GOST_DH
    if (!strcmp (abbr, "GostDh")) return PROV_GOST_DH;
#endif /* PROV_GOST_DH */
#ifdef PROV_GOST_2001_DH
    if (!strcmp (abbr, "cpEl")) return PROV_GOST_2001_DH;
#endif /* PROV_GOST_2001_DH */
#endif /* UNIX */

    return atoi (abbr);
}

int
HandleError (const char *s, const char *f, int l)
{
    DWORD err = GetLastError ();
    LPVOID lpMsgBuf = NULL;
    int len;
    int len_u;
    wchar_t *str_u;


    if (!FormatMessage (FORMAT_MESSAGE_ALLOCATE_BUFFER
	| FORMAT_MESSAGE_FROM_SYSTEM
	| FORMAT_MESSAGE_IGNORE_INSERTS,
	NULL, err, 0, /*MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),*/
	(LPTSTR) &lpMsgBuf, 0, NULL)) {
	if (ERROR_RESOURCE_LANG_NOT_FOUND == GetLastError ()) {
	    printf ("can not format error message\n");
	}
	lpMsgBuf = NULL;
    }
    
    if (lpMsgBuf) {
	len = strlen (lpMsgBuf);
	len_u = len * sizeof (wchar_t) + 2;
	str_u = _alloca (len_u);
	
	MultiByteToWideChar (CP_ACP, 0, lpMsgBuf, -1, str_u, len_u);
	WideCharToMultiByte (GetConsoleOutputCP (), 0, str_u, -1, lpMsgBuf, len, NULL, NULL);
    }


    printf ("An error occurred in running the program.\n");
    printf ("%s:%d:%s\n",f,l,s);
    printf ("Error number %x (%ld).\n", err, err);
    if (lpMsgBuf) printf ("%s", lpMsgBuf);
    printf ("Program terminating.\n");


    if (lpMsgBuf) LocalFree (lpMsgBuf);

    if( !no_error_wait )
    {
	printf ("Press Enter to exit.\n");
	getchar ();
    }


    exit (err);
}

int DebugError (const char *s, const char *f, int l)
{
    DWORD err = GetLastError ();
    LPVOID lpMsgBuf = NULL;
    int len;
    int len_u;
    wchar_t *str_u;


    if (!FormatMessage (FORMAT_MESSAGE_ALLOCATE_BUFFER
	| FORMAT_MESSAGE_FROM_SYSTEM
	| FORMAT_MESSAGE_IGNORE_INSERTS,
	NULL, err, 0, /*MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),*/
	(LPTSTR) &lpMsgBuf, 0, NULL)) {
	if (ERROR_RESOURCE_LANG_NOT_FOUND == GetLastError ()) {
	    printf ("can not format error message\n");
	}
	lpMsgBuf = NULL;
    }
    
    if (lpMsgBuf) {
	len = strlen (lpMsgBuf);
	len_u = len * sizeof (wchar_t) + 2;
	str_u = _alloca (len_u);
	
	MultiByteToWideChar (CP_ACP, 0, lpMsgBuf, -1, str_u, len_u);
	WideCharToMultiByte (GetConsoleOutputCP (), 0, str_u, -1, lpMsgBuf, len, NULL, NULL);
    }


    printf ("An error occurred in running the program.\n");
    printf ("%s:%d:%s\n",f,l,s);
    printf ("Error number %x (%ld).\n", err, err);
    if (lpMsgBuf) printf ("%s", lpMsgBuf);
    printf ("Program terminating.\n");


    if (lpMsgBuf) LocalFree (lpMsgBuf);


    SetLastError(err);

    return 1;
}

#ifndef UNIX
/*--------------------------------------------------------------------*/
/* GetRecipientCert enumerates the certificates in a store and finds*/
/* the first certificate that has an AT_EXCHANGE key. If a certificate */
/* is found, a pointer to that certificate is returned.  */

PCCERT_CONTEXT GetRecipientCert (
    HCERTSTORE hCertStore) /* the handle of the store to be searched.*/
{ 
    PCCERT_CONTEXT pCertContext = NULL; 
    BOOL fMore = TRUE; 
    DWORD dwSize = 0; 
    CRYPT_KEY_PROV_INFO* pKeyInfo = NULL; 
    DWORD PropId = CERT_KEY_PROV_INFO_PROP_ID; 
    
    /*-------------------------------------------------------------------- */
    /* Find certificates in the store until the end of the store */
    /* is reached or a certificate with an AT_KEYEXCHANGE key is found. */
    
    while(fMore && (pCertContext = CertFindCertificateInStore( 
	hCertStore, /* Handle of the store to be searched. */
	0,          /* Encoding type. Not used for this search. */
	0,          /* dwFindFlags. Special find criteria. */
	/* Not used in this search. */
	CERT_FIND_PROPERTY, 
	/* Find type. Determines the kind of search */
	/* to be done. In this case, search for */
	/* certificates that have a specific */
	/* extended property. */
	&PropId,    /* pvFindPara. Gives the specific */
	/* value searched for, here the identifier */
	/* of an extended property. */
	pCertContext)) != 0) 
	/* pCertContext is NULL for the  */
	/* first call to the function. */
	/* If the function were being called */
	/* in a loop, after the first call */
	/* pCertContext would be the pointer */
	/* returned by the previous call. */
    {
	/*------------------------------------------------------------- */
	/* For simplicity, this code only searches */
	/* for the first occurrence of an AT_KEYEXCHANGE key. */
	/* In many situations, a search would also look for a */
	/* specific subject name as well as the key type. */
	
	/*------------------------------------------------------------- */
	/* Call CertGetCertificateContextProperty once to get the */
	/* returned structure size. */
	
	if(!(CertGetCertificateContextProperty( 
	    pCertContext, 
	    CERT_KEY_PROV_INFO_PROP_ID, 
	    NULL, &dwSize))) 
	{ 
	    HandleErrorFL("Error getting key property. Handeled..."); 
	} 
	
	/*-------------------------------------------------------------- */
	/* Allocate memory for the returned structure. */
	
	if(pKeyInfo) 
	    free(pKeyInfo); 
	pKeyInfo = (CRYPT_KEY_PROV_INFO*)malloc(dwSize);
	if(!pKeyInfo) { 
	    HandleErrorFL("Error allocating memory for pKeyInfo."); 
	} 
	
	/*-------------------------------------------------------------- */
	/* Get the key information structure. */
	
	if(!(CertGetCertificateContextProperty( 
	    pCertContext, 
	    CERT_KEY_PROV_INFO_PROP_ID, 
	    pKeyInfo, 
	    &dwSize))) 
	{ 
	    HandleErrorFL("The second call to the function failed."); 
	} 
	
	/*------------------------------------------- */
	/* Check the dwKeySpec member for an exchange key. */
	
	if (pKeyInfo->dwKeySpec == AT_KEYEXCHANGE)
	    fMore = FALSE;
    }    /* End of while loop */
    
    if(pKeyInfo)
	free(pKeyInfo);
    return (pCertContext);
} /* End of GetRecipientCert */



/*--------------------------------------------------------------------*/
/* Программа по заданному сертификату определяет наличие секретного ключа*/
/* и загружает требуемый провайдер.*/
/* Для определения провайдера используется функция CryptAcquireCertificatePrivateKey, */
/* если она присутствует в crypt32.dll. Иначе производистя поиск ключа по сертификату в справочнике.*/
/* Функция должна быть только*/
/*  Windows NT/2000: Requires Windows NT 4.0 SP3 or later (or Windows NT 4.0 with Internet Explorer 3.02 or later).*/
/*  Windows 95/98: Unsupported.*/

typedef BOOL (WINAPI *CPCryptAcquireCertificatePrivateKey)(
  PCCERT_CONTEXT pCert,        
  DWORD dwFlags,               
  void *pvReserved,            
  HCRYPTPROV *phCryptProv,     
  DWORD *pdwKeySpec,           
  BOOL *pfCallerFreeProv       
);

static CPCryptAcquireCertificatePrivateKey WantContext;
  

BOOL WINAPI CryptAcquireProvider (char *pszStoreName, PCCERT_CONTEXT cert, HCRYPTPROV *phCryptProv, DWORD *keytype, BOOL *release)
{
   /*--------------------------------------------------------------------*/
    
    HANDLE           hCertStore = 0;        
    PCCERT_CONTEXT   pCertContext=NULL;      
    CRYPT_KEY_PROV_INFO *pCryptKeyProvInfo;
    /*char pszNameString[256];*/
    void*            pvData;
    DWORD            cbData;
    DWORD            dwPropId = 0;   /* 0 must be used on the first*/
    CRYPT_HASH_BLOB  blob;
    BOOL	    ret = FALSE;
    DWORD	    len = 0;
    BYTE	    data[128];
    USES_CONVERSION;
    HMODULE	    crypt32;
    wchar_t	    wszStoreName[80];
    _lpw, _lpa;
    /*--------------------------------------------------------------------*/
    /* Попробуем определеить наличие функции CryptAcquireCertificatePrivateKey*/

    crypt32 = GetModuleHandle ("crypt32.dll");
    if (crypt32) {
        WantContext = (CPCryptAcquireCertificatePrivateKey) GetProcAddress (crypt32,"CryptAcquireCertificatePrivateKey");
	if (WantContext) 
	    return WantContext(cert, 0, NULL, phCryptProv,keytype,release);
	else 
	    printf ("CRYPT32.DLL not supported CryptAcquireCertificatePrivateKey function\n");
    }

    /*--------------------------------------------------------------------*/
    /* Open the named system certificate store. */
    /*    if (!( hCertStore = CertOpenSystemStore(*/
    /*	0,*/
    /*	pszStoreName))) {*/
    if (MultiByteToWideChar(CP_OEMCP, MB_ERR_INVALID_CHARS, 
			    pszStoreName, -1, 
			    wszStoreName, sizeof(wszStoreName)/sizeof(wszStoreName[0])) <= 0) {
	DebugErrorFL("MultiByteToWideChar");
	goto err;
    }
    hCertStore = CertOpenStore(
			    CERT_STORE_PROV_SYSTEM, /* LPCSTR lpszStoreProvider*/
			    0,			    /* DWORD dwMsgAndCertEncodingType*/
			    0,			    /* HCRYPTPROV hCryptProv*/
			    CERT_STORE_OPEN_EXISTING_FLAG|CERT_STORE_READONLY_FLAG|
			    CERT_SYSTEM_STORE_CURRENT_USER, /* DWORD dwFlags*/
			    wszStoreName		    /* const void *pvPara*/
			    );
    if (!hCertStore) {
	DebugErrorFL("CertOpenStore");
	goto err;
    }
	
    /* Определим значение дополненя subjectKeyIdentifier*/
    /* If nonexistent, searches for the szOID_SUBJECT_KEY_IDENTIFIER extension. */
    /* If that fails, a SHA1 hash is done on the certificate's SubjectPublicKeyInfo */
    /* to produce the identifier values*/
    len = sizeof(data);
    ret = CertGetCertificateContextProperty(cert, 
	CERT_KEY_IDENTIFIER_PROP_ID,
	(void*) data,
	&len);

    if (! ret) {
	DebugErrorFL("CertGetCertificateContextProperty");
	goto err;
    }
    blob.cbData = len;
    blob.pbData = data;

    /* Найдем сертификат с соответствующим значением*/
    pCertContext = CertFindCertificateInStore (hCertStore, 
	TYPE_DER,
	0, 
	CERT_FIND_KEY_IDENTIFIER,
	&blob,
	NULL);

    if (! pCertContext ) goto err;
    	/*--------------------------------------------------------------------*/
	/* In a loop, find all of the property IDs for the given certificate.*/
	/* The loop continues until the CertEnumCertificateContextProperties */
	/* returns 0.*/
	
	while((dwPropId = CertEnumCertificateContextProperties(
	    pCertContext, /* the context whose properties are to be listed.*/
	    dwPropId)) != 0)    /* number of the last property found. Must be*/
	    /* 0 to find the first property ID.*/
	{
	    /*--------------------------------------------------------------------*/
	    /* Each time through the loop, a property ID has been found.*/
	    /* Print the property number and information about the property.*/
	    
	    printf("Property # %d found->", dwPropId);
	    switch(dwPropId)
	    {
	    case CERT_FRIENDLY_NAME_PROP_ID:
		{
		    /*--------------------------------------------------------------------*/
		    /*  Retrieve the actual friendly name certificate property.*/
		    /*  First, get the length of the property setting the*/
		    /*  pvData parameter to NULL to get a value for cbData*/
		    /*  to be used to allocate memory for the pvData buffer.*/
		    printf("FRIENDLY_NAME_PROP_ID ");
		    if(!(CertGetCertificateContextProperty(
			pCertContext, 
			dwPropId, 
			NULL, 
			&cbData)))
			goto err;
		    /*--------------------------------------------------------------------*/
		    /* The call succeeded. Use the size to allocate memory for the */
		    /* property.*/
		    pvData = (void*)malloc(cbData);
		    if(!pvData) {
			HandleErrorFL("Memory allocation failed.");
		    }
		    /*--------------------------------------------------------------------*/
		    /* Allocation succeeded. Retrieve the property data.*/
		    if(!(CertGetCertificateContextProperty(
			pCertContext,
			dwPropId,
			pvData, 
			&cbData)))
		    {
			HandleErrorFL("Call #2 getting the data failed.");
		    }
		    else
		    {
			printf("\n  The friendly name is -> %s.", pvData);
			free(pvData);
		    }
		    break;
		}
	    case CERT_SIGNATURE_HASH_PROP_ID:
		{
		    printf("Signature hash ID. ");
		    break;
		}
	    case CERT_KEY_PROV_HANDLE_PROP_ID:
		{
		    printf("KEY PROVIDER HANDLE.");
		    break;
		}
	    case CERT_KEY_PROV_INFO_PROP_ID:
		{
		    printf("KEY PROV INFO PROP ID.");
		    if(!(CertGetCertificateContextProperty(
			pCertContext,  /* A pointer to the certificate*/
			/* where the property will be set.*/
			dwPropId,      /* An identifier of the property to get. */
			/* In this case,*/
			/* CERT_KEY_PROV_INFO_PROP_ID*/
			NULL,          /* NULL on the first call to get the*/
			/* length.*/
			&cbData)))     /* The number of bytes that must be*/
			/* allocated for the structure.*/
			goto err;
		    pCryptKeyProvInfo = 
			(CRYPT_KEY_PROV_INFO *)malloc(cbData);
		    if(!pCryptKeyProvInfo)
		    {
			HandleErrorFL("Error in allocation of memory.");
		    }
		    if(CertGetCertificateContextProperty(
			pCertContext,
			dwPropId,
			pCryptKeyProvInfo,
			&cbData))
		    {
			printf("\nThe current key container is %S\n",
			    pCryptKeyProvInfo->pwszContainerName);
			printf("\nThe provider name is:%S",
			    pCryptKeyProvInfo->pwszProvName);
			*keytype = pCryptKeyProvInfo->dwKeySpec;
			*release = TRUE;
			/* Откроем провайдер*/
			ret = CryptAcquireContext (phCryptProv, 
			    W2A(pCryptKeyProvInfo->pwszContainerName),
			    W2A(pCryptKeyProvInfo->pwszProvName),
			    pCryptKeyProvInfo->dwProvType,
			    0);
    			free(pCryptKeyProvInfo);
			if (ret)
			    break;
		    }
		    else
		    {
			free(pCryptKeyProvInfo);
			HandleErrorFL("The property was not retrieved.");
		    }
		    break;
		}
	    case CERT_SHA1_HASH_PROP_ID:
		{
		    printf("SHA1 HASH id.");
		    break;
		}
	    case CERT_MD5_HASH_PROP_ID:
		{
		    printf("md5 hash id. ");
		    break;
		}
	    case CERT_KEY_CONTEXT_PROP_ID:
		{
		    printf("KEY CONTEXT PROP id.");
		    break;
		}
	    case CERT_KEY_SPEC_PROP_ID:
		{
		    printf("KEY SPEC PROP id.");
		    break;
		}
	    case CERT_ENHKEY_USAGE_PROP_ID:
		{
		    printf("ENHKEY USAGE PROP id.");
		    break;
		}
	    case CERT_NEXT_UPDATE_LOCATION_PROP_ID:
		{
		    printf("NEXT UPDATE LOCATION PROP id.");
		    break;
		}
	    case CERT_PVK_FILE_PROP_ID:
		{
		    printf("PVK FILE PROP id. ");
		    break;
		}
	    case CERT_DESCRIPTION_PROP_ID:
		{
		    printf("DESCRIPTION PROP id. ");
		    break;
		}
	    case CERT_ACCESS_STATE_PROP_ID:
		{
		    printf("ACCESS STATE PROP id. ");
		    break;
		}
	    case CERT_SMART_CARD_DATA_PROP_ID:
		{
		    printf("SMART_CARD DATA PROP id. ");
		    break;
		}
	    case CERT_EFS_PROP_ID:
		{
		    printf("EFS PROP id. ");
		    break;
		}
	    case CERT_FORTEZZA_DATA_PROP_ID:
		{
		    printf("FORTEZZA DATA PROP id.");
		    break;
		}
	    case CERT_ARCHIVED_PROP_ID:
		{
		    printf("ARCHIVED PROP id.");
		    break;
		}
	    case CERT_KEY_IDENTIFIER_PROP_ID:
		{
		    printf("KEY IDENTIFIER PROP id. ");
		    break;
		}
	    case CERT_AUTO_ENROLL_PROP_ID:
		{
		    printf("AUTO ENROLL id. ");
		    break;
		}
        }  /* end switch*/
        printf("\n");
      } /* end the inner while loop. This is the end of the display of*/
      /* a single property of a single certificate.*/
/*--------------------------------------------------------------------*/
/* Free Memory and close the open store.*/
    if(pCertContext)
    {
	CertFreeCertificateContext(pCertContext);
    }
    if(hCertStore)
	CertCloseStore(hCertStore, CERT_CLOSE_STORE_FORCE_FLAG);

err:
    return ret;
}
#endif /* UNIX */

/*----------------------------------------------------------------------*/
/* Чтение файла*/
#ifdef HAVE_MAPVIEWOFFILE
int get_file_data_pointer (const char *infile, size_t *len,
    unsigned char **buffer)
{
    DWORD dwSize;
    HANDLE hFile;
    HANDLE hMap;
    unsigned char *pStart;

    if (!infile || !len || !buffer) {
	fprintf (stderr, "Invalid argument specified\n");
	return 0;
    }
    hFile = CreateFile(infile, GENERIC_READ, 0,
	NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL|FILE_FLAG_SEQUENTIAL_SCAN,
	NULL);
    if (INVALID_HANDLE_VALUE == hFile) {
	fprintf (stderr, "Unable to open file\n");
	return 0;
    }
    dwSize = GetFileSize (hFile, NULL);
    hMap = CreateFileMapping(hFile, NULL, PAGE_READONLY, 0, 0, NULL);
    if (NULL == hMap) {
	fprintf (stderr, "Unable to create map file\n");
	CloseHandle(hFile);
	return 0;
    }
    pStart = MapViewOfFile(hMap, FILE_MAP_READ, 0, 0, 0);
    if (NULL == pStart) {
	fprintf (stderr, "Unable to map file into memory\n");
	CloseHandle(hMap);
	CloseHandle(hFile);
	return 0;
    }
    CloseHandle(hMap);
    CloseHandle(hFile);
    *len = dwSize;
    *buffer = pStart;
    return 1;
}

int
release_file_data_pointer (unsigned char *buffer)
{
    if (buffer) UnmapViewOfFile ((char *)buffer);
    return 1;
}
#else /* */

int
get_file_data_pointer (const char *infile, size_t *len, unsigned char **buffer)
{
    FILE *f = NULL;
    size_t mem_len = 0;
    int ret = 0;
    unsigned char *mem_tbr = NULL;
    
    /*--------------------------------------------------------------------*/
    /* Откроем файл */
    if (infile  == NULL) {
	fprintf (stderr, "No file specified\n");
	goto err;
    }
    f = fopen (infile, "rb");
    if (!f) {
	fprintf (stderr, "Cannot open input file\n");
	goto err;
    }

    /*--------------------------------------------------------------------*/
    /* Прочитает файл в память*/
    
    mem_len = 0;
    mem_tbr = NULL;
    if (fseek(f, 0, SEEK_END) == 0) {
        mem_len = (size_t)ftell(f);

        if ((long)mem_len == ftell(f)) {
            if ((mem_tbr = (unsigned char*)malloc (mem_len)) == NULL) {
                goto err;
            }
        } else {
            mem_len = 0;
        }
        if (fseek(f, 0, SEEK_SET)) {
            goto err;
        }
        if (mem_tbr) {
            if (fread (mem_tbr, mem_len, 1, f) != 1) {
                goto err;
            }
        }
    }

    while (!feof(f)) {
	int r = 0;
	BYTE tmp[10240];
	r = fread (tmp, sizeof(tmp[0]), sizeof(tmp)/sizeof(tmp[0]), f);
        if (r == 0 && ferror(f)) {
            goto err;
        }
	mem_tbr = (unsigned char*)realloc (mem_tbr, mem_len+r);
	memcpy (&mem_tbr[mem_len], tmp, r);
	mem_len += r;
    }
    *len = mem_len;
    *buffer = mem_tbr;
    ret = 1;
err:
    if (f) fclose (f);
    f = NULL;
    if (!ret) {
        if (mem_tbr) free(mem_tbr);
        mem_tbr = NULL;
    }
    return ret;
}

int
release_file_data_pointer (unsigned char *buffer)
{
    if (buffer) free (buffer);
    return 1;
}
#endif

/*----------------------------------------------------------------------*/
/* Запись в файл*/
int write_file (const char *file, long len, const unsigned char *buffer)
{
    FILE *f = NULL;
    int ret = 0;
    
    f = fopen (file, "wb");
    if (!f) {
	fprintf (stderr, __FILE__":%d:%s:%s", __LINE__, file,
	    "Cannot open file for writing\n");
	goto err;
    }
    if (1 != fwrite (buffer, len, 1, f)) {
	fprintf (stderr, __FILE__":%d:%s:%s", __LINE__, file,
	    "Cannot write to file\n");
	goto err;
    }
    if (ferror (f)) {
	fprintf (stderr, __FILE__":%d:%s:%s", __LINE__, file,
	    "Write to file error (ferror)\n");
	goto err;
    }
    if (fclose (f)) {
	fprintf (stderr, __FILE__":%d:%s:%s", __LINE__, file,
	    "Write to file error (fclose)\n");
	goto err;
    }
    f = NULL;
    ret = 1;
err:
    if (f) fclose (f);
    return ret;
}

/*----------------------------------------------------------------------*/
/* Форматное чтение файла*/
int get_file_data_pointer_fmt (const FMTFILEDESCR *infile, size_t *len,
    unsigned char **buffer)
{
    int ret = 0;
    size_t mem_len = 0;
    unsigned char *mem_file = NULL;
    size_t ret_len = 0;
    unsigned char *ret_file = NULL;
    
    /*--------------------------------------------------------------------*/
    /* Откроем файл */
    if (!infile || infile->mSizeOF != sizeof(*infile) || !infile->mFileName
	|| !len || !buffer) {
	fprintf (stderr, __FILE__":%d:%s", __LINE__, "No file specified\n");
	goto err;
    }


    /*--------------------------------------------------------------------*/
    /* Прочитает файл в память*/
    if (!get_file_data_pointer (infile->mFileName, &mem_len, &mem_file))
	goto err;

    switch (infile->mFormat) {
    case DER:
        ret_len = mem_len;
        ret_file = mem_file;
        break;
    case BASE64:
        if(!base64_decode(mem_file, (DWORD)mem_len, NULL, (DWORD*)&ret_len)) {
	    fprintf (stderr, __FILE__":%d:%s:%s", __LINE__,
		infile->mFileName, "BASE64 format error on input file\n");
	    goto err;
        }
	ret_file = (unsigned char*)malloc(ret_len);
        if (!ret_file) {
	    fprintf (stderr, __FILE__":%d:%s:%s", __LINE__,
		infile->mFileName, "no memory\n");
	    goto err;
        }
        if(!base64_decode(mem_file, (DWORD)mem_len, ret_file, (DWORD*)&ret_len)) {
	    fprintf (stderr, __FILE__":%d:%s:%s", __LINE__,
		infile->mFileName, "BASE64 format error on input file\n");
	    goto err;
        }
	release_file_data_pointer (mem_file);
        break;
    case BASE64HDR:
        if(!base64hdr_decode(infile->mB64Hdrs, mem_file, (DWORD)mem_len, NULL,
	    (DWORD *)&ret_len)) {
	    fprintf (stderr, __FILE__":%d:%s:%s", __LINE__,
		infile->mFileName, "BASE64HDR format error on input file\n");
	    goto err;
        }
	ret_file = (unsigned char*)malloc(ret_len);
        if (!ret_file) {
	    fprintf (stderr, __FILE__":%d:%s:%s", __LINE__,
		infile->mFileName, "no memory\n");
	    goto err;
        }
        if(!base64hdr_decode(infile->mB64Hdrs, mem_file, (DWORD)mem_len, ret_file,
	    (DWORD *)&ret_len)) {
	    fprintf (stderr, __FILE__":%d:%s:%s", __LINE__,
		infile->mFileName, "BASE64HDR format error on input file\n");
	    goto err;
        }
	release_file_data_pointer (mem_file);
        break;
    default:
	fprintf (stderr, __FILE__":%d:%s:%s", __LINE__, infile->mFileName,
	    "No file specified\n");
	goto err;
    }
    ret = 1;
    *len = ret_len;
    *buffer = ret_file;
err:
    if (!ret) {
	release_file_data_pointer (mem_file);
        if (ret_file) free(ret_file);
    }
    return ret;
}

int release_file_data_pointer_fmt (const FMTFILEDESCR *infile,
    unsigned char *buffer)
{
    if (!infile)
	return 0;
    switch (infile->mFormat) {
    case DER:
	release_file_data_pointer (buffer);
	break;
    case BASE64:
    case BASE64HDR:
	if (buffer) free (buffer);
	break;
    default:
	return 0;
    }
    return 1;
}

/*----------------------------------------------------------------------*/
/* Форматная запись в файл*/
int
write_file_fmt (const FMTFILEDESCR *file, size_t len, const unsigned char *buffer)
{
    int ret = 0;
    size_t mem_len = 0;
    BYTE *mem_tbw = NULL;
    const unsigned char *cmem_tbw = NULL;
    
    /*--------------------------------------------------------------------*/
    /* Откроем файл */
    if (!file || file->mSizeOF != sizeof(*file) || !file->mFileName || !buffer) {
	fprintf (stderr, __FILE__":%d:%s", __LINE__, "No file specified\n");
	goto end;
    }
  
    switch (file->mFormat) {
    case DER:
        cmem_tbw = (const unsigned char *)buffer;
        mem_len = len;
        break;
    case BASE64:
        if(!base64_encode(buffer, (DWORD)len, NULL, (DWORD *)&mem_len)) {
	    fprintf (stderr, __FILE__":%d:%s:%s", __LINE__, file->mFileName,
		"BASE64 format error on output file\n");
	    goto end;
        }
	mem_tbw = (BYTE*)malloc(mem_len);
        if (!mem_tbw) {
	    fprintf (stderr, __FILE__":%d:%s:%s", __LINE__, file->mFileName,
		"no memory\n");
	    goto end;
        }
        if(!base64_encode(buffer, (DWORD)len, mem_tbw, (DWORD *)&mem_len)) {
	    fprintf (stderr, __FILE__":%d:%s:%s", __LINE__, file->mFileName,
		"BASE64 format error on input file\n");
	    goto end;
        }
        cmem_tbw = mem_tbw;
        break;
    case BASE64HDR:
        if(!base64hdr_encode(file->mB64Hdrs, buffer, (DWORD)len, NULL,
	    (DWORD *)&mem_len)) {
	    fprintf (stderr, __FILE__":%d:%s:%s", __LINE__, file->mFileName,
		"BASE64HDR format error on output file\n");
	    goto end;
        }
	mem_tbw = (BYTE*)malloc(mem_len);
        if (!mem_tbw) {
	    fprintf (stderr, __FILE__":%d:%s:%s", __LINE__, file->mFileName,
		"no memory\n");
	    goto end;
        }
        if(!base64hdr_encode(file->mB64Hdrs, buffer, (DWORD)len, mem_tbw,
	    (DWORD *)&mem_len)) {
	    fprintf (stderr, __FILE__":%d:%s:%s", __LINE__, file->mFileName,
		"BASE64HDR format error on input file\n");
	    goto end;
        }
        cmem_tbw = mem_tbw;
        break;
    default:
	fprintf (stderr, __FILE__":%d:%s:%s", __LINE__, file->mFileName,
	    "No file specified\n");
        goto end;
    }
    if (!write_file (file->mFileName, mem_len, cmem_tbw)) {
	fprintf (stderr, __FILE__":%d:%s:%s", __LINE__, file->mFileName,
	    "Cannot open input file\n");
	goto end;
    }
    ret = 1;
end:
    if (mem_tbw) {
        free(mem_tbw);
        mem_tbw = NULL;
    }
    return ret;
}

#ifndef UNIX
/*--------------------------------------------------------------------*/
/*  Функция чтения сертификата из файла*/
PCCERT_CONTEXT read_cert_from_file (const char *fname)
{
    BYTE *cert = NULL;
    size_t len = 0;
    PCCERT_CONTEXT ret = NULL;

    if (!get_file_data_pointer (fname, &len, &cert))
	return NULL;
    
    ret = MyCertCreateCertificateContext (X509_ASN_ENCODING
	| PKCS_7_ASN_ENCODING, cert, len);
    if (!ret) {
	DebugErrorFL("CertCreateCertificateContext");
    }
    release_file_data_pointer (cert);
    return ret;
}

/*--------------------------------------------------------------------*/
/*  Функция чтения сертификата из файла*/
PCCERT_CONTEXT read_cert_from_file_fmt (const FMTFILEDESCR *file)
{
    BYTE *cert = NULL;
    size_t cert_len = 0;
    PCCERT_CONTEXT ret = NULL;

    if (!get_file_data_pointer_fmt (file, &cert_len, &cert))
        goto end;

    ret = MyCertCreateCertificateContext (X509_ASN_ENCODING
	| PKCS_7_ASN_ENCODING, cert, cert_len);
    if (!ret) {
 	DebugErrorFL("MyCertCreateCertificateContext");
    }
end:
    release_file_data_pointer_fmt (file, cert);
    return ret;
}

/*--------------------------------------------------------------------*/
/*  Функция чтения сертификата из системного справочника сертификатов 'MY'*/

PCCERT_CONTEXT read_cert_from_my (char *subject_name)
{
    PCCERT_CONTEXT  ret = NULL;
    HANDLE	    hCertStore = 0;        
    USES_CONVERSION;
    _lpw;

    /*--------------------------------------------------------------------*/
    /* Открываем справочник 'MY'*/
    /*    if (!( hCertStore = CertOpenSystemStore(0,"MY"))) {*/
    hCertStore = CertOpenStore(
			    CERT_STORE_PROV_SYSTEM, /* LPCSTR lpszStoreProvider*/
			    0,			    /* DWORD dwMsgAndCertEncodingType*/
			    0,			    /* HCRYPTPROV hCryptProv*/
			    CERT_STORE_OPEN_EXISTING_FLAG|CERT_STORE_READONLY_FLAG|
			    CERT_SYSTEM_STORE_CURRENT_USER, /* DWORD dwFlags*/
			    L"MY"		    /* const void *pvPara*/
			    );
    if (!hCertStore) {
	DebugErrorFL("CertOpenStore");
	return ret;
    }

    /* Найдем сертификат с соответствующим значением*/
    ret = CertFindCertificateInStore (hCertStore, 
	TYPE_DER,
	0, 
	CERT_FIND_SUBJECT_STR,
	A2W(subject_name),
	NULL);
    if (!ret) {
	DebugErrorFL("CertFindCertificateInStore");
    }

    if (!CertCloseStore (hCertStore, 0)) {
	DebugErrorFL("CertCloseStore");
    }

    return ret;
}
/*--------------------------------------------------------------------*/
/*  Функция чтения сертификата из системного справочника сертификатов компьютера'MY'*/

PCCERT_CONTEXT read_cert_from_MY (char *subject_name)
{
    PCCERT_CONTEXT  ret = NULL;
    HANDLE	    hCertStore = 0;        
    USES_CONVERSION;
    _lpw;

    /*--------------------------------------------------------------------*/
    /* Открываем справочник 'MY'*/
    /*    if (!( hCertStore = CertOpenSystemStore(0,"MY"))) {*/
    hCertStore = CertOpenStore(
			    CERT_STORE_PROV_SYSTEM, /* LPCSTR lpszStoreProvider*/
			    0,			    /* DWORD dwMsgAndCertEncodingType*/
			    0,			    /* HCRYPTPROV hCryptProv*/
			    CERT_STORE_OPEN_EXISTING_FLAG|CERT_STORE_READONLY_FLAG|
			    CERT_SYSTEM_STORE_LOCAL_MACHINE, /* DWORD dwFlags*/
			    L"MY"		    /* const void *pvPara*/
			    );


    if (!hCertStore) {
	DebugErrorFL("CertOpenStore");
	return ret;
    }
    ret = CertFindCertificateInStore (hCertStore, 
	TYPE_DER,
	0, 
	CERT_FIND_SUBJECT_STR,
	A2W(subject_name),
	NULL);	

    if (!ret) {
	DebugErrorFL("CertFindCertificateInStore");
    }

    if (!CertCloseStore (hCertStore, 0)) {
	DebugErrorFL("CertCloseStore");
    }

    return ret;
}

// Преобразование строк Ansi Unicode
LPWSTR WINAPI AtlA2WHelper(LPWSTR lpw, LPCSTR lpa, int nChars, UINT acp)
{
    /* verify that no illegal character present*/
    /* since lpw was allocated based on the size of lpa*/
    /* don't worry about the number of chars*/
    lpw[0] = '\0';
    MultiByteToWideChar(acp, 0, lpa, -1, lpw, nChars);
    return lpw;
}

// Преобразование строк Unicode Ansi 
LPSTR WINAPI AtlW2AHelper(LPSTR lpa, LPCWSTR lpw, int nChars, UINT acp)
{
    /* verify that no illegal character present*/
    /* since lpa was allocated based on the size of lpw*/
    /* don't worry about the number of chars*/
    lpa[0] = '\0';
    WideCharToMultiByte(acp, 0, lpw, -1, lpa, nChars, NULL, NULL);
    return lpa;
}
#endif /* UNIX */

#if !defined(lint) && !defined(NO_RCSID) && !defined(OK_MODULE_VERSION)
static volatile const char rcsid[] = "\n$Id: tmain.c,v 1.59.4.4 2002/08/28 07:06:29 vasilij Exp $";
#endif
