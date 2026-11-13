# Checklist de revisión · pedidos-web

## Antes de pedir revisión
- [ ] Corrí el subagente revisor sobre mi rama y atendí lo que encontró.
- [ ] `dotnet build` y `dotnet test` están en verde.
- [ ] Hay un test por cada criterio de aceptación de la spec, incluidos los límites.
- [ ] No hay secretos, claves ni tokens en el cambio.
- [ ] La descripción del PR explica qué cambia, por qué y cómo probarlo, sin `[COMPLETAR]`.

## Al revisar (persona)
- [ ] ¿Qué pasa con los casos límite y con las entradas inválidas (por ejemplo, `desde` mayor que `hasta`)?
- [ ] ¿El cambio hace solo lo que pide la spec, sin alcance de más?
- [ ] ¿El porqué del PR tiene sentido de negocio, no solo técnico?
- [ ] ¿Un test falla si el código se rompe, o solo pasa porque repite la implementación?

## Con los hallazgos de la IA
- [ ] Cada hallazgo tiene respuesta: acepto, discutir o descarto, con argumento.
- [ ] Un «ok» sin argumento en todos los hilos no cuenta como revisar.
- [ ] Lo que acepto queda corregido y probado antes del merge.

## Merge
- [ ] El CI está en verde.
- [ ] Todos los hilos están respondidos o resueltos.
- [ ] Al menos una persona revisó el PR y aportó un hallazgo propio.

## Mi PR de HU-03
- **PR:** <enlace>
- **Hallazgo de la IA que más discutimos:** <cuál y qué decidimos>
- **Mi hallazgo humano:** <qué encontraste que la IA no vio>
