using personapi_dotnet.Models.Entities;
namespace personapi_dotnet.Repositories.Interfaces;

public interface ITelefonoRepository
{
    Task<IEnumerable<Telefono>> GetAllAsync();
    Task<Telefono?> GetByIdAsync(string num);
    Task<bool> ExistsAsync(string num);
    Task AddAsync(Telefono entity);
    Task UpdateAsync(Telefono entity);
    Task DeleteAsync(string num);
}
