import React, { useState, useEffect } from 'react';
import { User, CreateUserRequest, EditUserRequest, UpdatePasswordRequest } from '../types/api';
import { apiService } from '../services/api';
import './UserManagement.css';

interface UserManagementProps {
  onClose: () => void;
}

const UserManagement: React.FC<UserManagementProps> = ({ onClose }) => {
  const [users, setUsers] = useState<User[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [editingUser, setEditingUser] = useState<{ id: number; name: string } | null>(null);
  const [changingPassword, setChangingPassword] = useState<{ id: number; password: string } | null>(null);
  
  // Create form state
  const [createForm, setCreateForm] = useState({
    name: '',
    username: '',
    password: '',
    isAdmin: false,
  });

  useEffect(() => {
    loadUsers();
  }, []);

  const loadUsers = async () => {
    try {
      setError('');
      const data = await apiService.getUsers();
      setUsers(data);
    } catch (error: any) {
      console.error('Failed to load users:', error);
      setError('Failed to load users');
    } finally {
      setIsLoading(false);
    }
  };

  const handleCreateUser = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!createForm.name.trim() || !createForm.username.trim() || !createForm.password.trim()) return;

    try {
      const request: CreateUserRequest = {
        name: createForm.name.trim(),
        username: createForm.username.trim(),
        password: createForm.password,
        isAdmin: createForm.isAdmin,
      };
      const newUser = await apiService.createUser(request);
      setUsers([...users, newUser]);
      setCreateForm({ name: '', username: '', password: '', isAdmin: false });
      setShowCreateForm(false);
    } catch (error: any) {
      console.error('Failed to create user:', error);
      setError(error.response?.data || 'Failed to create user');
    }
  };

  const handleEditUser = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingUser) return;

    try {
      const request: EditUserRequest = { name: editingUser.name };
      const updatedUser = await apiService.updateUser(editingUser.id, request);
      setUsers(users.map(user => user.id === editingUser.id ? { ...user, ...updatedUser } : user));
      setEditingUser(null);
    } catch (error: any) {
      console.error('Failed to edit user:', error);
      setError('Failed to edit user');
    }
  };

  const handleChangePassword = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!changingPassword || !changingPassword.password.trim()) return;

    try {
      const request: UpdatePasswordRequest = { password: changingPassword.password };
      await apiService.updateUserPassword(changingPassword.id, request);
      setChangingPassword(null);
      alert('Password updated successfully');
    } catch (error: any) {
      console.error('Failed to change password:', error);
      setError('Failed to change password');
    }
  };

  const handleDeleteUser = async (userId: number) => {
    if (!window.confirm('Are you sure you want to delete this user? This will also delete all their lists and items.')) return;

    try {
      await apiService.deleteUser(userId);
      setUsers(users.filter(user => user.id !== userId));
    } catch (error: any) {
      console.error('Failed to delete user:', error);
      setError('Failed to delete user');
    }
  };

  if (isLoading) {
    return <div className="loading">Loading users...</div>;
  }

  return (
    <div className="user-management">
      <header className="user-management-header">
        <h1>User Management</h1>
        <div className="header-actions">
          <button 
            className="create-btn"
            onClick={() => setShowCreateForm(true)}
          >
            Create New User
          </button>
          <button className="close-btn" onClick={onClose}>
            Back to Dashboard
          </button>
        </div>
      </header>

      {error && <div className="error-message">{error}</div>}

      {showCreateForm && (
        <div className="modal-overlay">
          <div className="modal">
            <h2>Create New User</h2>
            <form onSubmit={handleCreateUser}>
              <div className="form-group">
                <label htmlFor="name">Name:</label>
                <input
                  type="text"
                  id="name"
                  value={createForm.name}
                  onChange={(e) => setCreateForm({ ...createForm, name: e.target.value })}
                  required
                />
              </div>
              <div className="form-group">
                <label htmlFor="username">Username:</label>
                <input
                  type="text"
                  id="username"
                  value={createForm.username}
                  onChange={(e) => setCreateForm({ ...createForm, username: e.target.value })}
                  required
                />
              </div>
              <div className="form-group">
                <label htmlFor="password">Password:</label>
                <input
                  type="password"
                  id="password"
                  value={createForm.password}
                  onChange={(e) => setCreateForm({ ...createForm, password: e.target.value })}
                  required
                />
              </div>
              <div className="form-group checkbox-group">
                <label>
                  <input
                    type="checkbox"
                    checked={createForm.isAdmin}
                    onChange={(e) => setCreateForm({ ...createForm, isAdmin: e.target.checked })}
                  />
                  Admin User
                </label>
              </div>
              <div className="form-actions">
                <button type="submit">Create User</button>
                <button type="button" onClick={() => setShowCreateForm(false)}>Cancel</button>
              </div>
            </form>
          </div>
        </div>
      )}

      <div className="users-table">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th>Username</th>
              <th>Admin</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {users.map(user => (
              <tr key={user.id}>
                <td>{user.id}</td>
                <td>
                  {editingUser?.id === user.id ? (
                    <form onSubmit={handleEditUser} className="inline-edit">
                      <input
                        type="text"
                        value={editingUser.name}
                        onChange={(e) => setEditingUser({ ...editingUser, name: e.target.value })}
                        autoFocus
                      />
                      <button type="submit">Save</button>
                      <button type="button" onClick={() => setEditingUser(null)}>Cancel</button>
                    </form>
                  ) : (
                    user.name
                  )}
                </td>
                <td>{user.username}</td>
                <td>{user.isAdmin ? 'Yes' : 'No'}</td>
                <td>
                  <div className="actions">
                    {editingUser?.id !== user.id && (
                      <>
                        <button 
                          className="edit-btn"
                          onClick={() => setEditingUser({ id: user.id, name: user.name })}
                        >
                          Edit Name
                        </button>
                        <button 
                          className="password-btn"
                          onClick={() => setChangingPassword({ id: user.id, password: '' })}
                        >
                          Change Password
                        </button>
                        <button 
                          className="delete-btn"
                          onClick={() => handleDeleteUser(user.id)}
                        >
                          Delete
                        </button>
                      </>
                    )}
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {changingPassword && (
        <div className="modal-overlay">
          <div className="modal">
            <h2>Change Password</h2>
            <form onSubmit={handleChangePassword}>
              <div className="form-group">
                <label htmlFor="newPassword">New Password:</label>
                <input
                  type="password"
                  id="newPassword"
                  value={changingPassword.password}
                  onChange={(e) => setChangingPassword({ ...changingPassword, password: e.target.value })}
                  required
                  autoFocus
                />
              </div>
              <div className="form-actions">
                <button type="submit">Update Password</button>
                <button type="button" onClick={() => setChangingPassword(null)}>Cancel</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default UserManagement;