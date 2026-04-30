import { MealPlanDto } from '../../core/services/meal-plan.service';
import { CalendarDay, CalendarWeek } from './planner.types';

export function normalizeToMonday(date: Date): Date {
  const d = new Date(date);
  const day = (d.getDay() + 6) % 7; // JS 0=Sun → ISO 0=Mon
  d.setDate(d.getDate() - day);
  d.setHours(0, 0, 0, 0);
  return d;
}

export function toWeekKey(date: Date): string {
  return normalizeToMonday(date).toISOString().split('T')[0];
}

export function startOfMonth(date: Date): Date {
  return new Date(date.getFullYear(), date.getMonth(), 1);
}

export function isFutureWeek(weekStart: Date, todayMonday?: Date): boolean {
  const base = todayMonday ?? normalizeToMonday(new Date());
  const nextMonday = new Date(base);
  nextMonday.setDate(nextMonday.getDate() + 7);
  return weekStart >= nextMonday;
}

export function isCurrentWeek(weekStart: Date, todayMonday?: Date): boolean {
  const base = todayMonday ?? normalizeToMonday(new Date());
  return toWeekKey(weekStart) === toWeekKey(base);
}

export function isPastWeek(weekStart: Date, todayMonday?: Date): boolean {
  const base = todayMonday ?? normalizeToMonday(new Date());
  return !isFutureWeek(weekStart, base) && !isCurrentWeek(weekStart, base);
}

export function formatMonthYear(date: Date): string {
  return date.toLocaleDateString('en-US', { month: 'long', year: 'numeric' });
}

export function buildWeeksForMonth(month: Date, plans: MealPlanDto[]): CalendarWeek[] {
  const year = month.getFullYear();
  const monthIndex = month.getMonth();

  // First Monday whose date falls within the target month
  const firstDay = new Date(year, monthIndex, 1);
  let monday = normalizeToMonday(firstDay);
  if (monday.getMonth() !== monthIndex || monday.getFullYear() !== year) {
    monday = new Date(monday);
    monday.setDate(monday.getDate() + 7);
  }

  const todayMonday = normalizeToMonday(new Date());
  const planByKey = new Map(plans.map(p => [toWeekKey(new Date(p.weekStartDate)), p]));
  const weeks: CalendarWeek[] = [];
  const cursor = new Date(monday);

  while (cursor.getFullYear() === year && cursor.getMonth() === monthIndex) {
    const weekKey = toWeekKey(cursor);
    const plan = planByKey.get(weekKey) ?? null;

    const days: CalendarDay[] = Array.from({ length: 7 }, (_, i) => {
      const dayDate = new Date(cursor);
      dayDate.setDate(cursor.getDate() + i);
      const item = plan?.items.find(it => it.dayOfWeek === i);
      return {
        date: dayDate,
        dayOfWeek: i,
        recipeName: item?.recipeName ?? null,
        recipeId: item?.recipeId ?? null
      };
    });

    weeks.push({
      weekStart: new Date(cursor),
      weekKey,
      planId: plan?.id ?? null,
      planItems: plan?.items.map(i => ({ recipeId: i.recipeId, dayOfWeek: i.dayOfWeek })) ?? [],
      days,
      isPast: isPastWeek(cursor, todayMonday),
      isCurrent: isCurrentWeek(cursor, todayMonday),
      isFuture: isFutureWeek(cursor, todayMonday)
    });

    cursor.setDate(cursor.getDate() + 7);
  }

  return weeks;
}
