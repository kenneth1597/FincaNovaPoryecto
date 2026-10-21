using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FincaNovaGestionLotes.Web.Filters;

public class RequiereSesionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var esPublica = context.ActionDescriptor.EndpointMetadata
            .OfType<PermitirSinSesionAttribute>()
            .Any();

        if (esPublica)
            return;

        var usuarioId = context.HttpContext.Session.GetInt32("UsuarioId");
        if (usuarioId is null)
            context.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
