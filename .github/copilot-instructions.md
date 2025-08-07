# CopilotCodingAgentTest Repository Instructions

**ALWAYS** follow these instructions first and only fallback to additional search and context gathering if the information in these instructions is incomplete or found to be in error.

## Repository Overview
CopilotCodingAgentTest is a full-stack TodoList application with a .NET 8 ASP.NET Core Web API backend and React TypeScript frontend. This repository demonstrates modern web development practices with JWT authentication, Entity Framework Core, SQL Server/SQLite database, and Azure CI/CD deployment. It serves as both a functional application and a testing ground for validating GitHub Copilot coding agent instructions and workflows.

## Working Effectively

### Repository Structure
The repository contains the following files and directories:
- `README.md` - Comprehensive application documentation
- `.github/` - GitHub configuration directory
  - `.github/copilot-instructions.md` - This instructions file
  - `.github/workflows/` - CI/CD GitHub Actions workflows
- `ToDoList.Backend/` - .NET 8 ASP.NET Core Web API
  - `Models/` - Entity models (User, List, Item)
  - `Data/` - Entity Framework DbContext
  - `Services/` - Business logic services
  - `Contracts/` - Request/Response DTOs
  - `Migrations/` - Entity Framework database migrations
  - `Program.cs` - Application entry point and API endpoints
- `todolist-frontend/` - React TypeScript frontend
  - `src/components/` - Reusable UI components
  - `src/pages/` - Page components (Login, Dashboard)
  - `src/context/` - React contexts (AuthContext)
  - `src/services/` - API services
  - `src/types/` - TypeScript definitions
  - `package.json` - Frontend dependencies and scripts
- `infrastructure/` - Azure Infrastructure as Code
  - `main.bicep` - Azure Bicep template for App Service deployment
- `CI-CD-SETUP.md` - CI/CD setup documentation
- `ToDoList.Backend.sln` - .NET solution file
- `.gitignore` - Git ignore patterns for build artifacts

### Basic Navigation and Git Operations
- Always start by checking repository status: `git status`
- View recent commits: `git log --oneline -10`
- Check current branch: `git branch -a`
- List repository contents: `ls -la`
- Find all markdown files: `find . -type f -name "*.md"`

### Tech Stack Overview
**Backend:**
- .NET 8 ASP.NET Core Web API
- Entity Framework Core ORM
- SQL Server (production) / SQLite (development)
- JWT authentication with BCrypt password hashing
- Swagger/OpenAPI documentation

**Frontend:**
- React 18 with TypeScript
- React Router for client-side routing
- Axios for HTTP client
- JWT token-based authentication

**Infrastructure:**
- Azure App Service deployment
- Azure Bicep templates for Infrastructure as Code
- GitHub Actions for CI/CD pipelines

### Build and Test Status
This repository contains a full-stack application with the following build and test capabilities:

**Backend (.NET 8):**
- Build: `dotnet build` (in ToDoList.Backend directory)
- Run: `dotnet run` (starts API on localhost:5011/7011)
- Database migrations: `dotnet ef database update`
- Test: Currently no unit tests (can be added as needed)

**Frontend (React TypeScript):**
- Install dependencies: `npm install` (in todolist-frontend directory)
- Build: `npm run build`
- Run development server: `npm start` (starts on localhost:3000)
- Test: `npm test` (React testing framework available)

**Infrastructure:**
- Validate Bicep: `az bicep build --file infrastructure/main.bicep`
- Deploy via GitHub Actions workflows (manual trigger)

### Validation Steps
When working in this repository:
1. Always run `git status` to check for uncommitted changes
2. Always run `ls -la` to see current directory contents
3. Use `find . -type f -name "*" | grep -v ".git" | grep -v "node_modules" | grep -v "bin" | grep -v "obj"` to see tracked source files
4. When adding new files, verify they appear in `git status`
5. Before committing, always run `git diff` to review changes
6. For backend changes: run `dotnet build` to ensure compilation
7. For frontend changes: run `npm run build` to ensure TypeScript compilation
8. For database changes: ensure migrations are created with `dotnet ef migrations add`

### Repository Capabilities
- **Full-Stack Application**: Complete .NET 8 backend with React TypeScript frontend
- **Database Operations**: Entity Framework Core with SQL Server/SQLite support
- **Build Systems**: .NET build system and npm/React build pipeline
- **Dependencies**: NuGet packages for .NET, npm packages for React
- **Testing Framework**: .NET testing capabilities and React testing library available
- **CI/CD Pipelines**: GitHub Actions workflows for Azure deployment
- **Local Development**: Full development environment with hot reload support
- **API Documentation**: Swagger/OpenAPI documentation at /swagger endpoint
- **Authentication**: JWT-based authentication with secure password hashing

## Common Tasks

### Backend Development (.NET 8)
1. **Build the backend:**
   ```bash
   cd ToDoList.Backend
   dotnet build
   ```

2. **Run the backend API:**
   ```bash
   cd ToDoList.Backend
   dotnet run
   # API available at https://localhost:7011 and http://localhost:5011
   # Swagger documentation at https://localhost:7011/swagger
   ```

3. **Database operations:**
   ```bash
   cd ToDoList.Backend
   # Install EF Core tools if not already installed
   dotnet tool install --global dotnet-ef
   
   # Create new migration
   dotnet ef migrations add <MigrationName>
   
   # Update database
   dotnet ef database update
   
   # Remove last migration (if not applied)
   dotnet ef migrations remove
   
   # List existing migrations
   dotnet ef migrations list
   ```

### Frontend Development (React TypeScript)
1. **Install dependencies:**
   ```bash
   cd todolist-frontend
   npm install
   ```

2. **Run development server:**
   ```bash
   cd todolist-frontend
   npm start
   # React app available at http://localhost:3000
   ```

3. **Build for production:**
   ```bash
   cd todolist-frontend
   npm run build
   ```

4. **Run tests:**
   ```bash
   cd todolist-frontend
   npm test
   ```

### Full Application Startup
1. **Start backend:** (Terminal 1)
   ```bash
   cd ToDoList.Backend
   dotnet ef database update  # Ensure database is up to date
   dotnet run
   ```

2. **Start frontend:** (Terminal 2)
   ```bash
   cd todolist-frontend
   npm install  # Only needed first time or after package changes
   npm start
   ```

3. **Access application:**
   - Frontend: http://localhost:3000
   - Backend API: https://localhost:7011
   - API Documentation: https://localhost:7011/swagger

### Authentication Testing
**Demo Credentials:**
- Admin User: alice / password123
- Regular Users: bob / password123, carol / password123

### Adding New Files
1. Create the file: `touch filename.ext`
2. Verify creation: `ls -la`
3. Check git status: `git status`
4. Add to git: `git add filename.ext`
5. Commit: `git commit -m "Add filename.ext"`

### CI/CD Operations
**GitHub Actions Workflows** (Manual trigger only):

1. **Deploy Infrastructure:**
   - Workflow: `.github/workflows/deploy-infrastructure.yml`
   - Creates Azure resources using Bicep templates
   - Requires `AZURE_CREDENTIALS` GitHub secret

2. **Deploy Application:**
   - Workflow: `.github/workflows/deploy-application.yml`
   - Builds and deploys both backend and frontend
   - Deploys to existing Azure infrastructure

3. **Destroy Infrastructure:**
   - Workflow: `.github/workflows/destroy-infrastructure.yml`
   - Safely removes Azure resources
   - Requires "DESTROY" confirmation

### Viewing Repository Contents
```bash
# List all files in repository root
ls -la

# Find all source files excluding build artifacts
find . -type f | grep -v ".git" | grep -v "node_modules" | grep -v "bin" | grep -v "obj" | grep -v "dist"

# Count total source files
find . -type f | grep -v ".git" | grep -v "node_modules" | grep -v "bin" | grep -v "obj" | wc -l

# View backend project structure
find ToDoList.Backend -type f -name "*.cs" -o -name "*.json" -o -name "*.csproj"

# View frontend project structure  
find todolist-frontend/src -type f -name "*.tsx" -o -name "*.ts" -o -name "*.json"

# View infrastructure files
find infrastructure -type f -name "*.bicep"

# View CI/CD workflows
find .github/workflows -type f -name "*.yml"
```

### Expected Command Outputs
The following are outputs from frequently run commands. Reference them instead of running bash commands unnecessarily.

#### Repository Root Listing
```
$ ls -la
total 56
drwxr-xr-x 7 runner docker  4096 [date] .
drwxr-xr-x 3 runner docker  4096 [date] ..
drwxr-xr-x 7 runner docker  4096 [date] .git
drwxr-xr-x 3 runner docker  4096 [date] .github
-rw-r--r-- 1 runner docker  1168 [date] .gitignore
-rw-r--r-- 1 runner docker  2675 [date] CI-CD-SETUP.md
-rw-r--r-- 1 runner docker 12732 [date] README.md
drwxr-xr-x 8 runner docker  4096 [date] ToDoList.Backend
-rw-r--r-- 1 runner docker  1024 [date] ToDoList.Backend.sln
drwxr-xr-x 2 runner docker  4096 [date] infrastructure
drwxr-xr-x 4 runner docker  4096 [date] todolist-frontend
```

#### Backend Project Structure
```
$ ls -la ToDoList.Backend/
total X
drwxr-xr-x X runner docker XXXX [date] .
drwxr-xr-x X runner docker XXXX [date] ..
drwxr-xr-x X runner docker XXXX [date] Contracts
drwxr-xr-x X runner docker XXXX [date] Data
drwxr-xr-x X runner docker XXXX [date] Migrations
drwxr-xr-x X runner docker XXXX [date] Models
-rw-r--r-- X runner docker XXXX [date] Program.cs
drwxr-xr-x X runner docker XXXX [date] Properties
drwxr-xr-x X runner docker XXXX [date] Services
-rw-r--r-- X runner docker XXXX [date] ToDoList.Backend.csproj
-rw-r--r-- X runner docker XXXX [date] appsettings.json
-rw-r--r-- X runner docker XXXX [date] appsettings.Development.json
```

#### Frontend Project Structure
```
$ ls -la todolist-frontend/
total X
drwxr-xr-x X runner docker XXXX [date] .
drwxr-xr-x X runner docker XXXX [date] ..
-rw-r--r-- X runner docker XXXX [date] .gitignore
-rw-r--r-- X runner docker XXXX [date] package.json
-rw-r--r-- X runner docker XXXX [date] package-lock.json
drwxr-xr-x X runner docker XXXX [date] public
-rw-r--r-- X runner docker XXXX [date] README.md
drwxr-xr-x X runner docker XXXX [date] src
-rw-r--r-- X runner docker XXXX [date] tsconfig.json
```

#### Git Status (Clean State)
```
$ git status
On branch [branch-name]
Your branch is up to date with 'origin/[branch-name]'.

nothing to commit, working tree clean
```

## File Modification Guidelines
- Always use absolute paths when referencing files: `/home/runner/work/CopilotCodingAgentTest/CopilotCodingAgentTest/`
- When editing files, use `str_replace_editor` with the `view` command first to understand current content
- For new files, use `str_replace_editor` with the `create` command
- For modifications, use `str_replace_editor` with the `str_replace` command

## What NOT to Do
- Do NOT run `dotnet ef database update` without ensuring you're in the ToDoList.Backend directory
- Do NOT run `npm install` or `npm start` without ensuring you're in the todolist-frontend directory
- Do NOT commit `node_modules/`, `bin/`, `obj/`, or other build artifacts (use .gitignore)
- Do NOT run database migrations in production without proper backups
- Do NOT hardcode sensitive information like connection strings or API keys
- Do NOT modify Entity Framework migrations that have already been applied to production
- Do NOT run CI/CD workflows without proper Azure credentials configured
- Do NOT delete infrastructure via GitHub Actions without proper confirmation
- Do NOT bypass authentication for protected API endpoints in production

## Emergency Procedures
If you encounter unexpected errors:
1. Run `git status` to check repository state
2. Run `ls -la` to verify file structure
3. For backend issues:
   - Check if you're in ToDoList.Backend directory: `pwd`
   - Verify .NET SDK is available: `dotnet --version`
   - Check build errors: `dotnet build`
4. For frontend issues:
   - Check if you're in todolist-frontend directory: `pwd`
   - Verify Node.js is available: `node --version`
   - Check for dependency issues: `npm install`
5. For database issues:
   - Check database connection in appsettings.json
   - Verify migrations: `dotnet ef migrations list`
   - Check database status: `dotnet ef database update --dry-run`
6. If files are missing, check if you're in the correct directory: `pwd`
7. Verify you're using absolute paths: `/home/runner/work/CopilotCodingAgentTest/CopilotCodingAgentTest/`

## Success Criteria
You are working effectively in this repository when:
- You can successfully navigate and list repository contents
- You can build both backend (.NET) and frontend (React) applications
- You can run the full-stack application locally with database connectivity
- You understand the JWT authentication flow and API endpoints
- You can create and apply Entity Framework migrations
- You use git commands to track changes appropriately
- You can identify and use the CI/CD workflows for Azure deployment
- You follow security best practices for authentication and data handling
- You understand this is a production-ready full-stack application with proper architecture