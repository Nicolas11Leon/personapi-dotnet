using System.ComponentModel.DataAnnotations;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.UnitTests.Models;

/// <summary>
/// Valida las DataAnnotations de las entidades (las mismas reglas que aplica
/// ASP.NET Core al hacer model binding en MVC y en la API).
/// </summary>
public class EntityValidationTests
{
    private static List<ValidationResult> Validar(object modelo)
    {
        var resultados = new List<ValidationResult>();
        Validator.TryValidateObject(modelo, new ValidationContext(modelo), resultados, validateAllProperties: true);
        return resultados;
    }

    private static Persona PersonaValida() =>
        new() { Cc = 1, Nombre = "Ana", Apellido = "Gomez", Genero = "F", Edad = 30 };

    [Fact]
    public void Persona_Valida_NoTieneErrores() => Assert.Empty(Validar(PersonaValida()));

    [Theory]
    [InlineData("M")]
    [InlineData("F")]
    [InlineData("m")]
    [InlineData("f")]
    public void Persona_GeneroPermitido_EsValido(string genero)
    {
        var p = PersonaValida();
        p.Genero = genero;
        Assert.Empty(Validar(p));
    }

    [Theory]
    [InlineData("X")]
    [InlineData("MF")]
    [InlineData("")]
    public void Persona_GeneroInvalido_TieneError(string genero)
    {
        var p = PersonaValida();
        p.Genero = genero;
        Assert.Contains(Validar(p), r => r.MemberNames.Contains(nameof(Persona.Genero)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Persona_CedulaNoPositiva_TieneError(long cc)
    {
        var p = PersonaValida();
        p.Cc = cc;
        Assert.Contains(Validar(p), r => r.MemberNames.Contains(nameof(Persona.Cc)));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(151)]
    public void Persona_EdadFueraDeRango_TieneError(int edad)
    {
        var p = PersonaValida();
        p.Edad = edad;
        Assert.Contains(Validar(p), r => r.MemberNames.Contains(nameof(Persona.Edad)));
    }

    [Fact]
    public void Persona_NombreMuyLargo_TieneError()
    {
        var p = PersonaValida();
        p.Nombre = new string('a', 46);
        Assert.Contains(Validar(p), r => r.MemberNames.Contains(nameof(Persona.Nombre)));
    }

    [Fact]
    public void Profesion_IdCero_TieneError()
    {
        var prof = new Profesion { Id = 0, Nom = "Ingenieria" };
        Assert.Contains(Validar(prof), r => r.MemberNames.Contains(nameof(Profesion.Id)));
    }

    [Fact]
    public void Telefono_SinOperador_TieneError()
    {
        var tel = new Telefono { Num = "3001234567", Oper = null!, Duenio = 1 };
        Assert.Contains(Validar(tel), r => r.MemberNames.Contains(nameof(Telefono.Oper)));
    }

    [Fact]
    public void Estudio_UniversidadMuyLarga_TieneError()
    {
        var est = new Estudio { IdProf = 1, CcPer = 1, Univer = new string('u', 51) };
        Assert.Contains(Validar(est), r => r.MemberNames.Contains(nameof(Estudio.Univer)));
    }
}
