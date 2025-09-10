using ModelContextProtocol.Server;
using System.ComponentModel;

namespace DWG.JobFinder.JobOffering.SSE.Services.Employees;

[McpServerToolType]
public static class EmployeesTools
{
    [McpServerTool, Description("Get a list of employees.")]
    public static async Task<List<Employee>> GetEmployees(EmployeesService employeesService)
    {
        var employees = await employeesService.GetEmployees();
        return employees;
    }

    [McpServerTool, Description("Get an employee by id.")]
    public static async Task<Employee?> GetEmployee(EmployeesService employeesService, [Description("The id of the employee to get details for")] int id)
    {
        var employee = await employeesService.GetEmployee(id);
        return employee;
    }

    [McpServerTool, Description("Get an employee by (partial) name.")]
    public static async Task<Employee?> GetEmployeeByName(
      EmployeesService employeesService,
      [Description("The (partial) name of the employee to get details for")] string name)
    {
        var employee = await employeesService.GetEmployeeByName(name);
        return employee;
    }

    [McpServerTool, Description("Find employees that have a specific hard skill (case-insensitive, substring match).")]
    public static async Task<List<Employee>> FindByHardSkill(
        EmployeesService employeesService,
        [Description("The hard skill to search for (e.g. '.NET', 'Azure', 'AI')")] string skill)
    {
        var employees = await employeesService.FindByHardSkill(skill);
        return employees;
    }

    [McpServerTool, Description("Find employees that have a specific soft skill (case-insensitive, substring match).")]
    public static async Task<List<Employee>> FindBySoftSkill(
        EmployeesService employeesService,
        [Description("The soft skill to search for (e.g. 'Teamwerk', 'Communicatie', 'Leiderschap')")] string skill)
    {
        var employees = await employeesService.FindBySoftSkill(skill);
        return employees;
    }
}
