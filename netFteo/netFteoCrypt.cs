using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;

// CADES extentions from Microsoft CSP:
using CAPICOM;


namespace netFteo.Cryptography
{
    class netFteoCapiComExtender
    {
        CAPICOM.Certificate cert = new Certificate();

    }



    /// <summary>
    /// Класс-обертка для Cryptography
    /// </summary>
    public static class CryptographyWrapper
    {
        private static string encrFolder = @"C:\Encrypt\";
        // Encrypt a file using a public key.
        public static void EncryptFile(string inFile, RSACryptoServiceProvider rsaPublicKey)
        {
            using (AesManaged aesManaged = new AesManaged())
            {
                // Create instance of AesManaged for
                // symetric encryption of the data.
                aesManaged.KeySize = 256;
                aesManaged.BlockSize = 128;
                aesManaged.Mode = CipherMode.CBC;
                using (ICryptoTransform transform = aesManaged.CreateEncryptor())
                {
                    RSAPKCS1KeyExchangeFormatter keyFormatter = new RSAPKCS1KeyExchangeFormatter(rsaPublicKey);
                    byte[] keyEncrypted = keyFormatter.CreateKeyExchange(aesManaged.Key, aesManaged.GetType());

                    // Create byte arrays to contain
                    // the length values of the key and IV.
                    byte[] LenK = new byte[4];
                    byte[] LenIV = new byte[4];

                    int lKey = keyEncrypted.Length;
                    LenK = BitConverter.GetBytes(lKey);
                    int lIV = aesManaged.IV.Length;
                    LenIV = BitConverter.GetBytes(lIV);

                    // Write the following to the FileStream
                    // for the encrypted file (outFs):
                    // - length of the key
                    // - length of the IV
                    // - ecrypted key
                    // - the IV
                    // - the encrypted cipher content

                    int startFileName = inFile.LastIndexOf("\\") + 1;
                    // Change the file's extension to ".enc"
                    string outFile = encrFolder + inFile.Substring(startFileName, inFile.LastIndexOf(".") - startFileName) + ".enc";
                    Directory.CreateDirectory(encrFolder);

                    using (FileStream outFs = new FileStream(outFile, FileMode.Create))
                    {

                        outFs.Write(LenK, 0, 4);
                        outFs.Write(LenIV, 0, 4);
                        outFs.Write(keyEncrypted, 0, lKey);
                        outFs.Write(aesManaged.IV, 0, lIV);

                        // Now write the cipher text using
                        // a CryptoStream for encrypting.
                        using (CryptoStream outStreamEncrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                        {

                            // By encrypting a chunk at
                            // a time, you can save memory
                            // and accommodate large files.
                            int count = 0;
                            int offset = 0;

                            // blockSizeBytes can be any arbitrary size.
                            int blockSizeBytes = aesManaged.BlockSize / 8;
                            byte[] data = new byte[blockSizeBytes];
                            int bytesRead = 0;

                            using (FileStream inFs = new FileStream(inFile, FileMode.Open))
                            {
                                do
                                {
                                    count = inFs.Read(data, 0, blockSizeBytes);
                                    offset += count;
                                    outStreamEncrypted.Write(data, 0, count);
                                    bytesRead += blockSizeBytes;
                                }
                                while (count > 0);
                                inFs.Close();
                            }
                            outStreamEncrypted.FlushFinalBlock();
                            outStreamEncrypted.Close();
                        }
                        outFs.Close();
                    }
                }
            }
        }


        public static byte[] Sign(string text, string certSubject)

        {

            // Access Personal (MY) certificate store of current user

            X509Store my = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            my.Open(OpenFlags.ReadOnly);


            // Find the certificate we'll use to sign

            RSACryptoServiceProvider csp = null;

            foreach (X509Certificate2 cert in my.Certificates)

            {

                if (cert.Subject.Contains(certSubject))

                {

                    // We found it.

                    // Get its associated CSP and private key

                    csp = (RSACryptoServiceProvider)cert.PrivateKey;

                }

            }

            if (csp == null)

            {

                throw new Exception("No valid cert was found");

            }


            // Hash the data

            SHA1Managed sha1 = new SHA1Managed();

            UnicodeEncoding encoding = new UnicodeEncoding();

            byte[] data = encoding.GetBytes(text);

            byte[] hash = sha1.ComputeHash(data);


            // Sign the hash

            return csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));

        }
        public static byte[] Sign(byte[] filebody, X509Certificate2 certificate)
        {

            // Access Personal (MY) certificate store of current user

            X509Store my = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            my.Open(OpenFlags.ReadOnly);
            RSACryptoServiceProvider csp = null;
            if (certificate.HasPrivateKey)
            {
                csp = (RSACryptoServiceProvider)certificate.PrivateKey;
                if (csp == null)
                {
                    throw new Exception("No valid CSP was found");
                }
            }
            //else
            //    csp = (RSACryptoServiceProvider)certificate.PublicKey.Key;


            // Hash the data

            SHA1Managed sha1 = new SHA1Managed();
            UnicodeEncoding encoding = new UnicodeEncoding();



            byte[] hash = sha1.ComputeHash(filebody);

            //return  csp.SignData(filebody, new SHA512CryptoServiceProvider());
            // Sign the hash
            return csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
        }

        public static bool Verify(string text, byte[] signature, string certPath)

        {

            // Load the certificate we'll use to verify the signature from a file

            X509Certificate2 cert = new X509Certificate2(certPath);

            // Note:

            // If we want to use the client cert in an ASP.NET app, we may use something like this instead:

            // X509Certificate2 cert = new X509Certificate2(Request.ClientCertificate.Certificate);


            // Get its associated CSP and public key

            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key;


            // Hash the data

            SHA1Managed sha1 = new SHA1Managed();

            UnicodeEncoding encoding = new UnicodeEncoding();

            byte[] data = encoding.GetBytes(text);

            byte[] hash = sha1.ComputeHash(data);


            // Verify the signature with the hash

            return csp.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), signature);

        }

        /// <summary>
        /// verifying the signature of PKCS #7 formatted messages
        /// </summary>
        /*
        public static bool VerifyP7s(byte[] signature, X509Certificate2 certificate)
        {
            if (signature == null)
                throw new ArgumentNullException("signature");


            // decode the signature

            System.Security.Cryptography.Pkcs.SignedCms verifyCms = new System.Security.Cryptography.Pkcs.SignedCms();
            verifyCms.Decode(signature);
            var test = verifyCms.ContentInfo;
            //verifyCms.CheckSignature(true);
            X509Certificate2Collection cmsCerts = verifyCms.Certificates;


            // verify it
            if (certificate == null)
                throw new ArgumentNullException("certificate");
            try
            {
                verifyCms.CheckSignature(new X509Certificate2Collection(certificate), false);
                return true;
            }
            catch (CryptographicException)
            {
                return false;
            }
        }
          */
        public static List<string> DisplayCerts(byte[] signature)
        {
            if (signature == null)
                throw new ArgumentNullException("signature");
            List<string> res = new List<string>();
            try
            {
                // decode the signature

                System.Security.Cryptography.Pkcs.SignedCms verifyCms = new System.Security.Cryptography.Pkcs.SignedCms();
                verifyCms.Decode(signature);
                var test = verifyCms.ContentInfo;
                X509Certificate2Collection cmsCerts = verifyCms.Certificates;
                foreach (X509Certificate2 c in cmsCerts)
                    res.Add(c.GetNameInfo(X509NameType.SimpleName, false));

            }
            catch (CryptographicException)
            {
                return null;
            }
            return res;
        }

        /// <summary>
        /// Листинг сертификатов. Средствами только wyncrypt.
        /// </summary>
        /// <returns></returns>
        public static List<string> DisplayCerts()
        {
            List<string> res = new List<string>();
            X509Store store;
            /*
            store = new X509Store(StoreName.Root);
            store.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 c in store.Certificates)
                res.Add(c.SubjectName.Name);
            store.Close();
            */
            store = new X509Store(StoreName.My);
            store.Open(OpenFlags.ReadOnly);
            /* for .FindBySubjectDistinguishedName:
             foreach (X509Certificate2 c in store.Certificates)
                 res.Add(c.SubjectName.Name);
                 */
            // for .FindBySubjectCNName
            foreach (X509Certificate2 c in store.Certificates)
            {
                res.Add(c.GetNameInfo(X509NameType.SimpleName, false));
            }
            /*

             foreach (X509Certificate2 c in store.Certificates)
                 res.Add(c.GetName().ToString());
                 */
            store.Close();
            return res;
        }

        public static List<X509Certificate2> DisplayCerts(string storename)
        {
            List<X509Certificate2> res = new List<X509Certificate2>();
            X509Store store = new X509Store(StoreName.My);
            store.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 c in store.Certificates)
            {
                res.Add(c);
            }
            store.Close();
            return res;
        }

        /// <summary>
        /// Поиск сертификата по имени
        /// </summary>
        /// <param name="subject">X500DistinguishedName - string eq</param>
        /// <returns></returns>
        public static X509Certificate2 GetCertBySubject(string subject)
        {

            X509Store store = new X509Store(StoreName.My);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection listCerts = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, subject, false);
            if (listCerts.Count == 1)
            { return listCerts[0]; }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Поиск сертификата по CN-имени
        /// </summary>
        /// <param name="subject">CN name of subject</param>
        /// <returns></returns>
        public static X509Certificate2 GetCertBySubjectCN(string subjectCN)
        {

            X509Store store = new X509Store(StoreName.My);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection listCerts = store.Certificates.Find(X509FindType.FindBySubjectName, subjectCN, false);
            if (listCerts.Count == 1)
            { return listCerts[0]; }
            else
            {
                return null;
            }
        }

        public static X509Certificate2 GetCertBySerial(string serial)
        {

            X509Store store = new X509Store(StoreName.My);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection listCerts = store.Certificates.Find(X509FindType.FindBySerialNumber, serial, false);
            if (listCerts.Count == 1)
            { return listCerts[0]; }
            else
            {
                return null;
            }
        }
    }


    //****************************  CADES -  GOST CSP territorry   ****************************


    /// <summary>
    /// GOST CSP Provider wrapper class. Требует установленнoго CADESCOM (cadescom.dll)
    /// </summary>
    /*
    public static class CadesWrapper
    {
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
               file_sig =  Sign_GOST(filebody, Find(subjectname));

            if (cert.SignatureAlgorithm.Value == "1.2.840.113549.1.1.4") // szOID_RSA_MD5RSA   "1.2.840.113549.1.1.4"
                file_sig = Sign_CAPICOM(filebody, (CAPICOM.Certificate) cert);
            
            System.IO.File.WriteAllBytes(filename + ".sig", file_sig);
        }


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
            byte[] sig= csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
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

        /// <summary>
        /// Подписание по ГОСТ 34.11-94 (CADESCOM - COM of CryptoPro), OID = 1.2.643.2.2.3
        /// </summary>
        /// <param name="filebody">Сообщение для подписания</param>
        /// <param name="cert">Сертификат</param>
        private static byte[] Sign_GOST(byte[] filebody, CAdESCOM.CPCertificate cert)
        {
            CAdESCOM.CadesSignedData CSPdata = new CAdESCOM.CadesSignedData();
            CSPdata.ContentEncoding = CAdESCOM.CADESCOM_CONTENT_ENCODING_TYPE.CADESCOM_BASE64_TO_BINARY; // Первым строкой - кодировку
            CSPdata.Content = Convert.ToBase64String(filebody);                 // иначе перекодирует дважды !!!!
            //Хэш-значение данных
            CAdESCOM.CPHashedData Hash = new CAdESCOM.CPHashedData();
            Hash.Algorithm = (CAPICOM.CAPICOM_HASH_ALGORITHM)CAdESCOM.CADESCOM_HASH_ALGORITHM.CADESCOM_HASH_ALGORITHM_CP_GOST_3411;
            Hash.DataEncoding = CAdESCOM.CADESCOM_CONTENT_ENCODING_TYPE.CADESCOM_BASE64_TO_BINARY;
            Hash.Hash(CSPdata.Content); // Создать хэш строки. Есть расширение - SetHashValue() - инициализация готовым хэш-значением
            
          
            CAdESCOM.CPSigner CSPSigner = new CAdESCOM.CPSigner();
            CSPSigner.Certificate = cert;
            //TSA адрес в какой-то момент требовался - никак не хотело работать, а потом перестало требоваться....блеатдь (я такой внимательный)
            //CSPSigner.TSAAddress = "http://www.cryptopro.ru/tsp/tsp.srf"; //  адрес службы штампов времени.
                                                                          //"http://testca.cryptopro.ru/tsp/";
            CSPSigner.Options = CAPICOM.CAPICOM_CERTIFICATE_INCLUDE_OPTION.CAPICOM_CERTIFICATE_INCLUDE_WHOLE_CHAIN;

            try
            {
                string resHashCades = CSPdata.SignHash((CAPICOM.HashedData)Hash, 
                                                       CSPSigner, 
                                                       CAdESCOM.CADESCOM_CADES_TYPE.CADESCOM_CADES_BES,
                                                       CAdESCOM.CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);
                return Encoding.Default.GetBytes(resHashCades);
                /*  // Это тоже дает верную подпись, без употребелния CAdESCOM.CPHashedData :
                string resSignCades = CSPdata.SignCades(CSPSigner, 
                 *                                      CAdESCOM.CADESCOM_CADES_TYPE.CADESCOM_CADES_BES, 
                 *                                      true,
                 *                                      CAdESCOM.CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);       
                   return Encoding.Default.GetBytes(resSignCades);
                
            }




            catch (System.Runtime.InteropServices.COMException ex)
            {

                return Encoding.Default.GetBytes("OID = 1.2.643.2.2.3 SCP Error: \r\n" +
                    ex.Message + "\r\n" +
                    " ErrorCode " + ex.ErrorCode.ToString() + "\r\n" +
                    " source " + ex.Source);
            }
         }

    }
    */


}
