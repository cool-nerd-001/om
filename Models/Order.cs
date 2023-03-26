using System.ComponentModel.DataAnnotations;

namespace OrderMicroservice.Models
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; }

        public Guid CId { get; set; }

        public Guid PId { get; set; }

        public DateTime OrderDate { get; set; }

        public int? OrderAmount { get; set; }
    }
}
