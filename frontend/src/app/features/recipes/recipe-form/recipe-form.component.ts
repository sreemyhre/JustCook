import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import {
  ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators, AbstractControl
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBarModule } from '@angular/material/snack-bar';

import { RecipeService } from '../../../core/services/recipe.service';
import { TagService } from '../../../core/services/tag.service';
import { ToastService } from '../../../core/services/toast.service';
import { AuthService } from '../../../core/services/auth.service';
import { TagDto } from '../../../core/models/tag.model';
import { UpdateRecipeDto } from '../../../core/models/recipe.model';

@Component({
  selector: 'app-recipe-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatCheckboxModule,
    MatSnackBarModule
  ],
  templateUrl: './recipe-form.component.html',
  styleUrl: './recipe-form.component.scss'
})
export class RecipeFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private recipeService = inject(RecipeService);
  private tagService = inject(TagService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toast = inject(ToastService);
  private authService = inject(AuthService);
  private destroyRef = inject(DestroyRef);

  private get userId(): number { return this.authService.currentUser()?.id ?? 0; }

  isEditMode = signal(false);
  recipeId = signal<number | null>(null);
  allTags = signal<TagDto[]>([]);
  saving = signal(false);

  form: FormGroup = this.fb.group({
    name: ['', Validators.required],
    description: [''],
    servingSize: [4, [Validators.required, Validators.min(1)]],
    prepTimeMinutes: [0, [Validators.required, Validators.min(0)]],
    cookTimeMinutes: [0, [Validators.required, Validators.min(0)]],
    instructions: [''],
    imageUrl: [''],
    tagIds: [[] as number[]],
    ingredients: this.fb.array([])
  });

  get ingredients(): FormArray {
    return this.form.get('ingredients') as FormArray;
  }

  ngOnInit(): void {
    this.tagService.getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(t => this.allTags.set(t));

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.recipeId.set(+id);
      this.recipeService.getById(+id)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: recipe => {
            this.form.patchValue({
              name: recipe.name,
              description: recipe.description,
              servingSize: recipe.servingSize,
              prepTimeMinutes: recipe.prepTimeMinutes,
              cookTimeMinutes: recipe.cookTimeMinutes,
              instructions: recipe.instructions,
              imageUrl: recipe.imageUrl,
              tagIds: recipe.tags?.map(t => t.id) ?? []
            });
            recipe.ingredients.forEach(ing =>
              this.ingredients.push(this.createIngredientGroup(ing.name, ing.quantity, ing.unit, ing.isStaple))
            );
          },
          error: () => this.toast.error('Failed to load recipe')
        });
    } else {
      this.addIngredient();
    }
  }

  createIngredientGroup(name = '', quantity = 0, unit = '', isStaple = false): FormGroup {
    return this.fb.group({
      name: [name, Validators.required],
      quantity: [quantity, [Validators.required, Validators.min(0)]],
      unit: [unit],
      isStaple: [isStaple]
    });
  }

  addIngredient(): void {
    this.ingredients.push(this.createIngredientGroup());
  }

  removeIngredient(index: number): void {
    this.ingredients.removeAt(index);
  }

  asFormGroup(ctrl: AbstractControl): FormGroup {
    return ctrl as FormGroup;
  }

  save(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.saving.set(true);
    const payload = this.buildPayload();

    const request$ = this.isEditMode()
      ? this.recipeService.update(this.recipeId()!, payload)
      : this.recipeService.create({ userId: this.userId, ...payload });

    request$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.saving.set(false);
        this.router.navigate(['/app/recipes']);
      },
      error: () => {
        this.saving.set(false);
        this.toast.error('Failed to save recipe');
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/app/recipes']);
  }

  private buildPayload(): UpdateRecipeDto {
    const raw = this.form.getRawValue();
    return {
      name: raw.name,
      description: raw.description,
      servingSize: raw.servingSize,
      prepTimeMinutes: raw.prepTimeMinutes,
      cookTimeMinutes: raw.cookTimeMinutes,
      instructions: raw.instructions,
      imageUrl: raw.imageUrl,
      tagIds: raw.tagIds,
      ingredients: raw.ingredients
    };
  }
}
