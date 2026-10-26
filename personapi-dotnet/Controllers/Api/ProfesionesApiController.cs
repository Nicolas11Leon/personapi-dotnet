using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.Repositories.Interfaces;
namespace personapi_dotnet.Controllers.Api;

[ApiController]
[Route("api/profesiones")]
public class ProfesionesApiController : ControllerBase
{
    private readonly IProfesionRepository _repo;
    public ProfesionesApiController(IProfesionRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Profesion>>> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Profesion>> Get(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? NotFound() : Ok(e);
    }

    [HttpPost]
    public async Task<ActionResult<Profesion>> Create(Profesion model)
    {
        if (await _repo.ExistsAsync(model.Id)) return Conflict("Ya existe un registro con esa llave.");
        try { await _repo.AddAsync(model); }
        catch (DbUpdateException) { return BadRequest("No se pudo guardar: verifique las llaves foráneas."); }
        return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Profesion model)
    {
        if (id != model.Id) return BadRequest("La llave de la ruta no coincide con el cuerpo.");
        if (!await _repo.ExistsAsync(id)) return NotFound();
        try { await _repo.UpdateAsync(model); }
        catch (DbUpdateException) { return BadRequest("No se pudo guardar: verifique las llaves foráneas."); }
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _repo.ExistsAsync(id)) return NotFound();
        try { await _repo.DeleteAsync(id); }
        catch (DbUpdateException) { return Conflict("No se puede eliminar: existen registros relacionados."); }
        return NoContent();
    }
}
