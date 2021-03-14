//2014-2020  Fixosoft wincrypt,CNG routines


#ifndef _SignerUtils_h_INCLUDED 
#define _SignerUtils_h_INCLUDED

#define HAVE_MAPVIEWOFFILE 1

#ifdef WIN32
#pragma warning (disable:4115)
#endif /* WIN32 */

#include <vector>
#include <string>
//#include <malloc.h>
//#include <wincrypt.h>
//#include <memory.h>
//#include <sys/stat.h>
//#include <wincryptex.h>
//#include "getopt.h"
//#include "base64.h"
#include "cades.h" //CryptoPro


namespace SignerUtils {
	void string_to_wstring(const std::string& src, std::wstring& dest);
	void wstring_to_string(const std::wstring& src, std::string& dest);

	struct CSPItem {
		int Type;
		std::string Name;
	};

	namespace CNG {
#define NT_SUCCESS(Status)          (((NTSTATUS)(Status)) >= 0)
#define STATUS_UNSUCCESSFUL         ((NTSTATUS)0xC0000001L)
#define NTSEC_SUCCESS(Status)          (((SECURITY_STATUS)(Status)) >= 0)
		void EnumerateKeys(); // Enumerate all keys
		std::vector<std::string> EnumerateStorageProviders();
	}

	namespace wincrypt {
		int    SignFileWinCrypt(LPCSTR FileName, PCCERT_CONTEXT  SignerCert);
		PCCERT_CONTEXT GetCertificat(LPCSTR lpszCertSubject);
		LPTSTR GetCertOUAtributes(PCCERT_CONTEXT Certificat);
		PCCERT_CONTEXT GetCert(PCCERT_CONTEXT SignerCert); //With getparam
		DWORD   GetCertParam(PCCERT_CONTEXT SignerCert);
		DWORD   GetCertALGID(PCCERT_CONTEXT SignerCert);

		std::vector<CSPItem> EnumProvidersTypes();
		std::vector<std::string>  EnumAllProviders(); //function retrieve in sequence all of the CSPs
		std::vector<std::string>  EnumAllContainers(); //function retrieve in sequence all of the CSPs
		CHAR* GetSertSerial(PCCERT_CONTEXT ret);
		LPTSTR GetCertIssuerName(PCCERT_CONTEXT Certificat);// Издатель сертификата
		LPTSTR GetCertEmail(PCCERT_CONTEXT Certificat);
		CHAR* GetLastErrorText(CHAR* pBuf, ULONG bufSize);
	}
	namespace cades {
		LPTSTR Error2Message(DWORD dwErr);
	}

#define TYPE_DER (X509_ASN_ENCODING | PKCS_7_ASN_ENCODING)


#ifdef RememberExamples
	namespace examples {


		// Всяко разно define для работы API утилит


		int    SignCadesFile(System::String^ FileToSign, PCCERT_CONTEXT CertToSign);
		int    SignCadesExample(PCCERT_CONTEXT CertToSign);
		DWORD  SignCAdES_Example_01(System::String^ FileToSign, PCCERT_CONTEXT CertToSign);
	}
#endif

#endif 


	/* _SignerUtils_h_INCLUDED */
}