using personapi_dotnet.Models.Entities;
namespace personapi_dotnet.Repositories.Interfaces;

public interface IEstudioRepository
{
    Task<IEnumerable<Estudio>> GetAllAsync();
    Task<Estudio?> GetByIdAsync(int idProf, long ccPer);
    Task<bool> ExistsAsync(int idProf, long ccPer);
    Task AddAsync(Estudio entity);
    Task UpdateAsync(Estudio entity);
    Task DeleteAsync(int idProf, long ccPer);
}
