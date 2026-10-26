using personapi_dotnet.Models.Entities;
namespace personapi_dotnet.Repositories.Interfaces;

public interface IPersonaRepository
{
    Task<IEnumerable<Persona>> GetAllAsync();
    Task<Persona?> GetByIdAsync(long cc);
    Task<bool> ExistsAsync(long cc);
    Task AddAsync(Persona entity);
    Task UpdateAsync(Persona entity);
    Task DeleteAsync(long cc);
}
