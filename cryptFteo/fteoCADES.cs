// Copyright ©2018, Fixosoft (serg.home153@gmail.com)
// All rights reserved.
//
//****************************  CADES -  GOST CSP territorry   ****************************
//Crypto extension for GOST`s: CAdES - CMS Advanced Electronic Signature. 
// Also, CAdESCOM - com interfaces to cades runtime library




using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CAPICOM;


namespace netFteo.Crypt.CADESCOM
{

    /// <summary>
    /// Cadescom type wrapper
    /// for "CAdESCOM.CPCertificate"
    /// </summary>
    public class CAdESCOMCert
    {
       public CAdESCOM.CPCertificate api;

        public CAdESCOMCert()
        {
           if (! CadesWrapper.TestCADESCOM())
            {
          ////TODO  
                throw new System.Runtime.InteropServices.COMException("CADESCOM not present");
            }
        }

        public bool HasPrivateKey()
        {
            return this.api.HasPrivateKey();
            
        }
        public string PrivateKeyProviderName
        {
            get
            {
                return this.api.PrivateKey.ProviderName;
            }
        }
        public string PrivateKeyContainerName
        {
            get
            {
                return this.api.PrivateKey.ContainerName;
            }
        }

        
    }

    /// <summary>
    /// GOST CSP Provider wrapper class. Требует установленнoго CADESCOM (cadescom.dll)
    /// </summary>

    public static class CadesWrapper
    {

        public static bool TestCADESCOM()
        {
            try
            {
                CAdESCOM.CadesSignedData cadesSignedData = new CAdESCOM.CadesSignedData();
                return true;
            }
            catch (System.Runtime.InteropServices.COMException fuckingCOMNotFoundException)
            {
                string testMessage = fuckingCOMNotFoundException.Message;
                return false;
            }
        }


        /// <summary>
        /// Поиск сертификата по CN-имени. Из поля SubjectName выбирается cn=subjectCN 
        /// </summary>
        /// <param name="subject">CN name of subject</param>
        /// <returns></returns>
        public static CAdESCOM.CPCertificate Find(string subjectCN)
        {
            // Сразу ищем в Wyncrypt:
            X509Store store = new X509Store(StoreName.My);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection listCerts = store.Certificates.Find(X509FindType.FindBySubjectName, subjectCN, false);
            string serial = listCerts[0].SerialNumber; // Сохраняем серийник
                                                       // Теперь выбираем в WinCryptEx (CADES) по серийнику:
            CAdESCOM.CPStore Cstore = new CAdESCOM.CPStore();
            Cstore.Open(CAPICOM_STORE_LOCATION.CAPICOM_CURRENT_USER_STORE,
                        "My",
                        CAPICOM_STORE_OPEN_MODE.CAPICOM_STORE_OPEN_READ_ONLY);

            foreach (CAdESCOM.CPCertificate crt in Cstore.Certificates)
            {
                if (crt.SerialNumber == serial)
                    return crt;
            }
            return null;
        }

        public static CAdESCOMCert FindBySerialwr(string serial)
        {
            CAdESCOMCert res = new CAdESCOMCert();
            res.api = FindBySerial(serial);
            return res;
        }

        public static CAdESCOM.CPCertificate FindBySerial(string serial)
        {
            // Сразу ищем в Wyncrypt:
            //X509Store store = new X509Store(StoreName.My);
            //store.Open(OpenFlags.ReadOnly);
            //X509Certificate2Collection listCerts = store.Certificates.Find(X509FindType.FindBySerialNumber, serial, false);
            //string serial = listCerts[0].SerialNumber; // Сохраняем серийник
            // Теперь выбираем в WinCryptEx (CADES) по серийнику:
            CAdESCOM.CPStore Cstore = new CAdESCOM.CPStore();
            Cstore.Open(CAPICOM_STORE_LOCATION.CAPICOM_CURRENT_USER_STORE,
                        "My",
                        CAPICOM_STORE_OPEN_MODE.CAPICOM_STORE_OPEN_READ_ONLY);

            foreach (CAdESCOM.CPCertificate crt in Cstore.Certificates)
            {
                if (crt.SerialNumber == serial)
                    return crt;
            }
            return null;
        }


    /*
        /// <summary>
        /// Подписать файл отсоедниенной подписью
        /// </summary>
        /// <param name="filename">Имя файла</param>
        /// <param name="subjectname">Владелец сертифката (Субьект)</param>
        public static void SignFile(string filename, string subjectname)
        {
            byte[] filebody = System.IO.File.ReadAllBytes(filename);
            byte[] file_sig = null;

            X509Certificate2 cert = CryptographyWrapper.GetCertBySubjectCN(subjectname);
            //Select CryptoProviderType by szOID:
            if (cert.SignatureAlgorithm.Value == "1.2.643.2.2.3") // szOID for CSP Crypto Pro (GOST 3411)
                file_sig = Sign_GOST(filebody, Find(subjectname));

            if (cert.SignatureAlgorithm.Value == "1.2.840.113549.1.1.4") // szOID_RSA_MD5RSA   "1.2.840.113549.1.1.4"
                file_sig = Sign_CAPICOM(filebody, (CAPICOM.Certificate)cert);

            System.IO.File.WriteAllBytes(filename + ".sig", file_sig);
        }

        */
        private static byte[] Sign_X509(byte[] filebody, string subjectname)
        {
            // Access Personal (MY) certificate store of current user

            X509Store my = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            my.Open(OpenFlags.ReadOnly);


            // Find the certificate we'll use to sign

            RSACryptoServiceProvider csp = null;

            foreach (X509Certificate2 cert in my.Certificates)
            {

                if (cert.Subject.Contains(subjectname))
                {

                    // We found it.

                    // Get its associated CSP and private key

                    csp = (RSACryptoServiceProvider)cert.PrivateKey;

                }

            }

            if (csp == null)
            {

                throw new Exception("No valid cert (CSP) was found");

            }


            // Hash the data

            SHA1Managed sha1 = new SHA1Managed();

            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] hash = sha1.ComputeHash(filebody);
            // Sign the hash
            byte[] sig = csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
            return sig;
        }


        /// <summary>
        /// Подпись через COM-объекты  "CAPICOM"  - нужна dll (классы COM)
        /// </summary>
        /// <param name="_FileName"></param>
        /// <param name="cert"></param>
        private static byte[] Sign_CAPICOM(byte[] filebody, CAPICOM.Certificate cert)
        {
            CAPICOM.SignedData CSPdata = new CAPICOM.SignedData();
            CSPdata.Content = Convert.ToBase64String(filebody);
            if (cert.HasPrivateKey())
            {

                // RSACryptoServiceProvider csp = 
                // Hash the data
                // SHA1Managed sha1 = new SHA1Managed();
                // byte[] hash = sha1.ComputeHash(filebody);
                // byte[] sig = csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
            }

            CAPICOM.Signer Signer = new CAPICOM.Signer();
            Signer.Certificate = cert;
            Signer.Options = CAPICOM.CAPICOM_CERTIFICATE_INCLUDE_OPTION.CAPICOM_CERTIFICATE_INCLUDE_WHOLE_CHAIN;
            string signature = CSPdata.Sign(Signer, true, CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_ANY);
            byte[] sigMsg = Encoding.Default.GetBytes(signature);
            return sigMsg;
        }
        // * /

        /// <summary>
        /// Sign file with GOST CSP 34.11-94 (CADESCOM - COM of CryptoPro), OID = 1.2.643.2.2.3
        /// return detached message
        /// </summary>
        /// <remarks>Used CAPICOM, CADESCOM COM interfaces</remarks>
        /// <param name="filebody">Message to sign (file body) </param>
        /// <param name="cert">Subject certificate</param>
        public static byte[] Sign_GOST(string filename, string subject)
        {
            byte[] filebody = System.IO.File.ReadAllBytes(filename);
            CAdESCOM.CPCertificate cert = Find(subject);
            if (cert == null) return null;
            CAdESCOM.CadesSignedData cadesSignedData = new CAdESCOM.CadesSignedData();

            cadesSignedData.ContentEncoding = CAdESCOM.CADESCOM_CONTENT_ENCODING_TYPE.CADESCOM_BASE64_TO_BINARY; // Первым строкой - кодировку
            cadesSignedData.Content = Convert.ToBase64String(filebody);             // иначе перекодирует дважды !!!!
                                                                                    //Хэш-значение данных


            CAdESCOM.CPSigner CSPSigner = new CAdESCOM.CPSigner();
            CSPSigner.Certificate = cert;
            //TSA не нужен только для CADESCOM_CADES_BES
            CSPSigner.TSAAddress = "http://www.cryptopro.ru/tsp/tsp.srf"; //  адрес службы штампов времени.
                                                                          //"http://testca.cryptopro.ru/tsp/";
            CSPSigner.Options = CAPICOM.CAPICOM_CERTIFICATE_INCLUDE_OPTION.CAPICOM_CERTIFICATE_INCLUDE_WHOLE_CHAIN;

            try
            {
                CAdESCOM.CPHashedData Hash = new CAdESCOM.CPHashedData();
                Hash.Algorithm = (CAPICOM.CAPICOM_HASH_ALGORITHM)CAdESCOM.CADESCOM_HASH_ALGORITHM.CADESCOM_HASH_ALGORITHM_CP_GOST_3411_2012_256;//.CADESCOM_HASH_ALGORITHM_CP_GOST_3411;
                Hash.DataEncoding = CAdESCOM.CADESCOM_CONTENT_ENCODING_TYPE.CADESCOM_BASE64_TO_BINARY; // нет в примерах cdn.crypto
                Hash.Hash(cadesSignedData.Content); // Создать хэш строки. Есть расширение - SetHashValue() - инициализация готовым хэш-значением
                                                    /* CADESCOM_CADES_X_LONG_TYPE_1 : got errror 
                                                     * OID = 1.2.643.2.2.3 SCP Error:  Лицензия на КриптоПро TSP Client истекла или не была введена
                                                     * ErrorCode - 1039138496
                                                     * source CAdESCOM.CadesSignedData.1
                                                                                   */
                string resHashCades = cadesSignedData.SignHash((CAPICOM.HashedData)Hash,
                                                       CSPSigner,
                                                       CAdESCOM.CADESCOM_CADES_TYPE.CADESCOM_CADES_BES,//  for  HASH_ALGORITHM_CP_GOST_3411 must be CADESCOM_CADES_BES,//CADESCOM_CADES_DEFAULT,
                                                       CAdESCOM.CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);
                //Здесь требуется лицензия на КриптоПро TSP Client:
                //' Создание параллельной подписи CAdES X Long Type 1
                //resHashCades = cadesSignedData.CoSignCades(CSPSigner, CAdESCOM.CADESCOM_CADES_TYPE.CADESCOM_CADES_DEFAULT, CAdESCOM.CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);
                //дополнение подписи CAdES BES до подписи CAdES X Long Type 1:
                //   resHashCades = cadesSignedData.EnhanceCades(CAdESCOM.CADESCOM_CADES_TYPE.CADESCOM_CADES_DEFAULT, CSPSigner.TSAAddress, CAdESCOM.CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);

                // this'll return BASE64 coded message:
                // Encoding.Default.GetBytes(resHashCades);
                // return decoded from base64 message
                return Convert.FromBase64String(resHashCades);
            }




            catch (System.Runtime.InteropServices.COMException ex)
            {

                return Encoding.Default.GetBytes("OID = 1.2.643.2.2.3 SCP Error: \r\n" +
                    ex.Message + "\r\n" +
                    " ErrorCode " + ex.ErrorCode.ToString() + "\r\n" +
                    " source " + ex.Source);
            }
        } //Sign_GOST

        public static void ReadSign(string filename)
        {
            byte[] filebody = System.IO.File.ReadAllBytes(filename);
            using (var reader = new System.IO.StreamReader(filename, Encoding.Default, true))
            {
                if (reader.Peek() >= 0) // you need this!
                    reader.Read();
                string BodyEncoding = reader.CurrentEncoding.EncodingName;

            }

            string convString = Convert.ToString(filebody);

            RSACryptoServiceProvider prov = new RSACryptoServiceProvider();
            //prov.CspKeyContainerInfo
            CAdESCOM.CadesSignedData sig = new CAdESCOM.CadesSignedData();
            sig.Content = Convert.ToString(filebody);
            sig.Verify(convString, true);

        }

    } 
 }

 
    





