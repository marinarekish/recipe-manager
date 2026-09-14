import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import {CategoryResponse, CuisineResponse, IngredientResponse} from './admin-lookup.models';

@Injectable({
  providedIn: 'root',
})
export class AdminLookupService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/api`;

  // getters
  getCategories(): Observable<CategoryResponse[]> {
    return this.http.get<CategoryResponse[]>(`${this.baseUrl}/categories`);
  }

  getCuisines() : Observable<CuisineResponse[]> {
    return this.http.get<CuisineResponse[]>(`${this.baseUrl}/cuisines`);
  }

  getIngredients(): Observable<IngredientResponse[]> {
    return this.http.get<IngredientResponse[]>(`${this.baseUrl}/ingredients`);
  }

  // delete
  deleteCategory(categoryId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/categories/${categoryId}`);
  }

  deleteCuisine(cuisineId: number): Observable<void>{
    return this.http.delete<void>(`${this.baseUrl}/cuisines/${cuisineId}`);
  }

  deleteIngredient(ingredientId: number): Observable<void>{
    return this.http.delete<void>(`${this.baseUrl}/ingredients/${ingredientId}`);
  }
}
