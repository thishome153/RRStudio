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
 */

/*!
 * \file $RCSfile: msqueue.c,v $
 * \version $Revision: 1.4 $
 * \date $Date: 2001/05/24 09:34:32 $
 * \author $Author: vpopov $
 *
 * \brief Программа тестирования криптографии в MSQUEUE.
 */

#include <windows.h>
#include <stdio.h>
#include <mq.h>                               /* MSMQ header file*/
#include <wincrypt.h>
#include "tmain.h"

int main_q (int argc, char **argv)
{
  /*//////////////////////////////////////////////////////////*/
  /*  Define the structures needed to send the message.*/
  /*//////////////////////////////////////////////////////////*/

  #define NUMBEROFPROPERTIES  3               /* Number of properties*/
  DWORD cPropId = 0;                          /* Property counter*/

  /*Define MQMSGPROPS structure.*/
  MQMSGPROPS MsgProps;
  MSGPROPID aMsgPropId[NUMBEROFPROPERTIES];
  PROPVARIANT aMsgPropVar[NUMBEROFPROPERTIES];
  HRESULT aMsgStatus[NUMBEROFPROPERTIES];

  HANDLE hQueue;                              /* Queue handle*/
  HRESULT hr;                                 /* Define results.*/
  DWORD dwAccess = MQ_SEND_ACCESS;            /* Access mode of queue*/
  DWORD dwShareMode = MQ_DENY_NONE;           /* Share Mode of queue*/

  /*Destination queue format name buffer*/
  DWORD dwDestFormatNameBufferLength = 256;
  WCHAR wszDestFormatNameBuffer[256];

  DWORD dwCertCreateFlag = MQCERT_REGISTER_ALWAYS;

  /*//////////////////////////////////////////////////////////////////*/
  /* Obtain the external certificate. This example takes the first*/
  /* certificate it finds in "My" store. The code can be modified to*/
  /* find another certificate if necessary.*/
  /*//////////////////////////////////////////////////////////////////*/

  PCCERT_CONTEXT pContext = NULL ;
  HCERTSTORE hStore = NULL ;
  HCRYPTPROV hProv = 0 ;

  if (!CryptAcquireContextA( &hProv,
                             NULL,
                             NULL,
                             PROV_GOST_DH, /*PROV_RSA_FULL,*/
                             CRYPT_VERIFYCONTEXT))
  {
    return FALSE ;
  }

  /*hStore = CertOpenSystemStoreA( hProv, "My" ) ;*/
  hStore = CertOpenStore(CERT_STORE_PROV_SYSTEM,    /* LPCSTR lpszStoreProvider*/
			    0,			    /* DWORD dwMsgAndCertEncodingType*/
			    hProv,		    /* HCRYPTPROV hCryptProv*/
			    CERT_STORE_OPEN_EXISTING_FLAG|CERT_STORE_READONLY_FLAG|
			    CERT_SYSTEM_STORE_CURRENT_USER, /* DWORD dwFlags*/
			    L"MY");
  if (!hStore)
  {
    return FALSE ;
  }

  /* Take first certificate from store*/
  pContext = CertEnumCertificatesInStore(hStore, NULL) ;
  if (!pContext)
  {
    return FALSE ;
  }

  /*///////////////////////////////////////////////////////////////*/
  /* Specify the authentication level of the message:*/
  /* PROPID_M_AUTH_LEVEL and PROPID_M_SENDER_CERT.*/
  /*///////////////////////////////////////////////////////////////*/

  aMsgPropId[cPropId] = PROPID_M_AUTH_LEVEL;             /* Property ID*/
  aMsgPropVar[cPropId].vt = VT_UI4;                      /* Type indicator*/
  aMsgPropVar[cPropId].ulVal = MQMSG_AUTH_LEVEL_ALWAYS;  /* Format name*/
  cPropId++;
  
  
  /*///////////////////////////////////////////////////////////////*/
  /* Specify PROPID_M_SENDER_CERT. Security context information*/
  /* is not used in this example.*/
  /*///////////////////////////////////////////////////////////////*/
  
  aMsgPropId[cPropId] = PROPID_M_SENDER_CERT;            /* Property ID*/
  aMsgPropVar[cPropId].vt = VT_VECTOR | VT_UI1;          /* Type indicator*/
  aMsgPropVar[cPropId].caub.pElems = pContext->pbCertEncoded ;
  aMsgPropVar[cPropId].caub.cElems = pContext->cbCertEncoded ;
  cPropId++;
  
  
  /*//////////////////////////////////////////////////////////////*/
  /* Add additional message properties as needed. This example*/
  /* specifies a label for the message (PROPID_M_LABLE) as well.*/
  /*///////////////////////////////////////////////////////////////*/
  
  aMsgPropId[cPropId] = PROPID_M_LABEL;                /* Property ID*/
  aMsgPropVar[cPropId].vt = VT_LPWSTR;                 /* Type indicator*/
  aMsgPropVar[cPropId].pwszVal = L"Authentication Test";  /* Value*/
  cPropId++;


  /*///////////////////////////////////////////////////////////////*/
  /* Initialize the MQMSGPROPS structure.*/
  /*///////////////////////////////////////////////////////////////*/

  MsgProps.cProp = cPropId;                         /* Number of message properties.*/
  MsgProps.aPropID = aMsgPropId;                    /* IDs of message properties*/
  MsgProps.aPropVar = aMsgPropVar;                  /* Values of message properties*/
  MsgProps.aStatus  = aMsgStatus;                   /* Error reports*/


  /*///////////////////////////////////////////////////////////////*/
  /* Register external certificate in the directory service.*/
  /*///////////////////////////////////////////////////////////////  */

/*
  hr = MQRegisterCertificate(dwCertCreateFlag,
                             pContext->pbCertEncoded,
                             pContext->cbCertEncoded);
  if (FAILED(hr))
  {
    fprintf(stderr, "Failed in MQRegisterCertificate, error = 0x%x\n",hr);
    return -1;
  }
  */
  
  
  /*///////////////////////////////////////////////////////////////*/
  /* Convert destination queue pathname to a format name.*/
  /*///////////////////////////////////////////////////////////////*/
  
  hr = MQPathNameToFormatName(L".\\testqueue",
                              wszDestFormatNameBuffer,
                              &dwDestFormatNameBufferLength);
  if (FAILED(hr))
  {
    fprintf(stderr, "Failed in MQPathNameToFormatName for destination queue, error = 0x%x\n",hr);
    return -1;
  }


  /*///////////////////////////////////////////////////////////////*/
  /* Open the destination queue to send message.*/
  /*///////////////////////////////////////////////////////////////*/

  hr = MQOpenQueue(wszDestFormatNameBuffer, /* Format name of queue*/
                   dwAccess,                /* Access mode*/
                   dwShareMode,             /* Share mode*/
                   &hQueue);                /* OUT: Handle to queue*/
  if (FAILED(hr))
  {
    fprintf(stderr, "Failed opening destination queue, error = 0x%x\n",hr);
    return -1;
  }


  /*///////////////////////////////////////////////////////////////*/
  /* Send message to the destination queue.*/
  /*///////////////////////////////////////////////////////////////*/

  hr = MQSendMessage(hQueue,
                     &MsgProps,
                     NULL);
  if (FAILED(hr))
  {
    switch( hr )
    {
      case MQ_ERROR_INVALID_CERTIFICATE:
        puts("Failed sending message! The external certificate is corrupted ");
        puts("or it has not been registered in the Microsoft Internet ") ;
        puts("Explorer personal certificate store.\n");
        break;

      default:
        fprintf(stderr, "Failed sending message, error = 0x%x\n",hr);
        break;
    }
    return -1;
  }


  /*///////////////////////////////////////////////////////////////*/
  /* Close the queue.*/
  /*///////////////////////////////////////////////////////////////*/

  hr = MQCloseQueue(hQueue);
  if (FAILED(hr))
  {
    fprintf(stderr, "Failed closing destination queue, error = 0x%x\n",hr);
    return -1;
  }

  puts("The message is sent and the queue is closed.");
  return 0;

}

