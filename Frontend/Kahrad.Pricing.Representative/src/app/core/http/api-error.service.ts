import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ApiErrorService {
  private readonly lastErrorState = signal<HttpErrorResponse | null>(null);
  readonly lastError = this.lastErrorState.asReadonly();

  report(error: HttpErrorResponse): void {
    this.lastErrorState.set(error);
  }

  clear(): void {
    this.lastErrorState.set(null);
  }
}
