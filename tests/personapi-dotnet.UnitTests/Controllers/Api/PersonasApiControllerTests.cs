using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using personapi_dotnet.Controllers.Api;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.Repositories.Interfaces;

namespace personapi_dotnet.UnitTests.Controllers.Api;

/// <summary>
/// Pruebas unitarias del controlador REST de personas.
/// El repositorio se sustituye por un doble (NSubstitute), así se prueba
/// solo la lógica HTTP del controlador, sin base de datos.
/// </summary>
public class PersonasApiControllerTests
{
    private readonly IPersonaRepository _repo = Substitute.For<IPersonaRepository>();
    private readonly PersonasApiController _sut;

    public PersonasApiControllerTests() => _sut = new PersonasApiController(_repo);

    private static Persona NuevaPersona(long cc = 123) =>
        new() { Cc = cc, Nombre = "Ana", Apellido = "Gomez", Genero = "F", Edad = 30 };

    [Fact]
    public async Task GetAll_DevuelveOkConLasPersonas()
    {
        var personas = new[] { NuevaPersona(1), NuevaPersona(2) };
        _repo.GetAllAsync().Returns(personas);

        var result = await _sut.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(personas, ok.Value);
    }

    [Fact]
    public async Task Get_CuandoExiste_DevuelveOk()
    {
        var persona = NuevaPersona();
        _repo.GetByIdAsync(persona.Cc).Returns(persona);

        var result = await _sut.Get(persona.Cc);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(persona, ok.Value);
    }

    [Fact]
    public async Task Get_CuandoNoExiste_DevuelveNotFound()
    {
        _repo.GetByIdAsync(Arg.Any<long>()).Returns((Persona?)null);

        var result = await _sut.Get(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_Valida_DevuelveCreatedAtAction()
    {
        var persona = NuevaPersona();
        _repo.ExistsAsync(persona.Cc).Returns(false);

        var result = await _sut.Create(persona);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(PersonasApiController.Get), created.ActionName);
        Assert.Equal(persona.Cc, created.RouteValues!["cc"]);
        await _repo.Received(1).AddAsync(persona);
    }

    [Fact]
    public async Task Create_Duplicada_DevuelveConflictYNoGuarda()
    {
        var persona = NuevaPersona();
        _repo.ExistsAsync(persona.Cc).Returns(true);

        var result = await _sut.Create(persona);

        Assert.IsType<ConflictObjectResult>(result.Result);
        await _repo.DidNotReceive().AddAsync(Arg.Any<Persona>());
    }

    [Fact]
    public async Task Create_ErrorDeBaseDeDatos_DevuelveBadRequest()
    {
        var persona = NuevaPersona();
        _repo.ExistsAsync(persona.Cc).Returns(false);
        _repo.AddAsync(persona).ThrowsAsync(new DbUpdateException("FK"));

        var result = await _sut.Create(persona);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_LlaveDistintaALaDelCuerpo_DevuelveBadRequest()
    {
        var result = await _sut.Update(1, NuevaPersona(2));

        Assert.IsType<BadRequestObjectResult>(result);
        await _repo.DidNotReceive().UpdateAsync(Arg.Any<Persona>());
    }

    [Fact]
    public async Task Update_NoExiste_DevuelveNotFound()
    {
        _repo.ExistsAsync(5).Returns(false);

        var result = await _sut.Update(5, NuevaPersona(5));

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Update_Valida_DevuelveNoContent()
    {
        var persona = NuevaPersona(5);
        _repo.ExistsAsync(5).Returns(true);

        var result = await _sut.Update(5, persona);

        Assert.IsType<NoContentResult>(result);
        await _repo.Received(1).UpdateAsync(persona);
    }

    [Fact]
    public async Task Delete_NoExiste_DevuelveNotFound()
    {
        _repo.ExistsAsync(7).Returns(false);

        var result = await _sut.Delete(7);

        Assert.IsType<NotFoundResult>(result);
        await _repo.DidNotReceive().DeleteAsync(Arg.Any<long>());
    }

    [Fact]
    public async Task Delete_ConRegistrosRelacionados_DevuelveConflict()
    {
        _repo.ExistsAsync(7).Returns(true);
        _repo.DeleteAsync(7).ThrowsAsync(new DbUpdateException("FK"));

        var result = await _sut.Delete(7);

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task Delete_Existente_DevuelveNoContent()
    {
        _repo.ExistsAsync(7).Returns(true);

        var result = await _sut.Delete(7);

        Assert.IsType<NoContentResult>(result);
        await _repo.Received(1).DeleteAsync(7);
    }
}
