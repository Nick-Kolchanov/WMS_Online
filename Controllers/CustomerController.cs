using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS_Online.Data;
using WMS_Online.Models;
using WMS_Online.Models.CustomerViewModels;

namespace WMS_Online.Controllers
{
    [Authorize(Policy = "IsAdmin")]
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly WmsDbContext _db;
        

        public CustomerController(ILogger<CustomerController> logger, WmsDbContext context)
        {
            _logger = logger;
            _db = context;
        }

        public async Task<IActionResult> Index(string name, SortState sortOrder = SortState.NameAsc, int page = 1, int pageSize = 5)
        {
            IQueryable<Customer> customers = _db.Customers;

            if (!string.IsNullOrEmpty(name))
            {
                customers = customers.Where(c => c.Name!.Contains(name) || c.Phone.Contains(name));
            }

            var count = await customers.CountAsync();
            customers = customers.Skip((page - 1) * pageSize).Take(pageSize);

            switch (sortOrder)
            {
                case SortState.PhoneAsc:
                    customers = customers.OrderBy(s => s.Phone);
                    break;
                case SortState.PhoneDesc:
                    customers = customers.OrderByDescending(s => s.Phone);
                    break;
                case SortState.NameDesc:
                    customers = customers.OrderByDescending(s => s.Name);
                    break;
                default:
                    customers = customers.OrderBy(s => s.Name);
                    break;
            }

            IndexViewModel viewModel = new IndexViewModel(
               customers.ToList(),
               new PageViewModel(count, page, pageSize),
               new FilterViewModel(name),
               new SortViewModel(sortOrder)
            );

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult AddCustomer(int? id)
        {
            if (id == null)
                return View();

            var customer = _db.Customers.Find(id);
            if (customer == null)
                return NotFound();
            
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            var foundCustomer = _db.Customers.FirstOrDefault(s => s.Phone == customer.Phone);
            if (foundCustomer != null)
            {
                ModelState.AddModelError("", "Покупатель с таким номером телефона уже есть!");
            }

            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            if (customer.Id != 0) 
                _db.Customers.Update(customer);
            else
                _db.Customers.Add(customer);

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCustomer(int? id)
        {
            if (id == null)
                return NotFound();

            Customer customer = new Customer { Id = id.Value };
            _db.Entry(customer).State = EntityState.Deleted;
            await _db.SaveChangesAsync();            

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult CheckSellings(int id)
        {
            var customer = _db.Customers.Find(id);
            
            if (customer == null)
                return NotFound();

            return View(customer);
        }

        [HttpGet]
        public IActionResult CheckExactSelling(int id)
        {
            var products = _db.ProductSellings.Where(ps => ps.SellingId == id).ToList();

            if (products == null)
                return NotFound();

            return View(products);
        }
        
    }
}