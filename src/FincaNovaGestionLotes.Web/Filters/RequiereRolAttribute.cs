using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FincaNovaGestionLotes.Web.Filters;

public class RequiereRolAttribute : ActionFilterAttribute
{
    private readonly string[] _roles;

    public RequiereRolAttribute(params string[] roles)
    {
        _roles = roles;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var rol = context.HttpContext.Session.GetString("UsuarioRol");

        if (string.IsNullOrEmpty(rol))
        {
            context.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
            return;
        }

        if (_roles.Length > 0 && !_roles.Contains(rol, StringComparer.OrdinalIgnoreCase))
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Account", new { area = "" });
        }
    }
}
