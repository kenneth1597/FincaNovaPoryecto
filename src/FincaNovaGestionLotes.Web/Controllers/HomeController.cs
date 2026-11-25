using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FincaNovaGestionLotes.Web.Filters;
using FincaNovaGestionLotes.Web.Models;
using FincaNovaGestionLotes.Web.ViewSupport;

namespace FincaNovaGestionLotes.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (Roles.EsAdmin(HttpContext))
            return RedirectToAction("Index", "Terrenos", new { area = "Terrenos" });

        return RedirectToAction("Index", "Labores", new { area = "Labores" });
    }

    [PermitirSinSesion]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
