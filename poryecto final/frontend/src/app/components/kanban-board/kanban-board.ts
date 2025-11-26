import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  CdkDragDrop,
  DragDropModule,
  moveItemInArray,
  transferArrayItem
} from '@angular/cdk/drag-drop';

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';

import { Subject, takeUntil } from 'rxjs';
import {
  Task,
  TaskStatus,
  CreateTaskRequest,
  UpdateTaskRequest,
  MoveTaskRequest
} from '../../models/task.model';

import { TaskService } from '../../services/task.service';
import { TaskDialogComponent } from '../task-dialog/task-dialog.component';
import { TaskDetail } from '../task-detail/task-detail';
import { TaskEdit } from '../task-edit/task-edit';
import { ConfirmDialog, ConfirmDialogData } from '../confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-kanban-board',
  standalone: true,
  imports: [
    CommonModule,
    DragDropModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatSnackBarModule
  ],
  templateUrl: './kanban-board.html',
  styleUrl: './kanban-board.scss',
})
export class KanbanBoard implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  TaskStatus = TaskStatus;

  todoTasks: Task[] = [];
  inProgressTasks: Task[] = [];
  doneTasks: Task[] = [];

  todoCount = 0;
  inProgressCount = 0;
  doneCount = 0;

  constructor(
    private taskService: TaskService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadTasks(): void {
    this.taskService.getTasks()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (tasks) => {
          this.distributeTasks(tasks);
          this.todoCount = this.todoTasks.length;
          this.inProgressCount = this.inProgressTasks.length;
          this.doneCount = this.doneTasks.length;
        },
        error: () => {
          this.snackBar.open('Error al cargar tareas', 'Cerrar', { duration: 3000 });
        }
      });
  }

  private distributeTasks(tasks: Task[]): void {
    this.todoTasks = tasks
      .filter(t => t.status === TaskStatus.ToDo)
      .sort((a, b) => a.position - b.position);

    this.inProgressTasks = tasks
      .filter(t => t.status === TaskStatus.InProgress)
      .sort((a, b) => a.position - b.position);

    this.doneTasks = tasks
      .filter(t => t.status === TaskStatus.Done)
      .sort((a, b) => a.position - b.position);
  }

  // ---------------------------------------------------------
  //                DRAG & DROP COMPLETAMENTE CORREGIDO
  // ---------------------------------------------------------
  onDrop(event: CdkDragDrop<Task[]>): void {
    const task = event.item.data as Task;
    const newStatus = this.getStatusFromContainerId(event.container.id);
    const newPosition = event.currentIndex;

    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
      this.updateTaskPositions(event.container.data);
    } else {
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );

      const moveRequest: MoveTaskRequest = {
        status: newStatus,
        position: newPosition
      };

      this.taskService.moveTask(task.id, moveRequest).subscribe({
        next: () => {
          this.updateTaskPositions(event.container.data);
          this.loadTasks();
        },
        error: () => {
          this.snackBar.open('Error al mover tarea', 'Cerrar', { duration: 2500 });
          this.loadTasks();
        }
      });
    }
  }

  private getStatusFromContainerId(containerId: string): TaskStatus {
    switch (containerId) {
      case 'todo-list': return TaskStatus.ToDo;
      case 'inprogress-list': return TaskStatus.InProgress;
      case 'done-list': return TaskStatus.Done;
      default: return TaskStatus.ToDo;
    }
  }

  private updateTaskPositions(tasks: Task[]): void {
    tasks.forEach((task, index) => task.position = index);
  }

  // ---------------------------------------------------------
  //                     CREAR TAREA
  // ---------------------------------------------------------
  addTask(): void {
    const dialogRef = this.dialog.open(TaskDialogComponent, {
      width: '580px',
      panelClass: 'task-dialog-panel',
      data: { task: null }
    });

    dialogRef.afterClosed().subscribe((result: CreateTaskRequest | null) => {
      if (!result) return;

      this.taskService.createTask(result).subscribe({
        next: () => {
          this.loadTasks();
          this.snackBar.open('Tarea creada correctamente', 'Cerrar', { duration: 2500 });
        },
        error: () => {
          this.snackBar.open('Error al crear tarea', 'Cerrar', { duration: 2500 });
        }
      });
    });
  }

  // ---------------------------------------------------------
  //                     VER DETALLES DE TAREA
  // ---------------------------------------------------------
  viewTask(task: Task): void {
    const dialogRef = this.dialog.open(TaskDetail, {
      width: '600px',
      panelClass: 'task-dialog-panel',
      data: { task }
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result?.action === 'edit') {
        // User clicked edit from detail view
        this.editTask(task);
      }
    });
  }

  // ---------------------------------------------------------
  //                     EDITAR TAREA
  // ---------------------------------------------------------
  editTask(task: Task): void {
    const dialogRef = this.dialog.open(TaskEdit, {
      width: '580px',
      panelClass: 'task-dialog-panel',
      data: { taskId: task.id }
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result?.action === 'updated') {
        this.loadTasks();
        this.snackBar.open('Tarea actualizada correctamente', 'Cerrar', { duration: 2500 });
      }
    });
  }

  // ---------------------------------------------------------
//                     BORRAR TAREA
// ---------------------------------------------------------
  deleteTask(task: Task): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      width: '400px',
      panelClass: 'task-dialog-panel',
      data: { taskTitle: task.title } as ConfirmDialogData
    });

    dialogRef.afterClosed().subscribe((confirmed: boolean) => {
      if (!confirmed) return;

      this.taskService.deleteTask(task.id).subscribe({
        next: () => {
          this.loadTasks();
          this.snackBar.open('Tarea eliminada', 'Cerrar', { duration: 2500 });
        },
        error: () => {
          this.snackBar.open('Error al eliminar tarea', 'Cerrar', { duration: 2500 });
        }
      });
    });
  }

  getConnectedLists(): string[] {
    return ['todo-list', 'inprogress-list', 'done-list'];
  }

  trackByTaskId(index: number, task: Task): number {
    return task.id;
  }
}
