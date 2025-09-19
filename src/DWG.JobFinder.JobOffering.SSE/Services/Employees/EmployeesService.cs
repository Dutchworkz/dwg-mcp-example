using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DWG.JobFinder.JobOffering.SSE.Services.Employees;

public class EmployeesService
{
    private readonly List<Employee> employees;

    public EmployeesService()
    {
        // Load the embedded employees.json file at startup
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("employees.json", StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
            throw new InvalidOperationException("employees.json resource not found.");

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        employees = JsonSerializer.Deserialize<List<Employee>>(stream)!;
    }

    public Task<List<Employee>> GetEmployees()
    {
        return Task.FromResult(employees);
    }

    public Task<Employee?> GetEmployee(int id)
    {
        var employee = employees.FirstOrDefault(e => e.Id == id);
        return Task.FromResult(employee);
    }

    public Task<Employee?> GetEmployeeByName(string name)
    {
        var employee = employees.FirstOrDefault(e => e.Name.Contains(name));
        return Task.FromResult(employee);
    }

    /// <summary>
    /// Find employees that have a specific hard skill (case-insensitive).
    /// </summary>
    public Task<List<Employee>> FindByHardSkill(string skill)
    {
        var matches = employees
            .Where(e => e.HardSkills != null &&
                        e.HardSkills.Any(s => s.Contains(skill, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        return Task.FromResult(matches);
    }

    /// <summary>
    /// Find employees that have a specific soft skill (case-insensitive).
    /// </summary>
    public Task<List<Employee>> FindBySoftSkill(string skill)
    {
        var matches = employees
            .Where(e => e.SoftSkills != null &&
                        e.SoftSkills.Any(s => s.Contains(skill, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        return Task.FromResult(matches);
    }
}

public record Employee(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("hardSkills")] List<string>? HardSkills,
    [property: JsonPropertyName("softSkills")] List<string>? SoftSkills,
    [property: JsonPropertyName("latestSkillsetSummary")] string? LatestSkillsetSummary
);
