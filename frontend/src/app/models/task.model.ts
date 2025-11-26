export enum TaskStatus {
  ToDo = 0,
  InProgress = 1,
  Done = 2
}

export enum TaskPriority {
  Low = 0,
  Medium = 1,
  High = 2,
  Urgent = 3
}

export interface Task {
  id: number;
  title: string;
  description?: string;
  status: TaskStatus;
  position: number;
  userId: number;
  userName: string;
  assignedToUserId?: number;
  assignedToUserName?: string;
  dueDate?: string;
  priority: TaskPriority;
  tags?: string;
  comments?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  status: TaskStatus;
  position: number;
  assignedToUserId?: number;
  dueDate?: string;
  priority: TaskPriority;
  tags?: string;
  comments?: string;
}

export interface UpdateTaskRequest {
  title: string;
  description?: string;
  status: TaskStatus;
  position: number;
  assignedToUserId?: number;
  dueDate?: string;
  priority: TaskPriority;
  tags?: string;
  comments?: string;
}

export interface MoveTaskRequest {
  status: TaskStatus;
  position: number;
}

export interface TaskFilters {
  search?: string;
  status?: TaskStatus;
  priority?: TaskPriority;
  dueDateFrom?: string;
  dueDateTo?: string;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface TaskHistory {
  id: number;
  taskId: number;
  userId: number;
  userName: string;
  action: string;
  oldValue?: string;
  newValue?: string;
  details?: string;
  createdAt: string;
}
