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
 * \file $RCSfile: mycert.c,v $
 * \version $Revision: 1.6 $
 * \date $Date: 2001/12/25 15:57:52 $
 * \author $Author: pre $
 *
 * \brief Полномасштабная реализация CertCreateCertificateContext
 */

#include "tmain.h"
#include "mycert.h"

PCCERT_CONTEXT WINAPI MyCertCreateCertificateContext(
  DWORD dwCertEncodingType,                
  const BYTE *pbCertEncoded,               
  DWORD cbCertEncoded                      
  ) {
    HCERTSTORE hcs = NULL;
    int loop;
    int count;
    PCCERT_CONTEXT psc = NULL;
    PCCERT_CONTEXT pic = NULL;
    PCCERT_CONTEXT ret = NULL;
    DWORD dwf;
    const int MAXCERTCHAIN = 1000;

    __try {
	ret = CertCreateCertificateContext(dwCertEncodingType, pbCertEncoded, cbCertEncoded);
        if (ret)
            return ret;
        if (dwCertEncodingType&PKCS_7_ASN_ENCODING) {
            hcs = CryptGetMessageCertificates(PKCS_7_ASN_ENCODING, 0, 0, pbCertEncoded, cbCertEncoded);
            loop = 0;
            do {
                if (++loop > MAXCERTCHAIN) {
	            fprintf (stderr, __FILE__":%d:%s", __LINE__, "Too long certificates chain\n");
                    return NULL;
                }
                    /* Цикл для каждого сертификата в сообщении PKCS#7*/
                count = 0;
                psc = NULL;
                while ((psc = CertEnumCertificatesInStore(hcs, psc)) != NULL) {
                    count++;
                        /* Удаляем первого попавшегося issuer-а из store и повторяем цикл*/
                    dwf = 0;
                    if ((pic = CertGetIssuerCertificateFromStore(hcs, psc, NULL, &dwf)) != NULL) {
                        CertDeleteCertificateFromStore(pic);
                        /*CertFreeCertificateContext(pic);*/
                        pic = NULL;
                        CertFreeCertificateContext(psc);
                        psc = NULL;
                        count = MAXCERTCHAIN;
                        break;
                    }
                }
            } while (count > 1);
            ret = CertEnumCertificatesInStore(hcs, NULL);
            return ret;
        }
        if (dwCertEncodingType & X509_ASN_ENCODING) {
	    ret = CertCreateCertificateContext (X509_ASN_ENCODING,
		pbCertEncoded, cbCertEncoded);
            if (ret)
		return ret;
        }
    } __finally {
        if (pic) {
            CertFreeCertificateContext(pic);
            pic = NULL;
        }
        if (psc) {
            CertFreeCertificateContext(psc);
            psc = NULL;
        }
        if (hcs) {
            CertCloseStore(hcs, 0); /* Отложенное закрытие по CertFreeCertificateContext(ret)*/
            hcs = NULL;
        }
    }
    return NULL;
}

#if !defined(lint) && !defined(NO_RCSID) && !defined(OK_MODULE_VERSION)
static volatile const char rcsid[] = "\n$Id: mycert.c,v 1.6 2001/12/25 15:57:52 pre Exp $";
#endif
