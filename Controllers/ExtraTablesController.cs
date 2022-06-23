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
    }
}