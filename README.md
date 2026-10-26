# personapi-dotnet

Laboratorio 1 – Arquitectura de Software. Monolito **ASP.NET Core MVC** con patrón **DAO/Repositorio**, **EF Core** y **SQL Server**, que expone:

- **Vistas MVC** con CRUD para `Persona`, `Profesion`, `Telefono` y `Estudio`.
- **API REST** (`/api/personas`, `/api/profesiones`, `/api/telefonos`, `/api/estudios`) documentada con **Swagger** (`/swagger`).

## Stack
.NET 7 (`net7.0`) · EF Core 7 · SQL Server 2019 Express / 2022 · Swashbuckle (OpenAPI) · Bootstrap.

## Estructura
```
db/01_ddl.sql, db/02_dml.sql      Script DDL y DML
docs/Informe.docx                 Documento del laboratorio
personapi-dotnet/
  Models/Entities/                Entidades + PersonaDbContext (estilo Scaffold-DbContext)
  Repositories/Interfaces/        Interfaces DAO
  Repositories/                   Implementaciones con EF Core
  Controllers/                    Controladores MVC (vistas)
  Controllers/Api/                Controladores REST
  Views/                          Vistas Razor
```

## Modelo de datos
`persona(cc PK, nombre, apellido, genero, edad)` · `profesion(id PK, nom, des)` ·
`telefono(num PK, oper, duenio FK→persona)` · `estudios(id_prof FK, cc_per FK, fecha, univer)` con PK compuesta (`id_prof`,`cc_per`).

## Configurar el ambiente
1. Instalar SQL Server Express (o 2022) y SSMS; o usar Docker (ver abajo).
2. Crear la BD y datos: ejecutar en SSMS `db/01_ddl.sql` y luego `db/02_dml.sql` (crea `persona_db`).
3. Instalar el SDK de .NET 7 (o superior con *roll-forward*, ver nota) y/o Visual Studio 2022.
4. Ajustar la cadena de conexión `ConnectionStrings:PersonaDb` en `personapi-dotnet/appsettings.json`
   (por defecto `Server=localhost\SQLEXPRESS;Database=persona_db;Trusted_Connection=True;TrustServerCertificate=true`).
   `appsettings.Development.json` apunta a un SQL Server en Docker (`localhost,1433`, usuario `sa`).

### SQL Server con Docker (opcional)
```bash
docker compose up -d
docker cp db/01_ddl.sql personapi-sql:/tmp/ && docker cp db/02_dml.sql personapi-sql:/tmp/
docker exec personapi-sql /opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P 'Your_password123' -i /tmp/01_ddl.sql
docker exec personapi-sql /opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P 'Your_password123' -i /tmp/02_dml.sql
```

## Compilar y desplegar
```bash
cd personapi-dotnet
dotnet restore
dotnet build
ASPNETCORE_ENVIRONMENT=Development ASPNETCORE_URLS=http://localhost:5080 dotnet run
```
- Aplicación MVC: http://localhost:5080/
- Swagger: http://localhost:5080/swagger

Publicar: `dotnet publish -c Release -o publish` y ejecutar `dotnet publish/personapi-dotnet.dll`.

> Nota: si solo tiene instalado .NET 8+, ejecute con `DOTNET_ROLL_FORWARD=Major`.

## Regenerar entidades desde la BD (opcional)
```
Scaffold-DbContext "Server=localhost\SQLEXPRESS;Database=persona_db;Trusted_Connection=True;TrustServerCertificate=true" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models/Entities
```

## API REST
| Recurso | Rutas |
|---|---|
| Personas | `GET/POST /api/personas`, `GET/PUT/DELETE /api/personas/{cc}` |
| Profesiones | `GET/POST /api/profesiones`, `GET/PUT/DELETE /api/profesiones/{id}` |
| Teléfonos | `GET/POST /api/telefonos`, `GET/PUT/DELETE /api/telefonos/{num}` |
| Estudios | `GET/POST /api/estudios`, `GET/PUT/DELETE /api/estudios/{idProf}/{ccPer}` |

Códigos: 200/201/204 éxito, 400 validación o FK inválida, 404 no existe, 409 duplicado o con registros relacionados.

## Entrega
Publicar en GitHub un repositorio público `personapi-dotnet`, hacer `push` y crear el tag:
```bash
git tag -a v1.0 -m "Laboratorio 1" && git push origin main --tags
```
