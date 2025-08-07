# ToDoList Backend API

A .NET 8 ASP.NET Core Web API for managing todo lists, users, and items. This API provides RESTful endpoints for creating and managing hierarchical todo lists where users can own multiple lists, and each list can contain multiple items.

## Features

- **User Authentication**: JWT-based authentication with secure password hashing
- **User Management**: Create, update, and delete users (admin-only operations)
- **List Management**: Create and delete todo lists owned by authenticated users  
- **Item Management**: Add, edit, and remove items from todo lists (owner access only)
- **Role-Based Authorization**: Admin and regular user roles with appropriate permissions
- **SQL Server Database**: Uses Entity Framework Core with SQL Server LocalDB
- **Database Migrations**: Entity Framework migrations for database schema management
- **Data Seeding**: Automatic population of sample data in development environment
- **API Documentation**: Integrated Swagger/OpenAPI documentation
- **RESTful Design**: Clean REST API endpoints following standard conventions

## Tech Stack

- **.NET 8**: Latest .NET framework
- **ASP.NET Core Web API**: Web API framework
- **Entity Framework Core**: ORM with multi-database support
- **SQL Server**: Production database (configurable)
- **SQLite**: Development/testing database
- **Swagger/OpenAPI**: API documentation and testing interface

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- **For Production**: [SQL Server LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (included with Visual Studio or SQL Server Express)
- **For Development/Testing**: SQLite (included automatically)

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
│   ├── AuthContracts.cs # Authentication contracts
│   ├── ItemContracts.cs
│   ├── ListContracts.cs
│   └── UserContracts.cs
├── Data/               # Database context
├── Migrations/         # Database migrations
├── Models/             # Entity models
│   ├── Item.cs
│   ├── List.cs
│   └── User.cs
├── Services/           # Business logic services
│   └── AuthService.cs  # Authentication and password hashing
├── Program.cs          # Application entry point and API endpoints
├── appsettings.json    # Configuration
└── ToDoList.Backend.csproj
```

## API Endpoints

### Authentication

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| POST | `/auth/login` | User login with username/password | Public |

#### Login
```http
POST /auth/login
Content-Type: application/json

{
  "username": "alice",
  "password": "password123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": 1,
  "name": "Alice Johnson",
  "isAdmin": true
}
```

### Users

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| POST | `/users` | Create a new user | Admin only |
| PUT | `/users/{id}` | Update an existing user | Admin only |
| DELETE | `/users/{id}` | Delete a user | Admin only |

#### Create User
```http
POST /users
Content-Type: application/json
Authorization: Bearer {admin-token}

{
  "name": "John Doe",
  "username": "john",
  "password": "securePassword123",
  "isAdmin": false
}
```

#### Update User
```http
PUT /users/1
Content-Type: application/json
Authorization: Bearer {admin-token}

{
  "name": "Jane Doe"
}
```

### Lists

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| POST | `/lists` | Create a new todo list | Authenticated users |
| DELETE | `/lists/{id}` | Delete a todo list | List owner or admin |

#### Create List
```http
POST /lists
Content-Type: application/json
Authorization: Bearer {user-token}

{
  "name": "Shopping List",
  "ownerID": 1
}
```

**Note**: Users can only create lists for themselves unless they are admins.

### Items

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| POST | `/lists/{listId}/items` | Add an item to a list | List owner or admin |
| PUT | `/items/{id}` | Update an item | List owner or admin |
| DELETE | `/items/{id}` | Remove an item | List owner or admin |

#### Add Item
```http
POST /lists/1/items
Content-Type: application/json
Authorization: Bearer {user-token}

{
  "name": "Buy milk"
}
```

#### Update Item
```http
PUT /items/1
Content-Type: application/json
Authorization: Bearer {user-token}

{
  "name": "Buy organic milk"
}
```

## Data Models

### User
- `Id` (int): Unique identifier
- `Name` (string): User's display name
- `Username` (string): Unique login username
- `PasswordHash` (string): BCrypt hashed password
- `IsAdmin` (bool): Administrative privileges flag
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

## Authentication and Security

The API uses JWT (JSON Web Token) based authentication with the following security features:

### Password Security
- **BCrypt Hashing**: All passwords are hashed using BCrypt with automatic salt generation
- **Secure Storage**: Plain text passwords are never stored in the database
- **Password Requirements**: Passwords can be enforced at the application level

### JWT Token Authentication
- **Token Expiration**: JWT tokens expire after 7 days
- **Claims-Based**: Tokens include user ID, username, and admin status
- **Stateless**: No server-side session storage required

### Authorization Levels
- **Public Endpoints**: Login endpoint accessible without authentication
- **Authenticated Users**: List and item management requires valid JWT token
- **Admin Only**: User management operations restricted to admin users
- **Resource Ownership**: Users can only access their own lists and items (unless admin)

### API Security Headers
When making authenticated requests, include the JWT token in the Authorization header:
```
Authorization: Bearer {your-jwt-token}
```

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
The application uses different database providers based on the environment:

- **Development**: SQLite database (`todolist_dev.db`) for easy setup and testing
- **Production**: SQL Server with connection string configuration

In Development environment, the database is automatically seeded with sample data including:

- **Sample Users**: 
  - Alice Johnson (username: `alice`, password: `password123`, admin: true)
  - Bob Smith (username: `bob`, password: `password123`, admin: false)
  - Carol Davis (username: `carol`, password: `password123`, admin: false)
- **Sample Lists**: Personal Tasks, Work Projects, Shopping List, Home Improvement
- **Sample Items**: Various todo items distributed across the lists

**Default Login Credentials for Testing:**
- **Admin User**: alice / password123
- **Regular Users**: bob / password123, carol / password123

#### Database Commands
```bash
# Create a new migration
dotnet ef migrations add <MigrationName>

# Update database to latest migration
dotnet ef database update

# Remove last migration (if not applied)
dotnet ef migrations remove
```

#### Database Configuration
The application automatically selects the appropriate database provider:
- SQLite for Development environment (easier setup, no dependencies)
- SQL Server for Production environment (better performance, enterprise features)

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test your changes
5. Submit a pull request

## License

This project is a coding agent test repository.