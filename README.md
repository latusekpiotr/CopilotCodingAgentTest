# ToDoList Backend API

A .NET 8 ASP.NET Core Web API for managing todo lists, users, and items. This API provides RESTful endpoints for creating and managing hierarchical todo lists where users can own multiple lists, and each list can contain multiple items.

## Features

- **User Management**: Create, update, and delete users
- **List Management**: Create and delete todo lists owned by users  
- **Item Management**: Add, edit, and remove items from todo lists
- **SQL Server Database**: Uses Entity Framework Core with SQL Server LocalDB
- **Database Migrations**: Entity Framework migrations for database schema management
- **Data Seeding**: Automatic population of sample data in development environment
- **API Documentation**: Integrated Swagger/OpenAPI documentation
- **RESTful Design**: Clean REST API endpoints following standard conventions

## Tech Stack

- **.NET 8**: Latest .NET framework
- **ASP.NET Core Web API**: Web API framework
- **Entity Framework Core**: ORM with SQL Server database provider
- **SQL Server LocalDB**: Local database for development
- **Swagger/OpenAPI**: API documentation and testing interface

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (included with Visual Studio or SQL Server Express)

## Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/latusekpiotr/CopilotCodingAgentTest.git
cd CopilotCodingAgentTest
```

### 2. Setup Database
```bash
cd ToDoList.Backend
dotnet ef database update
```

### 3. Build the Project
```bash
dotnet build
```

### 4. Run the Application
```bash
cd ToDoList.Backend
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5011`
- HTTPS: `https://localhost:7011`

**Note**: When running in Development environment, the application will automatically seed the database with sample data for testing.

### 5. Access API Documentation
Once running, visit the Swagger UI at:
- `https://localhost:7011/swagger` (or `http://localhost:5011/swagger`)

## Project Structure

```
ToDoList.Backend/
├── Contracts/           # Request/Response DTOs
│   ├── ItemContracts.cs
│   ├── ListContracts.cs
│   └── UserContracts.cs
├── Data/               # Database context
├── Models/             # Entity models
│   ├── Item.cs
│   ├── List.cs
│   └── User.cs
├── Program.cs          # Application entry point and API endpoints
├── appsettings.json    # Configuration
└── ToDoList.Backend.csproj
```

## API Endpoints

### Users

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/users` | Create a new user |
| PUT | `/users/{id}` | Update an existing user |
| DELETE | `/users/{id}` | Delete a user |

#### Create User
```http
POST /users
Content-Type: application/json

{
  "name": "John Doe"
}
```

#### Update User
```http
PUT /users/1
Content-Type: application/json

{
  "name": "Jane Doe"
}
```

### Lists

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/lists` | Create a new todo list |
| DELETE | `/lists/{id}` | Delete a todo list |

#### Create List
```http
POST /lists
Content-Type: application/json

{
  "name": "Shopping List",
  "ownerID": 1
}
```

### Items

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/lists/{listId}/items` | Add an item to a list |
| PUT | `/items/{id}` | Update an item |
| DELETE | `/items/{id}` | Remove an item |

#### Add Item
```http
POST /lists/1/items
Content-Type: application/json

{
  "name": "Buy milk"
}
```

#### Update Item
```http
PUT /items/1
Content-Type: application/json

{
  "name": "Buy organic milk"
}
```

## Data Models

### User
- `Id` (int): Unique identifier
- `Name` (string): User's name
- `Lists` (ICollection<List>): User's todo lists

### List  
- `Id` (int): Unique identifier
- `Name` (string): List name
- `OwnerID` (int): ID of the user who owns this list
- `Owner` (User): Navigation property to owner
- `Items` (ICollection<Item>): Items in this list

### Item
- `Id` (int): Unique identifier  
- `Name` (string): Item description
- `ListId` (int): ID of the list this item belongs to
- `List` (List): Navigation property to parent list

## Development

### Building
```bash
dotnet build
```

### Running in Development Mode
```bash
cd ToDoList.Backend
dotnet run --environment Development
```

### Database
The application uses SQL Server LocalDB for data persistence. In Development environment, the database is automatically seeded with sample data including:

- **Sample Users**: Alice Johnson, Bob Smith, Carol Davis
- **Sample Lists**: Personal Tasks, Work Projects, Shopping List, Home Improvement
- **Sample Items**: Various todo items distributed across the lists

#### Database Commands
```bash
# Create a new migration
dotnet ef migrations add <MigrationName>

# Update database to latest migration
dotnet ef database update

# Remove last migration (if not applied)
dotnet ef migrations remove
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test your changes
5. Submit a pull request

## License

This project is a coding agent test repository.