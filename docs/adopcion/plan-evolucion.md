# Plan de evolución de la IA para una Business Unit

> Fuente de datos: `docs/adopcion/analytics-ficticio.csv` (4 equipos × 8 semanas, datos ficticios). Las cifras del diagnóstico salen de ese archivo; las metas y fechas de las secciones 3 y 5 son propuestas por validar con cada equipo.

**BU:** Desarrollo de la distribuidora · **Autor:** Jeffry Vargas · **Fecha:** 2026-11-13

## 1. Resumen para la gerencia

Pedidos Web es el único equipo con un proceso AI-native (nivel 4): acepta entre 74 % y 78 % de lo que sugiere la IA y 80 % de sus PR se hacen con Claude Code. Bodega usa la IA todas las semanas (7 personas, unas 115 sesiones) pero acepta solo entre 11 % y 17 %, y Reportes la usa de forma intermitente; ambos están en nivel 2. Facturacion casi no la usa (nivel 1).

Proponemos que los otros tres equipos adopten la forma de trabajar de Pedidos Web instalando el plugin `pedidos-playbook` con un comando: revisor de cambios, documentación de módulos, planificador de proyectos, guardián de secretos, plantilla de AGENTS.md y acceso a Claude Academy. El avance se mide cada semana con los mismos indicadores del CSV.

## 2. Diagnóstico por equipo

Modelo de madurez del curso: 1 Uso a escondidas · 2 Uso individual · 3 Equipo con reglas · 4 Proceso AI-native · 5 Organización que mide y mejora.

| Equipo | Nivel | Evidencia 1 (indicador del CSV) | Evidencia 2 (indicador del CSV) |
|---|---|---|---|
| Pedidos Web | 4 · Proceso AI-native | `tasa_aceptacion` entre 0.74 y 0.78 en las 8 semanas, con 5 o 6 `usuarios_activos` cada semana | 86 de 107 PR con Claude Code (80 %) y 84 sesiones por semana en promedio |
| Facturacion | 1 · Uso a escondidas | 0 o 1 `usuarios_activos` por semana y 15 `sesiones` en total en 8 semanas (2 semanas sin ninguna) | 1 PR con Claude Code de 71 (1 %) y 200 `lineas_aceptadas` en total |
| Bodega | 2 · Uso individual | 7 `usuarios_activos` y 115 `sesiones` por semana en promedio, el mayor uso de la BU | `tasa_aceptacion` entre 0.11 y 0.17 (la más baja) y 30 de 109 PR con Claude Code (28 %) |
| Reportes | 2 · Uso individual | Uso intermitente: 3 o 4 `usuarios_activos` en las semanas 1, 3, 5 y 7, y 0 o 1 en las semanas 2, 4, 6 y 8 | `tasa_aceptacion` entre 0.50 y 0.58 cuando hay sesiones y 18 de 49 PR con Claude Code (37 %) |

> Ningún indicador solo: mira el uso, la tasa de aceptación y los PR juntos. Una celda vacía en `tasa_aceptacion` significa que esa semana no hubo sesiones.
>
> Bodega es el caso a vigilar: mucho uso con poca aceptación es una señal de que la herramienta se usa sin reglas ni contexto, no un éxito. Facturacion y Reportes tienen poco uso, pero cuando lo usan aceptan más de la mitad de lo sugerido (0.55 a 0.65 y 0.50 a 0.58).

## 3. Objetivo por equipo para el próximo trimestre

| Equipo | Objetivo medible (propuesta) | Indicador que lo muestra |
|---|---|---|
| Pedidos Web | Mantener el nivel 4 y llegar a 90 % de PR con Claude Code (hoy 80 %) | `pr_con_cc` / `pr_totales` y `tasa_aceptacion` sin bajar de 0.74 |
| Facturacion | Pasar de nivel 1 a 2: al menos 3 usuarios activos y ninguna semana sin sesiones | `usuarios_activos` y `sesiones` semanales (hoy 0 o 1 usuarios y 2 semanas sin sesiones) |
| Bodega | Subir la tasa de aceptación de 0.14 a 0.40 manteniendo los 7 usuarios | `tasa_aceptacion` y `lineas_aceptadas` (hoy 0.11 a 0.17) |
| Reportes | Uso estable: al menos 3 usuarios activos todas las semanas | `usuarios_activos` (hoy 0 o 1 en las semanas 2, 4, 6 y 8) |

## 4. El playbook

`pedidos-playbook` es un plugin de Claude Code que empaqueta la forma de trabajar de Pedidos Web. Cada pieza resuelve un problema distinto:

| Pieza | Para qué sirve |
|---|---|
| Subagente `revisor` | Revisa en solo lectura los cambios de una rama contra las reglas del repo antes de abrir un PR. |
| Skill `documentar-modulo` | Escribe la documentación de un módulo a partir de su código y sus tests. |
| Skill `planificador-proyectos-web` | Planifica y documenta un proyecto web por iteraciones y con especificaciones. |
| Guardián (`guardian.py`) | Frena toda acción que nombre archivos de secretos o de claves. |
| Plantilla de AGENTS.md | Punto de partida para que cada equipo escriba las reglas de su repo. |
| MCP `claude-academy` | Consulta en solo lectura el material de Claude Academy desde Claude Code. |

**Cómo lo instala otro equipo** (desde su repo, con Claude Code):

```
/plugin marketplace add jvargas2487/curso-pedidos-web
/plugin install pedidos-playbook@pedidos-jvargas2487
```

Después, `/mcp` debe mostrar `claude-academy`. Para recibir una versión nueva se actualiza el marketplace y luego el plugin; el plugin instalado es una copia y no cambia hasta que se publica una versión nueva. Las reglas del repo (AGENTS.md, REVIEW.md y permisos) no viajan en el plugin: cada equipo parte de la plantilla y las adapta.

## 5. Hoja de ruta a 6 y 12 meses

| Hito | Mes | Equipo | Champion | Cómo sabemos que se cumplió |
|---|---|---|---|---|
| Designar un champion por equipo | 1 | Facturacion, Bodega, Reportes | PENDIENTE: lo designa la gerencia | Hay un nombre por equipo |
| Instalar `pedidos-playbook` | 1 | Facturacion, Bodega, Reportes | PENDIENTE | El plugin figura instalado y `claude-academy` aparece en `/mcp` |
| Escribir el AGENTS.md de cada equipo desde la plantilla | 2 | Facturacion, Bodega, Reportes | PENDIENTE | El archivo está en el repo y fue revisado por el champion |
| Medir la línea base semanal con los 4 indicadores | 3 | Todos | PENDIENTE | Hay 4 semanas seguidas de datos en el mismo formato del CSV |
| Bodega: primer ajuste de reglas y contexto | 4 | Bodega | PENDIENTE | La `tasa_aceptacion` sube respecto de 0.14 durante 4 semanas |
| Facturacion y Reportes: uso estable | 6 | Facturacion, Reportes | PENDIENTE | Ninguna semana sin sesiones y 3 o más `usuarios_activos` |
| Revisar objetivos del trimestre y subir de nivel donde corresponda | 6 | Todos | PENDIENTE | Cada equipo tiene su nivel actualizado con evidencia del CSV |
| Convertir el revisor en paso previo de todo PR | 9 | Todos | PENDIENTE | `pr_con_cc` sobre `pr_totales` sube respecto de la línea base |
| Publicar versiones nuevas del plugin con lo aprendido | 12 | Pedidos Web | PENDIENTE | Los 3 equipos reciben la versión nueva y la usan |

El ciclo se repite cada trimestre: **estandarizar** (el playbook y el AGENTS.md), **medir** (los indicadores del CSV cada semana), **ajustar** (reglas, contexto y capacitación donde los números no mejoran) y **escalar** (llevar lo que funcionó al resto de la BU).

## 6. Indicadores que vamos a seguir

Los mismos indicadores del CSV, por equipo y por semana:

| Indicador | Qué muestra | Cada cuánto | Quién lo revisa |
|---|---|---|---|
| `usuarios_activos` y `sesiones` | Si el equipo usa la IA de forma estable | Semanal | PENDIENTE: champion de cada equipo |
| `tasa_aceptacion` | Si lo que sugiere la IA sirve | Semanal | PENDIENTE: champion de cada equipo |
| `pr_con_cc` sobre `pr_totales` | Si el trabajo real pasa por Claude Code | Semanal | PENDIENTE: champion de cada equipo |
| Nivel de madurez (1 a 5) | La lectura conjunta de los anteriores | Trimestral | PENDIENTE: gerencia de la BU |

Ningún indicador se lee solo: mucho uso con poca aceptación, como en Bodega, es una señal de alerta y no un éxito.

## 7. Riesgos y próximos pasos

- Bodega usa mucho la IA pero acepta poco; sin cambios, el equipo puede frustrarse y abandonarla → partir de la plantilla de AGENTS.md con sus reglas y revisar sus indicadores cada semana.
- Facturacion usa la IA a escondidas y sin reglas, y hay semanas sin ninguna sesión → asignar un champion y fijar un uso mínimo semanal.
- Reportes depende de pocas personas (0 o 1 usuarios en las semanas pares) → ampliar el grupo que lo usa y volver a medir.
- Los datos son 8 semanas de un CSV ficticio; los niveles pueden cambiar con más historia → recalcular cada trimestre.
- El plugin instalado es una copia y puede quedar desactualizado → publicar cada cambio como versión nueva y pedir la actualización a los equipos.
- El guardián necesita Python 3 en cada equipo → comprobarlo al instalar el plugin.

**Próximos pasos:** designar a los champions, instalar el playbook en los 3 equipos y fijar la fecha de la primera medición.
