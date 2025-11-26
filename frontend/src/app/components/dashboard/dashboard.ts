import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
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
import { Chart, ChartConfiguration, ChartData, ChartType, ChartOptions } from 'chart.js';
import { DoughnutController, ArcElement, Tooltip, Legend } from 'chart.js';

// Register Chart.js components
Chart.register(DoughnutController, ArcElement, Tooltip, Legend);

import { AuthService } from '../../services/auth.service';
import { TaskService } from '../../services/task.service';
import { User, Task, TaskStatus, UpdateTaskRequest, CreateTaskRequest } from '../../models';
import { TaskDialogComponent } from '../task-dialog/task-dialog.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
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
  styleUrl: './dashboard.scss',
})
export class Dashboard implements OnInit {
  currentUser: User | null = null;

  tasks: Task[] = [];
  totalTasks = 0;
  todoCount = 0;
  inProgressCount = 0;
  doneCount = 0;

  // Chart configuration
  public doughnutChartOptions: ChartOptions<'doughnut'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: false // We'll create custom legend
      },
      tooltip: {
        callbacks: {
          label: (context) => {
            const label = context.label || '';
            const value = context.parsed;
            const percentage = ((value / this.totalTasks) * 100).toFixed(1);
            return `${label}: ${value} (${percentage}%)`;
          }
        }
      }
    },
    cutout: '70%', // Creates donut effect
    animation: {
      animateScale: true,
      animateRotate: true
    }
  };

  public doughnutChartData: ChartData<'doughnut'> = {
    labels: ['Por Hacer', 'En Progreso', 'Completadas'],
    datasets: [{
      data: [0, 0, 0],
      backgroundColor: ['#f4a300', '#1e6cff', '#1eca8c'],
      hoverBackgroundColor: ['#e69300', '#1a5ae6', '#1ab380'],
      borderWidth: 3,
      borderColor: '#0f1217'
    }]
  };

  constructor(
    private authService: AuthService,
    private taskService: TaskService,
    private router: Router,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });

    this.loadTaskStats();
  }

  loadTaskStats(): void {
    this.taskService.getTasks().subscribe({
      next: (tasks: Task[]) => {
        this.tasks = tasks;
        this.totalTasks = tasks.length;
        this.todoCount = tasks.filter(t => t.status === TaskStatus.ToDo).length;
        this.inProgressCount = tasks.filter(t => t.status === TaskStatus.InProgress).length;
        this.doneCount = tasks.filter(t => t.status === TaskStatus.Done).length;

        // Update chart data
        this.updateChartData();
      },
      error: () => {
        this.snackBar.open('Error al cargar estadísticas', 'Cerrar', {
          duration: 3000,
        });
      },
    });
  }

  private updateChartData(): void {
    this.doughnutChartData = {
      labels: ['Por Hacer', 'En Progreso', 'Completadas'],
      datasets: [{
        data: [this.todoCount, this.inProgressCount, this.doneCount],
        backgroundColor: ['#f4a300', '#1e6cff', '#1eca8c'],
        hoverBackgroundColor: ['#e69300', '#1a5ae6', '#1ab380'],
        borderWidth: 3,
        borderColor: '#0f1217'
      }]
    };
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  exportTasksToPdf(): void {
    this.taskService.exportPdf().subscribe({
      next: (blob) => {
        import('file-saver').then(({ saveAs }) => {
          saveAs(blob, 'Tareas_KanbanFlow.pdf');
          this.snackBar.open('Archivo PDF descargado correctamente', 'Cerrar', { duration: 3000 });
        });
      },
      error: () => {
        this.snackBar.open('Error al exportar PDF', 'Cerrar', { duration: 3000 });
      }
    });
  }

  exportTasksToExcel(): void {
    this.taskService.exportExcel().subscribe({
      next: (blob) => {
        import('file-saver').then(({ saveAs }) => {
          saveAs(blob, 'Tareas_KanbanFlow.xlsx');
          this.snackBar.open('Archivo Excel descargado correctamente', 'Cerrar', { duration: 3000 });
        });
      },
      error: () => {
        this.snackBar.open('Error al exportar Excel', 'Cerrar', { duration: 3000 });
      }
    });
  }

  changeTaskStatus(task: Task, event: any): void {
    const newStatus = parseInt(event.target.value) as TaskStatus;
    const updateRequest: UpdateTaskRequest = {
      title: task.title,
      description: task.description,
      status: newStatus,
      position: task.position,
      priority: task.priority
    };

    this.taskService.updateTask(task.id, updateRequest).subscribe({
      next: () => {
        this.loadTaskStats();
        this.snackBar.open('Estado de tarea actualizado', 'Cerrar', { duration: 2000 });
      },
      error: () => {
        this.snackBar.open('Error al actualizar tarea', 'Cerrar', { duration: 3000 });
      }
    });
  }

  createNewTask(): void {
    const dialogRef = this.dialog.open(TaskDialogComponent, {
      width: '580px',
      panelClass: 'task-dialog-panel',
      data: { task: null }
    });

    dialogRef.afterClosed().subscribe((result: CreateTaskRequest | null) => {
      if (!result) return;

      // Calculate the next position for the selected status
      const tasksInStatus = this.tasks.filter(t => t.status === result.status);
      const maxPosition = tasksInStatus.length > 0 ? Math.max(...tasksInStatus.map(t => t.position)) : 0;

      // Create a new request object with calculated position
      const taskRequest: CreateTaskRequest = {
        title: result.title,
        description: result.description,
        status: result.status,
        position: maxPosition + 1,
        priority: result.priority,
        assignedToUserId: result.assignedToUserId,
        dueDate: result.dueDate,
        tags: result.tags,
        comments: result.comments
      };

      this.taskService.createTask(taskRequest).subscribe({
        next: () => {
          this.loadTaskStats();
          this.snackBar.open('Tarea creada correctamente', 'Cerrar', { duration: 2500 });
        },
        error: (error) => {
          console.error('Error creating task:', error);
          this.snackBar.open('Error al crear tarea', 'Cerrar', { duration: 2500 });
        }
      });
    });
  }

  deleteTask(task: Task): void {
    if (!confirm('¿Estás seguro que deseas eliminar esta tarea?')) return;

    this.taskService.deleteTask(task.id).subscribe({
      next: () => {
        this.loadTaskStats();
        this.snackBar.open('Tarea eliminada', 'Cerrar', { duration: 2000 });
      },
      error: () => {
        this.snackBar.open('Error al eliminar tarea', 'Cerrar', { duration: 3000 });
      }
    });
  }



  private downloadFile(blob: Blob, filename: string): void {
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
  }
}
