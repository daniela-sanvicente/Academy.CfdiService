using Academy.CfdiService.Application.Abstractions;
using Academy.CfdiService.Application.FiscalSituation.Models;
using Academy.CfdiService.Domain.Repositories;
using MediatR;

namespace Academy.CfdiService.Application.FiscalSituation.Queries.GetFiscalSituation;

public sealed class GetFiscalSituationQueryHandler : IRequestHandler<GetFiscalSituationQuery, FiscalSituationDto?>
{
    private readonly ICfdiReadRepository _repository;
    private readonly ICacheService _cacheService;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public GetFiscalSituationQueryHandler(ICfdiReadRepository repository, ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task<FiscalSituationDto?> Handle(GetFiscalSituationQuery request, CancellationToken cancellationToken)
    {
        var normalizedRfc = request.Rfc.Trim().ToUpperInvariant();
        var cacheKey = $"fiscal-situation:{normalizedRfc}";

        var cached = await _cacheService.GetAsync<FiscalSituationDto>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var documents = await _repository.GetByRfcAsync(normalizedRfc, cancellationToken);

        if (documents.Count == 0)
        {
            return null;
        }

        var issuedCount = documents.Count(cfdi =>
            string.Equals(cfdi.IssuerRfc, normalizedRfc, StringComparison.OrdinalIgnoreCase));

        var receivedCount = documents.Count(cfdi =>
            string.Equals(cfdi.ReceiverRfc, normalizedRfc, StringComparison.OrdinalIgnoreCase));

        var latestIssueDate = documents.Max(cfdi => (DateTime?)cfdi.IssueDate);
        var lastUpdated = documents.Max(cfdi => (DateTime?)cfdi.LastUpdated);

        var summaries = documents
            .Select(cfdi => new CfdiSummaryDto(
                cfdi.Uuid,
                cfdi.IssueDate,
                cfdi.IssuerRfc,
                cfdi.ReceiverRfc,
                cfdi.Status,
                cfdi.LastUpdated))
            .ToList()
            .AsReadOnly();

        var fiscalSituation = new FiscalSituationDto(
            normalizedRfc,
            issuedCount,
            receivedCount,
            documents.Count,
            latestIssueDate,
            lastUpdated,
            summaries);

        await _cacheService.SetAsync(cacheKey, fiscalSituation, CacheDuration, cancellationToken);

        return fiscalSituation;
    }
}
