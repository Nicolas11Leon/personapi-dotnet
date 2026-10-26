using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace personapi_dotnet.Models.Entities;

public partial class Telefono
{
    [Required, StringLength(15), Display(Name = "Número")]
    public string Num { get; set; } = null!;

    [Required, StringLength(45), Display(Name = "Operador")]
    public string Oper { get; set; } = null!;

    [Display(Name = "Dueño (cédula)")]
    public long Duenio { get; set; }

    [JsonIgnore] public virtual Persona? DuenioNavigation { get; set; }
}
