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

Development uses a connection string style value under `ConnectionStrings:openai` in `Client.Web/appsettings.json`:
```
Endpoint={{ENDPOINT_URL}};Key={{API_KEY}}
```
At publish time the Aspire host can bind to an Azure OpenAI resource named `openai`.

Set the model deployment name via environment variable or user secrets:
- Key: `openAiModel`
- Example: `gpt-4o`

### User Secrets (alternative)
From the `DWG.JobFinder.Client.Web` project directory:
```powershell
# Add secrets (example)
dotnet user-secrets set ConnectionStrings:openai "Endpoint=https://YOUR-RESOURCE.openai.azure.com/;Key=YOUR_KEY" -p ./DWG.JobFinder.Client.Web.csproj

# Model deployment name
dotnet user-secrets set openAiModel gpt-4o -p ./DWG.JobFinder.Client.Web.csproj
```

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

## Next Ideas
- Add authentication (e.g., Azure AD) for MCP server endpoints.
- Implement caching layer (Redis) for frequently accessed tool results.
- Add streaming chat UI integrating MCP tool calls inline.
- Provide integration tests for tool methods.

---
Generated README – adjust deployment steps or model names as your environment evolves.
