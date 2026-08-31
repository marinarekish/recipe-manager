import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { RecipeService } from '../../data/recipe.service';
import { Recipe } from '../../data/recipe.models';
import { RecipeGridComponent } from '../../../../shared/components/recipe-grid/recipe-grid.component';

@Component({
  selector: 'app-my-recipes',
  standalone: true,
  imports: [RouterLink, RecipeGridComponent],
  templateUrl: './my-recipes.component.html',
  styleUrl: './my-recipes.component.scss',
})
export class MyRecipesComponent implements OnInit {
  private readonly recipeService = inject(RecipeService);

  recipes: Recipe[] = [];
  loading = true;
  errorMessage = '';

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.errorMessage = '';

    this.recipeService.getMine().subscribe({
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
    // later: FavoriteService.add / remove
    console.log('favorite toggle', recipeId);
  }
}
