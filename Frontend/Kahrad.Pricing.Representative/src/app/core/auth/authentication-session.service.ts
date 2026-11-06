import { Injectable, computed, signal } from '@angular/core';

export interface TokenResponse {
  readonly accessToken: string;
  readonly accessTokenExpiresAtUtc: string;
  readonly refreshToken: string;
  readonly refreshTokenExpiresAtUtc: string;
}

export interface AuthenticatedUser {
  readonly displayName: string;
}

interface JwtPayload {
  readonly unique_name?: string;
  readonly exp?: number;
  readonly [claim: string]: unknown;
}

@Injectable({ providedIn: 'root' })
export class AuthenticationSession {
  private readonly storageKey = 'kahrad.pricing.representative.authentication';
  private readonly tokensState = signal<TokenResponse | null>(this.restoreTokens());
  private readonly currentUserState = signal<AuthenticatedUser | null>(
    this.userFromTokens(this.tokensState()),
  );

  readonly currentUser = this.currentUserState.asReadonly();
  readonly isAuthenticated = computed(() => this.currentUserState() !== null);

  accessToken(): string | null {
    return this.tokensState()?.accessToken ?? null;
  }

  refreshToken(): string | null {
    return this.tokensState()?.refreshToken ?? null;
  }

  establish(tokens: TokenResponse): AuthenticatedUser {
    const user = this.userFromTokens(tokens);
    if (!user) {
      throw new Error('The authentication response contains an invalid access token.');
    }

    this.tokensState.set(tokens);
    this.currentUserState.set(user);
    try {
      sessionStorage.setItem(this.storageKey, JSON.stringify(tokens));
    } catch {
      // Authentication remains active for this page when session storage is unavailable.
    }
    return user;
  }

  clear(): void {
    this.tokensState.set(null);
    this.currentUserState.set(null);
    try {
      sessionStorage.removeItem(this.storageKey);
    } catch {
      // In-memory state is already cleared.
    }
  }

  private restoreTokens(): TokenResponse | null {
    try {
      const value = sessionStorage.getItem(this.storageKey);
      if (!value) return null;
      const tokens = JSON.parse(value) as TokenResponse;
      return Date.parse(tokens.accessTokenExpiresAtUtc) > Date.now() ? tokens : null;
    } catch {
      return null;
    }
  }

  private userFromTokens(tokens: TokenResponse | null): AuthenticatedUser | null {
    if (!tokens || Date.parse(tokens.accessTokenExpiresAtUtc) <= Date.now()) return null;
    try {
      const encodedPayload = tokens.accessToken.split('.')[1];
      if (!encodedPayload) return null;
      const base64Payload = encodedPayload.replace(/-/g, '+').replace(/_/g, '/');
      const payload = JSON.parse(
        atob(base64Payload.padEnd(Math.ceil(base64Payload.length / 4) * 4, '=')),
      ) as JwtPayload;
      if (payload.exp && payload.exp * 1000 <= Date.now()) return null;
      const claimName = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
      const displayName = payload.unique_name ?? payload[claimName];
      return typeof displayName === 'string' && displayName.trim()
        ? { displayName: displayName.trim() }
        : null;
    } catch {
      return null;
    }
  }
}
