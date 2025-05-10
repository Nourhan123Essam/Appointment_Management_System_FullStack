import { HttpClient, HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { AuthService } from "../services/auth.service";
import { inject } from "@angular/core";
import { catchError, switchMap, throwError } from "rxjs";
import { Router } from "@angular/router";
import { LoginResult } from "../Interfaces/LoginResult";

export const refreshTokenInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const http = inject(HttpClient);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && authService.getRefreshToken()) {
        return authService.refreshToken().pipe(
          switchMap((res: LoginResult) => {
            authService.setTokens(res.accessToken, res.refreshToken);

            const newReq = req.clone({
              setHeaders: {
                Authorization: `Bearer ${res.accessToken}`
              }
            });

            console.log("Refresh Token succeeded and request retried");

            return next(newReq); // Return retried request
          }),
          catchError(err => {
            console.log("Refresh token failed, redirecting to login");

            authService.clearTokens();
            router.navigate(['/login']);
            return throwError(() => err);
          })
        );
      }

      console.log("Not a 401 or no refresh token, passing error");
      return throwError(() => error);
    })
  );
};
