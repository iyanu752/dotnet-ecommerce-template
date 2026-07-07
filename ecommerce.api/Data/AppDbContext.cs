using System;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.api;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Product> Products => Set<Product>();

}
