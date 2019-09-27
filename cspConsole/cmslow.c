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
 * \file $RCSfile: cmslow.c,v $
 * \version $Revision: 1.7 $
 * \date $Date: 2002/01/14 12:17:53 $
 * \author $Author: lse $
 *
 * \brief Программа тестирования ЭЦП с использованием функий low level CMS.
 *
 * Подробное описание модуля.
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

static int CMSdo_low_sign (char *in_filename, char *out_filename,
    char *my_certfile, char *OID, int include);
static int CMSdo_low_verify (char *in_filename, char *my_certfile,
    char *signature_filename);

int main_CMSsign (int argc, char **argv)
{
    char *in_filename = NULL;    
    char *out_filename = NULL;
    char *my_certfile = NULL;
    int  ret = 0;
    int  sign = 0;
    int  verify  = 0;
    int  print_help = 0;
    char OID[64] = szOID_CP_GOST_R3411;
    int c;
    int include = 0;
    int detached = 0;
    char *signature_filename = NULL;
   
    static struct option long_options[] = {
	{"in",		required_argument,	NULL, 'i'},
	{"out",		required_argument,	NULL, 'o'},
	{"my",		required_argument,	NULL, 'm'},
	{"sign",	no_argument,		NULL, 'e'},
	{"detached",	no_argument,		NULL, '2'},
	{"add",		no_argument,		NULL, '1'},
	{"verify",	no_argument,		NULL, 'd'},
	{"signature",	required_argument,	NULL, 's'},
	{"alg",		required_argument,	NULL, 'a'},
	{"help",	no_argument,		NULL, 'h'},
	{0, 0, 0, 0}
    };

    while ((c = getopt_long_only (argc, argv, "", long_options, (int *)0)) != EOF) {
	switch (c) {
	case 's':
	     signature_filename = optarg;
	    break;
	case '2':
	    detached = 1;
	    break;
	case 'i':
	     in_filename = optarg;
	    break;
	case 'o':
	     out_filename = optarg;
	    break;
	case 'm':
	     my_certfile = optarg;
	    break;
	case 'e':
	    sign = 1;
	    break;
	case 'd':
	    verify  = 1;
	    break;
	case '1':
	    include = 1;
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

    if (!sign && !verify) {
	print_help = 1;
	goto bad;
    }

    if (sign) {
	ret = CMSdo_low_sign (in_filename,
	    out_filename, my_certfile, OID, include);
    }
    else if (verify) {
	ret = CMSdo_low_verify (in_filename,
	    my_certfile, signature_filename);
    }
    else {
	print_help = 1;
    }

bad:
    if (print_help) {
	const char *fmt =
"%s -cmslowsign [options]\n"
SoftName " generate CMS Signed message \n"
"using CAPI low level message function type\n"
"options:\n"
"  -in arg        input filename to be signed or verified\n"
"  -out arg       output CMS filename\n"
"  -my            use sender certificate from file to acquire context\n"
"  -sign          sign input file\n"
"  -detached      save signature in detached file\n"
"  -add           add sender certificate to CMS\n"
"  -verify        decrypt enveloped file, specified by input filename\n"
"  -signature     detached signature file\n"
"  -alg           perform hash with OID.\n"
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
/* Тест ЭЦП*/
int CMSdo_low_sign (char *infile, char *outfile, char *certfile,
    char *OID, int include)
{
    HCRYPTPROV hCryptProv = 0;               /* CSP handle*/
    PCCERT_CONTEXT pUserCert = NULL;		/* User certificate to be used*/

    DWORD keytype = 0;
    BOOL should_release_ctx = FALSE;
    int ret = 0;
    BYTE *mem_tbs = NULL;
    size_t mem_len = 0;
    
    HCRYPTMSG hMsg = 0;
    
    DWORD			HashAlgSize;
    CRYPT_ALGORITHM_IDENTIFIER	HashAlgorithm;
    CMSG_SIGNER_ENCODE_INFO	SignerEncodeInfo;
    CERT_BLOB			SignerCertBlob;
    CERT_BLOB			SignerCertBlobArray[1];
    DWORD			cbEncodedBlob;
    BYTE			*pbEncodedBlob = NULL;
    CMSG_SIGNER_ENCODE_INFO	SignerEncodeInfoArray[1];
    CMSG_SIGNED_ENCODE_INFO	SignedMsgEncodeInfo;
    
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

    pUserCert = read_cert_from_file (certfile);
    if (!pUserCert)
	goto err;
    
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
       /* Initialize the algorithm identifier structure.*/
    
    HashAlgSize = sizeof(HashAlgorithm);
    memset(&HashAlgorithm, 0, HashAlgSize); /* Init. to zero.*/
    HashAlgorithm.pszObjId = OID;	    /* Initialize the necessary member.*/
    
    /*--------------------------------------------------------------------*/
    /* Initialize the CMSG_SIGNER_ENCODE_INFO structure.*/
    
    memset(&SignerEncodeInfo, 0, sizeof(CMSG_SIGNER_ENCODE_INFO));
    SignerEncodeInfo.cbSize = sizeof(CMSG_SIGNER_ENCODE_INFO);
    SignerEncodeInfo.pCertInfo = pUserCert->pCertInfo;
    SignerEncodeInfo.hCryptProv = hCryptProv;
    SignerEncodeInfo.dwKeySpec = keytype;
    SignerEncodeInfo.HashAlgorithm = HashAlgorithm;
    SignerEncodeInfo.pvHashAuxInfo = NULL;
    
    /*--------------------------------------------------------------------*/
    /* Create an array of one. Note: Currently, there can be only one*/
    /* signer.*/
    
    SignerEncodeInfoArray[0] = SignerEncodeInfo;
    
    /*--------------------------------------------------------------------*/
    /* Initialize the CMSG_SIGNED_ENCODE_INFO structure.*/
    
    SignerCertBlob.cbData = pUserCert->cbCertEncoded;
    SignerCertBlob.pbData = pUserCert->pbCertEncoded;
    
    /*--------------------------------------------------------------------*/
    /* Initialize the array of one CertBlob.*/
    
    SignerCertBlobArray[0] = SignerCertBlob;
    memset(&SignedMsgEncodeInfo, 0, sizeof(CMSG_SIGNED_ENCODE_INFO));
    SignedMsgEncodeInfo.cbSize = sizeof(CMSG_SIGNED_ENCODE_INFO);
    SignedMsgEncodeInfo.cSigners = 1;
    SignedMsgEncodeInfo.rgSigners = SignerEncodeInfoArray;
    SignedMsgEncodeInfo.cCertEncoded = include;
    if (include)
	SignedMsgEncodeInfo.rgCertEncoded = SignerCertBlobArray;
    else
	SignedMsgEncodeInfo.rgCertEncoded = NULL;
    SignedMsgEncodeInfo.rgCrlEncoded = NULL;
    
    /*--------------------------------------------------------------------*/
    /* Get the size of the encoded message blob.*/
    cbEncodedBlob = CryptMsgCalculateEncodedLength(
	TYPE_DER,       /* Message encoding type*/
	0,                      /* Flags*/
	CMSG_SIGNED,            /* Message type*/
	&SignedMsgEncodeInfo,   /* Pointer to structure*/
	NULL,                   /* Inner content object ID*/
	mem_len);		/* Size of content*/
    if(cbEncodedBlob)
    {
	printf("The length of the data has been calculated. \n");
    }
    else
    {
	HandleErrorFL("Getting cbEncodedBlob length failed");
    }
    /*--------------------------------------------------------------------*/
    /* Allocate memory for the encoded blob.*/
    
    pbEncodedBlob = (BYTE *) malloc(cbEncodedBlob);
    if (!pbEncodedBlob)
	HandleErrorFL("Memory allocation failed");
    /*--------------------------------------------------------------------*/
    /* Open a message to encode.*/
    hMsg = CryptMsgOpenToEncode(
	TYPE_DER,        /* Encoding type*/
	0,                       /* Flags*/
	CMSG_SIGNED,             /* Message type*/
	&SignedMsgEncodeInfo,    /* Pointer to structure*/
	NULL,                    /* Inner content object ID*/
	NULL);                   /* Stream information (not used)*/
    if(hMsg)
    {
	printf("The message to be encoded has been opened. \n");
    }
    else
    {
	HandleErrorFL("OpenToEncode failed");
    }
    /*--------------------------------------------------------------------*/
    /* Update the message with the data.*/
    
    if(CryptMsgUpdate(
        hMsg,		/* Handle to the message*/
        mem_tbs,		/* Pointer to the content*/
        mem_len,	/* Size of the content*/
        TRUE))		/* Last call*/
    {
	printf("Content has been added to the encoded message. \n");
    }
    else
    {
	HandleErrorFL("MsgUpdate failed");
    }
    /*--------------------------------------------------------------------*/
    /* Get the resulting message.*/
    
    if(CryptMsgGetParam(
	hMsg,                      /* Handle to the message*/
	CMSG_CONTENT_PARAM,        /* Parameter type*/
	0,                         /* Index*/
	pbEncodedBlob,             /* Pointer to the blob*/
	&cbEncodedBlob))           /* Size of the blob*/
    {
	printf("Message encoded successfully. \n");
    }
    else
    {
	HandleErrorFL("MsgGetParam failed");
    }
    /*--------------------------------------------------------------------*/
    /* pbEncodedBlob now points to the encoded, signed content.*/
    /*--------------------------------------------------------------------*/
    if (outfile) {
	if (!write_file (outfile, cbEncodedBlob, pbEncodedBlob)) {
	    printf ("Output file (%s) has been saved\n", outfile);
	}
    }
    
    /*--------------------------------------------------------------------*/
    /* Clean up.*/
err:
    release_file_data_pointer (mem_tbs);
    if(pbEncodedBlob) free(pbEncodedBlob);
    if(hMsg) CryptMsgClose(hMsg);
    if(hCryptProv) CryptReleaseContext(hCryptProv,0);
    return 1;
} /*  End of main*/


/*--------------------------------------------------------------------*/
/* Тест проверки ЭЦП*/
int CMSdo_low_verify (char *infile, char *certfile, char *signature_filename)
{
    HCRYPTPROV hCryptProv = 0;               /* CSP handle*/
    PCCERT_CONTEXT pUserCert = NULL;		/* User certificate to be used*/

    DWORD keytype = 0;
    BOOL should_release_ctx = FALSE;
    int ret = 0;
    BYTE *mem_tbs = NULL;
    size_t mem_len = 0;
    BYTE *mem_signature = NULL;
    size_t signature_len = 0;
    int detached = 0;
    
    HCRYPTMSG hMsg = 0;
    
    DWORD cbDecoded;
    BYTE *pbDecoded;
    DWORD cbSignerCertInfo = 0;
    PCERT_INFO pSignerCertInfo = 0;
    PCCERT_CONTEXT pSignerCertContext;
    PCERT_INFO pSignerCertificateInfo;
    HCERTSTORE hStoreHandle = NULL;

    /*--------------------------------------------------------------------*/
    /*  Используем сертификат из файла для инициализации контекста*/

    if (! infile) {
	fprintf (stderr, "No input file was specified\n");
	goto err;
    }
   
    if (certfile)
    {
	pUserCert = read_cert_from_file (certfile);
	if (!pUserCert)
	    goto err;
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
    if (signature_filename) {
	if (!get_file_data_pointer (signature_filename, &signature_len,
	    &mem_signature)) {
	    fprintf (stderr, "Cannot open signature file\n");
	    goto err;
	}
	detached = 1;
    }

    /*--------------------------------------------------------------------*/
    /* Open a message for decoding.*/
    
    hMsg = CryptMsgOpenToDecode(
	TYPE_DER,      /* Encoding type.*/
	0,                     /* Flags.*/
	0,                     /* Use the default message type.*/
	hCryptProv,            /* Cryptographic provider.*/
	NULL,                  /* Recipient information.*/
	NULL);                 /* Stream information.*/
    if (hMsg) 
	printf("The message to decode is open. \n");
    else
    	HandleErrorFL("OpenToDecode failed");
    /*--------------------------------------------------------------------*/
    /* Update the message with an encoded blob.*/
    /* Both pbEncodedBlob, the encoded data, */
    /* and cbEnclodeBlob, the length of the encoded data,*/
    /* must be available. */
    
    ret = CryptMsgUpdate(
	hMsg,                 /* Handle to the message*/
	mem_tbs,        /* Pointer to the encoded blob*/
	mem_len,        /* Size of the encoded blob*/
	TRUE);                /* Last call*/
    if (ret) 
	printf("The encoded blob has been added to the message. \n");
    else { /* сюда попадем если задан нулевой провайдер и наше сообщения*/


	HandleErrorFL("Decode MsgUpdate failed");
    }
    /*--------------------------------------------------------------------*/
    /* Get the size of the content.*/
    
    ret = CryptMsgGetParam(
	hMsg,                  /* Handle to the message*/
	CMSG_CONTENT_PARAM,    /* Parameter type*/
	0,                     /* Index*/
	NULL,                  /* Address for returned info*/
	&cbDecoded);           /* Size of the returned info*/
    if (ret)
	printf("The message parameter (CMSG_CONTENT_PARAM) has been acquired. "
	    "Message size: %d\n", cbDecoded);
    else
	HandleErrorFL("Decode CMSG_CONTENT_PARAM failed");
    /*--------------------------------------------------------------------*/
    /* Allocate memory.*/
    
    pbDecoded = (BYTE *) malloc(cbDecoded);
    if (!pbDecoded)
	HandleErrorFL("Decode memory allocation failed");
    /*--------------------------------------------------------------------*/
    /* Get a pointer to the content.*/
    
    ret = CryptMsgGetParam(
	hMsg,                 /* Handle to the message*/
	CMSG_CONTENT_PARAM,   /* Parameter type*/
	0,                    /* Index*/
	pbDecoded,            /* Address for returned */
	&cbDecoded);          /* Size of the returned        */
    if (ret)
	printf("The message param (CMSG_CONTENT_PARAM) updated. "
	    "Length is %lu.\n",cbDecoded);
    else
	HandleErrorFL("Decode CMSG_CONTENT_PARAM #2 failed");
    /*--------------------------------------------------------------------*/
    /* Verify the signature.*/
    /* First, get the signer CERT_INFO from the message.*/
    
    /*--------------------------------------------------------------------*/
    /* Get the size of memory required.*/
 
    if (! pUserCert) { /* попробуем определим сертификат из сообщения*/
	ret = CryptMsgGetParam(
	    hMsg,                         /* Handle to the message*/
	    CMSG_SIGNER_CERT_INFO_PARAM,  /* Parameter type*/
	    0,                            /* Index*/
	    NULL,                        /* Address for returned */
	    &cbSignerCertInfo);          /* Size of the returned */
	if (ret)
	    printf("Try to get user cert. OK. Length %d.\n",cbSignerCertInfo);
	else {
	    printf("No user certificate found in message.\n");
	}
    }

    if (pUserCert) {
	hStoreHandle = CertOpenStore(CERT_STORE_PROV_MEMORY, TYPE_DER, 0,
	    CERT_STORE_CREATE_NEW_FLAG,NULL);
	if (!hStoreHandle)
	    HandleErrorFL("Cannot create temporary store in memory.");
	if (pUserCert) {
	    ret = CertAddCertificateContextToStore(hStoreHandle, pUserCert,
		CERT_STORE_ADD_ALWAYS, NULL);
	    pSignerCertInfo = pUserCert->pCertInfo;
	}
	else
	    ret = 0;
	if (!ret)
	    HandleErrorFL("Cannot add user certificate to store.");
    }
    
    /*--------------------------------------------------------------------*/
    /* Allocate memory.*/
    
    if (!pUserCert) {
	pSignerCertInfo = (PCERT_INFO) malloc(cbSignerCertInfo);
	if (!pSignerCertInfo)
	    HandleErrorFL("Verify memory allocation failed");
    }
    
    /*--------------------------------------------------------------------*/
    /* Get the message certificate information (CERT_INFO*/
    /* structure).*/
    
    if (! pUserCert) {
	ret = CryptMsgGetParam(
	    hMsg,                         /* Handle to the message*/
	    CMSG_SIGNER_CERT_INFO_PARAM,  /* Parameter type*/
	    0,                            /* Index*/
	    pSignerCertInfo,              /* Address for returned */
	    &cbSignerCertInfo);           /* Size of the returned */
	    if (ret) 
		printf("The signer info has been returned. \n");
	    else
		HandleErrorFL("Verify SIGNER_CERT_INFO #2 failed");
    }
    /*--------------------------------------------------------------------*/
    /* Open a certificate store in memory using CERT_STORE_PROV_MSG,*/
    /* which initializes it with the certificates from the message.*/
    
    if (! hStoreHandle) {
	hStoreHandle = CertOpenStore(
	    CERT_STORE_PROV_MSG,        /* Store provider type */
	    TYPE_DER,		    /* Encoding type*/
	    hCryptProv,                 /* Cryptographic provider*/
	    0,                          /* Flags*/
	    hMsg);                      /* Handle to the message*/
	if (hStoreHandle)
	    printf("The message certificate store be used for verifying\n");
    }

    if (! hStoreHandle) {
	    HandleErrorFL("Cannot open certificate store form message\n");
    }
    /*--------------------------------------------------------------------*/
    /* Find the signer's certificate in the store.*/
    pSignerCertContext = CertGetSubjectCertificateFromStore(
	hStoreHandle,       /* Handle to store*/
	TYPE_DER,   /* Encoding type*/
	pSignerCertInfo);
    if(pSignerCertContext)   /* Pointer to retrieved CERT_CONTEXT*/
    {
	printf("A signer certificate has been retrieved. \n");
    }
    else
    {
	HandleErrorFL("Verify GetSubjectCert failed");
    }
    /*--------------------------------------------------------------------*/
    /* Use the CERT_INFO from the signer certificate to verify*/
    /* the signature.*/
    
    pSignerCertificateInfo = pSignerCertContext->pCertInfo;
    if(CryptMsgControl(
	hMsg,                       /* Handle to the message*/
	0,                          /* Flags*/
	CMSG_CTRL_VERIFY_SIGNATURE, /* Control type*/
	pSignerCertificateInfo))    /* Pointer to the CERT_INFO*/
    {
	printf("\nSignature was VERIFIED.\n");
    }
    else
    {
	printf("\nThe signature was NOT VEIFIED.\n");
    }
    if(hStoreHandle)
	CertCloseStore(hStoreHandle, CERT_CLOSE_STORE_FORCE_FLAG);

   
err:
    release_file_data_pointer (mem_tbs);
    release_file_data_pointer (mem_signature);
    return ret;
}

#if !defined(lint) && !defined(NO_RCSID) && !defined(OK_MODULE_VERSION)
static volatile const char rcsid[] = "\n$Id: cmslow.c,v 1.7 2002/01/14 12:17:53 lse Exp $";
#endif
