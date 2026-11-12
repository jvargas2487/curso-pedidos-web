# REVIEW.md · Reglas de revisión de pedidos-web

Las leen el subagente `revisor` y el workflow `claude-review.yml`. Siempre se leen desde `main`:
un PR no puede aflojar las reglas que lo revisan.

## Qué es Importante
Reporta como Importante solo lo que puede causar un error real o romper una regla del repo:
- Cálculos de dinero con `double` o `float`, o con decimales en CLP (debe ser `decimal`, sin decimales).
- Fechas de pedido que no sean `DateOnly`, o rangos de fechas con límites mal tratados.
- Lógica de negocio en `PedidosWeb.Api` (debe vivir en `PedidosWeb.Core`).
- Un criterio de aceptación de la spec sin su test xUnit.
- Entradas inválidas sin validar (por ejemplo `desde` mayor que `hasta`) o excepciones sin mensaje claro.

## Revisar siempre
1. Que el cambio cumpla cada criterio de aceptación de su `specs/<historia>/spec.md` y tenga un test por criterio.
2. Que no haya secretos, claves, tokens ni cadenas de conexión en código, workflows o documentos.
3. Que se respete `AGENTS.md`: nombres en español, `DateOnly`, `decimal`, namespaces file-scoped, clases `sealed`.

## No reportar
- Estilo, formato o nombres que el compilador o el editor ya resuelven.
- Código que el PR no tocó.
- Preferencias personales sin una regla escrita detrás.
- Sugerencias de nuevas funcionalidades o paquetes fuera de la spec.

## Tope de detalles
- Máximo 5 hallazgos por revisión, ordenados por importancia.
- Cada hallazgo en una o dos frases, con `archivo:línea` y por qué importa.
- Si no hay nada Importante, dilo en una línea; no inventes hallazgos.
