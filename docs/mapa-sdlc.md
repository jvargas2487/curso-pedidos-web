# Mapa de uso de IA en el SDLC · Pedidos Web

**Autor:** Jeffry Vargas
**Fecha:** 2026-11-09

| Etapa | Se delega a Claude | Artefacto versionado | Revisa una persona | Riesgo | Evento Scrum |
|---|---|---|---|---|---|
| Requisitos | Le pido que lea lo que sabemos del sistema viejo y me ayude a dejar las reglas por escrito, con ejemplos y casos raros. Así salió la tabla del folio. | `README.md` | El PO. Cada regla la confirma con la gente del negocio. Si algo no está escrito, se pregunta, no se adivina. | Que invente una regla que suena lógica pero que el legado no tiene, por ejemplo aceptar la Ñ. | Refinamiento |
| Diseño | Que me proponga cómo armar la solución: firma, dónde va cada clase y qué excepción usar en cada caso. | El código en `src/` y las notas en `docs/` | Yo o el líder técnico. Antes de que toque nada, me tiene que mostrar el plan y yo lo apruebo. | Que se complique de más (interfaces, capas o paquetes que no hacen falta) o que cambie el formato y los folios antiguos ya no calcen. | Sprint Planning |
| Código | Escribir la implementación siguiendo las reglas del README. | `FolioFormatter.cs` | Yo leo el diff completo. Si no lo entiendo, no entra. | Errores chicos que pasan colados: mayúsculas que dependen del idioma del equipo, aceptar letras con tilde o validar en otro orden. | Durante el Sprint |
| Pruebas | Generar los tests de todos los ejemplos del README, sobre todo los casos que fallan. | `FolioFormatterTests.cs` | Yo reviso que cada regla tenga al menos un test y que el test falle si rompo la regla a propósito. | Que escriba tests que pasan porque copian lo que hace el código, no lo que pide la regla. | Durante el Sprint (DoD) |
| Revisión y despliegue | Que corra el build y los tests y me sugiera el mensaje de commit. | El commit en Git (más adelante, el pipeline de CI) | El commit, el push y el merge los hago yo, nunca la IA. | Confiarse porque todo salió en verde y no leer los cambios, o que haya tocado archivos que no debía. | Sprint Review |
| Mejora continua | Pedirle un resumen de dónde se equivocó en el sprint y qué reglas nuevas convendría agregar a `AGENTS.md`. | `docs/mapa-sdlc.md` y los cambios a `AGENTS.md` | El equipo. Decidimos juntos qué reglas se agregan. | Llenar `AGENTS.md` de reglas que nadie lee, o dejar que la IA decida cosas que le tocan al equipo. | Retrospectiva |

## Evidencia del paso 3

| | Sin IA | Con Claude Code |
|---|---|---|
| Tiempo (min) | | |
| Errores encontrados | | |
| Qué tuve que corregir | | |
