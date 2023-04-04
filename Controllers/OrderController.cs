using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderMicroservice.DbContexts;
using OrderMicroservice.Dto;
using OrderMicroservice.Models;
using OrderMicroservice.Utils;
using System.Collections;
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
        public async Task<IActionResult> PlaceOrder(OrderDto data)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            // Retrieve the JWT token from the Authorization header
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var token = authorizationHeader.Replace("Bearer ", "");

            Helper h = new Helper(_configuration);

            var flag = await h.isAuthorised(token);

            if (!flag)
            {
                return Unauthorized();
            }

            Guid CId = h.getUserId(token);






            ArrayList ProductIdList = new ArrayList();
            foreach (var i in data.Orders)
            {
                ProductIdList.Add(i.pId);
            }

            using (var client = new HttpClient())
            {
                string? domin = _configuration["ProductMicroservice:domin"];
                client.BaseAddress = new Uri(domin);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                

                HttpResponseMessage response = client.PostAsJsonAsync("/api/rest/v1/verify/products", new {array = ProductIdList }).Result;

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest(new {error = "invalid products"});

                }

            }





            using (var client = new HttpClient())
            {
                string? domin = _configuration["PaymentMicroservice:domin"];
                client.BaseAddress = new Uri(domin);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.PostAsJsonAsync("/api/rest/v1/payment", data.Payment).Result;

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest(new {error = "invalid payment details"});

                }

            }



            foreach(var i in data.Orders)
            {
                Order new_record = new Order()
                {
                    OrderId = Guid.NewGuid(),
                    CId = CId,
                    PId = i.pId,
                    ProductName = i.name,
                    quantity = i.quantity,
                    OrderAmount = i.quantity * i.price,
                    OrderDate = DateTime.Now,
                    Address = data.Address
                    
                };
               await _context.Orders.AddAsync(new_record);

            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("items")]
        public async Task<IActionResult> GetOrders()
        {
            // Retrieve the JWT token from the Authorization header
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var token = authorizationHeader.Replace("Bearer ", "");

            Helper h = new Helper(_configuration);

            var flag = await h.isAuthorised(token);

            if (!flag)
            {
                return Unauthorized();
            }

            Guid CId = h.getUserId(token);
            var records = _context.Orders.Where(x => x.CId == CId);

            if (!records.Any())
            {
                return Ok(new { array = records });
            }

            return Ok(new { array = records });
        }




    }
}
