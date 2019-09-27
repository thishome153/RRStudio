/*
 * Copyright(C) 2000 Проект ИОК
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
 */

/*!
 * \file $RCSfile: WinCryptEx.h,v $
 * \version $Revision: 1.105.4.7 $
 * \date $Date: 2002/10/23 13:45:50 $
 * \author $Author: chudov $
 *
 * \brief Интерфейс Крипто-Про CSP, добавление к WinCrypt.h.
 */


#ifndef _WINCRYPTEX_H_INCLUDED
#define _WINCRYPTEX_H_INCLUDED


#define CP_DEF_PROV_A "Crypto-Pro Cryptographic Service Provider"
#define CP_DEF_PROV_W L"Crypto-Pro Cryptographic Service Provider"

#define CP_GR3410_94_PROV_A "Crypto-Pro GOST R 34.10-94 Cryptographic Service Provider"
#define CP_GR3410_94_PROV_W L"Crypto-Pro GOST R 34.10-94 Cryptographic Service Provider"

#define CP_GR3410_2001_PROV_A "Crypto-Pro GOST R 34.10-2001 Cryptographic Service Provider"
#define CP_GR3410_2001_PROV_W L"Crypto-Pro GOST R 34.10-2001 Cryptographic Service Provider"

#ifdef UNICODE 
#define CP_DEF_PROV CP_DEF_PROV_W 
#define CP_GR3410_94_PROV CP_GR3410_94_PROV_W
#define CP_GR3410_2001_PROV CP_GR3410_2001_PROV_W 
#else 
#define CP_DEF_PROV CP_DEF_PROV_A 
#define CP_GR3410_94_PROV CP_GR3410_94_PROV_A
#define CP_GR3410_2001_PROV CP_GR3410_2001_PROV_A
#endif 

/*
 * ???? Надо заставить PROV_GOST_DH вызывать предупреждение,
 * т.к. PROV_GOST_DH == 2 == PROV_RSA_SIG
 */
#define PROV_GOST_DH 2

/*
 * На 09.07.01 в Platform SDK последний зарегистрированный 
 * CSP - PROV_RSA_AES == 24
 *
 * Я выбрал для  PROV_GOST_* два случайных числа из диапазона [53..89]
 */
#define PROV_GOST_94_DH 71
#define PROV_GOST_2001_DH 75

/* Дополнительные флаги AcquireContext. Глобальные установки провайдера. */
#define CRYPT_GENERAL 0x4000

/* Описатели пользовательских ключей */
#define USERKEY_KEYEXCHANGE			AT_KEYEXCHANGE
#define USERKEY_SIGNATURE			AT_SIGNATURE
#define USERKEY_SIMMERYMASTERKEY		27
/* Algorithm types */
#define ALG_TYPE_GR3410				(7 << 9)
/* GR3411 sub-ids */
#define ALG_SID_GR3411				30
/* G28147 sub_ids */
#define ALG_SID_G28147				30
#define ALG_SID_PRODIVERS			38
#define ALG_SID_RIC1DIVERS			40
/* Export Key sub_id */
#define ALG_SID_PRO_EXP				31
#define ALG_SID_SIMPLE_EXP			32
/* Hash sub ids */
#define ALG_SID_GR3410				30
#define ALG_SID_G28147_MAC			31
#define ALG_SID_TLS1_MASTER_HASH		32

/* GOST_DH sub ids */
#define ALG_SID_DH_EX_SF			30
#define ALG_SID_DH_EX_EPHEM			31
#define ALG_SID_PRO_AGREEDKEY_DH		33
#define ALG_SID_PRO_SIMMETRYKEY			34
#define ALG_SID_GR3410EL			35
#define ALG_SID_DH_EL_SF			36
#define ALG_SID_DH_EL_EPHEM			37


#define CALG_GR3411 \
    (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_GR3411)

#define CALG_G28147_MAC \
    (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_G28147_MAC)

#define CALG_G28147_IMIT \
    CALG_G28147_MAC

#define CALG_GR3410 \
    (ALG_CLASS_SIGNATURE | ALG_TYPE_GR3410 | ALG_SID_GR3410)

#define CALG_GR3410EL \
    (ALG_CLASS_SIGNATURE | ALG_TYPE_GR3410 | ALG_SID_GR3410EL)

#define CALG_G28147 \
    (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_G28147)

#define CALG_DH_EX_SF \
    (ALG_CLASS_KEY_EXCHANGE | ALG_TYPE_DH | ALG_SID_DH_EX_SF)

#define CALG_DH_EX_EPHEM \
    (ALG_CLASS_KEY_EXCHANGE | ALG_TYPE_DH | ALG_SID_DH_EX_EPHEM)

#define CALG_DH_EX \
    CALG_DH_EX_SF

#define CALG_DH_EL_SF \
    (ALG_CLASS_KEY_EXCHANGE | ALG_TYPE_DH | ALG_SID_DH_EL_SF)

#define CALG_DH_EL_EPHEM \
    (ALG_CLASS_KEY_EXCHANGE | ALG_TYPE_DH | ALG_SID_DH_EL_EPHEM)

#define CALG_PRO_AGREEDKEY_DH \
    (ALG_CLASS_KEY_EXCHANGE | ALG_TYPE_BLOCK | ALG_SID_PRO_AGREEDKEY_DH)

#define CALG_PRO_EXPORT \
    (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_PRO_EXP)

#define CALG_SIMPLE_EXPORT \
    (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_SIMPLE_EXP)

#define CALG_SIMMETRYKEY \
    CALG_G28147
    /* (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_SIMMETRYKEY) */

#define CALG_TLS1_MASTER_HASH \
    (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_TLS1_MASTER_HASH)

#define CALG_TLS1_MAC_KEY \
    (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_SECURECHANNEL | ALG_SID_SCHANNEL_MAC_KEY)

#define CALG_TLS1_ENC_KEY \
    (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_SECURECHANNEL | ALG_SID_SCHANNEL_ENC_KEY)

#define CALG_PRO_DIVERS \
    (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_PRODIVERS)
#define CALG_RIC_DIVERS \
    (ALG_CLASS_DATA_ENCRYPT | ALG_TYPE_BLOCK | ALG_SID_RIC1DIVERS)
#define CALG_OSCAR_DIVERS CALG_RIC_DIVERS

#define CRYPT_ALG_PARAM_OID_GROUP_ID            20


#define CRYPT_PROMIX_MODE 0x00000001
#define CRYPT_SIMPLEMIX_MODE 0x00000000

/*Тип ключевого блоба для диверсификации ключей с помощью 
    функции CPImportKey в режиме ключа импорта CALG_PRO_EXPORT*/
#define DIVERSKEYBLOB	0x70

/* Дополнительные параметры криптопровайдера */
/* Дополнительные параметры объекта хеша */#define PP_LAST_ERROR 90
#define PP_ENUMOIDS_EX 91
#define PP_HASHOID 92
#define PP_CIPHEROID 93                                  
#define PP_SIGNATUREOID 94
#define PP_DHOID 95
#define PP_BIO_STATISTICA_LEN 97
#define PP_REBOOT 98
/*Следующий параметр используется для перехда на платформы, отличные от WIN32*/
#define PP_ANSILASTERROR 99
#define PP_RANDOM 100
#define PP_DRVCONTAINER	101
#define PP_MUTEX_ARG	102
#define PP_ENUM_HASHOID 103
#define PP_ENUM_CIPHEROID 104
#define PP_ENUM_SIGNATUREOID 105
#define PP_ENUM_DHOID	106
#define PP_ENCRYPTION_CONTAINER 107
#define PP_CHANGE_PIN 108
#define PP_HCRYPTPROV 109
#define PP_SELECT_CONTAINER 110
#define PP_FQCN 111
#define PP_CHECKPUBLIC 112
#define PP_ADMIN_CHECKPUBLIC 113
#define PP_VERSION_TIMESTAMP 114
/* Флаг используемые при перечислении контейнеров, для получения:
    Fully Qualified Container Name */
#define CRYPT_FQCN 0x10
/* Флаг, используемый при перечислении контейнеров, для приоритета
    получения уникальных имен контейнеров перед обычными именами. 
    В случае достаточно выделенной памяти под уникальный номер,
    после уникального номера копируется обычное имя контейнера. */
#define CRYPT_UNIQUE 0x08
/* Флаг используется для получение #build/#prebuild при получении 
   версии по PP_VERSION */
#define CRYPT_VERSION_BUILD  0x20
/* Флаг используется для получение информации о версиях 
   промежуточных модулей с интерфейсом CSP (cpcspr, cprmcspf и т.п.) */
#define CRYPT_VERSION_INFORMATION_LEVELS  0xF00

/* Дополнительные параметры объекта хеша */
#define HP_HASHSTARTVECT 0x0008
#define HP_HASHCOPYVAL	 0x0009
#define HP_OID 0x000a
#define HP_OPEN 0x000B

/* Дополнительные параметры ключа */
#define KP_SV KP_IV
#define KP_MIXMODE 101
#define KP_OID 102
#define KP_HASHOID 103
#define KP_CIPHEROID 104
#define KP_SIGNATUREOID 105
#define KP_DHOID 106

/* Общие коды ошибок
 * Определение кодов ошибок возвращаемые через
 * CryptGetProvParam(PP_LAST_ERROR) */
/* Коды ошибок ДСЧ */
#define GPE_FAIL_TESTBUFFER 0x0301
#define GPE_FAIL_STATBUFFER 0x0401
#define GPE_DIFERENT_PARAMETERS 0x0501
/* Коды ошибок функций ГОСТ 28147-89 */
#define GPE_CORRUPT_KEYCONTEXT 0x0102
#define	GPE_CHECKPROC_GAMMING_OFB 0x0402
#define	GPE_CHECKPROC_ENCRYPT_CFB 0x0502

/* Код ошибки контроля наличия носителя в считывателе при закрытии
 * контейнера */
#define GPE_CHECKCARRIER 0x0805
/* Коды ошибок функций подписи */
#define GPE_CORRUPT_KEYPAIR_INFO 0x0104
/* Код отказа системы тестирования */
#define GPE_CHECKPROC_TESTFAIL 0x0704
#define GPE_RANDOMFAIL 0x0604

/* CRYPT_HASH_ALG_OID_GROUP_ID */
#define szOID_CP_GOST_R3411 "1.2.643.2.2.9"

/* CRYPT_ENCRYPT_ALG_OID_GROUP_ID */
#define szOID_CP_GOST_28147 "1.2.643.2.2.21"

/* CRYPT_PUBKEY_ALG_OID_GROUP_ID */
#define szOID_CP_GOST_R3410 "1.2.643.2.2.20"
#define szOID_CP_GOST_R3410EL "1.2.643.2.2.19"
#define szOID_CP_DH_EX "1.2.643.2.2.99"
#define szOID_CP_DH_EL "1.2.643.2.2.98"

/* CRYPT_SIGN_ALG_OID_GROUP_ID */
#define szOID_CP_GOST_R3411_R3410 "1.2.643.2.2.4"
#define szOID_CP_GOST_R3411_R3410EL "1.2.643.2.2.3"

/* CRYPT_ENHKEY_USAGE_OID_GROUP_ID */
#define szOID_KP_TLS_PROXY "1.2.643.2.2.34.1"
#define szOID_KP_RA_CLIENT_AUTH "1.2.643.2.2.34.2"
#define szOID_KP_WEB_CONTENT_SIGNING "1.2.643.2.2.34.3"
#define szOID_KP_RA_ADMINISTRATOR "1.2.643.2.2.34.4"
#define szOID_KP_RA_OPERATOR "1.2.643.2.2.34.5"


/* OID for HASH */
#define OID_HashTest "1.2.643.2.2.30.0"
#define OID_HashVerbaO "1.2.643.2.2.30.1"	/* ГОСТ Р 34.11-94, параметры по умолчанию */
#define OID_HashVar_1 "1.2.643.2.2.30.2"
#define OID_HashVar_2 "1.2.643.2.2.30.3"
#define OID_HashVar_3 "1.2.643.2.2.30.4"
#define OID_HashVar_Default OID_HashVerbaO

/* OID for Crypt */
#define OID_CryptTest "1.2.643.2.2.31.0"
#define OID_CipherVerbaO "1.2.643.2.2.31.1"	/* ГОСТ 28147-89, параметры по умолчанию */
#define OID_CipherVar_1 "1.2.643.2.2.31.2"	/* ГОСТ 28147-89, параметры шифрования 1 */
#define OID_CipherVar_2 "1.2.643.2.2.31.3" 	/* ГОСТ 28147-89, параметры шифрования 2 */
#define OID_CipherVar_3 "1.2.643.2.2.31.4"	/* ГОСТ 28147-89, параметры шифрования 3 */
#define OID_CipherVar_Default OID_CipherVerbaO
#define OID_CipherOSCAR "1.2.643.2.2.31.5"	/* ГОСТ 28147-89, параметры Оскар 1.1 */
#define OID_CipherTestHash "1.2.643.2.2.31.6"	/* ГОСТ 28147-89, параметры Оскар 1.0 */
#define OID_CipherRIC1 "1.2.643.2.2.31.7"	/* ГОСТ 28147-89, параметры РИК 1 */
/* OID for Signature 1024*/
#define OID_SignDH128VerbaO   "1.2.643.2.2.32.2" 	/*VerbaO*/
#define OID_Sign128Var_1   "1.2.643.2.2.32.3" 
#define OID_Sign128Var_2   "1.2.643.2.2.32.4" 
#define OID_Sign128Var_3   "1.2.643.2.2.32.5" 
/* OID for DH 1024*/
#define OID_DH128Var_1   "1.2.643.2.2.33.1" 
#define OID_DH128Var_2   "1.2.643.2.2.33.2" 
#define OID_DH128Var_3   "1.2.643.2.2.33.3" 

#define OID_ECCTest3410 "1.2.643.2.2.35.0"
#define OID_ECCSignDHPRO "1.2.643.2.2.35.1"	/* ГОСТ Р 34.10-2001, параметры по умолчанию */
#define OID_ECCSignDHOSCAR "1.2.643.2.2.35.2"	/* ГОСТ Р 34.10-2001, параметры Оскар 2.x */
#define OID_ECCSignDHVar_1 "1.2.643.2.2.35.3"	/* ГОСТ Р 34.10-2001, параметры подписи 1 */
#define OID_ECCDHPRO "1.2.643.2.2.36.0"		/* ГОСТ Р 34.10-2001, параметры обмена по умолчанию */
#define OID_ECCDHPVar_1 "1.2.643.2.2.36.1"	/* ГОСТ Р 34.10-2001, параметры обмена 1 */


#define X509_GR3410_PARAMETERS ((LPCSTR) 5001)
#define OBJ_ASN1_CERT_28147_ENCRYPTION_PARAMETERS ((LPCSTR) 5007)

/* Синонимы для совместимости с версией 1.1*/
#define OID_SipherVerbaO	OID_CipherVerbaO
#define OID_SipherVar_1		OID_CipherVar_1 
#define OID_SipherVar_2		OID_CipherVar_2 
#define OID_SipherVar_3		OID_CipherVar_3 
#define OID_SipherVar_Default	OID_CipherVar_Default


/* Длина секретного ключа для ГОСТ 28147, подписи и обмена.*/
/*! \ingroup ProCSPData 
 *  \def SECRET_KEY_LEN
 * \brief Длина ключа
 *
 * Длина в байтах ключа ГОСТ 28147-89 и секретных ключей 
 * ГОСТ Р 34.10-94 и ГОСТ Р 28147-89.
 */
#define SECRET_KEY_LEN		32

/*! \ingroup ProCSPData 
 *  \def G28147_KEYLEN
 *  \brief Длина ключа ГОСТ 28147-89
 *
 * Длина в байтах ключа ГОСТ 28147-89.
 * \ref SECRET_KEY_LEN
 */
#define G28147_KEYLEN        SECRET_KEY_LEN

/*! \ingroup ProCSPData
 *  \def EXPORT_IMIT_SIZE 
 *  \brief Длина имитовставки
 *
 * Длина в байтах имитовставки при импорте/экспорте 
 */
#define EXPORT_IMIT_SIZE		4

/*! \ingroup ProCSPData 
 *  \def SEANCE_VECTOR_LEN
 *  \brief Длина  вектора инциализации алгоритма
 *
 * Длина в байтах вектора инциализации алгоритма 
 */
#define SEANCE_VECTOR_LEN		8


/* Константы и структуры для схем цифровой подписи и*/
/* открытого распределения ключей*/

/*! \ingroup ProCSPData 
 *  \def GR3410_1_MAGIC
 *  \brief Признак ключей
 *
 * Признак ключей ГОСТ Р 34.10-94 и ГОСТ Р 34.10-2001
 */
#define GR3410_1_MAGIC			0x3147414D
#define GR3410_2_MAGIC			GR3410_1_MAGIC//0x3145474a
	
#define DH_1_MAGIC			GR3410_1_MAGIC
#define DH_2_MAGIC			GR3410_1_MAGIC
#define DH_3_MAGIC			GR3410_1_MAGIC

/*! \ingroup ProCSPData 
 *  \def G28147_MAGIC
 *  \brief Признак ключей ГОСТ 28147-89 и TLS
 *
 * Признак ключей ГОСТ 28147-89 и мастер-ключей TLS
 */
#define G28147_MAGIC			0x374a51fd
#define SIMPLEBLOB_MAGIC		G28147_MAGIC

/*! \ingroup ProCSPData 
 *  \def DIVERS_MAGIC
 *  \brief Признак ключевого блоба функции диверсификации ключа 
 */
#define DIVERS_MAGIC			0x31564944

/*! \ingroup ProCSPData 
 *  \def BLOB_VERSION
 *  \brief Текущее значение версии ключевого блоба.
 */
#define BLOB_VERSION			(BYTE)0x20



/* Определения для структуры SIMPLEBLOB*/
/* Заголовок SIMPLEBLOB*/
/*!
 * \ingroup ProCSPData
 *
 * \brief Структура CRYPT_SIMPLEBLOB_HEADER
 *
 * Является расширением структуры BLOBHEADER и 
 * находится в начале поля \b pbData ключевого блоба типа SIMPLEBLOB для ключей "КриптоПро CSP".
 *
 * \requirements "Windows NT/2000:" Необходимо Windows NT 4.0 или
 * старше с Internet Explorer 5.0 или старше.
 *
 * \requirements "Windows 95/98/ME:" Необходимо Windows 95 OSR2 или
 * старше с Internet Explorer 5.0 или старше.
 *
 * \requirements "Файл описания:" Прототип описан в файле CSPKernel.h.
 *
 * \sa _PUBLICKEYSTRUC
 * \sa PCRYPT_SIMPLEBLOB
 * \sa CPExportKey
 * \sa CPGetKeyParam
 */
typedef struct CRYPT_SIMPLEBLOB_HEADER {
    BLOBHEADER BlobHeader;
                    /*!< Общий заголовок ключевого блоба. Определяет алгоритм ключа
                     * находящегося в ключевом блобе. См. \ref _PUBLICKEYSTRUC.
                     */
    DWORD Magic;
                    /*!< Признак ключей по ГОСТ 28147-89 или мастер ключей TLS,
                     * устанавливается в \ref G28147_MAGIC.
                     */
    ALG_ID EncryptKeyAlgId;
                    /*!< Определяет алгоритм экспорта ключа. Этот алгоритм является
                     * параметром ключа экспорта. См. \ref #CPGetKeyParam.
                     */
} CRYPT_SIMPLEBLOB_HEADER;
/*!
 * \ingroup ProCSPData
 *
 * \brief Псевдоструктура CRYPT_SIMPLEBLOB
 *
 * Полностью описывает ключевой блоб
 * типа SIMPLEBLOB для ключей "КриптоПро CSP". Все поля этой псевдоструктуры 
 * выравнены по границе байта и находятся в сетевом порядке байт (ASN1 DER).
 *
 * \requirements "Windows NT/2000:" Необходимо Windows NT 4.0 или
 * старше с Internet Explorer 5.0 или старше.
 *
 * \requirements "Windows 95/98/ME:" Необходимо Windows 95 OSR2 или
 * старше с Internet Explorer 5.0 или старше.
 *
 * \requirements "Файл описания:" Прототип описан в файле CSPKernel.h.
 *
 * \sa CRYPT_SIMPLEBLOB_HEADER
 * \sa CPExportKey
 * \sa CPGetKeyParam
 */
typedef struct CRYPT_SIMPLEBLOB {
    CRYPT_SIMPLEBLOB_HEADER tSimpleBlobHeader;
                    /*!< Общий заголовок ключевого блоба типа SIMPLEBLOB "КриптоПро CSP".
                     */
    BYTE    bSV[SEANCE_VECTOR_LEN];
                    /*!< Вектор инциализации для алгоритма CALG_PRO_EXPORT. 
                     * См. \ref SEANCE_VECTOR_LEN.
                     */
    BYTE    bEncryptedKey[G28147_KEYLEN];
                    /*!< Зашифрованный ключ ГОСТ 28147-89.
                     * См. \ref G28147_KEYLEN.
                     */
    BYTE    bMacKey[EXPORT_IMIT_SIZE];
                    /*!< Имитовставка по ГОСТ 28147-89 на ключ. Рассчитывается
                     * до зашифрования и проверяется после расшифрования.
                     * См. \ref EXPORT_IMIT_SIZE.
                     */
    BYTE    bEncryptionParamSet[1/*максимум MAX_OID_LEN*/];
                    /*!< Содержит ASN1 структуру в DER кодировке, определяющую 
                     * параметры алгоритама шифрования ГОСТ 28147-89:
                     * \code
                     *      encryptionParamSet
                     *          OBJECT IDENTIFIER (
                     *              id-Gost28147-89-TestParamSet |      -- Only for tests use
                     *              id-Gost28147-89-CryptoPro-A-ParamSet |
                     *              id-Gost28147-89-CryptoPro-B-ParamSet |
                     *              id-Gost28147-89-CryptoPro-C-ParamSet |
                     *              id-Gost28147-89-CryptoPro-D-ParamSet |
                     *              id-Gost28147-89-CryptoPro-Simple-A-ParamSet |
                     *              id-Gost28147-89-CryptoPro-Simple-B-ParamSet |
                     *              id-Gost28147-89-CryptoPro-Simple-C-ParamSet |
                     *              id-Gost28147-89-CryptoPro-Simple-D-ParamSet
                     * \endcode
                     * См. \ref MAX_OID_LEN.
                     */
}   CRYPT_SIMPLEBLOB, *PCRYPT_SIMPLEBLOB;


/*!
 * \ingroup ProCSPData
 *
 * \brief Структура CRYPT_PUBKEYPARAM
 *
 * Содержит признак ключей 
 * по ГОСТ Р 34.10-94 или ГОСТ Р 34.10-2001.
 *
 * \requirements "Windows NT/2000:" Необходимо Windows NT 4.0 или
 * старше с Internet Explorer 5.0 или старше.
 *
 * \requirements "Windows 95/98/ME:" Необходимо Windows 95 OSR2 или
 * старше с Internet Explorer 5.0 или старше.
 *
 * \requirements "Файл описания:" Прототип описан в файле CSPKernel.h.
 *
 * \sa CRYPT_PUBKEY_INFO_HEADER
 * \sa CPExportKey
 * \sa CPGetKeyParam
 */
typedef struct _CRYPT_PUBKEYPARAM_ {
    DWORD Magic;
                    /*!< Признак ключей по ГОСТ Р 34.10-94 или ГОСТ Р 34.10-2001
                     * устанавливается в \ref GR3410_1_MAGIC.
                     */
    DWORD BitLen;
                    /*!< Длина открытого ключа в битах.
                     */
} CRYPT_PUBKEYPARAM, *LPCRYPT_PUBKEYPARAM;

/* Заголовок PUBLICKEYBLOB и PRIVATEKEYBLOB*/
/*!
 * \ingroup ProCSPData
 *
 * \brief Структура CRYPT_PUBKEY_INFO_HEADER
 * 
 * Содержит заголовок
 * блоба открытго ключа или блоба ключевой пары
 * по ГОСТ Р 34.10-94 или ГОСТ Р 34.10-2001.
 *
 * \requirements "Windows NT/2000:" Необходимо Windows NT 4.0 или
 * старше с Internet Explorer 5.0 или старше.
 *
 * \requirements "Windows 95/98/ME:" Необходимо Windows 95 OSR2 или
 * старше с Internet Explorer 5.0 или старше.
 *
 * \requirements "Файл описания:" Прототип описан в файле CSPKernel.h.
 *
 * \sa _PUBLICKEYSTRUC
 * \sa CRYPT_PUBKEYPARAM
 * \sa CPExportKey
 * \sa CPGetKeyParam
 */
typedef struct CRYPT_PUBKEY_INFO_HEADER {
    BLOBHEADER BlobHeader;
                    /*!< Общий заголовок ключевого блоба. Определяет его тип и алгоритм ключа
                     * находящегося в ключевом блобе. Для открытых ключей алгоритм 
                     * ключа всегда, либо CALG_GR3410, либо CALG_GR3410EL. Для ключевых 
                     * пар алгоритм отражает её назначение. См. \ref _PUBLICKEYSTRUC.
                     */
    CRYPT_PUBKEYPARAM KeyParam;
                    /*!< Основной признак и длинна ключей ГОСТ Р 34.10-94 и ГОСТ Р 34.10-2001.
                     */
} CRYPT_PUBKEY_INFO_HEADER;

/*!
 * \ingroup ProCSPData
 *
 * \brief Псевдоструктура CRYPT_PUBLICKEYBLOB
 *
 * Описывает ключевой блоб
 * типа PUBLICKEYBLOB для ключей "КриптоПро CSP". Все поля этой псевдоструктуры 
 * выравнены по границе байта и находятся в сетевом порядке байт (ASN1 DER).
 *
 * \requirements "Windows NT/2000:" Необходимо Windows NT 4.0 или
 * старше с Internet Explorer 5.0 или старше.
 *
 * \requirements "Windows 95/98/ME:" Необходимо Windows 95 OSR2 или
 * старше с Internet Explorer 5.0 или старше.
 *
 * \requirements "Файл описания:" Прототип описан в файле CSPKernel.h.
 *
 * \sa CRYPT_PUBKEY_INFO_HEADER
 * \sa CPExportKey
 * \sa CPGetKeyParam
 */
typedef struct CRYPT_PUBLICKEYBLOB {
    CRYPT_PUBKEY_INFO_HEADER tPublicKeyParam;
                    /*!< Общий заголовок ключевого блоба типа PUBLICKEYBLOB "КриптоПро CSP".
                     */
    BYTE    bASN1GostR3410_94_PublicKeyParameters[1/*псевдомассив*/];
                    /*!< Содержит ASN1 структуру в DER кодировке, определяющую 
                     * параметры открытого ключа:
                     * \code
                     *      GostR3410-94-PublicKeyParameters ::=
                     *           SEQUENCE {
                     *              publicKeyParamSet
                     *                  OBJECT IDENTIFIER (
                     *                      id-GostR3410-94-TestParamSet |      -- Only for tests use
                     *                      id-GostR3410-94-CryptoPro-A-ParamSet |
                     *                      id-GostR3410-94-CryptoPro-B-ParamSet |
                     *                      id-GostR3410-94-CryptoPro-C-ParamSet |
                     *                      id-GostR3410-94-CryptoPro-D-ParamSet |
                     *                      id-GostR3410-94-CryptoPro-XchA-ParamSet |
                     *                      id-GostR3410-94-CryptoPro-XchB-ParamSet |
                     *                      id-GostR3410-94-CryptoPro-XchC-ParamSet
                     *                  ),
                     *              digestParamSet
                     *                  OBJECT IDENTIFIER (
                     *                      id-GostR3411-94-TestParamSet |      -- Only for tests use
                     *                      id-GostR3411-94-CryptoProParamSet
                     *                  ),
                     *              encryptionParamSet
                     *                  OBJECT IDENTIFIER (
                     *                      id-Gost28147-89-TestParamSet |      -- Only for tests use
                     *                      id-Gost28147-89-CryptoPro-A-ParamSet |
                     *                      id-Gost28147-89-CryptoPro-B-ParamSet |
                     *                      id-Gost28147-89-CryptoPro-C-ParamSet |
                     *                      id-Gost28147-89-CryptoPro-D-ParamSet |
                     *                      id-Gost28147-89-CryptoPro-Simple-A-ParamSet |
                     *                      id-Gost28147-89-CryptoPro-Simple-B-ParamSet |
                     *                      id-Gost28147-89-CryptoPro-Simple-C-ParamSet |
                     *                      id-Gost28147-89-CryptoPro-Simple-D-ParamSet
                     *                  ) OPTIONAL
                     *          }
                     * \endcode
                     */
    BYTE    bPublicKey[1/*псевдомассив*/];
                    /*!< Содержит открытый ключ в сетевом представлении (ASN1 DER). 
                     * Длина массива равна tPublicKeyParam.KeyParam.BitLen/8.
                     */
}   CRYPT_PUBLICKEYBLOB, *PCRYPT_PUBLICKEYBLOB;

/*!
 * \ingroup ProCSPData
 *
 * \brief Псевдоструктура CRYPT_PRIVATEKEYBLOB
 *
 * Описывает ключевой блоб
 * типа PRIVATEKEYBLOB для ключей "КриптоПро CSP". Все поля этой псевдоструктуры 
 * выравнены по границе байта и находятся в сетевом порядке байт (ASN1 DER).
 *
 * \requirements "Windows NT/2000:" Необходимо Windows NT 4.0 или
 * старше с Internet Explorer 5.0 или старше.
 *
 * \requirements "Windows 95/98/ME:" Необходимо Windows 95 OSR2 или
 * старше с Internet Explorer 5.0 или старше.
 *
 * \requirements "Файл описания:" Прототип описан в файле CSPKernel.h.
 *
 * \sa CRYPT_PUBKEY_INFO_HEADER
 * \sa CPExportKey
 * \sa CPGetKeyParam
 */
typedef struct CRYPT_PRIVATEKEYBLOB {
    CRYPT_PUBKEY_INFO_HEADER tPublicKeyParam;
                    /*!< Общий заголовок ключевого блоба типа PRIVATEKEYBLOB "КриптоПро CSP".
                     */
    BYTE    bExportedKeys[1/* Псевдо массив.*/];
	/*
	KeyTransferContent ::=
	SEQUENCE {
	    encryptedPrivateKey  GostR3410EncryptedKey,
	    privateKeyParameters PrivateKeyParameters,
	}
	KeyTransfer ::=
	SEQUENCE {
	    keyTransferContent       KeyTransferContent,
	    hmacKeyTransferContent   Gost28147HMAC
	}
	*/
}   CRYPT_PRIVATEKEYBLOB, *PCRYPT_PRIVATEKEYBLOB;

/* Определения для структуры DIVERSBLOB*/
/*!
 * \ingroup ProCSPData
 *
 * \brief Структура CRYPT_DIVERSBLOBHEADER
 * 
 * Описывает блоб
 * типа DIVERSBLOB для процедуры диверсификации ключей "КриптоПро CSP-ECC/TLS".
 * Все поля этой псевдоструктуры
 * выравнены по границе байта и находятся в сетевом порядке байт (ASN1 DER).
 *
 * \requirements "Windows NT/2000:" Необходимо Windows NT 4.0 или
 * старше с Internet Explorer 5.0 или старше.
 *
 * \requirements "Windows 95/98/ME:" Необходимо Windows 95 OSR2 или
 * старше с Internet Explorer 5.0 или старше.
 *
 * \requirements "Файл описания:" Прототип описан в файле WinCryptEx.h 
 * или CSP_WinCrypt.h для Solaris.
 *
 * \sa CPImportKey
 */
typedef struct _CRYPT_DIVERSBLOBHEADER {
    BLOBHEADER BlobHeader;
                /*!< Общий заголовок блоба, диверсифицирующего ключ.
                 *  Определяет алгоритм диверсифицирумого ключа.
                 */
    ALG_ID      aiDiversAlgId;
                /*!< Определяет алгоритм диверсификации ключа.
                 */
    DWORD       dwDiversMagic;
                /*!< Признак диверсификации ключа,
                 * устанавливается в \ref DIVERS_MAGIC.
                 */
   /*    BYTE        *pbDiversData;
                !< Указатель на данные, по которым диверсифицируется ключ.
                 */
    DWORD       cbDiversData;
                /*!< Длина данных, по которым диверсифицируется ключ.
                 */
} CRYPT_DIVERSBLOBHEADER, *LPCRYPT_DIVERSBLOBHEADER;
typedef struct _CRYPT_DIVERSBLOB {
    CRYPT_DIVERSBLOBHEADER DiversBlobHeader;
                /*!< Общий заголовок блоба, диверсифицирующего ключ.
                 *  Определяет алгоритм диверсифицирумого ключа.
                 */
    BYTE        bDiversData[4/*максимум 40*/];
                /*!< Данные, по которым диверсифицируется ключ.
                 */
} CRYPT_DIVERSBLOB, *LPCRYPT_DIVERSBLOB;

/*! \brief Тип устанавливаемого пароля: пароль или pin */
#define CRYPT_CHANGE_PIN_PASSWD 0
/*! \brief Тип устанавливаемого пароля: имя контейнера зашифрования */
#define CRYPT_CHANGE_PIN_CONTAINER 1
/*! \brief Тип устанавливаемого пароля: HANDLE контейнера зашифрования.
     Используется имя контейнера. */
#define CRYPT_CHANGE_PIN_HANDLE_CONTAINER 2
/*! \brief Тип устанавливаемого пароля: HANDLE контейнера зашифрования.
     Используется уникальное имя контейнера. */
#define CRYPT_CHANGE_PIN_HANDLE_UNIQUE 3
/*! \brief Тип устанавливаемого пароля: тип и значение выбираются в окне. */
#define CRYPT_CHANGE_PIN_WINDOW 4
/*! \brief Тип устанавливаемого пароля: Очитсить пароль. Эквивалентно
    установке пароля CRYPT_CHANGE_PIN_PASSWD с NULL или пустой строкой. */
#define CRYPT_CHANGE_PIN_CLEAR 5

/*!
 * \brief Структура передачи пароля, pin-кода, имени контейнера,
 *  HANDLE контейнера при смене пароля.
 */
typedef union CRYPT_CHANGE_PIN_SOURCE_ { 
    char *passwd; /*!< Пароль, PIN-код, имя контейнера. */
    HCRYPTPROV prov; /*!< HANDLE контейнера. */
} CRYPT_CHANGE_PIN_SOURCE;

/*!
 * \ingroup ProCSPData
 *
 * \brief Структура передачи информации для:
 *  1) смены пароля контейнера, 
 *  2) указания способа доступа к контейнеру (имя, handle, пароль), на ключе которого 
 *     зашифровано содержимое другого контейнера.
 *
 * \requirements "Windows NT/2000:" Необходимо Windows NT 4.0 или
 * старше с Internet Explorer 5.0 или старше.
 *
 * \requirements "Windows 95/98/ME:" Необходимо Windows 95 OSR2 или
 * старше с Internet Explorer 5.0 или старше.
 *
 * \requirements "Файл описания:" Прототип описан в файле WinCryptEx.h 
 * или CSP_WinCrypt.h для Solaris.
 *  
 * \sa CPSetProvParam
 */
typedef struct CRYPT_CHANGE_PIN_PARAM_ {
    BYTE type; 
    /*!< Тип данных.
 *  CRYPT_CHANGE_PIN_PASSWD - пароль или PIN,
 *  CRYPT_CHANGE_PIN_CONTAINER - имя контейнера зашифрования,
 *  CRYPT_CHANGE_PIN_HANDLE_CONTAINER - HANDLE контейнера зашифрования.
 *     Используется имя контейнера.
 *  CRYPT_CHANGE_PIN_HANDLE_UNIQUE - HANDLE контейнера зашифрования.
 *     Используется уникальное имя контейнера.
 *  CRYPT_CHANGE_PIN_WINDOW - тип и значение выбираются в окне,
 *  CRYPT_CHANGE_PIN_CLEAR - очитсить пароль.
 */
     CRYPT_CHANGE_PIN_SOURCE dest; /*!< Данные соответствующего типа */
} CRYPT_CHANGE_PIN_PARAM;


#endif /* _WINCRYPTEX_H_INCLUDED */



/*! \ingroup ProCSPEx 
 * \def DP1 
 * Идентификаторы алгоритмов приведены в файле WinCryptEx.h 
 * <table>
 * <tr><th>Идентификатор</th><th>Описание идентификатора</th></tr>
 * <tr><td>CALG_GR3411</td><td>Идентификатор алгоритма хеширования по ГОСТ Р 34.11-94.</td></tr>
 * <tr><td>CALG_G28147_MAC</td><td>Идентификатор алгоритма имитозащиты по ГОСТ 28147 89.</td></tr>
 * <tr><td>CALG_G28147_IMIT </td><td>Идентификатор алгоритма имитозащиты по ГОСТ 28147 89.</td></tr><tr><td> </td><td> </td></tr>
 * <tr><td> CALG_GR3410 </td><td> Идентификатор алгоритма имитозащиты по ГОСТ 28147 89. </td></tr>
 * <tr><td> CALG_GR3410EL </td><td> Идентификатор алгоритма ЭЦП по ГОСТ Р 34.10-2001.</td></tr>
 * <tr><td>CALG_G28147</td><td>Идентификатор алгоритма шифрования по ГОСТ 28147 89. </td></tr>
 * <tr><td>CALG_DH_EX_SF </td><td>Идентификатор алгоритма обмена ключей по Диффи-Хеллману на базе секретного ключа пользователя. </td></tr>
 * <tr><td>CALG_DH_EX_EPHEM </td><td>Идентификатор CALG_DH_EX алгоритма обмена ключей по Диффи-Хеллману на базе эфемерного секретного ключа. Открытый ключ получается по ГОСТ Р 34.10 94.</td></tr>
 * <tr><td>CALG_DH_EX </td><td>Идентификатор CALG_DH_EX алгоритма обмена ключей по Диффи-Хеллману на базе секретного ключа пользователя. Открытый ключ получается по ГОСТ Р 34.10 94. </td></tr>
 * <tr><td>CALG_DH_EL_SF </td><td>Идентификатор алгоритма обмена ключей по Диффи-Хеллману на базе секретного ключа пользователя. Открытый ключ получается по ГОСТ Р 34.10 2001.</td></tr>
 * <tr><td> CALG_DH_EL_EPHEM</td><td> Идентификатор алгоритма обмена ключей по Диффи-Хеллману на базе эфемерного секретного ключа. Открытый ключ получается по ГОСТ Р 34.10 2001.</td></tr>
 * <tr><td>CALG_PRO_AGREEDKEY_DH</td><td>Идентификатор алгоритма выработки ключа парной связи по Диффи-Хеллману. </td></tr>
 * <tr><td>CALG_PRO_EXPORT </td><td> Идентификатор алгоритма защищённого экспорта ключа.</td></tr>
 * <tr><td>CALG_SIMPLE_EXPORT </td><td>Идентификатор алгоритма простого экспорта ключа. </td></tr>
 * <tr><td>CALG_SIMMETRYKEY </td><td> Идентификатор алгоритма шифрования по ГОСТ 28147 8.</td></tr>
 * <tr><td> CALG_TLS1_MASTER_HASH</td><td>Идентификатор алгоритма выработки объекта MASTER_HASH протокола TLS1.</td></tr>
 * <tr><td> CALG_TLS1_MAC_KEY</td><td>Идентификатор алгоритма выработки ключа имитозащиты протокола TLS1. </td></tr>
 * <tr><td>CALG_TLS1_ENC_KEY </td><td> Идентификатор алгоритма выработки ключа шифрования протокола TLS1.</td></tr>
 * <tr><td> CALG_PRO_DIVERS</td><td>Идентификатор алгоритма КриптоПро диверсификации ключа.</td></tr>
 * <tr><td> CALG_RIC_DIVERS</td><td>Идентификатор алгоритма РИК диверсификации ключа. </td></tr>
 *</table>
 * \brief Идентификаторы алгоритмов криптопровайдера
 */

#define DP1 


/*! \ingroup ProCSPEx 
 * \def DP2 
 *  Определения данных параметров приведены в файле WinCryptEx.h 
 * <table>
 * <tr><th>Параметр</th><th>Значение параметра</th></tr>
 * <tr><td>CRYPT_PROMIX_MODE </td><td>Задание режимов шифрования/расшифрования по ГОСТ 28147-89 с преобразованием ключа через каждые 1 КВ обрабатываемой информации </td></tr>
 * <tr><td>CRYPT_SIMPLEMIX_MODE </td><td>Задание режимов шифрования/расшифрования по ГОСТ 28147-89 без преобразования ключа в процессе обработки информации</td></tr>
 *</table>
 * \brief Режимы шифрования
*/

#define DP2 


/*! \ingroup ProCSPEx 
 * \def DP3 
 * Определения данных параметров приведены в файле WinCryptEx.h 
 *
 * <table>
 * <tr><th>Параметр</th><th>Значение параметра</th></tr>
 * <tr><td>PP_ENUMOIDS_EX</td><td>Задает перечень идентификаторов объектов, используемых в криптопровайдере</td></tr>
 * <tr><td>PP_HASHOID</td><td>Выдает установленный в контейнере OID узла замены функции хеширования ГОСТ Р 34.11-94 для наследования криптографическими объектами</td></tr>
 * <tr><td>PP_CIPHEROID</td><td>Выдает установленный в контейнере OID узла замены алгоритма шифрования ГОСТ 28147-89 для наследования криптографическими объектами </td></tr>
 * <tr><td>PP_SIGNATUREOID</td><td>Выдает установленный в контейнере OID параметров цифровой подписи для наследования криптографическими объектами </td></tr>
 * <tr><td>PP_DHOID </td><td>Выдает установленный в контейнере OID параметров алгоритма Диффи-Хеллмана для наследования криптографическими объектами  </td></tr>
 * <tr><td>PP_CHECKPUBLIC </td><td>Флаг контроля открытого ключа. Если флаг установлен, осуществляется проверка алгебраических свойств открытого ключа </td></tr>
 *</table>
 *
 *
 * Параметры криптопровайдера, используемые для перехода на платформы, отличные от WIN32
 *
 * <table>
 * <tr><th>Параметр</th><th>Значение параметра</th></tr>
 * <tr><td>PP_ANSILASTERROR</td><td>Запрос последней ошибки при обращении к функциям ANSI </td></tr>
 * <tr><td>PP_RANDOM</td><td>Выдает блоб типа SIMPLEBLOB для инициализации ДСЧ в драйвере шифрования</td></tr>
 * <tr><td>PP_DRVCONTAINER </td><td>Выдает указатель (handel) контейнера в драйвере</td></tr>
 * <tr><td>PP_MUTEX_ARG</td><td>Инициализирует синхронизацию потоков криптопровайдера в драйверном исполнении</td></tr>
 * <tr><td>PP_ENUM_HASHOID</td><td>Задает в провайдере перечень идентификаторов криптографических объектов, связанных с функцией хеширования </td></tr>
 * <tr><td>PP_ENUM_CIPHEROID</td><td>Задает в провайдере перечень идентификаторов криптографических объектов, связанных с функцией шифрования  </td></tr>
 * <tr><td>PP_ENUM_SIGNATUREOID</td><td>Задает в провайдере перечень идентификаторов криптографических объектов, связанных с функцией цифровой подписи</td></tr>
 * <tr><td>PP_ENUM_DHOID</td><td>Задает в провайдере перечень идентификаторов криптографических объектов, связанных с алгоритмом Диффи-Хеллмана  </td></tr>
 *</table>
 * \brief Дополнительные параметры криптопровайдера
*/

#define DP3 

/*! \ingroup ProCSPEx 
 * \def DP4 
 * Определения данных параметров приведены в файле WinCryptEx.h 
 * <table>
 * <tr><th>Параметр</th><th>Значение параметра</th></tr>
 * <tr><td>DIVERSKEYBLOB</td><td>Тип ключевого блоба для диверсификации ключей с помощью 
    функции CPImportKey в режиме ключа импорта CALG_PRO_EXPORT</td></tr>
 *</table>
 * \brief Параметры дополнительных ключевых блобов   
*/

#define DP4 

/*! \ingroup ProCSPEx 
 * \def DP5
 * Определения данных параметров приведены в файле WinCryptEx.h 
 * <table>
 * <tr><th>Параметр</th><th>Значение параметра</th></tr>
 * <tr><td>HP_HASHSTARTVECT</td><td>Стартовый вектор функции хештрования, устанавливаемый приложением</td></tr>
 * <tr><td>HP_OID</td><td>Задает узел замены функции хеширования</td></tr>
 *</table>
 * \brief Дополнительные параметры объекта хеширования 
*/

#define DP5

/*! \ingroup ProCSPEx 
 * \def DP6 
 * Определения данных параметров приведены в файле WinCryptEx.h 
 *
 * <table>
 * <tr><th>Параметр</th><th>Значение параметра</th></tr>
 * <tr><td>KP_IV </td><td>Начальный вектор инициализации алгоритма шифрования ГОСТ 28147-89</td></tr>
 * <tr><td>KP_MIXMODE</td><td>Определяет использование преобразования ключа после обработки 1КВ информации в режимах шифрования/расшифрования и вычисления имитовставки алгоритма ГОСТ 28147-89 </td></tr>
 * <tr><td>KP_OID</td><td>Задает узел замены функции хеширования</td></tr>
 * <tr><td>KP_HASHOID</td><td>Идентификатор узла замены функции хеширования ГОСТ Р 34.11-94</td></tr>
 * <tr><td>KP_CIPHEROID</td><td>Идентификатор узла замены алгоритма шифрования ГОСТ 28147-89</td></tr>
 * <tr><td>KP_SIGNATUREOID</td><td>Идентификатор параметров цифровой подписи</td></tr>
 * <tr><td>KP_DHOID</td><td>Идентификатор параметров алгоритма Диффи-Хеллмана</td></tr>
 *</table>
 * \brief Дополнительные параметры ключей
*/

#define DP6

/*! \ingroup ProCSPEx 
 * \def DP7 
 * Определения кодов приведены в файле WinCryptEx.h 
 *
 * <table>
 * <tr><th>Параметр</th><th>Значение параметра</th></tr>
 * <tr><td>GPE_FAIL_TESTBUFFER</td><td>Последовательности ДСЧ не прошла статистический контроль на материале 1 КВ</td></tr>
 * <tr><td>GPE_FAIL_STATBUFFER</td><td>Последовательности ДСЧ не прошла статистический контроль на материале 18 КВ</td></tr>
 * <tr><td>GPE_DIFERENT_PARAMETERS</td><td>Неправильные параметры </td></tr>
 * <tr><td>GPE_CORRUPT_KEYCONTEXT </td><td>Нарушена целостность ключей алгоритма ГОСТ 28147-89 </td></tr>
 * <tr><td>GPE_CHECKPROC_GAMMING_OFB </td><td>Ошибка в режиме гаммирования алгоритма ГОСТ 28147-89 </td></tr>
 * <tr><td>GPE_CHECKPROC_ENCRYPT_CFB </td><td>Ошибка в режиме гаммирования с обратной связьюалгоритма ГОСТ 28147-89</td></tr>
 * <tr><td>GPE_CHECKCARRIER </td><td>При закрытии ключевого контейнера в считывателе отсутствует или подменен ключевой носитель</td></tr>
 * <tr><td>GPE_CORRUPT_KEYPAIR_INFO </td><td>Искажена ключевая пара цифровой подписи </td></tr>
 * <tr><td>GPE_CHECKPROC_TESTFAIL </td><td>Ошибка при тестировании функций провайдера </td></tr>
 * <tr><td>GPE_RANDOMFAIL</td><td>Ошибка в функционировании ДСЧ </td></tr>
 *</table>
 *
 *
 * Коды ошибок возвращаются 4 байтами в аргументе pbData функции CryptGetProvParam() при значении аргумента hProv=PP_LAST_ERROR.
 * \brief Дополнительные коды ошибок криптопровайдера
*/

#define DP7

/*! \ingroup ProCSPEx 
 * \def DP8 
 * Определения идентификаторов приведены в файле WinCryptEx.h 
 *
 * <table>
 * <tr><th>Параметр</th><th>Индекс</th><th>Значение параметра</th></tr>
 * <tr><td>szOID_CP_GOST_28147</td><td>"1.2.643.2.2.21"</td><td>Алгоритм шифрования ГОСТ 28147-89</td><td></td></tr>
 * <tr><td>szOID_CP_GOST_R3411</td><td>"1.2.643.2.2.9"</td><td>Функция хеширования ГОСТ Р 34.11-94</td><td></td></tr>
 * <tr><td>szOID_CP_GOST_R3410</td><td>"1.2.643.2.2.20"</td><td>Алгоритм ГОСТ Р 34.10-94, используемый при экспорте/импорте ключей</td></tr>
 * <tr><td>szOID_CP_GOST_R3410EL</td><td>"1.2.643.2.2.19"</td><td>Алгоритм ГОСТ Р 34.10-2001, используемый при экспорте/импорте ключей</td></tr>
 * <tr><td>szOID_CP_DH_EX</td><td>"1.2.643.2.2.99"</td><td>Алгоритм Диффи-Хеллмана на базе потенциальной функции</td></tr>
 * <tr><td>szOID_CP_DH_EL</td><td>"1.2.643.2.2.98"</td><td>Алгоритм Диффи-Хеллмана на базе эллиптической кривой</td></tr>
 * <tr><td>szOID_CP_GOST_R3411_R3410</td><td>"1.2.643.2.2.4"</td><td>Алгоритм цифровой подписи ГОСТ Р 34.10-94</td></tr>
 * <tr><td>szOID_CP_GOST_R3411_R3410EL</td><td>"1.2.643.2.2.3"</td><td>Алгоритм цифровой подписи ГОСТ Р 34.10-2001</td></tr>
 * <tr><td>szOID_KP_TLS_PROXY</td><td>"1.2.643.2.2.34.1"</td><td>Аудит TLS-трафика</td></tr>
 * <tr><td>szOID_KP_RA_CLIENT_AUTH</td><td>"1.2.643.2.2.34.2"</td><td>Идентификация пользователя на центре регистрации</td></tr>
 * <tr><td>szOID_KP_WEB_CONTENT_SIGNING</td><td>"1.2.643.2.2.34.3"</td><td>Подпись содержимого сервера Интернет</td></tr>
 *</table>
 * \brief Групповые идентификаторы криптографических параметров алгоритмов
*/

#define DP8 

/*! \ingroup ProCSPEx 
 * \def DP9
 * Определения идентификаторов приведены в файле WinCryptEx.h 
 *
 * <table>
 * <tr><th>Параметр</th><th>Индекс</th><th>Значение параметра</th></tr>
 * <tr><td>OID_HashTest</td><td>"1.2.643.2.2.30.0"</td><td>Тестовый узел замены</td></tr>
 * <tr><td>OID_HashVerbaO</td><td>"1.2.643.2.2.30.1"</td><td>Узел замены функции хеширования, вариант "Верба-О"</td></tr>
 * <tr><td>OID_HashVerba_1</td><td>"1.2.643.2.2.30.2"</td><td>Узел замены функции хеширования, вариант 1</td></tr>
 * <tr><td>OID_HashVerba_2</td><td>"1.2.643.2.2.30.3"</td><td>Узел замены функции хеширования, вариант 2</td></tr>
 * <tr><td>OID_HashVerba_3</td><td>"1.2.643.2.2.30.4"</td><td>Узел замены функции хеширования, вариант 3</td></tr>
 * <tr><td>OID_HashVar_Default</td><td>"1.2.643.2.2.30.1"</td><td>Узел замены функции хеширования, дефолтовый вариант. В качестве дефолтового используется узел замены варианта  "Верба-О"</td></tr>
 * <tr><td>OID_CryptTest</td><td>"1.2.643.2.2.31.0"</td><td>Тестовый узел замены алгоритма шифрования</td></tr>
 * <tr><td>OID_CipherVerbaO</td><td>"1.2.643.2.2.31.1"</td><td>Узел замены алгоритма шифрования,вариант "Верба-О"</td></tr>
 * <tr><td>OID_CipherVerba_1</td><td>"1.2.643.2.2.31.2"</td><td>Узел замены алгоритма шифрования,вариант 1</td></tr>
 * <tr><td>OID_CipherVerba_2</td><td>"1.2.643.2.2.31.3"</td><td>Узел замены алгоритма шифрования,вариант 2</td></tr>
 * <tr><td>OID_CipherVerba_3</td><td>"1.2.643.2.2.31.4"</td><td>Узел замены алгоритма шифрования,вариант 3</td></tr>
 * <tr><td>OID_CipherVar_Default</td><td>"1.2.643.2.2.31.0"</td><td>Узел замены функции алгоритма шифрования, дефолтовый вариант. В качестве дефолтового используется узел замены варианта  "Верба-О</td></tr>
 * <tr><td>OID_CipherOSCAR</td><td>"1.2.643.2.2.31.5" </td><td>Узел замены, вариант карты КриптоРИК</tr>
 * <tr><td>OID_CipherTestHash</td><td>"1.2.643.2.2.31.6" </td><td>Узел замены, используемый при шифровании с хешированием</td></tr>
 * <tr><td>OID_SignDH128VerbaO</td><td>"1.2.643.2.2.32.2"</td><td>Параметры P,Q,A цифровой подписи ГОСТ Р 34.10-94, вариант "Верба-О". Могут использоваться также в алгоритме Диффи-Хеллмана</td></tr>
 * <tr><td>OID_SignDH128Verba_1</td><td>"1.2.643.2.2.32.3"</td><td>Параметры P,Q,A цифровой подписи ГОСТ Р 34.10-94, вариант 1. Могут использоваться также в алгоритме Диффи-Хеллмана</td></tr>
 * <tr><td>OID_SignDH128Verba_2</td><td>"1.2.643.2.2.32.4"</td><td>Параметры P,Q,A цифровой подписи ГОСТ Р 34.10-94, вариант 2. Могут использоваться также в алгоритме Диффи-Хеллмана</td></tr>
 * <tr><td>OID_SignDH128Verba_3</td><td>"1.2.643.2.2.32.5"</td><td>Параметры P,Q,A цифровой подписи ГОСТ Р 34.10-94, вариант 3. Могут использоваться также в алгоритме Диффи-Хеллмана</td></tr>
 * <tr><td>OID_DH128Var_1 </td><td>"1.2.643.2.2.33.1" </td><td>Параметры P,Q,A алгоритма Диффи-Хеллмана на базе экспоненциальной функции, вариант 1</td></tr>
 * <tr><td>OID_DH128Var_2 </td><td>"1.2.643.2.2.33.2" </td><td>Параметры P,Q,A алгоритма Диффи-Хеллмана на базе экспоненциальной функции, вариант 2</td></tr>
 * <tr><td>OID_DH128Var_3 </td><td>"1.2.643.2.2.33.3" </td><td>Параметры P,Q,A алгоритма Диффи-Хеллмана на базе экспоненциальной функции, вариант 3</td></tr>
 * <tr><td>OID_ECCTest3410</td><td>"1.2.643.2.2.35.0"</td><td>Тестовые параметры a, b, p,q, (x,y) алгоритма ГОСТ Р 34.10-2001 </td></tr>
 * <tr><td>OID_ECCSignDHPRO </td><td>"1.2.643.2.2.35.1"</td><td>Параметры a, b, p,q, (x,y) цифровой подписи и алгоритма Диффи-Хеллмана на базе алгоритма ГОСТ Р 34.10-2001, вариант провайдера </td></tr>
 * <tr><td>OID_ECCSignDHOSCAR</td><td>"1.2.643.2.2.35.2"</td><td>Параметры a, b, p,q, (x,y) цифровой подписи и алгоритма Диффи-Хеллмана на базе алгоритма ГОСТ Р 34.10-2001, вариант карты КриптоРИК</td></tr>
 * <tr><td>OID_ECCDHPRO </td><td>"1.2.643.2.2.36.0"</td><td> Параметры a, b, p,q, (x,y) алгоритма Диффи-Хеллмана на базе алгоритма ГОСТ Р 34.10-2001, вариант провайдера. Используются те же параметры, что и с идентификатором OID_ECCSignDHPRO</td></tr>
 * <tr><td>OID_ECCSignDHVar1</td><td>"1.2.643.2.2.35.3"</td><td>Параметры a, b, p,q, (x,y) цифровой подписи и алгоритма Диффи-Хеллмана на базе алгоритма ГОСТ Р 34.10-2001.</td></tr>
 *</table>
 * \brief Идентификаторы криптографических параметров алгоритмов
*/

#define DP9

/* end of file: $Id: WinCryptEx.h,v 1.105.4.7 2002/10/23 13:45:50 chudov Exp $ */
