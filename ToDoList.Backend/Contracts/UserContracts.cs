namespace ToDoList.Backend.Contracts;

public record CreateUserRequest(string Name);

public record EditUserRequest(string Name);

public record UserResponse(int Id, string Name);