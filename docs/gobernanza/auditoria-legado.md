# Auditoría del legado · pedidos-web

Alcance: `legado/src/UFrmPedidos.pas` y `legado/src/UDMPrincipal.pas`. El subagente `analista-legado` propuso los hallazgos; yo abrí cada línea citada antes de marcar el estado.

Patrón general: todas las reglas de negocio viven en el formulario. `EjecutarSQL` (`legado/src/UDMPrincipal.pas:92-98`) ejecuta cualquier texto SQL sin validar nada.

## Hallazgos

| ID | Hallazgo | Evidencia (archivo:línea) | Tipo | Severidad | Estado | Argumento |
|---|---|---|---|---|---|---|
| AU-01 | Clave de la base DBISAM en claro dentro del código y entregada a la sesión. | `legado/src/UDMPrincipal.pas:41` (`CLAVE_BD = 'Distrib#2008'`), `:59` (`Session.AddPassword(CLAVE_BD)`) | Seguridad | Alta | Validado | Las dos líneas dicen exactamente eso. |
| AU-02 | Inyección SQL: el RUT, el código de producto, el usuario y la observación se concatenan sin escapar. | `legado/src/UFrmPedidos.pas:102`, `:152`, `:262` (usuario y observación); `legado/src/UDMPrincipal.pas:85` | Seguridad | Alta | Validado | Las cuatro líneas concatenan texto en el SQL con `'''' + valor + ''''`. `EjecutarSQL` (`UDMPrincipal.pas:92-98`) lo ejecuta tal cual. |
| AU-03 | El grabado del pedido no es transaccional: cabecera con TOTAL 0, líneas, UPDATE del total y descuento de stock son sentencias sueltas. | `legado/src/UFrmPedidos.pas:255` (`// TODO transaccion`), `:263`, `:272`, `:320`, `:324` | Integridad | Alta | Validado | El comentario lo reconoce y cada paso llama a `EjecutarSQL` por separado. Un fallo intermedio deja un pedido parcial. |
| AU-04 | El folio sale de `MAX(FOLIO)+1` sin bloqueo: dos puestos grabando a la vez calculan el mismo folio y el segundo INSERT falla a mitad del grabado (combina con AU-03). | `legado/src/UFrmPedidos.pas:249-251`; `legado/db/esquema-dbisam.sql:45`, `:52` | Integridad | Alta | Validado | `:249-251` lee el máximo y suma 1. El esquema tiene `PRIMARY KEY ("FOLIO")`, así que no quedan duplicados, pero sí un error con el pedido a medias. |
| AU-05 | Las reglas de negocio solo se validan en el formulario: bloqueo por deuda, mínimo de 30.000 (salvo región 13), máximo 40 líneas, cantidad > 0, precio > 0, tope de descuento 10%. | `legado/src/UFrmPedidos.pas:111-112`, `:143`, `:214`, `:221`, `:231`, `:306`; capa de datos `legado/src/UDMPrincipal.pas:92-98` | Regla de negocio | Alta | Validado | Cada línea contiene la regla y `EjecutarSQL` no valida nada: cualquier otro cliente de la base (script, otro módulo, la futura API) se las salta. |
| AU-06 | El bloqueo por deuda solo se evalúa al buscar al cliente; al grabar solo se comprueba `FIdCliente = 0`. | `legado/src/UFrmPedidos.pas:111-117` (evaluación), `:204` (grabar) | Regla de negocio | Media | Validado | Dejar la pantalla abierta y grabar después no vuelve a evaluar la deuda. |
| AU-07 | Doble implementación del total: el grabado recalcula en SQL y compara con `CalcularTotales`, pero la diferencia solo se avisa con `gModoPrueba`. | `legado/src/UFrmPedidos.pas:327-332`; `legado/src/UGlobales.pas:34` | Regla de negocio | Media | Validado | `:330` condiciona el aviso a `gModoPrueba`; en producción se graba el total del recálculo en silencio. |
| AU-08 | Descuento, flete y mínimo con números fijos en el formulario (tramos 500.000/1.000.000, VIP +3%, flete +15% en regiones 11 y 12). | `legado/src/UFrmPedidos.pas:296-308`, `:312-315`, `:231` | Regla de negocio | Media | Validado | Valores literales en el código; solo el IVA usa `gTasaIVA` (`:318`). |
| AU-09 | «El cliente puede grabar igualmente con otra fecha» cuando el pedido entra fuera de horario. | `legado/src/UFrmPedidos.pas:240-243` | Regla de negocio | Baja | Descartado | La fecha la fija `FechaParaPedido(Now)` en `:240` y el usuario no la elige: `:241-243` solo muestra un `ShowMessage` informativo. No hay entrada de fecha que el cliente pueda cambiar. |
| AU-10 | Dos pedidos del mismo cliente grabados a la vez harían que «Primer pedido» no se avise nunca. | `legado/src/UFrmPedidos.pas:338-342` | Regla de negocio | Baja | Descartado | La condición `N = 1` se evalúa después de grabar (`:338-342`); depende del orden de ejecución, no hay evidencia en estas líneas de la carrera, y el aviso es solo informativo. Sin evidencia, es una opinión. |

## Cruce con el diagnóstico de la U4

Comparado con `docs/diagnostico/riesgos.md`:

- **Coinciden:** AU-01 con RS-09; AU-02 con RS-06; AU-03 con RS-04; AU-04 con RS-11; AU-07 con RS-01; AU-08 con RS-05.
- **Nuevos en esta auditoría:** AU-05 (reglas solo en el formulario, enfocado en dónde se validan, no en qué valida) y AU-06 (bloqueo por deuda solo al buscar).
- **De la U4 que el subagente no vio:** RS-02, RS-03, RS-07, RS-08, RS-10, RS-12, RS-13 y RS-14, porque viven en `UStock.pas`, `UUtilidades.pas`, `UFrmClientes.pas`, `UFrmPrincipal.pas` y los CSV, fuera del alcance de esta auditoría. RS-10 (autenticación sin contraseña) es el más relevante para el tema de seguridad.

Precisión sobre AU-04: el esquema define `PRIMARY KEY ("FOLIO")` (`esquema-dbisam.sql:52`), así que el riesgo real es un error a mitad del grabado, no un folio duplicado. Los índices no únicos de RS-11 son los de RUT y código de producto.
