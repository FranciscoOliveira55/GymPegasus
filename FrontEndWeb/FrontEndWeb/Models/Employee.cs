using FrontEndWeb.Models.Enums;

namespace FrontEndWeb.Models
{
    public class Employee:EntityBase
    {
        public int Id { get; set; }
        public string? EmployeeName { get; set; }
        public string? Nif { get; set; }
        public EmployeePosition Position { get; set; }

        public DateTime? BirthDate { get; set; }
        public Gender? Gender { get; set; }

        public string? Motto { get; set; }
        public string? ImageUrl { get; set; }

        public required string UserId { get; set; }
        public virtual User? User { get; set; }

        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<Schedule>? Schedules { get; set; }
        public virtual ICollection<WorkoutPlan>? WorkoutPlans { get; set; }


    }
}
