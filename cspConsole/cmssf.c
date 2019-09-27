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
 * \file $RCSfile: cmssf.c,v $
 * \version $Revision: 1.7 $
 * \date $Date: 2002/01/14 12:17:53 $
 * \author $Author: lse $
 *
 * \brief Создание и проверка ЭЦП CMS Signed.
 *
 * Тест создания и проверки CMS Signed.
 */

#define CMSG_SIGNED_ENCODE_INFO_HAS_CMS_FIELDS
#define CMSG_ENVELOPED_ENCODE_INFO_HAS_CMS_FIELDS
#define CRYPT_SIGN_MESSAGE_PARA_HAS_CMS_FIELDS

#include "tmain.h"

#if !defined(CMSG_SIGNED_ENCODE_INFO_HAS_CMS_FIELDS)
# error !defined(CMSG_SIGNED_ENCODE_INFO_HAS_CMS_FIELDS)
#endif /* !defined(CMSG_SIGNED_ENCODE_INFO_HAS_CMS_FIELDS) */

#if !defined(CMSG_ENVELOPED_ENCODE_INFO_HAS_CMS_FIELDS)
# error !defined(CMSG_ENVELOPED_ENCODE_INFO_HAS_CMS_FIELDS)
#endif /* !defined(CMSG_ENVELOPED_ENCODE_INFO_HAS_CMS_FIELDS) */

#if !defined(CRYPT_SIGN_MESSAGE_PARA_HAS_CMS_FIELDS)
# error !defined(CRYPT_SIGN_MESSAGE_PARA_HAS_CMS_FIELDS)
#endif /* !defined(CRYPT_SIGN_MESSAGE_PARA_HAS_CMS_FIELDS) */

static int CMSdo_sign (char *infile, char *certfile, char *outfile,
    int detached, char *OID);
static int CMSdo_verify (char *infile, char *certfile, char *outfile,
    char *signature_filename, int detached);
static PCCERT_CONTEXT WINAPI my_get_cert (void *pvGetArg,
    DWORD dwCertEncodingType, PCERT_INFO pSignerId, HCERTSTORE hMsgCertStore);

int
main_CMSsign_sf (int argc, char **argv)
{
    char *in_filename = NULL;    
    char * out_filename = NULL;
    char * certfile = NULL;
    int ret = 0;
    int sign = 0;
    int verify = 0;
    int print_help = 0;
    int detached = 0;
    char *signature_filename = NULL;
    char OID[64] = szOID_CP_GOST_R3411;
    int c;
   
    
    static struct option long_options[] = {
	{"in",		required_argument,	NULL, 'i'},
	{"out",		required_argument,	NULL, 'o'},
	{"sign",	no_argument,		NULL, 'g'},
	{"verify",	no_argument,		NULL, 'v'},
	{"cert",	required_argument,	NULL, 'x'},
	{"detached",	no_argument,		NULL, 'd'},
	{"alg",		required_argument,	NULL, 'a'},
	{"signature",	required_argument,	NULL, 's'},
	{"help",	no_argument,		NULL, 'h'},
	{0, 0, 0, 0}
    };

    while ((c = getopt_long_only (argc, argv, "", long_options, (int *)0)) != EOF) {
	switch (c) {
	case 'i':
	     in_filename = optarg;
	    break;
	case 'o':
	     out_filename = optarg;
	    break;
	case 's':
	     signature_filename = optarg;
	    break;
	case 'd':
	    detached = 1;
	    break;
	case 'g':
	    sign = 1;
	    break;
	case 'v':
	    verify = 1;
	    break;
	case 'x':
	    certfile = optarg;
	    break;
	case 'a':
	    strncpy (OID, optarg, sizeof (OID)-1);
	    break;
	case 'h':
	    ret = 1;
	    print_help = 1;
	    goto bad;
	case '?':
	default:
	    goto bad;
	}
    }
    if (c != EOF) {
	print_help = 1;
	goto bad;
    }

    if (!sign && !verify ) {
	print_help = 1;
	goto bad;
    }

    if (sign) {
	ret = CMSdo_sign (in_filename, certfile, out_filename, detached, OID);
    }
    else if (verify) {
	ret = CMSdo_verify (in_filename, certfile, out_filename,
	    signature_filename, detached);
    }
    else {
	print_help = 1;
    }

bad:
    if (print_help) {
	const char *fmt =
"%s -cmssfsign [options]\n"
SoftName " generate CMS Signed message \n"
"using CAPI simplified message function type\n"
"options:\n"
"  -in arg        input filename to be signed or verified\n"
"  -out arg       output filename (whole CMS or detached only)\n"
"  -cert          use certificate from file to acquire context, sign or verify\n"
"  -sign          sign input file\n"
"  -verify        verify signed file, specified by input filename\n"
"  -signature     detached signature file\n"
"  -detached      save signature in detached file\n"
"  -alg           perform Hash with OID.\n"
"                 default: %s (szOID_CP_GOST_R3411)\n"
"                 additional alg: SHA1 %s (szOID_OIWSEC_sha1)\n"
"                               : MD5  %s (szOID_RSA_MD5)\n"
"  -help          print this help\n\n";

	fprintf(stderr, fmt, prog,
	    szOID_CP_GOST_R3411, szOID_OIWSEC_sha1, szOID_RSA_MD5);
    }

    return ret;
}
    
/*--------------------------------------------------------------------*/
/**/
/* Формирование ЭЦП*/
/**/
/*--------------------------------------------------------------------*/

static int CMSdo_sign (char *infile, char *certfile, char *outfile, int detached,
    char *OID)
{
    /*--------------------------------------------------------------------*/
    
    HCRYPTPROV hCryptProv;                      /* CSP handle*/
    PCCERT_CONTEXT pUserCert = NULL;		/* User certificate to be used*/

    DWORD keytype = 0;
    BOOL should_release_ctx = FALSE;
    int ret = 0;
    BYTE *mem_tbs = NULL;
    size_t mem_len = 0;

    CRYPT_SIGN_MESSAGE_PARA param;
    DWORD MessageSizeArray[1];
    const BYTE* MessageArray[1];
    DWORD signed_len = 0;
    BYTE *signed_mem = NULL;
    
    /*--------------------------------------------------------------------*/
    /*  Используем сертификат из файла для инициализации контекста*/

    if (! certfile)
    {
	fprintf (stderr, "No user cert specified\n");
	goto err;
    }
    if (! infile) {
	fprintf (stderr, "No input file was specified\n");
	goto err;
    }

    pUserCert = read_cert_from_my (certfile);
    if (!pUserCert)
    {
	HandleErrorFL("Certificate not found\n");
	goto err;
    }
    
    ret = CryptAcquireCertificatePrivateKey(
	pUserCert,        
	0,		/*DWORD dwFlags,               */
	NULL,            
	&hCryptProv,     
	&keytype,           /* returned key type AT_SIGNATURE ! AT_KEYEXCAHGE*/
	&should_release_ctx  /* if FALSE DO NOT Release CTX*/
	);
    if (ret) {
	printf("A CSP has been acquired. \n");
    }
    else {
	HandleErrorFL("Cryptographic context could not be acquired.");
    }

    /*--------------------------------------------------------------------*/
    /* Откроем файл который будем подписывать*/
    if (!get_file_data_pointer (infile, &mem_len, &mem_tbs)) {
	fprintf (stderr, "Cannot open input file\n");
	goto err;
    }

    /*--------------------------------------------------------------------*/
    /* Установим параметры*/
    
    ZeroMemory (&param, sizeof (param));
    param.cbSize = sizeof(CRYPT_SIGN_MESSAGE_PARA);
    param.dwMsgEncodingType = TYPE_DER;
    param.pSigningCert = pUserCert;
    
    param.HashAlgorithm.pszObjId = OID; /*szOID_RSA_MD5;*/
    param.HashAlgorithm.Parameters.cbData = 0;
    param.HashAlgorithm.Parameters.pbData = NULL;
    
    param.pvHashAuxInfo = NULL;	/* not used*/
    
    param.cMsgCert = 0;		/* не вклачаем сертификат отправителя*/

    param.rgpMsgCert = NULL;

    param.cAuthAttr = 0;
    param.dwInnerContentType = 0;
    param.cMsgCrl = 0;
    param.cUnauthAttr = 0;
    /*
    dwFlags 
    Normally zero. If the encoded output is to be a CMSG_SIGNED inner content 
    of an outer cryptographic message such as a CMSG_ENVELOPED message, 
    the CRYPT_MESSAGE_BARE_CONTENT_OUT_FLAG must be set. 
    If it is not set, the message will be encoded as an inner content type
    of CMSG_DATA. 
    With Windows 2000, CRYPT_MESSAGE_ENCAPSULATED_CONTENT_OUT_FLAG can be set 
    to encapsulate non-data inner content into an OCTET STRING. 
    Also, CRYPT_MESSAGE_KEYID_SINGER_FLAG can be set to identify signers 
    by their Key Identifier and not their Issuer and Serial Number. 
    */
    param.dwFlags = 0;
    param.rgAuthAttr = NULL;
    
    MessageArray[0] = mem_tbs;
    MessageSizeArray[0] = mem_len;

    ret = CryptSignMessage(
	&param,
	detached,
	1,  /* количество detached или обих (всегда тогда 1) вложений*/
	MessageArray,
	MessageSizeArray,
	NULL,
	&signed_len);

    if (ret) {
	printf("Calculate message len OK \n");
    }
    else
    {
	HandleErrorFL("Calculate message len");
    }

    signed_mem = (BYTE*) malloc (signed_len);
    if (!signed_mem)
	goto err;


    ret = CryptSignMessage(
	&param,
	detached,
	1,  /* количество detached или обих (всегда тогда 1) вложений*/
	MessageArray,
	MessageSizeArray,
	signed_mem,
	&signed_len);

    if (ret) {
	printf("Signature was done\n");
    }
    else
    {
	HandleErrorFL("Signature was done");
    }

    if (outfile) {
	if (!write_file (outfile, signed_len, signed_mem)) {
	    printf ("Output file (%s) has been saved\n", outfile);
	}
   }

    
err:
    if (signed_mem) free (signed_mem);
    release_file_data_pointer (mem_tbs);
    return ret;
} /* End of do_sign*/


/*--------------------------------------------------------------------*/
/**/
/*  Проверка ЭЦП*/
/**/
/*--------------------------------------------------------------------*/

static 
int CMSdo_verify (char *infile, char *certfile, char *outfile,
    char *signature_filename, int detached)
{
    /*--------------------------------------------------------------------*/
    
    HCRYPTPROV hCryptProv = 0;                      /* CSP handle*/
    BOOL should_release_ctx = FALSE;

    PCCERT_CONTEXT pUserCert = NULL;		/* User certificate to be used*/

    DWORD keytype = 0;
    int ret = 0;
    BYTE *mem_tbs = NULL;
    size_t mem_len = 0;

    BYTE *mem_signature = NULL;
    size_t signature_len = 0;
    
    CRYPT_VERIFY_MESSAGE_PARA param;
    DWORD MessageSizeArray[1];
    const BYTE* MessageArray[1];

    DWORD signed_len = 0;
    BYTE *signed_mem = NULL;
    
    /*--------------------------------------------------------------------*/
    /*  Используем сертификат из файла для инициализации контекста*/

    if (! infile) {
	fprintf (stderr, "No input file was specified\n");
	goto err;
    }
    if (detached && signature_filename == NULL) {
	fprintf (stderr, "No detached signature file was specified\n");
	goto err;
    }

    
    if (certfile)
    {
	pUserCert = read_cert_from_file (certfile);
	if (!pUserCert)
	{
	    HandleErrorFL("Certificate not found\n");
	    goto err;
	}
	ret = CryptAcquireCertificatePrivateKey(
	    pUserCert,        
	    0,		/*DWORD dwFlags,               */
	    NULL,            
	    &hCryptProv,     
	    &keytype, /* returned key type AT_SIGNATURE ! AT_KEYEXCAHGE*/
	    &should_release_ctx  /* if FALSE DO NOT Release CTX*/
	    );
	if (ret) {
	    printf("A CSP has been acquired. \n");
	}
	else {
	    HandleErrorFL("Cryptographic context could not be acquired.");
	}
    }
    else {
	fprintf (stderr, "No user cert specified. "
	    "Cryptocontext will be opened automaticaly.\n");
    }

    /*--------------------------------------------------------------------*/
    /* Откроем файл который будем проверять*/
    if (!get_file_data_pointer (infile, &mem_len, &mem_tbs)) {
	fprintf (stderr, "Cannot open input file\n");
	goto err;
    }

    /*--------------------------------------------------------------------*/
    /* Прочитает файл подписи в память*/
    if (detached) {
	if (!get_file_data_pointer (signature_filename, &signature_len,
	    &mem_signature)) {
	    fprintf (stderr, "Cannot open signature file\n");
	    goto err;
	}
    }

    /*--------------------------------------------------------------------*/
    /* Установим параметры структуры CRYPT_VERIFY_MESSAGE_PARA*/
    
    param.cbSize = sizeof(CRYPT_VERIFY_MESSAGE_PARA);
    param.dwMsgAndCertEncodingType = TYPE_DER;


    param.hCryptProv = hCryptProv;
    /* автоматически определим необходимый CSP по OID
     * не используем автоматическое определение, зададим провайдер явно,
     * используя определенный из сертификата */

    /* PFN_CRYPT_GET_SIGNER_CERTIFICATE  callback определен следущим образом
    typedef PCCERT_CONTEXT (WINAPI *PFN_CRYPT_GET_SIGNER_CERTIFICATE)(
    IN void *pvGetArg,
    IN DWORD dwCertEncodingType,
    IN PCERT_INFO pSignerId,    // Only the Issuer and SerialNumber
                                // fields have been updated
    IN HCERTSTORE hMsgCertStore
    ) 
    определим следующую функцию, котрая будет считывать заданный сертификат
    из файла и возваращать его
    PCCERT_CONTEXT my_get_cert (void *pvGetArg, DWORD dwCertEncodingType,
    DWORD dwCertEncodingType, PCERT_INFO pSignerId, HCERTSTORE hMsgCertStore)
    */

    param.pfnGetSignerCertificate = my_get_cert; /* этот callback должен
    возвернуть сертификат на котором проверяем сообщение */
    param.pvGetArg = (void*) certfile; /* передадим имя файла сертификата
    в функцию */

   /*------------------------------------------------*/
    /* Раздвояемся.
     *
     * Для проверки detached используется функция
     * CryptVerifyDetachedMessageSignature
     * Если все в одном флаконе, используется
     * CryptVerifyMessageSignature */

    if (detached == 0) {
        DWORD dwSignerIndex = 0;
	/* Используется вцикле если подпись не одна.*/
	/* Пока только 0*/
	signed_mem = (BYTE*)malloc(signed_len = mem_len);
	if (!signed_mem) {
	    HandleErrorFL("Memory allocation error allocating decode blob.");
	}

	ret = CryptVerifyMessageSignature(
	    &param,
	    dwSignerIndex,
	    mem_tbs,	    /* все лежит в одном флаконе*/
	    mem_len,	    /* с заданной длиной*/
	    signed_mem,	    /* если нужно сохранить вложение BYTE *pbDecoded,*/
	    &signed_len,    /* куда сохраняет вложение DWORD *pcbDecoded,*/
	    NULL);	    /* сертификат на котором проверяем PCCERT_CONTEXT *ppSignerCert*/
/*	    &pUserCert);     сертификат на котором проверяем PCCERT_CONTEXT *ppSignerCert*/
	
	if (ret) {
	    printf("Signature was verified OK\n");
	}
	else
	{
	    HandleErrorFL("Signature was NOT verified\n");
	    goto err;
	}

#if 0	/* Почему-то не работает? Вроде все как в описании и примере.*/
	if (!(signed_mem = (BYTE*)malloc(signed_len))) {
	    HandleErrorFL("Memory allocation error allocating decode blob.");
	}

	ret = CryptVerifyMessageSignature(
	    &param,
	    dwSignerIndex,
	    mem_tbs,		    /* все лежит в одном флаконе*/
	    mem_len,		    /* с заданной длиной*/
	    signed_mem,		    /* если нужно сохранить вложение BYTE *pbDecoded,*/
	    &signed_len,	    /* куда сохраняет вложение DWORD *pcbDecoded,*/
	    NULL);		    /* сертификат на котором проверяем PCCERT_CONTEXT *ppSignerCert*/
/*	    &pUserCert);	    // сертификат на котором проверяем PCCERT_CONTEXT *ppSignerCert*/

	if (ret) {
	    printf("Signature was verified OK 2\n");
	}
	else
	{
	    HandleErrorFL("Signature was NOT verified 2\n");
	    goto err;
	}
#endif /* 0*/

	if (outfile && signed_mem && signed_len) {
	    if (!write_file (outfile, signed_len, signed_mem)) {
		printf ("Output file (%s) has been saved\n", outfile);
	    }
	}
    }
    else { /* detached подпись*/
	
        DWORD dwSignerIndex = 0;    /* Используется вцикле если подпись не одна.*/
				    /* Пока только 0*/
        MessageArray[0] = mem_tbs;
	MessageSizeArray[0] = mem_len;

	ret = CryptVerifyDetachedMessageSignature(
	    &param, 
	    dwSignerIndex,
	    (const BYTE*) mem_signature,  /* detached signature*/
	    signature_len,		    /* ее длина*/
	    1,			    /* количество проверяемых исходных файлов*/
	    MessageArray,	    /* Список исходных файлов*/
	    MessageSizeArray,	    /* Список размеров исходных файлов*/
	    &pUserCert);	    /* сертификат на котором проверяем PCCERT_CONTEXT *ppSignerCert*/
	    
	if (ret) {
	    printf("Detached Signature was verified OK\n");
	}
	else
	{
	    HandleErrorFL("Detached Signature was NOT verified\n");
	}
    }

    
err:
    release_file_data_pointer (mem_tbs);
    release_file_data_pointer (mem_signature);
    if (signed_mem) free (signed_mem);
    if (should_release_ctx) CryptReleaseContext(hCryptProv, 0);
    return ret;
} /* End of do_verify*/

/*--------------------------------------------------------------------*/
/* callback, используемый для определения сертификата проверки */

static
PCCERT_CONTEXT WINAPI my_get_cert (void *pvGetArg,
    DWORD dwCertEncodingType, PCERT_INFO pSignerId, HCERTSTORE hMsgCertStore)
{
    BYTE *cert = NULL;
    PCCERT_CONTEXT ret = NULL;
    size_t len = 0;

    if (!pvGetArg)
	return NULL;

    if (!get_file_data_pointer (pvGetArg, &len, &cert))
	return NULL;
   
    ret = CertCreateCertificateContext(TYPE_DER, cert, (DWORD) len);
    return ret;
    UNREFERENCED_PARAMETER (dwCertEncodingType);
    UNREFERENCED_PARAMETER (pSignerId);
    UNREFERENCED_PARAMETER (hMsgCertStore);
}

#if !defined(lint) && !defined(NO_RCSID) && !defined(OK_MODULE_VERSION)
static volatile const char rcsid[] = "\n$Id: cmssf.c,v 1.7 2002/01/14 12:17:53 lse Exp $";
#endif
