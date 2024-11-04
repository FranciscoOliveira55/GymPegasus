using BackEnd.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    public class GetInOut : EntityBase
    {
        public int Id { get; set; }
        public required GetInOutType GetInOutType { get; set; }
        public required DateTime DateTime { get; set; } = DateTime.Now;

        public required int ClienteId { get; set; }
        public virtual Cliente? Cliente { get; set; }
    }
}
