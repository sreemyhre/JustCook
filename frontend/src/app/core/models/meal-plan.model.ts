export interface TagQuota {
  tagId: number;
  count: number;
}

export interface GenerateMealPlanRequest {
  userId: number;
  weekStartDate: string;
  tagQuotas: TagQuota[];
}

export interface MealPlanItemDto {
  id: number;
  dayOfWeek: number;
  recipeId: number;
  recipeName: string;
}

export interface MealPlanDto {
  id: number;
  userId: number;
  weekStartDate: string;
  items: MealPlanItemDto[];
}

export interface MealPlanSaveDto {
  userId: number;
  weekStartDate: string;
  items: { recipeId: number; dayOfWeek: number }[];
}
