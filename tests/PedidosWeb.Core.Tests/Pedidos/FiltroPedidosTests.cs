using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Tests.Pedidos;

public class FiltroPedidosTests
{
    private static readonly DateOnly Desde = new(2026, 3, 10);
    private static readonly DateOnly Hasta = new(2026, 3, 20);

    private static Pedido CrearPedido(int id, DateOnly fecha) =>
        new(id, $"PED-STGO-{id:000000}", "C-001", "Cliente de prueba", fecha, "Pendiente", 10000m, "STGO");

    // CA-01
    [Fact]
    public void EntreFechas_FechaIgualADesde_IncluyeElPedido()
    {
        var pedidos = new[] { CrearPedido(1, Desde) };

        var resultado = FiltroPedidos.EntreFechas(pedidos, Desde, Hasta);

        Assert.Equal(new[] { 1 }, resultado.Select(p => p.Id));
    }

    // CA-02
    [Fact]
    public void EntreFechas_FechaIgualAHasta_IncluyeElPedido()
    {
        var pedidos = new[] { CrearPedido(2, Hasta) };

        var resultado = FiltroPedidos.EntreFechas(pedidos, Desde, Hasta);

        Assert.Equal(new[] { 2 }, resultado.Select(p => p.Id));
    }

    // CA-03
    [Theory]
    [InlineData(2026, 3, 9)]
    [InlineData(2026, 3, 21)]
    public void EntreFechas_FechaFueraDelRango_ExcluyeElPedido(int anio, int mes, int dia)
    {
        var pedidos = new[] { CrearPedido(3, new DateOnly(anio, mes, dia)) };

        var resultado = FiltroPedidos.EntreFechas(pedidos, Desde, Hasta);

        Assert.Empty(resultado);
    }

    // CA-04
    [Fact]
    public void EntreFechas_DesdeMayorQueHasta_LanzaArgumentException()
    {
        var pedidos = new[] { CrearPedido(4, Desde) };

        var excepcion = Assert.Throws<ArgumentException>(() => FiltroPedidos.EntreFechas(pedidos, Hasta, Desde));

        Assert.Contains("no puede ser posterior", excepcion.Message);
    }
}
