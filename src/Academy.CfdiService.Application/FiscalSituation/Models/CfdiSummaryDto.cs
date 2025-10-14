namespace Academy.CfdiService.Application.FiscalSituation.Models;

public sealed record CfdiSummaryDto(
    Guid Uuid,
    DateTime IssueDate,
    string IssuerRfc,
    string ReceiverRfc,
    string Status,
    DateTime LastUpdated);
