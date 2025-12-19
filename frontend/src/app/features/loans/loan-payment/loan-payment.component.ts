import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { LoansService } from '../../../core/services/loans.service';
import { ToastService } from '../../../core/services/toast.service';
import { LoanDto } from '../../../core/models/loan.model';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
  ],
  templateUrl: './loan-payment.component.html',
  styleUrls: ['./loan-payment.component.scss'],
})
export class LoanPaymentComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly loansService = inject(LoansService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);

  loanId = '';
  loan?: LoanDto;

  form = this.fb.group({
    amount: [null as number | null, [Validators.required, Validators.min(1)]],
  });

  ngOnInit(): void {
    this.loanId = this.route.snapshot.paramMap.get('id') ?? '';

    this.loansService.getById(this.loanId).subscribe({
      next: (loan) => (this.loan = loan),
      error: () => this.toast.error('Failed to load loan details'),
    });
  }

  submit(): void {
    if (this.form.invalid) return;

    const amount = Number(this.form.value.amount);

    this.loansService.pay(this.loanId, { amount }).subscribe({
      next: () => {
        this.toast.success('Payment applied successfully');
        this.router.navigateByUrl('/loans');
      },
      error: () => this.toast.error('Failed to apply payment'),
    });
  }
}
