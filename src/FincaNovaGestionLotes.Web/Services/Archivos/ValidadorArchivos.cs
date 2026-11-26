namespace FincaNovaGestionLotes.Web.Services.Archivos;

// HU-65: reglas únicas para cualquier archivo adjunto de la aplicación.
// Revisa tamaño, extensión y que el contenido sea realmente de ese tipo (no solo el nombre).
public static class ValidadorArchivos
{
    public const long TamanoMaximoBytes = 5 * 1024 * 1024;

    private static readonly Dictionary<string, string> TiposPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        [".pdf"] = "application/pdf",
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png"
    };

    // Devuelve el mensaje de error, o null si el archivo es válido o no se envió ninguno.
    public static async Task<string?> ValidarAsync(IFormFile? archivo, string nombreCampo = "El archivo")
    {
        // Sin archivo seleccionado el navegador manda un campo vacío: no es un error.
        if (archivo is null || string.IsNullOrEmpty(archivo.FileName)) return null;

        if (archivo.Length == 0)
            return $"{nombreCampo} está vacío.";

        if (archivo.Length > TamanoMaximoBytes)
            return $"{nombreCampo} no puede pesar más de 5 MB.";

        var extension = Path.GetExtension(archivo.FileName);
        if (!TiposPermitidos.ContainsKey(extension))
            return $"{nombreCampo} debe ser un archivo PDF, JPG o PNG.";

        var cabecera = new byte[8];
        await using var flujo = archivo.OpenReadStream();
        var leidos = await flujo.ReadAsync(cabecera.AsMemory(0, cabecera.Length));

        if (!CabeceraCoincide(extension.ToLowerInvariant(), cabecera.AsSpan(0, leidos)))
            return "El contenido del archivo no corresponde a un PDF, JPG o PNG válido.";

        return null;
    }

    // Tipo de contenido que se guarda y se devuelve al descargar. Solo para extensiones ya validadas.
    public static string TipoContenido(string extension) => TiposPermitidos[extension];

    private static bool CabeceraCoincide(string extension, ReadOnlySpan<byte> cabecera) => extension switch
    {
        ".pdf" => cabecera.StartsWith("%PDF"u8),
        ".jpg" or ".jpeg" => cabecera.StartsWith(new byte[] { 0xFF, 0xD8, 0xFF }),
        ".png" => cabecera.StartsWith(new byte[] { 0x89, 0x50, 0x4E, 0x47 }),
        _ => false
    };
}
