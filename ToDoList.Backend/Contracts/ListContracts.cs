namespace ToDoList.Backend.Contracts;

public record CreateListRequest(string Name, int OwnerID);

public record EditListRequest(string Name);

public record ListResponse(int Id, string Name, int OwnerID);