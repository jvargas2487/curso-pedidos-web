namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Filtros sobre listas de pedidos.
/// </summary>
public static class FiltroPedidos
{
    /// <summary>
    /// Devuelve los pedidos emitidos entre dos fechas, ambas inclusive.
    /// </summary>
    /// <exception cref="ArgumentException">Cuando <paramref name="desde"/> es posterior a <paramref name="hasta"/>.</exception>
    public static IReadOnlyList<Pedido> EntreFechas(IEnumerable<Pedido> pedidos, DateOnly desde, DateOnly hasta)
    {
        ArgumentNullException.ThrowIfNull(pedidos);

        if (desde > hasta)
        {
            throw new ArgumentException(
                $"La fecha desde ({desde:yyyy-MM-dd}) no puede ser posterior a la fecha hasta ({hasta:yyyy-MM-dd}).",
                nameof(desde));
        }

        return pedidos.Where(p => p.Fecha >= desde && p.Fecha <= hasta).ToList();
    }
}
