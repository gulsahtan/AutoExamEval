using Microsoft.AspNetCore.Mvc;

namespace AutoExamEval.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
