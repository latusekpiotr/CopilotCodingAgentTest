namespace ToDoList.Backend.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // Display name
    public string Username { get; set; } = string.Empty; // Login username
    public string PasswordHash { get; set; } = string.Empty; // BCrypt hash
    public bool IsAdmin { get; set; } = false; // Admin flag
    
    // Navigation property
    public ICollection<List> Lists { get; set; } = new List<List>();
}