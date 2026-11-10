# Plan HU-01 · Listar pedidos con filtros

## Contexto
La spec `specs/HU-01-listar-pedidos/spec.md` define `GET /api/pedidos` con filtros (cliente, estado, rango de fechas),
orden fijo, paginación de 20 y errores 400 en ProblemDetails. Hoy el endpoint no existe (`Program.cs` tiene el comentario
"HU-01 · PENDIENTE"). AGENTS.md pide:
- un test xUnit por criterio, en `tests/PedidosWeb.Core.Tests`;
- lógica en Core y Api sin lógica de negocio;
- nombres en español, `record` sellado, `DateOnly` y `decimal`;
- aprobación previa para editar `specs/`.

Decisiones del PO en esta sesión:
- CA-17: el mensaje de fechas invertidas va bajo las claves `desde` y `hasta`.
- `pagina` no numérica: error en la clave `pagina`.
- Parámetro vacío o solo espacios: se trata como no enviado.
- Los tests prueban la lógica en Core (`ListadoPedidosServicio`), no el endpoint. No hay proyecto nuevo ni NuGet.
- El límite superior de `pagina` solo se valida cuando los filtros son válidos, porque sin ellos no se puede calcular
  la última página. Con filtros inválidos, `pagina` solo se valida si es menor que 1 o no numérica. Se agrega a RN-9.

## Enfoque
- **Core** valida, filtra, ordena y pagina. Devuelve un resultado con la página o con errores por nombre de parámetro.
- **Api delgada**: el endpoint recibe los 5 parámetros como `string?` y llama al servicio. Si es válido responde
  `Results.Ok(resultado.Pagina)`; si no, responde `Results.ValidationProblem(resultado.Errores)`, que ya produce
  `application/problem+json`, el `title` del ejemplo 3 de la spec y `errors` por clave.
- **Parámetros como texto**: con `DateOnly?`/`int?` el binding respondería 400 sin `errors` (rompe CA-16 y CA-21).
  Las fechas se leen con `DateOnly.TryParseExact(valor, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out ...)`
  y `pagina` con `int.TryParse`. Los vacíos o con solo espacios (`string.IsNullOrWhiteSpace`) cuentan como no enviados.
- **Comparaciones**: `ClienteId` y `Estado` con `StringComparison.OrdinalIgnoreCase`.
- **Datos**: filtrado en memoria sobre `IPedidoRepositorio.ObtenerTodos()`; el repositorio no cambia.
- **Orden**: `OrderBy(Fecha).ThenBy(Id)`.
- **Paginación**: `total` cuenta las coincidencias; `ultimaPagina = max(1, ceil(total / 20))`.

## Archivos
Nuevos en `src/PedidosWeb.Core/Pedidos/`:
- `EstadosPedido.cs`: clase estática con los 5 estados válidos y `EsValido(string)`, sin distinguir mayúsculas.
- `PaginaPedidos.cs`: `sealed record PaginaPedidos(IReadOnlyList<Pedido> Elementos, int Total, int Pagina)`.
- `ResultadoListadoPedidos.cs`: `PaginaPedidos? Pagina`, `IReadOnlyDictionary<string, string[]> Errores` y `EsValido`.
- `ListadoPedidosServicio.cs`: `sealed`, recibe `IPedidoRepositorio` por constructor.
  - Método `Listar(string? clienteId, string? estado, string? desde, string? hasta, string? pagina)`.
  - Constantes: `TamanoPagina = 20` y los mensajes de error (`const string`, como en `FolioFormatter`).

Modificados:
- `src/PedidosWeb.Api/Program.cs`:
  - registrar `ListadoPedidosServicio` como singleton, igual que el repositorio;
  - agregar `app.MapGet("/api/pedidos", ...)` en el lugar del comentario PENDIENTE.
- `specs/HU-01-listar-pedidos/spec.md` (requiere tu aprobación explícita; es el paso 1):
  - RN-2 / RN-6: un parámetro vacío se trata como no enviado.
  - RN-7: el error de fechas invertidas va en las claves `desde` y `hasta`.
  - RN-9: `pagina` no numérica es error; el límite superior solo se valida con los filtros válidos.
  - CA-17: precisar las claves.
  - CA-21 y CA-22 nuevos (ver tabla).
  - Preguntas abiertas: cambiar "tests HTTP en Api.Tests" por "tests en Core; el mapeo HTTP se verifica a mano".

Nuevo:
- `tests/PedidosWeb.Core.Tests/Pedidos/ListadoPedidosServicioTests.cs`.
  - Usa `new ListadoPedidosServicio(new PedidoRepositorioEnMemoria())`, como `PedidoRepositorioEnMemoriaTests`.

No se tocan: `PedidosWeb.sln`, los `.csproj`, `AGENTS.md`, `IPedidoRepositorio`, `PedidoRepositorioEnMemoria` y `Pedido`.

## Tests (uno por criterio, en `ListadoPedidosServicioTests`)
En los criterios de error, el test verifica `EsValido == false` y la clave en `Errores`. Ese es el resultado que la Api
convierte en 400 ProblemDetails.

| CA | Test | Llamada a `Listar` | Assert |
|---|---|---|---|
| 1 | `Listar_SinParametros_DevuelvePrimeraPaginaDe20` | todo null | válido, total 45, pagina 1, 20 elementos, primero Id 40, último Id 43 |
| 2 | `Listar_TerceraPagina_DevuelveLosUltimos5` | pagina "3" | total 45, Ids [44, 23, 2, 42, 21] |
| 3 | `Listar_PorCliente_DevuelveSusPedidosOrdenados` | clienteId "C-001" | Ids [40, 32, 24, 16, 8] |
| 4 | `Listar_ClienteEnMinusculas_DevuelveLosMismosPedidos` | clienteId "c-001" | Ids [40, 32, 24, 16, 8] |
| 5 | `Listar_PorEstado_DevuelvePendientesOrdenados` | estado "Pendiente" | Ids [36, 9, 45, 18, 27] |
| 6 | `Listar_EstadoEnMinusculas_DevuelveLosMismosPedidos` | estado "pendiente" | Ids [36, 9, 45, 18, 27] |
| 7 | `Listar_EstadoAnulado_DevuelveLosAnulados` | estado "Anulado" | Ids [38, 11, 20, 29, 2] |
| 8 | `Listar_RangoOctubre_DevuelveTotal23YPrimeraPaginaDe20` | desde "2026-10-01", hasta "2026-10-31" | total 23, 20 elementos |
| 9 | `Listar_DesdeIgualHasta_IncluyeElDia` | desde = hasta = "2026-09-02" | Ids [40] |
| 10 | `Listar_SoloUnExtremo_IncluyeElDia` (`[Theory]`, 2 casos) | desde "2026-10-31" / hasta "2026-09-02" | Ids [21] / [40] |
| 11 | `Listar_FiltrosCombinados_AplicaTodos` | C-002, Despachado, 2026-10-01..2026-10-31 | total 2, Ids [37, 21] |
| 12 | `Listar_ClienteYMes_DevuelveOrdenPorFecha` | C-002, 2026-09-01..2026-09-30 | Ids [13, 5, 45] |
| 13 | `Listar_NombreDeCliente_NoDevuelvePedidos` | clienteId "Ferretería El Clavo" | válido, total 0, vacío |
| 14 | `Listar_ClienteInexistente_DevuelvePaginaVacia` | clienteId "C-999" | válido, total 0, pagina 1, vacío |
| 15 | `Listar_EstadoInvalido_DevuelveErrorEnEstado` | estado "Cancelado" | inválido, clave `estado` |
| 16 | `Listar_FechaMalFormada_DevuelveErrorEnDesde` | desde "2026-13-01" | inválido, clave `desde` |
| 17 | `Listar_FechasInvertidas_DevuelveErrorEnDesdeYHasta` | desde "2026-10-31", hasta "2026-10-01" | inválido, claves `desde` y `hasta` |
| 18 | `Listar_PaginaCero_DevuelveErrorEnPagina` | pagina "0" | inválido, clave `pagina` |
| 19 | `Listar_PaginaMayorALaUltima_DevuelveErrorEnPagina` | pagina "4" | inválido, clave `pagina` |
| 20 | `Listar_SinCoincidenciasYPagina2_DevuelveErrorEnPagina` | clienteId "C-999", pagina "2" | inválido, clave `pagina` |
| 21 (nuevo) | `Listar_PaginaNoNumerica_DevuelveErrorEnPagina` | pagina "abc" | inválido, clave `pagina` |
| 22 (nuevo) | `Listar_ParametrosVacios_SeIgnoran` | todos "" o "   " | válido, total 45 |

Los Ids se comparan con `Assert.Equal(new[] { ... }, ids)` para verificar también el orden (RN-8).

## Riesgos
- **Mapeo HTTP sin test**: el 200/400, `application/problem+json` y el JSON en camelCase dependen de
  `Results.Ok`/`Results.ValidationProblem`. Es una decisión aceptada; se verifica a mano (ver Verificación).
- **Binding**: si algún parámetro del endpoint queda tipado, el 400 sale sin `errors`. Todo entra como `string?`.
- **Desempate por `Id`**: los datos no tienen fechas repetidas, así que queda implementado pero sin test que lo distinga.
- **Filtrado en memoria**: sirve para 45 pedidos. Con PostgreSQL habrá que llevarlo al repositorio; queda fuera de esta historia.
- **Reglas globales**: tu CLAUDE.md global pide Result/Messages/Clean Architecture. Aquí mandan las convenciones del repo (decisión previa).

## Pasos
1. Con tu aprobación, actualizar la spec (RN-2/6, RN-7, RN-9, CA-17, CA-21, CA-22, Preguntas abiertas).
2. Escribir los 22 tests en `ListadoPedidosServicioTests` y confirmar que no compilan o fallan (el servicio no existe).
3. Implementar en Core: `EstadosPedido`, `PaginaPedidos`, `ResultadoListadoPedidos` y `ListadoPedidosServicio`.
4. Registrar el servicio y mapear `GET /api/pedidos` en `Program.cs`.
5. Correr `dotnet build` y `dotnet test` hasta que estén en verde: 22 existentes + 22 nuevos = 44.
6. Entregar el mensaje de commit (sin hacer commit).

## Verificación
- `dotnet build` sin errores ni advertencias; `dotnet test` con 44 tests en verde.
- `dotnet run --project src/PedidosWeb.Api` y probar a mano el mapeo HTTP:
  - `/api/pedidos?clienteId=C-002&estado=Despachado&desde=2026-10-01&hasta=2026-10-31` debe ser igual al ejemplo 1 de la spec;
  - `/api/pedidos?clienteId=C-999` debe ser igual al ejemplo 2;
  - `/api/pedidos?estado=Cancelado` debe devolver 400 `application/problem+json`, igual al ejemplo 3.
