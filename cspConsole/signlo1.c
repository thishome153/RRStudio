/*
 * Copyright(C) 2000-2001 ������ ���
 *
 * ���� ���� �������� ����������, ����������
 * �������������� �������� ������ ���.
 *
 * ����� ����� ����� ����� �� ����� ���� �����������,
 * ����������, ���������� �� ������ �����,
 * ������������ ��� �������������� ����� ��������,
 * ���������������, �������� �� ���� � ��� ��
 * ����� ������������ ������� ��� ����������������
 * ���������� ���������� � ��������� ������ ���.
 *
 * ����������� ���, ������������ � ���� �����, ������������
 * ������������� ��� ����� �������� � �� ����� ���� �����������
 * ��� ������ ����������.
 *
 * �������� ������-��� �� ����� �������
 * ��������������� �� ���������������� ����� ����.
 */

/*!
 * \file $RCSfile: signlo1.c,v $
 * \version $Revision: 1.5.4.1 $
 * \date $Date: 2002/08/28 07:06:15 $
 * \author $Author: vasilij $
 *
 * \brief ������ �������� � ��������� ����������� ��������� PKCS#7
 * Signed � �������������� ������� ������� ������
 * (Low Level Message Functions)
 *
 */

#include "tmain.h"

int do_low_sign_1 (char *in_filename, char *my_certfile, char *OID, int include, int detached, int repeat);
int do_low_verify_1 (char *in_filename, char *signature_filename, int detached);

/* ���������� ����������*/
HCRYPTPROV	    hCryptProv = 0;	    /* ���������� ����������*/
PCCERT_CONTEXT	    pUserCert = NULL;	    /* ����������, ������������ ��� ������������ ���*/
DWORD		    keytype = 0;	    /* ��� ����� (������������)*/
BOOL		    should_release_ctx = FALSE;	


/* ��������� � ����� � ���������� ���������� ���������� � �����������*/

/*--------------------------------------------------------------*/
/* MAIN*/
/*--------------------------------------------------------------*/

int main_sign_1 (int argc, char **argv)
{
    char *in_filename = NULL;    
    char *out_filename = NULL;
    char *my_certfile = NULL;
    int  Cert_LM = 0;
    int  ret = 0;
    int  sign = 0;
    int  verify  = 0;
    int  print_help = 0;
    char OID[64] = szOID_CP_GOST_R3411;
    int c;
    int  include = 0;
    char *signature_filename = NULL;
    char *ptr_hash_alg = NULL;
    int  detached = 0;
    int	 repeat = 1;
   
    /*-----------------------------------------------------------------------------*/
    /* ����������� ����� ������� ����������*/
    static struct option long_options[] = {
	{"in",		required_argument,	NULL, 'i'},
	{"out",		required_argument,	NULL, 'o'},
	{"my",		required_argument,	NULL, 'm'},
	{"MY",		required_argument,	NULL, 'M'},
	{"sign",	no_argument,		NULL, 'e'},
	{"add",		no_argument,		NULL, '1'},
	{"verify",	no_argument,		NULL, 'd'},
	{"detached",	no_argument,		NULL, 't'},
	{"signature",	required_argument,	NULL, 's'},
	{"alg",		required_argument,	NULL, 'a'},
	{"rep",		required_argument,	NULL, 'r'},
	{"help",	no_argument,		NULL, 'h'},
	{0, 0, 0, 0}
    };

    /*-----------------------------------------------------------------------------*/
    /* ������ ����������*/
    /* ��� ������� ���������� ������������ ������ getopt.c*/
    while ((c = getopt_long_only (argc, argv, "", long_options, (int *)0)) != EOF) {
	switch (c) {
	case 't':
	     detached = 1;
	    break;
	case 's':
	     signature_filename = optarg;
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
	case 'M':
	     my_certfile = optarg;
	     Cert_LM = 1;
	    break;
	case 'e':
	    sign = 1;
	    break;
	case 'd':
	    verify  = 1;
	    break;
	case 'r':
	    repeat = atoi (optarg);
	    break;
	case '1':
	    include = 1;
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


    /* ������ ���������� � �������������� ��������*/
    if (my_certfile)
    {
	if(Cert_LM)
	    pUserCert = read_cert_from_MY(my_certfile);
	else
	    pUserCert = read_cert_from_my(my_certfile);
	if (!pUserCert) {
	    printf ("Cannot find User certificate: %s\n", my_certfile);
	    goto bad;
	}
	/* ��������� �� ��������� ����������� ���������� ������� ���������� �����*/
	/* � ��������� ��������� ���������.*/
	/* ��� ����������� ���������� ������������ ������� CryptAcquireCertificatePrivateKey, */
	/* ���� ��� ������������ � crypt32.dll. ����� ������������ ����� ����� �� ����������� � �����������.*/
	ret = CryptAcquireProvider ("my", pUserCert, &hCryptProv, &keytype, &should_release_ctx);
	if (ret) {
	    printf("A CSP has been acquired. \n");
	}
	else {
	    HandleErrorFL("Cryptographic context could not be acquired.");
	}
    }
    else {
	fprintf (stderr, "No user cert specified. Cryptocontext will be opened automaticaly.\n");
    }

    
    if (sign) {
	ret = do_low_sign_1 (in_filename, my_certfile, OID, include, detached, repeat);
	ret = do_low_sign_1 (in_filename, my_certfile, OID, include, detached, repeat);
	ret = do_low_sign_1 (in_filename, my_certfile, OID, include, detached, repeat);
	ret = do_low_sign_1 (in_filename, my_certfile, OID, include, detached, repeat);
    }
    else if (verify) {
	ret = do_low_verify_1 (in_filename, signature_filename, detached);
    }
    else {
	print_help = 1;
    }

    /* Release Context*/
    if(hCryptProv) 
	CryptReleaseContext(hCryptProv,0);

bad:
    if (print_help) {
	fprintf(stderr,"%s -sfsign [options]\n", prog);
	fprintf(stderr,SoftName " generate PKCS#7 Signed message \nusing CAPI low level message function type\n");
	fprintf(stderr,"options:\n");
	fprintf(stderr,"  -in arg        input filename to be signed or verified\n");
	fprintf(stderr,"  -out arg       output PKCS#7 filename\n");
	fprintf(stderr,"  -my            use my certificate from file in CURRENT_USER store to sign/verify data\n");
        fprintf(stderr,"  -MY            use my certificate from file in LOCAL_MACHINE store to sign/verify data\n");
	fprintf(stderr,"  -sign          signing data from input filename\n");
	fprintf(stderr,"  -detached      save signature in detached file\n");
	fprintf(stderr,"  -add           add sender certificate to PKCS#7\n");
	fprintf(stderr,"  -verify        verify signature on data specified by input filename\n");
	fprintf(stderr,"  -signature     detached signature file\n");
	fprintf(stderr,"  -alg           perform Hash with OID. Default: GOST\n");
	fprintf(stderr,"                 additional alg: SHA1, MD5, MD2\n");
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
int do_low_sign_1 (char *infile, char *outfile, char *OID, int include, int detached, int repeat)
{
    int ret = 0;
    int i;

    if (! infile) {
	fprintf (stderr, "No input file was specified\n");
	return 0;
    }
    /*--------------------------------------------------------------------*/
    /* ��������� ����, ������� ����� �����������.*/

    for (i=0; i<repeat; ++i) {
	BYTE	    *mem_tbs = NULL;	    /* �������� ������*/
	size_t	    mem_len = 0;	    /* ����� ������*/
	HCRYPTMSG	    hMsg = 0;		    /* ���������� ���������*/
	CRYPT_ALGORITHM_IDENTIFIER	HashAlgorithm;	/* ������������� ��������� �����������*/
	DWORD			HashAlgSize;	
	CMSG_SIGNER_ENCODE_INFO	SignerEncodeInfo;   /* ���������, ����������� �����������*/
	CMSG_SIGNER_ENCODE_INFO	SignerEncodeInfoArray[1]; /* ������ ��������, ����������� �����������*/
	CERT_BLOB			SignerCertBlob;
	CERT_BLOB			SignerCertBlobArray[1];
	DWORD			cbEncodedBlob;
	BYTE			*pbEncodedBlob = NULL;
	CMSG_SIGNED_ENCODE_INFO	SignedMsgEncodeInfo;	/* ���������, ����������� ����������� ���������*/
	DWORD			flags = 0;		
	ret = get_file_data_pointer (infile, &mem_len, &mem_tbs);
	if (!ret) {
	    fprintf (stderr, "Cannot read input file\n");
	    goto err;
	}
	
	/*--------------------------------------------------------------------*/
	/* �������������� ��������� ���������*/
	
	HashAlgSize = sizeof(HashAlgorithm);
	memset(&HashAlgorithm, 0, HashAlgSize);
	HashAlgorithm.pszObjId = OID;	    /* ������������� ��������� ����*/
	
	/*--------------------------------------------------------------------*/
	/* �������������� ��������� CMSG_SIGNER_ENCODE_INFO*/
	
	memset(&SignerEncodeInfo, 0, sizeof(CMSG_SIGNER_ENCODE_INFO));
	SignerEncodeInfo.cbSize = sizeof(CMSG_SIGNER_ENCODE_INFO);
	SignerEncodeInfo.pCertInfo = pUserCert->pCertInfo;
	SignerEncodeInfo.hCryptProv = hCryptProv;
	SignerEncodeInfo.dwKeySpec = keytype;
	SignerEncodeInfo.HashAlgorithm = HashAlgorithm;
	SignerEncodeInfo.pvHashAuxInfo = NULL;
	
	/*--------------------------------------------------------------------*/
	/* �������� ������ ������������. ������ ������ �� ������.*/
	
	SignerEncodeInfoArray[0] = SignerEncodeInfo;
	
	/*--------------------------------------------------------------------*/
	/* �������������� ��������� CMSG_SIGNED_ENCODE_INFO*/
	
	SignerCertBlob.cbData = pUserCert->cbCertEncoded;
	SignerCertBlob.pbData = pUserCert->pbCertEncoded;
	
	/*--------------------------------------------------------------------*/
	/* �������������� ��������� ������ �������� CertBlob.*/
	
	SignerCertBlobArray[0] = SignerCertBlob;
	memset(&SignedMsgEncodeInfo, 0, sizeof(CMSG_SIGNED_ENCODE_INFO));
	SignedMsgEncodeInfo.cbSize = sizeof(CMSG_SIGNED_ENCODE_INFO);
	SignedMsgEncodeInfo.cSigners = 1;
	SignedMsgEncodeInfo.rgSigners = SignerEncodeInfoArray;
	SignedMsgEncodeInfo.cCertEncoded = include;
	/* ���� ����� ���� ���������� ����������� �����������*/
	if (include)
	    SignedMsgEncodeInfo.rgCertEncoded = SignerCertBlobArray;
	else
	    SignedMsgEncodeInfo.rgCertEncoded = NULL;
	
	SignedMsgEncodeInfo.rgCrlEncoded = NULL;
	if (detached)
	    flags = CMSG_DETACHED_FLAG;
	
	/*--------------------------------------------------------------------*/
	/* ��������� ����� ������������ ���������*/
	
	cbEncodedBlob = CryptMsgCalculateEncodedLength(
	    TYPE_DER,		/* Message encoding type*/
	    flags,                  /* Flags*/
	    CMSG_SIGNED,            /* Message type*/
	    &SignedMsgEncodeInfo,   /* Pointer to structure*/
	    NULL,                   /* Inner content object ID*/
	    mem_len);		/* Size of content*/
	if(cbEncodedBlob)
	{
	    printf("The length of the data has been calculated: %d.\n", cbEncodedBlob);
	}
	else
	{
	    HandleErrorFL("Getting cbEncodedBlob length failed");
	}
	/*--------------------------------------------------------------------*/
	/* ����������� ������, ��������� �����*/
	
	pbEncodedBlob = (BYTE *) malloc(cbEncodedBlob);
	if (!pbEncodedBlob)
	    HandleErrorFL("Memory allocation failed");
	/*--------------------------------------------------------------------*/
	/* �������� ���������� ���������*/
	
	hMsg = CryptMsgOpenToEncode(
	    TYPE_DER,		/* Encoding type*/
	    flags,                  /* Flags (CMSG_DETACHED_FLAG )*/
	    CMSG_SIGNED,            /* Message type*/
	    &SignedMsgEncodeInfo,   /* Pointer to structure*/
	    NULL,                   /* Inner content object ID*/
	    NULL);                  /* Stream information (not used)*/
	if(hMsg)
	{
	    printf("The message to be encoded has been opened. \n");
	}
	else
	{
	    HandleErrorFL("OpenToEncode failed");
	}
	/*--------------------------------------------------------------------*/
	/* �������� � ��������� ������������� ������*/
	
	if(CryptMsgUpdate(
	    hMsg,		    /* Handle to the message*/
	    mem_tbs,	    /* Pointer to the content*/
	    mem_len,	    /* Size of the content*/
	    TRUE))		    /* Last call*/
	{
	    printf("Content has been added to the encoded message. \n");
	}
	else
	{
	    HandleErrorFL("MsgUpdate failed");
	}
	/*--------------------------------------------------------------------*/
	/* ������ ����������� ��������� ��� ������ �������� ���, ���� ���������� ������� detached*/
	
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
	/* ���� ������, ������� ����������� ������ � ����*/
	if (outfile) {
	    if (write_file (outfile, cbEncodedBlob, pbEncodedBlob))
		printf ("Output file (%s) has been saved\n", outfile);
	}
	/*--------------------------------------------------------------------*/
	/* ������� ������*/
err:
	release_file_data_pointer (mem_tbs);
	if(pbEncodedBlob)
	    free(pbEncodedBlob);
	if(hMsg)
	    CryptMsgClose(hMsg);
	/*    if(hCryptProv) */
	/*	CryptReleaseContext(hCryptProv,0);*/
    }
    return 1;
} /*  End of main*/


/*--------------------------------------------------------------------*/
/*--------------------------------------------------------------------*/
/**/
/* ������ �������� PKCS#7 Signed*/
/**/
/*--------------------------------------------------------------------*/
/*--------------------------------------------------------------------*/
int do_low_verify_1 (char *infile, char *signature_filename, int detached)
{

    int			ret = 0;
    BYTE		*mem_tbs = NULL;
    size_t		mem_len = 0;
    BYTE		*mem_signature = NULL;
    size_t		signature_len = 0;
    HCRYPTMSG		hMsg = 0;		/* ���������� ���������*/
    DWORD		cbDecoded;
    BYTE		*pbDecoded = NULL;
    DWORD		cbSignerCertInfo = 0;
    PCERT_INFO		pSignerCertInfo = NULL;
    PCCERT_CONTEXT	pSignerCertContext = NULL;
    PCERT_INFO		pSignerCertificateInfo = NULL;
    HCERTSTORE		hStoreHandle = NULL;
    DWORD		flags = 0;	/* CMSG_DETACHED_FLAG */

    /*--------------------------------------------------------------------*/
    /*  ���������� ���������� �� ����� ��� ������������� ���������*/

    if (! infile) {
	fprintf (stderr, "No input file was specified\n");
	goto err;
    }
   
    /*--------------------------------------------------------------------*/
    /* ��������� ����, ������� ����� ���������.*/
    ret = get_file_data_pointer (infile, &mem_len, &mem_tbs);
    if (!ret) {
	fprintf (stderr, "Cannot read input file\n");
	goto err;
    }
    
    /*--------------------------------------------------------------------*/
    /* ��������� ���� �������*/
    if (detached && !signature_filename)
	HandleErrorFL("Signature filename was not specified.");

    if (signature_filename && detached) {
	ret = get_file_data_pointer (signature_filename, &signature_len, &mem_signature );
	flags = CMSG_DETACHED_FLAG;
	if (!ret) {
	    fprintf (stderr, "Cannot read signature file\n");
	    goto err;
	}
    }
    /*--------------------------------------------------------------------*/
    /* ������� ��������� ��� �������������*/
    
    hMsg = CryptMsgOpenToDecode(
	TYPE_DER,	    /* Encoding type.*/
	flags,              /* Flags.*/
	0,                  /* Use the default message type.*/
	hCryptProv,         /* Cryptographic provider.*/
	NULL,               /* Recipient information.*/
	NULL);              /* Stream information.*/
    if (hMsg) 
	printf("The message to decode is open. \n");
    else
    	HandleErrorFL("OpenToDecode failed");

    if (flags == CMSG_DETACHED_FLAG) {
    /*--------------------------------------------------------------------*/
    /* ���� ���������� ���� detached, ������� �������*/
	ret = CryptMsgUpdate(
	    hMsg,           
	    mem_signature,  
	    signature_len,  
	    TRUE);          
	if (ret) 
	    printf("The signature blob has been added to the message. \n");
	else 
	    HandleErrorFL("Decode MsgUpdate failed");

	ret = CryptMsgUpdate(
	    hMsg,           /* Handle to the message*/
	    mem_tbs,        /* Pointer to the encoded blob*/
	    mem_len,        /* Size of the encoded blob*/
	    TRUE);         
	if (ret) 
	    printf("The encoded blob has been added to the message. \n");
	else 
	    HandleErrorFL("Decode MsgUpdate failed");

    }
    else {
    /*--------------------------------------------------------------------*/
    /* �������� � ��������� ����������� ������*/
 	ret = CryptMsgUpdate(
	    hMsg,           /* Handle to the message*/
	    mem_tbs,        /* Pointer to the encoded blob*/
	    mem_len,        /* Size of the encoded blob*/
	    TRUE);          /* Last call*/
	if (ret) 
	    printf("The encoded blob has been added to the message. \n");
	else 
	    HandleErrorFL("Decode MsgUpdate failed");
    }

    /*--------------------------------------------------------------------*/
    /* ��������� ����� ����������� ������*/
    
    ret = CryptMsgGetParam(
	hMsg,                  /* Handle to the message*/
	CMSG_CONTENT_PARAM,    /* Parameter type*/
	0,                     /* Index*/
	NULL,                  /* Address for returned info*/
	&cbDecoded);           /* Size of the returned info*/
    if (ret)
	printf("The message parameter (CMSG_CONTENT_PARAM) has been acquired. Message size: %d\n", cbDecoded);
    else
	HandleErrorFL("Decode CMSG_CONTENT_PARAM failed");
    /*--------------------------------------------------------------------*/
    /* ����������� ������*/
    
    pbDecoded = (BYTE *) malloc(cbDecoded);
    if (!pbDecoded)
	HandleErrorFL("Decode memory allocation failed");
    /*--------------------------------------------------------------------*/
    /* ������ ����������� ������*/
    
    ret = CryptMsgGetParam(
	hMsg,                 /* Handle to the message*/
	CMSG_CONTENT_PARAM,   /* Parameter type*/
	0,                    /* Index*/
	pbDecoded,            /* Address for returned */
	&cbDecoded);          /* Size of the returned        */
    if (ret)
	printf("The message param (CMSG_CONTENT_PARAM) updated. Length is %lu.\n",cbDecoded);
    else
	HandleErrorFL("Decode CMSG_CONTENT_PARAM #2 failed");
    /*--------------------------------------------------------------------*/
    /* �������� ���*/
    /* ������� ��������� ���������� CERT_INFO �� �����������.*/
    
    /*--------------------------------------------------------------------*/
    /* ���������� ��������� ������ ��� ���������*/
    /* ��������� ��������� ���������� �� ���������*/
 
    if (! pUserCert) { 
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

    /*--------------------------------------------------------------------*/
    /* ���� ���������� ����� ���������� ������ �������, */
    /* �������� ���������� � ������ � ���� ������������.*/
    /* ��� ������� ������ ��� ����, ����� ����� ������� ���������� �������� */
    /* CertGetSubjectCertificateFromStore, ������� ����� ������������, ����*/
    /* ���������� ����������� ��������� � ����� ���������.*/
    if (pUserCert) {
	hStoreHandle = CertOpenStore(CERT_STORE_PROV_MEMORY, TYPE_DER, 0, CERT_STORE_CREATE_NEW_FLAG,NULL);
	if (!hStoreHandle)
	    HandleErrorFL("Cannot create temporary store in memory.");
	/* ������� ���������� � ����������*/
	if (pUserCert) {
	    ret = CertAddCertificateContextToStore(hStoreHandle, pUserCert, CERT_STORE_ADD_ALWAYS, NULL);
	    pSignerCertInfo = pUserCert->pCertInfo;
	}
	else
	    ret = 0;
	if (!ret)
	    HandleErrorFL("Cannot add user certificate to store.");
    }
    
    /*--------------------------------------------------------------------*/
    /* ���� ���������� �� �����, ������������� ������*/
    
    if (!pUserCert) {
	pSignerCertInfo = (PCERT_INFO) malloc(cbSignerCertInfo);
	if (!pSignerCertInfo)
	    HandleErrorFL("Verify memory allocation failed");
    }
    
    /*--------------------------------------------------------------------*/
    /* ��������� ������� CERT_INFO �� ��������� (���� ���������� �� �����).*/
    
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
    /* ���� ���������� �� ����� � �� ����� ��������� ���������� � ������*/
    /* �������� ����������, ��������� ��������� (CERT_STORE_PROV_MSG),*/
    /* ������� �������������� ������������ �� ���������.*/
    
    if (! hStoreHandle) {
	hStoreHandle = CertOpenStore(
	    CERT_STORE_PROV_MSG,    /* Store provider type */
	    TYPE_DER,		    /* Encoding type*/
	    hCryptProv,             /* Cryptographic provider*/
	    0,                      /* Flags*/
	    hMsg);                  /* Handle to the message*/
	if (hStoreHandle)
	    printf("The message certificate store be used for verifying\n");
    }

    if (! hStoreHandle) {
	    HandleErrorFL("Cannot open certificate store form message\n");
    }
    /*--------------------------------------------------------------------*/
    /* ������ ���������� ����������� � �����������*/
    
    pSignerCertContext = CertGetSubjectCertificateFromStore(
	hStoreHandle,       /* Handle to store*/
	TYPE_DER,	    /* Encoding type*/
	pSignerCertInfo);   /* Pointer to retrieved CERT_CONTEXT*/
    if(pSignerCertContext)
    {
	printf("A signer certificate has been retrieved. \n");
    }
    else
    {
	HandleErrorFL("Verify GetSubjectCert failed");
    }
    /*--------------------------------------------------------------------*/
    /* ��������� ��������� CERT_INFO ��������� ��� ���������*/
    
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

   
err:
    if(hStoreHandle)
	CertCloseStore(hStoreHandle, CERT_CLOSE_STORE_FORCE_FLAG);
    release_file_data_pointer (mem_signature);
    release_file_data_pointer (mem_tbs);
    return ret;

}

#if !defined(lint) && !defined(NO_RCSID) && !defined(OK_MODULE_VERSION)
static volatile const char rcsid[] = "\n$Id: signlo1.c,v 1.5.4.1 2002/08/28 07:06:15 vasilij Exp $";
#endif
