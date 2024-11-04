using FrontEndWeb.Models.Enums;

namespace FrontEndWeb.Models
{
    public class GetInOut : EntityBase
    {
        public int Id { get; set; }
        public required GetInOutType GetInOutType { get; set; }
        public required DateTime DateTime { get; set; } = DateTime.Now;

        public required int ClienteId { get; set; }
    }
}
