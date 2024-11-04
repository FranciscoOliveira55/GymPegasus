namespace BackEnd.Models
{
    public class Workout : EntityBase
    {
        public int Id { get; set; }
        public required DateTime HourInit { get; set; } = DateTime.Now;
        public DateTime HourEnd { get; set; }


        public required int ClienteId { get; set; }
        public virtual Cliente? Cliente { get; set; }
        public int? WorkoutPlanId { get; set; }
        public virtual WorkoutPlan? WorkoutPlan { get; set; }
        public virtual ICollection<WorkoutExercise>? WorkoutExercises { get; set; } 

    }
}
