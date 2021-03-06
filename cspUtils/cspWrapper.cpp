//Fixosoft 2014
//



// This is the main DLL file.
#include "Stdafx.h"
#include "cspWrapper.h"
//#include <string>
//#include <atlstr.h> // CW2A

#include "cades.h" //CryptoPro
#include "WinCryptEx.h" // Windows Crypto distribute with Crypto Pro SDK 

#include "MyStrMarshal.h" // fixosoft String Collection Utils
#include "cspUtilsIO.h"
#include "SignerUtils.h"

namespace cspUtils {



	CadesWrapper::CadesWrapper()
	{

	}



	//Отображение подписи через CADES UI 
	int CadesWrapper::DisplaySig(System::String^ FileSign, System::IntPtr  Parent)
	{
		LPVOID	    mem_tbs = NULL;
		size_t	    mem_len = 0;
		//читаем файл 
		LPCSTR FileName = (LPCSTR)StringtoChar(FileSign);
		int retFile = IO::read_file(FileName, &mem_len, &mem_tbs);
		if (retFile = 0) goto err;

		const BYTE* SignBlob;
		SignBlob = (BYTE*)mem_tbs;


		CADES_VIEW_SIGNATURE_PARA viewPara = { sizeof(viewPara) };
		viewPara.dwMsgEncodingType = X509_ASN_ENCODING | PKCS_7_ASN_ENCODING;

		if (!CadesUIDisplaySignatures(&viewPara, SignBlob, (DWORD)mem_len, (HWND)Parent.ToInt32(), L"Отображение подписи (UI Display Cades)"))
		{
		err:
			LPCWSTR rs = L"CadesUIDisplaySignatures() failed.";
		}
		return  GetLastError();
	}



	/*
	System::String^ CadesWrapper::GetCertDateExp(PCCERT_CONTEXT CertContext)
	{
		//throw gcnew System::NotImplementedException();
		// TODO: insert return statement here
		return LPTSTRToString(SignerUtils::wincrypt::GetCertDateExp(CertContext));
	}
	*/
	/*
	System::String^ CadesWrapper::GetCertDateExpirate(PCCERT_CONTEXT Certificat)
	{
		DWORD cbSize;
		LPTSTR pszName;
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


	/*
	PCCERT_CONTEXT_CLR^ CadesWrapper::GetCertificatCLR(System::String^ SubjectName)
	{
		PCCERT_CONTEXT_CLR^ clr = gcnew PCCERT_CONTEXT_CLR();
		PCCERT_CONTEXT ccrt = SignerUtils::wincrypt::GetCertificat(SubjectName);
		clr->Item = &ccrt;
		return  clr;//PCCERT_CONTEXT_CLR();
	}

	
	PCCERT_CONTEXT_WR^ CadesWrapper::GetCertificatWrapped(System::String^ SubjectName)
	{
		PCCERT_CONTEXT_WR^ res = gcnew PCCERT_CONTEXT_WR();
		PCCERT_CONTEXT cert = SignerUtils::wincrypt::GetCertificat(SubjectName);
		res->Certificat = &cert;
		return res;
	}
	*/


	/*
	System::String^ CadesWrapper::GetCertificatSerialNumber(System::String^ SubjectName)
	{
		//throw gcnew System::NotImplementedException();
		PCCERT_CONTEXT	ret = SignerUtils::wincrypt::GetCertificat(SubjectName);
		if (ret)
		{
			PBYTE serial = ((CRYPT_INTEGER_BLOB)ret->pCertInfo->SerialNumber).pbData;
			return PBYTEToStr(serial, ((CRYPT_INTEGER_BLOB)ret->pCertInfo->SerialNumber).cbData);

		}
		else return "";
	}
	*/

	//Подпись получается без хэша
	/*
	System::Int16  CadesWrapper::SignFile(System::String^ filename, System::String^ SubjectName)
	{
		PCCERT_CONTEXT	CertToSign = SignerUtils::wincrypt::GetCertificat(SubjectName);
		if (CertToSign)
		{
			const int detached = 1;
			char  OID[64] = "1.2.643.2.2.9";  //szOID_CP_GOST_R3411;  :  WinCryptEx.h
			LPVOID	    mem_tbs = NULL;
			size_t	    mem_len = 0;
			DWORD		signed_len = 0;
			BYTE* signed_mem = NULL;  // Буффер с подписью

			LPCSTR FileName = (LPCSTR)StringtoChar(filename);
			char* OutFileName = StringtoChar(filename + ".sig");
			CRYPT_SIGN_MESSAGE_PARA param;
			int retFile = IO::read_file(FileName, &mem_len, &mem_tbs);
			if (retFile = 0) goto err;

			/* Установим параметры*/
			/* Обязательно нужно обнулить все поля структуры. */
			/* Иначе это может привести к access violation в функциях CryptoAPI*/
			/* В примере из MSDN это отсутствует
			memset(&param, 0, sizeof(CRYPT_SIGN_MESSAGE_PARA));
			param.cbSize = sizeof(CRYPT_SIGN_MESSAGE_PARA);
			param.dwMsgEncodingType = TYPE_DER; //X509_ASN_ENCODING | PKCS_7_ASN_ENCODING; // TYPE_DER;
			param.pSigningCert = CertToSign;// Выберем сертификат!! //pUserCert;
											//char *OID;
			char    AlgOID[64] = szOID_CP_GOST_R3411; //CRYPT_HASH_ALG_OID_ Алгоритм функции хэширования по ГОСТ Р 34.11-94 установлен по умоланию
													  //char    AlgOID[64] = szOID_RSA_SHA512RSA; //CRYPT_HASH_ALG_OID_ Алгоритм функции хэширования по ГОСТ Р 34.11-94 установлен по умоланию
													  //#define szOID_RSA_SHA512RSA     "1.2.840.113549.1.1.13"
			param.HashAlgorithm.pszObjId = AlgOID;
			param.HashAlgorithm.Parameters.cbData = 0;
			param.HashAlgorithm.Parameters.pbData = NULL;
			param.pvHashAuxInfo = NULL;	/* не используется
			param.cMsgCert = 1;		/* 0 -не включаем сертификат отправителя.If set to zero no certificates are included in the signed message/
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

			CADES_SIGN_MESSAGE_PARA para = { sizeof(para) };
			para.pSignMessagePara = &param;

			PCRYPT_DATA_BLOB pSignedMessage = 0;
			//* Определение длины подписанного сообщения
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
			/* Формирование подписанного сообщения
			// Длина оказалась 1024
			ret = CryptSignMessage(&param, detached, 1, MessageArray, MessageSizeArray,
				signed_mem,
				&signed_len);
			if (ret) {
				// printf("Signature was done. Signature (or signed message) length: %lu\n", signed_len);
			}
			else
			{
				return 26;//HandleErrorFL("Signature creation error");
			}

			if (IO::write_file(OutFileName, signed_len, signed_mem)) {
				// printf ("Output file (%s) has been saved\n", outfile);
			}
			return 0;
		err:
			if (signed_mem) free(signed_mem);
			//    release_file_data_pointer (mem_tbs);
		}


		//return nullptr;
	}
	*/


	System::Int16 CadesWrapper::Sign_Example1(System::String^ filename, System::String^ SubjectName)
	{
		//Certificat
		/*
		PCCERT_CONTEXT pContext = SignerUtils::wincrypt::GetCertificat(SubjectName.);
		int ret = SignerUtils::examples::SignCAdES_Example_01(filename, SignerUtils::wincrypt::GetCertificat(SubjectName));
		*/
		return -1;
	}

	System::Int16 CadesWrapper::Sign_Examples(System::String^ filename, System::String^ SubjectName)
	{
		//Certificat
		/*
		PCCERT_CONTEXT pContext = SignerUtils::wincrypt::GetCertificat(SubjectName);
		int ret = SignerUtils::examples::SignCAdES_Example_01(filename, SignerUtils::wincrypt::GetCertificat(SubjectName));
		*/
		return -1;
	}


	System::Int16 CadesWrapper::Sign_GOST_2012(System::String^ filename, System::String^ SerialNumber)
	{

		PCCERT_CONTEXT SignerCert = GetCertificatbySN(SerialNumber); 		//Certificat
		if (!SignerCert) return -1;

		const int detached = 1;
		LPVOID	    mem_tbs = NULL;
		size_t	    mem_len = 0;
		DWORD		signed_len = 0;
		

		//Source file
		LPCSTR FileName = (LPCSTR)StringtoChar(filename);
		char* OutFileName = StringtoChar(filename + ".sig");
		int retFile = IO::read_file(FileName, &mem_len, &mem_tbs);
		if (retFile = 0) goto err;

		//Source message
		DWORD		MessageSizeArray[1];
		const BYTE* MessageBody[1];
		MessageBody[0] = (BYTE*)mem_tbs; //pbToBeSigned, инициализируем содержимым файла
		MessageSizeArray[0] = mem_len;    //cbToBeSigned

		//Params for cades api, Установим параметры
		CRYPT_SIGN_MESSAGE_PARA signPara = { sizeof(signPara) };

		/* Обязательно нужно обнулить все поля структуры. */
		/* Иначе это может привести к access violation в функциях CryptoAPI*/
		/* В примере из MSDN это отсутствует*/
		/*
		memset(&signPara, 0, sizeof(CRYPT_SIGN_MESSAGE_PARA));
		signPara.cbSize = sizeof(CRYPT_SIGN_MESSAGE_PARA);
		*/
		signPara.dwMsgEncodingType = TYPE_DER;// X509_ASN_ENCODING | PKCS_7_ASN_ENCODING;
		signPara.pSigningCert = SignerCert; // 0 for window
		signPara.HashAlgorithm.pszObjId = (LPSTR)szOID_CP_GOST_R3411_12_256; //For obsolete CP_GOST_R3411 "1.2.643.2.2.9" got error -2146885628


		signPara.HashAlgorithm.Parameters.cbData = 0;
		signPara.HashAlgorithm.Parameters.pbData = NULL;
		signPara.cMsgCert = 1;		// 0 -If set to zero no certificates are included in the signed message
		signPara.rgpMsgCert = &SignerCert; //not NULL if cMSgCert = 1
		signPara.cAuthAttr = 0;
		signPara.dwInnerContentType = 0;
		signPara.cMsgCrl = 0;  // Cписки отзыва
		signPara.cUnauthAttr = 0;
		signPara.dwFlags = 0; //
		signPara.pvHashAuxInfo = NULL;
		signPara.rgAuthAttr = NULL; //NULL in WinCrypt


		CADES_SIGN_PARA cadesSignPara = { sizeof(cadesSignPara) };
		cadesSignPara.dwCadesType = CADES_BES;
		/*
		memset(&cadesSignPara, 0, sizeof(CADES_SIGN_PARA));
		cadesSignPara.dwSize = sizeof(CADES_SIGN_PARA);
		cadesSignPara.szHashAlgorithm = szOID_CP_GOST_R3411_12_256; // CRYPT_HASH_ALG_OID_GROUP_ID = "1.2.643.7.1.1.2.2"
		cadesSignPara.pSignerCert = SignerCert;
		*/




		CADES_SIGN_MESSAGE_PARA para = { sizeof(para) };
		//memset(&para, 0, sizeof(CADES_SIGN_MESSAGE_PARA)); 	/* Обязательно нужно обнулить все поля структуры. */
		para.pSignMessagePara = &signPara;
		para.pCadesSignPara = &cadesSignPara;

		//para.dwSize = para.pSignMessagePara->cbSize + para.pCadesSignPara->dwSize;


		PCRYPT_DATA_BLOB pSignedMessage = 0;

		/* Определение длины подписанного сообщения*/
		//??
		//

		//Count of the number of array elements in rgpbToBeSigned and rgpbToBeSigned.
		// This parameter must be set to one unless fDetachedSignature is set to TRUE
		int cToBeSigned = 1; //
		if (!CadesSignMessage(&para, detached, cToBeSigned, MessageBody, MessageSizeArray, &pSignedMessage))
		{
			//std::cout << "CadesSignMessage() failed" << std::endl;
			int err = GetLastError();
			LPTSTR errMeesage = SignerUtils::cades::Error2Message(err);
			return -1;
		}

		if (cspUtils::IO::write_file(OutFileName, pSignedMessage->cbData, pSignedMessage->pbData)) {
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


		if (!CadesFreeBlob(pSignedMessage))
		{
			//std::cout << "CadesFreeBlob() failed" << std::endl;
			return -2;
		}

		return 0;


	err:
		return System::Int16();
	}
	/*
	System::Int16 CadesWrapper::Sign_GOST(System::String^ filename, System::String^ SubjectName)
	{
		const int detached = 1;
		//char  OID[64] = "1.2.643.2.2.9";  //szOID_CP_GOST_R3411;  :  WinCryptEx.h
		LPVOID	    mem_tbs = NULL;
		size_t	    mem_len = 0;
		DWORD		signed_len = 0;
		BYTE* signed_mem = NULL;  // Буффер с подписью
		//Certificat
		PCCERT_CONTEXT pContext = SignerUtils::wincrypt::GetCertificat(SubjectName);

		//Source file
		LPCSTR FileName = (LPCSTR)StringtoChar(filename);
		char* OutFileName = StringtoChar(filename + ".sig");
		int retFile = IO::read_file(FileName, &mem_len, &mem_tbs);
		if (retFile = 0) goto err;
		DWORD		MessageSizeArray[1];
		const BYTE* MessageArray[1];
		MessageArray[0] = (BYTE*)mem_tbs; //pbToBeSigned, инициализируем содержимым файла
		MessageSizeArray[0] = mem_len;    //cbToBeSigned

		//Params for cades api, Установим параметры
		CRYPT_SIGN_MESSAGE_PARA signPara = { sizeof(signPara) };

		// Обязательно нужно обнулить все поля структуры.
		// Иначе это может привести к access violation в функциях CryptoAPI
		// В примере из MSDN это отсутствует
		memset(&signPara, 0, sizeof(CRYPT_SIGN_MESSAGE_PARA));

		signPara.cbSize = sizeof(CRYPT_SIGN_MESSAGE_PARA);
		signPara.dwMsgEncodingType = X509_ASN_ENCODING | PKCS_7_ASN_ENCODING;
		signPara.pSigningCert = pContext; // 0 for window
		signPara.HashAlgorithm.pszObjId = szOID_CP_GOST_R3411;// szOID_OIWSEC_sha1; //При работе с сертификатами с алгоритмом ключа ГОСТ Р 34.10-2001
															 //или ГОСТ Р 34.10-94 поле HashAlgorithm.pszObjId в структуре CRYPT_SIGN_MESSAGE_PARA
															 //должно иметь значение szOID_CP_GOST_R3411 (алгоритм хэширования ГОСТ Р 34.11-94). Определение szOID_CP_GOST_R3411 содержится в файле wincryptex.h.
		signPara.HashAlgorithm.Parameters.cbData = 0;
		signPara.HashAlgorithm.Parameters.pbData = NULL;
		signPara.pvHashAuxInfo = NULL;	// не используется
		signPara.cMsgCert = 1;		// 0 -не включаем сертификат отправителя f set to zero no certificates are included in the signed message
		signPara.rgpMsgCert = NULL; //&pContext;
		signPara.cAuthAttr = 0;
		signPara.dwInnerContentType = 0;
		signPara.cMsgCrl = 0;  // Cписки отзыва
		signPara.cUnauthAttr = 0;
		signPara.dwFlags = 0; //


		CADES_SIGN_PARA cadesSignPara = { sizeof(cadesSignPara) };
		memset(&cadesSignPara, 0, sizeof(CADES_SIGN_PARA));
		cadesSignPara.dwSize = sizeof(CADES_SIGN_PARA);
		cadesSignPara.szHashAlgorithm = szOID_CP_GOST_R3411;
		cadesSignPara.pSignerCert = pContext;
		cadesSignPara.dwCadesType = CADES_BES;

		CADES_SIGN_MESSAGE_PARA para = { sizeof(para) };
		memset(&para, 0, sizeof(CADES_SIGN_MESSAGE_PARA)); 	// Обязательно нужно обнулить все поля структуры.
		para.pSignMessagePara = &signPara;
		para.pCadesSignPara = &cadesSignPara;
		para.dwSize = para.pSignMessagePara->cbSize + para.pCadesSignPara->dwSize;


		PCRYPT_DATA_BLOB pSignedMessage = 0;

		// Определение длины подписанного сообщения
		//??
		//

		//Count of the number of array elements in rgpbToBeSigned and rgpbToBeSigned.
		// This parameter must be set to one unless fDetachedSignature is set to TRUE
		int cToBeSigned = 1; //
		if (!CadesSignMessage(&para, true, cToBeSigned, MessageArray, MessageSizeArray, &pSignedMessage))
		{
			//std::cout << "CadesSignMessage() failed" << std::endl;
			int err = GetLastError();
			LPTSTR errMeesage = SignerUtils::cades::Error2Message(err);
			return -1;
		}



		if (!CadesFreeBlob(pSignedMessage))
		{
			//std::cout << "CadesFreeBlob() failed" << std::endl;
			return -2;
		}

		return 0;


	err:
		return System::Int16();
	}
	*/

	int CadesWrapper::ReadTimeStamp(const char* Filename)
	{
		std::vector<unsigned char> message;
		// Reaf sign from file:
		if (cspUtils::IO::read_file_to_vector(Filename, message))
		{
			//cout << "Reading signature from file \"sign.dat\" failed" << endl;
			return -101;
		}

		if (message.empty())
		{
			//cout << "File \"sign.dat\" is empty" << endl;
			return -102;
		}

		// Открываем дескриптор сообщения для декодирования
		HCRYPTMSG hMsg = CryptMsgOpenToDecode(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, 0, 0, 0, 0, 0);
		if (!hMsg)
		{
			//cout << "CryptMsgOpenToDecode() failed" << endl;
			return -1;
		}

		// Заполняем прочитанной подписью криптографическое сообщение
		if (!CryptMsgUpdate(hMsg, &message[0], (unsigned long)message.size(), 1))
		{
			CryptMsgClose(hMsg);
			//cout << "CryptMsgUpdate() failed" << endl;
			return -1;
		}

		// Возвращаем алгоритм хэширования сертификата
		ALG_ID hashAlgId = CadesMsgGetSigningCertIdHashAlg(hMsg, 0);
		if (!hashAlgId)
		{
			CryptMsgClose(hMsg);
			//cout << "CadesMsgGetSigningCertIdHashAlg() failed" << endl;
			return -1;
		}

		// Возвращаем закодированные штампы времени на подпись, вложенные в сообщение в виде массива
		PCADES_BLOB_ARRAY pTimestamps = 0;
		if (!CadesMsgGetSignatureTimestamps(hMsg, 0, &pTimestamps))
		{
			CryptMsgClose(hMsg);
			//cout << "CadesGetSignatureTimestamps() failed" << endl;
			return 2; //no timestamps
		}

		// Освобождаем ресурсы
		if (!CadesFreeBlobArray(pTimestamps))
		{
			CryptMsgClose(hMsg);
			//cout << "CadesFreeBlobArray() failed" << endl;
			return -1;
		}

		PCADES_BLOB_ARRAY pCerts = 0;
		// Возвращаем закодированные сертификаты из доказательств подлинности подписи, вложенных в сообщение, в виде массива
		if (!CadesMsgGetCertificateValues(hMsg, 0, &pCerts))
		{
			CryptMsgClose(hMsg);
			//cout << "CadesGetCertificateValues() failed" << endl;
			return -1;
		}

		// Освобождаем ресурсы
		if (!CadesFreeBlobArray(pCerts))
		{
			CryptMsgClose(hMsg);
			//cout << "CadesFreeBlobArray() failed" << endl;
			return -1;
		}

		PCADES_BLOB_ARRAY pCRLs = 0;
		PCADES_BLOB_ARRAY pOCSPs = 0;
		// Возвращаем вложенные в сообщение доказательства проверки на отзыв (закодированные списки отозванных сертификатов и закодированные ответы службы OCSP) в виде массивов
		if (!CadesMsgGetRevocationValues(hMsg, 0, &pCRLs, &pOCSPs))
		{
			CryptMsgClose(hMsg);
			//cout << "CadesGetRevocationValues() failed" << endl;
			return 31;
		}

		PCADES_BLOB_ARRAY pCadesCTimestamps = 0;
		// Возвращаем закодированные штампы времени на доказательства подлинности подписи, вложенные в сообщение, в виде массива
		if (!CadesMsgGetCadesCTimestamps(hMsg, 0, &pCadesCTimestamps))
		{
			CryptMsgClose(hMsg);
			//cout << "CadesMsgGetCadesCTimestamps() failed" << endl;
			return -1;
		}

		// Освобождаем ресурсы
		if (!CadesFreeBlobArray(pCadesCTimestamps))
		{
			CryptMsgClose(hMsg);
			//cout << "CadesFreeBlobArray() failed" << endl;
			return -1;
		}

		// Освобождаем ресурсы
		if (!CadesFreeBlobArray(pCRLs))
		{
			CryptMsgClose(hMsg);
			CadesFreeBlobArray(pOCSPs);
			//cout << "CadesFreeBlobArray() failed" << endl;
			return -1;
		}

		// Освобождаем ресурсы
		if (!CadesFreeBlobArray(pOCSPs))
		{
			CryptMsgClose(hMsg);
			//cout << "CadesFreeBlobArray() failed" << endl;
			return -1;
		}

		// Закрываем дескриптор сообщения
		if (!CryptMsgClose(hMsg))
		{
			//cout << "CryptMsgClose() failed" << endl;
			return -1;
		}

		//cout << "All CAdES attributes obtained successfully." << endl;

	}



	/*
	PCCERT_CONTEXT_WR::PCCERT_CONTEXT_WR()
	{
		//throw gcnew System::NotImplementedException();
		this->id = 1975;// watch value
	}
	*/

	System::Byte WinCryptWrapper::GetCertDateExpirate(PCCERT_CONTEXT Certificat)
	{
		return (Byte) CertVerifyTimeValidity(NULL,               // Use current time.
			Certificat->pCertInfo);
		/*
	
		*/
	}

	System::String^ WinCryptWrapper::GetCertDateExpirate(System::Byte DateExpirateCode)
	{
		switch (DateExpirateCode)   
		{
		case -1:
		{
			return "Certificate is not valid yet. \n";
			break;
		}
		case 1:
		{
			return "Certificate is expired. \n";
			break;
		}
		case 0:
		{
			//return LPWSTRToString(cspUtils::IO::StrTime(Certificat->pCertInfo->NotAfter)) +
			return	"Certificate's time is valid. ";
			break;
		}
		};
	}

	PCCERT_CONTEXT WinCryptWrapper::GetCertificat(System::String^ SubjectName)
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

	PCCERT_CONTEXT WinCryptWrapper::GetCertificatbySN(String^ SerialNumber)
	{
		PCCERT_CONTEXT  ret = NULL;
		HANDLE	    hCertStore = 0;

		hCertStore = CertOpenStore(
			CERT_STORE_PROV_SYSTEM, /* LPCSTR lpszStoreProvider*/
			0,			    /* DWORD dwMsgAndCertEncodingType*/
			0,			    /* HCRYPTPROV hCryptProv*/
			CERT_SYSTEM_STORE_CURRENT_USER, /* DWORD dwFlags*/
			L"MY"		    /* const void *pvPara*/
		);

		while (ret = CertEnumCertificatesInStore(hCertStore, ret))
		{
			String^ Serial = PBYTEToStr(ret->pCertInfo->SerialNumber.pbData, ret->pCertInfo->SerialNumber.cbData);
			if (Serial == SerialNumber )
				return ret;
		}

		return PCCERT_CONTEXT();
	}

	PCCERT_CONTEXT WinCryptWrapper::GetCertificatbySN(CRYPT_INTEGER_BLOB SerialNumber)
	{
		//String^ Serial = PBYTEToStr(SerialNumber.pbData, SerialNumber.cbData);


		PCCERT_CONTEXT  ret = NULL;
		HANDLE	    hCertStore = 0;

		hCertStore = CertOpenStore(
			CERT_STORE_PROV_SYSTEM, /* LPCSTR lpszStoreProvider*/
			0,			    /* DWORD dwMsgAndCertEncodingType*/
			0,			    /* HCRYPTPROV hCryptProv*/
			CERT_SYSTEM_STORE_CURRENT_USER, /* DWORD dwFlags*/
			L"MY"		    /* const void *pvPara*/
		);

		while (ret = CertEnumCertificatesInStore(hCertStore, ret))
		{
			if (ret->pCertInfo->SerialNumber.pbData == SerialNumber.pbData)
				return ret;
		}

		return PCCERT_CONTEXT();
	}


	System::String^ WinCryptWrapper::GetCertificatSerialNumber(PCCERT_CONTEXT	ret)
	{
		/*
		if (ret)
		{
			return PBYTEToStr(((CRYPT_INTEGER_BLOB)ret->pCertInfo->SerialNumber).pbData,
				((CRYPT_INTEGER_BLOB)ret->pCertInfo->SerialNumber).cbData);
		}
		else return "";
		*/
		return CharToString(SignerUtils::wincrypt::GetSertSerial(ret));
	}

	System::String^ WinCryptWrapper::GetCertIssuerName(PCCERT_CONTEXT Certificat)
	{
		//throw gcnew System::NotImplementedException();
		// TODO: insert return statement here
		return LPTSTRToString(SignerUtils::wincrypt::GetCertIssuerName(Certificat));
		//TODO : debug reading
		//SignerUtils::wincrypt::GetCertOUAtributes(Certificat);
	}

	System::Int32 WinCryptWrapper::GetCertKeyParams(PCCERT_CONTEXT Certificat)
	{
		return SignerUtils::wincrypt::GetCertParam(Certificat);
	}

	List<String^>^ WinCryptWrapper::EnumerateCSPTypes()
	{
		std::vector<std::string> Containers =  SignerUtils::wincrypt::EnumAllContainers();
		List<String^>^ provs = gcnew List<String^>();
		std::vector<SignerUtils::CSPItem> provStd = SignerUtils::wincrypt::EnumProvidersTypes();
		for each (SignerUtils::CSPItem var in provStd)
		{
			std::string vr=  "Type [" + std::to_string(var.Type)  +"], " + var.Name;
			provs->Add(gcnew String(vr.c_str()));
		}
		return provs;
	}

	List<String^>^ WinCryptWrapper::EnumerateCSP()
	{
		List<String^>^ provs = gcnew List<String^>();
		std::vector<std::string> provStd = SignerUtils::wincrypt::EnumAllProviders();
		for each (std::string var in provStd)
		{
			provs->Add(gcnew String(var.c_str()));
		}
		return provs;
	}

	System::Boolean WinCryptWrapper::KeyPermission(PCCERT_CONTEXT Certificat, DWORD PermissionFlag)
	{
		return SignerUtils::wincrypt::GetCertParam(Certificat) & PermissionFlag;
	}

	System::Boolean WinCryptWrapper::KeyPermission_CRYPT_EXPORT_KEY(PCCERT_CONTEXT Certificat)
	{
		return SignerUtils::wincrypt::GetCertParam(Certificat) & CRYPT_EXPORT_KEY;
	}

	/*
	System::String^ WinCryptWrapper::GetCertificatSerialNumber(System::String^ SubjectName)
	{
		return (GetCertificatSerialNumber(GetCertificat(SubjectName)));
	}
	*/

	System::String^ WinCryptWrapper::DisplayCertInfo(PCCERT_CONTEXT	ret)
	{
		if (!ret) return "NULL";
		String^ res;
		DWORD sz = ((CRYPT_INTEGER_BLOB)ret->pCertInfo->SerialNumber).cbData;
		PBYTE serial = ((CRYPT_INTEGER_BLOB)ret->pCertInfo->SerialNumber).pbData;
		res = "Издатель: " +  GetCertIssuerName(ret) +
			//CN
			"\r\n" + LPTSTRToString(SignerUtils::wincrypt::GetCertOUAtributes(ret)) +
			"\r\n" + " e-mail:" + LPTSTRToString(SignerUtils::wincrypt::GetCertEmail(ret)) +
			"\r\n" + " серийный номер: " + PBYTEToStr(serial, sz) +
			"\r\n\ Срок действия " + GetCertDateExpirate(GetCertDateExpirate(ret)); 

		if (strcmp(ret->pCertInfo->SignatureAlgorithm.pszObjId, szOID_CP_GOST_R3411_12_256_R3410) == 0)
		{
			res += "\r\n CSP  - GOST Crypto Provider 256bit";
		}

		if (strcmp(ret->pCertInfo->SignatureAlgorithm.pszObjId, szOID_CP_GOST_R3411_12_512_R3410) == 0)
		{
			res += "\r\n CSP  - GOST Crypto Provider 512bit";
		}

		if (SignerUtils::wincrypt::GetCert(ret))
		{
			res += "  { DWORD ALGID " + SignerUtils::wincrypt::GetCertALGID(ret).ToString()+ " }";
			int KeyParams = GetCertKeyParams(ret);
			/*
			res += " DWORD Params " + KeyParams.ToString();

			if (KeyPermission(ret, CRYPT_EXPORT))
				res += "\r\n Allow key to be exported";
			else res += "\r\n Key export disabled";
			*/
			if (KeyPermission(ret, CRYPT_ENCRYPT))
				res += "\r\n Allow encryption";
			if (KeyPermission(ret, CRYPT_DECRYPT))
				res += "\r\n Allow decryption";
			if (KeyPermission(ret, CRYPT_READ))
				res += "\r\n Allow parameters to be read";
			if (KeyPermission(ret, CRYPT_WRITE))
				res += "\r\n Allow parameters to be write";
			if (KeyPermission(ret, CRYPT_EXPORT_KEY))
				res += "\r\n Allow key to be used for exporting keys";
			if (KeyPermission(ret, CRYPT_IMPORT_KEY))
				res += "\r\n Allow key to be used for importing keys";
		}
		SignerUtils::CNG::EnumerateKeys();
		return res;
	}

	System::String^ WinCryptWrapper::DisplayCertInfo(System::String^ SerialNumber)
	{
		//throw gcnew System::NotImplementedException();
		 return DisplayCertInfo(GetCertificatbySN(SerialNumber));
	}


	List<System::String^>^ WinCryptWrapper::GetCertificates()
	{
		//throw gcnew System::NotImplementedException();

		List<System::String^>^ res = gcnew List<System::String^>();

		// Список сертификатов:
		HANDLE	    hCertStore = 0;
		PCCERT_CONTEXT CrlCertificat = NULL;
		// Открыть хранилище
		hCertStore = CertOpenStore(CERT_STORE_PROV_SYSTEM, // LPCSTR lpszStoreProvider
			0,			   // DWORD dwMsgAndCertEncodingType
			0,			   //  HCRYPTPROV hCryptProv
			CERT_STORE_OPEN_EXISTING_FLAG | CERT_STORE_READONLY_FLAG |
			CERT_SYSTEM_STORE_CURRENT_USER, // DWORD dwFlags
			L"MY");    //  const void *pvPara

					   // перечисляем:
		while (CrlCertificat = CertEnumCertificatesInStore(hCertStore, CrlCertificat))
		{
			LPTSTR pszString;
			LPTSTR pszName;
			DWORD cbSize;
			CERT_BLOB blobEncodedName;
			//        Get and display 
			//        the name of subject of the certificate.

			if (!(cbSize = CertGetNameString(CrlCertificat, CERT_NAME_SIMPLE_DISPLAY_TYPE,
				0,
				NULL,
				NULL,
				0)))
			{
				//richTextBox1->AppendText("\nCertGetName 1 failed.");
			}

			if (!(pszName = (LPTSTR)malloc(cbSize * sizeof(TCHAR))))
			{
				// richTextBox1->AppendText("\nMemory allocation failed.");
			}

			if (CertGetNameString(CrlCertificat, CERT_NAME_SIMPLE_DISPLAY_TYPE,
				0,
				NULL,
				pszName,
				cbSize))

			{
				res->Add(LPTSTRToString(pszName));
				//-------------------------------------------------------
				//       Free the memory allocated for the string.
				free(pszName);
			}
			else
			{
				// "\nCertGetName failed."
			}

		}
		return res;
	}

	List<CertInfo^>^ WinCryptWrapper::GetCertificatesObj()
	{
		List<CertInfo^>^ res = gcnew List<CertInfo^>();
		HANDLE	    hCertStore = 0;
		PCCERT_CONTEXT Certificat = NULL;
		hCertStore = CertOpenStore(CERT_STORE_PROV_SYSTEM, // LPCSTR lpszStoreProvider
			0,			   // DWORD dwMsgAndCertEncodingType
			0,			   //  HCRYPTPROV hCryptProv
			CERT_STORE_OPEN_EXISTING_FLAG | CERT_STORE_READONLY_FLAG |
			CERT_SYSTEM_STORE_CURRENT_USER, // DWORD dwFlags
			L"MY");    //  const void *pvPara

					   // enum certs:
		while (Certificat = CertEnumCertificatesInStore(hCertStore, Certificat))
		{
			LPTSTR pszString;
			LPTSTR pszName;
			DWORD cbSize;
			CERT_BLOB blobEncodedName;
			//        Get and display 
			//        the name of subject of the certificate.

			if (!(cbSize = CertGetNameString(Certificat, CERT_NAME_SIMPLE_DISPLAY_TYPE,
				0,
				NULL,
				NULL,
				0)))
			{
				// CertGetName  failed.
			}

			if (!(pszName = (LPTSTR)malloc(cbSize * sizeof(TCHAR))))
			{
				// richTextBox1->AppendText("\nMemory allocation failed.");
				CertInfo^ ci = gcnew CertInfo();
				ci->SubjectName = "Memory allocation failed";
				res->Add(ci);
			}

			if (CertGetNameString(Certificat, CERT_NAME_SIMPLE_DISPLAY_TYPE,
				0,
				NULL,
				pszName,
				cbSize))

			{
				CertInfo^ ci = gcnew CertInfo();
				ci->SubjectName = LPTSTRToString(pszName);
				ci->SerialNumber = PBYTEToStr(((CRYPT_INTEGER_BLOB)Certificat->pCertInfo->SerialNumber).pbData,
					((CRYPT_INTEGER_BLOB)Certificat->pCertInfo->SerialNumber).cbData);
				ci->Serial = &Certificat->pCertInfo->SerialNumber;
				ci->ValidNotAfter = LPWSTRToString(cspUtils::IO::StrTime(Certificat->pCertInfo->NotAfter));
				ci->ValidNotAfterCode = GetCertDateExpirate(Certificat);
				ci->ContainerName = "-"; //TODO GetName of Container
				ci->KeyParams = SignerUtils::wincrypt::GetCertParam(Certificat);
				//if ()
				ci->EXPORT_KEY = ci->KeyParams & CRYPT_EXPORT;
				//else 
					//ci->EXPORT_KEY = false;

				HCRYPTPROV hCryptProv;
				DWORD pdwKeySpec = 0;
				BOOL* pfCallerFreeProvOrNCryptKey = false;
				if (CryptAcquireCertificatePrivateKey(Certificat, 0, NULL, &hCryptProv, &pdwKeySpec, NULL))
				{
					ci->PrivateKeyPresent = true;
					ci->PrivateKeyHandle = hCryptProv;
				}
				res->Add(ci);
				//-------------------------------------------------------
				//       Free the memory allocated for the string.
				free(pszName);
			}
			else
			{
				// "\nCertGetName failed."
			}
			
		}

		return res;
	}


	System::Int16 WinCryptWrapper::SignFileWinCrypt(System::String^ filename, System::String^ SerialNumber)
	{
		PCCERT_CONTEXT	ret = GetCertificatbySN(SerialNumber);
		if (ret)
		{
			
			return SignerUtils::wincrypt::SignFileWinCrypt(StringtoChar(filename), ret);
		}
		else
			return 21; //certificate error
	}



	CNGWrapper::CNGWrapper()
	{
		throw gcnew System::NotImplementedException();
	}

	List<String^>^ CNGWrapper::EnumerateProviders()
	{
		List<String^>^ provs = gcnew List<String^>();
		
		std::vector<std::string> provStd =  SignerUtils::CNG::EnumerateStorageProviders();
		for each (std::string var in provStd)
		{
			provs->Add(gcnew String(var.c_str()));
		}
		return provs;
	}

};
