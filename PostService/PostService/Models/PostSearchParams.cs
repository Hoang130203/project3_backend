using SocialAppObjects.Enums;

namespace PostService.Models
{
    public class PostSearchParams
    {
        public string? Keyword { get; set; }
        public List<string>? HashTags { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<Guid>? AuthorIds { get; set; }
        public List<Guid>? GroupIds { get; set; }
        public Location? NearLocation { get; set; }
        public double? RadiusKm { get; set; }
        public PostStatus? Status { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }

}
