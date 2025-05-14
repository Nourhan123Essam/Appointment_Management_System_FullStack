import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  console.log("token in interceptor", token);

  // Always clone with credentials
  req = req.clone({
    withCredentials: true,
    setHeaders: token
      ? { Authorization: `Bearer ${token}` }
      : {}
  });

  return next(req);
};
  