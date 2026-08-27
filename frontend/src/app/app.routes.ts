import { Routes } from '@angular/router';
import { LayoutComponent } from './features/layout/layout.component';
import { PlaceholderComponent } from './shared/components/placeholder/placeholder.component';
import { authGuard } from './core/auth/auth.guard';
import { adminGuard } from './core/auth/admin.guard';
import {RegisterComponent} from './features/auth/register/register.component';
import {LoginComponent} from './features/auth/login/login.component';
import {VerifyCodeComponent} from './features/auth/verify-code/verify-code.component';

export const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'recipes', pathMatch: 'full' },
      { path: 'recipes', component: PlaceholderComponent, data: { title: 'My Recipes' } },
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
