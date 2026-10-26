using Microsoft.EntityFrameworkCore;

namespace personapi_dotnet.Models.Entities;

public partial class PersonaDbContext : DbContext
{
    public PersonaDbContext(DbContextOptions<PersonaDbContext> options) : base(options) { }

    public virtual DbSet<Persona> Personas { get; set; }
    public virtual DbSet<Profesion> Profesiones { get; set; }
    public virtual DbSet<Estudio> Estudios { get; set; }
    public virtual DbSet<Telefono> Telefonos { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Persona>(e =>
        {
            e.ToTable("persona"); e.HasKey(x => x.Cc);
            e.Property(x => x.Cc).HasColumnName("cc").ValueGeneratedNever();
            e.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(45).IsUnicode(false);
            e.Property(x => x.Apellido).HasColumnName("apellido").HasMaxLength(45).IsUnicode(false);
            e.Property(x => x.Genero).HasColumnName("genero").HasMaxLength(1).IsUnicode(false).IsFixedLength();
            e.Property(x => x.Edad).HasColumnName("edad");
        });
        mb.Entity<Profesion>(e =>
        {
            e.ToTable("profesion"); e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
            e.Property(x => x.Nom).HasColumnName("nom").HasMaxLength(90).IsUnicode(false);
            e.Property(x => x.Des).HasColumnName("des").HasColumnType("text");
        });
        mb.Entity<Estudio>(e =>
        {
            e.ToTable("estudios"); e.HasKey(x => new { x.IdProf, x.CcPer });
            e.Property(x => x.IdProf).HasColumnName("id_prof");
            e.Property(x => x.CcPer).HasColumnName("cc_per");
            e.Property(x => x.Fecha).HasColumnName("fecha").HasColumnType("date");
            e.Property(x => x.Univer).HasColumnName("univer").HasMaxLength(50).IsUnicode(false);
            e.HasOne(x => x.CcPerNavigation).WithMany(p => p.Estudios).HasForeignKey(x => x.CcPer).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_estudio_persona");
            e.HasOne(x => x.IdProfNavigation).WithMany(p => p.Estudios).HasForeignKey(x => x.IdProf).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_estudio_profesion");
        });
        mb.Entity<Telefono>(e =>
        {
            e.ToTable("telefono"); e.HasKey(x => x.Num);
            e.Property(x => x.Num).HasColumnName("num").HasMaxLength(15).IsUnicode(false);
            e.Property(x => x.Oper).HasColumnName("oper").HasMaxLength(45).IsUnicode(false);
            e.Property(x => x.Duenio).HasColumnName("duenio");
            e.HasOne(x => x.DuenioNavigation).WithMany(p => p.Telefonos).HasForeignKey(x => x.Duenio).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_telefono_persona");
        });
    }
}
