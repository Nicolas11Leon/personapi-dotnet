using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace personapi_dotnet.Models.Entities;

public partial class Profesion
{
    [Range(1, int.MaxValue)]
    public int Id { get; set; }

    [Required, StringLength(90)]
    [Display(Name = "Nombre")]
    public string Nom { get; set; } = null!;

    [Display(Name = "Descripción")]
    public string? Des { get; set; }

    [JsonIgnore] public virtual ICollection<Estudio> Estudios { get; set; } = new List<Estudio>();
}
