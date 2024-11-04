using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FrontEndWeb.Models
{
    public class SubscriptionProductPurchaseRecord : EntityBase
    {
        public int Id { get; set; }
        public DateTime PurchaseDate { get; set; }
        public long PurchasePrice { get; set; }
        public long PurchaseTimeTicks { get; set; }

        [JsonIgnore]
        [NotMapped]
        public TimeSpan PurchaseTime
        {
            get => TimeSpan.FromTicks(PurchaseTimeTicks); // Convert ticks to TimeSpan
            set => PurchaseTimeTicks = value.Ticks; // Convert TimeSpan to ticks
        }

        public int ClienteId { get; set; }
        public int? SubscriptionProductId { get; set; }

        public virtual Cliente? Cliente { get; set; }
        public virtual SubscriptionProduct? SubscriptionProduct { get; set; }
    }
}
