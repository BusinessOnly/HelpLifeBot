using Microsoft.AspNetCore.Mvc;

namespace HelpLifeBot.Host.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
