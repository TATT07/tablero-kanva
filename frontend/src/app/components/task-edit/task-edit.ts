import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { Task, TaskStatus, TaskPriority, UpdateTaskRequest } from '../../models/task.model';
import { TaskService } from '../../services/task.service';

@Component({
  selector: 'app-task-edit',
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
  templateUrl: './task-edit.html',
  styleUrl: './task-edit.scss'
})
export class TaskEdit implements OnInit {
  taskForm!: FormGroup;
  task: Task | null = null;
  isLoading = false;

  taskStatuses = [TaskStatus.ToDo, TaskStatus.InProgress, TaskStatus.Done];

  // Predefined tags for multi-select
  availableTags = [
    'urgente',
    'importante',
    'trabajo',
    'personal',
    'proyecto',
    'reunión',
    'desarrollo',
    'diseño',
    'testing',
    'bug',
    'feature',
    'mejora',
    'documentación',
    'investigación'
  ];

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<TaskEdit>,
    private taskService: TaskService,
    @Inject(MAT_DIALOG_DATA) public data: { taskId: number }
  ) {}

  ngOnInit(): void {
    this.loadTask();
  }

  loadTask(): void {
    this.isLoading = true;
    this.taskService.getTaskById(this.data.taskId).subscribe({
      next: (task) => {
        this.task = task;
        this.initializeForm();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading task:', error);
        this.isLoading = false;
        this.onCancel();
      }
    });
  }

  initializeForm(): void {
    if (!this.task) return;

    // Parse tags from string to array if needed
    const tagsValue = this.task.tags
      ? (typeof this.task.tags === 'string'
          ? this.task.tags.split(',').map(tag => tag.trim()).filter(tag => tag)
          : this.task.tags)
      : [];

    this.taskForm = this.fb.group({
      title: [this.task.title, [Validators.required, Validators.maxLength(100)]],
      description: [this.task.description || '', [Validators.maxLength(500)]],
      status: [this.task.status, Validators.required],
      position: [this.task.position, [Validators.required, Validators.min(0)]],
      priority: [this.task.priority, Validators.required],
      dueDate: [this.task.dueDate ? new Date(this.task.dueDate).toISOString().split('T')[0] : ''],
      tags: [tagsValue],
      comments: [this.task.comments || ''],
      assignedToUserId: [this.task.assignedToUserId || null],
    });
  }

  getStatusLabel(status: TaskStatus): string {
    switch (status) {
      case TaskStatus.ToDo: return 'Por hacer';
      case TaskStatus.InProgress: return 'En progreso';
      case TaskStatus.Done: return 'Completada';
      default: return '';
    }
  }

  onSubmit(): void {
    if (this.taskForm.invalid || !this.task) return;

    const f = this.taskForm.value;
    const updateRequest: UpdateTaskRequest = {
      title: f.title,
      description: f.description,
      status: f.status,
      position: f.position,
      priority: f.priority,
      tags: Array.isArray(f.tags) ? f.tags.join(', ') : f.tags,
      comments: f.comments,
      assignedToUserId: f.assignedToUserId,
    };

    // Only add dueDate if it has a value
    if (f.dueDate) {
      updateRequest.dueDate = new Date(f.dueDate).toISOString();
    }

    this.taskService.updateTask(this.task.id, updateRequest).subscribe({
      next: (updatedTask) => {
        this.dialogRef.close({ action: 'updated', task: updatedTask });
      },
      error: (error) => {
        console.error('Error updating task:', error);
        // You could show a snackbar here
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close({ action: 'cancel' });
  }
}
