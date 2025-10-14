using Academy.CfdiService.Application;
using Academy.CfdiService.Infrastructure;
using Academy.CfdiService.Presentation;
using Academy.CfdiService.Presentation.FiscalSituation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Timeouts;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation();

builder.Services.AddRequestTimeouts(options =>
{
    options.DefaultPolicy = new RequestTimeoutPolicy
    {
        Timeout = TimeSpan.FromSeconds(10),
        TimeoutStatusCode = StatusCodes.Status408RequestTimeout
    };
});

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRequestTimeouts();

app.MapFiscalSituationEndpoints();

app.Run();
