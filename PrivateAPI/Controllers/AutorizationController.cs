using System.Threading.Tasks;
using PrivateAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PrivateAPI.Entity;
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
        [HttpPost("newuser/{sessionID}")]
        public async Task<ActionResult<NewDeviceID>> StartSession([FromBody] AuthorizationData authorizationData, int sessionID)
        {
            return await _autorizationsRepository.CreateAccount(authorizationData, sessionID);
        }
    }
}
