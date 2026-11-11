using System.Globalization;
using PedidosWeb.Core.Totales;

namespace PedidosWeb.Core.Tests.Totales;

/// <summary>
/// Un caso del golden master: los datos del pedido y lo que calcula el legado.
/// </summary>
public sealed record CasoGoldenMaster(
    int Caso,
    int Folio,
    bool EsVip,
    int Region,
    decimal FleteBase,
    decimal TasaIva,
    IReadOnlyList<LineaCalculo> Lineas,
    ResultadoTotales Esperado);

/// <summary>
/// Lee <c>migracion/golden-master.csv</c>, copiado junto a los tests.
/// </summary>
public static class GoldenMasterLector
{
    private const string NombreArchivo = "Totales/golden-master.csv";

    /// <summary>
    /// Devuelve todos los casos del archivo, en el orden del CSV.
    /// </summary>
    public static IReadOnlyList<CasoGoldenMaster> LeerCasos()
    {
        var ruta = Path.Combine(AppContext.BaseDirectory, NombreArchivo);

        return File.ReadLines(ruta)
            .Skip(1)
            .Where(linea => !string.IsNullOrWhiteSpace(linea))
            .Select(LeerCaso)
            .ToList();
    }

    private static CasoGoldenMaster LeerCaso(string fila)
    {
        var c = fila.Split(',');

        var esperado = new ResultadoTotales(
            Subtotal: Decimal(c[7]),
            BaseDescuento: Decimal(c[8]),
            PorcDescuento: Decimal(c[9]),
            Descuento: Decimal(c[10]),
            Flete: Decimal(c[11]),
            Neto: Decimal(c[12]),
            Iva: Decimal(c[13]),
            Total: Decimal(c[14]));

        return new CasoGoldenMaster(
            Caso: int.Parse(c[0], CultureInfo.InvariantCulture),
            Folio: int.Parse(c[1], CultureInfo.InvariantCulture),
            EsVip: c[2] == "V",
            Region: int.Parse(c[3], CultureInfo.InvariantCulture),
            FleteBase: Decimal(c[4]),
            TasaIva: Decimal(c[5]),
            Lineas: c[6].Split('|').Select(LeerLinea).ToList(),
            Esperado: esperado);
    }

    private static LineaCalculo LeerLinea(string linea)
    {
        var partes = linea.Split('x');

        return new LineaCalculo(
            int.Parse(partes[0], CultureInfo.InvariantCulture),
            Decimal(partes[1]),
            partes[2]);
    }

    private static decimal Decimal(string valor) => decimal.Parse(valor, CultureInfo.InvariantCulture);
}
