using PrivateAPI.HelperClasses;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PrivateAPI.Models;
using PrivateAPI.Repositories.Interfaces;
using PrivateAPI.Entities;
namespace PrivateAPI.Repositories.Implementations
{
    public class DialoguesRepository : IDialoguesRepository
    {
        private readonly Context _context;
        public DialoguesRepository(Context context)
        {
            _context = context;
        }
        public async  Task<StatusCodeResult> CreateRequestStartDialogue(RequestStartDialogue requestStartDialogue, int sessionId)
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
    }
}
