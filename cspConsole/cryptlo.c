
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
 * \file $RCSfile: cryptlo.c,v $
 * \version $Revision: 1.24.4.3 $
 * \date $Date: 2002/10/04 10:11:08 $
 * \author $Author: wlt $
 *
 * \brief ������ �������� � ��������� ��������� PKCS#7 Enveloped
 * � �������������� ������� ������� ������
 * (Low Level Message Functions)
 *
 */

#include "tmain.h"

#define MAX_ADD_SENDERS 64
    int recipient_Cert_LM[MAX_ADD_SENDERS]; //������ ��� �������� ����� ������ ������������ �����������
					    // (0-CURRENT_USER, 1-LOCAL_MACHINE store)

static int do_low_encrypt (char *in_filename, char *out_filename,
    char *my_certfile, char **recipient_certfile, int recipient_cnt,
    char *OID, int ask, int Cert_LM);
static int do_low_decrypt (char *in_filename, char *out_filename,
    char *my_certfile, int ask, int Cert_LM);

typedef HCRYPTPROV WINAPI I_CRYPTGETDEFAULTCRYPTPROVFORENCRYPT (
    ALG_ID KeyExchangeAlgId, ALG_ID EncryptAlgId, DWORD Reserved);

/*--------------------------------------------------------------*/
/* MAIN*/
/*--------------------------------------------------------------*/

int main_encrypt (int argc, char **argv) {
    char *in_filename = NULL;    
    char *out_filename = NULL;
    char *my_certfile = NULL;
    char *recipient_certfile[MAX_ADD_SENDERS];
    int  recipient_cnt = 0;
    int  ret = 0;
    int  Cert_LM = 0;
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
	{"MY",		required_argument,	NULL, 'M'},
	{"cert",	required_argument,	NULL, 'c'},
	{"CERT",	required_argument,	NULL, 'C'},
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
	case 'M':
	     my_certfile = optarg;
	     Cert_LM = 1;
	    break;
	case 'c':
	    if (recipient_cnt > MAX_ADD_SENDERS) {
		fprintf(stderr, "too many recipient certificates (maximum %d)\n",
		    MAX_ADD_SENDERS);
		goto bad;
	    }
	    recipient_certfile[recipient_cnt] = optarg;
	    recipient_Cert_LM[recipient_cnt]=0;
	    recipient_cnt++;
	    break;
	case 'C':
	    if (recipient_cnt > MAX_ADD_SENDERS) {
		fprintf(stderr, "too many recipient certificates (maximum %d)\n",
		    MAX_ADD_SENDERS);
		goto bad;
	    }
	    recipient_certfile[recipient_cnt] = optarg;
	    recipient_Cert_LM[recipient_cnt]=1;
	    recipient_cnt++;
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
	ret = do_low_encrypt (in_filename, out_filename, my_certfile,
	    recipient_certfile, recipient_cnt, OID, ask, Cert_LM);
    } else if (decrypt) {
	ret = do_low_decrypt (in_filename, out_filename, my_certfile, ask, Cert_LM);
    } else {
	print_help = 1;
    }

    if( reboot )
	cpcsp_reboot( NULL );
bad:
    if (print_help) {
	fprintf(stderr,"%s -lowenc [options]\n", prog);
	fprintf(stderr,SoftName " generate PKCS#7 Enveloped message \nusing CAPI low level message function type\n");
	fprintf(stderr,"options:\n");
	fprintf(stderr,"  -in arg          input filename to be encrypted or decrypted\n");
	fprintf(stderr,"  -out arg         output PKCS#7 filename\n");
	fprintf(stderr,"  -my name         use my certificate with commonName = name from CURRENT_USER store 'MY' to sign/verify data\n");
	fprintf(stderr,"  -MY name         use my certificate with commonName = name from LOCAL_MACHINE store 'MY' to sign/verify data\n");
	fprintf(stderr,"                   if certificate not spcified, default provider will be used\n");
	fprintf(stderr,"  -cert            (muliply) recipient certificate from CURRENT_USER  store 'MY'\n");
	fprintf(stderr,"  -CERT            (muliply) recipient certificate from LOCAL_MACHINE store 'MY'\n");
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
int
do_low_encrypt (char *in_filename, char *out_filename, char *my_certfile,
    char **recipient_certfile, int recipient_cnt, char *OID, int ask, int Cert_LM)
{
    HCRYPTPROV  hCryptProv = 0;				/* ���������� ����������*/
    HCRYPTMSG	    hMsg;				/* ��������*/
    BYTE	    *pbEncodedBlob = NULL;		/* ������*/
    DWORD	    cbEncodedBlob = 0;			/* ����� ������*/
    CMSG_ENVELOPED_ENCODE_INFO  EnvelopedEncodeInfo;    /* ��������� ������������ ��� dwMsgType == CMSG_ENVELOPED*/
    CRYPT_ALGORITHM_IDENTIFIER  ContentEncryptAlgorithm; /* ��������� ��������� ���������� ������*/
    DWORD			ContentEncryptAlgSize;   /* ������ ���������*/
    PCCERT_CONTEXT  pRecipientCerts[MAX_ADD_SENDERS];	 /* ������ �������� PCCERT_CONTEXT, ���������� ����������� �����������*/
    PCERT_INFO	    RecipCertArray[MAX_ADD_SENDERS];	 /* ������ �������� PCERT_INFO, ���������� ����������� �����������*/

/* ������ ���������� ������������ ������ ��� �������� ������� RFC 2630*/
#ifdef CMSG_ENVELOPED_ENCODE_INFO_HAS_CMS_FIELDS
    CMSG_RECIPIENT_ENCODE_INFO RecipArray[MAX_ADD_SENDERS]; 
    CMSG_KEY_AGREE_RECIPIENT_ENCODE_INFO RecipKeyAgreeArray[MAX_ADD_SENDERS]; /* ������ �����������*/
    CMSG_RECIPIENT_ENCRYPTED_KEY_ENCODE_INFO RecipKeyAgreeEncryptedKeysArray[MAX_ADD_SENDERS]; /* ������ �����������*/
    PCMSG_RECIPIENT_ENCRYPTED_KEY_ENCODE_INFO RecipKeyAgreeEncryptedKeysPtrArray[MAX_ADD_SENDERS]; /* ������ �����������*/
#endif

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
	if(Cert_LM)
	    pUserCert = read_cert_from_MY(my_certfile);
	else
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
	if(recipient_Cert_LM[i])
	    tmp = read_cert_from_MY ((char*) recipient_certfile[i]);
	else
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
	RecipCertArray[i] = tmp->pCertInfo;
#ifdef CMSG_ENVELOPED_ENCODE_INFO_HAS_CMS_FIELDS
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
#endif
    }
    EnvelopedEncodeInfo.cRecipients = recipient_cnt;

#ifdef CMSG_ENVELOPED_ENCODE_INFO_HAS_CMS_FIELDS
    EnvelopedEncodeInfo.rgCmsRecipients = RecipArray;
#else
    EnvelopedEncodeInfo.rgpRecipients = RecipCertArray;
#endif
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
int do_low_decrypt (char *in_filename, char *out_filename, char *my_certfile, int ask, int Cert_LM)
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
    DWORD	    recip_count = 0;
    CERT_INFO	    *recip_info = NULL;		    /* ���������� ��� �������� ������ �� ������ �����������*/
    int		    i;
#ifdef CMSG_ENVELOPED_ENCODE_INFO_HAS_CMS_FIELDS
    union {
	CMSG_CTRL_KEY_TRANS_DECRYPT_PARA pKeyTrans;
	CMSG_CTRL_KEY_AGREE_DECRYPT_PARA pKeyAgree;
    } DecryptPara;
    PCMSG_CMS_RECIPIENT_INFO pbDecryptPara = NULL;
    DWORD cbDecryptPara = 0;
#else
    CMSG_CTRL_DECRYPT_PARA DecryptPara;
#endif

    /*--------------------------------------------------------------------*/
    /*  ������ ���� ��� �������������*/
    ret = get_file_data_pointer (in_filename, &tbenc_len, &tbenc);
    if (! ret)
	HandleErrorFL("Cannot read input file.");

    /*--------------------------------------------------------------------*/
    /*  �������������� ��������, ��������� ���� ����������*/

    if (!my_certfile)
	    HandleErrorFL("No user certificate specified.\n");
	if(Cert_LM)
	    pUserCert = read_cert_from_MY(my_certfile);
	else 
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
    
    /*----------------------------------------------------------------*/
    /* ������������� ��������� CMSG_CONTROL_DECRYPT_PARA */
    
    memset(&DecryptPara, 0, sizeof(DecryptPara));

/* ��������� ��������� � ������� RFC 2630*/
#ifdef CMSG_ENVELOPED_ENCODE_INFO_HAS_CMS_FIELDS    
    ret = CryptMsgGetParam(
	hMsg,				/* Handle to the message*/
	CMSG_CMS_RECIPIENT_INFO_PARAM,	/* Parameter type*/
	0,				/* Index*/
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
	0,                             /* Index*/
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
	    DecryptPara.pKeyTrans.cbSize = sizeof(DecryptPara.pKeyTrans);
	    DecryptPara.pKeyTrans.hCryptProv = hCryptProv; /* Using handle opened in */
	    DecryptPara.pKeyTrans.dwKeySpec = AT_KEYEXCHANGE;
	    DecryptPara.pKeyTrans.pKeyTrans = pbDecryptPara->pKeyTrans;
	    DecryptPara.pKeyTrans.dwRecipientIndex = 0;
	    break;
	case CMSG_KEY_AGREE_RECIPIENT:
	    choice_name = "CMSG_KEY_AGREE_RECIPIENT";
	    choice_opt = CMSG_CTRL_KEY_AGREE_DECRYPT;
	    DecryptPara.pKeyAgree.cbSize = sizeof(DecryptPara.pKeyAgree);
	    DecryptPara.pKeyAgree.hCryptProv = hCryptProv; /* Using handle opened in */
	    DecryptPara.pKeyAgree.dwKeySpec = AT_KEYEXCHANGE;
	    DecryptPara.pKeyAgree.pKeyAgree = pbDecryptPara->pKeyAgree;
	    DecryptPara.pKeyAgree.dwRecipientIndex = 0;
	    DecryptPara.pKeyAgree.dwRecipientEncryptedKeyIndex = 0;
	    DecryptPara.pKeyAgree.OriginatorPublicKey = pbDecryptPara->pKeyAgree->OriginatorPublicKeyInfo.PublicKey;
	    break;
	default:
	    printf("The recipient choice is %d;\n", pbDecryptPara->dwRecipientChoice);
	    HandleErrorFL("It is not supported");
	}

	printf("The recipient choice is: %s.\n", choice_name);
    }
    else
	HandleErrorFL("Decode CMSG_CMS_RECIPIENT_INFO_PARAM #2 failed");
/* ��������� ��������� � ������� PKCS#7 */
#else
    DecryptPara.dwRecipientIndex = (DWORD)-1; 

    DecryptPara.cbSize = sizeof(DecryptPara);
    DecryptPara.hCryptProv = hCryptProv; /* Using handle opened in */
    DecryptPara.dwKeySpec = AT_KEYEXCHANGE;
    
    /*----------------------------------------------------------------*/
    /* ��������� ������ � ������ ����������� ���������, ��������������� */
    /* �����������, ��������� ��� �������������*/

    cbData = sizeof (DWORD);
    ret = CryptMsgGetParam (hMsg,
	CMSG_RECIPIENT_COUNT_PARAM,
	0,
	&recip_count,
	&cbData);
    if (! ret) 
    	HandleErrorFL("CryptMsgGetParam. CMSG_RECIPIENT_COUNT_PARAM failed.");

    for (i = 0; i < (int) recip_count; i++) {
	ret = CryptMsgGetParam (hMsg,
	    CMSG_RECIPIENT_INFO_PARAM,
	    i,
	    NULL,
	    &cbData);
	if (! ret) 
    	    HandleErrorFL("CryptMsgGetParam. CMSG_RECIPIENT_INFO_PARAM failed.");

	recip_info = (CERT_INFO*) malloc (cbData);
	if (! recip_info) 
	    HandleErrorFL("Memory allocation failed.");

	ret = CryptMsgGetParam (hMsg,
	    CMSG_RECIPIENT_INFO_PARAM,
	    i,
	    recip_info,
	    &cbData);
	if (! ret) 
    	    HandleErrorFL("CryptMsgGetParam. CMSG_RECIPIENT_INFO_PARAM failed.");

	/*----------------------------------------------------------------*/
	/* ���������� ��������� �������� ������� ������������ � ���� ���������*/
	if (CertCompareCertificateName(TYPE_DER, 
	    &(pUserCert->pCertInfo->Issuer), 
	    &(recip_info->Issuer)) &&
	    CertCompareIntegerBlob(&(pUserCert->pCertInfo->SerialNumber),
	    &(recip_info->SerialNumber))) {
	    free( recip_info );
            recip_info = NULL;
	    DecryptPara.dwRecipientIndex = i; 
	    break;
	}
	free( recip_info );
        recip_info = NULL;
    }
    if (DecryptPara.dwRecipientIndex < 0 )
	HandleErrorFL("Recipient matching with user certificate was not found.");
#endif    
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
    if(recip_info)
        free(recip_info);
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
static volatile const char rcsid[] = "\n$Id: cryptlo.c,v 1.24.4.3 2002/10/04 10:11:08 wlt Exp $";
#endif
