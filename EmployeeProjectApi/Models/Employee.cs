namespace EmployeeProjectApi.Models;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Department { get; set; } = null!;
    public DateTime DateOfJoining { get; set; }
    public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();
    public string Position { get; set; } = null!;    // add this

}
