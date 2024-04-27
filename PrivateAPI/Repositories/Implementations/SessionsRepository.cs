﻿using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using PrivateAPI.Repositories.Interfaces;
using PrivateAPI.Models;
using PrivateAPI.Entity;

namespace PrivateAPI.Repositories.Implementations
{
    public class SessionsRepository: ISessionsRepository
    {
        private readonly Context _context;

        public SessionsRepository(Context context)
        {
            _context = context;
        }

        public async Task<ActionResult<Session>> CreateNewSession(RSAPublicKey rsaPublicKey)
        {
            Session session = new();
            Aes aes = Aes.Create();
            aes.GenerateIV();
            aes.GenerateKey();
            session.SymmetricKey = IntArrayToStr(ByteArrayToIntArray(aes.Key));
            session.InitVector = IntArrayToStr(ByteArrayToIntArray(aes.IV));
            session.LastUsed = DateTime.Now;
            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            RSAParameters rsaParameters = new RSAParameters();
            rsaParameters.Modulus = rsaPublicKey.Modulus;
            rsaParameters.Exponent = rsaPublicKey.Exponent;
            session.SymmetricKey = IntArrayToStr(ByteArrayToIntArray(Encrypt(aes.Key, rsaParameters)));
            session.InitVector = IntArrayToStr(ByteArrayToIntArray(Encrypt(aes.IV, rsaParameters)));      
            return session;
        }

       /* public async Task EndSession(string sessionId, EndSessionConfirm endSessionConfirm)
        {
            var selectedSession = await _context.Sessions.FindAsync(sessionId);
            byte[] key = endSessionConfirm.StrToByteArray(selectedSession.SymmetricKey);
            byte[] initVector = endSessionConfirm.StrToByteArray(selectedSession.InitVector);
            if (endSessionConfirm.IsTrue(sessionId, key, initVector))
            {
                _context.Sessions.Remove(selectedSession);
                await _context.SaveChangesAsync();
            }
        }*/

        public byte[] Encrypt(byte[] data, RSAParameters publicKey)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(publicKey);
                byte[] encryptedData = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA1);
                return encryptedData;
            }
        }

        public int[] ByteArrayToIntArray(byte[] data)
        {
            int[] arrayInt = new int[data.Length];
            for (int i = 0; i < arrayInt.Length; i++)
                arrayInt[i] = Convert.ToInt32(data[i]);
            return arrayInt;
        }

        public string IntArrayToStr(int[] data)
        {
            StringBuilder sb = new("");
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i]);
                sb.Append('-');
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}
