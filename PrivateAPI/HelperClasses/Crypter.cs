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
        public (Account, DeviceID, string, RSAPublicKey) Decrypt(RequestStartDialogue requestStartDialogue, Session session)
        {
            Account decryptedAccount = new();
            DeviceID decryptedDeviceId = new();
            string decryptedReceiverLogin;
            RSAPublicKey decryptedPublicKey = new();
            using (Aes aes = Aes.Create())
            {
                aes.Key = StrToIntArrayToByteArray(session.SymmetricKey);
                aes.IV = StrToIntArrayToByteArray(session.InitVector);
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                decryptedAccount.Login = DecryptAES(requestStartDialogue.Sender, decryptor);
                decryptedAccount.Sample = ByteArrayToIntArrayToStr(new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(DecryptAES(requestStartDialogue.SenderPassword, decryptor))));
                decryptedDeviceId.Id = Int16.Parse(DecryptAES(requestStartDialogue.SenderdDeviceId, decryptor));
                decryptedReceiverLogin = DecryptAES(requestStartDialogue.Receiver, decryptor);
                decryptedPublicKey.ModulusStr = DecryptAES(requestStartDialogue.PublicKeyModulus, decryptor);
                decryptedPublicKey.ExponentStr = DecryptAES(requestStartDialogue.PublicKeyExponent, decryptor);
            }
            return (decryptedAccount, decryptedDeviceId, decryptedReceiverLogin, decryptedPublicKey);
        } 
        public (Account, DeviceID, RSAPublicKey) Decrypt(RequestAcceptDialogue requestAccepttDialogue, Session session)
        {
            Account decryptedAccount = new();
            DeviceID decryptedDeviceId = new();
            RSAPublicKey decryptedPublicKey = new();
            using (Aes aes = Aes.Create())
            {
                aes.Key = StrToIntArrayToByteArray(session.SymmetricKey);
                aes.IV = StrToIntArrayToByteArray(session.InitVector);
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                decryptedAccount.Login = DecryptAES(requestAccepttDialogue.Accepted, decryptor);
                decryptedAccount.Sample = ByteArrayToIntArrayToStr(new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(DecryptAES(requestAccepttDialogue.AcceptedPassword, decryptor))));
                decryptedDeviceId.Id = Int16.Parse(DecryptAES(requestAccepttDialogue.AcceptedDeviceId, decryptor));
                decryptedPublicKey.ModulusStr = DecryptAES(requestAccepttDialogue.PublicKeyModulus, decryptor);
                decryptedPublicKey.ExponentStr = DecryptAES(requestAccepttDialogue.PublicKeyExponent, decryptor);
            }
            return (decryptedAccount, decryptedDeviceId, decryptedPublicKey);
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
