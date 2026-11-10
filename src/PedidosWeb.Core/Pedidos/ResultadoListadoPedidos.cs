namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Resultado del listado de pedidos: la página pedida o los errores agrupados por nombre de parámetro.
/// </summary>
public sealed class ResultadoListadoPedidos
{
    private ResultadoListadoPedidos(PaginaPedidos? pagina, IReadOnlyDictionary<string, string[]> errores)
    {
        Pagina = pagina;
        Errores = errores;
    }

    public PaginaPedidos? Pagina { get; }

    public IReadOnlyDictionary<string, string[]> Errores { get; }

    public bool EsValido => Pagina is not null;

    public static ResultadoListadoPedidos Exito(PaginaPedidos pagina) =>
        new(pagina, new Dictionary<string, string[]>());

    public static ResultadoListadoPedidos ConErrores(IReadOnlyDictionary<string, string[]> errores) =>
        new(null, errores);
}
