import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { TodoList, Item, CreateListRequest, EditListRequest, AddItemRequest, EditItemRequest } from '../types/api';
import { apiService } from '../services/api';
import UserManagement from '../components/UserManagement';
import './Dashboard.css';

const Dashboard: React.FC = () => {
  const { user, logout } = useAuth();
  const [lists, setLists] = useState<TodoList[]>([]);
  const [selectedList, setSelectedList] = useState<TodoList | null>(null);
  const [items, setItems] = useState<Item[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [showUserManagement, setShowUserManagement] = useState(false);
  
  // Form states
  const [newListName, setNewListName] = useState('');
  const [newItemName, setNewItemName] = useState('');
  const [editingList, setEditingList] = useState<{ id: number; name: string } | null>(null);
  const [editingItem, setEditingItem] = useState<{ id: number; name: string } | null>(null);

  useEffect(() => {
    loadLists();
  }, []);

  useEffect(() => {
    if (selectedList) {
      loadItems(selectedList.id);
    }
  }, [selectedList]);

  const loadLists = async () => {
    try {
      setError('');
      const data = await apiService.getLists();
      setLists(data);
    } catch (error: any) {
      console.error('Failed to load lists:', error);
      setError('Failed to load lists');
    } finally {
      setIsLoading(false);
    }
  };

  const loadItems = async (listId: number) => {
    try {
      setError('');
      const data = await apiService.getItems(listId);
      setItems(data);
    } catch (error: any) {
      console.error('Failed to load items:', error);
      setError('Failed to load items');
    }
  };

  const handleCreateList = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newListName.trim() || !user) return;

    try {
      const request: CreateListRequest = {
        name: newListName.trim(),
        ownerID: user.id,
      };
      const newList = await apiService.createList(request);
      setLists([...lists, newList]);
      setNewListName('');
    } catch (error: any) {
      console.error('Failed to create list:', error);
      setError('Failed to create list');
    }
  };

  const handleEditList = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingList) return;

    try {
      const request: EditListRequest = { name: editingList.name };
      const updatedList = await apiService.updateList(editingList.id, request);
      setLists(lists.map(list => list.id === editingList.id ? { ...list, ...updatedList } : list));
      if (selectedList?.id === editingList.id) {
        setSelectedList({ ...selectedList, ...updatedList });
      }
      setEditingList(null);
    } catch (error: any) {
      console.error('Failed to edit list:', error);
      setError('Failed to edit list');
    }
  };

  const handleDeleteList = async (listId: number) => {
    if (!window.confirm('Are you sure you want to delete this list and all its items?')) return;

    try {
      await apiService.deleteList(listId);
      setLists(lists.filter(list => list.id !== listId));
      if (selectedList?.id === listId) {
        setSelectedList(null);
        setItems([]);
      }
    } catch (error: any) {
      console.error('Failed to delete list:', error);
      setError('Failed to delete list');
    }
  };

  const handleAddItem = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newItemName.trim() || !selectedList) return;

    try {
      const request: AddItemRequest = { name: newItemName.trim() };
      const newItem = await apiService.addItem(selectedList.id, request);
      setItems([...items, newItem]);
      setNewItemName('');
    } catch (error: any) {
      console.error('Failed to add item:', error);
      setError('Failed to add item');
    }
  };

  const handleEditItem = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingItem) return;

    try {
      const request: EditItemRequest = { name: editingItem.name };
      const updatedItem = await apiService.updateItem(editingItem.id, request);
      setItems(items.map(item => item.id === editingItem.id ? { ...item, ...updatedItem } : item));
      setEditingItem(null);
    } catch (error: any) {
      console.error('Failed to edit item:', error);
      setError('Failed to edit item');
    }
  };

  const handleDeleteItem = async (itemId: number) => {
    if (!window.confirm('Are you sure you want to delete this item?')) return;

    try {
      await apiService.deleteItem(itemId);
      setItems(items.filter(item => item.id !== itemId));
    } catch (error: any) {
      console.error('Failed to delete item:', error);
      setError('Failed to delete item');
    }
  };

  if (isLoading) {
    return <div className="loading">Loading...</div>;
  }

  if (showUserManagement && user?.isAdmin) {
    return <UserManagement onClose={() => setShowUserManagement(false)} />;
  }

  return (
    <div className="dashboard">
      <header className="dashboard-header">
        <h1>Welcome, {user?.name}!</h1>
        <div className="header-actions">
          {user?.isAdmin && (
            <button 
              className="admin-btn"
              onClick={() => setShowUserManagement(true)}
            >
              Manage Users
            </button>
          )}
          <button className="logout-btn" onClick={logout}>
            Logout
          </button>
        </div>
      </header>

      {error && <div className="error-message">{error}</div>}

      <div className="dashboard-content">
        <div className="lists-panel">
          <h2>Your Lists</h2>
          
          <form onSubmit={handleCreateList} className="create-list-form">
            <input
              type="text"
              placeholder="New list name"
              value={newListName}
              onChange={(e) => setNewListName(e.target.value)}
              required
            />
            <button type="submit">Add List</button>
          </form>

          <div className="lists-container">
            {lists.map(list => (
              <div 
                key={list.id} 
                className={`list-item ${selectedList?.id === list.id ? 'selected' : ''}`}
                onClick={() => setSelectedList(list)}
              >
                {editingList?.id === list.id ? (
                  <form onSubmit={handleEditList} className="edit-form">
                    <input
                      type="text"
                      value={editingList.name}
                      onChange={(e) => setEditingList({ ...editingList, name: e.target.value })}
                      autoFocus
                    />
                    <button type="submit">Save</button>
                    <button type="button" onClick={() => setEditingList(null)}>Cancel</button>
                  </form>
                ) : (
                  <>
                    <span className="list-name">{list.name}</span>
                    {user?.isAdmin && list.ownerName && (
                      <span className="list-owner">({list.ownerName})</span>
                    )}
                    <div className="list-actions">
                      <button onClick={(e) => {
                        e.stopPropagation();
                        setEditingList({ id: list.id, name: list.name });
                      }}>
                        Edit
                      </button>
                      <button onClick={(e) => {
                        e.stopPropagation();
                        handleDeleteList(list.id);
                      }}>
                        Delete
                      </button>
                    </div>
                  </>
                )}
              </div>
            ))}
          </div>
        </div>

        <div className="items-panel">
          {selectedList ? (
            <>
              <h2>Items in "{selectedList.name}"</h2>
              
              <form onSubmit={handleAddItem} className="add-item-form">
                <input
                  type="text"
                  placeholder="New item"
                  value={newItemName}
                  onChange={(e) => setNewItemName(e.target.value)}
                  required
                />
                <button type="submit">Add Item</button>
              </form>

              <div className="items-container">
                {items.map(item => (
                  <div key={item.id} className="item">
                    {editingItem?.id === item.id ? (
                      <form onSubmit={handleEditItem} className="edit-form">
                        <input
                          type="text"
                          value={editingItem.name}
                          onChange={(e) => setEditingItem({ ...editingItem, name: e.target.value })}
                          autoFocus
                        />
                        <button type="submit">Save</button>
                        <button type="button" onClick={() => setEditingItem(null)}>Cancel</button>
                      </form>
                    ) : (
                      <>
                        <span className="item-name">{item.name}</span>
                        <div className="item-actions">
                          <button onClick={() => setEditingItem({ id: item.id, name: item.name })}>
                            Edit
                          </button>
                          <button onClick={() => handleDeleteItem(item.id)}>
                            Delete
                          </button>
                        </div>
                      </>
                    )}
                  </div>
                ))}
                {items.length === 0 && (
                  <p className="no-items">No items in this list yet.</p>
                )}
              </div>
            </>
          ) : (
            <div className="no-selection">
              <p>Select a list to view its items</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Dashboard;