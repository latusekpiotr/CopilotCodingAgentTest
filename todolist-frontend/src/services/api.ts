import axios, { AxiosInstance } from 'axios';
import {
  LoginRequest,
  LoginResponse,
  User,
  CreateUserRequest,
  EditUserRequest,
  UpdatePasswordRequest,
  TodoList,
  CreateListRequest,
  EditListRequest,
  Item,
  AddItemRequest,
  EditItemRequest
} from '../types/api';

class ApiService {
  private api: AxiosInstance;

  constructor() {
    this.api = axios.create({
      baseURL: process.env.REACT_APP_API_URL || 'http://localhost:5011',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Add request interceptor to include JWT token
    this.api.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('token');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );

    // Add response interceptor to handle auth errors
    this.api.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          this.logout();
        }
        return Promise.reject(error);
      }
    );
  }

  // Auth methods
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    const response = await this.api.post<LoginResponse>('/auth/login', credentials);
    return response.data;
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = '/login';
  }

  // User methods
  async getUsers(): Promise<User[]> {
    const response = await this.api.get<User[]>('/users');
    return response.data;
  }

  async createUser(user: CreateUserRequest): Promise<User> {
    const response = await this.api.post<User>('/users', user);
    return response.data;
  }

  async updateUser(id: number, user: EditUserRequest): Promise<User> {
    const response = await this.api.put<User>(`/users/${id}`, user);
    return response.data;
  }

  async updateUserPassword(id: number, password: UpdatePasswordRequest): Promise<User> {
    const response = await this.api.put<User>(`/users/${id}/password`, password);
    return response.data;
  }

  async deleteUser(id: number): Promise<void> {
    await this.api.delete(`/users/${id}`);
  }

  // List methods
  async getLists(): Promise<TodoList[]> {
    const response = await this.api.get<TodoList[]>('/lists');
    return response.data;
  }

  async createList(list: CreateListRequest): Promise<TodoList> {
    const response = await this.api.post<TodoList>('/lists', list);
    return response.data;
  }

  async updateList(id: number, list: EditListRequest): Promise<TodoList> {
    const response = await this.api.put<TodoList>(`/lists/${id}`, list);
    return response.data;
  }

  async deleteList(id: number): Promise<void> {
    await this.api.delete(`/lists/${id}`);
  }

  // Item methods
  async getItems(listId: number): Promise<Item[]> {
    const response = await this.api.get<Item[]>(`/lists/${listId}/items`);
    return response.data;
  }

  async addItem(listId: number, item: AddItemRequest): Promise<Item> {
    const response = await this.api.post<Item>(`/lists/${listId}/items`, item);
    return response.data;
  }

  async updateItem(id: number, item: EditItemRequest): Promise<Item> {
    const response = await this.api.put<Item>(`/items/${id}`, item);
    return response.data;
  }

  async deleteItem(id: number): Promise<void> {
    await this.api.delete(`/items/${id}`);
  }
}

export const apiService = new ApiService();