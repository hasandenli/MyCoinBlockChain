using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Entities
{
    public class SecurityUtil
    {
        public static String ApplySha256(String input){
            try 
            {
                SHA256 sha256 = SHA256.Create();
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] outputBytes = sha256.ComputeHash(inputBytes);
                return Convert.ToBase64String(outputBytes);
            }
            catch(Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public static string GetPrivateKeyBase64(AsymmetricKeyParameter key)
        {
            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(key);
            byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetDerEncoded();

            return Convert.ToBase64String(serializedPrivateBytes);
        }

        public static string GetPublicKeyBase64(AsymmetricKeyParameter key)
        {
            SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(key);
            byte[] serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();

            return Convert.ToBase64String(serializedPublicBytes);
        }

        public static string Sign(AsymmetricKeyParameter pvtKey, string msgToSign)
        {
            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(pvtKey);
            byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetDerEncoded();

            RsaPrivateCrtKeyParameters privateKey = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(serializedPrivateBytes);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            RSAParameters rsaParameters = ToRSAParameters((RsaPrivateCrtKeyParameters)privateKey);

            rsa.ImportParameters(rsaParameters);
            byte[] dataBytes = Encoding.UTF8.GetBytes(msgToSign);
            byte[] signedBytes = rsa.SignData(dataBytes, "SHA256");

            return Convert.ToBase64String(signedBytes);
        }

        public static  bool Verify(AsymmetricKeyParameter pubKey, string originalMessage, string signedMessage)
        {
            SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pubKey);
            byte[] serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();

            RsaKeyParameters publicKey = (RsaKeyParameters)PublicKeyFactory.CreateKey(serializedPublicBytes);

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            RSAParameters rsaParameters = new RSAParameters();
            rsaParameters.Modulus = publicKey.Modulus.ToByteArrayUnsigned();
            rsaParameters.Exponent = publicKey.Exponent.ToByteArrayUnsigned();
            rsa.ImportParameters(rsaParameters);

            byte[] bytesToVerify = Encoding.UTF8.GetBytes(originalMessage);
            byte[] signedBytes = Convert.FromBase64String(signedMessage);

            bool success = rsa.VerifyData(bytesToVerify, CryptoConfig.MapNameToOID("SHA256"), signedBytes);

            return success;
        }

        public static  string Encrypt(byte[] pubKey, string txtToEncrypt)
        {
            RsaKeyParameters publicKey = (RsaKeyParameters)PublicKeyFactory.CreateKey(pubKey);

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            RSAParameters rsaParameters = new RSAParameters();
            rsaParameters.Modulus = publicKey.Modulus.ToByteArrayUnsigned();
            rsaParameters.Exponent = publicKey.Exponent.ToByteArrayUnsigned();
            rsa.ImportParameters(rsaParameters);

            byte[] bytes = Encoding.UTF8.GetBytes(txtToEncrypt);
            byte[] enc = rsa.Encrypt(bytes, false);
            string base64Enc = Convert.ToBase64String(enc);

            return base64Enc;
        }

        public string Decrypt(string pvtKey, string txtToDecrypt)
        {
            RsaPrivateCrtKeyParameters privateKey = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(pvtKey));
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            RSAParameters rsaParameters2 = ToRSAParameters((RsaPrivateCrtKeyParameters)privateKey);

            rsa.ImportParameters(rsaParameters2);

            byte[] dec = rsa.Decrypt(Convert.FromBase64String(txtToDecrypt), false);
            string decStr = Encoding.UTF8.GetString(dec);

            return decStr;
        }

        public static RSAParameters ToRSAParameters(RsaPrivateCrtKeyParameters privKey)
		{
			RSAParameters rp = new RSAParameters();
			rp.Modulus = privKey.Modulus.ToByteArrayUnsigned();
			rp.Exponent = privKey.PublicExponent.ToByteArrayUnsigned();
			rp.D = privKey.Exponent.ToByteArrayUnsigned();
			rp.P = privKey.P.ToByteArrayUnsigned();
			rp.Q = privKey.Q.ToByteArrayUnsigned();
			rp.DP = privKey.DP.ToByteArrayUnsigned();
			rp.DQ = privKey.DQ.ToByteArrayUnsigned();
			rp.InverseQ = privKey.QInv.ToByteArrayUnsigned();
			return rp;
		}
    }
}