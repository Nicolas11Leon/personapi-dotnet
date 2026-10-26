using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.Repositories.Interfaces;
namespace personapi_dotnet.Controllers;

public class TelefonosController : Controller
{
    private readonly ITelefonoRepository _repo;
    private readonly IPersonaRepository _personas;
    public TelefonosController(ITelefonoRepository repo, IPersonaRepository personas) { _repo = repo; _personas = personas; }

    private async Task LoadLookups(long? selected = null) =>
        ViewBag.Personas = new SelectList((await _personas.GetAllAsync()).Select(p => new { p.Cc, Texto = $"{p.Cc} - {p.Nombre} {p.Apellido}" }), "Cc", "Texto", selected);

    public async Task<IActionResult> Index() => View(await _repo.GetAllAsync());

    public async Task<IActionResult> Details(string id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? NotFound() : View(e);
    }

    public async Task<IActionResult> Create() { await LoadLookups(); return View(); }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Telefono model)
    {
        if (!string.IsNullOrEmpty(model.Num) && await _repo.ExistsAsync(model.Num)) ModelState.AddModelError(nameof(model.Num), "Ya existe ese teléfono.");
        if (!ModelState.IsValid) { await LoadLookups(model.Duenio); return View(model); }
        await _repo.AddAsync(model);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        var e = await _repo.GetByIdAsync(id);
        if (e == null) return NotFound();
        await LoadLookups(e.Duenio);
        return View(e);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Telefono model)
    {
        if (id != model.Num) return BadRequest();
        if (!ModelState.IsValid) { await LoadLookups(model.Duenio); return View(model); }
        if (!await _repo.ExistsAsync(id)) return NotFound();
        await _repo.UpdateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(string id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? NotFound() : View(e);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        await _repo.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
