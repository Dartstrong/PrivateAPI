﻿using System.Threading.Tasks;
using PrivateAPI.Models;
using PrivateAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PrivateAPI.Entities;

namespace PrivateAPI.Controllers
{
    [Route("api/session")]
    public class SessionsController : Controller
    {
        private readonly ISessionsRepository _sessionsRepository;
        public SessionsController(ISessionsRepository sessionsRepository)
        {
            _sessionsRepository = sessionsRepository;
        }
        [HttpPost]
        public async Task<ActionResult<Session>> StartSession([FromBody] RSAPublicKey rsaPublicKey)
        {
            return await _sessionsRepository.CreateNewSession(rsaPublicKey);
        }
    }
}
