using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TemplateEngine;
using System.Globalization;
using WMS_Online.Data;
using WMS_Online.Models;
using WMS_Online.Utils;
using TemplateEngine.Docx;
using Microsoft.AspNetCore.Authorization;

namespace WMS_Online.Controllers
{
    [Authorize(Policy = "IsAdmin")]
    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;
        private readonly WmsDbContext _db;
        private readonly IWebHostEnvironment _appEnvironment;

        public ReportController(ILogger<ReportController> logger, WmsDbContext context, IWebHostEnvironment appEnvironment)
        {
            _logger = logger;
            _db = context;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
            var tables = new List<Table>() { new Table { Name = "Номеклатуры" },
                new Table { Name = "Характеристики"},
                new Table { Name = "Покупатели"},
                new Table { Name = "Поставщики"},
                new Table { Name = "Поставки"},
                new Table { Name = "Склады"},
                new Table { Name = "Типы складов"},};

            ViewData["ImportTables"] = new SelectList(tables, "Name", "Name", "Номеклатуры");
            ViewData["ExportTables"] = new SelectList(tables, "Name", "Name", "Номеклатуры");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Import(bool isRewriteImport, string importTable, IFormFile importFile)
        {
            if (Path.GetExtension(importFile.FileName) != ".csv")
            {
                ModelState.AddModelError("", "Неверное расширение файла!");
                return RedirectToAction("Index");
            }
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower().Replace("_", ""),
            };
            using var reader = new StreamReader(importFile.OpenReadStream());
            using var csv = new CsvReader(reader, config);

            try
            {
                switch (importTable)
                {
                    case "Номеклатуры":
                        if (isRewriteImport)
                        {
                            try
                            {
                                _db.Database.ExecuteSqlRaw("TRUNCATE nomenclatures RESTART IDENTITY;");
                            }
                            catch (Exception)
                            {
                                ModelState.AddModelError("", "Невозможно перезаписать данные, на таблицу ссылаются внешние ключи");
                                return RedirectToAction("Index");
                            }
                        }
                        var recordsNomenclature = csv.GetRecords<Nomenclature>().ToList();
                        _db.Nomenclatures.AddRange(recordsNomenclature);
                        await _db.SaveChangesAsync();
                        _db.Database.ExecuteSqlRaw("SELECT setval('nomenclatures_id_seq', (SELECT MAX(id) from nomenclatures));");
                        await _db.SaveChangesAsync();
                        break;

                    case "Характеристики":
                        if (isRewriteImport)
                        {
                            try
                            {
                                _db.Database.ExecuteSqlRaw("TRUNCATE properties RESTART IDENTITY;");
                            }
                            catch (Exception)
                            {
                                ModelState.AddModelError("", "Невозможно перезаписать данные, на таблицу ссылаются внешние ключи");
                                return RedirectToAction("Index");
                            }
                        }
                        var recordsProperty = csv.GetRecords<Property>().ToList();
                        _db.Properties.AddRange(recordsProperty);
                        await _db.SaveChangesAsync();
                        _db.Database.ExecuteSqlRaw("SELECT setval('properties_id_seq', (SELECT MAX(id) from properties));");
                        await _db.SaveChangesAsync();
                        break;

                    case "Покупатели":
                        if (isRewriteImport)
                        {
                            try
                            {
                                _db.Database.ExecuteSqlRaw("TRUNCATE customers RESTART IDENTITY;");
                            }
                            catch (Exception)
                            {
                                ModelState.AddModelError("", "Невозможно перезаписать данные, на таблицу ссылаются внешние ключи");
                                return RedirectToAction("Index");
                            }
                        }
                        var recordsCustomer = csv.GetRecords<Customer>().ToList();
                        _db.Customers.AddRange(recordsCustomer);
                        await _db.SaveChangesAsync();
                        _db.Database.ExecuteSqlRaw("SELECT setval('customers_id_seq', (SELECT MAX(id) from customers));");
                        await _db.SaveChangesAsync();
                        break;

                    case "Поставщики":
                        if (isRewriteImport)
                        {
                            try
                            {
                                _db.Database.ExecuteSqlRaw("TRUNCATE suppliers RESTART IDENTITY;");
                            }
                            catch (Exception)
                            {
                                ModelState.AddModelError("", "Невозможно перезаписать данные, на таблицу ссылаются внешние ключи");
                                return RedirectToAction("Index");
                            }
                        }
                        var recordsSupplier = csv.GetRecords<Supplier>().ToList();
                        _db.Suppliers.AddRange(recordsSupplier);
                        await _db.SaveChangesAsync();
                        _db.Database.ExecuteSqlRaw("SELECT setval('suppliers_id_seq', (SELECT MAX(id) from suppliers));");
                        await _db.SaveChangesAsync();
                        break;

                    case "Поставки":
                        if (isRewriteImport)
                        {
                            try
                            {
                                _db.Database.ExecuteSqlRaw("TRUNCATE deliveries RESTART IDENTITY;");
                            }
                            catch (Exception)
                            {
                                ModelState.AddModelError("", "Невозможно перезаписать данные, на таблицу ссылаются внешние ключи");
                                return RedirectToAction("Index");
                            }
                        }
                        var recordsDelivery = csv.GetRecords<Delivery>().ToList();
                        _db.Deliveries.AddRange(recordsDelivery);
                        await _db.SaveChangesAsync();
                        _db.Database.ExecuteSqlRaw("SELECT setval('deliveries_id_seq', (SELECT MAX(id) from deliveries));");
                        await _db.SaveChangesAsync();
                        break;

                    case "Склады":
                        if (isRewriteImport)
                        {
                            try
                            {
                                _db.Database.ExecuteSqlRaw("TRUNCATE warehouses RESTART IDENTITY;");
                            }
                            catch (Exception)
                            {
                                ModelState.AddModelError("", "Невозможно перезаписать данные, на таблицу ссылаются внешние ключи");
                                return RedirectToAction("Index");
                            }
                        }
                        var recordsWarehouse = csv.GetRecords<Warehouse>().ToList();
                        _db.Warehouses.AddRange(recordsWarehouse);
                        await _db.SaveChangesAsync();
                        _db.Database.ExecuteSqlRaw("SELECT setval('warehouses_id_seq', (SELECT MAX(id) from warehouses));");
                        await _db.SaveChangesAsync();
                        break;

                    case "Типы складов":
                        if (isRewriteImport)
                        {
                            try
                            {
                                _db.Database.ExecuteSqlRaw("TRUNCATE warehouse_types RESTART IDENTITY;");
                            }
                            catch (Exception)
                            {
                                ModelState.AddModelError("", "Невозможно перезаписать данные, на таблицу ссылаются внешние ключи");
                                return RedirectToAction("Index");
                            }
                        }
                        var recordsWarehouseType = csv.GetRecords<WarehouseType>().ToList();
                        _db.WarehouseTypes.AddRange(recordsWarehouseType);
                        await _db.SaveChangesAsync();
                        _db.Database.ExecuteSqlRaw("SELECT setval('warehouse_types_id_seq', (SELECT MAX(id) from warehouse_types));");
                        await _db.SaveChangesAsync();
                        break;
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Неверный формат данных в файле, импорт не удался");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Export(string exportTable)
        {
            string path = "/files/export.csv";
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower().Replace("_", "")
            };
            var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create, FileAccess.ReadWrite);
            using var writer = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
            using var csv = new CsvWriter(writer, config);

            switch (exportTable)
            {
                case "Номеклатуры":
                    await csv.WriteRecordsAsync(_db.Nomenclatures.ToList());
                    break;

                case "Характеристики":
                    await csv.WriteRecordsAsync(_db.Properties.ToList());
                    break;

                case "Покупатели":
                    await csv.WriteRecordsAsync(_db.Customers.ToList());
                    break;

                case "Поставщики":
                    await csv.WriteRecordsAsync(_db.Suppliers.ToList());
                    break;

                case "Поставки":
                    await csv.WriteRecordsAsync(_db.Deliveries.ToList());
                    break;

                case "Склады":
                    await csv.WriteRecordsAsync(_db.Warehouses.ToList());
                    break;

                case "Типы складов":
                    await csv.WriteRecordsAsync(_db.WarehouseTypes.ToList());
                    break;
            }            

            return File(path, "text/csv", "exportted_table.csv");
        }

        [HttpPost]
        public IActionResult SalesReport(DateTime? dateStartSales, DateTime? dateEndSales)
        {
            var sampleReportPath = "/files/salesReport.docx";
            var reportPath = "/files/salesReportFilled.docx";

            try { System.IO.File.Delete(_appEnvironment.WebRootPath + reportPath); }
            catch { }

            System.IO.File.Copy(_appEnvironment.WebRootPath + sampleReportPath, _appEnvironment.WebRootPath + reportPath);

            using var outputDocument = new TemplateProcessor(_appEnvironment.WebRootPath + reportPath).SetRemoveContentControls(true);

            if (dateStartSales == null) dateStartSales = DateTime.Now;
            if (dateEndSales == null) dateEndSales = DateTime.Now;

            var valuesToFillNew = new Content();
            var valuesToFill = valuesToFillNew.Append(new FieldContent("Date", DateOnly.FromDateTime(DateTime.Now).ToString()));
            valuesToFill = valuesToFill.Append(new FieldContent("DateStart", DateOnly.FromDateTime(dateStartSales.Value).ToString()));
            valuesToFill = valuesToFill.Append(new FieldContent("DateEnd", DateOnly.FromDateTime(dateEndSales.Value).ToString()));

            var querySellings = _db.ProductSellings.Where(ps => ps.Selling!.Date >= DateOnly.FromDateTime(dateStartSales.Value) && ps.Selling!.Date <= DateOnly.FromDateTime(dateEndSales.Value));
            decimal sumWorth = 0;

            List<TableRowContent> rows = new();
            foreach (var productSelling in querySellings.ToList())
            {
                var worth = productSelling.Product!.Nomenclature!.ProductWorths!.Where(pw => pw.Date <= DateOnly.FromDateTime(dateEndSales.Value)).OrderBy(pw => pw.Date).ThenBy(pw => pw.Id).FirstOrDefault()!.Worth;
                sumWorth += worth * (decimal)(1 - 1.0 * productSelling.Selling!.PersonalDiscount! / 100);
                rows.Add(new TableRowContent(new FieldContent("ProductId", productSelling.Product!.Id.ToString()),
                new FieldContent("NomenclatureName", productSelling.Product!.Nomenclature!.Name),
                new FieldContent("NomenclatureWorth", worth.ToString()),
                new FieldContent("NomenclatureWorthDiscounted", (worth * (decimal)(1 - 1.0 * productSelling.Selling!.PersonalDiscount! / 100)).ToString())));
            }
            valuesToFill = valuesToFill.Append(new TableContent("SoldProducts", rows));

            var productTypeSellings = querySellings.GroupBy(ps => ps.Product!.Nomenclature!.Type!.Name).Select(g => new
            {
                g.Key,
                Sum = g.Sum(s => s.Product!.Nomenclature!.ProductWorths!.Where(pw => pw.Date <= DateOnly.FromDateTime(dateEndSales.Value)).OrderBy(pw => pw.Date).ThenBy(pw => pw.Id).Select(t => t.Worth * (decimal)(1 - 1.0 * s.Selling!.PersonalDiscount! / 100)).Sum())
            }); ;
            rows = new();
            foreach (var productSelling in productTypeSellings)
            {
                rows.Add(new TableRowContent(new FieldContent("NomenclatureType", productSelling.Key),
                new FieldContent("NomenclatureTypeWorth", productSelling.Sum.ToString())));
            }
            valuesToFill = valuesToFill.Append(new TableContent("ProductTypeSellings", rows));

            valuesToFill = valuesToFill.Append(new FieldContent("ProductCount", querySellings.Count().ToString()));
            valuesToFill = valuesToFill.Append(new FieldContent("ProductSumWorth", sumWorth.ToString()));
            outputDocument.FillContent(new Content(valuesToFill.ToArray()));
            outputDocument.SaveChanges();

            return File(reportPath, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Отчет о продажах (от {DateOnly.FromDateTime(DateTime.Now)}).docx");
        }

        [HttpPost]
        public IActionResult PurchasesReport(DateTime? dateStartPurchases, DateTime? dateEndPurchases)
        {
            var sampleReportPath = "/files/purchasesReport.docx";
            var reportPath = "/files/purchasesReportFilled.docx";

            try { System.IO.File.Delete(_appEnvironment.WebRootPath + reportPath); }
            catch { }

            System.IO.File.Copy(_appEnvironment.WebRootPath + sampleReportPath, _appEnvironment.WebRootPath + reportPath);

            using var outputDocument = new TemplateProcessor(_appEnvironment.WebRootPath + reportPath).SetRemoveContentControls(true);

            if (dateStartPurchases == null) dateStartPurchases = DateTime.Now;
            if (dateEndPurchases == null) dateEndPurchases = DateTime.Now;

            var valuesToFillNew = new Content();
            var valuesToFill = valuesToFillNew.Append(new FieldContent("Date", DateOnly.FromDateTime(DateTime.Now).ToString()));
            valuesToFill = valuesToFill.Append(new FieldContent("DateStart", DateOnly.FromDateTime(dateStartPurchases.Value).ToString()));
            valuesToFill = valuesToFill.Append(new FieldContent("DateEnd", DateOnly.FromDateTime(dateEndPurchases.Value).ToString()));

            var queryDeliveries = _db.Products.Where(ps => ps.Delivery!.Date >= DateOnly.FromDateTime(dateStartPurchases.Value) && ps.Delivery!.Date <= DateOnly.FromDateTime(dateEndPurchases.Value));
            decimal sumWorth = 0;

            List<TableRowContent> rows = new();
            foreach (var product in queryDeliveries.ToList())
            {
                var worth = product!.Nomenclature!.ProductWorths!.Where(pw => pw.Date <= DateOnly.FromDateTime(dateEndPurchases.Value)).OrderBy(pw => pw.Date).ThenBy(pw => pw.Id).FirstOrDefault()!.Worth;
                sumWorth += worth;
                rows.Add(new TableRowContent(new FieldContent("ProductId", product.Id.ToString()),
                new FieldContent("NomenclatureName", product.Nomenclature!.Name),
                new FieldContent("NomenclatureWorth", worth.ToString()),
                new FieldContent("DeliveryId", product.DeliveryId.ToString())));
            }
            valuesToFill = valuesToFill.Append(new TableContent("ProductBuyings", rows));

            valuesToFill = valuesToFill.Append(new FieldContent("ProductCount", queryDeliveries.Count().ToString()));
            valuesToFill = valuesToFill.Append(new FieldContent("ProductSumWorth", sumWorth.ToString()));

            rows = new();
            var queryDeliveriesGroup = _db.Deliveries.Where(d => d.Date >= DateOnly.FromDateTime(dateStartPurchases.Value) && d.Date <= DateOnly.FromDateTime(dateEndPurchases.Value) && d.Products.Count > 0);
            foreach (var delivery in queryDeliveriesGroup.ToList())
            {
                rows.Add(new TableRowContent(new FieldContent("DeliveryIdGroup", delivery.Id.ToString()),
                new FieldContent("DeliveriesSumCost", delivery.Products.Sum(p => p.Nomenclature!.ProductWorths!.Where(pw => pw.Date <= DateOnly.FromDateTime(dateEndPurchases.Value)).OrderBy(pw => pw.Date).ThenBy(pw => pw.Id).FirstOrDefault()!.Worth).ToString()),
                new FieldContent("DeliveriesSumCount", delivery.Products.Count.ToString())));
            }
            valuesToFill = valuesToFill.Append(new TableContent("ProductsDeliveries", rows));

            rows = new();
            var querySuppliersGroup = _db.Suppliers.Where(s => s.Deliveries.Where(d => d.Date >= DateOnly.FromDateTime(dateStartPurchases.Value) && d.Date <= DateOnly.FromDateTime(dateEndPurchases.Value)).Any());
            foreach (var supplier in querySuppliersGroup.ToList())
            {
                rows.Add(new TableRowContent(new FieldContent("SupplierName", supplier.Name ?? "-"),
                new FieldContent("SupplierTin", supplier.Tin.ToString()),
                new FieldContent("SupplierSumWorth", supplier.Deliveries.Where(d => d.Date >= DateOnly.FromDateTime(dateStartPurchases.Value) && d.Date <= DateOnly.FromDateTime(dateEndPurchases.Value)).Sum(d => d.Products.Sum(p => p.Nomenclature!.ProductWorths!.Where(pw => pw.Date <= DateOnly.FromDateTime(dateEndPurchases.Value)).OrderBy(pw => pw.Date).ThenBy(pw => pw.Id).FirstOrDefault()!.Worth)).ToString())));
            }
            valuesToFill = valuesToFill.Append(new TableContent("SuppliersProducts", rows));

            outputDocument.FillContent(new Content(valuesToFill.ToArray()));
            outputDocument.SaveChanges();

            return File(reportPath, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Отчет о покупках (от {DateOnly.FromDateTime(DateTime.Now)}).docx");
        }

        [HttpPost]
        public IActionResult WarehousesReport()
        {
            var sampleReportPath = "/files/warehouseReport.docx";
            var reportPath = "/files/warehouseReportFilled.docx";

            try { System.IO.File.Delete(_appEnvironment.WebRootPath + reportPath); }
            catch { }

            System.IO.File.Copy(_appEnvironment.WebRootPath + sampleReportPath, _appEnvironment.WebRootPath + reportPath);

            using var outputDocument = new TemplateProcessor(_appEnvironment.WebRootPath + reportPath).SetRemoveContentControls(true);
            var valuesToFillNew = new Content();
            var valuesToFill = valuesToFillNew.Append(new FieldContent("Date", DateOnly.FromDateTime(DateTime.Now).ToString()));

            var queryWarehouses = _db.Warehouses;
            List<TableRowContent> rows = new();
            var sumProducts = queryWarehouses.Sum(w => w.Products.Count);
            foreach (var warehouse in queryWarehouses.ToList())
            {
                rows.Add(new TableRowContent(new FieldContent("WarehouseId", warehouse.Id.ToString()),
                new FieldContent("WarehouseAddress", warehouse.Address),
                new FieldContent("WarehouseProductCount", warehouse.Products.Count.ToString()),
                new FieldContent("WarehouseProductPercentage", (1.0 * warehouse.Products.Count / sumProducts * 100).ToString()),
                new FieldContent("WarehouseProductMaxWorth", warehouse.Products.Max(p => p.Nomenclature!.ProductWorths.OrderBy(pw => pw.Date).ThenBy(pw => pw.Id).FirstOrDefault()!.Worth).ToString())));
            }
            valuesToFill = valuesToFill.Append(new TableContent("WarehouseProducts", rows));


            rows = new();
            var queryBrokenProducts = _db.Products.Where(p => p.Status == Product.ProductStatus.Списан);
            foreach (var product in queryBrokenProducts.ToList())
            {
                rows.Add(new TableRowContent(new FieldContent("BrokenProductId", product.Id.ToString()),
                new FieldContent("BrokenProductName", product.Nomenclature!.Name)));
            }
            valuesToFill = valuesToFill.Append(new TableContent("BrokenProducts", rows));


            outputDocument.FillContent(new Content(valuesToFill.ToArray()));
            outputDocument.SaveChanges();

            return File(reportPath, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Отчет о заполненности складов (от {DateOnly.FromDateTime(DateTime.Now)}).docx");
        }

        [HttpPost]
        public IActionResult ExcelReport()
        {            
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Отчет по продуктам");

            sheet.Cells["B2"].Value = "Отчет по продуктам";
            sheet.Cells["B2"].Style.Font.Bold = true;
            sheet.Cells["B3"].Value = "Дата = ";
            sheet.Cells["B3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells["C3"].Value = DateOnly.FromDateTime(DateTime.Now);

            sheet.Cells["B4:I4"].LoadFromArrays(new object[][] { new[] { "ID продукта", "Наименование", "Статус", "Адрес на складе", "ИНН поставщика", "Дата поставки", "Адрес склада", "Стоимость" } });

            var list = _db.Products.Include(p => p.Nomenclature).Include(p => p.Delivery).Include(p => p.Delivery!.Supplier).Include(p => p.Warehouse).Include(p => p.Nomenclature!.ProductWorths);
            var row = 5;
            var column = 2;
            foreach (var item in list)
            {
                sheet.Cells[row, column].Value = item.Id;
                sheet.Cells[row, column + 1].Value = item.Nomenclature!.Name;
                sheet.Cells[row, column + 2].Value = item.Status;
                sheet.Cells[row, column + 3].Value = item.CellAddress;
                sheet.Cells[row, column + 4].Value = item.Delivery!.Supplier!.Tin;
                sheet.Cells[row, column + 5].Value = item.Delivery.Date;
                sheet.Cells[row, column + 6].Value = item.Warehouse == null ? "-" : item.Warehouse.Address;
                sheet.Cells[row, column + 7].Value = item.Nomenclature.ProductWorths.OrderBy(pw => pw.Date).ThenBy(pw => pw.Id).FirstOrDefault()!.Worth;
                row++;
            }

            sheet.Cells[1, 1, row, column + 9].AutoFitColumns();
            sheet.Cells[4, 2, row, column + 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

            sheet.Cells[row, column + 6].Value = "Итого:";
            sheet.Cells[5, column + 7, row - 1, column + 7].Style.Numberformat.Format = "#";
            sheet.Cells[row, column + 7].Formula = $"SUM({(char)('A' + column + 6)}{5}:{(char)('A' + column + 6)}{row - 1})";
            sheet.Cells[row, column + 7].Calculate();
            sheet.Cells[row, column + 8].Value = "Кол-во:";
            sheet.Cells[row, column + 9].Value = _db.Products.Count();
            sheet.Cells[row, column + 6, row, column + 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);

            sheet.Protection.IsProtected = true;

            string path = "/files/productReport.xlsx";
            System.IO.File.WriteAllBytes(_appEnvironment.WebRootPath + path, package.GetAsByteArray());

            return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Отчет о продуктах.xlsx");
        }
    }
}