import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Recipe } from '../../../recipes/data/recipe.models';
import { AdminRecipesService } from '../../data/admin-recipe.service';
import {NotificationService} from '../../../../core/ui/notification.service';

@Component({
  selector: 'app-admin-recipes',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './admin-recipes.component.html',
  styleUrl: './admin-recipes.component.scss',
})

export class AdminRecipesComponent implements OnInit {
  private readonly adminRecipeService = inject(AdminRecipesService)
  private readonly notify = inject(NotificationService);

  recipes: Recipe[] = [];
  filtered: Recipe[] = [];

  searchQuery: string = '';

  loading = true;
  errorMessage = '';

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.errorMessage = '';

    this.adminRecipeService.getRecipes().subscribe({
      next: (recipes) => {
        this.recipes = recipes ?? [];
        this.applyFilter();
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.errorMessage = 'Could not load recipes. Please try again.';
      }
    })
  }

  deleteRecipe(recipe: Recipe): void {
    if (!window.confirm('Are you sure you want to delete this recipe?')) {
      return;
    }

    this.adminRecipeService.deleteRecipe(recipe.recipeId).subscribe({
      next: () => {
        this.recipes = this.recipes.filter((r) => r.recipeId !== recipe.recipeId);
        this.notify.success('Recipe was successfully deleted.');
        this.applyFilter();
      },
      error: () => {
        this.errorMessage = 'Could not delete this recipe. Please try again.';
        this.notify.error('Could not delete this recipe.');
      }
    })
  }

  onSearchInput(value: string): void {
    this.searchQuery = value;
    this.applyFilter();
  }

  totalTime(recipe: Recipe): number {
    return recipe.prepTimeMinutes + recipe.cookTimeMinutes;
  }

  private applyFilter(): void {
    const q = this.searchQuery.trim().toLowerCase();

    if (!q) {
      this.filtered = this.recipes;
      return;
    }

    this.filtered = this.recipes.filter(
      (r) =>
        r.title.toLowerCase().includes(q) ||
        (r.authorName?.toLowerCase().includes(q) ?? false),
    );
  }
}
