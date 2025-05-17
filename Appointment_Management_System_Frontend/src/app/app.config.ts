import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { HttpClient, HttpClientModule, provideHttpClient, withInterceptors } from '@angular/common/http';

import { provideAnimations } from '@angular/platform-browser/animations';
import { authInterceptor } from './core/interceptors/auth.interceptor';

import { ConfirmationService, MessageService } from 'primeng/api';
import { refreshTokenInterceptor } from './core/interceptors/refreshTokenInterceptor';

import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { HttpLoaderFactory } from './translate-loader';


export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    MessageService,
    ConfirmationService,
    provideAnimations(),
    provideHttpClient(withInterceptors([authInterceptor, refreshTokenInterceptor])), // Register the interceptors
    importProvidersFrom(
      HttpClientModule,
      TranslateModule.forRoot({
        defaultLanguage: 'en',
        loader: {
          provide: TranslateLoader,
          useFactory: HttpLoaderFactory,
          deps: [HttpClient]
        }
      })
    )
  ]
};
