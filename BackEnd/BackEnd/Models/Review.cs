using BackEnd.Models.Enums;

namespace BackEnd.Models
{
    public class Review : EntityBase
    {
        public int Id { get; set; }
        public required DateTime DateTime { get; set; } = DateTime.Now;
        public required double Classification { get; set; }
        public string? Description { get; set; }

        public int? ClienteId { get; set; }
        public required int EmployeeId { get; set; }

        public virtual Cliente? Cliente { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}
