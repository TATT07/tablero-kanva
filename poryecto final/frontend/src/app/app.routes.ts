import { Routes } from '@angular/router';
import { Dashboard } from './components/dashboard/dashboard';
import { KanbanBoard } from './components/kanban-board/kanban-board';
import { AnalyticsComponent } from './components/analytics/analytics';
import { Settings } from './components/settings/settings';
import { LoginComponent } from './components/login/login';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'dashboard', component: Dashboard, canActivate: [AuthGuard] },
  { path: 'board', component: KanbanBoard, canActivate: [AuthGuard] },
  { path: 'analytics', component: AnalyticsComponent, canActivate: [AuthGuard] },
  { path: 'settings', component: Settings, canActivate: [AuthGuard] },
  { path: '', redirectTo: '/login', pathMatch: 'full' }
];
