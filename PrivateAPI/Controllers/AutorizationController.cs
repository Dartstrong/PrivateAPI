using System.Threading.Tasks;
using PrivateAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PrivateAPI.Entities;
namespace PrivateAPI.Controllers
{
    [Route("api/autorization")]
    public class AutorizationController : Controller
    {
        private readonly IAutorizationsRepository _autorizationsRepository;
        public AutorizationController(IAutorizationsRepository autorizationsRepository)
        {
            _autorizationsRepository = autorizationsRepository;
        }
        [HttpGet("newdevice/{sessionId}")]
        public async Task<ActionResult<NewDeviceID>> CreateDevice(int sessionId)
        {
            return await _autorizationsRepository.CreateDevice(sessionId);
        }
        [HttpPost("newuser/{sessionId}")]
        public async Task<ActionResult> CreateAccount([FromBody] AuthorizationData authorizationData, int sessionId)
        {
            return await _autorizationsRepository.CreateAccount(authorizationData, sessionId);
        }
        [HttpPost("entry/{sessionId}")]
        public async Task<StatusCodeResult> Autorization([FromBody] AuthorizationData authorizationData, int sessionId)
        {
            return await _autorizationsRepository.Autorization(authorizationData, sessionId);
        }
    }
}
