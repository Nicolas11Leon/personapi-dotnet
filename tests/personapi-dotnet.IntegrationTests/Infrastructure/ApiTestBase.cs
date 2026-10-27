namespace personapi_dotnet.IntegrationTests.Infrastructure;

/// <summary>
/// Clase base para pruebas HTTP: comparte una instancia de la app por clase
/// (IClassFixture) y restablece la BD antes de cada prueba (IAsyncLifetime).
/// </summary>
public abstract class ApiTestBase : IClassFixture<PersonApiFactory>, IAsyncLifetime
{
    protected readonly PersonApiFactory Factory;
    protected readonly HttpClient Client;

    protected ApiTestBase(PersonApiFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public Task InitializeAsync() => Factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}
