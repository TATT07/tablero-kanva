import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { TaskService } from '../../services/task.service';
import { Task, TaskStatus } from '../../models';

@Component({
  selector: 'app-analytics',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './analytics.html',
  styleUrl: './analytics.scss'
})
export class AnalyticsComponent implements OnInit {
  tasks: Task[] = [];
  completionRate = 0;
  totalTasks = 0;
  activeTasks = 0;
  completedTasks = 0;
  avgTasksPerDay = 0;
  productivityScore = 0;

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.loadAnalytics();
  }

  loadAnalytics(): void {
    this.taskService.getTasks().subscribe({
      next: (tasks: Task[]) => {
        this.tasks = tasks;
        this.calculateMetrics();
      },
      error: (error) => {
        console.error('Error loading analytics:', error);
      }
    });
  }

  calculateMetrics(): void {
    this.totalTasks = this.tasks.length;
    this.completedTasks = this.tasks.filter(t => t.status === TaskStatus.Done).length;
    this.activeTasks = this.tasks.filter(t => t.status !== TaskStatus.Done).length;

    // Tasa de completación
    this.completionRate = this.totalTasks > 0
      ? Math.round((this.completedTasks / this.totalTasks) * 100)
      : 0;

    // Productividad (tareas completadas / tareas activas)
    this.productivityScore = this.activeTasks > 0
      ? Math.round((this.completedTasks / (this.completedTasks + this.activeTasks)) * 100)
      : 0;

    // Promedio de tareas por día (simplificado)
    this.avgTasksPerDay = this.totalTasks > 0 ? Math.round(this.totalTasks / 7) : 0;
  }

  getTodoPercentage(): number {
    return this.totalTasks > 0 ? (this.tasks.filter(t => t.status === TaskStatus.ToDo).length / this.totalTasks) * 100 : 0;
  }

  getInProgressPercentage(): number {
    return this.totalTasks > 0 ? (this.tasks.filter(t => t.status === TaskStatus.InProgress).length / this.totalTasks) * 100 : 0;
  }

  getDonePercentage(): number {
    return this.totalTasks > 0 ? (this.tasks.filter(t => t.status === TaskStatus.Done).length / this.totalTasks) * 100 : 0;
  }

  getTodoCount(): number {
    return this.tasks.filter(t => t.status === TaskStatus.ToDo).length;
  }

  getInProgressCount(): number {
    return this.tasks.filter(t => t.status === TaskStatus.InProgress).length;
  }

  getDoneCount(): number {
    return this.tasks.filter(t => t.status === TaskStatus.Done).length;
  }
}
