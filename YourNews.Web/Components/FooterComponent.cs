using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YourNews.Core.Data;
using YourNews.Core.Models;

namespace YourNews.Web.Components
{
    public class FooterComponent:ViewComponent
    {
        private ApplicationDbContext context;
        //servisleri alabilmek için constructor tanımlanır
        public FooterComponent(ApplicationDbContext context)
        {
            this.context = context;
        }

        //InvokeAsync component'in dışarıdan çağırılan ve view döndüren metodudur
        public async Task<IViewComponentResult> InvokeAsync(int count = 5)
        {
            var recentNews = await GetRecentNews(count);
            return View(recentNews);
        }

        public async Task<IEnumerable<News>> GetRecentNews(int count)
        {
            return await context.News.OrderByDescending(n => n.PublishDate).Take(count).ToListAsync();
        }
    }
}
