# ToDoList Frontend

A React TypeScript frontend for the ToDoList application that provides a modern, responsive web interface for managing todo lists with authentication and admin capabilities.

## Features

### 🔐 Authentication
- JWT-based authentication system
- Secure login/logout functionality
- Role-based access control (admin vs regular users)
- Protected routes with automatic redirection

### 📋 Todo List Management
- View all user's lists (admins can see all lists from all users)
- Create new todo lists
- Edit list names inline
- Delete lists with confirmation dialogs
- Real-time list updates

### 📝 Item Management
- View items within selected lists
- Add new items to lists
- Edit existing items inline
- Delete items with confirmation
- Responsive item display

### 👥 Admin Features
- Dedicated user management interface
- Create new users with admin privileges
- Edit user information
- Change user passwords
- Delete users (with confirmation)
- View all users in a clean table format

### 🎨 User Interface
- Clean, modern design
- Responsive layout that works on desktop and mobile
- Two-panel dashboard layout
- Modal dialogs for forms
- Error handling with user-friendly messages
- Loading states for better UX

## Tech Stack

- **React 18** - Modern React with hooks
- **TypeScript** - Type-safe development
- **React Router** - Client-side routing
- **Axios** - HTTP client for API calls
- **CSS Modules** - Scoped styling
- **JWT** - Secure authentication tokens

## Getting Started

### Prerequisites

- Node.js 14+ and npm
- ToDoList Backend running on `http://localhost:5011`

### Installation

1. Navigate to the frontend directory:
```bash
cd todolist-frontend
```

2. Install dependencies:
```bash
npm install
```

3. Start the development server:
```bash
npm start
```

4. Open [http://localhost:3000](http://localhost:3000) in your browser

### Building for Production

```bash
npm run build
```

This creates a `build` folder with optimized production files.

## Demo Credentials

The application comes with pre-seeded demo accounts:

- **Admin User**: `alice` / `password123`
- **Regular User**: `bob` / `password123`
- **Regular User**: `carol` / `password123`

## API Integration

The frontend integrates with the .NET backend API running on `http://localhost:5011`. The API base URL can be configured via the `REACT_APP_API_URL` environment variable.

### Environment Variables

Create a `.env` file in the frontend root directory:

```
REACT_APP_API_URL=http://localhost:5011
```

## Project Structure

```
src/
├── components/          # Reusable UI components
│   ├── ProtectedRoute.tsx
│   └── UserManagement.tsx
├── context/            # React contexts
│   └── AuthContext.tsx
├── pages/              # Page components
│   ├── Dashboard.tsx
│   └── Login.tsx
├── services/           # API and external services
│   └── api.ts
├── types/              # TypeScript type definitions
│   └── api.ts
└── App.tsx             # Main application component
```

## Key Components

### AuthContext
Manages authentication state, login/logout functionality, and provides user information across the application.

### Dashboard
The main application interface where users can:
- View and manage their todo lists
- Access admin features (if admin user)
- Create, edit, and delete lists and items

### UserManagement
Admin-only component for managing users:
- Create new users
- Edit user information
- Change passwords
- Delete users

### API Service
Centralized service for all backend API calls with:
- Automatic JWT token handling
- Error handling and auth token refresh
- Type-safe request/response handling

## Authentication Flow

1. User enters credentials on login page
2. Frontend sends login request to backend
3. Backend validates credentials and returns JWT token
4. Frontend stores token in localStorage
5. Token is automatically included in subsequent API requests
6. Protected routes check authentication status
7. Logout clears token and redirects to login

## Features by User Role

### Regular Users
- View and manage their own todo lists
- Create, edit, delete lists and items
- Cannot see other users' data
- Cannot access admin functions

### Admin Users
- All regular user capabilities
- View all lists from all users (with owner indication)
- Access user management interface
- Create, edit, delete users
- Change user passwords

## Development

### Running Tests
```bash
npm test
```

### Code Style
The project uses standard React/TypeScript conventions with:
- Functional components with hooks
- TypeScript for type safety
- CSS modules for scoped styling
- Consistent naming conventions

## Browser Support

The application supports all modern browsers including:
- Chrome 60+
- Firefox 60+
- Safari 12+
- Edge 79+