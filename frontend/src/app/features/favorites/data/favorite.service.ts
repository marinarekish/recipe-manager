import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import { AddFavoriteRequest, FavoriteResponse } from './favorite.models';

@Injectable({
  providedIn: 'root'
})

export class FavoriteService {

  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/api/favorites`;

  getAllFavorites(): Observable<FavoriteResponse[]> {
    return this.http.get<FavoriteResponse[]>(this.baseUrl);
  }

  add(body: AddFavoriteRequest): Observable<FavoriteResponse> {
    return this.http.post<FavoriteResponse>(`${this.baseUrl}`, body);
  }

  remove(id: number): Observable<void>  {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
