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

#include "tmain.h"

#define MY_ENCODING_TYPE  (PKCS_7_ASN_ENCODING | X509_ASN_ENCODING)

int main_hash (int argc, char **argv) {
    /*--------------------------------------------------------------------*/
    /*  Declare and ini vatializeriables. This includes creating a */
    /*  pointer to the message content. In real situations, */
    /*  the message content will usually exist somewhere and a pointer*/
    /*  to it will get passed to the application. */
    
    BYTE* pbContent = (BYTE*) "A razzle-dazzle hashed message \n"
	"Hashing is better than trashing. \n";    /* The message*/
    DWORD cbContent = strlen((char *)pbContent);  /* Size of message*/
    DWORD HashAlgSize;
    CRYPT_ALGORITHM_IDENTIFIER HashAlgorithm;
    CMSG_HASHED_ENCODE_INFO HashedEncodeInfo;
    DWORD cbEncodedBlob;
    BYTE *pbEncodedBlob;
    HCRYPTMSG hMsg;
    /*-------------------------------------------------------------------*/
    /*  Variables to be used in decoding.*/
    DWORD cbData = sizeof(DWORD);
    DWORD dwMsgType;
    DWORD cbDecoded;
    BYTE  *pbDecoded;

    argc, argv;
    /*--------------------------------------------------------------------*/
    /*  Begin processing.*/
    
    printf("Begin processing. \n");
    printf("The message to be hashed and encoded is: \n");
    printf("%s\n",pbContent);    /* Display original message.*/
    printf("The starting message length is %d\n",cbContent);
    
    /*--------------------------------------------------------------------*/
    /* Initialize the algorithm identifier structure.*/
    
    HashAlgSize = sizeof(HashAlgorithm);
    memset(&HashAlgorithm, 0, HashAlgSize);   /* Initialize to zero.*/
    HashAlgorithm.pszObjId = szOID_CP_GOST_R3411;/*szOID_RSA_MD5;   // Then set the */
    /*   necessary member.*/
    
    /*--------------------------------------------------------------------*/
    /* Initialize the CMSG_HASHED_ENCODE_INFO structure.*/
    
    memset(&HashedEncodeInfo, 0, sizeof(CMSG_HASHED_ENCODE_INFO));
    HashedEncodeInfo.cbSize = sizeof(CMSG_HASHED_ENCODE_INFO);
    HashedEncodeInfo.hCryptProv = 0;
    HashedEncodeInfo.HashAlgorithm = HashAlgorithm;
    HashedEncodeInfo.pvHashAuxInfo = NULL;
    
    /*--------------------------------------------------------------------*/
    /* Get the size of the encoded message blob.*/
    cbEncodedBlob = CryptMsgCalculateEncodedLength(
	MY_ENCODING_TYPE,     /* Message encoding type*/
	0,                    /* Flags*/
	CMSG_HASHED,          /* Message type*/
	&HashedEncodeInfo,    /* Pointer to structure*/
	NULL,                 /* Inner content object ID*/
	cbContent);           /* Size of content*/
    if(cbEncodedBlob)
    {
	printf("The length to be allocated is %d bytes.\n",cbEncodedBlob);
    }
    else
    {
	HandleErrorFL("Getting cbEncodedBlob length failed");
    }
    /*--------------------------------------------------------------------*/
    /* Allocate memory for the encoded blob.*/
    pbEncodedBlob = (BYTE *) malloc(cbEncodedBlob);
    if(pbEncodedBlob)
    {
	printf("%d bytes of memory have been allocated.\n",cbEncodedBlob);
    }
    else
    {
	HandleErrorFL("Malloc operation failed.");
    }
    /*--------------------------------------------------------------------*/
    /* Open a message to encode.*/
    hMsg = CryptMsgOpenToEncode(
	MY_ENCODING_TYPE,        /* Encoding type*/
	0,                       /* Flags*/
	CMSG_HASHED,             /* Message type*/
	&HashedEncodeInfo,       /* Pointer to structure*/
	NULL,                    /* Inner content object ID*/
	NULL);
    if(hMsg)                   /* Stream information (not used)*/
    {
	printf("The message to encode has been opened. \n");
    }
    else
    {
	HandleErrorFL("OpenToEncode failed");
    }
    /*--------------------------------------------------------------------*/
    /* Update the message with the data.*/
    
    if(CryptMsgUpdate(
	hMsg,          /* Handle to the message*/
	pbContent,     /* Pointer to the content*/
	cbContent,     /* Size of the content*/
	TRUE))         /* Last call*/
    {
	printf("Data has been added to the message to encode. \n");
    }
    else
    {
	HandleErrorFL("MsgUpdate failed");
    }
    /*--------------------------------------------------------------------*/
    /* Вернем хэшированное сообщение*/
    
    if(CryptMsgGetParam(
	hMsg,                      /* Handle to the message*/
	CMSG_CONTENT_PARAM,        /* Parameter type*/
	0,                         /* Index*/
	pbEncodedBlob,             /* Pointer to the blob*/
	&cbEncodedBlob))           /* Size of the blob*/
    {
	printf("Message encoded successfully. \n");
    }
    else
    {
	HandleErrorFL("MsgGetParam failed");
    }
    /*--------------------------------------------------------------------*/
    /* Close both messages to prepare for decoding.*/
    
    CryptMsgClose(hMsg);

    if (!write_file ("hash.p7h", cbEncodedBlob, pbEncodedBlob))
	HandleErrorFL ("file open error\n");

    /* The following code decodes the hashed message.*/
    /* Usually, this would be in a separate program and the encoded,*/
    /* hashed data would be input from a file, from an e-mail message, */
    /* or from some other source.*/
    /**/
    /* The variables used in this code have already been*/
    /* declared and initialized.*/
    
    /*--------------------------------------------------------------------*/
    /* Open the  message for decoding.*/
    hMsg = CryptMsgOpenToDecode(
	MY_ENCODING_TYPE,       /* Encoding type*/
	0,                      /* Flags*/
	0,                      /* Message type (get from message)*/
	0,			/* Cryptographic provider*/
	NULL,                   /* Recipient information*/
	NULL);                  /* Stream information*/
    if(hMsg)
    {
	printf("The message has been opened for decoding. \n");
    }
    else
    {
	HandleErrorFL("OpenToDecode failed");
    }
    /*--------------------------------------------------------------------*/
    /* Update the message with the encoded blob. */
    
    if(CryptMsgUpdate(
	hMsg,             /* Handle to the message*/
	pbEncodedBlob,    /* Pointer to the encoded blob*/
	cbEncodedBlob,    /* Size of the encoded blob*/
	TRUE))            /* Last call*/
    {
	printf("The encoded data is added to the message to decode. \n");
    }
    else
    {
	HandleErrorFL("Decode MsgUpdate failed");
    }
    /*--------------------------------------------------------------------*/
    /* Get the message type.*/
    
    if(CryptMsgGetParam(
	hMsg,               /* Handle to the message*/
	CMSG_TYPE_PARAM,    /* Parameter type*/
	0,                  /* Index*/
	&dwMsgType,         /* Address for returned information*/
	&cbData))           /* Size of the returned information*/
    {
	printf("The message type has been obtained. \n");
    }
    else
    {
	HandleErrorFL("Decode CMSG_TYPE_PARAM failed");
    }
    /*--------------------------------------------------------------------*/
    /* Some applications may need to use a switch statement here*/
    /* and process the message differently, depending on the*/
    /* message type.*/
    
    if(dwMsgType == CMSG_HASHED)
    {
	printf("The message is a hashed message. Proceed. \n");
    }
    else
    {
	HandleErrorFL("Wrong message type");
    }
    /*--------------------------------------------------------------------*/
    /* Get the size of the content.*/
    
    if(CryptMsgGetParam(
	hMsg,                   /* Handle to the message*/
	CMSG_CONTENT_PARAM,     /* Parameter type*/
	0,                      /* Index*/
	NULL,                   /* Address for returned */
	/* information*/
	&cbDecoded))            /* Size of the returned */
	/* information*/
    {
	printf("The length %d of the message obtained. \n", cbDecoded);
    }
    else
    {
	HandleErrorFL("Decode CMSG_CONTENT_PARAM failed");
    }
    /*--------------------------------------------------------------------*/
    /* Allocate memory.*/
    pbDecoded = (BYTE *) malloc(cbDecoded+1);
    if(pbDecoded)
    {
	printf("Memory for the decoded message has been allocated.\n");
    }
    else
    {
	HandleErrorFL("Decoding memory allocation failed");
    }
    /*--------------------------------------------------------------------*/
    /* Copy the decoded message into the buffer just allocated.*/
    
    if(CryptMsgGetParam(
	hMsg,                    /* Handle to the message*/
	CMSG_CONTENT_PARAM,      /* Parameter type*/
	0,                       /* Index*/
	pbDecoded,               /* Address for returned */
	/* information*/
	&cbDecoded))             /* Size of the returned */
	/* information*/
    {
	pbDecoded[cbDecoded] = '\0';
	printf("Message decoded successfully \n");
	printf("The decoded message is \n%s\n", (LPSTR)pbDecoded);
    }
    else
    {
	HandleErrorFL("Decoding CMSG_CONTENT_PARAM #2 failed");
    }
    /*--------------------------------------------------------------------*/
    /* Verify the hash.*/
    
    if(CryptMsgControl(
	hMsg,                        /* Handle to the message*/
	0,                           /* Flags*/
	CMSG_CTRL_VERIFY_HASH,       /* Control type*/
	NULL))                       /* Pointer not used*/
    {
	printf("Verification of hash succeeded. \n");
	printf("The data has not been tampered with.\n");
    }
    else
    {
	printf("Verification of hash failed. Something changed this message .\n");
    }
    
    printf("Test program completed without error. \n");
    
    /*--------------------------------------------------------------------*/
    /* Clean up*/
    
    if(pbEncodedBlob)
	free(pbEncodedBlob);
    if(pbDecoded)
	free(pbDecoded);
    
    CryptMsgClose(hMsg); 
    
    /* Release the CSP.*/
    
    return 1;
} /* End of main*/

/*--------------------------------------------------------------------*/



#if !defined(lint) && !defined(NO_RCSID) && !defined(OK_MODULE_VERSION)
static volatile const char rcsid[] = "\n$Id: hashlo.c,v 1.13 2001/12/25 15:57:52 pre Exp $";
#endif
