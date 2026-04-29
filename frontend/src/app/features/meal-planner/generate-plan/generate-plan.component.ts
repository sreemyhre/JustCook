import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators, AbstractControl } from '@angular/forms';
import { CdkDragDrop, DragDropModule } from '@angular/cdk/drag-drop';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { MealPlanService, MealPlanDto, MealPlanItemDto } from '../../../core/services/meal-plan.service';
import { RecipeService } from '../../../core/services/recipe.service';
import { RecipeDto } from '../../../core/models/recipe.model';
import { TagService } from '../../../core/services/tag.service';
import { TagDto } from '../../../core/models/tag.model';
import { environment } from '../../../../environments/environment';

const DAY_NAMES = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

@Component({
  selector: 'app-generate-plan',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DragDropModule,
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
  private recipeService = inject(RecipeService);
  private tagService = inject(TagService);
  private snackBar = inject(MatSnackBar);

  allTags = signal<TagDto[]>([]);
  allRecipes = signal<RecipeDto[]>([]);
  generating = signal(false);
  saving = signal(false);
  regenerating = signal(false);
  editMode = signal(false);
  searchText = signal('');

  filteredRecipes = computed(() => {
    const query = this.searchText().toLowerCase().trim();
    if (!query) return this.allRecipes();
    return this.allRecipes().filter(recipe =>
      recipe.name.toLowerCase().includes(query)
    );
  });

  form: FormGroup = this.fb.group({
    weekStartDate: [this.getNextMonday(), Validators.required],
    tagQuotas: this.fb.array([])
  });

  get tagQuotas(): FormArray {
    return this.form.get('tagQuotas') as FormArray;
  }

  plans = signal<MealPlanDto[]>([]);
  draftPlan = signal<MealPlanDto | null>(null);
  editItems = signal<MealPlanItemDto[]>([]);

  // Draft (unsaved preview) takes precedence over the last saved plan
  currentPlan = computed(() =>
    this.draftPlan() ??
    [...this.plans()].sort((a, b) =>
      new Date(b.weekStartDate).getTime() - new Date(a.weekStartDate).getTime()
    )[0] ?? null
  );

  isDraft = computed(() => this.draftPlan() !== null);

  weekGrid = computed(() => {
    const plan = this.currentPlan();
    return DAY_NAMES.map((name, index) => ({
      dayName: name,
      item: plan?.items.find(i => i.dayOfWeek === index) ?? null
    }));
  });

  editGrid = computed(() => {
    const items = this.editItems();
    return DAY_NAMES.map((name, index) => ({
      dayName: name,
      item: items.find(i => i.dayOfWeek === index) ?? null
    }));
  });

  // Prevent recipes from being dropped back into the panel
  readonly noReturn = () => false;

  ngOnInit(): void {
    this.tagService.getAll().subscribe(tags => this.allTags.set(tags));
    this.recipeService.getAll().subscribe(recipes => this.allRecipes.set(recipes));
    this.loadPlans();
  }

  asFormGroup(ctrl: AbstractControl): FormGroup {
    return ctrl as FormGroup;
  }

  formatLastCooked(date: string | null): string {
    if (!date) return 'Never cooked';
    const d = new Date(date);
    const diffDays = Math.floor((Date.now() - d.getTime()) / 86400000);
    if (diffDays === 0) return 'Today';
    if (diffDays === 1) return 'Yesterday';
    if (diffDays < 7) return `${diffDays}d ago`;
    if (diffDays < 30) return `${Math.floor(diffDays / 7)}w ago`;
    return d.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
  }

  toggleEdit(): void {
    if (!this.editMode()) {
      const plan = this.currentPlan();
      if (!plan) return;
      this.editItems.set([...plan.items]);
      this.editMode.set(true);
    } else {
      this.editMode.set(false);
    }
  }

  onRecipeDrop(event: CdkDragDrop<RecipeDto[]>, dayOfWeek: number): void {
    if (event.previousContainer === event.container) return;
    const recipe: RecipeDto = event.item.data;
    this.editItems.update(items => [
      ...items.filter(i => i.dayOfWeek !== dayOfWeek),
      { id: 0, recipeId: recipe.id, recipeName: recipe.name, dayOfWeek }
    ]);
  }

  generate(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.generating.set(true);
    this.editMode.set(false);
    const raw = this.form.getRawValue();
    this.mealPlanService.preview({
      userId: environment.defaultUserId,
      weekStartDate: (raw.weekStartDate as Date).toISOString(),
      tagQuotas: raw.tagQuotas
    }).subscribe({
      next: (draft) => {
        this.draftPlan.set(draft);
        this.generating.set(false);
      },
      error: () => {
        this.generating.set(false);
        this.snackBar.open('Failed to generate plan', 'Close', { duration: 3000 });
      }
    });
  }

  savePlan(): void {
    const plan = this.currentPlan();
    if (!plan) return;
    this.saving.set(true);
    const items = this.editMode() ? this.editItems() : plan.items;
    const payload = {
      userId: environment.defaultUserId,
      weekStartDate: plan.weekStartDate,
      items: items.map(i => ({ recipeId: i.recipeId, dayOfWeek: i.dayOfWeek }))
    };

    const save$ = plan.id === 0
      ? this.mealPlanService.create(payload)
      : this.mealPlanService.update(plan.id, payload);

    save$.subscribe({
      next: () => {
        this.draftPlan.set(null);
        this.editMode.set(false);
        this.saving.set(false);
        this.snackBar.open('Plan saved!', 'Close', { duration: 3000 });
        this.loadPlans();
      },
      error: () => {
        this.saving.set(false);
        this.snackBar.open('Failed to save plan', 'Close', { duration: 3000 });
      }
    });
  }

  regenerate(): void {
    const plan = this.currentPlan();
    if (!plan) return;
    this.regenerating.set(true);
    this.editMode.set(false);

    const weekStartDate = plan.weekStartDate;

    const doPreview = () => {
      this.mealPlanService.preview({
        userId: environment.defaultUserId,
        weekStartDate,
        tagQuotas: []
      }).subscribe({
        next: (draft) => { this.draftPlan.set(draft); this.regenerating.set(false); },
        error: () => { this.regenerating.set(false); this.snackBar.open('Failed to regenerate plan', 'Close', { duration: 3000 }); }
      });
    };

    // If an existing saved plan is shown, delete it before re-previewing
    if (plan.id !== 0) {
      this.mealPlanService.delete(plan.id).subscribe({
        next: () => { this.plans.update(list => list.filter(p => p.id !== plan.id)); doPreview(); },
        error: () => { this.regenerating.set(false); this.snackBar.open('Failed to regenerate plan', 'Close', { duration: 3000 }); }
      });
    } else {
      doPreview();
    }
  }

  loadPlans(): void {
    this.mealPlanService.getAll().subscribe({
      next: plans => this.plans.set(plans),
      error: () => this.snackBar.open('Failed to load meal plans', 'Close', { duration: 3000 })
    });
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
