using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS_Online.Data;
using WMS_Online.Models;
using WMS_Online.Models.ProductViewModels;
using WMS_Online.Utils;

namespace WMS_Online.Controllers
{
    [Authorize]
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
            IQueryable<ProductToScreen> products = _db.Products.AsQueryable().GroupBy(p => new { p.NomenclatureId, p.Status, p.WarehouseId }).Select(pg => new ProductToScreen()
            {
                IdList = pg.Select(pgg => pgg.Id).ToList(),
                NomenclatureId = pg.Key.NomenclatureId,
                Status = pg.Key.Status,
                WarehouseId = pg.Key.WarehouseId,
                Count = pg.Count()
            });

            var tmp = products.ToList();

            foreach (var product in tmp)
            {
                product.Nomenclature = _db.Nomenclatures.Find(product.NomenclatureId)!;
                product.Warehouse = _db.Warehouses.Find(product.WarehouseId);
            }

            products = tmp.AsQueryable();

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

            var count = products.Count();
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
        public IActionResult AddProducts(string? name)
        {
            ViewData["Warehouses"] = new SelectList(_db.Warehouses.ToList(), "Id", "Address", 1);
            ViewData["Suppliers"] = new SelectList(_db.Suppliers.ToList(), "Id", "Tin", 1);

            if(name != null) 
                ViewData["Nomenclatures"] = new SelectList(_db.Nomenclatures.Where(n => n.Name.Contains(name)).ToList(), "Id", "Name", 1);
            else
                ViewData["Nomenclatures"] = new SelectList(_db.Nomenclatures.ToList(), "Id", "Name", 1);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProducts(int nomenclatureId ,int warehouseId, int supplierId, DateTime date, int amount)
        {
            ViewData["Warehouses"] = new SelectList(_db.Warehouses.ToList(), "Id", "Address", 1);
            ViewData["Suppliers"] = new SelectList(_db.Suppliers.ToList(), "Id", "Tin", 1);
            ViewData["Nomenclatures"] = new SelectList(_db.Nomenclatures.ToList(), "Id", "Name", 1);

            if (amount <= 0)
            {
                ModelState.AddModelError("", "Количество должно быть положительным");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }            

            var delivery = _db.Deliveries.Where(d => d.Date == DateOnly.FromDateTime(date)).FirstOrDefault();
            if (delivery == null)
            {
                var newDelivery = _db.Deliveries.Add(new Delivery() { Date = DateOnly.FromDateTime(date), SupplierId = supplierId });
                await _db.SaveChangesAsync();

                delivery = newDelivery.Entity;
            }

            var newProducts = new List<Product>();
            for (int i = 0; i < amount; i++)
            {
                newProducts.Add(new Product() { NomenclatureId = nomenclatureId, WarehouseId = warehouseId, DeliveryId = delivery.Id });
            }

            _db.Products.AddRange(newProducts);
            await _db.SaveChangesAsync();
           
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetAddress(string? idList)
        {
           
            if (idList == null)
                return NotFound();

            var idListNum = idList.Split('-').Select(int.Parse).ToList();
            var addresses = _db.Products.Where(p => idListNum.Contains(p.Id)).Select(p => p.CellAddress!).ToList();
            var product = _db.Products.First(p => idListNum.Contains(p.Id));
            var viewModel = new AddressViewModel(product, addresses);
            ViewData["Nomenclature"] = product.Nomenclature.Name;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GetAddress(int id, string newAddress)
        {
            var product = _db.Products.Find(id);
            if (product == null)
                return NotFound();

            product.CellAddress = newAddress;
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult ChangeStatus(int? id, bool addressError = false)
        {
            if (id == null)
                return NotFound();

            ViewData["Id"] = id;

            var product = _db.Products.Find(id)!;
            var oldStatus = product.Status;
            ViewData["OldStatus"] = Enum.GetName(typeof(Product.ProductStatus), oldStatus);

            var Statuses = new List<Status>();
            switch ((int)oldStatus)
            {
                case 0: // На_приемке:
                    Statuses.Add(new Status() { Id = 1, Name = "На складе" });
                    Statuses.Add(new Status() { Id = 4, Name = "Списан" }); 
                    break;
                case 1: // На_складе:
                    Statuses.Add(new Status() { Id = 4, Name = "Списан" });
                    break;                
                case 2: // Продан:
                    Statuses.Add(new Status() { Id = 3, Name = "Возвращен" });
                    break;
                case 3: // Возвращен:
                    Statuses.Add(new Status() { Id = 1, Name = "На складе" });
                    Statuses.Add(new Status() { Id = 4, Name = "Списан" });
                    break;
                case 4: // Списан:
                    Statuses.Add(new Status() { Id = 1, Name = "На складе" });
                    break;
            }

            ViewData["Statuses"] = new SelectList(Statuses, "Id", "Name", 0);

            ViewData["Warehouses"] = new SelectList(_db.Warehouses, "Id", "Address", product.WarehouseId);

            if (addressError)
            {
                ModelState.AddModelError("", "При смене статуса на \"На складе\" необходимо установить адрес ячейки для товара");
            }

            return View();
        }

        [HttpPost]
        public IActionResult ChangeStatus(int? id, int status, string newAddress, int warehouse)
        {
            if (id == null)
                return View();

            if ((Product.ProductStatus)status == Product.ProductStatus.На_складе && string.IsNullOrEmpty(newAddress))
            {
                return RedirectToAction("ChangeStatus", new { id = id, addressError = true });                
            }

            var product = _db.Products.Find(id);
            if (product == null)
                return NotFound();

            product.Status = (Product.ProductStatus)status;

            if ((Product.ProductStatus)status == Product.ProductStatus.На_складе)
            {
                product.CellAddress = newAddress;
                product.WarehouseId = warehouse;
            }

            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public RedirectToActionResult AddToCart(int id, int count)
        {
            Product? product = _db.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                var cart = GetCart();
                var productToCart = new ProductToCart(product);
                if (cart.GetQuantity(productToCart) + 1 <= count)
                {
                    cart.AddItem(productToCart, 1);
                    SaveCart(cart);
                }                
            }
            return RedirectToAction("CartView");
        }

        [HttpPost]
        public RedirectToActionResult RemoveFromCart(int id)
        {
            Product? product = _db.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                var cart = GetCart();
                cart.RemoveLine(new ProductToCart(product));
                SaveCart(cart);
            }
            return RedirectToAction("CartView");
        }

        [HttpGet]
        public IActionResult CartView()
        {
            return View(GetCart());            
        }

        [HttpPost]
        public async Task<RedirectToActionResult> ProceedPurchase(string? phone)
        {
            var cart = GetCart();

            if (!cart.Lines.Any())
                return RedirectToAction("Index");

            int? customerId = null;
            if (phone != null)
            {
                var customer = _db.Customers.Where(c => c.Phone == phone).FirstOrDefault();
                if (customer != null)
                {
                    customerId = customer.Id;
                }
            }

            var selling = new Selling() { CustomerId = customerId, Date = DateOnly.FromDateTime(DateTime.Now), PersonalDiscount = 0 };
            var newSelling = _db.Sellings.Add(selling);
            await _db.SaveChangesAsync();


            int sellingId = newSelling.Entity.Id;
            foreach (var product in cart.Products)
            {
                var soldProduct = _db.Products.Find(product.Id)!;
                _db.ProductSellings.Add(new ProductSelling() { SellingId = newSelling.Entity.Id, ProductId = soldProduct.Id });
                soldProduct.Status = Product.ProductStatus.Продан;
                soldProduct.CellAddress = "";
                soldProduct.Warehouse = null;
                soldProduct.WarehouseId = null;
            }

            await _db.SaveChangesAsync();

            cart.Clear();
            SaveCart(cart);

            return RedirectToAction("Index");
        }

        private Cart GetCart()
        {
            return HttpContext.Session.GetObjectFromJson<Cart>("Cart") ?? new Cart();
        }

        private void SaveCart(Cart cart)
        {
            HttpContext.Session.SetObjectAsJson("Cart", cart);
        }
    }
}