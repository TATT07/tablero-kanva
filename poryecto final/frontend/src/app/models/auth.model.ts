export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  expiration: Date;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
}

export interface User {
  id: number;
  email: string;
  role: Role;
}

export interface Role {
  id: number;
  name: string;
}