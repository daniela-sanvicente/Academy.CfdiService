using Academy.CfdiService.Application.FiscalSituation.Models;
using Academy.CfdiService.Domain.Repositories;
using MediatR;

namespace Academy.CfdiService.Application.FiscalSituation.Queries.GetFiscalSituation;

public sealed class GetFiscalSituationQueryHandler : IRequestHandler<GetFiscalSituationQuery, FiscalSituationDto?>
{
    private readonly ICfdiReadRepository _repository;

    public GetFiscalSituationQueryHandler(ICfdiReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<FiscalSituationDto?> Handle(GetFiscalSituationQuery request, CancellationToken cancellationToken)
    {
        var documents = await _repository.GetByRfcAsync(request.Rfc, cancellationToken);

        if (documents.Count == 0)
        {
            return null;
        }

        var issuedCount = documents.Count(cfdi =>
            string.Equals(cfdi.IssuerRfc, request.Rfc, StringComparison.OrdinalIgnoreCase));

        var receivedCount = documents.Count(cfdi =>
            string.Equals(cfdi.ReceiverRfc, request.Rfc, StringComparison.OrdinalIgnoreCase));

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

        return new FiscalSituationDto(
            request.Rfc,
            issuedCount,
            receivedCount,
            documents.Count,
            latestIssueDate,
            lastUpdated,
            summaries);
    }
}
