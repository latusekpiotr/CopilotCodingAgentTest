namespace ToDoList.Backend.Contracts;

public record CreateUserRequest(string Name, string Username, string Password, bool IsAdmin = false);

public record EditUserRequest(string Name);

public record UpdatePasswordRequest(string Password);

public record UserResponse(int Id, string Name, string Username, bool IsAdmin);