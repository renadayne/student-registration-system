namespace StudentRegistration.Domain.Entities;

/// <summary>
/// Entity User cho Domain
/// </summary>
public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // "Admin" hoặc "Student"
} 