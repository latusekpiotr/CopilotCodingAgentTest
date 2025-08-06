namespace ToDoList.Backend.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ListId { get; set; }
    
    // Navigation property
    public List List { get; set; } = null!;
}