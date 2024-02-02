namespace MSIT155Site.Models.DTO
{
    public class SpotsPagingDTO
    {
        public int TotalPages { get; set; }
        // 用不到
        public int TotalCount { get; set; }

        public List<SpotImagesSpot>? SpotsResult { get; set; }

    }
}
