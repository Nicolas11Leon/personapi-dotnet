using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.Repositories.Interfaces;
namespace personapi_dotnet.Controllers;

public class PersonasController : Controller
{
    private readonly IPersonaRepository _repo;
    public PersonasController(IPersonaRepository repo) => _repo = repo;

    public async Task<IActionResult> Index() => View(await _repo.GetAllAsync());

    public async Task<IActionResult> Details(long id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? NotFound() : View(e);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Persona model)
    {
        if (await _repo.ExistsAsync(model.Cc)) ModelState.AddModelError(nameof(model.Cc), "Ya existe una persona con esa cédula.");
        if (!ModelState.IsValid) return View(model);
        await _repo.AddAsync(model);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(long id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? NotFound() : View(e);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, Persona model)
    {
        if (id != model.Cc) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        if (!await _repo.ExistsAsync(id)) return NotFound();
        await _repo.UpdateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(long id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? NotFound() : View(e);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        try { await _repo.DeleteAsync(id); }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "No se puede eliminar: la persona tiene teléfonos o estudios asociados.");
            return View(await _repo.GetByIdAsync(id));
        }
        return RedirectToAction(nameof(Index));
    }
}
