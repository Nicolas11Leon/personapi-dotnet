#!/bin/bash
# Inicializa persona_db solo si aún no existe (01_ddl.sql hace DROP TABLE,
# así que no se debe re-ejecutar en cada `docker compose up`).
set -euo pipefail

export SQLCMDPASSWORD="${MSSQL_SA_PASSWORD}"
SQLCMD="/opt/mssql-tools18/bin/sqlcmd -C -S ${DB_HOST:-sqlserver} -U sa -b"

exists=$($SQLCMD -h -1 -W -Q "SET NOCOUNT ON; SELECT CASE WHEN DB_ID('persona_db') IS NULL THEN 0 ELSE 1 END" | tr -d '[:space:]')

if [ "$exists" = "1" ]; then
  echo "persona_db ya existe, se omite la inicialización."
  exit 0
fi

echo "Creando esquema (01_ddl.sql)..."
$SQLCMD -i /scripts/01_ddl.sql
echo "Cargando datos (02_dml.sql)..."
$SQLCMD -i /scripts/02_dml.sql
echo "persona_db inicializada."
