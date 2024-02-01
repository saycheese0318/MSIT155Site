using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSIT155Site.Models;
using MSIT155Site.Models.DTO;
using System.Text;

namespace MSIT155Site.Controllers
{
    public class ApiController : Controller
    {
        private readonly MyDBContext _context;
        public ApiController(MyDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            Thread.Sleep(3000);
            //int x = 10;
            //int y = 0;
            //int z = x / y;
            //Encoding.UTF8 將前面的Content編碼
            return Content("我是 Content", "text/plain", Encoding.UTF8);
        }

        //資料的接收
        //public IActionResult Register(string name, int age = 28)
        public IActionResult Register(UserDTO _user)
        {
            if (string.IsNullOrEmpty(_user.Name))
            {
                _user.Name = "guest";
            }
            return Content($"Hello {_user.Name}, {_user.Age}歲了, 電子郵件是 { _user.Email}", "text/plain", Encoding.UTF8);
        }
        //讀取城市
        public IActionResult Cities()
        {
            //回傳資料
            var cities = _context.Addresses.Select(a => a.City).Distinct();
            return Json(cities);
        }
        //根據城市名稱讀取鄉鎮區
        public IActionResult District(string city)
        {
            var districts = _context.Addresses.Where(a => a.City == city).Select(a => a.SiteId).Distinct();
            return Json(districts);
        }
        //根據鄉鎮區名稱讀取路名
        public IActionResult Township(string District)
        {
            var townships = _context.Addresses.Where(a => a.SiteId == District).Select(a => a.Road).Distinct();
            return Json(townships);
        }
        public IActionResult Avatar(int id = 1)
        {
            Member? member = _context.Members.Find(id);
            if (member != null)
            {
                byte[] img = member.FileData;
                if (img != null)
                {
                    return File(img, "image/jpeg");
                }
            }
            return NotFound();
        }

    }
}
