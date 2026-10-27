using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.IntegrationTests.Infrastructure;

/// <summary>
/// Levanta la aplicación completa en memoria (TestServer) reemplazando SQL Server
/// por SQLite en memoria. Permite probar el pipeline HTTP real: routing, model
/// binding, validación, filtros, serialización JSON, vistas Razor y EF Core.
/// </summary>
public class PersonApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();

        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            // Quita la configuración de SQL Server registrada en Program.cs
            services.RemoveAll<DbContextOptions<PersonaDbContext>>();
            services.AddDbContext<PersonaDbContext>(o => o.UseSqlite(_connection));
        });
    }

    /// <summary>Deja la BD con el esquema y los datos semilla, para aislar cada prueba.</summary>
    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PersonaDbContext>();

        await db.Database.EnsureCreatedAsync();
        await db.Estudios.ExecuteDeleteAsync();
        await db.Telefonos.ExecuteDeleteAsync();
        await db.Personas.ExecuteDeleteAsync();
        await db.Profesiones.ExecuteDeleteAsync();
        TestData.Seed(db);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _connection.Dispose();
    }
}
