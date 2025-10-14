using Academy.CfdiService.Application;
using Academy.CfdiService.Infrastructure;
using Academy.CfdiService.Presentation;
using Academy.CfdiService.Presentation.FiscalSituation;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapFiscalSituationEndpoints();

app.Run();
