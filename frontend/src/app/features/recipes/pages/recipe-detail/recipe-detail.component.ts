import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { Recipe} from '../../data/recipe.models';

import { RecipeService } from '../../data/recipe.service';
import { AuthService } from '../../../../core/auth/auth.service';

@Component({
  selector: 'app-recipe-detail',
  imports: [RouterLink],
  templateUrl: './recipe-detail.component.html',
  styleUrls: ['./recipe-detail.component.scss']
})

export class RecipeDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private recipeService = inject(RecipeService);
  private authService = inject(AuthService);

  recipe: Recipe | null = null;
  loading = true;
  errorMessage = '';

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
      },
      error: (err) => {
        this.loading = false;
        this.recipe = null;

        if (err.status === 404) {
          this.errorMessage = "Could not find recipe";
        } else if (err.status === 403) {
          this.errorMessage = "You do not have access to this recipe.";
        } else {
          this.errorMessage = 'Could not load recipe. Please try again.';
        }
      }
    })
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
        if (err.status === 403) {
          this.errorMessage = 'You cannot delete this recipe.';
        } else if (err.status === 404) {
          this.errorMessage = 'Recipe was not found.';
        } else {
          this.errorMessage = 'Could not delete recipe. Please try again.';
        }
      }
    })
  }
}
