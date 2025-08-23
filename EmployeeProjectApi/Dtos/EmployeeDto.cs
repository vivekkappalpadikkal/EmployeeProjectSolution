namespace EmployeeProjectApi.Dtos;

public record EmployeeDto
(
    int Id,
    string Name,
    string Email,
    string Department,
    DateTime DateOfJoining
);
