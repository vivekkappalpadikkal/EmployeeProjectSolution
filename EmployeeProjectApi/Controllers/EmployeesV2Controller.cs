using EmployeeProjectApi.Data;
using EmployeeProjectApi.Dtos;
using EmployeeProjectApi.Models;
using EmployeeProjectApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectApi.Controllers;
[ApiVersion("2.0")]
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesV2Controller : ControllerBase
{
    private readonly AppDbContext _db;
    public EmployeesV2Controller(AppDbContext db) => _db = db;

    // GET /api/employees?page=1&pageSize=10
    //[HttpGet]
    //public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll(
    //    int page = 1, int pageSize = 10)
    //{
    //    var query = _db.Employees
    //                   .OrderBy(e => e.Id)
    //                   .Skip((page - 1) * pageSize)
    //                   .Take(pageSize);

    //    var data = await query
    //        .Select(e => new EmployeeDto(e.Id, e.Name, e.Email,
    //                                     e.Department, e.DateOfJoining))
    //        .ToListAsync();

    //    return Ok(data);
    //}

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll([FromQuery] QueryParameters qp)
    {
        var list = await _db.Employees
                            .Apply(qp)
                            .Select(e => new EmployeeDto(e.Id, e.Name, e.Email,
                                                         e.Department, e.DateOfJoining))
                            .ToListAsync();

        return Ok(list);
    }

    // GET /api/employees/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDto>> Get(int id)
    {
        var e = await _db.Employees.FindAsync(id);
        if (e is null) return NotFound();
        return new EmployeeDto(e.Id, e.Name, e.Email,
                               e.Department, e.DateOfJoining);
    }

    // POST /api/employees
    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create(EmployeeDto dto)
    {
        var entity = new Employee
        {
            Name = dto.Name,
            Email = dto.Email,
            Department = dto.Department,
            DateOfJoining = dto.DateOfJoining
        };
        _db.Employees.Add(entity);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = entity.Id },
            new EmployeeDto(entity.Id, entity.Name, entity.Email,
                            entity.Department, entity.DateOfJoining));
    }

    // PUT /api/employees/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmployeeDto dto)
    {
        if (id != dto.Id) return BadRequest();
        var e = await _db.Employees.FindAsync(id);
        if (e is null) return NotFound();

        e.Name = dto.Name;
        e.Email = dto.Email;
        e.Department = dto.Department;
        e.DateOfJoining = dto.DateOfJoining;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/employees/5
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await _db.Employees.FindAsync(id);
        if (e is null) return NotFound();
        _db.Employees.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // GET /api/employees/5/projects
    [HttpGet("{id:int}/projects")]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects(int id)
    {
        var projects = await _db.EmployeeProjects
            .Where(ep => ep.EmployeeId == id)
            .Select(ep => new ProjectDto(
                ep.Project.Id,
                ep.Project.Name,
                ep.Project.StartDate,
                ep.Project.EndDate,
                ep.Project.Status))
            .ToListAsync();

        return Ok(projects);
    }
}
