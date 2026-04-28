import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { RecipeService } from '../../../core/services/recipe.service';
import { RecipeDto } from '../../../core/models/recipe.model';

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
  private snackBar = inject(MatSnackBar);

  recipe = signal<RecipeDto | null>(null);
  loggingCook = signal(false);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) { this.router.navigate(['/app/recipes']); return; }
    this.recipeService.getById(+id).subscribe({
      next: r => this.recipe.set(r),
      error: () => {
        this.snackBar.open('Recipe not found', 'Close', { duration: 3000 });
        this.router.navigate(['/app/recipes']);
      }
    });
  }

  get daysSinceCooked(): number | null {
    const d = this.recipe()?.lastCookedDate;
    if (!d) return null;
    return Math.floor((Date.now() - new Date(d).getTime()) / 86_400_000);
  }

  logCook(): void {
    const r = this.recipe();
    if (!r) return;
    this.loggingCook.set(true);
    this.recipeService.logCook(r.id).subscribe({
      next: updated => {
        this.recipe.set(updated);
        this.loggingCook.set(false);
        this.snackBar.open('Cook logged!', 'Close', { duration: 3000 });
      },
      error: () => {
        this.loggingCook.set(false);
        this.snackBar.open('Failed to log cook', 'Close', { duration: 3000 });
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
    this.recipeService.delete(this.recipe()!.id).subscribe({
      next: () => {
        this.snackBar.open('Recipe deleted', 'Close', { duration: 3000 });
        this.router.navigate(['/app/recipes']);
      },
      error: () => this.snackBar.open('Failed to delete recipe', 'Close', { duration: 3000 })
    });
  }
}
