// dllmain.cpp : Defines the entry point for the DLL application.
#include "Stdafx.h"
#include "dllmain.h"
#include "SignerUtils.h"

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}




DLL_EXPORTABLE int SignFile_api_Lite(char* FileName, PCCERT_CONTEXT SignerCertificat)
{

	return SignerUtils::wincrypt::SignFileWinCrypt(FileName, SignerCertificat);
	//return DLL_EXPORTABLE int();
}

DLL_EXPORTABLE DWORD CertKeyParams(PCCERT_CONTEXT Certificat)
{

	return SignerUtils::wincrypt::GetCertParam( Certificat);
	//return DLL_EXPORTABLE int();
}