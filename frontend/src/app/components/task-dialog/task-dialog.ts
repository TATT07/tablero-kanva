import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { TaskService } from '../../services/task.service';
import { Task, TaskPriority } from '../../models/task.model';
import { TaskStatus } from '../../models/task.model';

@Component({
  selector: 'app-task-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './task-dialog.html',
  styleUrls: ['./task-dialog.scss']
})
export class TaskDialogComponent {
  taskForm: FormGroup;
  isEdit: boolean = false;
  taskStatuses = Object.values(TaskStatus);
  taskPriorities = Object.values(TaskPriority);

  constructor(
    private fb: FormBuilder,
    private taskService: TaskService,
    private dialogRef: MatDialogRef<TaskDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { task?: Task; status?: TaskStatus }
  ) {
    this.isEdit = !!data.task;
    this.taskForm = this.fb.group({
      title: [data.task?.title || '', [Validators.required, Validators.maxLength(100)]],
      description: [data.task?.description || '', Validators.maxLength(500)],
      status: [data.task?.status || data.status || TaskStatus.ToDo, Validators.required],
      position: [data.task?.position || 0, [Validators.required, Validators.min(0)]],
      priority: [data.task?.priority || TaskPriority.Medium, Validators.required]
    });
  }

  onSubmit(): void {
    if (this.taskForm.valid) {
      const formValue = this.taskForm.value;
      if (this.isEdit && this.data.task) {
        this.taskService.updateTask(this.data.task.id, formValue).subscribe({
          next: (updatedTask) => {
            this.dialogRef.close(updatedTask);
          },
          error: (error) => {
            console.error('Error updating task:', error);
          }
        });
      } else {
        this.taskService.createTask(formValue).subscribe({
          next: (newTask) => {
            this.dialogRef.close(newTask);
          },
          error: (error) => {
            console.error('Error creating task:', error);
          }
        });
      }
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  getStatusIcon(status: string): string {
    switch (status.toLowerCase()) {
      case 'todo':
        return 'schedule';
      case 'inprogress':
        return 'play_arrow';
      case 'done':
        return 'check_circle';
      default:
        return 'help';
    }
  }

  getStatusLabel(status: string): string {
    switch (status.toLowerCase()) {
      case 'todo':
        return 'Por Hacer';
      case 'inprogress':
        return 'En Progreso';
      case 'done':
        return 'Completada';
      default:
        return status;
    }
  }

  getPriorityIcon(priority: string): string {
    switch (priority.toLowerCase()) {
      case 'low':
        return 'arrow_downward';
      case 'medium':
        return 'remove';
      case 'high':
        return 'arrow_upward';
      case 'urgent':
        return 'priority_high';
      default:
        return 'help';
    }
  }

  getPriorityLabel(priority: string): string {
    switch (priority.toLowerCase()) {
      case 'low':
        return 'Baja';
      case 'medium':
        return 'Media';
      case 'high':
        return 'Alta';
      case 'urgent':
        return 'Urgente';
      default:
        return priority;
    }
  }
}