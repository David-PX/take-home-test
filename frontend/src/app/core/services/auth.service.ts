import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { AuthRequestDto, AuthResponseDto } from '../models/auth.model';
import { Observable, tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/auth`;

  register(payload: AuthRequestDto): Observable<AuthResponseDto> {
    return this.http
      .post<AuthResponseDto>(`${this.baseUrl}/register`, payload)
      .pipe(tap((res) => this.setToken(res.accessToken)));
  }

  login(payload: AuthRequestDto): Observable<AuthResponseDto> {
    return this.http
      .post<AuthResponseDto>(`${this.baseUrl}/login`, payload)
      .pipe(tap((res) => this.setToken(res.accessToken)));
  }

  logout(): void {
    localStorage.removeItem('accessToken');
  }

  getToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  private setToken(token: string): void {
    localStorage.setItem('accessToken', token);
  }
}
