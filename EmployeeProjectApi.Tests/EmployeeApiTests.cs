namespace EmployeeProjectApi.Tests;

public class EmployeeApiTests
    : IClassFixture<WebApplicationFactory<EmployeeProjectApi.Program>>
{
    private readonly HttpClient _http;

    public EmployeeApiTests(
        WebApplicationFactory<EmployeeProjectApi.Program> factory)
    {
        // spins up the full API pipeline in-memory
        _http = factory.CreateClient();
    }

    /* ─────────  AUTH  ───────── */

    [Fact]
    public async Task Login_returns_JWT()
    {
        var r = await _http.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginDto("admin@example.com", "admin123"));

        r.EnsureSuccessStatusCode();
        var jwt = (await r.Content.ReadFromJsonAsync<JwtDto>())!.Token;

        Assert.False(string.IsNullOrWhiteSpace(jwt));
    }

    /* ─────────  AUTHZ  ───────── */

    [Fact]
    public async Task Protected_endpoint_without_token_returns_401()
    {
        var r = await _http.GetAsync("/api/v1/employees");
        Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
    }

    /* ─────────  CRUD  (v2) ───────── */

    [Fact]
    public async Task Create_employee_with_token_returns_201_and_Position()
    {
        var jwt = await GetAdminTokenAsync();
        _http.DefaultRequestHeaders.Authorization = new("Bearer", jwt);

        var dto = new EmployeeDtoV2(
            0, "Test User", "test@company.com", "HR",
            DateTime.UtcNow.AddYears(-1), "Manager");

        var r = await _http.PostAsJsonAsync("/api/v2/employees", dto);
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);

        var body = await r.Content.ReadFromJsonAsync<EmployeeDtoV2>();
        Assert.Equal("Manager", body!.Position);
    }

    /* ─────────  VALIDATION ───────── */

    [Fact]
    public async Task Create_employee_bad_email_returns_400()
    {
        var jwt = await GetAdminTokenAsync();
        _http.DefaultRequestHeaders.Authorization = new("Bearer", jwt);

        var invalid = new
        {
            name = "Bad",
            email = "not-an-email",
            department = "IT",
            dateOfJoining = DateTime.UtcNow
        };

        var r = await _http.PostAsJsonAsync("/api/v1/employees", invalid);
        Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode);
    }

    /* ─────────  helpers ───────── */

    private async Task<string> GetAdminTokenAsync()
    {
        var r = await _http.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginDto("admin@example.com", "admin123"));

        r.EnsureSuccessStatusCode();
        return (await r.Content.ReadFromJsonAsync<JwtDto>())!.Token;
    }

    private record JwtDto(string Token);
}
