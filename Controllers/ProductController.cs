using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS_Online.Data;
using WMS_Online.Models;
using WMS_Online.Models.ProductViewModels;

namespace WMS_Online.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly WmsDbContext _db;        

        public ProductController(ILogger<ProductController> logger, WmsDbContext context)
        {
            _logger = logger;
            _db = context;
        }

        public async Task<IActionResult> Index(int? id, string name, int type = 0, int status = 0, int warehouse = 0, SortState sortOrder = SortState.NameAsc, int page = 1, int pageSize = 5)
        {
            IQueryable<Product> products = _db.Products;
            
            if (type != 0)
            {
                products = products.Where(n => n.Nomenclature.TypeId == type);
            }

            if (status != 0)
                products = products.Where(n => n.Status == (Product.ProductStatus)(status-1));

            if (warehouse != 0)
                products = products.Where(p => p.WarehouseId == warehouse);

            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(n => n.Nomenclature.Name.Contains(name) || n.Nomenclature.ProductWorths.OrderByDescending(pw => pw.Date).FirstOrDefault()!.Worth.ToString().Contains(name));
            }

            var count = await products.CountAsync();
            products = products.Skip((page - 1) * pageSize).Take(pageSize);

            switch (sortOrder)
            {
                case SortState.WorthAsc:
                    products = products.OrderBy(s => s.Nomenclature.ProductWorths.OrderByDescending(pw => pw.Date).FirstOrDefault()!.Worth);
                    break;
                case SortState.WorthDesc:
                    products = products.OrderByDescending(s => s.Nomenclature.ProductWorths.OrderByDescending(pw => pw.Date).FirstOrDefault()!.Worth);
                    break;
                case SortState.NameDesc:
                    products = products.OrderByDescending(s => s.Nomenclature.Name);
                    break;
                default:
                    products = products.OrderBy(s => s.Nomenclature.Name);
                    break;
            }

            IndexViewModel viewModel = new IndexViewModel(
               products.ToList(),
               new PageViewModel(count, page, pageSize),
               new FilterViewModel(_db.NomenclatureTypes.ToList(), _db.Warehouses.ToList(), type, warehouse, status, name),
               new SortViewModel(sortOrder)
            );

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