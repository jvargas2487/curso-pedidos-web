using PedidosWeb.Core.Lineas;

namespace PedidosWeb.Core.Detalles;

/// <summary>
/// Calcula los montos de un pedido a partir de sus líneas, en CLP y sin IVA (HU-02).
/// </summary>
public static class CalculadoraTotalPedido
{
    /// <summary>
    /// Monto de una línea: cantidad por precio unitario.
    /// </summary>
    public static decimal CalcularSubtotal(LineaPedido linea) => linea.Cantidad * linea.PrecioUnitario;

    /// <summary>
    /// Total del pedido: la suma de los subtotales de sus líneas. Cero si no tiene líneas.
    /// </summary>
    public static decimal CalcularTotal(IEnumerable<LineaPedido> lineas) => lineas.Sum(CalcularSubtotal);
}
