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
 * \file $RCSfile: export.c,v $
 * \version $Revision: 1.6 $
 * \date $Date: 2001/12/25 15:57:52 $
 * \author $Author: pre $
 *
 * \brief Импорт/экспорт ключей и создание зашифрованного сообщения
 * на низком уровне
 */
#include "tmain.h"

int export_encrypt(LPSTR, LPSTR, LPSTR, LPSTR);
int export_decrypt(LPSTR, LPSTR, LPSTR, LPSTR);

int main_export_public(int argc, char **argv)
{
    BOOL encrypt = FALSE;
    BOOL decrypt = FALSE;
    BOOL print_help = FALSE;
    LPSTR infile_name = NULL;
    LPSTR outfile_name = NULL;
    LPSTR mycert_name  = NULL;
    LPSTR rcpcert_name  = NULL;
    int c;
    int ret = 0;

    /*-----------------------------------------------------------------------------*/
    /* определение опций разбора параметров*/
    static struct option long_options[] = {
    {"encrypt", no_argument,        NULL, 'e'},
    {"decrypt", no_argument,        NULL, 'd'},
	{"my",	    required_argument,  NULL, 'm'},
	{"rcp",	    required_argument,	NULL, 'r'},
	{"in",	    required_argument,	NULL, 'i'},
    {"out",     required_argument,  NULL, 'o'},
    {"help",    no_argument,        NULL, 'h'},
    {0, 0, 0, 0}
    };

    /*-----------------------------------------------------------------------------*/
    /* разбор параметров*/
    /* для разбора параметров используется модуль getopt.c*/
    while ((c = getopt_long_only (argc, argv, "", long_options, (int *)0)) != EOF) {
	switch (c) {
	case 'i':
	    infile_name = optarg;
	    break;
	case 'o':
	    outfile_name = optarg;
	    break;
	case 'm':
	    mycert_name = optarg;
	    break;
	case 'r':
        rcpcert_name = optarg;
	    break;
	case 'e':
	    encrypt = TRUE;
	    break;
	case 'd':
	    decrypt = TRUE;
	    break;
	case 'h':
	    ret = 1;
	    print_help = TRUE;
	    goto bad;
	case '?':
	default:
	    goto bad;
	}
    }
    if (c != EOF) {
	print_help = TRUE;
	goto bad;
    }

    if (!encrypt && !decrypt) {
	print_help = TRUE;
	goto bad;
    }

    if (encrypt) {
	ret = export_encrypt (mycert_name, rcpcert_name, infile_name, outfile_name);
    }
    else if (decrypt) {
	ret = export_decrypt (mycert_name, rcpcert_name, infile_name, outfile_name);
    }
    else {
	print_help = TRUE;
    }

bad:
    if (print_help) {
	fprintf(stderr,"%s -ep [options]\n", prog);
	fprintf(stderr,SoftName " export public key\nusing CAPI lowlevel message function type\n");
	fprintf(stderr,"options:\n");
	fprintf(stderr,"  -in arg        input filename to be encrypted or decrypted\n");
	fprintf(stderr,"  -out arg       output PKCS#7 filename\n");
	fprintf(stderr,"  -my name       use my certificate with commonName = name from system store \n");
    fprintf(stderr,"                 'MY' to sign/verify data\n");
	fprintf(stderr,"  -rcp name      use recepient's certificate commonName\n");
	fprintf(stderr,"  -encrypt       encrypt input file\n");
	fprintf(stderr,"  -decrypt       decrypt enveloped file, specified by input filename.\n");
	fprintf(stderr,"  -help          print this help\n\n");
    }

    return ret;


}

int export_encrypt(LPSTR mycert_name, LPSTR rcpcert_name, LPSTR infile_name, LPSTR outfile_name)
{
    HCRYPTPROV hProv = 0;
    HCRYPTKEY  hUserKey = 0;
    HCRYPTKEY  hPublicKey = 0;
    HCRYPTKEY  hExchKey = 0;
    HCRYPTKEY  hSessionKey = 0;
    PCCERT_CONTEXT pMyCert = 0;
    PCCERT_CONTEXT pRcpCert = 0;
    DWORD dwKeytype;
    BOOL bRelease;
    BYTE *pbData = NULL;
    DWORD cbData;
    BYTE *pbIn = NULL;
    BYTE *pbOut = NULL;
    BYTE *pbIV = NULL;
    DWORD dwIV = 0;
    UINT uiIn = 0;
    UINT uiOut = 0;
    int ret = 0;

    SetLastError(0);

    //чтение сертификата отправителя из хранилища
    if((pMyCert = read_cert_from_my(mycert_name)) == NULL)
    {
        DebugErrorFL("read_cert_from_my");
        goto end;
    }

    //определение соответствия алгоритма отправителя ГОСТу
    if(strcmp(pMyCert->pCertInfo->SubjectPublicKeyInfo.Algorithm.pszObjId, szOID_CP_GOST_R3410))
    {
        DebugErrorFL("Wrong algorithm");
        goto end;
    }

    //чтение сертификата получателя из хранилища
    if((pRcpCert = read_cert_from_my(rcpcert_name)) == NULL)
    {
        DebugErrorFL("read_cert_from_my");
        goto end;
    }

    //определения соответствия алгоритма получателя ГОСТу
    if(strcmp(pRcpCert->pCertInfo->SubjectPublicKeyInfo.Algorithm.pszObjId, szOID_CP_GOST_R3410))
    {
        DebugErrorFL("Wrong algorithm");
        goto end;
    }

    //инициализируем провайдер, использую свой сертификат с секретным ключом
    if(!CryptAcquireProvider("MY", pMyCert, &hProv, &dwKeytype, &bRelease))
    {
        DebugErrorFL("CryptAcquireProvider");
        goto end;
    }

    //проверка пригодности ключа для шифрования данных
    if (! (dwKeytype & AT_KEYEXCHANGE))
    {
        DebugErrorFL("Wrong key type");
        goto end;
    }

    //получение секретного ключа отправителя
    if (!CryptGetUserKey(hProv, AT_KEYEXCHANGE, &hUserKey))
    {
        DebugErrorFL("CryptGetUserKey");
        goto end;
    }

    //импорт открытого ключа получателя из сертификата, где он лежит в ASN 1.1
    if (!CryptImportPublicKeyInfoEx(hProv, X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, &pRcpCert->pCertInfo->SubjectPublicKeyInfo, CALG_GR3410, 0, NULL, &hPublicKey))
    {
        DebugErrorFL("CryptImportPublicKey");
        goto end;
    }

    //экспорт открытого ключа получателя в BLOB - определение размера требуемого обьема памяти
    if (!CryptExportKey(hPublicKey, 0, PUBLICKEYBLOB, 0, NULL, &cbData))
    {
        DebugErrorFL("CryptExportKey");
        goto end;
    }

    if ((pbData = (BYTE*)malloc(cbData)) == NULL)
    {
        DebugErrorFL("malloc");
        goto end;
    }

    //экспорт открытого ключа получателя в BLOB - собственно экспорт
    if (!CryptExportKey(hPublicKey, 0, PUBLICKEYBLOB, 0, pbData, &cbData))
    {
        DebugErrorFL("CryptExportKey");
        goto end;
    }

    //получение ключа обмена импортом секретного ключа отправителя 
    //на открытый ключ получателя
    if (!CryptImportKey(hProv, pbData, cbData, hUserKey, 0, &hExchKey))
    {
        DebugErrorFL("CryptImportKey");
        goto end;
    }

    free(pbData);
    pbData = NULL;

    //создание сессионного ключа
    if (!CryptGenKey(hProv, CALG_G28147, CRYPT_EXPORTABLE, &hSessionKey))
    {
        DebugErrorFL("CryptGenKey");
        goto end;
    }

    //шифрование ключа обмена на сессионном ключе - определение размера результата
    if (!CryptExportKey(hSessionKey, hExchKey, SIMPLEBLOB, 0, NULL, &cbData))
    {
        DebugErrorFL("CryptExportKey");
        goto end;
    }

    if ((pbData = (BYTE*)malloc(cbData)) == NULL)
    {
        DebugErrorFL("malloc");
        goto end;
    }

    //шифрование ключа обмена на сессионном ключе - собственно экспорт
    if (!CryptExportKey(hSessionKey, hExchKey, SIMPLEBLOB, 0, pbData, &cbData))
    {
        DebugErrorFL("CryptExportKey");
        goto end;
    }

    if (!get_file_data_pointer(infile_name, &uiIn, &pbOut))
    {
        DebugErrorFL("get_file_data_pointer");
        goto end;
    }

    if ((pbIn = (BYTE*)malloc(uiIn)) == NULL)
    {
        DebugErrorFL("malloc");
        release_file_data_pointer(pbOut);
        pbOut = NULL;
        goto end;
    }

    memcpy(pbIn, pbOut, uiIn);
    release_file_data_pointer(pbOut);
    pbOut = NULL;
    uiOut = uiIn;

    //определение вектора инициализации сессионного ключа - размер вектора
    if (!CryptGetKeyParam(hSessionKey, KP_IV, NULL, &dwIV, 0))
    {
        DebugErrorFL("CryptGetKeyParam");
        goto end;
    }

    if ((pbIV = (BYTE*)malloc(dwIV)) == NULL)
    {
        DebugErrorFL("malloc");
        goto end;
    }

    //определение вектора инициализации сессионного ключа - собственно определение
    if (!CryptGetKeyParam(hSessionKey, KP_IV, pbIV, &dwIV, 0))
    {
        DebugErrorFL("CryptGetKeyParam");
        goto end;
    }
    
    //шифрование данных на сессионном ключе
    if (!CryptEncrypt(hSessionKey, 0, TRUE, 0, pbIn, (DWORD*)&uiOut, uiIn))
    {
        DebugErrorFL("CryptEncrypt");
        goto end;
    }

    //запись в файл зашифрованного сессионного ключа, вектора инициализации,
    //их длин и собственно зашифрованных данных
    pbOut = (BYTE*)malloc(sizeof(cbData) + sizeof(dwIV) + dwIV + cbData + uiOut);
    memcpy(pbOut, &cbData, sizeof(cbData));
    memcpy(pbOut + sizeof(cbData), &dwIV, sizeof(dwIV));
    memcpy(pbOut + sizeof(cbData) + sizeof(dwIV), pbIV, dwIV);
    memcpy(pbOut + sizeof(cbData) + sizeof(dwIV) + dwIV, pbData, cbData);
    memcpy(pbOut + sizeof(cbData) + sizeof(dwIV) + dwIV + cbData, pbIn, uiOut);

    if (!write_file(outfile_name, sizeof(cbData) + sizeof(dwIV) + dwIV + cbData + uiOut, pbOut))
    {
        DebugErrorFL("write_file");
        goto end;
    }

end:
    if(hProv != 0)
        CryptReleaseContext(hProv, 0);
    if(hUserKey != 0)
        CryptDestroyKey(hUserKey);
    if(hPublicKey != 0)
        CryptDestroyKey(hPublicKey);
    if(hExchKey != 0)
        CryptDestroyKey(hExchKey);
    if(hSessionKey != 0)
        CryptDestroyKey(hSessionKey);
    if(pMyCert != NULL)
        CertFreeCertificateContext(pMyCert);
    if(pRcpCert != NULL)
        CertFreeCertificateContext(pRcpCert);
    if(pbData != NULL)
        free(pbData);
    if(pbIn != NULL)
        free(pbIn);
    if(pbOut != NULL)
        free(pbOut);
    if(pbIV != NULL)
        free(pbIV);

    ret = GetLastError();

    return GetLastError();
}   

int export_decrypt(LPSTR mycert_name, LPSTR rcpcert_name, LPSTR infile_name, LPSTR outfile_name)
{
    HCRYPTPROV hProv = 0;
    HCRYPTKEY  hUserKey = 0;
    HCRYPTKEY  hPublicKey = 0;
    HCRYPTKEY  hExchKey = 0;
    HCRYPTKEY  hSessionKey = 0;
    PCCERT_CONTEXT pMyCert = 0;
    PCCERT_CONTEXT pRcpCert = 0;
    DWORD dwKeytype;
    BOOL bRelease;
    BYTE *pbData = NULL;
    DWORD cbData;
    BYTE *pbIn = NULL;
    BYTE *pbOut = NULL;
    BYTE *pbIV = NULL;
    DWORD dwIV = 0;
    UINT uiIn = 0;
    UINT uiOut = 0;
    int ret = 0;

    //чтение сертификата получателя из хранилища
    if((pMyCert = read_cert_from_my(mycert_name)) == NULL)
    {
        DebugErrorFL("read_cert_from_my");
        goto end;
    }

    //определения соответствия алгоритма получателя ГОСТу
    if(strcmp(pMyCert->pCertInfo->SignatureAlgorithm.pszObjId , szOID_CP_GOST_R3411_R3410))
    {
        DebugErrorFL("Wrong algorithm");
        goto end;
    }

    //чтение сертификата отправителя из хранилища
    if((pRcpCert = read_cert_from_my(rcpcert_name)) == NULL)
    {
        DebugErrorFL("read_cert_from_my");
        goto end;
    }

    //определение соответствия алгоритма отправителя ГОСТу
    if(strcmp(pRcpCert->pCertInfo->SignatureAlgorithm.pszObjId , szOID_CP_GOST_R3411_R3410))
    {
        DebugErrorFL("Wrong algorithm");
        goto end;
    }

    //инициализируем провайдер, использую свой сертификат с секретным ключом
    if(!CryptAcquireProvider("MY", pMyCert, &hProv, &dwKeytype, &bRelease))
    {
        DebugErrorFL("CryptAcquireProvider");
        goto end;
    }

    //проверка пригодности ключа для шифрования данных
    if (! (dwKeytype & AT_KEYEXCHANGE))
    {
        DebugErrorFL("Wrong key type");
        goto end;
    }

    //получение секретного ключа получателя
    if (!CryptGetUserKey(hProv, AT_KEYEXCHANGE, &hUserKey))
    {
        DebugErrorFL("CryptGetUserKey");
        goto end;
    }

    //импорт открытого ключа отправителя из сертификата, где он лежит в ASN 1.1
    if (!CryptImportPublicKeyInfoEx(hProv, X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, &pRcpCert->pCertInfo->SubjectPublicKeyInfo, CALG_GR3410, 0, NULL, &hPublicKey))
    {
        DebugErrorFL("CryptImportPublicKey");
        goto end;
    }

    //экспорт открытого ключа отправителя в BLOB - определение размера требуемого обьема памяти
    if (!CryptExportKey(hPublicKey, 0, PUBLICKEYBLOB, 0, NULL, &cbData))
    {
        DebugErrorFL("CryptExportKey");
        goto end;
    }

    if ((pbData = (BYTE*)malloc(cbData)) == NULL)
    {
        DebugErrorFL("malloc");
        goto end;
    }

    //экспорт открытого ключа отправителя в BLOB - собственно экспорт
    if (!CryptExportKey(hPublicKey, 0, PUBLICKEYBLOB, 0, pbData, &cbData))
    {
        DebugErrorFL("CryptExportKey");
        goto end;
    }

    //получение ключа обмена импортом секретного ключа получателя
    //на открытый ключ отправителя
    if (!CryptImportKey(hProv, pbData, cbData, hUserKey, 0, &hExchKey))
    {
        DebugErrorFL("CryptImportKey");
        goto end;
    }

    free(pbData);
    pbData = NULL;

    if (!get_file_data_pointer(infile_name, &uiIn, &pbIn))
    {
        DebugErrorFL("get_file_data_pointer");
        goto end;
    }

    //чтение размера зашифрованного сессионного ключа из файла
    cbData = *((DWORD*)pbIn);
    //чтение размера вектора инициализации из файла
    dwIV = *((DWORD*)(pbIn + sizeof(cbData)));

    if ((pbIV = (BYTE*)malloc(dwIV)) == NULL)
    {
        DebugErrorFL("malloc");
        goto end;
    }

    //чтение вектора инициализации из файла
    memcpy(pbIV, pbIn + sizeof(cbData) + sizeof(dwIV), dwIV);

    if ((pbData = (BYTE*)malloc(cbData)) == NULL)
    {
        DebugErrorFL("malloc");
        goto end;
    }

    //чтение зашифрованного сессионного ключа из файла
    memcpy(pbData, pbIn + sizeof(cbData) + sizeof(dwIV) + dwIV, cbData);

    //расшифровка сессионного ключа на ключе обмена
    if (!CryptImportKey(hProv, pbData, cbData, hExchKey, 0, &hSessionKey))
    {
        DebugErrorFL("CryptImportKey");
        goto end;
    }

    uiOut = uiIn - sizeof(cbData) - sizeof(dwIV) - dwIV - cbData;

    if ((pbOut = (BYTE*)malloc(uiOut)) == NULL)
    {
        DebugErrorFL("malloc");
        goto end;
    }

    memcpy(pbOut, pbIn + uiIn - uiOut, uiOut);

    pbIn = NULL;

    //установка вектора инициализации - без него первые 8 байт расшифруются неправильно
    if (!CryptSetKeyParam(hSessionKey, KP_IV, pbIV, 0))
    {
        DebugErrorFL("CryptSetKeyParam");
        goto end;
    }

    //расшифровка данных на сессионном ключе
    if (!CryptDecrypt(hSessionKey, 0, TRUE, 0, pbOut, (DWORD*)&uiOut))
    {
        DebugErrorFL("CryptDecrypt");
        goto end;
    }

    //запись расшифрованных данных
    if (!write_file(outfile_name, uiOut, pbOut))
    {
        DebugErrorFL("write_file");
        goto end;
    }

    free(pbData);
    pbData = NULL;

end:
    if(hProv != 0)
        CryptReleaseContext(hProv, 0);
    if(hUserKey != 0)
        CryptDestroyKey(hUserKey);
    if(hPublicKey != 0)
        CryptDestroyKey(hPublicKey);
    if(hExchKey != 0)
        CryptDestroyKey(hExchKey);
    if(hSessionKey != 0)
        CryptDestroyKey(hSessionKey);
    if(pMyCert != NULL)
        CertFreeCertificateContext(pMyCert);
    if(pRcpCert != NULL)
        CertFreeCertificateContext(pRcpCert);
    if(pbData != NULL)
        free(pbData);
    if(pbIn != NULL)
        release_file_data_pointer(pbIn);
    if(pbOut != NULL)
        free(pbOut);
    if(pbIV != NULL)
        free(pbIV);

    return ret;

}
/* end of file: $Id: export.c,v 1.6 2001/12/25 15:57:52 pre Exp $ */
