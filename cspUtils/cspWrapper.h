//*********************************************
// CLR Wrapper for Cryptorgaphy (i.e. wincrypt)
//2019 Fixosoft CSP Wrapper

#ifndef _csp_Wrapper_h_INCLUDED // типа защита от множественного включения
#define _csp_Wrapper_h_INCLUDED

#include <windows.h>  //типы основные

//#include <string.h>

#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace cspUtils {

	/* CRYPT_SIGN_ALG_OID_GROUP_ID */
	constexpr auto szOID_CP_GOST_R3411_12_256_R3410 = "1.2.643.7.1.1.3.2";
	/* CRYPT_SIGN_ALG_OID_GROUP_ID */
	constexpr auto szOID_CP_GOST_R3411_R3410EL = "1.2.643.2.2.3";
	// Cryptography portal
	constexpr auto microsoft_doc_portal_URL = "https://docs.microsoft.com/en-us/windows/win32/seccrypto/cryptography-portal";



#pragma managed
	//  Certificate context struct CLR wrapper:
	/*
	public ref struct PCCERT_CONTEXT_CLR {
	public: PCCERT_CONTEXT* Item;
	public:	int id;
	};
	*/
	/*
	public ref class PCCERT_CONTEXT_WR {
	public: int id;


	public: PCCERT_CONTEXT* Certificat;

	public:	property  PCCERT_CONTEXT* Native
	{
	public: PCCERT_CONTEXT* get()
	{
		return Certificat;
	}

	public: void set(PCCERT_CONTEXT* value)
	{
		this->Certificat = value;
	}
	}

	public: PCCERT_CONTEXT_WR();
	};
	*/
	
	public ref struct CertInfo
	{
		String^ ValidNotAfter;
		String^ SubjectName;
		String^ SerialNumber;
		CRYPT_INTEGER_BLOB* Serial;
	};

	
	// Windows 'wincrypt' wrapper class. Simplify usage of system cryptography
	public ref class WinCryptWrapper
	{
	public:
		System::String^ GetCertDateExpirate(PCCERT_CONTEXT Certificat);
		PCCERT_CONTEXT  GetCertificat(System::String^ SubjectName);
		PCCERT_CONTEXT GetCertificatbySN(String^ SerialNumber);
		//Find certificate by SN, in case difficult Subject Name finds by Subject may not sucess
		PCCERT_CONTEXT  GetCertificatbySN(CRYPT_INTEGER_BLOB SerialNumber);
	public:
		System::String^ GetCertificatSerialNumber(PCCERT_CONTEXT ret);
		System::String^ GetCertIssuerName(PCCERT_CONTEXT Certificat);
		//System::String^ GetCertificatSerialNumber(System::String^ SubjectName);
		System::String^ DisplayCertInfo(PCCERT_CONTEXT ret);
			// Разбор полей спертификата в строку
	public:	System::String^ DisplayCertInfo(System::String^ SerialNumber);
	public:	List<System::String^>^ GetCertificates();
	public: List<CertInfo^>^ GetCertificatesObj();
			// подпись чисто по WinCrypt
	public:	System::Int16	SignFileWinCrypt(System::String^ filename, System::String^ SerialNumber);
	};


	// CadesWrapper - GOST CSP Provider wrapper class for .NET. of CADES CSP
	// Инкапсулирует методы работы с API криптопровайдера CryptoPro.
	// Требует установленнoго CSP (cades.dll) Runtime
	public ref class CadesWrapper :WinCryptWrapper
	{

	public:
		CadesWrapper();

		// Отображение системного окна свойств подписи
	public:	 int  DisplaySig(System::String^ FileSign, System::IntPtr Parent);
	//public:	PCCERT_CONTEXT_CLR^ GetCertificatCLR(System::String^ SubjectName);
	//public:	PCCERT_CONTEXT_WR^ GetCertificatWrapped(System::String^ SubjectName);



	//public:	System::Int16			Sign_GOST(System::String^ filename, System::String^ SubjectName); // подпись по CADES, с расчетом hash
	//Read atributes - timestamp
			int ReadTimeStamp(const char* Filename);
	// Sign file with GOST_R3411_12_256 using cades (CSP Crypto Pro)
	public:	System::Int16			Sign_GOST_2012(System::String^ filename, System::String^ SerialNumber);
	public: System::Int16			Sign_Example1(System::String^ filename, System::String^ SubjectName);
	public: System::Int16			Sign_Examples(System::String^ filename, System::String^ SubjectName);

	};


#endif /* _csp_Wrapper_h_INCLUDE */



}
