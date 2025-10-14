namespace Academy.CfdiService.Application.FiscalSituation.Models;

public sealed record FiscalSituationDto(
    string Rfc,
    int IssuedCount,
    int ReceivedCount,
    int TotalDocuments,
    DateTime? LatestIssueDate,
    DateTime? LastUpdated,
    IReadOnlyCollection<CfdiSummaryDto> Documents);
