/*
 * Copyright(C) 2000-2001 ������ ���
 *
 * ���� ���� �������� ����������, ����������
 * �������������� �������� ������ ���.
 *
 * ����� ����� ����� ����� �� ����� ���� �����������,
 * ����������, ���������� �� ������ �����,
 * ������������ ��� �������������� ����� ��������,
 * ���������������, �������� �� ���� � ��� ��
 * ����� ������������ ������� ��� ����������������
 * ���������� ���������� � ��������� ������ ���.
 *
 * ����������� ���, ������������ � ���� �����, ������������
 * ������������� ��� ����� �������� � �� ����� ���� �����������
 * ��� ������ ����������.
 *
 * �������� ������-��� �� ����� �������
 * ��������������� �� ���������������� ����� ����.
 */

/*!
 * \file $RCSfile: tprf.c,v $
 * \version $Revision: 1.3 $
 * \date $Date: 2001/12/25 15:57:52 $
 * \author $Author: pre $
 *
 * \brief ������/������� ������ � �������� �������������� ���������
 * �� ������ ������
 * 
 */
#include "tmain.h"

int do_prf (LPSTR, LPSTR, DWORD, LPSTR);

int main_prf(int argc, char **argv)
{
    DWORD count = 32;
    BOOL print_help = FALSE;
    LPSTR infile_name = NULL;
    LPSTR outfile_name = NULL;
    LPSTR label_text = "";
    int c;
    int ret = 0;

    /*-----------------------------------------------------------------------------*/
    /* ����������� ����� ������� ����������*/
    static struct option long_options[] = {
	{"count",   required_argument,  NULL, 'c'},
	{"in",	    required_argument,	NULL, 'i'},
	{"out",     required_argument,  NULL, 'o'},
	{"label",   required_argument,  NULL, 'l'},
	{"help",    no_argument,        NULL, 'h'},
	{0, 0, 0, 0}
    };

    /*-----------------------------------------------------------------------------*/
    /* ������ ����������*/
    /* ��� ������� ���������� ������������ ������ getopt.c*/
    while ((c = getopt_long_only (argc, argv, "", long_options, (int *)0)) != EOF) {
	switch (c) {
	case 'i':
	    infile_name = optarg;
	    break;
	case 'o':
	    outfile_name = optarg;
	    break;
	case 'l':
	    label_text = optarg;
	    break;
	case 'c':
	    count = atoi (optarg);
	    break;
	case 'h':
	    ret = 1;
	    print_help = TRUE;
	    goto bad;
	case '?':
	default:
	    goto bad;
	}
    }
    if (c != EOF) {
	print_help = TRUE;
	goto bad;
    }

    if (!infile_name || !outfile_name || !count) {
	print_help = TRUE;
	goto bad;
    }

    ret = do_prf (infile_name, outfile_name, count, label_text);

bad:
    if (print_help) {
	fprintf(stderr,"%s -ep [options]\n", prog);
	fprintf(stderr,SoftName " export public key\nusing CAPI lowlevel message function type\n");
	fprintf(stderr,"options:\n");
	fprintf(stderr,"  -in arg        input filename to be encrypted or decrypted\n");
	fprintf(stderr,"  -out arg       output PKCS#7 filename\n");
	fprintf(stderr,"  -my name       use my certificate with commonName = name from system store \n");
    fprintf(stderr,"                 'MY' to sign/verify data\n");
	fprintf(stderr,"  -rcp name      use recepient's certificate commonName\n");
	fprintf(stderr,"  -encrypt       encrypt input file\n");
	fprintf(stderr,"  -decrypt       decrypt enveloped file, specified by input filename.\n");
	fprintf(stderr,"  -help          print this help\n\n");
    }

    return ret;


}

int do_prf (LPSTR infile_name, LPSTR outfile_name, DWORD count, LPSTR label)
{
    int ret = 0;
    HCRYPTPROV hProv = 0;
    HCRYPTHASH hFinishHash = 0;
    HCRYPTKEY hMasterSecret = 0;
    CRYPT_DATA_BLOB Data1;
    CRYPT_DATA_BLOB Data2;
    LPBYTE md = NULL;

    Data2.pbData = NULL;

    if (!CryptAcquireContext (&hProv, NULL, CP_DEF_PROV, PROV_GOST_DH,
	CRYPT_VERIFYCONTEXT|CRYPT_SILENT)) {
	HandleErrorFL ("Error during CryptAcquireContext.\n");
	goto err;
    }
    
    md = malloc (count);
    if (!md)
	goto err;

    if (!get_file_data_pointer(infile_name, (UINT*)&Data2.cbData, &Data2.pbData))
    {
        DebugErrorFL("get_file_data_pointer");
        goto err;
    }

    Data1.pbData = (PBYTE)label;
    Data1.cbData = strlen (label);

    if (!CryptGenKey (hProv, CALG_TLS1_MASTER, CRYPT_EXPORTABLE, &hMasterSecret)) {
	HandleErrorFL ("Error.\n");
	goto err;
    }

    if (!CryptCreateHash (hProv, CALG_TLS1PRF, hMasterSecret, 0, &hFinishHash)
      ||!CryptSetHashParam (hFinishHash, HP_TLS1PRF_LABEL, (PBYTE)&Data1, 0)
      ||!CryptSetHashParam (hFinishHash, HP_TLS1PRF_SEED, (PBYTE)&Data2, 0)
      ||!CryptGetHashParam (hFinishHash, HP_HASHVAL, md, &count, 0)
      ||!CryptDestroyHash (hFinishHash)
      )
    {
	HandleErrorFL ("Error.\n");
	goto err;
    }

    if (!write_file(outfile_name, count, md))
    {
        DebugErrorFL("write_file");
        goto err;
    }

  err:
    if (Data2.pbData)
	release_file_data_pointer(Data2.pbData);
    if (md)
	free (md);
    if (hFinishHash)
	CryptDestroyHash (hFinishHash);
    if (hMasterSecret)
	CryptDestroyKey (hMasterSecret);
    if (hProv)
	CryptReleaseContext (hProv, 0);
    return ret;
}   

/* end of file: $Id: tprf.c,v 1.3 2001/12/25 15:57:52 pre Exp $ */
