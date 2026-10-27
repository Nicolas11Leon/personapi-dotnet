using System.Net;
using personapi_dotnet.IntegrationTests.Infrastructure;

namespace personapi_dotnet.IntegrationTests.Web;

/// <summary>Pruebas de humo de las vistas MVC, Swagger y protección antiforgery.</summary>
public class MvcPagesTests : ApiTestBase
{
    public MvcPagesTests(PersonApiFactory factory) : base(factory) { }

    [Theory]
    [InlineData("/")]
    [InlineData("/Personas")]
    [InlineData("/Profesiones")]
    [InlineData("/Telefonos")]
    [InlineData("/Estudios")]
    [InlineData("/Personas/Create")]
    public async Task Paginas_RenderizanHtml(string url)
    {
        var response = await Client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task PersonasIndex_MuestraLosDatosDeLaBD()
    {
        var html = await Client.GetStringAsync("/Personas");

        Assert.Contains("Ana", html);
        Assert.Contains("Perez", html);
    }

    [Fact]
    public async Task PersonasDetails_Inexistente_Devuelve404()
    {
        var response = await Client.GetAsync("/Personas/Details/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostFormulario_SinTokenAntiforgery_EsRechazado()
    {
        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Cc"] = "7000", ["Nombre"] = "X", ["Apellido"] = "Y", ["Genero"] = "M",
        });

        var response = await Client.PostAsync("/Personas/Create", form);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Swagger_ExponeElDocumentoOpenApi()
    {
        var json = await Client.GetStringAsync("/swagger/v1/swagger.json");

        Assert.Contains("/api/personas", json);
        Assert.Contains("/api/estudios/{idProf}/{ccPer}", json);
    }
}
