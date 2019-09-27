/*
 * Copyright(C) 2000-2001 Проект ИОК
 *
 * Этот файл содержит информацию, являющуюся
 * собственностью компании Крипто Про.
 *
 * Любая часть этого файла не может быть скопирована,
 * исправлена, переведена на другие языки,
 * локализована или модифицирована любым способом,
 * откомпилирована, передана по сети с или на
 * любую компьютерную систему без предварительного
 * заключения соглашения с компанией Крипто Про.
 *
 * Программный код, содержащийся в этом файле, предназначен
 * исключительно для целей обучения и не может быть использован
 * для защиты информации.
 *
 * Компания Крипто-Про не несет никакой
 * ответственности за функционирование этого кода.
 */

/*!
 * \file $RCSfile: WebServer.c,v $
 * \version $Revision: 1.20.4.1 $
 * \date $Date: 2002/07/15 12:13:16 $
 * \author $Author: chudov $
 *
 * \brief Schannel web server sample application.
 *
 * Подробное описание модуля.
 */
#include "tmain.h"

#include <stdlib.h>
#include <process.h>
#include <winsock.h>
#pragma warning (push, 3)
#include <wintrust.h>
#pragma warning (pop)
#include <schannel.h>

#define SECURITY_WIN32
#include <security.h>
#include <sspi.h>
#include "crypt32.h"
//#include "..\..\src\SSP\SSPMap.h"
#include "DIRECT.H"

extern SecurityFunctionTable g_SecurityFunc;
extern BOOL LoadSecurityLibrary();
extern void UnloadSecurityLibrary();
extern void PrintHexDump(DWORD length, PBYTE buffer);
extern void DisplayWinVerifyTrustError(DWORD Status);

#pragma warning (disable: 4127)

#define IO_BUFFER_SIZE  0x10000
#define TLS_MIN(a,b)        ((a) < (b) ? (a) : (b))
#define MAX_IOBSZ           (gen_iobsz())

// User options.
static INT     iPortNumber     = 443;
static int     fVerbose        = 0;
static LPSTR   pszUserName     = NULL;
static BOOL    fClientAuth     = FALSE;
static BOOL    fFragmentMessages = FALSE;
static BOOL    fHttpsClientAuth = FALSE;
static DWORD   dwObufClntAut   = (DWORD)-1; // Запрос на клиентскую аутентификацию перед выдачей 
                                            // блока данных dwObufClntAut.
                                            // При dwObufClntAut == 1 - аутентификация как в IIS, 
                                            // т.е. запрос на аутентификацию в ответ на получение 
                                            // HTTPS запроса.
static BOOL    fMachineStore   = FALSE;
static DWORD   dwProtocol      = 0;
static BOOL    fSendNothing    = FALSE;

static HCERTSTORE  hMyCertStore = NULL;

static DWORD    min_gen_iobsz = 1000000;
static DWORD    max_gen_iobsz = 1000000;
static DWORD    p_sleep = 0;
static volatile LONG    dwMaxThread = 1;

static BOOL 	fShutdown = FALSE;
static SOCKET	ListenSocket = INVALID_SOCKET;

static
DWORD 
gen_iobsz(void) {
    DWORD r = rand();

    /* 0..RAND_MAX -> min_gen_iobsz..max_gen_iobsz */

    r *= (max_gen_iobsz-min_gen_iobsz);
    r /= (RAND_MAX-0);
    r += min_gen_iobsz;

    if (r < 1) {
        r = 1;
    }

    if (p_sleep > 0) {
        if ((DWORD)rand() <= RAND_MAX/p_sleep) {
            Sleep(100);
        }
    }

    return r;
}

static
DWORD
CreateCredentials(
    LPSTR pszUserName,
    PCredHandle phCreds);

static void
WebServer(CredHandle *phServerCreds);

static
BOOL
ParseRequest (
    IN PCHAR InputBuffer,
    IN INT InputBufferLength,
    OUT PCHAR ObjectName,
    OUT DWORD *pcbContentLength);

static
BOOL
SSPINegotiateLoop(
    SOCKET          Socket,
    PCtxtHandle     phContext,
    PCredHandle     phCred,
    BOOL            fClientAuth,
    BOOL            fDoInitialRead,
    BOOL            NewContext,
    CHAR            IoBuffer[IO_BUFFER_SIZE],
    DWORD          *pcbIoBuffer);

static
LONG
DisconnectFromClient(
    SOCKET          Socket, 
    PCredHandle     phCreds,
    CtxtHandle *    phContext);

typedef struct tagWebServerArg {
    CredHandle *mpServerCreds;
    SOCKET      mSocket;
}   WEBSERVERARG, *PWEBSERVERARG;

static unsigned int __stdcall
WebServerThread(LPVOID lpParameter);


/*****************************************************************************/
int
main_tlss(int argc, char *argv[])
{
    WSADATA WsaData;
    CredHandle hServerCreds;

    int	    ret = 0;
    int	    print_help = 0;
    int    c;
   
    /*-----------------------------------------------------------------------------*/
    /* определение опций разбора параметров*/
    static struct option long_options[] = {
/*	{"cert",	required_argument,	NULL, 'x'},*/
	{"help",	no_argument,		NULL, 'h'},
	{"port",	required_argument,	NULL, 'p'},
	{"proto",	required_argument,	NULL, 'P'},
	{"username",	required_argument,	NULL, 'u'},
	{"Username",	required_argument,	NULL, 'U'},
	{"verbose",	no_argument,		NULL, 'v'},
	{"auth",	no_argument,		NULL, 'a'},
	{"Auth",	no_argument,		NULL, 'A'},
	{"nop",		no_argument,		NULL, 'n'},
        {"fragment",    no_argument,		NULL, 'f'},
        {"bmin",        required_argument,	NULL, 'b'},
        {"bmax",        required_argument,	NULL, 'B'},
        {"pslp",        required_argument,	NULL, 'r'},
        {"tmax",        required_argument,	NULL, 't'},
	{0, 0, 0, 0}
    };

    /*-----------------------------------------------------------------------------*/
    /* разбор параметров*/
    /* для разбора параметров используется модуль getopt.c*/
    while ((c = getopt_long_only (argc, argv, "", long_options, (int *)0)) != EOF) {
	switch (c) {
	case 'h':
	    ret = 1;
	    print_help = 1;
	    goto bad;
        case 'p':
            iPortNumber = atoi(optarg);
            break;
        case 'v':
            fVerbose++;
            break;
        case 'u':
            pszUserName = optarg;
            fMachineStore = FALSE;
            break;
        case 'U':
            pszUserName = optarg;
            fMachineStore = TRUE;
            break;
	case 'n':
	    fSendNothing = TRUE;
            break;
        case 'a':
            fClientAuth = TRUE;
            break;
        case 'A':
            fHttpsClientAuth = TRUE;
            break;
        case 'f':
            fFragmentMessages = TRUE;
            break;
        case 'P':
            switch(atoi(optarg))
            {
                case 1:
                    dwProtocol = SP_PROT_PCT1;
                    break;
                case 2:
                    dwProtocol = SP_PROT_SSL2;
                    break;
                case 3:
                    dwProtocol = SP_PROT_SSL3;
                    break;
                case 4:
                    dwProtocol = SP_PROT_TLS1;
                    break;
                default:
                    dwProtocol = 0;
                    break;
            }
            break;
        case 'b':
            min_gen_iobsz = atoi(optarg);
            break;
        case 'B':
            max_gen_iobsz = atoi(optarg);
            break;
        case 'r':
            p_sleep = atoi(optarg);
            break;
        case 't':
            dwMaxThread = atoi(optarg);
            break;
	case '?':
	default:
	    goto bad;
        }
    }
    
    if (c != EOF) {
	print_help = 1;
	goto bad;
    }
    
    for (c = 0; c < argc; c++) {
        printf("%s ", argv[c] ? argv[c] : "(NULL)");
    }
    printf("\n");

    if(!LoadSecurityLibrary())
    {
        printf("Error initializing the security library\n");
        return 0;
    }

    //
    // Initialize the WinSock subsystem.
    //

    if(WSAStartup(0x0101, &WsaData) == SOCKET_ERROR)
    {
        printf("Error %d returned by WSAStartup\n", GetLastError());
        return 0;
    }

    //
    // NOTE: In theory, an application could enumerate the security packages 
    // until it finds one with attributes it likes. Some applications 
    // (such as IIS) enumerate the packages and call AcquireCredentialsHandle 
    // on each until it finds one that accepts the SCHANNEL_CRED structure. 
    // If an application has its heart set on using SSL, like this sample does, 
    // then just hardcoding the UNISP_NAME package name when calling 
    // AcquireCredentialsHandle is not a bad thing.
    //

    //
    // Create credentials.
    //

    if(CreateCredentials(pszUserName, &hServerCreds))
    {
        printf("Error creating credentials\n");
	return 0;
    }


    WebServer(&hServerCreds);

    // Free SSPI credentials handle.
    g_SecurityFunc.FreeCredentialsHandle(&hServerCreds);

    // Shutdown WinSock subsystem.
    WSACleanup();

    // Close "MY" certificate store.
    if(hMyCertStore)
    {
        CertCloseStore(hMyCertStore, 0);
    }

#ifdef _DEBUG
    {
	I_CRYPTGETDEFAULTCRYPTPROV *
	    fI_CryptGetDefaultCryptProv = 0;
	HANDLE h = GetModuleHandle ("crypt32.dll");
	HCRYPTPROV hProv;
	if (!h)
	    goto bad;
	fI_CryptGetDefaultCryptProv
	    = (I_CRYPTGETDEFAULTCRYPTPROV *)
		GetProcAddress (h, "I_CryptGetDefaultCryptProv");
	if (!fI_CryptGetDefaultCryptProv)
	    goto bad;
	hProv = fI_CryptGetDefaultCryptProv (CALG_GR3410);
	if (!hProv)
	    goto bad;
	CryptReleaseContext (hProv, 0);
	CryptAcquireContext (&hProv, "$reboot$", CP_DEF_PROV, PROV_GOST_DH, CRYPT_DELETEKEYSET);
    }
#endif

bad:
    if (print_help) {
	fprintf(stderr,"%s -tlss [options]\n", prog);
	fprintf(stderr,SoftName " test tls (server)\n");
	fprintf(stderr,"options:\n");
	fprintf(stderr,"  -user name         Name of user (in existing certificate)\n");
	fprintf(stderr,"  -User name         Name of user (machine store)\n");
	fprintf(stderr,"  -port num          Port to listen on (default 443).\n");
	fprintf(stderr,"  -verbose           Verbose Mode.\n");
	fprintf(stderr,"  -a                 Ask for client auth by connect request.\n");
	fprintf(stderr,"  -A                 Ask for client auth by https request.\n");
	fprintf(stderr,"  -proto num         Protocol to use\n");
	fprintf(stderr,"                        1 = PCT 1.0         3 = SSL 3.0\n");
	fprintf(stderr,"                        2 = SSL 2.0         4 = TLS 1.0\n");
	fprintf(stderr,"  -bmin N            Lower range random max input buffer\n");
	fprintf(stderr,"  -bmax N            Upper range random max input buffer\n");
	fprintf(stderr,"  -pslp N            sleep 100 ms in 1 from N choice\n");
	fprintf(stderr,"  -tmax N            maximum number of thread\n");
	fprintf(stderr,"  -help              print this help\n\n");
    }

    return ret;
}

/*****************************************************************************/
static 
void
DisplayCertChain(
    PCCERT_CONTEXT pServerCert)
{
    CHAR szName[1000];
    PCCERT_CONTEXT pCurrentCert;
    PCCERT_CONTEXT pIssuerCert;
    DWORD dwVerificationFlags;

    printf("\n");

    // display leaf name
    if(!CertNameToStr(pServerCert->dwCertEncodingType,
                      &pServerCert->pCertInfo->Subject,
                      CERT_X500_NAME_STR | CERT_NAME_STR_NO_PLUS_FLAG,
                      szName, sizeof(szName)))
    {
        printf("**** Error 0x%x building subject name\n", GetLastError());
    }
    printf("Client subject: %s\n", szName);
    if(!CertNameToStr(pServerCert->dwCertEncodingType,
                      &pServerCert->pCertInfo->Issuer,
                      CERT_X500_NAME_STR | CERT_NAME_STR_NO_PLUS_FLAG,
                      szName, sizeof(szName)))
    {
        printf("**** Error 0x%x building issuer name\n", GetLastError());
    }
    printf("Client issuer: %s\n\n", szName);

    // display certificate chain
    pCurrentCert = pServerCert;
    while(pCurrentCert != NULL)
    {
        dwVerificationFlags = 0;
        pIssuerCert = CertGetIssuerCertificateFromStore(pServerCert->hCertStore,
                                                        pCurrentCert,
                                                        NULL,
                                                        &dwVerificationFlags);
        if(pIssuerCert == NULL)
        {
            if(pCurrentCert != pServerCert)
            {
                CertFreeCertificateContext(pCurrentCert);
            }
            break;
        }

        if(!CertNameToStr(pIssuerCert->dwCertEncodingType,
                          &pIssuerCert->pCertInfo->Subject,
                          CERT_X500_NAME_STR | CERT_NAME_STR_NO_PLUS_FLAG,
                          szName, sizeof(szName)))
        {
            printf("**** Error 0x%x building subject name\n", GetLastError());
        }
        printf("CA subject: %s\n", szName);
        if(!CertNameToStr(pIssuerCert->dwCertEncodingType,
                          &pIssuerCert->pCertInfo->Issuer,
                          CERT_X500_NAME_STR | CERT_NAME_STR_NO_PLUS_FLAG,
                          szName, sizeof(szName)))
        {
            printf("**** Error 0x%x building issuer name\n", GetLastError());
        }
        printf("CA issuer: %s\n\n", szName);

        if(pCurrentCert != pServerCert)
        {
            CertFreeCertificateContext(pCurrentCert);
        }
        pCurrentCert = pIssuerCert;
        pIssuerCert = NULL;
    }
}

/*****************************************************************************/
static 
DWORD
VerifyClientCertificate(
    PCCERT_CONTEXT  pServerCert,
    DWORD           dwCertFlags)
{
    HTTPSPolicyCallbackData  polHttps;
    CERT_CHAIN_POLICY_PARA   PolicyPara;
    CERT_CHAIN_POLICY_STATUS PolicyStatus;
    CERT_CHAIN_PARA          ChainPara;
    PCCERT_CHAIN_CONTEXT     pChainContext = NULL;

    DWORD   Status;

    if(pServerCert == NULL)
    {
        return (DWORD)SEC_E_WRONG_PRINCIPAL;
    }


    //
    // Build certificate chain.
    //

    ZeroMemory(&ChainPara, sizeof(ChainPara));
    ChainPara.cbSize = sizeof(ChainPara);

    if(!CertGetCertificateChain(
                            NULL,
                            pServerCert,
                            NULL,
                            pServerCert->hCertStore,
                            &ChainPara,
                            CERT_CHAIN_CACHE_END_CERT|CERT_CHAIN_REVOCATION_CHECK_CHAIN,
                            NULL,
                            &pChainContext))
    {
        Status = GetLastError();
        printf("Error 0x%x returned by CertGetCertificateChain!\n", Status);
        goto cleanup;
    }


    //
    // Validate certificate chain.
    // 

    ZeroMemory(&polHttps, sizeof(HTTPSPolicyCallbackData));
    polHttps.cbStruct           = sizeof(HTTPSPolicyCallbackData);
    polHttps.dwAuthType         = AUTHTYPE_CLIENT;
    polHttps.fdwChecks          = dwCertFlags;
    polHttps.pwszServerName     = NULL;

    memset(&PolicyPara, 0, sizeof(PolicyPara));
    PolicyPara.cbSize            = sizeof(PolicyPara);
    PolicyPara.pvExtraPolicyPara = &polHttps;

    memset(&PolicyStatus, 0, sizeof(PolicyStatus));
    PolicyStatus.cbSize = sizeof(PolicyStatus);

    if(!CertVerifyCertificateChainPolicy(
                            CERT_CHAIN_POLICY_SSL,
                            pChainContext,
                            &PolicyPara,
                            &PolicyStatus))
    {
        Status = GetLastError();
        printf("Error 0x%x returned by CertVerifyCertificateChainPolicy!\n", Status);
        goto cleanup;
    }

    if(PolicyStatus.dwError)
    {
        Status = PolicyStatus.dwError;
        DisplayWinVerifyTrustError(Status); 
        goto cleanup;
    }


    Status = SEC_E_OK;

cleanup:

    if(pChainContext)
    {
        CertFreeCertificateChain(pChainContext);
    }

    return Status;
}

/*****************************************************************************/
static void
WebServer(CredHandle *phServerCreds)
{
    SOCKET Socket;
    SOCKADDR_IN address;
    SOCKADDR_IN remoteAddress;
    INT remoteSockaddrLength;
    DWORD cConnections = 0;
    INT err;
    HANDLE hThread;
    unsigned int dwThreadId;

    //
    // Set up a socket listening on the HTTPS port.
    //

    ListenSocket = socket( AF_INET, SOCK_STREAM, 0 );
    if ( ListenSocket == INVALID_SOCKET )
    {
        printf( "socket() failed for ListenSocket: %ld\n", GetLastError( ) );
        exit(1);
    }

    RtlZeroMemory( &address, sizeof(address) );
    address.sin_family = AF_INET;
    address.sin_port = htons( (short)iPortNumber );    // https port
    address.sin_addr.s_addr = 0;

    err = bind(ListenSocket, (PSOCKADDR) &address, sizeof(address));
    if (err == SOCKET_ERROR)
    {
        printf("bind failed: %ld\n", GetLastError());
        exit(1);
    }

    err = listen(ListenSocket, SOMAXCONN);
    if (err == SOCKET_ERROR)
    {
        printf("listen failed: %ld\n", GetLastError());
        exit(1);
    }


    //
    // Loop processing connections.
    //

    while (1)
    {
        PWEBSERVERARG pWebServerArg;

        //
        // First accept an incoming connection.
        //

	if (fVerbose >= 1)
        printf("\nWaiting for connection %d\n", ++cConnections);

        remoteSockaddrLength = sizeof(remoteAddress);

        Socket = accept(ListenSocket,
                        (LPSOCKADDR)&remoteAddress,
                        &remoteSockaddrLength);

        if(Socket == INVALID_SOCKET)
        {
	    if (WSAEINTR == GetLastError ())
	    {
		// Wait for all threads to end!!!
		//WaitForSingleObject (,INFINITE);
		if (fVerbose >= 1)
		    printf("Shutting down...\n");
		Sleep (2000);
		break;
	    }
            printf( "accept() failed: %ld\n", GetLastError( ) );
            exit(1);
        }
	
	if (fVerbose >= 1)
            printf("Socket connection established\n");

        
        while (InterlockedDecrement((LPLONG)&dwMaxThread) < 0) {
            InterlockedIncrement((LPLONG)&dwMaxThread);
        }

        pWebServerArg = malloc(sizeof(*pWebServerArg));
        pWebServerArg->mpServerCreds = phServerCreds;
        pWebServerArg->mSocket = Socket;
	hThread = (HANDLE) _beginthreadex (NULL, 0,
		(unsigned int (__stdcall *)(void *))WebServerThread,
		pWebServerArg, 0, &dwThreadId);
#if 0
        hThread = CreateThread( 
            NULL,                        // no security attributes 
            0,                           // use default stack size  
            WebServerThread,             // thread function 
            pWebServerArg,               // argument to thread function 
            0,                           // use default creation flags 
            &dwThreadId);                // returns the thread identifier 
#endif
 
        // Check the return value for success. 

        if (hThread == NULL) {
          printf("CreateThread failed.\n"); 
        } else {
          CloseHandle( hThread );
        }
    }

    if (ListenSocket != INVALID_SOCKET) closesocket (ListenSocket);
    ListenSocket = INVALID_SOCKET;

}   // WebServer

static unsigned int __stdcall
WebServerThread(LPVOID lpParameter) {

    CredHandle *phServerCreds = ((PWEBSERVERARG)lpParameter)->mpServerCreds;
    SOCKET      Socket = ((PWEBSERVERARG)lpParameter)->mSocket;

    INT err;
    INT i;
    
    CHAR objectName[256];
    DWORD currentDirectoryLength;
    HANDLE objectHandle;
    BY_HANDLE_FILE_INFORMATION fileInfo;
    INT bytesSent;
    DWORD bytesRead;
    DWORD cbContentLength;

    CtxtHandle      hContext;
    SecBufferDesc   Message;
    SecBuffer       Buffers[4];
    SecBufferDesc   MessageOut;
    SecBuffer       OutBuffers[4];
    SecPkgContext_StreamSizes Sizes;
    SECURITY_STATUS scRet;
    PCCERT_CONTEXT pRemoteCertContext = NULL;

    PSecBuffer pDataBuffer;
    DWORD cbIoBufferLength;

    CHAR IoBuffer[IO_BUFFER_SIZE];
    DWORD cbIoBuffer = 0;

    //
    // Figure out our current directory so that we can open files
    // relative to it.
    //

    currentDirectoryLength = GetCurrentDirectory( 256, objectName );
    if ( currentDirectoryLength == 0 )
    {
        printf( "GetCurrentDirectory failed: %ld\n", GetLastError( ) );
        exit(1);
    }


    //
    // Initialize security buffer structs
    //

    Message.ulVersion = SECBUFFER_VERSION;
    Message.cBuffers = 4;
    Message.pBuffers = Buffers;

    Buffers[0].BufferType = SECBUFFER_EMPTY;
    Buffers[1].BufferType = SECBUFFER_EMPTY;
    Buffers[2].BufferType = SECBUFFER_EMPTY;
    Buffers[3].BufferType = SECBUFFER_EMPTY;

    MessageOut.ulVersion = SECBUFFER_VERSION;
    MessageOut.cBuffers = 4;
    MessageOut.pBuffers = OutBuffers;

    OutBuffers[0].BufferType = SECBUFFER_EMPTY;
    OutBuffers[1].BufferType = SECBUFFER_EMPTY;
    OutBuffers[2].BufferType = SECBUFFER_EMPTY;
    OutBuffers[3].BufferType = SECBUFFER_EMPTY;



    hContext.dwLower = 0;
    hContext.dwUpper = 0;

    objectHandle = INVALID_HANDLE_VALUE;

    // 
    // Perform handshake
    //

    cbIoBuffer = 0;

    if(!SSPINegotiateLoop(Socket,
                      &hContext,
                      phServerCreds,
                      fClientAuth,
                      TRUE,
                      TRUE,
                      IoBuffer,
                      &cbIoBuffer))
    {
        printf("Couldn't connect\n");
        goto cleanup;
    }

    if(fClientAuth)
    {
        // Read the client certificate.
        scRet = g_SecurityFunc.QueryContextAttributes(&hContext,
                                        SECPKG_ATTR_REMOTE_CERT_CONTEXT,
                                        (PVOID)&pRemoteCertContext);
        if(scRet != SEC_E_OK)
        {
            printf("Error 0x%lx querying client certificate\n", scRet);
            goto cleanup;
        }
        else
        {
            // Display client certificate chain.
            if (fVerbose >= 1) {
                DisplayCertChain(pRemoteCertContext);
            }

            // Attempt to validate client certificate.
            scRet = VerifyClientCertificate(pRemoteCertContext, 0);
            if (scRet) {
                printf("Error 0x%lx authenticating client credentials\n", scRet);
                goto cleanup;
            } else if (fVerbose >= 1) {
                printf("\nAuth succeeded, ready for command\n");
            }

	    CertFreeCertificateContext (pRemoteCertContext);
            pRemoteCertContext = NULL;
        }
    }



    //
    // Find out how big the header will be:
    //

    scRet = g_SecurityFunc.QueryContextAttributes(&hContext, SECPKG_ATTR_STREAM_SIZES, &Sizes);


    if(scRet != SEC_E_OK)
    {
        printf("Couldn't get Sizes\n");
        goto cleanup;
    }


    //
    // Receive the HTTP request from the client.  Note the
    // assumption that the client will send the request all in one
    // chunk.
    //

    do
    {
        if (fVerbose >= 1 && cbIoBuffer > 0) {
            printf("X");
        }
        Buffers[0].pvBuffer = IoBuffer;
        Buffers[0].cbBuffer = cbIoBuffer;
        Buffers[0].BufferType = SECBUFFER_DATA;

        Buffers[1].BufferType = SECBUFFER_EMPTY;
        Buffers[2].BufferType = SECBUFFER_EMPTY;
        Buffers[3].BufferType = SECBUFFER_EMPTY;

        scRet = g_SecurityFunc.DecryptMessage(&hContext, &Message, 0, NULL);

        if(scRet == SEC_E_INCOMPLETE_MESSAGE)
        {
            err = recv(Socket, IoBuffer + cbIoBuffer, TLS_MIN(MAX_IOBSZ, IO_BUFFER_SIZE - cbIoBuffer), 0);
            if ((err == SOCKET_ERROR) || (err == 0))
            {
                printf("recv failed: %ld\n", GetLastError());
                goto cleanup;
            }

	    if (fVerbose >= 1)
	    {
		printf("\nReceived %d (request) bytes from client\n", err);
		if (fVerbose >= 3)
		{
		    PrintHexDump(err, (unsigned char *)(IoBuffer+cbIoBuffer));
		}
		else
		if (fVerbose >= 2)
		{
		    PrintHexDump(16, (unsigned char *)(IoBuffer+cbIoBuffer));
		}
	    }

            cbIoBuffer += err;
        }
    }
    while(scRet == SEC_E_INCOMPLETE_MESSAGE);

    if(scRet != SEC_E_OK)
    {
        printf("Couldn't decrypt, error %lx\n", scRet);
        goto cleanup;
    }
    cbIoBuffer = 0;


    // Technically, walk through the buffer desc, looking for the first
    // buffer labelled SECBUFFER_DATA.  Practically, it will be the second
    // buffer section.

    pDataBuffer = &Buffers[1];
    ((CHAR *) pDataBuffer->pvBuffer)[pDataBuffer->cbBuffer] = '\0';
    if (fVerbose >= 1)
    {
	printf("\nMessage is: '%s'\n", pDataBuffer->pvBuffer);
    }


    // Parse the request in order to determine the requested object.
    // Note that we only handle the GET verb in this server.

    if(!ParseRequest(
                    pDataBuffer->pvBuffer,
                    pDataBuffer->cbBuffer,
                    objectName+currentDirectoryLength,
                    &cbContentLength))
    {
        printf("Unable to parse message\n");
        goto cleanup;
    }


    if (fHttpsClientAuth)
    {
	if(!SSPINegotiateLoop(Socket,
			  &hContext,
			  phServerCreds,
			  TRUE,
			  FALSE,
			  FALSE,
                          IoBuffer,
                          &cbIoBuffer))
	{
	    printf("Couldn't connect\n");
	    goto cleanup;
	}
	do
	{
	    if (fVerbose >= 1 && cbIoBuffer > 0) {
		printf("X");
	    }
	    Buffers[0].pvBuffer = IoBuffer;
	    Buffers[0].cbBuffer = cbIoBuffer;
	    Buffers[0].BufferType = SECBUFFER_DATA;

	    Buffers[1].BufferType = SECBUFFER_EMPTY;
	    Buffers[2].BufferType = SECBUFFER_EMPTY;
	    Buffers[3].BufferType = SECBUFFER_EMPTY;

	    scRet = g_SecurityFunc.DecryptMessage(&hContext, &Message, 0, NULL);

	    if(scRet == SEC_E_INCOMPLETE_MESSAGE)
	    {
		err = recv(Socket, IoBuffer + cbIoBuffer, TLS_MIN(MAX_IOBSZ, IO_BUFFER_SIZE - cbIoBuffer), 0);
		if ((err == SOCKET_ERROR) || (err == 0))
		{
		    printf("recv failed: %ld\n", GetLastError());
		    goto cleanup;
		}

		if (fVerbose >= 1)
		{
		    printf("\nReceived %d (request) bytes from client\n", err);
		    if (fVerbose >= 3)
		    {
			PrintHexDump(err, (unsigned char *)(IoBuffer+cbIoBuffer));
		    }
		    else
		    if (fVerbose >= 2)
		    {
			PrintHexDump(16, (unsigned char *)(IoBuffer+cbIoBuffer));
		    }
		}

		cbIoBuffer += err;
	    }
	}
	while(scRet == SEC_E_INCOMPLETE_MESSAGE);

	if(scRet != SEC_I_RENEGOTIATE)
	{
	    printf("Couldn't decrypt, error %lx\n", scRet);
	    goto cleanup;
	}

        if (Buffers[3].BufferType == SECBUFFER_EXTRA)
        {
            memcpy(IoBuffer,
                   (LPBYTE) (IoBuffer + (cbIoBuffer - Buffers[3].cbBuffer)),
                    Buffers[3].cbBuffer);
            cbIoBuffer = Buffers[3].cbBuffer;
        }
        else
        {
            cbIoBuffer = 0;
        }

	if(!SSPINegotiateLoop(Socket,
			  &hContext,
			  phServerCreds,
			  TRUE,
			  FALSE,
			  FALSE,
                          IoBuffer,
                          &cbIoBuffer))
	{
	    printf("Couldn't connect\n");
	    goto cleanup;
	}

            // Read the client certificate.
            scRet = g_SecurityFunc.QueryContextAttributes(&hContext,
                                            SECPKG_ATTR_REMOTE_CERT_CONTEXT,
                                            (PVOID)&pRemoteCertContext);
            if(scRet != SEC_E_OK)
            {
                printf("Error 0x%lx querying client certificate\n", scRet);
                goto cleanup;
            }
            else
            {
                // Display client certificate chain.
                if (fVerbose >= 1) {
                    DisplayCertChain(pRemoteCertContext);
                }

                // Attempt to validate client certificate.
                scRet = VerifyClientCertificate(pRemoteCertContext, 0);
                if (scRet) {
                    printf("Error 0x%lx authenticating client credentials\n", scRet);
                    goto cleanup;
                } else if (fVerbose >= 1) {
                    printf("\nAuth succeeded, ready for command\n");
                }
               CertFreeCertificateContext (pRemoteCertContext);
               pRemoteCertContext = NULL;
            }

//	    fClientAuth = 1;
//	    fHttpsClientAuth = 0;
    }

    if (!strcmp (objectName+currentDirectoryLength, "\\shutdown"))
    {
        fShutdown = TRUE;
	closesocket (ListenSocket);
	ListenSocket = INVALID_SOCKET;
        goto cleanup;
    }

    objectHandle = CreateFileA(
                       objectName,
                       GENERIC_READ,
                       FILE_SHARE_READ,
                       NULL,
                       OPEN_EXISTING,
                       0,
                       NULL);
    if (objectHandle == INVALID_HANDLE_VALUE)
    {
        printf("CreateFile(%s) failed: %ld\n", objectName, GetLastError());
        goto cleanup;
    }

    // Determine the length of the file.

    if(!GetFileInformationByHandle(objectHandle, &fileInfo))
    {
        printf("GetFileInformationByHandle failed: %ld\n", GetLastError());
        goto cleanup;
    }

    //
    // Build and the HTTP response header.
    //

    ZeroMemory(IoBuffer, Sizes.cbHeader);

    i = sprintf(
        IoBuffer + Sizes.cbHeader,
        "HTTP/1.0 200 OK\r\nContent-Length: %d\r\n\r\n",
        fileInfo.nFileSizeLow);

    //
    // Line up the buffers so that the header and content will be
    // all set to go.
    //

    Buffers[0].pvBuffer = IoBuffer;
    Buffers[0].cbBuffer = Sizes.cbHeader;
    Buffers[0].BufferType = SECBUFFER_STREAM_HEADER;

    Buffers[1].pvBuffer = IoBuffer + Sizes.cbHeader;
    Buffers[1].cbBuffer = i;
    Buffers[1].BufferType = SECBUFFER_DATA;

    Buffers[2].pvBuffer = IoBuffer + Sizes.cbHeader + i;
    Buffers[2].cbBuffer = Sizes.cbTrailer;
    Buffers[2].BufferType = SECBUFFER_STREAM_TRAILER;

    Buffers[3].BufferType = SECBUFFER_EMPTY;

    scRet = g_SecurityFunc.EncryptMessage(&hContext, 0, &Message, 0);

    if ( FAILED( scRet ) )
    {
        printf(" EncryptMessage failed with %#x\n", scRet );
        goto cleanup;
    }


    err = send( Socket,
                IoBuffer,
                Buffers[0].cbBuffer + Buffers[1].cbBuffer + Buffers[2].cbBuffer,
                0 );

    if (fVerbose >= 1)
    {
	printf("\nSend %d header bytes to client\n", Buffers[0].cbBuffer + Buffers[1].cbBuffer + Buffers[2].cbBuffer);
	if (fVerbose >= 3)
	{
	    PrintHexDump(Buffers[0].cbBuffer + Buffers[1].cbBuffer + Buffers[2].cbBuffer, (PBYTE)IoBuffer);
	}
	else
	if (fVerbose >= 2)
	{
	    PrintHexDump(16, (PBYTE)IoBuffer);
	}
    }
    if ( err == SOCKET_ERROR )
    {
        printf( "send failed: %ld\n", GetLastError( ) );
        goto cleanup;
    }

    //
    // Now read and send the file data.
    //

    cbIoBufferLength = Sizes.cbMaximumMessage;
    //cbIoBufferLength = IO_BUFFER_SIZE;

    if (cbIoBufferLength > IO_BUFFER_SIZE - (Sizes.cbHeader + Sizes.cbTrailer)) 
	cbIoBufferLength = IO_BUFFER_SIZE - (Sizes.cbHeader + Sizes.cbTrailer);

    //cbIoBufferLength -= cbIoBufferLength % 1024 + Sizes.cbTrailer;
    //cbIoBufferLength = 1019;

    for(bytesSent = 0;
        bytesSent < (INT) fileInfo.nFileSizeLow;
        bytesSent += err)
    {
	DWORD BeginE = GetTickCount();
	static DWORD cR=0, sR=0;
	static DWORD cE=0, sE=0;
        if(!ReadFile(objectHandle,
                      IoBuffer + Sizes.cbHeader,
                      cbIoBufferLength,
                      &bytesRead,
                      NULL))
        {
            printf( "ReadFile failed: %ld\n", GetLastError( ) );
            break;
        }
	cR++; sR += GetTickCount()-BeginE;
	//if (!(cR%10)) printf ("Read: %d\n", sR/cR);

        if(bytesRead == 0)
        {
            printf( "zero bytes read\n");
            break;
        }


        Buffers[0].pvBuffer = IoBuffer;
        Buffers[0].cbBuffer = Sizes.cbHeader;
        Buffers[0].BufferType = SECBUFFER_STREAM_HEADER;

        Buffers[1].pvBuffer = IoBuffer + Sizes.cbHeader;
        Buffers[1].cbBuffer = bytesRead;
        Buffers[1].BufferType = SECBUFFER_DATA;

        Buffers[2].pvBuffer = IoBuffer + Sizes.cbHeader + bytesRead;
        Buffers[2].cbBuffer = Sizes.cbTrailer;
        Buffers[2].BufferType = SECBUFFER_STREAM_TRAILER;

        Buffers[3].BufferType = SECBUFFER_EMPTY;

	BeginE = GetTickCount();
        scRet = g_SecurityFunc.EncryptMessage(&hContext,
                            0,
                            &Message,
                            0);
	cE++; sE += GetTickCount()-BeginE;
	//if (!(cE%10)) printf ("Enc: %d\n", sE/cE);

        if ( FAILED( scRet ) )
        {
            printf(" EncryptMessage failed with %#x\n", scRet );
            goto cleanup;
        }

	if (fSendNothing)
	    err = Buffers[0].cbBuffer + Buffers[1].cbBuffer + Buffers[2].cbBuffer;
	else
	    err = send( Socket,
			IoBuffer,
			Buffers[0].cbBuffer + Buffers[1].cbBuffer + Buffers[2].cbBuffer,
			0 );

	if (fVerbose >= 1)
	{
	    printf("\nSend %d data bytes to client\n", Buffers[0].cbBuffer + Buffers[1].cbBuffer + Buffers[2].cbBuffer);
	    if (fVerbose >= 3)
	    {
		PrintHexDump(Buffers[0].cbBuffer + Buffers[1].cbBuffer + Buffers[2].cbBuffer, (PBYTE)IoBuffer);
	    }
	    else
	    if (fVerbose >= 2)
	    {
		PrintHexDump(16, (PBYTE)IoBuffer);
	    }
	}

        if ( err == SOCKET_ERROR )
        {
            printf( "send failed: %ld\n", GetLastError( ) );
            break;
        }
    }

cleanup:

    if(DisconnectFromClient(Socket, phServerCreds, &hContext))
    {
        printf("Error disconnecting from server\n");
    }

    if(objectHandle != INVALID_HANDLE_VALUE)
    {
        CloseHandle(objectHandle);
    }

    if (pRemoteCertContext) {
        CertFreeCertificateContext (pRemoteCertContext);
        pRemoteCertContext = NULL;
    }

    free(lpParameter);

    InterlockedIncrement((LPLONG)&dwMaxThread);

    return 0;
} // WebServerThread


static
BOOL
SSPINegotiateLoop(
    SOCKET          Socket,
    PCtxtHandle     phContext,
    PCredHandle     phCred,
    BOOL            fClientAuth,
    BOOL            fDoInitialRead,
    BOOL            NewContext,
    CHAR            IoBuffer[IO_BUFFER_SIZE],
    DWORD          *pcbIoBuffer)
{
    TimeStamp            tsExpiry;
    SECURITY_STATUS      scRet;
    SecBufferDesc        InBuffer;
    SecBufferDesc        OutBuffer;
    SecBuffer            InBuffers[2];
    SecBuffer            OutBuffers[1];
    DWORD                err;

    BOOL                 fDoRead;
    BOOL                 fInitContext = NewContext;

    DWORD                dwSSPIFlags, dwSSPIOutFlags;


    scRet = SEC_E_SECPKG_NOT_FOUND; //default error if we run out of packages
    err = 0;

    fDoRead = fDoInitialRead;

    dwSSPIFlags =   ASC_REQ_SEQUENCE_DETECT        |
                    ASC_REQ_REPLAY_DETECT      |
                    ASC_REQ_CONFIDENTIALITY  |
                    ASC_REQ_EXTENDED_ERROR    |
                    ASC_REQ_ALLOCATE_MEMORY  |
                    ASC_REQ_STREAM;

    if (fClientAuth)
        dwSSPIFlags |= ASC_REQ_MUTUAL_AUTH;

    if (fFragmentMessages)
	dwSSPIFlags |= ASC_REQ_FRAGMENT_SUPPLIED;


    //
    //  set OutBuffer for InitializeSecurityContext call
    //

    OutBuffer.cBuffers = 1;
    OutBuffer.pBuffers = OutBuffers;
    OutBuffer.ulVersion = SECBUFFER_VERSION;

    //
    // Check to see if we've done Client Authentication with
    //  this Web Server before we handshake.  If we have, we'll have a Client
    //  Certificate that we can use to make this secure connection.
    //





    scRet = SEC_I_CONTINUE_NEEDED;

    while( scRet == SEC_I_CONTINUE_NEEDED ||
            scRet == SEC_E_INCOMPLETE_MESSAGE ||
            scRet == SEC_I_INCOMPLETE_CREDENTIALS) 
    {

        if(0 == (*pcbIoBuffer) || scRet == SEC_E_INCOMPLETE_MESSAGE)
        {
            if(fDoRead)
            {
                err = recv(Socket, IoBuffer+(*pcbIoBuffer), TLS_MIN(MAX_IOBSZ, IO_BUFFER_SIZE-(*pcbIoBuffer)), 0);

                if (err == SOCKET_ERROR || err == 0)
                {
                    printf(" recv failed: %d\n", GetLastError() );
                    return FALSE;
                }
                else
                {
		    if (fVerbose >= 1)
		    {
			printf("\nReceived %d (handshake) bytes from client\n", err);
			if (fVerbose >= 3)
			{
			    PrintHexDump(err, (PBYTE)(IoBuffer+(*pcbIoBuffer)));
			}
			else
			if (fVerbose >= 2)
			{
			    PrintHexDump(min(16, err), (PBYTE)(IoBuffer+(*pcbIoBuffer)));
			}
		    }

                    (*pcbIoBuffer) += err;
                }
            }
            else
            {
                fDoRead = TRUE;
            }
        }





        //
        // InBuffers[1] is for getting extra data that
        //  SSPI/SCHANNEL doesn't proccess on this
        //  run around the loop.
        //

        InBuffers[0].pvBuffer = IoBuffer;
        InBuffers[0].cbBuffer = (*pcbIoBuffer);
        InBuffers[0].BufferType = SECBUFFER_TOKEN;

        InBuffers[1].pvBuffer   = NULL;
        InBuffers[1].cbBuffer   = 0;
        InBuffers[1].BufferType = SECBUFFER_EMPTY;

        InBuffer.cBuffers        = 2;
        InBuffer.pBuffers        = InBuffers;
        InBuffer.ulVersion       = SECBUFFER_VERSION;


        //
        // Initialize these so if we fail, pvBuffer contains NULL,
        // so we don't try to free random garbage at the quit
        //

        OutBuffers[0].pvBuffer   = NULL;
        OutBuffers[0].BufferType = SECBUFFER_TOKEN;
        OutBuffers[0].cbBuffer   = 0;


        scRet = g_SecurityFunc.AcceptSecurityContext(
                        phCred,
                        (fInitContext?NULL:phContext),
                        &InBuffer,
                        dwSSPIFlags,
                        SECURITY_NATIVE_DREP,
                        (fInitContext?phContext:NULL),
                        &OutBuffer,
                        &dwSSPIOutFlags,
                        &tsExpiry);

	if (scRet != SEC_E_INCOMPLETE_MESSAGE)
	    fInitContext = FALSE;

        if ( scRet == SEC_E_OK ||
             scRet == SEC_I_CONTINUE_NEEDED ||
             (FAILED(scRet) && (0 != (dwSSPIOutFlags & ISC_RET_EXTENDED_ERROR))))
        {
            if  (OutBuffers[0].cbBuffer != 0    &&
                 OutBuffers[0].pvBuffer != NULL )
            {
                //
                // Send response to server if there is one
                //
                err = send( Socket,
                            OutBuffers[0].pvBuffer,
                            OutBuffers[0].cbBuffer,
                            0 );

		if (fVerbose >= 1)
		{
		    printf("\nSend %d handshake bytes to client\n", OutBuffers[0].cbBuffer);
		    if (fVerbose >= 3)
		    {
			PrintHexDump(OutBuffers[0].cbBuffer, OutBuffers[0].pvBuffer);
		    }
		    else
		    if (fVerbose >= 2)
		    {
			PrintHexDump(16, OutBuffers[0].pvBuffer);
		    }
		}

                g_SecurityFunc.FreeContextBuffer( OutBuffers[0].pvBuffer );
                OutBuffers[0].pvBuffer = NULL;
            }
        }


        if ( scRet == SEC_E_OK )
        {


            if ( InBuffers[1].BufferType == SECBUFFER_EXTRA )
            {

                    memcpy(IoBuffer,
                           (LPBYTE) (IoBuffer + ((*pcbIoBuffer) - InBuffers[1].cbBuffer)),
                            InBuffers[1].cbBuffer);
                    (*pcbIoBuffer) = InBuffers[1].cbBuffer;
                    printf("S");
            }
            else
            {
                (*pcbIoBuffer) = 0;
            }

            if(fClientAuth)
            {
                // Display info about cert...


            }

            return TRUE;
        }
        else if (FAILED(scRet) && (scRet != SEC_E_INCOMPLETE_MESSAGE))
        {

            printf("Accept Security Context Failed with error code %lx\n", scRet);
            return FALSE;

        }



        if ( scRet != SEC_E_INCOMPLETE_MESSAGE &&
             scRet != SEC_I_INCOMPLETE_CREDENTIALS)
        {


            if ( InBuffers[1].BufferType == SECBUFFER_EXTRA )
            {



                memcpy(IoBuffer,
                       (LPBYTE) (IoBuffer + ((*pcbIoBuffer) - InBuffers[1].cbBuffer)),
                        InBuffers[1].cbBuffer);
                (*pcbIoBuffer) = InBuffers[1].cbBuffer;
                printf("A");
            }
            else
            {
                //
                // prepare for next receive
                //

                (*pcbIoBuffer) = 0;
            }
        }
    }

    return FALSE;
}



static
BOOL
ParseRequest (
    IN PCHAR InputBuffer,
    IN INT InputBufferLength,
    OUT PCHAR ObjectName,
    OUT DWORD *pcbContentLength)
{
    PCHAR s = InputBuffer;
    DWORD i;

    *pcbContentLength = 0;

    while ( (INT)(s - InputBuffer) < InputBufferLength )
    {

        // Parse off verb

        //
        // First determine whether this line starts with the GET
        // verb.
        //

        while(*s != '\0' && *s != ' ' && *s != '\t')
        {
            *s = (char)toupper(*s);
            s++;
        }

        if(*s == '\0' || *s == ' ' || *s == '\0')
        {
            *s = '\0';
//            printf("Verb is :%s\n", InputBuffer);

            //
            // It is a GET.  Skip over white space.
            //
            for ( s++; *s == ' ' || *s == '\t'; s++ );

            //
            // Now grab the object name.
            //

            for ( i = 0; *s != 0xA && *s != 0xD && *s != ' ' && *s != '\0'; s++, i++ ) {
                ObjectName[i] = *s;
                if ( ObjectName[i] == '/' ) {
                    ObjectName[i] = '\\';
                }
            }

            ObjectName[i] = '\0';

            //
            // We're done parsing.
            //

            if(strcmp(ObjectName, "\\") == 0)
            {
                strcpy(ObjectName, "\\default.html");
            }
            if(strcmp(InputBuffer, "POST") == 0)
            {
                char * content_length;
                DWORD cbContent = 0;
                // look for content length;
                content_length = strstr(s, "Content-Length: ");
                if(content_length)
                {
                    cbContent = atoi(content_length+16);
                    printf("Content Length is %d\n", cbContent);
                    *pcbContentLength = cbContent;
                }
            }
            return TRUE;
        }

        //
        // Skip to the end of the line and continue parsing.
        //

        while ( *s != 0xA && *s != 0xD )
        {
            s++;
        }

        s++;

        if ( *s == 0xD || *s == 0xA )
        {
            s++;
        }
    }

    return FALSE;

} // ParseRequest

/*****************************************************************************/
static
DWORD
CreateCredentials(
    LPSTR pszUserName,              // in
    PCredHandle phCreds)            // out
{
    SCHANNEL_CRED   SchannelCred;
    TimeStamp       tsExpiry;
    SECURITY_STATUS Status;
    PCCERT_CONTEXT  pCertContext = NULL;
#if 0
    HMODULE	    hIisCrMap = NULL;
    //struct _HMAPPER * phMapper = NULL;
    SCHANNEL_MAPPER * phMapper = NULL;
    MapperCreateInstanceFunc MapperCreateInstance = NULL;
#endif
    SecPkgCred_SupportedAlgs sup_algs;
    SecPkgCred_CipherStrengths ciph_str;
    SecPkgCred_SupportedProtocols sup_proto;
    DWORD i;

    if(pszUserName == NULL || strlen(pszUserName) == 0)
    {
        printf("**** No user name specified!\n");
        return (DWORD)SEC_E_NO_CREDENTIALS;
    }

    // Open the "MY" certificate store.
    if(hMyCertStore == NULL)
    {
        if(fMachineStore)
        {
            hMyCertStore = CertOpenStore(CERT_STORE_PROV_SYSTEM,
                                         X509_ASN_ENCODING,
                                         0,
                                         CERT_STORE_OPEN_EXISTING_FLAG|CERT_STORE_READONLY_FLAG|
                                         CERT_SYSTEM_STORE_LOCAL_MACHINE,
                                         L"MY");
        }
        else
        {
            /*hMyCertStore = CertOpenSystemStore(0, "MY");*/
            hMyCertStore = CertOpenStore(
			        CERT_STORE_PROV_SYSTEM, /* LPCSTR lpszStoreProvider*/
			        0,			    /* DWORD dwMsgAndCertEncodingType*/
			        0,			    /* HCRYPTPROV hCryptProv*/
			        CERT_STORE_OPEN_EXISTING_FLAG|CERT_STORE_READONLY_FLAG|
			        CERT_SYSTEM_STORE_CURRENT_USER, /* DWORD dwFlags*/
			        L"MY"		    /* const void *pvPara*/
			        );
        }

        if(!hMyCertStore)
        {
            printf("**** Error 0x%x returned by CertOpenSystemStore\n", 
                GetLastError());
            return (DWORD)SEC_E_NO_CREDENTIALS;
        }
    }

    // Find certificate. Note that this sample just searchs for a 
    // certificate that contains the user name somewhere in the subject name.
    // A real application should be a bit less casual.
    pCertContext = CertFindCertificateInStore(hMyCertStore, 
                                              X509_ASN_ENCODING, 
                                              0,
                                              CERT_FIND_SUBJECT_STR_A,
                                              pszUserName,
                                              NULL);
    if(pCertContext == NULL)
    {
        printf("**** Error 0x%x returned by CertFindCertificateInStore\n",
            GetLastError());
        return (DWORD)SEC_E_NO_CREDENTIALS;
    }


    //
    // Build Schannel credential structure. Currently, this sample only
    // specifies the protocol to be used (and optionally the certificate, 
    // of course). Real applications may wish to specify other parameters 
    // as well.
    //

    ZeroMemory(&SchannelCred, sizeof(SchannelCred));

    SchannelCred.dwVersion = SCHANNEL_CRED_VERSION;

    SchannelCred.cCreds = 1;
    SchannelCred.paCred = &pCertContext;

    SchannelCred.grbitEnabledProtocols = dwProtocol;

#if 0
    _chdir ("F:\\WINNT\\system32\\inetsrv");
    hIisCrMap = LoadLibrary ("iiscrmap.dll");
    if (!hIisCrMap)
    {
        printf("**** Error 0x%x returned by LoadLibrary (iiscrmap.dll)\n",
            GetLastError());
        return (DWORD)SEC_E_NO_CREDENTIALS;
    }
    MapperCreateInstance = (MapperCreateInstanceFunc) GetProcAddress (hIisCrMap, "CreateInstance");
    if (!MapperCreateInstance)
    {
        printf("**** Error 0x%x returned by GetProcAddress (iiscrmap.dll, CreateInstance)\n",
            GetLastError());
        return (DWORD)SEC_E_NO_CREDENTIALS;
    }
    Status = MapperCreateInstance (&phMapper);
    if (Status)
    {
        printf("**** Error 0x%x returned by MapperCreateInstance\n",
            Status);
        return (DWORD)SEC_E_NO_CREDENTIALS;
    }
    SchannelCred.cMappers = 1;
    SchannelCred.aphMappers = (struct _HMAPPER **)&phMapper;

    phMapper->vtable->ReferenceMapper (phMapper);
    phMapper->vtable->DeReferenceMapper (phMapper);
#endif

    //
    // Create an SSPI credential.
    //

    Status = g_SecurityFunc.AcquireCredentialsHandle(
                        NULL,                   // Name of principal
                        UNISP_NAME_A,           // Name of package
                        SECPKG_CRED_INBOUND,    // Flags indicating use
                        NULL,                   // Pointer to logon ID
                        &SchannelCred,          // Package specific data
                        NULL,                   // Pointer to GetKey() func
                        NULL,                   // Value to pass to GetKey()
                        phCreds,                // (out) Cred Handle
                        &tsExpiry);             // (out) Lifetime (optional)

    if(Status != SEC_E_OK)
    {
        printf("**** Error 0x%x returned by AcquireCredentialsHandle\n", Status);
	goto err;
    }

    Status = g_SecurityFunc.QueryCredentialsAttributes(
	    phCreds,
	    SECPKG_ATTR_SUPPORTED_ALGS,
	    &sup_algs);

    if(Status != SEC_E_OK)
	printf("**** Error 0x%x returned by QueryCredentialsAttributes (SECPKG_ATTR_SUPPORTED_ALGS)\n", Status);
    else
    {
	if (fVerbose >= 1)
	{
	    printf("%d algorithms supported:\n", sup_algs.cSupportedAlgs);
	    for (i=0; i < sup_algs.cSupportedAlgs; i++)
	    {                                                        
		PCCRYPT_OID_INFO oid_info = CryptFindOIDInfo (CRYPT_OID_INFO_ALGID_KEY, sup_algs.palgSupportedAlgs+i, 0);
		if (oid_info)
		    printf("[%d] %s (%ls)\n", i, oid_info->pszOID, oid_info->pwszName);
		else
		    printf("[%d] 0x%X\n", i, sup_algs.palgSupportedAlgs[i]);
	    }
	}

	g_SecurityFunc.FreeContextBuffer (sup_algs.palgSupportedAlgs);
    }
	
    Status = g_SecurityFunc.QueryCredentialsAttributes(
	    phCreds,
	    SECPKG_ATTR_CIPHER_STRENGTHS,
	    &ciph_str);

    if(Status != SEC_E_OK)
	printf("**** Error 0x%x returned by QueryCredentialsAttributes (SECPKG_ATTR_CIPHER_STRENGTHS)\n", Status);
    else
    {
	if (fVerbose >= 1)
	    printf("Cipher strengths: %d..%d\n", ciph_str.dwMinimumCipherStrength, ciph_str.dwMaximumCipherStrength);
    }

    Status = g_SecurityFunc.QueryCredentialsAttributes(
	    phCreds,
	    SECPKG_ATTR_SUPPORTED_PROTOCOLS,
	    &sup_proto);

    if(Status != SEC_E_OK)
	printf("**** Error 0x%x returned by QueryCredentialsAttributes (SECPKG_ATTR_SUPPORTED_PROTOCOLS)\n", Status);
    else
    {
	if (fVerbose >= 1)
	    printf("Supported protocols: 0x%X\n", sup_proto.grbitProtocol);
    }

#if 0
    if (phMapper)
	((SCHANNEL_MAPPER*)phMapper)->vtable->DeReferenceMapper (phMapper);
#endif

err:
    //
    // Free the certificate context. Schannel has already made its own copy.
    //
    if(pCertContext)
        CertFreeCertificateContext(pCertContext);

    return Status;
}

/*****************************************************************************/
static
LONG
DisconnectFromClient(
    SOCKET          Socket, 
    PCredHandle     phCreds,
    CtxtHandle *    phContext)
{
    DWORD           dwType;
    PBYTE           pbMessage;
    DWORD           cbMessage;
    DWORD           cbData;

    SecBufferDesc   OutBuffer;
    SecBuffer       OutBuffers[1];
    DWORD           dwSSPIFlags;
    DWORD           dwSSPIOutFlags;
    TimeStamp       tsExpiry;
    DWORD           Status;

    //
    // Notify schannel that we are about to close the connection.
    //

    dwType = SCHANNEL_SHUTDOWN;

    OutBuffers[0].pvBuffer   = &dwType;
    OutBuffers[0].BufferType = SECBUFFER_TOKEN;
    OutBuffers[0].cbBuffer   = sizeof(dwType);

    OutBuffer.cBuffers  = 1;
    OutBuffer.pBuffers  = OutBuffers;
    OutBuffer.ulVersion = SECBUFFER_VERSION;

    Status = g_SecurityFunc.ApplyControlToken(phContext, &OutBuffer);

    if(FAILED(Status)) 
    {
        printf("**** Error 0x%x returned by ApplyControlToken\n", Status);
        goto cleanup;
    }

    //
    // Build an SSL close notify message.
    //

    dwSSPIFlags =   ASC_REQ_SEQUENCE_DETECT     |
                    ASC_REQ_REPLAY_DETECT       |
                    ASC_REQ_CONFIDENTIALITY     |
                    ASC_REQ_EXTENDED_ERROR      |
                    ASC_REQ_ALLOCATE_MEMORY     |
                    ASC_REQ_STREAM;


    OutBuffers[0].pvBuffer   = NULL;
    OutBuffers[0].BufferType = SECBUFFER_TOKEN;
    OutBuffers[0].cbBuffer   = 0;

    OutBuffer.cBuffers  = 1;
    OutBuffer.pBuffers  = OutBuffers;
    OutBuffer.ulVersion = SECBUFFER_VERSION;

    Status = g_SecurityFunc.AcceptSecurityContext(
                    phCreds,
                    phContext,
                    NULL,
                    dwSSPIFlags,
                    SECURITY_NATIVE_DREP,
                    NULL,
                    &OutBuffer,
                    &dwSSPIOutFlags,
                    &tsExpiry);

    if(FAILED(Status)) 
    {
        printf("**** Error 0x%x returned by AcceptSecurityContext\n", Status);
        goto cleanup;
    }

    pbMessage = OutBuffers[0].pvBuffer;
    cbMessage = OutBuffers[0].cbBuffer;


    //
    // Send the close notify message to the client.
    //

    if(pbMessage != NULL && cbMessage != 0)
    {
	if (fSendNothing)
	    cbData = cbMessage;
	else
	    cbData = send(Socket, (char *)pbMessage, cbMessage, 0);
        if(cbData == SOCKET_ERROR || cbData == 0)
        {
            Status = WSAGetLastError();
            printf("**** Error %d sending close notify\n", Status);
            goto cleanup;
        }

	if (fVerbose >= 1)
	{
	    printf("\n%d bytes of handshake data sent\n", cbData);
	    if (fVerbose >= 3)
	    {
		PrintHexDump(cbData, pbMessage);
	    }
	    else
	    if (fVerbose >= 2)
	    {
		PrintHexDump(min(16, cbData), pbMessage);
	    }
	}

        // Free output buffer.
        g_SecurityFunc.FreeContextBuffer(pbMessage);
    }
    

cleanup:

    // Free the security context.
    g_SecurityFunc.DeleteSecurityContext(phContext);

    // Close the socket.
    closesocket(Socket);

    return Status;
}
