using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MSIT155Site.Models;
using MSIT155Site.Models.DTO;
using System.Text;

namespace MSIT155Site.Controllers
{
    public class HomeWorkController : Controller
    {
        private readonly MyDBContext _context;
        public HomeWorkController(MyDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Card()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public async Task<IActionResult> CheckAccount(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Content("帳號未填寫", "text/plain", Encoding.UTF8);
            }
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Name == name);

            string Name = member.Name;
            if (Name == name)
            {
                return Content("帳號已存在", "text/plain", Encoding.UTF8);
            }
            return Content("帳號可使用", "text/plain", Encoding.UTF8);
        }
    }
}
