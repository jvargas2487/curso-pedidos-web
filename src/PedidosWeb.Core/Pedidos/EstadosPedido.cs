namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Estados que puede tener un pedido, tal como los guarda el legado.
/// </summary>
public static class EstadosPedido
{
    public static readonly IReadOnlyList<string> Validos =
        new[] { "Pendiente", "Confirmado", "Despachado", "Entregado", "Anulado" };

    /// <summary>
    /// Indica si el texto es un estado válido, sin distinguir mayúsculas.
    /// </summary>
    public static bool EsValido(string estado) =>
        Validos.Any(e => string.Equals(e, estado, StringComparison.OrdinalIgnoreCase));
}
