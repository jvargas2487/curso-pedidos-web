# Modelo de ROI · piloto de Pedidos Web

> Plantilla de la Unidad 10. Complétala con Claude Code en las etapas 1 y 2 y revísala tú.
> Fuente de datos: `docs/roi/datos-piloto.csv` (equipo Pedidos Web de 6 devs; semanas 1-6 antes, 7-12 con IA; datos ficticios) y tus entregables de las unidades 5 a 9.
> Regla: ningún número sin su fuente o su supuesto. Si un dato no está, escribe FALTA.

**Autor:** Jeffry Vargas · **Fecha:** 2026-11-13 · **Horizonte:** 12 meses (50 semanas con beneficio; ver supuesto 7)

## 1. Datos (etapa 1)

Períodos: **antes** = semanas 1-6; **con IA** = semanas 7-12 (promedio de las 6); **estable** = semanas 9-12, cuando ya pasó el arranque (ver más abajo). Todas las cifras salen de `docs/roi/datos-piloto.csv`.

| Dato | Antes | Con IA (7-12) | Estable (9-12) | Fuente (archivo y semanas) |
|---|---|---|---|---|
| Despliegues por semana | 2,0 | 3,0 | 3,5 | CSV, `despliegues`: 12/6, 18/6 y 14/4 |
| Lead time (días) | 6,58 | 5,17 | 4,38 | CSV, `lead_time_dias`: promedio simple de las semanas 1-6, 7-12 y 9-12 |
| Change fail rate | 41,7 % | 33,3 % | 21,4 % | CSV, `cambios_con_falla` ÷ `despliegues`: 5/12, 6/18 y 3/14 |
| Tiempo de recuperación (horas por cambio fallido) | 6,0 | 4,0 | 3,3 | CSV, `horas_recuperacion` ÷ `cambios_con_falla`: 30/5, 24/6 y 10/3 |
| Defectos escapados por semana | 2,0 | 1,5 | 1,0 | CSV, `defectos_escapados`: 12/6, 9/6 y 4/4 |
| Horas de retrabajo por semana | 12,0 | 10,7 | 8,75 | CSV, `horas_retrabajo`: 72/6, 64/6 y 35/4 |
| Horas ahorradas por semana (equipo) | 0 | 8,5 | 11,0 | CSV, `horas_ahorradas_equipo`: 51/6 y 44/4. Son horas que el equipo reporta, no medidas |
| Deployment rework rate | FALTA | FALTA | FALTA | El CSV no trae la proporción de despliegues no planificados; solo horas de retrabajo |

Las cinco métricas DORA: throughput = despliegues, lead time y tiempo de recuperación; inestabilidad = change fail rate y deployment rework rate (esta última FALTA).

**Evidencia propia** (una línea por archivo: qué dato aporta):

- `docs/migracion/defectos-corregidos.md` (U5): la migración encontró un defecto de redondeo corregido (D1: 13 de 60 casos del golden master cambian en 1 o 2 pesos; la suma de la columna total sube 9 pesos) y un hallazgo sin corregir (H1: umbrales de descuento distintos entre pantalla y grabado, 13 pedidos reales en el límite). Aporta evidencia cualitativa de defectos detectados antes de producción; no trae horas ni costos, por eso no entra en las cuentas.
- `docs/calidad/registro-u6.md` (U6): 116 tests en verde al inicio y 126 al final; 5 tests nuevos (10 casos) y cada uno falló al menos una vez con un cambio hecho a propósito (7 cambios probados). Las etapas 2 y 3 (pipeline y falla de CI) siguen con marcadores sin completar: FALTA el enlace al run en verde, el PR y el diagnóstico de `@claude`.
- `docs/checklist-revision.md` (U7): checklist de 4 bloques (antes de pedir revisión, al revisar, con hallazgos de la IA, merge). La sección «Mi PR de HU-03» está sin completar: FALTA el enlace al PR y los hallazgos, y no hay dato de tiempos de revisión.
- `docs/gobernanza/politica-ia.md` (U8): 7 controles declarados; 4 probados el 13-nov-2026 (lectura de archivos de entorno, lectura de archivos de llaves, edición de `legado/` y hook `guardian.sh`) y 3 sin probar (las reglas `ask` de `curl`/`wget`/`WebFetch` y del golden master, y el hook de tests tras editar). Aporta el riesgo que se controla, no un ahorro.
- `docs/adopcion/plan-evolucion.md` (U9): Pedidos Web está en nivel 4 de madurez, con `tasa_aceptacion` entre 0,74 y 0,78 y 86 de 107 PR hechos con Claude Code (80 %); los otros tres equipos están en nivel 1 o 2. Es la base de la extensión a otros equipos.

**Lo que veo en las semanas 7 y 8:** el piloto empeora antes de mejorar. El lead time promedio es 6,75 días (igual que antes: 6,58), el change fail rate sube a 75 % (3 cambios fallidos en 4 despliegues; en la semana 7 fallaron los 2), el retrabajo sube a 14,5 h por semana (antes 12,0) y los defectos a 2,5 por semana. Es la curva de aprendizaje; por eso el promedio 7-12 es menos favorable que el 9-12, y el equipo reportó 3 y 4 horas ahorradas en esas semanas aunque el retrabajo medido subió.

## 2. Supuestos (etapa 2)

Tipos: **oficial** (publicado, con fecha) · **medido** (dato del piloto) · **estimado** (juicio del equipo, el más débil).

| # | Supuesto | Valor | Tipo | Fuente |
|---|---|---|---|---|
| 1 | Asiento | USD 25 por dev al mes (Team Standard, facturación mensual) | oficial | claude.com/pricing, consultado el 13-nov-2026 (con facturación anual cuesta USD 20; se usa el mayor) |
| 2 | Capacitación | 8 horas por dev, una vez | estimado | Juicio del equipo; no hay dato del piloto |
| 3 | Curva de adopción | 9 horas netas perdidas en las semanas 7-8: 5 de retrabajo (29 h frente a 24 h esperadas) y 4 de recuperación (14 h frente a 10 h esperadas) | medido | CSV, semanas 7-8 frente al promedio de las semanas 1-6 |
| 4 | Valor hora | USD 30 | estimado | Juicio del equipo; **FALTA el costo hora real de un dev**: debe reemplazarse por el dato de RR. HH. |
| 5 | Horas ahorradas | 8,5 h por semana el equipo (conservador) · 11,0 (optimista) | medido (reportado por el equipo) | CSV, `horas_ahorradas_equipo`, semanas 7-12 y 9-12 |
| 6 | Defectos evitados | 0,5 por semana (conservador) · 1,0 (optimista), costo de 2,5 horas por defecto | medido (cantidad) · estimado (costo) | CSV, `defectos_escapados`: 2,0 antes frente a 1,5 y 1,0. Las 2,5 h salen de 30 h de recuperación ÷ 12 defectos de las semanas 1-6; es una aproximación |
| 7 | Horizonte | 12 meses, con 50 semanas de beneficio (se restan 2 de arranque) | estimado | Juicio del equipo, tomando como arranque las semanas 7-8 |
| 8 | Tamaño del equipo | 6 devs | medido | `docs/roi/datos-piloto.csv` (encabezado de la plantilla) |

## 3. Costos

| Costo | Cálculo | USD |
|---|---|---|
| Asientos | 6 devs × USD 25 × 12 meses | 1.800 |
| Capacitación | 6 devs × 8 h × USD 30 | 1.440 |
| Curva de adopción | 9 h × USD 30 | 270 |
| **Total** | | **3.510** |

## 4. Beneficios y escenarios

ROI = (beneficio − costo) / costo

| | Conservador | Optimista |
|---|---|---|
| Horas ahorradas × valor hora | 8,5 h × 50 sem × USD 30 = 12.750 | 11,0 h × 50 sem × USD 30 = 16.500 |
| Defectos evitados × costo en producción | 0,5 × 50 sem × 2,5 h × USD 30 = 1.875 | 1,0 × 50 sem × 2,5 h × USD 30 = 3.750 |
| **Beneficio total** | **14.625** | **20.250** |
| **ROI** | (14.625 − 3.510) / 3.510 = **317 %** | (20.250 − 3.510) / 3.510 = **477 %** |

Conservador: promedio de las semanas 7-12, incluye el arranque. Optimista: el piloto se replica tal cual, con el ritmo de las semanas 9-12.

## 5. Sensibilidad

**Supuesto que más pesa:** las horas ahorradas × valor hora, que son el 87 % del beneficio conservador (12.750 de 14.625). Las dos son frágiles: las horas las reporta el propio equipo y el valor hora es un estimado.

| Si… | ROI |
|---|---|
| El ahorro es la mitad del conservador (4,25 h por semana; defectos igual) | (8.250 − 3.510) / 3.510 = **135 %** |
| El valor hora es la mitad (USD 15; cambian beneficios y costos de capacitación y curva) | (7.312,5 − 2.655) / 2.655 = **175 %** |
| El asiento es Premium (USD 125 al mes, oficial, claude.com/pricing, 13-nov-2026) en vez de Standard: costo 10.710 | (14.625 − 10.710) / 10.710 = **37 %** |

El ROI no se vuelve negativo en ninguno de estos casos, pero el último muestra que el tipo de asiento pesa más que el ahorro a la mitad.

## 6. Punto de equilibrio (Tu reto)

**Pregunta:** ¿cuántas horas ahorradas por dev al mes dejan el ROI en 0?

**Qué vale el ROI en el equilibrio:** 0 %, es decir beneficio = costo.

**Qué despejo y qué dejo fijo:** despejo las horas ahorradas; dejo fijos los costos (3.510), el valor hora (USD 30), el beneficio por defectos del escenario conservador (1.875), 6 devs y 12 meses.

**Despeje:**

1. Beneficio = costo → horas × 30 + 1.875 = 3.510.
2. Horas × 30 = 3.510 − 1.875 = 1.635.
3. Horas = 1.635 / 30 = 54,5 h en el año para el equipo.
4. Por dev al mes: 54,5 / (6 devs × 12 meses) = **0,76 h**.

**Comparación:** el piloto reporta 8,5 h por semana en el equipo, que son 5,9 h por dev al mes (8,5 × 50 / 12 / 6). El equilibrio es el 13 % de lo reportado.

**Si no cuento el beneficio por defectos:** horas = 3.510 / 30 = 117 h en el año → 1,63 h por dev al mes.

**Para la gerencia:** con ahorrar menos de una hora al mes por persona (0,76 h) el piloto se paga; el equipo reporta 5,9 h.

## 7. Limitaciones

- Los datos son ficticios y la muestra es pequeña: este ROI es un ejercicio de método.
- Las horas ahorradas las reporta el propio equipo y no están verificadas; el retrabajo medido baja menos (de 12,0 a 10,7 h por semana, o 8,75 h en las semanas 9-12) que lo reportado como ahorro.
- El valor hora (USD 30) y las horas de capacitación (8 h) son estimados; el valor hora debe reemplazarse por el costo real antes de presentar el informe.
- El beneficio por defectos puede solaparse con las horas ahorradas y su costo por defecto es una aproximación; ambos pesan poco (13 % del beneficio conservador).
- El costo solo incluye asientos Standard. No incluye uso por encima del cupo del asiento (créditos de uso) ni asientos Premium, que suben el costo (ver sensibilidad).
- Es el ROI de un equipo de 6 devs. La extensión a Facturacion, Bodega y Reportes no está calculada: no hay datos de su costo ni de su ahorro, y su nivel de madurez es menor (`docs/adopcion/plan-evolucion.md`).
- Los indicadores de inestabilidad mejoran, pero con 6 semanas por período y 5 o 6 cambios fallidos no se puede separar el efecto de la IA de otras causas (por ejemplo, más despliegues más pequeños).
