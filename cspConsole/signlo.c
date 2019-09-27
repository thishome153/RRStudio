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
 * \file $RCSfile: signlo.c,v $
 * \version $Revision: 1.20.4.1 $
 * \date $Date: 2002/08/28 07:06:09 $
 * \author $Author: vasilij $
 *
 * \brief Пример создания и обработки подписанных сообщений PKCS#7
 * Signed с использованием функций низкого уровня
 * (Low Level Message Functions)
 *
 */

#include "tmain.h"

int do_low_sign (char *in_filename, char *out_filename, char *my_certfile, char *OID, int include, int detached, int Cert_LM);
int do_low_verify (char *in_filename, char *my_certfile, char *signature_filename, int detached, int Cert_LM);
int get_signing_time (HCRYPTMSG msg, int signerIndex);

/*--------------------------------------------------------------*/
/* MAIN*/
/*--------------------------------------------------------------*/

int main_sign (int argc, char **argv)
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
    int	 reboot = 0;
   
    /*-----------------------------------------------------------------------------*/
    /* определение опций разбора параметров*/
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
	{"reboot",	no_argument,		NULL, 'r'},
	{"help",	no_argument,		NULL, 'h'},
	{0, 0, 0, 0}
    };

    /*-----------------------------------------------------------------------------*/
    /* разбор параметров*/
    /* для разбора параметров используется модуль getopt.c*/
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

    if (!sign && !verify) {
	print_help = 1;
	goto bad;
    }

    if (sign) {
	ret = do_low_sign (in_filename, out_filename, my_certfile, OID, include, detached, Cert_LM);
    }
    else if (verify) {
	ret = do_low_verify (in_filename, my_certfile, signature_filename, detached, Cert_LM);
    }
    else {
	print_help = 1;
    }
    if( reboot )
	cpcsp_reboot( NULL );

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
/* Пример создания PKCS#7 Signed*/
/**/
/*--------------------------------------------------------------------*/
/*--------------------------------------------------------------------*/
int do_low_sign (char *infile, char *outfile, char *certfile, char *OID, int include, int detached, int Cert_LM)
{
    HCRYPTPROV	    hCryptProv = 0;	    /* Дескриптор провайдера*/
    PCCERT_CONTEXT  pUserCert = NULL;	    /* Сертификат, используемый для формирования ЭЦП*/

    DWORD	    keytype = 0;	    /* Тип ключа (возвращается)*/
    BOOL	    should_release_ctx = FALSE;	
    int		    ret = 0;
    BYTE	    *mem_tbs = NULL;	    /* Исходные данные*/
    size_t	    mem_len = 0;	    /* Длина данных*/
    
    HCRYPTMSG	    hMsg = 0;		    /* Дескриптор сообщения*/
    
    CRYPT_ALGORITHM_IDENTIFIER	HashAlgorithm;	/* Идентификатор алгоритма хэширования*/
    DWORD			HashAlgSize;	
    CMSG_SIGNER_ENCODE_INFO	SignerEncodeInfo;   /* Структура, описывающая отправителя*/
    CMSG_SIGNER_ENCODE_INFO	SignerEncodeInfoArray[1]; /* Массив структур, описывающих отправителя*/
    CERT_BLOB			SignerCertBlob;
    CERT_BLOB			SignerCertBlobArray[1];
    DWORD			cbEncodedBlob;
    BYTE			*pbEncodedBlob = NULL;
    CMSG_SIGNED_ENCODE_INFO	SignedMsgEncodeInfo;	/* Структура, описывающая подписанное сообщение*/
    DWORD			flags = 0;		
    
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
    
    if(Cert_LM) pUserCert = read_cert_from_MY(certfile);
    else pUserCert = read_cert_from_my(certfile);
    
    if (!pUserCert) {
	printf ("Cannot find User certificate: %s\n", certfile);
	goto err;
    }
    
    /*--------------------------------------------------------------------*/
    /* Программа по заданному сертификату определяет наличие секретного ключа*/
    /* и загружает требуемый провайдер.*/
    /* Для определения провайдера используется функция CryptAcquireCertificatePrivateKey, */
    /* если она присутствует в crypt32.dll. Иначе производистя поиск ключа по сертификату в справочнике.*/
    ret = CryptAcquireProvider ("my", pUserCert, &hCryptProv, &keytype, &should_release_ctx);
    if (ret) {
	printf("A CSP has been acquired. \n");
    }
    else {
	HandleErrorFL("Cryptographic context could not be acquired.");
    }

    /*--------------------------------------------------------------------*/
    /* Прочитаем файл, который будем подписывать.*/

    ret = get_file_data_pointer (infile, &mem_len, &mem_tbs);
    if (!ret) {
	fprintf (stderr, "Cannot read input file\n");
	goto err;
    }
    
   /*--------------------------------------------------------------------*/
    /* Инициализируем структуру алгоритма*/
    
    HashAlgSize = sizeof(HashAlgorithm);
    memset(&HashAlgorithm, 0, HashAlgSize);
    HashAlgorithm.pszObjId = OID;	    /* Идентификатор алгоритма хэша*/
    
    /*--------------------------------------------------------------------*/
    /* Инициализируем структуру CMSG_SIGNER_ENCODE_INFO*/
    
    memset(&SignerEncodeInfo, 0, sizeof(CMSG_SIGNER_ENCODE_INFO));
    SignerEncodeInfo.cbSize = sizeof(CMSG_SIGNER_ENCODE_INFO);
    SignerEncodeInfo.pCertInfo = pUserCert->pCertInfo;
    SignerEncodeInfo.hCryptProv = hCryptProv;
    SignerEncodeInfo.dwKeySpec = keytype;
    SignerEncodeInfo.HashAlgorithm = HashAlgorithm;
    SignerEncodeInfo.pvHashAuxInfo = NULL;
    
    /*--------------------------------------------------------------------*/
    /* Создадим массив отправителей. Сейчас только из одного.*/
    
    SignerEncodeInfoArray[0] = SignerEncodeInfo;
    
    /*--------------------------------------------------------------------*/
    /* Инициализируем структуру CMSG_SIGNED_ENCODE_INFO*/
    
    SignerCertBlob.cbData = pUserCert->cbCertEncoded;
    SignerCertBlob.pbData = pUserCert->pbCertEncoded;
    
    /*--------------------------------------------------------------------*/
    /* Инициализируем структуру массив структур CertBlob.*/
    
    SignerCertBlobArray[0] = SignerCertBlob;
    memset(&SignedMsgEncodeInfo, 0, sizeof(CMSG_SIGNED_ENCODE_INFO));
    SignedMsgEncodeInfo.cbSize = sizeof(CMSG_SIGNED_ENCODE_INFO);
    SignedMsgEncodeInfo.cSigners = 1;
    SignedMsgEncodeInfo.rgSigners = SignerEncodeInfoArray;
    SignedMsgEncodeInfo.cCertEncoded = include;
    /* Если задан флаг добавления сертификата отправителя*/
    if (include)
	SignedMsgEncodeInfo.rgCertEncoded = SignerCertBlobArray;
    else
	SignedMsgEncodeInfo.rgCertEncoded = NULL;
    
    SignedMsgEncodeInfo.rgCrlEncoded = NULL;
    if (detached)
	flags = CMSG_DETACHED_FLAG;
    
    /*--------------------------------------------------------------------*/
    /* Определим длину подписанного сообщения*/
    
    cbEncodedBlob = CryptMsgCalculateEncodedLength(
	TYPE_DER,		/* Message encoding type*/
	flags,                  /* Flags*/
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
    /* Резервируем память, требуемой длины*/
    
    pbEncodedBlob = (BYTE *) malloc(cbEncodedBlob);
    if (!pbEncodedBlob)
	HandleErrorFL("Memory allocation failed");
    /*--------------------------------------------------------------------*/
    /* Создадим дескриптор сообщения*/
    hMsg = CryptMsgOpenToEncode(
	TYPE_DER,		/* Encoding type*/
	flags,                  /* Flags (CMSG_DETACHED_FLAG )*/
	CMSG_SIGNED,            /* Message type*/
	&SignedMsgEncodeInfo,   /* Pointer to structure*/
	NULL,                   /* Inner content object ID*/
	NULL);                  /* Stream information (not used)*/
    if(hMsg) {
	printf("The message to be encoded has been opened. \n");
    }
    else
    {
	HandleErrorFL("OpenToEncode failed");
    }
    /*--------------------------------------------------------------------*/
    /* Поместим в сообщение подписываемые данные*/
    
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
    /* Вернем подписанное сообщение или только значение ЭЦП, если установлен признак detached*/
    
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
    /* Если задано, запишем подписанные данные в файл*/
    if (outfile) {
	if (write_file (outfile, cbEncodedBlob, pbEncodedBlob))
	    printf ("Output file (%s) has been saved\n", outfile);
    }
    /*--------------------------------------------------------------------*/
    /* Очистка памяти*/
err:
    release_file_data_pointer (mem_tbs);
    if(pbEncodedBlob)
	free(pbEncodedBlob);
    if(hMsg)
	CryptMsgClose(hMsg);
    if(hCryptProv) 
	CryptReleaseContext(hCryptProv,0);
  
    return 1;
} /*  End of main*/


/*--------------------------------------------------------------------*/
/*--------------------------------------------------------------------*/
/**/
/* Пример проверки PKCS#7 Signed*/
/**/
/*--------------------------------------------------------------------*/
/*--------------------------------------------------------------------*/
int do_low_verify (char *infile, char *certfile, char *signature_filename, int detached, int Cert_LM)
{
    HCRYPTPROV		hCryptProv = 0;		/* Дескриптор провайдера*/
    PCCERT_CONTEXT	pUserCert = NULL;	/* Сертификат, используемый для проверки ЭЦП*/

    DWORD		keytype = 0;		/* Возвращаемый тип ключа*/
    BOOL		should_release_ctx = FALSE;
    int			ret = 0;
    BYTE		*mem_tbs = NULL;
    size_t		mem_len = 0;
    BYTE		*mem_signature = NULL;
    size_t		signature_len = 0;
    
    HCRYPTMSG		hMsg = 0;		/* Дескриптор сообщения*/
    
    DWORD		cbDecoded;
    BYTE		*pbDecoded = NULL;
    DWORD		cbSignerCertInfo = 0;
    PCERT_INFO		pSignerCertInfo = NULL;
    PCCERT_CONTEXT	pSignerCertContext = NULL;
    PCERT_INFO		pSignerCertificateInfo = NULL;
    HCERTSTORE		hStoreHandle = NULL;
    DWORD		flags = 0;	/* CMSG_DETACHED_FLAG */
    /*--------------------------------------------------------------------*/
    /*  Используем сертификат из файла для инициализации контекста*/

    if (! infile) {
	fprintf (stderr, "No input file was specified\n");
	goto err;
    }
   
    if (certfile)
    {
        if(Cert_LM) pUserCert = read_cert_from_MY(certfile);
	else pUserCert = read_cert_from_my(certfile);

	if (!pUserCert) {
	    printf ("Cannot find User certificate: %s\n", certfile);
	    goto err;
	}
	/* Программа по заданному сертификату определяет наличие секретного ключа*/
	/* и загружает требуемый провайдер.*/
	/* Для определения провайдера используется функция CryptAcquireCertificatePrivateKey, */
	/* если она присутствует в crypt32.dll. Иначе производистя поиск ключа по сертификату в справочнике.*/
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

    /*--------------------------------------------------------------------*/
    /* Прочитаем файл, который будем проверять.*/
    ret = get_file_data_pointer (infile, &mem_len, &mem_tbs);
    if (!ret) {
	fprintf (stderr, "Cannot read input file\n");
	goto err;
    }
    
    /*--------------------------------------------------------------------*/
    /* Прочитаем файл подписи*/
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
    /* Откроем сообщение для декодирования*/
    
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
    /* Если установлен флаг detached, добавим подпись*/
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
    /* Поместим в сообщение проверяемые данные*/
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
    /* Определим длину подписанных данных*/
    
    ret = CryptMsgGetParam(
	hMsg,                  /* Handle to the message*/
	CMSG_CONTENT_PARAM,    /* Parameter type*/
	0,                     /* Signed Index*/
	NULL,                  /* Address for returned info*/
	&cbDecoded);           /* Size of the returned info*/
    if (ret)
	printf("The message parameter (CMSG_CONTENT_PARAM) has been acquired. Message size: %d\n", cbDecoded);
    else
	HandleErrorFL("Decode CMSG_CONTENT_PARAM failed");
    /*--------------------------------------------------------------------*/
    /* Резервируем память*/
    
    pbDecoded = (BYTE *) malloc(cbDecoded);
    if (!pbDecoded)
	HandleErrorFL("Decode memory allocation failed");
    /*--------------------------------------------------------------------*/
    /* Вернем подписанные данные*/
    
    ret = CryptMsgGetParam(
	hMsg,                 /* Handle to the message*/
	CMSG_CONTENT_PARAM,   /* Parameter type*/
	0,                    /* Signer Index*/
	pbDecoded,            /* Address for returned info*/
	&cbDecoded);          /* Size of the returned info*/
    if (ret)
	printf("The message param (CMSG_CONTENT_PARAM) returned. Length is %lu.\n",cbDecoded);
    else
	HandleErrorFL("Decode CMSG_CONTENT_PARAM #2 failed");
    /*--------------------------------------------------------------------*/
    /* Проверка ЭЦП*/
    /* Сначала определим информация CERT_INFO об отправителе.*/
    
    /*--------------------------------------------------------------------*/
    /* Определеим требуемый размер для структуры*/
    /* попробуем определим сертификат из сообщения*/
 
    if (! pUserCert) { 
	ret = CryptMsgGetParam(
	    hMsg,                         /* Handle to the message*/
	    CMSG_SIGNER_CERT_INFO_PARAM,  /* Parameter type*/
	    0,                            /* Signer Index*/
	    NULL,                         /* Address for returned info*/
	    &cbSignerCertInfo);           /* Size of the returned info*/
	if (ret)
	    printf("Try to get user cert. OK. Length %d.\n",cbSignerCertInfo);
	else {
	    printf("No user certificate found in message.\n");
	}
    }

    /*--------------------------------------------------------------------*/
    /* Если сертификат задан параметром вызова функции, */
    /* создадим справочник в памяти с этим сертификатом.*/
    /* Это сделано только для того, чтобы затем вернуть сертификат функцией */
    /* CertGetSubjectCertificateFromStore, которая также используется, если*/
    /* сертификат отправителя находится в самом сообщении.*/
    if (pUserCert) {
	hStoreHandle = CertOpenStore(CERT_STORE_PROV_MEMORY, TYPE_DER, 0, CERT_STORE_CREATE_NEW_FLAG,NULL);
	if (!hStoreHandle)
	    HandleErrorFL("Cannot create temporary store in memory.");
	/* Добавим сертификат в справочник*/
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
    /* Если сертификат не задан, зарезервируем память*/
    
    if (!pUserCert) {
	pSignerCertInfo = (PCERT_INFO) malloc(cbSignerCertInfo);
	if (!pSignerCertInfo)
	    HandleErrorFL("Verify memory allocation failed");
    }
    
    /*--------------------------------------------------------------------*/
    /* Попробуем извлечь CERT_INFO из сообщения (если сертификат не задан).*/
    
    if (! pUserCert) {
	ret = CryptMsgGetParam(
	    hMsg,                         /* Handle to the message*/
	    CMSG_SIGNER_CERT_INFO_PARAM,  /* Parameter type*/
	    0,                            /* Signer Index*/
	    pSignerCertInfo,              /* Address for returned info*/
	    &cbSignerCertInfo);           /* Size of the returned info*/
	    if (ret) 
		printf("The signer info has been returned. \n");
	    else
		HandleErrorFL("Verify SIGNER_CERT_INFO #2 failed");
    }
    /*--------------------------------------------------------------------*/
    /* Если сертификат не задан и не содан временный справочник в памяти*/
    /* создадим справочник, используя сообщение (CERT_STORE_PROV_MSG),*/
    /* который инициализируем сертификатом из сообщения.*/
    
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
    /* Найдем сертификат отправителя в справочнике*/
    
    pSignerCertContext = CertGetSubjectCertificateFromStore(
	hStoreHandle,       /* Handle to store*/
	TYPE_DER,	    /* Encoding type*/
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
    /* Используя структуру CERT_INFO проверяем ЭЦП сообщения*/
    
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

    
/*--------------------------------------------------------------------*/
/* Попробуем извлечь время формирования ЭЦП (для первой подписи) из сообщения.*/
/* Аналигично можно сделать для всех подписей сообщения.*/

    ret = get_signing_time (hMsg, 0);
    
    if(hStoreHandle)
	CertCloseStore(hStoreHandle, CERT_CLOSE_STORE_FORCE_FLAG);
   
err:
    if(hMsg)
	CryptMsgClose(hMsg);
    if(hCryptProv && should_release_ctx) 
	CryptReleaseContext(hCryptProv,0);

    release_file_data_pointer (mem_tbs);
    release_file_data_pointer (mem_signature);
    return ret;

}


/*--------------------------------------------------------------------*/
/* Определение времени формирования ЭЦП из сообщения*/

int get_signing_time (HCRYPTMSG hMsg, int signerIndex)
{
    DWORD		ret = 0;
    PCRYPT_ATTRIBUTES	authAttr = NULL;
    DWORD		authAttr_len = 0;
    DWORD		i = 0;
    PCRYPT_ATTR_BLOB	timeBlob = NULL;
    DWORD		size = 0;
    FILETIME		fileTime;
    SYSTEMTIME		systemTime;
    

    if (! hMsg)
	return ret;

    ret = CryptMsgGetParam(
	hMsg,				/* Handle to the message*/
	CMSG_SIGNER_AUTH_ATTR_PARAM,    /* Parameter type*/
	signerIndex,                    /* Signer Index*/
	NULL,				/* Address for returned info*/
	&authAttr_len);			/* Size of the returned info*/
    if (ret) {
	printf("The attribute (CMSG_SIGNER_AUTH_ATTR_PARAM) has been acquired. Attribute size: %d\n", authAttr_len);
    
    }
    else 
    {
	if (GetLastError() == CRYPT_E_ATTRIBUTES_MISSING) {
	    printf("The attribute CMSG_SIGNER_AUTH_ATTR_PARAM is not included into message.\n");
	    ret = 1;
	    return ret;
	}
	else
	    HandleErrorFL("Decode CMSG_SIGNER_AUTH_ATTR_PARAM failed");
    }
    /*--------------------------------------------------------------------*/
    /* Резервируем память*/
    
    if (authAttr_len) {
	authAttr= (PCRYPT_ATTRIBUTES) malloc(authAttr_len);
	if (!authAttr)
	    HandleErrorFL("Decode memory allocation failed");
	/*--------------------------------------------------------------------*/
	/* Вернем атрибут*/
	
	ret = CryptMsgGetParam(
	    hMsg,			    /* Handle to the message*/
	    CMSG_SIGNER_AUTH_ATTR_PARAM,    /* Parameter type*/
	    signerIndex,		    /* Signer Index*/
	    authAttr,			    /* Address for returned info*/
	    &authAttr_len);		    /* Size of the returned info*/
	if (ret) 
	    printf("The attribute (CMSG_SIGNER_AUTH_ATTR_PARAM) returned. Length is %lu.\n", authAttr_len);
	else
	    HandleErrorFL("Decode CMSG_SIGNER_AUTH_ATTR_PARAM #2 failed");
    }
    
    for (i = 0; i < authAttr->cAttr; i++)
    {
    /* 1.2.840.113549.1.9.3 - content type attribute "pkcs9 contentType"*/
    /* 1.2.840.113549.1.9.5 - signing time attribute "pkcs9 signingTime"*/
    /* 1.2.840.113549.1.9.4 - message digest attribute "pkcs9 messageDigest"*/
	if (strcmp (authAttr->rgAttr[i].pszObjId, szOID_RSA_signingTime) == 0 &&
	    authAttr->rgAttr[i].cValue )
	{
	    timeBlob =(authAttr->rgAttr[i]).rgValue;

	    size = sizeof (FILETIME);
	    ret = CryptDecodeObject(TYPE_DER, 
		szOID_RSA_signingTime,    
		timeBlob->pbData,    
		timeBlob->cbData,          
		0,            /* no Flags*/
		(DWORD*) &fileTime,
		&size);
	    if (! ret)
		return ret;

	    if ( ! FileTimeToSystemTime(&fileTime, &systemTime))
		return 0;

	    printf ("Signing time is: %d:%d.%d %02d-%02d-%d\n",
		systemTime.wHour, systemTime.wMinute, systemTime.wSecond,
		systemTime.wDay, systemTime.wMonth, systemTime.wYear);
	}

    }
    
    return ret;

}


#if !defined(lint) && !defined(NO_RCSID) && !defined(OK_MODULE_VERSION)
static volatile const char rcsid[] = "\n$Id: signlo.c,v 1.20.4.1 2002/08/28 07:06:09 vasilij Exp $";
#endif
