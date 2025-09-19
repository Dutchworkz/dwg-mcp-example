# DWG JobFinder MCP Demo

An example .NET Aspire distributed application that demonstrates integrating the Model Context Protocol (MCP) with a Blazor Server UI and an SSE (Server Sent Events) based MCP server exposing domain tools for Employees and Job Offerings. It also shows how to plug in Azure OpenAI for chat based enrichment.

## Projects

| Project | Purpose |
| ------- | ------- |
| `DWG.JobFinder.AppHost` | Aspire app host that composes the distributed application (OpenAI resource, MCP server, web client). |
| `DWG.JobFinder.Client.Web` | Blazor Server application (Razor Components) acting as the front-end; connects to Azure OpenAI + MCP tools. |
| `DWG.JobFinder.JobOffering.SSE` | ASP.NET Core MCP server exposing Employees & JobOffering tools over HTTP/SSE. |
| `DWG.JobFinder.ServiceDefaults` | Shared service defaults (health checks, service discovery, OpenTelemetry, resilience). |

## Architecture Overview

- **AppHost** wires together resources using .NET Aspire fluent builder.
- **Client.Web** resolves the MCP server via service discovery (`https+http://mcpserver`) and creates an `IMcpClient` using SSE transport.
- **JobOffering.SSE** registers tools via attribute model (`[McpServerToolType]`, `[McpServerTool]`) and hosts them with `AddMcpServer().WithHttpTransport().WithToolsFromAssembly()`.
- **Azure OpenAI** (or a connection string in dev) provides the chat model. The model deployment name is injected via `openAiModel` parameter/environment.
- **OpenTelemetry & Health** are centralized in `ServiceDefaults` and applied to each service by calling `AddServiceDefaults()`.

## MCP Tools

### Employees
- `GetEmployees()` – list all employees.
- `GetEmployee(id)` – details for a specific employee id.
- `GetEmployeeByName(name)` – partial match on name.
- `FindByHardSkill(skill)` – employees with a given technical skill.
- `FindBySoftSkill(skill)` – employees with a given soft skill.

### Job Offerings
- `GetJobOfferings()` – list all job offerings.
- `GetJobOffering(id)` – details of a job offering.
- `FindByTechnicalSkill(skill)` – offerings requiring a technical skill.
- `FindBySoftSkill(skill)` – offerings requiring a soft skill.

## Prerequisites

- .NET 9 SDK (preview/RC as of project time) installed.
- (Optional) Azure OpenAI resource & deployment (e.g. `gpt-4o`).
- PowerShell 7+ (commands below use `pwsh`).

## Configuration

Configuration for local development lives in `src/DWG.JobFinder.AppHost/appsettings.Development.json`.

Example (already present with placeholders):
```json
{
	"Logging": {
		"LogLevel": {
			"Default": "Information",
			"Microsoft.AspNetCore": "Warning"
		}
	},
	"ConnectionStrings": {
		"openAi": "Endpoint=https://YOUR-RESOURCE.openai.azure.com/;Key=YOUR_KEY;"
	},
	"openAiModel": "YOUR_MODEL_DEPLOYMENT_NAME"  // e.g. gpt-4o
}
```

Fill in:
- `ConnectionStrings.openAi.Endpoint` with your Azure OpenAI resource endpoint URL.
- `ConnectionStrings.openAi.Key` with the Azure OpenAI API key.
- `openAiModel` with the deployed model name (e.g. `gpt-4o`).


## Running the Solution (Dev)
From the repository root (where the `.sln` lives):
```powershell
# Restore
 dotnet restore

# Run the Aspire orchestrator (AppHost)
 dotnet run --project ./src/DWG.JobFinder.AppHost/DWG.JobFinder.AppHost.csproj
```
The Aspire dashboard (if enabled) will list resources. The Blazor app will be reachable on the bound port (check console output). The MCP server is discovered internally; no manual URL wiring needed.

## Health & Observability
- Health endpoints (dev only): `/health` and `/alive` on each service.
- OpenTelemetry is configured for ASP.NET Core + HttpClient metrics & traces; exporting via OTLP if `OTEL_EXPORTER_OTLP_ENDPOINT` is set.

## Extending
1. Add a new domain service + tools assembly (follow Employees / JobOffering pattern).
2. Register the service in the MCP server DI container.
3. Add `[McpServerToolType]` static class and `[McpServerTool]` methods.
4. They are auto-registered by `WithToolsFromAssembly()`.
5. Consume from the client using the `IMcpClient` (enumerate tools, call invoke API, or integrate with chat orchestration).

## Troubleshooting
- 404 / SSE connect issues: ensure the client resolves `https+http://mcpserver` (Aspire service name must match "mcpserver" in AppHost).
- Empty tool list: confirm attributes & that the static class is public and decorated with `[McpServerToolType]`.
- OpenAI auth errors: verify endpoint URL ends with your resource domain and key is valid; model name matches a deployed model.
- use `npx @modelcontextprotocol/inspector` to check your mcp endpoint use the HTTP URL from the console window of the MCP server (the url from the aspire dashboard does not work)

## Next Ideas
- Add authentication (e.g., Azure AD) for MCP server endpoints.



