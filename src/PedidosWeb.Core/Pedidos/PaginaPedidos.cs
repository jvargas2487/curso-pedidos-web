namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Una página del listado de pedidos.
/// </summary>
/// <param name="Elementos">Pedidos de la página, ordenados por fecha y luego por id.</param>
/// <param name="Total">Cantidad de pedidos que cumplen todos los filtros.</param>
/// <param name="Pagina">Número de la página devuelta, desde 1.</param>
public sealed record PaginaPedidos(IReadOnlyList<Pedido> Elementos, int Total, int Pagina);
