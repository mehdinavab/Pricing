import { Injectable, computed, signal } from '@angular/core';
import { Observable, delay, of, tap, throwError } from 'rxjs';

export interface LoginCredentials {
  readonly username: string;
  readonly password: string;
}

export interface AuthenticatedUser {
  readonly displayName: string;
}

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  private readonly currentUserState = signal<AuthenticatedUser | null>(null);

  readonly currentUser = this.currentUserState.asReadonly();
  readonly isAuthenticated = computed(() => this.currentUserState() !== null);

  login(credentials: LoginCredentials): Observable<AuthenticatedUser> {
    const username = credentials.username.trim();
    if (!username || !credentials.password) {
      return throwError(() => new Error('Credentials are required.'));
    }

    const user: AuthenticatedUser = { displayName: username };
    return of(user).pipe(
      delay(350),
      tap((authenticatedUser) => this.currentUserState.set(authenticatedUser)),
    );
  }

  logout(): void {
    this.currentUserState.set(null);
  }
}
