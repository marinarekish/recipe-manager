import { Routes } from '@angular/router';
import { LayoutComponent } from './features/layout/layout.component';
import { PlaceholderComponent } from './shared/components/placeholder/placeholder.component';

export const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: '', redirectTo: 'recipes', pathMatch: 'full' },
      { path: 'recipes', component: PlaceholderComponent, data: { title: 'My Recipes' } },
      { path: 'favorites', component: PlaceholderComponent, data: { title: 'Favorites' } },
      { path: 'profile', component: PlaceholderComponent, data: { title: 'Profile' } },
      { path: 'admin/users', component: PlaceholderComponent, data: { title: 'Admin — Users' } },
    ],
  },
  { path: 'login', component: PlaceholderComponent, data: { title: 'Login' } },
  { path: '**', redirectTo: '' },
];
