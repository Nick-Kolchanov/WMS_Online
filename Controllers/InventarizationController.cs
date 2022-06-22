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
        public IActionResult AddNomenclature(int? id)
        {
            ViewData["Types"] = new SelectList(_db.NomenclatureTypes.ToList(), "Id", "Name", 1);

            if (id == null)
                return View();

            var nomenclature = _db.Nomenclatures.Find(id);
            if (nomenclature == null)
                return NotFound();

            ViewData["Types"] = new SelectList(_db.NomenclatureTypes.ToList(), "Id", "Name", nomenclature.TypeId);
            return View(nomenclature);
        }

        [HttpPost]
        public async Task<IActionResult> AddNomenclature(Nomenclature nomenclature, int worth)
        {
            ViewData["Types"] = new SelectList(_db.NomenclatureTypes.ToList(), "Id", "Name", nomenclature.TypeId);

            if (!ModelState.IsValid)
            {
                return View(nomenclature);
            }

            if (worth <= 0)
            {
                ModelState.AddModelError("", "Цена должна быть положительной");
            }

            if (!ModelState.IsValid)
            {
                return View(nomenclature);
            }

            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Nomenclature> newNomenclature;
            if (nomenclature.Id != 0)
                newNomenclature = _db.Nomenclatures.Update(nomenclature);                
            else
                newNomenclature = _db.Nomenclatures.Add(nomenclature);
            await _db.SaveChangesAsync();

            _db.ProductWorths.Add(new ProductWorth() { Date = DateOnly.FromDateTime(DateTime.Now), NomenclatureId = newNomenclature.Entity.Id, Worth = worth });

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveNomenclature(int? id)
        {
            if (id == null)
                return NotFound();

            Nomenclature nomenclature = new Nomenclature { Id = id.Value };
            _db.Entry(nomenclature).State = EntityState.Deleted;
            await _db.SaveChangesAsync();            

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult AddNomenclatureProperty(int id)
        {
            var np = new NomenclatureProperty() { NomenclatureId = id };
            ViewData["Properties"] = new SelectList(_db.Properties.ToList(), "Id", "Name", 1);
            return View(np);
        }

        [HttpPost]
        public async Task<IActionResult> AddNomenclatureProperty(NomenclatureProperty nomenclatureProperty)
        {
            ViewData["Properties"] = new SelectList(_db.Properties.ToList(), "Id", "Name", nomenclatureProperty.PropertyId);

            if (!ModelState.IsValid)
            {
                return View(nomenclatureProperty);
            }

            _db.NomenclatureProperties.Add(nomenclatureProperty);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", new { id = nomenclatureProperty.NomenclatureId});
        }

        [HttpGet]
        public IActionResult ChangeNomenclatureProperty(int nId, int pId)
        {
            var nomenclatureProperty = _db.NomenclatureProperties.Where(np => np.NomenclatureId == nId && np.PropertyId == pId).First();
            return View(nomenclatureProperty);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeNomenclatureProperty(NomenclatureProperty nomenclatureProperty, int nId, int pId)
        {
            if (!ModelState.IsValid)
            {
                return View(nomenclatureProperty);
            }

            _db.NomenclatureProperties.Update(nomenclatureProperty);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", new { id = nomenclatureProperty.NomenclatureId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveNomenclatureProperty(int nId, int pId)
        {
            NomenclatureProperty nomenclatureProperty = new NomenclatureProperty { NomenclatureId = nId, PropertyId = pId };
            _db.Entry(nomenclatureProperty).State = EntityState.Deleted;
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", new { id = nomenclatureProperty.NomenclatureId });
        }
    }
}