import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { AuthenticationService, OtpRequestResponse } from './authentication.service';
import { TokenResponse } from './authentication-session.service';

describe('AuthenticationService', () => {
  let service: AuthenticationService;
  let http: HttpTestingController;

  beforeEach(() => {
    sessionStorage.clear();
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(AuthenticationService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('requests an OTP without exposing account existence', () => {
    const response: OtpRequestResponse = {
      requestId: '7ee54a93-22af-4cf0-aa9a-e038f395460d',
      expiresAtUtc: new Date(Date.now() + 300_000).toISOString(),
      retryAfterSeconds: 60,
    };

    service.requestOtp('09121234567').subscribe((result) => expect(result).toEqual(response));

    const request = http.expectOne('/api/identity/auth/otp/request');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ phoneNumber: '09121234567' });
    request.flush(response);
  });

  it('verifies an OTP and establishes the authenticated session', () => {
    service
      .verifyOtp({
        requestId: '7ee54a93-22af-4cf0-aa9a-e038f395460d',
        phoneNumber: '09121234567',
        code: '123456',
      })
      .subscribe((user) => expect(user.displayName).toBe('otp.user'));

    const request = http.expectOne('/api/identity/auth/otp/verify');
    expect(request.request.method).toBe('POST');
    request.flush(tokens('otp.user'));

    expect(service.isAuthenticated()).toBe(true);
    expect(service.currentUser()?.displayName).toBe('otp.user');
  });

  it('uses the real password login endpoint', () => {
    service
      .login({ username: 'admin', password: 'password' })
      .subscribe((user) => expect(user.displayName).toBe('admin'));

    const request = http.expectOne('/api/identity/auth/login');
    expect(request.request.method).toBe('POST');
    request.flush(tokens('admin'));
  });

  function tokens(username: string): TokenResponse {
    const expiresAt = new Date(Date.now() + 3_600_000);
    const payload = btoa(
      JSON.stringify({
        unique_name: username,
        exp: Math.floor(expiresAt.getTime() / 1000),
      }),
    );
    return {
      accessToken: `header.${payload}.signature`,
      accessTokenExpiresAtUtc: expiresAt.toISOString(),
      refreshToken: 'refresh-token',
      refreshTokenExpiresAtUtc: new Date(Date.now() + 86_400_000).toISOString(),
    };
  }
});

