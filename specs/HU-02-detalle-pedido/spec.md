# HU-02 · Detalle de un pedido

**Historia:** Como vendedor, quiero ver el detalle de un pedido (cliente, líneas, total y estado), para responderle al cliente sin entrar al sistema legado.

## Qué devuelve

La ruta `GET /api/pedidos/{id}/detalle` devuelve:

- Del pedido: `id`, `folio`, `fecha`, `estado` y `sucursal`.
- Del cliente: `clienteId` y `clienteNombre`.
- De cada línea: `numero`, `producto`, `cantidad`, `precioUnitario` y `subtotal` (`cantidad × precioUnitario`), ordenadas por `numero`.
- Total: `total`, la suma de los subtotales de las líneas.

Los montos son pesos chilenos (CLP), `decimal` sin decimales, **sin IVA**.

## Criterios de aceptación

Cada criterio se puede comprobar con un sí o un no.

- **CA-01** · Dado el pedido 8, cuando calculo su total desde sus líneas, entonces el `total` es 120100 y es igual a la suma de los subtotales de sus 3 líneas.
- **CA-02** · Dado el pedido 8, cuando pido su detalle, entonces trae el folio `PED-STGO-000008`, el estado `Entregado`, el cliente `C-001` (`Ferretería El Clavo`) y sus 3 líneas ordenadas por número.
- **CA-03** · Dado un id que no existe (999), cuando pido su detalle, entonces no hay detalle y la API responde 404.

## Si el pedido no existe

La API responde `404 Not Found`, sin cuerpo, igual que `GET /api/pedidos/{id}`.

## Restricciones

- La ruta `GET /api/pedidos/{id}` no cambia.
- La lógica (total y armado del detalle) vive en `PedidosWeb.Core`, con tests xUnit. La API solo la expone.
- Se construye en 3 incrementos (total, detalle, ruta), con un commit por incremento.

## Fuera de alcance

- IVA, descuentos y recargos (llegan con la migración del cálculo de totales, en la Unidad 5).
- Formato de moneda para mostrar en pantalla.
- Tests de integración HTTP: el incremento 3 se verifica en el navegador.
