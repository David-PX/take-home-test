import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import * as CryptoJS from 'crypto-js';

import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatInputModule,
    MatButtonModule,
  ],
  template: `
    <div class="auth-page">
      <mat-card class="auth-card">
        <h2>Login</h2>

        <form [formGroup]="form" (ngSubmit)="onSubmit()">
          <mat-form-field appearance="outline" class="full">
            <mat-label>Email</mat-label>
            <input matInput formControlName="email" />
          </mat-form-field>

          <mat-form-field appearance="outline" class="full">
            <mat-label>Password</mat-label>
            <input matInput type="password" formControlName="password" />
          </mat-form-field>

          <button
            mat-raised-button
            color="primary"
            class="full"
            [disabled]="form.invalid || loading"
          >
            {{ loading ? 'Logging in...' : 'Login' }}
          </button>
        </form>

        <p class="mt">
          Don't have an account?
          <a routerLink="/auth/register">Register</a>
        </p>

        <p class="error" *ngIf="error">{{ error }}</p>
      </mat-card>
    </div>
  `,
  styles: [
    `
      .auth-page {
        min-height: 100vh;
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 16px;
      }
      .auth-card {
        width: 420px;
        padding: 16px;
      }
      .full {
        width: 100%;
      }
      .mt {
        margin-top: 12px;
      }
      .error {
        color: #b00020;
        margin-top: 12px;
      }
    `,
  ],
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  loading = false;
  error = '';

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading = true;
    this.error = '';

    const email = this.form.value.email!;
    const password = this.form.value.password!;
    const passwordHash = CryptoJS.SHA256(password).toString(CryptoJS.enc.Hex);

    this.auth.login({ email, passwordHash }).subscribe({
      next: () => this.router.navigateByUrl('/loans'),
      error: () => {
        this.error = 'Invalid credentials.';
        this.loading = false;
      },
      complete: () => (this.loading = false),
    });
  }
}
