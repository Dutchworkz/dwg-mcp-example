using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DWG.JobFinder.JobOffering.SSE.Services.JobOffering;

public class JobOfferingService
{
    private readonly List<JobOffering> jobOfferings;

    public JobOfferingService()
    {
        // Load the embedded jobofferings.json file at startup
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("jobofferings.json", StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
            throw new InvalidOperationException("jobofferings.json resource not found.");

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        jobOfferings = JsonSerializer.Deserialize(
            stream,
            JobOfferingContext.Default.ListJobOffering
        ) ?? [];
    }

    public Task<List<JobOffering>> GetJobOfferings()
    {
        return Task.FromResult(jobOfferings);
    }

    public Task<JobOffering?> GetJobOffering(int id)
    {
        var job = jobOfferings.FirstOrDefault(j => j.Id == id);
        return Task.FromResult(job);
    }

    /// <summary>
    /// Find job offerings that require a specific technical skill (case-insensitive).
    /// </summary>
    public Task<List<JobOffering>> FindByTechnicalSkill(string skill)
    {
        var matches = jobOfferings
            .Where(j => j.RequiredSkills != null &&
                        j.RequiredSkills.Any(s => s.Contains(skill, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        return Task.FromResult(matches);
    }

    /// <summary>
    /// Find job offerings that require a specific soft skill (case-insensitive).
    /// </summary>
    public Task<List<JobOffering>> FindBySoftSkill(string skill)
    {
        var matches = jobOfferings
            .Where(j => j.SoftSkills != null &&
                        j.SoftSkills.Any(s => s.Contains(skill, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        return Task.FromResult(matches);
    }
}

public partial class JobOffering
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("requiredSkills")]
    public List<string>? RequiredSkills { get; set; }

    [JsonPropertyName("softSkills")]
    public List<string>? SoftSkills { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("employmentType")]
    public string? EmploymentType { get; set; }
}

[JsonSerializable(typeof(List<JobOffering>))]
internal sealed partial class JobOfferingContext : JsonSerializerContext
{
}
