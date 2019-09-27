/*++

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED
TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.

Copyright (C) 1999 - 2000 Microsoft Corporation.  All rights reserved.

Module Name:

    webclient.c

Abstract:

    Schannel web client sample application.


--*/

#include "tmain.h"

#include <stdlib.h>
#include <winsock.h>
#pragma warning (push, 3)
#include <wintrust.h>
#pragma warning (pop)
#include <schannel.h>

#define SECURITY_WIN32
#include <security.h>
#include <sspi.h>

#define DLL_NAME TEXT("Secur32.dll")
#define NT4_DLL_NAME TEXT("Security.dll")

static HMODULE g_hSecurity = NULL;

SecurityFunctionTable g_SecurityFunc;


/*****************************************************************************/
BOOL
LoadSecurityLibrary(void)
{
    PSecurityFunctionTable          pSecurityFunc;
    INIT_SECURITY_INTERFACE         pInitSecurityInterface;
//    QUERY_CREDENTIALS_ATTRIBUTES_FN pQueryCredentialsAttributes;
    OSVERSIONINFO VerInfo;
    UCHAR lpszDLL[MAX_PATH];

    //
    //  Find out which security DLL to use, depending on
    //  whether we are on Win2K, NT or Win9x
    //

    VerInfo.dwOSVersionInfoSize = sizeof (OSVERSIONINFO);
    if (!GetVersionEx (&VerInfo))   
    {
        return FALSE;
    }

    if (VerInfo.dwPlatformId == VER_PLATFORM_WIN32_NT 
        && VerInfo.dwMajorVersion == 4)
    {
        strcpy ((char *)lpszDLL, NT4_DLL_NAME );
    }
    else if (VerInfo.dwPlatformId == VER_PLATFORM_WIN32_NT)
    {
        strcpy ((char *)lpszDLL, DLL_NAME );
    }
    else if (VerInfo.dwPlatformId == VER_PLATFORM_WIN32_WINDOWS)
    {
        strcpy ((char *)lpszDLL, "schannel.dll" );
    }
    else
    {
        return FALSE;
    }

    //
    //  Load Security DLL
    //

    g_hSecurity = LoadLibrary((char *)lpszDLL);
    if(g_hSecurity == NULL)
    {
        printf("Error 0x%x loading %s.\n", GetLastError(), lpszDLL);
        return FALSE;
    }

    pInitSecurityInterface = (INIT_SECURITY_INTERFACE)GetProcAddress(
                                    g_hSecurity,
                                    "InitSecurityInterfaceA");
    
    if(pInitSecurityInterface == NULL)
    {
        printf("Error 0x%x reading InitSecurityInterface entry point.\n", 
               GetLastError());
        return FALSE;
    }

    pSecurityFunc = pInitSecurityInterface();

    if(pSecurityFunc == NULL)
    {
        printf("Error 0x%x reading security interface.\n",
               GetLastError());
        return FALSE;
    }

    CopyMemory(&g_SecurityFunc, pSecurityFunc, sizeof(g_SecurityFunc));

    return TRUE;
}

/*****************************************************************************/
void
UnloadSecurityLibrary(void)
{
    FreeLibrary(g_hSecurity);
    g_hSecurity = NULL;
}

void 
PrintHexDump(DWORD length, PBYTE buffer)
{
    DWORD i,count,index;
    CHAR rgbDigits[]="0123456789abcdef";
    CHAR rgbLine[100];
    char cbLine;

    for(index = 0; length; length -= count, buffer += count, index += count) 
    {
        count = (length > 16) ? 16:length;

        sprintf(rgbLine, "%4.4x  ",index);
        cbLine = 6;

        for(i=0;i<count;i++) 
        {
            rgbLine[cbLine++] = rgbDigits[buffer[i] >> 4];
            rgbLine[cbLine++] = rgbDigits[buffer[i] & 0x0f];
            if(i == 7) 
            {
                rgbLine[cbLine++] = ':';
            } 
            else 
            {
                rgbLine[cbLine++] = ' ';
            }
        }
        for(; i < 16; i++) 
        {
            rgbLine[cbLine++] = ' ';
            rgbLine[cbLine++] = ' ';
            rgbLine[cbLine++] = ' ';
        }

        rgbLine[cbLine++] = ' ';

        for(i = 0; i < count; i++) 
        {
            if(buffer[i] < 32 || buffer[i] > 126) 
            {
                rgbLine[cbLine++] = '.';
            } 
            else 
            {
                rgbLine[cbLine++] = buffer[i];
            }
        }

        rgbLine[cbLine++] = 0;
        printf("%s\n", rgbLine);
    }
}

void
DisplayWinVerifyTrustError(DWORD Status)
{
    LPSTR pszName = NULL;

    switch(Status)
    {
    case CERT_E_EXPIRED:                pszName = "CERT_E_EXPIRED";                 break;
    case CERT_E_VALIDITYPERIODNESTING:  pszName = "CERT_E_VALIDITYPERIODNESTING";   break;
    case CERT_E_ROLE:                   pszName = "CERT_E_ROLE";                    break;
    case CERT_E_PATHLENCONST:           pszName = "CERT_E_PATHLENCONST";            break;
    case CERT_E_CRITICAL:               pszName = "CERT_E_CRITICAL";                break;
    case CERT_E_PURPOSE:                pszName = "CERT_E_PURPOSE";                 break;
    case CERT_E_ISSUERCHAINING:         pszName = "CERT_E_ISSUERCHAINING";          break;
    case CERT_E_MALFORMED:              pszName = "CERT_E_MALFORMED";               break;
    case CERT_E_UNTRUSTEDROOT:          pszName = "CERT_E_UNTRUSTEDROOT";           break;
    case CERT_E_CHAINING:               pszName = "CERT_E_CHAINING";                break;
    case TRUST_E_FAIL:                  pszName = "TRUST_E_FAIL";                   break;
    case CERT_E_REVOKED:                pszName = "CERT_E_REVOKED";                 break;
    case CERT_E_UNTRUSTEDTESTROOT:      pszName = "CERT_E_UNTRUSTEDTESTROOT";       break;
    case CERT_E_REVOCATION_FAILURE:     pszName = "CERT_E_REVOCATION_FAILURE";      break;
    case CERT_E_CN_NO_MATCH:            pszName = "CERT_E_CN_NO_MATCH";             break;
    case CERT_E_WRONG_USAGE:            pszName = "CERT_E_WRONG_USAGE";             break;
    default:                            pszName = "(unknown)";                      break;
    }

    printf("Error 0x%x (%s) returned by CertVerifyCertificateChainPolicy!\n", 
        Status, pszName);
}
