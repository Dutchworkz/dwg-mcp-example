using ModelContextProtocol.Server;
using System.ComponentModel;

namespace DWG.JobFinder.JobOffering.SSE.Services.JobOffering;

[McpServerToolType]
public static class JobOfferingTools
{
    [McpServerTool, Description("Get a list of job offerings.")]
    public static async Task<List<JobOffering>> GetJobOfferings(JobOfferingService jobOfferingService)
    {
        var jobs = await jobOfferingService.GetJobOfferings();
        return jobs;
    }

    [McpServerTool, Description("Get a job offering by id.")]
    public static async Task<JobOffering?> GetJobOffering(JobOfferingService jobOfferingService, [Description("The id of the job offering to get details for")] int id)
    {
        var job = await jobOfferingService.GetJobOffering(id);
        return job;
    }

    [McpServerTool, Description("Find job offerings that require a specific technical skill (case-insensitive, substring match).")]
    public static async Task<List<JobOffering>> FindByTechnicalSkill(
       JobOfferingService jobOfferingService,
       [Description("The technical skill to search for (e.g. 'C#', 'Azure', 'React')")] string skill)
    {
        var jobs = await jobOfferingService.FindByTechnicalSkill(skill);
        return jobs;
    }

    [McpServerTool, Description("Find job offerings that require a specific soft skill (case-insensitive, substring match).")]
    public static async Task<List<JobOffering>> FindBySoftSkill(
        JobOfferingService jobOfferingService,
        [Description("The soft skill to search for (e.g. 'Teamwerk', 'Communicatie', 'Leiderschap')")] string skill)
    {
        var jobs = await jobOfferingService.FindBySoftSkill(skill);
        return jobs;
    }
}