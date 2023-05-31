using Microsoft.AspNetCore.Mvc;
using OrderMicroservice.DbContexts;
using OrderMicroservice.Dto;
using OrderMicroservice.Models;
using OrderMicroservice.Services;

namespace OrderMicroservice.Controllers
{
    [Route("api/rest/v1/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly OrderMicroserviceDbContext _context;
        public readonly IApiService _api;

        public OrderController(OrderMicroserviceDbContext context, IApiService api)
        {
            _context = context;

            _api = api ??
                    throw new ArgumentNullException(nameof(api));
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

            HttpResponseMessage authresponse = await _api.isAuthorized(token);

            if (!authresponse.IsSuccessStatusCode)
            {
                return Unauthorized();
            }

            Guid CId = _api.getUserId(token);



            //check if products are valid 
            List<Guid> ProductIdList = new List<Guid>();

            foreach (var i in data.Orders)
            {
                ProductIdList.Add(i.pId);
            }

            HttpResponseMessage productresponse = await _api.areValidProducts(ProductIdList);

            if (!productresponse.IsSuccessStatusCode)
            {
                return BadRequest(new {error = "invalid products"});

            }

           
            //process the payment 
            var paymentresponse = await  _api.processPayment(data.Payment);

            if (!paymentresponse.IsSuccessStatusCode)
            {
                return BadRequest(new { error = "invalid payment details" });
            }
            


            //add order details to database
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

            HttpResponseMessage response = await _api.isAuthorized(token);

            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized();
            }

            Guid CId = _api.getUserId(token);

            var records = _context.Orders.Where(x => x.CId == CId).OrderByDescending(o => o.OrderDate);

            if (!records.Any())
            {
                return Ok(new { array = records });
            }

            return Ok(new { array = records });
        }




    }
}
