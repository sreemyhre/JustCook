import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { RecipeDto } from '../../../core/models/recipe.model';

@Component({
  selector: 'app-recipe-card',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatButtonModule],
  templateUrl: './recipe-card.component.html',
  styleUrl: './recipe-card.component.scss'
})
export class RecipeCardComponent {
  @Input({ required: true }) recipe!: RecipeDto;
  @Output() deleteRequest = new EventEmitter<RecipeDto>();
  @Output() logCookRequest = new EventEmitter<RecipeDto>();

  private router = inject(Router);

  get daysSinceCooked(): number | null {
    if (!this.recipe.lastCookedDate) return null;
    const diff = Date.now() - new Date(this.recipe.lastCookedDate).getTime();
    return Math.floor(diff / 86_400_000);
  }

  goToDetail(): void {
    this.router.navigate(['/app/recipes', this.recipe.id]);
  }

  goToEdit(event: MouseEvent): void {
    event.stopPropagation();
    this.router.navigate(['/app/recipes', this.recipe.id, 'edit']);
  }

  onDelete(event: MouseEvent): void {
    event.stopPropagation();
    this.deleteRequest.emit(this.recipe);
  }

  onLogCook(event: MouseEvent): void {
    event.stopPropagation();
    this.logCookRequest.emit(this.recipe);
  }
}
