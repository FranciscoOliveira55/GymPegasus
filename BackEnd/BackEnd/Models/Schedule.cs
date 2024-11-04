namespace BackEnd.Models
{
    public class Schedule : EntityBase
    {
        public int Id { get; set; }
        public required string Room { get; set; }
        public required DateTime HourInit { get; set; }
        public required DateTime HourEnd { get; set; }
        public int? MaxCapacity { get; set; } = 20;

        public required int EventId { get; set; }
        public virtual Event? Event { get; set; }
        public int? EmployeeID { get; set; }
        public virtual Employee? Employee { get; set; }
        public virtual ICollection<Ticket>? Tickets { get; set; }


    }
}
