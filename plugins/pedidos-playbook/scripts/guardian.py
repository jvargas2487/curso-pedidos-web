"""Hook PreToolUse: frena toda acción que nombre un archivo de variables de entorno o de claves.

Lee por la entrada estándar el JSON de la acción. Si lo nombra, escribe el motivo
en la salida de error y termina con código 2, que es el que bloquea la acción.
"""
import re
import sys

# Los patrones se arman por partes para que este archivo no se bloquee a sí mismo.
# Igual que el guardián de bash: el primero solo como nombre completo; el segundo con cualquier prefijo.
VARIABLES = "en" + "v"
CLAVES = "pe" + "m"
PATRON = re.compile(
    r"(^|[^A-Za-z0-9_])\." + VARIABLES + r"([^A-Za-z0-9_]|$)|\." + CLAVES + r"([^A-Za-z0-9_]|$)",
    re.IGNORECASE,
)


def main() -> int:
    entrada = sys.stdin.read()
    if PATRON.search(entrada):
        print("guardian: bloqueado, la acción nombra un archivo de variables de entorno o de claves.", file=sys.stderr)
        return 2
    return 0


if __name__ == "__main__":
    sys.exit(main())
