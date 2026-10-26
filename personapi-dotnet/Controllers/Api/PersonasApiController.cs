using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.Repositories.Interfaces;
namespace personapi_dotnet.Controllers.Api;

[ApiController]
[Route("api/personas")]
public class PersonasApiController : ControllerBase
{
    private readonly IPersonaRepository _repo;
    public PersonasApiController(IPersonaRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Persona>>> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpGet("{cc:long}")]
    public async Task<ActionResult<Persona>> Get(long cc)
    {
        var e = await _repo.GetByIdAsync(cc);
        return e == null ? NotFound() : Ok(e);
    }

    [HttpPost]
    public async Task<ActionResult<Persona>> Create(Persona model)
    {
        if (await _repo.ExistsAsync(model.Cc)) return Conflict("Ya existe un registro con esa llave.");
        try { await _repo.AddAsync(model); }
        catch (DbUpdateException) { return BadRequest("No se pudo guardar: verifique las llaves foráneas."); }
        return CreatedAtAction(nameof(Get), new { cc = model.Cc }, model);
    }

    [HttpPut("{cc:long}")]
    public async Task<IActionResult> Update(long cc, Persona model)
    {
        if (cc != model.Cc) return BadRequest("La llave de la ruta no coincide con el cuerpo.");
        if (!await _repo.ExistsAsync(cc)) return NotFound();
        try { await _repo.UpdateAsync(model); }
        catch (DbUpdateException) { return BadRequest("No se pudo guardar: verifique las llaves foráneas."); }
        return NoContent();
    }

    [HttpDelete("{cc:long}")]
    public async Task<IActionResult> Delete(long cc)
    {
        if (!await _repo.ExistsAsync(cc)) return NotFound();
        try { await _repo.DeleteAsync(cc); }
        catch (DbUpdateException) { return Conflict("No se puede eliminar: existen registros relacionados."); }
        return NoContent();
    }
}
