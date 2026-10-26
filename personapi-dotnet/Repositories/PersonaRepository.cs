using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.Repositories.Interfaces;
namespace personapi_dotnet.Repositories;

public class PersonaRepository : IPersonaRepository
{
    private readonly PersonaDbContext _db;
    public PersonaRepository(PersonaDbContext db) => _db = db;

    public async Task<IEnumerable<Persona>> GetAllAsync() => await _db.Personas.AsNoTracking().OrderBy(p => p.Cc).ToListAsync();
    public Task<Persona?> GetByIdAsync(long cc) => _db.Personas.AsNoTracking().FirstOrDefaultAsync(p => p.Cc == cc);
    public Task<bool> ExistsAsync(long cc) => _db.Personas.AnyAsync(p => p.Cc == cc);
    public async Task AddAsync(Persona e) { _db.Personas.Add(e); await _db.SaveChangesAsync(); }
    public async Task UpdateAsync(Persona e) { _db.Personas.Update(e); await _db.SaveChangesAsync(); }
    public async Task DeleteAsync(long cc)
    {
        var e = await _db.Personas.FindAsync(cc);
        if (e == null) return;
        _db.Personas.Remove(e);
        await _db.SaveChangesAsync();
    }
}
