import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const recipeIdGuard: CanActivateFn = (route) => {
  const id = Number(route.paramMap.get('id'));
  const valid = Number.isInteger(id) && id > 0;

  if (valid) {
    return true;
  }

  return inject(Router).createUrlTree(['/recipes']);
};
