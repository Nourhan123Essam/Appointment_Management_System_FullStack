import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { AppStateService } from '../services/State/app-state.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const appState = inject(AppStateService);
  const token = authService.getToken();

  const shortLang = appState.getSavedLanguage(); // 'en' or 'ar'
  const langHeader = shortLang === 'ar' ? 'ar-EG' : 'en-US';

  req = req.clone({
    withCredentials: true,
    setHeaders: {
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      'Accept-Language': langHeader
    }
  });

  return next(req);
};
  