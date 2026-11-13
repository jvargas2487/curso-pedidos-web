"""Convierte un documento Markdown en un PDF con la plantilla de la Skill pdf-con-estilo.

Uso:
    python html_a_pdf.py <origen.md> <destino.pdf> [--subtitulo TEXTO] [--autor TEXTO]

Requiere: pip install markdown playwright  y  python -m playwright install chromium
"""
import argparse
import datetime
import html
import re
import sys
from pathlib import Path

import markdown
from playwright.sync_api import sync_playwright

PLANTILLA = Path(__file__).resolve().parent.parent / "assets" / "plantilla.html"
ETIQUETA_PORTADA = "Documento"
PIE_DE_PAGINA = (
    '<div style="font-size:8px;width:100%;text-align:center;color:#5b6675;">'
    '<span class="pageNumber"></span> / <span class="totalPages"></span></div>'
)


def separar_titulo(texto: str) -> tuple[str, str]:
    """Devuelve el primer título de nivel 1 y el resto del documento."""
    lineas = texto.splitlines()
    for i, linea in enumerate(lineas):
        if linea.startswith("# "):
            return linea[2:].strip(), "\n".join(lineas[:i] + lineas[i + 1:])
    raise SystemExit("El Markdown debe empezar con un título '# ...' para la portada.")


def construir_html(origen: Path, subtitulo: str, autor: str) -> str:
    titulo, cuerpo = separar_titulo(origen.read_text(encoding="utf-8"))
    contenido = markdown.markdown(cuerpo, extensions=["tables", "fenced_code", "sane_lists"])

    fecha = datetime.date.today().isoformat()
    meta = f"{html.escape(autor)} · {fecha}" if autor else fecha

    plantilla = PLANTILLA.read_text(encoding="utf-8")
    reemplazos = {
        "{{TITULO}}": html.escape(titulo),
        "{{ETIQUETA}}": ETIQUETA_PORTADA,
        "{{SUBTITULO}}": f'<p class="subtitulo">{html.escape(subtitulo)}</p>' if subtitulo else "",
        "{{META}}": f'<p class="meta">{meta}</p>',
        "{{CONTENIDO}}": contenido,
    }
    # Un solo pase: así un marcador dentro del contenido no se vuelve a reemplazar.
    patron = re.compile("|".join(re.escape(k) for k in reemplazos))
    return patron.sub(lambda m: reemplazos[m.group(0)], plantilla)


def generar_pdf(contenido_html: str, destino: Path) -> None:
    destino.parent.mkdir(parents=True, exist_ok=True)
    with sync_playwright() as p:
        navegador = p.chromium.launch()
        pagina = navegador.new_page()
        pagina.set_content(contenido_html, wait_until="load")
        pagina.pdf(
            path=str(destino),
            format="A4",
            print_background=True,
            display_header_footer=True,
            header_template="<span></span>",
            footer_template=PIE_DE_PAGINA,
            margin={"top": "22mm", "bottom": "20mm", "left": "18mm", "right": "18mm"},
        )
        navegador.close()


def main() -> int:
    sys.stdout.reconfigure(encoding="utf-8")
    parser = argparse.ArgumentParser(description="Markdown a PDF con estilo.")
    parser.add_argument("origen", type=Path)
    parser.add_argument("destino", type=Path)
    parser.add_argument("--subtitulo", default="")
    parser.add_argument("--autor", default="")
    args = parser.parse_args()

    if not args.origen.is_file():
        print(f"No existe el archivo de origen: {args.origen}", file=sys.stderr)
        return 1

    generar_pdf(construir_html(args.origen, args.subtitulo, args.autor), args.destino)
    print(f"PDF generado: {args.destino}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
