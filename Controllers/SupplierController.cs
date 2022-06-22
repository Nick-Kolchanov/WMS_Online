using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS_Online.Data;
using WMS_Online.Models;
using WMS_Online.Models.SupplierViewModels;

namespace WMS_Online.Controllers
{
    [Authorize(Policy = "IsAdmin")]
    public class SupplierController : Controller
    {
        private readonly ILogger<SupplierController> _logger;
        private readonly WmsDbContext _db;
        

        public SupplierController(ILogger<SupplierController> logger, WmsDbContext context)
        {
            _logger = logger;
            _db = context;
        }

        public async Task<IActionResult> Index(string name, SortState sortOrder = SortState.TinAsc, int page = 1, int pageSize = 5)
        {
            IQueryable<Supplier> suppliers = _db.Suppliers;

            if (!string.IsNullOrEmpty(name))
            {
                suppliers = suppliers.Where(
                    p => p.Name!.Contains(name) || 
                    p.Tin!.ToString().Contains(name) || 
                    p.Phone!.Contains(name) || 
                    p.Email!.Contains(name));
            }

            var count = await suppliers.CountAsync();
            suppliers = suppliers.Skip((page - 1) * pageSize).Take(pageSize);

            switch (sortOrder)
            {
                case SortState.NameAsc:
                    suppliers = suppliers.OrderBy(s => s.Name);
                    break;
                case SortState.NameDesc:
                    suppliers = suppliers.OrderByDescending(s => s.Name);
                    break;
                case SortState.TinDesc:
                    suppliers = suppliers.OrderByDescending(s => s.Tin);
                    break;
                default:
                    suppliers = suppliers.OrderBy(s => s.Tin);
                    break;
            }

            IndexViewModel viewModel = new IndexViewModel(
               suppliers.ToList(),
               new PageViewModel(count, page, pageSize),
               new FilterViewModel(name),
               new SortViewModel(sortOrder)
            );

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult AddSupplier(int? id)
        {
            if (id == null)
                return View();

            var supplier = _db.Suppliers.Find(id);
            if (supplier == null)
                return NotFound();
            
            return View(supplier);
        }

        [HttpPost]
        public async Task<IActionResult> AddSupplier(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return View(supplier);
            }

            var foundSupplier = _db.Suppliers.FirstOrDefault(s => s.Tin == supplier.Tin);
            if (foundSupplier != null)
            {
                ModelState.AddModelError("", "Поставщик с таким ИНН уже есть!");
            }

            if (!ModelState.IsValid)
            {
                return View(supplier);
            }

            if (supplier.Id != 0) 
                _db.Suppliers.Update(supplier);
            else
                _db.Suppliers.Add(supplier);

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSupplier(int? id)
        {
            if (id == null)
                return NotFound();

            Supplier supplier = new Supplier { Id = id.Value };
            _db.Entry(supplier).State = EntityState.Deleted;
            await _db.SaveChangesAsync();            

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult CheckDeliveries(int id)
        {
            var supplier = _db.Suppliers.Find(id);
            
            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        [HttpGet]
        public IActionResult CheckExactDelivery(int id)
        {
            var delivery = _db.Deliveries.Find(id);

            if (delivery == null)
                return NotFound();

            return View(delivery);
        }
        
    }
}