import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ApiErrorService } from './api-error.service';

export const apiErrorInterceptor: HttpInterceptorFn = (request, next) => {
  const errors = inject(ApiErrorService);
  const requestWithCredentials = request.clone({ withCredentials: true });

  return next(requestWithCredentials).pipe(
    catchError((error: HttpErrorResponse) => {
      errors.report(error);
      return throwError(() => error);
    }),
  );
};
