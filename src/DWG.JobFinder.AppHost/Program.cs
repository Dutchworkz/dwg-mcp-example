var builder = DistributedApplication.CreateBuilder(args);

var openai = builder.ExecutionContext.IsPublishMode ?
      builder.AddAzureOpenAI("openai")
    : builder.AddConnectionString("openai");

var openAiModelParam = builder.AddParameterFromConfiguration("openAiModel", "openAiModel");

var mcp = builder.AddProject<Projects.DWG_JobFinder_JobOffering_SSE>("mcpserver");

builder.AddProject<Projects.DWG_JobFinder_Client_Web>("client-web")
    .WithEnvironment("openAiModel", openAiModelParam)
    .WithReference(mcp)
    .WithReference(openai)
    .WithExternalHttpEndpoints();

builder.Build().Run();
