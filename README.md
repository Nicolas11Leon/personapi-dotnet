# personapi-dotnet

Laboratorio 1 – Arquitectura de Software. Monolito **ASP.NET Core MVC** con patrón **DAO/Repositorio**, **EF Core** y **SQL Server**, que expone:

- **Vistas MVC** con CRUD para `Persona`, `Profesion`, `Telefono` y `Estudio`.
- **API REST** (`/api/personas`, `/api/profesiones`, `/api/telefonos`, `/api/estudios`) documentada con **Swagger** (`/swagger`).

## Stack
.NET 8 (`net8.0`) · EF Core 8 · SQL Server 2022 · Swashbuckle (OpenAPI) · Bootstrap · xUnit · NSubstitute · TestServer (WebApplicationFactory).

## Estructura
```
db/01_ddl.sql, db/02_dml.sql, db/init.sh   Scripts de BD e inicialización automática
docs/Informe.docx                          Documento del laboratorio
personapi-dotnet/                          Proyecto principal Web (MVC + API REST)
  Models/Entities/                         Entidades + PersonaDbContext
  Repositories/Interfaces/                 Interfaces DAO
  Repositories/                            Implementaciones con EF Core
  Controllers/                             Controladores MVC (vistas)
  Controllers/Api/                         Controladores REST
  Views/                                   Vistas Razor
tests/
  personapi-dotnet.UnitTests/              Pruebas unitarias (xUnit + NSubstitute + DataAnnotations)
  personapi-dotnet.IntegrationTests/       Pruebas de integración (WebApplicationFactory + SQLite en memoria)
```

## Modelo de datos
`persona(cc PK, nombre, apellido, genero, edad)` · `profesion(id PK, nom, des)` ·
`telefono(num PK, oper, duenio FK→persona)` · `estudios(id_prof FK, cc_per FK, fecha, univer)` con PK compuesta (`id_prof`,`cc_per`).

## Despliegue con Docker Compose (Recomendado)
Levanta SQL Server 2022, inicializa la BD automáticamente con los scripts DDL/DML y compila/despliega la API en .NET 8:
```bash
docker compose up -d --build
```
- Aplicación MVC: http://localhost:5080/
- Documentación Swagger: http://localhost:5080/swagger

Para ver logs del servicio:
```bash
docker compose logs -f api
```

## Pruebas (Testing en .NET 8)
El proyecto incluye **40 pruebas automáticas** que cubren:
- **Unitarias:** Controladores API y MVC con repositorios simulados (`NSubstitute`), y reglas de validación en modelos (`DataAnnotations`).
- **Integración:** Repositorios contra SQLite relacional en memoria (valida PK, FK y restricciones), pruebas de endpoints HTTP con `WebApplicationFactory<Program>`, y renderizado de vistas Razor / Swagger.

### 1. Ejecutar pruebas con Docker (sin instalar SDK local):
```bash
docker compose --profile test run --rm tests
```
Los resultados y reportes de cobertura se guardan en el directorio `./TestResults/`.

### 2. Ejecutar pruebas localmente (con SDK .NET 8 instalado):
```bash
dotnet test personapi-dotnet.sln --logger "console;verbosity=normal"
```

## Compilar y desplegar localmente (sin Docker)
```bash
dotnet restore personapi-dotnet.sln
dotnet build personapi-dotnet.sln
cd personapi-dotnet
ASPNETCORE_ENVIRONMENT=Development ASPNETCORE_URLS=http://localhost:5080 dotnet run
```

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
