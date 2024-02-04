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
        //IWebHostEnvironment 介面，提供應用程式執行所在之伺服器的相關環境資訊
        private readonly IWebHostEnvironment _environment;
        public ApiController(MyDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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
        //public IActionResult Register(UserDTO _user)
        [HttpPost]
        public IActionResult Register(UserDTO _user)
        {
            if (string.IsNullOrEmpty(_user.Name))
            {
                _user.Name = "guest";
            }
            //如果沒有選圖，按下送出按鈕，會將empty.jpg複製到uploads資料夾
            string fileName = "empty.jpg";
            if (_user.Avatar != null)
            {
                fileName = _user.Avatar.FileName;
            }
            //string uploadPath = @"C:\Shared\AjaxWorkspace\MSIT155Site\wwwroot\uploads\a.jpg";
            //Path.Combine方法 : 將多個字串合併為一個路徑
            //WebRootPath 取得 wwwroot 的實際路徑//ContentRootPath 取得專案資料夾的實際路徑
            string uploadPath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
            //檔案上傳 FileStream(實際路徑, FileMode.Create)
            using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            {
                //把圖檔複製到指定的路徑(fileStream):uploads資料夾
                _user.Avatar?.CopyTo(fileStream);
            }

            //新增到資料庫
            //_context.Members.Add(_user);
            //_context.SaveChanges();
            return Content($"Hello {_user.Name}, {_user.Age}歲了, 電子郵件是 { _user.Email} ，檔案的FileName:{_user.Avatar?.FileName} ， 檔案的ContentType:{_user.Avatar?.ContentType} ， 檔案的Length: {_user.Avatar?.Length}", "text/plain", Encoding.UTF8);
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
        //景點資料
        [HttpPost]
        public IActionResult Spots([FromBody] SearchDTO _search)
        {
            //根據分類編號搜尋
            var spots = _search.CategoryId == 0 ? _context.SpotImagesSpots : _context.SpotImagesSpots.Where(s => s.CategoryId == _search.CategoryId);
            //根據關鍵字搜尋
            if (!string.IsNullOrEmpty(_search.Keyword))
            {
                spots = spots.Where(s => s.SpotTitle.Contains(_search.Keyword) || s.SpotDescription.Contains(_search.Keyword));
            }
            if (!string.IsNullOrEmpty(_search.Title))
            {
                spots = spots.Where(s => s.SpotTitle.Contains(_search.Title)).Take(8);
            }
            //排序
            switch (_search.SortBy)
            {
                case "spotTitle":
                    spots = _search.SortType == "asc" ? spots.OrderBy(s => s.SpotTitle) : spots.OrderByDescending(s => s.SpotTitle);
                    break;
                case "categoryId":
                    spots = _search.SortType == "asc" ? spots.OrderBy(s => s.CategoryId) : spots.OrderByDescending(s => s.CategoryId);
                    break;
                default: //spotId
                    spots = _search.SortType == "asc" ? spots.OrderBy(s => s.SpotId) : spots.OrderByDescending(s => s.SpotId);
                    break;
            }

            //總共有幾筆
            int totalCount = spots.Count();
            //一頁幾筆資料//沒有寫pageSize的話是9
            int pageSize = _search.PageSize ?? 9;
            //計算總共有幾頁//Math.Ceiling()無條件進位//Math.Floor()無條件捨去
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            //目前第幾頁//沒有寫page的話是1
            int page = _search.Page ?? 1;

            //分頁
            //Skip 略過運算子略過序列中指定數目的項目，然後傳回其餘的項目。
            //Take 運算子傳回序列中指定數目的項目，然後略過其餘的項目。
            spots = spots.Skip((page - 1) * pageSize).Take(pageSize);

            SpotsPagingDTO spotsPaging = new SpotsPagingDTO();
            //因為有需要計算才有的功能，所以才要再多一個DTO
            spotsPaging.TotalPages = totalPages;
            //用不到
            //spotsPaging.TotalCount = totalCount;
            spotsPaging.SpotsResult = spots.ToList();
            return Json(spotsPaging);
        }
        public IActionResult SpotTitle(string title)
        {
            var titles = _context.Spots.Where(s => s.SpotTitle.Contains(title)).Select(s => s.SpotTitle).Take(8);
            return Json(titles);
        }
    }
}
