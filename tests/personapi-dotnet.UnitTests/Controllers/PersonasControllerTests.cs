using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using personapi_dotnet.Controllers;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.Repositories.Interfaces;

namespace personapi_dotnet.UnitTests.Controllers;

/// <summary>Pruebas del controlador MVC (vistas) de personas.</summary>
public class PersonasControllerTests
{
    private readonly IPersonaRepository _repo = Substitute.For<IPersonaRepository>();
    private readonly PersonasController _sut;

    public PersonasControllerTests() => _sut = new PersonasController(_repo);

    private static Persona NuevaPersona(long cc = 123) =>
        new() { Cc = cc, Nombre = "Ana", Apellido = "Gomez", Genero = "F", Edad = 30 };

    [Fact]
    public async Task Index_DevuelveVistaConLasPersonas()
    {
        var personas = new[] { NuevaPersona() };
        _repo.GetAllAsync().Returns(personas);

        var result = await _sut.Index();

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(personas, view.Model);
    }

    [Fact]
    public async Task Details_NoExiste_DevuelveNotFound()
    {
        _repo.GetByIdAsync(1).Returns((Persona?)null);

        Assert.IsType<NotFoundResult>(await _sut.Details(1));
    }

    [Fact]
    public async Task Create_Valida_RedirigeAIndex()
    {
        var persona = NuevaPersona();
        _repo.ExistsAsync(persona.Cc).Returns(false);

        var result = await _sut.Create(persona);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(PersonasController.Index), redirect.ActionName);
        await _repo.Received(1).AddAsync(persona);
    }

    [Fact]
    public async Task Create_CedulaDuplicada_VuelveALaVistaConError()
    {
        var persona = NuevaPersona();
        _repo.ExistsAsync(persona.Cc).Returns(true);

        var result = await _sut.Create(persona);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(persona, view.Model);
        Assert.True(_sut.ModelState.ContainsKey(nameof(Persona.Cc)));
        await _repo.DidNotReceive().AddAsync(Arg.Any<Persona>());
    }

    [Fact]
    public async Task Edit_ModeloInvalido_VuelveALaVistaSinGuardar()
    {
        var persona = NuevaPersona(5);
        _sut.ModelState.AddModelError(nameof(Persona.Nombre), "Requerido");

        var result = await _sut.Edit(5, persona);

        Assert.IsType<ViewResult>(result);
        await _repo.DidNotReceive().UpdateAsync(Arg.Any<Persona>());
    }

    [Fact]
    public async Task Edit_IdDistinto_DevuelveBadRequest()
    {
        Assert.IsType<BadRequestResult>(await _sut.Edit(1, NuevaPersona(2)));
    }

    [Fact]
    public async Task DeleteConfirmed_ConRelaciones_MuestraErrorEnLaVista()
    {
        var persona = NuevaPersona(5);
        _repo.DeleteAsync(5).ThrowsAsync(new DbUpdateException("FK"));
        _repo.GetByIdAsync(5).Returns(persona);

        var result = await _sut.DeleteConfirmed(5);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(persona, view.Model);
        Assert.False(_sut.ModelState.IsValid);
    }
}
