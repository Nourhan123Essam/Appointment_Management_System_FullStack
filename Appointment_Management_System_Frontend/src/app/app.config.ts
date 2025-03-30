import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { provideAnimations } from '@angular/platform-browser/animations';
import { authInterceptor } from './core/interceptors/auth.interceptor';


export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes),
    // provideHttpClient(),
    provideAnimations(),
    provideHttpClient(withInterceptors([authInterceptor])) // Register the interceptor
  ]
};
