import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { provideAnimations } from '@angular/platform-browser/animations';
import { authInterceptor } from './core/interceptors/auth.interceptor';

import { MessageService } from 'primeng/api';



export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes),
    MessageService,
    provideAnimations(),
    provideHttpClient(withInterceptors([authInterceptor])) // Register the interceptor
  ]
};
