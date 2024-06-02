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
                        Dialogue newDialogue = new()
                        {
                            Started = sender.AccountId,
                            StartedDeviceId = sender.DeviceId,
                            StartedModulusStr = requestInfo.publicKey.ModulusStr,
                            StatedExponentStr = requestInfo.publicKey.ExponentStr,
                            Accepted = receiver.Id,
                            AcceptedDeviceId = null,
                            AcceptedModulusStr = null,
                            AcceptedExponentStr = null,
                            LastMessage = DateTime.Now
                        };
                        _context.Dialogues.Add(newDialogue);
                        await _context.SaveChangesAsync();
                        return new DialogueRequest
                        {
                            IdStr = crypter.Encrypt(newDialogue.Id.ToString(), session),
                            Sender = crypter.Encrypt(requestInfo.senderAccount.Login, session),
                            Receiver = crypter.Encrypt(requestInfo.receiverLogin, session)
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
        public async Task<ActionResult<IEnumerable<DialogueRequest?>>> RetunAllOutcomingDialogues(AuthorizationData authorizationData, int sessionId)
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
                    var selectedDialogues = await _context.Dialogues.Where(dialogue => dialogue.Started == selectedAccount.Id &&
                                                                            dialogue.StartedDeviceId == userInfo.deviceId.Id
                                                                            && dialogue.AcceptedDeviceId == null).ToListAsync();
                    List<DialogueRequest> outRequests = new();
                    foreach (var dialogue in selectedDialogues)
                    {
                        outRequests.Add(new DialogueRequest
                        {
                            IdStr = crypter.Encrypt(dialogue.Id.ToString(), session),
                            Sender = crypter.Encrypt(userInfo.account.Login, session),
                            Receiver = crypter.Encrypt(_context.Accounts.FirstOrDefault(account => account.Id == dialogue.Accepted).Login, session)
                        });
                    }
                    return outRequests;
                }
            }
            catch
            {
                return new StatusCodeResult(400);
            }
        }
        public async Task<StatusCodeResult> DeleteOutDialogue(AuthorizationData authorizationData, int dialogueId, int sessionId)
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
                    var selectedDialogue = await _context.Dialogues.FindAsync(dialogueId);
                    if (selectedDialogue == null)
                    {
                        return new StatusCodeResult(400);
                    }
                    else
                    {
                        if (selectedDialogue.Started == selectedAccount.Id)
                        {
                            _context.Dialogues.Remove(selectedDialogue);
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
        public async Task<ActionResult<IEnumerable<DialogueRequest?>>> RetunAllIncomingDialogues(AuthorizationData authorizationData, int sessionId)
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
                    var selectedDialogues = await _context.Dialogues.Where(dialogue => dialogue.Accepted == selectedAccount.Id
                                                        && dialogue.AcceptedDeviceId == null).ToListAsync();
                    List<DialogueRequest> outRequests = new();
                    foreach (var dialogue in selectedDialogues)
                    {
                        outRequests.Add(new DialogueRequest
                        {
                            IdStr = crypter.Encrypt(dialogue.Id.ToString(), session),
                            Sender = crypter.Encrypt(_context.Accounts.FirstOrDefault(account => account.Id == dialogue.Started).Login, session),
                            Receiver = crypter.Encrypt(userInfo.account.Login, session)
                        });
                    }
                    return outRequests;
                }
            }
            catch
            {
                return new StatusCodeResult(400);
            }
        }

        public async Task<StatusCodeResult> AcceptInDialogue(RequestAcceptDialogue requestAcceptDialogue, int dialogueId, int sessionId)
        {
            var session = await _context.Sessions.FindAsync(sessionId);
            Crypter crypter = new();
            (Account acceptedAccount, DeviceID acceptedDeviceId, RSAPublicKey publicKey) requestInfo = crypter.Decrypt(requestAcceptDialogue, session);
            var selectedAccount = _context.Accounts.FirstOrDefault(account => account.Login == requestInfo.acceptedAccount.Login);
            try
            {
                if (selectedAccount == null)
                {
                    return new StatusCodeResult(400);
                }
                else if (selectedAccount.Sample != requestInfo.acceptedAccount.Sample)
                {
                    return new StatusCodeResult(400);
                }
                else
                {
                    var selectedDialogue = await _context.Dialogues.FindAsync(dialogueId);
                    if(selectedDialogue.Accepted != selectedAccount.Id )
                    {
                        return new StatusCodeResult(400);
                    }
                    else
                    {
                        selectedDialogue.Accepted = selectedAccount.Id;
                        selectedDialogue.AcceptedDeviceId = requestInfo.acceptedDeviceId.Id;
                        selectedDialogue.AcceptedModulusStr = requestInfo.publicKey.ModulusStr;
                        selectedDialogue.AcceptedExponentStr = requestInfo.publicKey.ExponentStr;
                        selectedDialogue.LastMessage = DateTime.Now;
                        _context.Dialogues.Update(selectedDialogue);
                        await _context.SaveChangesAsync();
                        return new StatusCodeResult(200);
                    }
                }
            }
            catch
            {
                return new StatusCodeResult(400);
            }
        }
        public async Task<ActionResult<IEnumerable<StartedDialogue?>>> RetunAllStartedDialogues(AuthorizationData authorizationData, int sessionId)
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
                    var selectedLoginHistory = _context.LoginHistories.FirstOrDefault(history => history.AccountId == selectedAccount.Id && history.DeviceId==userInfo.deviceId.Id);
                    var selectedDialogues = await _context.Dialogues.Where(dialogue => (dialogue.Accepted == selectedLoginHistory.Id
                                                        || dialogue.Started == selectedLoginHistory.Id) && dialogue.AcceptedDeviceId!=null).ToListAsync();
                    List<StartedDialogue> outRequests = new();
                    foreach (var dialogue in selectedDialogues)
                    {
                        outRequests.Add(new StartedDialogue
                        {
                            IdStr = crypter.Encrypt(dialogue.Id.ToString(), session),
                            Receiver = (dialogue.Accepted==selectedAccount.Id) ? crypter.Encrypt(_context.Accounts.FirstOrDefault(account => account.Id == dialogue.Started).Login, session) : crypter.Encrypt(_context.Accounts.FirstOrDefault(account => account.Id == dialogue.Accepted).Login, session),
                            PublicKeyModulus = (dialogue.Accepted == selectedAccount.Id) ? crypter.Encrypt(dialogue.StartedModulusStr, session) : crypter.Encrypt(dialogue.AcceptedModulusStr, session),
                            PublicKeyExponent = (dialogue.Accepted == selectedAccount.Id) ? crypter.Encrypt(dialogue.StatedExponentStr, session) : crypter.Encrypt(dialogue.AcceptedExponentStr, session)
                        });
                    }
                    return outRequests;
                }
            }
            catch
            {
                return new StatusCodeResult(400);
            }
        }
    }
}
