using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourNews.Core.Data;
using YourNews.Web.Models;

namespace YourNews.Web.Controllers
{
    public class CategoryController : Controller
    {
        private ApplicationDbContext context;
        public CategoryController(ApplicationDbContext context)
        {
            this.context = context;
        }
        
        public IActionResult Index(int? id)
        {
            var categories = context.Categories.OrderBy(o => o.Name).Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name, Quantity = c.News.Count }).ToList();
            ViewBag.Categories = categories;

            var news = context.News.Include(i => i.Category).Where(n => (id != null?n.CategoryId == id:true) && n.IsPublished == true).OrderByDescending(o => o.PublishDate).ToList();

            if (id != null)
            {
                ViewBag.ActiveCategory = context.Categories.FirstOrDefault(c => c.Id == id);
            }
            return View(news);
        }
    }
}