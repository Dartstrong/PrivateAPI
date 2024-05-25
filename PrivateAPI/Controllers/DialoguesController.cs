﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrivateAPI.Repositories.Interfaces;
using PrivateAPI.Entities;
namespace PrivateAPI.Controllers
{
    [Route("api/dialogues")]
    public class DialoguesController : Controller
    {
        private readonly IDialoguesRepository _dialoguesRepository;
        public DialoguesController(IDialoguesRepository dialoguesRepository)
        {
            _dialoguesRepository = dialoguesRepository;
        }
        [HttpPost("startdialogue/{sessionId}")]
        public async Task<ActionResult<DialogueRequest>>StartNewDialogue([FromBody] RequestStartDialogue requestStartDialogue, int sessionId)
        {
            return await _dialoguesRepository.CreateRequestStartDialogue(requestStartDialogue, sessionId);
        }
        /*[HttpPost("getoutcomingdialogues/{sessionId}")]
        public async Task<ActionResult<IEnumerable<DialogueRequest>>>GetOutDialogues([FromBody] AuthorizationData authorizationData, int sessionId)
        {
            return await _dialoguesRepository.RetunAllOutcomingDialogues(authorizationData, sessionId);
        }
        [HttpPost("deleteoutdialogue/{dialogueId}/{sessionId}")]
        public async Task<ActionResult<StatusCodeResult>> DeleteOutcomingDialogue([FromBody] AuthorizationData authorizationData, int dialogueId, int sessionId)
        {
            return await _dialoguesRepository.DeleteOutDialogue(authorizationData, dialogueId, sessionId);
        }
        [HttpPost("getincomingdialogues/{sessionId}")]
        public async Task<ActionResult<IEnumerable<DialogueRequest>>> GetIncomingDialogues([FromBody] AuthorizationData authorizationData, int sessionId)
        {
            return await _dialoguesRepository.RetunAllIncomingDialogues(authorizationData, sessionId);
        }*/

    }
}