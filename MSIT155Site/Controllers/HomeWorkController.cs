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
        //IWebHostEnvironment 介面，提供應用程式執行所在之伺服器的相關環境資訊
        private readonly IWebHostEnvironment _environment;
        public HomeWorkController(MyDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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
            if (_context.Members.Any(x => x.Name == name))
            {
                return Content("True", "text/plain", Encoding.UTF8);
            }
            return Content("False", "text/plain", Encoding.UTF8);

            //var member = await _context.Members
            //    .FirstOrDefaultAsync(m => m.Name == name);

            //string Name = member.Name;
            //if (Name == name)
            //{
            //    return Content("帳號已存在", "text/plain", Encoding.UTF8);
            //}
            //return Content("帳號可使用", "text/plain", Encoding.UTF8);
        }

        public IActionResult RegisterToDB()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegisterAjax(Member _user, IFormFile Avatar)
        {
            if (string.IsNullOrEmpty(_user.Name))
            {
                _user.Name = "guest";
            }
            //如果沒有選圖，按下送出按鈕，會將empty.jpg複製到uploads資料夾
            string fileName = "empty.jpg";
            if (Avatar != null)
            {
                fileName = Avatar.FileName;
            }
            //string uploadPath = @"C:\Shared\AjaxWorkspace\MSIT155Site\wwwroot\uploads\a.jpg";
            //Path.Combine方法 : 將多個字串合併為一個路徑
            //WebRootPath 取得 wwwroot 的實際路徑//ContentRootPath 取得專案資料夾的實際路徑
            string uploadPath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
            //檔案上傳 FileStream(實際路徑, FileMode.Create)
            using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            {
                //把圖檔複製到指定的路徑(fileStream):uploads資料夾
                Avatar?.CopyTo(fileStream);
            }

            //新增到資料庫
            _user.FileName = fileName;
            
            byte[]? imgByte = null;
            //把資料放進去記憶體資料流
            using (var memoryStream = new MemoryStream())
            {
                Avatar?.CopyTo(memoryStream);
                //轉成二進位
                imgByte = memoryStream.ToArray();
            }
            _user.FileData = imgByte;

            _context.Members.Add(_user);
            _context.SaveChanges();

            return Content($"Hello {_user.Name}, {_user.Age}歲了, 電子郵件是 {_user.Email} ，檔案的FileName:{Avatar?.FileName} ， 檔案的ContentType:{Avatar?.ContentType} ， 檔案的Length: {Avatar?.Length} ， uploadPath: {uploadPath}", "text/plain", Encoding.UTF8);
        }
    }
}
