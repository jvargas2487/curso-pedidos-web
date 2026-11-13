---
name: pdf-con-estilo
description: "Convierte un documento Markdown (por ejemplo, de docs/) en un PDF con portada, títulos numerados y tablas legibles, con el mismo diseño siempre. Úsala cuando pidan un PDF, un informe, un plan o un documento imprimible o para enviar a la gerencia a partir de un .md, aunque no nombren la Skill."
---

# PDF con estilo

El diseño vive en `assets/plantilla.html` y la conversión en `scripts/html_a_pdf.py`. Para cambiar el contenido se edita el Markdown; el PDF sale igual de bien sin tocar el CSS.

## Requisitos
Python 3 con `pip install markdown playwright` y, una sola vez, `python -m playwright install chromium`.

## Pasos
1. Confirma que el Markdown de origen empieza con un título `# ...`: es el título de la portada y no se repite en el cuerpo.
2. Desde la raíz del repo, ejecuta:
   `python .claude/skills/pdf-con-estilo/scripts/html_a_pdf.py <origen.md> <destino.pdf>`
   Opciones: `--subtitulo "texto"` para la portada y `--autor "nombre"`.
3. Abre el PDF y revisa: portada, títulos, tablas completas y fondos de color visibles.
4. Si algo se ve mal, corrige el Markdown. Si el problema es de diseño y afectaría a cualquier documento, corrige `assets/plantilla.html`; no agregues estilos para un solo documento.

## Reglas
- No modifiques el Markdown de origen: esta Skill solo escribe el PDF de destino.
- Los PDF están ignorados por git (`*.pdf`); si el PDF debe versionarse, avisa para agregar una excepción puntual en `.gitignore`.
- No inventes contenido de portada: título, subtítulo, autor y fecha salen del documento o de las opciones.
