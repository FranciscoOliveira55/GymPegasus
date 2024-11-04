namespace BackEnd.Models
{
    public class WorkoutExercise : Exercise
    {
        public int Id { get; set; }
        public required bool Done { get; set; } = false;

        public required int WorkoutId { get; set; }
        public virtual Workout? Workout { get; set; }

    }
}
