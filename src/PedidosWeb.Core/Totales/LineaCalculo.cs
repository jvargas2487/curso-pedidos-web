namespace PedidosWeb.Core.Totales;

/// <summary>
/// Una línea que entra al cálculo de totales.
/// </summary>
/// <param name="Cantidad">Unidades pedidas.</param>
/// <param name="PrecioUnitario">Precio de una unidad en pesos chilenos (CLP), sin IVA.</param>
/// <param name="Categoria">Categoría del producto; las líneas <c>OFE</c> no entran a la base de descuento.</param>
public sealed record LineaCalculo(
    int Cantidad,
    decimal PrecioUnitario,
    string Categoria);
