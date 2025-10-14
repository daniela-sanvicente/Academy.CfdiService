using Academy.CfdiService.Domain.Entities;

namespace Academy.CfdiService.Domain.Repositories;

public interface ICfdiReadRepository
{
    Task<IReadOnlyList<Cfdi>> GetByRfcAsync(string rfc, CancellationToken cancellationToken = default);
}
