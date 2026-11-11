using PedidosWeb.Core.Totales;

namespace PedidosWeb.Core.Tests.Totales;

public sealed class CalculoTotalesTests
{
    public static TheoryData<int> Casos()
    {
        var datos = new TheoryData<int>();

        foreach (var caso in GoldenMasterLector.LeerCasos())
        {
            datos.Add(caso.Caso);
        }

        return datos;
    }

    [Fact]
    public void GoldenMaster_Lectura_Tiene60Casos()
    {
        var casos = GoldenMasterLector.LeerCasos();

        Assert.Equal(60, casos.Count);
    }

    [Theory]
    [MemberData(nameof(Casos))]
    public void Calcular_CasoGoldenMaster_CoincideConElLegado(int numeroCaso)
    {
        var caso = GoldenMasterLector.LeerCasos().Single(c => c.Caso == numeroCaso);

        var resultado = CalculoTotales.Calcular(caso.Lineas, caso.EsVip, caso.Region, caso.FleteBase, caso.TasaIva);

        Assert.Equal(caso.Esperado.Subtotal, resultado.Subtotal);
        Assert.Equal(caso.Esperado.BaseDescuento, resultado.BaseDescuento);
        Assert.Equal(caso.Esperado.PorcDescuento, resultado.PorcDescuento);
        Assert.Equal(caso.Esperado.Descuento, resultado.Descuento);
        Assert.Equal(caso.Esperado.Flete, resultado.Flete);
        Assert.Equal(caso.Esperado.Neto, resultado.Neto);
        Assert.Equal(caso.Esperado.Iva, resultado.Iva);
        Assert.Equal(caso.Esperado.Total, resultado.Total);
    }
}
