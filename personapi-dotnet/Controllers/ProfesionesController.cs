using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.Repositories.Interfaces;
namespace personapi_dotnet.Controllers;

public class ProfesionesController : Controller
{
    private readonly IProfesionRepository _repo;
    public ProfesionesController(IProfesionRepository repo) => _repo = repo;

    public async Task<IActionResult> Index() => View(await _repo.GetAllAsync());

    public async Task<IActionResult> Details(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? NotFound() : View(e);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Profesion model)
    {
        if (await _repo.ExistsAsync(model.Id)) ModelState.AddModelError(nameof(model.Id), "Ya existe una profesión con ese id.");
        if (!ModelState.IsValid) return View(model);
        await _repo.AddAsync(model);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? NotFound() : View(e);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Profesion model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        if (!await _repo.ExistsAsync(id)) return NotFound();
        await _repo.UpdateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? NotFound() : View(e);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try { await _repo.DeleteAsync(id); }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "No se puede eliminar: la profesión tiene estudios asociados.");
            return View(await _repo.GetByIdAsync(id));
        }
        return RedirectToAction(nameof(Index));
    }
}
