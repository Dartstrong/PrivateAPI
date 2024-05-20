using System.Security.Cryptography;
using PrivateAPI.Models;
using PrivateAPI.Entities;
using System.IO;
using System.Text;
using System;

namespace PrivateAPI.HelperClasses
{
    public class Crypter : Converter
    {
        public string Encrypt(byte[] data, RSAParameters publicKey)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(publicKey);
                byte[] encryptedData = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA1);
                return ByteArrayToIntArrayToStr(encryptedData);
            }
        }
        public string Encrypt(string data, Session session)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = StrToIntArrayToByteArray(session.SymmetricKey);
                aes.IV = StrToIntArrayToByteArray(session.InitVector);
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream msEncrypt = new())
                {
                    using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new(csEncrypt))
                        {
                            swEncrypt.Write(data);
                        }
                        return ByteArrayToIntArrayToStr(msEncrypt.ToArray());
                    }
                }
            }
        }
        public (Account, DeviceID) Decrypt(AuthorizationData authorizationData, Session session)
        {
            Account decryptedAccount = new();
            DeviceID decryptedDeviceID = new();
            using (Aes aes= Aes.Create())
            {
                aes.Key = StrToIntArrayToByteArray(session.SymmetricKey);
                aes.IV = StrToIntArrayToByteArray(session.InitVector);
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                decryptedAccount.Login = DecryptAES(authorizationData.LoginStr, decryptor);
                decryptedAccount.Sample = ByteArrayToIntArrayToStr(new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(DecryptAES(authorizationData.PasswordStr, decryptor))));
                if(authorizationData.EmailStr!=null) decryptedAccount.Email = DecryptAES(authorizationData.EmailStr, decryptor);        
                decryptedDeviceID.Id = Int16.Parse(DecryptAES(authorizationData.DeviceIdStr, decryptor));
            }
            return (decryptedAccount, decryptedDeviceID);
        }
        public RequestStartDialogue Decrypt(RequestStartDialogue requestStartDialogue, Session session)
        {
            RequestStartDialogue decryptedRequestStartDialogue = new();
            using (Aes aes = Aes.Create())
            {
                aes.Key = StrToIntArrayToByteArray(session.SymmetricKey);
                aes.IV = StrToIntArrayToByteArray(session.InitVector);
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                decryptedRequestStartDialogue.Sender = DecryptAES(requestStartDialogue.Sender, decryptor);
                decryptedRequestStartDialogue.SenderdDeviceId = DecryptAES(requestStartDialogue.SenderdDeviceId, decryptor);
                decryptedRequestStartDialogue.Receiver = DecryptAES(requestStartDialogue.Receiver, decryptor);
                decryptedRequestStartDialogue.PublicKeyModulus = DecryptAES(requestStartDialogue.PublicKeyModulus, decryptor);
                decryptedRequestStartDialogue.PublicKeyExponent = DecryptAES(requestStartDialogue.PublicKeyExponent, decryptor);
            }
        }
        private string DecryptAES(string data, ICryptoTransform decryptor)
        {
            using (MemoryStream msDecrypt = new(StrToIntArrayToByteArray(data)))
            {
                using (CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new(csDecrypt))
                    {
                        string result = srDecrypt.ReadToEnd();
                        srDecrypt.Close();
                        csDecrypt.Close();
                        msDecrypt.Close();
                        return result;
                    }
                }
            }
        }
    }
}
