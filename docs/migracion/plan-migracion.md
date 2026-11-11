# Plan de migración · Pedidos Legado → Pedidos Web

## 1. Etapas
Un módulo a la vez (strangler fig), en el orden de `docs/diagnostico/matriz-decision.md`:
1. Cálculo de totales en C# (`src/PedidosWeb.Core/Totales/`): hecho en la Unidad 5, con 60 casos de golden master y R05 corregido.
2. Toma y grabado de pedidos: Modernizar (3,70). Transacción única y consultas con parámetros (RS-04, RS-06), reutilizando `CalculoTotales`; incluye pedido mínimo, bloqueo por deuda y cierre de las 18:00.
3. Stock: Modernizar (3,65). Conservar el -20 para VIP y hacerlo atómico (RS-02, RS-03).
4. Clientes y productos: Reemplazar (3,80), con un CRUD estándar y un solo validador de RUT (RS-07).
5. Informes: Reemplazar al final (3,45), cuando exista la nueva fuente de datos; hoy usa 0,19 fijo (RS-05).

## 2. Convivencia
- El legado sigue siendo la fuente de verdad de cada módulo hasta su corte; el portal calcula en paralelo y los resultados se comparan pedido a pedido.
- Para cada etapa se crea antes un golden master (como `migracion/golden-master.csv`) con lo que calcula el legado, y los tests del portal lo leen.
- Diferencias conocidas y aceptadas: D1 (R05, 1 o 2 pesos por pedido) y H1 (umbrales `>` / `>=`), ambas en `docs/migracion/defectos-corregidos.md`.

## 3. Riesgos
| Riesgo | Mitigación |
|---|---|
| Perder reglas implícitas del legado (pedido mínimo, bloqueo por deuda, cierre de las 18:00) | Inventario en `docs/diagnostico/reglas-negocio.md`; un caso de golden master por regla antes de migrar el módulo. |
| El negocio no decide H1 y portal y legado graban totales distintos con base de 500.000 o 1.000.000 | Comercial y Contabilidad deciden antes del corte de pedidos; hasta entonces el portal replica la pantalla y se documenta. |
| Diferencias de 1 o 2 pesos por R05 al comparar con el legado | Conciliar por pedido según D1; el criterio de comparación es «igual salvo los casos de D1». |
| El legado usa `Double` y el portal `decimal`: pueden diferir en casos límite | El golden master es la autoridad; cualquier caso nuevo con diferencia se registra como defecto u hallazgo. |
| Pedidos parciales o stock negativo al migrar datos (RS-03, RS-04) | Validar la carga con conteos y sumas, y revisar los datos negativos antes de importar. |

## 4. Criterio de corte
- Se apaga el legado de un módulo cuando sus tests del golden master están en verde, la comparación en paralelo no muestra diferencias fuera de las aceptadas durante un ciclo completo de facturación y Contabilidad lo aprueba.
- Vuelta atrás: el legado se mantiene intacto y operativo hasta el cierre de la etapa siguiente, de modo que se puede volver a él sin migrar datos de regreso.
