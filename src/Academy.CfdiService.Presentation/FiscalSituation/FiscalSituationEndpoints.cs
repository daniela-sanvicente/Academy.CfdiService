using Academy.CfdiService.Application.FiscalSituation.Models;
using Academy.CfdiService.Application.FiscalSituation.Queries.GetFiscalSituation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Academy.CfdiService.Presentation.FiscalSituation;

public static class FiscalSituationEndpoints
{
    public static RouteGroupBuilder MapFiscalSituationEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/fiscal-situation");

        group.MapGet(
                "/{rfc}",
                async Task<Results<Ok<FiscalSituationDto>, NotFound>> (
                    string rfc,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetFiscalSituationQuery(rfc);
                    var result = await sender.Send(query, cancellationToken);

                    if (result is null)
                    {
                        return TypedResults.NotFound();
                    }

                    return TypedResults.Ok(result);
                })
            .WithName("GetFiscalSituation")
            .WithOpenApi();

        return group;
    }
}
