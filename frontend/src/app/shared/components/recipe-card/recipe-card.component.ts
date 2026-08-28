import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import {Recipe} from '../../../features/recipes/data/recipe.models';

@Component({
  selector: 'app-recipe-card',
  standalone: true,
  imports: [RouterLink, MatCardModule, MatButtonModule, MatIconModule],
  templateUrl: './recipe-card.component.html',
  styleUrl: './recipe-card.component.scss',
})
export class RecipeCardComponent {
  @Input({ required: true }) recipe!: Recipe;
  @Input() isFavorite = false;

  @Output() favoriteToggle = new EventEmitter<number>();

  get totalTime(): number {
    return this.recipe.prepTimeMinutes + this.recipe.cookTimeMinutes;
  }

  onToggleFavorite(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.favoriteToggle.emit(this.recipe.recipeId);
  }
}
