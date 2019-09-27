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
 * \file $RCSfile: reqcert.c,v $
 * \version $Revision: 1.16 $
 * \date $Date: 2001/12/25 15:57:52 $
 * \author $Author: pre $
 *
 * \brief Утилита проверки ЭЦП запроса на сертификат и сертификата
 *
 */

#include "tmain.h"

static int do_verify_request (const FMTFILEDESCR *req_ffd);
static int do_verify_certificate (const FMTFILEDESCR *cert_ffd,
    const FMTFILEDESCR *issuer_ffd);
static int verify_cert_chain (const char *cname);
static int get_usage (PCCERT_CONTEXT pCertContext);

/*--------------------------------------------------------------*/
/*--------------------------------------------------------------*/
/**/
/* MAIN*/
/**/
/*--------------------------------------------------------------*/
/*--------------------------------------------------------------*/
int main_rc (int argc, char **argv) 
{
    FMTFILEDESCR req_ffd = {
            sizeof(req_ffd), 
            NULL,
            DER,
            &Base64RequestHdrs
        };    
    FMTFILEDESCR cert_ffd = {
            sizeof(cert_ffd), 
            NULL,
            DER,
            &Base64CertificateHdrs
        };
    FMTFILEDESCR issuer_ffd = {
            sizeof(issuer_ffd), 
            NULL,
            DER,
            &Base64CertificateHdrs
        };
    FILEFORMAT fmt = DER;
    int     ret = 0;
    int     print_help = 0;
    int     c;
    int	    verify_chain = 0;
    char*   cname = NULL;    

    /*-----------------------------------------------------------------------------*/
    /* определение опций разбора параметров*/
    static const struct option long_options[] = {
        {"req",         required_argument,      NULL, 'r'},
        {"cert",        required_argument,      NULL, 'c'},
        {"issuer",      required_argument,      NULL, 'i'},
        {"der",         no_argument,            NULL, 'D'},
        {"base64",      no_argument,            NULL, 'B'},
        {"base64hdr",   no_argument,            NULL, 'H'},
        {"chain",       required_argument,      NULL, 'a'},
        {"help",        no_argument,            NULL, 'h'},
        {0, 0, 0, 0}
    };

    /*-----------------------------------------------------------------------------*/
    /* разбор параметров*/
    /* для разбора параметров используется модуль getopt.c*/
    while ((c = getopt_long_only (argc, argv, "", long_options, (int *)0)) != EOF) {
        switch (c) {
        case 'r':
            req_ffd.mFileName = optarg;
            req_ffd.mFormat = fmt;
            break;
        case 'a':
            cname  = optarg;
            verify_chain = 1;
            break;
        case 'c':
            cert_ffd.mFileName = optarg;
            cert_ffd.mFormat = fmt;
            break;
        case 'i':
            issuer_ffd.mFileName = optarg;
            issuer_ffd.mFormat = fmt;
            break;
        case 'D':
            fmt = DER;
            break;
        case 'B':
            fmt = BASE64;
            break;
        case 'H':
            fmt = BASE64HDR;
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

    if (verify_chain) {

	ret = verify_cert_chain (cname);
	return ret;
    }

    if (req_ffd.mFileName && cert_ffd.mFileName) {
        print_help = 1;
        goto bad;
    }

    if (req_ffd.mFileName) {
        ret = do_verify_request (&req_ffd);
    } else if (cert_ffd.mFileName) {
        ret = do_verify_certificate (&cert_ffd, &issuer_ffd);
    } else {
        print_help = 1;
    }

bad:
    if (print_help) {
        fprintf(stderr,"%s -rc [options]\n", prog);
        fprintf(stderr,SoftName " verify pkcs#10 request and x.509 certificate signature\n");
        fprintf(stderr,"options:\n");
        fprintf(stderr,"  -req arg       input pkcs#10 request filename to be verified\n");
        fprintf(stderr,"  -cert arg      input x.509 certificate filename to be verified\n");
        fprintf(stderr,"  -chain arg     verify certificates chain using cert witn commonName from store MY\n");
        fprintf(stderr,"  -issuer arg    issuer certificate (not required if self-signed)\n");
        fprintf(stderr,"  -der           next file(s) DER formated\n");
        fprintf(stderr,"  -base64        next file(s) BASE64 formated\n");
        fprintf(stderr,"  -base64hdr     next file(s) BASE64HDR formated\n");
        fprintf(stderr,"  -help          print this help\n\n");
        printf ("Press Enter to continue.\n");
        getchar ();
    }
    return ret;
}

/*--------------------------------------------------------------*/
/*--------------------------------------------------------------*/
/**/
/* Проверка ЭЦП запроса на сертификат*/
/**/
/*--------------------------------------------------------------*/
/*--------------------------------------------------------------*/

static int do_verify_request (const FMTFILEDESCR *req_ffd)
{
    int                     ret = 0;
    BYTE                    *request = NULL;
    size_t                   request_len = 0;
    CERT_SIGNED_CONTENT_INFO *req_tbs = NULL;
    DWORD                   req_tbs_len = 0;    
    CERT_REQUEST_INFO       *req_info = NULL;
    DWORD                   req_info_len = 0;

    ret = get_file_data_pointer_fmt (req_ffd, &request_len, &request);
    if (!ret) {
        HandleErrorFL ("Cannot read file.");
    }

    /* Определим необходимую длину*/
    ret = CryptDecodeObject(TYPE_DER, 
        X509_CERT,
        request, request_len,
        CRYPT_DECODE_NOCOPY_FLAG, 
        NULL, &req_tbs_len);
    if (! ret) {
        HandleErrorFL ("Cannot decode request.");
    }

    req_tbs = malloc (req_tbs_len);
    if (! req_tbs) {
        HandleErrorFL ("Memory allocation error.");
    }

    /* Декодируем структуру*/
    ret = CryptDecodeObject(TYPE_DER, 
        X509_CERT,
        request, request_len,
        CRYPT_DECODE_NOCOPY_FLAG, 
        req_tbs, &req_tbs_len);
    if (! ret) {
        HandleErrorFL ("Cannot decode request.");
    }

    /* Декодируем структуру CERT_REQUEST_INFO*/
    ret = CryptDecodeObject(TYPE_DER, 
        X509_CERT_REQUEST_TO_BE_SIGNED,
        req_tbs->ToBeSigned.pbData, req_tbs->ToBeSigned.cbData,
        CRYPT_DECODE_NOCOPY_FLAG, 
        NULL, &req_info_len);
    if (! ret) {
        HandleErrorFL ("Cannot decode request.");
    }

    req_info = malloc (req_info_len);
    if (! req_info) {
        HandleErrorFL ("Memory allocation error.");
    }

    ret = CryptDecodeObject(TYPE_DER, 
        X509_CERT_REQUEST_TO_BE_SIGNED,
        req_tbs->ToBeSigned.pbData, req_tbs->ToBeSigned.cbData,
        CRYPT_DECODE_NOCOPY_FLAG, 
        req_info, &req_info_len);
    if (! ret) {
        HandleErrorFL ("Cannot decode request.");
    }

    /*--------------------------------------------------------------*/
    /* Проверяем подпись*/
    ret = CryptVerifyCertificateSignature (0,
        TYPE_DER, 
        (BYTE*) request, 
        request_len,
        &(*req_info).SubjectPublicKeyInfo);
    if (! ret) {
        HandleErrorFL ("Cannot verify signature.");
    } else {
        printf ("Signature was VERIFIED.\n");
    }
    printf ("Press Enter to continue.\n");
    getchar ();

    if (req_info) free (req_info);
    if (req_tbs) free (req_tbs);
    release_file_data_pointer_fmt (req_ffd, request);
    return ret;
}

/*--------------------------------------------------------------*/
/*--------------------------------------------------------------*/
/**/
/* Проверка ЭЦП сертификата*/
/**/
/*--------------------------------------------------------------*/
/*--------------------------------------------------------------*/

static int do_verify_certificate (const FMTFILEDESCR *cert_ffd, const FMTFILEDESCR *issuer_ffd)
{
    int                     ret = 0;
    BYTE                    *cert = NULL;
    size_t                   cert_len = 0;
    CERT_SIGNED_CONTENT_INFO *cert_tbs = NULL;
    DWORD                   cert_tbs_len = 0;   
    CERT_INFO               *cert_info = NULL;
    DWORD                   cert_info_len = 0;
    CERT_PUBLIC_KEY_INFO    *key_info = NULL;
    PCCERT_CONTEXT          issuer = NULL;

    ret = get_file_data_pointer_fmt (cert_ffd, &cert_len, &cert);
    if (!ret) {
        HandleErrorFL ("Cannot read file.");
    }

    /* Определим необходимую длину*/
    ret = CryptDecodeObject(TYPE_DER, 
        X509_CERT,
        cert, cert_len,
        CRYPT_DECODE_NOCOPY_FLAG, 
        NULL, &cert_tbs_len);
    if (! ret) {
        HandleErrorFL ("Cannot decode certificate.");
    }

    cert_tbs = malloc (cert_tbs_len);
    if (! cert_tbs) {
        HandleErrorFL ("Memory allocation error.");
    }

    /* Декодируем структуру*/
    ret = CryptDecodeObject(TYPE_DER, 
        X509_CERT,
        cert, cert_len,
        CRYPT_DECODE_NOCOPY_FLAG, 
        cert_tbs, &cert_tbs_len);
    if (! ret) {
        HandleErrorFL ("Cannot decode certificate");
    }

    /* Декодируем структуру CERT_INFO*/
    ret = CryptDecodeObject(TYPE_DER, 
        X509_CERT_TO_BE_SIGNED,
        cert_tbs->ToBeSigned.pbData, cert_tbs->ToBeSigned.cbData,
        CRYPT_DECODE_NOCOPY_FLAG, 
        NULL, &cert_info_len);
    if (! ret) {
        HandleErrorFL ("Cannot decode request.");
    }

    cert_info = malloc (cert_info_len);
    if (! cert_info) {
        HandleErrorFL ("Memory allocation error.");
    }

    ret = CryptDecodeObject(TYPE_DER, 
        X509_CERT_TO_BE_SIGNED,
        cert_tbs->ToBeSigned.pbData, cert_tbs->ToBeSigned.cbData,
        CRYPT_DECODE_NOCOPY_FLAG, 
        cert_info, &cert_info_len);
    if (! ret) {
        HandleErrorFL ("Cannot decode certificate.");
    }

    /*--------------------------------------------------------------*/
    /* Определим ключ, на котором будем проверять ЭЦП*/
    /* Если задан сертификат издателя, ключ берем оттуда*/

    if (issuer_ffd->mFileName) {
        issuer = read_cert_from_file_fmt (issuer_ffd);
        if (! issuer) {
            HandleErrorFL ("Cannot read issuer certificate.");
        }
        key_info = &issuer->pCertInfo->SubjectPublicKeyInfo;
    } else {
        key_info = &cert_info->SubjectPublicKeyInfo;
    }

#if 0 /* Только для CryptoAPI с Win2000*/
        /* Проверяем подпись*/
    {
        CRYPT_DATA_BLOB dbcert = { 0, NULL };

        dbcert.pbData = cert;
        dbcert.cbData = cert_len;

        ret = CryptVerifyCertificateSignatureEx(0,
            TYPE_DER,
            CRYPT_VERIFY_CERT_SIGN_SUBJECT_BLOB,
            &dbcert,
            CRYPT_VERIFY_CERT_SIGN_ISSUER_PUBKEY,
            key_info,
            0,
            NULL);
    }
    if (! ret) {
        HandleErrorFL ("Cannot verify signature.");
    } else {
        printf ("Signature was VERIFIED.\n");
    }
#endif
        /* Проверяем подпись*/
    ret = CryptVerifyCertificateSignature (0,
        TYPE_DER, 
        (BYTE*) cert, 
        cert_len,
        key_info);
    if (! ret) {
        HandleErrorFL ("Cannot verify signature.");
    } else {
        printf ("Signature was VERIFIED.\n");
    }
    printf ("Press Enter to continue.\n");
    getchar ();

    if (issuer) CertFreeCertificateContext(issuer);
    release_file_data_pointer_fmt (cert_ffd, cert);
    if (cert) free (cert);
    if (cert_info) free (cert_info);
    if (cert_tbs) free (cert_tbs);
    return ret;
}

/*--------------------------------------------------------------*/
/*--------------------------------------------------------------*/
/**/
/* Проверка цепочки сертификатов */
/**/
/*--------------------------------------------------------------*/
/*--------------------------------------------------------------*/

static int verify_cert_chain (const char *cname)
{

    HCERTCHAINENGINE         hChainEngine;
    CERT_CHAIN_ENGINE_CONFIG ChainConfig;
    PCCERT_CHAIN_CONTEXT     pChainContext;
    PCCERT_CONTEXT           pCertContext = NULL;
    CERT_ENHKEY_USAGE        EnhkeyUsage;
    CERT_USAGE_MATCH         CertUsage;  
    CERT_CHAIN_PARA          ChainPara;
    DWORD                    dwFlags=0;

    
    int ret = 0;
    
    if (! cname)
    {
	fprintf (stderr, "No user certificate specified\n");
	goto err;
    }
    
    pCertContext = read_cert_from_my((char*)cname);
    if (! pCertContext) {
	printf ("Cannot find User certificate: %s\n", cname);
	goto err;
    }

    ret = get_usage (pCertContext);

    
    EnhkeyUsage.cUsageIdentifier = 0;
    EnhkeyUsage.rgpszUsageIdentifier=NULL;
    CertUsage.dwType = USAGE_MATCH_TYPE_AND;
    CertUsage.Usage  = EnhkeyUsage;
    ChainPara.cbSize = sizeof(CERT_CHAIN_PARA);
    ChainPara.RequestedUsage=CertUsage;
    
    ChainConfig.cbSize = sizeof(CERT_CHAIN_ENGINE_CONFIG);
    ChainConfig.hRestrictedRoot= NULL ;
    ChainConfig.hRestrictedTrust= NULL ;
    ChainConfig.hRestrictedOther= NULL ;
    ChainConfig.cAdditionalStore=0 ;
    ChainConfig.rghAdditionalStore = NULL ;
    ChainConfig.dwFlags = CERT_CHAIN_CACHE_END_CERT;
    ChainConfig.dwUrlRetrievalTimeout= 0 ;
    ChainConfig.MaximumCachedCertificates=0 ;
    ChainConfig.CycleDetectionModulus = 0;
    
    
    //---------------------------------------------------------
    //   Create the nondefault certificate chain engine.
    
    if(CertCreateCertificateChainEngine(
	&ChainConfig,
	&hChainEngine))
    {
	printf("A chain engine has been created.\n");
    }
    else
    {
	HandleErrorFL ("The engine creation function failed.");
    }
	
	if(CertGetCertificateChain(
	    NULL,                  // Use the default chain engine.
	    pCertContext,          // Pointer to the end certificate.
	    NULL,                  // Use the default time.
	    NULL,                  // Search no additional stores.
	    &ChainPara,            // Use AND logic, and enhanced key usage 
	    // as indicated in the ChainPara 
	    // data structure.
	    dwFlags,
	    NULL,                  // Currently reserved.
	    &pChainContext))       // Return a pointer to the chain created.
	{
	    printf("The chain has been created. \n");
	}
	else
	{
	    HandleErrorFL ("The chain could not be created.");
	}
	
	//---------------------------------------------------------------
	// Display some of the contents of the chain.
	
	printf("The size of the chain context is %d. \n",pChainContext->cbSize);
	printf("%d simple chains found.\n",pChainContext->cChain);
	printf("\nError status for the chain:\n");
	
	switch(pChainContext->TrustStatus.dwErrorStatus)
	{
	case CERT_TRUST_NO_ERROR :
	    printf("No error found for this certificate or chain.\n");
	    break;
	case CERT_TRUST_IS_NOT_TIME_VALID: 
	    printf("This certificate or one of the certificates in the certificate chain is not time-valid.\n");
	    break;
	case CERT_TRUST_IS_NOT_TIME_NESTED: 
	    printf("Certificates in the chain are not properly time-nested.\n");
	    break;
	case CERT_TRUST_IS_REVOKED:
	    printf("Trust for this certificate or one of the certificates in the certificate chain has been revoked.\n");
	    break;
	case CERT_TRUST_IS_NOT_SIGNATURE_VALID:
	    printf("The certificate or one of the certificates in the certificate chain does not have a valid signature.\n");
	    break;
	case CERT_TRUST_IS_NOT_VALID_FOR_USAGE:
	    printf("The certificate or certificate chain is not valid in its proposed usage.\n");
	    break;
	case CERT_TRUST_IS_UNTRUSTED_ROOT:
	    printf("The certificate or certificate chain is based on an untrusted root.\n");
	    break;
	case CERT_TRUST_REVOCATION_STATUS_UNKNOWN:
	    printf("The revocation status of the certificate or one of the certificates in the certificate chain is unknown.\n");
	    break;
	case CERT_TRUST_IS_CYCLIC :
	    printf("One of the certificates in the chain was issued by a certification authority that the original certificate had certified.\n");
	    break;
	case CERT_TRUST_IS_PARTIAL_CHAIN: 
	    printf("The certificate chain is not complete.\n");
	    break;
	case CERT_TRUST_CTL_IS_NOT_TIME_VALID: 
	    printf("A CTL used to create this chain was not time-valid.\n");
	    break;
	case CERT_TRUST_CTL_IS_NOT_SIGNATURE_VALID: 
	    printf("A CTL used to create this chain did not have a valid signature.\n");
	    break;
	case CERT_TRUST_CTL_IS_NOT_VALID_FOR_USAGE: 
	    printf("A CTL used to create this chain is not valid for this usage.\n");
	} // End switch
	
	printf("\nInfo status for the chain:\n");
	switch(pChainContext->TrustStatus.dwInfoStatus)
	{
	case 0:
	    printf("No information status reported.\n");
	    break;
	case CERT_TRUST_HAS_EXACT_MATCH_ISSUER :
	    printf("An exact match issuer certificate has been found for this certificate.\n");
	    break;
	case CERT_TRUST_HAS_KEY_MATCH_ISSUER: 
	    printf("A key match issuer certificate has been found for this certificate.\n");
	    break;
	case CERT_TRUST_HAS_NAME_MATCH_ISSUER: 
	    printf("A name match issuer certificate has been found for this certificate.\n");
	    break;
	case CERT_TRUST_IS_SELF_SIGNED:
	    printf("This certificate is self-signed.\n");
	    break;
	case CERT_TRUST_IS_COMPLEX_CHAIN:
	    printf("The certificate chain created is a complex chain.\n");
	    break;
	} // End switch
	
	//--------------------------------------------------------------------
	//  Free both chains.
	
	CertFreeCertificateChain(pChainContext);
	printf("The Original chain is free.\n");
	printf("\nPress any key to continue.");


//---------------------------------------------------------
//  Free the chain engine.

    CertFreeCertificateChainEngine(hChainEngine);
    printf("The chain engine has been release.\n");


err:
    if (pCertContext)
	CertFreeCertificateContext (pCertContext);
return ret;

}

/*--------------------------------------------------------------*/
/*--------------------------------------------------------------*/
/**/
/* Проверка наличия дополнения keyUsage и extendedKeyUsage  */
/**/
/*--------------------------------------------------------------*/
/*--------------------------------------------------------------*/
static int get_usage (PCCERT_CONTEXT pCertContext)
{
    
    int ret = 0;
    const BYTE* ptemp;
    int i;
    
    DWORD cBufferSize = 0;
    DWORD cbCount;
    
    PCERT_EXTENSION  pCertExtInfo   = NULL;
    
    PCRL_DIST_POINTS_INFO pCRLDist  =NULL;  
    //PCERT_KEY_USAGE_RESTRICTION_INFO pKeyUsage = NULL;
//    PCERT_KEY_ATTRIBUTES_INFO pInfo = NULL;
    PCRYPT_BIT_BLOB pBlob = NULL;
    

        // search for the dist points extension
        pCertExtInfo = CertFindExtension(szOID_CRL_DIST_POINTS/* your OIDhere */, 
	    pCertContext->pCertInfo->cExtension,
	    pCertContext->pCertInfo->rgExtension);
    
    
    if (NULL == pCertExtInfo)
	return ret;
    
    
	// convert the BLOB info into something useful for decoding
	ptemp = pCertExtInfo->Value.pbData;
	cbCount = pCertExtInfo->Value.cbData;
	
	
	if(CryptDecodeObject(
	    TYPE_DER,
	    X509_CRL_DIST_POINTS, // your OID goes here
	    ptemp ,     // The buffer to be decoded.
	    cbCount,
	    CRYPT_DECODE_NOCOPY_FLAG,
	    //0,
	    NULL,
	    &cBufferSize))
	{
	    printf ("The needed buffer length is %d\n",cBufferSize);
	}
	else
	{
	    HandleErrorFL("The first decode pass failed.");
	}
	
	//--------------------------------------------------------------------
	// Allocate memory for the decoded information
	
	pCRLDist = (CRL_DIST_POINTS_INFO*) malloc (cBufferSize*2);
	if(!pCRLDist)
	{
	    HandleErrorFL("Decode buffer memory allocation failed.");
	}
	
	if(CryptDecodeObject(
	    TYPE_DER,
	    X509_CRL_DIST_POINTS,  // your OID goes here
	    ptemp,     // The buffer to be decoded.
	    cBufferSize,
	    CRYPT_DECODE_NOCOPY_FLAG,
	    pCRLDist,
	    &cBufferSize)) // FRANK: the bug was here
	{
	    
	        
	    printf ("Decode object OK!");
	    
	    // do what you need to do...
	}

	pCertExtInfo = NULL;
	
        // search for the dist points extension
        pCertExtInfo = CertFindExtension(szOID_KEY_USAGE/* your OIDhere */, 
	    pCertContext->pCertInfo->cExtension,
	    pCertContext->pCertInfo->rgExtension);
	
    if (NULL == pCertExtInfo)
	return ret;
    
    
	// convert the BLOB info into something useful for decoding
	ptemp = pCertExtInfo->Value.pbData;
	cbCount = pCertExtInfo->Value.cbData;
	
	
	if(CryptDecodeObject(
	    TYPE_DER,
	    X509_KEY_USAGE, // your OID goes here
	    ptemp ,     // The buffer to be decoded.
	    cbCount,
	    CRYPT_DECODE_NOCOPY_FLAG,
	    //0,
	    NULL,
	    &cBufferSize))
	{
	    printf ("The needed buffer length is %d\n",cBufferSize);
	}
	else
	{
	    HandleErrorFL("The first decode pass failed.");
	}
	
	//--------------------------------------------------------------------
	// Allocate memory for the decoded information
	

	//pInfo = (CERT_KEY_ATTRIBUTES_INFO*) malloc (cBufferSize);
	pBlob = (CRYPT_BIT_BLOB*) malloc (cBufferSize);
	if(!pBlob)
	{
	    HandleErrorFL("Decode buffer memory allocation failed.");
	}
	
	if(CryptDecodeObject(
	    TYPE_DER,
	    X509_KEY_USAGE,  // your OID goes here
	    ptemp,     // The buffer to be decoded.
	    cBufferSize,
	    CRYPT_DECODE_NOCOPY_FLAG,
	    pBlob,
	    &cBufferSize)) // FRANK: the bug was here
	{
	    

	    printf ("Decode object OK!");
	    printf ("Number of bytes in the pbData array of bytes: %ld\n", pBlob->cbData);

	    for (i = 0; i < (int) pBlob->cbData; i++) 
	    {
		printf ("Data [%d] : %d\n", i, pBlob->pbData[i]);
	    }
	    printf ("Number of unused bits in the last byte of the array: %ld\n", pBlob->cUnusedBits);
	    
	    
	    // do what you need to do...
	}
	
    return ret;
}

#if !defined(lint) && !defined(NO_RCSID) && !defined(OK_MODULE_VERSION)
static volatile const char rcsid[] = "\n$Id: reqcert.c,v 1.16 2001/12/25 15:57:52 pre Exp $";
#endif

