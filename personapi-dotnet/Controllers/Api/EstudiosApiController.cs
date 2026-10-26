using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.Repositories.Interfaces;
namespace personapi_dotnet.Controllers.Api;

[ApiController]
[Route("api/estudios")]
public class EstudiosApiController : ControllerBase
{
    private readonly IEstudioRepository _repo;
    public EstudiosApiController(IEstudioRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Estudio>>> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpGet("{idProf:int}/{ccPer:long}")]
    public async Task<ActionResult<Estudio>> Get(int idProf, long ccPer)
    {
        var e = await _repo.GetByIdAsync(idProf, ccPer);
        return e == null ? NotFound() : Ok(e);
    }

    [HttpPost]
    public async Task<ActionResult<Estudio>> Create(Estudio model)
    {
        if (await _repo.ExistsAsync(model.IdProf, model.CcPer)) return Conflict("Ya existe un registro con esa llave.");
        try { await _repo.AddAsync(model); }
        catch (DbUpdateException) { return BadRequest("No se pudo guardar: verifique las llaves foráneas."); }
        return CreatedAtAction(nameof(Get), new { idProf = model.IdProf, ccPer = model.CcPer }, model);
    }

    [HttpPut("{idProf:int}/{ccPer:long}")]
    public async Task<IActionResult> Update(int idProf, long ccPer, Estudio model)
    {
        if (idProf != model.IdProf || ccPer != model.CcPer) return BadRequest("La llave de la ruta no coincide con el cuerpo.");
        if (!await _repo.ExistsAsync(idProf, ccPer)) return NotFound();
        try { await _repo.UpdateAsync(model); }
        catch (DbUpdateException) { return BadRequest("No se pudo guardar: verifique las llaves foráneas."); }
        return NoContent();
    }

    [HttpDelete("{idProf:int}/{ccPer:long}")]
    public async Task<IActionResult> Delete(int idProf, long ccPer)
    {
        if (!await _repo.ExistsAsync(idProf, ccPer)) return NotFound();
        try { await _repo.DeleteAsync(idProf, ccPer); }
        catch (DbUpdateException) { return Conflict("No se puede eliminar: existen registros relacionados."); }
        return NoContent();
    }
}
