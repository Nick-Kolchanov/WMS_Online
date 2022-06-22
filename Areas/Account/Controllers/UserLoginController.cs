using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WMS_Online.Data;
using WMS_Online.Models;
using Microsoft.AspNetCore.Authentication;

namespace WMS_Online.Areas.Login.Controllers
{
    [Area("Account")]
    public class UserLoginController : Controller
    {
        private readonly ILogger<UserLoginController> _logger;
        private readonly WmsDbContext _db;

        public UserLoginController(ILogger<UserLoginController> logger, WmsDbContext context)
        {
            _logger = logger;
            _db = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string login, string password)
        {
            var passHash = Utils.PasswordHash.GetHash(password.ToCharArray());
            var user = _db.Users.FirstOrDefault(u => u.Name == login && u.PasswordHash == passHash);
            if (user == null)
            {
                ModelState.AddModelError("", "Такого пользователя нет");
                return View();
            }

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            claims.Add(new Claim("role", user.IsAdmin ? "Admin" : "Seller"));

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        
            return RedirectToRoute("default", new {controller = "Product", action = "Index"});
        }

        public async Task<IActionResult> UserSignOut()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index");
        }
    }
}