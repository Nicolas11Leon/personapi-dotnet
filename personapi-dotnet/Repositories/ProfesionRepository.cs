using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.Repositories.Interfaces;
namespace personapi_dotnet.Repositories;

public class ProfesionRepository : IProfesionRepository
{
    private readonly PersonaDbContext _db;
    public ProfesionRepository(PersonaDbContext db) => _db = db;

    public async Task<IEnumerable<Profesion>> GetAllAsync() => await _db.Profesiones.AsNoTracking().OrderBy(p => p.Id).ToListAsync();
    public Task<Profesion?> GetByIdAsync(int id) => _db.Profesiones.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    public Task<bool> ExistsAsync(int id) => _db.Profesiones.AnyAsync(p => p.Id == id);
    public async Task AddAsync(Profesion e) { _db.Profesiones.Add(e); await _db.SaveChangesAsync(); }
    public async Task UpdateAsync(Profesion e) { _db.Profesiones.Update(e); await _db.SaveChangesAsync(); }
    public async Task DeleteAsync(int id)
    {
        var e = await _db.Profesiones.FindAsync(id);
        if (e == null) return;
        _db.Profesiones.Remove(e);
        await _db.SaveChangesAsync();
    }
}
