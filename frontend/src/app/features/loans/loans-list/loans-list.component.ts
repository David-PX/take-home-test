import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { RouterModule } from '@angular/router';
import { LoansService } from '../../../core/services/loans.service';
import { LoanDto } from '../../../core/models/loan.model';

@Component({
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, RouterModule],
  templateUrl: './loans-list.component.html',
  styleUrls: ['./loans-list.component.scss'],
})
export class LoansListComponent implements OnInit {
  private readonly loansService = inject(LoansService);

  displayedColumns: string[] = [
    'loanAmount',
    'currentBalance',
    'applicant',
    'status',
    'actions',
  ];

  loans: LoanDto[] = [];

  ngOnInit(): void {
    this.loansService.getAll().subscribe({
      next: (data) => {
        this.loans = data;
      },
      error: (err) => console.error('Error loading loans', err),
    });
  }
}
