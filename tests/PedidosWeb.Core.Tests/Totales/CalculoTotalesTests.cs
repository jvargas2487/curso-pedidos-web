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

    [Theory]
    [InlineData(5)]
    [InlineData(11)]
    public void Calcular_PedidoSinLineas_TodosLosMontosEnCero(int region)
    {
        var lineas = new List<LineaCalculo>();

        var resultado = CalculoTotales.Calcular(lineas, esVip: false, region, fleteBase: 4_900m, tasaIva: 0.19m);

        Assert.Equal(new ResultadoTotales(0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m), resultado);
    }

    [Theory]
    [InlineData(0, 0, 104_900)]
    [InlineData(0.10, 10_490, 115_390)]
    public void Calcular_TasaIvaDistintaDe19_AplicaLaTasaRecibida(double tasa, int ivaEsperado, int totalEsperado)
    {
        var lineas = new List<LineaCalculo> { new(10, 10_000m, "ABA") };

        var resultado = CalculoTotales.Calcular(lineas, esVip: false, region: 5, fleteBase: 4_900m, tasaIva: (decimal)tasa);

        Assert.Equal(ivaEsperado, resultado.Iva);
        Assert.Equal(totalEsperado, resultado.Total);
    }

    [Theory]
    [InlineData("ofe")]
    [InlineData(" OFE ")]
    [InlineData("Ofe")]
    public void Calcular_CategoriaOfeConMinusculasOEspacios_NoEntraAlDescuento(string categoria)
    {
        var lineas = new List<LineaCalculo> { new(10, 100_000m, categoria) };

        var resultado = CalculoTotales.Calcular(lineas, esVip: false, region: 5, fleteBase: 4_900m, tasaIva: 0.19m);

        Assert.Equal(
            new ResultadoTotales(1_000_000m, 0m, 0m, 0m, 1_000_000m, 4_900m, 190_931m, 1_195_831m),
            resultado);
    }

    [Fact]
    public void Calcular_ClienteVipConSoloLineasOfe_AplicaPorcentajeVipSobreBaseCero()
    {
        var lineas = new List<LineaCalculo> { new(8, 35_990m, "OFE") };

        var resultado = CalculoTotales.Calcular(lineas, esVip: true, region: 10, fleteBase: 9_900m, tasaIva: 0.19m);

        Assert.Equal(
            new ResultadoTotales(287_920m, 0m, 0.03m, 0m, 287_920m, 9_900m, 56_586m, 354_406m),
            resultado);
    }

    [Theory]
    [InlineData(500_001, 0.05, 25_000, 91_181, 571_082)]
    [InlineData(1_000_001, 0.08, 80_000, 175_731, 1_100_632)]
    public void Calcular_BaseUnPesoSobreElUmbral_AplicaElTramoSuperior(
        int baseDescuento, double porcentaje, int descuento, int iva, int total)
    {
        var lineas = new List<LineaCalculo> { new(1, baseDescuento, "ABA") };

        var resultado = CalculoTotales.Calcular(lineas, esVip: false, region: 5, fleteBase: 4_900m, tasaIva: 0.19m);

        Assert.Equal((decimal)porcentaje, resultado.PorcDescuento);
        Assert.Equal(descuento, resultado.Descuento);
        Assert.Equal(iva, resultado.Iva);
        Assert.Equal(total, resultado.Total);
    }
}
