# Inventario de Pedidos Legado

<!--
Etapa 1 · Entender (plan mode).
Una fila por cada archivo de legado/src/ (.pas y .dfm). Puedes agregar legado/PedidosLegado.dpr al final.
- Tipo: Formulario, Data module, Unidad de lógica, Utilidades, Globales…
- Responsabilidad: qué hace, en una frase y con tus palabras.
- Líneas: pídele a Claude que las cuente con una herramienta; no las estimes a ojo.
Punto de control E1: están todos los archivos de legado/src/.
-->

| Archivo | Tipo | Responsabilidad | Líneas |
|---|---|---|---|
| `src/UCalculoTotales.pas` | Unidad de lógica | Calcula subtotal, descuento (volumen + VIP, con tope), flete por región, IVA y total; se usa para los totales que ve el vendedor en pantalla. | 147 |
| `src/UDMPrincipal.pas` | Data module | Conecta la base DBISAM, abre las 5 tablas y ofrece `LeerParametro` y `EjecutarSQL` al resto del sistema. | 100 |
| `src/UDMPrincipal.dfm` | Data module (diseño) | Define la base `PEDIDOS` en `C:\PEDIDOS\DATOS`, las 5 tablas y las 2 consultas compartidas. | 65 |
| `src/UFrmClientes.pas` | Formulario | Mantenedor de clientes: busca por razón social, crea y graba con validaciones (RUT, tipo, región, VIP solo jefatura) y elimina si no tiene pedidos. | 215 |
| `src/UFrmClientes.dfm` | Formulario (diseño) | Pantalla de clientes: grilla, campos editables, botones y su propia tabla `CLIENTES`. | 213 |
| `src/UFrmInformes.pas` | Formulario | Informe de ventas por rango de fechas y cliente, con totales neto, IVA y general, y exportación a CSV para contabilidad. | 137 |
| `src/UFrmInformes.dfm` | Formulario (diseño) | Pantalla del informe: filtros, grilla de resultados, resumen y su propia consulta. | 160 |
| `src/UFrmPedidos.pas` | Formulario | Toma de pedidos: busca el cliente por RUT, agrega y quita líneas, muestra totales, graba pedido y líneas, recalcula el total grabado y descuenta stock. | 420 |
| `src/UFrmPedidos.dfm` | Formulario (diseño) | Pantalla de toma de pedidos: datos del cliente, grilla de líneas, totales, observaciones y botones. | 226 |
| `src/UFrmPrincipal.pas` | Formulario principal | Pide el usuario, fija su nivel, conecta la base, carga parámetros (IVA, impresora) y abre las demás pantallas desde el menú. | 104 |
| `src/UFrmPrincipal.dfm` | Formulario (diseño) | Menú principal (Archivo, Maestros, Ventas) y barra de estado. | 62 |
| `src/UFrmProductos.pas` | Formulario | Mantenedor de productos: filtra por descripción y bajo mínimo, valida al grabar y solo deja borrar al administrador. | 105 |
| `src/UFrmProductos.dfm` | Formulario (diseño) | Pantalla de productos: filtros, grilla, navegador y su propia tabla `PRODUCTOS`. | 98 |
| `src/UGlobales.pas` | Globales | Variables compartidas por todo el sistema (usuario, nivel, IVA, flete base, descuento de navidad, ruta de datos) y su inicialización. | 41 |
| `src/UStock.pas` | Unidad de lógica | Consulta y descuenta el stock de cada línea del pedido (los VIP pueden quedar en negativo) y registra el movimiento. | 93 |
| `src/UUtilidades.pas` | Utilidades | Funciones de apoyo: redondeo y formato de pesos, RUT, fechas, día hábil, fecha del pedido según la hora de cierre y textos. | 189 |
| `PedidosLegado.dpr` | Programa | Arranque de la aplicación: crea el data module y el formulario principal. | 24 |

**Total `legado/src/`:** 16 archivos (10 `.pas` y 6 `.dfm`), 2.375 líneas. Líneas contadas con `wc -l`.

## Tablas de la base (DBISAM)

Definidas en `legado/db/esquema-dbisam.sql`; las filas son las de los CSV de `legado/db/datos/`.

| Tabla | Contenido | Quién la usa | Filas |
|---|---|---|---|
| `REGIONES` | Código, nombre y flete base de cada región. | `UFrmPedidos` (nombre y flete de la región del cliente). | 16 |
| `CLIENTES` | RUT, razón social, tipo (N/V), región, deuda vencida y estado. | `UFrmClientes` (mantenedor), `UFrmPedidos` (busca por RUT), `UFrmInformes` (nombre en el informe). | 30 |
| `PRODUCTOS` | Código, descripción, categoría, precio, stock y stock mínimo. | `UFrmProductos` (mantenedor), `UFrmPedidos` (busca por código), `UStock` (lee y actualiza el stock). | 40 |
| `PEDIDOS` | Folio, fecha, cliente, usuario, estado, observación y total. | `UFrmPedidos` (graba y actualiza el total), `UFrmClientes` (cuenta pedidos), `UFrmInformes` (lista). | 60 |
| `PEDIDO_LINEAS` | Folio, número de línea, producto, cantidad y precio unitario. | `UFrmPedidos` (graba y suma), `UStock` (descuenta stock). | 169 |
| `PARAMETROS` | Clave, valor y descripción de la configuración. | `UDMPrincipal.LeerParametro`, llamado desde `UFrmPrincipal` (solo `TASA_IVA` e `IMPRESORA`). | 6 |

## Lo que me llamó la atención

- **El README está desactualizado.** Dice «Delphi 5» y «Paradox vía BDE» (`legado/README.md:5-6`), pero el código usa DBISAM (`legado/src/UDMPrincipal.pas:6`, con el comentario «antes era alias BDE» en la línea 58) y la unidad `Variants` (`legado/src/UFrmPedidos.pas:8`), que no existe en Delphi 5 (por verificar con la versión real del compilador).
- **No hay Crystal Reports.** El README dice «Informes en Crystal Reports 8» (`legado/README.md:7`), pero en `legado/` no hay ningún informe ni referencia; el único informe es una grilla con exportación a CSV (`legado/src/UFrmInformes.pas`).
- **Una tabla que no existe.** `UStock` graba en `MOV_STOCK` (`legado/src/UStock.pas:85`), pero esa tabla no está en `esquema-dbisam.sql` y el error se descarta sin aviso (`except` vacío en `legado/src/UStock.pas:89`).
