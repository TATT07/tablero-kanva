import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models/auth.model';

@Component({
  selector: 'app-user-card',
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule
  ],
  templateUrl: './user-card.html',
  styleUrl: './user-card.scss',
})
export class UserCard implements OnInit {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {}

  get currentUser$() {
    return this.authService.currentUser$;
  }

  currentUser(): User | null {
    return this.authService.getCurrentUser();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  getUserDisplayName(): string {
    const user = this.currentUser();
    if (user?.id === 999) {
      // Special teacher account
      return 'Docente FESC';
    }
    // For other users, extract name from email or use a default
    const email = user?.email || '';
    const namePart = email.split('@')[0];
    // Convert snake_case or dots to title case
    return namePart
      .split(/[._]/)
      .map(word => word.charAt(0).toUpperCase() + word.slice(1).toLowerCase())
      .join(' ');
  }

  getUserRoleDisplay(): string {
    const user = this.currentUser();
    if (!user) return 'USER';

    // Handle different role structures
    let roleName = 'USER';

    if (typeof user.role === 'string') {
      roleName = user.role;
    } else if (user.role && typeof user.role === 'object' && (user.role as any).name) {
      roleName = (user.role as any).name;
    }

    return roleName === 'Admin' ? 'ADMINISTRADOR' : roleName.toUpperCase();
  }
}
