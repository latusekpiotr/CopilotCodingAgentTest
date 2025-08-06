using Microsoft.EntityFrameworkCore;
using ToDoList.Backend.Data;
using ToDoList.Backend.Models;
using ToDoList.Backend.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ToDoListDbContext>(options =>
{
    // Use SQLite for development and testing environments, SQL Server for production
    if (builder.Environment.IsDevelopment())
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
    else
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure database is created and seed data in development
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ToDoListDbContext>();
        await context.Database.EnsureCreatedAsync();
        await SeedDevelopmentDataAsync(context);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// User endpoints
app.MapPost("/users", async (CreateUserRequest request, ToDoListDbContext db) =>
{
    var user = new User { Name = request.Name };
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Ok(new UserResponse(user.Id, user.Name));
})
.WithName("CreateUser")
.WithOpenApi();

app.MapPut("/users/{id}", async (int id, EditUserRequest request, ToDoListDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();
    
    user.Name = request.Name;
    await db.SaveChangesAsync();
    return Results.Ok(new UserResponse(user.Id, user.Name));
})
.WithName("EditUser")
.WithOpenApi();

app.MapDelete("/users/{id}", async (int id, ToDoListDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();
    
    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeleteUser")
.WithOpenApi();

// List endpoints
app.MapPost("/lists", async (CreateListRequest request, ToDoListDbContext db) =>
{
    var owner = await db.Users.FindAsync(request.OwnerID);
    if (owner is null) return Results.BadRequest("Owner not found");
    
    var list = new List { Name = request.Name, OwnerID = request.OwnerID };
    db.Lists.Add(list);
    await db.SaveChangesAsync();
    return Results.Ok(new ListResponse(list.Id, list.Name, list.OwnerID));
})
.WithName("CreateList")
.WithOpenApi();

app.MapDelete("/lists/{id}", async (int id, ToDoListDbContext db) =>
{
    var list = await db.Lists.FindAsync(id);
    if (list is null) return Results.NotFound();
    
    db.Lists.Remove(list);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeleteList")
.WithOpenApi();

// Item endpoints
app.MapPost("/lists/{listId}/items", async (int listId, AddItemRequest request, ToDoListDbContext db) =>
{
    var list = await db.Lists.FindAsync(listId);
    if (list is null) return Results.BadRequest("List not found");
    
    var item = new Item { Name = request.Name, ListId = listId };
    db.Items.Add(item);
    await db.SaveChangesAsync();
    return Results.Ok(new ItemResponse(item.Id, item.Name, item.ListId));
})
.WithName("AddItem")
.WithOpenApi();

app.MapPut("/items/{id}", async (int id, EditItemRequest request, ToDoListDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();
    
    item.Name = request.Name;
    await db.SaveChangesAsync();
    return Results.Ok(new ItemResponse(item.Id, item.Name, item.ListId));
})
.WithName("EditItem")
.WithOpenApi();

app.MapDelete("/items/{id}", async (int id, ToDoListDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();
    
    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("RemoveItem")
.WithOpenApi();

app.Run();

// Data seeding method for development environment
static async Task SeedDevelopmentDataAsync(ToDoListDbContext context)
{
    // Check if data already exists
    if (await context.Users.AnyAsync())
    {
        return; // Database has been seeded
    }

    // Create sample users
    var users = new[]
    {
        new User { Name = "Alice Johnson" },
        new User { Name = "Bob Smith" },
        new User { Name = "Carol Davis" }
    };

    context.Users.AddRange(users);
    await context.SaveChangesAsync();

    // Create sample lists
    var lists = new[]
    {
        new List { Name = "Personal Tasks", OwnerID = users[0].Id },
        new List { Name = "Work Projects", OwnerID = users[0].Id },
        new List { Name = "Shopping List", OwnerID = users[1].Id },
        new List { Name = "Home Improvement", OwnerID = users[2].Id }
    };

    context.Lists.AddRange(lists);
    await context.SaveChangesAsync();

    // Create sample items
    var items = new[]
    {
        // Personal Tasks items
        new Item { Name = "Review monthly budget", ListId = lists[0].Id },
        new Item { Name = "Schedule dentist appointment", ListId = lists[0].Id },
        new Item { Name = "Update resume", ListId = lists[0].Id },
        
        // Work Projects items
        new Item { Name = "Prepare quarterly report", ListId = lists[1].Id },
        new Item { Name = "Review team performance", ListId = lists[1].Id },
        new Item { Name = "Plan next sprint", ListId = lists[1].Id },
        
        // Shopping List items
        new Item { Name = "Buy groceries", ListId = lists[2].Id },
        new Item { Name = "Pick up dry cleaning", ListId = lists[2].Id },
        new Item { Name = "Get new phone charger", ListId = lists[2].Id },
        
        // Home Improvement items
        new Item { Name = "Paint living room", ListId = lists[3].Id },
        new Item { Name = "Fix kitchen faucet", ListId = lists[3].Id },
        new Item { Name = "Install new light fixtures", ListId = lists[3].Id }
    };

    context.Items.AddRange(items);
    await context.SaveChangesAsync();
}
