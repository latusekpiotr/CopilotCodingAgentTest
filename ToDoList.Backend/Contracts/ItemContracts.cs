namespace ToDoList.Backend.Contracts;

public record AddItemRequest(string Name);

public record EditItemRequest(string Name);

public record ItemResponse(int Id, string Name, int ListId);