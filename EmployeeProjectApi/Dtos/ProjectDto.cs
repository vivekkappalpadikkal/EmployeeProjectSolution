namespace EmployeeProjectApi.Dtos;

public record ProjectDto
(
    int Id,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    string Status
);
