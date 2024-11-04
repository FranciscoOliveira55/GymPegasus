namespace FrontEndWeb.Models
{
    public class Event:EntityBase
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }


        public ICollection<Schedule>? Schedules { get; set; }

    }
}
