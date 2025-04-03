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

  const expectedRoles: string[] = route.data['roles']; // Expecting an array of roles
  if (expectedRoles && !expectedRoles.some(role => authService.hasRole(role))) {
    router.navigate(['/appointments']);
    return false;
  }

  return true;
};
