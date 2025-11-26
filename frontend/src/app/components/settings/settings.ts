import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { TaskService } from '../../services/task.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCheckboxModule,
    MatIconModule
  ],
  templateUrl: './settings.html',
  styleUrl: './settings.scss',
})
export class Settings {
  // Configuración de notificaciones
  notifications = {
    taskCompleted: true,
    taskOverdue: true,
    dailySummary: false,
    weeklyReport: true
  };

  // Perfil de usuario
  profile = {
    name: 'Usuario Demo',
    email: 'usuario@demo.com',
    avatar: 'U'
  };

  constructor(
    private taskService: TaskService,
    private dialog: MatDialog
  ) {}

  exportData(): void {
    this.taskService.exportExcel().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `kanban-tasks-${new Date().toISOString().split('T')[0]}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
        alert('Datos exportados exitosamente');
      },
      error: () => {
        alert('Error al exportar datos. Inténtalo de nuevo.');
      }
    });
  }

  editProfile(): void {
    const newName = prompt('Ingresa tu nuevo nombre:', this.profile.name);
    if (newName && newName.trim()) {
      this.profile.name = newName.trim();
      this.profile.avatar = newName.charAt(0).toUpperCase();
      alert('Perfil actualizado exitosamente');
    }
  }

  configureNotifications(): void {
    const settings = `
Notificaciones activas:
${this.notifications.taskCompleted ? '✅' : '❌'} Tareas completadas
${this.notifications.taskOverdue ? '✅' : '❌'} Tareas vencidas
${this.notifications.dailySummary ? '✅' : '❌'} Resumen diario
${this.notifications.weeklyReport ? '✅' : '❌'} Reporte semanal

¿Deseas cambiar alguna configuración?
    `.trim();

    if (confirm(settings)) {
      // Aquí se podría abrir un modal más complejo
      this.notifications.taskCompleted = confirm('¿Activar notificaciones de tareas completadas?');
      this.notifications.taskOverdue = confirm('¿Activar notificaciones de tareas vencidas?');
      this.notifications.dailySummary = confirm('¿Activar resumen diario?');
      this.notifications.weeklyReport = confirm('¿Activar reporte semanal?');

      alert('Configuración de notificaciones actualizada');
    }
  }
}
