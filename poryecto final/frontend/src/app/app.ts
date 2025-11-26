import { Component, signal, OnInit } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonModule } from '@angular/common';
import { AuthService } from './services/auth.service';
import { User } from './models/auth.model';

@Component({
  selector: 'app-root',
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  protected readonly title = signal('frontend');
  sidebarOpen = false;

  constructor(private authService: AuthService, private router: Router) {}

  get currentUser$() {
    return this.authService.currentUser$;
  }

  ngOnInit(): void {
    // Check if user is logged in on app start
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
    }
  }

  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  currentUser(): User | null {
    return this.authService.getCurrentUser();
  }

  getUserInitial(): string {
    const user = this.currentUser();
    return user?.email?.charAt(0).toUpperCase() || 'U';
  }

  getUserDisplayName(): string {
    const user = this.currentUser();
    return user?.email || 'Unknown';
  }

  getUserRoleDisplay(): string {
    const user = this.currentUser();
    return user?.role?.name || 'User';
  }

  logout(): void {
    this.authService.logout();
  }

  toggleSidebar(): void {
    this.sidebarOpen = !this.sidebarOpen;
  }

  closeSidebarOnMobile(): void {
    // Only close sidebar on mobile/tablet devices
    if (window.innerWidth < 1024) {
      this.sidebarOpen = false;
    }
  }
}
