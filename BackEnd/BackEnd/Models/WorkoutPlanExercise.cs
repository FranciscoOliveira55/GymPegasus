using BackEnd.Models.Enums;

namespace BackEnd.Models
{
    public class WorkoutPlanExercise : Exercise
    {
        public int Id { get; set; }
        public required int WorkoutPlanId { get; set; }
        public virtual WorkoutPlan? WorkoutPlan { get; set; }

    }
}
