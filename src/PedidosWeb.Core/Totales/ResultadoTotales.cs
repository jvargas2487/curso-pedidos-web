namespace PedidosWeb.Core.Totales;

/// <summary>
/// Montos calculados de un pedido, en pesos chilenos (CLP).
/// </summary>
/// <param name="Subtotal">Suma de todas las líneas, sin descuento.</param>
/// <param name="BaseDescuento">Suma de las líneas que entran al descuento (todas menos las <c>OFE</c>).</param>
/// <param name="PorcDescuento">Porcentaje de descuento como fracción (0,08 = 8 %).</param>
/// <param name="Descuento">Descuento en pesos.</param>
/// <param name="Neto">Subtotal menos descuento.</param>
/// <param name="Flete">Flete de la región; cero si el pedido no tiene líneas.</param>
/// <param name="Iva">IVA sobre neto más flete.</param>
/// <param name="Total">Neto más flete más IVA.</param>
public sealed record ResultadoTotales(
    decimal Subtotal,
    decimal BaseDescuento,
    decimal PorcDescuento,
    decimal Descuento,
    decimal Neto,
    decimal Flete,
    decimal Iva,
    decimal Total);
