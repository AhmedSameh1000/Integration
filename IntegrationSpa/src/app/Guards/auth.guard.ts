import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  let router = inject(Router);

  const token =
    localStorage.getItem('token') || sessionStorage.getItem('token');
  if (token == null || token == undefined) {
    router.navigate(['/auth/login']);
    return false;
  } else {
    return true;
  }
};
export const authGuardforLogin: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const token =
    localStorage.getItem('token') || sessionStorage.getItem('token');

  if (token) {
    router.navigate(['/']);
    return false;
  } else {
    return true;
  }
};
