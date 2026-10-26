using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.Repositories.Interfaces;
namespace personapi_dotnet.Controllers.Api;

[ApiController]
[Route("api/telefonos")]
public class TelefonosApiController : ControllerBase
{
    private readonly ITelefonoRepository _repo;
    public TelefonosApiController(ITelefonoRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Telefono>>> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpGet("{num}")]
    public async Task<ActionResult<Telefono>> Get(string num)
    {
        var e = await _repo.GetByIdAsync(num);
        return e == null ? NotFound() : Ok(e);
    }

    [HttpPost]
    public async Task<ActionResult<Telefono>> Create(Telefono model)
    {
        if (await _repo.ExistsAsync(model.Num)) return Conflict("Ya existe un registro con esa llave.");
        try { await _repo.AddAsync(model); }
        catch (DbUpdateException) { return BadRequest("No se pudo guardar: verifique las llaves foráneas."); }
        return CreatedAtAction(nameof(Get), new { num = model.Num }, model);
    }

    [HttpPut("{num}")]
    public async Task<IActionResult> Update(string num, Telefono model)
    {
        if (num != model.Num) return BadRequest("La llave de la ruta no coincide con el cuerpo.");
        if (!await _repo.ExistsAsync(num)) return NotFound();
        try { await _repo.UpdateAsync(model); }
        catch (DbUpdateException) { return BadRequest("No se pudo guardar: verifique las llaves foráneas."); }
        return NoContent();
    }

    [HttpDelete("{num}")]
    public async Task<IActionResult> Delete(string num)
    {
        if (!await _repo.ExistsAsync(num)) return NotFound();
        try { await _repo.DeleteAsync(num); }
        catch (DbUpdateException) { return Conflict("No se puede eliminar: existen registros relacionados."); }
        return NoContent();
    }
}
