//2014 Fixosoft String routines
// ����������� �����
#ifndef _SignerUtils_h_INCLUDED // ���� ������ �� �������������� ���������
#define _SignerUtils_h_INCLUDED

#define HAVE_MAPVIEWOFFILE 1

#ifdef WIN32
#pragma warning (disable:4115)
#endif /* WIN32 */

#include <windows.h>  //���� ��������

//#include <stdio.h>
//#include <malloc.h>
//#include <wincrypt.h>
//#include <memory.h>
//#include <sys/stat.h>
//#include <wincryptex.h>
//#include "getopt.h"
//#include "base64.h"

//#include "..\..\Cades SDK\include\cades.h"
#include "cades.h" //CryptoPro

using namespace System; // ���� .NET

namespace SignerUtils {
	namespace wincrypt {
		int    SignFileWinCrypt(System::String^ FileToSign, PCCERT_CONTEXT  SignerCert);
		PCCERT_CONTEXT GetCertificat(System::String^ SubjectName);
		LPTSTR GetCertIssuerName(PCCERT_CONTEXT Certificat);// �������� �����������
		LPTSTR GetCertEmail(PCCERT_CONTEXT Certificat);
		//LPTSTR GetCertDateExp(PCCERT_CONTEXT Certificat);
		CHAR* GetLastErrorText(CHAR* pBuf, ULONG bufSize);
	}
	namespace cades {
		LPTSTR Error2Message(DWORD dwErr);
	}
	namespace examples {


		// ����� ����� define ��� ������ API ������
#define TYPE_DER (X509_ASN_ENCODING | PKCS_7_ASN_ENCODING)

		int    SignCadesFile(System::String^ FileToSign, PCCERT_CONTEXT CertToSign);
		int    SignCadesExample(PCCERT_CONTEXT CertToSign);
		DWORD  SignCAdES_Example_01(System::String^ FileToSign, PCCERT_CONTEXT CertToSign);




#endif /* _SignerUtils_h_INCLUDED */

	}
}