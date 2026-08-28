import { Component, OnInit, inject } from '@angular/core';

import { RecipeService } from '../../data/recipe.service';
import { Recipe } from '../../data/recipe.models';
import { RecipeGridComponent } from '../../../../shared/components/recipe-grid/recipe-grid.component';

@Component({
  selector: 'app-explore',
  standalone: true,
  imports: [RecipeGridComponent],
  templateUrl: './explore.component.html',
  styleUrl: './explore.component.scss',
})
export class ExploreComponent implements OnInit {
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

    this.recipeService.getAll().subscribe({
      next: (data) => {
        this.recipes = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.errorMessage = 'Could not load recipes. Please try again.';
      },
    });
  }

  onFavoriteToggle(recipeId: number): void {
    // later: FavoriteService.add / remove
    console.log('favorite toggle', recipeId);
  }
}
