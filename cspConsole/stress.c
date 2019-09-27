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
 * \file $RCSfile: stress.c,v $
 * \version $Revision: 1.16.4.5 $
 * \date $Date: 2002/08/22 13:59:51 $
 * \author $Author: lse $
 *
 * \brief Тест на скорость создания и уничтожения контекстов.
 */

#include "tmain.h"

typedef struct stresstest_thread_ctx_t
{
    DWORD   dwProvType;     /* Тип CSP */
    LPCSTR  szDllName;      /* Имя библиотеки */
    LPCSTR  szContainer;    /* Имя ключевого контейнера */
    int	verconflag;	    /* Флаг VERIFYCONTEXT */
    int cTries;		    /* Количество прогонов */
    int cplevel;	    /* cplevel */
} stresstest_thread_ctx;

static int
StressTest (stresstest_thread_ctx *ctx);
static int
do_threads (stresstest_thread_ctx *ctx);

#if !defined(VTABLEPROVSTRUC_DEFINED)
typedef struct _VTABLEPROVSTRUC {
    DWORD Version;
    FARPROC FuncVerifyImage;
    FARPROC FuncReturnhWnd;
    DWORD dwProvType;
    BYTE *pbContextInfo;

    DWORD cbContextInfo;
    LPSTR pszProvName;
} VTABLEPROVSTRUC, *PVTABLEPROVSTRUC;
#endif /* !defined(VTABLEPROVSTRUC_DEFINED) */

VTABLEPROVSTRUC vTable;
#if !defined(UNIX)
extern HANDLE	hCSPDLL;	/* Дескриптор загруженного модуля криптопровайдера*/
#else
extern void	*hCSPDLL;	/* Дескриптор загруженного модуля криптопровайдера*/
#endif /* !defined(UNIX) */

/*------------------------------------------------------*/
/* Определение функции CPAcquireContext*/
typedef BOOL (WINAPI *CPAcquireContext_t) (
    HCRYPTPROV *phProv,       /* out*/
    CHAR *pszContainer,       /* in, out*/
    DWORD dwFlags,            /* in*/
    PVTABLEPROVSTRUC pVTable  /* in*/
);
static CPAcquireContext_t MyAcquireContext;

/*------------------------------------------------------*/
/* Определение функции CPReleaseContext */
typedef BOOL (WINAPI *CPReleaseContext_t) (
    HCRYPTPROV hProv,  /* in*/
    DWORD dwFlags      /* in*/
);
static CPReleaseContext_t MyReleaseContext;

#ifdef UNIX
#define UniAcquireContext(hl,hP,p1,p2,p3) (MyAcquireContext(hP,p1,p2,p3))
#define UniReleaseContext(hl,hP,p1) (MyReleaseContext(hP,p1)
#else /* UNIX */
#define UniAcquireContext(hl,hP,p1,p2,p3) (CPlevel?MyAcquireContext(hP,p1,p2,p3):CryptAcquireContext(hP,p1,p2,p3))
#define UniReleaseContext(hl,hP,p1) ((hl)?MyReleaseContext(hP,p1):CryptReleaseContext(hP,p1))
#endif /* UNIX */


#define MAX_THREAD_NUMBER 128
#define THREAD_STACK_SIZE 0x10000
int thread_number=1;

/*--------------------------------------------------------------*/
/*--------------------------------------------------------------*/
/**/
/* MAIN*/
/**/
/*--------------------------------------------------------------*/
/*--------------------------------------------------------------*/
int main_stress (int argc, char **argv) {
    stresstest_thread_ctx ctx;
    int	    ret = 0;
    int	    print_help = 0;
    int    c;
   
    /*-----------------------------------------------------------------------------*/
    /* определение опций разбора параметров*/
    static struct option long_options[] = {
/*	{"cert",	 required_argument,	NULL, 'x'},*/
	{"help",	 no_argument,		NULL, 'h'},
	{"container",	 required_argument,	NULL, 'c'},
	{"verifycontext",no_argument,		NULL, 'V'},
	{"threads",	 required_argument,	NULL, 't'},
	{"repeat",	 required_argument,	NULL, 'r'},
	{"cplevel",	 no_argument,		NULL, 'p'},
	{"provtype",	 required_argument,	NULL, 'T'},
	{"dllname",	 required_argument,	NULL, 'D'},
	{0, 0, 0, 0}
    };

    ctx.dwProvType = PROV_GOST_94_DH;
    ctx.szDllName = NULL;
    ctx.szContainer = NULL;
    ctx.cTries = 50;//500;
    ctx.verconflag = 0;
    ctx.cplevel = 
#if !defined(UNIX)
        0
#else
        1
#endif /*!defined(UNIX)*/
        ;
        
    /*-----------------------------------------------------------------------------*/
    /* разбор параметров*/
    /* для разбора параметров используется модуль getopt.c*/
    while ((c = getopt_long_only (argc, argv, "", long_options, (int *)0)) != EOF) {
	switch (c) {
	case 'h':
	    ret = 1;
	    print_help = 1;
	    goto bad;
	case 'c':
	    ctx.szContainer = optarg;
	    break;
	case 'V':
	    ctx.verconflag = CRYPT_VERIFYCONTEXT;
	    break;
	case 'r':
	    ctx.cTries = atoi (optarg);
	    break;
	case 't':
	    thread_number = atoi (optarg);
            if (MAX_THREAD_NUMBER <= thread_number || 1 > thread_number) {
	        fprintf(stderr,"-threads number must be from interval 1..%d\n", MAX_THREAD_NUMBER-1);
                goto bad;
            }
	    break;
	case 'p':
	    ctx.cplevel = 1;
	    break;
#if !defined(UNIX)
	case 'T':
	    ctx.cplevel = 0;
	    ctx.dwProvType = atoi (optarg);
	    break;
#endif /* !defined(UNIX) */
	case 'D':
	    ctx.cplevel = 1;
	    ctx.szDllName = optarg;
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

if(ctx.verconflag) ctx.szContainer = NULL;

    ret = do_threads (&ctx);
/*    ret = StressTest (&ctx);*/

    /* Как зашатдаунить сервис в этом месте?*/
    /* Необходимо для того, чтобы его профилировать и баундчекить.*/
    /*CryptAcquireContext (&hProv, "$reboot$", CP_DEF_PROV, PROV_GOST_DH, CRYPT_DELETEKEYSET);*/

bad:
    if (print_help) {
	fprintf(stderr,"%s -stress [options]\n", prog);
	fprintf(stderr,SoftName " stress-test csp\n");
	fprintf(stderr,"options:\n");
	fprintf(stderr,"  -container name       specify container name\n");
	fprintf(stderr,"  -verifycontext	VERIFYCONTEXT test, container name ignored\n");
	fprintf(stderr,"  -threads number       specify number of threads\n");
	fprintf(stderr,"  -repeat  number       specify number of calls Acquire/ReleaseContext functions in each thread\n");
	fprintf(stderr,"  -cplevel		load CSP DLL and use CP functions instead Crypt\n");
	fprintf(stderr,"  -provtype		choice CSP by Provider Type (force without -cplevel)\n");
	fprintf(stderr,"  -dllname		load CSP DLL by DLL file name (force -cplevel)\n");
	fprintf(stderr,"  -help			print this help\n\n");

    }
    return ret;
}

char
GetThreadChar ()
{
#if !defined(UNIX)
    static const char syms[] = "@#$%*ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    static DWORD ids[sizeof(syms)-1];
    int i;
    for (i=0; i < sizeof(syms)-1; i++)
    {
	if (!ids[i]) ids[i] = GetCurrentThreadId();
	if (ids[i] == GetCurrentThreadId()) return syms[i];
    }
    return syms[0];
#else
    static const char syms[] = "@#$%*ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    static pthread_t ids[sizeof(syms)-1];
    int i;
    for (i=0; i < sizeof(syms)-1; i++)
    {
	if (!ids[i]) {
            ids[i] = pthread_self();
        }
	if (pthread_self() == ids[i]) {
            return syms[i];
        }
    }
    return syms[0];
#endif /* !defined(UNIX) */
}

int
StressTest (stresstest_thread_ctx * ctx)
{
    int cOk;
    BOOL b;
    HCRYPTPROV hProv;
    for (cOk = 0; cOk < ctx->cTries; cOk ++)
    {
	if( ctx->cplevel )
	    b = MyAcquireContext( &hProv, (char*)ctx->szContainer, ctx->verconflag, NULL);
#ifndef UNIX
	else
	    b = CryptAcquireContext (&hProv, ctx->szContainer, CP_DEF_PROV, PROV_GOST_94_DH, 0);
#endif /* UNIX */
	if( b )
	{
/*	if (CryptAcquireContext (&hProv, NULL, CP_DEF_PROV, PROV_GOST_DH,*/
/*	    CRYPT_VERIFYCONTEXT|CRYPT_SILENT)) {*/
#if 0
	    char *av1[] = { "",
		"-in",		"a.txt",
		"-out",		"1024e.e",
		"-my",		"1024e",
	    	"-cert",	"512e",
	    	"-cert",	"1024e",
	    	"-cert",	"512b",
	    	"-cert",	"1024b",
		"-encrypt",
	    	};
	    int ac1 = sizeof (av1)/sizeof (av1[0]);
	    char *av2[] = { "",
		"-in",		"1024e.e",
		"-out",		"1024e.d",
		"-my",		"512e",
		"-decrypt",
	    	};
	    int ac2 = sizeof (av2)/sizeof (av2[0]);
	    char *av3[] = { "",
		"-in",		"a.txt",
		"-out",		"1024e.s",
		"-my",		"512b",
		"-sign",
		"-add",
	    	};
	    int ac3 = sizeof (av3)/sizeof (av3[0]);
	    char *av4[] = { "",
		"-in",		"1024e.s",
/*		"-out",		"1024e.c",*/
		"-verify",
	    	};
	    int ac4 = sizeof (av4)/sizeof (av4[0]);
	    optind = 0;
	    if (!main_encrypt_sf (ac1, av1)) HandleErrorFL("encryption failed");
	    optind = 0;
	    if (!main_encrypt_sf (ac2, av2)) HandleErrorFL("decryption failed");
	    optind = 0;
	    if (!main_sign (ac3, av3)) HandleErrorFL("signing failed");
	    optind = 0;
	    if (!main_sign (ac4, av4)) HandleErrorFL("verify failed");
#endif
	    /*fprintf(stderr, "%c", GetThreadChar ());*/
	    /*fflush(stderr);*/
	    if( ctx->cplevel ) {
		MyReleaseContext (hProv, 0);
            }
#if !defined(UNIX)
	    else {
		CryptReleaseContext (hProv, 0);
            }
#endif /* !defined(UNIX) */
	} else
	{
	    HandleErrorFL("Cryptographic context could not be acquired.");
	    return 0;
	}
    }
    return 1;
}

static int 
ndoit(stresstest_thread_ctx *ctx)
{
#if !defined(UNIX)
    /*srand (GetCurrentThreadId()^GetTickCount());*/
    fprintf (stderr, "Thread %d (%c) started up.\n", GetCurrentThreadId (), GetThreadChar ());
    /*Sleep (rand()%1024);*/
    StressTest (ctx);
    fprintf (stderr, "Thread %d (%c) finished.\n", GetCurrentThreadId (), GetThreadChar ());
#else
    /*srand (GetCurrentThreadId()^GetTickCount());*/
    fprintf (stderr, "Thread %d (%c) started up.\n", pthread_self(), GetThreadChar ());
    /*Sleep (rand()%1024);*/
    StressTest (ctx);
    fprintf (stderr, "Thread %d (%c) finished.\n", pthread_self(), GetThreadChar ());
#endif /* !defined(UNIX) */
    return 0;
}

static int
do_threads (stresstest_thread_ctx *ctx)
{
#if !defined(UNIX)
    double ret;
    DWORD thread_id[MAX_THREAD_NUMBER];
    HANDLE thread_handle[MAX_THREAD_NUMBER];
    int i;
    SYSTEMTIME start,end;

    if (ctx->cplevel) {
	/* Если непосредственно используем криптопровайдер, загрузим соответствующий модуль.*/
	hCSPDLL = load_CSP_library (ctx->dwProvType, ctx->szDllName);
	if (! hCSPDLL)
	    HandleErrorFL ("Cannot load CSP DLL.\n");

	vTable.Version = 1; /* min*/
	vTable.FuncVerifyImage = NULL;
	vTable.FuncReturnhWnd = NULL;

        MyAcquireContext = (CPAcquireContext_t) GetProcAddress (hCSPDLL,"CPAcquireContext");
        MyReleaseContext = (CPReleaseContext_t) GetProcAddress (hCSPDLL,"CPReleaseContext");

        if (!MyAcquireContext || !MyReleaseContext ) {
	    HandleErrorFL ("Cannot load CSP functions.\n");
        }
    }
    
    GetSystemTime(&start);
    for (i=0; i<thread_number; i++)
    {
	thread_handle[i]=CreateThread(NULL,
	    THREAD_STACK_SIZE,
	    (LPTHREAD_START_ROUTINE)ndoit,
	    ctx,
	    0L,
	    &(thread_id[i]));
    }
    
    printf("reaping\n");
    for (i=0; i<thread_number; i += MAXIMUM_WAIT_OBJECTS-1)
    {
	int j;
	
        j = min(thread_number-i, MAXIMUM_WAIT_OBJECTS-1);
	
	if (WaitForMultipleObjects(j,
	    (CONST HANDLE *)&(thread_handle[i]), TRUE, INFINITE)
	    == WAIT_FAILED)
	{
	    fprintf(stderr,"WaitForMultipleObjects failed:%d\n",GetLastError());
	    exit(1);
	}
    }
    GetSystemTime(&end);
    
    if (start.wDayOfWeek > end.wDayOfWeek) end.wDayOfWeek+=7;
    ret=(end.wDayOfWeek-start.wDayOfWeek)*24;
    
    ret=(ret+end.wHour-start.wHour)*60;
    ret=(ret+end.wMinute-start.wMinute)*60;
    ret=(ret+end.wSecond-start.wSecond);
    ret+=(end.wMilliseconds-start.wMilliseconds)/1000.0;
    
    printf("win32 threads done - %.3f seconds\n",ret);
    return 1;
#else
    double ret;
    pthread_t thread_id[MAX_THREAD_NUMBER];
    int i;
    struct timeb start;
    struct timeb end;

    if (ctx->cplevel) {
	/* Если непосредственно используем криптопровайдер, загрузим соответствующий модуль.*/
	hCSPDLL = load_CSP_library (ctx->dwProvType, ctx->szDllName);
	if (! hCSPDLL)
	    HandleErrorFL ("Cannot load CSP DLL.\n");

        MyAcquireContext = (CPAcquireContext_t) dlsym(hCSPDLL,"CPAcquireContext");
        MyReleaseContext = (CPReleaseContext_t) dlsym(hCSPDLL,"CPReleaseContext");

        if (!MyAcquireContext || !MyReleaseContext ) {
	    HandleErrorFL ("Cannot load CSP functions.\n");
        }
    }
    
    ftime(&start);
    for (i = 0; i < thread_number; i++) {
	if (pthread_create(&thread_id[i], NULL, &ndoit, ctx)) {
            perror("pthread_create - fails");
            exit(1);
        }
    }
    
    printf("reaping\n");
    
    for (i = 0; i < thread_number; i++) {
        if (pthread_join(thread_id[i], NULL)) {
            perror("pthread_join - fails");
            exit(1);
        }
    }
    ftime(&end);
    
    ret  = (end.time    - start.time);
    ret += (end.millitm - start.millitm)/1000.0;
    
    printf("unix threads done - %.3f seconds\n",ret);
    return 1;
#endif !defined(UNIX)
}

#if defined( DEBUG_PRO ) || defined( __BOUNDSCHECKER__ )
typedef HCRYPTPROV WINAPI I_CRYPTGETDEFAULTCRYPTPROVFORENCRYPT (
    ALG_ID KeyExchangeAlgId, ALG_ID EncryptAlgId, DWORD Reserved);

void cpcsp_reboot( HANDLE hCSPDLL )
{
    CPAcquireContext_t MyAcquireContext;
    HCRYPTPROV hProv;

#if !defined(UNIX)
    if( !hCSPDLL )
    {
	HANDLE h = GetModuleHandle ("crypt32.dll");
	I_CRYPTGETDEFAULTCRYPTPROVFORENCRYPT *
	    fI_CryptGetDefaultCryptProvForEncrypt;
	if( !h ) return;
        fI_CryptGetDefaultCryptProvForEncrypt =
	    (I_CRYPTGETDEFAULTCRYPTPROVFORENCRYPT *)
		GetProcAddress (h, "I_CryptGetDefaultCryptProvForEncrypt");
	if( !fI_CryptGetDefaultCryptProvForEncrypt ) return;
	hProv = fI_CryptGetDefaultCryptProvForEncrypt (
		CALG_GR3410, CALG_G28147, 0);
	if( !hProv ) return;
	CryptReleaseContext (hProv, 0);
	CryptAcquireContext (&hProv, "$reboot$", CP_DEF_PROV, PROV_GOST_DH, CRYPT_DELETEKEYSET);
	return;
    }
    MyAcquireContext = (CPAcquireContext_t) GetProcAddress (hCSPDLL,"CPAcquireContext");
    #else
    MyAcquireContext = (CPAcquireContext_t)dlsym(hCSPDLL,"CPAcquireContext");
#endif /*!defined(UNIX)*/
    if( !MyAcquireContext )
	return;
    MyAcquireContext (&hProv, "$reboot$", CRYPT_DELETEKEYSET, NULL);
    return;
}
#endif
