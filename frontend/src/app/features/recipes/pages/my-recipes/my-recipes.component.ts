import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { switchMap } from 'rxjs';

import { RecipeService } from '../../data/recipe.service';
import { Recipe } from '../../data/recipe.models';
import { RecipeGridComponent } from '../../../../shared/components/recipe-grid/recipe-grid.component';
import { FavoriteService } from '../../../favorites/data/favorite.service';

@Component({
  selector: 'app-my-recipes',
  standalone: true,
  imports: [RouterLink, RecipeGridComponent],
  templateUrl: './my-recipes.component.html',
  styleUrl: './my-recipes.component.scss',
})
export class MyRecipesComponent implements OnInit {
  private readonly recipeService = inject(RecipeService);
  private readonly favoriteService = inject(FavoriteService);

  recipes: Recipe[] = [];
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
        switchMap((favorites) => {
          this.favoriteIds = new Set((favorites ?? []).map((f) => f.recipeId));
          return this.recipeService.getMine();
        }),
      )
      .subscribe({
        next: (data) => {
          this.recipes = data;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
          this.errorMessage = 'Could not load your recipes. Please try again.';
        },
      });
  }

  onFavoriteToggle(recipeId: number): void {
    if (this.favoriteIds.has(recipeId)) {
      this.favoriteService.remove(recipeId).subscribe({
        next: () => {
          this.favoriteIds.delete(recipeId);
        },
        error: () => {
          this.errorMessage = 'Could not remove from favorites.';
        },
      });
      return;
    }

    this.favoriteService.add({ recipeId }).subscribe({
      next: () => {
        this.favoriteIds.add(recipeId);
      },
      error: () => {
        this.errorMessage = 'Could not add to favorites.';
      },
    });
  }
}
