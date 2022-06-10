using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS_Online.Data;
using WMS_Online.Models;
using WMS_Online.Models.WarehouseViewModels;

namespace WMS_Online.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly ILogger<WarehouseController> _logger;
        private readonly WmsDbContext _db;
        

        public WarehouseController(ILogger<WarehouseController> logger, WmsDbContext context)
        {
            _logger = logger;
            _db = context;
        }

        public async Task<IActionResult> Index(string name, int type = 0, SortState sortOrder = SortState.AddressAsc, int page = 1, int pageSize = 5)
        {
            IQueryable<Warehouse> warehouses = _db.Warehouses;
            
            if (type != 0)
            {
                warehouses = warehouses.Where(w => w.WarehouseType == type);
            }

            if (!string.IsNullOrEmpty(name))
            {
                warehouses = warehouses.Where(w => w.Address!.Contains(name) || w.Phone!.Contains(name));
            }

            var count = await warehouses.CountAsync();
            warehouses = warehouses.Skip((page - 1) * pageSize).Take(pageSize);

            switch (sortOrder)
            {
                case SortState.PhoneAsc:
                    warehouses = warehouses.OrderBy(s => s.Phone);
                    break;
                case SortState.PhoneDesc:
                    warehouses = warehouses.OrderByDescending(s => s.Phone);
                    break;
                case SortState.AddressDesc:
                    warehouses = warehouses.OrderByDescending(s => s.Address);
                    break;
                default:
                    warehouses = warehouses.OrderBy(s => s.Address);
                    break;
            }

            IndexViewModel viewModel = new IndexViewModel(
               warehouses.ToList(),
               new PageViewModel(count, page, pageSize),
               new FilterViewModel(_db.WarehouseTypes.ToList(), type, name),
               new SortViewModel(sortOrder)
            );

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult AddWarehouse(int? id)
        {
            ViewData["Types"] = new SelectList(_db.WarehouseTypes.ToList(), "Id", "Name", 1);

            if (id == null)
                return View();

            var warehouse = _db.Warehouses.Find(id);
            if (warehouse == null)
                return NotFound();

            ViewData["Types"] = new SelectList(_db.WarehouseTypes.ToList(), "Id", "Name", warehouse.WarehouseType);
            return View(warehouse);
        }

        [HttpPost]
        public async Task<IActionResult> AddWarehouse(Warehouse warehouse)
        {
            ViewData["Types"] = new SelectList(_db.WarehouseTypes.ToList(), "Id", "Name", warehouse.WarehouseType);

            if (!ModelState.IsValid)
            {
                return View(warehouse);
            }

            var foundWarehouse = _db.Warehouses.FirstOrDefault(w => w.Address == warehouse.Address);
            if (foundWarehouse != null)
            {
                ModelState.AddModelError("", "Склад с таким адресом уже есть!");
            }

            if (!ModelState.IsValid)
            {
                return View(warehouse);
            }

            if (warehouse.Id != 0) 
                _db.Warehouses.Update(warehouse);
            else
                _db.Warehouses.Add(warehouse);

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveWarehouse(int? id)
        {
            if (id == null)
                return NotFound();

            Warehouse warehouse = new Warehouse { Id = id.Value };
            _db.Entry(warehouse).State = EntityState.Deleted;
            await _db.SaveChangesAsync();            

            return RedirectToAction("Index");
        }        
    }
}