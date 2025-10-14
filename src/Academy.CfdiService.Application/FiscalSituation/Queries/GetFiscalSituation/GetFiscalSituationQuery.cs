using Academy.CfdiService.Application.FiscalSituation.Models;
using MediatR;

namespace Academy.CfdiService.Application.FiscalSituation.Queries.GetFiscalSituation;

public sealed record GetFiscalSituationQuery(string Rfc) : IRequest<FiscalSituationDto?>;
