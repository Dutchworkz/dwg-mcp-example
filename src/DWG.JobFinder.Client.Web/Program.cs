using Azure.AI.OpenAI;
using DWG.JobFinder.Client.Web.Components;
using DWG.JobFinder.Client.Web.Extensions;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureOpenAIClient("openai");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddChatClient(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var aiClient = sp.GetRequiredService<AzureOpenAIClient>();
    return aiClient.GetChatClient(config.GetValue<string>("openAiModel")).AsIChatClient();
})
.UseFunctionInvocation()
.UseLogging();

builder.Services.AddSingleton<IMcpClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

    // With .NET Aspire
    var uri = new Uri("https+http://mcpserver").Resolve(config);

    var clientTransportOptions = new SseClientTransportOptions()
    {
        Endpoint = new Uri($"{uri.AbsoluteUri.TrimEnd('/')}/sse")
    };
    var clientTransport = new SseClientTransport(clientTransportOptions, loggerFactory);

    var clientOptions = new McpClientOptions()
    {
        ClientInfo = new Implementation()
        {
            Name = "MCP EmployeeJob Client",
            Version = "1.0.0",
        }
    };

    return McpClientFactory.CreateAsync(clientTransport, clientOptions, loggerFactory)
        .GetAwaiter()
        .GetResult();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
