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
 * \file $RCSfile: cmsenclo.c,v $
 * \version $Revision: 1.1 $
 * \date $Date: 2002/01/14 12:17:53 $
 * \author $Author: lse $
 *
 * \brief ������ �������� � ��������� ��������� CMS Enveloped
 * � �������������� ������� ������� ������
 * (Low Level Message Functions)
 *
 */

#define CMSG_SIGNED_ENCODE_INFO_HAS_CMS_FIELDS      1
#define CMSG_ENVELOPED_ENCODE_INFO_HAS_CMS_FIELDS   1
#define CRYPT_SIGN_MESSAGE_PARA_HAS_CMS_FIELDS      1

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

#define MAX_ADD_SENDERS 64

static int do_cms_low_encrypt (char *in_filename, char *out_filename,
    char *my_certfile, char **recipient_certfile, int recipient_cnt,
    char *OID, int ask);
static int do_cms_low_decrypt (char *in_filename, char *out_filename,
    char *my_certfile, int ask);

typedef HCRYPTPROV WINAPI I_CRYPTGETDEFAULTCRYPTPROVFORENCRYPT (
    ALG_ID KeyExchangeAlgId, ALG_ID EncryptAlgId, DWORD Reserved);

/*--------------------------------------------------------------*/
/* MAIN*/
/*--------------------------------------------------------------*/

int main_cms_encrypt (int argc, char **argv) {
    char *in_filename = NULL;    
    char *out_filename = NULL;
    char *my_certfile = NULL;
    char *recipient_certfile[MAX_ADD_SENDERS];
    int  recipient_cnt = 0;
    int  ret = 0;
    int  encrypt = 0;
    int  decrypt  = 0;
    int  print_help = 0;
    char OID[64] = szOID_CP_GOST_28147;
    int c;
    int ask = 0;
    char *ptr_alg = NULL;
    int reboot = 0;

    /*-----------------------------------------------------------------------------*/
    /* ����������� ����� ������� ����������*/
    static struct option long_options[] = {
	{"in",		required_argument,	NULL, 'i'},
	{"out",		required_argument,	NULL, 'o'},
	{"my",		required_argument,	NULL, 'm'},
	{"cert",	required_argument,	NULL, 'c'},
	{"encrypt",	no_argument,		NULL, 'e'},
	{"decrypt",	no_argument,		NULL, 'd'},
	{"ask",		no_argument,		NULL, 's'},
	{"alg",		required_argument,	NULL, 'a'},
	{"help",	no_argument,		NULL, 'h'},
	{"reboot",	no_argument,		NULL, 'r'},
	{0, 0, 0, 0}
    };

    /*-----------------------------------------------------------------------------*/
    /* ������ ����������*/
    /* ��� ������� ���������� ������������ ������ getopt.c*/
    while ((c = getopt_long_only (argc, argv, "", long_options, (int *)0)) != EOF) {
	switch (c) {
	case 's':
	     ask = 1;
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
	case 'c':
	    if (recipient_cnt > MAX_ADD_SENDERS) {
		fprintf(stderr, "too many recipient certificates (maximum %d)\n", MAX_ADD_SENDERS);
		goto bad;
	    }
	    recipient_certfile[recipient_cnt++] = optarg;
	    break;
	case 'e':
	    encrypt = 1;
	    break;
	case 'd':
	    decrypt = 1;
	    break;
	case 'a':
	    ptr_alg = optarg;
	    if (strcmp(ptr_alg, "RC2") == 0)
		strcpy (OID, szOID_RSA_RC2CBC);
	    else if (strcmp(ptr_alg, "RC4") == 0)
		strcpy (OID, szOID_RSA_RC4);
	    else if (strcmp(ptr_alg, "DES") == 0)
		strcpy (OID, szOID_OIWSEC_desCBC);
	    else if (strcmp(ptr_alg, "3DES") == 0)
		strcpy (OID,  szOID_RSA_DES_EDE3_CBC);
	    else if (strcmp(ptr_alg, "GOST") == 0)
		strcpy (OID, szOID_CP_GOST_28147);
	    else {
		print_help = 1;
		goto bad;
	    }
	    break;
	case 'r':
	    reboot = 1;
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

    if (!encrypt && !decrypt) {
	print_help = 1;
	goto bad;
    }

    if (encrypt) {
	ret = do_cms_low_encrypt (in_filename, out_filename, my_certfile,
	    recipient_certfile, recipient_cnt, OID, ask);
    } else if (decrypt) {
	ret = do_cms_low_decrypt (in_filename, out_filename, my_certfile, ask);
    } else {
	print_help = 1;
    }

    if( reboot )
	cpcsp_reboot( NULL );
bad:
    if (print_help) {
	fprintf(stderr,"%s -cmsenclow [options]\n", prog);
	fprintf(stderr,SoftName " generate CMS Enveloped message \n");
        fprintf(stderr,"using CAPI low level message function type\n");
	fprintf(stderr,"options:\n");
	fprintf(stderr,"  -in arg          input filename to be encrypted or decrypted\n");
	fprintf(stderr,"  -out arg         output CMS filename\n");
	fprintf(stderr,"  -my name         use my certificate with commonName = name from system store 'MY' to sign/verify data\n");
	fprintf(stderr,"                   if certificate not spcified, default provider will be used\n");
	fprintf(stderr,"  -cert            (muliply) recipient certificate file\n");
	fprintf(stderr,"  -encrypt         encrypt input file\n");
	fprintf(stderr,"  -decrypt         decrypt enveloped file, specified by input filename\n");
	fprintf(stderr,"  -ask             acquire context using my certfilename (default: none)\n");
	fprintf(stderr,"  -alg             encryption alghorithm to be used. Default: GOST\n"); 
	fprintf(stderr,"                   additional alg: RC2, RC4, DES, 3DES\n"); 
	fprintf(stderr,"  -help            print this help\n\n");
    }

    return ret;

}

/*----------------------------------------------------------------------*/
/*----------------------------------------------------------------------*/
/**/
/* ������ �������� ������������� ���������*/
/**/
/*----------------------------------------------------------------------*/
/*----------------------------------------------------------------------*/
static int do_cms_low_encrypt (char *in_filename, char *out_filename, char *my_certfile,
    char **recipient_certfile, int recipient_cnt, char *OID, int ask)
{
    HCRYPTPROV  hCryptProv = 0;				/* ���������� ����������*/
    HCRYPTMSG	    hMsg;				/* ��������*/
    BYTE	    *pbEncodedBlob = NULL;		/* ������*/
    DWORD	    cbEncodedBlob = 0;			/* ����� ������*/
    CMSG_ENVELOPED_ENCODE_INFO  EnvelopedEncodeInfo;    /* ��������� ������������ ��� dwMsgType == CMSG_ENVELOPED*/
    CRYPT_ALGORITHM_IDENTIFIER  ContentEncryptAlgorithm; /* ��������� ��������� ���������� ������*/
    DWORD			ContentEncryptAlgSize;   /* ������ ���������*/
    PCCERT_CONTEXT  pRecipientCerts[MAX_ADD_SENDERS];	 /* ������ �������� PCCERT_CONTEXT, ���������� ����������� �����������*/

        /* ������ ���������� ������������ ������ ��� �������� ������� RFC 2630*/
    CMSG_RECIPIENT_ENCODE_INFO RecipArray[MAX_ADD_SENDERS]; 
    CMSG_KEY_AGREE_RECIPIENT_ENCODE_INFO RecipKeyAgreeArray[MAX_ADD_SENDERS]; /* ������ �����������*/
    CMSG_RECIPIENT_ENCRYPTED_KEY_ENCODE_INFO RecipKeyAgreeEncryptedKeysArray[MAX_ADD_SENDERS]; /* ������ �����������*/
    PCMSG_RECIPIENT_ENCRYPTED_KEY_ENCODE_INFO RecipKeyAgreeEncryptedKeysPtrArray[MAX_ADD_SENDERS]; /* ������ �����������*/

    PCCERT_CONTEXT  pUserCert = NULL;	/* ���������� �����������*/
    BYTE *tbenc = NULL; /* �������� ������ ��� ����������*/
    size_t tbenc_len = 0; /* ����� ������*/
    BOOL	    should_release_ctx = 0;  /* ���� �������� ����������� ����������*/
    DWORD	    keytype = 0;	 /* ��� �����*/
    int		    ret = 0;		 /* ��� ��������*/
    int		    i;
    
    /*--------------------------------------------------------------------*/
    /*  ������ ���� ��� ����������*/
    ret = get_file_data_pointer (in_filename, &tbenc_len, &tbenc);
    if (! ret)
	HandleErrorFL("Cannot read input file.");
    /*--------------------------------------------------------------------*/
    /*  �������������� �������� (���� ���������), ��������� ���� ����������*/
    if (ask)
    {
	pUserCert = read_cert_from_my(my_certfile);
	if (!pUserCert) {
	    HandleErrorFL("Caanot read user certificate to acquire context\n.");
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
	fprintf (stderr, "Default Provider will be used with CryptMsgOpenToEncode.\n");
    }
    /*--------------------------------------------------------------------*/
    /*  ������ ����������� �����������*/

    for (i = 0; i < recipient_cnt; i++) {
	PCCERT_CONTEXT tmp;
	tmp = read_cert_from_my ((char*) recipient_certfile[i]);
	if (!tmp)
	    HandleErrorFL("Cannot read sender certfile.");
	pRecipientCerts[i] = tmp;
    }
    
    /*--------------------------------------------------------------------*/
    /* �������������� ��������� CMSG_ENVELOPED_ENCODE_INFO*/
    
    memset(&EnvelopedEncodeInfo, 0, sizeof(CMSG_ENVELOPED_ENCODE_INFO));
    EnvelopedEncodeInfo.cbSize = sizeof(CMSG_ENVELOPED_ENCODE_INFO);
    /* ������������� ���������� ����������*/
    /* ��������!!! �� ����� ���� ���������� � NULL (� ������������ � MSDN)*/
    /* ������������� ������ ��� ����������������.*/
    EnvelopedEncodeInfo.hCryptProv = hCryptProv;
    
    /* �������������� �������� ���������� ������*/
    ContentEncryptAlgSize = sizeof(CRYPT_ALGORITHM_IDENTIFIER);
    memset(&ContentEncryptAlgorithm, 0, ContentEncryptAlgSize); 
    ContentEncryptAlgorithm.pszObjId = OID;   
    EnvelopedEncodeInfo.ContentEncryptionAlgorithm = ContentEncryptAlgorithm;

    EnvelopedEncodeInfo.pvEncryptionAuxInfo = NULL;

    /*--------------------------------------------------------------------*/
    /* �������� ������ �������� CERT_INFO, ��������� �������� ����������*/
    /* �� ����� ���� ��������� ����������� ����������� � ������������� � PCCERT_CONTEXT*/
    for (i = 0; i < recipient_cnt; i++) {
	PCCERT_CONTEXT tmp = pRecipientCerts[i]; 

        RecipArray[i].dwRecipientChoice = CMSG_KEY_AGREE_RECIPIENT;
        RecipArray[i].pKeyAgree = RecipKeyAgreeArray+i;
	ZeroMemory (RecipKeyAgreeArray+i, sizeof (RecipKeyAgreeArray[i]));
	RecipKeyAgreeArray[i].cbSize = sizeof (RecipKeyAgreeArray[i]);
	RecipKeyAgreeArray[i].KeyEncryptionAlgorithm.pszObjId = tmp->pCertInfo->SubjectPublicKeyInfo.Algorithm.pszObjId;
	RecipKeyAgreeArray[i].KeyEncryptionAlgorithm.Parameters = tmp->pCertInfo->SubjectPublicKeyInfo.Algorithm.Parameters;
	RecipKeyAgreeArray[i].dwKeyChoice = CMSG_KEY_AGREE_EPHEMERAL_KEY_CHOICE;
	RecipKeyAgreeArray[i].pEphemeralAlgorithm = &tmp->pCertInfo->SubjectPublicKeyInfo.Algorithm;
	RecipKeyAgreeArray[i].cRecipientEncryptedKeys = 1;
	RecipKeyAgreeArray[i].rgpRecipientEncryptedKeys = RecipKeyAgreeEncryptedKeysPtrArray+i;
	RecipKeyAgreeEncryptedKeysPtrArray[i] = RecipKeyAgreeEncryptedKeysArray+i;
	ZeroMemory (RecipKeyAgreeEncryptedKeysArray+i, sizeof (RecipKeyAgreeEncryptedKeysArray[i]));
	RecipKeyAgreeEncryptedKeysArray[i].cbSize = sizeof (RecipKeyAgreeEncryptedKeysArray[i]);
	RecipKeyAgreeEncryptedKeysArray[i].RecipientPublicKey = tmp->pCertInfo->SubjectPublicKeyInfo.PublicKey;
	RecipKeyAgreeEncryptedKeysArray[i].RecipientId.dwIdChoice = CERT_ID_ISSUER_SERIAL_NUMBER;
	RecipKeyAgreeEncryptedKeysArray[i].RecipientId.IssuerSerialNumber.Issuer = tmp->pCertInfo->Issuer;
	RecipKeyAgreeEncryptedKeysArray[i].RecipientId.IssuerSerialNumber.SerialNumber = tmp->pCertInfo->SerialNumber;
    }
    EnvelopedEncodeInfo.cRecipients = recipient_cnt;

    EnvelopedEncodeInfo.rgCmsRecipients = RecipArray;
    /*--------------------------------------------------------------------*/
    /* �������������� ���������, ��������� �������������� ���������*/
    hMsg = CryptMsgOpenToEncode(
	TYPE_DER,			    /* Encoding type*/
	CMSG_CRYPT_RELEASE_CONTEXT_FLAG,    /* Flags*/
	CMSG_ENVELOPED,			    /* Message type*/
	&EnvelopedEncodeInfo,		    /* Pointer to structure*/
	NULL,				    /* Inner content OID*/
	NULL);				    /* Stream information (not used)*/
    if (hMsg)
	printf("The message to be encoded has been opened. \n");
    else
	HandleErrorFL("OpenToEncode failed.");
    /*-------------------------------------------------------------*/
    /* � ��������� ��������� �������� ������ ��� ����������*/
    ret = CryptMsgUpdate(
	hMsg,		/* Handle to the message*/
	tbenc,		/* Pointer to the content*/
	tbenc_len,	/* Size of the content*/
	TRUE);		/* Last call*/
    if (ret)
	printf("Content has been added to the open message. \n");
    else
	HandleErrorFL("MsgUpdate failed.");
   /*--------------------------------------------------------------------*/
   /* ��������� ������ ������������ ���������*/
    ret = CryptMsgGetParam(
	hMsg,                  /* Handle to the message*/
	CMSG_CONTENT_PARAM,    /* Parameter type*/
	0,                     /* Index*/
	NULL,		       /* Pointer to the blob*/
	&cbEncodedBlob);       /* Size of the blob*/
    if (ret)
	printf("Message length fetched successfully  \n");
    else
	HandleErrorFL("MsgGetParam failed");
    pbEncodedBlob = (BYTE *) malloc(cbEncodedBlob);
    if(pbEncodedBlob)
	printf("Memory has been allocated for the encoded blob. \n");
    else
	HandleErrorFL("Memory allocation for the encoded blob failed.");

    /*--------------------------------------------------------------------*/
    /* ������� ������������� ���������*/
    ret = CryptMsgGetParam(
	hMsg,                  /* Handle to the message*/
	CMSG_CONTENT_PARAM,    /* Parameter type*/
	0,                     /* Index*/
	pbEncodedBlob,         /* Pointer to the blob*/
	&cbEncodedBlob);       /* Size of the blob*/
    if (ret)
	printf("Message encoded successfully  \n");
    else
	HandleErrorFL("MsgGetParam failed");

    if (out_filename) {
        ret = write_file (out_filename, cbEncodedBlob, pbEncodedBlob);
	printf ("Output file (%s) has been saved\n", out_filename);
    }
    /*--------------------------------------------------------------------*/
    /*  ������� ���������*/
    if(hMsg)
	CryptMsgClose(hMsg);
    
    /*--------------------------------------------------------------------*/
    /* ������ ������*/
    if (pUserCert)
	CertFreeCertificateContext(pUserCert);

    for (i = 0; i < recipient_cnt; i++) {
	CertFreeCertificateContext(pRecipientCerts[i]);
    }
#ifndef DEBUG_PRO
    if (should_release_ctx) 
#endif /*DEBUG_PRO*/
    {
	if(hCryptProv)
	{
	    CryptReleaseContext(hCryptProv,0);
	    printf("The CSP has been released. \n");
	}
    }
    release_file_data_pointer (tbenc);
    if (pbEncodedBlob) free (pbEncodedBlob);

    return ret;

}

/*----------------------------------------------------------------------*/
/*----------------------------------------------------------------------*/
/**/
/* ������ ������������� ���������*/
/**/
/*----------------------------------------------------------------------*/
/*----------------------------------------------------------------------*/
static int 
do_cms_low_decrypt (char *in_filename, char *out_filename, char *my_certfile, int ask)
{
    HCRYPTPROV	    hCryptProv = 0;	    /* ���������� ����������*/
    HCRYPTMSG	    hMsg;		    /* ��������*/
    PCCERT_CONTEXT  pUserCert = NULL;	    /* ���������� ����������*/
    BYTE*	    tbenc = NULL;	    /* ������*/
    size_t	    tbenc_len = 0;	    /* ����� ������*/
    BOOL	    should_release_ctx = FALSE; /* if FALSE DO NOT Release CTX*/
    DWORD	    keytype = 0;
    int		    ret = 0;
    DWORD	    choice_opt = CMSG_CTRL_DECRYPT;

    DWORD	    cbData = sizeof(DWORD);
    DWORD	    dwMsgType;
    DWORD	    cbInnerContentObjId = sizeof(DWORD);
    BYTE	    *pbInnerContentObjId = NULL;
    LPSTR	    pszInnerContentObjId;
    DWORD	    cbDecoded;
    BYTE	    *pbDecoded = NULL;
    TCHAR           *choice_name;
    union {
	CMSG_CTRL_KEY_TRANS_DECRYPT_PARA KeyTrans;
	CMSG_CTRL_KEY_AGREE_DECRYPT_PARA KeyAgree;
    } DecryptPara;
    PCMSG_CMS_RECIPIENT_INFO pbDecryptPara = NULL;
    DWORD cbDecryptPara = 0;
    DWORD recip_count = (DWORD)-1;
    DWORD i;
    DWORD j;

    /*--------------------------------------------------------------------*/
    /*  ������ ���� ��� �������������*/
    ret = get_file_data_pointer (in_filename, &tbenc_len, &tbenc);
    if (! ret)
	HandleErrorFL("Cannot read input file.");

    /*--------------------------------------------------------------------*/
    /*  �������������� ��������, ��������� ���� ����������*/

    if (!my_certfile)
	    HandleErrorFL("No user certificate specified.\n");
    
	pUserCert = read_cert_from_my(my_certfile);
	if (!pUserCert) {
	    printf ("Cannot find User certificate: %s\n", my_certfile);
	    goto err;
	}
    /* ��������� �� ��������� ����������� ���������� ������� ���������� �����*/
    /* � ��������� ��������� ���������.*/
    /* ��� ����������� ���������� ������������ ������� CryptAcquireCertificatePrivateKey, */
    /* ���� ��� ������������ � crypt32.dll. ����� ������������ ����� ����� �� ����������� � �����������.*/
    ret = CryptAcquireProvider ("my", pUserCert, &hCryptProv, &keytype, &should_release_ctx);
    if (ret) 
	printf("A CSP has been acquired. \n");
    else
	HandleErrorFL("Cryptographic context could not be acquired.");

    if (ask) {
	fprintf (stderr, "CryptoContext will be inited with CryptMsgOpenToDecode.\n");
        /*--------------------------------------------------------------------*/
	/* �������� ����������� ��������� � ��������� ��������� ����������*/
	hMsg = CryptMsgOpenToDecode(
	    TYPE_DER,		    /* Encoding type*/
	    CMSG_CRYPT_RELEASE_CONTEXT_FLAG,	/* Flags*/
	    0,                      /* Message type (get from message)*/
	    hCryptProv,             /* Cryptographic provider*/
	    NULL,                   /* Recipient information future*/
	    NULL);                  /* Stream information*/
    }
    else {
	fprintf (stderr, "Default Provider will be used with CryptMsgOpenToEncode.\n");
        /*--------------------------------------------------------------------*/
	/* �������� ����������� ��������� ��� �������� ��������� ����������*/
	hMsg = CryptMsgOpenToDecode(
	    TYPE_DER,		    /* Encoding type*/
	    CMSG_CRYPT_RELEASE_CONTEXT_FLAG,                      /* Flags*/
	    0,                      /* Message type (get from message)*/
	    0,			    /* Cryptographic provider*/
	    NULL,                   /* Recipient information future*/
	    NULL);                  /* Stream information*/
    }
    
    if (hMsg)
	printf("The message to decode has been opened. \n");
    else
	HandleErrorFL("OpenToDecode failed");
    /*--------------------------------------------------------------------*/
    /* ���������� � ���������� ��������� ���������� ������ */
    ret = CryptMsgUpdate(
        hMsg,               /* Handle to the message*/
        tbenc,		    /* Pointer to the encoded blob*/
        tbenc_len,	    /* Size of the encoded blob*/
        TRUE);              /* Last call*/
    if (ret) 
	printf("The message to decode has been updated. \n");
    else
	HandleErrorFL("Decode MsgUpdate failed");
    /*--------------------------------------------------------------------*/
    /* ��� ��������*/
    ret = CryptMsgGetParam(
        hMsg,                   /* Handle to the message*/
        CMSG_TYPE_PARAM,        /* Parameter type*/
        0,                      /* Index*/
        &dwMsgType,             /* Address for returned // information*/
        &cbData);               /* Size of the returned // information*/
	
    if (ret) 
	printf("The message type has been retrieved. \n");
    else
	HandleErrorFL("Decode CMSG_TYPE_PARAM failed");

    /*--------------------------------------------------------------------*/
    /* �������������� ������ ��� �������� CMSG_ENVELOPED*/
    
    if(dwMsgType != CMSG_ENVELOPED)
	HandleErrorFL("Message is not Enveloped message.");
    /*-------------------------------------------------------------*/
    /* ����������� �����, ��������� ��� �������� ���� ��������*/
    
    ret = CryptMsgGetParam(
	hMsg,                          /* Handle to the message*/
	CMSG_INNER_CONTENT_TYPE_PARAM, /* Parameter type*/
	0,                             /* Index*/
	NULL,                          /* Address for returned // information*/
	&cbInnerContentObjId);        /* Size of the returned // information*/
    if (!ret) 
	HandleErrorFL("Decode CMSG_INNER_CONTENT_TYPE_PARAM failed");
    /*-------------------------------------------------------------*/
    /* Allocate memory for the string.*/
    
    pbInnerContentObjId = (BYTE *) malloc(cbInnerContentObjId);
    if (!pbInnerContentObjId) 
	HandleErrorFL("Decode inner content malloc operation failed.");
    
    /*-------------------------------------------------------------*/
    /* ����������� ���� ��������*/
  
    ret = CryptMsgGetParam(
	hMsg,                          /* Handle to the message*/
	CMSG_INNER_CONTENT_TYPE_PARAM, /* Parameter type*/
	0,                             /* Index*/
	pbInnerContentObjId,           /* Address for returned information*/
	&cbInnerContentObjId);         /* Size of the returned information*/
    if (ret) 
	printf("The OID of the inner content type is: %s.\n", (LPSTR) pbInnerContentObjId);
    else
	HandleErrorFL("Decode CMSG_INNER_CONTENT_TYPE_PARAM #2 failed");
    /*--------------------------------------------------------------------*/
    /* ����������� ���� ��������*/
    
    pszInnerContentObjId = (LPSTR) pbInnerContentObjId;
    free( pbInnerContentObjId );
    pbInnerContentObjId = NULL;
    
    /*----------------------------------------------------------------*/
    /* ������������� ��������� CMSG_CONTROL_DECRYPT_PARA */
    
    memset(&DecryptPara, 0, sizeof(DecryptPara));

/* ��������� ��������� � ������� RFC 2630*/
    choice_name = NULL;
    /*----------------------------------------------------------------*/
    /* ��������� ������ � ������ ����������� ���������, ��������������� */
    /* �����������, ��������� ��� �������������*/

    cbData = sizeof (recip_count);
    ret = CryptMsgGetParam (hMsg,
	CMSG_RECIPIENT_COUNT_PARAM,
	0,
	&recip_count,
	&cbData);
    if (! ret) 
    	HandleErrorFL("CryptMsgGetParam. CMSG_RECIPIENT_COUNT_PARAM failed.");

    for (i = 0; i < recip_count; i++) {
        ret = CryptMsgGetParam(
	    hMsg,				/* Handle to the message*/
	    CMSG_CMS_RECIPIENT_INFO_PARAM,	/* Parameter type*/
	    i,				/* Index*/
	    NULL,				/* Address for returned // information*/
	    &cbDecryptPara);		/* Size of the returned // information*/
        if (!ret) 
	    HandleErrorFL("Decode CMSG_CMS_RECIPIENT_INFO_PARAM failed");
        /*-------------------------------------------------------------*/
        /* �������������� ������*/
    
        pbDecryptPara = (PCMSG_CMS_RECIPIENT_INFO) malloc(cbDecryptPara);
        if (!pbDecryptPara) 
	    HandleErrorFL("CMSG_CMS_RECIPIENT_INFO_PARAM: malloc operation failed.");
    
        /*-------------------------------------------------------------*/
        /* ����������� ���������� � �����������*/
 
        ret = CryptMsgGetParam(
	    hMsg,                          /* Handle to the message*/
	    CMSG_CMS_RECIPIENT_INFO_PARAM, /* Parameter type*/
	    i,                             /* Index*/
	    pbDecryptPara,			/* Address for returned information*/
	    &cbDecryptPara);		/* Size of the returned information*/
        /*--------------------------------------------------------------------*/
        /* ��������� � ������������ � ��������� �������� ������*/
        if (ret) 
        {
	    switch (pbDecryptPara->dwRecipientChoice)
	    {
	    case CMSG_KEY_TRANS_RECIPIENT:
	        choice_name = "CMSG_KEY_TRANS_RECIPIENT";
	        choice_opt = CMSG_CTRL_KEY_TRANS_DECRYPT;
	        DecryptPara.KeyTrans.cbSize = sizeof(DecryptPara.KeyTrans);
	        DecryptPara.KeyTrans.hCryptProv = hCryptProv; /* Using handle opened in */
	        DecryptPara.KeyTrans.dwKeySpec = AT_KEYEXCHANGE;
	        DecryptPara.KeyTrans.pKeyTrans = pbDecryptPara->pKeyTrans;
	        DecryptPara.KeyTrans.dwRecipientIndex = i;
                /*----------------------------------------------------------------*/
	        /* ���������� ��������� �������� ������� ������������ � ���� ���������*/
	        if (pbDecryptPara->pKeyTrans->RecipientId.dwIdChoice == CERT_ID_ISSUER_SERIAL_NUMBER &&
                    CertCompareCertificateName(TYPE_DER, 
	                &(pUserCert->pCertInfo->Issuer), 
	                &(pbDecryptPara->pKeyTrans->RecipientId.IssuerSerialNumber.Issuer)) &&
	            CertCompareIntegerBlob(
                        &(pUserCert->pCertInfo->SerialNumber),
	                &(pbDecryptPara->pKeyTrans->RecipientId.IssuerSerialNumber.SerialNumber))) {
	            goto found_rec;
	        }
                /* pbDecryptPara->pKeyTrans->RecipientId.dwIdChoice == CERT_ID_KEY_IDENTIFIER */
                /* pbDecryptPara->pKeyTrans->RecipientId.dwIdChoice == CERT_ID_SHA1_HASH */
	        break;
	    case CMSG_KEY_AGREE_RECIPIENT:
	        choice_name = "CMSG_KEY_AGREE_RECIPIENT";
	        choice_opt = CMSG_CTRL_KEY_AGREE_DECRYPT;
	        DecryptPara.KeyAgree.cbSize = sizeof(DecryptPara.KeyAgree);
	        DecryptPara.KeyAgree.hCryptProv = hCryptProv; /* Using handle opened in */
	        DecryptPara.KeyAgree.dwKeySpec = AT_KEYEXCHANGE;
	        DecryptPara.KeyAgree.pKeyAgree = pbDecryptPara->pKeyAgree;
	        DecryptPara.KeyAgree.dwRecipientIndex = i;
	        DecryptPara.KeyAgree.OriginatorPublicKey = pbDecryptPara->pKeyAgree->OriginatorPublicKeyInfo.PublicKey;
                /* ����� ����� ����������� */
                for(j = 0; j < pbDecryptPara->pKeyAgree->cRecipientEncryptedKeys; j++){

                    /*----------------------------------------------------------------*/
	            /* ���������� ��������� �������� ������� ������������ � ���� ���������*/
	            if (pbDecryptPara->pKeyAgree->rgpRecipientEncryptedKeys[j]->RecipientId.dwIdChoice 
                            == CERT_ID_ISSUER_SERIAL_NUMBER &&
                        CertCompareCertificateName(TYPE_DER, 
	                    &(pUserCert->pCertInfo->Issuer), 
	                    &(pbDecryptPara->pKeyAgree->rgpRecipientEncryptedKeys[j]->RecipientId.IssuerSerialNumber.Issuer)) &&
	                CertCompareIntegerBlob(
                            &(pUserCert->pCertInfo->SerialNumber),
	                    &(pbDecryptPara->pKeyAgree->rgpRecipientEncryptedKeys[j]->RecipientId.IssuerSerialNumber.SerialNumber))) {
                        DecryptPara.KeyAgree.dwRecipientEncryptedKeyIndex = j;
	                goto found_rec;
	            }
                    /* pbDecryptPara->pKeyAgree->rgpRecipientEncryptedKeys[j]->RecipientId.dwIdChoice == CERT_ID_KEY_IDENTIFIER */
                    /* pbDecryptPara->pKeyAgree->rgpRecipientEncryptedKeys[j]->RecipientId.dwIdChoice == CERT_ID_SHA1_HASH */
                }
	        break;
	    default:
	        printf("The recipient choice is %d;\n", pbDecryptPara->dwRecipientChoice);
	        HandleErrorFL("It is not supported");
	    }
        }
        else {
	    HandleErrorFL("Decode CMSG_CMS_RECIPIENT_INFO_PARAM #2 failed");
        }

	free( pbDecryptPara );
        pbDecryptPara = NULL;
    }
    HandleErrorFL("Recipient matching with user certificate was not found.");

found_rec:
    printf("The recipient choice is: %s.\n", choice_name);
    if(choice_opt == CMSG_CTRL_KEY_AGREE_DECRYPT) {
        printf("The recipient key index: %d\n", DecryptPara.KeyAgree.dwRecipientEncryptedKeyIndex);
    }

    /*----------------------------------------------------------------*/
    /* ������������� ���������*/
    ret = CryptMsgControl(
        hMsg,				/* Message handle*/
        0,				/* Flags*/
        choice_opt,			/* Control type*/
        &DecryptPara);			/* Address of the parameters*/
    if (! ret) 
    	HandleErrorFL("CryptMsgControl. Decode decryption failed.");
    /*----------------------------------------------------------------*/
    /* ����������� ����� ��������*/
    
    ret = CryptMsgGetParam(
	hMsg,                   /* Handle to the message*/
	CMSG_CONTENT_PARAM,     /* Parameter type*/
	0,                      /* Index*/
	NULL,                   /* Address for returned information*/
	&cbDecoded);            /* Size of the returned information*/
    if (! ret)
    	HandleErrorFL("Decode CMSG_CONTENT_PARAM failed");
    /*----------------------------------------------------------------*/
    /* �������������� ������*/
    
    pbDecoded = (BYTE *) malloc(cbDecoded);
    if (!pbDecoded)
    	HandleErrorFL("Decode memory allocation failed");
    
    /*----------------------------------------------------------------*/
    /* ����������� ��������� ��������*/
    ret = CryptMsgGetParam(
	hMsg,                   /* Handle to the message*/
	CMSG_CONTENT_PARAM,     /* Parameter type*/
	0,                      /* Index*/
	pbDecoded,              /* Address for returned information*/
	&cbDecoded);            /* Size of the returned information*/
    if (ret) {
	printf("Message decoded successfully. \n");
	/* ������ �������� � ����*/
	if (out_filename) {
	    ret = write_file (out_filename, cbDecoded, pbDecoded);
	    printf ("Output file (%s) has been saved\n", out_filename);
	}
    }
    else
	HandleErrorFL("Decode CMSG_CONTENT_PARAM failed");
    /*--------------------------------------------------------------------*/
    /* ������� ������*/
    
    release_file_data_pointer (tbenc);
    if(pbDecoded)
	free(pbDecoded);
    if(hMsg)
	CryptMsgClose(hMsg);
    if(pbDecryptPara)
	free(pbDecryptPara);
err:
#ifndef DEBUG_PRO
    if (should_release_ctx) 
#endif /*DEBUG_PRO*/
    {
	if(hCryptProv)
	{
	    CryptReleaseContext(hCryptProv,0);
	    printf("The CSP has been released. \n");
	}
    }
    return ret;
} 

#if !defined(lint) && !defined(NO_RCSID) && !defined(OK_MODULE_VERSION)
static volatile const char rcsid[] = "\n$Id: cmsenclo.c,v 1.1 2002/01/14 12:17:53 lse Exp $";
#endif
