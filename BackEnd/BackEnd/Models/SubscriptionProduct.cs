using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BackEnd.Models
{
    public class SubscriptionProduct: ProductBase
    {
        public long SubscriptionTimeTicks { get; set; } // Store TimeSpan as ticks in the database

        [JsonIgnore]
        [NotMapped]
        public TimeSpan SubscriptionTime
        {
            get => TimeSpan.FromTicks(SubscriptionTimeTicks); // Convert ticks to TimeSpan
            set => SubscriptionTimeTicks = value.Ticks; // Convert TimeSpan to ticks
        }
    }
}
