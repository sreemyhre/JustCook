import { Component, Input, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';

import { RecipeDto } from '../../../core/models/recipe.model';
import { formatLastCooked } from '../../../shared/utils/date.utils';

@Component({
  selector: 'app-recipe-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    DragDropModule,
    MatInputModule,
    MatFormFieldModule,
    MatIconModule
  ],
  templateUrl: './recipe-sidebar.component.html',
  styleUrl: './recipe-sidebar.component.scss'
})
export class RecipeSidebarComponent {
  @Input() allRecipes: RecipeDto[] = [];
  @Input() editMode = false;

  searchText = signal('');

  readonly noReturn = () => false;
  readonly formatLastCooked = formatLastCooked;

  get filteredRecipes(): RecipeDto[] {
    const query = this.searchText().toLowerCase().trim();
    if (!query) return this.allRecipes;
    return this.allRecipes.filter(r => r.name.toLowerCase().includes(query));
  }
}
