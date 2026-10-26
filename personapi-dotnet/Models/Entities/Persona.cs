using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace personapi_dotnet.Models.Entities;

public partial class Persona
{
    [Display(Name = "Cédula")]
    [Range(1, long.MaxValue, ErrorMessage = "La cédula debe ser mayor que cero")]
    public long Cc { get; set; }

    [Required, StringLength(45)]
    public string Nombre { get; set; } = null!;

    [Required, StringLength(45)]
    public string Apellido { get; set; } = null!;

    [Required, RegularExpression("^[MFmf]$", ErrorMessage = "Género: M o F")]
    [Display(Name = "Género")]
    public string Genero { get; set; } = null!;

    [Range(0, 150)]
    public int? Edad { get; set; }

    [JsonIgnore] public virtual ICollection<Estudio> Estudios { get; set; } = new List<Estudio>();
    [JsonIgnore] public virtual ICollection<Telefono> Telefonos { get; set; } = new List<Telefono>();
}
