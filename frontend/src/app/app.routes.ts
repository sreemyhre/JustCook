import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'tags', pathMatch: 'full' },
  {
    path: 'tags',
    loadComponent: () =>
      import('./features/tags/tag-list.component').then(m => m.TagListComponent)
  },
  {
    path: 'recipes',
    loadComponent: () =>
      import('./features/recipes/recipe-list/recipe-list.component').then(m => m.RecipeListComponent)
  },
  {
    path: 'recipes/new',
    loadComponent: () =>
      import('./features/recipes/recipe-form/recipe-form.component').then(m => m.RecipeFormComponent)
  },
  {
    path: 'recipes/:id/edit',
    loadComponent: () =>
      import('./features/recipes/recipe-form/recipe-form.component').then(m => m.RecipeFormComponent)
  },
  {
    path: 'recipes/:id',
    loadComponent: () =>
      import('./features/recipes/recipe-detail/recipe-detail.component').then(m => m.RecipeDetailComponent)
  },
  {
    path: 'planner',
    loadComponent: () =>
      import('./features/meal-planner/planner-home/planner-home.component').then(m => m.PlannerHomeComponent)
  },
  {
    path: 'planner/generate',
    loadComponent: () =>
      import('./features/meal-planner/generate-plan/generate-plan.component').then(m => m.GeneratePlanComponent)
  },
  { path: '**', redirectTo: 'tags' }
];
