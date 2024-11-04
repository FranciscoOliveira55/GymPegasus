namespace FrontEndWeb.Models
{
    public class Workout: EntityBase
    {
        public int Id { get; set; }
        public required int ClienteId { get; set; }
        public DateTime HourInit { get; set; } = DateTime.Now;
        public DateTime HourEnd { get; set; }


        public int? WorkoutPlanId { get; set; }
        public WorkoutPlan? WorkoutPlan { get; set; }
        public List<WorkoutExercise>? WorkoutExercises { get; set; }

    }
}
