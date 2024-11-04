using FrontEndWeb.Models.Enums;

namespace FrontEndWeb.Models
{
    public abstract class Exercise:EntityBase
    {
        public double? Effort { get; set; }
        public int? Repeat { get; set; }

        public required int ExerciseTemplateId { get; set; }
        public ExerciseTemplate? ExerciseTemplate { get; set; }

    }
}
