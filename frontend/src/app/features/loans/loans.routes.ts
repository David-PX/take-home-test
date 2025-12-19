import { Routes } from '@angular/router';

export const LOANS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./loans-list/loans-list.component').then(
        (m) => m.LoansListComponent
      ),
  },
  {
    path: 'new',
    loadComponent: () =>
      import('./loan-create/loan-create.component').then(
        (m) => m.LoanCreateComponent
      ),
  },
  {
    path: ':id/pay',
    loadComponent: () =>
      import('./loan-payment/loan-payment.component').then(
        (m) => m.LoanPaymentComponent
      ),
  },
];
