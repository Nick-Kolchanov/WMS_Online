using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS_Online.Data;
using WMS_Online.Models;
using WMS_Online.Models.NomenclatureViewModels;

namespace WMS_Online.Controllers
{
    public class NomenclatureController : Controller
    {
        private readonly ILogger<NomenclatureController> _logger;
        private readonly WmsDbContext _db;        

        public NomenclatureController(ILogger<NomenclatureController> logger, WmsDbContext context)
        {
            _logger = logger;
            _db = context;
        }

        public async Task<IActionResult> Index(int? id, string name, int type = 0, SortState sortOrder = SortState.NameAsc, int page = 1, int pageSize = 5)
        {
            IQueryable<Nomenclature> nomenclatures = _db.Nomenclatures;
            
            if (type != 0)
            {
                nomenclatures = nomenclatures.Where(n => n.TypeId == type);
            }

            if (!string.IsNullOrEmpty(name))
            {
                nomenclatures = nomenclatures.Where(n => n.Name.Contains(name) || n.ProductWorths.OrderByDescending(pw => pw.Date).FirstOrDefault()!.Worth.ToString().Contains(name));
            }

            var count = await nomenclatures.CountAsync();
            nomenclatures = nomenclatures.Skip((page - 1) * pageSize).Take(pageSize);

            switch (sortOrder)
            {
                case SortState.WorthAsc:
                    nomenclatures = nomenclatures.OrderBy(s => s.ProductWorths.OrderByDescending(pw => pw.Date).FirstOrDefault()!.Worth);
                    break;
                case SortState.WorthDesc:
                    nomenclatures = nomenclatures.OrderByDescending(s => s.ProductWorths.OrderByDescending(pw => pw.Date).FirstOrDefault()!.Worth);
                    break;
                case SortState.NameDesc:
                    nomenclatures = nomenclatures.OrderByDescending(s => s.Name);
                    break;
                default:
                    nomenclatures = nomenclatures.OrderBy(s => s.Name);
                    break;
            }

            IndexViewModel viewModel = new IndexViewModel(
               nomenclatures.ToList(),
               null,
               new PageViewModel(count, page, pageSize),
               new FilterViewModel(_db.NomenclatureTypes.ToList(), type, name),
               new SortViewModel(sortOrder)
            );

            if (id != null && id != -1)
                viewModel.NomenclatureProperties = _db.NomenclatureProperties.Where(np => np.NomenclatureId == id).ToList();

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
        public IActionResult AddNomenclatureProperty(int? id)
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
        public async Task<IActionResult> AddNomenclatureProperty(Nomenclature nomenclature, int worth)
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
        public async Task<IActionResult> RemoveNomenclatureProperty(int? id)
        {
            if (id == null)
                return NotFound();

            Nomenclature nomenclature = new Nomenclature { Id = id.Value };
            _db.Entry(nomenclature).State = EntityState.Deleted;
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}