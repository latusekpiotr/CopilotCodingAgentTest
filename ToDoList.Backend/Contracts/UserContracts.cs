namespace ToDoList.Backend.Contracts;

public record CreateUserRequest(string Name, string Username, string Password, bool IsAdmin = false);

public record EditUserRequest(string Name);

public record UpdatePasswordRequest(string Password);

public record UserResponse(int Id, string Name, string Username, bool IsAdmin);

// Debug-only response that includes password hash for troubleshooting
public record DebugUserResponse(int Id, string Name, string Username, string PasswordHash, bool IsAdmin);