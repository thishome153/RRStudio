//2014-2020  Fixosoft wincrypt routines


#ifndef _SignerUtils_h_INCLUDED // типа защита от множественного включения
#define _SignerUtils_h_INCLUDED

#define HAVE_MAPVIEWOFFILE 1

#ifdef WIN32
#pragma warning (disable:4115)
#endif /* WIN32 */

//#include <windows.h>  //типы основные
//#include <stdio.h>
//#include <malloc.h>
//#include <wincrypt.h>
//#include <memory.h>
//#include <sys/stat.h>
//#include <wincryptex.h>
//#include "getopt.h"
//#include "base64.h"
#include "cades.h" //CryptoPro


namespace SignerUtils {
	namespace wincrypt {
		int    SignFileWinCrypt(LPCSTR FileName, PCCERT_CONTEXT  SignerCert);
		PCCERT_CONTEXT GetCertificat(LPCSTR lpszCertSubject);
		DWORD   GetCertParam(PCCERT_CONTEXT SignerCert);
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