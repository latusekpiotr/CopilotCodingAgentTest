import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import { apiService } from '../services/api';
import { DebugUser } from '../types/api';
import './Login.css';

const Login: React.FC = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [debugUsers, setDebugUsers] = useState<DebugUser[]>([]);
  const [debugError, setDebugError] = useState('');
  const { login } = useAuth();
  const navigate = useNavigate();

  // Fetch debug user data on component mount
  useEffect(() => {
    const fetchDebugUsers = async () => {
      try {
        const users = await apiService.getDebugUsers();
        setDebugUsers(users);
      } catch (error: any) {
        console.error('Failed to fetch debug users:', error);
        setDebugError('Failed to load user debug data');
      }
    };

    fetchDebugUsers();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      await login({ username, password });
      navigate('/dashboard');
    } catch (error: any) {
      console.error('Login error:', error);
      setError(error.response?.status === 401 ? 'Invalid username or password' : 'Login failed. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="login-container">
      <div className="login-form">
        <h1>ToDoList Login</h1>
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="username">Username:</label>
            <input
              type="text"
              id="username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
              disabled={isLoading}
            />
          </div>
          <div className="form-group">
            <label htmlFor="password">Password:</label>
            <input
              type="password"
              id="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              disabled={isLoading}
            />
          </div>
          {error && <div className="error-message">{error}</div>}
          <button type="submit" disabled={isLoading}>
            {isLoading ? 'Logging in...' : 'Login'}
          </button>
        </form>
        <div className="demo-credentials">
          <h3>Demo Credentials:</h3>
          <p><strong>Admin:</strong> alice / password123</p>
          <p><strong>User:</strong> bob / password123</p>
          <p><strong>User:</strong> carol / password123</p>
        </div>
        
        <div className="debug-info">
          <h3>Debug - User Table Contents:</h3>
          {debugError ? (
            <p style={{ color: 'red' }}>Error: {debugError}</p>
          ) : debugUsers.length === 0 ? (
            <p>Loading user data...</p>
          ) : (
            <div style={{ fontSize: '12px', fontFamily: 'monospace', backgroundColor: '#f5f5f5', padding: '10px', border: '1px solid #ccc', marginTop: '10px' }}>
              <p><strong>Total Users: {debugUsers.length}</strong></p>
              {debugUsers.map(user => (
                <div key={user.id} style={{ marginBottom: '10px', padding: '5px', backgroundColor: 'white', border: '1px solid #ddd' }}>
                  <p><strong>ID:</strong> {user.id}</p>
                  <p><strong>Name:</strong> {user.name}</p>
                  <p><strong>Username:</strong> {user.username}</p>
                  <p><strong>Password Hash:</strong> {user.passwordHash.substring(0, 20)}...</p>
                  <p><strong>Is Admin:</strong> {user.isAdmin ? 'Yes' : 'No'}</p>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Login;