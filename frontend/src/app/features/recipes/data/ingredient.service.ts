import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import {Ingredient} from './ingredient.models';

@Injectable({
  providedIn: 'root'
})
export class IngredientService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/api/ingredients`;

  getAll(): Observable<Ingredient[]> {
    return this.http.get<Ingredient[]>(`${this.baseUrl}`);
  }
}
