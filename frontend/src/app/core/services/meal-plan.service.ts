import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  MealPlanDto,
  MealPlanSaveDto,
  GenerateMealPlanRequest
} from '../models/meal-plan.model';

// Re-export so existing imports from this path continue to work
export type { TagQuota, GenerateMealPlanRequest, MealPlanItemDto, MealPlanDto, MealPlanSaveDto } from '../models/meal-plan.model';

@Injectable({ providedIn: 'root' })
export class MealPlanService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/mealplans`;

  preview(dto: GenerateMealPlanRequest): Observable<MealPlanDto> {
    return this.http.post<MealPlanDto>(`${this.base}/preview`, dto);
  }

  generate(dto: GenerateMealPlanRequest): Observable<MealPlanDto> {
    return this.http.post<MealPlanDto>(`${this.base}/generate`, dto);
  }

  create(dto: MealPlanSaveDto): Observable<MealPlanDto> {
    return this.http.post<MealPlanDto>(this.base, dto);
  }

  getAll(): Observable<MealPlanDto[]> {
    return this.http.get<MealPlanDto[]>(this.base);
  }

  getById(id: number): Observable<MealPlanDto> {
    return this.http.get<MealPlanDto>(`${this.base}/${id}`);
  }

  update(id: number, dto: MealPlanSaveDto): Observable<MealPlanDto> {
    return this.http.put<MealPlanDto>(`${this.base}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
