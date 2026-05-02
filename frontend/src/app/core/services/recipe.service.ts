import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, shareReplay, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { RecipeDto, CreateRecipeDto, UpdateRecipeDto } from '../models/recipe.model';

@Injectable({ providedIn: 'root' })
export class RecipeService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/recipes`;
  private cache$: Observable<RecipeDto[]> | null = null;

  getAll(): Observable<RecipeDto[]> {
    if (!this.cache$) {
      this.cache$ = this.http.get<RecipeDto[]>(this.base).pipe(shareReplay(1));
    }
    return this.cache$;
  }

  getById(id: number): Observable<RecipeDto> {
    return this.http.get<RecipeDto>(`${this.base}/${id}`);
  }

  create(dto: CreateRecipeDto): Observable<RecipeDto> {
    return this.http.post<RecipeDto>(this.base, dto).pipe(
      tap(() => this.clearCache())
    );
  }

  update(id: number, dto: UpdateRecipeDto): Observable<RecipeDto> {
    return this.http.put<RecipeDto>(`${this.base}/${id}`, dto).pipe(
      tap(() => this.clearCache())
    );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`).pipe(
      tap(() => this.clearCache())
    );
  }

  logCook(id: number): Observable<RecipeDto> {
    return this.http.patch<RecipeDto>(`${this.base}/${id}/log-cook`, {}).pipe(
      tap(() => this.clearCache())
    );
  }

  getRotationSuggestions(count = 7): Observable<RecipeDto[]> {
    return this.http.get<RecipeDto[]>(`${this.base}/rotation-suggestions`, {
      params: new HttpParams().set('count', count)
    });
  }

  private clearCache(): void {
    this.cache$ = null;
  }
}
