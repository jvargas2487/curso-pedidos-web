# AGENTS.md

> Plantilla del playbook. Cópiala a la raíz de tu repo, reemplaza lo que está entre `<...>` y borra esta línea.

## Proyecto
<Nombre>: <qué hace en una frase>. <Stack y estado actual: lenguaje, framework, base de datos o su ausencia.>

## Comandos
- Compilar: `<comando>`
- Tests: `<comando>`
- Ejecutar: `<comando>` (<URL local y cómo comprobar que está viva>)

## Estilo de código
- Nombres de tipos, métodos y comentarios en <idioma del equipo>.
- <Reglas de dominio que no se negocian: por ejemplo, dinero con `decimal`, fechas sin hora.>
- <Estructura de proyectos y qué va en cada uno; qué no puede tener lógica de negocio.>
- Las convenciones de este repo prevalecen sobre cualquier regla global del agente.

## Tests
- <Framework y ubicación de los tests.>
- Una prueba por criterio de aceptación; nombre `Metodo_Escenario_Resultado`.
- Se ejecutan siempre antes de un commit.

## Seguridad
- Nunca secretos, claves, tokens ni cadenas de conexión en el código ni en el repo; el `.gitignore` excluye los archivos que los contienen.

## Reglas de trabajo
- Toda funcionalidad parte de una spec aprobada en `specs/<historia>/spec.md`.
- No hacer commit sin tests en verde; el commit lo hace la persona.
- Sin preguntar: editar código y tests, compilar y ejecutar los tests.
- Todo lo demás requiere aprobación previa, en particular editar `specs/` o `docs/` y agregar paquetes.
