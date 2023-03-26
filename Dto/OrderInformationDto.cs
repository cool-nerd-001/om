using System.ComponentModel.DataAnnotations;

namespace OrderMicroservice.Dto
{
    public class OrderInformationDto
    {

        [Required]
        public Guid CId { get; set; }

        [Required]
        public Guid PId { get; set; }


        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Price Should be a Positive Number")]
        public int? Price { get; set; }


        [Range(1, 1000, ErrorMessage = "Quantity Should be a Positive Number")]
        public int Quantity { get; set; } = 1;


        [Required]
        public string? CreditCardNumber { get; set; }

        [Required]
        public int? ExpiryYear { get; set; }


        [Required]
        public int? Cvv { get; set; }
    }
}
