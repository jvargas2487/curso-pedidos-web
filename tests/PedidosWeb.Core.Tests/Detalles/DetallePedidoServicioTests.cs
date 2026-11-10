using PedidosWeb.Core.Detalles;
using PedidosWeb.Core.Lineas;
using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Tests.Detalles;

public class DetallePedidoServicioTests
{
    private readonly DetallePedidoServicio _servicio =
        new(new PedidoRepositorioEnMemoria(), new LineaPedidoRepositorioEnMemoria());

    // CA-02
    [Fact]
    public void Obtener_Pedido8_DevuelveSuDetalleCompleto()
    {
        var detalle = _servicio.Obtener(8);

        Assert.NotNull(detalle);
        Assert.Equal("PED-STGO-000008", detalle.Folio);
        Assert.Equal("Entregado", detalle.Estado);
        Assert.Equal("C-001", detalle.ClienteId);
        Assert.Equal("Ferretería El Clavo", detalle.ClienteNombre);
        Assert.Equal(new[] { 1, 2, 3 }, detalle.Lineas.Select(l => l.Numero));
        Assert.Equal(120100m, detalle.Total);
    }

    // CA-03
    [Fact]
    public void Obtener_IdInexistente_DevuelveNull()
    {
        var detalle = _servicio.Obtener(999);

        Assert.Null(detalle);
    }
}
