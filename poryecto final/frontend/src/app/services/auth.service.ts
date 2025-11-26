import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { LoginRequest, LoginResponse, User } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_URL = `${environment.apiUrl}/auth`;
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.loadUserFromStorage();
  }

  // ======================
  // 🔐 LOGIN
  // ======================
  login(credentials: LoginRequest): Observable<any> {
    const payload = {
      email: credentials.email,
      password: credentials.password
    };

    return this.http.post<any>(`${this.API_URL}/login`, payload)
      .pipe(tap(response => this.setSession(response)));
  }

  // ======================
  // 📝 REGISTER
  // ======================
  register(credentials: LoginRequest): Observable<any> {
    const payload = {
      Email: credentials.email,
      Password: credentials.password
    };

    return this.http.post(`${this.API_URL}/register`, payload);
  }

  // ======================
  // 🚪 LOGOUT
  // ======================
  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('expiration');
    localStorage.removeItem('user');
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  // ======================
  // 🔄 REFRESH TOKEN
  // ======================
  refreshToken(): Observable<LoginResponse> {
    const refreshToken = localStorage.getItem('refreshToken');
    return this.http.post<LoginResponse>(`${this.API_URL}/refresh`, { refreshToken })
      .pipe(tap(response => this.setSession(response)));
  }

  // ======================
  // 🧪 CHECK LOGIN
  // ======================
  isLoggedIn(): boolean {
    const token = localStorage.getItem('token');
    const expiration = localStorage.getItem('expiration');
    if (!token || !expiration) return false;
    return new Date(expiration) > new Date();
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  isAdmin(): boolean {
    const user = this.getCurrentUser();
    return user?.role?.name === 'Admin';
  }

  // ======================
  // 🔐 SAVE SESSION
  // ======================
  private setSession(response: any): void {
    localStorage.setItem('token', response.token);
    localStorage.setItem('refreshToken', response.refreshToken || '');
    localStorage.setItem('expiration', new Date(response.expiration).toISOString());

    let user: User;

    // Check if user info comes directly in response (teacher bypass)
    if (response.user) {
      user = {
        id: response.user.id,
        email: response.user.email,
        role: { id: 1, name: response.user.role || 'User' }
      };
    } else {
      // Extract user info from JWT token (normal login)
      try {
        const payload = JSON.parse(atob(response.token.split('.')[1]));
        user = {
          id: parseInt(payload.sub),
          email: payload.email,
          role: { id: 1, name: payload.role || 'User' }
        };
      } catch (error) {
        console.error('Error decoding JWT token:', error);
        // Fallback user
        user = {
          id: 0,
          email: 'unknown',
          role: { id: 1, name: 'User' }
        };
      }
    }

    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  // ======================
  // 📌 LOAD USER
  // ======================
  private loadUserFromStorage(): void {
    const userStr = localStorage.getItem('user');
    if (userStr) {
      try {
        const user = JSON.parse(userStr);
        this.currentUserSubject.next(user);
      } catch {
        this.logout();
      }
    }
  }
}
