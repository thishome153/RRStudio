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
 * \file $RCSfile: cryptsf.c,v $
 * \version $Revision: 1.22.4.2 $
 * \date $Date: 2002/08/28 08:11:08 $
 * \author $Author: vasilij $
 *
 * \brief Пример создания и обработка сообщения PKCS#7 Enveloped
 * с использованием функций высокого уровня
 * (Simplified Message Functions)
 *
 */

#include "tmain.h"

#define MAX_ADD_SENDERS 64
    int recipient_Cert_LM[MAX_ADD_SENDERS]; //флажки для указания места поиска суртификатов получателей
					    // (0-CURRENT_USER, 1-LOCAL_MACHINE store)

static int do_encrypt (char *in_filename, char *out_filename,
    char *my_certfile, char **recipient_certfile, int recipient_cnt,
    char *OID, int Cert_LM);
static int do_decrypt (char *in_filename, char *out_filename,
    char *my_certfile, int Cert_LM);


/*--------------------------------------------------------------*/
/* MAIN*/
/*--------------------------------------------------------------*/

int main_encrypt_sf (int argc, char **argv) 
{

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
    char *ptr_alg = NULL;
   
    /*-----------------------------------------------------------------------------*/
    /* определение опций разбора параметров*/
    static struct option long_options[] = {
	{"in",		required_argument,	NULL, 'i'},
	{"out",		required_argument,	NULL, 'o'},
	{"my",		required_argument,	NULL, 'm'},
	{"MY",		required_argument,	NULL, 'M'},
	{"cert",	required_argument,	NULL, 'c'},
	{"CERT",	required_argument,	NULL, 'C'},
	{"encrypt",	no_argument,		NULL, 'e'},
	{"decrypt",	no_argument,		NULL, 'd'},
	{"alg",		required_argument,	NULL, 'a'},
	{"help",	no_argument,		NULL, 'h'},
	{0, 0, 0, 0}
    };

    /*-----------------------------------------------------------------------------*/
    /* разбор параметров*/
    /* для разбора параметров используется модуль getopt.c*/
    while ((c = getopt_long_only (argc, argv, "", long_options, (int *)0)) != EOF) {
	switch (c) {
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
	ret = do_encrypt (in_filename, out_filename, my_certfile,
	    recipient_certfile, recipient_cnt, OID, Cert_LM);
    }
    else if (decrypt) {
	ret = do_decrypt (in_filename, out_filename, my_certfile, Cert_LM);
    }
    else {
	print_help = 1;
    }

bad:
    if (print_help) {
	fprintf(stderr,"%s -sfenc [options]\n", prog);
	fprintf(stderr,SoftName " generate PKCS#7 Enveloped message \nusing CAPI simplified message function type\n");
	fprintf(stderr,"options:\n");
	fprintf(stderr,"  -in arg        input filename to be encrypted or decrypted\n");
	fprintf(stderr,"  -out arg       output PKCS#7 filename\n");
	fprintf(stderr,"  -my name       use my certificate with commonName = name from CURRENT_USER  store 'MY' to sign/verify data\n");
	fprintf(stderr,"  -MY name       use my certificate with commonName = name from LOCAL_MACHINE store 'MY' to sign/verify data\n");
	fprintf(stderr,"                 if no certificate, default provider will be used for encryption\n");
	fprintf(stderr,"  -cert          (muliply) recipient certificate from CURRENT_USER  store 'MY'\n");
	fprintf(stderr,"  -CERT          (muliply) recipient certificate from LOCAL_MACHINE store 'MY'\n");
	fprintf(stderr,"  -encrypt       encrypt input file\n");
	fprintf(stderr,"  -decrypt       decrypt enveloped file, specified by input filename.\n");
	fprintf(stderr,"                 Default context always used with decryption.\n");
	fprintf(stderr,"  -alg           encryption alghorithm to be used. Default: GOST\n"); 
	fprintf(stderr,"                 additional alg: RC2, RC4, DES, 3DES\n"); 
	fprintf(stderr,"  -help          print this help\n\n");
    }

    return ret;
}
    
/*----------------------------------------------------------------------*/
/* Пример создания зашифрованого сообщения*/
/*----------------------------------------------------------------------*/
int do_encrypt (char *in_filename, char *out_filename, char *my_certfile, char **recipient_certfile, int recipient_cnt, char *OID, int Cert_LM)
{
    DWORD EncryptAlgSize;

    CRYPT_ALGORITHM_IDENTIFIER EncryptAlgorithm;
    CRYPT_ENCRYPT_MESSAGE_PARA EncryptParams;

    HCRYPTPROV hCryptProv = 0;          /* Дескриптор провайдера*/
    PCCERT_CONTEXT pUserCert = NULL;	/* Ссертификат отправителя*/
    BYTE *tbenc = NULL;			/* Данные для шифрования*/
    size_t tbenc_len = 0;		/* Длина*/

    DWORD EncryptParamsSize;
    BYTE*    pbEncryptedBlob = NULL;	/* Зашифрованные данные*/
    DWORD    cbEncryptedBlob = 0;	/* Длина зашифрованных данных*/
    
    BOOL  should_release_ctx = 0;		/* if FALSE DO NOT Release CTX*/
    
    PCCERT_CONTEXT pRecipientCerts[MAX_ADD_SENDERS];	/* Сертификаты получателей*/

    int	    ret = 0;			/* статус возврата*/
    DWORD   keytype = 0;
    int	    i;
    PublicTime *s;
    PublicTime *e;
  
    /*--------------------------------------------------------------------*/
    /*  читаем файл для шифрования*/

    s = MTimeGet(NULL);
    ret = get_file_data_pointer (in_filename, &tbenc_len, &tbenc);
    if (! ret)
	HandleErrorFL("Cannot read input file.");
    e = MTimeGet(s);
    printf("Read time: "); MTimePrint(e);
    printf("Read perf: "); MTimePerfPrint(e, tbenc_len, 1024, "Kb");

    /*--------------------------------------------------------------------*/
    /*  инициализируем контекст, использую свой сертификат*/

    if (my_certfile)
    {
	if(Cert_LM)
	    pUserCert = read_cert_from_MY(my_certfile);
	else
	    pUserCert = read_cert_from_my(my_certfile);
	if (!pUserCert) {
	    printf ("Cannot find User certificate: %s\n", my_certfile);
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
    /*  Читаем сертификаты получателей*/

    for (i = 0; i < recipient_cnt; i++) {
	PCCERT_CONTEXT tmp;
	if(recipient_Cert_LM[i])
	    tmp = read_cert_from_MY ((char*) recipient_certfile[i]);
	else
	    tmp = read_cert_from_my ((char*) recipient_certfile[i]);
	if (!tmp)
	    HandleErrorFL("Cannot read recipient certfile.");
	
	pRecipientCerts[i] = tmp;
    }
    
    /*--------------------------------------------------------------------*/
    /* Инициализируем структуру описания алгоритма */
    
    EncryptAlgSize = sizeof(CRYPT_ALGORITHM_IDENTIFIER);
    memset(&EncryptAlgorithm, 0, EncryptAlgSize);
    
    /*--------------------------------------------------------------------*/
    /* Устанавливаем алгоритм шифрования данных*/
    
    EncryptAlgorithm.pszObjId = OID;  
    
    /*--------------------------------------------------------------------*/
    /* Инициализируем параметры структуры CRYPT_ENCRYPT_MESSAGE_PARA */
    
    EncryptParamsSize = sizeof(EncryptParams);
    memset(&EncryptParams, 0, EncryptParamsSize);
    EncryptParams.cbSize =  EncryptParamsSize;

    EncryptParams.dwMsgEncodingType = TYPE_DER;
    
    EncryptParams.hCryptProv = hCryptProv;
    EncryptParams.ContentEncryptionAlgorithm = EncryptAlgorithm;
    
    /*--------------------------------------------------------------------*/
    /* Вызов CryptEncryptMessage для определения длины шифрованных данных*/
    
    if(CryptEncryptMessage(
	&EncryptParams,
	recipient_cnt,
	pRecipientCerts,
	tbenc,
	tbenc_len,
	NULL,
	&cbEncryptedBlob))
    {
	printf("The encrypted message is %d bytes. \n",cbEncryptedBlob);
    }
    else
    {
	HandleErrorFL( "Getting EncrypBlob size failed.");
    }
    /*--------------------------------------------------------------------*/
    /* Резервируем память для шифрованных данных*/
    pbEncryptedBlob = (BYTE*)malloc(cbEncryptedBlob);
    if(pbEncryptedBlob)
    	printf("Memory has been allocated for the encrypted blob. \n");
    else
    	HandleErrorFL("Memory allocation error while encrypting.");
    
    /*--------------------------------------------------------------------*/
    /* Вызов CryptEncryptMessage для шифрования данных*/
    
    s = MTimeGet(NULL);
    ret = CryptEncryptMessage(
	&EncryptParams,
	recipient_cnt,
	pRecipientCerts,
	tbenc,
	tbenc_len,
	pbEncryptedBlob,
	&cbEncryptedBlob);

    if (ret) {
	printf ("File has been encrypted with alg: %s\n", OID);
    }
    else
	HandleErrorFL("Encryption failed.");

    e = MTimeGet(s);
    printf("Encrypt time: "); MTimePrint(e);
    printf("Encrypt perf: "); MTimePerfPrint(e, tbenc_len, 1024, "Kb");
    
    if (ret && out_filename) {
        s = MTimeGet(NULL);
	ret = write_file (out_filename, cbEncryptedBlob, pbEncryptedBlob);
        e = MTimeGet(s);
        printf("Write time: "); MTimePrint(e);
        printf("Write perf: "); MTimePerfPrint(e, tbenc_len, 1024, "Kb");
    }
    /*--------------------------------------------------------------------*/
    /* Очистка памяти*/
    
    CertFreeCertificateContext(pUserCert);

    for (i = 0; i < recipient_cnt; i++) {
	CertFreeCertificateContext(pRecipientCerts[i]);
    }
    if (should_release_ctx) {
	if(hCryptProv)
	{
	    CryptReleaseContext(hCryptProv,0);
	    printf("The CSP has been released. \n");
	}
    }
    
    release_file_data_pointer (tbenc);
    if (pbEncryptedBlob) free (pbEncryptedBlob);

err:
    return ret;
} 

/*--------------------------------------------------------------------*/
/*  Тест расшифрования сообщения*/
/*--------------------------------------------------------------------*/

int do_decrypt (char *in_filename, char *out_filename, char *my_certfile, int Cert_LM)
{

    PCCERT_CONTEXT pUserCert = NULL;	/* Сертификат получателя*/
    HCERTSTORE CertStoreArray[1];	/* Список справочников сертификатов*/

    CRYPT_DECRYPT_MESSAGE_PARA  DecryptParams;

    BYTE *tbdec = NULL;	/* Шифрованные данные*/
    size_t tbdec_len = 0;	/* Длина*/
    
    BYTE *pbDecryptedMessage = NULL;	/* Расшифрованные данные*/
    DWORD cbDecryptedMessage = 0;	/* Длина*/

    int	    ret = 0;
 
    HCERTSTORE mem = NULL;		/* Дескриптор временного справочника сертификатов в памяти*/

    /*--------------------------------------------------------------------*/
    /*  читаем файл для расшифрования*/

    ret = get_file_data_pointer (in_filename, &tbdec_len, &tbdec);
    if (! ret)
	HandleErrorFL("Cannot read input file.");

    /*--------------------------------------------------------------------*/
    /*  Читаем сертификат, который будет использоваться для расшифрования.*/

	if(Cert_LM)
	    pUserCert = read_cert_from_MY(my_certfile);
	else
	    pUserCert = read_cert_from_my(my_certfile);

    if (!pUserCert) {
	HandleErrorFL("Cannot find User certificate");
    }
    
    /*--------------------------------------------------------------------*/
    /*   Функция расшифровывает только с использованием справочника*/
    /*   Сделаем временный справочник в памяти и положим туда сертификат получателя*/

    mem = CertOpenStore(CERT_STORE_PROV_MEMORY, TYPE_DER, 0, CERT_STORE_CREATE_NEW_FLAG,NULL);
    if (!mem)
	HandleErrorFL("Cannot create temporary store in memory.");

    ret = CertAddCertificateContextToStore(mem, pUserCert, CERT_STORE_ADD_ALWAYS, NULL);
    if (!ret)
	HandleErrorFL("Cannot add certificate to store.");
    
    CertStoreArray[0] = mem;
    /*--------------------------------------------------------------------*/
    /*   Инициализация структуры CRYPT_DECRYPT_MESSAGE_PARA */
    
    memset(&DecryptParams, 0, sizeof(CRYPT_DECRYPT_MESSAGE_PARA));
    DecryptParams.cbSize = sizeof(CRYPT_DECRYPT_MESSAGE_PARA);

    DecryptParams.dwMsgAndCertEncodingType = TYPE_DER;

    DecryptParams.cCertStore = 1;
    DecryptParams.rghCertStore = CertStoreArray;

    /*--------------------------------------------------------------------*/
    /* Резервируем память для расшифрованных данных заранне.*/
    /* Это позволяет увеличить производительность.*/

    cbDecryptedMessage = tbdec_len;
    pbDecryptedMessage = (BYTE*)malloc(cbDecryptedMessage);

    if (NULL == pbDecryptedMessage)
	HandleErrorFL("Memory allocation error while decrypting");
    
    /*--------------------------------------------------------------------*/
    /*  Расшифровываем сообщение */
    
    ret = CryptDecryptMessage(
	&DecryptParams,
	tbdec,
	tbdec_len,
	pbDecryptedMessage,
	&cbDecryptedMessage,
	NULL);

    if (!ret)
    {
        /* Если выделили недостаточно памяти для расшифрованных данных*/
	/* резервируем их еще раз*/
	if(GetLastError() != ERROR_MORE_DATA )
		HandleErrorFL("Error decrypting message.");

	free (pbDecryptedMessage);
	pbDecryptedMessage = (BYTE*)malloc(cbDecryptedMessage);

	if (NULL == pbDecryptedMessage)
	    HandleErrorFL("Memory allocation error while decrypting");
	/*--------------------------------------------------------------------*/
	/* Повторный вызов функции, если произошла ошибка*/
	    
	ret = CryptDecryptMessage(
		&DecryptParams,
		tbdec,
		tbdec_len,
		pbDecryptedMessage,
		&cbDecryptedMessage,
		NULL);

	if (!ret)
	    HandleErrorFL("Error decrypting message.");
     }

    printf("Message Decrypted Successfully.\n");
    if (out_filename)
	ret = write_file (out_filename, cbDecryptedMessage, pbDecryptedMessage);
    
    /*--------------------------------------------------------------------*/
    /* Очистка памяти*/
    
    release_file_data_pointer (tbdec);
    if (pbDecryptedMessage) free (pbDecryptedMessage);

    if (mem)
	CertCloseStore (mem, 0);
    return ret;

}  



#if !defined(lint) && !defined(NO_RCSID) && !defined(OK_MODULE_VERSION)
static volatile const char rcsid[] = "\n$Id: cryptsf.c,v 1.22.4.2 2002/08/28 08:11:08 vasilij Exp $";
#endif
