---
name: documentar-modulo
description: "Escribe la documentación para el equipo de un módulo, servicio o clase de PedidosWeb en docs/modulos/<Modulo>.md, a partir de su código y sus tests. Úsala cuando pidan documentar, explicar o describir un servicio, módulo o clase del código (por ejemplo, el servicio de clientes o el detalle de pedidos), aunque no nombren la Skill."
---

# Documentar un módulo

## Pasos
1. Identifica el módulo pedido y ubica su código en `src/PedidosWeb.Core/<Funcionalidad>/` (o en `src/PedidosWeb.Api/` si es un endpoint).
2. Lee el código completo del módulo, los tipos que usa y sus tests en `tests/PedidosWeb.Core.Tests/<Funcionalidad>/`. Los tests son la fuente de los ejemplos y del comportamiento esperado.
3. Escribe `docs/modulos/<Modulo>.md` (por ejemplo `docs/modulos/ClientesService.md`) con las secciones de abajo. Si el archivo ya existe, actualízalo en vez de duplicarlo.
4. Revisa el documento contra el código: cada método, valor y texto que menciones tiene que existir tal cual.

## Qué debe tener docs/modulos/<Modulo>.md
- Propósito
- API pública
- Ejemplo de uso
- Diagrama Mermaid

## Reglas
- Documenta solo lo que el código hace hoy; no inventes métodos, parámetros ni reglas.
- Si algo del código parece un error o una rareza, anótalo en una sección «Observaciones», sin corregirlo.
- En español, breve y para desarrolladores del equipo; los nombres de código van entre comillas invertidas.
- Los ejemplos de uso salen de los tests existentes, con sus mismos datos.
- No modifiques el código ni los tests: esta Skill solo escribe en `docs/modulos/`.
