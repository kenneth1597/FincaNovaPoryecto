using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;

namespace FincaNovaGestionLotes.Web.Domain;

/// <summary>Configuración de la finca; una fila por empresa.</summary>
public class ParametrosFinca : AuditableEntity
{
    public int EmpresaId { get; set; }
    public Empresa Empresa { get; set; } = null!;

    /// <summary>Regla de conversión 1 cajuela = X kg (HU-13). 0 = sin configurar.</summary>
    [Range(0, 1000)]
    public decimal KgPorCajuela { get; set; }

    /// <summary>Tamaño máximo de adjuntos en MB (HU-52).</summary>
    [Range(1, 100)]
    public int TamanoMaximoAdjuntoMB { get; set; } = 5;

    /// <summary>Extensiones permitidas para adjuntos, separadas por coma (HU-52).</summary>
    [Required, StringLength(100)]
    public string ExtensionesAdjuntoPermitidas { get; set; } = "pdf,jpg,jpeg,png";
}
