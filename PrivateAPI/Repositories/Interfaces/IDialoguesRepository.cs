﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PrivateAPI.Entities;
using PrivateAPI.Models;
namespace PrivateAPI.Repositories.Interfaces
{
    public interface IDialoguesRepository
    {
        Task<ActionResult<DialogueRequest>> CreateRequestStartDialogue(RequestStartDialogue requestStartDialogue, int sessionId);
        Task<ActionResult<IEnumerable<DialogueRequest>>> RetunAllOutcomingDialogues(AuthorizationData authorizationData, int sessionId);
        Task<StatusCodeResult> DeleteOutDialogue(AuthorizationData authorizationData, int dialogueId, int sessionId);
        Task<ActionResult<IEnumerable<DialogueRequest>>> RetunAllIncomingDialogues(AuthorizationData authorizationData, int sessionId);
        Task<StatusCodeResult> AcceptInDialogue(RequestAcceptDialogue requestAcceptDialogue, int dialogueId, int sessionId);
        Task<ActionResult<IEnumerable<StartedDialogue>>> RetunAllStartedDialogues(AuthorizationData authorizationData, int sessionId);
        Task<ActionResult<IEnumerable<CustomMessage>>> RetunAllDialogueMessages(AuthorizationData authorizationData, int dialogueId, int sessionId);
        Task<StatusCodeResult> CreateDialogueMessages(NewMessage newMessage, int dialogueId, int sessionId);
    }
}
