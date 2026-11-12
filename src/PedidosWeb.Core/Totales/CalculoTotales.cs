namespace PedidosWeb.Core.Totales;

/// <summary>
/// Calcula descuento, flete, IVA y total de un pedido (migración de <c>UCalculoTotales.pas</c>).
/// </summary>
public static class CalculoTotales
{
    private const string CategoriaSinDescuento = "OFE";

    private const decimal UmbralDescuentoAlto = 1_000_000m;
    private const decimal UmbralDescuentoMedio = 500_000m;
    private const decimal PorcDescuentoAlto = 0.08m;
    private const decimal PorcDescuentoMedio = 0.05m;
    private const decimal PorcAdicionalVip = 0.03m;
    private const decimal PorcDescuentoMaximo = 0.10m;

    private const int RegionAysen = 11;
    private const int RegionMagallanes = 12;
    private const decimal RecargoFleteRegionesExtremas = 1.15m;

    /// <summary>
    /// Calcula los montos de un pedido a partir de sus líneas, el tipo de cliente y la región.
    /// </summary>
    public static ResultadoTotales Calcular(
        IReadOnlyList<LineaCalculo> lineas,
        bool esVip,
        int region,
        decimal fleteBase,
        decimal tasaIva)
    {
        var subtotal = lineas.Sum(Monto);
        var baseDescuento = lineas.Where(l => !EsSinDescuento(l)).Sum(Monto);
        var porcDescuento = CalcularPorcentajeDescuento(baseDescuento, esVip);
        var descuento = Redondear(baseDescuento * porcDescuento);
        var neto = subtotal - descuento;
        var flete = lineas.Count == 0 ? 0m : CalcularFlete(fleteBase, region);
        var iva = Redondear(neto * tasaIva);

        return new ResultadoTotales(
            subtotal,
            baseDescuento,
            porcDescuento,
            descuento,
            neto,
            flete,
            iva,
            neto + flete + iva);
    }

    private static decimal Monto(LineaCalculo linea) => linea.Cantidad * linea.PrecioUnitario;

    private static bool EsSinDescuento(LineaCalculo linea) =>
        string.Equals(linea.Categoria.Trim(), CategoriaSinDescuento, StringComparison.OrdinalIgnoreCase);

    private static decimal CalcularPorcentajeDescuento(decimal baseDescuento, bool esVip)
    {
        var porcentaje = baseDescuento > UmbralDescuentoAlto ? PorcDescuentoAlto
            : baseDescuento > UmbralDescuentoMedio ? PorcDescuentoMedio
            : 0m;

        if (esVip)
        {
            porcentaje += PorcAdicionalVip;
        }

        return Math.Min(porcentaje, PorcDescuentoMaximo);
    }

    private static decimal CalcularFlete(decimal fleteBase, int region) =>
        region is RegionAysen or RegionMagallanes
            ? Redondear(fleteBase * RecargoFleteRegionesExtremas)
            : Redondear(fleteBase);

    // R05: el legado usaba Trunc(x + 0.49) y el .50 bajaba; ahora sube al entero superior.
    private static decimal Redondear(decimal valor) => Math.Round(valor, 0, MidpointRounding.AwayFromZero);
}
