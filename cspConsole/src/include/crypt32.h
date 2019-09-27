
/*
 *  ��������� ������������������� ������� crypt32.dll.
 */

#ifndef _CRYPT32_H_INCLUDED
#define _CRYPT32_H_INCLUDED

#ifdef __cplusplus
extern "C" {
#endif

/*
 * ���������� �������� ��� ��������� AlgId - ����� ���������:
 *
 * ALG_CLASS_ANY          | ALG_TYPE_ANY | ALG_SID_ANY     (0x0000)
 * ALG_CLASS_SIGNATURE    | ALG_TYPE_DSS | ALG_SID_DSS_ANY (0x2200)
 * ALG_CLASS_SIGNATURE    | ALG_TYPE_RSA | ALG_SID_RSA_ANY (0x2400)
 * ALG_CLASS_KEY_EXCHANGE | ALG_TYPE_RSA | ALG_SID_RSA_ANY (0xA400)
 */
extern HCRYPTPROV WINAPI I_CryptGetDefaultCryptProv (ALG_ID AlgId);

typedef HCRYPTPROV WINAPI I_CRYPTGETDEFAULTCRYPTPROV (ALG_ID AlgId);

/*
 * ���������� �������� ��� ���������� KeyExchangeAlgId � EncryptAlgId:
 * (����� 0-0x10000, ����� 0-0x10000)
 * ��� EncryptAlgId �� ���������:
 * 0x6603 = ALG_CLASS_06 | ALG_TYPE_SECURECHANNEL | ALG_SID_SCHANNEL_MAC_KEY
 * 0x6609 = ALG_CLASS_06 | ALG_TYPE_SECURECHANNEL | ALG_SID_09 (HMAC)
 * ALG_CLASS_06 � ALG_SID_09 �� ������� � WinCrypt.h
 */
extern HCRYPTPROV WINAPI I_CryptGetDefaultCryptProvForEncrypt (
    ALG_ID KeyExchangeAlgId, ALG_ID EncryptAlgId, DWORD Reserved);
typedef HCRYPTPROV WINAPI I_CRYPTGETDEFAULTCRYPTPROVFORENCRYPT (
    ALG_ID KeyExchangeAlgId, ALG_ID EncryptAlgId, DWORD Reserved);
	
#ifdef __cplusplus
}
#endif

#endif /* _CRYPT32_H_INCLUDED */

/* end of file: $Id: crypt32.h,v 1.6 2001/08/31 10:46:54 pre Exp $ */
