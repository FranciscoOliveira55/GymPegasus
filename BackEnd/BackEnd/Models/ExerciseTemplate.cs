using BackEnd.Models.Enums;

namespace BackEnd.Models
{
    public class ExerciseTemplate : EntityBase
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required EffortUnit EffortUnit { get; set; }
        public  required RepeatUnit RepeatUnit { get; set; }

        //public virtual ICollection<WorkoutExercise>? WorkoutExercises { get; set; }
        //public virtual ICollection<WorkoutPlanExercise>? WorkoutPlanExercises { get; set; }


    }
}
