using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WMS_Online.Data;
using WMS_Online.Models;

namespace WMS_Online.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WmsDbContext _db;

        public HomeController(ILogger<HomeController> logger, WmsDbContext context)
        {
            _logger = logger;
            _db = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_db.Customers);
        }

        [HttpPost]
        public IActionResult Index(string login, string password)
        {
            var passHash = Utils.PasswordHash.GetHash(password.ToCharArray());
            var user = _db.Users.FirstOrDefault(u => u.Name == login && u.PasswordHash == passHash);
            if (user == null)
            {
                ModelState.AddModelError("", "Такого пользователя нет");
                return View();
            }

            if (user.IsAdmin)
                return RedirectToAction("Index", "User");
            else
                return RedirectToAction("Index", "Product");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}