using Academy.CfdiService.Domain.Entities;
using Academy.CfdiService.Domain.Repositories;
using Academy.CfdiService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Academy.CfdiService.Infrastructure.Repositories;

public class EfCfdiReadRepository : ICfdiReadRepository
{
    private readonly CfdiDbContext _context;

    public EfCfdiReadRepository(CfdiDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Cfdi>> GetByRfcAsync(string rfc, CancellationToken cancellationToken = default)
    {
        var normalizedRfc = rfc.Trim().ToUpperInvariant();

        return await _context.Cfdis
            .AsNoTracking()
            .Where(cfdi =>
                cfdi.IssuerRfc.ToUpper() == normalizedRfc ||
                cfdi.ReceiverRfc.ToUpper() == normalizedRfc)
            .OrderByDescending(cfdi => cfdi.IssueDate)
            .ThenByDescending(cfdi => cfdi.LastUpdated)
            .ToListAsync(cancellationToken);
    }
}
