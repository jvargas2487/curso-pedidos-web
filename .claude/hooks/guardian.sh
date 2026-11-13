#!/bin/bash
# Hook PreToolUse: frena toda acción que nombre un .env o un .pem.
entrada=$(cat)

if echo "$entrada" | grep -Eiq '(^|[^[:alnum:]_])\.env([^[:alnum:]_]|$)|\.pem([^[:alnum:]_]|$)'; then
  echo "guardian: bloqueado, la acción nombra un .env o un .pem." >&2
  exit 2
fi

exit 0
