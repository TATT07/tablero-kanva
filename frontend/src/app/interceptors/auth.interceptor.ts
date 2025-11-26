import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

let isRefreshing = false;

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  let authReq = req;
  if (token) {
    authReq = addTokenHeader(req, token);
  }

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !isRefreshing) {
        return handle401Error(authReq, next, authService);
      }

      return throwError(() => error);
    })
  );
};

function addTokenHeader(request: HttpRequest<any>, token: string): HttpRequest<any> {
  return request.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`
    }
  });
}

function handle401Error(request: HttpRequest<any>, next: HttpHandlerFn, authService: AuthService): Observable<any> {
  if (!isRefreshing) {
    isRefreshing = true;

    return authService.refreshToken().pipe(
      switchMap((response) => {
        isRefreshing = false;
        return next(addTokenHeader(request, response.token));
      }),
      catchError((error) => {
        isRefreshing = false;
        authService.logout();
        return throwError(() => error);
      })
    );
  }

  return next(request);
}