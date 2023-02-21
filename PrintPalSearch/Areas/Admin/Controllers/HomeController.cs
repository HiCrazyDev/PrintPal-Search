using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintPalSearch.Models.Database;
using System;

namespace PrintPalSearch.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        printpal_search_dbContext _context;

        public HomeController(printpal_search_dbContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult Excelupload()
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
                return NotFound();
                throw e;
            }
        }

        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            try
            {
                var willDeleted = _context.Products.Where(s => s.Id == id).Single();
                if(willDeleted == null)
                {
                    return Json(new { status = "failed" });
                }
                _context.Products.Remove(willDeleted);
                _context.SaveChanges();
                return Json(new { status = "success" });
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public ActionResult UpdateProduct(int id, string title, string category, string description, string link)
        {
            try
            {
                var willUpdatedOne = _context.Products.Where(s => s.Id == id).Single();
                if (willUpdatedOne == null)
                {
                    return Json(new { status = "failed" });
                }
                willUpdatedOne.ProductTitle         = title;
                willUpdatedOne.ProductCategory      = category;
                willUpdatedOne.ProductDescription   = description;
                willUpdatedOne.ProductSrcurl        = link;
                _context.Products.Update(willUpdatedOne);
                _context.SaveChanges();
                return Json(new { status = "success" });
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }
    }
}
