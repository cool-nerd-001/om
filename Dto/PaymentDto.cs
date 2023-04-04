namespace OrderMicroservice.Dto
{
    public class PaymentDto
    {
        public string? creditCardNumber { get; set; }
        public string? cvv { get; set; } 
        
        public string? expiryYear { get; set; }

        
    }
}
