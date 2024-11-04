using Microsoft.EntityFrameworkCore;

namespace BackEnd.Models
{
    [Comment("Anything that can be purchased by a cliente")]
    public class ProductBase: EntityBase
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public long Price { get; set; }
    }
}
