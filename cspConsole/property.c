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
 * \file $RCSfile: property.c,v $
 * \version $Revision: 1.24 $
 * \date $Date: 2001/12/25 15:57:52 $
 * \author $Author: pre $
 *
 * \brief ������ �������� �������� ����������� ������������,
 * ��������� ���������� �� ���������� ����������� 'MY' � �������
 * �����������/����������� ������� ����������� � �����������.
 * ������� ���������� �������� ���� ���������� � ��������
 * (property), ����������� ���������� � ��������� ������
 * � ����������������� (��. �������� ��������� CRYPT_KEY_PROV_INFO).
 *
 * ��� ���� ����� ����� ���� ������������ �������
 * CertSerializeCertificateStoreElement() � 
 * CertAddSerializedElementToStore()
 */

#include "tmain.h"

#define MY_ENCODING_TYPE  (PKCS_7_ASN_ENCODING | X509_ASN_ENCODING)

typedef BOOL (WINAPI *CPCryptAcquireCertificatePrivateKey)(
    PCCERT_CONTEXT pCert, DWORD dwFlags, void *pvReserved,
    HCRYPTPROV *phCryptProv, DWORD *pdwKeySpec, BOOL *pfCallerFreeProv);

/* �������� ������� ������������/�������� ��� � �������������� */
/* ������� CryptSignMessage � ����������� �� �������� ����������� */
int WINAPI property_test (char *in_filename, char *certfile, 
    char *store_type, char *store_name, int repeat);
/* �������� ������� �������� �������� ����������� � ������������*/
/* ���� ������� ����������� �� ���������� ����������� 'MY'*/
PCCERT_CONTEXT WINAPI CertMakeStoreWithProperty (char *cer,
    char *new_store_type, char *new_store_name);
/* �������� ������� �������� �������� �����������*/
PCCERT_CONTEXT MakeStore (char *certfile, char *store_type, char *store_name);
/* �������� ������� ����������� ������� �����������*/
int ShowCertPropery (PCCERT_CONTEXT pCertContext);
/* �������� ������� ����������� ������� �����������*/
int CopyProperty (PCCERT_CONTEXT source, PCCERT_CONTEXT destination);
/* �������� ������� ��������� ����������� �� �������� ����������� � 'MY'*/
int InstallCert (char *filename);
/* �������� main ������� ��������� ����������� �� �������� ����������� � 'MY'*/
int InstallCarrierCertMain (int argc, char **argv);
/* �������� ������� ��������� ����������� � �������� � 'MY'*/
int InstallCarrierCert (char *szContainer, char *szProvider, 
    DWORD dwProvType, DWORD dwAquireContextFlags, DWORD dwKeySpec);
/* ������� ��������� ������ ������������ ����������� �� ����������� � 'MY'*/
int Install1CarrierCert (HCRYPTPROV hProv, HCRYPTKEY hUserKey, 
    DWORD dwAquireContextFlags, DWORD dwOpenStore, LPWSTR lpwStoreName);
int KPSetCertMain (int argc, char **argv);
int KPSetCert (char *szContainer, char *szProvider, 
    DWORD dwProvType, DWORD dwAquireContextFlags, DWORD dwKeySpec,
    const char *certfile, const char *store_type, const char *store_name);
int KPSet1Cert (HCRYPTKEY hUserKey, PCCERT_CONTEXT pCert);

int main_property (int argc, char **argv)
{
    char *in_filename = NULL;
    char *out_filename = NULL;
    char *certfile = NULL;
    int ret = 0;
    int print_help = 0;
    char OID[64] = szOID_CP_GOST_R3411;
    char *store_type = NULL;
    char *store_name = NULL;
    int repeat = 1;
    char *ptr_hash_alg = NULL;
    int c;
    int sign = 0;
    int make = 0;
    int install = 0;
    int carrier_install = 0;

    /* ����������� ����� ������� ����������*/
    static struct option long_options[] = {
	{"in",		required_argument,	NULL, 'i'},
	{"sign",	no_argument,		NULL, 's'},
	{"make",	no_argument,		NULL, 'm'},
	{"cert",	required_argument,	NULL, 'c'},
	{"store",	required_argument,	NULL, 't'},
	{"storename",	required_argument,	NULL, 'n'},
	{"repeat",	required_argument,	NULL, 'r'},
	{"alg",		required_argument,	NULL, 'a'},
	{"install",	no_argument,		NULL, 'l'},
	{"help",	no_argument,		NULL, 'h'},
	{"cinstall",    no_argument,		NULL, 'e'},
        {"setcert",     no_argument,		NULL, 'S'},
	{0, 0, 0, 0}
    };

    /* ������ ����������. ��� ������� ���������� ������������ ������
     * getopt.c */
    while ((c = getopt_long_only (argc, argv, "", long_options, (int *)0))
	!= EOF) {
	switch (c) {
	case 'i':
	     in_filename = optarg;
	    break;
	case 'o':
	     out_filename = optarg;
	    break;
	case 'c':
	    certfile = optarg;
	    break;
	case 'r':
	    repeat = atoi (optarg);
	    break;
	case 's':
	    sign = 1;
	    break;
	case 'm':
	    make = 1;
	    break;
	case 'l':
	    install = 1;
	    break;
	case 'e':
	    return InstallCarrierCertMain (argc,argv);
	case 'S':
	    return KPSetCertMain (argc,argv);
	case 't':
	    store_type = optarg;
	    break;
	case 'n':
	    store_name = optarg;
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

    if (make == 0 && sign == 0 && install == 0 && carrier_install == 0) {
	print_help = 1;
	goto bad;
    }

    if (make) {
	MakeStore (certfile, store_type, store_name);
	ret = 1;
    }
    else if (sign) {
	ret = property_test (in_filename, certfile, store_type, store_name,
	    repeat);
    }
    else if (install) {
	ret = InstallCert (store_name);
    }
    else {
	print_help = 1;
	goto bad;
    }

bad:
    if (print_help) {
	fprintf(stderr,"%s -property [options]\n", prog);
	fprintf(stderr,SoftName " testing programm to create new certificate store '-make'\n");
	fprintf(stderr,"contaning one certificate with progerty from system store 'MY'. Another option\n");
	fprintf(stderr,"'-sign' perform signature/verification test using certificate from new store\n");
	fprintf(stderr,"options:\n");
	fprintf(stderr,"  -cert name     certificate, which property will be obtained from system store 'MY'\n");
	fprintf(stderr,"  -store type    output store in which certificate will be added with property (MEM, FILE)\n");
	fprintf(stderr,"  -storename arg output store file name (not used for store MEM)\n");
	fprintf(stderr,"  -make          make new store\n");
	fprintf(stderr,"  -install       install certificate with property from some store to 'MY'\n");
	fprintf(stderr,"  -cinstall      install certificate from carrier to 'MY' store\n");
	fprintf(stderr,"  -setcert       set certificate from system store to carrier\n");
	fprintf(stderr,"  -sign          perform signature/verification test with new store\n");
	fprintf(stderr,"  -alg           perform Hash with OID. Default: GOST\n");
	fprintf(stderr,"                 additional alg: SHA1, MD5, MD2\n");
	fprintf(stderr,"  -in arg        input filename to be signed with CryptSignMessage function\n");
	fprintf(stderr,"  -repeat cnt    repeant CryptSignMessage function 'cnt' time\n");
	fprintf(stderr,"  -help          print this help\n\n");
    }

    return ret;
}

/*!
 * \brief �������� ���������� � ������ �����������.
 */
PCCERT_CONTEXT
MakeStore (char *certfile, char *store_type, char *store_name) 
{
    PCCERT_CONTEXT pUserCert = CertMakeStoreWithProperty (certfile,
	store_type, store_name);
    return pUserCert;
}

/*----------------------------------------------------------------------*/
/* ������� ������������/�������� ��� � �������������� */
/* ���������� ������ �����������*/
/*----------------------------------------------------------------------*/
int WINAPI
property_test (char *in_filename, char *certfile, char *store_type,
    char *store_name, int repeat)
{
    char OID[64] = szOID_CP_GOST_R3411;
    PCCERT_CONTEXT pUserCert = NULL;
    int i;
    int ret = 0;
    BYTE *mem_tbs = NULL;
    size_t len = 0;
    DWORD MessageSizeArray[1];
    const BYTE *MessageArray[1];
    DWORD signed_len = 0;
    BYTE *signed_mem = NULL;
    BOOL fMore = TRUE;
    DWORD dwTries = 0; 
    HCERTSTORE newStore = 0;
    HANDLE hFile = 0;
    CRYPT_SIGN_MESSAGE_PARA	param;
    CRYPT_VERIFY_MESSAGE_PARA	vparam;
    CRYPT_KEY_PROV_INFO *pCryptKeyProvInfo = NULL;
    DWORD cbData;

    /* ��������� ��������� ���������� */
    store_type = _strupr (store_type);
    if (0 == strcmp (store_type, "MEM")) {
	newStore = CertOpenStore (CERT_STORE_PROV_MEMORY, TYPE_DER, 0,
	    CERT_STORE_DEFER_CLOSE_UNTIL_LAST_FREE_FLAG
	    | CERT_STORE_CREATE_NEW_FLAG, NULL);
    } else if (0 == strcmp (store_type, "FILE")) {
	hFile = CreateFile (store_name, GENERIC_READ | GENERIC_WRITE, 0,
	    NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
	
	newStore = CertOpenStore (CERT_STORE_PROV_FILE, 0, 0,
	    CERT_STORE_READONLY_FLAG, hFile);
    } else {
	printf ("Not supported store type\n");
	goto err;
    }
    if (!newStore) {
	HandleErrorFL ("Cannot create store.");
	goto err;
    }

    /* ������ ������ � ������������ ���������� �� �����������*/
    pUserCert = CertEnumCertificatesInStore(newStore, NULL);
    if (!pUserCert) {
	printf ("No certificate in store\n");
	goto err;
    }
    
    /* ����������� ������� ����������� � �����������*/
    printf ("\nShow user cert property\n");
    ShowCertPropery (pUserCert);
    
    /* ��� ���� ����� ������� CryptAcquireContext �� ��������� ��������� 
     * ��������� � ���� ����� ������������ ���� CERT_SET_KEY_CONTEXT_PROP_ID
     * ��� CERT_SET_KEY_PROV_HANDLE_PROP_ID � �������� ����� ���������
     * CRYPT_KEY_PROV_INFO. ��� ����� ��������� ������� ����� ��������
     * � ����������� ���� */
    ret = CertGetCertificateContextProperty (pUserCert,
	CERT_KEY_PROV_INFO_PROP_ID, NULL, &cbData);
    if (!ret) 
	goto err;
    pCryptKeyProvInfo = (CRYPT_KEY_PROV_INFO *)malloc (cbData);
    if (!pCryptKeyProvInfo)
	HandleErrorFL("Error in allocation of memory.");
    
    ret = CertGetCertificateContextProperty (pUserCert,
	CERT_KEY_PROV_INFO_PROP_ID, pCryptKeyProvInfo,&cbData);
    if (ret) {
	/* ��������� ���� ����������� ���������� */
	pCryptKeyProvInfo->dwFlags = CERT_SET_KEY_CONTEXT_PROP_ID;
	/* ��������� �������� � ��������� ����������� */
	ret = CertSetCertificateContextProperty (pUserCert,
	    CERT_KEY_PROV_INFO_PROP_ID, CERT_STORE_NO_CRYPT_RELEASE_FLAG,
	    pCryptKeyProvInfo);
	free(pCryptKeyProvInfo);
    } else
	HandleErrorFL("The property was not installed.");
  
    ret = get_file_data_pointer (in_filename, &len, &mem_tbs);
    if (!ret)
	return 0;

    /* ������� � �������� � �����*/
    for (i = 0; i < repeat; i++) {
	/* ��������� ���������*/
	param.cbSize = sizeof (CRYPT_SIGN_MESSAGE_PARA);
	param.dwMsgEncodingType = TYPE_DER;
	param.pSigningCert = pUserCert;
	
	param.HashAlgorithm.pszObjId = OID;
	param.HashAlgorithm.Parameters.cbData = 0;
	param.HashAlgorithm.Parameters.pbData = NULL;
	
	param.pvHashAuxInfo = NULL;
	param.cMsgCert = 0; /* �� �������� ���������� �����������*/
	param.rgpMsgCert = NULL;
	param.cAuthAttr = 0;
	param.dwInnerContentType = 0;
	param.cMsgCrl = 0;
	param.cUnauthAttr = 0;
	param.dwFlags = 0;
	param.rgAuthAttr = NULL;
	
	MessageArray[0] = mem_tbs;
	MessageSizeArray[0] = len;
	fMore = TRUE;
	
	while (fMore) {
	    signed_len = len*2 + 256*(dwTries+1); 
	    signed_mem = (BYTE*) malloc (signed_len);
	    if (!signed_mem)
		goto err;
	    /* ������������ ��� ���������*/
	    ret = CryptSignMessage (&param, 0, 1, MessageArray,
		MessageSizeArray, signed_mem, &signed_len);
	    if (ret) {
		printf("Signature was done. Signature (or signed message) "
		    "length: %lu\n", signed_len);
		fMore = FALSE;
	    } else {
		/* ������ ��� ���������� ������� */
		if (signed_mem)
		    free(signed_mem);
		/* ���� ������ �� ������� � ����������� ������,
		 * ��������� ������ */
		if (ERROR_MORE_DATA != GetLastError()) {
		    HandleErrorFL("Signature creation error");
		}
		/* ������ ����������� ��� ������� ����������� ����� ������ */
		if (++dwTries > 1) {
		    HandleErrorFL("Too mach tries");
		}
	    }
	}
	
	/* �������� ���*/
	/* ��������� ��������� ��������� CRYPT_VERIFY_MESSAGE_PARA */
	vparam.cbSize = sizeof (CRYPT_VERIFY_MESSAGE_PARA);
	vparam.dwMsgAndCertEncodingType = TYPE_DER;
	vparam.hCryptProv = 0;  
	vparam.pfnGetSignerCertificate = global_my_get_cert;
	/* ���� callback ������ ������� ����������, �� ������� ���������
	 * ���������*/
	vparam.pvGetArg = (void*) certfile; /* ��������� ��� �����
	 * ����������� � �������*/
	
	ret = CryptVerifyMessageSignature (&vparam, 0, signed_mem, signed_len,
	    NULL, NULL, NULL);
	if (ret) {
	    printf("Signature was verified OK\n");
	} else {
	    HandleErrorFL("Signature was NOT verified\n");
	    goto err;
	}
    }

err:
    release_file_data_pointer (mem_tbs);
    return ret;
}

/*!
 * \brief ������� ������� ���������� ��������� ���� �� ������ �����������
 * � ������������� �������� ��������� �����������.
 *
 *  ������������������ ���������:
 *  - ������ �������� ���������� �� �������� �����
 *  - ���� ����������� ���������� � ����������� 'MY'
 *  - ������� ���������� ���������� ����
 *  - �������� � ���� �������� ����������
 *  - �������� �������� ����������� �� ��������� �����������
 *  - ���������� �������� �����������
 * ��������� ���������� �� �����������, ��� ��� ��������� 
 * ���������� ����� ����������� ��� ������������/�������� ���.
 */
PCCERT_CONTEXT WINAPI
CertMakeStoreWithProperty (char *certname, char *new_store_type,
    char *new_store_name)
{
    HANDLE hCertStore = 0;
    HCERTSTORE newStore = 0;
    PCCERT_CONTEXT pSourceCert = NULL;
    PCCERT_CONTEXT pCertContext = NULL;
    CRYPT_HASH_BLOB  blob;
    BOOL ret = FALSE;
    DWORD len = 0;
    BYTE data[128];
    HANDLE hFile = 0;

    /* ������ �������� ���������� �� ����� */
    if (!new_store_type)
	goto err;
    if (!certname)
	goto err;
    pSourceCert = read_cert_from_my (certname);
    if (!pSourceCert) return NULL;
    
    hCertStore = CertOpenStore (CERT_STORE_PROV_SYSTEM, 0, 0,
	CERT_STORE_OPEN_EXISTING_FLAG |CERT_STORE_READONLY_FLAG
	| CERT_SYSTEM_STORE_CURRENT_USER, L"MY");
    if (!hCertStore) {
	DebugErrorFL("CertOpenStore");
	goto err;
    }

    /* ������� ���������� ���������� ����.
     * �������������� ����:
     * CERT_STORE_PROV_FILENAME
     * CERT_STORE_PROV_MEMORY */
    new_store_type = _strupr (new_store_type);

    if (0 == strcmp (new_store_type, "MEM")) {
	newStore = CertOpenStore (CERT_STORE_PROV_MEMORY, TYPE_DER, 0,
	  CERT_STORE_DEFER_CLOSE_UNTIL_LAST_FREE_FLAG
	  | CERT_STORE_CREATE_NEW_FLAG, NULL);
    } else if (0 == strcmp (new_store_type, "FILE")) {
	hFile = CreateFile (new_store_name, GENERIC_WRITE, 0, NULL,
	    CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
	
	newStore = CertOpenStore (CERT_STORE_PROV_FILE, 0, 0,
	    CERT_STORE_CREATE_NEW_FLAG, hFile);
    } else {
	printf ("Not supported store type\n");
	goto err;
    }
    if (!newStore) {
	printf ("Cannot create store\n");
	goto err;
    }

    /* ��������� �������� ��������� subjectKeyIdentifier
     * � �������� �����������
     * If nonexistent, searches for the szOID_SUBJECT_KEY_IDENTIFIER extension.
     * If that fails, a SHA1 hash is done on the certificate's
     * SubjectPublicKeyInfo to produce the identifier values */
    len = sizeof (data);
    ret = CertGetCertificateContextProperty (pSourceCert,
	CERT_KEY_IDENTIFIER_PROP_ID, (void*) data, &len);
    if (!ret)
	goto err;
    blob.cbData = len;
    blob.pbData = data;

    /* ������ ���������� � ��������������� ��������� */
    /*
    pCertContext = CertFindCertificateInStore (hCertStore, 
	TYPE_DER, 0, CERT_FIND_KEY_IDENTIFIER, &blob, NULL);
    if (!pCertContext) {
	HandleErrorFL("CertFindCertificateInStore failed\n");
	goto err;
    }
    */

    /* ����������� �������*/
    /* ret = CopyProperty (pCertContext, pSourceCert); */
    /* printf ("\nShow property froms new store\n"); */
    /* ShowCertPropery (pCertContext); */

    printf ("\nShow property froms store 'MY'\n");
    ShowCertPropery (pSourceCert);
    /* ������� �������� ���������� � ����� ����������*/
    ret = CertAddCertificateContextToStore (newStore, pSourceCert,
	CERT_STORE_ADD_REPLACE_EXISTING, NULL);
    if (!ret) {
	printf ("Cannot add certificate to store\n");
	goto err;
    }
err:
    if (pCertContext)
	CertFreeCertificateContext (pCertContext);
    if (hCertStore)
	CertCloseStore (hCertStore, CERT_CLOSE_STORE_FORCE_FLAG);
    if (pSourceCert)
	CertFreeCertificateContext (pSourceCert);
    if (newStore) {
	if (hFile) {
	    ret = CertSaveStore(newStore,TYPE_DER, CERT_STORE_SAVE_AS_STORE,
		CERT_STORE_SAVE_TO_FILE, hFile, 0);
	}
	CertCloseStore (newStore, CERT_CLOSE_STORE_FORCE_FLAG);
	if (hFile)
	    CloseHandle (hFile);
    }
    return NULL;
}

/*!
 * \brief ������� ����������� ������� �����������.
 */
int
CopyProperty (PCCERT_CONTEXT source, PCCERT_CONTEXT destination)
{
    int ret = 0;
    DWORD cbData;
    void *pvData;
    DWORD dwPropId = 0; /* 0 to find the first property ID.*/
	
    while (0 != (dwPropId = CertEnumCertificateContextProperties (
	source, dwPropId))) {
	ret = CertGetCertificateContextProperty (source, dwPropId, NULL,
	    &cbData);
	if (!ret)
	    return 0;
	pvData = (BYTE*) malloc (cbData);
	if (!pvData)
	    return 0;
	ret = CertGetCertificateContextProperty (source, dwPropId,pvData,
	    &cbData);
	if (!ret)
	    return 0;
	if (dwPropId != CERT_KEY_IDENTIFIER_PROP_ID) {
	    ret = CertSetCertificateContextProperty (destination, dwPropId,
		0, pvData);
	} else {
	    ret = CertGetCertificateContextProperty (destination, dwPropId,
		NULL, &cbData);
	}
	free (pvData);
    } 
    return ret;
}

/*!
 * \brief ������� ����������� ������� �����������.
 */
int
ShowCertPropery (PCCERT_CONTEXT pCertContext)
{
    CRYPT_KEY_PROV_INFO *pCryptKeyProvInfo = NULL;
    void *pvData;
    DWORD cbData;
    int ret = 1;
    DWORD dwPropId = 0; /* 0 to find the first property ID.*/

    /* � ����� ��������� ��� �������� (property) �����������, ������������ �
     * �������� ����������� � ������������� ����������� � ����� ����������.
     * ���� ������������ CertEnumCertificateContextProperties ������ 0.*/

    while (0 != (dwPropId = CertEnumCertificateContextProperties (
	pCertContext,dwPropId))) {
	/* Each time through the loop, a property ID has been found.*/
	/* Print the property number and information about the property.*/
	printf("Property # %d found->", dwPropId);
	switch(dwPropId) {
	case CERT_FRIENDLY_NAME_PROP_ID:
	    /*  Retrieve the actual friendly name certificate property.*/
	    /*  First, get the length of the property setting the*/
	    /*  pvData parameter to NULL to get a value for cbData*/
	    /*  to be used to allocate memory for the pvData buffer.*/
	    printf("FRIENDLY_NAME_PROP_ID ");
	    if (!(CertGetCertificateContextProperty (pCertContext, dwPropId,
		NULL, &cbData)))
		goto err;
	    /* The call succeeded. Use the size to allocate memory for the */
	    /* property.*/
	    pvData = (void*)malloc (cbData);
	    if (!pvData) {
		HandleErrorFL("Memory allocation failed.");
	    }
	    /* Allocation succeeded. Retrieve the property data.*/
	    if (!(CertGetCertificateContextProperty (pCertContext, dwPropId,
		pvData, &cbData))) {
		HandleErrorFL("Call #2 getting the data failed.");
	    } else {
		printf("\n  The friendly name is -> %s.", pvData);
		free(pvData);
	    }
	    break;
	case CERT_SIGNATURE_HASH_PROP_ID:
	    printf("Signature hash ID. ");
	    break;
	case CERT_KEY_PROV_HANDLE_PROP_ID:
	    printf("KEY PROVIDER HANDLE.");
	    break;
	case CERT_KEY_PROV_INFO_PROP_ID:
	    printf("KEY PROV INFO PROP ID.");
	    if (!(CertGetCertificateContextProperty (pCertContext, dwPropId,
		NULL, &cbData)))
		goto err;
	    pCryptKeyProvInfo = (CRYPT_KEY_PROV_INFO *)malloc (cbData);
	    if (!pCryptKeyProvInfo)
		HandleErrorFL("Error in allocation of memory.");
	    
	    if (CertGetCertificateContextProperty (pCertContext, dwPropId,
		pCryptKeyProvInfo, &cbData)) {
		printf("\nThe current key container is %S\n", pCryptKeyProvInfo->pwszContainerName);
		printf("The provider name is:%S",pCryptKeyProvInfo->pwszProvName);
		free(pCryptKeyProvInfo);
	    } else
		HandleErrorFL("The property was not retrieved.");
	    break;
	case CERT_SHA1_HASH_PROP_ID:
	    printf("SHA1 HASH id.");
	    break;
	case CERT_MD5_HASH_PROP_ID:
	    printf("md5 hash id. ");
	    break;
	case CERT_KEY_CONTEXT_PROP_ID:
	    printf("KEY CONTEXT PROP id.");
	    break;
	case CERT_KEY_SPEC_PROP_ID:
	    printf("KEY SPEC PROP id.");
	    break;
	case CERT_ENHKEY_USAGE_PROP_ID:
	    printf("ENHKEY USAGE PROP id.");
	    break;
	case CERT_NEXT_UPDATE_LOCATION_PROP_ID:
	    printf("NEXT UPDATE LOCATION PROP id.");
	    break;
	case CERT_PVK_FILE_PROP_ID:
	    printf("PVK FILE PROP id. ");
	    break;
	case CERT_DESCRIPTION_PROP_ID:
	    printf("DESCRIPTION PROP id. ");
	    break;
	case CERT_ACCESS_STATE_PROP_ID:
	    printf("ACCESS STATE PROP id. ");
	    break;
	case CERT_SMART_CARD_DATA_PROP_ID:
	    printf("SMAART_CARD DATA PROP id. ");
	    break;
	case CERT_EFS_PROP_ID:
	    printf("EFS PROP id. ");
	    break;
	case CERT_FORTEZZA_DATA_PROP_ID:
	    printf("FORTEZZA DATA PROP id.");
	    break;
	case CERT_ARCHIVED_PROP_ID:
	    printf("ARCHIVED PROP id.");
	    break;
	case CERT_KEY_IDENTIFIER_PROP_ID:
	    printf("KEY IDENTIFIER PROP id. ");
	    break;
	case CERT_AUTO_ENROLL_PROP_ID:
	    printf("AUTO ENROLL id. ");
	    break;
        }
        printf("\n");
      }
err:
      return ret;
}

int WINAPI
property_test1 (char *certfile, int repeat)
{
    PCCERT_CONTEXT pUserCert = NULL; /* ���������� �����������*/
    while (repeat-- > 0) {
	DWORD dwStrType = CERT_X500_NAME_STR; /* CERT_OID_NAME_STR;*/
	LPSTR pszName;
	DWORD cbName;
	
	pUserCert = read_cert_from_my(certfile);
	if (!pUserCert) {
	    HandleErrorFL("Cannot read user certificate to acquire context\n.");
	}
	/* Convert the subject name to an ASN1 encoded
	 * string and print the octets in that string.
	 * First : Get the number of bytes that must
	 * be allocated for the string. */
	cbName = CertNameToStr (MY_ENCODING_TYPE,
	    &(pUserCert->pCertInfo->Subject), dwStrType, NULL, 0);
	/*  The function CertNameToStr returns the number
	 *  of bytes needed for a string to hold the
	 *  converted name. If it returns zero, there
	 *  has been an error. */
	if (0 == cbName) {
	    HandleErrorFL("Getting lenght of name failed.");
	}
	/* Allocated the needed buffer. Note that this
	 * memory must be freed inside the loop or the
	 * application could use up all memory. */
	pszName = (char *)malloc (cbName);
	if (!pszName) {
	    HandleErrorFL("Memory allocation failed.");
	}
	/* Call the function again to get the string */
	cbName = CertNameToStr (MY_ENCODING_TYPE,
	    &(pUserCert->pCertInfo->Subject), dwStrType, pszName, cbName);
	/* If the function succeeded, it returns the 
	 * number of bytes copied to the pszName buffer.*/
	if (cbName < 1) {
	    HandleErrorFL("Getting name failed.");
	}
	printf ("Cert found: %s\n", pszName);
	ShowCertPropery (pUserCert);
        /* ������ ������*/
	free(pszName);
        if (pUserCert)
	    CertFreeCertificateContext (pUserCert);
    }
    return 1;
}

int
main_property1 (int argc, char **argv)
{
    char *certfile = NULL;
    int ret = 0;
    int print_help = 0;
    int repeat = 1;
    int c;
   
    /* ����������� ����� ������� ����������*/
    static struct option long_options[] = {
	{"cert",	required_argument,	NULL, 'c'},
	{"repeat",	required_argument,	NULL, 'r'},
	{"help",	no_argument,		NULL, 'h'},
	{0, 0, 0, 0}
    };
    /* ������ ����������*/
    /* ��� ������� ���������� ������������ ������ getopt.c*/
    while ((c = getopt_long_only (argc, argv, "", long_options, (int *)0))
	!= EOF) {
	switch (c) {
	case 'c':
	    certfile = optarg;
	    break;
	case 'r':
	    repeat = atoi (optarg);
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
    if (!certfile) {
	print_help = 1;
	goto bad;
    }
    ret = property_test1 (certfile, repeat);
bad:
    if (print_help) {
	fprintf(stderr,"%s -property [options]\n", prog);
	fprintf(stderr,SoftName " testing programm to find a certificate in store 'MY'\n");
	fprintf(stderr,"options:\n");
	fprintf(stderr,"  -cert fname    certificate, which property will be obtained from system store 'MY'\n");
	fprintf(stderr,"  -repeat cnt    repeat CryptSignMessage function 'cnt' time\n");
	fprintf(stderr,"  -help          print this help\n\n");
    }
    return ret;
}

/*!
 * \brief ������� ��������� ����������� �� �������� ����������� � 'MY'.
 */
int InstallCert (char *filename)
{
    int ret = 0;
    HCERTSTORE newStore = 0;
    HCERTSTORE hCertStore = 0;
    HANDLE hFile = 0;
    PCCERT_CONTEXT pUserCert = 0;

    /* ��������� ��������� ����������*/
    if (filename == NULL) {
	printf ("No store name was sprecified\n");
	return ret;
    }
    hFile = CreateFile (filename, GENERIC_READ, 0, NULL, OPEN_ALWAYS,
	FILE_ATTRIBUTE_NORMAL, NULL);
    if (!hFile) {
	HandleErrorFL ("Cannot read file.");
	goto err;
    }
    newStore = CertOpenStore (CERT_STORE_PROV_FILE, 0, 0,
	CERT_STORE_READONLY_FLAG, hFile);
    if (!newStore) {
	HandleErrorFL ("Cannot create store.");
	goto err;
    }
    /* ��������� ���������� 'MY' */
    hCertStore = CertOpenStore (CERT_STORE_PROV_SYSTEM, 0, 0,
	CERT_STORE_OPEN_EXISTING_FLAG | CERT_SYSTEM_STORE_CURRENT_USER,
	L"MY");
    if (!hCertStore) {
	DebugErrorFL("CertOpenStore");
	goto err;
    }
    /* ������ ������ � ������������ ���������� �� �����������*/
    pUserCert = CertEnumCertificatesInStore (newStore, NULL);
    if (!pUserCert) {
	printf ("No certificate in store\n");
	goto err;
    }
    /* ����������� ������� ����������� � �����������*/
    printf ("\nShow user cert property\n");
    ShowCertPropery (pUserCert);
#if 0
    /* ��� ���� ����� ������� CryptAcquireContext �� ��������� ���������
     * ��������� � ���� ����� ������������ ���� CERT_SET_KEY_CONTEXT_PROP_ID
     * ��� CERT_SET_KEY_PROV_HANDLE_PROP_ID � �������� ����� ���������
     * CRYPT_KEY_PROV_INFO. ��� ����� ��������� ������� ����� ��������
     * � ����������� ���� */
    ret = CertGetCertificateContextProperty (pUserCert,
	CERT_KEY_PROV_INFO_PROP_ID, NULL, &cbData);
    if (!ret) 
	goto err;
    if (!(pCryptKeyProvInfo = (CRYPT_KEY_PROV_INFO *)malloc (cbData)))
	HandleErrorFL("Error in allocation of memory.");
    
    ret = CertGetCertificateContextProperty (pUserCert,
	CERT_KEY_PROV_INFO_PROP_ID, pCryptKeyProvInfo,&cbData);
    if (ret) {
	/* ��������� ���� ����������� ����������*/
	pCryptKeyProvInfo->dwFlags = CERT_SET_KEY_CONTEXT_PROP_ID;
	/* ��������� �������� � ��������� �����������*/
	ret = CertSetCertificateContextProperty (pUserCert, CERT_KEY_PROV_INFO_PROP_ID, 
	    CERT_STORE_NO_CRYPT_RELEASE_FLAG, pCryptKeyProvInfo);
	free(pCryptKeyProvInfo);
    } else
	HandleErrorFL("The property was not installed.");
#endif
    /* ������� ���������� � ����������*/
    ret = CertAddCertificateContextToStore(hCertStore, pUserCert,
	CERT_STORE_ADD_REPLACE_EXISTING, NULL);
    if (!ret) {
	printf ("Cannot add certificate to store\n");
	goto err;
    }
err:
    if (pUserCert)
    	CertFreeCertificateContext (pUserCert);
    if (hCertStore)
	CertCloseStore (hCertStore, CERT_CLOSE_STORE_FORCE_FLAG);
    if (hFile) 
        CloseHandle (hFile);
    if (newStore) 
	CertCloseStore (newStore, CERT_CLOSE_STORE_FORCE_FLAG);
    return ret;
}

int
InstallCarrierCertMain (int argc, char **argv)
{
    char *szContainer = NULL; /* ��� ��������� ����������*/
    char *szProvider = NULL; /* ��� ����������*/
    DWORD dwProvType = PROV_GOST_DH; /*��� ���������� �� ���������*/
    int dwKeyType = AT_KEYEXCHANGE | AT_SIGNATURE;
    DWORD dwAquireContextFlags = 0;
    int print_help = 0;
    int ret = 0;
    int c;
    /* ����������� ����� ������� ����������*/
    static struct option long_options[] = {
	{"keytype",		required_argument,	NULL, '1'},
	{"container",		required_argument,	NULL, 'c'},
	{"provider",		required_argument,	NULL, 'p'},
	{"provtype",		required_argument,	NULL, 't'},
	{"machine",		no_argument,		NULL, 'm'},
	{"help",		no_argument,		NULL, 'h'},
	{0, 0, 0, 0}
    };
    /* ������ ����������, ��� ������� ���������� ������������ ������
     * getopt.c*/
    while ((c = getopt_long_only (argc, argv, "", long_options, (int *)0))
	!= EOF) {
	switch (c) {
	case 'p':
	    szProvider = abbr2provider (optarg);
	    break;
	case 'v':
	    dwProvType = abbr2provtype (optarg);
	    break;
	case 'c':
	    szContainer = optarg;
	    break;
        case 'm':
            dwAquireContextFlags = CRYPT_MACHINE_KEYSET;
            break;
	case '1':
	    {
		char *ptr;
		ptr = optarg;
		if (strcmp(ptr, "signature") == 0)
		    dwKeyType = AT_SIGNATURE;
		else if (strcmp(ptr, "exchange") == 0)
		    dwKeyType = AT_KEYEXCHANGE;
		else if (strcmp(ptr, "none") == 0)
		    dwKeyType = 0;
		else {
		    print_help = 1;
		    goto bad;
		}
	    }
	    break;
	case 'h':
	case '?':
	    ret = 1;
	default:
	    print_help = 1;
	    goto bad;
	}
    }
    if (c != EOF) {
	print_help = 1;
	goto bad;
    }
    ret = InstallCarrierCert(szContainer, szProvider, dwProvType, dwAquireContextFlags,
	dwKeyType);
bad:
    if (print_help) {
	fprintf(stderr,"%s -property -cinstall [options]\n", prog);
	fprintf(stderr,"options:\n");
	printf ("  -provider name    specify provider name or next abbriviation:\n");
	printf ("                   ");
	printf ("   cpDef\n");
	printf ("                   ");
	printf ("   msDef ");
	printf ("msEnhanced ");
#ifdef MS_STRONG_PROV
	printf ("msStrong ");
#endif /* MS_STRONG_PROV */
	printf ("\n                   ");
	printf ("   msDefRsaSig ");
	printf ("msDefRsaSchannel ");
#ifdef MS_ENHANCED_RSA_SCHANNEL_PROV
	printf ("msEnhancedRsaSchannel ");
#endif /* MS_ENHANCED_RSA_SCHANNEL_PROV */
	printf ("msDefDss ");
	printf ("\n                   ");
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
	printf ("  -provtype type    specify provider type or next abbriviation:\n");
	printf ("                   ");
	printf ("   CProCSP\n");
	printf ("                   ");
	printf ("   RsaFull ");
	printf ("RsaSig ");
	printf ("Dss ");
	printf ("Fortezza ");
	printf ("MsExchange ");
	printf ("Ssl ");
	printf ("\n                   ");
	printf ("   RsaSchannel ");
	printf ("DssDh ");
	printf ("EcEcdsaSig ");
	printf ("EcEcnraSig ");
	printf ("EcEcdsaFull ");
	printf ("\n                   ");
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
	printf ("  -container name   specify container name\n");
        printf ("  -machine          use local machine key set\n");
	printf ("  -keytype type     public key type to be used for signing or exporting \n"
	        "                      (signature, exchange)\n");
	fprintf(stderr,
	        "  -help             print this help\n\n");
    }
    return ret;
}

/*!
 * \brief ������� ��������� ������������ ����������� �� ����������� � 'MY'.
 */
int InstallCarrierCert (char *szContainer, char *szProvider, 
    DWORD dwProvType, DWORD dwAquireContextFlags, DWORD dwKeySpec)
{
    HCRYPTPROV hProv = 0; /* ���������� ����������*/
    HCRYPTKEY hUserKey = 0; /* ���������� �����*/
    int ret = 0;
    DWORD dwOpenStore = CERT_SYSTEM_STORE_CURRENT_USER;
    LPWSTR lpwStoreName = L"My";

    if (dwAquireContextFlags&CRYPT_MACHINE_KEYSET) {
        dwOpenStore = CERT_SYSTEM_STORE_LOCAL_MACHINE;
    }
    /* �������� �������� ����������.*/
    if (!CryptAcquireContext (&hProv, szContainer, szProvider, dwProvType,
	dwAquireContextFlags)) {
	HandleErrorFL ("Error during CryptAcquireContext.\n");
	goto err;
    }
    if (dwKeySpec & AT_KEYEXCHANGE) {
	/* �������� HANDLE ���������� �����.*/
	if (!CryptGetUserKey (hProv, AT_KEYEXCHANGE, &hUserKey)) {
	    dwKeySpec &= ~AT_KEYEXCHANGE;
	    printf ("No exchange key found.\n");
	} else {
	    printf ("Exchange key found.\n");
	    if (!Install1CarrierCert (hProv, hUserKey, dwAquireContextFlags,
		dwOpenStore, lpwStoreName))
		goto err;
	    if (hUserKey) {
		CryptDestroyKey (hUserKey);
		hUserKey = 0;
	    }
	    printf ("Exchange key installed successfully.\n");
	}
    }
    if (dwKeySpec & AT_SIGNATURE) {
	/* �������� HANDLE ���������� �����.*/
	if (!CryptGetUserKey (hProv, AT_SIGNATURE, &hUserKey)) {
	    dwKeySpec &= ~AT_SIGNATURE;
	    printf ("No signature key found.\n");
	} else {
	    printf ("Signature key found.\n");
	    if (Install1CarrierCert (hProv, hUserKey, dwAquireContextFlags,
		dwOpenStore, lpwStoreName))
		goto err;
	    printf ("Signature key installed successfully.\n");
	}
    }
    if (!(dwKeySpec & (AT_SIGNATURE | AT_KEYEXCHANGE))) {
	HandleErrorFL ("No key found.\n");
	goto err;
    } 
    ret = 1;
err:
    if (hUserKey)
	CryptDestroyKey (hUserKey);
    if (hProv)
	CryptReleaseContext (hProv, 0);
    return ret;
}

/*!
 * \brief ������� ��������� ������ ������������ �����������
 * �� ����������� � 'MY'.
 */
int Install1CarrierCert (HCRYPTPROV hProv, HCRYPTKEY hUserKey, 
    DWORD dwAquireContextFlags, DWORD dwOpenStore, LPWSTR lpwStoreName)
{
    int ret = 0;
    DWORD dwUserCertLength = 0; /* ������ �����������*/
    BYTE *pbUserCert = NULL; /* ����������.*/
    HANDLE hCertStore = 0; /* ���������� Store MY*/
    PCCERT_CONTEXT pUserCert = NULL; /* ���������� ���������������� */
	/* �����������*/
    CRYPT_KEY_PROV_INFO stProvInfo; /* ��������� �������� ������ ���������
     * ����� �� ��������� ����.*/
    DWORD szNameLength = 0; /* ������ �����.*/
    char *szName = NULL; /* ��� (�� UNICODE).*/
    DWORD dwAlgID; /* ALG_ID*/
    DWORD dwProvType; /* PROV_TYPE*/
    DWORD dwProvTypeLength = sizeof (dwProvType);

    memset(&stProvInfo, 0, sizeof (CRYPT_KEY_PROV_INFO));
    /* �������� ����������.*/
    if (!CryptGetKeyParam (hUserKey, KP_CERTIFICATE, NULL, 
	&dwUserCertLength, 0)) {
	HandleErrorFL ("Error during GetKeyParam.\n");
	goto err;
    }
    pbUserCert = malloc (dwUserCertLength);
    if (pbUserCert == NULL) {
	HandleErrorFL ("Error during malloc.\n");
	goto err;
    }
    if (!CryptGetKeyParam (hUserKey, KP_CERTIFICATE, pbUserCert, 
	&dwUserCertLength, 0)) {
	HandleErrorFL ("Error during GetKeyParam.\n");
	goto err;
    }
    /* ������������ ���������� */
    pUserCert = CertCreateCertificateContext (
	X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, pbUserCert, 
	dwUserCertLength);
    if (pUserCert == NULL) {
	HandleErrorFL ("Error during CertCreateCertificateContext.\n");
	goto err;
    }
    /* ��������� ��������� ������ �� ��������� ����.*/
    stProvInfo.dwFlags = 0;
    if (!CryptGetProvParam (hProv, PP_NAME, NULL, &szNameLength, 0)) {
	HandleErrorFL ("Error during CryptGetProvParam PP_NAME.\n");
	goto err;
    }
    szName = malloc (szNameLength);
    if (szName == NULL) {
	HandleErrorFL ("Error during malloc.\n");
	goto err;
    }
    if (!CryptGetProvParam (hProv, PP_NAME, (LPBYTE)szName, &szNameLength,
	0)) {
	HandleErrorFL ("Error during CryptGetProvParam PP_NAME.\n");
	goto err;
    }
    stProvInfo.pwszProvName = malloc (szNameLength * sizeof (wchar_t));
    MultiByteToWideChar(CP_ACP, 0, szName, -1, stProvInfo.pwszProvName, 
	szNameLength);
    free(szName);
    szName = NULL;

    szNameLength = 0;
    if (!CryptGetProvParam (hProv, PP_UNIQUE_CONTAINER, NULL, &szNameLength,
	0)) {
	HandleErrorFL ("Error during CryptGetProvParam "
	    "PP_UNIQUE_CONTAINER.\n");
	goto err;
    }
    szName = malloc (szNameLength);
    if (szName == NULL) {
	HandleErrorFL ("Error during malloc.\n");
	goto err;
    }
    if (!CryptGetProvParam (hProv, PP_UNIQUE_CONTAINER, (LPBYTE)szName, 
	&szNameLength, 0)) {
	HandleErrorFL ("Error during CryptGetProvParam"
	    "PP_UNIQUE_CONTAINER.\n");
	goto err;
    }
    stProvInfo.pwszContainerName = malloc (szNameLength * sizeof (wchar_t));
    MultiByteToWideChar(CP_ACP, 0, szName, -1, stProvInfo.pwszContainerName, 
	szNameLength);

    szNameLength = sizeof (DWORD);
    if (!CryptGetKeyParam (hUserKey, KP_ALGID, (BYTE*)&dwAlgID,
	&szNameLength, 0)) {
	HandleErrorFL ("Error during CryptGetKeyParam"
	    "KP_ALGID.\n");
	goto err;
    }
    if (!CryptGetProvParam (hProv, PP_PROVTYPE, (BYTE*)&dwProvType, 
	&dwProvTypeLength, 0)) {
	HandleErrorFL ("Error during CryptGetProvParam PP_PROVTYPE.\n");
	goto err;
    }
    stProvInfo.dwFlags = dwAquireContextFlags;
    stProvInfo.dwKeySpec = (GET_ALG_CLASS(dwAlgID) == ALG_CLASS_SIGNATURE
	? AT_SIGNATURE : AT_KEYEXCHANGE);
    stProvInfo.dwProvType = dwProvType;
    /* ���������� ������ �� ��������� ����.*/
    if (!CertSetCertificateContextProperty (pUserCert, 
	CERT_KEY_PROV_INFO_PROP_ID, 0, &stProvInfo)) {
	HandleErrorFL ("Error during CertSetCertificateContextProperty.\n");
	goto err;
    }
    /* ����������� ������� ����������� �� ��������*/
    printf ("\nShow user cert property\n");
    ShowCertPropery (pUserCert);

    /* ��������� ���������� 'MY'*/
    hCertStore = CertOpenStore (CERT_STORE_PROV_SYSTEM, 0, 0,
	CERT_STORE_OPEN_EXISTING_FLAG | dwOpenStore, lpwStoreName);
    if (!hCertStore) {
	DebugErrorFL("CertOpenStore");
	goto err;
    }

    /* ������� ���������� � ����������*/
    ret = CertAddCertificateContextToStore (hCertStore, pUserCert, 
	CERT_STORE_ADD_REPLACE_EXISTING, NULL);
    if (!ret) {
	printf ("Cannot add certificate to store\n");
	goto err;
    }
    ret = 1;
err:
    if (szName)
	free (szName);
    if (stProvInfo.pwszContainerName)
	free (stProvInfo.pwszContainerName);
    if (stProvInfo.pwszProvName)
	free (stProvInfo.pwszProvName);
    if (pUserCert)
    	CertFreeCertificateContext (pUserCert);
    if (hCertStore)
	CertCloseStore (hCertStore, CERT_CLOSE_STORE_FORCE_FLAG);
    if (pbUserCert)
	free (pbUserCert);
    return ret;
}

int
KPSetCertMain (int argc, char **argv)
{
    char *szContainer = NULL; /* ��� ��������� ����������*/
    char *szProvider = NULL; /* ��� ����������*/
    DWORD dwProvType = PROV_GOST_DH; /*��� ���������� �� ���������*/
    int dwKeyType = AT_KEYEXCHANGE | AT_SIGNATURE;
    DWORD dwAquireContextFlags = 0;
    char *certfile = NULL;
    char *store_type = NULL;
    char *store_name = NULL;
    int print_help = 0;
    int ret = 0;
    int c;

    /* ����������� ����� ������� ����������*/
    static struct option long_options[] = {
	{"keytype",		required_argument,	NULL, '1'},
	{"container",		required_argument,	NULL, 'c'},
	{"provider",		required_argument,	NULL, 'p'},
	{"provtype",		required_argument,	NULL, 't'},
	{"machine",		no_argument,    	NULL, 'm'},
        {"cert",                required_argument,	NULL, 'C'},
	{"store",	        required_argument,	NULL, 's'},
	{"storename",	        required_argument,	NULL, 'n'},
	{"help",		no_argument,		NULL, 'h'},
	{0, 0, 0, 0}
    };

    /* ������ ����������, ��� ������� ���������� ������������ ������
     * getopt.c*/
    while ((c = getopt_long_only (argc, argv, "", long_options, (int *)0))
	!= EOF) {
	switch (c) {
	case 'p':
	    szProvider = abbr2provider (optarg);
	    break;
	case 'v':
	    dwProvType = abbr2provtype (optarg);
	    break;
	case 'c':
	    szContainer = optarg;
	    break;
        case 'm':
            dwAquireContextFlags = CRYPT_MACHINE_KEYSET;
            break;
	case '1':
	    {
		char *ptr;
		ptr = optarg;
		if (strcmp(ptr, "signature") == 0)
		    dwKeyType = AT_SIGNATURE;
		else if (strcmp(ptr, "exchange") == 0)
		    dwKeyType = AT_KEYEXCHANGE;
		else if (strcmp(ptr, "none") == 0)
		    dwKeyType = 0;
		else {
		    print_help = 1;
		    goto bad;
		}
	    }
	    break;
	case 'C':
	    certfile = optarg;
	    break;
	case 's':
	    store_type = optarg;
	    break;
	case 'n':
	    store_name = optarg;
	    break;
	case 'h':
	case '?':
	    ret = 1;
	default:
	    print_help = 1;
	    goto bad;
	}
    }
    if (c != EOF) {
	print_help = 1;
	goto bad;
    }
    ret = KPSetCert(szContainer, szProvider, dwProvType,
	dwAquireContextFlags, dwKeyType, certfile, store_type, store_name);
bad:
    if (print_help) {
	fprintf(stderr,"%s -property -setcert [options]\n", prog);
	fprintf(stderr,"options:\n");
	printf ("  -provider name    specify provider name or next abbriviation:\n");
	printf ("                      cpDef\n");
	printf ("                      msDef msEnhanced ");
#ifdef MS_STRONG_PROV
	printf ("msStrong ");
#endif /* MS_STRONG_PROV */
	printf ("\n");
	printf ("                      msDefRsaSig msDefRsaSchannel ");
#ifdef MS_ENHANCED_RSA_SCHANNEL_PROV
	printf ("msEnhancedRsaSchannel ");
#endif /* MS_ENHANCED_RSA_SCHANNEL_PROV */
	printf ("msDefDss ");
	printf ("\n");
	printf ("                      msDefDssDh ");
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
	printf ("  -provtype type    specify provider type or next abbriviation:\n");
	printf ("                      CProCSP\n");
	printf ("                      RsaFull RsaSig Dss Fortezza MsExchange Ssl\n");
	printf ("                      RsaSchannel DssDh EcEcdsaSig EcEcnraSig EcEcdsaFull \n");
	printf ("                      EcEcnraFull ");
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
	printf ("  -container name   specify container name\n");
        printf ("  -machine          use local machine key set\n");
	printf ("  -keytype type     public key type to be used for signing or exporting \n"
	        "                      (signature, exchange)\n");
	printf ("  -cert name        certificate, which set into container\n");
	printf ("  -store type       store for search certificate (user, machine, service,\n"
	        "                      user_group_policy, machine_enterprise,\n"
	        "                      machine_group_policy, services, users)\n");
	printf ("  -storename arg    store name\n");
	fprintf(stderr,
	        "  -help             print this help\n\n");
    }
    return ret;
}

/* ������� ��������� ����������� � ��������� �����              */
int
KPSetCert (char *szContainer, char *szProvider, 
    DWORD dwProvType, DWORD dwAquireContextFlags, DWORD dwKeySpec,
    const char *certfile, const char *store_type, const char *store_name)
{
    LPWSTR lpwStore_name = NULL;
    DWORD dwStoreLoc = CERT_SYSTEM_STORE_CURRENT_USER;
    HANDLE hCertStore = 0;
    PCCERT_CONTEXT pCert = NULL;

    HMODULE         crypt32;
    CPCryptAcquireCertificatePrivateKey AcquireCertificatePrivateKey;

    BOOL fFound = FALSE;
    DWORD dwFoundKeySpec = 0;
    BOOL fCallerFreeProv = TRUE;
    HCRYPTPROV hProv = 0; /* ���������� ����������*/
    HCRYPTKEY hUserKey = 0; /* ���������� �����*/
    DWORD cbData = 0;
    int ret = 0;

    USES_CONVERSION;
    _lpw;

    if (store_type == NULL ||
        !strcmp(store_type, "user")) {
        dwStoreLoc = CERT_SYSTEM_STORE_CURRENT_USER;
    } else if (!strcmp(store_type, "service")) {
        dwStoreLoc = CERT_SYSTEM_STORE_CURRENT_SERVICE;
    } else if (!strcmp(store_type, "user_group_policy")) {
        dwStoreLoc = CERT_SYSTEM_STORE_CURRENT_USER_GROUP_POLICY;
    } else if (!strcmp(store_type, "machine")) {
        dwStoreLoc = CERT_SYSTEM_STORE_LOCAL_MACHINE;
    } else if (!strcmp(store_type, "machine_enterprise")) {
        dwStoreLoc = CERT_SYSTEM_STORE_LOCAL_MACHINE_ENTERPRISE;
    } else if (!strcmp(store_type, "machine_group_policy")) {
        dwStoreLoc = CERT_SYSTEM_STORE_LOCAL_MACHINE_GROUP_POLICY;
    } else if (!strcmp(store_type, "services")) {
        dwStoreLoc = CERT_SYSTEM_STORE_SERVICES;
    } else if (!strcmp(store_type, "users")) {
        dwStoreLoc = CERT_SYSTEM_STORE_USERS;
    } else {
        DebugErrorFL("Bad store type");
	return 0;
    }
    if (store_name == NULL) {
        lpwStore_name = L"My";
    } else {
        lpwStore_name = A2W(store_name);
    }
    
    hCertStore = CertOpenStore (CERT_STORE_PROV_SYSTEM, 0, 0,
	CERT_STORE_OPEN_EXISTING_FLAG | CERT_STORE_READONLY_FLAG | dwStoreLoc,
	lpwStore_name);
    if (!hCertStore) {
	DebugErrorFL("CertOpenStore");
	return 0;
    }
    /* ������ ���������� � ��������������� ���������*/
    pCert = CertFindCertificateInStore (hCertStore, TYPE_DER, 0, 
	CERT_FIND_SUBJECT_STR, A2W(certfile), NULL);
    if (!pCert) {
	DebugErrorFL("CertFindCertificateInStore");
        return 0;
    }
    /* ��������� ����������� ������� �������
     * CryptAcquireCertificatePrivateKey*/
    crypt32 = GetModuleHandle ("crypt32.dll");
    if (crypt32) {
	AcquireCertificatePrivateKey = (CPCryptAcquireCertificatePrivateKey) 
	    GetProcAddress (crypt32,"CryptAcquireCertificatePrivateKey");
	if (AcquireCertificatePrivateKey) {
	    fFound = AcquireCertificatePrivateKey(pCert, dwAquireContextFlags,
		NULL, &hProv, &dwFoundKeySpec, &fCallerFreeProv);
	} else {
	    printf ("CRYPT32.DLL not support "
		"CryptAcquireCertificatePrivateKey function\n");
	}
    }

    if (!fFound) {
	/* ���� ��������� �������� */
	cbData = sizeof (hProv);
	if (CertGetCertificateContextProperty (pCert,
	    CERT_KEY_PROV_HANDLE_PROP_ID, &hProv, &cbData)) {
	    cbData = sizeof (dwFoundKeySpec);
	    if (CertGetCertificateContextProperty (pCert,
		CERT_KEY_SPEC_PROP_ID, &dwFoundKeySpec, &cbData)) {
		fFound = TRUE;
	    } else if (hProv) {
		CryptReleaseContext (hProv, 0);
	    }
	}
    }
    if (!fFound) {
	/* ���� �� �����, �� ���������� �������� */
	if (!CryptAcquireContext (&hProv, szContainer, szProvider, dwProvType,
	    dwAquireContextFlags)) {
	    HandleErrorFL ("Error during CryptAcquireContext.\n");
	    goto err;
	}
	dwFoundKeySpec = dwKeySpec;
    }
    if (dwFoundKeySpec & AT_KEYEXCHANGE) {
	/* �������� HANDLE ���������� �����.*/
	if (!CryptGetUserKey (hProv, AT_KEYEXCHANGE, &hUserKey)) {
	    dwFoundKeySpec &= ~AT_KEYEXCHANGE;
	    printf ("No exchange key found.\n");
	} else {
	    printf ("Exchange key found.\n");
            if (!KPSet1Cert (hUserKey, pCert))
                goto err;
	    if (hUserKey) {
		CryptDestroyKey (hUserKey);
		hUserKey = 0;
	    }
	    printf ("Exchange key installed successfully.\n");
	}
    }
    if (dwFoundKeySpec & AT_SIGNATURE) {
	/* �������� HANDLE ���������� �����.*/
	if (!CryptGetUserKey (hProv, AT_SIGNATURE, &hUserKey)) {
	    dwFoundKeySpec &= ~AT_SIGNATURE;
	    printf ("No signature key found.\n");
	} else {
	    printf ("Signature key found.\n");
            if (!KPSet1Cert (hUserKey, pCert))
                goto err;
	    printf ("Signature key installed successfully.\n");
	}
    }
    if (!(dwFoundKeySpec & (AT_SIGNATURE | AT_KEYEXCHANGE))) {
	HandleErrorFL ("No key found.\n");
	goto err;
    }
    ret = 1;
err:
    if (hUserKey)
	CryptDestroyKey (hUserKey);
    if (hProv && fCallerFreeProv)
	CryptReleaseContext (hProv, 0);
    return ret;
}

/*!
 * \brief ������� ��������� ������ ������������ �����������
 * �� ����������� � 'MY'.
 */
int
KPSet1Cert (HCRYPTKEY hUserKey, PCCERT_CONTEXT pCert)
{
    printf ("KPSet1Cert: pCert->dwCertEncodingType =");
    if (pCert->dwCertEncodingType&X509_ASN_ENCODING)
        printf (" X509_ASN_ENCODING");
    if (pCert->dwCertEncodingType&PKCS_7_ASN_ENCODING)
        printf (" PKCS_7_ASN_ENCODING");
    if (pCert->dwCertEncodingType
	& (~(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING))) {
        printf (" 0x%x", pCert->dwCertEncodingType
	    & (~(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING)));
    }
    printf ("\n");
    /* ���������� ����������.*/
    if (!CryptSetKeyParam (hUserKey, KP_CERTIFICATE,
	pCert->pbCertEncoded, 0)) {
	HandleErrorFL ("Error during CryptSetKeyParam.\n");
	return 0;
    }
    return 1;
}

#if !defined(lint) && !defined(NO_RCSID) && !defined(OK_MODULE_VERSION)
static volatile const char rcsid[] = "\n$Id: property.c,v 1.24 2001/12/25 15:57:52 pre Exp $";
#endif

