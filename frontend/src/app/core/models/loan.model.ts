import { CustomerDto } from './customer.model';

export type LoanStatus = 'active' | 'paid';

export interface LoanDto {
  id: string;
  originalAmount: number;
  currentBalance: number;
  status: LoanStatus;
  createdAtUtc: string;
  customer: CustomerDto;
}

export interface CreateLoanRequestDto {
  customerId: string;
  amount: number;
}

export interface PaymentRequestDto {
  amount: number;
}
