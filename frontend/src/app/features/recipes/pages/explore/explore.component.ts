import { Component, OnInit, inject } from '@angular/core';
import { forkJoin } from 'rxjs';

import { RecipeService } from '../../data/recipe.service';
import { Recipe } from '../../data/recipe.models';
import { RecipeGridComponent } from '../../../../shared/components/recipe-grid/recipe-grid.component';
import { FavoriteService } from '../../../favorites/data/favorite.service';

@Component({
  selector: 'app-explore',
  standalone: true,
  imports: [RecipeGridComponent],
  templateUrl: './explore.component.html',
  styleUrl: './explore.component.scss',
})
export class ExploreComponent implements OnInit {
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

    // Load recipes and favorited ids in parallel.
    forkJoin({
      recipes: this.recipeService.getAll(),
      favorites: this.favoriteService.getAllFavorites(),
    }).subscribe({
      next: ({ recipes, favorites }) => {
        this.recipes = recipes;
        this.favoriteIds = new Set((favorites ?? []).map((f) => f.recipeId));
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.errorMessage = 'Could not load recipes. Please try again.';
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
