using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PrivateAPI.Entities;
using PrivateAPI.Models;
using System.Collections.Generic;

namespace PrivateAPI.Repositories.Interfaces
{
    public interface IAutorizationsRepository
    {
        Task<ActionResult<NewDeviceID>> CreateDevice(int sessionId);
        Task<StatusCodeResult> CreateAccount(AuthorizationData authorizationData, int sessionId);
        Task<StatusCodeResult> Autorization(AuthorizationData authorizationData, int sessionId);
        Task<ActionResult<IEnumerable<LoginEntry>>> ReturnAllLoginHistory(AuthorizationData authorizationData, int sessionId);
    }
}
