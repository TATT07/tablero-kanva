import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import {
  Task,
  TaskStatus,
  TaskPriority,
  CreateTaskRequest,
  UpdateTaskRequest
} from '../../models/task.model';

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
  templateUrl: './task-dialog.component.html',
  styleUrls: ['./task-dialog.component.scss'],
})
export class TaskDialogComponent implements OnInit {
  taskForm!: FormGroup;
  isEdit = false;
  isReadOnly = false;

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
    private dialogRef: MatDialogRef<TaskDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { task: Task | null, readOnly?: boolean }
  ) {}

  ngOnInit(): void {
    this.isEdit = !!this.data?.task;
    this.isReadOnly = this.data?.readOnly || false;

    // Parse tags from string to array if needed
    const tagsValue = this.data?.task?.tags
      ? (typeof this.data.task.tags === 'string'
          ? this.data.task.tags.split(',').map(tag => tag.trim()).filter(tag => tag)
          : this.data.task.tags)
      : [];

    this.taskForm = this.fb.group({
      title: [this.data?.task?.title || '', [Validators.required, Validators.maxLength(100)]],
      description: [this.data?.task?.description || '', [Validators.maxLength(500)]],
      status: [this.data?.task?.status ?? TaskStatus.ToDo, Validators.required],
      position: [this.data?.task?.position ?? 0, [Validators.required, Validators.min(0)]],
      priority: [this.data?.task?.priority ?? TaskPriority.Medium, Validators.required],
      dueDate: [this.data?.task?.dueDate || ''],
      tags: [tagsValue],
      comments: [this.data?.task?.comments || ''],
      assignedToUserId: [this.data?.task?.assignedToUserId || null],
    });

    // Disable form if read-only
    if (this.isReadOnly) {
      this.taskForm.disable();
    }
  }

  // Etiquetas según el enum numérico
  getStatusLabel(status: TaskStatus): string {
    switch (status) {
      case TaskStatus.ToDo: return 'Por hacer';
      case TaskStatus.InProgress: return 'En progreso';
      case TaskStatus.Done: return 'Completada';
      default: return '';
    }
  }

  // Iconos según enum
  getStatusIcon(status: TaskStatus): string {
    switch (status) {
      case TaskStatus.ToDo: return 'schedule';
      case TaskStatus.InProgress: return 'play_arrow';
      case TaskStatus.Done: return 'check_circle';
      default: return '';
    }
  }

  onSubmit(): void {
    if (this.taskForm.invalid) return;

    const f = this.taskForm.value;

    const req: any = {
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
      req.dueDate = new Date(f.dueDate).toISOString();
    }

    this.dialogRef.close(req);
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }
}
