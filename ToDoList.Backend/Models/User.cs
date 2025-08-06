namespace ToDoList.Backend.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // Navigation property
    public ICollection<List> Lists { get; set; } = new List<List>();
}