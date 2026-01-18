using Microsoft.AspNetCore.Mvc;

namespace HelpLifeBot.Host.Controllers.Api
{
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DbgController : ControllerBase
    {
        public DbgController()
        {
        }

        [HttpPost]
        public async Task<IActionResult> DbgAsync()
        {
            await Task.Yield();
            return Ok();
        }
    }
}
