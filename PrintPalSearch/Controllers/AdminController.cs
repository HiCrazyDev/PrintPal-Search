using Microsoft.AspNetCore.Mvc;
using System;
using System.Xml.Linq;
using ClosedXML.Excel;
using PrintPalSearch.Models.Database;
using System.Net.Http.Headers;

namespace PrintPalSearch.Controllers
{
    public class AdminController : Controller
    {
        printpal_search_dbContext _context;
        public AdminController(printpal_search_dbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddDB(IFormFile file)
        {
            try
            {
                var fileextension = Path.GetExtension(file.FileName);
                var filename = Guid.NewGuid().ToString() + fileextension;
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }
                filepath = Path.Combine(filepath, filename);
                using (FileStream fs = System.IO.File.Create(filepath))
                {
                    file.CopyTo(fs);
                }
                int rowno = 1;
                XLWorkbook workbook = XLWorkbook.OpenFromTemplate(filepath);
                var sheets = workbook.Worksheets.First();
                var rows = sheets.Rows().ToList();
                foreach (var row in rows)
                {
                    if (rowno != 1)
                    {
                        var test = row.Cell(1).Value.ToString();
                        if (string.IsNullOrWhiteSpace(test) || string.IsNullOrEmpty(test))
                        {
                            break;
                        }
                        Product prod = new Product();
                        prod.ProductCategory = row.Cell(1).Value.ToString();
                        prod.ProductTitle = row.Cell(2).Value.ToString();
                        prod.ProductSrcurl = row.Cell(3).Value.ToString();
                        _context.Products.Add(prod);
                    }
                    else
                    {
                        rowno = 2;
                    }
                }
                _context.SaveChanges();
                return Json(new { status = "success" });
            }
            catch (Exception e)
            {
                return Json(new { status = "failed" });
                throw e;
            }
        }
    }
}
