namespace FrontEndWeb.Models
{
    public class WorkoutExercise : Exercise
    {
        public int Id { get; set; }
        public required int WorkoutId { get; set; }
        public bool Done { get; set; } = false;
    }
}
