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
 * \file $RCSfile: ctmsg.c,v $
 * \version $Revision: 1.12 $
 * \date $Date: 2001/12/25 15:57:52 $
 * \author $Author: pre $
 *
 * \brief Краткое описание модуля.
 *
 * Подробное описание модуля.
 */

#include "tmain.h"

#ifndef PROG
#define PROG main_newmsg
#endif

#define MY_TYPE  (PKCS_7_ASN_ENCODING | X509_ASN_ENCODING)
#define MAX_NAME  128
#define SIGNER_NAME L"Test (Exch)"

BYTE *SignAndEncrypt (HCRYPTPROV hProv,
     const BYTE     *pbToBeSignedAndEncrypted,
     DWORD          cbToBeSignedAndEncrypted,
     DWORD          *pcbSignedAndEncryptedBlob);

BYTE *DecryptAndVerify(HCRYPTPROV hProv,
     BYTE  *pbSignedAndEncryptedBlob,
     DWORD  cbSignedAndEncryptedBlob);

int do_task (HCRYPTPROV hProv);

int
PROG (int argc, char **argv)
{
    HCRYPTPROV hProv = 0;
    char *szContainer = NULL;
    char *szProvider = NULL;
    DWORD dwProvType = PROV_GOST_DH; /*PROV_RSA_FULL;*/

    /*HCRYPTKEY hKey;*/
    BOOL bResult;
    /*ALG_ID Algid;*/
    DWORD dwFlags = 0;
    /*DWORD dwParam;*/
    /*BYTE pbProvName [1024*4];*/
    /*DWORD dwProvNameLen;*/
    /*BYTE pbProvContainer [1024*4];*/
    /*DWORD dwProvContainerLen;*/
    /*int exported = 0;*/
    int ret = 0;
    int print_help = 0;
    int c;
    
    static struct option long_options[] = {
	{"container",		required_argument,	NULL, 'c'},
	{"provider",		required_argument,	NULL, 'p'},
	{"provtype",		required_argument,	NULL, 't'},
	{"verifycontext",	no_argument,		NULL, 'v'},
	{"newkeyset",		no_argument,		NULL, 'n'},
	{"machinekeyset",	no_argument,		NULL, 'm'},
#if(_WIN32_WINNT >= 0x0500)
	{"silent",		no_argument,		NULL, 'i'},
#endif /*(_WIN32_WINNT >= 0x0500)*/
	{"help",	no_argument,		NULL, 'h'},
	{0, 0, 0, 0}
    };

    while ((c = getopt_long_only (argc, argv, "", long_options, (int *)0)) != EOF) {
	switch (c) {
	case 'c':
	    szContainer = optarg;
	    break;
	case 'p':
	    szProvider = abbr2provider (optarg);
	    break;
	case 't':
	    dwProvType = abbr2provtype (optarg);
	    break;
	case 'v':
	    dwFlags |= CRYPT_VERIFYCONTEXT;
	    break;
	case 'n':
	    dwFlags |= CRYPT_NEWKEYSET;
	    break;
	case 'm':
	    dwFlags |= CRYPT_MACHINE_KEYSET;
	    break;
#if(_WIN32_WINNT >= 0x0500)
	case 'i':
	    dwFlags |= CRYPT_SILENT;
	    break;
#endif /*(_WIN32_WINNT >= 0x0500)*/
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

    /* Try to open key set*/
    bResult = CryptAcquireContext (&hProv, szContainer, szProvider, dwProvType, dwFlags);
    if (bResult)
	printf ("CryptAcquireContext succeeded.\n");
    else {
	HandleErrorFL ("Error during CryptAcquireContext.\n");
	goto bad;
    }

    ret = do_task (hProv);

    ret = 1;
bad:
    if (hProv) CryptReleaseContext (hProv, 0);
    if (print_help) {
	printf ("%s -keyset [options]\n", prog);
	printf (SoftName " PKCS#7 manipulation\n");
	printf ("options:\n");
	printf ("  -provider name   [optional] specify provider name or next abbriviation:\n");
	printf ("                   cpDef,\n");
	printf ("                   msDef, msEnhanced, msDefRsaSig, msDefRsaSchannel,\n");
	printf ("                   msEnhancedRsaSchannel, msDefDss, msDefDssDh\n");
	printf ("  -provtype type   [optional] specify provider type or next abbriviation:\n");
	printf ("                   CProCSP,\n");
	printf ("                   RsaFull, RsaSig, Dss, Fortezza, MsExchange, Ssl,\n");
	printf ("                   RsaSchannel, DssDh, EcEcdsaSig, EcEcnraSig, EcEcdsaFull,\n");
	printf ("                   EcEcnraFull, SpyrusLynks\n");
	printf ("  -container name  [optional] specify container name\n");
	printf ("  -verifycontext   [optional] open context for verification only\n");
	printf ("  -newkeyset       [optional] create new key set\n");
	printf ("  -machinekeyset   [optional] open HKLM\n");
#if(_WIN32_WINNT >= 0x0500)
	printf ("  -silent          [optional] do not display any user interface\n");
#endif /*(_WIN32_WINNT >= 0x0500)*/
	printf ("  -help            print this help\n");
    }
    return ret;
}

int
do_task (HCRYPTPROV hProv)
{
    const BYTE *msg = (const BYTE *)"Where do you want to go today?";
    DWORD msg_len = strlen ((const char *)msg) + 1;
    BYTE *encmsg;
    DWORD encmsg_len;
    BYTE *msg2;
    
    /*------------------------------------------------------*/
    /*  Call the local function SignAndEncrypt.*/
    /*  This function returns a pointer to the */
    /*  signed and encrypted BLOB and also returns*/
    /*  the length of that BLOB.*/
    
    encmsg = SignAndEncrypt (hProv, msg, msg_len, &encmsg_len);

    if (!write_file ("file.p7", encmsg_len, encmsg))
	HandleErrorFL ("file open error\n");
    
    /*--------------------------------------------------------------*/
    /*   Call the local function DecryptAndVerify.*/
    /*   This function decrypts and displays the */
    /*   encrypted message and also verifies the */
    /*   messages signature.*/

    msg2 = DecryptAndVerify (hProv, encmsg, encmsg_len);
    if (msg2) {
	printf("    The returned, verified message is ->\n%s\n", msg2);
	printf("    The program executed without error.\n");
    } else {
	printf("Verification failed.\n");
    }
    return 1;
} /* End Main.*/

/*--------------------------------------------------------------------*/
/*     Begin definition of the SignAndEncrypt function.*/

BYTE *
SignAndEncrypt (HCRYPTPROV hProv,
    const BYTE     *pbToBeSignedAndEncrypted,
    DWORD          cbToBeSignedAndEncrypted,
    DWORD          *pcbSignedAndEncryptedBlob)
{
    /*--------------------------------------------------------------------*/
    /*   Declare and initialize local variables.*/
    HCERTSTORE              hCertStore;
    /*--------------------------------------------------------------------*/
    /*   pSignerCertContext will be the certificate of the*/
    /*   the message signer.*/
    PCCERT_CONTEXT          pSignerCertContext ;
    /*--------------------------------------------------------------------*/
    /*   pReceiverCertContext will be the certificate of the */
    /*   message receiver.*/
    PCCERT_CONTEXT          pReceiverCertContext;
    /*char pszNameString[256];*/
    CRYPT_SIGN_MESSAGE_PARA       SignPara;
    CRYPT_ENCRYPT_MESSAGE_PARA    EncryptPara;
    DWORD                         cRecipientCert;
    PCCERT_CONTEXT                rgpRecipientCert[5];
    BYTE                          *pbSignedAndEncryptedBlob = NULL;
    /*CERT_NAME_BLOB                *ReceiverNameBlob;*/
    /*DWORD                         cbNameBlob;*/
    /*DWORD                         dwKeySpec;*/
    /*HCRYPTPROV                    hCryptProv;*/
 
    hProv;
    /*--------------------------------------------------------------------*/
    /*     Open the MY certificate store. */
    /*     For details, see CertOpenStore.*/
    hCertStore = CertOpenStore(
	CERT_STORE_PROV_SYSTEM,
	0,
	0, /*hProv*/
	CERT_SYSTEM_STORE_CURRENT_USER | CERT_STORE_READONLY_FLAG,
	L"my");
    if (!hCertStore) {
	HandleErrorFL("The MY store could not be opened.");
    }
    
    /*--------------------------------------------------------------------*/
    /* Get the certificate for the signer.*/
    pSignerCertContext = CertFindCertificateInStore(
	hCertStore,
	MY_TYPE,
	0,
	CERT_FIND_SUBJECT_STR,
	SIGNER_NAME,
	NULL);
    if(!pSignerCertContext)
    {
	HandleErrorFL("Cert not found.\n");
    }
    /*--------------------------------------------------------------------*/
    /*  Initialize variables and data structures*/
    /*  for the call to CryptSignAndEncryptMessage.*/
    pReceiverCertContext = CertFindCertificateInStore(
	hCertStore,
	MY_TYPE,
	0,
	CERT_FIND_SUBJECT_STR,
	SIGNER_NAME,
	NULL);
    if(!pReceiverCertContext)
    {
	HandleErrorFL("Cert not found.\n");
    }

    
    SignPara.cbSize = sizeof(CRYPT_SIGN_MESSAGE_PARA);
    SignPara.dwMsgEncodingType = MY_TYPE;
    SignPara.pSigningCert = pSignerCertContext ;
    SignPara.HashAlgorithm.pszObjId = szOID_CP_GOST_R3411; /*szOID_RSA_MD2;*/
    SignPara.HashAlgorithm.Parameters.cbData = 0;
    SignPara.pvHashAuxInfo = NULL;
    SignPara.cMsgCert = 1;
    SignPara.rgpMsgCert = &pSignerCertContext ;
    SignPara.cMsgCrl = 0;
    SignPara.rgpMsgCrl = NULL;
    SignPara.cAuthAttr = 0;
    SignPara.rgAuthAttr = NULL;
    SignPara.cUnauthAttr = 0;
    SignPara.rgUnauthAttr = NULL;
    SignPara.dwFlags = 0;
    SignPara.dwInnerContentType = 0;

    EncryptPara.cbSize = sizeof(CRYPT_ENCRYPT_MESSAGE_PARA);
    EncryptPara.dwMsgEncodingType = MY_TYPE;
    EncryptPara.hCryptProv = 0;
    EncryptPara.ContentEncryptionAlgorithm.pszObjId = szOID_CP_GOST_28147; /*szOID_OIWSEC_dhCommMod; //szOID_RSA_RC4;*/
    EncryptPara.ContentEncryptionAlgorithm.Parameters.cbData = 0;
    EncryptPara.pvEncryptionAuxInfo = NULL;
    EncryptPara.dwFlags = 0;
    EncryptPara.dwInnerContentType = 0;
    
    cRecipientCert = 1;
    rgpRecipientCert[0] = pReceiverCertContext;
    *pcbSignedAndEncryptedBlob = 0;
    pbSignedAndEncryptedBlob = NULL;
    
    if( CryptSignAndEncryptMessage(
	&SignPara,
	&EncryptPara,
	cRecipientCert,
	rgpRecipientCert,
	pbToBeSignedAndEncrypted,
	cbToBeSignedAndEncrypted,
	NULL,                      /* the pbSignedAndEncryptedBlob*/
	pcbSignedAndEncryptedBlob))
    {
	printf("%d bytes for the buffer .\n",*pcbSignedAndEncryptedBlob);
    }
    else
    {
	HandleErrorFL("Getting the buffer length failed.");
    }
    
    /*--------------------------------------------------------------------*/
    /*    Allocated memory for the buffer*/
    pbSignedAndEncryptedBlob = (unsigned char *)malloc (
	*pcbSignedAndEncryptedBlob);
    if(!pbSignedAndEncryptedBlob)
	HandleErrorFL("Memory allocation failed.");
    
    /*--------------------------------------------------------------------*/
    /*   Call the function a second time to copy the signed and encrypted*/
    /*   message into the buffer*/
    
    if( CryptSignAndEncryptMessage(
	&SignPara,
	&EncryptPara,
	cRecipientCert,
	rgpRecipientCert,
	pbToBeSignedAndEncrypted,
	cbToBeSignedAndEncrypted,
	pbSignedAndEncryptedBlob,
	pcbSignedAndEncryptedBlob))
    {
	printf("The message is signed and enrypted.\n");
    }
    else
    {
	HandleErrorFL("The message failed to sign and encrypt.");
    }
    
    /*--------------------------------------------------------------------*/
    /*   Clean up.*/
    
    if(pSignerCertContext )
    {
	CertFreeCertificateContext (pSignerCertContext );
    }
    if(pReceiverCertContext )
    {
	CertFreeCertificateContext (pReceiverCertContext );
    }
    CertCloseStore(
	hCertStore,
	0);
    
    /*--------------------------------------------------------------------*/
    /*   Return the signed and encrypted message.*/
    
    return pbSignedAndEncryptedBlob;
    
}  /* End SignandEncrypt.*/

/*--------------------------------------------------------------------*/
/*   Define the DecryptAndVerify function.*/

BYTE  *
DecryptAndVerify (HCRYPTPROV hProv,
    BYTE  *pbSignedAndEncryptedBlob,
    DWORD  cbSignedAndEncryptedBlob)
{
    
    /*--------------------------------------------------------------------*/
    /*  Declare and initialize local variables.*/
    
    HCERTSTORE                     hCertStore;
    CRYPT_DECRYPT_MESSAGE_PARA     DecryptPara;
    CRYPT_VERIFY_MESSAGE_PARA      VerifyPara;
    DWORD                          dwSignerIndex = 0;
    BYTE                           *pbDecrypted;
    DWORD                          cbDecrypted;

    hProv;
    /*--------------------------------------------------------------------*/
    /*   Open the certificate store*/
    hCertStore = CertOpenStore(
	CERT_STORE_PROV_SYSTEM,
	0,
	0, /* hProv*/
	CERT_SYSTEM_STORE_CURRENT_USER | CERT_STORE_READONLY_FLAG,
	L"my");
    if (!hCertStore)
    {
	HandleErrorFL("The MY store could not be openned.");
    }
    
    /*--------------------------------------------------------------------*/
    /*   Initialize the needed data structures.*/
    
    DecryptPara.cbSize = sizeof(CRYPT_DECRYPT_MESSAGE_PARA);
    DecryptPara.dwMsgAndCertEncodingType = MY_TYPE;
    DecryptPara.cCertStore = 1;
    DecryptPara.rghCertStore = &hCertStore;
    
    VerifyPara.cbSize = sizeof(CRYPT_VERIFY_MESSAGE_PARA);
    VerifyPara.dwMsgAndCertEncodingType = MY_TYPE;
    VerifyPara.hCryptProv = 0;
    VerifyPara.pfnGetSignerCertificate = NULL;
    VerifyPara.pvGetArg = NULL;
    pbDecrypted = NULL;
    cbDecrypted = 0;
    
    /*--------------------------------------------------------------------*/
    /*     Call CryptDecryptAndVerifyMessageSignature a first time*/
    /*     to determine the needed size of the buffer to hold the */
    /*     decrypted message.*/
    
    if(!(CryptDecryptAndVerifyMessageSignature(
	&DecryptPara,
	&VerifyPara,
	dwSignerIndex,
	pbSignedAndEncryptedBlob,
	cbSignedAndEncryptedBlob,
	NULL,           /* pbDecrypted*/
	&cbDecrypted,
	NULL,
	NULL)))
    {
	HandleErrorFL("Failed.");
    }
    
    /*--------------------------------------------------------------------*/
    /*    Allocate memory for the buffer to hold the decrypted message.*/
    pbDecrypted = (BYTE *)malloc(cbDecrypted);
    if(!pbDecrypted)
	HandleErrorFL("Memory allocation failed.");
    
    if(!(CryptDecryptAndVerifyMessageSignature(
	&DecryptPara,
	&VerifyPara,
	dwSignerIndex,
	pbSignedAndEncryptedBlob,
	cbSignedAndEncryptedBlob,
	pbDecrypted,
	&cbDecrypted,
	NULL,
	NULL)))
    {
	pbDecrypted = NULL;
    }
    
    /*--------------------------------------------------------------------*/
    /*  Close the certificate store.*/
    
    CertCloseStore(
	hCertStore,
	0);
    
    /*--------------------------------------------------------------------*/
    /*    Return the decrypted string or NULL*/
    
    return pbDecrypted;
    
} /* end of DecryptandVerify*/

#if !defined(lint) && !defined(NO_RCSID) && !defined(OK_MODULE_VERSION)
static volatile const char rcsid[] = "\n$Id: ctmsg.c,v 1.12 2001/12/25 15:57:52 pre Exp $";
#endif
