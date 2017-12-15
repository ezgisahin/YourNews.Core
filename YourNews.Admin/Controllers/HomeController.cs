using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YourNews.Admin.Models;

namespace YourNews.Admin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Security.LoginCheck(HttpContext);
            return View();
        }

        public IActionResult About()
        {
            Security.LoginCheck(HttpContext);
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            Security.LoginCheck(HttpContext);
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
