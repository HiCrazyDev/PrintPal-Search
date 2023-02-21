using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrintPalSearch.Models;
using PrintPalSearch.Models.Database;
using System.Diagnostics;

namespace PrintPalSearch.Controllers
{
    public class HomeController : Controller
    {
        private readonly printpal_search_dbContext _context;

        public HomeController(printpal_search_dbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Result(string id)
        {
            var text = id;
            if (string.IsNullOrEmpty(text))
                return View(new List<Product>());
            text.Replace(' ', '*');
            text = String.Format("\"*{0}*\"", text);
            var query = string.Format("SELECT id,  product_title, product_category, product_description, product_image, product_srcurl, KEY_TBL.RANK FROM Products INNER JOIN CONTAINSTABLE(Products, product_title, \'{0}\' ) AS KEY_TBL ON id = KEY_TBL.[KEY] WHERE KEY_TBL.RANK > 2 ORDER BY KEY_TBL.RANK DESC;", text);
            //var query = string.Format("select * FROM dbo.Products WHERE FREETEXT(product_title, '{0}')", text);
            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    var prods = _context.Products.FromSqlRaw(query);
                    ViewBag.searchText = id;
                    return View(prods);
                }
                return View(new List<Product>());
            }
            catch (Exception e)
            {
                return View(new List<Product>());
                throw e;
            }
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


        [HttpPost]
        public ActionResult SearchText(string text)
        {
            // Advanced Free Text Query : 
            // SELECT FT_TBL.id,  
            // FT_TBL.product_title,   
            // KEY_TBL.RANK
            // FROM dbo.Products AS FT_TBL INNER JOIN
            // FREETEXTTABLE(dbo.Products,
            // product_title,
            // '*foil*busi*card*'
            // ) AS KEY_TBL
            // ON FT_TBL.id = KEY_TBL.[KEY]
            // WHERE KEY_TBL.RANK > 2
            // ORDER BY KEY_TBL.RANK DESC;
            // text = "*foil*busi*card*";
            if(string.IsNullOrEmpty(text))
                return Json(new { products = new List<Product>() });
            text.Replace(' ', '*');
            text = String.Format("\"*{0}*\"", text);
            var query = string.Format("SELECT id,  product_title, product_category, product_description, product_image, product_srcurl, KEY_TBL.RANK FROM Products INNER JOIN CONTAINSTABLE(Products, product_title, \'{0}\' ) AS KEY_TBL ON id = KEY_TBL.[KEY] WHERE KEY_TBL.RANK > 2 ORDER BY KEY_TBL.RANK DESC;", text);
            //var query = string.Format("select * FROM dbo.Products WHERE FREETEXT(product_title, '{0}')", text);
            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    var prods = _context.Products.FromSqlRaw(query);
                    return Json(new { products = prods });
                }
                return Json(new { products = new List<Product>() });
            }
            catch (Exception e)
            {
                return Json(new { status = "failed"});
                throw e;
            }
        }
    }
}