using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace personapi_dotnet.Models.Entities;

public partial class Estudio
{
    [Display(Name = "Profesión")]
    public int IdProf { get; set; }

    [Display(Name = "Persona (cédula)")]
    public long CcPer { get; set; }

    [DataType(DataType.Date)]
    public DateTime? Fecha { get; set; }

    [StringLength(50), Display(Name = "Universidad")]
    public string? Univer { get; set; }

    [JsonIgnore] public virtual Persona? CcPerNavigation { get; set; }
    [JsonIgnore] public virtual Profesion? IdProfNavigation { get; set; }
}
