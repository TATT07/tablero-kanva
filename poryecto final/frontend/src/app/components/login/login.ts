import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatTabsModule } from '@angular/material/tabs';
import { AuthService } from '../../services/auth.service';
import { LoginRequest, RegisterRequest } from '../../models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatTabsModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class LoginComponent {
  loginForm: FormGroup;
  registerForm: FormGroup;
  isLoading = false;
  isRegistering = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });

    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(group: AbstractControl): { [key: string]: any } | null {
    const password = group.get('password');
    const confirmPassword = group.get('confirmPassword');
    return password && confirmPassword && password.value !== confirmPassword.value
      ? { passwordMismatch: true }
      : null;
  }

  errorMessage = '';

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      const credentials: LoginRequest = this.loginForm.value;

      this.authService.login(credentials).subscribe({
        next: () => {
          this.isLoading = false;
          this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = 'Invalid credentials';
        }
      });
    }
  }

  onRegisterSubmit(): void {
    if (this.registerForm.valid) {
      this.isRegistering = true;
      const credentials: RegisterRequest = this.registerForm.value;

      this.authService.register(credentials).subscribe({
        next: () => {
          this.isRegistering = false;
          this.snackBar.open('Registration successful! Please login.', 'Close', {
            duration: 3000
          });
          this.registerForm.reset();
        },
        error: (error) => {
          this.isRegistering = false;
          this.snackBar.open(error.error || 'Registration failed', 'Close', {
            duration: 3000
          });
        }
      });
    }
  }
}
