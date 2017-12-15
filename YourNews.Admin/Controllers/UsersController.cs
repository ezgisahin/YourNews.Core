using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using YourNews.Admin.Models;
using YourNews.Core.Data;
using YourNews.Core.Models;

namespace YourNews.Admin.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                login.Password = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(login.Password)));
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.UserName && u.Password == login.Password);
                if (user != null)
                {
                    //login işlemi
                    HttpContext.Session.SetString("UserName",login.UserName);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("Result", "Geçersiz kullanıcı adı veya şifre!");
            }
            return View(login);
        }

        public IActionResult Logout()
        {
            Security.LoginCheck(HttpContext);
            HttpContext.Session.SetString("UserName", "");
            return RedirectToAction("Index","Home");
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            Security.LoginCheck(HttpContext);
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            Security.LoginCheck(HttpContext);
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            //Security.LoginCheck(HttpContext);
            User u = new User();
            u.CreateDate = DateTime.Now;
            u.CreatedBy = User.Identity.Name;
            u.UpdateDate = DateTime.Now;
            u.UpdatedBy = User.Identity.Name;
            return View(u);
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Password,ConfirmPassword,CreateDate,CreatedBy,UpdateDate,UpdatedBy")] User user)
        {
            //Security.LoginCheck(HttpContext);
            if (ModelState.IsValid)
            {
                user.CreateDate = DateTime.Now;
                user.CreatedBy = User.Identity.Name;
                user.UpdateDate = DateTime.Now;
                user.UpdatedBy = User.Identity.Name;
                user.Password = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(user.Password)));
                try
                {
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                //catch (ArgumentException ex)
                //{
                //    //argument hatası olursa burası çalışır
                //}
                catch (Exception ex)
                {
                    //herhangi bir hata olursa burası çalışır (argument dışındakiler)
                    ModelState.AddModelError("Email", "Bu e-posta adresi daha önce kullanılmış.");
                }
                //finally
                //{
                //    //her zaman çalışır
                //}
                
            }
            return View(user);
        }

        //aktif oturumu olan kullanıcının düzenlenmesi
        public async Task<IActionResult> EditProfile()
        {
            Security.LoginCheck(HttpContext);

            var userName = HttpContext.Session.GetString("UserName");
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Email == userName);
            if (user == null)
            {
                return NotFound();
            }
            return View("Edit",user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            Security.LoginCheck(HttpContext);

            var user = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Password,ConfirmPassword,CreateDate,CreatedBy,UpdateDate,UpdatedBy")] User user)
        {
            Security.LoginCheck(HttpContext);
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.UpdateDate = DateTime.Now;
                    user.UpdatedBy = User.Identity.Name;

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            Security.LoginCheck(HttpContext);
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Security.LoginCheck(HttpContext);
            var user = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
