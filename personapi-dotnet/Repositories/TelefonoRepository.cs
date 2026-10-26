using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.Repositories.Interfaces;
namespace personapi_dotnet.Repositories;

public class TelefonoRepository : ITelefonoRepository
{
    private readonly PersonaDbContext _db;
    public TelefonoRepository(PersonaDbContext db) => _db = db;

    public async Task<IEnumerable<Telefono>> GetAllAsync() =>
        await _db.Telefonos.AsNoTracking().Include(t => t.DuenioNavigation).OrderBy(t => t.Num).ToListAsync();
    public Task<Telefono?> GetByIdAsync(string num) =>
        _db.Telefonos.AsNoTracking().Include(t => t.DuenioNavigation).FirstOrDefaultAsync(t => t.Num == num);
    public Task<bool> ExistsAsync(string num) => _db.Telefonos.AnyAsync(t => t.Num == num);
    public async Task AddAsync(Telefono e) { _db.Telefonos.Add(e); await _db.SaveChangesAsync(); }
    public async Task UpdateAsync(Telefono e) { _db.Telefonos.Update(e); await _db.SaveChangesAsync(); }
    public async Task DeleteAsync(string num)
    {
        var e = await _db.Telefonos.FindAsync(num);
        if (e == null) return;
        _db.Telefonos.Remove(e);
        await _db.SaveChangesAsync();
    }
}
