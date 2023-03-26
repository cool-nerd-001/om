using System.ComponentModel.DataAnnotations;

namespace OrderMicroservice.DbContexts
{
    public class PaymentDetailsDto
    {

        public string? CreditCardNumber { get; set; }

  
        public int? ExpiryYear { get; set; }


        public int? Cvv { get; set; }
    }
}
