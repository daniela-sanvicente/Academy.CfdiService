using Academy.CfdiService.Application.FiscalSituation.Models;
using Academy.CfdiService.Application.FiscalSituation.Queries.GetFiscalSituation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Academy.CfdiService.Presentation.FiscalSituation;

public static class FiscalSituationModule
{
    public static RouteGroupBuilder MapFiscalSituationModule(this IEndpointRouteBuilder routes)
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
            .WithTags("Fiscal Situation")
            .WithSummary("Obtiene la situación fiscal (CFDI emitidos/recibidos) para un RFC.")
            .WithDescription("Devuelve conteos y detalles de CFDI relacionados con el RFC proporcionado. "
                             + "Incluye comprobantes emitidos y recibidos, fechas relevantes y estado.")
            .Produces<FiscalSituationDto>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(operation =>
            {
                operation.OperationId = "GetFiscalSituation";
                operation.Summary = "Consulta la situación fiscal de un RFC.";
                operation.Description =
                    "Permite recuperar información consolidada de los CFDI en los que participa el RFC dado. "
                    + "La búsqueda incluye CFDI emitidos y recibidos. Si no se encuentran registros, regresa 404.";

                operation.Parameters[0].Description = "RFC del emisor o receptor cuyos CFDI se desean consultar.";
                operation.Parameters[0].Schema = new Microsoft.OpenApi.Models.OpenApiSchema
                {
                    Type = "string",
                    Example = new Microsoft.OpenApi.Any.OpenApiString("ABC010203XYZ")
                };

                if (operation.Responses.TryGetValue(StatusCodes.Status200OK.ToString(), out var okResponse))
                {
                    okResponse.Description = "Consulta exitosa; devuelve la situación fiscal del RFC.";

                    if (okResponse.Content.TryGetValue("application/json", out var okMediaType))
                    {
                        okMediaType.Example = BuildSuccessExample();
                    }
                }

                if (operation.Responses.TryGetValue(StatusCodes.Status404NotFound.ToString(), out var notFoundResponse))
                {
                    notFoundResponse.Description = "No se encontraron CFDI asociados al RFC proporcionado.";
                }

                return operation;
            });

        return group;
    }

    private static OpenApiObject BuildSuccessExample()
    {
        return new OpenApiObject
        {
            ["rfc"] = new OpenApiString("ABC010203XYZ"),
            ["issuedCount"] = new OpenApiInteger(12),
            ["receivedCount"] = new OpenApiInteger(8),
            ["totalDocuments"] = new OpenApiInteger(20),
            ["latestIssueDate"] = new OpenApiString("2024-07-15T10:30:00Z"),
            ["lastUpdated"] = new OpenApiString("2024-07-15T12:45:00Z"),
            ["documents"] = new OpenApiArray
            {
                new OpenApiObject
                {
                    ["uuid"] = new OpenApiString("4c42a9e4-3fa1-4f2c-97a2-ced62fd5e4ad"),
                    ["issueDate"] = new OpenApiString("2024-07-14T09:10:00Z"),
                    ["issuerRfc"] = new OpenApiString("ABC010203XYZ"),
                    ["receiverRfc"] = new OpenApiString("DEF040506HIJ"),
                    ["status"] = new OpenApiString("Vigente"),
                    ["lastUpdated"] = new OpenApiString("2024-07-14T10:00:00Z")
                },
                new OpenApiObject
                {
                    ["uuid"] = new OpenApiString("9f85b12a-9c42-4f5a-b901-7bf8bdfdc324"),
                    ["issueDate"] = new OpenApiString("2024-07-10T13:25:00Z"),
                    ["issuerRfc"] = new OpenApiString("GHI070809KLM"),
                    ["receiverRfc"] = new OpenApiString("ABC010203XYZ"),
                    ["status"] = new OpenApiString("Cancelado"),
                    ["lastUpdated"] = new OpenApiString("2024-07-10T15:12:00Z")
                }
            }
        };
    }
}
