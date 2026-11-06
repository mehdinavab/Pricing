import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import {
  AuthenticatedUser,
  AuthenticationSession,
  TokenResponse,
} from './authentication-session.service';

export interface LoginCredentials {
  readonly username: string;
  readonly password: string;
}

export interface OtpRequestResponse {
  readonly requestId: string;
  readonly expiresAtUtc: string;
  readonly retryAfterSeconds: number;
}

export interface OtpVerification {
  readonly requestId: string;
  readonly phoneNumber: string;
  readonly code: string;
}

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  private readonly http = inject(HttpClient);
  private readonly session = inject(AuthenticationSession);

  readonly currentUser = this.session.currentUser;
  readonly isAuthenticated = this.session.isAuthenticated;

  login(credentials: LoginCredentials): Observable<AuthenticatedUser> {
    return this.http
      .post<TokenResponse>('/api/identity/auth/login', {
        username: credentials.username.trim(),
        password: credentials.password,
      })
      .pipe(map((tokens) => this.session.establish(tokens)));
  }

  requestOtp(phoneNumber: string): Observable<OtpRequestResponse> {
    return this.http.post<OtpRequestResponse>('/api/identity/auth/otp/request', {
      phoneNumber: phoneNumber.trim(),
    });
  }

  verifyOtp(verification: OtpVerification): Observable<AuthenticatedUser> {
    return this.http
      .post<TokenResponse>('/api/identity/auth/otp/verify', verification)
      .pipe(map((tokens) => this.session.establish(tokens)));
  }

  logout(): void {
    const refreshToken = this.session.refreshToken();
    this.session.clear();
    if (refreshToken) {
      this.http
        .post<void>('/api/identity/auth/logout', { refreshToken })
        .subscribe({ error: () => undefined });
    }
  }
}
