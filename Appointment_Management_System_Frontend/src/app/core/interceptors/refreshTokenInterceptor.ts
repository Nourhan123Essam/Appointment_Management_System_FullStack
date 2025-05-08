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
          // Try refreshing the token
          const refreshToken = authService.getRefreshToken();
          authService.refreshToken().subscribe({
            next:(res: LoginResult) => {
              authService.setTokens(res.accessToken, res.refreshToken);
  
              // Clone the original request with new token
              const newReq = req.clone({
                setHeaders: {
                  Authorization: `Bearer ${res.accessToken}`
                }
              });
              console.log("Refresh Token is working!!!!!!!!!!!!!!!!");
                
              // Retry the original request
              return next(newReq);
            },
            error: err => {
              console.log("catch error in refresh token interceptor!");
              
              authService.clearTokens();
              // redirect to login
              router.navigate(['/login']);

              return throwError(() => err);
            }
          });
        }
        console.log("look here!!!!!!!!!!!!!!!");
        
        return throwError(() => error);
      })
    );
  };
  