# JustCook — Project Reference

Full-stack meal planning app. **Backend is feature-complete. Frontend Phases 0–3 complete.**

---

## Quick Start

```bash
# Backend (from repo root)
cd backend/RecipeVault.API
dotnet run              # http://localhost:5000  (Swagger: /swagger)

# Frontend (from repo root)
cd frontend
npm install
ng serve               # http://localhost:4200
```

**Database:** PostgreSQL — `justcookdb` on `localhost:5432`, user `postgres`, password `1234`

---

## Architecture

**Clean Architecture** — 4 backend layers + Angular frontend:

```
RecipeVault.Core          Domain entities, enums, repository interfaces
RecipeVault.Application   DTOs, service interfaces, services, AutoMapper profiles
RecipeVault.Infrastructure DbContext (EF Core / PostgreSQL), repositories, migrations
RecipeVault.API           Controllers, Program.cs (DI + CORS + Swagger)
RecipeVault.Tests         xUnit + Moq + InMemory EF — 86 tests passing

frontend/                 Angular 19 standalone, SCSS, Angular Material
```

---

## Backend — Entities & DB Schema

**Database:** `justcookdb` (PostgreSQL via EF Core 8)

| Entity | Key Fields |
|---|---|
| User | Id, Email, PasswordHash, FirstName, CreatedAt |
| Recipe | Id, UserId(FK), Name, Description, ServingSize, PrepTimeMinutes, CookTimeMinutes, Instructions, ImageUrl, LastCookedDate, CookCount, CreatedAt |
| Ingredient | Id, RecipeId(FK), Name, Quantity(decimal), Unit, IsStaple |
| Tag | Id, Name |
| RecipeTag | RecipeId(FK), TagId(FK) — join table |
| MealPlan | Id, UserId(FK), WeekStartDate |
| MealPlanItem | Id, MealPlanId(FK), RecipeId(FK), DayOfWeek(enum), MealType(enum) |
| PantryStaple | Id, UserId(FK), Name |

**Enums:**
- `MealType`: Breakfast=0, Lunch=1, Dinner=2
- `DayOfWeekEnum`: Monday=0 … Sunday=6

**Migrations (3 applied):**
1. `20260213015410_InitialCreate_Users`
2. `20260213233256_AddAllEntities`
3. `20260215220836_AddInstructionsToRecipe`

---

## Backend — DTOs

**Recipe:**
- `CreateRecipeDto` — UserId, Name, Description, ServingSize, PrepTimeMinutes, CookTimeMinutes, Instructions, ImageUrl, `List<CreateIngredientDto> Ingredients`
- `UpdateRecipeDto` — same minus UserId
- `RecipeDto` — all fields + Id, CreatedAt, `List<IngredientDto> Ingredients`

**Ingredient:**
- `CreateIngredientDto` — Name, Quantity, Unit, IsStaple
- `IngredientDto` — same + Id, RecipeId

**Tag:**
- `CreateTagDto` — Name
- `UpdateTagDto` — Name
- `TagDto` — Id, Name

**MealPlan:**
- `CreateMealPlanDto` — UserId, WeekStartDate, `List<CreateMealPlanItemDto> Items`
- `MealPlanDto` — Id, UserId, WeekStartDate, `List<MealPlanItemDto> Items`
- `CreateMealPlanItemDto` — RecipeId, DayOfWeek, MealType
- `MealPlanItemDto` — Id, RecipeId, RecipeName, DayOfWeek, MealType
- `GenerateMealPlanDto` — UserId, WeekStartDate, `List<TagQuotaDto> TagQuotas`
- `TagQuotaDto` — TagId, Count

---

## Backend — API Endpoints

### Recipes `/api/recipes`

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/recipes` | Create recipe (inline ingredients) |
| GET | `/api/recipes?userId={id}` | All recipes for user |
| GET | `/api/recipes/{id}` | Single recipe |
| PUT | `/api/recipes/{id}` | Update recipe |
| DELETE | `/api/recipes/{id}` | Delete recipe |
| PATCH | `/api/recipes/{id}/log-cook` | Log a cook (updates CookCount + LastCookedDate) |
| GET | `/api/recipes/rotation-suggestions?userId={id}&count={n}` | Least-recently-cooked recipes |

### Tags `/api/tags`

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/tags` | Create tag |
| GET | `/api/tags` | All tags |
| GET | `/api/tags/{id}` | Single tag |
| PUT | `/api/tags/{id}` | Update tag |
| DELETE | `/api/tags/{id}` | Delete tag |
| POST | `/api/tags/{id}/recipes/{recipeId}` | Add tag to recipe |
| DELETE | `/api/tags/{id}/recipes/{recipeId}` | Remove tag from recipe |
| GET | `/api/tags/{id}/recipes` | All recipes with this tag |

### Meal Plans `/api/mealplans`

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/mealplans` | Create meal plan manually |
| GET | `/api/mealplans?userId={id}` | All meal plans for user |
| GET | `/api/mealplans/{id}` | Single meal plan |
| PUT | `/api/mealplans/{id}` | Update meal plan |
| DELETE | `/api/mealplans/{id}` | Delete meal plan |
| POST | `/api/mealplans/generate` | Auto-generate 7-day plan with tag quotas |

---

## Backend — NuGet Packages

| Package | Version |
|---|---|
| .NET | 8.0 |
| EF Core | 8.x |
| Npgsql.EntityFrameworkCore.PostgreSQL | 8.x |
| AutoMapper | 12.x |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.x |
| Swashbuckle.AspNetCore | 6.x |
| xUnit | 2.x |
| Moq | 4.x |
| Microsoft.EntityFrameworkCore.InMemory | 8.x |

---

## Frontend — Current State

**Angular 19** — standalone components, SCSS, Angular Material

**Phases 0–3 complete.**

```
frontend/src/app/
├── core/
│   ├── models/
│   │   ├── tag.model.ts          ✅ TagDto, CreateTagDto, UpdateTagDto
│   │   ├── ingredient.model.ts   ✅ IngredientDto, CreateIngredientDto
│   │   ├── recipe.model.ts       ✅ RecipeDto, CreateRecipeDto, UpdateRecipeDto
│   │   ├── enums.ts              ✅ DayOfWeekEnum, MealType
│   │   └── meal-plan.model.ts    ✅ MealPlanDto, MealPlanItemDto, GenerateMealPlanDto, TagQuotaDto
│   └── services/
│       ├── tag.service.ts        ✅ getAll, create, update, delete
│       ├── recipe.service.ts     ✅ getAll, getById, create, update, delete, logCook, getRotationSuggestions
│       └── meal-plan.service.ts  ✅ getAll, getById, generate, delete
├── features/
│   ├── tags/
│   │   └── tag-list/             ✅ Material table, inline edit, snackbar
│   ├── recipes/
│   │   ├── recipe-list/          ✅ responsive grid, tag filter dropdown
│   │   ├── recipe-form/          ✅ ReactiveForm, FormArray ingredients, multi-select tags
│   │   └── recipe-detail/        ✅ full view, Log Cook button
│   └── meal-planner/
│       ├── planner-home/         ✅ 7-day grid, delete plan, empty state
│       └── generate-plan/        ✅ datepicker, tag quota FormArray, generate + navigate
├── shared/
│   ├── components/
│   │   └── recipe-card/          ✅ name, times, servings, last-cooked badge, tag chips, buttons
│   └── nav/                      ✅ navbar
├── app.component.ts     ✅ Root component
├── app.config.ts        ✅ provideHttpClient + provideRouter + provideAnimationsAsync + provideNativeDateAdapter
└── app.routes.ts        ✅ all routes registered (lazy-loaded)
```

**Key config files:**

`src/environments/environment.ts`:
```ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  defaultUserId: 1
};
```

`src/styles.scss` — global CSS variables:
```scss
--primary:    #1B3A3A  (dark teal — navbar, headers)
--accent:     #FF6B35  (orange — CTAs, badges, active states)
--surface:    #FFFFFF  (white — cards)
--bg:         #F5F5F0  (off-white — page background)
--text:       #1F2328  (dark — body text)
--text-light: #FFFFFF  (white — text on dark surfaces)
--border:     #D0D7DE  (light gray — borders)
```

Global utility classes: `.card`, `.btn-accent`, `.btn-outline`, `.badge`, `.badge-accent`, `.badge-grey`, `.tag-chip`, `.page-container`, `.page-title`

---

## Frontend — Angular Packages

| Package | Version |
|---|---|
| Angular Core | 19.2.0 |
| Angular Material | 19.2.19 |
| Angular CDK | 19.2.19 |
| RxJS | 7.8.0 |
| TypeScript | 5.7.2 |
| Zone.js | 0.15.0 |
| Angular CLI | 19.2.1 |

---

## Frontend — Build Plan (Feature-by-Feature)

Build order: **Tags → Recipes → Meal Planner** — each phase is fully end-to-end (model + service + UI) so the backend can be verified at each step.

### Phase 0 — Routing Shell
Update `app.routes.ts` with lazy-loaded routes. Route ordering matters: `recipes/new` must appear before `recipes/:id`.

| Route | Component |
|---|---|
| `/tags` | `features/tags/tag-list.component` |
| `/recipes` | `features/recipes/recipe-list/` |
| `/recipes/new` | `features/recipes/recipe-form/` |
| `/recipes/:id/edit` | `features/recipes/recipe-form/` |
| `/recipes/:id` | `features/recipes/recipe-detail/` |
| `/planner` | `features/meal-planner/planner-home/` |
| `/planner/generate` | `features/meal-planner/generate-plan/` |
| `**` | redirect → `/tags` |

Also add `provideNativeDateAdapter()` to `app.config.ts` (required for MatDatepicker in Phase 3).

### Phase 1 — Tags Feature
**Files:**
- `core/models/tag.model.ts` — `TagDto`, `CreateTagDto`, `UpdateTagDto`
- `core/services/tag.service.ts` — `getAll()`, `create()`, `update(id)`, `delete(id)`
- `features/tags/tag-list.component.ts/html/scss`

**UI:** Material table with Name + Actions columns. Inline edit on click (input replaces text, save/cancel icons). "+ New Tag" input row at bottom of table. MatSnackBar for feedback.

**Verify:** `http://localhost:4200/tags` — tags load from DB, CRUD works.

### Phase 2 — Recipes Feature
**Files:**
- `core/models/ingredient.model.ts` — `IngredientDto`, `CreateIngredientDto`
- `core/models/recipe.model.ts` — `RecipeDto`, `CreateRecipeDto`, `UpdateRecipeDto`
- `core/services/recipe.service.ts` — `getAll(userId)`, `getById`, `create`, `update`, `delete`, `logCook`, `getRotationSuggestions`
- `shared/components/recipe-card/` — reusable card (name, times, servings, last-cooked badge, tag chips, edit/delete/log-cook buttons)
- `features/recipes/recipe-list/` — responsive grid + tag filter dropdown
- `features/recipes/recipe-form/` — ReactiveForm with FormArray for ingredients, multi-select tags
- `features/recipes/recipe-detail/` — full view + Log Cook button

**Key note:** Check first GET `/api/recipes` response to confirm if backend returns `tags: [{id,name}]` or `tagIds: [1,2]` — adjust `RecipeDto` accordingly.

**Verify:** Recipe grid at `/recipes`, create/edit with ingredients + tags, log cook updates badge.

### Phase 3 — Meal Planner Feature

**Files to create (new unless marked modify):**
- `core/models/enums.ts` — `DayOfWeekEnum` (Mon=0…Sun=6), `MealType` (Breakfast=0, Lunch=1, Dinner=2)
- `core/models/meal-plan.model.ts` — `MealPlanDto`, `MealPlanItemDto`, `CreateMealPlanDto`, `GenerateMealPlanDto`, `TagQuotaDto`
- `core/services/meal-plan.service.ts` — `getAll(userId)`, `getById`, `generate(dto)`, `delete(id)`
- `features/meal-planner/planner-home/planner-home.component.ts` — MODIFY stub
- `features/meal-planner/planner-home/planner-home.component.html` — NEW
- `features/meal-planner/planner-home/planner-home.component.scss` — NEW
- `features/meal-planner/generate-plan/generate-plan.component.ts` — MODIFY stub
- `features/meal-planner/generate-plan/generate-plan.component.html` — NEW
- `features/meal-planner/generate-plan/generate-plan.component.scss` — NEW

**`planner-home` — weekly plan view:**
- On init: call `getAll(userId)` → display `plans[0]` (most recent plan, backend sorts by descending `WeekStartDate`)
- 7-day grid (one card per day Mon–Sun): show recipe name from `items` where `item.dayOfWeek === index`
- Empty day slots show "—" placeholder
- Empty state (no plan yet): centred message + "Generate Plan" `[routerLink]="['/planner/generate']"` button
- "Delete Plan" button (bottom of page) when a plan exists → calls `delete(plan.id)` → clears state
- Use `signal<MealPlanDto | null>` for the loaded plan and `signal(false)` for loading/deleting state

**`generate-plan` — plan generation form:**
- `ReactiveForm` with:
  - `weekStartDate: FormControl` — `MatDatepicker`, defaults to next Monday (compute on init)
  - `tagQuotas: FormArray` — each row is a `FormGroup { tagId: FormControl, count: FormControl }`
- Tags loaded via `TagService.getAll()` for the MatSelect options
- "+ Add Tag Quota" button appends a new row; "✕" removes the row
- Submit → POST `GenerateMealPlanDto` to `/api/mealplans/generate` → navigate to `/planner`
- `GenerateMealPlanDto` shape: `{ userId, weekStartDate: string (ISO), tagQuotas: [{tagId, count}] }`

**Key technical notes:**
- `DayOfWeekEnum`: Monday=0 … Sunday=6 — index matches the 7 days array `['Monday','Tuesday','Wednesday','Thursday','Friday','Saturday','Sunday']`
- Backend `generate` always assigns `MealType.Dinner` — frontend does not need to send meal type
- `planner-home` uses `plans[0]` not date-matching — avoids mismatches when the generated plan is for a future week
- Next Monday calculation (for `weekStartDate` default in generate-plan):
  ```ts
  const today = new Date();
  const day = today.getDay(); // 0=Sun
  const offset = day === 0 ? 1 : 8 - day;
  const monday = new Date(today);
  monday.setDate(today.getDate() + offset);
  monday.setHours(0, 0, 0, 0);
  ```
- `generate-plan` needs `MatDatepickerModule` in its `imports[]` (adapter provided globally via `provideNativeDateAdapter`)
- Send `weekStartDate` as ISO string: `date.toISOString()`

**Verify:** `/planner` shows empty state → click Generate → fill form → submit → redirected back to `/planner` showing 7-day grid with recipe names.

### Files Created Per Phase

| Phase | Files | Status |
|---|---|---|
| 0 | `app.routes.ts` (replace), `app.config.ts` (add provider) | ✅ done |
| 1 | tag.model, tag.service, tag-list (×3) | ✅ done |
| 2 | ingredient.model, recipe.model, recipe.service, recipe-card (×3), recipe-list (×3), recipe-form (×3), recipe-detail (×3) | ✅ done |
| 3 | enums, meal-plan.model, meal-plan.service, planner-home (×3), generate-plan (×3) | ✅ done |

### Angular Technical Constraints
- Standalone components — every component declares its own `imports: []`
- Angular signals (`signal()`, `computed()`) for state — no BehaviorSubject
- Angular 17+ control flow: `@if`, `@for` — not `*ngIf`/`*ngFor`
- `::ng-deep .mat-mdc-form-field-subscript-wrapper` needed in table cells to suppress extra spacing
- `tagIds: number[]` is what the backend `CreateRecipeDto` expects on POST/PUT

---

## Git Branches

| Branch | Purpose |
|---|---|
| main | Production-ready, all merged |
| Angular-SetUp | CORS + Angular scaffold (merged) |
| Recipe-API-Endpoints | Recipe CRUD + log-cook (merged) |
| Ingredient-API-Endpoints | Inline ingredients (merged) |
| tag-API | Tag CRUD + recipe association (merged) |
| MealPlan-API | Meal plan generation (merged) |
| EF-Core-Foundation | Initial EF Core setup (merged) |
| APIfolderstucture | Clean Architecture folder setup (merged) |
| Implementing-Recipe-Feature | Phase 1+2 frontend — Tags + Recipes (merged) |
| MealPrep-Feature | Phase 3 frontend — Meal Planner (active) |

**Working branch convention:** `kebab-case` feature branches, PR into main.

---

## Notes

- `defaultUserId: 1` hardcoded in environment — no auth yet
- Backend on port 5000, CORS allows `localhost:4200`
- 86 backend tests pass
- Angular Material 19 installed — use `MatTableModule`, `MatButtonModule`, `MatIconModule`, `MatInputModule`, `MatFormFieldModule`, `MatSelectModule`, `MatSnackBarModule`, `MatDatepickerModule`, `MatProgressSpinnerModule`, `MatChipsModule` as needed per component
