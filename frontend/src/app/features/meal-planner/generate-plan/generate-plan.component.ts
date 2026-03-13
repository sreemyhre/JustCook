import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import {
  ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators, AbstractControl
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { MealPlanService } from '../../../core/services/meal-plan.service';
import { TagService } from '../../../core/services/tag.service';
import { TagDto } from '../../../core/models/tag.model';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-generate-plan',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatDatepickerModule,
    MatSnackBarModule
  ],
  templateUrl: './generate-plan.component.html',
  styleUrl: './generate-plan.component.scss'
})
export class GeneratePlanComponent implements OnInit {
  private fb = inject(FormBuilder);
  private mealPlanService = inject(MealPlanService);
  private tagService = inject(TagService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  allTags = signal<TagDto[]>([]);
  generating = signal(false);

  form: FormGroup = this.fb.group({
    weekStartDate: [this.getNextMonday(), Validators.required],
    tagQuotas: this.fb.array([])
  });

  get tagQuotas(): FormArray {
    return this.form.get('tagQuotas') as FormArray;
  }

  ngOnInit(): void {
    this.tagService.getAll().subscribe(tags => this.allTags.set(tags));
  }

  addQuotaRow(): void {
    this.tagQuotas.push(this.fb.group({
      tagId: [null, Validators.required],
      count: [1, [Validators.required, Validators.min(1)]]
    }));
  }

  removeQuotaRow(index: number): void {
    this.tagQuotas.removeAt(index);
  }

  asFormGroup(ctrl: AbstractControl): FormGroup {
    return ctrl as FormGroup;
  }

  generate(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.generating.set(true);
    const raw = this.form.getRawValue();
    this.mealPlanService.generate({
      userId: environment.defaultUserId,
      weekStartDate: (raw.weekStartDate as Date).toISOString(),
      tagQuotas: raw.tagQuotas
    }).subscribe({
      next: () => {
        this.generating.set(false);
        this.router.navigate(['/planner']);
      },
      error: () => {
        this.generating.set(false);
        this.snackBar.open('Failed to generate plan', 'Close', { duration: 3000 });
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/planner']);
  }

  private getNextMonday(): Date {
    const today = new Date();
    const day = today.getDay();
    const offset = day === 0 ? 1 : 8 - day;
    const monday = new Date(today);
    monday.setDate(today.getDate() + offset);
    monday.setHours(0, 0, 0, 0);
    return monday;
  }
}
