# HU-03 · Cálculo de totales del pedido

**Historia:** Como contabilidad, quiero que el portal calcule descuento, flete, IVA y total igual que Pedidos Legado, para migrar el cálculo sin que ningún cliente pague un peso distinto, salvo el defecto de redondeo que se corrige y se registra.

## Qué calcula

`CalculoTotales.Calcular(lineas, esVip, region, fleteBase, tasaIva)` en `PedidosWeb.Core/Totales/` replica `legado/src/UCalculoTotales.pas`:

- Subtotal: suma de `cantidad × precio` de todas las líneas.
- Base de descuento: igual que el subtotal, sin las líneas de categoría `OFE`.
- Porcentaje: base > 1.000.000 → 8 %; base > 500.000 → 5 %; si no, 0 %. Cliente VIP suma 3 %; tope 10 %.
- Descuento: base × porcentaje, redondeado. Neto: subtotal − descuento.
- Flete: 0 si no hay líneas; si no, el flete base de la región (× 1,15 en las regiones 11 y 12), redondeado.
- IVA: (neto + flete) × tasa, redondeado. Total: neto + flete + IVA.

Los montos son pesos chilenos (CLP), `decimal`.

## Criterios de aceptación

El contrato es `migracion/golden-master.csv` (60 casos, uno por pedido del legado).

- **CA-01** · Dado cada uno de los 60 casos del golden master, cuando calculo sus totales, entonces subtotal, descuento, flete, neto, IVA y total coinciden con el legado, y la suma de la columna `total` es 58.202.135.
- **CA-02** · Dada la corrección del redondeo (R05: el .50 sube al entero superior), cuando se recalculan los 60 casos, entonces cambian 13 casos y la suma de `total` pasa a 58.202.144.

## Restricciones

- Los umbrales de descuento replican el cálculo de pantalla (`>`); la diferencia con el grabado (`>=`) se registra como hallazgo, no se corrige.
- El golden master solo cambia al corregir R05, y cada caso que cambia queda en `docs/migracion/defectos-corregidos.md`.

## Fuera de alcance

- Descuento de Navidad (regla inactiva en el legado).
- Exponer el cálculo en la API.
- Leer tasa de IVA, flete o regiones desde parámetros: el cálculo los recibe como argumentos.
