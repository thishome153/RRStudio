// Copyright ©2018, Fixosoft (serg.home153@gmail.com)
// All rights reserved.
//
//****************************  CADES -  GOST CSP territorry   ****************************
//Crypto extension for GOST`s: CAdES - CMS Advanced Electronic Signature. 
// Also, CAdESCOM - com interfaces to cades runtime library




using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
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
                throw new COMException("CADESCOM not present");
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

        
        /*
        public static bool GettimeStamp(System.Security.Cryptography.Pkcs.SignedCms  signedCms)
        {
            foreach (var signerInfo in signedCms.SignerInfos)
            {
                foreach (var unsignedAttribute in signerInfo.UnsignedAttributes)
                {
                    if (unsignedAttribute.Oid.Value == cryptFteo.WinCrypt.szOID_RSA_counterSign)
                    {
                        foreach (var counterSignInfo in signerInfo.CounterSignerInfos)
                        {
                            foreach (var signedAttribute in counterSignInfo.SignedAttributes)
                            {
                                if (signedAttribute.Oid.Value == cryptFteo.WinCrypt.szOID_RSA_signingTime)
                                {
                                    System.Runtime.InteropServices.ComTypes.FILETIME fileTime = new System.Runtime.InteropServices.ComTypes.FILETIME();
                                    int fileTimeSize = Marshal.SizeOf(fileTime);
                                    IntPtr fileTimePtr = Marshal.AllocCoTaskMem(fileTimeSize);
                                    Marshal.StructureToPtr(fileTime, fileTimePtr, true);

                                    byte[] buffdata = new byte[fileTimeSize];
                                    Marshal.Copy(fileTimePtr, buffdata, 0, fileTimeSize);

                                    uint buffSize = (uint)buffdata.Length;

                                    uint encoding = cryptFteo.WinCrypt.X509_ASN_ENCODING | cryptFteo.WinCrypt.PKCS_7_ASN_ENCODING;

                                    UIntPtr rsaSigningTime = (UIntPtr)(uint)Marshal.StringToHGlobalAnsi(cryptFteo.WinCrypt.szOID_RSA_signingTime);

                                    byte[] pbData = signedAttribute.Values[0].RawData;
                                    uint ucbData = (uint)pbData.Length;

                                    bool workie = cryptFteo.WinCrypt.CryptDecodeObject(encoding, RsaSigningTime.ToUInt32(), pbData, ucbData, 0, buffdata, ref buffSize);

                                    if (workie)
                                    {
                                        IntPtr fileTimePtr2 = Marshal.AllocCoTaskMem(buffdata.Length);
                                        Marshal.Copy(buffdata, 0, fileTimePtr2, buffdata.Length);
                                        System.Runtime.InteropServices.ComTypes.FILETIME fileTime2 = (System.Runtime.InteropServices.ComTypes.FILETIME)Marshal.PtrToStructure(fileTimePtr2, typeof(System.Runtime.InteropServices.ComTypes.FILETIME));

                                        long hFT2 = (((long)fileTime2.dwHighDateTime) << 32) + ((uint)fileTime2.dwLowDateTime);

                                        DateTime dte = DateTime.FromFileTime(hFT2);
                                        Console.WriteLine(dte.ToString());
                                    }
                                    else
                                    {
                                        throw new Exception(Marshal.GetLastWin32Error());
                                    }

                                }
                            }

                        }

                        return true;
                    }

                }
            }
            return false;
        }
        */


  
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



        // 1.2.643.100.111  -- software
        // 1.2.643.100.112  -- software

        //  1.2.643.7.1.1.3.2 - GOST signature
        // "1.2.840.113549.1.9.16.2.14";// =  id - aa - timeStampToken(14)
        // 1.2.840.113549.1.9.16.2.12 = signing-certificate(12) 
        // 1.2.840.113549.1.9.16.2.47 = id - aa - signingCertificateV2
        // 1.2.840.113549.1.9.3 = contenttype
        // 1.2.840.113549.1.9.4  = messageDigest(4) 
        // szOID_RSA_signingTime = "1.2.840.113549.1.9.5";
        // szOID_RSA_counterSign = "1.2.840.113549.1.9.6";

        // 1.2.643.2.51.1.1.1 = TExpress v. 2.7.40828.4 @ CPCryptoNet v. 3.30.40628.0, [80] - Crypto-Pro GOST R


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

            OID oid = new OID();
            oid.Value = "1.2.643.2.51.1.1.1";
            oid.Name = CAPICOM_OID.CAPICOM_OID_OTHER;

            CAdESCOM.CPAttribute SignTimeAttr = new CAdESCOM.CPAttribute();
            SignTimeAttr.Name = CAdESCOM.CADESCOM_ATTRIBUTE.CADESCOM_AUTHENTICATED_ATTRIBUTE_SIGNING_TIME;
            SignTimeAttr.Value = DateTime.UtcNow;

            CAdESCOM.CPAttribute Attr = new CAdESCOM.CPAttribute();
            Attr.Name = CAdESCOM.CADESCOM_ATTRIBUTE.CADESCOM_AUTHENTICATED_ATTRIBUTE_DOCUMENT_NAME;
            Attr.Value = "XML Reader signature";

            CAdESCOM.CPAttribute AttrDesc = new CAdESCOM.CPAttribute();
            AttrDesc.Name = CAdESCOM.CADESCOM_ATTRIBUTE.CADESCOM_AUTHENTICATED_ATTRIBUTE_DOCUMENT_DESCRIPTION;
            AttrDesc.Value = "Signed document";

            //Pkcs9SigningTime stime = new Pkcs9SigningTime(DateTime.UtcNow);
            Pkcs9AttributeObject p9 = new Pkcs9SigningTime(DateTime.UtcNow);
            Pkcs9AttributeObject ao = new Pkcs9AttributeObject();
            AsnEncodedData dt = new AsnEncodedData(p9.Oid, p9.RawData);
           // ao.CopyFrom(dt);
            
            if (cert == null) return null;
            CAdESCOM.CadesSignedData cadesSignedData = new CAdESCOM.CadesSignedData();

            cadesSignedData.ContentEncoding = CAdESCOM.CADESCOM_CONTENT_ENCODING_TYPE.CADESCOM_BASE64_TO_BINARY; // Первым строкой - кодировку
            cadesSignedData.Content = Convert.ToBase64String(filebody);             // иначе перекодирует дважды !!!!        //Хэш-значение данных
            cadesSignedData.DisplayData = CAdESCOM.CADESCOM_DISPLAY_DATA.CADESCOM_DISPLAY_DATA_CONTENT;

            CAdESCOM.CPSigner CSPSigner = new CAdESCOM.CPSigner();
            CSPSigner.Certificate = cert;
            //TSA не нужен только для CADESCOM_CADES_BES
            CSPSigner.TSAAddress = "http://www.cryptopro.ru/tsp/tsp.srf"; //  адрес службы штампов времени.

//            CAdESCOM.About ab = new CAdESCOM.About();


            CSPSigner.UnauthenticatedAttributes.Add(SignTimeAttr);            //"http://testca.cryptopro.ru/tsp/";
            CSPSigner.UnauthenticatedAttributes.Add(Attr);
            CSPSigner.UnauthenticatedAttributes.Add(AttrDesc);

            //CSPSigner.UnauthenticatedAttributes.Add(p9);

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
                                                     * source CAdESCOM.CadesSignedData.1 */


                string resHashCades = cadesSignedData.SignHash((CAPICOM.HashedData)Hash,
                                                       CSPSigner,
                                                       CAdESCOM.CADESCOM_CADES_TYPE.CADESCOM_CADES_BES,//  for  HASH_ALGORITHM_CP_GOST_3411 must be CADESCOM_CADES_BES,//CADESCOM_CADES_DEFAULT,
                                                       CAdESCOM.CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);

                /*
                //Working, but long sig arrived:
                string resHashCades = cadesSignedData.SignCades(        CSPSigner,
                                                       CAdESCOM.CADESCOM_CADES_TYPE.CADESCOM_CADES_BES//  for  HASH_ALGORITHM_CP_GOST_3411 must be CADESCOM_CADES_BES,//CADESCOM_CADES_DEFAULT,
                                                       );
                                                       */

                //Здесь требуется лицензия на КриптоПро TSP Client:
                //' Создание параллельной подписи CAdES X Long Type 1
                //resHashCades = cadesSignedData.CoSignCades(CSPSigner, CAdESCOM.CADESCOM_CADES_TYPE.CADESCOM_CADES_DEFAULT, CAdESCOM.CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);
                //дополнение подписи CAdES BES до подписи CAdES X Long Type 1:
                 //resHashCades = cadesSignedData.EnhanceCades(CAdESCOM.CADESCOM_CADES_TYPE.CADESCOM_CADES_T, CSPSigner.TSAAddress, CAdESCOM.CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);
                // Got
                /*
                 OID = 1.2.643.2.2.3 SCP Error: Лицензия на КриптоПро TSP Client истекла или не была введена. ErrorCode -1039138496
                 */


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

 
    





