import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { Task, TaskStatus, CreateTaskRequest, UpdateTaskRequest } from '../models';

export interface TaskDialogData {
  mode: 'create' | 'edit';
  task?: Task;
  status?: TaskStatus;
}

@Component({
  selector: 'app-task-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule
  ],
  template: `
    <div class="task-dialog">
      <h2 mat-dialog-title>{{ data.mode === 'create' ? 'Create Task' : 'Edit Task' }}</h2>

      <mat-dialog-content>
        <form [formGroup]="taskForm">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Title</mat-label>
            <input matInput formControlName="title" placeholder="Enter task title">
          </mat-form-field>

          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Description</mat-label>
            <textarea matInput formControlName="description" placeholder="Enter task description" rows="3"></textarea>
          </mat-form-field>

          <mat-form-field appearance="outline" class="full-width" *ngIf="data.mode === 'create'">
            <mat-label>Status</mat-label>
            <mat-select formControlName="status">
              <mat-option [value]="0">To Do</mat-option>
              <mat-option [value]="1">In Progress</mat-option>
              <mat-option [value]="2">Done</mat-option>
            </mat-select>
          </mat-form-field>
        </form>
      </mat-dialog-content>

      <mat-dialog-actions align="end">
        <button mat-button mat-dialog-close>Cancel</button>
        <button mat-raised-button color="primary" [disabled]="taskForm.invalid" (click)="onSubmit()">
          {{ data.mode === 'create' ? 'Create' : 'Update' }}
        </button>
      </mat-dialog-actions>
    </div>
  `,
  styles: [`
    .task-dialog {
      min-width: 400px;
    }

    .full-width {
      width: 100%;
      margin-bottom: 16px;
    }

    mat-form-field {
      display: block;
    }
  `]
})
export class TaskDialogComponent {
  taskForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<TaskDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TaskDialogData
  ) {
    this.taskForm = this.fb.group({
      title: [data.task?.title || '', [Validators.required]],
      description: [data.task?.description || ''],
      status: [data.status || 0]
    });
  }

  onSubmit(): void {
    if (this.taskForm.valid) {
      this.dialogRef.close(this.taskForm.value);
    }
  }
}
