import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { RecipeDto, CreateRecipeDto, UpdateRecipeDto } from '../models/recipe.model';

@Injectable({ providedIn: 'root' })
export class RecipeService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/recipes`;

  getAll(userId = environment.defaultUserId): Observable<RecipeDto[]> {
    return this.http.get<RecipeDto[]>(this.base, {
      params: new HttpParams().set('userId', userId)
    });
  }

  getById(id: number): Observable<RecipeDto> {
    return this.http.get<RecipeDto>(`${this.base}/${id}`);
  }

  create(dto: CreateRecipeDto): Observable<RecipeDto> {
    return this.http.post<RecipeDto>(this.base, dto);
  }

  update(id: number, dto: UpdateRecipeDto): Observable<RecipeDto> {
    return this.http.put<RecipeDto>(`${this.base}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  logCook(id: number): Observable<RecipeDto> {
    return this.http.patch<RecipeDto>(`${this.base}/${id}/log-cook`, {});
  }

  getRotationSuggestions(userId = environment.defaultUserId, count = 7): Observable<RecipeDto[]> {
    return this.http.get<RecipeDto[]>(`${this.base}/rotation-suggestions`, {
      params: new HttpParams().set('userId', userId).set('count', count)
    });
  }
}
