import {inject} from '@angular/core';
import {Router} from '@angular/router';
import {HttpErrorResponse, HttpInterceptorFn} from '@angular/common/http';
import {catchError, throwError} from 'rxjs';
import {AuthService} from './auth.service';

const PUBLIC_AUTH_PATHS = [
  '/api/auth/request-code',
  '/api/auth/verify-code',
  '/api/auth/register',
];

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const token = auth.getToken();

  if(!token) return next(req);

  const withAuthorization = req.clone({
    setHeaders: {Authorization: `Bearer ${token}`,}
  })

  return next(withAuthorization).pipe(
    catchError((error: unknown) => {
      const sessionExpired =
        error instanceof HttpErrorResponse &&
        error.status === 401 &&
        !PUBLIC_AUTH_PATHS.some((path) => req.url.includes(path));

      if (sessionExpired) {
        auth.logout();

        const returnUrl = router.url;

        if (returnUrl && returnUrl !== '/' && returnUrl !== '/login') {
          void router.navigate(['/login'], {queryParams: {returnUrl}});
        } else {
          void router.navigate(['/login']);
        }
      }

      return throwError(() => error);
    }),
  );
};
