using Microsoft.AspNetCore.Mvc;

namespace YouthChat.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}