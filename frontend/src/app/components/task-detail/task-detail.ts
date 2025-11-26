import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { Task } from '../../models/task.model';

@Component({
  selector: 'app-task-detail',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './task-detail.html',
  styleUrl: './task-detail.scss'
})
export class TaskDetail implements OnInit {
  task: Task | null = null;

  constructor(
    private dialogRef: MatDialogRef<TaskDetail>,
    @Inject(MAT_DIALOG_DATA) public data: { task: Task }
  ) {}

  ngOnInit(): void {
    this.task = this.data.task;
  }

  getStatusLabel(status: number): string {
    switch (status) {
      case 0: return 'Por hacer';
      case 1: return 'En progreso';
      case 2: return 'Completada';
      default: return 'Desconocido';
    }
  }

  getPriorityLabel(priority: number): string {
    switch (priority) {
      case 0: return 'Baja';
      case 1: return 'Media';
      case 2: return 'Alta';
      case 3: return 'Urgente';
      default: return 'Media';
    }
  }

  onEdit(): void {
    this.dialogRef.close({ action: 'edit', task: this.task });
  }

  onClose(): void {
    this.dialogRef.close({ action: 'close' });
  }
}
