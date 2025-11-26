import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTabsModule } from '@angular/material/tabs';
import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../../models/auth.model';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatTabsModule
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  loginForm: FormGroup;
  registerForm: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private notificationService: NotificationService
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });

    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  onLogin(): void {
    if (this.loginForm.invalid) return;

    this.isLoading = true;
    const credentials: LoginRequest = {
      email: this.loginForm.get('email')?.value,
      password: this.loginForm.get('password')?.value
    };


    this.authService.login(credentials).subscribe({
      next: () => {
        this.notificationService.success('Inicio de sesión exitoso');
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        this.notificationService.error('Credenciales inválidas');
        this.isLoading = false;
      }
    });
  }

  onRegister(): void {
    if (this.registerForm.invalid) return;

    this.isLoading = true;
    const { confirmPassword, ...credentials } = this.registerForm.value;

    this.authService.register(credentials).subscribe({
      next: () => {
        this.notificationService.success('Usuario registrado exitosamente. Ahora puedes iniciar sesión.');
        this.registerForm.reset();
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationService.error('Error al registrar usuario');
        this.isLoading = false;
      }
    });
  }

  private passwordMatchValidator(group: FormGroup): any {
    const password = group.get('password');
    const confirmPassword = group.get('confirmPassword');

    if (password?.value !== confirmPassword?.value) {
      confirmPassword?.setErrors({ mismatch: true });
    } else {
      const errors = confirmPassword?.errors;
      if (errors) {
        delete errors['mismatch'];
        confirmPassword?.setErrors(Object.keys(errors).length ? errors : null);
      }
    }
    return null;
  }
}