namespace PedidosWeb.Core.Folios;

/// <summary>
/// Genera el folio de un pedido con el mismo formato del sistema legado.
/// Las reglas completas y los ejemplos están en el README, sección «La función del taller».
/// </summary>
public static class FolioFormatter
{
    private const int NumeroMinimo = 1;
    private const int NumeroMaximo = 999999;
    private const int LargoMinimoSucursal = 2;
    private const int LargoMaximoSucursal = 4;

    /// <summary>
    /// Devuelve el folio con el formato PED-{SUCURSAL}-{NUMERO}, por ejemplo "PED-STGO-000123".
    /// </summary>
    /// <param name="numero">Número correlativo del pedido, entre 1 y 999999.</param>
    /// <param name="sucursal">Código de la sucursal: de 2 a 4 letras de la A a la Z.</param>
    public static string FormatearFolio(int numero, string sucursal)
    {
        // R5: el número se valida antes que la sucursal.
        if (numero < NumeroMinimo || numero > NumeroMaximo)
        {
            throw new ArgumentOutOfRangeException(
                nameof(numero), numero, $"El número debe estar entre {NumeroMinimo} y {NumeroMaximo}.");
        }

        ArgumentNullException.ThrowIfNull(sucursal);

        var codigo = sucursal.Trim().ToUpperInvariant();

        if (!EsCodigoSucursalValido(codigo))
        {
            throw new ArgumentException(
                $"La sucursal debe tener entre {LargoMinimoSucursal} y {LargoMaximoSucursal} letras de la A a la Z.",
                nameof(sucursal));
        }

        return $"PED-{codigo}-{numero:D6}";
    }

    // El legado guarda la sucursal en un campo ASCII de 4 caracteres: solo A-Z, sin Ñ ni tildes.
    private static bool EsCodigoSucursalValido(string codigo) =>
        codigo.Length is >= LargoMinimoSucursal and <= LargoMaximoSucursal
        && codigo.All(char.IsAsciiLetterUpper);
}
