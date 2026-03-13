import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { MealPlanDto, GenerateMealPlanDto } from '../models/meal-plan.model';

@Injectable({ providedIn: 'root' })
export class MealPlanService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/mealplans`;

  getAll(userId: number): Observable<MealPlanDto[]> {
    return this.http.get<MealPlanDto[]>(`${this.base}?userId=${userId}`);
  }

  getById(id: number): Observable<MealPlanDto> {
    return this.http.get<MealPlanDto>(`${this.base}/${id}`);
  }

  generate(dto: GenerateMealPlanDto): Observable<MealPlanDto> {
    return this.http.post<MealPlanDto>(`${this.base}/generate`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
