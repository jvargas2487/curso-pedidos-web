# HU-01 · Listar pedidos con filtros

Estado: borrador (pendiente de aprobación del PO)

## Historia de usuario
Como vendedor, quiero listar pedidos filtrando por cliente, estado y rango de fechas.

## Objetivo
Que el vendedor obtenga, con una llamada a `GET /api/pedidos`, los pedidos que cumplen los filtros que indique
(cliente, estado, fecha desde y fecha hasta), paginados y en un orden estable y predecible, para revisar la cartera
de un cliente o el avance de los pedidos en un periodo sin consultar uno por uno el detalle.

## Reglas de negocio
- RN-1. Endpoint `GET /api/pedidos`. Cada elemento es el pedido completo, con los mismos campos que `GET /api/pedidos/{id}`:
  `id`, `folio`, `clienteId`, `clienteNombre`, `fecha`, `estado`, `total`, `sucursal`.
- RN-2. Parámetros de consulta: `clienteId`, `estado`, `desde`, `hasta` y `pagina`. Todos opcionales.
  Sin filtros se listan todos los pedidos. Un parámetro vacío o con solo espacios se trata como no enviado.
- RN-3. Los filtros presentes se combinan con Y (AND): un pedido aparece solo si cumple todos.
- RN-4. Cliente: coincidencia exacta con `ClienteId`, sin distinguir mayúsculas (`c-001` = `C-001`). No se busca por
  `ClienteNombre`. Un `clienteId` que no existe no es error: devuelve 200 sin elementos.
- RN-5. Estado: solo se aceptan `Pendiente`, `Confirmado`, `Despachado`, `Entregado` y `Anulado`, sin distinguir
  mayúsculas. Cualquier otro valor es error 400.
- RN-6. Fechas en formato `yyyy-MM-dd`. Rango inclusivo: `desde` y `hasta` incluyen ese día. Se puede enviar solo uno
  de los dos. Una fecha con otro formato o inexistente es error 400. Una fecha vacía se trata como no enviada.
- RN-7. Si `desde` es posterior a `hasta`, es error 400, con el mensaje en las claves `desde` y `hasta`.
- RN-8. Orden: `Fecha` ascendente; a igual fecha, `Id` ascendente.
- RN-9. Paginación fija de 20 pedidos por página. `pagina` empieza en 1 y vale 1 si no se envía.
  `pagina` no numérica, menor que 1 o mayor que la última página es error 400. Si no hay coincidencias, `pagina=1`
  es válida y devuelve 200 sin elementos. El límite superior solo se valida cuando los filtros son válidos (sin ellos
  no se puede calcular la última página); con filtros inválidos, `pagina` solo se valida si es no numérica o menor que 1.
- RN-10. Respuesta 200: `{ "elementos": [...], "total": <coincidencias de todos los filtros>, "pagina": <página devuelta> }`.
- RN-11. Errores: 400 con `application/problem+json` (ProblemDetails de validación de ASP.NET Core), con `errors`
  agrupado por nombre de parámetro.

## Criterios de aceptación
Datos: `PedidoRepositorioEnMemoria` (45 pedidos, 2026-09-02 a 2026-10-31, sin fechas repetidas).
Tamaño de página: 20.
- [ ] CA-1. Sin parámetros: 200, `total` 45, `pagina` 1, 20 elementos; el primero es el Id 40 (2026-09-02) y el último el Id 43 (2026-09-28).
- [ ] CA-2. `pagina=3`: 200, `total` 45, 5 elementos en este orden: 44, 23, 2, 42, 21.
- [ ] CA-3. `clienteId=C-001`: 5 elementos en este orden: 40, 32, 24, 16, 8.
- [ ] CA-4. `clienteId=c-001` devuelve lo mismo que CA-3.
- [ ] CA-5. `estado=Pendiente`: 5 elementos en este orden: 36, 9, 45, 18, 27.
- [ ] CA-6. `estado=pendiente` devuelve lo mismo que CA-5.
- [ ] CA-7. `estado=Anulado`: 5 elementos (Ids 38, 11, 20, 29, 2).
- [ ] CA-8. `desde=2026-10-01&hasta=2026-10-31`: `total` 23 (todo octubre), 20 elementos en la página 1.
- [ ] CA-9. `desde=2026-09-02&hasta=2026-09-02`: solo el Id 40 (los extremos se incluyen).
- [ ] CA-10. Solo `desde=2026-10-31`: solo el Id 21. Solo `hasta=2026-09-02`: solo el Id 40.
- [ ] CA-11. `clienteId=C-002&estado=Despachado&desde=2026-10-01&hasta=2026-10-31`: 37, 21 (en ese orden), `total` 2.
- [ ] CA-12. `clienteId=C-002&desde=2026-09-01&hasta=2026-09-30`: 13, 5, 45 (en ese orden).
- [ ] CA-13. `clienteId=Ferretería El Clavo`: 200, `total` 0, sin elementos (no se busca por nombre).
- [ ] CA-14. `clienteId=C-999`: 200, `total` 0, `pagina` 1, sin elementos.
- [ ] CA-15. `estado=Cancelado`: 400, `application/problem+json`, `errors` con la clave `estado`.
- [ ] CA-16. `desde=2026-13-01`: 400, `application/problem+json`, `errors` con la clave `desde`.
- [ ] CA-17. `desde=2026-10-31&hasta=2026-10-01`: 400, `application/problem+json`, `errors` con las claves `desde` y `hasta`.
- [ ] CA-18. `pagina=0`: 400, `application/problem+json`, `errors` con la clave `pagina`.
- [ ] CA-19. `pagina=4` (sin filtros, la última página es la 3): 400, `application/problem+json`, `errors` con la clave `pagina`.
- [ ] CA-20. `clienteId=C-999&pagina=2`: 400 (con 0 coincidencias solo la página 1 es válida).
- [ ] CA-21. `pagina=abc`: 400, `application/problem+json`, `errors` con la clave `pagina`.
- [ ] CA-22. Todos los parámetros vacíos o con solo espacios: 200, `total` 45 (igual que sin parámetros).

## Ejemplos de entrada y salida
Entrada: `GET /api/pedidos?clienteId=C-002&estado=Despachado&desde=2026-10-01&hasta=2026-10-31`
Salida: `200 OK`

    {
      "elementos": [
        { "id": 37, "folio": "PED-ANT-000037", "clienteId": "C-002", "clienteNombre": "Minimarket Doña Rosa",
          "fecha": "2026-10-07", "estado": "Despachado", "total": 56650, "sucursal": "ANT" },
        { "id": 21, "folio": "PED-ANT-000021", "clienteId": "C-002", "clienteNombre": "Minimarket Doña Rosa",
          "fecha": "2026-10-31", "estado": "Despachado", "total": 75450, "sucursal": "ANT" }
      ],
      "total": 2,
      "pagina": 1
    }

Entrada: `GET /api/pedidos?clienteId=C-999`
Salida: `200 OK`

    { "elementos": [], "total": 0, "pagina": 1 }

Entrada: `GET /api/pedidos?estado=Cancelado`
Salida: `400 Bad Request`, `Content-Type: application/problem+json` (el texto del mensaje lo fija la implementación)

    {
      "title": "One or more validation errors occurred.",
      "status": 400,
      "errors": { "estado": [ "<mensaje de estado no válido>" ] }
    }

## Errores esperados
Todos responden 400 con ProblemDetails (RN-11):
- `estado` fuera de la lista válida (RN-5).
- `desde` o `hasta` con formato distinto de `yyyy-MM-dd` o fecha inexistente (RN-6).
- `desde` posterior a `hasta` (RN-7).
- `pagina` no numérica, menor que 1 o mayor que la última página (RN-9).

No es error: `clienteId` inexistente (200 sin elementos, RN-4).

## Fuera de alcance
- Filtros no pedidos en la historia: sucursal, folio, total y nombre de cliente (descartado en RN-4).
- Elegir el orden o el tamaño de página desde la petición (son fijos: RN-8 y RN-9).
- Crear, modificar o anular pedidos.
- Validar `clienteId` contra un maestro de clientes.
- Base de datos: los datos siguen en memoria (PostgreSQL llega en una unidad posterior, según README).

## Preguntas abiertas
Ninguna. Decisión para el plan: los criterios se prueban en Core (`tests/PedidosWeb.Core.Tests`) sobre el servicio
de listado; el mapeo HTTP (200/400, `application/problem+json`) se verifica a mano. Sin proyecto de tests nuevo ni NuGet.
