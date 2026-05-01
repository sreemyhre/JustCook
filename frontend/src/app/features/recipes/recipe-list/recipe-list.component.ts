import { Component, DestroyRef, inject, OnInit, signal, computed } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSnackBarModule } from '@angular/material/snack-bar';

import { RecipeService } from '../../../core/services/recipe.service';
import { TagService } from '../../../core/services/tag.service';
import { ToastService } from '../../../core/services/toast.service';
import { RecipeDto } from '../../../core/models/recipe.model';
import { TagDto } from '../../../core/models/tag.model';
import { RecipeCardComponent } from '../../../shared/components/recipe-card/recipe-card.component';

@Component({
  selector: 'app-recipe-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatFormFieldModule,
    MatSnackBarModule,
    RecipeCardComponent
  ],
  templateUrl: './recipe-list.component.html',
  styleUrl: './recipe-list.component.scss'
})
export class RecipeListComponent implements OnInit {
  private recipeService = inject(RecipeService);
  private tagService = inject(TagService);
  private toast = inject(ToastService);
  private destroyRef = inject(DestroyRef);

  recipes = signal<RecipeDto[]>([]);
  tags = signal<TagDto[]>([]);
  selectedTagId = signal<number | null>(null);

  filteredRecipes = computed(() => {
    const tagId = this.selectedTagId();
    if (!tagId) return this.recipes();
    return this.recipes().filter(r => r.tags?.some(t => t.id === tagId));
  });

  ngOnInit(): void {
    this.recipeService.getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: r => this.recipes.set(r),
        error: () => this.toast.error('Failed to load recipes')
      });

    this.tagService.getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({ next: t => this.tags.set(t) });
  }

  onDelete(recipe: RecipeDto): void {
    if (!confirm(`Delete "${recipe.name}"?`)) return;
    this.recipeService.delete(recipe.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.recipes.update(list => list.filter(r => r.id !== recipe.id));
          this.toast.success('Recipe deleted');
        },
        error: () => this.toast.error('Failed to delete recipe')
      });
  }

  onLogCook(recipe: RecipeDto): void {
    this.recipeService.logCook(recipe.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: updated => {
          this.recipes.update(list => list.map(r => r.id === updated.id ? updated : r));
          this.toast.success('Cook logged!');
        },
        error: () => this.toast.error('Failed to log cook')
      });
  }
}
