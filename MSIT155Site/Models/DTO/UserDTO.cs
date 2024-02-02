namespace MSIT155Site.Models.DTO
{
    public class UserDTO
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int? Age { get; set; }
        // 透過 IFormFile 接收上傳的檔案
        public IFormFile? Avatar { get; set; }
    }
}
