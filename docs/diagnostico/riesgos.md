# Riesgos de Pedidos Legado

<!--
Etapa 2 · Encontrar.
Un riesgo es algo que puede salir mal en producción o al migrar.
- Evidencia: archivo:línea. Sin evidencia, es una sospecha, no un riesgo.
- Categoría: Seguridad · Datos e integridad · Fechas · Mantenibilidad.
- Prioridad: Alta · Media · Baja (¿qué tan grave es y qué tan probable?).
Punto de control E2: al menos 5 riesgos con evidencia y prioridad.
-->

Hallazgos del subagente `analista-legado`; cada archivo:línea fue abierto de nuevo por el agente principal antes de escribir esta tabla.

| ID | Riesgo | Evidencia (archivo:línea) | Categoría | Prioridad |
|---|---|---|---|---|
| RS-01 | Pantalla y grabado calculan el descuento con lógica duplicada y umbrales distintos (`>` frente a `>=`). Con base exacta de 500.000 o 1.000.000 el vendedor ve un total y se graba otro. El aviso de diferencia solo sale con `gModoPrueba`, que nunca es verdadero. | `legado/src/UCalculoTotales.pas:77`; `legado/src/UFrmPedidos.pas:298`, `:326-332`; `legado/src/UGlobales.pas:34`; `legado/db/datos/PEDIDOS.csv:9` | Datos e integridad | Alta |
| RS-02 | `MOV_STOCK` no existe en el esquema. `RegistrarMovimiento` falla siempre y el `except` vacío lo oculta: no hay trazabilidad de stock. | `legado/src/UStock.pas:85`, `:89-90`; `legado/db/esquema-dbisam.sql` (ninguna tabla `MOV_STOCK`); solo `UStock.pas` la menciona | Datos e integridad | Alta |
| RS-03 | Hay stock negativo en los datos (producto 9 con -12). El descuento lee y luego escribe sin bloqueo (carrera entre usuarios). El stock no se valida al agregar la línea. | `legado/db/datos/PRODUCTOS.csv:10`; `legado/src/UStock.pas:38-52`; `legado/src/UFrmPedidos.pas:148-168` | Datos e integridad | Media |
| RS-04 | Grabado sin transacción (hay un `TODO`). La cabecera se inserta con TOTAL 0, luego las líneas y luego el UPDATE del total, como sentencias sueltas: si una falla queda un pedido parcial o con total 0. Si falta stock, el pedido ya está grabado y solo sale un mensaje. | `legado/src/UFrmPedidos.pas:255`, `:262`, `:320`, `:323-324`; `legado/src/UStock.pas:69-72`; `legado/src/UDMPrincipal.pas:92-98` | Datos e integridad | Alta |
| RS-05 | La tasa de IVA vive en 3 sitios: `UGlobales`, PARAMETROS y 0,19 fijo en el informe. El 1,15 del flete y el 30.000 mínimo también están fijos. `StrToFloat` depende del separador decimal del equipo (por verificar). | `legado/src/UGlobales.pas:31`; `legado/src/UFrmPrincipal.pas:62-64`; `legado/src/UFrmInformes.pas:92`; `legado/src/UFrmPedidos.pas:231` | Mantenibilidad | Media |
| RS-06 | SQL concatenado en todo el sistema: una comilla en observación, RUT, búsqueda o usuario rompe la sentencia o permite inyección. `EscapaComillas` existe pero nadie la usa. | `legado/src/UFrmPedidos.pas:102`, `:152`, `:262`; `legado/src/UFrmClientes.pas:76-77`; `legado/src/UFrmInformes.pas:69`; `legado/src/UStock.pas:23`, `:85-88`; `legado/src/UDMPrincipal.pas:85`; `legado/src/UUtilidades.pas:164-167` | Seguridad | Alta |
| RS-07 | El mantenedor de clientes rechaza RUT válidos: con dígito 0 o K, `RutOk` invierte ambos. No se pueden grabar cambios en los clientes 7, 11, 18 y 23. Además `ValidaRut` no pasa la `k` a mayúscula y rechaza `76500019-k` (cliente 18). La búsqueda de pedido valida el RUT limpio pero consulta con el texto tal cual (con puntos no encuentra al cliente). | `legado/src/UFrmClientes.pas:192-199`; `legado/src/UUtilidades.pas:72-79`; `legado/db/datos/CLIENTES.csv:8`, `:12`, `:19`, `:24`; `legado/src/UFrmPedidos.pas:90-102` | Datos e integridad | Media |
| RS-08 | Código y configuración muertos: los parámetros `DESC_NAVIDAD`, `PEDIDO_MINIMO`, `VERSION` y `EMPRESA` no se leen, y la regla de Navidad no se puede activar. Más elementos en «Código muerto». | `legado/db/datos/PARAMETROS.csv:3`, `:5-7`; `legado/src/UFrmPrincipal.pas:62-67`; `legado/src/UGlobales.pas:15-21` | Mantenibilidad | Media |
| RS-09 | Credencial y rutas fijas: la clave de la base DBISAM está en claro en el ejecutable. La ruta `C:\PEDIDOS\DATOS` aparece en 3 sitios (el informe CSV se exporta ahí) y la impresora `LPT1` también es fija. | `legado/src/UDMPrincipal.pas:41`, `:59`; `legado/src/UGlobales.pas:36-37`; `legado/src/UDMPrincipal.dfm:12`; `legado/src/UFrmInformes.pas:111` | Seguridad | Alta |
| RS-10 | Sin autenticación real: el usuario se escribe en un cuadro de texto sin contraseña y escribir `ADMIN` da nivel 9. Los permisos (informes, VIP, borrar productos) solo se controlan en la interfaz. El borrado de productos solo oculta el botón del navegador (si la grilla permite borrar con teclado, por verificar). | `legado/src/UFrmPrincipal.pas:46-50`, `:95`; `legado/src/UFrmClientes.pas:143`; `legado/src/UFrmProductos.pas:44-45` | Seguridad | Alta |
| RS-11 | Folios e IDs con `MAX+1` sin bloqueo: dos usuarios pueden obtener el mismo folio. Esquema sin FK y con índices no únicos en RUT y código de producto. El borrado de clientes solo se protege en la pantalla; el de productos no se revisa. | `legado/src/UFrmPedidos.pas:249-251`; `legado/src/UFrmClientes.pas:107`, `:160-163`; `legado/db/esquema-dbisam.sql:27`, `:41` | Datos e integridad | Alta |
| RS-12 | Fechas: `StrAFecha` lee solo 2 dígitos de año, así que «01/01/2025» se lee como 2020, y un formato malo lanza excepción sin captura. El pivote 50 es fijo. El cierre de 18:00 y el día hábil (sin feriados) están fijos en el código. El informe «hasta hoy» no muestra pedidos con fecha futura (los hechos tras el cierre). | `legado/src/UUtilidades.pas:100-107`, `:124`, `:133`; `legado/src/UFrmInformes.pas:52-53`, `:63-64` | Fechas | Media |
| RS-13 | El estado `A` (anulado) existe en los datos, pero ningún código lo asigna ni repone stock. El informe suma anulados si se desmarca «Solo confirmados». | `legado/db/datos/PEDIDOS.csv:19`, `:43`; `legado/src/UFrmInformes.pas:70-71`; `legado/src/UFrmPedidos.pas:259-262` | Datos e integridad | Media |
| RS-14 | Los clientes inactivos pueden pedir: la búsqueda no filtra `ACTIVO`. Hay un pedido de un cliente inactivo (folio 1020 → cliente 30). | `legado/src/UFrmPedidos.pas:99-102`; `legado/db/datos/PEDIDOS.csv:21`; `legado/db/datos/CLIENTES.csv:31` | Datos e integridad | Baja |

## Código muerto

Confirmado con Grep: no hay llamadas ni lecturas fuera de la declaración y la asignación.

| Elemento | Evidencia |
|---|---|
| Funciones sin llamadas: `FormateaRut`, `SoloNumeros`, `Rellena`, `EscapaComillas`, `NombreMes` | `legado/src/UUtilidades.pas:82`, `:142`, `:154`, `:164`, `:169` |
| Variables globales asignadas y nunca leídas: `gFleteBase`, `gUltimoFolio`, `gFechaSistema`, `gImpresora` | `legado/src/UGlobales.pas:15`, `:18`, `:20-21`; asignaciones en `legado/src/UFrmPedidos.pas:346`, `legado/src/UFrmPrincipal.pas:53`, `:67` |
| Banderas que nunca cambian de `False`: `gDescNavidad`, `gModoPrueba` | `legado/src/UGlobales.pas:33-34` |
| Tablas del data module que solo se abren y cierran: `tblClientes`, `tblProductos`, `tblPedidos`, `tblLineas`, `tblParametros` | `legado/src/UDMPrincipal.pas:62-66`, `:72-76` |
| Parámetros sin lectura: `PEDIDO_MINIMO`, `DESC_NAVIDAD`, `VERSION`, `EMPRESA` (solo se leen `TASA_IVA` e `IMPRESORA`) | `legado/src/UFrmPrincipal.pas:62`, `:65` |
| Redondeo duplicado: `Calc3` y `RedondeaPesos` hacen lo mismo y ambas tienen llamadas | `legado/src/UCalculoTotales.pas:50`; `legado/src/UUtilidades.pas:30` |

## Por verificar

- Si un saldo vencido sin fecha de vencimiento bloquea al cliente (`AsDateTime` con valor nulo).
- Si `DaysBetween` bloquea por vencimientos futuros a más de 60 días.
- Si la grilla de productos permite borrar con teclado aunque se oculte el botón.
- Si `StrToFloat` y `FloatToStr` (`legado/src/UFrmPrincipal.pas:64`, `legado/src/UFrmPedidos.pas:271`, `:320`) fallan según el separador decimal del equipo.
- Si `FormatFloat('#,##0')` (`legado/src/UUtilidades.pas:35`) imprime el separador de miles chileno o el estadounidense.
- Cómo llegó el stock de -12 del producto 9: sin `MOV_STOCK` no hay rastro.
- Qué proceso o quién usa el estado `A`: no se encontró código que lo maneje.
- Si los precios de `PEDIDO_LINEAS.csv` coinciden con `PRODUCTOS.csv` (solo se revisó por muestreo).
- Cuántos folios históricos de producción caen exactamente en 500.000 o 1.000.000 (en los CSV son 13).
