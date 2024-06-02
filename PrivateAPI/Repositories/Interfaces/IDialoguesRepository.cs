using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PrivateAPI.Entities;
namespace PrivateAPI.Repositories.Interfaces
{
    public interface IDialoguesRepository
    {
        Task<ActionResult<DialogueRequest>> CreateRequestStartDialogue(RequestStartDialogue requestStartDialogue, int sessionId);
        Task<ActionResult<IEnumerable<DialogueRequest>>> RetunAllOutcomingDialogues(AuthorizationData authorizationData, int sessionId);
        
        Task<StatusCodeResult> DeleteOutDialogue(AuthorizationData authorizationData, int dialogueId, int sessionId);
        Task<ActionResult<IEnumerable<DialogueRequest>>> RetunAllIncomingDialogues(AuthorizationData authorizationData, int sessionId);
        Task<StatusCodeResult> AcceptInDialogue(RequestAcceptDialogue requestAcceptDialogue, int dialogueId, int sessionId);
    }
}
