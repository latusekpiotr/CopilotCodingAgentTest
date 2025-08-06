using Microsoft.EntityFrameworkCore;
using ToDoList.Backend.Data;
using ToDoList.Backend.Models;
using ToDoList.Backend.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ToDoListDbContext>(options =>
    options.UseInMemoryDatabase("ToDoListDb"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
