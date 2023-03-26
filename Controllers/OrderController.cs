using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderMicroservice.DbContexts;
using OrderMicroservice.Dto;
using OrderMicroservice.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace OrderMicroservice.Controllers
{
    [Route("api/rest/v1/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly OrderMicroserviceDbContext _context;
        private readonly IConfiguration _configuration;

        public OrderController(OrderMicroserviceDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration ??
                    throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost("product")]
        public async Task<IActionResult> PlaceOrder(OrderInformationDto data)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var client = new HttpClient())
            {
                string? domin = _configuration["ProductMicroservice:domin"];
                client.BaseAddress = new Uri(domin);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("/api/rest/v1/verify/" + data.PId);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();

                }

            }



            PaymentDetailsDto payment_details = new PaymentDetailsDto()
            {
                CreditCardNumber = data.CreditCardNumber,
                ExpiryYear = data.ExpiryYear,
                Cvv = data.Cvv

            };

            using (var client = new HttpClient())
            {
                string? domin = _configuration["PaymentMicroservice:domin"];
                client.BaseAddress = new Uri(domin);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.PostAsJsonAsync("/api/rest/v1/payment", payment_details).Result;


                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest();

                }

            }


            Order new_record = new Order()
            {
                OrderId = Guid.NewGuid(),
                CId = data.CId,
                PId = data.PId,
                OrderDate = DateTime.Now,
                OrderAmount = data.Quantity * data.Price
            };

            await _context.Orders.AddAsync(new_record);
            await _context.SaveChangesAsync();

            return Ok(new_record);


        }

        [HttpGet("{customerId:guid}")]
        public async Task<IActionResult> GetOrders([FromRoute] Guid customerId)
        {
            var records = _context.Orders.Where(x => x.CId == customerId);

            if (!records.Any())
            {
                return Ok(new { array = records });
            }

            return Ok(new { array = records });
        }
    }
}
