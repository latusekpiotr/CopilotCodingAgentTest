// API types matching the backend contracts

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  userId: number;
  name: string;
  isAdmin: boolean;
}

export interface User {
  id: number;
  name: string;
  username: string;
  isAdmin: boolean;
}

export interface CreateUserRequest {
  name: string;
  username: string;
  password: string;
  isAdmin?: boolean;
}

export interface EditUserRequest {
  name: string;
}

export interface UpdatePasswordRequest {
  password: string;
}

export interface TodoList {
  id: number;
  name: string;
  ownerID: number;
  ownerName?: string;
}

export interface CreateListRequest {
  name: string;
  ownerID: number;
}

export interface EditListRequest {
  name: string;
}

export interface Item {
  id: number;
  name: string;
  listId: number;
}

export interface AddItemRequest {
  name: string;
}

export interface EditItemRequest {
  name: string;
}