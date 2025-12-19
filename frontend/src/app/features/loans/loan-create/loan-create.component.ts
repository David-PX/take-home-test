import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CustomersService } from '../../../core/services/customers.service';
import { LoansService } from '../../../core/services/loans.service';
import { ToastService } from '../../../core/services/toast.service';
import { CustomerDto } from '../../../core/models/customer.model';
import { map, startWith } from 'rxjs/operators';
import { Observable } from 'rxjs';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
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
    MatSelectModule,
    MatInputModule,
    MatButtonModule,
  ],
  templateUrl: './loan-create.component.html',
  styleUrls: ['./loan-create.component.scss'],
})
export class LoanCreateComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly customersService = inject(CustomersService);
  private readonly loansService = inject(LoansService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);

  customers: CustomerDto[] = [];
  filteredCustomers$!: Observable<CustomerDto[]>;

  form = this.fb.group({
    customerSearch: [''],
    customerId: ['', Validators.required],
    amount: [null as number | null, [Validators.required, Validators.min(1)]],
  });

  ngOnInit(): void {
    this.customersService.getAll().subscribe({
      next: (data) => {
        this.customers = data;

        this.filteredCustomers$ =
          this.form.controls.customerSearch.valueChanges.pipe(
            startWith(''),
            map((value) => (value ?? '').toLowerCase().trim()),
            map((term) =>
              this.customers.filter((c) =>
                `${c.fullName} ${c.email}`.toLowerCase().includes(term)
              )
            )
          );
      },
      error: () => this.toast.error('Failed to load customers'),
    });
  }

  submit(): void {
    if (this.form.invalid) return;

    const customerId = this.form.value.customerId!;
    const amount = Number(this.form.value.amount);

    this.loansService.create({ customerId, amount }).subscribe({
      next: () => {
        this.toast.success('Loan created successfully');
        this.router.navigateByUrl('/loans');
      },
      error: () => this.toast.error('Failed to create loan'),
    });
  }
}
