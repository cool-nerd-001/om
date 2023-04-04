using System.ComponentModel.DataAnnotations;

namespace OrderMicroservice.Dto
{
    public class OrderDto
    {
        [Required]
        public string? Address { get; set; }

        [Required]
        public List<ProductsDto>? Orders { get; set; }

        [Required]
        public PaymentDto? Payment { get; set; }
    }
}
