import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AbstractControl, FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { RecipeService } from '../../data/recipe.service';
import { AuthService } from '../../../../core/auth/auth.service';
import { IngredientService } from '../../data/ingredient.service';

import { CreateRecipeRequest, Recipe, RecipeIngredientRequest, UpdateRecipeRequest} from '../../data/recipe.models';
import { Ingredient } from '../../data/ingredient.models';
import { INGREDIENT_UNITS } from '../../data/ingredient-units';

@Component({
  selector: 'app-recipe-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './recipe-form.component.html',
  styleUrl: './recipe-form.component.scss',
})

export class RecipeFormComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private formBuilder = inject(FormBuilder);

  private recipeService = inject(RecipeService);
  private ingredientService = inject(IngredientService);
  private authService = inject(AuthService);

  recipe: Recipe | null = null;
  allIngredients: Ingredient[] = [];
  readonly ingredientUnits = INGREDIENT_UNITS;

  isEditing = false;
  recipeId: number | null = null;
  errorMessage = '';
  submitting = false;

  // form
  recipeForm = this.formBuilder.group({
    title: ['', [Validators.required, Validators.minLength(3)]],
    cuisineName: ['', Validators.required],
    categoryName: ['', Validators.required],
    prepTimeMinutes: [null as number | null, [Validators.required, Validators.min(1)]],
    cookTimeMinutes: [null as number | null, [Validators.required, Validators.min(1)]],
    servings: [1, [Validators.required, Validators.min(1)]],
    instructions: [''],
    imageUrl: [''],
    ingredients: this.formBuilder.array([]),
  });

  get ingredients(): FormArray {
    return this.recipeForm.get('ingredients') as FormArray;
  }

  // lifecycle
  ngOnInit(): void {
    this.loadIngredients(); // cash for autocomplete

    const rawId = this.route.snapshot.paramMap.get('id');
    const paramId = rawId !== null ? Number(rawId) : NaN;

    if (!Number.isInteger(paramId) || paramId <= 0) {
      this.isEditing = false;
      this.recipeId = null;
      this.addIngredient();
      return;
    }

    this.isEditing = true;
    this.recipeId = paramId;
    this.loadRecipe(paramId);
  }

  // Template helpers
  addIngredient(): void {
    this.ingredients.push(this.createIngredientGroup());
  }

  removeIngredient(row: AbstractControl): void {
    if (this.ingredients.length <= 1) {
      return;
    }
    const index = this.ingredients.controls.indexOf(row);
    if (index >= 0) {
      this.ingredients.removeAt(index);
    }
  }

  unitOptionsForRow(index: number): string[] {
    const group = this.ingredients.at(index);
    const current = group
      ? String(group.get('unit')?.value ?? '').trim()
      : '';
    return current && !this.ingredientUnits.includes(current)
      ? [current, ...this.ingredientUnits]
      : this.ingredientUnits;
  }

  filteredIngredients(rowIndex: number): Ingredient[] {
    const q = (this.ingredients.at(rowIndex).get('name')?.value ?? '')
      .toString()
      .trim()
      .toLowerCase();

    if (!q) {
      return [];
    }

    return this.allIngredients
      .filter((i) => i.name.toLowerCase().includes(q))
      .slice(0, 20);
  }

  canEdit(): boolean {
    const user = this.authService.currentUser();
    const recipe = this.recipe;
    if (!user || !recipe) {
      return false;
    }
    return user.userId === recipe.authorId;
  }

  // Submitting
  submit(): void {
    if (this.recipeForm.invalid || this.submitting) {
      this.recipeForm.markAllAsTouched();
      return;
    }

    if (this.isEditing && !this.recipe?.recipeId) {
      return;
    }

    const body = this.buildBody();
    this.submitting = true;
    this.errorMessage = '';

    if (this.isEditing && this.recipeId !== null) {
      this.updateRecipe(this.recipeId, body);
    } else {
      this.createRecipe(body);
    }
  }

  // --- Private: load ---
  private loadIngredients(): void {
    this.ingredientService.getAll().subscribe({
      next: (list) => { this.allIngredients = list ?? []; },
      error: () => { this.allIngredients = []; },
    });
  }

  private loadRecipe(id: number) {
    this.errorMessage = '';
    this.recipe = null;

    this.recipeService.getById(id).subscribe({
      next: (recipe) => {
        this.recipe = recipe;

        if (!this.canEdit()) {
          this.errorMessage = 'You cannot edit this recipe.';
          void this.router.navigateByUrl(`/recipes/${id}`);
          return;
        }

        this.recipeForm.patchValue({
          title: recipe.title,
          cuisineName: recipe.cuisineName,
          categoryName: recipe.categoryName,
          prepTimeMinutes: recipe.prepTimeMinutes,
          cookTimeMinutes: recipe.cookTimeMinutes,
          servings: recipe.servings,
          instructions: recipe.instructions ?? '',
          imageUrl: recipe.imageUrl ?? '',
        });

        this.ingredients.clear();
        for (const item of recipe.ingredients) {
          this.ingredients.push(
            this.formBuilder.group({
              name: [item.name, Validators.required],
              amount: [item.amount, [Validators.required, Validators.min(0.1)]],
              unit: [item.unit, Validators.required],
            }),
          );
        }

        if (this.ingredients.length === 0) {
          this.addIngredient();
        }
      },
      error: (err) => {
        this.recipe = null;
        this.handleSaveError(err);
      }
    })

    this.loadIngredients();
  }

  private createIngredientGroup() {
    return this.formBuilder.group({
      name: ['', Validators.required],
      amount: [null, [Validators.required, Validators.min(0.1)]],
      unit: ['g', Validators.required]
    });
  }

  // API write
  private buildBody() {
    const v = this.recipeForm.getRawValue();

    const ingredients: RecipeIngredientRequest[] = (
      v.ingredients as { name: string; amount: number; unit: string }[]
    ).map((row) => ({
      name: String(row.name ?? '').trim(),
      amount: Number(row.amount),
      unit: String(row.unit ?? '').trim(),
    }));

    return {
      title: String(v.title ?? '').trim(),
      cuisineName: String(v.cuisineName ?? '').trim() || null,
      categoryName: String(v.categoryName ?? '').trim() || null,
      prepTimeMinutes: Number(v.prepTimeMinutes),
      cookTimeMinutes: Number(v.cookTimeMinutes),
      servings: Number(v.servings),
      instructions: String(v.instructions ?? '').trim() || null,
      imageUrl: String(v.imageUrl ?? '').trim() || null,
      ingredients,
    };
  }

  private createRecipe(body: CreateRecipeRequest): void {
    this.recipeService.create(body).subscribe({
      next: () => {
        this.submitting = false;
        void this.router.navigateByUrl(`/recipes/me`);
      },
      error: (err) => {
        this.submitting = false;
        this.handleSaveError(err);
      },
    })
  }

  private updateRecipe(id: number, body: UpdateRecipeRequest): void {
    this.recipeService.update(id, body).subscribe({
      next: (updated) => {
        this.submitting = false;
        void this.router.navigateByUrl(`/recipes/${updated.recipeId}`);
      },
      error: (err) => {
        this.submitting = false;
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
