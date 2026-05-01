import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { Subject, switchMap } from 'rxjs';

import { MealPlanService, MealPlanDto } from '../../../core/services/meal-plan.service';
import { RecipeService } from '../../../core/services/recipe.service';
import { ToastService } from '../../../core/services/toast.service';
import { RecipeDto } from '../../../core/models/recipe.model';
import { environment } from '../../../../environments/environment';

import { DropEvent } from '../planner.types';
import { buildWeeksForMonth, isFutureWeek, normalizeToMonday, startOfMonth, toWeekKey } from '../planner.utils';
import { RecipeSidebarComponent } from '../recipe-sidebar/recipe-sidebar.component';
import { PlannerCalendarComponent } from '../planner-calendar/planner-calendar.component';

@Component({
  selector: 'app-generate-plan',
  standalone: true,
  imports: [
    CommonModule,
    DragDropModule,
    MatSnackBarModule,
    RecipeSidebarComponent,
    PlannerCalendarComponent
  ],
  templateUrl: './generate-plan.component.html',
  styleUrl: './generate-plan.component.scss'
})
export class GeneratePlanComponent implements OnInit {
  private mealPlanService = inject(MealPlanService);
  private recipeService = inject(RecipeService);
  private toast = inject(ToastService);
  private destroyRef = inject(DestroyRef);

  viewMonth = signal<Date>(startOfMonth(new Date()));
  editMode = signal(false);
  plans = signal<MealPlanDto[]>([]);
  allRecipes = signal<RecipeDto[]>([]);
  generating = signal(false);
  savingDay = signal<string | null>(null);

  calendarWeeks = computed(() => buildWeeksForMonth(this.viewMonth(), this.plans()));

  private readonly reload$ = new Subject<void>();

  constructor() {
    this.reload$
      .pipe(
        switchMap(() => this.mealPlanService.getAll()),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: plans => this.plans.set(plans),
        error: () => this.toast.error('Failed to load plans.')
      });
  }

  ngOnInit(): void {
    this.recipeService.getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(recipes => this.allRecipes.set(recipes));
    this.loadPlans();
  }

  private shiftMonth(delta: -1 | 1): void {
    const m = this.viewMonth();
    this.viewMonth.set(new Date(m.getFullYear(), m.getMonth() + delta, 1));
    if (!this.calendarWeeks().some(w => w.isFuture)) {
      this.editMode.set(false);
    }
  }

  prevMonth(): void { this.shiftMonth(-1); }
  nextMonth(): void { this.shiftMonth(1); }

  toggleEdit(): void {
    this.editMode.update(v => !v);
  }

  generate(rawDate: Date): void {
    const weekStart = normalizeToMonday(rawDate);
    const weekKey = toWeekKey(weekStart);

    if (!isFutureWeek(weekStart)) {
      this.toast.error('Please select a future week to generate a plan.');
      return;
    }

    const existing = this.plans().find(p => toWeekKey(new Date(p.weekStartDate)) === weekKey);
    if (existing) {
      this.toast.error('A plan already exists for that week.');
      return;
    }

    this.generating.set(true);
    this.mealPlanService.generate({
      userId: environment.defaultUserId,
      weekStartDate: weekStart.toISOString(),
      tagQuotas: []
    }).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.generating.set(false);
        this.toast.success('Plan generated!');
        this.loadPlans();
        this.viewMonth.set(startOfMonth(weekStart));
      },
      error: () => {
        this.generating.set(false);
        this.toast.error('Failed to generate plan.');
      }
    });
  }

  onDrop(event: DropEvent): void {
    const { week, dayOfWeek, recipe } = event;
    if (!week.isFuture) return;

    this.savingDay.set(`${week.weekKey}-${dayOfWeek}`);

    const updatedItems = [
      ...week.planItems.filter(i => i.dayOfWeek !== dayOfWeek),
      { recipeId: recipe.id, dayOfWeek }
    ];

    const save$ = week.planId
      ? this.mealPlanService.update(week.planId, {
          userId: environment.defaultUserId,
          weekStartDate: week.weekStart.toISOString(),
          items: updatedItems
        })
      : this.mealPlanService.create({
          userId: environment.defaultUserId,
          weekStartDate: week.weekStart.toISOString(),
          items: updatedItems
        });

    save$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.savingDay.set(null);
        this.toast.success(`${recipe.name} saved!`);
        this.loadPlans();
      },
      error: () => {
        this.savingDay.set(null);
        this.toast.error('Failed to save. Please try again.');
      }
    });
  }

  loadPlans(): void {
    this.reload$.next();
  }
}
