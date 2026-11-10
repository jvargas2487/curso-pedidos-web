using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Tests.Pedidos;

public class ListadoPedidosServicioTests
{
    private readonly ListadoPedidosServicio _servicio = new(new PedidoRepositorioEnMemoria());

    // CA-1
    [Fact]
    public void Listar_SinParametros_DevuelvePrimeraPaginaDe20()
    {
        var resultado = _servicio.Listar(null, null, null, null, null);

        Assert.True(resultado.EsValido);
        Assert.Equal(45, resultado.Pagina!.Total);
        Assert.Equal(1, resultado.Pagina.Pagina);
        Assert.Equal(20, resultado.Pagina.Elementos.Count);
        Assert.Equal(40, resultado.Pagina.Elementos[0].Id);
        Assert.Equal(43, resultado.Pagina.Elementos[^1].Id);
    }

    // CA-2
    [Fact]
    public void Listar_TerceraPagina_DevuelveLosUltimos5()
    {
        var resultado = _servicio.Listar(null, null, null, null, "3");

        Assert.Equal(45, resultado.Pagina!.Total);
        Assert.Equal(new[] { 44, 23, 2, 42, 21 }, Ids(resultado));
    }

    // CA-3
    [Fact]
    public void Listar_PorCliente_DevuelveSusPedidosOrdenados()
    {
        var resultado = _servicio.Listar("C-001", null, null, null, null);

        Assert.Equal(new[] { 40, 32, 24, 16, 8 }, Ids(resultado));
    }

    // CA-4
    [Fact]
    public void Listar_ClienteEnMinusculas_DevuelveLosMismosPedidos()
    {
        var resultado = _servicio.Listar("c-001", null, null, null, null);

        Assert.Equal(new[] { 40, 32, 24, 16, 8 }, Ids(resultado));
    }

    // CA-5
    [Fact]
    public void Listar_PorEstado_DevuelvePendientesOrdenados()
    {
        var resultado = _servicio.Listar(null, "Pendiente", null, null, null);

        Assert.Equal(new[] { 36, 9, 45, 18, 27 }, Ids(resultado));
    }

    // CA-6
    [Fact]
    public void Listar_EstadoEnMinusculas_DevuelveLosMismosPedidos()
    {
        var resultado = _servicio.Listar(null, "pendiente", null, null, null);

        Assert.Equal(new[] { 36, 9, 45, 18, 27 }, Ids(resultado));
    }

    // CA-7
    [Fact]
    public void Listar_EstadoAnulado_DevuelveLosAnulados()
    {
        var resultado = _servicio.Listar(null, "Anulado", null, null, null);

        Assert.Equal(new[] { 38, 11, 20, 29, 2 }, Ids(resultado));
    }

    // CA-8
    [Fact]
    public void Listar_RangoOctubre_DevuelveTotal23YPrimeraPaginaDe20()
    {
        var resultado = _servicio.Listar(null, null, "2026-10-01", "2026-10-31", null);

        Assert.Equal(23, resultado.Pagina!.Total);
        Assert.Equal(20, resultado.Pagina.Elementos.Count);
    }

    // CA-9
    [Fact]
    public void Listar_DesdeIgualHasta_IncluyeElDia()
    {
        var resultado = _servicio.Listar(null, null, "2026-09-02", "2026-09-02", null);

        Assert.Equal(new[] { 40 }, Ids(resultado));
    }

    // CA-10
    [Theory]
    [InlineData("2026-10-31", null, 21)]
    [InlineData(null, "2026-09-02", 40)]
    public void Listar_SoloUnExtremo_IncluyeElDia(string? desde, string? hasta, int idEsperado)
    {
        var resultado = _servicio.Listar(null, null, desde, hasta, null);

        Assert.Equal(new[] { idEsperado }, Ids(resultado));
    }

    // CA-11
    [Fact]
    public void Listar_FiltrosCombinados_AplicaTodos()
    {
        var resultado = _servicio.Listar("C-002", "Despachado", "2026-10-01", "2026-10-31", null);

        Assert.Equal(2, resultado.Pagina!.Total);
        Assert.Equal(new[] { 37, 21 }, Ids(resultado));
    }

    // CA-12
    [Fact]
    public void Listar_ClienteYMes_DevuelveOrdenPorFecha()
    {
        var resultado = _servicio.Listar("C-002", null, "2026-09-01", "2026-09-30", null);

        Assert.Equal(new[] { 13, 5, 45 }, Ids(resultado));
    }

    // CA-13
    [Fact]
    public void Listar_NombreDeCliente_NoDevuelvePedidos()
    {
        var resultado = _servicio.Listar("Ferretería El Clavo", null, null, null, null);

        Assert.True(resultado.EsValido);
        Assert.Equal(0, resultado.Pagina!.Total);
        Assert.Empty(resultado.Pagina.Elementos);
    }

    // CA-14
    [Fact]
    public void Listar_ClienteInexistente_DevuelvePaginaVacia()
    {
        var resultado = _servicio.Listar("C-999", null, null, null, null);

        Assert.True(resultado.EsValido);
        Assert.Equal(0, resultado.Pagina!.Total);
        Assert.Equal(1, resultado.Pagina.Pagina);
        Assert.Empty(resultado.Pagina.Elementos);
    }

    // CA-15
    [Fact]
    public void Listar_EstadoInvalido_DevuelveErrorEnEstado()
    {
        var resultado = _servicio.Listar(null, "Cancelado", null, null, null);

        Assert.False(resultado.EsValido);
        Assert.True(resultado.Errores.ContainsKey("estado"));
    }

    // CA-16
    [Fact]
    public void Listar_FechaMalFormada_DevuelveErrorEnDesde()
    {
        var resultado = _servicio.Listar(null, null, "2026-13-01", null, null);

        Assert.False(resultado.EsValido);
        Assert.True(resultado.Errores.ContainsKey("desde"));
    }

    // CA-17
    [Fact]
    public void Listar_FechasInvertidas_DevuelveErrorEnDesdeYHasta()
    {
        var resultado = _servicio.Listar(null, null, "2026-10-31", "2026-10-01", null);

        Assert.False(resultado.EsValido);
        Assert.True(resultado.Errores.ContainsKey("desde"));
        Assert.True(resultado.Errores.ContainsKey("hasta"));
    }

    // CA-18
    [Fact]
    public void Listar_PaginaCero_DevuelveErrorEnPagina()
    {
        var resultado = _servicio.Listar(null, null, null, null, "0");

        Assert.False(resultado.EsValido);
        Assert.True(resultado.Errores.ContainsKey("pagina"));
    }

    // CA-19
    [Fact]
    public void Listar_PaginaMayorALaUltima_DevuelveErrorEnPagina()
    {
        var resultado = _servicio.Listar(null, null, null, null, "4");

        Assert.False(resultado.EsValido);
        Assert.True(resultado.Errores.ContainsKey("pagina"));
    }

    // CA-20
    [Fact]
    public void Listar_SinCoincidenciasYPagina2_DevuelveErrorEnPagina()
    {
        var resultado = _servicio.Listar("C-999", null, null, null, "2");

        Assert.False(resultado.EsValido);
        Assert.True(resultado.Errores.ContainsKey("pagina"));
    }

    // CA-21
    [Fact]
    public void Listar_PaginaNoNumerica_DevuelveErrorEnPagina()
    {
        var resultado = _servicio.Listar(null, null, null, null, "abc");

        Assert.False(resultado.EsValido);
        Assert.True(resultado.Errores.ContainsKey("pagina"));
    }

    // CA-22
    [Fact]
    public void Listar_ParametrosVacios_SeIgnoran()
    {
        var resultado = _servicio.Listar("", "   ", "", "   ", "");

        Assert.True(resultado.EsValido);
        Assert.Equal(45, resultado.Pagina!.Total);
    }

    private static int[] Ids(ResultadoListadoPedidos resultado) =>
        resultado.Pagina!.Elementos.Select(p => p.Id).ToArray();
}
