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
        public async Task<ActionResult<NewDeviceID>> CreateAccount(AuthorizationData authorizationData, int sessionId)
        {
            var session = await _context.Sessions.FindAsync(sessionId);
            Crypter crypter = new();
            (Account, DeviceID) userInfo = crypter.Decrypt(authorizationData, session);
            if (_context.Accounts.FirstOrDefault(account => account.Login == userInfo.Item1.Login) == null)
            {
                _context.Accounts.Add(userInfo.Item1);
                await _context.SaveChangesAsync();
                userInfo.Item2.AccountID = _context.Accounts.First(account => account.Login == userInfo.Item1.Login).Id;
                _context.DeviceID.Add(userInfo.Item2);
                await _context.SaveChangesAsync();
                NewDeviceID newDeviceID = new();
                newDeviceID.DeviceID = crypter.Encrypt(BitConverter.GetBytes(_context.DeviceID.First(deviceID => deviceID.AccountID == userInfo.Item2.AccountID).Id), session);
                return newDeviceID;
            }
            else return new BadRequestResult();

        }
        public async Task<ActionResult<StatusCodeResult>> Autorization(AuthorizationData authorizationData, int sessionId)
        {
            var session = await _context.Sessions.FindAsync(sessionId);
            Crypter crypter = new();
            (Account, DeviceID) userInfo = crypter.Decrypt(authorizationData, session);
            var selectedAccount = _context.Accounts.First(account => account.Login == userInfo.Item1.Login);
            if (selectedAccount == null)
            {
                return new StatusCodeResult(401);
            }
            else if(selectedAccount.Sample != userInfo.Item1.Sample)
            {
                return new StatusCodeResult(403);
            }
            else
            {
                var selectedDevice = await _context.DeviceID.FindAsync(userInfo.Item2.Id);
                if(selectedDevice.AccountID!=selectedAccount.Id)
                {
                    return new StatusCodeResult(303);
                }
                else
                {
                   return new StatusCodeResult(200);
                }
            }
        }
        public async Task<ActionResult<AuthorizationData>> AutorizationNewDevice(AuthorizationData authorizationData, int sessionId)
        {
            return new StatusCodeResult(401);
        }


    }
}
