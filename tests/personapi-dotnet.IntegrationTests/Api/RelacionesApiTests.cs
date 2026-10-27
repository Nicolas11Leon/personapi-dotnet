using System.Net;
using System.Net.Http.Json;
using personapi_dotnet.IntegrationTests.Infrastructure;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.IntegrationTests.Api;

/// <summary>
/// Pruebas end-to-end de los recursos con llaves foráneas
/// (/api/telefonos, /api/estudios, /api/profesiones).
/// </summary>
public class RelacionesApiTests : ApiTestBase
{
    public RelacionesApiTests(PersonApiFactory factory) : base(factory) { }

    [Fact]
    public async Task Telefono_ConDuenioExistente_Devuelve201()
    {
        var tel = new Telefono { Num = "3109998877", Oper = "Movistar", Duenio = 1002 };

        var response = await Client.PostAsJsonAsync("/api/telefonos", tel);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Telefono_ConDuenioInexistente_Devuelve400()
    {
        var tel = new Telefono { Num = "3109998877", Oper = "Movistar", Duenio = 9999 };

        var response = await Client.PostAsJsonAsync("/api/telefonos", tel);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Telefono_NumeroDuplicado_Devuelve409()
    {
        var tel = new Telefono { Num = TestData.NumTelefono, Oper = "Tigo", Duenio = 1002 };

        var response = await Client.PostAsJsonAsync("/api/telefonos", tel);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Estudio_GetPorLlaveCompuesta_Devuelve200()
    {
        var response = await Client.GetAsync($"/api/estudios/{TestData.ProfConEstudios}/{TestData.CcConRelaciones}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var estudio = await response.Content.ReadFromJsonAsync<Estudio>();
        Assert.Equal("Universidad Nacional", estudio!.Univer);
    }

    [Fact]
    public async Task Estudio_ConProfesionInexistente_Devuelve400()
    {
        var est = new Estudio { IdProf = 99, CcPer = 1002, Univer = "UdeA" };

        var response = await Client.PostAsJsonAsync("/api/estudios", est);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Estudio_Nuevo_Devuelve201ConLocationCompuesta()
    {
        var est = new Estudio { IdProf = 2, CcPer = 1002, Fecha = new DateTime(2010, 12, 1), Univer = "UdeA" };

        var response = await Client.PostAsJsonAsync("/api/estudios", est);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.EndsWith("/api/estudios/2/1002", response.Headers.Location!.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Profesion_ConEstudios_NoSePuedeEliminar_Devuelve409()
    {
        var response = await Client.DeleteAsync($"/api/profesiones/{TestData.ProfConEstudios}");
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Profesion_SinEstudios_SeElimina_Devuelve204()
    {
        var response = await Client.DeleteAsync($"/api/profesiones/{TestData.ProfSinEstudios}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
