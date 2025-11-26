import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';

import { BaseChartDirective } from 'ng2-charts';
import { Chart, ChartData, ChartOptions, Chart as ChartJS, registerables } from 'chart.js';

// Registrar todos los componentes de Chart.js
ChartJS.register(...registerables);


import { AuthService } from '../../services/auth.service';
import { TaskService } from '../../services/task.service';
import { User, Task, TaskStatus, UpdateTaskRequest, CreateTaskRequest } from '../../models';
import { TaskDialogComponent } from '../task-dialog/task-dialog.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatDividerModule,
    MatSnackBarModule,
    MatSidenavModule,
    MatListModule,
    MatDialogModule,
    BaseChartDirective
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit, AfterViewInit {

  currentUser: User | null = null;

  tasks: Task[] = [];
  totalTasks = 0;
  todoCount = 0;
  inProgressCount = 0;
  doneCount = 0;

  public doughnutChartOptions: ChartOptions<'doughnut'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false },
      tooltip: {
        callbacks: {
          label: (ctx) => {
            const label = ctx.label || '';
            const value = ctx.parsed;
            const percent = this.totalTasks > 0 ? ((value / this.totalTasks) * 100).toFixed(1) : '0';
            return `${label}: ${value} (${percent}%)`;
          }
        }
      }
    },
    cutout: '70%',
    animation: { animateScale: true, animateRotate: true }
  };

  public doughnutChartData: ChartData<'doughnut'> = {
    labels: ['Por Hacer', 'En Progreso', 'Completadas'],
    datasets: [{
      data: [0, 0, 0],
      backgroundColor: ['#FACC15', '#3B82F6', '#22C55E'],
      hoverBackgroundColor: ['#EAB308', '#2563EB', '#16A34A'],
      borderWidth: 2,
      borderColor: '#ffffff'
    }]
  };

  constructor(
    private authService: AuthService,
    private taskService: TaskService,
    private router: Router,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  public ngOnInit(): void {
    this.authService.currentUser$.subscribe((user) => {
      this.currentUser = user;
    });

    this.loadTaskStats();
  }

  public ngAfterViewInit(): void {
    console.log('[CHART] View initialized, Chart.js should be ready');
    // Forzar una actualización inicial del gráfico después de que la vista esté lista
    setTimeout(() => {
      this.updateChartData();
    }, 100);
  }

  public loadTaskStats(): void {
    console.log('[DASHBOARD] Loading task statistics...');
    this.taskService.getTasks().subscribe({
      next: (tasks: Task[]) => {
        console.log('[DASHBOARD] Tasks loaded:', tasks.length);
        this.tasks = tasks;
        this.totalTasks = tasks.length;

        this.todoCount = tasks.filter((t) => t.status === TaskStatus.ToDo).length;
        this.inProgressCount = tasks.filter((t) => t.status === TaskStatus.InProgress).length;
        this.doneCount = tasks.filter((t) => t.status === TaskStatus.Done).length;

        console.log('[DASHBOARD] Counts calculated:', {
          total: this.totalTasks,
          todo: this.todoCount,
          inProgress: this.inProgressCount,
          done: this.doneCount
        });

        // Actualizar el gráfico después de un pequeño delay para asegurar que el DOM esté listo
        setTimeout(() => {
          this.updateChartData();
        }, 50);
      },
      error: (error) => {
        console.error('[DASHBOARD] Error loading tasks:', error);
        this.snackBar.open('Error al cargar estadísticas', 'Cerrar', { duration: 3000 });
      }
    });
  }

  private updateChartData(): void {
    console.log('[CHART] Updating chart data:', {
      todo: this.todoCount,
      inProgress: this.inProgressCount,
      done: this.doneCount,
      total: this.totalTasks
    });

    // Forzar la actualización del gráfico creando un nuevo array
    this.doughnutChartData = {
      ...this.doughnutChartData,
      datasets: [{
        ...this.doughnutChartData.datasets[0],
        data: [this.todoCount, this.inProgressCount, this.doneCount]
      }]
    };

    console.log('[CHART] Chart data updated successfully');
  }

  public logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  public exportTasksToPdf(): void {
    this.taskService.exportPdf().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'Tareas_KanbanFlow.pdf';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
        this.snackBar.open('Archivo PDF descargado correctamente', 'Cerrar', { duration: 3000 });
      },
      error: () => {
        this.snackBar.open('Error al exportar PDF', 'Cerrar', { duration: 3000 });
      }
    });
  }

  public exportTasksToExcel(): void {
    this.taskService.exportExcel().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'Tareas_KanbanFlow.xlsx';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
        this.snackBar.open('Archivo Excel descargado correctamente', 'Cerrar', { duration: 3000 });
      },
      error: () => {
        this.snackBar.open('Error al exportar Excel', 'Cerrar', { duration: 3000 });
      }
    });
  }

  public changeTaskStatus(task: Task, event: any): void {
    const newStatus = parseInt(event.target.value) as TaskStatus;

    const request: UpdateTaskRequest = {
      title: task.title,
      description: task.description,
      status: newStatus,
      position: task.position,
      priority: task.priority
    };

    this.taskService.updateTask(task.id, request).subscribe({
      next: () => {
        this.loadTaskStats();
      },
      error: () => {
        this.snackBar.open('Error al actualizar la tarea', 'Cerrar', { duration: 3000 });
      }
    });
  }

  public createNewTask(): void {
    const dialogRef = this.dialog.open(TaskDialogComponent, {
      width: '580px',
      panelClass: 'task-dialog-panel',
      data: { task: null }
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) return;

      const tasksInStatus = this.tasks.filter((t) => t.status === result.status);
      const nextPosition = tasksInStatus.length > 0 ? Math.max(...tasksInStatus.map((t) => t.position)) + 1 : 1;

      const request: CreateTaskRequest = {
        ...result,
        position: nextPosition
      };

      this.taskService.createTask(request).subscribe({
        next: () => {
          this.loadTaskStats();
        },
        error: () => {
          this.snackBar.open('Error al crear tarea', 'Cerrar', { duration: 3000 });
        }
      });
    });
  }

  public deleteTask(task: Task): void {
    if (!confirm('¿Seguro que deseas eliminar esta tarea?')) return;

    this.taskService.deleteTask(task.id).subscribe({
      next: () => {
        this.loadTaskStats();
      },
      error: () => {
        this.snackBar.open('Error al eliminar tarea', 'Cerrar', { duration: 3000 });
      }
    });
  }
}
