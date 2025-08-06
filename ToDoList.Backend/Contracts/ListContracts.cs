namespace ToDoList.Backend.Contracts;

public record CreateListRequest(string Name, int OwnerID);

public record ListResponse(int Id, string Name, int OwnerID);