using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PrivateAPI.Repositories.Interfaces;
using PrivateAPI.Models;
using PrivateAPI.Entities;
using PrivateAPI.HelperClasses;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
        public async Task<StatusCodeResult> CreateAccount(AuthorizationData authorizationData, int sessionId)
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
                    if (!_context.LoginHistories.Any(history => history.AccountId == userInfo.Item1.Id && history.DeviceId == userInfo.Item2.Id))
                    {
                        LoginHistory loginHistory = new()
                        {
                            AccountId = userInfo.Item1.Id,
                            DeviceId = userInfo.Item2.Id
                        };
                        _context.LoginHistories.Add(loginHistory);
                        await _context.SaveChangesAsync();
                    }
                    return new StatusCodeResult(200);
                }
                else return new StatusCodeResult(409);
            }
            catch
            {
                return new StatusCodeResult(400);
            }
        }
        public async Task<StatusCodeResult> Autorization(AuthorizationData authorizationData, int sessionId)
        {
            var session = await _context.Sessions.FindAsync(sessionId);
            Crypter crypter = new();
            (Account, DeviceID) userInfo = crypter.Decrypt(authorizationData, session);
            var selectedAccount = _context.Accounts.FirstOrDefault(account => account.Login == userInfo.Item1.Login);
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
                    if(!_context.LoginHistories.Any(history => history.AccountId == selectedAccount.Id && history.DeviceId == userInfo.Item2.Id))
                    {
                        LoginHistory loginHistory = new()
                        {
                            AccountId = selectedAccount.Id,
                            DeviceId = userInfo.Item2.Id
                        };
                        _context.LoginHistories.Add(loginHistory);
                        await _context.SaveChangesAsync();
                    }
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
