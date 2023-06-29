using aboutus.Models;
using Microsoft.AspNetCore.Mvc;

namespace aboutus.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Validation()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Validation(Sing_In sing_in)
        {

            return View("yes");
        }
    }
}
