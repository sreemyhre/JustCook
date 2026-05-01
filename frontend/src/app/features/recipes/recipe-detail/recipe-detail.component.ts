import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBarModule } from '@angular/material/snack-bar';

import { RecipeService } from '../../../core/services/recipe.service';
import { ToastService } from '../../../core/services/toast.service';
import { RecipeDto } from '../../../core/models/recipe.model';
import { daysSinceDate } from '../../../shared/utils/date.utils';

@Component({
  selector: 'app-recipe-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatSnackBarModule
  ],
  templateUrl: './recipe-detail.component.html',
  styleUrl: './recipe-detail.component.scss'
})
export class RecipeDetailComponent implements OnInit {
  private recipeService = inject(RecipeService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private toast = inject(ToastService);
  private destroyRef = inject(DestroyRef);

  recipe = signal<RecipeDto | null>(null);
  loggingCook = signal(false);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) { this.router.navigate(['/app/recipes']); return; }
    this.recipeService.getById(+id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: r => this.recipe.set(r),
        error: () => {
          this.toast.error('Recipe not found');
          this.router.navigate(['/app/recipes']);
        }
      });
  }

  get daysSinceCooked(): number | null {
    return daysSinceDate(this.recipe()?.lastCookedDate ?? null);
  }

  logCook(): void {
    const r = this.recipe();
    if (!r) return;
    this.loggingCook.set(true);
    this.recipeService.logCook(r.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: updated => {
          this.recipe.set(updated);
          this.loggingCook.set(false);
          this.toast.success('Cook logged!');
        },
        error: () => {
          this.loggingCook.set(false);
          this.toast.error('Failed to log cook');
        }
      });
  }

  backToRecipes(): void {
    this.router.navigate(['/app/recipes']);
  }

  edit(): void {
    this.router.navigate(['/app/recipes', this.recipe()!.id, 'edit']);
  }

  delete(): void {
    if (!confirm(`Delete "${this.recipe()!.name}"?`)) return;
    this.recipeService.delete(this.recipe()!.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.toast.success('Recipe deleted');
          this.router.navigate(['/app/recipes']);
        },
        error: () => this.toast.error('Failed to delete recipe')
      });
  }
}
