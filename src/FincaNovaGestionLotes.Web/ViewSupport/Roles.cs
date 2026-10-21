namespace FincaNovaGestionLotes.Web.ViewSupport;

public static class Roles
{
    public const string Administrador = "Administrador";
    public const string Duenio = "Dueño";
    public const string Trabajador = "Trabajador";

    public static bool EsAdmin(string? rol) =>
        string.Equals(rol, Administrador, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(rol, Duenio, StringComparison.OrdinalIgnoreCase);

    public static bool EsAdmin(HttpContext context) =>
        EsAdmin(context.Session.GetString("UsuarioRol"));
}
