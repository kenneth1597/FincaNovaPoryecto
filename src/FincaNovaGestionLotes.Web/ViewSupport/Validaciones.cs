namespace FincaNovaGestionLotes.Web.ViewSupport;

public static class Validaciones
{
    public const string Codigo = @"^[A-Za-z0-9\-_]{1,30}$";
    public const string CodigoMsg = "El código solo admite letras, números, guion y guion bajo (sin espacios).";

    /// <summary>Letras (con acentos/ñ), números, espacios y . ' - . Ej. "Villa Sarchí", "V14".</summary>
    public const string Texto = @"^[A-Za-zÀ-ÿ0-9][A-Za-zÀ-ÿ0-9 .'-]*$";
    public const string TextoMsg = "Use solo letras, números, espacios y . ' - (debe empezar con letra o número).";
}
