using BackEnd.Models.Enums;

namespace BackEnd.Models
{
    public abstract class Exercise : EntityBase
    {
        public double? Effort { get; set; }
        public int? Repeat { get; set; }

        public required int ExerciseTemplateId { get; set; }
        public virtual ExerciseTemplate? ExerciseTemplate { get; set; }


    }
}
