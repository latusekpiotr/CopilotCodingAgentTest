using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ToDoList.Backend.Data;
using ToDoList.Backend.Models;
using ToDoList.Backend.Contracts;
using ToDoList.Backend.Services;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ToDoListDbContext>(options =>
{
    // Use SQLite for development and testing environments, SQL Server for production
    if (builder.Environment.IsDevelopment())
    {
        // Check if running in Azure App Service (which has read-only file system)
        var isAzure = !string.IsNullOrEmpty(builder.Configuration["WEBSITE_SITE_NAME"]);
        
        if (isAzure)
        {
            // Use InMemory database for Azure dev environment to avoid file system issues
            options.UseInMemoryDatabase("TodoListDb");
        }
        else
        {
            // Use file-based SQLite for local development
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
        }
    }
    else
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

// Add Authentication Service
builder.Services.AddScoped<IAuthService, AuthService>();

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                builder.Configuration["Jwt:Key"] ?? "your-secret-key-must-be-at-least-256-bits-long-for-security")),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = new List<string>
        {
            "http://localhost:3000", 
            "https://localhost:3000"
        };

        // Add Azure App Service domain if running in Azure
        var azureWebsiteName = builder.Configuration["WEBSITE_SITE_NAME"];
        if (!string.IsNullOrEmpty(azureWebsiteName))
        {
            allowedOrigins.Add($"https://{azureWebsiteName}.azurewebsites.net");
        }

        policy.WithOrigins(allowedOrigins.ToArray())
              .SetIsOriginAllowedToAllowWildcardSubdomains()
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
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
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Ensuring database is created...");
        await context.Database.EnsureCreatedAsync();
        
        // For Azure environments, always seed data since /tmp is cleared on restart
        // For local development, only seed if database is empty  
        var isAzure = !string.IsNullOrEmpty(app.Configuration["WEBSITE_SITE_NAME"]);
        var userCount = await context.Users.CountAsync();
        
        logger.LogInformation("Running in Azure: {IsAzure}, User count: {UserCount}", isAzure, userCount);
        
        if (isAzure || userCount == 0)
        {
            logger.LogInformation("Seeding development data...");
            await SeedDevelopmentDataAsync(context, logger);
            logger.LogInformation("Development data seeding completed.");
        }
        else
        {
            logger.LogInformation("Database already contains data, skipping seeding.");
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Serve static files (React frontend)
app.UseDefaultFiles();
app.UseStaticFiles();

// Use CORS
app.UseCors("AllowFrontend");

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
.WithName("HealthCheck")
.WithOpenApi();

// Authentication endpoints
app.MapPost("/auth/login", async (LoginRequest request, ToDoListDbContext db, IAuthService authService, ILogger<Program> logger) =>
{
    logger.LogInformation("Login attempt for username: {Username}", request.Username);
    
    // Log database connection info (without sensitive data)
    var userCount = await db.Users.CountAsync();
    logger.LogInformation("Total users in database: {UserCount}", userCount);
    
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
    if (user is null)
    {
        logger.LogWarning("User not found: {Username}", request.Username);
        return Results.Unauthorized();
    }
    
    logger.LogInformation("User found: {Username}, ID: {UserId}", user.Username, user.Id);
    
    if (!authService.VerifyPassword(request.Password, user.PasswordHash))
    {
        logger.LogWarning("Password verification failed for user: {Username}", request.Username);
        return Results.Unauthorized();
    }
    
    logger.LogInformation("Password verified successfully for user: {Username}", request.Username);
    
    var token = authService.GenerateJwtToken(user.Id, user.Username, user.IsAdmin);
    logger.LogInformation("JWT token generated successfully for user: {Username}", request.Username);
    
    return Results.Ok(new LoginResponse(token, user.Id, user.Name, user.IsAdmin));
})
.WithName("Login")
.WithOpenApi();

// Helper method to get current user from claims
static int GetCurrentUserId(ClaimsPrincipal user)
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
}

// Helper method to check if current user is admin
static bool IsCurrentUserAdmin(ClaimsPrincipal user)
{
    var adminClaim = user.FindFirst("IsAdmin");
    return adminClaim != null && bool.Parse(adminClaim.Value);
}

// User endpoints (Admin only)
app.MapPost("/users", async (CreateUserRequest request, ToDoListDbContext db, IAuthService authService, ClaimsPrincipal currentUser) =>
{
    if (!IsCurrentUserAdmin(currentUser))
    {
        return Results.Forbid();
    }
    
    // Check if username already exists
    if (await db.Users.AnyAsync(u => u.Username == request.Username))
    {
        return Results.BadRequest("Username already exists");
    }
    
    var user = new User 
    { 
        Name = request.Name,
        Username = request.Username,
        PasswordHash = authService.HashPassword(request.Password),
        IsAdmin = request.IsAdmin
    };
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Ok(new UserResponse(user.Id, user.Name, user.Username, user.IsAdmin));
})
.WithName("CreateUser")
.WithOpenApi()
.RequireAuthorization();

app.MapPut("/users/{id}", async (int id, EditUserRequest request, ToDoListDbContext db, ClaimsPrincipal currentUser) =>
{
    if (!IsCurrentUserAdmin(currentUser))
    {
        return Results.Forbid();
    }
    
    var user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();
    
    user.Name = request.Name;
    await db.SaveChangesAsync();
    return Results.Ok(new UserResponse(user.Id, user.Name, user.Username, user.IsAdmin));
})
.WithName("EditUser")
.WithOpenApi()
.RequireAuthorization();

app.MapPut("/users/{id}/password", async (int id, UpdatePasswordRequest request, ToDoListDbContext db, IAuthService authService, ClaimsPrincipal currentUser) =>
{
    if (!IsCurrentUserAdmin(currentUser))
    {
        return Results.Forbid();
    }
    
    var user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();
    
    user.PasswordHash = authService.HashPassword(request.Password);
    await db.SaveChangesAsync();
    return Results.Ok(new UserResponse(user.Id, user.Name, user.Username, user.IsAdmin));
})
.WithName("UpdateUserPassword")
.WithOpenApi()
.RequireAuthorization();

app.MapDelete("/users/{id}", async (int id, ToDoListDbContext db, ClaimsPrincipal currentUser) =>
{
    if (!IsCurrentUserAdmin(currentUser))
    {
        return Results.Forbid();
    }
    
    var user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();
    
    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeleteUser")
.WithOpenApi()
.RequireAuthorization();

// Get all users (Admin only)
app.MapGet("/users", async (ToDoListDbContext db, ClaimsPrincipal currentUser) =>
{
    if (!IsCurrentUserAdmin(currentUser))
    {
        return Results.Forbid();
    }
    
    var users = await db.Users
        .Select(u => new UserResponse(u.Id, u.Name, u.Username, u.IsAdmin))
        .ToListAsync();
    return Results.Ok(users);
})
.WithName("GetUsers")
.WithOpenApi()
.RequireAuthorization();

// List endpoints (Authenticated users only)
app.MapGet("/lists", async (ToDoListDbContext db, ClaimsPrincipal currentUser) =>
{
    var currentUserId = GetCurrentUserId(currentUser);
    if (currentUserId == 0) return Results.Unauthorized();
    
    // Users can see only their own lists
    var lists = await db.Lists.Include(l => l.Owner)
        .Where(l => l.OwnerID == currentUserId)
        .Select(l => new { l.Id, l.Name, l.OwnerID, OwnerName = l.Owner.Name })
        .ToListAsync();
    
    return Results.Ok(lists);
})
.WithName("GetLists")
.WithOpenApi()
.RequireAuthorization();

app.MapPost("/lists", async (CreateListRequest request, ToDoListDbContext db, ClaimsPrincipal currentUser) =>
{
    var currentUserId = GetCurrentUserId(currentUser);
    if (currentUserId == 0) return Results.Unauthorized();
    
    // Users can only create lists for themselves
    if (request.OwnerID != currentUserId)
    {
        return Results.Forbid();
    }
    
    var owner = await db.Users.FindAsync(request.OwnerID);
    if (owner is null) return Results.BadRequest("Owner not found");
    
    var list = new List { Name = request.Name, OwnerID = request.OwnerID };
    db.Lists.Add(list);
    await db.SaveChangesAsync();
    return Results.Ok(new ListResponse(list.Id, list.Name, list.OwnerID));
})
.WithName("CreateList")
.WithOpenApi()
.RequireAuthorization();

app.MapPut("/lists/{id}", async (int id, EditListRequest request, ToDoListDbContext db, ClaimsPrincipal currentUser) =>
{
    var currentUserId = GetCurrentUserId(currentUser);
    if (currentUserId == 0) return Results.Unauthorized();
    
    var list = await db.Lists.FindAsync(id);
    if (list is null) return Results.NotFound();
    
    // Users can only edit their own lists
    if (list.OwnerID != currentUserId)
    {
        return Results.Forbid();
    }
    
    list.Name = request.Name;
    await db.SaveChangesAsync();
    return Results.Ok(new ListResponse(list.Id, list.Name, list.OwnerID));
})
.WithName("EditList")
.WithOpenApi()
.RequireAuthorization();

app.MapDelete("/lists/{id}", async (int id, ToDoListDbContext db, ClaimsPrincipal currentUser) =>
{
    var currentUserId = GetCurrentUserId(currentUser);
    if (currentUserId == 0) return Results.Unauthorized();
    
    var list = await db.Lists.FindAsync(id);
    if (list is null) return Results.NotFound();
    
    // Users can only delete their own lists
    if (list.OwnerID != currentUserId)
    {
        return Results.Forbid();
    }
    
    db.Lists.Remove(list);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeleteList")
.WithOpenApi()
.RequireAuthorization();

// Item endpoints (Authenticated users only)
app.MapGet("/lists/{listId}/items", async (int listId, ToDoListDbContext db, ClaimsPrincipal currentUser) =>
{
    var currentUserId = GetCurrentUserId(currentUser);
    if (currentUserId == 0) return Results.Unauthorized();
    
    var list = await db.Lists.FindAsync(listId);
    if (list is null) return Results.BadRequest("List not found");
    
    // Users can only view items in their own lists
    if (list.OwnerID != currentUserId)
    {
        return Results.Forbid();
    }
    
    var items = await db.Items
        .Where(i => i.ListId == listId)
        .Select(i => new ItemResponse(i.Id, i.Name, i.ListId))
        .ToListAsync();
    
    return Results.Ok(items);
})
.WithName("GetItems")
.WithOpenApi()
.RequireAuthorization();

app.MapPost("/lists/{listId}/items", async (int listId, AddItemRequest request, ToDoListDbContext db, ClaimsPrincipal currentUser) =>
{
    var currentUserId = GetCurrentUserId(currentUser);
    if (currentUserId == 0) return Results.Unauthorized();
    
    var list = await db.Lists.FindAsync(listId);
    if (list is null) return Results.BadRequest("List not found");
    
    // Users can only add items to their own lists
    if (list.OwnerID != currentUserId)
    {
        return Results.Forbid();
    }
    
    var item = new Item { Name = request.Name, ListId = listId };
    db.Items.Add(item);
    await db.SaveChangesAsync();
    return Results.Ok(new ItemResponse(item.Id, item.Name, item.ListId));
})
.WithName("AddItem")
.WithOpenApi()
.RequireAuthorization();

app.MapPut("/items/{id}", async (int id, EditItemRequest request, ToDoListDbContext db, ClaimsPrincipal currentUser) =>
{
    var currentUserId = GetCurrentUserId(currentUser);
    if (currentUserId == 0) return Results.Unauthorized();
    
    var item = await db.Items.Include(i => i.List).FirstOrDefaultAsync(i => i.Id == id);
    if (item is null) return Results.NotFound();
    
    // Users can only edit items in their own lists
    if (item.List.OwnerID != currentUserId)
    {
        return Results.Forbid();
    }
    
    item.Name = request.Name;
    await db.SaveChangesAsync();
    return Results.Ok(new ItemResponse(item.Id, item.Name, item.ListId));
})
.WithName("EditItem")
.WithOpenApi()
.RequireAuthorization();

app.MapDelete("/items/{id}", async (int id, ToDoListDbContext db, ClaimsPrincipal currentUser) =>
{
    var currentUserId = GetCurrentUserId(currentUser);
    if (currentUserId == 0) return Results.Unauthorized();
    
    var item = await db.Items.Include(i => i.List).FirstOrDefaultAsync(i => i.Id == id);
    if (item is null) return Results.NotFound();
    
    // Users can only delete items in their own lists
    if (item.List.OwnerID != currentUserId)
    {
        return Results.Forbid();
    }
    
    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("RemoveItem")
.WithOpenApi()
.RequireAuthorization();

// Fallback route for SPA client-side routing
// This ensures that routes like /login, /dashboard are served with index.html
// allowing React Router to handle client-side navigation
app.MapFallbackToFile("index.html");

app.Run();

// Data seeding method for development environment
static async Task SeedDevelopmentDataAsync(ToDoListDbContext context, ILogger<Program> logger)
{
    // Check if data already exists
    var existingUserCount = await context.Users.CountAsync();
    if (existingUserCount > 0)
    {
        logger.LogInformation("Database already contains {UserCount} users, skipping seeding", existingUserCount);
        return; // Database has been seeded
    }
    
    logger.LogInformation("Starting database seeding...");

    // Create auth service for password hashing
    var authService = new ToDoList.Backend.Services.AuthService(new ConfigurationBuilder().Build());

    // Create sample users with authentication data
    var users = new[]
    {
        new User 
        { 
            Name = "Alice Johnson", 
            Username = "alice",
            PasswordHash = authService.HashPassword("password123"),
            IsAdmin = true // Make Alice an admin
        },
        new User 
        { 
            Name = "Bob Smith", 
            Username = "bob",
            PasswordHash = authService.HashPassword("password123"),
            IsAdmin = false
        },
        new User 
        { 
            Name = "Carol Davis", 
            Username = "carol",
            PasswordHash = authService.HashPassword("password123"),
            IsAdmin = false
        }
    };

    context.Users.AddRange(users);
    await context.SaveChangesAsync();
    logger.LogInformation("Created {UserCount} users: {Usernames}", users.Length, string.Join(", ", users.Select(u => u.Username)));

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
    logger.LogInformation("Created {ListCount} lists", lists.Length);

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
    logger.LogInformation("Created {ItemCount} items", items.Length);
    
    // Final verification
    var finalUserCount = await context.Users.CountAsync();
    logger.LogInformation("Database seeding completed successfully. Total users: {UserCount}", finalUserCount);
}
