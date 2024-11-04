using FrontEndWeb.Models.DTOs;
using FrontEndWeb.Models.Enums;

namespace FrontEndWeb.Models
{
    public class ExerciseTemplate:EntityBase
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required EffortUnit EffortUnit { get; set; }
        public  required RepeatUnit RepeatUnit { get; set; }



        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ExerciseTemplate other)
            {
                return Id == other.Id;
            }
            return false;
        }
    }
}
