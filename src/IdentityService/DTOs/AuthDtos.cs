namespace IdentityService.DTOs;

public record RegisterRequest(string Email, string Password, string FullName);
public record LoginRequest(string Email, string Password);

public record AuthResponse(string AccessToken, DateTime ExpiresAtUtc);

public record UserProfile(string Id, string Email, string FullName, IReadOnlyCollection<string> Roles);
