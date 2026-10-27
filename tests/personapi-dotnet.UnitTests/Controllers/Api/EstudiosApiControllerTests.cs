using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using personapi_dotnet.Controllers.Api;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.Repositories.Interfaces;

namespace personapi_dotnet.UnitTests.Controllers.Api;

/// <summary>Pruebas del controlador REST de estudios (llave compuesta idProf + ccPer).</summary>
public class EstudiosApiControllerTests
{
    private readonly IEstudioRepository _repo = Substitute.For<IEstudioRepository>();
    private readonly EstudiosApiController _sut;

    public EstudiosApiControllerTests() => _sut = new EstudiosApiController(_repo);

    private static Estudio NuevoEstudio(int idProf = 1, long ccPer = 100) =>
        new() { IdProf = idProf, CcPer = ccPer, Fecha = new DateTime(2020, 1, 1), Univer = "UNAL" };

    [Fact]
    public async Task Get_CuandoNoExiste_DevuelveNotFound()
    {
        _repo.GetByIdAsync(1, 100).Returns((Estudio?)null);

        var result = await _sut.Get(1, 100);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_Valido_DevuelveCreatedConLlaveCompuesta()
    {
        var estudio = NuevoEstudio();
        _repo.ExistsAsync(1, 100).Returns(false);

        var result = await _sut.Create(estudio);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(1, created.RouteValues!["idProf"]);
        Assert.Equal(100L, created.RouteValues!["ccPer"]);
    }

    [Fact]
    public async Task Create_ConLlaveForaneaInvalida_DevuelveBadRequest()
    {
        var estudio = NuevoEstudio();
        _repo.ExistsAsync(1, 100).Returns(false);
        _repo.AddAsync(estudio).ThrowsAsync(new DbUpdateException("FK"));

        var result = await _sut.Create(estudio);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Theory]
    [InlineData(2, 100L)] // idProf distinto
    [InlineData(1, 200L)] // ccPer distinto
    public async Task Update_LlaveDeRutaDistinta_DevuelveBadRequest(int idProf, long ccPer)
    {
        var result = await _sut.Update(idProf, ccPer, NuevoEstudio(1, 100));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Delete_Existente_DevuelveNoContent()
    {
        _repo.ExistsAsync(1, 100).Returns(true);

        var result = await _sut.Delete(1, 100);

        Assert.IsType<NoContentResult>(result);
        await _repo.Received(1).DeleteAsync(1, 100);
    }
}
