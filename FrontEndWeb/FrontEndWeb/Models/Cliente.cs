using FrontEndWeb.Models.Enums;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontEndWeb.Models
{
    public class Cliente:EntityBase
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

        public ICollection<GetInOut>? GetInOuts { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Ticket>? Tickets { get; set; } 
        public ICollection<WorkoutPlan>? WorkoutPlans { get; set; }
        public ICollection<Workout>? Workout { get; set; }
        public ICollection<SubscriptionProductPurchaseRecord>? SubscriptionProductPurchaseRecords { get; set; }
    }
}
