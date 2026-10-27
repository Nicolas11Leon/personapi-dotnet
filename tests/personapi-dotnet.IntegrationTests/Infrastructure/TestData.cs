using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.IntegrationTests.Infrastructure;

/// <summary>Datos semilla conocidos que comparten todas las pruebas de integración.</summary>
public static class TestData
{
    public const long CcConRelaciones = 1001;   // tiene teléfono y estudio
    public const long CcSinRelaciones = 1003;   // se puede borrar
    public const int ProfConEstudios = 1;
    public const int ProfSinEstudios = 2;
    public const string NumTelefono = "3001112233";

    public static void Seed(PersonaDbContext db)
    {
        db.Personas.AddRange(
            new Persona { Cc = 1001, Nombre = "Ana", Apellido = "Gomez", Genero = "F", Edad = 30 },
            new Persona { Cc = 1002, Nombre = "Luis", Apellido = "Perez", Genero = "M", Edad = 45 },
            new Persona { Cc = 1003, Nombre = "Sofia", Apellido = "Ruiz", Genero = "F", Edad = 22 });

        db.Profesiones.AddRange(
            new Profesion { Id = 1, Nom = "Ingenieria de Sistemas", Des = "Software" },
            new Profesion { Id = 2, Nom = "Medicina", Des = "Salud" });

        db.Telefonos.Add(new Telefono { Num = NumTelefono, Oper = "Claro", Duenio = 1001 });

        db.Estudios.Add(new Estudio { IdProf = 1, CcPer = 1001, Fecha = new DateTime(2015, 6, 1), Univer = "Universidad Nacional" });

        db.SaveChanges();
    }
}
