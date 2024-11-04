namespace BackEnd.Models
{
    public class WorkoutPlan : EntityBase
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

        public required int ClienteId { get; set; }
        public virtual Cliente? Cliente { get; set; }
        public int? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
        public virtual ICollection<WorkoutPlanExercise>? WorkoutPlanExercises { get; set; } 
    }
}
