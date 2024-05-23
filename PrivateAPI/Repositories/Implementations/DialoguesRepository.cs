using PrivateAPI.HelperClasses;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PrivateAPI.Models;
using PrivateAPI.Repositories.Interfaces;
using PrivateAPI.Entities;
using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PrivateAPI.Repositories.Implementations
{
    public class DialoguesRepository : IDialoguesRepository
    {
        private readonly Context _context;
        public DialoguesRepository(Context context)
        {
            _context = context;
        }
        public async Task<ActionResult<DialogueRequest>> CreateRequestStartDialogue(RequestStartDialogue requestStartDialogue, int sessionId)
        {
            var session = await _context.Sessions.FindAsync(sessionId);
            Crypter crypter = new();
            (Account senderAccount, DeviceID senderDeviceId, string receiverLogin, RSAPublicKey publicKey) requestInfo = crypter.Decrypt(requestStartDialogue, session);
            var selectedAccount = _context.Accounts.FirstOrDefault(account => account.Login == requestInfo.senderAccount.Login);
            try
            {
                if (selectedAccount == null)
                {
                    return new StatusCodeResult(400);
                }
                else if (selectedAccount.Sample != requestInfo.senderAccount.Sample)
                {
                    return new StatusCodeResult(400);
                }
                else
                {
                    var sender = _context.LoginHistories.FirstOrDefault(history => history.AccountId == selectedAccount.Id && history.DeviceId == requestInfo.senderDeviceId.Id);
                    var receiver = _context.Accounts.FirstOrDefault(account => account.Login == requestInfo.receiverLogin);
                    if ((receiver != null) && (sender != null))
                    {
                        NewDialogue newDialogue = new()
                        {
                            Sender = sender.Id,
                            Receiver = receiver.Id,
                            PublicKeyModulus = requestInfo.publicKey.ModulusStr,
                            PublicKeyExponent = requestInfo.publicKey.ExponentStr,
                            CreationTime = DateTime.Now
                        };
                        _context.NewDialogues.Add(newDialogue);
                        await _context.SaveChangesAsync();
                        return new DialogueRequest
                        {
                            IdStr = crypter.Encrypt(newDialogue.Id.ToString(), session),
                            Sender = crypter.Encrypt(requestInfo.senderAccount.Login, session),
                            Receiver = crypter.Encrypt(requestInfo.receiverLogin,session)
                        };
                    }
                    else
                    {
                        return new StatusCodeResult(401);
                    }
                }
            }
            catch
            {
                return new StatusCodeResult(400);
            }
        }
        public async Task<ActionResult<IEnumerable<DialogueRequest?>>> RetunAllOutDialogues(AuthorizationData authorizationData, int sessionId)
        {
            var session = await _context.Sessions.FindAsync(sessionId);
            Crypter crypter = new();
            (Account account, DeviceID deviceId) userInfo = crypter.Decrypt(authorizationData, session);
            var selectedAccount = _context.Accounts.FirstOrDefault(account => account.Login == userInfo.account.Login);
            try
            {
                if (selectedAccount == null)
                {
                    return new StatusCodeResult(400);
                }
                else if (selectedAccount.Sample != userInfo.account.Sample)
                {
                    return new StatusCodeResult(400);
                }
                else
                {
                    var selectedHistory = _context.LoginHistories.FirstOrDefault(history => history.AccountId == selectedAccount.Id && history.DeviceId == userInfo.deviceId.Id);
                    if (selectedHistory==null)
                    {
                        return new StatusCodeResult(400);
                    }
                    else
                    {
                        var selectedDialogues = await _context.NewDialogues.Where(dialogue => dialogue.Sender == selectedHistory.Id).ToListAsync();
                        List<DialogueRequest> outRequests = new();
                        foreach(var dialogue in selectedDialogues)
                        {

                            outRequests.Add(new DialogueRequest
                            {
                                IdStr = crypter.Encrypt(dialogue.Id.ToString(), session),
                                Sender = crypter.Encrypt(userInfo.account.Login, session),
                                Receiver = crypter.Encrypt(_context.Accounts.FirstOrDefault(account => account.Id == dialogue.Receiver).Login, session)
                            });
                        }
                        return outRequests;
                    }
                }
            }
            catch
            {
                return new StatusCodeResult(400);
            }
        }
        public async Task<ActionResult<StatusCodeResult>> DeleteOutDialogue(AuthorizationData authorizationData, int dialogueId, int sessionId)
        {
            var session = await _context.Sessions.FindAsync(sessionId);
            Crypter crypter = new();
            (Account account, DeviceID deviceId) userInfo = crypter.Decrypt(authorizationData, session);
            var selectedAccount = _context.Accounts.FirstOrDefault(account => account.Login == userInfo.account.Login);
            try
            {
                if (selectedAccount == null)
                {
                    return new StatusCodeResult(400);
                }
                else if (selectedAccount.Sample != userInfo.account.Sample)
                {
                    return new StatusCodeResult(400);
                }
                else
                {
                    var selectedHistory = _context.LoginHistories.FirstOrDefault(history => history.AccountId == selectedAccount.Id && history.DeviceId == userInfo.deviceId.Id);
                    if (selectedHistory == null)
                    {
                        return new StatusCodeResult(400);
                    }
                    else
                    {
                        var selectedDialogue = await _context.NewDialogues.FindAsync(dialogueId);
                        if (selectedDialogue.Sender == selectedHistory.Id)
                        {
                            _context.NewDialogues.Remove(selectedDialogue);
                            await _context.SaveChangesAsync();
                            return new StatusCodeResult(200);
                        }
                        else return new StatusCodeResult(400);
                    }
                }
            }
            catch
            {
                return new StatusCodeResult(400);
            }
        }
    }
}
