# Política de uso de IA · pedidos-web

Los controles viven en `.claude/settings.json` y `.claude/hooks/guardian.sh`. Solo se declara como probado lo que se intentó violar en la sesión del 13-nov-2026.

## 1. Datos que nunca entran a Claude

- Archivos de entorno (`.env`) con contraseñas y claves: dan acceso a sistemas reales.
- Certificados y llaves (`*.pem`): permiten suplantar al equipo o a un servidor.
- Cualquier cadena de conexión o clave pegada en un prompt (el control técnico no revisa el texto que escribe la persona).
- Datos reales de clientes de la distribuidora: en el repo todo es ficticio y debe seguir así.

## 2. Controles activos

| Regla | Control | ¿Probado? (fecha) | Resultado |
|---|---|---|---|
| No leer `.env` | `deny` `Read(**/.env)` | Sí (13-nov-2026) | Bloqueado: «File is in a directory that is denied by your permission settings». |
| No leer `*.pem` | `deny` `Read(**/*.pem)` | Sí (13-nov-2026) | Bloqueado con el mismo mensaje. |
| No editar `legado/` | `deny` `Edit(**/legado/**)` (U4) | Sí (13-nov-2026) | Bloqueado. |
| Frenar acciones que nombran `.env` o `.pem` | Hook `PreToolUse` `guardian.sh` | Sí (13-nov-2026) | Un comando Bash que nombraba un `.pem` y otro que nombraba `.env*` fueron frenados (exit 2). También se probó el script a mano con entradas JSON: `.env` y `.pem` dan 2; una lectura normal da 0. |
| Preguntar antes de `curl`, `wget`, `Invoke-WebRequest`, `WebFetch` | `ask` | No | Las reglas están escritas y el JSON es válido, pero no se ejecutó ninguna de esas acciones. |
| Preguntar antes de editar `migracion/golden-master.csv` | `ask` | No | Sin intento de edición. |
| Correr los tests tras editar | Hook `PostToolUse` (U5) | No en esta sesión | Se conservó sin cambios. |

## 3. Lo no cubierto

- **`grep -r` por contenido:** el hook solo mira si la acción nombra un `.env` o un `.pem`. Una búsqueda como `grep -r DB_PASSWORD .` no los nombra y el hook la deja pasar (probado a mano: exit 0). En la sesión la frenó el clasificador automático del modo de permisos, no nuestra regla, y eso no está garantizado. Riesgo: leer secretos sin nombrar el archivo. Mitigación: no guardar secretos reales en el árbol del repo.
- **Falsos positivos del hook:** frena cualquier acción cuyo texto contenga `.env` o `.pem`, incluso para escribir esta misma documentación o crear el certificado falso de prueba. Riesgo: fricción y documentos que Claude Code no puede editar. Mitigación: editar esos archivos o crear los falsos fuera de Claude Code.
- **Reglas `ask` no probadas:** funcionan solo si la persona lee el aviso antes de aceptar.
- **`/permissions` y `/hooks`:** no se revisaron en la interfaz; solo se verificó el comportamiento.
- **`guardian.sh` en Windows:** corre con Git Bash. Se comprobó que el hook se ejecuta en esta máquina, pero no en otras.
- **Lo que escribe la persona:** nada impide pegar un secreto en un prompt. Mitigación: la regla de la sección 1.
- **Dependencias:** quedan vigiladas por `dependabot.yml`, pero un PR de Dependabot no se prueba solo: se revisa como cualquier PR.

## 4. Escenarios

| Escenario | Control que lo cubre |
|---|---|
| Alguien pide a Claude «lee el `.env` y dime la configuración». | `deny` de `Read(**/.env)` y hook `guardian.sh` (probados). |
| Un archivo del repo trae una instrucción escondida para descargar un script con `curl`. | `ask` sobre `curl`/`wget`/`Invoke-WebRequest`/`WebFetch`: pregunta antes de ejecutar (no probado). |
| Claude intenta «arreglar» el código de `legado/` o el golden master. | `deny` de `Edit(**/legado/**)` (probado); `ask` del golden master (no probado). |

## 5. Transparencia en los PR

Cada PR agrega esta línea:

`Uso de IA: <herramienta y para qué, p. ej. «Claude Code: escribió los tests; yo revisé el diff»>`