using Microsoft.EntityFrameworkCore;
using personapi_dotnet.IntegrationTests.Infrastructure;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.Repositories;

namespace personapi_dotnet.IntegrationTests.Repositories;

/// <summary>Pruebas del repositorio de estudios (llave compuesta + navegaciones).</summary>
public class EstudioRepositoryTests : IDisposable
{
    private readonly SqliteTestDatabase _db = new();

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task GetByIdAsync_IncluyePersonaYProfesion()
    {
        using var ctx = _db.CreateContext();

        var estudio = await new EstudioRepository(ctx).GetByIdAsync(TestData.ProfConEstudios, TestData.CcConRelaciones);

        Assert.NotNull(estudio);
        Assert.Equal("Ana", estudio!.CcPerNavigation?.Nombre);
        Assert.Equal("Ingenieria de Sistemas", estudio.IdProfNavigation?.Nom);
    }

    [Fact]
    public async Task ExistsAsync_UsaLaLlaveCompuesta()
    {
        using var ctx = _db.CreateContext();
        var repo = new EstudioRepository(ctx);

        Assert.True(await repo.ExistsAsync(1, 1001));
        Assert.False(await repo.ExistsAsync(2, 1001));
        Assert.False(await repo.ExistsAsync(1, 1002));
    }

    [Fact]
    public async Task AddAsync_PersonaInexistente_LanzaDbUpdateExceptionPorFK()
    {
        using var ctx = _db.CreateContext();
        var repo = new EstudioRepository(ctx);

        await Assert.ThrowsAsync<DbUpdateException>(() =>
            repo.AddAsync(new Estudio { IdProf = 1, CcPer = 9999, Univer = "X" }));
    }

    [Fact]
    public async Task DeleteAsync_EliminaPorLlaveCompuesta()
    {
        using (var ctx = _db.CreateContext())
        {
            await new EstudioRepository(ctx).DeleteAsync(1, 1001);
        }

        using var verify = _db.CreateContext();
        Assert.Empty(verify.Estudios);
    }
}
