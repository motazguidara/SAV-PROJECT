using CustomerClaimsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerClaimsService.Data;

public class ClaimsDb : DbContext
{
    public ClaimsDb(DbContextOptions<ClaimsDb> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Claim> Claims => Set<Claim>();
}
