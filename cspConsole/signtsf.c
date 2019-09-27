/*
 * 
 */

/*!
 * \file $RCSfile: signtsf.c,v $
 * \version $Revision: 1.32.4.1 $
 * \date $Date:  $
 * \author $Author:  $
 *
 * \brief Пример создания и обработки подписанных сообщений PKCS#7
 * Signed с использованием функций высокого уровня
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
    char    OID[64] = szOID_CP_GOST_R3411; //CRYPT_HASH_ALG_OID_ Алгоритм функции хэширования по ГОСТ Р 34.11-94 установлен по умоланию
    int    c;
    char    *ptr_hash_alg = NULL;
    int	    ask = 0;
    int     base64 = 0;
   
    /*-----------------------------------------------------------------------------*/
    /* определение опций разбора параметров*/
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
    /* разбор параметров*/
    /* для разбора параметров используется модуль getopt.c*/
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
/* Пример создания PKCS#7 Signed*/
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
    BYTE		*signed_mem = NULL;  // Буффер с подписью
    CRYPT_KEY_PROV_INFO *pCryptKeyProvInfo = NULL;
    DWORD		cbData = 0;
    
    /*--------------------------------------------------------------------*/
    /*  Переменные для определения системного времени и */
    /* записи его в формат подписанного сообщения*/

    CRYPT_ATTR_BLOB	cablob[1];
    CRYPT_ATTRIBUTE	ca[1];
    LPBYTE		pbAuth = NULL;
    DWORD		cbAuth = 0;
    FILETIME		fileTime;
    SYSTEMTIME		systemTime;
    
    /*--------------------------------------------------------------------*/
    /*  Используем сертификат из файла для инициализации контекста*/
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
    /* Для того чтобы функция CryptAcquireContext не загружала постоянно*/
    /* провайдер и ключ можно использовать флаг CERT_SET_KEY_CONTEXT_PROP_ID или*/
    /* CERT_SET_KEY_PROV_HANDLE_PROP_ID в значении флага структуры CRYPT_KEY_PROV_INFO.*/
    /**/
    /* Для этого определим наличие этого свойства и перезапишем флаг*/
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
	    /* Установим флаг кеширования провайдера*/
	    pCryptKeyProvInfo->dwFlags = CERT_SET_KEY_CONTEXT_PROP_ID;
	    /* Установим свойства в контексте сертификата*/
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
    /* Откроем файл который будем подписывать*/
    ret = get_file_data_pointer (infile, &mem_len, &mem_tbs);
    if (!ret) {
	fprintf (stderr, "Cannot open input file\n");
	goto err;
    }

    /*--------------------------------------------------------------------*/
    /* Установим параметры*/
    
    /* Обязательно ныжно обнулить все поля структуры. */
    /* Иначе это может привести к access violation в функциях CryptoAPI*/
    /* В примере из MSDN это отсутствует*/
    memset(&param, 0, sizeof(CRYPT_SIGN_MESSAGE_PARA));
    param.cbSize = sizeof(CRYPT_SIGN_MESSAGE_PARA);
	param.dwMsgEncodingType = TYPE_DER; //X509_ASN_ENCODING | PKCS_7_ASN_ENCODING; // TYPE_DER;
	param.pSigningCert =  pUserCert;// Выберем сертификат!! //pUserCert;
    
    param.HashAlgorithm.pszObjId = OID;
    param.HashAlgorithm.Parameters.cbData = 0;
    param.HashAlgorithm.Parameters.pbData = NULL;
    param.pvHashAuxInfo = NULL;	/* не используется*/
    param.cMsgCert = 0;		/* 0 -не вклачаем сертификат отправителя*/ /*If set to zero no certificates are included in the signed message*/
	param.rgpMsgCert =NULL;//pUserCert; // NULL;
    param.cAuthAttr = 0;
    param.dwInnerContentType = 0;
    param.cMsgCrl = 0;  // Cписки отзыва
    param.cUnauthAttr = 0;


   /*---------------------------------------------------------------------------------
    Определим системное время и добавим его в список аутентифицируемых (подписанных) 
    атрибутов PKCS#7 сообщения с идентификатором szOID_RSA_signingTime.
    ---------------------------------------------------------------------------------*/
    GetSystemTime(&systemTime);
    SystemTimeToFileTime(&systemTime, &fileTime);
    
    /* Определим требуемую длину для хранения времени*/
    ret = CryptEncodeObject(TYPE_DER,	szOID_RSA_signingTime,	(LPVOID)&fileTime, 
	                         NULL, 	&cbAuth);
    if (!ret)
	HandleErrorFL("Cannot encode object");
        
    pbAuth = (BYTE*) malloc (cbAuth);
    if (!pbAuth)
        HandleErrorFL("Memory allocation error");
    
    /* Кодирование времени в атрибут типа szOID_RSA_signingTime */
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

    /* Возможен вариант использования функции CryptSignMessage в двухпроходной схеме*/
    /* с передачей вместо исходных данных нуля для определения длины подписанных данных*/
    /* (см. раздел Возвращение данных неопределенной длины в Руководстве программиста).*/
    /* В этом случае функция CryptSignMessage производит инициализацию провайдера, соответствующего сертификату и */
    /* подпись данных для определения длины, что приводит к необходимости двойной загрузки ключа.*/
    /**/
    /* Для того чтобы этого избежать приложение может заранее зарезервировать определенное */
    /* количество памяти, достаточное для создания подписанного сообщения.*/
    
	/*--------------------------------------------------------------------*/
	/* Определение длины подписанного сообщения*/
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
	/* Формирование подписанного сообщения*/
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
	

	/* Запись в файл*/
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
/*  Пример проверки ЭЦП PKCS#7 Signed*/
/**/
/*--------------------------------------------------------------------*/
/*--------------------------------------------------------------------*/
static int do_verify (char *infile, char *certfile, char *outfile, char *signature_filename, int detached, int ask, int base64, int Cert_LM)
{
    HCRYPTPROV hCryptProv = 0;		    /* Дескриптор провайдера*/
    BOOL should_release_ctx = FALSE;

    PCCERT_CONTEXT pUserCert = NULL;	    /* Сертификат, используемый для проверки ЭЦП*/

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
    /*  Используем сертификат из файла для инициализации контекста*/

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
    /* Программа по заданному сертификату определяет наличие секретного ключа*/
    /* и загружает требуемый провайдер.*/
    /* Для определения провайдера используется функция CryptAcquireCertificatePrivateKey, */
    /* если она присутствует в crypt32.dll. Иначе производистя поиск ключа по сертификату в справочнике.*/
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
    /* Прочитаем файл, который будем проверять.*/
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
    /* Прочитаем файл подписи*/
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
    /* Установим параметры структуры CRYPT_VERIFY_MESSAGE_PARA */
  
    memset(&param, 0, sizeof(CRYPT_VERIFY_MESSAGE_PARA));
    param.cbSize = sizeof(CRYPT_VERIFY_MESSAGE_PARA);
    param.dwMsgAndCertEncodingType = TYPE_DER;

    param.hCryptProv = hCryptProv;  
    if(Cert_LM)
	param.pfnGetSignerCertificate = global_MY_get_cert;
    else
	param.pfnGetSignerCertificate = global_my_get_cert;    /* этот callback должен возвернуть сертификат на котором проверяем сообщение*/
    param.pvGetArg = (void*) certfile;		    /* передадим имя файла сертификата в функцию*/

    /*------------------------------------------------*/
    /* Различные ветки для проверки ЭЦП*/
    /* Для проверки detached используется функция CryptVerifyDetachedMessageSignature*/
    /* В противном случае используется CryptVerifyMessageSignature*/

    if (detached == 0) {
        DWORD dwSignerIndex = 0;    /* Используется вцикле если подпись не одна.*/
				    /* Пока только 0*/
	signed_mem = (BYTE*)malloc(signed_len = mem_len);
	if (!signed_mem) {
	    HandleErrorFL("Memory allocation error allocating decode blob.");
	}

	ret = CryptVerifyMessageSignature(
	    &param,
	    dwSignerIndex,
	    mem_tbs,		    /* подписанное сообщение*/
	    mem_len,		    /* длина*/
	    signed_mem,		    /* если нужно сохранить вложение BYTE *pbDecoded,*/
	    &signed_len,	    /* куда сохраняет вложение DWORD *pcbDecoded,*/
	    NULL);		    /* возвращаемый сертификат на котором проверена ЭЦП (PCCERT_CONTEXT *ppSignerCert)*/
	
	if (ret) {
	    printf("Signature was verified OK\n");
	}
	else
	{
	    HandleErrorFL("Signature was NOT verified\n");
	    goto err;
	}

	/* запись вложения в файл*/
	if (outfile && signed_mem && signed_len) {
	    if (write_file (outfile, signed_len, signed_mem))
		printf ("Output file (%s) has been saved\n", outfile);
	}
    } else { /* detached подпись*/
	
        DWORD dwSignerIndex = 0;    /* Используется вцикле если подпись не одна.*/
				    
        MessageArray[0] = mem_tbs;
	MessageSizeArray[0] = mem_len;

	/* Проверка ЭЦП*/
	ret = CryptVerifyDetachedMessageSignature(
	    &param, 
	    dwSignerIndex,
	    (const BYTE*) mem_signature,  /* detached signature*/
	    signature_len,	    /* ее длина*/
	    1,			    /* количество проверяемых исходных файлов*/
	    MessageArray,	    /* список исходных файлов*/
	    MessageSizeArray,	    /* список размеров исходных файлов*/
	    &pUserCert);	    /* возвращаемый сертификат на котором проверена ЭЦП (PCCERT_CONTEXT *ppSignerCert)*/
	    
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
/* Callback функция, используемая для определения сертификата проверки ЭЦП*/
/**/
/* Функция возвращает контекст сертификата, определяя сертификат из системного справочника 'MY'.*/
/* Поиск проводитсв по имени владельца сертификата, заданного параметром pvGetArg*/
/* Описание функции и параметры приведены в описании поля pfnGetSignerCertificate*/
/* структуры CRYPT_VERIFY_MESSAGE_PARA */

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
/* Callback функция, используемая для определения сертификата проверки ЭЦП*/
/**/
/* Функция возвращает контекст сертификата, определяя сертификат из системного справочника LM 'MY'.*/
/* Поиск проводитсв по имени владельца сертификата, заданного параметром pvGetArg*/
/* Описание функции и параметры приведены в описании поля pfnGetSignerCertificate*/
/* структуры CRYPT_VERIFY_MESSAGE_PARA */

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
