import { Component, EventEmitter, inject, Input, Output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';

import { RecipeDto } from '../../../core/models/recipe.model';
import { normalizeToMonday } from '../planner.utils';
import { formatLastCooked } from '../../../shared/utils/date.utils';

@Component({
  selector: 'app-recipe-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DragDropModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatDatepickerModule
  ],
  templateUrl: './recipe-sidebar.component.html',
  styleUrl: './recipe-sidebar.component.scss'
})
export class RecipeSidebarComponent {
  private fb = inject(FormBuilder);

  @Input() allRecipes: RecipeDto[] = [];
  @Input() generating = false;
  @Input() editMode = false;
  @Output() generatePlan = new EventEmitter<Date>();

  searchText = signal('');

  form = this.fb.group({
    weekStartDate: [this.getNextMonday(), Validators.required]
  });

  readonly noReturn = () => false;
  readonly formatLastCooked = formatLastCooked;

  get filteredRecipes(): RecipeDto[] {
    const query = this.searchText().toLowerCase().trim();
    if (!query) return this.allRecipes;
    return this.allRecipes.filter(r => r.name.toLowerCase().includes(query));
  }

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    const date = this.form.get('weekStartDate')!.value as Date;
    this.generatePlan.emit(date);
  }

  private getNextMonday(): Date {
    const d = normalizeToMonday(new Date());
    d.setDate(d.getDate() + 7);
    return d;
  }
}
