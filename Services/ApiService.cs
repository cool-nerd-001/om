using OrderMicroservice.Dto;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace OrderMicroservice.Services
{
    public class ApiService : IApiService
    {

        private readonly IConfiguration _configuration;

        public ApiService(IConfiguration configuration)
        {

            _configuration = configuration ??
                    throw new ArgumentNullException(nameof(configuration));

        }


        public Guid getUserId(string token)
        {

            // Parse the JWT token and extract the claims
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = jwtToken.Claims;

            // Access the claims
            var Id = claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            return new Guid(Id);

        }


        public async Task<HttpResponseMessage> isAuthorized(string token)
        {
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {

                string? domin = _configuration["ActiveDirectoryMicroservice:domin"];
                client.BaseAddress = new Uri(domin);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                response = await client.GetAsync("/api/rest/v1/validate/user/");
            }


            return response;
        }

        public async Task<HttpResponseMessage> areValidProducts(List<Guid> ProductIdList)
        {
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {
                string? domin = _configuration["ProductMicroservice:domin"];
                client.BaseAddress = new Uri(domin);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                response = await client.PostAsJsonAsync("/api/rest/v1/verify/products", new { array = ProductIdList });

            }
            return response;
        }

        public async Task<HttpResponseMessage> processPayment(PaymentDto paymentInfo)
        {
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {
                string? domin = _configuration["PaymentMicroservice:domin"];
                client.BaseAddress = new Uri(domin);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                response = await client.PostAsJsonAsync("/api/rest/v1/payment", paymentInfo);

            }
            return response;

        }
    }
}
