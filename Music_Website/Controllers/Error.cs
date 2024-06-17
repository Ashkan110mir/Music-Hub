using Microsoft.AspNetCore.Mvc;

namespace Music_Website.Controllers
{
    public class Error : Controller
    {
        public IActionResult Not_found()
        {
            return View("Views/Not_found.cshtml");
        }
    }
}
