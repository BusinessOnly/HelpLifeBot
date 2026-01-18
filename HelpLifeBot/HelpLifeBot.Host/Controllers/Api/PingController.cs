using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpLifeBot.Host.Controllers.Api
{
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PingController : ControllerBase
    {
        /// <response code="200"/>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetPing()
        {
            return Ok();
        }
    }
}
