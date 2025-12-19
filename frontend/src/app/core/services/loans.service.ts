import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import {
  CreateLoanRequestDto,
  LoanDto,
  PaymentRequestDto,
} from '../models/loan.model';

@Injectable({ providedIn: 'root' })
export class LoansService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/loans`;

  getAll(): Observable<LoanDto[]> {
    return this.http.get<LoanDto[]>(this.baseUrl);
  }

  create(payload: CreateLoanRequestDto): Observable<LoanDto> {
    return this.http.post<LoanDto>(this.baseUrl, payload);
  }

  pay(loanId: string, payload: PaymentRequestDto): Observable<LoanDto> {
    return this.http.post<LoanDto>(
      `${this.baseUrl}/${loanId}/payment`,
      payload
    );
  }

  getById(loanId: string): Observable<LoanDto> {
    return this.http.get<LoanDto>(`${this.baseUrl}/${loanId}`);
  }
}
