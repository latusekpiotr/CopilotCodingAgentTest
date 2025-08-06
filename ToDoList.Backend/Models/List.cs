namespace ToDoList.Backend.Models;

public class List
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int OwnerID { get; set; }
    
    // Navigation properties
    public User Owner { get; set; } = null!;
    public ICollection<Item> Items { get; set; } = new List<Item>();
}