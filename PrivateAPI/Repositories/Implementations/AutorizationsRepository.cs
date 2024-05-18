using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PrivateAPI.Repositories.Interfaces;
using PrivateAPI.Models;
using PrivateAPI.Entities;
using PrivateAPI.HelperClasses;
using System.Linq;
namespace PrivateAPI.Repositories.Implementations
{
    public class AutorizationsRepository : Converter, IAutorizationsRepository
    {
        private readonly Context _context;
        public AutorizationsRepository(Context context)
        {
            _context = context;
        }
        public async Task<ActionResult<NewDeviceID>> CreateDevice(int sessionId)
        {
            var session = await _context.Sessions.FindAsync(sessionId);
            Crypter crypter = new();
            var deviceId = new DeviceID();
            _context.DeviceID.Add(deviceId);
            await _context.SaveChangesAsync();
            var newDeviceId = new NewDeviceID();
            newDeviceId.DeviceIdStr = crypter.Encrypt(deviceId.Id.ToString(), session);
            return newDeviceId;
        }
        public async Task<ActionResult> CreateAccount(AuthorizationData authorizationData, int sessionId)
        {
            var session = await _context.Sessions.FindAsync(sessionId);
            Crypter crypter = new();
            (Account, DeviceID) userInfo = crypter.Decrypt(authorizationData, session);
            try
            {
                if (_context.Accounts.FirstOrDefault(account => account.Login == userInfo.Item1.Login) == null)
                {
                    _context.Accounts.Add(userInfo.Item1);
                    await _context.SaveChangesAsync();     
                    return new StatusCodeResult(200);
                }
                else return new StatusCodeResult(409);
            }
            catch
            {
                return new StatusCodeResult(400);
            }
        }
        public async Task<ActionResult> Autorization(AuthorizationData authorizationData, int sessionId)
        {
            var session = await _context.Sessions.FindAsync(sessionId);
            Crypter crypter = new();
            (Account, DeviceID) userInfo = crypter.Decrypt(authorizationData, session);
            var selectedAccount = _context.Accounts.First(account => account.Login == userInfo.Item1.Login);
            try 
            {             
                if (selectedAccount == null)
                {
                    return new StatusCodeResult(401);
                }
                else if (selectedAccount.Sample != userInfo.Item1.Sample)
                {
                    return new StatusCodeResult(403);
                }
                else
                {
                    return new StatusCodeResult(200);
                }
            }
            catch
            {
                return new StatusCodeResult(400);
            }

        }


    }
}
