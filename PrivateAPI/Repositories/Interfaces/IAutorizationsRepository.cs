using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PrivateAPI.Entities;
using PrivateAPI.Models;
namespace PrivateAPI.Repositories.Interfaces
{
    public interface IAutorizationsRepository
    {
        Task<ActionResult<NewDeviceID>> CreateAccount(AuthorizationData authorizationData, int sessionId);
        Task<ActionResult<StatusCodeResult>> Autorization(AuthorizationData authorizationData, int sessionId);
        Task<ActionResult<AuthorizationData>> AutorizationNewDevice(AuthorizationData authorizationData, int sessionId);
    }
}
