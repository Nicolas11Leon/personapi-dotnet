using Microsoft.EntityFrameworkCore;
using personapi_dotnet.IntegrationTests.Infrastructure;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.Repositories;

namespace personapi_dotnet.IntegrationTests.Repositories;

/// <summary>Pruebas del repositorio (DAO) de personas contra SQLite real.</summary>
public class PersonaRepositoryTests : IDisposable
{
    private readonly SqliteTestDatabase _db = new();

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task GetAllAsync_DevuelveTodasOrdenadasPorCedula()
    {
        using var ctx = _db.CreateContext();
        var repo = new PersonaRepository(ctx);

        var personas = (await repo.GetAllAsync()).ToList();

        Assert.Equal(new long[] { 1001, 1002, 1003 }, personas.Select(p => p.Cc));
    }

    [Fact]
    public async Task GetByIdAsync_NoExiste_DevuelveNull()
    {
        using var ctx = _db.CreateContext();
        Assert.Null(await new PersonaRepository(ctx).GetByIdAsync(9999));
    }

    [Fact]
    public async Task AddAsync_PersisteLaPersona()
    {
        using (var ctx = _db.CreateContext())
        {
            await new PersonaRepository(ctx).AddAsync(
                new Persona { Cc = 2000, Nombre = "Nueva", Apellido = "Persona", Genero = "M", Edad = 18 });
        }

        using var verify = _db.CreateContext();
        var guardada = await verify.Personas.FindAsync(2000L);
        Assert.NotNull(guardada);
        Assert.Equal("Nueva", guardada!.Nombre);
    }

    [Fact]
    public async Task AddAsync_CedulaDuplicada_LanzaDbUpdateException()
    {
        using var ctx = _db.CreateContext();
        var repo = new PersonaRepository(ctx);

        await Assert.ThrowsAsync<DbUpdateException>(() => repo.AddAsync(
            new Persona { Cc = 1001, Nombre = "X", Apellido = "Y", Genero = "M" }));
    }

    [Fact]
    public async Task UpdateAsync_EntidadDesconectada_ActualizaLosCampos()
    {
        using (var ctx = _db.CreateContext())
        {
            await new PersonaRepository(ctx).UpdateAsync(
                new Persona { Cc = 1002, Nombre = "Luis", Apellido = "Cambiado", Genero = "M", Edad = 46 });
        }

        using var verify = _db.CreateContext();
        var p = await verify.Personas.FindAsync(1002L);
        Assert.Equal("Cambiado", p!.Apellido);
        Assert.Equal(46, p.Edad);
    }

    [Fact]
    public async Task DeleteAsync_SinRelaciones_EliminaLaPersona()
    {
        using (var ctx = _db.CreateContext())
        {
            await new PersonaRepository(ctx).DeleteAsync(TestData.CcSinRelaciones);
        }

        using var verify = _db.CreateContext();
        Assert.False(await verify.Personas.AnyAsync(p => p.Cc == TestData.CcSinRelaciones));
    }

    [Fact]
    public async Task DeleteAsync_NoExiste_NoLanzaExcepcion()
    {
        using var ctx = _db.CreateContext();
        await new PersonaRepository(ctx).DeleteAsync(9999);
    }

    [Fact]
    public async Task DeleteAsync_ConTelefonosOEstudios_LanzaDbUpdateExceptionPorFK()
    {
        using var ctx = _db.CreateContext();
        var repo = new PersonaRepository(ctx);

        await Assert.ThrowsAsync<DbUpdateException>(() => repo.DeleteAsync(TestData.CcConRelaciones));
    }
}
