using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.Repositories.Interfaces;
namespace personapi_dotnet.Repositories;

public class EstudioRepository : IEstudioRepository
{
    private readonly PersonaDbContext _db;
    public EstudioRepository(PersonaDbContext db) => _db = db;

    public async Task<IEnumerable<Estudio>> GetAllAsync() =>
        await _db.Estudios.AsNoTracking().Include(e => e.CcPerNavigation).Include(e => e.IdProfNavigation)
            .OrderBy(e => e.CcPer).ThenBy(e => e.IdProf).ToListAsync();
    public Task<Estudio?> GetByIdAsync(int idProf, long ccPer) =>
        _db.Estudios.AsNoTracking().Include(e => e.CcPerNavigation).Include(e => e.IdProfNavigation)
            .FirstOrDefaultAsync(e => e.IdProf == idProf && e.CcPer == ccPer);
    public Task<bool> ExistsAsync(int idProf, long ccPer) => _db.Estudios.AnyAsync(e => e.IdProf == idProf && e.CcPer == ccPer);
    public async Task AddAsync(Estudio e) { _db.Estudios.Add(e); await _db.SaveChangesAsync(); }
    public async Task UpdateAsync(Estudio e) { _db.Estudios.Update(e); await _db.SaveChangesAsync(); }
    public async Task DeleteAsync(int idProf, long ccPer)
    {
        var e = await _db.Estudios.FindAsync(idProf, ccPer);
        if (e == null) return;
        _db.Estudios.Remove(e);
        await _db.SaveChangesAsync();
    }
}
