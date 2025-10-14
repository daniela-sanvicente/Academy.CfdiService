using Academy.CfdiService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Academy.CfdiService.Infrastructure.Persistence;

public class CfdiDbContext : DbContext
{
    public CfdiDbContext(DbContextOptions<CfdiDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cfdi> Cfdis => Set<Cfdi>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CfdiDbContext).Assembly);
    }
}
