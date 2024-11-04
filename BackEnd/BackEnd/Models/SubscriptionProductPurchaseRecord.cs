namespace BackEnd.Models
{
    public class SubscriptionProductPurchaseRecord : EntityBase
    {
        public int Id { get; set; }
        public DateTime PurchaseDate { get; set; }
        public long PurchasePrice { get; set; }
        public long PurchaseTimeTicks { get; set; }

        public int ClienteId { get; set; }
        public int? SubscriptionProductId { get; set; }

        public virtual Cliente? Cliente { get; set; }
        public virtual SubscriptionProduct? SubscriptionProduct { get; set; }
    }
}
