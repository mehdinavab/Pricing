import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthenticationSession } from '../auth/authentication-session.service';

export const authTokenInterceptor: HttpInterceptorFn = (request, next) => {
  const accessToken = inject(AuthenticationSession).accessToken();
  return next(
    accessToken
      ? request.clone({ setHeaders: { Authorization: `Bearer ${accessToken}` } })
      : request,
  );
};
