#include "stdafx.h"
#include "SignerUtils.h"
#include "cspUtilsIO.h"
#include <wincrypt.h>
#include "WinCryptEx.h"// Интерфейс КриптоПро CSP, добавление к WinCrypt.h
#include <vector>
#include <string>
#include <algorithm>


//using namespace System::IO;


namespace SignerUtils {

	void string_to_wstring(const std::string& src, std::wstring& dest)
	{
		std::wstring tmp;
		tmp.resize(src.size());
		std::transform(src.begin(), src.end(), tmp.begin(), btowc);
		tmp.swap(dest);
	}

	void wstring_to_string(const std::wstring& src, std::string& dest)
	{
		std::string tmp;
		tmp.resize(src.size());
		std::transform(src.begin(), src.end(), tmp.begin(), wctob);
		tmp.swap(dest);
	}





	namespace wincrypt {
		// Sign file for detached cms with wincrypt
		int  SignFileWinCrypt(LPCSTR FileName, PCCERT_CONTEXT SignerCert)
		{
			const int detached = 1;
			LPVOID	    mem_tbs = NULL;
			size_t	    mem_len = 0;
			DWORD		signed_len = 0;
			BYTE* signed_mem = NULL;  // Буффер с подписью
			//LPCSTR FileName = (LPCSTR)StringtoChar(FileToSign);
			std::string fname(FileName);
			//char* OutFileName =  StringtoChar(Path::GetFileName(FileToSign) + ".sig");
			std::string OutFileName = fname + ".sig";
			CRYPT_SIGN_MESSAGE_PARA param;
			int retFile = cspUtils::IO::read_file(FileName, &mem_len, &mem_tbs);
			if (retFile)
			{
				DWORD		MessageSizeArray[1];
				const BYTE* MessageArray[1];

				MessageArray[0] = (BYTE*)mem_tbs;// file body here in [0]
				MessageSizeArray[0] = mem_len;


				/* Установим параметры*/
				/* Обязательно нужно обнулить все поля структуры. */
				/* Иначе это может привести к access violation в функциях CryptoAPI*/
				/* В примере из MSDN это отсутствует*/
				memset(&param, 0, sizeof(CRYPT_SIGN_MESSAGE_PARA));
				param.cbSize = sizeof(CRYPT_SIGN_MESSAGE_PARA);
				param.dwMsgEncodingType = TYPE_DER; //X509_ASN_ENCODING | PKCS_7_ASN_ENCODING; // TYPE_DER;
				param.pSigningCert = SignerCert;//
				char    OID[64] = szOID_CP_GOST_R3410_12_256;
				param.HashAlgorithm.pszObjId = OID; //szOID_CP_GOST_R3410_12_256;// works fine - szOID_CP_DH_12_256;
				param.HashAlgorithm.Parameters.cbData = 0; // NULL by docs
				param.HashAlgorithm.Parameters.pbData = NULL;
				param.cMsgCert = 1;		// 0 - no certificates are included in the signed message
				param.rgpMsgCert = &SignerCert; // NULL;
				param.cAuthAttr = 0;
				param.dwInnerContentType = 0;
				param.cMsgCrl = 0;  // Cписки отзыва
				param.cUnauthAttr = 0;
				param.dwFlags = 0; //
				param.pvHashAuxInfo = NULL;	/* не используется*/
				param.rgAuthAttr = NULL;

				/*  cades here :) ?!
				CADES_SIGN_MESSAGE_PARA para = { sizeof(para) };
				para.pSignMessagePara = &param;
				*/

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

				if (cspUtils::IO::write_file(OutFileName.c_str(), signed_len, signed_mem)) {
					// printf ("Output file (%s) has been saved\n", outfile);
				}

				//Cleanup memory ? :
				/*
				if (SignerCert)
				{
					CertFreeCertificateContext(SignerCert);
				}
				*/
				return 1; // norm. all ok
			}
		err:
			if (signed_mem) free(signed_mem);
			//    release_file_data_pointer (mem_tbs);
			return -1;
		}

		//Get certificate params, like "CRYPT_EXPORT - Allow key to be exported."
		//uses advapi32.lib -be sure to set librarian
		DWORD   GetCertParam(PCCERT_CONTEXT SignerCert)
		{
			static HCRYPTPROV hProvSender = 0;         // CryptoAPI provider handle
			DWORD dwKeySpecSender;
			HCRYPTKEY hKey;
			//obtains the private key for a certificate
			if (CryptAcquireCertificatePrivateKey(SignerCert,
				0,
				NULL,
				&hProvSender,
				&dwKeySpecSender,
				NULL))
			{ //retrieves a handle of one of a user's two public/private key
				if (CryptGetUserKey(
					hProvSender,
					dwKeySpecSender,
					&hKey))
				{
					DWORD pdwDataLen = NULL;
					static BYTE* pbData = NULL;
					if (CryptGetKeyParam(hKey, KP_PERMISSIONS, NULL, &pdwDataLen, 0))
					{
						pbData = (BYTE*)malloc(pdwDataLen); //The pbData parameter is a pointer to a DWORD value 
															//that receives the permission flags for the key
						//retrieves data that governs the operations of a key
						if (CryptGetKeyParam(hKey, KP_PERMISSIONS, pbData, &pdwDataLen, 0))
						{
							DWORD Permissions_Flags = 0;
							memcpy(&Permissions_Flags, &pbData[0], pdwDataLen);

							for (DWORD i = 0; i < pdwDataLen; i++)
							{
								BYTE item = pbData[i];
							}
							return Permissions_Flags;
						}
						free(pbData);
					}
					if (hKey != 0) CryptDestroyKey(hKey);
					if (hProvSender)
					{
						CryptReleaseContext(hProvSender, 0);
						hProvSender = 0;
					}
				}
				return NULL;
			}
			return NULL;
		}

		DWORD GetCertALGID(PCCERT_CONTEXT SignerCert)
		{
			static HCRYPTPROV hProvSender = 0;         // CryptoAPI provider handle
			DWORD dwKeySpecSender;
			HCRYPTKEY hKey;
			//obtains the private key for a certificate
			if (CryptAcquireCertificatePrivateKey(SignerCert,
				0,
				NULL,
				&hProvSender,
				&dwKeySpecSender,
				NULL))
			{ //retrieves a handle of one of a user's two public/private key
				if (CryptGetUserKey(
					hProvSender,
					dwKeySpecSender,
					&hKey))
				{
					DWORD pdwDataLen = NULL;
					static BYTE* pbData = NULL;
					if (CryptGetKeyParam(hKey, KP_ALGID, NULL, &pdwDataLen, 0))
					{
						pbData = (BYTE*)malloc(pdwDataLen); //The pbData parameter is a pointer to a DWORD value 
															//that receives the permission flags for the key
						//retrieves data that governs the operations of a key
						if (CryptGetKeyParam(hKey, KP_ALGID, pbData, &pdwDataLen, 0))
						{
							DWORD Permissions_Flags = 0;
							memcpy(&Permissions_Flags, &pbData[0], pdwDataLen);
							return Permissions_Flags;
						}
						free(pbData);
					}
					if (hKey != 0) CryptDestroyKey(hKey);
					if (hProvSender)
					{
						CryptReleaseContext(hProvSender, 0);
						hProvSender = 0;
					}
				}
				return NULL;
			}
			return NULL;
		}

		std::vector<CSPItem>  EnumProvidersTypes()
		{
			DWORD       dwType;
			DWORD       cbName;
			DWORD       dwIndex = 0;
	
			static LPTSTR      pszName = NULL;
			std::vector<SignerUtils::CSPItem> ProvidersTypes;

			dwIndex = 0;
			while (CryptEnumProviderTypes(
				dwIndex,     // in -- dwIndex
				NULL,        // in -- pdwReserved- устанавливается в NULL
				0,           // in -- dwFlags -- устанавливается в ноль
				&dwType,     // out -- pdwProvType
				NULL,        // out -- pszProvName -- NULL при первом вызове
				&cbName      // in, out -- pcbProvName
			))
			{
				//  cbName - длина имени следующего типа провайдера.
				//  Распределение памяти в буфере для восстановления этого имени.
				pszName = (LPTSTR)malloc(cbName);
				if (!pszName)
					//HandleError("ERROR - malloc failed!");

					memset(pszName, 0, cbName);

				//--------------------------------------------------------------------
				//  Получение имени типа провайдера.

				if (CryptEnumProviderTypes(
					dwIndex++,
					NULL,
					0,
					&dwType,
					pszName,
					&cbName))
				{
					std::string std_sName;
					SignerUtils::wstring_to_string(pszName, std_sName);
					CSPItem  ProvTypeItem = { dwType, std_sName };
					ProvidersTypes.push_back(ProvTypeItem);
				}
				else
				{
					//	HandleError("ERROR - CryptEnumProviders");
				}
			}
			return ProvidersTypes;
		}

		std::vector<std::string>  EnumAllProviders()
		{
			DWORD       dwType;
			DWORD       cbName;
			DWORD       dwIndex = 0;
			BYTE* ptr = NULL;
			ALG_ID      aiAlgid;
			DWORD       dwBits;
			DWORD       dwNameLen;
			CHAR        szName[1024];//[NAME_LENGTH]; // Распределены динамически
			BYTE        pbData[1024];// Распределены динамически
			DWORD       cbData = 1024;
			DWORD       dwIncrement = sizeof(DWORD);
			DWORD       dwFlags = CRYPT_FIRST;
			CHAR* pszAlgType = NULL;
			static LPTSTR      pszName = NULL;
			BOOL        fMore = TRUE;
			DWORD       cbProvName;
			std::vector<std::string> Providers;

		
			dwIndex = 0;
			while (CryptEnumProviders(
				dwIndex,     // in -- dwIndex
				NULL,        // in -- pdwReserved- устанавливается в NULL
				0,           // in -- dwFlags -- устанавливается в ноль
				&dwType,     // out -- pdwProvType
				NULL,        // out -- pszProvName -- NULL при первом вызове
				&cbName      // in, out -- pcbProvName
			))
			{
				//  cbName - длина имени следующего провайдера.
				//  Распределение памяти в буфере для восстановления этого имени.
				pszName = (LPTSTR)malloc(cbName);
				if (!pszName)
					//	HandleError("ERROR - malloc failed!");

					memset(pszName, 0, cbName);

				//  Получение имени провайдера.
				if (CryptEnumProviders(
					dwIndex++,
					NULL,
					0,
					&dwType,
					pszName,
					&cbName     // pcbProvName -- длина pszName
				))
				{
					std::string std_sName;
					SignerUtils::wstring_to_string(pszName, std_sName);
					Providers.push_back(std_sName);
				}
				else
				{
					//	HandleError("ERROR - CryptEnumProviders");
				}

			} 
			return Providers;
		}

		std::vector<std::string> EnumAllContainers()
		{
			std::vector<std::string> Containers;
			HCRYPTPROV hProv;
			if (CryptAcquireContext(&hProv, NULL, NULL, 81, 0)) //CRYPT_SILENT
			{
				BYTE* pbData;
				DWORD pbdwDataLen = 0;
				char* container_name;
				DWORD base_flags = 0;
				if (CryptGetProvParam(hProv, PP_ENUMCONTAINERS, NULL, &pbdwDataLen, 0))
				{
					container_name = (char*)malloc(pbdwDataLen);
					while (CryptGetProvParam(hProv, PP_ENUMCONTAINERS, (BYTE*)container_name, &pbdwDataLen, 0))
					{
						if (base_flags & CRYPT_UNIQUE)
						{

						}
					}
				}
				free(container_name);
				CryptReleaseContext(hProv, 0);
			}
			
			return Containers;
		}

		PCCERT_CONTEXT GetCert(PCCERT_CONTEXT SignerCert)
		{
			static HCRYPTPROV hProvSender = 0;         // CryptoAPI provider handle
			DWORD dwKeySpecSender;
			HCRYPTKEY hKey;
			//obtains the private key for a certificate
			if (CryptAcquireCertificatePrivateKey(SignerCert,
				0,
				NULL,
				&hProvSender,
				&dwKeySpecSender,
				NULL))
			{ //retrieves a handle of one of a user's two public/private key
				if (CryptGetUserKey(
					hProvSender,
					dwKeySpecSender,
					&hKey))
				{
					DWORD pdwDataLen = NULL;
					static BYTE* pbCert = NULL;
					PCCERT_CONTEXT pCertContext = NULL;
					if (CryptGetKeyParam(hKey, KP_CERTIFICATE, NULL, &pdwDataLen, 0))
					{
						pbCert = (BYTE*)malloc(pdwDataLen);
						if (CryptGetKeyParam(hKey, KP_CERTIFICATE, pbCert, &pdwDataLen, 0))
						{
							pCertContext = CertCreateCertificateContext(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, pbCert, pdwDataLen);
							return pCertContext;
						}
						free(pbCert);
					}
					if (hKey != 0) CryptDestroyKey(hKey);
					if (hProvSender)
					{
						CryptReleaseContext(hProvSender, 0);
						hProvSender = 0;
					}
				}
				return NULL;
			}
			return NULL;
		}

		PCCERT_CONTEXT GetCertificat(LPCSTR lpszCertSubject)
		{
			if (!lpszCertSubject) return NULL;

			PCCERT_CONTEXT  ret = NULL;
			HANDLE	    hCertStore = 0;

			/*   //для случая поиска CERT_FIND_SUBJECT_NAME:
			PCERT_NAME_BLOB FindName =(PCERT_NAME_BLOB) malloc(sizeof(CERT_NAME_BLOB));
			FindName->cbData =sizeof(StringtoChar(CertName));
			FindName->pbData =(BYTE *) (StringtoChar(CertName));
			*/
			//LPCSTR lpszCertSubject = SubjectName.c_str();  //(LPCSTR)StringtoChar(SubjectName); //для случая поиска CERT_FIND_SUBJECT_STR

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
		/*
		// GetCertEmail - Get and display the e-mail of Issuer of the certificate.
		LPTSTR GetCertDateExp(PCCERT_CONTEXT Certificat)
		{
			DWORD cbSize;
			LPTSTR pszName;
			 LONG testTimeValiduty = CertVerifyTimeValidity(NULL, Certificat->pCertInfo);

			FILETIME timeAfter = Certificat->pCertInfo->NotAfter;
			switch (CertVerifyTimeValidity(
				NULL,               // Use current time.
				Certificat->pCertInfo))   // Pointer to CERT_INFO.
			{
			case -1:
			{
				return L"Certificate is not valid yet. \n";
				break;
			}
			case 1:
			{
				return L"Certificate is expired. \n";
				break;
			}
			case 0:
			{
				return L"Certificate's time is valid. \n";
				break;
			}
			};


		}
*/

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
				//std::cout << "Getlasterror" << std::end;//sprintf(pBuf, "%0.*s (0x%x)", bufSize - 16, pTemp, GetLastError());
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

#ifdef RememberExamples
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

		*/
			/**************************************************************************************
			/* Создание подписи CRypto Pro (упрощённые функции): Пример cpdn.
			/*
			/* Cоздание подписи CAdES - BES с помощью упрощённых функций
			/* Входные данные - файл
			*/
			*/
			DWORD SignCAdES_Example_01(System::String ^ FileToSign, PCCERT_CONTEXT CertToSign)
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
#endif // RememberExamples



	namespace CNG //CNG : bcrypt.h => Bcrypt.lib
	{

		void EnumerateKeys()
		{
			NTSTATUS                status = STATUS_UNSUCCESSFUL;
			SECURITY_STATUS			secstatus;
			BCRYPT_ALG_HANDLE       hAlg = NULL;
			NCRYPT_PROV_HANDLE phProvider;
			void* ppEnumState = NULL;
			std::vector<std::string> Keys;
			//=============================================
			//Opening the Algorithm Provider
			if (!NT_SUCCESS(status = BCryptOpenAlgorithmProvider( //loads and initializes a CNG provider
				&hAlg,
				BCRYPT_SHA256_ALGORITHM,
				NULL,
				0)))
			{
				wprintf(L"**** Error 0x%x returned by BCryptOpenAlgorithmProvider\n", status);
				goto Cleanup;
			}

			//load a key storage provider 
			//loads and initializes a CNG key storage provider
			secstatus = NCryptOpenStorageProvider( //loads and initializes a CNG provider
				&phProvider,
				MS_KEY_STORAGE_PROVIDER, // default provider
				0 //noflags
			);

			if (!NTSEC_SUCCESS(secstatus))
			{
				wprintf(L"**** Error 0x%x returned by NCryptOpenStorageProvider\n", secstatus);
				goto Cleanup;
			}

			//and then create or load the keys
			NCryptKeyName* key;

			while (secstatus != NTE_NO_MORE_ITEMS) //NTE_NO_MORE_ITEMS)
			{
				secstatus = NCryptEnumKeys(phProvider, NULL, &key, &ppEnumState, 0);
				if (secstatus == ERROR_SUCCESS)
				{
					std::string keyName;
					SignerUtils::wstring_to_string(key->pszName, keyName);
					Keys.push_back(keyName);
				}
			}

			NCryptFreeBuffer(key);
			NCryptFreeBuffer(ppEnumState);

			//=============================================
			//Getting or Setting Algorithm Properties
			//Creating or Importing a Key
			//Performing Cryptographic Operations
			//Closing the Algorithm Provider
			//=============================================

		Cleanup: // Destroyem ALL ::))

			if (hAlg)
			{
				BCryptCloseAlgorithmProvider(hAlg, 0);
			}
			if (phProvider)
			{
				NCryptFreeObject(phProvider);
			}
			/*
			if (hHash)
			{
				BCryptDestroyHash(hHash);
			}

			if (pbHashObject)
			{
				HeapFree(GetProcessHeap(), 0, pbHashObject);
			}

			if (pbHash)
			{
				HeapFree(GetProcessHeap(), 0, pbHash);
			}
			*/

		}

		std::vector<std::string> EnumerateStorageProviders()
		{
			SECURITY_STATUS			secstatus;
			std::vector<std::string> Providers;
			NCryptProviderName* ppProviderList = NULL; //array with names
			DWORD pdwProviderCount = 0;
			secstatus = NCryptEnumStorageProviders(&pdwProviderCount, &ppProviderList, 0);
			if (ppProviderList != NULL)
			{
				for (DWORD i = 0; i < pdwProviderCount; i++)
				{
					NCryptProviderName current = ppProviderList[i];
					std::string keyName;
					SignerUtils::wstring_to_string(current.pszName, keyName);
					Providers.push_back(keyName);
				}
			}
			NCryptFreeBuffer(ppProviderList);
			return Providers;
		}

	}
}