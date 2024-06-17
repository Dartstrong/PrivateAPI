using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PrivateAPI.Repositories.Interfaces;
using PrivateAPI.Models;
using PrivateAPI.Entities;
using PrivateAPI.HelperClasses;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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
            (Account account, DeviceID deviceId) userInfo = crypter.Decrypt(authorizationData, session);
            try
            {
                if (!_context.Accounts.Any(account => account.Login == userInfo.account.Login))
                {
                    _context.Accounts.Add(userInfo.Item1);
                    await _context.SaveChangesAsync();
                    if (!_context.LoginHistories.Any(history => history.AccountId == userInfo.account.Id && history.DeviceId == userInfo.deviceId.Id))
                    {
                        LoginHistory loginHistory = new()
                        {
                            AccountId = userInfo.account.Id,
                            DeviceId = userInfo.deviceId.Id
                        };
                        _context.LoginHistories.Add(loginHistory);
                        await _context.SaveChangesAsync();
                        await Autorization(authorizationData, sessionId);
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
            (Account account, DeviceID deviceId) userInfo = crypter.Decrypt(authorizationData, session);
            var selectedAccount = _context.Accounts.FirstOrDefault(account => account.Login == userInfo.account.Login);
            try 
            {             
                if (selectedAccount == null)
                {
                    return new StatusCodeResult(401);
                }
                else if (selectedAccount.Sample != userInfo.account.Sample)
                {
                    return new StatusCodeResult(403);
                }
                else
                {
                    if(!_context.LoginHistories.Any(history => history.AccountId == selectedAccount.Id && history.DeviceId == userInfo.deviceId.Id))
                    {
                        LoginHistory loginHistory = new()
                        {
                            AccountId = selectedAccount.Id,
                            DeviceId = userInfo.deviceId.Id
                        };
                        _context.LoginHistories.Add(loginHistory);
                        await _context.SaveChangesAsync();
                        LastEntryDate lastEntry = new()
                        {
                            LoginHistoryId = loginHistory.Id,
                            DateTime = DateTime.Now
                        };
                        _context.LastEntries.Add(lastEntry);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        var loginHistory = _context.LoginHistories.FirstOrDefault(history => history.AccountId == selectedAccount.Id && history.DeviceId == userInfo.deviceId.Id);
                        var lastEntry = _context.LastEntries.FirstOrDefault(record => record.LoginHistoryId == loginHistory.Id);
                        if(lastEntry == null)
                        {
                            LastEntryDate newLastEntry = new()
                            {
                                LoginHistoryId = loginHistory.Id,
                                DateTime = DateTime.Now
                            };
                            _context.LastEntries.Add(newLastEntry);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            lastEntry.DateTime = DateTime.Now;
                            _context.LastEntries.Update(lastEntry);
                            await _context.SaveChangesAsync();
                        }
                    }
                    return new StatusCodeResult(200);
                }
            }
            catch
            {
                return new StatusCodeResult(400);
            }
        }

        public async Task<ActionResult<IEnumerable<LoginEntry>>> ReturnAllLoginHistory(AuthorizationData authorizationData, int sessionId)
        {
            var session = await _context.Sessions.FindAsync(sessionId);
            Crypter crypter = new();
            (Account account, DeviceID deviceId) userInfo = crypter.Decrypt(authorizationData, session);
            var selectedAccount = _context.Accounts.FirstOrDefault(account => account.Login == userInfo.account.Login);
            try
            {
                if (selectedAccount == null)
                {
                    return new StatusCodeResult(401);
                }
                else if (selectedAccount.Sample != userInfo.account.Sample)
                {
                    return new StatusCodeResult(403);
                }
                else
                {
                    var selectedLoginHistory = await _context.LoginHistories.Where(history => history.AccountId == selectedAccount.Id).ToListAsync();
                    List<LoginEntry> loginEntries= new();
                    foreach(var record in selectedLoginHistory)
                    {
                        LoginEntry loginEntry = new()
                        {
                            DeviceIdStr = crypter.Encrypt(record.DeviceId.ToString(), session),
                            Date = _context.LastEntries.FirstOrDefault(entry => entry.LoginHistoryId == record.Id).DateTime
                        };
                        loginEntries.Add(loginEntry);
                    }
                    return loginEntries;
                }
            }
            catch
            {
                return new StatusCodeResult(400);
            }
        }
    }
}
