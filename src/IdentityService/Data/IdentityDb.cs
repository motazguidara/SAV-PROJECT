using IdentityService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Data;

public class IdentityDb : IdentityDbContext<ApplicationUser>
{
    public IdentityDb(DbContextOptions<IdentityDb> options) : base(options)
    {
    }
}
