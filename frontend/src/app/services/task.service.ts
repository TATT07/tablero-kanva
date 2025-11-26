import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Task, CreateTaskRequest, UpdateTaskRequest, MoveTaskRequest, TaskFilters, TaskHistory } from '../models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private readonly API_URL = `${environment.apiUrl}/tasks`;

  constructor(private http: HttpClient) { }

  getTasks(filters?: TaskFilters): Observable<Task[]> {
    let params = new HttpParams();

    if (filters) {
      if (filters.search) params = params.set('search', filters.search);
      if (filters.status !== undefined) params = params.set('status', filters.status.toString());
      if (filters.priority !== undefined) params = params.set('priority', filters.priority.toString());
      if (filters.dueDateFrom) params = params.set('dueDateFrom', filters.dueDateFrom);
      if (filters.dueDateTo) params = params.set('dueDateTo', filters.dueDateTo);
      if (filters.sortBy) params = params.set('sortBy', filters.sortBy);
      if (filters.sortDescending !== undefined) params = params.set('sortDescending', filters.sortDescending.toString());
    }

    return this.http.get<Task[]>(this.API_URL, { params });
  }

  getTaskById(id: number): Observable<Task> {
    return this.http.get<Task>(`${this.API_URL}/${id}`);
  }

  createTask(task: CreateTaskRequest): Observable<Task> {
    return this.http.post<Task>(this.API_URL, task);
  }

  updateTask(id: number, task: UpdateTaskRequest): Observable<Task> {
    return this.http.put<Task>(`${this.API_URL}/${id}`, task);
  }

  deleteTask(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`);
  }

  moveTask(id: number, moveRequest: MoveTaskRequest): Observable<Task> {
    return this.http.put<Task>(`${this.API_URL}/${id}/move`, moveRequest);
  }

  getTaskHistory(id: number): Observable<TaskHistory[]> {
    return this.http.get<TaskHistory[]>(`${this.API_URL}/${id}/history`);
  }

  exportExcel(): Observable<Blob> {
    return this.http.get(`${this.API_URL}/export/excel`, { responseType: 'blob' });
  }

  exportPdf(): Observable<Blob> {
    return this.http.get(`${this.API_URL}/export/pdf`, { responseType: 'blob' });
  }
}