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
        public string Encrypt(byte[] data, Session session)
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
                using (MemoryStream msDecrypt = new(authorizationData.Login))
                {
                    using (CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new(csDecrypt))
                        {
                            decryptedAccount.Login = srDecrypt.ReadToEnd();
                        }
                    }
                }
                using (MemoryStream msDecrypt = new(authorizationData.Password))
                {
                    using (CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var msPlain = new MemoryStream())
                        {
                            csDecrypt.CopyTo(msPlain);
                            byte[] tmpByteArray = new byte[msPlain.ToArray().Length + Encoding.UTF8.GetBytes(decryptedAccount.Login).Length];
                            Array.Copy(msPlain.ToArray(), 0, tmpByteArray, 0, msPlain.ToArray().Length);
                            Array.Copy(Encoding.UTF8.GetBytes(decryptedAccount.Login), 0, tmpByteArray, msPlain.ToArray().Length, Encoding.UTF8.GetBytes(decryptedAccount.Login).Length);
                            decryptedAccount.Sample = ByteArrayToIntArrayToStr(new MD5CryptoServiceProvider().ComputeHash(tmpByteArray));
                        }
                    }
                }
                if(authorizationData.Email!=null)
                {
                    using (MemoryStream msDecrypt = new(authorizationData.Email))
                    {
                        using (CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new(csDecrypt))
                            {
                                decryptedAccount.Email= srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                if(authorizationData.DeviceId!=null)
                {
                    using (MemoryStream msDecrypt = new(authorizationData.DeviceId))
                    {
                        using (CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var msPlain = new MemoryStream())
                            {
                                csDecrypt.CopyTo(msPlain);
                                decryptedDeviceID.Id = BitConverter.ToInt32(msPlain.ToArray(),0);
                            }
                        }
                    }
                }
            }
            return (decryptedAccount, decryptedDeviceID);
        }
    }
}
