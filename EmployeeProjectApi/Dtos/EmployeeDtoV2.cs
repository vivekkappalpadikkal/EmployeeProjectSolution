namespace EmployeeProjectApi.Dtos;

public record EmployeeDtoV2
(
    int Id,
    string Name,
    string Email,
    string Department,
    DateTime DateOfJoining,
    string Position        // ← new field
);
