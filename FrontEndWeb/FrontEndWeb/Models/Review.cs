using FrontEndWeb.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace FrontEndWeb.Models
{
    public class Review:EntityBase
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }

        [Range(0, 5, ErrorMessage = "Classification must be between 0 and 5.")]
        public required double Classification { get; set; }
        public string? Description { get; set; }

        public int? ClienteId { get; set; }
        public virtual Cliente? Cliente { get; set; }
        public required int EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }


    }
}
