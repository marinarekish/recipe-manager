import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Recipe } from '../../../features/recipes/data/recipe.models';
import { RecipeCardComponent } from '../recipe-card/recipe-card.component';

@Component({
  selector: 'app-recipe-grid',
  standalone: true,
  imports: [RecipeCardComponent],
  templateUrl: './recipe-grid.component.html',
  styleUrl: './recipe-grid.component.scss',
})
export class RecipeGridComponent {
  @Input() recipes: Recipe[] = [];
  @Input() loading = false;
  @Input() errorMessage = '';

  @Output() favoriteToggle = new EventEmitter<number>();

  onFavoriteToggle(recipeId: number): void {
    this.favoriteToggle.emit(recipeId);
  }
}
