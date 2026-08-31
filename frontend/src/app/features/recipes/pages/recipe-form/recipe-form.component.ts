import {Component, inject, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {FormArray, FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {RouterLink} from '@angular/router';

import {RecipeService} from '../../data/recipe.service';
import {AuthService} from '../../../../core/auth/auth.service';

import {CreateRecipeRequest, Recipe, RecipeIngredientRequest, UpdateRecipeRequest} from '../../data/recipe.models';

@Component({
  selector: 'app-recipe-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './recipe-form.component.html',
  styleUrl: './recipe-form.component.scss',
})

export class RecipeFormComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private recipeService = inject(RecipeService);
  private authService = inject(AuthService);

  private formBuilder = inject(FormBuilder);

  recipe: Recipe | null = null;

  isEditing = false;
  recipeId: number | null = null;
  errorMessage = '';
  submitting = false;

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

  ngOnInit(): void {
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
    this.load(paramId);
  }

  addIngredient(): void {
    this.ingredients.push(this.createIngredientGroup());
  }

  removeIngredient(index: number): void {
    if (this.ingredients.length > 1) {
      this.ingredients.removeAt(index);
    }
  }

  submit() {
    if (this.recipeForm.invalid) {
      this.recipeForm.markAllAsTouched();
      return;
    }

    if (!this.recipe?.recipeId && this.isEditing) return;
    if (this.submitting) return;

    const body = this.buildBody();
    this.submitting = true;
    this.errorMessage = '';

    if (this.isEditing && this.recipeId !== null) {
      this.updateRecipe(this.recipeId, body);
    } else {
      this.createRecipe(body);
    }
  }

  canEdit(): boolean {
    const user = this.authService.currentUser();
    const recipe = this.recipe;

    if (!user || !recipe) {
      return false;
    }

    return user.userId === recipe.authorId;
  }

  private load(id: number) {
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
  }

  private createIngredientGroup() {
    return this.formBuilder.group({
      name: ['', Validators.required],
      amount: [null, [Validators.required, Validators.min(0.1)]],
      unit: ['', Validators.required]
    });
  }

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
