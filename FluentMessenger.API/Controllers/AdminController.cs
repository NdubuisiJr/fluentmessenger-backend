using Microsoft.AspNetCore.Mvc;

namespace FluentMessenger.API.Controllers {
    [Route("api/admin")]
    public class AdminController : Controller {
        [HttpGet]
        public IActionResult Index() {
            return View();
        }
    }
}
