namespace ToDoList.Backend.Contracts;

public record LoginRequest(string Username, string Password);

public record LoginResponse(string Token, int UserId, string Name, bool IsAdmin);

public record RegisterRequest(string Name, string Username, string Password);

public record RegisterResponse(int Id, string Name, string Username, bool IsAdmin);