import { Routes } from '@angular/router';
import { LayoutComponent } from './features/layout/layout.component';
import { PlaceholderComponent } from './shared/components/placeholder/placeholder.component';
import { authGuard } from './core/auth/auth.guard';
import { adminGuard } from './core/auth/admin.guard';
import {RegisterComponent} from './features/auth/register/register.component';
import {LoginComponent} from './features/auth/login/login.component';
import {VerifyCodeComponent} from './features/auth/verify-code/verify-code.component';
import {ExploreComponent} from './features/recipes/pages/explore/explore.component';
import {MyRecipesComponent} from './features/recipes/pages/my-recipes/my-recipes.component';

export const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'recipes', pathMatch: 'full' },
      // Note: 'recipes/me' must be declared before a future 'recipes/:id'
      // so the static 'me' segment is matched as My Recipes, not a recipe id.
      { path: 'recipes/me', component: MyRecipesComponent, data: { title: 'My Recipes' } },
      { path: 'recipes', component: ExploreComponent, data: { title: 'Explore' } },
      { path: 'favorites', component: PlaceholderComponent, data: { title: 'Favorites' } },
      { path: 'profile', component: PlaceholderComponent, data: { title: 'Profile' } },
      {
        path: 'admin/users',
        component: PlaceholderComponent,
        data: { title: 'Admin — Users' },
        canActivate: [adminGuard],
      },
    ],
  },
  { path: 'login', component: LoginComponent, data: { title: 'Login' } },
  { path: 'verify', component: VerifyCodeComponent, data: { title: 'Verify' } },
  { path: 'register', component: RegisterComponent, data: { title: 'Register' } },
  { path: '**', redirectTo: '' },
];
