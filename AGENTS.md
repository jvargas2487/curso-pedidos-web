# AGENTS.md

## Proyecto
Pedidos Web: portal de pedidos de una distribuidora, modernización del sistema Pedidos Legado (Delphi + DBISAM).
API REST en ASP.NET Core (.NET 10, Minimal API) con 45 pedidos ficticios en memoria; todavía sin base de datos.

## Comandos
- Compilar: `dotnet build`
- Tests: `dotnet test`
- Ejecutar la API: `dotnet run --project src/PedidosWeb.Api` (http://localhost:5080; comprobar con `GET /salud`)

## Estilo de código
- Tipos, métodos, propiedades y comentarios en español (`Pedido`, `ObtenerPorId`, `IPedidoRepositorio`).
- Dinero: `decimal` en CLP sin decimales; nunca `double` ni `float`.
- Fechas de pedido: `DateOnly` (hora local de Chile, sin hora).
- `PedidosWeb.Core`: modelos (`record` sellado), interfaces y repositorios, en carpetas por funcionalidad (`Pedidos/`, `Folios/`).
- `PedidosWeb.Api`: endpoints Minimal API y registro de dependencias en `Program.cs`; sin lógica de negocio.
- Namespaces file-scoped, `Nullable` e `ImplicitUsings` habilitados, clases de implementación `sealed`.
- Las convenciones de este repo prevalecen sobre cualquier regla global del agente.

## Tests
- xUnit 2.9 en `tests/PedidosWeb.Core.Tests`, con las mismas carpetas por funcionalidad que Core.
- Clase `[Clase]Tests`; método `Metodo_Escenario_Resultado` (ej.: `ObtenerPorId_IdInexistente_DevuelveNull`).
- `[Theory]` con `[InlineData]` para variaciones de un mismo caso.
- Se ejecutan con `dotnet test` siempre antes de un commit.

## Seguridad
- Nunca secretos, claves, tokens ni cadenas de conexión en el código ni en el repo (`.gitignore` ya excluye `.env` y `*.pem`).

## Reglas de trabajo
- Toda funcionalidad parte de una spec aprobada en `specs/<historia>/spec.md`.
- Un test xUnit por criterio de aceptación.
- No hacer commit sin tests en verde.
- Sin preguntar: editar código en `src/` y tests en `tests/`; ejecutar `dotnet build`, `dotnet test` y `dotnet run`.
- Todo lo demás requiere aprobación previa, en particular editar `specs/` o `docs/` y agregar paquetes NuGet.
