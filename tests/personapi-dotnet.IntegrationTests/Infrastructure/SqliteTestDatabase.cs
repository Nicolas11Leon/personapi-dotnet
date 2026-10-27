using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.IntegrationTests.Infrastructure;

/// <summary>
/// Base de datos SQLite en memoria para probar los repositorios contra un
/// motor relacional real (a diferencia de EF InMemory, SQLite sí aplica PK y FK).
/// La BD vive mientras la conexión esté abierta: una por clase de prueba.
/// </summary>
public sealed class SqliteTestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<PersonaDbContext> _options;

    public SqliteTestDatabase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _options = new DbContextOptionsBuilder<PersonaDbContext>().UseSqlite(_connection).Options;

        using var db = CreateContext();
        db.Database.EnsureCreated();
        TestData.Seed(db);
    }

    /// <summary>Crea un contexto nuevo (sin entidades rastreadas) sobre la misma BD.</summary>
    public PersonaDbContext CreateContext() => new(_options);

    public void Dispose() => _connection.Dispose();
}
