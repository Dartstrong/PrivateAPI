using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PrivateAPI.Repositories.Interfaces;
using PrivateAPI.Models;
using PrivateAPI.Entity;
using PrivateAPI.HelperClasses;
namespace PrivateAPI.Repositories.Implementations
{
    public class SessionsRepository : Converter, ISessionsRepository
    {
        private readonly Context _context;

        public SessionsRepository(Context context)
        {
            _context = context;
        }

        public async Task<ActionResult<Session>> CreateNewSession(RSAPublicKey rsaPublicKey)
        {           
            Aes aes = Aes.Create();
            aes.GenerateIV();
            aes.GenerateKey();
            Session session = new(); 
            session.SymmetricKey = ByteArrayToIntArrayToStr(aes.Key);
            session.InitVector = ByteArrayToIntArrayToStr(aes.IV);
            session.LastUsed = DateTime.Now;
            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();
            RSAParameters rsaParameters = new();
            rsaParameters.Modulus = StrToIntArrayToByteArray(rsaPublicKey.ModulusStr);
            rsaParameters.Exponent = StrToIntArrayToByteArray(rsaPublicKey.ExponentStr);
            Crypter crypter = new();
            session.SymmetricKey = crypter.Encrypt(aes.Key, rsaParameters);
            session.InitVector = crypter.Encrypt(aes.IV, rsaParameters);      
            return session;
        }
    }
}
