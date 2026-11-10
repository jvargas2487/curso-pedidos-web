using PedidosWeb.Core.Lineas;
using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Detalles;

/// <summary>
/// Arma el detalle de un pedido con su cliente, sus líneas y su total (HU-02).
/// </summary>
public sealed class DetallePedidoServicio
{
    private readonly IPedidoRepositorio _pedidoRepositorio;
    private readonly ILineaPedidoRepositorio _lineaPedidoRepositorio;

    public DetallePedidoServicio(IPedidoRepositorio pedidoRepositorio, ILineaPedidoRepositorio lineaPedidoRepositorio)
    {
        _pedidoRepositorio = pedidoRepositorio;
        _lineaPedidoRepositorio = lineaPedidoRepositorio;
    }

    /// <summary>
    /// Devuelve el detalle del pedido, o null si el pedido no existe.
    /// </summary>
    public DetallePedido? Obtener(int id)
    {
        var pedido = _pedidoRepositorio.ObtenerPorId(id);
        if (pedido is null)
        {
            return null;
        }

        var lineas = _lineaPedidoRepositorio.ObtenerPorPedido(id);

        var lineasDetalle = lineas
            .Select(l => new LineaDetallePedido(
                l.Numero,
                l.Producto,
                l.Cantidad,
                l.PrecioUnitario,
                CalculadoraTotalPedido.CalcularSubtotal(l)))
            .ToList();

        return new DetallePedido(
            pedido.Id,
            pedido.Folio,
            pedido.Fecha,
            pedido.Estado,
            pedido.Sucursal,
            pedido.ClienteId,
            pedido.ClienteNombre,
            lineasDetalle,
            CalculadoraTotalPedido.CalcularTotal(lineas));
    }
}
