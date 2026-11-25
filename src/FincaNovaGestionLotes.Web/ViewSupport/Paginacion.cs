using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.ViewSupport;

public class PaginaInfo
{
    public int Pagina { get; init; } = 1;
    public int TamanoPagina { get; init; } = Paginacion.TamanoPorDefecto;
    public int Total { get; init; }

    public int TotalPaginas => Total == 0 ? 1 : (int)Math.Ceiling(Total / (double)TamanoPagina);
    public int Desde => Total == 0 ? 0 : (Pagina - 1) * TamanoPagina + 1;
    public int Hasta => Math.Min(Pagina * TamanoPagina, Total);
    public bool TieneAnterior => Pagina > 1;
    public bool TieneSiguiente => Pagina < TotalPaginas;
}

public static class Paginacion
{
    public const int TamanoPorDefecto = 10;

    public static async Task<(List<T> Items, PaginaInfo Info)> PaginarAsync<T>(
        this IQueryable<T> query, int pagina, int tamano = TamanoPorDefecto)
    {
        var total = await query.CountAsync();
        var info = CrearInfo(pagina, tamano, total);
        var items = await query
            .Skip((info.Pagina - 1) * info.TamanoPagina)
            .Take(info.TamanoPagina)
            .ToListAsync();
        return (items, info);
    }

    public static (List<T> Items, PaginaInfo Info) Paginar<T>(
        this IEnumerable<T> origen, int pagina, int tamano = TamanoPorDefecto)
    {
        var lista = origen as IList<T> ?? origen.ToList();
        var info = CrearInfo(pagina, tamano, lista.Count);
        var items = lista
            .Skip((info.Pagina - 1) * info.TamanoPagina)
            .Take(info.TamanoPagina)
            .ToList();
        return (items, info);
    }

    private static PaginaInfo CrearInfo(int pagina, int tamano, int total)
    {
        tamano = tamano < 1 ? TamanoPorDefecto : tamano;
        var totalPaginas = total == 0 ? 1 : (int)Math.Ceiling(total / (double)tamano);
        pagina = Math.Clamp(pagina, 1, totalPaginas);
        return new PaginaInfo { Pagina = pagina, TamanoPagina = tamano, Total = total };
    }
}
