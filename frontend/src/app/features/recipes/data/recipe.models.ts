/** GET response item / detail */
export interface Recipe {
  recipeId: number;
  title: string;
  prepTimeMinutes: number;
  cookTimeMinutes: number;
  servings: number;
  instructions: string | null;
  imageUrl: string | null;
  cuisineId: number;
  cuisineName: string;
  categoryId: number;
  categoryName: string;
  authorId: number;
  authorName: string;
  ingredients: RecipeIngredient[];
  createdAt: string; // ISO from API
  updatedAt: string;
}

/** Ingredient line in response */
export interface RecipeIngredient {
  ingredientId: number;
  name: string;
  amount: number;
  unit: string;
}

/** Ingredient line in create/update body */
export interface RecipeIngredientRequest {
  ingredientId?: number | null;
  name?: string | null;
  amount: number;
  unit: string;
}

export interface CreateRecipeRequest {
  title: string;
  cuisineId?: number | null;
  cuisineName?: string | null;
  categoryId?: number | null;
  categoryName?: string | null;
  prepTimeMinutes: number;
  cookTimeMinutes: number;
  servings: number;
  instructions?: string | null;
  imageUrl?: string | null;
  ingredients: RecipeIngredientRequest[]; // required, ≥ 1
}

export interface UpdateRecipeRequest {
  title?: string | null;
  cuisineId?: number | null;
  cuisineName?: string | null;
  categoryId?: number | null;
  categoryName?: string | null;
  prepTimeMinutes?: number | null;
  cookTimeMinutes?: number | null;
  servings?: number | null;
  instructions?: string | null;
  imageUrl?: string | null;
  ingredients?: RecipeIngredientRequest[] | null;
}
