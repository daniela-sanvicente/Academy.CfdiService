using Academy.CfdiService.Application.Common.Helpers;
using Academy.CfdiService.Application.FiscalSituation.Models;
using Academy.CfdiService.Application.FiscalSituation.Queries.GetFiscalSituation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Academy.CfdiService.Presentation.FiscalSituation;

public static class FiscalSituationModule
{
    public static RouteGroupBuilder MapFiscalSituationModule(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/fiscal-situation");

        group.MapGet(
                "/{rfc}",
                async Task<Results<Ok<FiscalSituationDto>, NotFound, BadRequest<ProblemDetails>>> (
                    string rfc,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    if (!RfcValidator.IsValid(rfc))
                    {
                        var problem = new ProblemDetails
                        {
                            Title = "Invalid RFC format",
                            Detail = "The provided value does not match the individual (13 characters) or corporate (12 characters) RFC formats.",
                            Status = StatusCodes.Status400BadRequest
                        };

                        return TypedResults.BadRequest(problem);
                    }

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
            .WithSummary("Retrieves the fiscal situation (issued and received CFDI) for an RFC.")
            .WithDescription("Returns counts and document details related to the provided RFC, including issued and received invoices, key dates, and current status.")
            .Produces<FiscalSituationDto>(StatusCodes.Status200OK, "application/json")
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, "application/json")
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(operation =>
            {
                operation.OperationId = "GetFiscalSituation";
                operation.Summary = "Gets the fiscal situation for an RFC.";
                operation.Description =
                    "Retrieves consolidated information for every CFDI where the provided RFC appears as issuer or receiver. Returns 404 when no matching documents are found.";

                operation.Parameters[0].Description = "RFC whose issued and received CFDI should be retrieved.";
                operation.Parameters[0].Schema = new Microsoft.OpenApi.Models.OpenApiSchema
                {
                    Type = "string",
                    Example = new Microsoft.OpenApi.Any.OpenApiString("ABC010203XYZ")
                };

                if (operation.Responses.TryGetValue(StatusCodes.Status200OK.ToString(), out var okResponse))
                {
                    okResponse.Description = "Successful lookup that returns the RFC fiscal situation.";

                    if (okResponse.Content.TryGetValue("application/json", out var okMediaType))
                    {
                        okMediaType.Example = BuildSuccessExample();
                    }
                }

                if (operation.Responses.TryGetValue(StatusCodes.Status400BadRequest.ToString(), out var badRequestResponse))
                {
                    badRequestResponse.Description = "The RFC value does not satisfy the individual or corporate RFC formats.";

                    if (badRequestResponse.Content.TryGetValue("application/json", out var badRequestMediaType))
                    {
                        badRequestMediaType.Example = new OpenApiObject
                        {
                            ["title"] = new OpenApiString("Invalid RFC format"),
                            ["status"] = new OpenApiInteger(StatusCodes.Status400BadRequest),
                            ["detail"] = new OpenApiString("The provided value does not match the individual (13 characters) or corporate (12 characters) RFC formats.")
                        };
                    }
                }

                if (operation.Responses.TryGetValue(StatusCodes.Status404NotFound.ToString(), out var notFoundResponse))
                {
                    notFoundResponse.Description = "No CFDI records were found for the provided RFC.";
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
