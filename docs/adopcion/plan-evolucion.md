# Plan de evolución de la IA para una Business Unit

> Plantilla de la Unidad 9. Complétala con Claude Code en la etapa 1 y revísala tú. En la etapa 3 la conviertes en PDF con tu Skill `pdf-con-estilo`.
> Fuente de datos: `docs/adopcion/analytics-ficticio.csv` (4 equipos × 8 semanas, datos ficticios). No uses cifras que no estén en el CSV.

**BU:** Desarrollo de la distribuidora · **Autor:** Jeffry Vargas · **Fecha:** 2026-11-13

## 1. Resumen para la gerencia

<3 a 5 líneas: dónde está hoy la BU, qué propones y qué se instala.>

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

| Equipo | Objetivo medible | Indicador que lo muestra |
|---|---|---|
| | | |

## 4. El playbook

<Qué trae `pedidos-playbook` y para qué sirve cada pieza. Cómo lo instala otro equipo (los comandos están en el README del taller).>

## 5. Hoja de ruta a 6 y 12 meses

| Hito | Mes | Equipo | Champion | Cómo sabemos que se cumplió |
|---|---|---|---|---|
| | | | | |

<El ciclo: estandarizar → medir → ajustar → escalar.>

## 6. Indicadores que vamos a seguir

<Cuáles, cada cuánto y quién los revisa.>

## 7. Riesgos y próximos pasos

- <riesgo> → <cómo lo mitigamos>
