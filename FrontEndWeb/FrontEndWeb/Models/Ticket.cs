using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontEndWeb.Models
{
    [PrimaryKey(nameof(ClienteId), nameof(ScheduleId))]
    public class Ticket : EntityBase
    {
        public int ClienteId { get; set; }
        public int ScheduleId { get; set; }

    }
}
