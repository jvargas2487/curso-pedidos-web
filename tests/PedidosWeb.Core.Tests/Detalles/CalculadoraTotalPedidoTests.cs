using PedidosWeb.Core.Detalles;
using PedidosWeb.Core.Lineas;

namespace PedidosWeb.Core.Tests.Detalles;

public class CalculadoraTotalPedidoTests
{
    // CA-01
    [Fact]
    public void CalcularTotal_LineasDelPedido8_Devuelve120100()
    {
        var lineas = new LineaPedidoRepositorioEnMemoria().ObtenerPorPedido(8);

        var total = CalculadoraTotalPedido.CalcularTotal(lineas);

        Assert.Equal(120100m, total);
        Assert.Equal(lineas.Sum(CalculadoraTotalPedido.CalcularSubtotal), total);
    }
}
