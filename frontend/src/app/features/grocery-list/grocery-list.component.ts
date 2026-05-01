import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule } from '@angular/material/snack-bar';

import { MealPlanService, MealPlanDto } from '../../core/services/meal-plan.service';
import { RecipeService } from '../../core/services/recipe.service';
import { ToastService } from '../../core/services/toast.service';
import { RecipeDto } from '../../core/models/recipe.model';
import { normalizeToMonday, toWeekKey } from '../meal-planner/planner.utils';

const DAY_NAMES = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

interface AggregatedIngredient {
  name: string;
  quantity: number;
  unit: string;
  isStaple: boolean;
}

function aggregateIngredients(recipes: RecipeDto[]): AggregatedIngredient[] {
  const map = new Map<string, AggregatedIngredient>();
  for (const recipe of recipes) {
    for (const ing of recipe.ingredients) {
      const key = `${ing.name.toLowerCase()}|${(ing.unit ?? '').toLowerCase()}`;
      const entry = map.get(key);
      if (entry) {
        entry.quantity += ing.quantity;
      } else {
        map.set(key, { name: ing.name, quantity: ing.quantity, unit: ing.unit ?? '', isStaple: ing.isStaple });
      }
    }
  }
  return [...map.values()].sort((a, b) => a.name.localeCompare(b.name));
}

@Component({
  selector: 'app-grocery-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatDatepickerModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './grocery-list.component.html',
  styleUrl: './grocery-list.component.scss'
})
export class GroceryListComponent implements OnInit {
  private mealPlanService = inject(MealPlanService);
  private recipeService = inject(RecipeService);
  private toast = inject(ToastService);
  private fb = inject(FormBuilder);
  private destroyRef = inject(DestroyRef);

  readonly dayNames = DAY_NAMES;

  selectedWeekStart = signal<Date>(normalizeToMonday(new Date()));
  plans = signal<MealPlanDto[]>([]);
  weekRecipes = signal<RecipeDto[]>([]);
  loading = signal(false);

  dateControl = this.fb.control<Date>(normalizeToMonday(new Date()));

  selectedPlan = computed(() => {
    const key = toWeekKey(this.selectedWeekStart());
    return this.plans().find(p => toWeekKey(new Date(p.weekStartDate)) === key) ?? null;
  });

  aggregatedIngredients = computed(() => aggregateIngredients(this.weekRecipes()));

  shoppingIngredients = computed(() =>
    this.aggregatedIngredients().filter(i => !i.isStaple)
  );

  stapleIngredients = computed(() =>
    this.aggregatedIngredients().filter(i => i.isStaple)
  );

  weekDaySlots = computed(() => {
    const plan = this.selectedPlan();
    return DAY_NAMES.map((name, i) => ({
      dayName: name,
      recipeName: plan?.items.find(item => item.dayOfWeek === i)?.recipeName ?? null
    }));
  });

  ngOnInit(): void {
    this.mealPlanService.getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: plans => {
          this.plans.set(plans);
          if (!this.selectedPlan()) {
            this.jumpToNearestPlan(plans);
          }
          this.loadWeekRecipes();
        },
        error: () => this.toast.error('Failed to load plans.')
      });
  }

  private jumpToNearestPlan(plans: MealPlanDto[]): void {
    const todayKey = toWeekKey(new Date());
    const nearest = plans
      .filter(p => toWeekKey(new Date(p.weekStartDate)) >= todayKey)
      .sort((a, b) => new Date(a.weekStartDate).getTime() - new Date(b.weekStartDate).getTime())[0];
    if (nearest) {
      const weekStart = normalizeToMonday(new Date(nearest.weekStartDate));
      this.selectedWeekStart.set(weekStart);
      this.dateControl.setValue(weekStart, { emitEvent: false });
    }
  }

  onDateChange(date: Date | null): void {
    if (!date) return;
    this.selectedWeekStart.set(normalizeToMonday(date));
    this.weekRecipes.set([]);
    this.loadWeekRecipes();
  }

  private loadWeekRecipes(): void {
    const plan = this.selectedPlan();
    if (!plan || plan.items.length === 0) {
      this.weekRecipes.set([]);
      return;
    }
    this.loading.set(true);
    const recipeIds = [...new Set(plan.items.map(i => i.recipeId))];
    forkJoin(recipeIds.map(id => this.recipeService.getById(id)))
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: recipes => {
          this.weekRecipes.set(recipes);
          this.loading.set(false);
        },
        error: () => {
          this.loading.set(false);
          this.toast.error('Failed to load recipe details.');
        }
      });
  }

  formatQuantity(qty: number): string {
    return qty % 1 === 0 ? qty.toString() : qty.toFixed(1);
  }
}
