using EmployeeProjectApi.Data;
using EmployeeProjectApi.Dtos;
using EmployeeProjectApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProjectsController(AppDbContext db) => _db = db;

    // GET /api/projects
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll() =>
        Ok(await _db.Projects
            .Select(p => new ProjectDto(p.Id, p.Name,
                                        p.StartDate, p.EndDate, p.Status))
            .ToListAsync());

    // GET /api/projects/3
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDto>> Get(int id)
    {
        var p = await _db.Projects.FindAsync(id);
        return p is null
            ? NotFound()
            : new ProjectDto(p.Id, p.Name, p.StartDate, p.EndDate, p.Status);
    }

    // POST /api/projects
    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(ProjectDto dto)
    {
        var p = new Project
        {
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = dto.Status
        };
        _db.Projects.Add(p);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = p.Id },
            new ProjectDto(p.Id, p.Name, p.StartDate, p.EndDate, p.Status));
    }

    // PUT /api/projects/3
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProjectDto dto)
    {
        if (id != dto.Id) return BadRequest();
        var p = await _db.Projects.FindAsync(id);
        if (p is null) return NotFound();

        p.Name = dto.Name;
        p.StartDate = dto.StartDate;
        p.EndDate = dto.EndDate;
        p.Status = dto.Status;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/projects/3
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _db.Projects.FindAsync(id);
        if (p is null) return NotFound();
        _db.Projects.Remove(p);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // POST /api/projects/3/assign/7
    [HttpPost("{projectId:int}/assign/{employeeId:int}")]
    public async Task<IActionResult> Assign(int projectId, int employeeId)
    {
        var exists = await _db.EmployeeProjects
            .AnyAsync(ep => ep.ProjectId == projectId && ep.EmployeeId == employeeId);
        if (exists) return Conflict("Employee already assigned");

        _db.EmployeeProjects.Add(new EmployeeProject
        {
            ProjectId = projectId,
            EmployeeId = employeeId
        });
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // GET /api/projects/3/employees
    [HttpGet("{id:int}/employees")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees(int id)
    {
        var employees = await _db.EmployeeProjects
            .Where(ep => ep.ProjectId == id)
            .Select(ep => new EmployeeDto(
                ep.Employee.Id,
                ep.Employee.Name,
                ep.Employee.Email,
                ep.Employee.Department,
                ep.Employee.DateOfJoining))
            .ToListAsync();

        return Ok(employees);
    }
}
