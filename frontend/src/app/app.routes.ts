import { Routes } from '@angular/router';

export const routes: Routes = [
  // ── Public layout ──────────────────────────────────────────────────────
  {
    path: '',
    loadComponent: () =>
      import('./layouts/public-layout/public-layout.component')
        .then(m => m.PublicLayoutComponent),
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadComponent: () =>
          import('./features/landing/landing.component')
            .then(m => m.LandingComponent)
      },
      {
        path: 'about',
        loadComponent: () =>
          import('./features/about/about.component')
            .then(m => m.AboutComponent)
      }
    ]
  },

  // ── App layout ─────────────────────────────────────────────────────────
  {
    path: 'app',
    loadComponent: () =>
      import('./layouts/app-layout/app-layout.component')
        .then(m => m.AppLayoutComponent),
    children: [
      { path: '', redirectTo: 'recipes', pathMatch: 'full' },
      {
        path: 'tags',
        loadComponent: () =>
          import('./features/tags/tag-list.component')
            .then(m => m.TagListComponent)
      },
      {
        path: 'recipes',
        loadComponent: () =>
          import('./features/recipes/recipe-list/recipe-list.component')
            .then(m => m.RecipeListComponent)
      },
      {
        path: 'recipes/new',
        loadComponent: () =>
          import('./features/recipes/recipe-form/recipe-form.component')
            .then(m => m.RecipeFormComponent)
      },
      {
        path: 'recipes/:id/edit',
        loadComponent: () =>
          import('./features/recipes/recipe-form/recipe-form.component')
            .then(m => m.RecipeFormComponent)
      },
      {
        path: 'recipes/:id',
        loadComponent: () =>
          import('./features/recipes/recipe-detail/recipe-detail.component')
            .then(m => m.RecipeDetailComponent)
      },
      {
        path: 'planner',
        loadComponent: () =>
          import('./features/meal-planner/planner-home/planner-home.component')
            .then(m => m.PlannerHomeComponent)
      },
      {
        path: 'planner/generate',
        loadComponent: () =>
          import('./features/meal-planner/generate-plan/generate-plan.component')
            .then(m => m.GeneratePlanComponent)
      }
    ]
  },

  // ── Fallback ───────────────────────────────────────────────────────────
  { path: '**', redirectTo: '' }
];
