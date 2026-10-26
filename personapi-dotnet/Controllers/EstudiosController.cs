using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using personapi_dotnet.Models.Entities;
using personapi_dotnet.Repositories.Interfaces;
namespace personapi_dotnet.Controllers;

public class EstudiosController : Controller
{
    private readonly IEstudioRepository _repo;
    private readonly IPersonaRepository _personas;
    private readonly IProfesionRepository _profesiones;
    public EstudiosController(IEstudioRepository repo, IPersonaRepository personas, IProfesionRepository profesiones)
    { _repo = repo; _personas = personas; _profesiones = profesiones; }

    private async Task LoadLookups(int? prof = null, long? per = null)
    {
        ViewBag.Personas = new SelectList((await _personas.GetAllAsync()).Select(p => new { p.Cc, Texto = $"{p.Cc} - {p.Nombre} {p.Apellido}" }), "Cc", "Texto", per);
        ViewBag.Profesiones = new SelectList(await _profesiones.GetAllAsync(), "Id", "Nom", prof);
    }

    public async Task<IActionResult> Index() => View(await _repo.GetAllAsync());

    public async Task<IActionResult> Details(int idProf, long ccPer)
    {
        var e = await _repo.GetByIdAsync(idProf, ccPer);
        return e == null ? NotFound() : View(e);
    }

    public async Task<IActionResult> Create() { await LoadLookups(); return View(); }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Estudio model)
    {
        if (await _repo.ExistsAsync(model.IdProf, model.CcPer)) ModelState.AddModelError("", "Esa persona ya tiene registrada esa profesión.");
        if (!ModelState.IsValid) { await LoadLookups(model.IdProf, model.CcPer); return View(model); }
        await _repo.AddAsync(model);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int idProf, long ccPer)
    {
        var e = await _repo.GetByIdAsync(idProf, ccPer);
        return e == null ? NotFound() : View(e);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Estudio model)
    {
        if (!ModelState.IsValid)
        {
            return View(await _repo.GetByIdAsync(model.IdProf, model.CcPer) is { } cur ? Merge(cur, model) : model);
        }
        if (!await _repo.ExistsAsync(model.IdProf, model.CcPer)) return NotFound();
        await _repo.UpdateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    private static Estudio Merge(Estudio cur, Estudio m) { cur.Fecha = m.Fecha; cur.Univer = m.Univer; return cur; }

    public async Task<IActionResult> Delete(int idProf, long ccPer)
    {
        var e = await _repo.GetByIdAsync(idProf, ccPer);
        return e == null ? NotFound() : View(e);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int idProf, long ccPer)
    {
        await _repo.DeleteAsync(idProf, ccPer);
        return RedirectToAction(nameof(Index));
    }
}
