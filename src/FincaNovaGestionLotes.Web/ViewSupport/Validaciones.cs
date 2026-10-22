namespace FincaNovaGestionLotes.Web.ViewSupport;

public static class Validaciones
{
    public const string Codigo = @"^[A-Za-z0-9\-_]{1,30}$";
    public const string CodigoMsg = "El código solo admite letras, números, guion y guion bajo (sin espacios).";

    /// <summary>Letras (con acentos/ñ), números, espacios y . ' - . Ej. "Villa Sarchí", "V14".</summary>
    public const string Texto = @"^\s*[A-Za-zÀ-ÿ0-9][A-Za-zÀ-ÿ0-9 .'-]*\s*$";
    public const string TextoMsg = "Use solo letras, números, espacios y . ' - (debe empezar con letra o número).";

    public const string NombrePersona = @"^\s*[A-Za-zÀ-ÿ][A-Za-zÀ-ÿ .'-]*\s*$";
    public const string NombrePersonaMsg = "El nombre solo admite letras (con tildes y ñ), espacios y . ' - (sin números ni símbolos).";

    public const string Telefono = @"^(?=(?:\D*\d){8})[0-9\-\+ ]{8,30}$";
    public const string TelefonoMsg = "El teléfono debe tener al menos 8 dígitos y solo admite números, espacios, + y guion.";

    public const string LongitudMsg = "{0} no puede superar {1} caracteres.";

    public static readonly DateTime FechaMinima = new(2000, 1, 1);
}
