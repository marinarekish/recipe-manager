export interface CategoryResponse {
  categoryId: number;
  name: string;
}

export interface CuisineResponse {
  cuisineId: number;
  name: string;
}

export interface IngredientResponse {
  ingredientId: number;
  name: string;
}

export enum ActiveTab {
  categories = 'categories',
  cuisines = 'cuisines',
  ingredients = 'ingredients',
}
