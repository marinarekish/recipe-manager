import { Routes } from '@angular/router';
import { LayoutComponent } from './features/layout/layout.component';

import { PlaceholderComponent } from './shared/components/placeholder/placeholder.component';

import { authGuard } from './core/auth/auth.guard';
import { adminGuard } from './core/auth/admin.guard';
import { recipeIdGuard } from './core/recipes/recipes.guard';

import { RegisterComponent } from './features/auth/register/register.component';
import { LoginComponent } from './features/auth/login/login.component';
import { VerifyCodeComponent } from './features/auth/verify-code/verify-code.component';

import { ExploreComponent } from './features/recipes/pages/explore/explore.component';
import { MyRecipesComponent } from './features/recipes/pages/my-recipes/my-recipes.component';
import { RecipeDetailComponent } from './features/recipes/pages/recipe-detail/recipe-detail.component';
import { RecipeFormComponent } from './features/recipes/pages/recipe-form/recipe-form.component';
import {FavoritesListComponent} from './features/favorites/pages/favorites-list/favorites-list.component';

export const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'recipes', pathMatch: 'full' },
      // Static 'me' before ':id', otherwise "me" is parsed as an id.
      { path: 'recipes/me', component: MyRecipesComponent, data: { title: 'My Recipes' } },
      // Create/edit are matched before ':id' so their static segments win.
      { path: 'recipes/new', component: RecipeFormComponent, data: { title: 'Create Recipe' } },
      { path: 'recipes/:id/edit', component: RecipeFormComponent, data: { title: 'Edit Recipe' } },
      {
        path: 'recipes/:id',
        component: RecipeDetailComponent,
        canActivate: [recipeIdGuard],
        data: { title: 'Recipe' },
      },
      { path: 'recipes', component: ExploreComponent, data: { title: 'Explore' } },
      { path: 'favorites', component: FavoritesListComponent, data: { title: 'Favorites' } },
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
