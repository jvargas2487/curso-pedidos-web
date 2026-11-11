# Defectos corregidos · migración del cálculo de totales

## D1 · R05 · Redondeo «al peso» (CORREGIDO)

- **Qué hace el legado:** `Calc3` redondea con `Trunc(x + 0.49)`, así que un valor terminado en `,50` baja al entero inferior (`legado/src/UCalculoTotales.pas:50`; copia idéntica `RedondeaPesos` en `legado/src/UUtilidades.pas:30`). Ejemplo: flete de la región 12 = 16.490 × 1,15 = 18.963,5 → el legado cobra 18.963.
- **Corrección:** el C# redondea con `Math.Round(valor, 0, MidpointRounding.AwayFromZero)`: el `,50` sube al entero superior (`src/PedidosWeb.Core/Totales/CalculoTotales.cs`). Se aplica a descuento, flete e IVA.
- **Por qué:** el redondeo «al peso» comercial y tributario lleva el `,50` hacia arriba; el legado cobra o descuenta un peso distinto en cada borde `,50`.
- **Casos del golden master que cambian:** 13 de 60, por 1 o 2 pesos cada uno: casos 2, 6, 8, 18, 21, 23, 25, 27, 36, 37, 42, 46 y 54 (folios 1002, 1006, 1008, 1018, 1021, 1023, 1025, 1027, 1036, 1037, 1042, 1046 y 1054). Cambia el flete (+1) en 9 casos, el IVA en 4 y el descuento (+1) en 2; el total baja en los casos 6 y 25, porque un descuento mayor reduce el neto.
- **Suma de la columna total:** 58.202.135 → 58.202.144 (+9).
- **Commit:** `fix(totales): corrige R05 y completa la documentación de migración (U5)`, separado del commit «igual al legado».
- **Convivencia:** mientras conviven ambos sistemas, el negocio debe conciliar diferencias de 1 o 2 pesos por pedido entre lo que graba el legado y lo que calcula el portal; son los 13 casos de arriba y cualquier otro pedido con un `,50` en descuento, flete o IVA.

## H1 · Umbrales de descuento distintos entre pantalla y grabado (HALLAZGO, NO CORREGIDO)

- **Qué encontraste:** la pantalla da descuento solo si la base es mayor que 500.000 / 1.000.000 (`legado/src/UCalculoTotales.pas:77` y `:79`, comparación `>`); el grabado lo da también con base igual a esos valores (`legado/src/UFrmPedidos.pas:296` y `:298`, comparación `>=`). El comentario de cabecera dice «5% sobre 400.000» (`legado/src/UCalculoTotales.pas:4`) y no coincide con ninguno.
- **Efecto:** con base exacta de 500.000 o 1.000.000 el vendedor ve un total y se graba otro; 13 pedidos de los datos reales caen justo en el límite (`docs/diagnostico/reglas-negocio.md`, regla 2, y RS-01 en `docs/diagnostico/riesgos.md`). El golden master refleja la pantalla, no lo grabado, y no se modificó por este hallazgo.
- **Quién decide:** Comercial junto con Contabilidad. El C# replica la versión de pantalla (`>`) hasta que decidan si el límite lleva descuento.
