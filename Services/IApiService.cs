using OrderMicroservice.Dto;

namespace OrderMicroservice.Services
{
    public interface IApiService
    {


        public  Task<HttpResponseMessage> isAuthorized(string token);

        public Guid getUserId(string token);


        public Task<HttpResponseMessage> areValidProducts(List<Guid> ProductIdList);

        public Task<HttpResponseMessage> processPayment(PaymentDto paymentInfo);
    }
}
