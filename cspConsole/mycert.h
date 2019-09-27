

/*!
 * \file $RCSfile: mycert.h,v $
 * \version $Revision: 1.1 $
 * \date $Date: 2001/02/16 12:01:09 $
 * \author $Author: pre $
 *
 * \brief Интерфейс полномасштабной реализации CertCreateCertificateContext.
 */
#ifndef _MYCERT_H_INCLUDED
#define _MYCERT_H_INCLUDED

#ifdef __cplusplus
extern "C" {
#endif

PCCERT_CONTEXT WINAPI MyCertCreateCertificateContext(
  DWORD dwCertEncodingType,                
  const BYTE *pbCertEncoded,               
  DWORD cbCertEncoded                      
  );

#ifdef __cplusplus
}
#endif

#endif /* _MYCERT_H_INCLUDED */
