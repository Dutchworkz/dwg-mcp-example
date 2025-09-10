using DWG.JobFinder.JobOffering.SSE.Services.Employees;
using DWG.JobFinder.JobOffering.SSE.Services.JobOffering;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services.
builder.Services
    .AddHttpClient()
    .AddSingleton<EmployeesService>()
    .AddSingleton<JobOfferingService>();

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

app.MapMcp();

app.Run();