# Matriz de decisión de Pedidos Legado

<!--
Etapa 3 · Decidir (modo por defecto).
1. Reparte los pesos entre los 5 criterios. Deben sumar 100 %.
2. Puntúa cada estrategia de 1 a 5 para cada módulo (5 = la mejor opción para ese criterio)
   y calcula el puntaje ponderado. Pídele a Claude que haga las cuentas, y revísalas.
3. La recomendación de cada módulo debe salir de la matriz, no de la intuición.
Punto de control E3: pesos que suman 100 %, al menos 4 módulos puntuados,
recomendación que se desprende de la matriz y resumen de 5 líneas.
-->

## Pesos

Aceptados por el responsable del taller a partir de la propuesta.

| Criterio | Qué mide | Peso |
|---|---|---|
| Costo | Cuánto cuesta en horas y dinero | 20 % |
| Complejidad | Qué tan difícil es técnicamente | 15 % |
| Riesgo | Qué puede salir mal: el que deja el legado más el del propio cambio | 30 % |
| Impacto | Cuánto valor entrega al negocio | 25 % |
| Personas | Conocimiento disponible en el equipo | 10 % |
| **Total** |  | **100 %** |

Puntajes de 1 a 5, donde 5 es la mejor opción para ese criterio (en Costo, la más barata; en Complejidad, la más simple; en Riesgo, la que deja menos riesgo).

Estrategias:
- **Mantener:** dejar el módulo como está.
- **Refactorizar:** ordenarlo y corregirlo dentro de Delphi.
- **Modernizar:** llevar su comportamiento actual a .NET (`PedidosWeb.Core`), conservando las reglas.
- **Reemplazar:** rediseñarlo desde cero, sin copiar el comportamiento actual.

## Puntajes por criterio

Orden de cada celda: Costo / Complejidad / Riesgo / Impacto / Personas. Son una propuesta de análisis; con otros puntajes cambia el resultado.

| Módulo (archivos) | Mantener | Refactorizar | Modernizar | Reemplazar |
|---|---|---|---|---|
| Cálculo de totales (`UCalculoTotales`, redondeo de `UUtilidades`, bloque de cálculo de `UFrmPedidos`) | 5/5/1/1/2 | 4/4/3/3/1 | 4/4/4/5/4 | 3/3/3/4/4 |
| Toma y grabado de pedidos (`UFrmPedidos`) | 5/5/1/1/2 | 3/2/2/3/1 | 2/3/4/5/4 | 1/2/3/5/4 |
| Clientes y productos (`UFrmClientes`, `UFrmProductos`) | 5/5/2/1/2 | 4/4/3/2/1 | 2/3/3/3/4 | 3/4/4/4/4 |
| Stock (`UStock`) | 5/5/1/1/2 | 4/3/2/3/1 | 3/3/4/4/4 | 2/2/3/4/4 |
| Informes (`UFrmInformes`) | 5/5/3/2/2 | 4/4/3/3/1 | 2/3/4/3/4 | 2/3/4/4/4 |

Por qué:
- **Cálculo de totales:** son 147 líneas de funciones puras, con reglas claras y duplicadas (RS-01). Llevarlas a .NET es barato, se prueba con xUnit y deja una sola implementación. Mantener deja la diferencia `>` frente a `>=` (reglas 1 y 2).
- **Toma y grabado de pedidos:** mezcla pantalla, SQL concatenado y reglas en 420 líneas, y graba sin transacción (RS-04, RS-06). Refactorizar en Delphi no resuelve eso y exige conocimiento que el equipo no tiene. Reemplazar es lo más caro y se arriesga a perder reglas implícitas (pedido mínimo, bloqueo por deuda, cierre de las 18:00), por eso Modernizar puntúa mejor.
- **Clientes y productos:** son mantenedores sobre grillas con poca lógica de negocio; lo único valioso es el RUT, hoy con dos algoritmos que se contradicen (RS-07). Conviene rehacerlos con un CRUD estándar y un solo validador, sin copiar el defecto.
- **Stock:** `MOV_STOCK` no existe (RS-02), hay stock negativo (RS-03) y la regla del -20 para VIP es de negocio. Hay que conservar la regla y hacerla atómica, así que Modernizar.
- **Informes:** es pequeño y poco crítico, pero lee la base del legado y usa 0,19 fijo (RS-05). Cualquier estrategia distinta de Mantener necesita antes la nueva fuente de datos, lo que sube su costo y complejidad.

## Matriz

Puntaje ponderado = suma de (puntaje × peso). Máximo posible: 5,00.

| Módulo | Mantener | Refactorizar | Modernizar | Reemplazar | Recomendación |
|---|---|---|---|---|---|
| Cálculo de totales | 2,50 | 3,15 | **4,25** | 3,35 | Modernizar |
| Toma y grabado de pedidos | 2,50 | 2,35 | **3,70** | 3,05 | Modernizar |
| Clientes y productos | 2,80 | 2,90 | 2,90 | **3,80** | Reemplazar |
| Stock | 2,50 | 2,70 | **3,65** | 3,00 | Modernizar |
| Informes | 3,35 | 3,15 | 3,20 | **3,45** | Reemplazar, al final (margen de solo 0,10 sobre Mantener) |

Mantener nunca gana: en cuatro módulos queda como la peor o la segunda peor, porque obtiene 1 en Impacto y en Riesgo. En Informes la ventaja es estrecha, así que ahí la decisión depende del orden de trabajo y no solo del puntaje.

## Resumen ejecutivo

<!-- Exactamente 5 líneas, para alguien que no leerá el resto. -->

1. **Qué es y su estado:** Pedidos Legado es el programa Delphi que toma, graba y reporta los pedidos de la distribuidora, con 16 archivos y unas 2.400 líneas, y su documentación actual está desactualizada (`legado/README.md:5-7`).
2. **Hallazgo principal (con evidencia):** el total que ve el vendedor puede diferir del que se graba, porque el descuento se calcula dos veces con umbrales distintos (`legado/src/UCalculoTotales.pas:77` y `legado/src/UFrmPedidos.pas:298`), y 13 pedidos reales caen justo en ese límite.
3. **Riesgo más urgente:** el pedido se graba sin transacción y con SQL armado a mano (`legado/src/UFrmPedidos.pas:255`, `:262`), así que una falla deja pedidos incompletos y una comilla en una observación puede romper o abusar del sistema.
4. **Estrategia y por dónde empezar:** llevar a .NET el cálculo de totales, los pedidos y el stock, y rehacer clientes, productos e informes; se empieza por el cálculo de totales (puntaje 4,25), que es chico, claro y fácil de probar.
5. **Próximo paso que se pide aprobar:** aprobar que el cálculo de totales sea el primer módulo, y que el negocio defina si el límite de 500.000 y 1.000.000 lleva descuento (hoy el grabado sí y la pantalla no).
