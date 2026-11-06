import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login-page').then((module) => module.LoginPage),
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./layout/dashboard-layout/dashboard-layout').then(
        (module) => module.DashboardLayout,
      ),
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard-page').then(
            (module) => module.DashboardPage,
          ),
      },
    ],
  },
  {
    path: 'not-found',
    loadComponent: () =>
      import('./features/not-found/not-found-page').then(
        (module) => module.NotFoundPage,
      ),
  },
  { path: '**', redirectTo: 'not-found' },
];
