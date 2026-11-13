# Plan integrado de adopción de IA · Pedidos Web

**Para:** Gerencia general · **Autor:** Jeffry Vargas · **Fecha:** 2026-11-13

## Informe ejecutivo

### 1. Problema
Terminó el piloto de 12 semanas del equipo Pedidos Web. La gerencia debe decidir si mantiene esta forma de trabajar y si la lleva a los otros tres equipos de desarrollo, que hoy usan la IA poco o sin reglas.

### 2. Qué se hizo
Seis desarrolladores trabajaron 6 semanas como siempre y 6 semanas con Claude Code, con revisión de cambios, controles de seguridad y reglas escritas. Se compararon despliegues, plazos, fallas y horas.

### 3. Resultados
- **Plazo de entrega:** de 6,6 a 4,4 días (semanas 9-12 frente a 1-6). **Cambios con falla:** de 42 % a 21 %. **Defectos en producción:** de 2,0 a 1,0 por semana.
- **Las dos primeras semanas fueron peores:** más fallas y más retrabajo mientras el equipo aprendía.
- **ROI a 12 meses: entre 317 % y 477 %** (costo total USD 3.510). Si el ahorro es la mitad, 135 %; con asientos Premium en vez de Standard, 37 %. No se vuelve negativo en ninguno.
- **Qué lo mueve:** las horas ahorradas, que reporta el propio equipo, y el valor hora (USD 30, estimado). Los datos son ficticios y la muestra es pequeña.

### 4. Recomendación
Aprobar 12 meses de continuidad de Pedidos Web (6 asientos, USD 3.510) y extender la forma de trabajar a Facturacion, Bodega y Reportes por fases, con revisión del ROI en el mes 6.

### 5. Próximo paso
La gerencia general decide antes del 30-nov-2026 (fecha propuesta) y designa un responsable por equipo; Jeffry Vargas instala el playbook en los tres equipos durante el primer mes.

*Preparado con apoyo de IA; cifras verificadas por Jeffry Vargas.*

<div style="page-break-before: always"></div>

## Anexo

No repite los entregables del curso: los cita.

### Roadmap
- **Mes 1:** designar un responsable por equipo e instalar el playbook en Facturacion, Bodega y Reportes.
- **Mes 3:** 4 semanas seguidas de indicadores en el mismo formato.
- **Mes 6:** revisar objetivos y el ROI; subir de nivel donde corresponda.
- **Mes 9:** el revisor de cambios como paso previo de todo PR.
- **Mes 12:** publicar versiones nuevas del playbook con lo aprendido.

Respaldo: `docs/adopcion/plan-evolucion.md` (U9)

### Indicadores
- **Entrega y estabilidad:** despliegues por semana, lead time, change fail rate y tiempo de recuperación. Meta propuesta: no empeorar los valores de las semanas 9-12 (3,5 despliegues, 4,4 días, 21 %, 3,3 h).
- **Uso de la IA:** usuarios activos, sesiones, tasa de aceptación y PR hechos con Claude Code. Meta de Pedidos Web: 90 % de PR (hoy 80 %) con aceptación sobre 0,74.
- **Calidad:** 126 tests en verde y checklist de revisión en cada PR.
- **Falta medir:** deployment rework rate, que el piloto no registró.

Respaldo: `docs/calidad/registro-u6.md` (U6) · `docs/checklist-revision.md` (U7)

### Gobernanza
Se llevan a los otros equipos el guardián de secretos, los permisos y la plantilla de AGENTS.md dentro del playbook, y la línea «Uso de IA» en cada PR. De 7 controles, 4 se probaron intentando violarlos y 3 no; hay que probar esos 3 antes de extenderlos.

Respaldo: `docs/gobernanza/politica-ia.md` (U8)

### ROI
| | Conservador | Optimista |
|---|---|---|
| Costo (asientos 1.800 + capacitación 1.440 + curva 270) | USD 3.510 | USD 3.510 |
| Beneficio (horas ahorradas + defectos evitados) | USD 14.625 | USD 20.250 |
| **ROI** | **317 %** | **477 %** |

- **Supuestos clave:** asiento USD 25 al mes (oficial, claude.com/pricing, 13-nov-2026); valor hora USD 30 y capacitación de 8 h por persona (estimados); horas ahorradas 8,5 y 11,0 por semana del equipo (reportadas por el equipo); 50 semanas de beneficio.
- **Sensibilidad:** ahorro a la mitad, 135 %; valor hora a la mitad, 175 %; asiento Premium, 37 %.
- **No incluye** uso por encima del cupo del asiento ni el costo de los otros equipos.

Respaldo: `docs/roi/modelo-roi.md` · `docs/migracion/defectos-corregidos.md` (U5)

### Punto de equilibrio
El piloto se paga si cada persona ahorra **0,76 horas al mes** (con costos, valor hora y beneficio por defectos del escenario conservador fijos). El equipo reporta 5,9 horas al mes por persona. Sin contar el beneficio por defectos, el equilibrio es 1,63 horas.

Respaldo: `docs/roi/modelo-roi.md`, sección 6

### Evolución
Pedidos Web mantiene el nivel 4 (proceso AI-native). Metas del próximo trimestre: Facturacion de nivel 1 a 2 (3 usuarios activos o más), Bodega sube la aceptación de 0,14 a 0,40 con sus 7 usuarios y Reportes llega a 3 usuarios activos todas las semanas.

Respaldo: `docs/adopcion/plan-evolucion.md` (U9)
