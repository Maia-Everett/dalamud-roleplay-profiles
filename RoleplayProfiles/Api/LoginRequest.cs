namespace RoleplayProfiles.Api;

public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string? Otp { get; set; } = null;
}
