# Registro de calidad · Unidad 6

<!--
Lo completas tú durante el taller. Frases cortas: lo que hiciste y por qué.
Borra estos comentarios al terminar.
-->

## Preparación

- **Tests en `Skip` (pendientes de unidades anteriores):** ninguno (116 tests en verde, 0 omitidos).

## Etapa 1 · Tests que sí prueban

- **Módulo elegido:** CalculoTotales.
- **Tests que propuso Claude:** 5 (10 casos con `[Theory]`), los que el golden master no cubría.
- **Cambio que hice a mano para probarlos:** los cambios en `CalculoTotales.cs` los aplicó Claude con un script, a petición mía (no los hice yo en Rider), y los revirtió después de cada prueba. Cada fila indica el test que falló:

  | Test | Cambio en `CalculoTotales.cs` | Falló |
  |---|---|---|
  | `PedidoSinLineas` | quitar `lineas.Count == 0 ? 0m :` (línea 36) | sí, región 5 y 11 |
  | `TasaIvaDistintaDe19` | `tasaIva` fija en `0.19m` (línea 37) | sí, tasa 0 y 0,10 |
  | `CategoriaOfeConMinusculasOEspacios` | quitar `.Trim()` (línea 53) | sí, `" OFE "` |
  | `CategoriaOfeConMinusculasOEspacios` | `OrdinalIgnoreCase` por `Ordinal` (línea 53) | sí, `"ofe"` y `"Ofe"` |
  | `ClienteVipConSoloLineasOfe` | `if (esVip && porcentaje > 0)` (línea 61) | sí; el golden master también falló (casos 11, 23, 38 y 52) |
  | `BaseUnPesoSobreElUmbral` | `UmbralDescuentoMedio` en `500_002m` (línea 11) | sí, caso 500.001 |
  | `BaseUnPesoSobreElUmbral` | `UmbralDescuentoAlto` en `1_000_002m` (línea 10) | sí, caso 1.000.001 |

- **Tests que me quedé:** los 5 (10 casos). Cada caso falló al menos una vez con un cambio. Total de la suite: 126 en verde.
- **Tests que borré y por qué:** ninguno. Todos fallaron con su cambio.

## Etapa 2 · Pipeline seguro

- **`ci.yml`:** <enlace al primer run en verde>
- **`claude.yml` en `main`:** <sí / no>
- **Revisé:** permisos mínimos ☐ · timeout ☐ · versiones fijas ☐ · token solo como secreto ☐

## Etapa 3 · La falla

- **PR:** <enlace>
- **Qué rompí (a mano):** <archivo y cambio>
- **Diagnóstico de `@claude` (en una línea):** <resumen>
- **Mi decisión:** Aceptar ☐ · Ajustar ☐ · Rechazar ☐
- **Por qué:** <tu razón>
- **Corrección y merge:** <commit de la corrección; CI en verde ☐; merge ☐>
