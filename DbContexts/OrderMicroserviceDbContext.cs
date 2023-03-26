using Microsoft.EntityFrameworkCore;
using OrderMicroservice.Models;

namespace OrderMicroservice.DbContexts
{
    public class OrderMicroserviceDbContext : DbContext
    {

        public OrderMicroserviceDbContext(DbContextOptions<OrderMicroserviceDbContext> options) : base(options)
        {

        }


        public DbSet<Order> Orders { get; set; }
    }
}
