using personapi_dotnet.Models.Entities;
namespace personapi_dotnet.Repositories.Interfaces;

public interface IProfesionRepository
{
    Task<IEnumerable<Profesion>> GetAllAsync();
    Task<Profesion?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(Profesion entity);
    Task UpdateAsync(Profesion entity);
    Task DeleteAsync(int id);
}
