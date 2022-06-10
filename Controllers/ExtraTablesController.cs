using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS_Online.Data;
using WMS_Online.Models;
using WMS_Online.Utils;

namespace WMS_Online.Controllers
{
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
       
        [HttpGet]
        public IActionResult ChooseTable()
        {           
            return RedirectToAction("Index");
        }
    }
}