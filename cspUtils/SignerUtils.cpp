#include "stdafx.h"
#include "SignerUtils.h"
#include "cspUtilsIO.h"
#include "MyStrMarshal.h"
#include "cades.h"
#include "WinCryptEx.h"// Интерфейс КриптоПро CSP, добавление к WinCrypt.h
#include <vector>


using namespace System::IO;


namespace SignerUtils {

	namespace wincrypt {
		//Подпись файла через Wincrypt
		int  SignFileWinCrypt(System::String^ FileToSign, PCCERT_CONTEXT CertToSign)
		{
			const int detached = 1;
			LPVOID	    mem_tbs = NULL;
			size_t	    mem_len = 0;
			DWORD		signed_len = 0;
			BYTE* signed_mem = NULL;  // Буффер с подписью
			LPCSTR FileName = (LPCSTR)StringtoChar(FileToSign);
			char* OutFileName = StringtoChar(Path::GetFileName(FileToSign) + ".sig");
			CRYPT_SIGN_MESSAGE_PARA param;
			int retFile = cspUtils::IO::read_file(FileName, &mem_len, &mem_tbs);
			if (retFile = 0) goto err;

			/* Установим параметры*/
			/* Обязательно нужно обнулить все поля структуры. */
			/* Иначе это может привести к access violation в функциях CryptoAPI*/
			/* В примере из MSDN это отсутствует*/
			memset(&param, 0, sizeof(CRYPT_SIGN_MESSAGE_PARA));
			param.cbSize = sizeof(CRYPT_SIGN_MESSAGE_PARA);
			param.dwMsgEncodingType = TYPE_DER; //X509_ASN_ENCODING | PKCS_7_ASN_ENCODING; // TYPE_DER;
			param.pSigningCert = CertToSign;//
			param.HashAlgorithm.pszObjId = szOID_CP_GOST_R3410_12_256;// works fine - szOID_CP_DH_12_256;
			param.HashAlgorithm.Parameters.cbData = 0;
			param.HashAlgorithm.Parameters.pbData = NULL;
			param.pvHashAuxInfo = NULL;	/* не используется*/
			param.cMsgCert = 1;		/* 0 -не включаем сертификат отправителя*/ /*If set to zero no certificates are included in the signed message*/
			param.rgpMsgCert = &CertToSign; // NULL;
			param.cAuthAttr = 0;
			param.dwInnerContentType = 0;
			param.cMsgCrl = 0;  // Cписки отзыва
			param.cUnauthAttr = 0;

			param.dwFlags = 0; //

			DWORD		MessageSizeArray[1];
			const BYTE* MessageArray[1];

			MessageArray[0] = (BYTE*)mem_tbs;// file body here in [0]
			MessageSizeArray[0] = mem_len;

			CADES_SIGN_MESSAGE_PARA para = { sizeof(para) };
			para.pSignMessagePara = &param;

			PCRYPT_DATA_BLOB pSignedMessage = 0;
			/* First call - just calculate size CMS */
			int ret = CryptSignMessage(&param, detached, 1, MessageArray, MessageSizeArray, NULL, &signed_len);

			if (ret)
			{
				// printf("Calculated signature (or signed message) length: %lu\n", signed_len);
				signed_mem = (BYTE*)malloc(signed_len);
				if (!signed_mem)
					goto err;
			}
			else
			{
				return 25;//HandleErrorFL("Signature creation error");
			}
			/*--------------------------------------------------------------------*/
			/* Second call - build CMS*/
			ret = CryptSignMessage(&param, detached, 1, MessageArray, MessageSizeArray,
				signed_mem,
				&signed_len);
			if (ret) {
				printf("Signature was done. Signature (or signed message) length: %lu\n", signed_len);
			}
			else
			{
				return 26;//HandleErrorFL("Signature creation error");
			}

			if (cspUtils::IO::write_file(OutFileName, signed_len, signed_mem)) {
				// printf ("Output file (%s) has been saved\n", outfile);
			}
			return 1; // norm. all ok
		err:
			if (signed_mem) free(signed_mem);
			//    release_file_data_pointer (mem_tbs);
		}


		
		//  Результат - сертификат типа PCCERT_CONTEXT
		//  Функция чтения сертификата из системного справочника пользователя'MY'
		PCCERT_CONTEXT GetCertificat(System::String^ SubjectName)
		{
			if (!SubjectName) return NULL;

			PCCERT_CONTEXT  ret = NULL;
			HANDLE	    hCertStore = 0;

			/*   //для случая поиска CERT_FIND_SUBJECT_NAME:
			PCERT_NAME_BLOB FindName =(PCERT_NAME_BLOB) malloc(sizeof(CERT_NAME_BLOB));
			FindName->cbData =sizeof(StringtoChar(CertName));
			FindName->pbData =(BYTE *) (StringtoChar(CertName));
			*/
			LPCSTR lpszCertSubject = (LPCSTR)StringtoChar(SubjectName); //для случая поиска CERT_FIND_SUBJECT_STR

			hCertStore = CertOpenStore(
				CERT_STORE_PROV_SYSTEM, /* LPCSTR lpszStoreProvider*/
				0,			    /* DWORD dwMsgAndCertEncodingType*/
				0,			    /* HCRYPTPROV hCryptProv*/
				CERT_SYSTEM_STORE_CURRENT_USER, /* DWORD dwFlags*/
				L"MY"		    /* const void *pvPara*/
			);

			ret = CertFindCertificateInStore(hCertStore,
				TYPE_DER,
				0,
				CERT_FIND_SUBJECT_STR_A, // Искать как Unicode?
				lpszCertSubject,
				NULL);
			return ret;
		}

		LPTSTR GetCertIssuerName(PCCERT_CONTEXT Certificat)
		{
			DWORD cbSize;
			LPTSTR pszName;
			if (!(cbSize = CertGetNameString(
				Certificat,
				CERT_NAME_SIMPLE_DISPLAY_TYPE,
				CERT_NAME_ISSUER_FLAG,
				NULL,
				NULL,
				0)))
			{
				return (LPTSTR)"CertGetName 1 failed.";
			}

			if (!(pszName = (LPTSTR)malloc(cbSize * sizeof(TCHAR))))
			{
				return (LPTSTR)"Memory allocation failed.";
			}

			if (CertGetNameString(
				Certificat,
				CERT_NAME_SIMPLE_DISPLAY_TYPE,
				CERT_NAME_ISSUER_FLAG,
				NULL,
				pszName,
				cbSize))
			{
				return  pszName;

				//-------------------------------------------------------
				//       Free the memory allocated for the string.
				free(pszName);
			}
			else
			{
				return (LPTSTR)"CertGetName failed.";
			}
		}

		// GetCertEmail - Get and display the e-mail of Issuer of the certificate.
		LPTSTR GetCertEmail(PCCERT_CONTEXT Certificat)
		{
			DWORD cbSize;
			LPTSTR pszName;
			if (!(cbSize = CertGetNameString(
				Certificat,
				CERT_NAME_EMAIL_TYPE,
				0,
				NULL,
				NULL,
				0)))
			{
				return (LPTSTR)"CertGetName 1 failed.";
			}
			if (cbSize <= 1) { //Bad e-mail
				return (LPTSTR)"-";
			}
			if (!(pszName = (LPTSTR)malloc(cbSize * sizeof(TCHAR))))
			{
				return (LPTSTR)"Memory allocation failed.";
			}

			if (CertGetNameString(
				Certificat,
				CERT_NAME_EMAIL_TYPE,
				0,
				NULL,
				pszName,
				cbSize))
			{
				return  pszName;

				//-------------------------------------------------------
				//       Free the memory allocated for the string.
				free(pszName);
			}
			else
			{

				return (LPTSTR)"CertGetName failed.";
			}
		}

		// GetCertEmail - Get and display the e-mail of Issuer of the certificate.
		LPTSTR GetSubjectInfo(PCCERT_CONTEXT Certificat) {
			return (LPTSTR)".";
		}


		// usage
		//     CHAR msgText[256];
		//     getLastErrorText(msgText,sizeof(msgText));
		CHAR* GetLastErrorText(                  // converts "Lasr Error" code into text
			CHAR* pBuf,                        //   message buffer
			ULONG bufSize)                     //   buffer size
		{
			DWORD retSize;
			LPTSTR pTemp = NULL;

			if (bufSize < 16) {
				if (bufSize > 0) {
					pBuf[0] = '\0';
				}
				return(pBuf);
			}
			retSize = FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER |
				FORMAT_MESSAGE_FROM_SYSTEM |
				FORMAT_MESSAGE_ARGUMENT_ARRAY,
				NULL,
				GetLastError(),
				LANG_NEUTRAL,
				(LPTSTR)& pTemp,
				0,
				NULL);

			if (!retSize || pTemp == NULL) {
				pBuf[0] = '\0';
			}
			else {
				// pTemp[strlen(pTemp)-2]='\0'; //remove cr and newline character
				sprintf(pBuf, "%0.*s (0x%x)", bufSize - 16, pTemp, GetLastError());
				LocalFree((HLOCAL)pTemp);
			}
			return(pBuf);
		}
	}


	namespace cades {
		//Поиск описания ошибки CADES по её коду

		LPTSTR Error2Message(DWORD dwErr)
		{
			//DWORD dwErr = GetLastError();
			wchar_t buf[1024];   // zero terminated string
			//LPTSTR buf = (LPTSTR)malloc(1024);
			DWORD dwFlagsMod = FORMAT_MESSAGE_IGNORE_INSERTS | FORMAT_MESSAGE_FROM_HMODULE;
			BOOL dwRet = CadesFormatMessage(dwFlagsMod, 0, dwErr, 0, buf, sizeof(buf), NULL);

			DWORD dwFlagsSys = FORMAT_MESSAGE_IGNORE_INSERTS | FORMAT_MESSAGE_FROM_SYSTEM;
			if (!dwRet)
			{
				dwRet = FormatMessage(dwFlagsSys, 0, dwErr, 0, buf, sizeof(buf), NULL);
			}
			if (dwRet)
				return (LPTSTR)buf;
			//std::cout << buf << std::endl;
		}


	}


	namespace examples {

		/**************************************************************************************
		/* Создание подписи CRypto Pro (упрощённые функции):
		 **************************************************************************************/
		 // Из примера Cpdn: подпись строки цифр - тест провайдера
		int  SignCadesExample(PCCERT_CONTEXT CertToSign)
		{
			CRYPT_SIGN_MESSAGE_PARA signpara = { sizeof(signpara) };
			signpara.dwMsgEncodingType = X509_ASN_ENCODING | PKCS_7_ASN_ENCODING;
			signpara.pSigningCert = CertToSign; // 0 for window
			signpara.HashAlgorithm.pszObjId = szOID_OIWSEC_sha1;  ///szOID_GostR3411_94_CryptoProParamSet; 

			CADES_SIGN_MESSAGE_PARA para = { sizeof(para) };
			para.pSignMessagePara = &signpara;

			std::vector<BYTE> data(10, 25);
			const BYTE* pbToBeSigned[] = { &data[0] };			 // буффер
			DWORD rgcbToBeSigned[] = { (DWORD)data.size() };

			PCRYPT_DATA_BLOB pSignedMessage = 0; // зануляем.....
			if (!CadesSignMessage(&para, FALSE, 1, pbToBeSigned, rgcbToBeSigned, &pSignedMessage))
			{
				int err = GetLastError();
				LPTSTR errMeesage = SignerUtils::cades::Error2Message(err);
				return err;

			}
			DWORD res = pSignedMessage->cbData;
			CadesFreeBlob(pSignedMessage);
			return 0;
		}

		int  SignCadesFile(System::String^ FileToSign, PCCERT_CONTEXT CertToSign)
		{
			//	char* infile = StringtoChar(Path::GetFileName(FileToSign));         


				//return -1; // trap
			const int detached = 1;
			char  OID[64] = "1.2.643.2.2.9";  //szOID_CP_GOST_R3411;  :  WinCryptEx.h
			LPVOID	    mem_tbs = NULL;
			size_t	    mem_len = 0;
			DWORD		signed_len = 0;
			BYTE* signed_mem = NULL;  // Буффер с подписью
			//LPCWSTR FileName =L"c:\\TEMP\\work.txt";// *StringtoChar(FileToSign);
			LPCSTR FileName = (LPCSTR)StringtoChar(FileToSign);
			char* OutFileName = StringtoChar(Path::GetFileName(FileToSign) + ".sig");



			int retFile = cspUtils::IO::read_file(FileName, &mem_len, &mem_tbs);
			if (retFile = 0) goto err;

			/* Установим параметры*/
			CRYPT_SIGN_MESSAGE_PARA param = { sizeof(param) };
			/* Обязательно нужно обнулить все поля структуры. */
			/* Иначе это может привести к access violation в функциях CryptoAPI*/
			/* В примере из MSDN это отсутствует*/
			memset(&param, 0, sizeof(CRYPT_SIGN_MESSAGE_PARA));
			param.cbSize = sizeof(CRYPT_SIGN_MESSAGE_PARA);
			param.dwMsgEncodingType = X509_ASN_ENCODING | PKCS_7_ASN_ENCODING; // TYPE_DER;
			param.pSigningCert = CertToSign;// Выберем сертификат!! //pUserCert;
			//char *OID;
					//char    AlgOID[64] = szOID_CP_GOST_R3411; //CRYPT_HASH_ALG_OID_ Алгоритм функции хэширования по ГОСТ Р 34.11-94 установлен по умоланию
					//char    AlgOID[64] = szOID_RSA_SHA512RSA;
					 //#define szOID_RSA_SHA512RSA     "1.2.840.113549.1.1.13"
			param.HashAlgorithm.pszObjId = szOID_OIWSEC_sha1;
			param.HashAlgorithm.Parameters.cbData = 0;
			param.HashAlgorithm.Parameters.pbData = NULL;
			param.pvHashAuxInfo = NULL;	/* не используется*/
			param.cMsgCert = 1;		/* 0 -не включаем сертификат отправителя*/ /*If set to zero no certificates are included in the signed message*/
			param.rgpMsgCert = &CertToSign; // NULL;
			param.cAuthAttr = 0;
			param.dwInnerContentType = 0;
			param.cMsgCrl = 0;  // Cписки отзыва
			param.cUnauthAttr = 0;
			param.dwFlags = 0; //

			DWORD		MessageSizeArray[1];
			const BYTE* MessageArray[1];

			MessageArray[0] = (BYTE*)mem_tbs;
			MessageSizeArray[0] = mem_len;

			CADES_SIGN_PARA CadesSignPara = { sizeof(CadesSignPara) };
			memset(&CadesSignPara, 0, sizeof(CADES_SIGN_PARA));
			CadesSignPara.dwSize = sizeof(CADES_SIGN_PARA);
			CadesSignPara.szHashAlgorithm = szOID_CP_GOST_R3411;
			CadesSignPara.pSignerCert = CertToSign;
			CadesSignPara.dwCadesType = CADES_BES;

			CADES_SIGN_MESSAGE_PARA CadesParams = { sizeof(CadesParams) };
			memset(&CadesParams, 0, sizeof(CADES_SIGN_MESSAGE_PARA));
			CadesParams.pSignMessagePara = &param;
			CadesParams.pCadesSignPara = &CadesSignPara;
			CadesParams.dwSize = CadesParams.pSignMessagePara->cbSize + CadesParams.pCadesSignPara->dwSize;

			PCRYPT_DATA_BLOB pSignedMessage = 0;
			/* Определение длины подписанного сообщения:
			//Данную функцию нужно вызывать только один раз(в отличие от функции CryptSignMessage */
			if (CadesSignMessage(&CadesParams, detached, 1, MessageArray, MessageSizeArray, &pSignedMessage))
				/*
				   __in BOOL fDetachedSignature,
				   __in DWORD cToBeSigned,
				   __in const BYTE* rgpbToBeSigned[],
				   __in DWORD rgcbToBeSigned[],
				   __out PCRYPT_DATA_BLOB *ppSignedBlob);
			   */

			   //if (ret) 
			{
				// printf("Calculated signature (or signed message) length: %lu\n", signed_len);
				int signed_byte_Count = pSignedMessage->cbData;
				signed_mem = (BYTE*)malloc(signed_len);
				if (!signed_mem)
					goto err;
			}
			else
			{
				//cades
				return 25;//HandleErrorFL("Signature creation error");
			}
			/*--------------------------------------------------------------------*/
			/* Формирование подписанного сообщения*/
			/* Длина оказалась 1024 */
			if (CadesSignMessage(&CadesParams, detached, 1, MessageArray, MessageSizeArray, &pSignedMessage))
				;
			// дописать запись в файл: write_file (OutFileName, signed_len, &pSignedMessage)
			else
			{
				return 26;//HandleErrorFL("Signature creation error");
			}



			return 1;
		err:
			if (signed_mem) free(signed_mem);
			//    release_file_data_pointer (mem_tbs);
		}


		/**************************************************************************************
		/* Создание подписи CRypto Pro (упрощённые функции): Пример cpdn.
		/*
		/* Cоздание подписи CAdES - BES с помощью упрощённых функций
		/* Входные данные - файл
		*/

		DWORD SignCAdES_Example_01(System::String^ FileToSign, PCCERT_CONTEXT CertToSign)
		{
			CRYPT_SIGN_MESSAGE_PARA signPara = { sizeof(signPara) };
			signPara.dwMsgEncodingType = X509_ASN_ENCODING | PKCS_7_ASN_ENCODING;
			signPara.pSigningCert = CertToSign; // 0 for window
			signPara.HashAlgorithm.pszObjId = szOID_CP_GOST_R3411_2012_256_HMAC;

			CADES_SIGN_PARA cadesSignPara = { sizeof(cadesSignPara) };
			cadesSignPara.dwCadesType = CADES_BES;

			CADES_SIGN_MESSAGE_PARA para = { sizeof(para) };
			para.pSignMessagePara = &signPara;
			para.pCadesSignPara = &cadesSignPara;

			std::vector<BYTE> data(10, 25);

			const BYTE* pbToBeSigned[] = { &data[0] };
			DWORD cbToBeSigned[] = { (DWORD)data.size() };

			PCRYPT_DATA_BLOB pSignedMessage = 0;
			if (!CadesSignMessage(&para, FALSE, 1, pbToBeSigned, cbToBeSigned, &pSignedMessage))
			{
				// std::cout << "CadesSignMessage() failed" << std::endl;
				 //DWORD Err = 		GetLastError();

				 //Получаем 2147944005,0x80070645,Warning,Win32,,,,"This action is only valid for products that are currently installed."
				return 25;
			}

			std::vector<BYTE> message(pSignedMessage->cbData);
			std::copy(pSignedMessage->pbData,
				pSignedMessage->pbData + pSignedMessage->cbData, message.begin());

			if (!CadesFreeBlob(pSignedMessage))
			{
				// std::cout << "CadesFreeBlob() failed" << std::endl;
				 //DWORD Err = 		GetLastError();
				return 26;
			}

			return 0; // ok
		}


	}
}









