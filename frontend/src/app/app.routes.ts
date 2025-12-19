import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'loans' },

  {
    path: 'auth',
    loadChildren: () =>
      import('./features/auth/auth.routes').then((m) => m.AUTH_ROUTES),
  },

  {
    path: 'loans',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./features/loans/loans.routes').then((m) => m.LOANS_ROUTES),
  },

  { path: '**', redirectTo: 'auth/login' },
];
