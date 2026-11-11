# Reglas de negocio de Pedidos Legado

<!--
Etapa 2 · Encontrar (modo por defecto + subagente analista-legado).
Una regla es una decisión del negocio escondida en el código: un monto, un porcentaje, una condición, un tope.
- Archivo:línea: dónde está. Sin esta evidencia, la regla no cuenta.
- Condición: cuándo se aplica. Efecto: qué cambia.
- Verificada: «Sí» solo si TÚ abriste el archivo en esa línea y leíste el código.
  Si la regla aparece en más de un lugar, anota todos.
Punto de control E2: al menos 8 reglas, y al menos 5 verificadas leyendo el código.
-->

Hallazgos del subagente `analista-legado`; cada archivo:línea fue abierto de nuevo por el agente principal antes de escribir esta tabla. La columna «Verificada» queda para la revisión humana.

| # | Regla | Archivo:línea | Condición | Efecto | Verificada |
|---|---|---|---|---|---|
| 1 | Descuento por volumen en pantalla (`CalcularTotales`) | `legado/src/UCalculoTotales.pas:77`, `:79`, `:81` | Base de descuento > 1.000.000 → 8 %; base > 500.000 → 5 %; si no, 0 % (comparación estricta). | Fija el % que ve el vendedor en la pantalla. El comentario de cabecera (`legado/src/UCalculoTotales.pas:4`) dice «5% sobre 400.000»: no coincide con el código. | Si |
| 2 | Descuento por volumen al grabar (duplicado, se comporta distinto) | `legado/src/UFrmPedidos.pas:296`, `:298` | Base >= 1.000.000 → 8 %; base >= 500.000 → 5 % (comparación inclusiva). | Con base exacta de 500.000 o 1.000.000 el grabado da más descuento que la pantalla. Al recalcular los pedidos de los CSV, 13 caen justo en el límite y todos cuadran solo con `>=`: ej. folio 1008 (`legado/db/datos/PEDIDOS.csv:9`), 1011 (`:12`) y 1023 (`:24`); sus líneas en `legado/db/datos/PEDIDO_LINEAS.csv:32`, `:41`, `:67-69`. | Si |
| 3 | Descuento VIP y tope del 10 % | `legado/src/UCalculoTotales.pas:88-91`; `legado/src/UFrmPedidos.pas:302-303`, `:306-307` | Cliente tipo `V` suma 3 % aunque la base no llegue a 500.000. El total del descuento nunca pasa de 10 %. | Igual en ambos lugares. El folio 1038 (`legado/db/datos/PEDIDOS.csv:39`, cliente 5, tipo V) cuadra con el 3 % VIP. | Si |
| 4 | Descuento de Navidad (+2 %) | `legado/src/UFrmPedidos.pas:304-305`; inicialización en `legado/src/UGlobales.pas:33` | `gDescNavidad` verdadero y mes del pedido = 12. | Solo existe en el grabado; `CalcularTotales` no lo tiene. `gDescNavidad` solo se asigna `False` y el parámetro `DESC_NAVIDAD` (`legado/db/datos/PARAMETROS.csv:6`) no se lee: regla inactiva (ver RS-08). | Si |
| 5 | Categoría `OFE` queda fuera de la base de descuento | `legado/src/UCalculoTotales.pas:70`; `legado/src/UFrmPedidos.pas:287` | Línea con categoría `OFE`. | Cuenta en el subtotal, no en la base. La pantalla compara con `UpperCase(Trim())` sobre la categoría guardada en la línea; el grabado compara en SQL con `P.CATEGORIA <> 'OFE'` y toma la categoría actual de PRODUCTOS. Casi igual, pero la fuente es distinta. | Si |
| 6 | Flete por región (+15 % en regiones 11 y 12) | `legado/src/UCalculoTotales.pas:97-100`; `legado/src/UFrmPedidos.pas:312-315`; bases en `legado/db/datos/REGIONES.csv:12-13` | Región 11 (Aysén) o 12 (Magallanes). Códigos y 1,15 fijos en el código. | Flete = base de la región × 1,15, redondeado. Igual en ambos lugares. Solo `CalcularTotales` pone flete 0 si no hay líneas (`legado/src/UCalculoTotales.pas:116-117`); el grabado exige líneas (`legado/src/UFrmPedidos.pas:209`). | Si |
| 7 | IVA sobre neto + flete | `legado/src/UCalculoTotales.pas:105`; `legado/src/UFrmPedidos.pas:318` | Siempre. | IVA = (neto + flete) × tasa, redondeado; total = neto + flete + IVA. Igual en ambos lugares. | |
| 8 | Redondeo «al peso» | `legado/src/UCalculoTotales.pas:50`; `legado/src/UUtilidades.pas:30` | Todo descuento, flete e IVA. | `Trunc(x + 0.49)`: un .50 baja al entero inferior. Dos copias idénticas (`Calc3` y `RedondeaPesos`): la pantalla usa una y el grabado la otra (`legado/src/UFrmPedidos.pas:308`, `:313`, `:315`, `:318`). Ejemplo: flete región 12 = 16.490 × 1,15 = 18.963,5 → 18.963. | |
| 9 | Tasa de IVA (3 fuentes) | `legado/src/UGlobales.pas:31`; `legado/db/datos/PARAMETROS.csv:2` leído en `legado/src/UFrmPrincipal.pas:62-64`; `legado/src/UFrmInformes.pas:92` | Al iniciar vale 0,19 fijo; si PARAMETROS tiene `TASA_IVA`, lo sobrescribe (19 / 100). El informe usa 0,19 fijo. | Los pedidos usan la tasa de PARAMETROS; el informe de ventas la ignora: si la tasa cambia, el informe queda mal. | |
| 10 | Pedido mínimo $30.000 (Santiago exento) | `legado/src/UFrmPedidos.pas:231-233` | Subtotal < 30.000 y región distinta de 13. | Rechaza el pedido. El parámetro `PEDIDO_MINIMO` = 25000 (`legado/db/datos/PARAMETROS.csv:3`) no se lee en ningún archivo: manda el 30.000 fijo. | |
| 11 | Bloqueo por deuda vencida | `legado/src/UFrmPedidos.pas:111-117` | Saldo vencido > 0 y `DaysBetween(Date, FECHA_VENCIMIENTO)` > 60. | Mensaje de bloqueo y no deja continuar. Límite de 60 días fijo. `DaysBetween` devuelve valor absoluto: un vencimiento futuro a más de 60 días también bloquearía (por verificar). Con saldo > 0 y fecha vacía, `AsDateTime` suele dar 0 y bloquearía (por verificar). | |
| 12 | Límites del pedido | `legado/src/UFrmPedidos.pas:143`, `:214`, `:221` | Cantidad <= 0; más de 40 líneas; precio unitario <= 0. | Rechazo. El máximo de 40 líneas se valida solo al grabar. | |
| 13 | Stock: los VIP pueden quedar hasta -20 | `legado/src/UStock.pas:43`, `:48` | Stock resultante < -20 (VIP) o < 0 (normal). | Si no cumple, no descuenta y avisa; el pedido ya está grabado (ver RS-04). El -20 es fijo. | |
| 14 | Fecha del pedido: cierre a las 18:00 y día hábil | `legado/src/UUtilidades.pas:133-138`, `:124`; uso en `legado/src/UFrmPedidos.pas:240-243` | Hora >= 18. Día hábil = lunes a viernes. | El pedido pasa al siguiente día hábil (sábado y domingo se saltan; no hay feriados). Un sábado o domingo antes de las 18:00 la fecha queda tal cual. La hora 18 no está en PARAMETROS (no encontrado). | |
| 15 | Aviso de primer pedido a cobranza | `legado/src/UFrmPedidos.pas:338-342` | El cliente tiene exactamente 1 pedido, ya contando el recién grabado. | Muestra «Avisar a cobranza». El comentario lo llama «log simple», pero no registra nada. | |
| 16 | Niveles de usuario y permisos | `legado/src/UFrmPrincipal.pas:49-50`, `:95`; `legado/src/UFrmClientes.pas:143`; `legado/src/UFrmProductos.pas:44-45` | Usuario `ADMIN` o `JEFEVENTAS` → nivel 9; cualquier otro → 1. | Informes exigen nivel >= 2; crear cliente VIP exige >= 2; borrar productos exige 9. El nivel 2 «supervisor» (`legado/src/UGlobales.pas:13`) nunca se asigna. | |
| 17 | Validación de RUT con dos algoritmos distintos | `legado/src/UUtilidades.pas:72-79`; `legado/src/UFrmClientes.pas:192-199` | El pedido usa `ValidaRut`; el mantenedor de clientes usa `RutOk`. | Cuando el resto es 11 o 10, el dígito se invierte: `ValidaRut` da 11→`0` y 10→`K`; `RutOk` da 11→`K` y 10→`0`. Consecuencia en RS-07. | |
| 18 | Informe de ventas: neto derivado del total | `legado/src/UFrmInformes.pas:70-71`, `:92-93`; `legado/src/UFrmInformes.dfm:73-75` | «Solo confirmados» viene marcado por defecto (`ESTADO = 'C'`). | Neto = total / 1,19 (incluye el flete) e IVA = diferencia; no suma los netos reales de cada pedido. | |

## Reglas repartidas en varios archivos

| Regla | Dónde está | ¿Se comporta igual? |
|---|---|---|
| Descuento por volumen | `legado/src/UCalculoTotales.pas:75-83` y `legado/src/UFrmPedidos.pas:296-301` | No: `>` frente a `>=`. |
| VIP y tope 10 % | `legado/src/UCalculoTotales.pas:88-91` y `legado/src/UFrmPedidos.pas:302-307` | Sí. |
| Navidad | Solo `legado/src/UFrmPedidos.pas:304` | No está en `UCalculoTotales`; además está inactiva. |
| Flete | `legado/src/UCalculoTotales.pas:95-101` y `legado/src/UFrmPedidos.pas:312-315` | Sí en la fórmula. |
| Redondeo | `legado/src/UCalculoTotales.pas:50` y `legado/src/UUtilidades.pas:30` | Sí (fórmula idéntica); es duplicación, no divergencia. |
| IVA | Ver regla 9 | No: 3 fuentes. |
| Stock y VIP | `legado/src/UStock.pas:40-50`; el grabado pasa `FEsVip` en `legado/src/UFrmPedidos.pas:324` | Una sola implementación. |

## Comentarios que no coinciden con el código

| Comentario | Evidencia del comentario | Qué hace el código | Evidencia del código |
|---|---|---|---|
| «descuento por volumen 5% sobre 400.000» | `legado/src/UCalculoTotales.pas:4` | 5 % sobre 500.000 y 8 % sobre 1.000.000. El folio 1017 (base 400.500) no tiene descuento. | `legado/src/UCalculoTotales.pas:77-79`; `legado/db/datos/PEDIDOS.csv:18` |
| «redondeo al peso (igual que en totales)» | `legado/src/UUtilidades.pas:27` | La fórmula sí es igual; lo impreciso es «al peso»: el .50 baja. | `legado/src/UUtilidades.pas:30` |
| «aviso a vendedor si el total cambio» | `legado/src/UFrmPedidos.pas:326` | Solo avisa si `gModoPrueba`, que nunca es verdadero: no avisa nunca. | `legado/src/UFrmPedidos.pas:330` |
| «log simple» | `legado/src/UFrmPedidos.pas:334` | No registra nada; es un mensaje de primer pedido. | `legado/src/UFrmPedidos.pas:341-342` |
| «promo navidad 2009» | `legado/src/UGlobales.pas:33` | Regla inactiva. | `legado/src/UGlobales.pas:33`; `legado/src/UFrmPedidos.pas:304` |
| «1 = vendedor, 2 = supervisor, 9 = admin» | `legado/src/UGlobales.pas:13` | El nivel 2 nunca se asigna. | `legado/src/UFrmPrincipal.pas:49-50` |
| «con deuda vencida no se le vende» | `legado/src/UFrmPedidos.pas:110` | Solo si pasan más de 60 días. | `legado/src/UFrmPedidos.pas:111-112` |
| README «Los informes solo los ve jefatura» | `legado/README.md:10` | Bastan los nombres `ADMIN` o `JEFEVENTAS`, sin contraseña. | `legado/src/UFrmPrincipal.pas:49-50` |
