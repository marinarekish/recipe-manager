import { Component, OnInit, inject } from '@angular/core';
import { forkJoin, of, switchMap } from 'rxjs';

import { FavoriteService } from '../../data/favorite.service';
import { FavoriteResponse } from '../../data/favorite.models';
import { Recipe } from '../../../recipes/data/recipe.models';
import { RecipeService } from '../../../recipes/data/recipe.service';
import { RecipeGridComponent } from '../../../../shared/components/recipe-grid/recipe-grid.component';

@Component({
  selector: 'app-favorites-list',
  standalone: true,
  imports: [RecipeGridComponent],
  templateUrl: './favorites-list.component.html',
  styleUrl: './favorites-list.component.scss',
})
export class FavoritesListComponent implements OnInit {
  private readonly favoriteService = inject(FavoriteService);
  private readonly recipeService = inject(RecipeService);

  favorites: FavoriteResponse[] = [];
  recipes: Recipe[] = [];
  /** recipeIds currently favorited — for card heart state */
  favoriteIds = new Set<number>();

  loading = true;
  errorMessage = '';

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.errorMessage = '';

    this.favoriteService
      .getAllFavorites()
      .pipe(
        switchMap((list) => {
          this.favorites = list ?? [];
          this.favoriteIds = new Set(this.favorites.map((f) => f.recipeId));

          if (this.favorites.length === 0) {
            return of([] as Recipe[]);
          }

          const requests = this.favorites.map((f) =>
            this.recipeService.getById(f.recipeId),
          );
          return forkJoin(requests);
        }),
      )
      .subscribe({
        next: (fullRecipes) => {
          this.recipes = fullRecipes;
          this.loading = false;
        },
        error: (err) => {
          this.loading = false;
          this.errorMessage =
            'Could not load your favorite recipes. Please try again.';
          console.error(err);
        },
      });
  }

  onFavoriteToggle(recipeId: number): void {
    if (this.favoriteIds.has(recipeId)) {
      this.favoriteService.remove(recipeId).subscribe({
        next: () => {
          this.favoriteIds.delete(recipeId);
          this.favorites = this.favorites.filter((f) => f.recipeId !== recipeId);
          this.recipes = this.recipes.filter((r) => r.recipeId !== recipeId);
        },
        error: (err) => {
          this.errorMessage = 'Could not remove from favorites.';
          console.error(err);
        },
      });
      return;
    }

    this.favoriteService.add({ recipeId }).subscribe({
      next: (created) => {
        this.favoriteIds.add(recipeId);
        this.favorites = [...this.favorites, created];
        // Full card only after reload or getById — optional:
        this.recipeService.getById(recipeId).subscribe({
          next: (recipe) => {
            this.recipes = [...this.recipes, recipe];
          },
        });
      },
      error: (err) => {
        this.errorMessage = 'Could not add to favorites.';
        console.error(err);
      },
    });
  }
}
