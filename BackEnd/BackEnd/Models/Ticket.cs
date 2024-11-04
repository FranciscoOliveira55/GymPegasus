using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    [PrimaryKey(nameof(ClienteId), nameof(ScheduleId))]
    public class Ticket : EntityBase
    {
        public required int ClienteId { get; set; }
        public virtual Cliente? Cliente { get; set; }

        public required int ScheduleId { get; set; }
        public virtual Schedule? Schedule { get; set; }


    }
}
