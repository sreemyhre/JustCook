import { DayOfWeekEnum, MealType } from './enums';

export interface MealPlanItemDto {
  id: number;
  recipeId: number;
  recipeName: string;
  dayOfWeek: DayOfWeekEnum;
  mealType: MealType;
}

export interface MealPlanDto {
  id: number;
  userId: number;
  weekStartDate: string;
  items: MealPlanItemDto[];
}

export interface TagQuotaDto {
  tagId: number;
  count: number;
}

export interface GenerateMealPlanDto {
  userId: number;
  weekStartDate: string;
  tagQuotas: TagQuotaDto[];
}
