using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Lista pedidos filtrando por cliente, estado y rango de fechas, ordenados y paginados (HU-01).
/// </summary>
public sealed class ListadoPedidosServicio
{
    public const int TamanoPagina = 20;

    private const string FormatoFecha = "yyyy-MM-dd";
    private const string ParametroEstado = "estado";
    private const string ParametroDesde = "desde";
    private const string ParametroHasta = "hasta";
    private const string ParametroPagina = "pagina";

    private const string MensajeEstadoInvalido =
        "El estado debe ser Pendiente, Confirmado, Despachado, Entregado o Anulado.";
    private const string MensajeFechaInvalida = "La fecha debe ser una fecha existente con formato yyyy-MM-dd.";
    private const string MensajeFechasInvertidas = "La fecha desde no puede ser posterior a la fecha hasta.";
    private const string MensajePaginaInvalida = "La página debe ser un número entero mayor o igual que 1.";
    private const string MensajePaginaFueraDeRango = "La página no puede ser mayor que la última página ({0}).";

    private readonly IPedidoRepositorio _repositorio;

    public ListadoPedidosServicio(IPedidoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    /// <summary>
    /// Devuelve la página de pedidos que cumple los filtros enviados, o los errores de los parámetros inválidos.
    /// Los parámetros vacíos o con solo espacios se tratan como no enviados.
    /// </summary>
    public ResultadoListadoPedidos Listar(string? clienteId, string? estado, string? desde, string? hasta, string? pagina)
    {
        var errores = new Dictionary<string, string[]>();

        if (EsEnviado(estado) && !EstadosPedido.EsValido(estado.Trim()))
        {
            errores[ParametroEstado] = [MensajeEstadoInvalido];
        }

        var fechaDesde = LeerFecha(desde, ParametroDesde, errores);
        var fechaHasta = LeerFecha(hasta, ParametroHasta, errores);

        if (fechaDesde is not null && fechaHasta is not null && fechaDesde > fechaHasta)
        {
            errores[ParametroDesde] = [MensajeFechasInvertidas];
            errores[ParametroHasta] = [MensajeFechasInvertidas];
        }

        var numeroPagina = 1;
        if (EsEnviado(pagina) && (!int.TryParse(pagina, out numeroPagina) || numeroPagina < 1))
        {
            errores[ParametroPagina] = [MensajePaginaInvalida];
        }

        if (errores.Count > 0)
        {
            return ResultadoListadoPedidos.ConErrores(errores);
        }

        var coincidencias = _repositorio.ObtenerTodos()
            .Where(p => !EsEnviado(clienteId)
                || string.Equals(p.ClienteId, clienteId.Trim(), StringComparison.OrdinalIgnoreCase))
            .Where(p => !EsEnviado(estado)
                || string.Equals(p.Estado, estado.Trim(), StringComparison.OrdinalIgnoreCase))
            .Where(p => fechaDesde is null || p.Fecha >= fechaDesde)
            .Where(p => fechaHasta is null || p.Fecha <= fechaHasta)
            .OrderBy(p => p.Fecha)
            .ThenBy(p => p.Id)
            .ToList();

        var ultimaPagina = Math.Max(1, (int)Math.Ceiling(coincidencias.Count / (double)TamanoPagina));
        if (numeroPagina > ultimaPagina)
        {
            errores[ParametroPagina] = [string.Format(CultureInfo.InvariantCulture, MensajePaginaFueraDeRango, ultimaPagina)];
            return ResultadoListadoPedidos.ConErrores(errores);
        }

        var elementos = coincidencias
            .Skip((numeroPagina - 1) * TamanoPagina)
            .Take(TamanoPagina)
            .ToList();

        return ResultadoListadoPedidos.Exito(new PaginaPedidos(elementos, coincidencias.Count, numeroPagina));
    }

    private static bool EsEnviado([NotNullWhen(true)] string? valor) => !string.IsNullOrWhiteSpace(valor);

    private static DateOnly? LeerFecha(string? valor, string parametro, Dictionary<string, string[]> errores)
    {
        if (!EsEnviado(valor))
        {
            return null;
        }

        if (DateOnly.TryParseExact(valor.Trim(), FormatoFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
        {
            return fecha;
        }

        errores[parametro] = [MensajeFechaInvalida];
        return null;
    }
}
