using FrontEndWeb.Models.Enums;

namespace FrontEndWeb.Models
{
    public class WorkoutPlanExercise : Exercise
    {
        public int Id { get; set; }
        public required int WorkoutPlanId { get; set; }
    }
}
