using System.Net;
using System.Net.Http.Json;
using personapi_dotnet.IntegrationTests.Infrastructure;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.IntegrationTests.Api;

/// <summary>Pruebas end-to-end (HTTP) de /api/personas.</summary>
public class PersonasApiTests : ApiTestBase
{
    public PersonasApiTests(PersonApiFactory factory) : base(factory) { }

    [Fact]
    public async Task GetAll_DevuelveLasPersonasSemilla()
    {
        var personas = await Client.GetFromJsonAsync<List<Persona>>("/api/personas");

        Assert.NotNull(personas);
        Assert.Equal(3, personas!.Count);
    }

    [Fact]
    public async Task Get_Existente_Devuelve200ConJson()
    {
        var response = await Client.GetAsync($"/api/personas/{TestData.CcConRelaciones}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var persona = await response.Content.ReadFromJsonAsync<Persona>();
        Assert.Equal("Ana", persona!.Nombre);
    }

    [Fact]
    public async Task Get_Inexistente_Devuelve404()
    {
        var response = await Client.GetAsync("/api/personas/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_Valida_Devuelve201ConLocation()
    {
        var nueva = new Persona { Cc = 5000, Nombre = "Carlos", Apellido = "Diaz", Genero = "M", Edad = 40 };

        var response = await Client.PostAsJsonAsync("/api/personas", nueva);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.EndsWith("/api/personas/5000", response.Headers.Location!.ToString(), StringComparison.OrdinalIgnoreCase);

        var get = await Client.GetAsync("/api/personas/5000");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
    }

    [Fact]
    public async Task Post_Duplicada_Devuelve409()
    {
        var dup = new Persona { Cc = TestData.CcConRelaciones, Nombre = "X", Apellido = "Y", Genero = "F" };

        var response = await Client.PostAsJsonAsync("/api/personas", dup);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Post_GeneroInvalido_Devuelve400ConProblemDetails()
    {
        var invalida = new Persona { Cc = 5001, Nombre = "X", Apellido = "Y", Genero = "Z" };

        var response = await Client.PostAsJsonAsync("/api/personas", invalida);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Genero", body);
    }

    [Fact]
    public async Task Put_Valida_Devuelve204YActualiza()
    {
        var cambio = new Persona { Cc = 1002, Nombre = "Luis", Apellido = "Actualizado", Genero = "M", Edad = 50 };

        var response = await Client.PutAsJsonAsync("/api/personas/1002", cambio);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var p = await Client.GetFromJsonAsync<Persona>("/api/personas/1002");
        Assert.Equal("Actualizado", p!.Apellido);
    }

    [Fact]
    public async Task Put_LlaveDistinta_Devuelve400()
    {
        var cambio = new Persona { Cc = 1003, Nombre = "A", Apellido = "B", Genero = "F" };

        var response = await Client.PutAsJsonAsync("/api/personas/1002", cambio);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_SinRelaciones_Devuelve204()
    {
        var response = await Client.DeleteAsync($"/api/personas/{TestData.CcSinRelaciones}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await Client.GetAsync($"/api/personas/{TestData.CcSinRelaciones}")).StatusCode);
    }

    [Fact]
    public async Task Delete_ConRelaciones_Devuelve409()
    {
        var response = await Client.DeleteAsync($"/api/personas/{TestData.CcConRelaciones}");
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
