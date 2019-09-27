/*
 * 
 */

/*!
 * \file $RCSfile: signtsf.c,v $
 * \version $Revision: 1.32.4.1 $
 * \date $Date:  $
 * \author $Author:  $
 *
 * \brief ������ �������� � ��������� ����������� ��������� PKCS#7
 * Signed � �������������� ������� �������� ������
 * (Simplified Message Functions)
 *
 */

#include "tmain.h"

static int do_sign (char *infile, char *certfile, char *outfile, int detached, char *OID, int base64, int Cert_LM);
static int do_verify (char *infile, char *certfile, char *outfile, char *signature_filename, int detached, int ask, int base64, int Cert_LM);

/*--------------------------------------------------------------*/
/*--------------------------------------------------------------*/
/**/
/* MAIN*/
/**/
/*--------------------------------------------------------------*/
/*--------------------------------------------------------------*/
int main_sign_sf (int argc, char **argv) {
    
    char    *in_filename = NULL;    
    char    *out_filename = NULL;
    char    *certfile = NULL;
    int	    Cert_LM = 0;
    int	    ret = 0;
    int	    sign = 0;
    int	    verify = 0;
    int	    print_help = 0;
    int	    detached = 0;
    char    *signature_filename = NULL;
    char    OID[64] = szOID_CP_GOST_R3411; //CRYPT_HASH_ALG_OID_ �������� ������� ����������� �� ���� � 34.11-94 ���������� �� ��������
    int    c;
    char    *ptr_hash_alg = NULL;
    int	    ask = 0;
    int     base64 = 0;
   
    /*-----------------------------------------------------------------------------*/
    /* ����������� ����� ������� ����������*/
    static struct option long_options[] = {
	{"in",		required_argument,	NULL, 'i'},
	{"out",		required_argument,	NULL, 'o'},
	{"sign",	no_argument,		NULL, 'g'},
	{"verify",	no_argument,		NULL, 'v'},
	{"my",		required_argument,	NULL, 'x'},
	{"MY",		required_argument,	NULL, 'X'},
	{"detached",	no_argument,		NULL, 'd'},
	{"ask",		no_argument,		NULL, 'k'},
	{"alg",		required_argument,	NULL, 'a'},
	{"signature",	required_argument,	NULL, 's'},
	{"base64",	no_argument,		NULL, 'b'},
	{"help",	no_argument,		NULL, 'h'},
	{0, 0, 0, 0}
    };

    /*-----------------------------------------------------------------------------*/
    /* ������ ����������*/
    /* ��� ������� ���������� ������������ ������ getopt.c*/
    while ((c = getopt_long_only (argc, argv, "", long_options, (int *)0)) != EOF) {
	switch (c) {
	case 'k':
	     ask = 1;
	    break;
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
	case 'X':
	    certfile = optarg;
	    Cert_LM = 1;
	    break;
	case 'a':
	    ptr_hash_alg = optarg;
	    if (strcmp(ptr_hash_alg, "SHA1") == 0)
		strcpy (OID, szOID_OIWSEC_sha1);
	    else if (strcmp(ptr_hash_alg, "GOST") == 0)
		strcpy (OID, szOID_CP_GOST_R3411);
	    else if (strcmp(ptr_hash_alg, "MD2") == 0)
		strcpy (OID, szOID_RSA_MD2);
	    else if (strcmp(ptr_hash_alg, "MD5") == 0)
		strcpy (OID, szOID_RSA_MD5);
	    else {
		print_help = 1;
		goto bad;
	    }
	    break;
        case 'b':
            base64 = 1;
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
	ret = do_sign (in_filename, certfile, out_filename, detached, OID, base64, Cert_LM);
    } 
    else if (verify) {
	ret = do_verify (in_filename, certfile, out_filename, signature_filename, detached, ask, base64, Cert_LM);
    }
    else {
	print_help = 1;
    }

bad:
    if (print_help) {
	fprintf(stderr,"%s -sfsign [options]\n", prog);
	fprintf(stderr,SoftName " generate PKCS#7 Signed message \nusing CAPI simplified message function type\n");
	fprintf(stderr,"options:\n");
	fprintf(stderr,"  -in arg        input filename to be signed or verified\n");
	fprintf(stderr,"  -out arg       output filename (whole PKCS#7 or detached only)\n");
	fprintf(stderr,"  -my name       use my certificate with commonName = name from CURRENT_USER store 'MY' to sign/verify data\n");
	fprintf(stderr,"  -MY name       use my certificate with commonName = name from LOCAL_MACHINE store 'MY' to sign/verify data\n");
	fprintf(stderr,"  -ask           acquire csp context to verify signature using my certfilename (default: none)\n");
	fprintf(stderr,"  -sign          signing data from input filename\n");
	fprintf(stderr,"  -detached      save signature in detached file\n");
	fprintf(stderr,"  -verify        verify signature on data specified by input filename\n");
	fprintf(stderr,"  -signature     detached signature file\n");
	fprintf(stderr,"  -alg           perform Hash with OID. Default: GOST\n");
	fprintf(stderr,"                 additional alg: SHA1, MD5, MD2\n");
	fprintf(stderr,"  -base64        input/output with base64<->DER conversion\n");
	fprintf(stderr,"  -help          print this help\n\n");
    }

    return ret;
}
    
/*--------------------------------------------------------------------*/
/*--------------------------------------------------------------------*/
/**/
/* ������ �������� PKCS#7 Signed*/
/**/
/*--------------------------------------------------------------------*/
/*--------------------------------------------------------------------*/

static int do_sign (char *infile, char *certfile, char *outfile, int detached, char *OID, int base64, int Cert_LM)
{
    PCCERT_CONTEXT  pUserCert = NULL;		/* User certificate to be used*/
    int		    ret = 0;
    BYTE	    *mem_tbs = NULL;
    size_t	    mem_len = 0;

    CRYPT_SIGN_MESSAGE_PARA param;
    DWORD		MessageSizeArray[1];
    const BYTE		*MessageArray[1];
    DWORD		signed_len = 0;
    BYTE		*signed_mem = NULL;  // ������ � ��������
    CRYPT_KEY_PROV_INFO *pCryptKeyProvInfo = NULL;
    DWORD		cbData = 0;
    
    /*--------------------------------------------------------------------*/
    /*  ���������� ��� ����������� ���������� ������� � */
    /* ������ ��� � ������ ������������ ���������*/

    CRYPT_ATTR_BLOB	cablob[1];
    CRYPT_ATTRIBUTE	ca[1];
    LPBYTE		pbAuth = NULL;
    DWORD		cbAuth = 0;
    FILETIME		fileTime;
    SYSTEMTIME		systemTime;
    
    /*--------------------------------------------------------------------*/
    /*  ���������� ���������� �� ����� ��� ������������� ���������*/
    if (! certfile)
    {
	fprintf (stderr, "No user certificate specified\n");
	goto err;
    }
    if (! infile) {
	fprintf (stderr, "No input file was specified\n");
	goto err;
    }
    if(Cert_LM) pUserCert = read_cert_from_MY(certfile);
    else pUserCert = read_cert_from_my(certfile);
    if (!pUserCert) {
	printf ("Cannot find User certificate: %s\n", certfile);
	goto err;
    }
    
    /*--------------------------------------------------------------------*/
    /* ��� ���� ����� ������� CryptAcquireContext �� ��������� ���������*/
    /* ��������� � ���� ����� ������������ ���� CERT_SET_KEY_CONTEXT_PROP_ID ���*/
    /* CERT_SET_KEY_PROV_HANDLE_PROP_ID � �������� ����� ��������� CRYPT_KEY_PROV_INFO.*/
    /**/
    /* ��� ����� ��������� ������� ����� �������� � ����������� ����*/
    ret = CertGetCertificateContextProperty(pUserCert,  
	CERT_KEY_PROV_INFO_PROP_ID, NULL, &cbData);
    if (ret) {
	pCryptKeyProvInfo = (CRYPT_KEY_PROV_INFO *)malloc(cbData);
	if(!pCryptKeyProvInfo)
	    HandleErrorFL("Error in allocation of memory.");
	
	ret = CertGetCertificateContextProperty(pUserCert,
	    CERT_KEY_PROV_INFO_PROP_ID, pCryptKeyProvInfo,&cbData);
	if (ret)
	{
	    /* ��������� ���� ����������� ����������*/
	    pCryptKeyProvInfo->dwFlags = CERT_SET_KEY_CONTEXT_PROP_ID;
	    /* ��������� �������� � ��������� �����������*/
	    ret = CertSetCertificateContextProperty(pUserCert, CERT_KEY_PROV_INFO_PROP_ID, 
		CERT_STORE_NO_CRYPT_RELEASE_FLAG, pCryptKeyProvInfo);
	    free(pCryptKeyProvInfo);
	}
	else		    
	    HandleErrorFL("The property was not retrieved.");
    }
    else {
	printf ("Cannot retrive certificate property.\n");
    }
    

    /*--------------------------------------------------------------------*/
    /* ������� ���� ������� ����� �����������*/
    ret = get_file_data_pointer (infile, &mem_len, &mem_tbs);
    if (!ret) {
	fprintf (stderr, "Cannot open input file\n");
	goto err;
    }

    /*--------------------------------------------------------------------*/
    /* ��������� ���������*/
    
    /* ����������� ����� �������� ��� ���� ���������. */
    /* ����� ��� ����� �������� � access violation � �������� CryptoAPI*/
    /* � ������� �� MSDN ��� �����������*/
    memset(&param, 0, sizeof(CRYPT_SIGN_MESSAGE_PARA));
    param.cbSize = sizeof(CRYPT_SIGN_MESSAGE_PARA);
	param.dwMsgEncodingType = TYPE_DER; //X509_ASN_ENCODING | PKCS_7_ASN_ENCODING; // TYPE_DER;
	param.pSigningCert =  pUserCert;// ������� ����������!! //pUserCert;
    
    param.HashAlgorithm.pszObjId = OID;
    param.HashAlgorithm.Parameters.cbData = 0;
    param.HashAlgorithm.Parameters.pbData = NULL;
    param.pvHashAuxInfo = NULL;	/* �� ������������*/
    param.cMsgCert = 0;		/* 0 -�� �������� ���������� �����������*/ /*If set to zero no certificates are included in the signed message*/
	param.rgpMsgCert =NULL;//pUserCert; // NULL;
    param.cAuthAttr = 0;
    param.dwInnerContentType = 0;
    param.cMsgCrl = 0;  // C����� ������
    param.cUnauthAttr = 0;


   /*---------------------------------------------------------------------------------
    ��������� ��������� ����� � ������� ��� � ������ ����������������� (�����������) 
    ��������� PKCS#7 ��������� � ��������������� szOID_RSA_signingTime.
    ---------------------------------------------------------------------------------*/
    GetSystemTime(&systemTime);
    SystemTimeToFileTime(&systemTime, &fileTime);
    
    /* ��������� ��������� ����� ��� �������� �������*/
    ret = CryptEncodeObject(TYPE_DER,	szOID_RSA_signingTime,	(LPVOID)&fileTime, 
	                         NULL, 	&cbAuth);
    if (!ret)
	HandleErrorFL("Cannot encode object");
        
    pbAuth = (BYTE*) malloc (cbAuth);
    if (!pbAuth)
        HandleErrorFL("Memory allocation error");
    
    /* ����������� ������� � ������� ���� szOID_RSA_signingTime */
    ret = CryptEncodeObject(TYPE_DER,	szOID_RSA_signingTime,	(LPVOID)&fileTime, 	pbAuth, 	&cbAuth);
      if (!ret)
	     HandleErrorFL("Cannot encode object");
    
    cablob[0].cbData = cbAuth;
    cablob[0].pbData = pbAuth;
    
    ca[0].pszObjId = szOID_RSA_signingTime;
    ca[0].cValue = 1;
    ca[0].rgValue = cablob;

    param.cAuthAttr = 1;
    param.rgAuthAttr = ca;

   /*---------------------------------------------------------------------------------
    dwFlags 
    Normally zero. If the encoded output is to be a CMSG_SIGNED inner content 
    of an outer cryptographic message such as a CMSG_ENVELOPED message, 
    the CRYPT_MESSAGE_BARE_CONTENT_OUT_FLAG must be set. 
    If it is not set, the message will be encoded as an inner content type of CMSG_DATA. 
    With Windows 2000, CRYPT_MESSAGE_ENCAPSULATED_CONTENT_OUT_FLAG can be set 
    to encapsulate non-data inner content into an OCTET STRING. 
    Also, CRYPT_MESSAGE_KEYID_SINGER_FLAG can be set to identify signers 
    by their Key Identifier and not their Issuer and Serial Number. 
    ---------------------------------------------------------------------------------*/
    param.dwFlags = 0;
    
    MessageArray[0] = mem_tbs;
    MessageSizeArray[0] = mem_len;

    printf ("Source message length: %lu\n", mem_len);

    /* �������� ������� ������������� ������� CryptSignMessage � ������������� �����*/
    /* � ��������� ������ �������� ������ ���� ��� ����������� ����� ����������� ������*/
    /* (��. ������ ����������� ������ �������������� ����� � ����������� ������������).*/
    /* � ���� ������ ������� CryptSignMessage ���������� ������������� ����������, ���������������� ����������� � */
    /* ������� ������ ��� ����������� �����, ��� �������� � ������������� ������� �������� �����.*/
    /**/
    /* ��� ���� ����� ����� �������� ���������� ����� ������� ��������������� ������������ */
    /* ���������� ������, ����������� ��� �������� ������������ ���������.*/
    
	/*--------------------------------------------------------------------*/
	/* ����������� ����� ������������ ���������*/
	ret = CryptSignMessage(&param, detached, 1,  MessageArray,  MessageSizeArray,  NULL,   &signed_len);
	
	if (ret) {
	    printf("Calculated signature (or signed message) length: %lu\n", signed_len);
	    signed_mem = (BYTE*) malloc (signed_len);
	    if (!signed_mem)
		goto err;
	}
	else
	{
	    HandleErrorFL("Signature creation error");
	}
	/*--------------------------------------------------------------------*/
	/* ������������ ������������ ���������*/
	ret = CryptSignMessage(	    &param,	    detached,	    1,	    MessageArray,	    MessageSizeArray,
	    signed_mem,
	    &signed_len);
	if (ret) {
	    printf("Signature was done. Signature (or signed message) length: %lu\n", signed_len);
	}
	else
	{
	    HandleErrorFL("Signature creation error");
	}	   
	

	/* ������ � ����*/
	if (outfile) {
            if (!base64) {
                if (write_file (outfile, signed_len, signed_mem)) {
		    printf ("Output file (%s) has been saved\n", outfile);
                }
            } else {
                BYTE *pb64;
                DWORD cb64;

                cb64 = 2*signed_len;
                pb64 = malloc(2*signed_len);
                if (!base64_encode(signed_mem, signed_len, pb64, &cb64) ||
                    cb64 >= 2*signed_len) {
	            HandleErrorFL("Base64 conversion error");
                } else if (write_file (outfile, cb64, pb64)) {
		    printf ("Output file (%s) has been saved\n", outfile);
                }
            }
	}
   
err:
    if (signed_mem) free (signed_mem);
    release_file_data_pointer (mem_tbs);
    return ret;
} 

/*--------------------------------------------------------------------*/
/*--------------------------------------------------------------------*/
/**/
/*  ������ �������� ��� PKCS#7 Signed*/
/**/
/*--------------------------------------------------------------------*/
/*--------------------------------------------------------------------*/
static int do_verify (char *infile, char *certfile, char *outfile, char *signature_filename, int detached, int ask, int base64, int Cert_LM)
{
    HCRYPTPROV hCryptProv = 0;		    /* ���������� ����������*/
    BOOL should_release_ctx = FALSE;

    PCCERT_CONTEXT pUserCert = NULL;	    /* ����������, ������������ ��� �������� ���*/

    DWORD keytype = 0;
    int ret = 0;
    int mem_tbs_need_free = 0;
    BYTE *mem_tbs = NULL;
    size_t mem_len = 0;

    int mem_signature_need_free = 0;
    BYTE *mem_signature = NULL;
    size_t signature_len = 0;
    
    CRYPT_VERIFY_MESSAGE_PARA param;
    DWORD MessageSizeArray[1];
    const BYTE* MessageArray[1];

    DWORD signed_len = 0;
    BYTE *signed_mem = NULL;
    
    /*--------------------------------------------------------------------*/
    /*  ���������� ���������� �� ����� ��� ������������� ���������*/

    if (! infile) {
	DebugErrorFL("No input file was specified");
	goto err;
    }
    if (detached && signature_filename == NULL) {
	DebugErrorFL("No detached signature file was specified");
	goto err;
    }
    
    if (certfile)
    {
	if(Cert_LM) pUserCert = read_cert_from_MY(certfile);
	else pUserCert = read_cert_from_my(certfile);
	
	if (!pUserCert) {
	    DebugErrorFL("read_cert_from_my");
	    printf ("Cannot find User certificate: %s\n", certfile);
	    goto err;
	}
    /* ��������� �� ��������� ����������� ���������� ������� ���������� �����*/
    /* � ��������� ��������� ���������.*/
    /* ��� ����������� ���������� ������������ ������� CryptAcquireCertificatePrivateKey, */
    /* ���� ��� ������������ � crypt32.dll. ����� ������������ ����� ����� �� ����������� � �����������.*/
	if (ask) {
	    ret = CryptAcquireProvider ("my", pUserCert, &hCryptProv, &keytype, &should_release_ctx);
	    if (ret) {
		printf("A CSP has been acquired. \n");
	    }
	    else {
		HandleErrorFL("Cryptographic context could not be acquired.");
	    }
	}
    }
    else {
	DebugErrorFL("No user cert specified. Cryptocontext will be opened automaticaly.");
    }

    /*--------------------------------------------------------------------*/
    /* ��������� ����, ������� ����� ���������.*/
    ret = get_file_data_pointer (infile, &mem_len, &mem_tbs);
    if (!ret) {
	DebugErrorFL("Cannot read input file");
	goto err;
    }

    if (base64 && !detached) {
        BYTE *pbDer;
        DWORD cbDer;

        cbDer = mem_len;
        pbDer = malloc(mem_len);
        if (!base64_decode(mem_tbs, mem_len, pbDer, &cbDer) ||
            cbDer >= mem_len) {
	    HandleErrorFL("Base64 conversion error");
            goto err;
        } else {
            release_file_data_pointer(mem_tbs);
            mem_tbs = pbDer;
            mem_tbs_need_free = 1;
            mem_len = cbDer;
        }
    }

    /*--------------------------------------------------------------------*/
    /* ��������� ���� �������*/
    if (detached) {
	if (signature_filename) {
	    ret = get_file_data_pointer (signature_filename, &signature_len, &mem_signature );
	    if (!ret) {
		DebugErrorFL("Cannot read signature file");
		goto err;
	    }
	}
        if (base64) {
            BYTE *pbDer;
            DWORD cbDer;

            cbDer = signature_len;
            pbDer = malloc(signature_len);
            if (!base64_decode(mem_signature, signature_len, pbDer, &cbDer) ||
                cbDer >= signature_len) {
	        HandleErrorFL("Base64 conversion error");
                goto err;
            } else {
                release_file_data_pointer(mem_signature);
                mem_signature = pbDer;
                mem_signature_need_free = 1;
                signature_len = cbDer;
            }
        }
    }

    /*--------------------------------------------------------------------*/
    /* ��������� ��������� ��������� CRYPT_VERIFY_MESSAGE_PARA */
  
    memset(&param, 0, sizeof(CRYPT_VERIFY_MESSAGE_PARA));
    param.cbSize = sizeof(CRYPT_VERIFY_MESSAGE_PARA);
    param.dwMsgAndCertEncodingType = TYPE_DER;

    param.hCryptProv = hCryptProv;  
    if(Cert_LM)
	param.pfnGetSignerCertificate = global_MY_get_cert;
    else
	param.pfnGetSignerCertificate = global_my_get_cert;    /* ���� callback ������ ���������� ���������� �� ������� ��������� ���������*/
    param.pvGetArg = (void*) certfile;		    /* ��������� ��� ����� ����������� � �������*/

    /*------------------------------------------------*/
    /* ��������� ����� ��� �������� ���*/
    /* ��� �������� detached ������������ ������� CryptVerifyDetachedMessageSignature*/
    /* � ��������� ������ ������������ CryptVerifyMessageSignature*/

    if (detached == 0) {
        DWORD dwSignerIndex = 0;    /* ������������ ������ ���� ������� �� ����.*/
				    /* ���� ������ 0*/
	signed_mem = (BYTE*)malloc(signed_len = mem_len);
	if (!signed_mem) {
	    HandleErrorFL("Memory allocation error allocating decode blob.");
	}

	ret = CryptVerifyMessageSignature(
	    &param,
	    dwSignerIndex,
	    mem_tbs,		    /* ����������� ���������*/
	    mem_len,		    /* �����*/
	    signed_mem,		    /* ���� ����� ��������� �������� BYTE *pbDecoded,*/
	    &signed_len,	    /* ���� ��������� �������� DWORD *pcbDecoded,*/
	    NULL);		    /* ������������ ���������� �� ������� ��������� ��� (PCCERT_CONTEXT *ppSignerCert)*/
	
	if (ret) {
	    printf("Signature was verified OK\n");
	}
	else
	{
	    HandleErrorFL("Signature was NOT verified\n");
	    goto err;
	}

	/* ������ �������� � ����*/
	if (outfile && signed_mem && signed_len) {
	    if (write_file (outfile, signed_len, signed_mem))
		printf ("Output file (%s) has been saved\n", outfile);
	}
    } else { /* detached �������*/
	
        DWORD dwSignerIndex = 0;    /* ������������ ������ ���� ������� �� ����.*/
				    
        MessageArray[0] = mem_tbs;
	MessageSizeArray[0] = mem_len;

	/* �������� ���*/
	ret = CryptVerifyDetachedMessageSignature(
	    &param, 
	    dwSignerIndex,
	    (const BYTE*) mem_signature,  /* detached signature*/
	    signature_len,	    /* �� �����*/
	    1,			    /* ���������� ����������� �������� ������*/
	    MessageArray,	    /* ������ �������� ������*/
	    MessageSizeArray,	    /* ������ �������� �������� ������*/
	    &pUserCert);	    /* ������������ ���������� �� ������� ��������� ��� (PCCERT_CONTEXT *ppSignerCert)*/
	    
	if (ret) {
	    printf("Detached Signature was verified OK\n");
	}
	else
	{
	    HandleErrorFL("Detached Signature was NOT verified\n");
	}
    }

    
err:
    if (mem_signature_need_free) {
        free(mem_signature);
    } else {
        release_file_data_pointer(mem_signature);
    }
    mem_signature = NULL;
    if (mem_tbs_need_free) {
        free(mem_tbs);
    } else {
        release_file_data_pointer(mem_tbs);
    }
    mem_tbs = NULL;
    if (signed_mem) {
        free(signed_mem);
        signed_mem = NULL;
    }
    if (should_release_ctx) CryptReleaseContext(hCryptProv, 0);
    return ret;
} 

/*-------------------------------------------------------------------------*/
/*-------------------------------------------------------------------------*/
/* Callback �������, ������������ ��� ����������� ����������� �������� ���*/
/**/
/* ������� ���������� �������� �����������, ��������� ���������� �� ���������� ����������� 'MY'.*/
/* ����� ���������� �� ����� ��������� �����������, ��������� ���������� pvGetArg*/
/* �������� ������� � ��������� ��������� � �������� ���� pfnGetSignerCertificate*/
/* ��������� CRYPT_VERIFY_MESSAGE_PARA */

PCCERT_CONTEXT WINAPI global_my_get_cert (void *pvGetArg, DWORD dwCertEncodingType, PCERT_INFO pSignerId, HCERTSTORE hMsgCertStore)
{

    PCCERT_CONTEXT ret = NULL;

    ret = read_cert_from_my((char*) pvGetArg);
    return ret;
    UNREFERENCED_PARAMETER(dwCertEncodingType);
    UNREFERENCED_PARAMETER(pSignerId);
    UNREFERENCED_PARAMETER(hMsgCertStore);
}
/*-------------------------------------------------------------------------*/
/*-------------------------------------------------------------------------*/
/* Callback �������, ������������ ��� ����������� ����������� �������� ���*/
/**/
/* ������� ���������� �������� �����������, ��������� ���������� �� ���������� ����������� LM 'MY'.*/
/* ����� ���������� �� ����� ��������� �����������, ��������� ���������� pvGetArg*/
/* �������� ������� � ��������� ��������� � �������� ���� pfnGetSignerCertificate*/
/* ��������� CRYPT_VERIFY_MESSAGE_PARA */

PCCERT_CONTEXT WINAPI global_MY_get_cert (void *pvGetArg, DWORD dwCertEncodingType, PCERT_INFO pSignerId, HCERTSTORE hMsgCertStore)
{

    PCCERT_CONTEXT ret = NULL;

    ret = read_cert_from_MY((char*) pvGetArg);
    return ret;
    UNREFERENCED_PARAMETER(dwCertEncodingType);
    UNREFERENCED_PARAMETER(pSignerId);
    UNREFERENCED_PARAMETER(hMsgCertStore);
}

#if !defined(lint) && !defined(NO_RCSID) && !defined(OK_MODULE_VERSION)
static volatile const char rcsid[] = "\n$Id: signtsf.c,v 1.32.4.1 2002/08/28 07:06:21 vasilij Exp $";
#endif
