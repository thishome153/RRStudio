#pragma once
#include "cades.h" //CryptoPro



#ifdef _USRDLL
#define DLL_EXPORTABLE __declspec(dllexport)
#else
#define DLL_EXPORTABLE __declspec(dllimport)
#endif


extern "C"
{
	DLL_EXPORTABLE int   SignFile_api_Lite(char* FileName, PCCERT_CONTEXT SignerCertificat);
	DLL_EXPORTABLE DWORD CertKeyParams(PCCERT_CONTEXT Certificat);
}
