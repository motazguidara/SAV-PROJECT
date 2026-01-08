using InterventionsService.Models;
using Microsoft.EntityFrameworkCore;

namespace InterventionsService.Data;

public class InterventionsDb : DbContext
{
    public InterventionsDb(DbContextOptions<InterventionsDb> options) : base(options)
    {
    }

    public DbSet<Intervention> Interventions => Set<Intervention>();
    public DbSet<PartLine> PartLines => Set<PartLine>();
}
