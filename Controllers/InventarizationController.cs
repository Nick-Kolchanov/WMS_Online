using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS_Online.Data;
using WMS_Online.Models;
using WMS_Online.Models.InventarizationViewModels;

namespace WMS_Online.Controllers
{
    [Authorize(Policy = "IsAdmin")]
    public class InventarizationController : Controller
    {
        private readonly ILogger<InventarizationController> _logger;
        private readonly WmsDbContext _db;        

        public InventarizationController(ILogger<InventarizationController> logger, WmsDbContext context)
        {
            _logger = logger;
            _db = context;
        }

        public async Task<IActionResult> Index(int? id, string name, int warehouse = 0, SortState sortOrder = SortState.StartDateDesc, int page = 1, int pageSize = 5)
        {
            IQueryable<Inventarization> inventarizations = _db.Inventarizations;
            
            if (warehouse != 0)
            {
                inventarizations = inventarizations.Where(n => n.WarehouseId == warehouse);
            }

            if (!string.IsNullOrEmpty(name))
            {
                inventarizations = inventarizations.Where(n => n.StartDate.ToString().Contains(name) || (n.EndDate.HasValue && n.EndDate.Value.ToString().Contains(name)));
            }

            var count = await inventarizations.CountAsync();
            inventarizations = inventarizations.Skip((page - 1) * pageSize).Take(pageSize);

            switch (sortOrder)
            {
                case SortState.EndDateAsc:
                    inventarizations = inventarizations.OrderBy(s => s.EndDate);
                    break;
                case SortState.EndDateDesc:
                    inventarizations = inventarizations.OrderByDescending(s => s.EndDate);
                    break;
                case SortState.StartDateAsc:
                    inventarizations = inventarizations.OrderBy(s => s.StartDate);
                    break;
                default:
                    inventarizations = inventarizations.OrderByDescending(s => s.StartDate);
                    break;
            }

            IndexViewModel viewModel = new IndexViewModel(
               inventarizations.ToList(),
               null,
               id,
               new PageViewModel(count, page, pageSize),
               new FilterViewModel(_db.Warehouses.ToList(), warehouse, name),
               new SortViewModel(sortOrder)
            );

            if (id != null && id != -1)
                viewModel.Discrepancies = _db.Discrepancies.Where(d => d.InventarizationId == id).ToList();

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult AddInventarization(int? id)
        {
            ViewData["Warehouses"] = new SelectList(_db.Warehouses.ToList(), "Id", "Address", 1);
            ViewData["Reasons"] = new SelectList(_db.InventarizationReasons.ToList(), "Id", "Name", 1);

            if (id == null)
                return View();

            var inventarization = _db.Inventarizations.Find(id);
            if (inventarization == null)
                return NotFound();

            ViewData["Warehouses"] = new SelectList(_db.Warehouses.ToList(), "Id", "Address", inventarization.WarehouseId);
            ViewData["Reasons"] = new SelectList(_db.InventarizationReasons.ToList(), "Id", "Name", inventarization.ReasonId);
            return View(inventarization);
        }

        [HttpPost]
        public async Task<IActionResult> AddInventarization(Inventarization inventarization, DateTime StartDate, DateTime? EndDate)
        {
            ViewData["Warehouses"] = new SelectList(_db.Warehouses.ToList(), "Id", "Address", inventarization.WarehouseId);
            ViewData["Reasons"] = new SelectList(_db.InventarizationReasons.ToList(), "Id", "Name", inventarization.ReasonId);

            if (!ModelState.IsValid)
            {
                return View(inventarization);
            }

            if (inventarization.StartDate > inventarization.EndDate)
            {
                ModelState.AddModelError("", "Дата начала не может быть позже даты конца инвентаризации");
            }

            if (!ModelState.IsValid)
            {
                return View(inventarization);
            }

            inventarization.StartDate = DateOnly.FromDateTime(StartDate);
            if (EndDate == null)
            {
                inventarization.EndDate = null;
            }
            else
            {
                inventarization.EndDate = DateOnly.FromDateTime(EndDate.Value);
            }

            if (inventarization.Id != 0)
                _db.Inventarizations.Update(inventarization);                
            else
                _db.Inventarizations.Add(inventarization);

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveInventarization(int? id)
        {
            if (id == null)
                return NotFound();

            Inventarization inventarization = new Inventarization { Id = id.Value };
            _db.Entry(inventarization).State = EntityState.Deleted;
            await _db.SaveChangesAsync();            

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult AddDiscrepancy(int id)
        {
            var disc = new Discrepancy() { InventarizationId = id };
            ViewData["Products"] = new SelectList(_db.Products.Select(p => new {Id = p.Id, Name = p.Nomenclature.Name}).ToList(), "Id", "Name", 1);
            ViewData["Statuses"] = new SelectList(_db.DiscrepancyStatuses.ToList(), "Id", "Name", 1);
            ViewData["Types"] = new SelectList(_db.DiscrepancyTypes.ToList(), "Id", "Name", 1);
            return View(disc);
        }

        [HttpPost]
        public async Task<IActionResult> AddDiscrepancy(Discrepancy discrepancy)
        {
            ViewData["Products"] = new SelectList(_db.Products.Select(p => new { Id = p.Id, Name = p.Nomenclature.Name }).ToList(), "Id", "Name", discrepancy.ProductId);
            ViewData["Statuses"] = new SelectList(_db.DiscrepancyStatuses.ToList(), "Id", "Name", discrepancy.StatusId);
            ViewData["Types"] = new SelectList(_db.DiscrepancyTypes.ToList(), "Id", "Name", discrepancy.TypeId);

            if (!ModelState.IsValid)
            {
                return View(discrepancy);
            }

            _db.Discrepancies.Add(discrepancy);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", new { id = discrepancy.InventarizationId});
        }

        [HttpGet]
        public IActionResult ChangeDiscrepancy(int invId, int pId)
        {
            var discrepancy = _db.Discrepancies.Where(d => d.InventarizationId == invId && d.ProductId == pId).First();
            ViewData["Statuses"] = new SelectList(_db.DiscrepancyStatuses.ToList(), "Id", "Name", discrepancy.StatusId);
            ViewData["Types"] = new SelectList(_db.DiscrepancyTypes.ToList(), "Id", "Name", discrepancy.TypeId);
            return View(discrepancy);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeDiscrepancy(Discrepancy discrepancy)
        {
            if (!ModelState.IsValid)
            {
                return View(discrepancy);
            }

            _db.Discrepancies.Update(discrepancy);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", new { id = discrepancy.InventarizationId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveDiscrepancy(int invId, int pId)
        {
            Discrepancy discrepancy = new Discrepancy { InventarizationId = invId, ProductId = pId };
            _db.Entry(discrepancy).State = EntityState.Deleted;
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", new { id = discrepancy.InventarizationId });
        }
    }
}