namespace FrontEndWeb.Models
{
    public class WorkoutPlan : EntityBase
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public required int ClienteId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public IEnumerable<WorkoutPlanExercise>? WorkoutPlanExercises { get; set; }
    }
}
