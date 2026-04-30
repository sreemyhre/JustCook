import { RecipeDto } from '../../core/models/recipe.model';

export interface CalendarDay {
  date: Date;
  dayOfWeek: number; // 0=Mon … 6=Sun (ISO)
  recipeName: string | null;
  recipeId: number | null;
}

export interface CalendarWeek {
  weekStart: Date;
  weekKey: string;
  planId: number | null;
  planItems: { recipeId: number; dayOfWeek: number }[];
  days: CalendarDay[];
  isPast: boolean;
  isCurrent: boolean;
  isFuture: boolean;
}

export interface DropEvent {
  week: CalendarWeek;
  dayOfWeek: number;
  recipe: RecipeDto;
}
