import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { MealPlanService } from '../../../core/services/meal-plan.service';
import { MealPlanDto } from '../../../core/models/meal-plan.model';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-planner-home',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './planner-home.component.html',
  styleUrl: './planner-home.component.scss'
})
export class PlannerHomeComponent implements OnInit {
  private mealPlanService = inject(MealPlanService);
  private snackBar = inject(MatSnackBar);

  plan = signal<MealPlanDto | null>(null);
  loading = signal(false);
  deleting = signal(false);

  readonly days = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

  ngOnInit(): void {
    this.loading.set(true);
    this.mealPlanService.getAll(environment.defaultUserId).subscribe({
      next: plans => {
        this.plan.set(plans[0] ?? null);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Failed to load meal plan', 'Close', { duration: 3000 });
      }
    });
  }

  getRecipeForDay(dayIndex: number): string | null {
    const item = this.plan()?.items.find(i => i.dayOfWeek === dayIndex);
    return item?.recipeName ?? null;
  }

  deletePlan(): void {
    const p = this.plan();
    if (!p || !confirm("Delete this week's meal plan?")) return;
    this.deleting.set(true);
    this.mealPlanService.delete(p.id).subscribe({
      next: () => {
        this.plan.set(null);
        this.deleting.set(false);
        this.snackBar.open('Meal plan deleted', 'Close', { duration: 3000 });
      },
      error: () => {
        this.deleting.set(false);
        this.snackBar.open('Failed to delete meal plan', 'Close', { duration: 3000 });
      }
    });
  }

}
