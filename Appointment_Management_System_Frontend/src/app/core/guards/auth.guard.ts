import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isAuthenticated()) {
    router.navigate(['/appointments']);
    return false;
  }

  const expectedRoles = route.data['role'];
  if (expectedRoles && !expectedRoles.includes(authService.getUserRole())) {
    router.navigate(['/appointments']);
    return false;
  }

  return true;

};
