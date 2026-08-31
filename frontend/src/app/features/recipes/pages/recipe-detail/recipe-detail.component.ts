import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { Recipe } from '../../data/recipe.models';

import { RecipeService } from '../../data/recipe.service';
import { AuthService } from '../../../../core/auth/auth.service';
import { FavoriteService } from '../../../favorites/data/favorite.service';

@Component({
  selector: 'app-recipe-detail',
  imports: [RouterLink],
  templateUrl: './recipe-detail.component.html',
  styleUrl: './recipe-detail.component.scss'
})

export class RecipeDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private recipeService = inject(RecipeService);
  private authService = inject(AuthService);
  private favoriteService = inject(FavoriteService);

  recipe: Recipe | null = null;
  loading = true;
  errorMessage = '';
  isFavorite = false;

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (!Number.isInteger(id) || id <= 0) {
      this.loading = false;
      this.errorMessage = 'Invalid recipe id.';
      return;
    }

    this.load(id);
  }

  load(id: number) {
    this.loading = true;
    this.errorMessage = '';
    this.recipe = null;

    this.recipeService.getById(id).subscribe({
      next: (recipe) => {
        this.recipe = recipe;
        this.loading = false;
        this.loadFavoriteState(recipe.recipeId);
      },
      error: (err) => {
        this.loading = false;
        this.handleSaveError(err);
      }
    })
  }

  private loadFavoriteState(recipeId: number): void {
    this.favoriteService.getAllFavorites().subscribe({
      next: (favorites) => {
        this.isFavorite = (favorites ?? []).some((f) => f.recipeId === recipeId);
      },
      error: () => {
        this.isFavorite = false;
      },
    });
  }

  onToggleFavorite(): void {
    if (!this.recipe) {
      return;
    }

    const recipeId = this.recipe.recipeId;

    if (this.isFavorite) {
      this.favoriteService.remove(recipeId).subscribe({
        next: () => {
          this.isFavorite = false;
        },
        error: () => {
          this.errorMessage = 'Could not remove from favorites.';
        },
      });
      return;
    }

    this.favoriteService.add({ recipeId }).subscribe({
      next: () => {
        this.isFavorite = true;
      },
      error: () => {
        this.errorMessage = 'Could not add to favorites.';
      },
    });
  }

  canManage(): boolean {
    const user = this.authService.currentUser();
    const recipe = this.recipe;

    if (!user || !recipe) {
      return false;
    }

    const isAuthor = user.userId === recipe.authorId;
    const isAdmin =
      user.roles?.some(r => r.name === 'Administrator') ?? false;

    return isAuthor || isAdmin;
  }

  canEdit(): boolean {
    const user = this.authService.currentUser();
    const recipe = this.recipe;

    if (!user || !recipe) {
      return false;
    }

    return user.userId === recipe.authorId;
  }

  goBack(): void {
    if (window.history.length > 1) {
      window.history.back();
    } else {
      void this.router.navigateByUrl('/recipes');
    }
  }

  delete() {
    if (!this.recipe || !this.canManage()) {
      return;
    }

    if (!window.confirm(`Are you sure you want to delete recipe?`)) {
      return;
    }

    const id = this.recipe.recipeId;
    this.recipeService.delete(id).subscribe({
      next: () => {
        void this.router.navigateByUrl(`/recipes/me`);
      },
      error: (err) => {
        this.handleSaveError(err);
      }
    })
  }

  private handleSaveError(err: { status?: number; error?: unknown }): void {
    if (err.status === 400) {
      this.errorMessage = 'Please check the form. Some values are invalid.';
    } else if (err.status === 403) {
      this.errorMessage = 'You cannot save this recipe.';
    } else if (err.status === 404) {
      this.errorMessage = 'Recipe was not found.';
    } else {
      this.errorMessage = 'Could not save recipe. Please try again.';
    }
  }
}
