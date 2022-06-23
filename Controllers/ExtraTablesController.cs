using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS_Online.Data;
using WMS_Online.Models;
using WMS_Online.Utils;

namespace WMS_Online.Controllers
{
    [Authorize(Policy = "IsAdmin")]
    public class ExtraTablesController : Controller
    {
        private readonly ILogger<ExtraTablesController> _logger;
        private readonly WmsDbContext _db;

        public ExtraTablesController(ILogger<ExtraTablesController> logger, WmsDbContext context)
        {
            _logger = logger;
            _db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ChooseTable()
        {
            return RedirectToAction("Index");
        }

        // Property

        [HttpGet]
        public IActionResult PropertyTable()
        {
            return View(_db.Properties.ToList());
        }

        [HttpGet]
        public IActionResult AddProperty(int? id)
        {
            ViewData["MeasurementUnits"] = new SelectList(_db.MeasurementUnits.ToList(), "Id", "Name");

            if (id == null)
                return View();

            var property = _db.Properties.Find(id);
            if (property == null)
                return NotFound();

            ViewData["MeasurementUnits"] = new SelectList(_db.MeasurementUnits.ToList(), "Id", "Name", property.MeasurementUnitId);

            return View(property);
        }

        [HttpPost]
        public async Task<IActionResult> AddProperty(Property property)
        {
            ViewData["MeasurementUnits"] = new SelectList(_db.MeasurementUnits.ToList(), "Id", "Name", property.MeasurementUnitId);

            if (!ModelState.IsValid)
            {
                return View(property);
            }

            if (property.Id != 0)
                _db.Properties.Update(property);
            else
                _db.Properties.Add(property);

            await _db.SaveChangesAsync();

            return RedirectToAction("PropertyTable");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveProperty(int? id)
        {
            if (id == null)
                return NotFound();

            Property property = new Property { Id = id.Value };
            _db.Entry(property).State = EntityState.Deleted;
            await _db.SaveChangesAsync();

            return RedirectToAction("PropertyTable");
        }

        // MeasurementUnit

        [HttpGet]
        public IActionResult MeasurementUnitTable()
        {
            return View(_db.MeasurementUnits.ToList());
        }

        [HttpGet]
        public IActionResult AddMeasurementUnit(int? id)
        {
            if (id == null)
                return View();

            var unit = _db.MeasurementUnits.Find(id);
            if (unit == null)
                return NotFound();

            return View(unit);
        }

        [HttpPost]
        public async Task<IActionResult> AddMeasurementUnit(MeasurementUnit unit)
        {
            if (!ModelState.IsValid)
            {
                return View(unit);
            }

            if (unit.Id != 0)
                _db.MeasurementUnits.Update(unit);
            else
                _db.MeasurementUnits.Add(unit);

            await _db.SaveChangesAsync();

            return RedirectToAction("MeasurementUnitTable");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveMeasurementUnit(int? id)
        {
            if (id == null)
                return NotFound();

            MeasurementUnit unit = new MeasurementUnit { Id = id.Value };
            _db.Entry(unit).State = EntityState.Deleted;
            await _db.SaveChangesAsync();

            return RedirectToAction("MeasurementUnitTable");
        }

        // NomenclatureType

        [HttpGet]
        public IActionResult NomenclatureTypeTable()
        {
            return View(_db.NomenclatureTypes.ToList());
        }

        [HttpGet]
        public IActionResult AddNomenclatureType(int? id)
        {
            if (id == null)
                return View();

            var unit = _db.NomenclatureTypes.Find(id);
            if (unit == null)
                return NotFound();

            return View(unit);
        }

        [HttpPost]
        public async Task<IActionResult> AddNomenclatureType(NomenclatureType type)
        {
            if (!ModelState.IsValid)
            {
                return View(type);
            }

            if (type.Id != 0)
                _db.NomenclatureTypes.Update(type);
            else
                _db.NomenclatureTypes.Add(type);

            await _db.SaveChangesAsync();

            return RedirectToAction("NomenclatureTypeTable");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveNomenclatureType(int? id)
        {
            if (id == null)
                return NotFound();

            NomenclatureType type = new NomenclatureType { Id = id.Value };
            _db.Entry(type).State = EntityState.Deleted;
            await _db.SaveChangesAsync();

            return RedirectToAction("NomenclatureTypeTable");
        }

        // WarehouseType

        [HttpGet]
        public IActionResult WarehouseTypeTable()
        {
            return View(_db.WarehouseTypes.ToList());
        }

        [HttpGet]
        public IActionResult AddWarehouseType(int? id)
        {
            if (id == null)
                return View();

            var unit = _db.WarehouseTypes.Find(id);
            if (unit == null)
                return NotFound();

            return View(unit);
        }

        [HttpPost]
        public async Task<IActionResult> AddWarehouseType(WarehouseType type)
        {
            if (!ModelState.IsValid)
            {
                return View(type);
            }

            if (type.Id != 0)
                _db.WarehouseTypes.Update(type);
            else
                _db.WarehouseTypes.Add(type);

            await _db.SaveChangesAsync();

            return RedirectToAction("WarehouseTypeTable");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveWarehouseType(int? id)
        {
            if (id == null)
                return NotFound();

            WarehouseType type = new WarehouseType { Id = id.Value };
            _db.Entry(type).State = EntityState.Deleted;
            await _db.SaveChangesAsync();

            return RedirectToAction("WarehouseTypeTable");
        }

        // DiscrepancyType

        [HttpGet]
        public IActionResult DiscrepancyTypeTable()
        {
            return View(_db.DiscrepancyTypes.ToList());
        }

        [HttpGet]
        public IActionResult AddDiscrepancyType(int? id)
        {
            if (id == null)
                return View();

            var unit = _db.DiscrepancyTypes.Find(id);
            if (unit == null)
                return NotFound();

            return View(unit);
        }

        [HttpPost]
        public async Task<IActionResult> AddDiscrepancyType(DiscrepancyType type)
        {
            if (!ModelState.IsValid)
            {
                return View(type);
            }

            if (type.Id != 0)
                _db.DiscrepancyTypes.Update(type);
            else
                _db.DiscrepancyTypes.Add(type);

            await _db.SaveChangesAsync();

            return RedirectToAction("DiscrepancyTypeTable");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveDiscrepancyType(int? id)
        {
            if (id == null)
                return NotFound();

            DiscrepancyType type = new DiscrepancyType { Id = id.Value };
            _db.Entry(type).State = EntityState.Deleted;
            await _db.SaveChangesAsync();

            return RedirectToAction("DiscrepancyTypeTable");
        }

        // DiscrepancyStatus

        [HttpGet]
        public IActionResult DiscrepancyStatusTable()
        {
            return View(_db.DiscrepancyStatuses.ToList());
        }

        [HttpGet]
        public IActionResult AddDiscrepancyStatus(int? id)
        {
            if (id == null)
                return View();

            var unit = _db.DiscrepancyStatuses.Find(id);
            if (unit == null)
                return NotFound();

            return View(unit);
        }

        [HttpPost]
        public async Task<IActionResult> AddDiscrepancyStatus(DiscrepancyStatus status)
        {
            if (!ModelState.IsValid)
            {
                return View(status);
            }

            if (status.Id != 0)
                _db.DiscrepancyStatuses.Update(status);
            else
                _db.DiscrepancyStatuses.Add(status);

            await _db.SaveChangesAsync();

            return RedirectToAction("DiscrepancyStatusTable");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveDiscrepancyStatus(int? id)
        {
            if (id == null)
                return NotFound();

            DiscrepancyStatus type = new DiscrepancyStatus { Id = id.Value };
            _db.Entry(type).State = EntityState.Deleted;
            await _db.SaveChangesAsync();

            return RedirectToAction("DiscrepancyStatusTable");
        }

        // InventarizationReason

        [HttpGet]
        public IActionResult InventarizationReasonTable()
        {
            return View(_db.InventarizationReasons.ToList());
        }

        [HttpGet]
        public IActionResult AddInventarizationReason(int? id)
        {
            if (id == null)
                return View();

            var unit = _db.InventarizationReasons.Find(id);
            if (unit == null)
                return NotFound();

            return View(unit);
        }

        [HttpPost]
        public async Task<IActionResult> AddInventarizationReason(InventarizationReason reason)
        {
            if (!ModelState.IsValid)
            {
                return View(reason);
            }

            if (reason.Id != 0)
                _db.InventarizationReasons.Update(reason);
            else
                _db.InventarizationReasons.Add(reason);

            await _db.SaveChangesAsync();

            return RedirectToAction("InventarizationReasonTable");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveInventarizationReason(int? id)
        {
            if (id == null)
                return NotFound();

            InventarizationReason type = new InventarizationReason { Id = id.Value };
            _db.Entry(type).State = EntityState.Deleted;
            await _db.SaveChangesAsync();

            return RedirectToAction("InventarizationReasonTable");
        }
    }
}