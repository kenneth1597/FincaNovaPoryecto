using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FincaNovaGestionLotes.Web.Models;

namespace FincaNovaGestionLotes.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
        => RedirectToAction("Index", "Lotes", new { area = "Lotes" });

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
