using BackEnd.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    [Table("Clientes")]
    [Comment("Clients of the gym")]
    public class Cliente: EntityBase
    {
        public int Id { get; set; }
        public string? ClienteName { get; set; }
        public string? Nif { get; set; }
        public DateTime? SubscriptionExpiration { get; set; } = DateTime.Now;

        public DateTime? BirthDate { get; set; }
        public Gender? Gender { get; set; }

        public string? Motto { get; set; }
        public string? ImageUrl { get; set; }  


        public required string UserId { get; set; }
        public virtual User? User { get; set; }

        public virtual ICollection<GetInOut>? GetInOuts { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<Ticket>? Tickets { get; set; }
        public virtual ICollection<WorkoutPlan>? WorkoutPlans { get; set; }
        public virtual ICollection<Workout>? Workout { get; set; }
        public virtual ICollection<SubscriptionProductPurchaseRecord>? SubscriptionProductPurchaseRecords { get; set; }

    }
}
