using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WMS_Online.Data;
using WMS_Online.Models;
using WMS_Online.Models.UserViewModels;

namespace WMS_Online.Controllers
{
    [Authorize(Policy = "IsAdmin")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly WmsDbContext _db;
        

        public UserController(ILogger<UserController> logger, WmsDbContext context)
        {
            _logger = logger;
            _db = context;
        }

        public async Task<IActionResult> Index(string name, SortState sortOrder = SortState.NameAsc, int page = 1, int pageSize = 5)
        {
            IQueryable<User> users = _db.Users;
            if (!string.IsNullOrEmpty(name))
            {
                users = users.Where(p => p.Name!.Contains(name));
            }

            var count = await users.CountAsync();
            users = users.Skip((page - 1) * pageSize).Take(pageSize);

            switch (sortOrder)
            {
                case SortState.NameDesc:
                    users = users.OrderByDescending(s => s.Name);
                    break;
                default:
                    users = users.OrderBy(s => s.Name);
                    break;
            }

            IndexViewModel viewModel = new IndexViewModel(
               users.ToList(),
               new PageViewModel(count, page, pageSize),
               new FilterViewModel(name),
               new SortViewModel(sortOrder)
            );

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult AddUser(int? id)
        {
            if (id == null)
                return View();

            var user = _db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();
            
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            user.PasswordHash = Utils.PasswordHash.GetHash(user.PasswordHash.ToCharArray());
            var foundUser = _db.Users.Where(usr => usr.Name == user.Name && usr.PasswordHash == user.PasswordHash).FirstOrDefault();
            if (foundUser != null)
            {
                ModelState.AddModelError("", "Такой пользователь уже есть!");
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            if (user.Id != 0) 
                _db.Users.Update(user);
            else
                _db.Users.Add(user);

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUser(int? id)
        {
            if (id == null)
                return NotFound();

            User user = new User { Id = id.Value };
            _db.Entry(user).State = EntityState.Deleted;
            await _db.SaveChangesAsync();            

            return RedirectToAction("Index");
        }
    }
}