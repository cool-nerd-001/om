namespace OrderMicroservice.Dto
{
    public class ProductsDto
    {
        public Guid pId { get; set; } 
        public string? name { get; set; }

        public int quantity { get; set; }

        public int price { get; set; }
    }
}
