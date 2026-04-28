import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface TagQuota { tagId: number; count: number; }
export interface GenerateMealPlanRequest { userId: number; weekStartDate: string; tagQuotas: TagQuota[]; }
export interface MealPlanItemDto { id: number; dayOfWeek: number; recipeId: number; recipeName: string; }
export interface MealPlanDto { id: number; userId: number; weekStartDate: string; items: MealPlanItemDto[]; }

@Injectable({ providedIn: 'root' })
export class MealPlanService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/mealplans`;

  generate(dto: GenerateMealPlanRequest): Observable<MealPlanDto> {
    return this.http.post<MealPlanDto>(`${this.base}/generate`, dto);
  }

  getAll(userId = environment.defaultUserId): Observable<MealPlanDto[]> {
    return this.http.get<MealPlanDto[]>(this.base, {
      params: new HttpParams().set('userId', userId)
    });
  }

  getById(id: number): Observable<MealPlanDto> {
    return this.http.get<MealPlanDto>(`${this.base}/${id}`);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
