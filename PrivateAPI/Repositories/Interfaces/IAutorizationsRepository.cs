﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PrivateAPI.Entities;
using PrivateAPI.Models;
namespace PrivateAPI.Repositories.Interfaces
{
    public interface IAutorizationsRepository
    {
        Task<ActionResult<NewDeviceID>> CreateDevice(int sessionId);
        Task<ActionResult> CreateAccount(AuthorizationData authorizationData, int sessionId);
        Task<ActionResult> Autorization(AuthorizationData authorizationData, int sessionId);
    }
}