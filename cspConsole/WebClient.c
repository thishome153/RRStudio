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
 * \file $RCSfile: WebClient.c,v $
 * \version $Revision: 1.21 $
 * \date $Date: 2002/05/23 14:44:39 $
 * \author $Author: chudov $
 *
 * \brief Schannel web client sample application.
 *
 * Подробное описание модуля.
 */
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

#if !defined(SEC_I_CONTEXT_EXPIRED)
#define SEC_I_CONTEXT_EXPIRED	_HRESULT_TYPEDEF_(0x00090317L)
#endif /* !defined(SEC_I_CONTEXT_EXPIRED) */

extern SecurityFunctionTable g_SecurityFunc;
extern BOOL LoadSecurityLibrary();
extern void UnloadSecurityLibrary();
extern void PrintHexDump(DWORD length, PBYTE buffer);
extern void DisplayWinVerifyTrustError (DWORD Status);

#pragma warning (disable: 4127)

#ifndef SEC_I_CONTEXT_EXPIRED
#define SEC_I_CONTEXT_EXPIRED            ((HRESULT)0x00090317L)
#endif /* SEC_I_CONTEXT_EXPIRED */

#define IO_BUFFER_SIZE  0x10000
#define TLS_MIN(a,b)        ((a) < (b) ? (a) : (b))
#define MAX_IOBSZ           (gen_iobsz())

static LPSTR    pszProxyServer  = "proxy";
static INT      iProxyPort      = 80;
static INT      noCache         = 0;

// User options.
static LPSTR    pszServerName   = NULL;
static INT      iPortNumber     = 443;
static LPSTR    pszFileName     = "default.htm";
static int      fVerbose        = 0;
static BOOL     fUseProxy       = FALSE;
static BOOL    fFragmentMessages = FALSE;
static LPSTR    pszUserName     = NULL;
static DWORD    dwProtocol      = 0;
static ALG_ID   aiKeyExch       = 0;
static DWORD    cbDataReceived  = 0;

static HCERTSTORE      hMyCertStore = NULL;
static SCHANNEL_CRED   SchannelCred;

static DWORD    min_gen_iobsz = 1000000;
static DWORD    max_gen_iobsz = 1000000;
static DWORD    p_sleep = 0;
static DWORD    dwMaxThread = 1;

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
SECURITY_STATUS
CreateCredentials(
    LPSTR pszUserName,
    PCredHandle phCreds);

static INT
ConnectToServer(
    LPSTR pszServerName,
    INT   iPortNumber,
    SOCKET *pSocket);

static
SECURITY_STATUS
PerformClientHandshake(
    SOCKET          Socket,
    PCredHandle     phCreds,
    LPSTR           pszServerName,
    CtxtHandle *    phContext,
    SecBuffer *     pExtraData);

static
SECURITY_STATUS
ClientHandshakeLoop(
    SOCKET          Socket,
    PCredHandle     phCreds,
    CtxtHandle *    phContext,
    BOOL            fDoInitialRead,
    SecBuffer *     pExtraData);

static
SECURITY_STATUS
HttpsGetFile(
    SOCKET          Socket,
    PCredHandle     phCreds,
    CtxtHandle *    phContext,
    LPSTR           pszFileName);

static 
void
DisplayCertChain(
    PCCERT_CONTEXT  pServerCert,
    BOOL            fLocal);

static 
DWORD
VerifyServerCertificate(
    PCCERT_CONTEXT  pServerCert,
    PSTR            pszServerName,
    DWORD           dwCertFlags);

/*
static void
WriteDataToFile(
    PSTR  pszFilename,
    PBYTE pbData,
    DWORD cbData);
*/
static
LONG
DisconnectFromServer(
    SOCKET          Socket, 
    PCredHandle     phCreds,
    CtxtHandle *    phContext);

static void
DisplayConnectionInfo(
    CtxtHandle *phContext);

static void
GetNewClientCredentials(
    CredHandle *phCreds,
    CtxtHandle *phContext);

double 
DiffTime (SYSTEMTIME * start, SYSTEMTIME * end)
{
    double ret;
    if (start->wDayOfWeek > end->wDayOfWeek) end->wDayOfWeek+=7;
    ret=(end->wDayOfWeek-start->wDayOfWeek)*24;
    
    ret=(ret+end->wHour-start->wHour)*60;
    ret=(ret+end->wMinute-start->wMinute)*60;
    ret=(ret+end->wSecond-start->wSecond);
    ret+=(end->wMilliseconds-start->wMilliseconds)/1000.0;
    return ret;
}


/*****************************************************************************/
int
main_tlsc(int argc, char *argv[])
{
    WSADATA WsaData;
    SOCKET  Socket;

    int nConn = 1;
    CredHandle hClientCreds;
    CtxtHandle hContext;
    SecBuffer  ExtraData;
    SECURITY_STATUS Status;

    PCCERT_CONTEXT pRemoteCertContext = NULL;
    PCCERT_CONTEXT pCurrRemoteCertContext = NULL;

    SYSTEMTIME start,end,end1;

    int	    ret = 1;
    int	    print_help = 0;
    int    c;
   
    /*-----------------------------------------------------------------------------*/
    /* определение опций разбора параметров*/
    static struct option long_options[] = {
/*	{"cert",	required_argument,	NULL, 'x'},*/
	{"help",	no_argument,		NULL, 'h'},
	{"server",	required_argument,	NULL, 's'},
	{"port",	required_argument,	NULL, 'p'},
	{"proto",	required_argument,	NULL, 'P'},
	{"exchange",	required_argument,	NULL, 'E'},
	{"file",	required_argument,	NULL, 'f'},
	{"username",	required_argument,	NULL, 'u'},
	{"reconnects",	required_argument,	NULL, 'n'},
	{"verbose",	no_argument,		NULL, 'v'},
	{"nocache",	no_argument,		NULL, 'c'},
	{"x",		no_argument,		NULL, 'x'},
        {"fragment",    no_argument,		NULL, 'F'},
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
	    ret = 0;
	    print_help = 1;
	    goto bad;
        case 's':
            pszServerName = optarg;
            break;
        case 'p':
            iPortNumber = atoi(optarg);
            break;
        case 'n':
            nConn = atoi(optarg);
            break;
        case 'f':
            pszFileName = optarg;
            break;
        case 'v':
            fVerbose++;
            break;
        case 'c':
	    noCache = TRUE;
	    break;
        case 'x':
            fUseProxy = TRUE;
            break;
        case 'u':
            pszUserName = optarg;
            break;
        case 'F':
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
        case 'E':
            switch(atoi(optarg))
            {
                case 1:
                    aiKeyExch = CALG_RSA_KEYX;
                    break;
                case 2:
                    aiKeyExch = CALG_DH_EPHEM;
                    break;
                case 3:
                    aiKeyExch = CALG_DH_EX_SF;
                    break;
                default:
                    aiKeyExch = 0;
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
        return 1;
    }

    //
    // Initialize the WinSock subsystem.
    //

    if(WSAStartup(0x0101, &WsaData) == SOCKET_ERROR)
    {
        printf("Error %d returned by WSAStartup\n", GetLastError());
        return 1;
    }

    //
    // Create credentials.
    //

    if(CreateCredentials(pszUserName, &hClientCreds))
    {
        printf("Error creating credentials\n");
        return 1;
    }


    GetSystemTime(&start);

    {
    int iConn;
    for (iConn = 0; iConn < nConn; iConn++)
    {
    //
    // Connect to server.
    //

    if(ConnectToServer(pszServerName, iPortNumber, &Socket))
    {
        printf("Error connecting to server\n");
        return 1;
    }


    //
    // Perform handshake
    //

    if(PerformClientHandshake(Socket,
                              &hClientCreds,
                              pszServerName,
                              &hContext,
                              &ExtraData))
    {
        printf("Error performing handshake\n");
        closesocket(Socket);
        return 1;
    }


    //
    // Authenticate server's credentials.
    //

    // Get server's certificate.
    Status = g_SecurityFunc.QueryContextAttributes(&hContext,
                                    SECPKG_ATTR_REMOTE_CERT_CONTEXT,
                                    (PVOID)&pRemoteCertContext);
    if(Status != SEC_E_OK)
    {
        printf("Error 0x%x querying remote certificate\n", Status);
        closesocket(Socket);
        return 1;
    }

    if (!pCurrRemoteCertContext
      || pCurrRemoteCertContext->cbCertEncoded != pRemoteCertContext->cbCertEncoded
      || memcmp (pCurrRemoteCertContext->pbCertEncoded, pRemoteCertContext->pbCertEncoded, pCurrRemoteCertContext->cbCertEncoded)
      || 1
      )
    {
	// Display server certificate chain.
	if (fVerbose >= 1)
	    DisplayCertChain(pRemoteCertContext, FALSE);

	// Attempt to validate server certificate.
	Status = VerifyServerCertificate(pRemoteCertContext,
					 pszServerName,
					 0);
	if(Status)
	{
	    printf("**** Error authenticating server credentials!\n");

	    //
	    // At this point, the client could decide to not continue
	    //
	}
    }

    if (pCurrRemoteCertContext)
	CertFreeCertificateContext (pCurrRemoteCertContext);
    pCurrRemoteCertContext = pRemoteCertContext;

    //
    // Display connection info. 
    //

    if (fVerbose >= 1)
	DisplayConnectionInfo(&hContext);


    //
    // Read file from server.
    //

    if(HttpsGetFile(Socket, 
                    &hClientCreds,
                    &hContext, 
                    pszFileName))
    {
        printf("Error fetching file from server\n");
	closesocket(Socket);
        return 1;
    }

    //
    // Cleanup.
    //

    if(DisconnectFromServer(Socket, &hClientCreds, &hContext))
    {
        printf("Error disconnecting from server\n");
    }

    if (!iConn) GetSystemTime(&end1);
    }
    }

    GetSystemTime(&end);

    if (pCurrRemoteCertContext)
	CertFreeCertificateContext (pCurrRemoteCertContext);

    // Free SSPI credentials handle.
    g_SecurityFunc.FreeCredentialsHandle(&hClientCreds);

    // Shutdown WinSock subsystem.
    WSACleanup();

    // Close "MY" certificate store.
    if(hMyCertStore)
    {
        CertCloseStore(hMyCertStore, 0);
    }

    printf("%d connections, %d bytes in %.3f seconds;\n", nConn, cbDataReceived, DiffTime (&start, &end));
    if (nConn > 1)
    {
	printf("First connection: %.3f seconds;\n", DiffTime (&start, &end1));
	printf("Other connections: %.3f seconds;\n", DiffTime (&end1, &end)/(nConn-1));
    }
    return 0;

bad:
    if (print_help) {
	fprintf(stderr,"%s -tlsc [options]\n", prog);
	fprintf(stderr,SoftName " test tls (client)\n");
	fprintf(stderr,"options:\n");
	fprintf(stderr,"  -server name       DNS name of server.\n");
	fprintf(stderr,"  -port num          Port that server is listing on (default 443).\n");
	fprintf(stderr,"  -reconnects num    Number of consequent requests.\n");
	fprintf(stderr,"  -file name         Name of file to retrieve (default \"%s\")\n", pszFileName);
	fprintf(stderr,"  -verbose           Verbose Mode.\n");
	fprintf(stderr,"  -x                 Connect via the \"%s\" proxy server.\n", pszProxyServer);
	fprintf(stderr,"\n");
	fprintf(stderr,"  -proto num         Protocol to use\n");
	fprintf(stderr,"                        1 = PCT 1.0         3 = SSL 3.0\n");
	fprintf(stderr,"                        2 = SSL 2.0         4 = TLS 1.0\n");
	fprintf(stderr,"\n");
	fprintf(stderr,"  -exchange num      Key exchange algorithm to use\n");
	fprintf(stderr,"                        1 = RSA             2 = DH\n");
	fprintf(stderr,"                        3 = GOST\n");
	fprintf(stderr,"\n");
	fprintf(stderr,"  For client auth\n");
	fprintf(stderr,"  -user name         Name of user (CN=) auth by default\n");
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
SECURITY_STATUS
CreateCredentials(
    LPSTR pszUserName,              // in
    PCredHandle phCreds)            // out
{
    TimeStamp       tsExpiry;
    SECURITY_STATUS Status;

    DWORD           cSupportedAlgs = 0;
    ALG_ID          rgbSupportedAlgs[16];

    PCCERT_CONTEXT  pCertContext = NULL;

    // Open the "MY" certificate store, which is where Internet Explorer
    // stores its client certificates.
    if(hMyCertStore == NULL)
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

        if(!hMyCertStore)
        {
            printf("**** Error 0x%x returned by CertOpenSystemStore\n", 
                GetLastError());
            return SEC_E_NO_CREDENTIALS;
        }
    }

    //
    // If a user name is specified, then attempt to find a client
    // certificate. Otherwise, just create a NULL credential.
    //

    if(pszUserName)
    {
        // Find client certificate. Note that this sample just searchs for a 
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
            return SEC_E_NO_CREDENTIALS;
        }
    }


    //
    // Build Schannel credential structure. Currently, this sample only
    // specifies the protocol to be used (and optionally the certificate, 
    // of course). Real applications may wish to specify other parameters 
    // as well.
    //

    ZeroMemory(&SchannelCred, sizeof(SchannelCred));

    SchannelCred.dwVersion  = SCHANNEL_CRED_VERSION;
    if(pCertContext)
    {
        SchannelCred.cCreds     = 1;
        SchannelCred.paCred     = &pCertContext;
    }

    SchannelCred.grbitEnabledProtocols = dwProtocol;

    if(aiKeyExch)
    {
        rgbSupportedAlgs[cSupportedAlgs++] = aiKeyExch;
    }

    if(cSupportedAlgs)
    {
        SchannelCred.cSupportedAlgs    = cSupportedAlgs;
        SchannelCred.palgSupportedAlgs = rgbSupportedAlgs;
    }

    SchannelCred.dwFlags |= SCH_CRED_NO_DEFAULT_CREDS;
    SchannelCred.dwFlags |= SCH_CRED_MANUAL_CRED_VALIDATION;


    //
    // Create an SSPI credential.  
    //


    Status = g_SecurityFunc.AcquireCredentialsHandleW(
                        NULL,                   // Name of principal    
                        UNISP_NAME_A,           // Name of package
                        SECPKG_CRED_OUTBOUND,   // Flags indicating use
                        NULL,                   // Pointer to logon ID
                        &SchannelCred,          // Package specific data
                        NULL,                   // Pointer to GetKey() func
                        NULL,                   // Value to pass to GetKey()
                        phCreds,                // (out) Cred Handle
                        &tsExpiry);             // (out) Lifetime (optional)
    if(Status != SEC_E_OK)
    {
        printf("**** Error 0x%x returned by AcquireCredentialsHandle\n", Status);
        return Status;
    }


    //
    // Free the certificate context. Schannel has already made its own copy.
    //

    if(pCertContext)
    {
        CertFreeCertificateContext(pCertContext);
    }


    return SEC_E_OK;
}

/*****************************************************************************/
static INT
ConnectToServer(
    LPSTR    pszServerName, // in
    INT      iPortNumber,   // in
    SOCKET * pSocket)       // out
{
    SOCKET Socket;
    struct sockaddr_in sin;
    struct hostent *hp;

    Socket = socket(PF_INET, SOCK_STREAM, 0);
    if(Socket == INVALID_SOCKET)
    {
        printf("**** Error %d creating socket\n", WSAGetLastError());
        return WSAGetLastError();
    }

    if(fUseProxy)
    {
        sin.sin_family = AF_INET;
        sin.sin_port = ntohs((u_short)iProxyPort);

        if((hp = gethostbyname(pszProxyServer)) == NULL)
        {
            printf("**** Error %d returned by gethostbyname\n", WSAGetLastError());
            return WSAGetLastError();
        }
        else
        {
            memcpy(&sin.sin_addr, hp->h_addr, 4);
        }
    }
    else
    {
        sin.sin_family = AF_INET;
        sin.sin_port = htons((u_short)iPortNumber);

        if((hp = gethostbyname(pszServerName)) == NULL)
        {
            printf("**** Error %d returned by gethostbyname\n", WSAGetLastError());
            return WSAGetLastError();
        }
        else
        {
            memcpy(&sin.sin_addr, hp->h_addr, 4);
        }
    }

    if(connect(Socket, (struct sockaddr *)&sin, sizeof(sin)) == SOCKET_ERROR)
    {
        printf("**** Error %d connecting to \"%s\" (%s)\n", 
            WSAGetLastError(),
            pszServerName, 
            inet_ntoa(sin.sin_addr));
        closesocket(Socket);
        return WSAGetLastError();
    }

    if(fUseProxy)
    {
        BYTE  pbMessage[200]; 
        DWORD cbMessage;

        // Build message for proxy server
        strcpy((char *)pbMessage, "CONNECT ");
        strcat((char *)pbMessage, pszServerName);
        strcat((char *)pbMessage, ":");
        _itoa(iPortNumber, (char *)(pbMessage + strlen((char *)pbMessage)), 10);
        strcat((char *)pbMessage, " HTTP/1.0\r\nUser-Agent: webclient\r\n\r\n");
        cbMessage = (DWORD)strlen((char *)pbMessage);

        // Send message to proxy server
        if(send(Socket, (char *)pbMessage, cbMessage, 0) == SOCKET_ERROR)
        {
            printf("**** Error %d sending message to proxy!\n", WSAGetLastError());
            return WSAGetLastError();
        }

        // Receive message from proxy server
        cbMessage = recv(Socket, (char *)pbMessage, 200, 0);
        if(cbMessage == SOCKET_ERROR)
        {
            printf("**** Error %d receiving message from proxy\n", WSAGetLastError());
            return WSAGetLastError();
        }

        // this sample is limited but in normal use it 
        // should continue to receive until CR LF CR LF is received
    }

    *pSocket = Socket;

    return SEC_E_OK;
}

/*****************************************************************************/
static
LONG
DisconnectFromServer(
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

    dwSSPIFlags = ISC_REQ_SEQUENCE_DETECT   |
                  ISC_REQ_REPLAY_DETECT     |
                  ISC_REQ_CONFIDENTIALITY   |
                  ISC_RET_EXTENDED_ERROR    |
                  ISC_REQ_ALLOCATE_MEMORY   |
                  ISC_REQ_STREAM;

    OutBuffers[0].pvBuffer   = NULL;
    OutBuffers[0].BufferType = SECBUFFER_TOKEN;
    OutBuffers[0].cbBuffer   = 0;

    OutBuffer.cBuffers  = 1;
    OutBuffer.pBuffers  = OutBuffers;
    OutBuffer.ulVersion = SECBUFFER_VERSION;

    Status = g_SecurityFunc.InitializeSecurityContextW(
                    phCreds,
                    phContext,
                    NULL,
                    dwSSPIFlags,
                    0,
                    SECURITY_NATIVE_DREP,
                    NULL,
                    0,
                    phContext,
                    &OutBuffer,
                    &dwSSPIOutFlags,
                    &tsExpiry);

    if(FAILED(Status)) 
    {
        printf("**** Error 0x%x returned by InitializeSecurityContext\n", Status);
        goto cleanup;
    }

    pbMessage = OutBuffers[0].pvBuffer;
    cbMessage = OutBuffers[0].cbBuffer;


    //
    // Send the close notify message to the server.
    //

    if(pbMessage != NULL && cbMessage != 0)
    {
        cbData = send(Socket, (char *)pbMessage, cbMessage, 0);
        if(cbData == SOCKET_ERROR || cbData == 0)
        {
            Status = WSAGetLastError();
            printf("**** Error %d sending close notify\n", Status);
            goto cleanup;
        }

    if (fVerbose >= 1)
    {
        printf("Sending Close Notify\n");
        printf("%d bytes of handshake data sent\n", cbData);
    }

        if (fVerbose >= 2)
        {
            PrintHexDump(cbData, pbMessage);
            printf("\n");
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

/*****************************************************************************/
static
SECURITY_STATUS
PerformClientHandshake(
    SOCKET          Socket,         // in
    PCredHandle     phCreds,        // in
    LPSTR           pszServerName,  // in
    CtxtHandle *    phContext,      // out
    SecBuffer *     pExtraData)     // out
{
    SecBufferDesc   OutBuffer;
    SecBuffer       OutBuffers[1];
    DWORD           dwSSPIFlags;
    DWORD           dwSSPIOutFlags;
    TimeStamp       tsExpiry;
    SECURITY_STATUS scRet;
    DWORD           cbData;

    dwSSPIFlags = ISC_REQ_SEQUENCE_DETECT   |
                  ISC_REQ_REPLAY_DETECT     |
                  ISC_REQ_CONFIDENTIALITY   |
                  ISC_RET_EXTENDED_ERROR    |
                  ISC_REQ_ALLOCATE_MEMORY   |
                  ISC_REQ_STREAM;

    if (fFragmentMessages)
	dwSSPIFlags |= ISC_REQ_FRAGMENT_SUPPLIED;

    //
    //  Initiate a ClientHello message and generate a token.
    //

    OutBuffers[0].pvBuffer   = NULL;
    OutBuffers[0].BufferType = SECBUFFER_TOKEN;
    OutBuffers[0].cbBuffer   = 0;

    OutBuffer.cBuffers = 1;
    OutBuffer.pBuffers = OutBuffers;
    OutBuffer.ulVersion = SECBUFFER_VERSION;

    scRet = g_SecurityFunc.InitializeSecurityContextW(
                    phCreds,
                    NULL,
                    noCache ? NULL : pszServerName,
                    dwSSPIFlags,
                    0,
                    SECURITY_NATIVE_DREP,
                    NULL,
                    0,
                    phContext,
                    &OutBuffer,
                    &dwSSPIOutFlags,
                    &tsExpiry);

    if(scRet != SEC_I_CONTINUE_NEEDED)
    {
        printf("**** Error %d returned by InitializeSecurityContext (1)\n", scRet);
        return scRet;
    }

    // Send response to server if there is one.
    if(OutBuffers[0].cbBuffer != 0 && OutBuffers[0].pvBuffer != NULL)
    {
        cbData = send(Socket,
                      OutBuffers[0].pvBuffer,
                      OutBuffers[0].cbBuffer,
                      0);
        if(cbData == SOCKET_ERROR || cbData == 0)
        {
            printf("**** Error %d sending data to server (1)\n", WSAGetLastError());
            g_SecurityFunc.FreeContextBuffer(OutBuffers[0].pvBuffer);
            g_SecurityFunc.DeleteSecurityContext(phContext);
            return SEC_E_INTERNAL_ERROR;
        }

	if (fVerbose >= 1)
            printf("%d bytes of handshake data sent\n", cbData);

        if (fVerbose >= 2)
        {
            PrintHexDump(cbData, OutBuffers[0].pvBuffer);
            printf("\n");
        }
    }

    // Free output buffer.
    if (OutBuffers[0].pvBuffer != NULL)
    {
        g_SecurityFunc.FreeContextBuffer(OutBuffers[0].pvBuffer);
        OutBuffers[0].pvBuffer = NULL;
    }

    return ClientHandshakeLoop(Socket, phCreds, phContext, TRUE, pExtraData);
}

/*****************************************************************************/
static
SECURITY_STATUS
ClientHandshakeLoop(
    SOCKET          Socket,         // in
    PCredHandle     phCreds,        // in
    CtxtHandle *    phContext,      // in, out
    BOOL            fDoInitialRead, // in
    SecBuffer *     pExtraData)     // out
{
    SecBufferDesc   InBuffer;
    SecBuffer       InBuffers[2];
    SecBufferDesc   OutBuffer;
    SecBuffer       OutBuffers[1];
    DWORD           dwSSPIFlags;
    DWORD           dwSSPIOutFlags;
    TimeStamp       tsExpiry;
    SECURITY_STATUS scRet;
    DWORD           cbData;

    PUCHAR          IoBuffer;
    DWORD           cbIoBuffer;
    BOOL            fDoRead;


    dwSSPIFlags = ISC_REQ_SEQUENCE_DETECT   |
                  ISC_REQ_REPLAY_DETECT     |
                  ISC_REQ_CONFIDENTIALITY   |
                  ISC_RET_EXTENDED_ERROR    |
                  ISC_REQ_ALLOCATE_MEMORY   |
                  ISC_REQ_STREAM;

    if (fFragmentMessages)
	dwSSPIFlags |= ISC_REQ_FRAGMENT_SUPPLIED;

    //
    // Allocate data buffer.
    //

    IoBuffer = LocalAlloc(LMEM_FIXED, IO_BUFFER_SIZE);
    if(IoBuffer == NULL)
    {
        printf("**** Out of memory (1)\n");
        return SEC_E_INTERNAL_ERROR;
    }
    cbIoBuffer = 0;

    fDoRead = fDoInitialRead;


    // 
    // Loop until the handshake is finished or an error occurs.
    //

    scRet = SEC_I_CONTINUE_NEEDED;

    while(scRet == SEC_I_CONTINUE_NEEDED        ||
          scRet == SEC_E_INCOMPLETE_MESSAGE     ||
          scRet == SEC_I_INCOMPLETE_CREDENTIALS) 
   {

        //
        // Read data from server.
        //

        if(0 == cbIoBuffer || scRet == SEC_E_INCOMPLETE_MESSAGE)
        {
            if(fDoRead)
            {
                cbData = recv(Socket, 
                              (char *)(IoBuffer + cbIoBuffer), 
                              TLS_MIN(MAX_IOBSZ, IO_BUFFER_SIZE - cbIoBuffer), 
                              0);
                if(cbData == SOCKET_ERROR)
                {
                    printf("**** Error %d reading data from server\n", WSAGetLastError());
                    scRet = SEC_E_INTERNAL_ERROR;
                    break;
                }
                else if(cbData == 0)
                {
                    printf("**** Server unexpectedly disconnected\n");
                    scRet = SEC_E_INTERNAL_ERROR;
                    break;
                }

		if (fVerbose >= 1)
                    printf("%d bytes of handshake data received\n", cbData);

	        if (fVerbose >= 2)
                {
                    PrintHexDump(cbData, IoBuffer + cbIoBuffer);
                    printf("\n");
                }

                cbIoBuffer += cbData;
            }
            else
            {
                fDoRead = TRUE;
            }
        }


        //
        // Set up the input buffers. Buffer 0 is used to pass in data
        // received from the server. Schannel will consume some or all
        // of this. Leftover data (if any) will be placed in buffer 1 and
        // given a buffer type of SECBUFFER_EXTRA.
        //

        InBuffers[0].pvBuffer   = IoBuffer;
        InBuffers[0].cbBuffer   = cbIoBuffer;
        InBuffers[0].BufferType = SECBUFFER_TOKEN;

        InBuffers[1].pvBuffer   = NULL;
        InBuffers[1].cbBuffer   = 0;
        InBuffers[1].BufferType = SECBUFFER_EMPTY;

        InBuffer.cBuffers       = 2;
        InBuffer.pBuffers       = InBuffers;
        InBuffer.ulVersion      = SECBUFFER_VERSION;

        //
        // Set up the output buffers. These are initialized to NULL
        // so as to make it less likely we'll attempt to free random
        // garbage later.
        //

        OutBuffers[0].pvBuffer  = NULL;
        OutBuffers[0].BufferType= SECBUFFER_TOKEN;
        OutBuffers[0].cbBuffer  = 0;

        OutBuffer.cBuffers      = 1;
        OutBuffer.pBuffers      = OutBuffers;
        OutBuffer.ulVersion     = SECBUFFER_VERSION;

        //
        // Call InitializeSecurityContext.
        //

        scRet = g_SecurityFunc.InitializeSecurityContextW(phCreds,
                                          phContext,
                                          NULL,
                                          dwSSPIFlags,
                                          0,
                                          SECURITY_NATIVE_DREP,
                                          &InBuffer,
                                          0,
                                          NULL,
                                          &OutBuffer,
                                          &dwSSPIOutFlags,
                                          &tsExpiry);

        //
        // If InitializeSecurityContext was successful (or if the error was 
        // one of the special extended ones), send the contends of the output
        // buffer to the server.
        //

        if(scRet == SEC_E_OK                ||
           scRet == SEC_I_CONTINUE_NEEDED   ||
           FAILED(scRet) && (dwSSPIOutFlags & ISC_RET_EXTENDED_ERROR))
        {
            if(OutBuffers[0].cbBuffer != 0 && OutBuffers[0].pvBuffer != NULL)
            {
                cbData = send(Socket,
                              OutBuffers[0].pvBuffer,
                              OutBuffers[0].cbBuffer,
                              0);
                if(cbData == SOCKET_ERROR || cbData == 0)
                {
                    printf("**** Error %d sending data to server (2)\n", 
                        WSAGetLastError());
                    g_SecurityFunc.FreeContextBuffer(OutBuffers[0].pvBuffer);
                    g_SecurityFunc.DeleteSecurityContext(phContext);
                    return SEC_E_INTERNAL_ERROR;
                }

		if (fVerbose >= 1)
                    printf("%d bytes of handshake data sent\n", cbData);

	        if (fVerbose >= 2)
                {
                    PrintHexDump(cbData, OutBuffers[0].pvBuffer);
                    printf("\n");
                }
            }
	    if (OutBuffers[0].pvBuffer != NULL) 
	    {
                // Free output buffer.
                g_SecurityFunc.FreeContextBuffer(OutBuffers[0].pvBuffer);
                OutBuffers[0].pvBuffer = NULL;
	    }
        }


        //
        // If InitializeSecurityContext returned SEC_E_INCOMPLETE_MESSAGE,
        // then we need to read more data from the server and try again.
        //

        if(scRet == SEC_E_INCOMPLETE_MESSAGE)
        {
            continue;
        }


        //
        // If InitializeSecurityContext returned SEC_E_OK, then the 
        // handshake completed successfully.
        //

        if(scRet == SEC_E_OK)
        {
            //
            // If the "extra" buffer contains data, this is encrypted application
            // protocol layer stuff. It needs to be saved. The application layer
            // will later decrypt it with DecryptMessage.
            //

	    if (fVerbose >= 1)
                printf("Handshake was successful\n");

            if(InBuffers[1].BufferType == SECBUFFER_EXTRA)
            {
                pExtraData->pvBuffer = LocalAlloc(LMEM_FIXED, 
                                                  InBuffers[1].cbBuffer);
                if(pExtraData->pvBuffer == NULL)
                {
                    printf("**** Out of memory (2)\n");
                    return SEC_E_INTERNAL_ERROR;
                }

                MoveMemory(pExtraData->pvBuffer,
                           IoBuffer + (cbIoBuffer - InBuffers[1].cbBuffer),
                           InBuffers[1].cbBuffer);

                pExtraData->cbBuffer   = InBuffers[1].cbBuffer;
                pExtraData->BufferType = SECBUFFER_TOKEN;

                printf("%d bytes of app data was bundled with handshake data\n",
                    pExtraData->cbBuffer);
            }
            else
            {
                pExtraData->pvBuffer   = NULL;
                pExtraData->cbBuffer   = 0;
                pExtraData->BufferType = SECBUFFER_EMPTY;
            }

            //
            // Bail out to quit
            //

            break;
        }


        //
        // Check for fatal error.
        //

        if(FAILED(scRet))
        {
            printf("**** Error 0x%x returned by InitializeSecurityContext (2)\n", scRet);
            break;
        }


        //
        // If InitializeSecurityContext returned SEC_I_INCOMPLETE_CREDENTIALS,
        // then the server just requested client authentication. 
        //

        if(scRet == SEC_I_INCOMPLETE_CREDENTIALS)
        {
            //
            // Display trusted issuers info. 
            //

            GetNewClientCredentials(phCreds, phContext);

	    dwSSPIFlags |= ISC_REQ_USE_SUPPLIED_CREDS;

            //
            // Now would be a good time perhaps to prompt the user to select
            // a client certificate and obtain a new credential handle, 
            // but I don't have the energy nor inclination.
            //
            // As this is currently written, Schannel will send a "no 
            // certificate" alert to the server in place of a certificate. 
            // The server might be cool with this, or it might drop the 
            // connection.
            // 

            // Go around again.
            fDoRead = FALSE;
            scRet = SEC_I_CONTINUE_NEEDED;
            continue;
        }


        //
        // Copy any leftover data from the "extra" buffer, and go around
        // again.
        //

        if ( InBuffers[1].BufferType == SECBUFFER_EXTRA )
        {
            MoveMemory(IoBuffer,
                       IoBuffer + (cbIoBuffer - InBuffers[1].cbBuffer),
                       InBuffers[1].cbBuffer);

            cbIoBuffer = InBuffers[1].cbBuffer;
            printf("H");
        }
        else
        {
            cbIoBuffer = 0;
        }
    }

    // Delete the security context in the case of a fatal error.
    if(FAILED(scRet))
    {
        g_SecurityFunc.DeleteSecurityContext(phContext);
    }

    LocalFree(IoBuffer);

    return scRet;
}


/*****************************************************************************/
static
SECURITY_STATUS
HttpsGetFile(
    SOCKET          Socket,         // in
    PCredHandle     phCreds,        // in
    CtxtHandle *    phContext,      // in
    LPSTR           pszFileName)    // in
{
    SecPkgContext_StreamSizes Sizes;
    SECURITY_STATUS scRet;
    SecBufferDesc   Message;
    SecBuffer       Buffers[4];
    SecBuffer *     pDataBuffer;
    SecBuffer *     pExtraBuffer;
    SecBuffer       ExtraBuffer;

    PBYTE pbIoBuffer;
    DWORD cbIoBuffer;
    DWORD cbIoBufferLength;
    PBYTE pbMessage;
    DWORD cbMessage;

    DWORD cbData;
    INT   i;


    //
    // Read stream encryption properties.
    //

    scRet = g_SecurityFunc.QueryContextAttributes(phContext,
                                   SECPKG_ATTR_STREAM_SIZES,
                                   &Sizes);
    if(scRet != SEC_E_OK)
    {
        printf("**** Error 0x%x reading SECPKG_ATTR_STREAM_SIZES\n", scRet);
        return scRet;
    }

    if (fVerbose >= 1)
    printf("\nHeader: %d, Trailer: %d, MaxMessage: %d\n",
        Sizes.cbHeader,
        Sizes.cbTrailer,
        Sizes.cbMaximumMessage);

    //
    // Allocate a working buffer. The plaintext sent to EncryptMessage
    // should never be more than 'Sizes.cbMaximumMessage', so a buffer 
    // size of this plus the header and trailer sizes should be safe enough.
    // 

    cbIoBufferLength = Sizes.cbHeader + 
                       Sizes.cbMaximumMessage +
                       Sizes.cbTrailer;

    pbIoBuffer = LocalAlloc(LMEM_FIXED, IO_BUFFER_SIZE > cbIoBufferLength ? IO_BUFFER_SIZE : cbIoBufferLength /*cbIoBufferLength*/);
    if(pbIoBuffer == NULL)
    {
        printf("**** Out of memory (2)\n");
        return SEC_E_INTERNAL_ERROR;
    }


    //
    // Build an HTTP request to send to the server.
    //

    // Remove the trailing backslash from the filename, should one exist.
    if(pszFileName && 
       strlen(pszFileName) > 1 && 
       pszFileName[strlen(pszFileName) - 1] == '/')
    {
        pszFileName[strlen(pszFileName)-1] = 0;
    }

    // Build the HTTP request offset into the data buffer by "header size"
    // bytes. This enables Schannel to perform the encryption in place,
    // which is a significant performance win.
    pbMessage = pbIoBuffer + Sizes.cbHeader;

    // Build HTTP request. Note that I'm assuming that this is less than
    // the maximum message size. If it weren't, it would have to be broken up.
    sprintf((char *)pbMessage, 
            "GET /%s HTTP/1.0\r\nUser-Agent: Webclient\r\nAccept:*/*\r\n\r\n", 
            pszFileName);

    if (fVerbose >= 1)
        printf("\nHTTP request: %s\n", pbMessage);

    cbMessage = (DWORD)strlen((char *)pbMessage);

    if (fVerbose >= 1)
	printf("Sending plaintext: %d bytes\n", cbMessage);

    if (fVerbose >= 2)
    {
        PrintHexDump(cbMessage, pbMessage);
        printf("\n");
    }

    //
    // Encrypt the HTTP request.
    //

    Buffers[0].pvBuffer     = pbIoBuffer;
    Buffers[0].cbBuffer     = Sizes.cbHeader;
    Buffers[0].BufferType   = SECBUFFER_STREAM_HEADER;

    Buffers[1].pvBuffer     = pbMessage;
    Buffers[1].cbBuffer     = cbMessage;
    Buffers[1].BufferType   = SECBUFFER_DATA;

    Buffers[2].pvBuffer     = pbMessage + cbMessage;
    Buffers[2].cbBuffer     = Sizes.cbTrailer;
    Buffers[2].BufferType   = SECBUFFER_STREAM_TRAILER;

    Buffers[3].BufferType   = SECBUFFER_EMPTY;

    Message.ulVersion       = SECBUFFER_VERSION;
    Message.cBuffers        = 4;
    Message.pBuffers        = Buffers;

    scRet = g_SecurityFunc.EncryptMessage(phContext, 0, &Message, 0);

    if(FAILED(scRet))
    {
        printf("**** Error 0x%x returned by EncryptMessage\n", scRet);
        return scRet;
    }


    // 
    // Send the encrypted data to the server.
    //

    cbData = send(Socket,
                  (char *)pbIoBuffer,
                  Buffers[0].cbBuffer + Buffers[1].cbBuffer + Buffers[2].cbBuffer,
                  0);
    if(cbData == SOCKET_ERROR || cbData == 0)
    {
        printf("**** Error %d sending data to server (3)\n", 
            WSAGetLastError());
        g_SecurityFunc.DeleteSecurityContext(phContext);
        return SEC_E_INTERNAL_ERROR;
    }

    if (fVerbose >= 1)
	printf("%d bytes of application data sent\n", cbData);

    if (fVerbose >= 2)
    {
        PrintHexDump(cbData, pbIoBuffer);
        printf("\n");
    }

    //
    // Read data from server until done.
    //

    cbIoBuffer = 0;

    while(TRUE)
    {
        //
        // Read some data.
        //

        if(0 == cbIoBuffer || scRet == SEC_E_INCOMPLETE_MESSAGE)
        {
            cbData = recv(Socket, 
                          (char *)(pbIoBuffer + cbIoBuffer), 
                          TLS_MIN(MAX_IOBSZ, cbIoBufferLength - cbIoBuffer), 
                          0);
            if(cbData == SOCKET_ERROR)
            {
                printf("**** Error %d reading data from server\n", WSAGetLastError());
                scRet = SEC_E_INTERNAL_ERROR;
                break;
            }
            else if(cbData == 0)
            {
                // Server disconnected.
                if(cbIoBuffer)
                {
                    printf("**** Server unexpectedly disconnected\n");
                    scRet = SEC_E_INTERNAL_ERROR;
                    return scRet;
                }
                else
                {
                    break;
                }
            }
            else
            {
		if (fVerbose >= 1)
                    printf("%d bytes of (encrypted) application data received\n", cbData);

	        if (fVerbose >= 2)
                {
                    PrintHexDump(cbData, pbIoBuffer + cbIoBuffer);
                    printf("\n");
                }

                cbIoBuffer += cbData;
            }
        }

        // 
        // Attempt to decrypt the received data.
        //

        Buffers[0].pvBuffer     = pbIoBuffer;
        Buffers[0].cbBuffer     = cbIoBuffer;
        Buffers[0].BufferType   = SECBUFFER_DATA;

        Buffers[1].BufferType   = SECBUFFER_EMPTY;
        Buffers[2].BufferType   = SECBUFFER_EMPTY;
        Buffers[3].BufferType   = SECBUFFER_EMPTY;

        Message.ulVersion       = SECBUFFER_VERSION;
        Message.cBuffers        = 4;
        Message.pBuffers        = Buffers;

        scRet = g_SecurityFunc.DecryptMessage(phContext, &Message, 0, NULL);

        if(scRet == SEC_E_INCOMPLETE_MESSAGE)
        {
            // The input buffer contains only a fragment of an
            // encrypted record. Loop around and read some more
            // data.
            continue;
        }

        // Server signalled end of session
        if(scRet == SEC_I_CONTEXT_EXPIRED)
            break;

        if( scRet != SEC_E_OK && 
            scRet != SEC_I_RENEGOTIATE && 
            scRet != SEC_I_CONTEXT_EXPIRED)
        {
            printf("**** Error 0x%x returned by DecryptMessage\n", scRet);
            return scRet;
        }

        // Locate data and (optional) extra buffers.
        pDataBuffer  = NULL;
        pExtraBuffer = NULL;
        for(i = 1; i < 4; i++)
        {

            if(pDataBuffer == NULL && Buffers[i].BufferType == SECBUFFER_DATA)
            {
                pDataBuffer = &Buffers[i];
		if (fVerbose >= 2)
                printf("Buffers[%d].BufferType = SECBUFFER_DATA\n",i);
            }
            if(pExtraBuffer == NULL && Buffers[i].BufferType == SECBUFFER_EXTRA)
            {
                pExtraBuffer = &Buffers[i];
                printf("D");
            }
        }

        // Display or otherwise process the decrypted data.
        if(pDataBuffer)
        {
	    cbDataReceived += pDataBuffer->cbBuffer;
	    if (fVerbose >= 1)
		printf("Decrypted data: %d bytes\n", pDataBuffer->cbBuffer);

	    if (fVerbose >= 2)
            {
                PrintHexDump(pDataBuffer->cbBuffer, pDataBuffer->pvBuffer);
                printf("\n");
            }
        }

        // Move any "extra" data to the input buffer.
        if(pExtraBuffer)
        {
            MoveMemory(pbIoBuffer, pExtraBuffer->pvBuffer, pExtraBuffer->cbBuffer);
            cbIoBuffer = pExtraBuffer->cbBuffer;
        }
        else
        {
            cbIoBuffer = 0;
        }

        if(scRet == SEC_I_RENEGOTIATE)
        {
            // The server wants to perform another handshake
            // sequence.

	    if (fVerbose >= 1)
		printf("Server requested renegotiate!\n");

            scRet = ClientHandshakeLoop(Socket, 
                                        phCreds, 
                                        phContext, 
                                        FALSE, 
                                        &ExtraBuffer);
            if(scRet != SEC_E_OK)
            {
                return scRet;
            }

            // Move any "extra" data to the input buffer.
            if(ExtraBuffer.pvBuffer)
            {
                MoveMemory(pbIoBuffer, ExtraBuffer.pvBuffer, ExtraBuffer.cbBuffer);
                cbIoBuffer = ExtraBuffer.cbBuffer;
            }
        }
    }

    if (pbIoBuffer) 
	LocalFree (pbIoBuffer);

    return SEC_E_OK;
}

/*****************************************************************************/
static 
void
DisplayCertChain(
    PCCERT_CONTEXT  pServerCert,
    BOOL            fLocal)
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
    if(fLocal)
    {
        printf("Client subject: %s\n", szName);
    }
    else
    {
        printf("Server subject: %s\n", szName);
    }
    if(!CertNameToStr(pServerCert->dwCertEncodingType,
                      &pServerCert->pCertInfo->Issuer,
                      CERT_X500_NAME_STR | CERT_NAME_STR_NO_PLUS_FLAG,
                      szName, sizeof(szName)))
    {
        printf("**** Error 0x%x building issuer name\n", GetLastError());
    }
    if(fLocal)
    {
        printf("Client issuer: %s\n", szName);
    }
    else
    {
        printf("Server issuer: %s\n\n", szName);
    }


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
VerifyServerCertificate(
    PCCERT_CONTEXT  pServerCert,
    PSTR            pszServerName,
    DWORD           dwCertFlags)
{
    HTTPSPolicyCallbackData  polHttps;
    CERT_CHAIN_POLICY_PARA   PolicyPara;
    CERT_CHAIN_POLICY_STATUS PolicyStatus;
    CERT_CHAIN_PARA          ChainPara;
    PCCERT_CHAIN_CONTEXT     pChainContext = NULL;

    DWORD   Status;
    PWSTR   pwszServerName;
    DWORD   cchServerName;

    if(pServerCert == NULL)
    {
        return (DWORD)SEC_E_WRONG_PRINCIPAL;
    }


    //
    // Convert server name to unicode.
    //

    if(pszServerName == NULL || strlen(pszServerName) == 0)
    {
        return (DWORD)SEC_E_WRONG_PRINCIPAL;
    }

    cchServerName = MultiByteToWideChar(CP_ACP, 0, pszServerName, -1, NULL, 0);
    pwszServerName = LocalAlloc(LMEM_FIXED, cchServerName * sizeof(WCHAR));
    if(pwszServerName == NULL)
    {
        return (DWORD)SEC_E_INSUFFICIENT_MEMORY;
    }
    cchServerName = MultiByteToWideChar(CP_ACP, 0, pszServerName, -1, pwszServerName, cchServerName);
    if(cchServerName == 0)
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
    polHttps.dwAuthType         = AUTHTYPE_SERVER;
    polHttps.fdwChecks          = dwCertFlags;
    polHttps.pwszServerName     = pwszServerName;

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

    if (pwszServerName)
	LocalFree (pwszServerName);

    return Status;
}


/*****************************************************************************/
static
void
DisplayConnectionInfo(
    CtxtHandle *phContext)
{
    SECURITY_STATUS Status;
    SecPkgContext_ConnectionInfo ConnectionInfo;

    Status = g_SecurityFunc.QueryContextAttributes(phContext,
                                    SECPKG_ATTR_CONNECTION_INFO,
                                    (PVOID)&ConnectionInfo);
    if(Status != SEC_E_OK)
    {
        printf("Error 0x%x querying connection info\n", Status);
        return;
    }

    printf("\n");

    switch(ConnectionInfo.dwProtocol)
    {
        case SP_PROT_TLS1_CLIENT:
            printf("Protocol: TLS1\n");
            break;

        case SP_PROT_SSL3_CLIENT:
            printf("Protocol: SSL3\n");
            break;

        case SP_PROT_PCT1_CLIENT:
            printf("Protocol: PCT\n");
            break;

        case SP_PROT_SSL2_CLIENT:
            printf("Protocol: SSL2\n");
            break;

        default:
            printf("Protocol: 0x%x\n", ConnectionInfo.dwProtocol);
    }

    switch(ConnectionInfo.aiCipher)
    {
        case CALG_RC4: 
            printf("Cipher: RC4\n");
            break;

        case CALG_3DES: 
            printf("Cipher: Triple DES\n");
            break;

        case CALG_RC2: 
            printf("Cipher: RC2\n");
            break;

        case CALG_DES: 
        case CALG_CYLINK_MEK:
            printf("Cipher: DES\n");
            break;

        case CALG_SKIPJACK: 
            printf("Cipher: Skipjack\n");
            break;

        default: 
            printf("Cipher: 0x%x\n", ConnectionInfo.aiCipher);
    }

    printf("Cipher strength: %d\n", ConnectionInfo.dwCipherStrength);

    switch(ConnectionInfo.aiHash)
    {
        case CALG_MD5: 
            printf("Hash: MD5\n");
            break;

        case CALG_SHA: 
            printf("Hash: SHA\n");
            break;

        default: 
            printf("Hash: 0x%x\n", ConnectionInfo.aiHash);
    }

    printf("Hash strength: %d\n", ConnectionInfo.dwHashStrength);

    switch(ConnectionInfo.aiExch)
    {
        case CALG_RSA_KEYX: 
        case CALG_RSA_SIGN: 
            printf("Key exchange: RSA\n");
            break;

        case CALG_KEA_KEYX: 
            printf("Key exchange: KEA\n");
            break;

        case CALG_DH_EPHEM:
            printf("Key exchange: DH Ephemeral\n");
            break;

        default: 
            printf("Key exchange: 0x%x\n", ConnectionInfo.aiExch);
    }

    printf("Key exchange strength: %d\n", ConnectionInfo.dwExchStrength);
}


/*****************************************************************************/
static
void
GetNewClientCredentials(
    CredHandle *phCreds,
    CtxtHandle *phContext)
{
    CredHandle hCreds;
    SecPkgContext_IssuerListInfoEx IssuerListInfo;
    PCCERT_CHAIN_CONTEXT pChainContext;
    CERT_CHAIN_FIND_BY_ISSUER_PARA FindByIssuerPara;
    PCCERT_CONTEXT  pCertContext;
    TimeStamp       tsExpiry;
    SECURITY_STATUS Status;

    //
    // Read list of trusted issuers from schannel.
    //

    Status = g_SecurityFunc.QueryContextAttributes(phContext,
                                    SECPKG_ATTR_ISSUER_LIST_EX,
                                    (PVOID)&IssuerListInfo);
    if(Status != SEC_E_OK)
    {
        printf("Error 0x%x querying issuer list info\n", Status);
        return;
    }

    //
    // Enumerate the client certificates.
    //

    ZeroMemory(&FindByIssuerPara, sizeof(FindByIssuerPara));

    FindByIssuerPara.cbSize = sizeof(FindByIssuerPara);
    FindByIssuerPara.pszUsageIdentifier = szOID_PKIX_KP_CLIENT_AUTH;
    FindByIssuerPara.dwKeySpec = 0;
    FindByIssuerPara.cIssuer   = IssuerListInfo.cIssuers;
    FindByIssuerPara.rgIssuer  = IssuerListInfo.aIssuers;

    pChainContext = NULL;

    while(TRUE)
    {
        // Find a certificate chain.
        pChainContext = CertFindChainInStore(hMyCertStore,
                                             X509_ASN_ENCODING,
                                             0,
                                             CERT_CHAIN_FIND_BY_ISSUER,
                                             &FindByIssuerPara,
                                             pChainContext);
        if(pChainContext == NULL)
        {
            printf("Error 0x%x finding cert chain\n", GetLastError());
            break;
        }
        printf("\ncertificate chain found\n");

        // Get pointer to leaf certificate context.
        pCertContext = pChainContext->rgpChain[0]->rgpElement[0]->pCertContext;

        // Create schannel credential.
        SchannelCred.cCreds = 1;
        SchannelCred.paCred = &pCertContext;

        Status = g_SecurityFunc.AcquireCredentialsHandleW(
                            NULL,                   // Name of principal
                            UNISP_NAME_A,           // Name of package
                            SECPKG_CRED_OUTBOUND,   // Flags indicating use
                            NULL,                   // Pointer to logon ID
                            &SchannelCred,          // Package specific data
                            NULL,                   // Pointer to GetKey() func
                            NULL,                   // Value to pass to GetKey()
                            &hCreds,                // (out) Cred Handle
                            &tsExpiry);             // (out) Lifetime (optional)
        if(Status != SEC_E_OK)
        {
            printf("**** Error 0x%x returned by AcquireCredentialsHandle\n", Status);
            continue;
        }
        printf("\nnew schannel credential created\n");

        // Destroy the old credentials.
        g_SecurityFunc.FreeCredentialsHandle(phCreds);

        *phCreds = hCreds;

        break;
    }
}
    
