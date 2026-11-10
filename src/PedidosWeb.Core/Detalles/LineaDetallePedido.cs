namespace PedidosWeb.Core.Detalles;

/// <summary>
/// Una línea en el detalle de un pedido, con su subtotal ya calculado.
/// </summary>
/// <param name="Numero">Número de la línea dentro del pedido, desde 1.</param>
/// <param name="Producto">Descripción del producto.</param>
/// <param name="Cantidad">Unidades pedidas.</param>
/// <param name="PrecioUnitario">Precio de una unidad en CLP, sin IVA.</param>
/// <param name="Subtotal">Cantidad por precio unitario, en CLP, sin IVA.</param>
public sealed record LineaDetallePedido(
    int Numero,
    string Producto,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal);
