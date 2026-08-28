import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import { CreateRecipeRequest, Recipe, UpdateRecipeRequest } from './recipe.models';

@Injectable({
  providedIn: 'root',
})
export class RecipeService {

  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/api/recipes`;

  getAll(): Observable<Recipe[]>{
    return this.http.get<Recipe[]>(this.baseUrl);
  }

  getMine(): Observable<Recipe[]> {
    return this.http.get<Recipe[]>(`${this.baseUrl}/me`);
  }

  getById(id: number): Observable<Recipe> {
    return this.http.get<Recipe>(`${this.baseUrl}/${id}`);
  }

  create(body: CreateRecipeRequest): Observable<Recipe> {
    return this.http.post<Recipe>(this.baseUrl, body);
  }

  update(id: number, body: UpdateRecipeRequest): Observable<Recipe> {
    return this.http.put<Recipe>(`${this.baseUrl}/${id}`, body);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}

